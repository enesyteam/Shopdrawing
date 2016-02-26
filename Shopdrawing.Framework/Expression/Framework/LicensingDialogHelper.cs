// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LicensingDialogHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Licenses;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.Framework
{
  public static class LicensingDialogHelper
  {
    public static LicenseDialogPopup DialogToDisplay(IServices services)
    {
      return LicensingDialogHelper.DialogToDisplay(services, 30);
    }

    public static LicenseDialogPopup DialogToDisplay(IServices services, int nagDays)
    {
      ILicenseService service = services.GetService<ILicenseService>();
      Guid mostPermissiveSkuId = service.MostPermissiveSkuId;
      if (LicensingDialogHelper.ShouldNag(service, mostPermissiveSkuId, nagDays))
        return LicenseDialogPopup.LicenseDialog;
      if (service.IsLicensed(mostPermissiveSkuId) || service.IsInGrace(mostPermissiveSkuId))
        return LicenseDialogPopup.NoDialog;
      if (!service.IsLicensed(mostPermissiveSkuId) && service.HasKey(mostPermissiveSkuId) && service.GetUnlicensedReason(mostPermissiveSkuId) == UnlicensedReason.GraceTimeExpired)
        return LicenseDialogPopup.ActivationGraceExpiredDialog;
      if (service.RightNotGranted)
        return LicenseDialogPopup.TrialExpiredDialog;
      return service.ProductLicenseNotInstalled ? LicenseDialogPopup.ProductSkuNotInstalled : LicenseDialogPopup.LicensingError;
    }

    private static bool ShouldNag(ILicenseService licenseService, Guid skuId, int nagDays)
    {
      if (licenseService.IsInGrace(skuId) && licenseService.FeaturesFromSku(skuId).Contains(ExpressionFeatureMapper.TrialLicense))
        return licenseService.GetRemainingGraceMinutes(skuId) / 1440 < nagDays;
      return false;
    }

    public static bool EnsureProductIsLicensed(IServices services)
    {
      return LicensingDialogHelper.EnsureProductIsLicensed(services, 30);
    }

    public static bool EnsureProductIsLicensed(IServices services, int nagAtDays)
    {
      services.GetService<ILicenseService>().WaitForValidLicenseInformation();
      IMessageDisplayService service = services.GetService<IMessageDisplayService>();
      bool flag;
      switch (LicensingDialogHelper.DialogToDisplay(services, nagAtDays))
      {
        case LicenseDialogPopup.NoDialog:
          flag = true;
          break;
        case LicenseDialogPopup.LicenseDialog:
          flag = LicensingDialogHelper.ShowLicensingDialog(services, (CommonDialogCreator) new LicensingDialogCreator(services));
          break;
        case LicenseDialogPopup.TrialExpiredDialog:
          flag = LicensingDialogHelper.ShowTrialExpiredDialog(services, (CommonDialogCreator) new TrialExpiredDialogCreator(services));
          break;
        case LicenseDialogPopup.ActivationGraceExpiredDialog:
          flag = LicensingDialogHelper.ShowActivationGraceExpiredDialog(services, (CommonDialogCreator) new ActivationGraceExpiredDialogCreator(services));
          break;
        case LicenseDialogPopup.ProductSkuNotInstalled:
          MessageBoxArgs args1 = new MessageBoxArgs()
          {
            Message = StringTable.LicensingProductSkuLicenseNotInstalledMessage,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Hand
          };
          if (service != null)
          {
            int num1 = (int) service.ShowMessage(args1);
          }
          flag = false;
          break;
        default:
          MessageBoxArgs args2 = new MessageBoxArgs()
          {
            Message = StringTable.LicensingInitializationFailureMessage,
            Button = MessageBoxButton.OK,
            Image = MessageBoxImage.Hand
          };
          if (service != null)
          {
            int num2 = (int) service.ShowMessage(args2);
          }
          flag = false;
          break;
      }
      return flag;
    }

    [CLSCompliant(false)]
    public static bool ShowActivationDialog(IServices services, CommonDialogCreator activationDialogCreator)
    {
      ILicensingDialogQuery getInstance = activationDialogCreator.GetInstance;
      return LicensingDialogHelper.ShowDialogWithMainWindowDisabled(services, getInstance);
    }

    [CLSCompliant(false)]
    public static bool ShowLicensingDialog(IServices services, CommonDialogCreator licensingDialogCreator)
    {
      bool flag = false;
      ILicenseService service1 = services.GetService<ILicenseService>();
      ILicensingDialogQuery getInstance = licensingDialogCreator.GetInstance;
      Guid mostPermissiveSkuId = service1.MostPermissiveSkuId;
      if (mostPermissiveSkuId != Guid.Empty)
      {
        if (service1.IsInGrace(mostPermissiveSkuId))
        {
          getInstance.IsPermanentLicense = false;
          getInstance.GraceDaysRemaining = (uint) Math.Floor((double) service1.GetRemainingGraceMinutes(mostPermissiveSkuId) / 1440.0);
        }
        else
        {
          getInstance.IsStudioPermanentLicense = service1.FeaturesFromSku(service1.MostPermissiveSkuId).Contains(ExpressionFeatureMapper.StudioLicense);
          getInstance.IsPermanentLicense = !getInstance.IsStudioPermanentLicense && !service1.FeaturesFromSku(service1.MostPermissiveSkuId).Contains(ExpressionFeatureMapper.TrialLicense);
        }
        if (LicensingDialogHelper.ShowDialogWithMainWindowDisabled(services, getInstance) && getInstance.IsLicensed)
          flag = true;
      }
      else
      {
        MessageBoxArgs args = new MessageBoxArgs()
        {
          Message = StringTable.LicensingTrialKeyInstallationFailureMessage,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Hand
        };
        IMessageDisplayService service2 = services.GetService<IMessageDisplayService>();
        if (service2 != null)
        {
          int num = (int) service2.ShowMessage(args);
        }
      }
      return flag;
    }

    [CLSCompliant(false)]
    public static bool ShowTrialExpiredDialog(IServices services, CommonDialogCreator trialExpiredDialogCreator)
    {
      bool flag = false;
      ILicensingDialogQuery getInstance = trialExpiredDialogCreator.GetInstance;
      if (LicensingDialogHelper.ShowDialogWithMainWindowDisabled(services, getInstance) && getInstance.IsLicensed)
        flag = true;
      return flag;
    }

    [CLSCompliant(false)]
    public static bool ShowActivationGraceExpiredDialog(IServices services, CommonDialogCreator activationGraceExpiredDialogCreator)
    {
      ILicensingDialogQuery getInstance = activationGraceExpiredDialogCreator.GetInstance;
      if (!LicensingDialogHelper.ShowDialogWithMainWindowDisabled(services, getInstance))
        return false;
      ILicenseService service = services.GetService<ILicenseService>();
      Guid mostPermissiveSkuId = service.MostPermissiveSkuId;
      return service.IsLicensed(mostPermissiveSkuId) || service.IsInGrace(mostPermissiveSkuId);
    }

    private static bool ShowDialogWithMainWindowDisabled(IServices services, ILicensingDialogQuery dialog)
    {
      dialog.InitializeDialog();
      Microsoft.Expression.Framework.UserInterface.IWindowService service = services.GetService<Microsoft.Expression.Framework.UserInterface.IWindowService>();
      service.IsEnabled = false;
      bool? nullable = dialog.ShowDialog();
      service.IsEnabled = true;
      if (nullable.HasValue)
        return nullable.Value;
      return false;
    }

    internal static void ShowErrorMessageFromResponse(ILicenseSubServiceResponse response, IMessageDisplayService messageDisplayService)
    {
      string str = LicensingDialogHelper.ErrorMessageFromResponse(response);
      if (string.IsNullOrEmpty(str))
        return;
      MessageBoxArgs args = new MessageBoxArgs()
      {
        Message = str,
        Button = MessageBoxButton.OK,
        Image = MessageBoxImage.Hand
      };
      int num = (int) messageDisplayService.ShowMessage(args);
    }

    private static string ErrorMessageFromResponse(ILicenseSubServiceResponse response)
    {
      if (response.Exception != null)
        return response.Exception.Message;
      if ((int) response.ErrorCode != 0)
        return LicensingDialogHelper.ErrorMessageFromHResult(response.ErrorCode);
      return string.Empty;
    }

    private static string ErrorMessageFromHResult(uint processResult)
    {
      uint num = processResult;
      if (num <= 2147954429U)
      {
        if ((int) num == -2147012894)
          return StringTable.ConnectionAttemptToRemoteServerTimedOut;
        if ((int) num == -2147012889)
          return StringTable.UnableToFindRemoteServer;
        if ((int) num == -2147012867)
          return StringTable.CannotConnectToRemoteServer;
      }
      else if (num <= 3221549077U)
      {
        if ((int) num == -1073430520)
          return StringTable.MaximumUnlockExceeded;
        if ((int) num == -1073418219)
          return StringTable.ProductSkuNotInstalled;
      }
      else
      {
        if ((int) num == -1073418203)
          return StringTable.LicensingInsufficientRightsMessage;
        if ((int) num == -1073418160)
          return StringTable.LicensingInvalidKeyMessage;
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1:X})", new object[2]
      {
        (object) StringTable.LicensingValidationFailureMessage,
        (object) processResult
      });
    }
  }
}
