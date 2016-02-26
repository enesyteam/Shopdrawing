// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.ToolGesture
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using System;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Windows.Design.Interaction
{
  public class ToolGesture : InputGesture
  {
    private ToolAction _toolAction;
    private MouseButtonState[] _buttons;
    private ModifierKeys _modifiers;

    public ToolAction ToolAction
    {
      get
      {
        return this._toolAction;
      }
      set
      {
        if (!EnumValidator.IsValid(value))
          throw new ArgumentOutOfRangeException("value");
        this._toolAction = value;
      }
    }

    public MouseButtonState LeftButton
    {
      get
      {
        return this._buttons[0];
      }
      set
      {
        this._buttons[0] = value;
      }
    }

    public MouseButtonState RightButton
    {
      get
      {
        return this._buttons[2];
      }
      set
      {
        this._buttons[2] = value;
      }
    }

    public MouseButtonState MiddleButton
    {
      get
      {
        return this._buttons[1];
      }
      set
      {
        this._buttons[1] = value;
      }
    }

    public MouseButtonState XButton1
    {
      get
      {
        return this._buttons[3];
      }
      set
      {
        this._buttons[3] = value;
      }
    }

    public MouseButtonState XButton2
    {
      get
      {
        return this._buttons[4];
      }
      set
      {
        this._buttons[4] = value;
      }
    }

    public ModifierKeys Modifiers
    {
      get
      {
        return this._modifiers;
      }
      set
      {
        this._modifiers = value;
      }
    }

    public ToolGesture()
    {
      this._toolAction = ToolAction.None;
      this._buttons = new MouseButtonState[5];
      for (int index = 0; index < this._buttons.Length; ++index)
        this._buttons[index] = MouseButtonState.Pressed;
      this._modifiers = ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows;
    }

    public ToolGesture(ToolAction action)
      : this()
    {
      if (!EnumValidator.IsValid(action))
        throw new ArgumentOutOfRangeException("action");
      this._toolAction = action;
    }

    public ToolGesture(ToolAction action, MouseButton button)
      : this()
    {
      this.ValidateButton(button);
      if (!EnumValidator.IsValid(action))
        throw new ArgumentOutOfRangeException("action");
      this._toolAction = action;
      for (int index = 0; index < this._buttons.Length; ++index)
        this._buttons[index] = MouseButtonState.Released;
      this._buttons[(int) button] = MouseButtonState.Pressed;
    }

    public ToolGesture(ToolAction action, MouseButton button, ModifierKeys modifiers)
      : this(action, button)
    {
      this.ValidateButton(button);
      if (!EnumValidator.IsValid(action))
        throw new ArgumentOutOfRangeException("action");
      this._toolAction = action;
      this._modifiers = modifiers;
    }

    private static ToolAction GetToolAction(InputEventArgs inputEventArgs)
    {
      ToolActionEventArgs toolActionEventArgs = inputEventArgs as ToolActionEventArgs;
      if (toolActionEventArgs != null)
        return toolActionEventArgs.ToolAction;
      return ToolAction.None;
    }

    private static ModifierKeys GetModifiers(InputEventArgs inputEventArgs)
    {
      DragEventArgs dragEventArgs = (DragEventArgs) null;
      ToolActionEventArgs toolActionEventArgs = inputEventArgs as ToolActionEventArgs;
      if (toolActionEventArgs != null)
        dragEventArgs = toolActionEventArgs.SourceEvent as DragEventArgs;
      ModifierKeys modifierKeys = ModifierKeys.None;
      if (dragEventArgs != null)
      {
        DragDropKeyStates keyStates = dragEventArgs.KeyStates;
        if ((keyStates & DragDropKeyStates.AltKey) != DragDropKeyStates.None)
          modifierKeys |= ModifierKeys.Alt;
        if ((keyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None)
          modifierKeys |= ModifierKeys.Control;
        if ((keyStates & DragDropKeyStates.ShiftKey) != DragDropKeyStates.None)
          modifierKeys |= ModifierKeys.Shift;
      }
      else
        modifierKeys = KeyboardHelper.Modifiers;
      return modifierKeys;
    }

    private static MouseButtonState[] GetButtons(InputEventArgs inputEventArgs)
    {
      MouseButtonState[] mouseButtonStateArray = new MouseButtonState[5];
      EventArgs eventArgs = (EventArgs) inputEventArgs;
      ToolActionEventArgs toolActionEventArgs = eventArgs as ToolActionEventArgs;
      if (toolActionEventArgs != null)
        eventArgs = toolActionEventArgs.SourceEvent;
      MouseButtonEventArgs mouseButtonEventArgs;
      if ((mouseButtonEventArgs = eventArgs as MouseButtonEventArgs) != null)
      {
        mouseButtonStateArray[(int) mouseButtonEventArgs.ChangedButton] = MouseButtonState.Pressed;
      }
      else
      {
        MouseEventArgs mouseEventArgs;
        if ((mouseEventArgs = eventArgs as MouseEventArgs) != null)
        {
          mouseButtonStateArray[0] = mouseEventArgs.LeftButton;
          mouseButtonStateArray[1] = mouseEventArgs.MiddleButton;
          mouseButtonStateArray[2] = mouseEventArgs.RightButton;
          mouseButtonStateArray[3] = mouseEventArgs.XButton1;
          mouseButtonStateArray[4] = mouseEventArgs.XButton2;
        }
        else
        {
          mouseButtonStateArray[0] = Mouse.LeftButton;
          mouseButtonStateArray[1] = Mouse.MiddleButton;
          mouseButtonStateArray[2] = Mouse.RightButton;
          mouseButtonStateArray[3] = Mouse.XButton1;
          mouseButtonStateArray[4] = Mouse.XButton2;
        }
      }
      return mouseButtonStateArray;
    }

    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
      if (inputEventArgs == null)
        throw new ArgumentNullException("inputEventArgs");
      if (this._toolAction != ToolGesture.GetToolAction(inputEventArgs))
        return false;
      ModifierKeys modifiers = ToolGesture.GetModifiers(inputEventArgs);
      if ((ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows) != this._modifiers && (modifiers == ModifierKeys.None && this._modifiers != ModifierKeys.None || (this._modifiers & modifiers) != modifiers))
        return false;
      MouseButtonState[] buttons = ToolGesture.GetButtons(inputEventArgs);
      for (int index = 0; index < this._buttons.Length; ++index)
      {
        if (this._buttons[index] == MouseButtonState.Released && buttons[index] == MouseButtonState.Pressed)
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this._buttons.Length; ++index)
      {
        if (this._buttons[index] == MouseButtonState.Pressed)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(":");
          stringBuilder.Append(((MouseButton) index).ToString());
        }
      }
      return (string) (object) this._toolAction + (object) ", " + stringBuilder.ToString() + ", " + (string) (object) this._modifiers;
    }

    private void ValidateButton(MouseButton button)
    {
      int num = (int) button;
      if (num < 0 || num > this._buttons.Length)
        throw new ArgumentOutOfRangeException("button");
    }
  }
}
