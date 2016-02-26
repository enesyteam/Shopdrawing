// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Triggers.TriggersPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.Workspaces.Extension;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.Triggers
{
  internal sealed class TriggersPane : DockPanel
  {
    private DesignerContext designerContext;

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public TriggersPane(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.Children.Add((UIElement) new TriggersView(this.DesignerContext));
    }

    public void Unload()
    {
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.UpdatePaneVisibility();
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs args)
    {
      this.UpdatePaneVisibility();
    }

    private void UpdatePaneVisibility()
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel == null)
        return;
      bool flag = JoltHelper.TriggersSupported(activeSceneViewModel.ProjectContext);
      IWorkspace activeWorkspace = this.designerContext.WorkspaceService.ActiveWorkspace;
      if (activeWorkspace == null)
        return;
      ExpressionView expressionView = activeWorkspace.FindPalette("Designer_TriggersPane") as ExpressionView;
      if (expressionView == null)
        return;
      expressionView.IsForcedInvisible = !flag;
    }
  }
}
