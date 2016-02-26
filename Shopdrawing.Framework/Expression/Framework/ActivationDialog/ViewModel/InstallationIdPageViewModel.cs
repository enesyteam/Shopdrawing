// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.InstallationIdPageViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class InstallationIdPageViewModel : ActivationWizardPageViewModelBase
  {
    private ReadOnlyCollection<PhoneActivationData> installationIds;

    public override ActivationWizardAction PreviousAction
    {
      get
      {
        return ActivationWizardAction.PhoneNumber;
      }
    }

    public override ActivationWizardAction NextAction
    {
      get
      {
        return ActivationWizardAction.ValidationIds;
      }
    }

    public ReadOnlyCollection<PhoneActivationData> InstallationIds
    {
      get
      {
        if (this.installationIds == null)
          this.RetrieveInstallationIds();
        return this.installationIds;
      }
    }

    public InstallationIdPageViewModel(ActivationInfo activationInfo)
      : base(activationInfo)
    {
      this.ActivationInfo.PropertyChanged += (PropertyChangedEventHandler) ((sender, e) =>
      {
        if (!(e.PropertyName == "InstallationIds"))
          return;
        this.RetrieveInstallationIds();
      });
    }

    private void RetrieveInstallationIds()
    {
      this.installationIds = this.ActivationInfo.InstallationIds;
    }
  }
}
