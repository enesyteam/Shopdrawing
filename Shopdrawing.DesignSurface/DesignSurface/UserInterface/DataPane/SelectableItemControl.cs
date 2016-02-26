// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SelectableItemControl
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SelectableItemControl : ContentControl
  {
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof (bool), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(SelectableItemControl.OnIsSelectedChanged)));
    public static readonly DependencyProperty LeftDoubleClickCommandProperty = DependencyProperty.Register("LeftDoubleClickCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftMouseDownCommandProperty = DependencyProperty.Register("LeftMouseDownCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftMouseUpCommandProperty = DependencyProperty.Register("LeftMouseUpCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlMouseDownCommandProperty = DependencyProperty.Register("LeftControlMouseDownCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlMouseUpCommandProperty = DependencyProperty.Register("LeftControlMouseUpCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftShiftMouseDownCommandProperty = DependencyProperty.Register("LeftShiftMouseDownCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftShiftMouseUpCommandProperty = DependencyProperty.Register("LeftShiftMouseUpCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlShiftMouseDownCommandProperty = DependencyProperty.Register("LeftControlShiftMouseDownCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty LeftControlShiftMouseUpCommandProperty = DependencyProperty.Register("LeftControlShiftMouseUpCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    public static readonly DependencyProperty RightMouseDownCommandProperty = DependencyProperty.Register("RightMouseDownCommand", typeof (ICommand), typeof (SelectableItemControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
    private bool hasModifiedSelection;

    public ICommand LeftMouseDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftMouseDownCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftMouseDownCommandProperty, (object) value);
      }
    }

    public ICommand LeftMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftShiftMouseDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftShiftMouseDownCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftShiftMouseDownCommandProperty, (object) value);
      }
    }

    public ICommand LeftShiftMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftShiftMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftShiftMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlMouseDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftControlMouseDownCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftControlMouseDownCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftControlMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftControlMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlShiftMouseDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftControlShiftMouseDownCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftControlShiftMouseDownCommandProperty, (object) value);
      }
    }

    public ICommand LeftControlShiftMouseUpCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftControlShiftMouseUpCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftControlShiftMouseUpCommandProperty, (object) value);
      }
    }

    public ICommand LeftDoubleClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.LeftDoubleClickCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.LeftDoubleClickCommandProperty, (object) value);
      }
    }

    public ICommand RightMouseDownCommand
    {
      get
      {
        return (ICommand) this.GetValue(SelectableItemControl.RightMouseDownCommandProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.RightMouseDownCommandProperty, (object) value);
      }
    }

    public bool IsSelected
    {
      get
      {
        return (bool) this.GetValue(SelectableItemControl.IsSelectedProperty);
      }
      set
      {
        this.SetValue(SelectableItemControl.IsSelectedProperty, (object) (bool) (value ? true : false));
      }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      Keyboard.Focus((IInputElement) null);
      this.hasModifiedSelection = false;
      if (e.ClickCount == 2)
      {
        e.Handled = this.ExecuteCommand(this.LeftDoubleClickCommand);
      }
      else
      {
        bool flag1 = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        bool flag2 = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        ICommand commandToExecute = !flag1 || !flag2 ? (!flag2 ? (!flag1 ? this.LeftMouseDownCommand : this.LeftControlMouseDownCommand) : this.LeftShiftMouseDownCommand) : this.LeftControlShiftMouseDownCommand;
        e.Handled = this.ExecuteCommand(commandToExecute);
      }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      if (this.hasModifiedSelection)
        return;
      bool flag1 = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
      bool flag2 = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
      ICommand commandToExecute = !flag1 || !flag2 ? (!flag2 ? (!flag1 ? this.LeftMouseUpCommand : this.LeftControlMouseUpCommand) : this.LeftShiftMouseUpCommand) : this.LeftControlShiftMouseUpCommand;
      e.Handled = this.ExecuteCommand(commandToExecute);
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseRightButtonDown(e);
      this.Focus();
      e.Handled = this.ExecuteCommand(this.RightMouseDownCommand);
    }

    private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      SelectableItemControl selectableItemControl = sender as SelectableItemControl;
      if (selectableItemControl == null)
        return;
      selectableItemControl.hasModifiedSelection = true;
    }

    protected bool ExecuteCommand(ICommand commandToExecute)
    {
      if (commandToExecute == null)
        return false;
      commandToExecute.Execute((object) null);
      return true;
    }
  }
}
