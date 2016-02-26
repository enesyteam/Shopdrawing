using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
    public class EditableComboBox : ComboBox
    {
        public readonly static DependencyProperty ValueProperty;

        private bool isOldValueValid;

        private string oldValue;

        public virtual string Value
        {
            get
            {
                return (string)base.GetValue(EditableComboBox.ValueProperty);
            }
            set
            {
                base.SetValue(EditableComboBox.ValueProperty, value);
            }
        }

        static EditableComboBox()
        {
            EditableComboBox.ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EditableComboBox));
            EditableComboBox.ValueProperty.OverrideMetadata(typeof(EditableComboBox), new PropertyMetadata("", new PropertyChangedCallback(EditableComboBox.OnValueInvalidated), null));
        }

        public EditableComboBox()
        {
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Return || e.Key == Key.Return)
            {
                this.UpdateValueWithTextField();
                return;
            }
            if (e.Key == Key.Escape)
            {
                base.Text = this.Value;
            }
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewLostKeyboardFocus(e);
            this.UpdateValueWithTextField();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                e.Handled = true;
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                e.Handled = true;
            }
            base.OnPreviewMouseUp(e);
        }

        protected virtual void OnValueChanged(object oldValue, object newValue)
        {
            base.Text = (string)newValue;
            if (this.ValueChanged != null)
            {
                DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArg = new DependencyPropertyChangedEventArgs(EditableComboBox.ValueProperty, oldValue, newValue);
                this.ValueChanged(this, dependencyPropertyChangedEventArg);
            }
        }

        private static void OnValueInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableComboBox editableComboBox = (EditableComboBox)d;
            string value = editableComboBox.Value;
            if (!editableComboBox.isOldValueValid || editableComboBox.oldValue != value)
            {
                editableComboBox.OnValueChanged(editableComboBox.oldValue, value);
            }
            editableComboBox.oldValue = value;
            editableComboBox.isOldValueValid = true;
        }

        public void SetValue(string value)
        {
            if (this.Value != value)
            {
                this.Value = value;
                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(this, EditableComboBox.ValueProperty);
                if (bindingExpression != null)
                {
                    bindingExpression.UpdateTarget();
                }
            }
            base.Text = value;
        }

        private void UpdateValueWithTextField()
        {
            string value = this.Value;
            this.SetValue(base.Text);
            if (this.Value == value)
            {
                base.Text = value;
            }
        }

        public event DependencyPropertyChangedEventHandler ValueChanged;
    }
}