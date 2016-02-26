// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.BlendEnterProductKeyCommand
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Licenses;
using System.Windows;

namespace Shopdrawing.App
{
  public sealed class BlendEnterProductKeyCommand : EnterProductKeyCommand
  {
    public BlendEnterProductKeyCommand(IServices services)
      : base(services)
    {
    }

    public override void Execute()
    {
      ILicenseService service1 = this.Services.GetService<ILicenseService>();
      bool flag1 = service1.IsAnySkuEnabled(service1.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)) | service1.IsAnySkuEnabled(service1.SkusFromFeature(ExpressionFeatureMapper.HobbledSketchFlowFeature));
      base.Execute();
      bool flag2 = service1.IsAnySkuEnabled(service1.SkusFromFeature(ExpressionFeatureMapper.SketchFlowFeature)) | service1.IsAnySkuEnabled(service1.SkusFromFeature(ExpressionFeatureMapper.HobbledSketchFlowFeature));
      if (!flag1 && flag2)
      {
        int num = (int) this.Services.GetService<IMessageDisplayService>().ShowMessage(StringTable.SketchFlowEnabled, StringTable.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
      }
      if (flag1 == flag2)
        return;
      SplashService service2 = this.Services.GetService<SplashService>();
      if (service2 == null)
        return;
      service2.UpdateSplashVersion(service1);
    }
  }
}
