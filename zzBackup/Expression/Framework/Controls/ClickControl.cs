// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ClickControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Controls
{
  public class ClickControl : ContentControl
  {
    public static readonly DependencyProperty MouseStartMoveCommandProperty = DependencyProperty.Register("MouseStartMoveCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ControlMouseStartMoveCommandProperty = DependencyProperty.Register("ControlMouseStartMoveCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftMouseUpCommandProperty = DependencyProperty.Register("LeftMouseUpCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlMouseUpCommandProperty = DependencyProperty.Register("LeftControlMouseUpCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftClickCommandProperty = DependencyProperty.Register("LeftClickCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftDoubleClickCommandProperty = DependencyProperty.Register("LeftDoubleClickCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftShiftClickCommandProperty = DependencyProperty.Register("LeftShiftClickCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlClickCommandProperty = DependencyProperty.Register("LeftControlClickCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty RightClickCommandProperty = DependencyProperty.Register("RightClickCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ContextMenuOpeningCommandProperty = DependencyProperty.Register("ContextMenuOpeningCommand", typeof (ICommand), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached("ScrollIntoView", typeof (bool), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ClickControl.ClickControl_ScrollIntoViewPropertyInvalidated), (CoerceValueCallback) null));
    public static readonly DependencyProperty DelayedScrollIntoViewProperty = DependencyProperty.RegisterAttached("DelayedScrollIntoView", typeof (bool), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ClickControl.ClickControl_DelayedScrollIntoViewPropertyInvalidated), (CoerceValueCallback) null));
    public static readonly DependencyProperty AllowDoubleClickToCancelSingleClickProperty = DependencyProperty.Register("AllowDoubleClickToCancelSingleClick", typeof (bool), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof (object), typeof (ClickControl), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
    private bool mouseStartMoveCommandExecuted;
    private bool singleClickCanceled;
    private DispatcherTimer timer;
    private ICommand storedClickCommand;

    public ICommand MouseStartMoveCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.MouseStartMoveCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.MouseStartMoveCommandProperty, (object) value);
      }
    }

    public ICommand ControlMouseStartMoveCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.ControlMouseStartMoveCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.ControlMouseStartMoveCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftControlMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftControlMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftClickCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftClickCommandProperty, (object) value);
      }
    }

    public ICommand LeftShiftClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftShiftClickCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftShiftClickCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftControlClickCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftControlClickCommandProperty, (object) value);
      }
    }

    public ICommand LeftDoubleClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.LeftDoubleClickCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.LeftDoubleClickCommandProperty, (object) value);
      }
    }

    public ICommand RightClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.RightClickCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.RightClickCommandProperty, (object) value);
      }
    }

    public ICommand ContextMenuOpeningCommand
    {
      get
      {
        return (ICommand) this.GetValue(ClickControl.ContextMenuOpeningCommandProperty);
      }
      set
      {
        this.SetValue(ClickControl.ContextMenuOpeningCommandProperty, (object) value);
      }
    }

    public bool ScrollIntoView
    {
        get
        {
            return (bool)base.GetValue(ClickControl.ScrollIntoViewProperty);
        }
        set
        {
            base.SetValue(ClickControl.ScrollIntoViewProperty, value);
        }
    }

    public bool AllowDoubleClickToCancelSingleClick
    {
        get
        {
            return (bool)base.GetValue(ClickControl.AllowDoubleClickToCancelSingleClickProperty);
        }
        set
        {
            base.SetValue(ClickControl.AllowDoubleClickToCancelSingleClickProperty, value);
        }
    }

    public object CommandParameter
    {
      get
      {
        return this.GetValue(ClickControl.CommandParameterProperty);
      }
      set
      {
        this.SetValue(ClickControl.CommandParameterProperty, value);
      }
    }

    public static bool GetScrollIntoView(DependencyObject target)
    {
      return (bool) target.GetValue(ClickControl.ScrollIntoViewProperty);
    }

    public static void SetScrollIntoView(DependencyObject target, bool value)
    {
        target.SetValue(ClickControl.ScrollIntoViewProperty, value);
    }

    public static bool GetDelayedScrollIntoView(DependencyObject target)
    {
      return (bool) target.GetValue(ClickControl.DelayedScrollIntoViewProperty);
    }

    public static void SetDelayedScrollIntoView(DependencyObject target, bool value)
    {
        target.SetValue(ClickControl.DelayedScrollIntoViewProperty, value);
    }

    protected void ExecuteCommand(ICommand commandToExecute)
    {
      if (commandToExecute == null)
        return;
      commandToExecute.Execute(this.CommandParameter);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      this.mouseStartMoveCommandExecuted = false;
      if (e.ClickCount == 2)
      {
        if (this.timer != null && this.AllowDoubleClickToCancelSingleClick)
        {
          this.singleClickCanceled = true;
          this.timer.Stop();
          this.timer = (DispatcherTimer) null;
        }
        ICommand commandToExecute = (ICommand) this.GetValue(ClickControl.LeftDoubleClickCommandProperty);
        if (commandToExecute == null)
          return;
        this.ExecuteCommand(commandToExecute);
        e.Handled = true;
      }
      else
      {
        this.singleClickCanceled = false;
        ICommand singleClickCommand = this.GetSingleClickCommand(e);
        if (singleClickCommand == null)
          return;
        this.ExecuteCommand(singleClickCommand);
        e.Handled = true;
      }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      this.mouseStartMoveCommandExecuted = false;
      ICommand singleClickCommand = this.GetSingleClickCommand(e);
      if (singleClickCommand == null)
        return;
      if (this.AllowDoubleClickToCancelSingleClick)
      {
        if (!this.singleClickCanceled)
        {
          if (this.timer != null)
            this.timer.Stop();
          this.timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
          this.timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
          this.timer.Tick += new EventHandler(this.Timer_Tick);
          this.storedClickCommand = singleClickCommand;
          this.timer.Start();
        }
      }
      else
        this.ExecuteCommand(singleClickCommand);
      e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (this.mouseStartMoveCommandExecuted || e.LeftButton != MouseButtonState.Pressed)
        return;
      ICommand commandToExecute = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? (ICommand) this.GetValue(ClickControl.ControlMouseStartMoveCommandProperty) : (ICommand) this.GetValue(ClickControl.MouseStartMoveCommandProperty);
      if (commandToExecute == null)
        return;
      this.ExecuteCommand(commandToExecute);
      this.mouseStartMoveCommandExecuted = true;
    }

    private void Timer_Tick(object sender, EventArgs args)
    {
      if (this.timer != null)
      {
        this.timer.Stop();
        this.timer = (DispatcherTimer) null;
      }
      if (this.singleClickCanceled)
        return;
      this.ExecuteCommand(this.storedClickCommand);
    }

    private ICommand GetSingleClickCommand(MouseButtonEventArgs e)
    {
      return e.ButtonState != MouseButtonState.Pressed ? (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? (ICommand) this.GetValue(ClickControl.LeftControlMouseUpCommandProperty) : (ICommand) this.GetValue(ClickControl.LeftMouseUpCommandProperty)) : (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? (ICommand) this.GetValue(ClickControl.LeftShiftClickCommandProperty) : (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ? (ICommand) this.GetValue(ClickControl.LeftControlClickCommandProperty) : (ICommand) this.GetValue(ClickControl.LeftClickCommandProperty)));
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseRightButtonDown(e);
      ICommand commandToExecute = (ICommand) this.GetValue(ClickControl.RightClickCommandProperty);
      if (commandToExecute == null)
        return;
      this.ExecuteCommand(commandToExecute);
      e.Handled = true;
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
      base.OnContextMenuOpening(e);
      ICommand commandToExecute = (ICommand) this.GetValue(ClickControl.ContextMenuOpeningCommandProperty);
      if (commandToExecute == null)
        return;
      this.ExecuteCommand(commandToExecute);
    }

    private static void ClickControl_ScrollIntoViewPropertyInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (!(bool) target.GetValue(ClickControl.ScrollIntoViewProperty))
        return;
      FrameworkElement frameworkElement = target as FrameworkElement;
      if (frameworkElement == null)
        return;
      frameworkElement.BringIntoView();
    }

    private static void ClickControl_DelayedScrollIntoViewPropertyInvalidated(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (!(bool) target.GetValue(ClickControl.DelayedScrollIntoViewProperty))
        return;
      FrameworkElement element = target as FrameworkElement;
      if (element == null)
        return;
      UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Render, (Action) (() =>
      {
        element.BringIntoView();
        element.SetValue(ClickControl.DelayedScrollIntoViewProperty, (object) false);
      }));
    }
  }
}
