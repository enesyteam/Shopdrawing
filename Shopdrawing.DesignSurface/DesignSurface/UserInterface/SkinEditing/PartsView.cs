// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.PartsView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class PartsView : System.Windows.Controls.UserControl, INotifyPropertyChanged, IComponentConnector, IStyleConnector
  {
    private DesignerContext designerContext;
    internal PartsView UserControl;
    internal Grid LayoutRoot;
    internal Border main_border;
    internal Icon IconImage;
    private bool _contentLoaded;

    public PartsModel PartsManager
    {
      get
      {
        if (this.designerContext.ActiveSceneViewModel != null)
          return this.designerContext.ActiveSceneViewModel.PartsModel;
        return (PartsModel) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal PartsView(DesignerContext context)
    {
      this.designerContext = context;
      this.InitializeComponent();
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.WorkspaceService.ActiveWorkspaceChanged += new EventHandler(this.WorkspaceService_ActiveWorkspaceChanged);
      this.DataContext = (object) this;
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.OnPropertyChanged("PartsManager");
    }

    private void WorkspaceService_ActiveWorkspaceChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("PartsManager");
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      PartInPartsExplorer part = (sender as FrameworkElement).DataContext as PartInPartsExplorer;
      if (part == null || e.ClickCount <= 1)
        return;
      this.PartsManager.OnDoubleClickItem(part);
    }

    private void OnClick(object sender, MouseButtonEventArgs e)
    {
      Run run = e.OriginalSource as Run;
      if (run == null || !(run.Parent is Hyperlink))
        return;
      this.designerContext.Services.GetService<ICommandService>().Execute("Application_ShowControlStylingTips", CommandInvocationSource.Command);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/skinediting/partsview.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (PartsView) target;
          break;
        case 3:
          this.LayoutRoot = (Grid) target;
          break;
        case 4:
          this.main_border = (Border) target;
          break;
        case 5:
          ((UIElement) target).PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.OnClick);
          break;
        case 6:
          this.IconImage = (Icon) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.OnMouseLeftButtonDown);
    }
  }
}
