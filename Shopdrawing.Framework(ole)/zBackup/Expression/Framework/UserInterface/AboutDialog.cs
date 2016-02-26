// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.AboutDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Licenses;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.UserInterface
{
  public sealed class AboutDialog : Dialog
  {
    private Version version;
    private IExpressionInformationService expressionInformationService;
    private IServices services;

    public Version Version
    {
      get
      {
        return this.version;
      }
      set
      {
        this.version = value;
        ((TextBlock) LogicalTreeHelper.FindLogicalNode((DependencyObject) this, "Title")).Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.FindResource((object) "ApplicationVersionTitle") as string, new object[1]
        {
          (object) this.version.ToString()
        });
      }
    }

    public bool IsReleaseVersion
    {
      get
      {
        return this.expressionInformationService.IsReleaseVersion;
      }
    }

    public string AboutDialogWarningText
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AboutDialogWarningText, new object[1]
        {
          (object) this.expressionInformationService.ShortApplicationName
        });
      }
    }

    public override bool IsOverridingWindowsChrome
    {
      get
      {
        return false;
      }
    }

    private string RenderPerformanceLevelAsString
    {
      get
      {
        switch (RenderCapability.Tier)
        {
          case 0:
            return StringTable.RenderTierZeroMessage;
          case 65536:
            return StringTable.RenderTierOneMessage;
          case 131072:
            return StringTable.RenderTierTwoMessage;
          default:
            return (string) null;
        }
      }
    }

    public AboutDialog(IServices services)
      : this(services, (string) null)
    {
    }

    public AboutDialog(IServices services, string pid)
    {
      this.services = services;
      this.expressionInformationService = services.GetService<IExpressionInformationService>();
      ILicenseService service = services.GetService<ILicenseService>();
      this.DialogContent = (UIElement) FileTable.GetElement("Resources\\AboutDialog.xaml");
      ResourceDictionary aboutDialogResources = service.AboutDialogResources;
      if (aboutDialogResources != null)
        this.Resources.MergedDictionaries.Add(aboutDialogResources);
      this.Title = this.Resources[(object) "AboutDialogTitle"] as string;
      this.WindowStyle = WindowStyle.None;
      this.AllowsTransparency = true;
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      ((Hyperlink) LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "WebPageLink")).Click += new RoutedEventHandler(this.Hyperlink_Click);
      TextBlock textBlock1 = (TextBlock) LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "ProductKey");
      TextBlock textBlock2 = (TextBlock) LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "LicensingStatus");
      ((TextBlock) LogicalTreeHelper.FindLogicalNode((DependencyObject) this.DialogContent, "PerformanceLevel")).Text = this.RenderPerformanceLevelAsString;
      if (!string.IsNullOrEmpty(pid))
        textBlock1.Text = pid;
      else if (!string.IsNullOrEmpty(service.MostPermissivePid))
        textBlock1.Text = service.MostPermissivePid;
      textBlock2.Text = this.GetLicensingStatusText(service);
      if (string.IsNullOrEmpty(textBlock2.Text))
      {
        int num = (int) this.services.GetService<IMessageDisplayService>().ShowMessage(new MessageBoxArgs()
        {
          Message = StringTable.UnableToAccessLicensingInformation,
          Button = MessageBoxButton.OK,
          Image = MessageBoxImage.Hand
        });
      }
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.DataContext = (object) this;
    }

    private string GetLicensingStatusText(ILicenseService licenseService)
    {
      Guid mostPermissiveSkuId = licenseService.MostPermissiveSkuId;
      if (licenseService.IsLicensed(mostPermissiveSkuId))
      {
        DateTime expiration = licenseService.GetExpiration(mostPermissiveSkuId);
        if (!(expiration > DateTime.UtcNow))
          return this.GetFullyLicensedMessage(licenseService, mostPermissiveSkuId);
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AboutDialogLicenseExpirationMessage, new object[1]
        {
          (object) expiration.ToShortDateString()
        });
      }
      if (licenseService.IsInGrace(mostPermissiveSkuId))
        return this.GetDaysRemainingMessage(licenseService, mostPermissiveSkuId);
      return string.Empty;
    }

    private string GetFullyLicensedMessage(ILicenseService licenseService, Guid skuId)
    {
      if (!licenseService.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(skuId))
        return StringTable.AboutDialogFullyLicensedMessage;
      return StringTable.AboutDialogFullyLicensedAndActivatedMessage;
    }

    private string GetDaysRemainingMessage(ILicenseService licenseService, Guid skuId)
    {
      uint num = (uint) Math.Floor((double) licenseService.GetRemainingGraceMinutes(skuId) / 1440.0);
      bool flag = (int) num == 1;
      if (licenseService.SkusFromFeature(ExpressionFeatureMapper.ActivationLicense).Contains(skuId))
      {
        if (flag)
          return StringTable.AboutDialogRemainingDaysToActivateMessageSingular;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AboutDialogRemainingDaysToActivateMessagePlural, new object[1]
        {
          (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)
        });
      }
      if (flag)
        return StringTable.AboutDialogRemainingTrialDaysMessageSingular;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.AboutDialogRemainingTrialDaysMessagePlural, new object[1]
      {
        (object) num.ToString((IFormatProvider) CultureInfo.CurrentCulture)
      });
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
      WebPageLauncher.Navigate(((Hyperlink) sender).NavigateUri, this.services.GetService<IMessageDisplayService>());
    }
  }
}
