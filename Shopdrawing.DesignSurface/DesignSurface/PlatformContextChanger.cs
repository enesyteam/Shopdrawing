// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.PlatformContextChanger
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface
{
  public sealed class PlatformContextChanger : IPlatformContextChanger, IDisposable
  {
    private bool allowsInitialRestart = true;
    private IViewService viewService;
    private PlatformContextChanger.PlatformChangeState platformChangeState;
    private IProjectContext oldProjectContext;
    private IProjectManager projectManager;

    public bool AllowViewActivation
    {
      get
      {
        return this.platformChangeState == null;
      }
    }

    public event EventHandler HostPlatformRestarted;

    public PlatformContextChanger(IViewService viewService, IProjectManager projectManager)
    {
      this.viewService = viewService;
      this.viewService.ActiveViewChanging += new ViewChangedEventHandler(this.OnViewServiceActiveViewChanging);
      this.projectManager = projectManager;
      this.projectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.OnProjectManagerSolutionClosed);
    }

    public void Dispose()
    {
      if (this.viewService != null)
      {
        this.viewService.ActiveViewChanging -= new ViewChangedEventHandler(this.OnViewServiceActiveViewChanging);
        this.viewService = (IViewService) null;
      }
      if (this.platformChangeState != null)
      {
        this.platformChangeState.Dispose();
        this.platformChangeState = (PlatformContextChanger.PlatformChangeState) null;
      }
      if (this.projectManager == null)
        return;
      this.projectManager.SolutionClosed -= new EventHandler<SolutionEventArgs>(this.OnProjectManagerSolutionClosed);
      this.projectManager = (IProjectManager) null;
    }

    public void UpdateForPlatformChange(SceneView view, SceneView oldView, bool useAsyncShutdown)
    {
      this.UpdateForPlatformChange(view, oldView, useAsyncShutdown, view.Platform.NeedsContextUpdate);
    }

    public void UpdatePlatformOnSolutionChanged(IProjectContext projectContext)
    {
      if (!this.allowsInitialRestart)
        return;
      ProjectXamlContext projectXamlContext = projectContext as ProjectXamlContext;
      IPlatform platform = projectXamlContext != null ? projectXamlContext.Platform : (IPlatform) null;
      if (this.viewService.Views.Count == 0 && platform != null && platform.NeedsContextUpdate)
        platform.ActivatePlatformContext();
      this.allowsInitialRestart = false;
    }

    public void UpdateForPlatformChange(SceneView view, SceneView oldView, bool useAsyncShutdown, bool needsContextUpdate)
    {
      bool flag1 = this.oldProjectContext != view.ProjectContext;
      this.oldProjectContext = view.ProjectContext;
      bool flag2 = !view.DesignerContext.ViewRootResolver.IsInitializingUnopenedView;
      if (needsContextUpdate)
      {
        if (!flag2)
          return;
        IPlatform platform = view.Platform;
        IDisposable postponeToken = this.platformChangeState == null ? view.ViewUpdateManager.PostponeUpdates() : this.platformChangeState.PostponeToken;
        bool flag3 = this.platformChangeState == null;
        this.platformChangeState = new PlatformContextChanger.PlatformChangeState(view.ViewUpdateManager, platform, oldView, view, postponeToken);
        if (useAsyncShutdown)
        {
          if (!flag3)
            return;
          UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, (Action) (() => this.ShutdownAndReactivate()));
        }
        else
          this.ShutdownAndReactivate();
      }
      else
      {
        if (flag1)
          this.RefreshAssemblies(view);
        if (this.platformChangeState == null)
          return;
        this.platformChangeState.Dispose();
        this.platformChangeState = (PlatformContextChanger.PlatformChangeState) null;
      }
    }

    private void RefreshAssemblies(SceneView view)
    {
      if (view == null || view.IsClosing)
        return;
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(view.ProjectContext);
      if (projectXamlContext == null)
        return;
      projectXamlContext.RefreshAssemblies();
    }

    private void ShutdownAndReactivate()
    {
      if (this.platformChangeState == null)
        return;
      PlatformContextChanger.PlatformChangeState platformChangeState = this.platformChangeState;
      this.platformChangeState = (PlatformContextChanger.PlatformChangeState) null;
      try
      {
        if (platformChangeState.NewView.IsClosing)
          return;
        ViewRootResolver viewRootResolver = platformChangeState.NewView.DesignerContext.ViewRootResolver;
        List<SceneView> list1 = new List<SceneView>((IEnumerable<SceneView>) viewRootResolver.ResolvedViews);
        if (platformChangeState.OldView != null && !list1.Contains(platformChangeState.OldView))
          list1.Add(platformChangeState.OldView);
        string identifier = platformChangeState.Platform.Metadata.TargetFramework.Identifier;
        list1.RemoveAll((Predicate<SceneView>) (v =>
        {
          if (!v.IsClosing)
            return !v.Platform.Metadata.TargetFramework.Identifier.Equals(identifier);
          return true;
        }));
        platformChangeState.ViewUpdateManager.InvalidateViews((ICollection<SceneView>) list1, false);
        platformChangeState.Platform.ActivatePlatformContext();
        List<SceneView> list2 = new List<SceneView>((IEnumerable<SceneView>) viewRootResolver.ResolvedViews);
        if (platformChangeState.OldView != null && !list2.Contains(platformChangeState.OldView))
          list2.Add(platformChangeState.OldView);
        platformChangeState.ViewUpdateManager.InvalidateViews((ICollection<SceneView>) list2, false);
        platformChangeState.PostponeToken.Dispose();
        platformChangeState.PostponeToken = (IDisposable) null;
        this.RefreshAssemblies(platformChangeState.NewView);
        platformChangeState.NewView.ShutdownVisualTree();
        platformChangeState.NewView.Activated();
        if (this.HostPlatformRestarted == null)
          return;
        this.HostPlatformRestarted((object) this, EventArgs.Empty);
      }
      finally
      {
        platformChangeState.Dispose();
      }
    }

    private void OnViewServiceActiveViewChanging(object sender, ViewChangedEventArgs e)
    {
      SceneView view = e.NewView as SceneView;
      if (view == null || view.IsClosing)
        return;
      this.UpdateForPlatformChange(view, e.OldView as SceneView, true);
    }

    private void OnProjectManagerSolutionClosed(object sender, SolutionEventArgs e)
    {
      this.allowsInitialRestart = true;
    }

    private class PlatformChangeState : IDisposable
    {
      public ViewUpdateManager ViewUpdateManager { get; private set; }

      public IPlatform Platform { get; private set; }

      public SceneView OldView { get; private set; }

      public SceneView NewView { get; private set; }

      public IDisposable PostponeToken { get; set; }

      public PlatformChangeState(ViewUpdateManager viewUpdateManager, IPlatform platform, SceneView oldView, SceneView newView, IDisposable postponeToken)
      {
        this.ViewUpdateManager = viewUpdateManager;
        this.Platform = platform;
        this.OldView = oldView;
        this.NewView = newView;
        this.PostponeToken = postponeToken;
      }

      public void Dispose()
      {
        if (this.PostponeToken != null)
        {
          this.PostponeToken.Dispose();
          this.PostponeToken = (IDisposable) null;
        }
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
