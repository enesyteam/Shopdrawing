// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.PartsPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class PartsPane : DockPanel
  {
    private DesignerContext designerContext;
    private SceneNode lastEditingContainer;

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    private bool PartsSupported
    {
      get
      {
        if (this.designerContext.ActiveSceneViewModel != null && this.designerContext.ActiveSceneViewModel.ActiveEditingContainer != null && this.designerContext.ActiveSceneViewModel.ActiveEditingContainer.DocumentNode.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsTemplateParts))
          return this.designerContext.ActiveSceneViewModel.ActiveEditingContainer is ControlTemplateElement;
        return false;
      }
    }

    private ExpressionView Palette
    {
      get
      {
        IWorkspace activeWorkspace = this.designerContext.WorkspaceService.ActiveWorkspace;
        if (activeWorkspace != null)
          return activeWorkspace.FindPalette("Interaction_Parts") as ExpressionView;
        return (ExpressionView) null;
      }
    }

    internal PartsPane(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.Children.Add((UIElement) new PartsView(this.DesignerContext));
      if (this.designerContext.SelectionManager == null)
        return;
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
    }

    public void Unload()
    {
      if (this.designerContext.SelectionManager != null)
        this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (this.designerContext.ActiveSceneViewModel == null)
      {
        this.lastEditingContainer = (SceneNode) null;
      }
      else
      {
        ExpressionView palette = this.Palette;
        if (palette == null || this.designerContext.ActiveSceneViewModel.ActiveEditingContainer == null)
          return;
        SceneNode sceneNode = this.designerContext.ActiveSceneViewModel.GetSceneNode(this.designerContext.ActiveSceneViewModel.ActiveEditingContainer.DocumentNode);
        if (sceneNode == this.lastEditingContainer)
          return;
        this.lastEditingContainer = sceneNode;
        if (palette.IsDesiredVisible || !this.PartsSupported)
          return;
        palette.IsDesiredVisible = true;
      }
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.UpdatePaneVisibility();
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.UpdatePaneVisibility();
    }

    private void UpdatePaneVisibility()
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel == null)
        return;
      bool flag = activeSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsTemplateParts);
      ExpressionView palette = this.Palette;
      if (palette == null)
        return;
      palette.IsForcedInvisible = !flag;
    }
  }
}
