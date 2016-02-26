// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DesignerDefaultPlatformService
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface
{
  public sealed class DesignerDefaultPlatformService : IDesignerDefaultPlatformService, IDisposable
  {
    private IServiceProvider serviceProvider;
    private IPlatform defaultPlatform;

    public IPlatform DefaultPlatform
    {
      get
      {
        if (this.defaultPlatform == null)
        {
          IPlatformService platformService = (IPlatformService) this.serviceProvider.GetService(typeof (IPlatformService));
          this.defaultPlatform = platformService.GetPlatformCreator(".NETFramework").CreatePlatform((IPlatformRuntimeContext) new PlatformRuntimeContext(".NETFramework", this.serviceProvider), (IPlatformReferenceContext) null, platformService, (IPlatformHost) new PlatformHost());
        }
        return this.defaultPlatform;
      }
    }

    public DesignerDefaultPlatformService(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
      if (this.defaultPlatform == null)
        return;
      ((IPlatformService) this.serviceProvider.GetService(typeof (IPlatformService))).GetPlatformCreator(".NETFramework").ReleasePlatform(this.defaultPlatform);
      this.defaultPlatform = (IPlatform) null;
    }
  }
}
