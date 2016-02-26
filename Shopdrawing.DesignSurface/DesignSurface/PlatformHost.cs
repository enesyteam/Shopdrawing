// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.PlatformHost
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.Platform.WPF;

namespace Microsoft.Expression.DesignSurface
{
  internal sealed class PlatformHost : IPlatformHost
  {
    private IPlatform platform;
    private IProjectContext defaultProjectContext;
    private IProjectContext systemThemeContext;

    public void SetPlatform(IPlatform platform)
    {
      this.platform = platform;
    }

    public IDocumentContext CreateDefaultContext(IDocumentLocator documentLocator)
    {
      if (this.defaultProjectContext == null)
        this.defaultProjectContext = (IProjectContext) new DefaultProjectContext(this.platform);
      return (IDocumentContext) new DocumentContext(this.defaultProjectContext, documentLocator);
    }

    public IDocumentContext CreateSystemThemeContext(IDocumentLocator documentLocator)
    {
      if (this.systemThemeContext == null)
      {
        SystemThemeAssemblies.LoadAssemblies();
        this.systemThemeContext = (IProjectContext) new SystemThemeContext(this.platform, SystemThemeAssemblies.ThemeAssemblyReferences);
      }
      return (IDocumentContext) new DocumentContext(this.systemThemeContext, documentLocator);
    }
  }
}
