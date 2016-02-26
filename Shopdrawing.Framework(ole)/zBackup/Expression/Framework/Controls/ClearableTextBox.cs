// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ClearableTextBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Controls
{
  public class ClearableTextBox : UserControl
  {
    public static readonly DependencyProperty ClearTextFieldCommandProperty = DependencyProperty.Register("ClearTextFieldCommand", typeof (ICommand), typeof (ClearableTextBox));
    public static readonly DependencyProperty OverlayTextProperty = DependencyProperty.Register("OverlayText", typeof (string), typeof (ClearableTextBox));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (ClearableTextBox));
    public static readonly DependencyProperty FocusOnLoadedProperty = DependencyProperty.Register("FocusOnLoaded", typeof (bool), typeof (ClearableTextBox));
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ClearableTextBox));
    private OverlayTextBox textField;
    private Button clearButton;
    private bool focusedAfterLoaded;

    public ICommand ClearTextFieldCommand
    {
      get
      {
        return this.GetValue(ClearableTextBox.ClearTextFieldCommandProperty) as ICommand;
      }
      set
      {
        this.SetValue(ClearableTextBox.ClearTextFieldCommandProperty, (object) value);
      }
    }

    public string Text
    {
      get
      {
        return this.GetValue(ClearableTextBox.TextProperty) as string;
      }
      set
      {
        this.SetValue(ClearableTextBox.TextProperty, (object) value);
      }
    }

    public string OverlayText
    {
      get
      {
        return this.GetValue(ClearableTextBox.OverlayTextProperty) as string;
      }
      set
      {
        this.SetValue(ClearableTextBox.OverlayTextProperty, (object) value);
      }
    }

    public bool FocusOnLoaded
    {
        get
        {
            return (bool)base.GetValue(ClearableTextBox.FocusOnLoadedProperty);
        }
        set
        {
            base.SetValue(ClearableTextBox.FocusOnLoadedProperty, value);
        }
    }

    private Button ClearButton
    {
      get
      {
        return this.clearButton;
      }
      set
      {
        if (this.clearButton != null)
          this.clearButton.Click -= new RoutedEventHandler(this.ClearButton_Click);
        this.clearButton = value;
        if (this.clearButton == null)
          return;
        this.clearButton.Click += new RoutedEventHandler(this.ClearButton_Click);
      }
    }

    private OverlayTextBox TextField
    {
      get
      {
        return this.textField;
      }
      set
      {
        if (this.textField != null)
        {
          this.textField.KeyDown -= new KeyEventHandler(this.TextField_KeyDown);
          this.textField.TextChanged -= new TextChangedEventHandler(this.OnTextChanged);
        }
        this.textField = value;
        if (this.textField == null)
          return;
        this.textField.KeyDown += new KeyEventHandler(this.TextField_KeyDown);
        this.textField.TextChanged += new TextChangedEventHandler(this.OnTextChanged);
      }
    }

    public event RoutedEventHandler TextChanged
    {
      add
      {
        this.AddHandler(ClearableTextBox.TextChangedEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(ClearableTextBox.TextChangedEvent, (Delegate) value);
      }
    }

    public ClearableTextBox()
    {
      this.Loaded += new RoutedEventHandler(this.ClearableTextBox_Loaded);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      DependencyObject child = VisualTreeHelper.GetChild((DependencyObject) this, 0);
      this.TextField = LogicalTreeHelper.FindLogicalNode(child, "PART_ContentHost") as OverlayTextBox;
      this.ClearButton = LogicalTreeHelper.FindLogicalNode(child, "ClearSearchButton") as Button;
      if (!this.FocusOnLoaded || this.focusedAfterLoaded)
        return;
      UIThreadDispatcher.Instance.BeginInvoke<bool>(DispatcherPriority.Loaded, (Func<bool>) (() => this.TextField.Focus()));
    }

    private void TextField_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Escape || string.IsNullOrEmpty(this.Text))
        return;
      this.Text = (string) null;
      if (this.ClearTextFieldCommand != null)
        this.ClearTextFieldCommand.Execute((object) null);
      e.Handled = true;
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
      e.Handled = true;
      this.RaiseEvent(new RoutedEventArgs(ClearableTextBox.TextChangedEvent));
    }

    private void ClearableTextBox_Loaded(object sender, RoutedEventArgs e)
    {
      if (!this.FocusOnLoaded || this.TextField == null)
        return;
      this.TextField.Focus();
      this.focusedAfterLoaded = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
      this.Text = (string) null;
    }
  }
}
