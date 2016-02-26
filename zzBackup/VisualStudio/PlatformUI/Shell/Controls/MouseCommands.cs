// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.MouseCommands
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public static class MouseCommands
  {
    public static readonly DependencyProperty MouseCommandActionProperty = DependencyProperty.RegisterAttached("MouseCommandAction", typeof (MouseAction), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata((object) MouseAction.LeftClick, new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty MouseCommandProperty = DependencyProperty.RegisterAttached("MouseCommand", typeof (ICommand), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty MouseCommandParameterProperty = DependencyProperty.RegisterAttached("MouseCommandParameter", typeof (object), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty ControlMouseCommandProperty = DependencyProperty.RegisterAttached("ControlMouseCommand", typeof (ICommand), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty ControlMouseCommandParameterProperty = DependencyProperty.RegisterAttached("ControlMouseCommandParameter", typeof (object), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty ShiftMouseCommandProperty = DependencyProperty.RegisterAttached("ShiftMouseCommand", typeof (ICommand), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));
    public static readonly DependencyProperty ShiftMouseCommandParameterProperty = DependencyProperty.RegisterAttached("ShiftMouseCommandParameter", typeof (object), typeof (MouseCommands), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseCommands.OnMouseCommandChanged)));

    public static MouseAction GetMouseCommandAction(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (MouseAction) element.GetValue(MouseCommands.MouseCommandActionProperty);
    }

    public static void SetMouseCommandAction(UIElement element, MouseAction value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.MouseCommandActionProperty, (object) value);
    }

    public static ICommand GetMouseCommand(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (ICommand) element.GetValue(MouseCommands.MouseCommandProperty);
    }

    public static void SetMouseCommand(UIElement element, ICommand value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.MouseCommandProperty, (object) value);
    }

    public static object GetMouseCommandParameter(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return element.GetValue(MouseCommands.MouseCommandParameterProperty);
    }

    public static void SetMouseCommandParameter(UIElement element, object value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.MouseCommandParameterProperty, value);
    }

    public static ICommand GetControlMouseCommand(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (ICommand) element.GetValue(MouseCommands.ControlMouseCommandProperty);
    }

    public static void SetControlMouseCommand(UIElement element, ICommand value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.ControlMouseCommandProperty, (object) value);
    }

    public static object GetControlMouseCommandParameter(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return element.GetValue(MouseCommands.ControlMouseCommandParameterProperty);
    }

    public static void SetControlMouseCommandParameter(UIElement element, object value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.ControlMouseCommandParameterProperty, value);
    }

    public static ICommand GetShiftMouseCommand(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return (ICommand) element.GetValue(MouseCommands.ShiftMouseCommandProperty);
    }

    public static void SetShiftMouseCommand(UIElement element, object value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.ShiftMouseCommandProperty, value);
    }

    public static object GetShiftMouseCommandParameter(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return element.GetValue(MouseCommands.ShiftMouseCommandParameterProperty);
    }

    public static void SetShiftMouseCommandParameter(UIElement element, object value)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      element.SetValue(MouseCommands.ShiftMouseCommandParameterProperty, value);
    }

    private static void OnMouseCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      MouseCommands.RefreshMouseBinding((UIElement) obj);
    }

    private static void RefreshMouseBinding(UIElement element)
    {
      if (MouseCommands.GetMouseCommand(element) == null)
        return;
      bool flag = false;
      foreach (InputBinding inputBinding in element.InputBindings)
      {
        MouseBinding mouseBinding = inputBinding as MouseBinding;
        if (mouseBinding != null)
        {
          MouseCommands.UpdateMouseBinding(element, mouseBinding);
          flag = true;
        }
      }
      if (flag)
        return;
      MouseCommands.AddMouseBindings(element);
    }

    private static void UpdateMouseBinding(UIElement element, MouseBinding mouseBinding)
    {
      ((MouseGesture) mouseBinding.Gesture).MouseAction = MouseCommands.GetMouseCommandAction(element);
      mouseBinding.Command = (ICommand) MouseCommands.KeyboardModifierChain.Instance;
      mouseBinding.CommandParameter = (object) element;
    }

    private static void AddMouseBindings(UIElement element)
    {
      MouseAction mouseCommandAction = MouseCommands.GetMouseCommandAction(element);
      MouseBinding mouseBinding1 = new MouseBinding();
      mouseBinding1.Gesture = (InputGesture) new MouseGesture(mouseCommandAction, ModifierKeys.None);
      mouseBinding1.Command = (ICommand) MouseCommands.KeyboardModifierChain.Instance;
      mouseBinding1.CommandParameter = (object) element;
      MouseBinding mouseBinding2 = mouseBinding1;
      element.InputBindings.Add((InputBinding) mouseBinding2);
      MouseBinding mouseBinding3 = new MouseBinding();
      mouseBinding3.Gesture = (InputGesture) new MouseGesture(mouseCommandAction, ModifierKeys.Control);
      mouseBinding3.Command = (ICommand) MouseCommands.KeyboardModifierChain.Instance;
      mouseBinding3.CommandParameter = (object) element;
      MouseBinding mouseBinding4 = mouseBinding3;
      element.InputBindings.Add((InputBinding) mouseBinding4);
      MouseBinding mouseBinding5 = new MouseBinding();
      mouseBinding5.Gesture = (InputGesture) new MouseGesture(mouseCommandAction, ModifierKeys.Shift);
      mouseBinding5.Command = (ICommand) MouseCommands.KeyboardModifierChain.Instance;
      mouseBinding5.CommandParameter = (object) element;
      MouseBinding mouseBinding6 = mouseBinding5;
      element.InputBindings.Add((InputBinding) mouseBinding6);
    }

    private class KeyboardModifierChain : ICommand
    {
      private static MouseCommands.KeyboardModifierChain instance;

      public static MouseCommands.KeyboardModifierChain Instance
      {
        get
        {
          return MouseCommands.KeyboardModifierChain.instance ?? (MouseCommands.KeyboardModifierChain.instance = new MouseCommands.KeyboardModifierChain());
        }
      }

      event EventHandler ICommand.CanExecuteChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public bool CanExecute(object parameter)
      {
        return true;
      }

      public void Execute(object parameter)
      {
        UIElement element = parameter as UIElement;
        if (element == null)
          return;
        ModifierKeys modifierKeys = NativeMethods.ModifierKeys;
        if (modifierKeys == ModifierKeys.Control && this.ExecuteCommand(element, new Func<UIElement, ICommand>(MouseCommands.GetControlMouseCommand), new Func<UIElement, object>(MouseCommands.GetControlMouseCommandParameter)) || modifierKeys == ModifierKeys.Shift && this.ExecuteCommand(element, new Func<UIElement, ICommand>(MouseCommands.GetShiftMouseCommand), new Func<UIElement, object>(MouseCommands.GetShiftMouseCommandParameter)))
          return;
        this.ExecuteCommand(element, new Func<UIElement, ICommand>(MouseCommands.GetMouseCommand), new Func<UIElement, object>(MouseCommands.GetMouseCommandParameter));
      }

      private bool ExecuteCommand(UIElement element, Func<UIElement, ICommand> commandAccessor, Func<UIElement, object> commandParameterAccessor)
      {
        ICommand command = commandAccessor(element);
        if (command == null)
          return false;
        object parameter = commandParameterAccessor(element);
        RoutedCommand routedCommand = command as RoutedCommand;
        if (routedCommand != null)
          routedCommand.Execute(parameter, (IInputElement) element);
        else
          command.Execute(parameter);
        return true;
      }
    }
  }
}
