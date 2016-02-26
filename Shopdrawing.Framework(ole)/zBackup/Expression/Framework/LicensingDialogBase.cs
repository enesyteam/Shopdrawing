// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LicensingDialogBase
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.ActivationDialog.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Licenses;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework
{
  public abstract class LicensingDialogBase : Dialog, ILicensingDialogQuery
  {
    [CLSCompliant(false)]
    protected static readonly UIntPtr IDI_SHIELD = new UIntPtr(32518U);
    private bool isStudioPermanentLicense;
    private bool isPermanentLicense;

    public abstract ProductKeyEditControl KeyValueEditorInternal { get; }

    protected IServices Services { get; private set; }

    protected IExpressionInformationService ExpressionInformationService
    {
      get
      {
        return this.Services.GetService<IExpressionInformationService>();
      }
    }

    public bool IsLicensed { get; protected set; }

    public bool IsStudioPermanentLicense
    {
      get
      {
        return this.isStudioPermanentLicense;
      }
      set
      {
        this.isStudioPermanentLicense = value;
      }
    }

    public bool IsPermanentLicense
    {
      get
      {
        if (!this.isPermanentLicense)
          return this.isStudioPermanentLicense;
        return true;
      }
      set
      {
        this.isPermanentLicense = value;
      }
    }

    public override bool IsOverridingWindowsChrome
    {
      get
      {
        return false;
      }
    }

    protected virtual bool ShouldActivate
    {
      get
      {
        return false;
      }
    }

    [CLSCompliant(false)]
    public uint GraceDaysRemaining { get; set; }

    public bool IsReleaseVersion
    {
      get
      {
        return this.Services.GetService<IExpressionInformationService>().IsReleaseVersion;
      }
    }

    protected LicensingDialogBase(IServices services)
    {
      this.GraceDaysRemaining = 0U;
      this.IsLicensed = false;
      this.Services = services;
    }

    public abstract void InitializeDialog();

    [CLSCompliant(false)]
    protected IEnterKeyResponse InsertKeyIntoLicenseStore(string productKey)
    {
      ILicenseService service = this.Services.GetService<ILicenseService>();
      bool flag = UIThreadDispatcher.Instance.Invoke<bool>(DispatcherPriority.Normal, (Func<bool>) (() => this.KeyValueEditorInternal.IsWellFormed));
      bool shouldActivate = UIThreadDispatcher.Instance.Invoke<bool>(DispatcherPriority.Normal, (Func<bool>) (() => this.ShouldActivate));
      if (flag)
        return service.EnterProductKey(productKey, shouldActivate);
      return (IEnterKeyResponse) new EnterKeyResponse();
    }

    [CLSCompliant(false)]
    protected void ProcessLicenseServiceResponse(IEnterKeyResponse response)
    {
      ILicenseService service1 = this.Services.GetService<ILicenseService>();
      IMessageDisplayService service2 = this.Services.GetService<IMessageDisplayService>();
      this.IsLicensed = response.IsEnabled;
      if ((int) response.ErrorCode != -1073418160)
        LicensingDialogHelper.ShowErrorMessageFromResponse((ILicenseSubServiceResponse) response, service2);
      if (!response.IsEnabled)
      {
        if (service1.HasKey(response.KeySku) && service1.GetUnlicensedReason(response.KeySku) == UnlicensedReason.GraceTimeExpired)
        {
          MessageBoxArgs args = new MessageBoxArgs()
          {
            Owner = (Window) this,
            Message = StringTable.LicensingYouNeedToActivate,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Exclamation
          };
          int num = (int) service2.ShowMessage(args);
        }
        else
        {
          MessageBoxArgs args = new MessageBoxArgs()
          {
            Owner = (Window) this,
            Message = StringTable.LicensingInvalidKeyMessage,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Hand
          };
          int num = (int) service2.ShowMessage(args);
        }
      }
      else if (this.ShouldActivate && service1.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(response.KeySku) && !response.IsActivated)
      {
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Owner = (Window) this,
          Message = StringTable.LicensingEnterKeySucceededAndActivationFailed,
          Image = MessageBoxImage.Exclamation
        };
        int num = (int) service2.ShowMessage(args);
        LicensingDialogHelper.ShowActivationDialog(this.Services, (CommonDialogCreator) new ActivationDialogCreator(this.Services, ActivationWizardAction.ChooseActivationType));
      }
      else
      {
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Owner = (Window) this,
          Message = StringTable.LicensingValidMessage
        };
        int num = (int) service2.ShowMessage(args);
      }
    }

    internal static void NavigateToLink(object sender, IServices services)
    {
      Hyperlink hyperlink = sender as Hyperlink;
      if (hyperlink == null)
        return;
      WebPageLauncher.Navigate(hyperlink.NavigateUri, services.GetService<IMessageDisplayService>());
    }
  }
}
