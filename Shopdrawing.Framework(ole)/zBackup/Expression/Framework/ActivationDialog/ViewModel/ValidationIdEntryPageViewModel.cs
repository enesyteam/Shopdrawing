// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.ValidationIdEntryPageViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class ValidationIdEntryPageViewModel : ActivationWizardPageViewModelBase
  {
    private ReadOnlyCollection<PhoneActivationData> validationIds;

    public override ActivationWizardAction PreviousAction
    {
      get
      {
        return ActivationWizardAction.InstallationIds;
      }
    }

    public override ActivationWizardAction NextAction
    {
      get
      {
        return ActivationWizardAction.AttemptPhoneActivation;
      }
    }

    public ReadOnlyCollection<PhoneActivationData> ValidationIds
    {
      get
      {
        if (this.validationIds == null)
          this.RetrieveValidationIdInformation();
        return this.validationIds;
      }
    }

    public override bool IsValid
    {
      get
      {
        return this.AreIdsValid();
      }
    }

    public ValidationIdEntryPageViewModel(ActivationInfo activationInfo)
      : base(activationInfo)
    {
    }

    private bool AreIdsValid()
    {
      foreach (PhoneActivationData phoneActivationData in this.ValidationIds)
      {
        if (!phoneActivationData.IsValid)
          return false;
      }
      return true;
    }

    private void RetrieveValidationIdInformation()
    {
      this.UnhookValidationIdChanged();
      this.validationIds = this.ActivationInfo.ValidationIds;
      this.HookValidationIdChanged();
      this.OnPropertyChanged("ValidationIds");
    }

    private void HookValidationIdChanged()
    {
      if (this.validationIds == null)
        return;
      foreach (NotifyingObject notifyingObject in this.validationIds)
        notifyingObject.PropertyChanged += new PropertyChangedEventHandler(this.ValidationId_PropertyChanged);
    }

    private void UnhookValidationIdChanged()
    {
      if (this.validationIds == null)
        return;
      foreach (NotifyingObject notifyingObject in this.validationIds)
        notifyingObject.PropertyChanged -= new PropertyChangedEventHandler(this.ValidationId_PropertyChanged);
    }

    private void ValidationId_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IdNumber"))
        return;
      this.OnPropertyChanged("IsValid");
    }
  }
}
