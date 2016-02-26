// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataPane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.Workspaces.Extension;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class DataPane : UserControl, IComponentConnector
  {
    private DesignerContext designerContext;
    private DataPanelModel model;
    private FrameworkElement menuHost;
    private FileDropUtility dropUtility;
    private bool initialized;
    internal ItemsControl ItemsControl;
    private bool _contentLoaded;

    public DataPanelModel DataPanelModel
    {
      get
      {
        return this.model;
      }
    }

    private SceneViewModel ActiveSceneViewModel
    {
      get
      {
        if (this.designerContext.ActiveSceneViewModel != null && this.designerContext.ActiveView.IsDesignSurfaceVisible)
          return this.designerContext.ActiveSceneViewModel;
        return (SceneViewModel) null;
      }
    }

    internal DataPane(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.dropUtility = new FileDropUtility(this.designerContext.ProjectManager, (FrameworkElement) this, new IDocumentType[1]
      {
        designerContext.DocumentTypeManager.DocumentTypes[DocumentTypeNamesHelper.Xml]
      });
      this.model = this.ActiveSceneViewModel == null ? new DataPanelModel((SceneViewModel) null, false) : this.ActiveSceneViewModel.DataPanelModel;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
      this.InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
      if (!this.initialized)
      {
        this.DataContext = (object) this.model;
        this.menuHost = ElementUtilities.FindElement((FrameworkElement) this, "AddDataSourceMenuClickControl");
        this.model.AddDataSourceMenuHost = this.menuHost;
        this.SetValue(PaletteRegistry.PaletteHeaderContentProperty, this.Resources[(object) "PaletteHeaderContent"]);
        this.initialized = true;
      }
      base.OnInitialized(e);
    }

    protected override void OnDragLeave(DragEventArgs e)
    {
      this.HandleDragOver(e);
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      this.HandleDragOver(e);
    }

    protected override void OnDragEnter(DragEventArgs e)
    {
      this.HandleDragOver(e);
    }

    private void HandleDragOver(DragEventArgs e)
    {
      if (this.designerContext.ActiveSceneViewModel != null && this.designerContext.ActiveSceneViewModel.Document.XamlDocument.ParseErrorsCount == 0 && this.designerContext.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && ((e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent("BlendProjectItem")) && this.dropUtility.GetSupportedFiles(e.Data).Length != 0))
        return;
      e.Effects = DragDropEffects.None;
      e.Handled = true;
    }

    protected override void OnDrop(DragEventArgs e)
    {
      if (this.ActiveSceneViewModel == null || this.model.DataHosts.Count <= 0)
        return;
      this.model.AddXmlDataSourceFromDrop(this.dropUtility.GetSupportedFiles(e.Data));
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.UpdatePaneVisibility();
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (this.model == null || this.model.ViewModel != this.ActiveSceneViewModel)
      {
        this.model = this.ActiveSceneViewModel == null ? new DataPanelModel((SceneViewModel) null, false) : this.ActiveSceneViewModel.DataPanelModel;
        this.model.AddDataSourceMenuHost = this.menuHost;
        this.DataContext = (object) this.model;
      }
      this.UpdatePaneVisibility();
    }

    private void UpdatePaneVisibility()
    {
      SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
      if (activeSceneViewModel == null)
        return;
      bool flag = JoltHelper.DatabindingSupported(activeSceneViewModel.ProjectContext);
      IWorkspace activeWorkspace = this.designerContext.WorkspaceService.ActiveWorkspace;
      if (activeWorkspace == null)
        return;
      ExpressionView expressionView = activeWorkspace.FindPalette("Designer_DataPane") as ExpressionView;
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/datapane/datapane.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.ItemsControl = (ItemsControl) target;
      else
        this._contentLoaded = true;
    }
  }
}
