// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.SceneViewTabControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.View
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class SceneViewTabControl : TabControl, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register("MainContent", typeof (FrameworkElement), typeof (SceneViewTabControl));
    private double headerPanelSize = SceneViewTabControl.LastHeaderPanelSize;
    private static double LastHeaderPanelSize;
    private FrameworkElement headerPanel;
    internal TabItem DesignContentTabItem;
    internal ContentControl DesignContent;
    internal TabItem CodeContent;
    internal TabItem SplitViewContent;
    private bool _contentLoaded;

    public FrameworkElement MainContent
    {
      get
      {
        return (FrameworkElement) this.GetValue(SceneViewTabControl.MainContentProperty);
      }
      set
      {
        this.SetValue(SceneViewTabControl.MainContentProperty, (object) value);
      }
    }

    public Thickness VerticalScrollBarMargin
    {
      get
      {
        return new Thickness(0.0, this.headerPanelSize, 0.0, 0.0);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SceneViewTabControl(FrameworkElement mainContent)
    {
      this.MainContent = mainContent;
      this.InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.headerPanel = (FrameworkElement) this.GetTemplateChild("HeaderPanel");
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo info)
    {
      if (this.headerPanelSize != this.headerPanel.ActualHeight)
      {
        this.headerPanelSize = this.headerPanel.ActualHeight;
        SceneViewTabControl.LastHeaderPanelSize = this.headerPanelSize;
        this.OnPropertyChanged("VerticalScrollBarMargin");
      }
      base.OnRenderSizeChanged(info);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Home || e.Key == Key.End || e.Key == Key.Tab && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        return;
      base.OnKeyDown(e);
    }

    protected void OnPropertyChanged(string propertyName)
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/sceneview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.DesignContentTabItem = (TabItem) target;
          break;
        case 2:
          this.DesignContent = (ContentControl) target;
          break;
        case 3:
          this.CodeContent = (TabItem) target;
          break;
        case 4:
          this.SplitViewContent = (TabItem) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
