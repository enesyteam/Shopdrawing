// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.PlatformRuntimeContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Project;
using System;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class PlatformRuntimeContext : IPlatformRuntimeContext
  {
    private IServiceProvider serviceProvider;
    private string frameworkSpec;
    private IAssemblyResolver assemblyResolver;

    public PlatformRuntimeContext(string frameworkSpec, IServiceProvider serviceProvider)
    {
      this.frameworkSpec = frameworkSpec;
      this.serviceProvider = serviceProvider;
    }

    public Assembly ResolveRuntimeAssembly(AssemblyName assemblyName)
    {
      if (this.assemblyResolver == null)
      {
        IAssemblyService assemblyService = (IAssemblyService) this.serviceProvider.GetService(typeof (IAssemblyService));
        if (assemblyService != null)
          this.assemblyResolver = assemblyService.GetPlatformResolver(this.frameworkSpec);
      }
      if (this.assemblyResolver != null)
        return this.assemblyResolver.ResolveAssembly(assemblyName);
      return (Assembly) null;
    }
  }
}
