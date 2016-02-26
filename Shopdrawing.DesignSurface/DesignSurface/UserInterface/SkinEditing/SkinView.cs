// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.SkinView
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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class SkinView : System.Windows.Controls.UserControl, IComponentConnector
  {
    private StateModelManager modelManager;
    private DesignerContext designerContext;
    internal SkinView UserControl;
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public StateModelManager VisualStatesModel
    {
      get
      {
        return this.modelManager;
      }
    }

    internal SkinView(DesignerContext context)
    {
      this.designerContext = context;
      this.modelManager = new StateModelManager(this.designerContext);
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.SkinView_Loaded);
      this.Unloaded += new RoutedEventHandler(this.SkinView_Unloaded);
      this.DataContext = (object) this;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
      this.MinWidth = 153.0;
    }

    private void SkinView_Loaded(object sender, RoutedEventArgs e)
    {
      this.modelManager.Attach();
      this.MouseRightButtonDown += new MouseButtonEventHandler(this.SkinView_MouseRightButtonDown);
    }

    private void SkinView_Unloaded(object sender, RoutedEventArgs e)
    {
      this.modelManager.Detach();
      this.MouseRightButtonDown -= new MouseButtonEventHandler(this.SkinView_MouseRightButtonDown);
    }

    private void SkinView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None || (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.None)
        return;
      this.modelManager.IsStructureEditable = !this.modelManager.IsStructureEditable;
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
      bool flag = activeSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager);
      IWorkspace activeWorkspace = this.designerContext.WorkspaceService.ActiveWorkspace;
      if (activeWorkspace == null)
        return;
      ExpressionView expressionView = activeWorkspace.FindPalette("Interaction_Skin") as ExpressionView;
      if (expressionView == null)
        return;
      expressionView.IsForcedInvisible = !flag;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/skinediting/skinview.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (SkinView) target;
          break;
        case 2:
          this.LayoutRoot = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
