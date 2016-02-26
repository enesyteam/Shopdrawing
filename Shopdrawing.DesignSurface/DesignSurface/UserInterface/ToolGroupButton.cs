// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ToolGroupButton
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ToolGroupButton : StickyButton, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty ToolSelectionPlacementProperty = DependencyProperty.Register("ToolSelectionPlacement", typeof (PlacementMode), typeof (ToolGroupButton), new PropertyMetadata((object) PlacementMode.Right));
    private List<ToolGroupItem> tools = new List<ToolGroupItem>();
    private ToolGroupItem activeTool;
    private DispatcherTimer timer;
    private string automationId;
    private bool isAssetMruTool;
    private bool _contentLoaded;

    public ToolGroupItem ActiveTool
    {
      get
      {
        return this.activeTool;
      }
    }

    public bool IsAssetMruTool
    {
      get
      {
        return this.isAssetMruTool;
      }
    }

    public PlacementMode ToolSelectionPlacement
    {
      get
      {
        return (PlacementMode) this.GetValue(ToolGroupButton.ToolSelectionPlacementProperty);
      }
      set
      {
        this.SetValue(ToolGroupButton.ToolSelectionPlacementProperty, (object) value);
      }
    }

    public int ToolCount
    {
      get
      {
        int num = 0;
        foreach (ToolGroupItem toolGroupItem in this.tools)
        {
          if (toolGroupItem.IsVisible)
            ++num;
        }
        return num;
      }
    }

    public string AutomationId
    {
      get
      {
        return this.automationId;
      }
    }

    public event EventHandler ToolGroupSelectionChanged;

    public event EventHandler ToolGroupActiveChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public ToolGroupButton(string automationId, bool isAssetMruTool)
    {
      this.automationId = automationId;
      this.isAssetMruTool = isAssetMruTool;
      this.InitializeComponent();
    }

    public void AddTool(ToolGroupItem tool)
    {
      this.tools.Add(tool);
      if (this.activeTool == null && tool.IsVisible)
        this.DisplayTool(tool);
      this.UpdateIsEnabled();
      this.OnPropertyChanged("ToolCount");
    }

    public bool RemoveTool(ToolGroupItem tool)
    {
      if (!this.tools.Remove(tool))
        return false;
      if (this.activeTool == tool)
        this.activeTool = (ToolGroupItem) null;
      this.UpdateIsEnabled();
      this.OnPropertyChanged("ToolCount");
      return true;
    }

    public void SetActive(ToolGroupItem tool)
    {
      if (this.tools.Contains(tool))
      {
        this.IsChecked = new bool?(true);
        this.DisplayTool(tool);
      }
      else
        this.IsChecked = new bool?(false);
    }

    public void UpdateIsEnabled()
    {
      if (this.ActiveTool != null)
        this.ActiveTool.FireIsEnabledChanged();
      bool flag1 = this.ActiveTool != null && !this.ActiveTool.IsVisible;
      bool flag2 = false;
      bool flag3 = false;
      foreach (ToolGroupItem toolGroupItem in this.tools)
      {
        if (toolGroupItem.IsEnabled)
        {
          flag2 = true;
          break;
        }
      }
      foreach (ToolGroupItem tool in this.tools)
      {
        if (tool.IsVisible)
        {
          if (flag1)
            this.DisplayTool(tool);
          flag3 = true;
          break;
        }
      }
      this.IsEnabled = flag2;
      this.Visibility = flag3 ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (this.timer == null)
        {
          this.timer = new DispatcherTimer();
          this.timer.Interval = TimeSpan.FromMilliseconds(200.0);
          this.timer.Tick += new EventHandler(this.timer_Tick);
        }
        if (this.IsEnabled)
        {
          if (this.ToolGroupSelectionChanged != null)
            this.ToolGroupSelectionChanged((object) this, EventArgs.Empty);
          this.IsChecked = new bool?(true);
        }
        if (this.tools.Count > 1)
          this.timer.Start();
        e.MouseDevice.Capture((IInputElement) this);
        e.Handled = true;
      }
      else if (e.ChangedButton == MouseButton.Right && this.tools.Count > 1)
      {
        this.DisplayToolSelectionPopup();
        e.Handled = true;
      }
      base.OnMouseDown(e);
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      if (this.IsEnabled && e.ChangedButton == MouseButton.Left && this.activeTool != null)
      {
        this.activeTool.DoubleClick();
        e.Handled = true;
      }
      base.OnMouseDoubleClick(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (this.timer != null)
          this.timer.Stop();
        e.MouseDevice.Capture((IInputElement) null);
      }
      base.OnMouseUp(e);
    }

    private void DisplayTool(ToolGroupItem tool)
    {
      this.activeTool = tool;
      this.OnPropertyChanged("ActiveTool");
      if (this.ToolGroupActiveChanged != null)
        this.ToolGroupActiveChanged((object) this, EventArgs.Empty);
      this.DataContext = (object) tool;
      ContextMenu contextMenu = tool.CreateContextMenu();
      if (contextMenu != null)
        this.ContextMenu = contextMenu;
      else
        this.ContextMenu = (ContextMenu) null;
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      this.timer.Stop();
      this.DisplayToolSelectionPopup();
    }

    private void DisplayToolSelectionPopup()
    {
      ContextMenu contextMenu = new ContextMenu();
      Style style = (Style) this.Resources[(object) "DropdownMenuItemStyle"];
      foreach (ToolGroupItem tool in this.tools)
      {
        if (tool.IsVisible)
        {
          MenuItem menuItem = new MenuItem();
          menuItem.Style = style;
          menuItem.DataContext = (object) tool;
          menuItem.Command = (ICommand) new ToolGroupButton.SwitchToolCommand(this, tool);
          contextMenu.Items.Add((object) menuItem);
        }
      }
      contextMenu.PlacementTarget = (UIElement) this;
      contextMenu.Placement = this.ToolSelectionPlacement;
      contextMenu.IsOpen = true;
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
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/toolpane/toolgroupbutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }

    private class SwitchToolCommand : ICommand
    {
      private ToolGroupItem tool;
      private ToolGroupButton button;

      public event EventHandler CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public SwitchToolCommand(ToolGroupButton button, ToolGroupItem tool)
      {
        this.button = button;
        this.tool = tool;
      }

      public bool CanExecute(object parameter)
      {
        return this.tool.IsEnabled;
      }

      public void Execute(object parameter)
      {
        this.button.DisplayTool(this.tool);
        if (this.button.ToolGroupSelectionChanged == null)
          return;
        this.button.ToolGroupSelectionChanged((object) this.button, EventArgs.Empty);
      }
    }
  }
}
