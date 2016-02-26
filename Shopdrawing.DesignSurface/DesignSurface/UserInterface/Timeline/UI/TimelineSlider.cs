// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelineSlider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimelineSlider : ClickControl
  {
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof (bool), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty SlideBeginCommandProperty = DependencyProperty.Register("SlideBeginCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty SlideEndCommandProperty = DependencyProperty.Register("SlideEndCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty SlideCommandProperty = DependencyProperty.Register("SlideCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (double), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof (double), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.PositiveInfinity));
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof (double), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NegativeInfinity));
    public static readonly DependencyProperty ReferenceElementProperty = DependencyProperty.Register("ReferenceElement", typeof (IInputElement), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register("SelectCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ExtendSelectCommandProperty = DependencyProperty.Register("ExtendSelectCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ToggleSelectCommandProperty = DependencyProperty.Register("ToggleSelectCommand", typeof (ICommand), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty AltKeyDownProperty = DependencyProperty.Register("AltKeyDown", typeof (bool), typeof (TimelineSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private bool isLeftButtonDown;
    private bool haveMoved;
    private bool toggleOnMouseUp;
    private bool selectOnMouseUp;
    private Point originalMousePosition;
    private double originalValue;
    private Point previousAbsoluteMousePosition;
    private DispatcherTimer autoScrollTimer;
    private Rect viewRect;

    public bool IsSelected
    {
      get
      {
        return (bool) this.GetValue(TimelineSlider.IsSelectedProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.IsSelectedProperty, (object) (bool) (value ? true : false));
      }
    }

    public ICommand SlideBeginCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.SlideBeginCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.SlideBeginCommandProperty, (object) value);
      }
    }

    public ICommand SlideCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.SlideCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.SlideCommandProperty, (object) value);
      }
    }

    public ICommand SlideEndCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.SlideEndCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.SlideEndCommandProperty, (object) value);
      }
    }

    public ICommand SelectCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.SelectCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.SelectCommandProperty, (object) value);
      }
    }

    public ICommand ExtendSelectCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.ExtendSelectCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.ExtendSelectCommandProperty, (object) value);
      }
    }

    public ICommand ToggleSelectCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelineSlider.ToggleSelectCommandProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.ToggleSelectCommandProperty, (object) value);
      }
    }

    public double Value
    {
      get
      {
        return (double) this.GetValue(TimelineSlider.ValueProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.ValueProperty, (object) value);
      }
    }

    public double Maximum
    {
      get
      {
        return (double) this.GetValue(TimelineSlider.MaximumProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.MaximumProperty, (object) value);
      }
    }

    public double Minimum
    {
      get
      {
        return (double) this.GetValue(TimelineSlider.MinimumProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.MinimumProperty, (object) value);
      }
    }

    public bool AltKeyDown
    {
      get
      {
        return (bool) this.GetValue(TimelineSlider.AltKeyDownProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.AltKeyDownProperty, (object) (bool) (value ? true : false));
      }
    }

    public IInputElement ReferenceElement
    {
      get
      {
        return (IInputElement) this.GetValue(TimelineSlider.ReferenceElementProperty);
      }
      set
      {
        this.SetValue(TimelineSlider.ReferenceElementProperty, (object) value);
      }
    }

    public TimelineSlider()
    {
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      if (this.isLeftButtonDown || !this.IsSelected)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ContextMenuRender, "Keyframe Context Menu");
      base.OnContextMenuOpening(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      if (!this.isLeftButtonDown)
        return;
      this.OnMouseLeftButtonUp(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (!this.isLeftButtonDown)
        return;
      e.Handled = true;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(this.Current_PreNotifyInput);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.Current_PreNotifyInput);
    }

    private void Current_PreNotifyInput(object sender, NotifyInputEventArgs e)
    {
      KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
      if (keyEventArgs == null || !keyEventArgs.IsDown && !keyEventArgs.IsUp)
        return;
      this.AltKeyDown = (Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      if (!this.isLeftButtonDown)
      {
        ValueEditorUtils.UpdateBinding((FrameworkElement) this, TimelineSlider.ValueProperty, UpdateBindingType.Target);
        e.MouseDevice.Capture((IInputElement) this);
        this.isLeftButtonDown = true;
        this.originalMousePosition = e.MouseDevice.GetPosition(this.GetReferenceElement());
        this.originalValue = this.Value;
        this.haveMoved = false;
        this.previousAbsoluteMousePosition = e.MouseDevice.GetPosition((IInputElement) null);
        this.viewRect = new Rect(e.MouseDevice.GetPosition((IInputElement) this).X - 3.0, 0.0, 6.0, this.ActualHeight);
        this.toggleOnMouseUp = false;
        this.selectOnMouseUp = false;
        ICommand command = (ICommand) null;
        if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
          command = this.ExtendSelectCommand;
        else if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
        {
          if (!this.IsSelected)
            command = this.ExtendSelectCommand;
          else
            this.toggleOnMouseUp = true;
        }
        else if (this.IsSelected)
          this.selectOnMouseUp = true;
        else
          command = this.SelectCommand;
        ValueEditorUtils.ExecuteCommand(command, (IInputElement) this, (object) null);
      }
      e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!this.isLeftButtonDown || !this.IsSelected)
        return;
      Point position = e.MouseDevice.GetPosition((IInputElement) null);
      if (position == this.previousAbsoluteMousePosition)
        return;
      this.previousAbsoluteMousePosition = position;
      if (!this.haveMoved)
      {
        this.haveMoved = true;
        ValueEditorUtils.ExecuteCommand((ICommand) this.GetValue(TimelineSlider.SlideBeginCommandProperty), (IInputElement) this, (object) null);
        this.autoScrollTimer = TimelineView.CreateAutoScrollTimer();
        this.autoScrollTimer.Tick += new EventHandler(this.AutoScrollTimer_Tick);
        this.autoScrollTimer.Start();
      }
      this.Value = this.ComputeValueFromPosition(e.MouseDevice.GetPosition(this.GetReferenceElement()));
      ValueEditorUtils.ExecuteCommand((ICommand) this.GetValue(TimelineSlider.SlideCommandProperty), (IInputElement) this, (object) null);
      this.BringIntoView(this.viewRect);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      if (!this.isLeftButtonDown)
        return;
      ICommand command = (ICommand) null;
      this.isLeftButtonDown = false;
      e.MouseDevice.Capture((IInputElement) null);
      if (this.IsSelected && this.haveMoved && this.autoScrollTimer != null)
      {
        this.autoScrollTimer.Stop();
        this.autoScrollTimer.Tick -= new EventHandler(this.AutoScrollTimer_Tick);
        this.autoScrollTimer = (DispatcherTimer) null;
        command = this.SlideEndCommand;
      }
      else if (!this.haveMoved)
      {
        if (this.selectOnMouseUp)
          command = this.SelectCommand;
        else if (this.toggleOnMouseUp)
          command = this.ToggleSelectCommand;
      }
      ValueEditorUtils.ExecuteCommand(command, (IInputElement) this, (object) null);
    }

    private void AutoScrollTimer_Tick(object sender, EventArgs e)
    {
      double valueFromPosition = this.ComputeValueFromPosition(InputManager.Current.PrimaryMouseDevice.GetPosition(this.GetReferenceElement()));
      if (this.Value == valueFromPosition)
        return;
      this.Value = valueFromPosition;
      ValueEditorUtils.ExecuteCommand((ICommand) this.GetValue(TimelineSlider.SlideCommandProperty), (IInputElement) this, (object) null);
      this.BringIntoView(this.viewRect);
    }

    private double ComputeValueFromPosition(Point currentPosition)
    {
      return Math.Min(this.Maximum, Math.Max(this.Minimum, this.originalValue + (currentPosition.X - this.originalMousePosition.X)));
    }

    private IInputElement GetReferenceElement()
    {
      return this.ReferenceElement ?? (IInputElement) this.Parent;
    }
  }
}
