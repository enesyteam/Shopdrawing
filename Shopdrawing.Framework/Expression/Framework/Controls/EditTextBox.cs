// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.EditTextBox
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
  public class EditTextBox : TextBox
  {
    public static readonly DependencyProperty CommitCommandProperty;
    public static readonly DependencyProperty RevertCommandProperty;
    public static readonly DependencyProperty LostFocusCommandProperty;
    public static readonly DependencyProperty DoubleClickCommandProperty;
    public static readonly DependencyProperty SelectOnCreationProperty;
    private bool handlingKey;

    public ICommand CommitCommand
    {
      get
      {
        return (ICommand) this.GetValue(EditTextBox.CommitCommandProperty);
      }
      set
      {
        this.SetValue(EditTextBox.CommitCommandProperty, (object) value);
      }
    }

    public ICommand RevertCommand
    {
      get
      {
        return (ICommand) this.GetValue(EditTextBox.RevertCommandProperty);
      }
      set
      {
        this.SetValue(EditTextBox.RevertCommandProperty, (object) value);
      }
    }

    public ICommand LostFocusCommand
    {
      get
      {
        return (ICommand) this.GetValue(EditTextBox.LostFocusCommandProperty);
      }
      set
      {
        this.SetValue(EditTextBox.LostFocusCommandProperty, (object) value);
      }
    }

    public ICommand DoubleClickCommand
    {
      get
      {
        return (ICommand) this.GetValue(EditTextBox.DoubleClickCommandProperty);
      }
      set
      {
        this.SetValue(EditTextBox.DoubleClickCommandProperty, (object) value);
      }
    }

    public bool SelectOnCreation
    {
        get
        {
            return (bool)base.GetValue(EditTextBox.SelectOnCreationProperty);
        }
        set
        {
            base.SetValue(EditTextBox.SelectOnCreationProperty, value);
        }
    }

    static EditTextBox()
    {
      FrameworkPropertyMetadata propertyMetadata1 = new FrameworkPropertyMetadata();
      propertyMetadata1.Inherits = false;
      propertyMetadata1.BindsTwoWayByDefault = false;
      propertyMetadata1.IsNotDataBindable = false;
      propertyMetadata1.DefaultValue = (object) null;
      EditTextBox.CommitCommandProperty = DependencyProperty.Register("CommitCommand", typeof (ICommand), typeof (EditTextBox), (PropertyMetadata) propertyMetadata1);
      FrameworkPropertyMetadata propertyMetadata2 = new FrameworkPropertyMetadata();
      propertyMetadata2.Inherits = false;
      propertyMetadata2.BindsTwoWayByDefault = false;
      propertyMetadata2.IsNotDataBindable = false;
      propertyMetadata2.DefaultValue = (object) null;
      EditTextBox.RevertCommandProperty = DependencyProperty.Register("RevertCommand", typeof (ICommand), typeof (EditTextBox), (PropertyMetadata) propertyMetadata2);
      FrameworkPropertyMetadata propertyMetadata3 = new FrameworkPropertyMetadata();
      propertyMetadata3.Inherits = false;
      propertyMetadata3.BindsTwoWayByDefault = false;
      propertyMetadata3.IsNotDataBindable = false;
      propertyMetadata3.DefaultValue = (object) null;
      EditTextBox.LostFocusCommandProperty = DependencyProperty.Register("LostFocusCommand", typeof (ICommand), typeof (EditTextBox), (PropertyMetadata) propertyMetadata3);
      FrameworkPropertyMetadata propertyMetadata4 = new FrameworkPropertyMetadata();
      propertyMetadata4.Inherits = false;
      propertyMetadata4.BindsTwoWayByDefault = false;
      propertyMetadata4.IsNotDataBindable = false;
      propertyMetadata4.DefaultValue = (object) null;
      EditTextBox.DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof (ICommand), typeof (EditTextBox), (PropertyMetadata) propertyMetadata4);
      FrameworkPropertyMetadata propertyMetadata5 = new FrameworkPropertyMetadata();
      propertyMetadata5.Inherits = false;
      propertyMetadata5.BindsTwoWayByDefault = false;
      propertyMetadata5.IsNotDataBindable = false;
      propertyMetadata5.DefaultValue = (object) false;
      EditTextBox.SelectOnCreationProperty = DependencyProperty.Register("SelectOnCreation", typeof (bool), typeof (EditTextBox), (PropertyMetadata) propertyMetadata5);
    }

    public EditTextBox()
    {
      this.AcceptsReturn = false;
      this.AcceptsTab = false;
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (this.Focusable && !this.IsFocused)
      {
        this.Focus();
        e.Handled = true;
      }
      base.OnPreviewMouseLeftButtonDown(e);
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      this.SelectAll();
      base.OnGotKeyboardFocus(e);
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      if (!this.handlingKey)
      {
        ICommand command = this.LostFocusCommand ?? this.CommitCommand;
        if (command != null)
          command.Execute((object) null);
      }
      base.OnPreviewLostKeyboardFocus(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      this.handlingKey = true;
      if (e.Key == Key.Return || e.Key == Key.Return)
      {
        ICommand commitCommand = this.CommitCommand;
        if (commitCommand != null)
          commitCommand.Execute((object) null);
        this.SelectAll();
        e.Handled = true;
      }
      else if (e.Key == Key.Escape)
      {
        ICommand revertCommand = this.RevertCommand;
        if (revertCommand != null)
          revertCommand.Execute((object) null);
        this.SelectAll();
        e.Handled = true;
      }
      base.OnKeyDown(e);
      this.handlingKey = false;
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      ICommand doubleClickCommand = this.DoubleClickCommand;
      if (doubleClickCommand != null)
        doubleClickCommand.Execute((object) null);
      base.OnMouseDoubleClick(e);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (args.Property.Name == "SelectOnCreation" && this.SelectOnCreation)
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Input, new Action(this.SelectAndFocus));
      base.OnPropertyChanged(args);
    }

    private void SelectAndFocus()
    {
      if (this.Visibility != Visibility.Visible)
        return;
      this.Focus();
      this.SelectAll();
    }
  }
}
