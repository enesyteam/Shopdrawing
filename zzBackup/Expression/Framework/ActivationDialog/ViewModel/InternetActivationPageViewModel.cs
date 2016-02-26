// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.InternetActivationPageViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class InternetActivationPageViewModel : ActivationWizardPageViewModelBase
  {
    private ActivationDataRepository dataRepository;
    private IActivateResponse internetActivationResponse;
    private IServices services;

    public override bool SupportsNavigation
    {
      get
      {
        return false;
      }
    }

    public override ActivationWizardAction PreviousAction
    {
      get
      {
        return ActivationWizardAction.None;
      }
    }

    public override ActivationWizardAction NextAction
    {
      get
      {
        if (this.internetActivationResponse == null)
          return ActivationWizardAction.ChooseActivationType;
        switch (this.internetActivationResponse.ActivationStatus)
        {
          case ActivationStatus.NoActivatableSkus:
            return ActivationWizardAction.NoSkusToActivate;
          case ActivationStatus.Failed:
            this.ShowMessageOnError(this.internetActivationResponse);
            return ActivationWizardAction.ChooseActivationType;
          case ActivationStatus.Activated:
            return ActivationWizardAction.SuccessfulInternetActivation;
          case ActivationStatus.AlreadyActivated:
            return ActivationWizardAction.ProductAlreadyActivated;
          case ActivationStatus.MultipleSkus:
            return ActivationWizardAction.MultipleSkusToActivate;
          default:
            return ActivationWizardAction.ChooseActivationType;
        }
      }
    }

    public InternetActivationPageViewModel(ActivationInfo activationInfo, ActivationDataRepository dataRepository, IServices services)
      : base(activationInfo)
    {
      this.dataRepository = dataRepository;
      this.services = services;
    }

    public override void Initialize()
    {
      base.Initialize();
      this.TryInternetActivation();
    }

    private void TryInternetActivation()
    {
      this.internetActivationResponse = (IActivateResponse) null;
      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.DoWork += new DoWorkEventHandler(this.InternetWorker_DoWork);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.InternetWorker_RunWorkerCompleted);
      backgroundWorker.RunWorkerAsync();
    }

    private void InternetWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      BackgroundWorker backgroundWorker = (BackgroundWorker) sender;
      backgroundWorker.DoWork -= new DoWorkEventHandler(this.InternetWorker_DoWork);
      backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.InternetWorker_RunWorkerCompleted);
      backgroundWorker.Dispose();
      this.RequestMoveNext();
    }

    private void ShowMessageOnError(IActivateResponse activateResponse)
    {
      if (activateResponse.ActivationStatus != ActivationStatus.Failed)
        return;
      LicensingDialogHelper.ShowErrorMessageFromResponse((ILicenseSubServiceResponse) this.internetActivationResponse, this.services.GetService<IMessageDisplayService>());
    }

    private void InternetWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.internetActivationResponse = this.dataRepository.TryInternetActivation();
      Thread.Sleep(1275);
    }
  }
}
