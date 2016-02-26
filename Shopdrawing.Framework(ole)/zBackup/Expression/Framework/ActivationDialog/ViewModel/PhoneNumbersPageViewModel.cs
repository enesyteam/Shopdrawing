// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.PhoneNumbersPageViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class PhoneNumbersPageViewModel : ActivationWizardPageViewModelBase
  {
    private ReadOnlyCollection<PhoneNumberInformation> phoneNumbers;

    public override ActivationWizardAction PreviousAction
    {
      get
      {
        return ActivationWizardAction.ChooseActivationType;
      }
    }

    public override ActivationWizardAction NextAction
    {
      get
      {
        return ActivationWizardAction.InstallationIds;
      }
    }

    public ReadOnlyCollection<PhoneNumberInformation> PhoneNumbers
    {
      get
      {
        if (this.phoneNumbers == null)
          this.RetrievePhoneNumbers();
        return this.phoneNumbers;
      }
    }

    public PhoneNumbersPageViewModel(ActivationInfo activationInfo)
      : base(activationInfo)
    {
      this.ActivationInfo.PropertyChanged += (PropertyChangedEventHandler) ((sender, e) =>
      {
        if (!(e.PropertyName == "PhoneNumbers"))
          return;
        this.RetrievePhoneNumbers();
      });
    }

    private void RetrievePhoneNumbers()
    {
      this.phoneNumbers = this.ActivationInfo.PhoneNumbers;
      this.OnPropertyChanged("PhoneNumbers");
    }
  }
}
