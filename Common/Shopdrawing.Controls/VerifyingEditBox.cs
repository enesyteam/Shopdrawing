using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Shopdrawing.Controls
{
    public class VerifyingEditBox : TextBox
    {
        public readonly static DependencyProperty ValueProperty;

        private bool isValueDirty;

        private string valueCache;

        private object LocalValue
        {
            get
            {
                return base.ReadLocalValue(VerifyingEditBox.ValueProperty);
            }
        }

        public string Value
        {
            get
            {
                return (string)base.GetValue(VerifyingEditBox.ValueProperty);
            }
            set
            {
                base.SetValue(VerifyingEditBox.ValueProperty, value);
            }
        }

        static VerifyingEditBox()
        {
            VerifyingEditBox.ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(string), typeof(VerifyingEditBox));
            VerifyingEditBox.ValueProperty.OverrideMetadata(typeof(VerifyingEditBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(VerifyingEditBox.OnValueInvalidated), null));
        }

        public VerifyingEditBox()
        {
            base.AcceptsReturn = false;
            base.AcceptsTab = false;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.SelectAll();
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.Return || e.Key == Key.Return) && this.isValueDirty)
            {
                this.SetValueFromText();
                base.SelectAll();
            }
            else if (e.Key == Key.Escape)
            {
                this.SetTextFromValue();
                base.SelectAll();
            }
            base.OnKeyDown(e);
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (this.isValueDirty)
            {
                this.SetValueFromText();
            }
            base.OnPreviewLostKeyboardFocus(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (base.Focusable && !base.IsFocused)
            {
                base.Focus();
                e.Handled = true;
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.isValueDirty = true;
            base.OnTextChanged(e);
        }

        private static void OnValueInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VerifyingEditBox value = (VerifyingEditBox)d;
            if (value.valueCache != value.Value)
            {
                value.SetTextFromValue();
                value.valueCache = value.Value;
            }
        }

        private void SetTextFromValue()
        {
            base.Text = this.Value;
            this.isValueDirty = false;
        }

        private void SetValueFromText()
        {
            this.Value = base.Text;
            BindingExpression bindingExpression = base.GetBindingExpression(VerifyingEditBox.ValueProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
            this.SetTextFromValue();
        }
    }
}