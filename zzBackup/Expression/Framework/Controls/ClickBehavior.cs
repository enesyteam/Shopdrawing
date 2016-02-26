// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ClickBehavior
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Controls
{
  public sealed class ClickBehavior
  {
    public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached("MouseDownCommand", typeof (ICommand), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ClickBehavior.ClickBehavior_MouseDownCommandPropertyChanged)));
    public static readonly DependencyProperty MouseClickCommandProperty = DependencyProperty.RegisterAttached("MouseClickCommand", typeof (ICommand), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ClickBehavior.ClickBehavior_MouseClickCommandPropertyChanged)));
    public static readonly DependencyProperty MouseDoubleDownCommandProperty = DependencyProperty.RegisterAttached("MouseDoubleDownCommand", typeof (ICommand), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ClickBehavior.ClickBehavior_MouseDoubleDownCommandPropertyChanged)));
    public static readonly DependencyProperty MouseEventParameterProperty = DependencyProperty.RegisterAttached("MouseEventParameter", typeof (object), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) null));
    public static readonly DependencyProperty LeftButtonDownCommandProperty = DependencyProperty.RegisterAttached("LeftButtonDownCommand", typeof (ICommand), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ClickBehavior.ClickBehavior_LeftButtonDownCommandPropertyChanged)));
    public static readonly DependencyProperty LeftButtonUpCommandProperty = DependencyProperty.RegisterAttached("LeftButtonUpCommand", typeof (ICommand), typeof (ClickBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(ClickBehavior.ClickBehavior_LeftButtonUpCommandPropertyChanged)));
    private static bool singleClickCanceled;
    private static DispatcherTimer timer;
    private static ICommand storedClickCommand;

    private ClickBehavior()
    {
    }

    public static void SetLeftButtonDownCommand(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.LeftButtonDownCommandProperty, value);
    }

    public static void SetLeftButtonUpCommand(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.LeftButtonUpCommandProperty, value);
    }

    public static void SetMouseEventParameter(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.MouseEventParameterProperty, value);
    }

    public static void SetMouseDownCommand(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.MouseDownCommandProperty, value);
    }

    private static void ClickBehavior_MouseDownCommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = target as UIElement;
      if (uiElement == null)
        return;
      if (e.OldValue == null && e.NewValue != null)
      {
        uiElement.MouseDown += new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDown);
      }
      else
      {
        if (e.OldValue == null || e.NewValue != null)
          return;
        uiElement.MouseDown -= new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDown);
      }
    }

    private static void ClickBehavior_LeftButtonDownCommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = target as UIElement;
      if (uiElement == null)
        return;
      if (e.OldValue == null && e.NewValue != null)
      {
        uiElement.MouseLeftButtonDown += new MouseButtonEventHandler(ClickBehavior.uiElement_LeftButtonDown);
      }
      else
      {
        if (e.OldValue == null || e.NewValue != null)
          return;
        uiElement.MouseLeftButtonDown -= new MouseButtonEventHandler(ClickBehavior.uiElement_LeftButtonDown);
      }
    }

    private static void ClickBehavior_LeftButtonUpCommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = target as UIElement;
      if (uiElement == null)
        return;
      if (e.OldValue == null && e.NewValue != null)
      {
        uiElement.MouseLeftButtonUp += new MouseButtonEventHandler(ClickBehavior.uiElement_LeftButtonUp);
      }
      else
      {
        if (e.OldValue == null || e.NewValue != null)
          return;
        uiElement.MouseLeftButtonUp -= new MouseButtonEventHandler(ClickBehavior.uiElement_LeftButtonUp);
      }
    }

    private static void uiElement_MouseDown(object sender, MouseButtonEventArgs e)
    {
      UIElement uiElement = sender as UIElement;
      ICommand command = (ICommand) uiElement.GetValue(ClickBehavior.MouseDownCommandProperty);
      if (command == null)
        return;
      RoutedCommand routedCommand = command as RoutedCommand;
      object parameter = uiElement.GetValue(ClickBehavior.MouseEventParameterProperty) ?? (object) uiElement;
      if (routedCommand != null)
        routedCommand.Execute(parameter, (IInputElement) uiElement);
      else
        command.Execute(parameter);
      e.Handled = true;
    }

    private static void uiElement_LeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ICommand command = (ICommand) (sender as UIElement).GetValue(ClickBehavior.LeftButtonDownCommandProperty);
      if (command == null || Keyboard.IsKeyDown(Key.LeftShift) || (Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftCtrl)) || Keyboard.IsKeyDown(Key.RightCtrl))
        return;
      if (e.ClickCount == 2)
      {
        if (ClickBehavior.timer == null)
          return;
        ClickBehavior.singleClickCanceled = true;
        ClickBehavior.timer.Stop();
        ClickBehavior.timer = (DispatcherTimer) null;
      }
      else
      {
        ClickBehavior.singleClickCanceled = false;
        command.Execute((object) null);
        e.Handled = true;
      }
    }

    private static void uiElement_LeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ICommand command = (ICommand) (sender as UIElement).GetValue(ClickBehavior.LeftButtonUpCommandProperty);
      if (command == null || Keyboard.IsKeyDown(Key.LeftShift) || (Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftCtrl)) || Keyboard.IsKeyDown(Key.RightCtrl))
        return;
      if (!ClickBehavior.singleClickCanceled)
      {
        if (ClickBehavior.timer != null)
          ClickBehavior.timer.Stop();
        ClickBehavior.timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        ClickBehavior.timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
        ClickBehavior.timer.Tick += new EventHandler(ClickBehavior.Timer_Tick);
        ClickBehavior.storedClickCommand = command;
        ClickBehavior.timer.Start();
      }
      e.Handled = true;
    }

    private static void Timer_Tick(object sender, EventArgs args)
    {
      if (ClickBehavior.timer != null)
      {
        ClickBehavior.timer.Stop();
        ClickBehavior.timer = (DispatcherTimer) null;
      }
      if (ClickBehavior.singleClickCanceled)
        return;
      ClickBehavior.storedClickCommand.Execute((object) null);
    }

    public static void SetMouseClickCommand(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.MouseClickCommandProperty, value);
    }

    private static void ClickBehavior_MouseClickCommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = target as UIElement;
      if (uiElement == null)
        return;
      if (e.OldValue == null && e.NewValue != null)
      {
        uiElement.MouseDown += new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDownInitial);
      }
      else
      {
        if (e.OldValue == null || e.NewValue != null)
          return;
        uiElement.MouseDown -= new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDownInitial);
      }
    }

    private static void uiElement_MouseDownInitial(object sender, MouseButtonEventArgs e)
    {
      UIElement uiElement = sender as UIElement;
      ICommand command = (ICommand) uiElement.GetValue(ClickBehavior.MouseClickCommandProperty);
      bool flag = false;
      if (command != null)
      {
        RoutedCommand routedCommand = command as RoutedCommand;
        object parameter = uiElement.GetValue(ClickBehavior.MouseEventParameterProperty) ?? (object) uiElement;
        flag = routedCommand == null ? command.CanExecute(parameter) : routedCommand.CanExecute(parameter, (IInputElement) uiElement);
      }
      if (!flag)
        return;
      uiElement.MouseUp += new MouseButtonEventHandler(ClickBehavior.uiElement_MouseUp);
      e.MouseDevice.Capture((IInputElement) uiElement);
      e.Handled = true;
    }

    private static void uiElement_MouseUp(object sender, MouseButtonEventArgs e)
    {
      UIElement uiElement = sender as UIElement;
      uiElement.MouseUp -= new MouseButtonEventHandler(ClickBehavior.uiElement_MouseUp);
      e.MouseDevice.Capture((IInputElement) null);
      ICommand command = (ICommand) uiElement.GetValue(ClickBehavior.MouseClickCommandProperty);
      if (command != null)
      {
        RoutedCommand routedCommand = command as RoutedCommand;
        object parameter = uiElement.GetValue(ClickBehavior.MouseEventParameterProperty) ?? (object) uiElement;
        if (routedCommand != null)
          routedCommand.Execute(parameter, (IInputElement) uiElement);
        else
          command.Execute(parameter);
      }
      e.Handled = true;
    }

    public static void SetMouseDoubleDownCommand(DependencyObject target, object value)
    {
      target.SetValue(ClickBehavior.MouseDoubleDownCommandProperty, value);
    }

    private static void ClickBehavior_MouseDoubleDownCommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      UIElement uiElement = target as UIElement;
      if (uiElement == null)
        return;
      if (e.OldValue == null && e.NewValue != null)
      {
        uiElement.AddHandler(UIElement.MouseDownEvent, (Delegate) new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDoubleDown), true);
      }
      else
      {
        if (e.OldValue == null || e.NewValue != null)
          return;
        uiElement.RemoveHandler(UIElement.MouseDownEvent, (Delegate) new MouseButtonEventHandler(ClickBehavior.uiElement_MouseDoubleDown));
      }
    }

    private static void uiElement_MouseDoubleDown(object sender, MouseButtonEventArgs e)
    {
      UIElement uiElement = sender as UIElement;
      if (e.ClickCount <= 1)
        return;
      ICommand command = (ICommand) uiElement.GetValue(ClickBehavior.MouseDoubleDownCommandProperty);
      if (command == null)
        return;
      RoutedCommand routedCommand = command as RoutedCommand;
      object parameter = uiElement.GetValue(ClickBehavior.MouseEventParameterProperty) ?? (object) uiElement;
      if (routedCommand != null)
        routedCommand.Execute(parameter, (IInputElement) uiElement);
      else
        command.Execute(parameter);
      e.Handled = true;
    }
  }
}
