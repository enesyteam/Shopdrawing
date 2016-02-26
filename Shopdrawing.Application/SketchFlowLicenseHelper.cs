// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.SketchFlowLicenseHelper
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.Licensing;

namespace Shopdrawing.App
{
  public static class SketchFlowLicenseHelper
  {
    public static void ResetCache()
    {
      LicensingHelper.ResetCache();
    }

    public static bool IsLicensed(ILicenseService licenseService)
    {
      return LicensingHelper.IsSketchFlowLicensed(licenseService);
    }
  }
}
