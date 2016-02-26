// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.EditableComboBox
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.Controls
{
  public class EditableComboBox : ComboBox
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof (object), typeof (EditableComboBox));
    private bool isOldValueValid;
    private string oldValue;

    public virtual string Value
    {
      get
      {
        return (string) this.GetValue(EditableComboBox.ValueProperty);
      }
      set
      {
        this.SetValue(EditableComboBox.ValueProperty, (object) value);
      }
    }

    public event DependencyPropertyChangedEventHandler ValueChanged;

    static EditableComboBox()
    {
      EditableComboBox.ValueProperty.OverrideMetadata(typeof (EditableComboBox), new PropertyMetadata((object) "", new PropertyChangedCallback(EditableComboBox.OnValueInvalidated), (CoerceValueCallback) null));
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);
      if (e.Key == Key.Return || e.Key == Key.Return)
      {
        this.UpdateValueWithTextField();
      }
      else
      {
        if (e.Key != Key.Escape)
          return;
        this.Text = this.Value;
      }
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnPreviewLostKeyboardFocus(e);
      this.UpdateValueWithTextField();
    }

    private void UpdateValueWithTextField()
    {
      string str = this.Value;
      this.SetValue(this.Text);
      if (!(this.Value == str))
        return;
      this.Text = str;
    }

    private static void OnValueInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      EditableComboBox editableComboBox = (EditableComboBox) d;
      string str = editableComboBox.Value;
      if (!editableComboBox.isOldValueValid || editableComboBox.oldValue != str)
        editableComboBox.OnValueChanged((object) editableComboBox.oldValue, (object) str);
      editableComboBox.oldValue = str;
      editableComboBox.isOldValueValid = true;
    }

    protected virtual void OnValueChanged(object oldValue, object newValue)
    {
      this.Text = (string) newValue;
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, new DependencyPropertyChangedEventArgs(EditableComboBox.ValueProperty, oldValue, newValue));
    }

    public void SetValue(string stringValue)
    {
      if (this.Value != stringValue)
      {
        this.Value = stringValue;
        BindingExpression bindingExpression = BindingOperations.GetBindingExpression((DependencyObject) this, EditableComboBox.ValueProperty);
        if (bindingExpression != null)
          bindingExpression.UpdateTarget();
      }
      this.Text = stringValue;
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Right)
        e.Handled = true;
      base.OnPreviewMouseDown(e);
    }

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Right)
        e.Handled = true;
      base.OnPreviewMouseUp(e);
    }
  }
}
