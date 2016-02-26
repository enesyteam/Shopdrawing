// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.VerifyingEditBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Controls
{
  public class VerifyingEditBox : TextBox
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof (string), typeof (VerifyingEditBox));
    private bool isValueDirty;
    private string valueCache;

    public string Value
    {
      get
      {
        return (string) this.GetValue(VerifyingEditBox.ValueProperty);
      }
      set
      {
        this.SetValue(VerifyingEditBox.ValueProperty, (object) value);
      }
    }

    private object LocalValue
    {
      get
      {
        return this.ReadLocalValue(VerifyingEditBox.ValueProperty);
      }
    }

    static VerifyingEditBox()
    {
      VerifyingEditBox.ValueProperty.OverrideMetadata(typeof (VerifyingEditBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(VerifyingEditBox.OnValueInvalidated), (CoerceValueCallback) null));
    }

    public VerifyingEditBox()
    {
      this.AcceptsReturn = false;
      this.AcceptsTab = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if ((e.Key == Key.Return || e.Key == Key.Return) && this.isValueDirty)
      {
        this.SetValueFromText();
        this.SelectAll();
      }
      else if (e.Key == Key.Escape)
      {
        this.SetTextFromValue();
        this.SelectAll();
      }
      base.OnKeyDown(e);
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
      if (this.isValueDirty)
        this.SetValueFromText();
      base.OnPreviewLostKeyboardFocus(e);
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
      this.isValueDirty = true;
      base.OnTextChanged(e);
    }

    private void SetTextFromValue()
    {
      this.Text = this.Value;
      this.isValueDirty = false;
    }

    private void SetValueFromText()
    {
      this.Value = this.Text;
      BindingExpression bindingExpression = this.GetBindingExpression(VerifyingEditBox.ValueProperty);
      if (bindingExpression != null)
        bindingExpression.UpdateTarget();
      this.SetTextFromValue();
    }

    private static void OnValueInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      VerifyingEditBox verifyingEditBox = (VerifyingEditBox) d;
      if (!(verifyingEditBox.valueCache != verifyingEditBox.Value))
        return;
      verifyingEditBox.SetTextFromValue();
      verifyingEditBox.valueCache = verifyingEditBox.Value;
    }
  }
}
