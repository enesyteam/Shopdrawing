// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.SplashService
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Licenses;
using Microsoft.Win32;

namespace Shopdrawing.App
{
  internal class SplashService
  {
    private static readonly string splashVersionValueName = "SplashVersion";
    private string splashVersionSubkeyName;

    public SplashService(string applicationId, string registryPath)
    {
      string str = applicationId.Trim('{', '}');
      this.splashVersionSubkeyName = registryPath + str + "\\";
    }

    public int GetSplashVersion()
    {
      return RegistryHelper.RetrieveRegistryValue<int>(Registry.LocalMachine, this.splashVersionSubkeyName, SplashService.splashVersionValueName);
    }

    public void UpdateSplashVersion(ILicenseService licenseService)
    {
      int num = licenseService.IsAnySkuEnabled(licenseService.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)) | licenseService.IsAnySkuEnabled(licenseService.SkusFromFeature(ExpressionFeatureMapper.HobbledSketchFlowFeature)) ? 1 : 0;
      RegistryHelper.SetRegistryValue<int>(Registry.LocalMachine, this.splashVersionSubkeyName, SplashService.splashVersionValueName, num, RegistryValueKind.DWord);
    }
  }
}
