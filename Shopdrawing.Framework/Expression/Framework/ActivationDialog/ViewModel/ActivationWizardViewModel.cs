// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ActivationDialog.ViewModel.ActivationWizardViewModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.ActivationDialog.DataAccess;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.ActivationDialog.ViewModel
{
  internal class ActivationWizardViewModel : NotifyingObject
  {
    private InternetActivationPageViewModel internetActivationPageViewModel;
    private ChooseActivationTypePageViewModel chooseActivationTypePageViewModel;
    private PhoneNumbersPageViewModel phoneNumbersPageViewModel;
    private InstallationIdPageViewModel installationIdPageViewModel;
    private ValidationIdEntryPageViewModel validationIdEntryPageViewModel;
    private ActivationDataRepository dataRepository;
    private IServices services;
    private ActivationWizardPageViewModelBase currentPage;
    private ICommand performNextActionCommand;
    private ICommand performPreviousActionCommand;

    public ActivationInfo ActivationInfo { get; private set; }

    public ActivationWizardPageViewModelBase CurrentPage
    {
      get
      {
        return this.currentPage;
      }
      private set
      {
        this.UpdatePage(value);
      }
    }

    public bool PreviousActionAvailable
    {
      get
      {
        return this.CurrentPage.PreviousAction != ActivationWizardAction.None;
      }
    }

    public bool NavigationEnabled
    {
      get
      {
        return this.CurrentPage.SupportsNavigation;
      }
    }

    public bool NextActionEnabled
    {
      get
      {
        if (this.NavigationEnabled)
          return this.CurrentPage.IsValid;
        return false;
      }
    }

    public ICommand PerformNextActionCommand
    {
      get
      {
        if (this.performNextActionCommand == null)
          this.performNextActionCommand = (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.PerformNextAction()));
        return this.performNextActionCommand;
      }
    }

    public ICommand PerformPreviousActionCommand
    {
      get
      {
        if (this.performPreviousActionCommand == null)
          this.performPreviousActionCommand = (ICommand) new DelegateCommand((DelegateCommand.SimpleEventHandler) (() => this.PerformPreviousAction()));
        return this.performPreviousActionCommand;
      }
    }

    public event EventHandler ActivationComplete;

    public ActivationWizardViewModel(ActivationDataRepository dataRepository, IServices services, ActivationWizardAction startAction)
    {
      this.dataRepository = dataRepository;
      this.ActivationInfo = new ActivationInfo(this.dataRepository);
      this.services = services;
      this.InitializeChildViewModels();
      this.HandleWizardAction(startAction);
    }

    private void CompleteActivation()
    {
      if (this.ActivationComplete == null)
        return;
      this.ActivationComplete((object) this, EventArgs.Empty);
    }

    private void PerformNextAction()
    {
      this.HandleWizardAction(this.CurrentPage.NextAction);
    }

    private void PerformPreviousAction()
    {
      this.HandleWizardAction(this.CurrentPage.PreviousAction);
    }

    private void HandleWizardAction(ActivationWizardAction action)
    {
      switch (action)
      {
        case ActivationWizardAction.ChooseActivationType:
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.chooseActivationTypePageViewModel;
          break;
        case ActivationWizardAction.PhoneNumber:
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.phoneNumbersPageViewModel;
          break;
        case ActivationWizardAction.InstallationIds:
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.installationIdPageViewModel;
          break;
        case ActivationWizardAction.ValidationIds:
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.validationIdEntryPageViewModel;
          break;
        case ActivationWizardAction.AttemptOnlineActivation:
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.internetActivationPageViewModel;
          break;
        case ActivationWizardAction.AttemptPhoneActivation:
          if (this.TryPhoneActivation())
          {
            this.FinishActivation(StringTable.LicensingSuccessfulActivationMessage);
            break;
          }
          int num = (int) this.services.GetService<IMessageDisplayService>().ShowMessage(new MessageBoxArgs()
          {
            Message = StringTable.LicensingPhoneActivationFailedMessage,
            Image = MessageBoxImage.Exclamation
          });
          this.CurrentPage = (ActivationWizardPageViewModelBase) this.validationIdEntryPageViewModel;
          break;
        case ActivationWizardAction.SuccessfulInternetActivation:
          this.FinishActivation(StringTable.LicensingSuccessfulActivationMessage);
          break;
        case ActivationWizardAction.ProductAlreadyActivated:
          this.FinishActivation(StringTable.LicensingAlreadyActivatedMessage);
          break;
        case ActivationWizardAction.NoSkusToActivate:
          this.FinishActivation(StringTable.LicensingNoActivateableKeyMessage);
          break;
        case ActivationWizardAction.MultipleSkusToActivate:
          this.FinishActivation(StringTable.LicensingMultipleActivateableSkus);
          break;
      }
    }

    private void FinishActivation(string message)
    {
      this.CompleteActivation();
      int num = (int) this.services.GetService<IMessageDisplayService>().ShowMessage(new MessageBoxArgs()
      {
        Message = message,
        Image = MessageBoxImage.None
      });
    }

    private bool TryPhoneActivation()
    {
      IActivateResponse activateResponse = this.dataRepository.TryPhoneActivation((IList<PhoneActivationData>) this.ActivationInfo.ValidationIds);
      LicensingDialogHelper.ShowErrorMessageFromResponse((ILicenseSubServiceResponse) activateResponse, this.services.GetService<IMessageDisplayService>());
      return activateResponse.ActivationStatus != ActivationStatus.Failed;
    }

    private void WizardPagePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsValid"))
        return;
      this.OnPropertyChanged("NextActionEnabled");
    }

    private void WizardPageRequestedMoveNext(object sender, EventArgs e)
    {
      this.PerformNextAction();
    }

    private void UpdatePage(ActivationWizardPageViewModelBase newPage)
    {
      if (this.currentPage == newPage)
        return;
      this.currentPage = newPage;
      if (this.currentPage != null)
        this.currentPage.Initialize();
      this.OnPropertyChanged("CurrentPage");
      this.OnPropertyChanged("PreviousActionAvailable");
      this.OnPropertyChanged("NavigationEnabled");
      this.OnPropertyChanged("NextActionEnabled");
    }

    private void InitializeChildViewModels()
    {
      this.internetActivationPageViewModel = new InternetActivationPageViewModel(this.ActivationInfo, this.dataRepository, this.services);
      this.RegisterPageHandlers((ActivationWizardPageViewModelBase) this.internetActivationPageViewModel);
      this.chooseActivationTypePageViewModel = new ChooseActivationTypePageViewModel(this.ActivationInfo, this.services);
      this.RegisterPageHandlers((ActivationWizardPageViewModelBase) this.chooseActivationTypePageViewModel);
      this.phoneNumbersPageViewModel = new PhoneNumbersPageViewModel(this.ActivationInfo);
      this.RegisterPageHandlers((ActivationWizardPageViewModelBase) this.phoneNumbersPageViewModel);
      this.installationIdPageViewModel = new InstallationIdPageViewModel(this.ActivationInfo);
      this.RegisterPageHandlers((ActivationWizardPageViewModelBase) this.installationIdPageViewModel);
      this.validationIdEntryPageViewModel = new ValidationIdEntryPageViewModel(this.ActivationInfo);
      this.RegisterPageHandlers((ActivationWizardPageViewModelBase) this.validationIdEntryPageViewModel);
    }

    private void RegisterPageHandlers(ActivationWizardPageViewModelBase page)
    {
      page.MoveNextRequested += new EventHandler(this.WizardPageRequestedMoveNext);
      page.PropertyChanged += new PropertyChangedEventHandler(this.WizardPagePropertyChanged);
    }
  }
}
