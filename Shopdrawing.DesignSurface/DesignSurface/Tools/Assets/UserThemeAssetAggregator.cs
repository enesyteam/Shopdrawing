// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.UserThemeAssetAggregator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class UserThemeAssetAggregator : AssetAggregator
  {
    private DesignerContext designerContext;
    private UserThemeManager themeManager;
    private IProject workingProject;
    private IViewService viewService;

    public UserThemeAssetAggregator(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.viewService = this.designerContext.ViewService;
      this.viewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.TryChangeWorkingProject();
    }

    protected override void InternalDispose(bool disposing)
    {
      base.InternalDispose(disposing);
      if (!disposing)
        return;
      this.viewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.viewService = (IViewService) null;
      this.designerContext = (DesignerContext) null;
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.TryChangeWorkingProject();
    }

    private void TryChangeWorkingProject()
    {
      IProject project = this.designerContext.ActiveProject;
      if (project == this.workingProject)
        return;
      this.workingProject = project;
      this.UpdateProviders(project);
      EnumerableExtensions.ForEach<UserThemeAssetProvider>(Enumerable.Cast<UserThemeAssetProvider>((IEnumerable) this.AssetProviders), (Action<UserThemeAssetProvider>) (provider => provider.OnProjectChanged(project)));
    }

    private void UpdateProviders(IProject project)
    {
      IXamlProject xamlProject = project as IXamlProject;
      IProjectContext projectContext = xamlProject == null ? (IProjectContext) null : xamlProject.ProjectContext;
      IPlatform platform = projectContext == null ? (IPlatform) null : projectContext.Platform;
      if (platform == null)
        return;
      IEnumerable<AssetProvider> items = (IEnumerable<AssetProvider>) platform.Metadata.GetPlatformCache(DesignSurfacePlatformCaches.UserThemeAssetAggregatorProviderCache);
      this.ClearProviders();
      if (items != null)
      {
        EnumerableExtensions.ForEach<AssetProvider>(items, (Action<AssetProvider>) (provider => this.AddProvider(provider)));
      }
      else
      {
        this.CreateProviders(platform);
        platform.Metadata.SetPlatformCache(DesignSurfacePlatformCaches.UserThemeAssetAggregatorProviderCache, (object) Enumerable.ToArray<AssetProvider>((IEnumerable<AssetProvider>) this.AssetProviders));
      }
    }

    private void CreateProviders(IPlatform platform)
    {
      this.themeManager = new UserThemeManager(platform);
      foreach (IDocumentLocator documentLocator in (IEnumerable<IDocumentLocator>) this.themeManager.Themes)
        this.AddProvider((AssetProvider) new UserThemeAssetProvider(this.designerContext, (ThemeManager) this.themeManager, DocumentReferenceLocator.GetDocumentReference(documentLocator)));
    }
  }
}
