using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Expression.Framework.UserInterface
{
    public static class TextBoxHelper
    {
        public readonly static DependencyProperty PreventSelectAllOnFocusProperty;

        static TextBoxHelper()
        {
            TextBoxHelper.PreventSelectAllOnFocusProperty = DependencyProperty.RegisterAttached("PreventSelectAllOnFocus", typeof(bool), typeof(TextBoxHelper));
        }

        public static bool GetPreventSelectAllOnFocus(DependencyObject target)
        {
            return (bool)target.GetValue(TextBoxHelper.PreventSelectAllOnFocusProperty);
        }

        public static void RegisterType(Type type)
        {
            EventManager.RegisterClassHandler(type, UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(TextBoxHelper.TextBox_PreviewMouseDown));
            EventManager.RegisterClassHandler(type, UIElement.GotFocusEvent, new RoutedEventHandler(TextBoxHelper.TextBox_GotFocus));
        }

        public static void SetPreventSelectAllOnFocus(DependencyObject target, bool value)
        {
            target.SetValue(TextBoxHelper.PreventSelectAllOnFocusProperty, value);
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.TextWrapping == TextWrapping.NoWrap && !TextBoxHelper.GetPreventSelectAllOnFocus(textBox))
            {
                textBox.SelectAll();
            }
        }

        private static void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Focusable && !textBox.IsFocused && textBox.TextWrapping == TextWrapping.NoWrap && !TextBoxHelper.GetPreventSelectAllOnFocus(textBox))
            {
                textBox.Focus();
                e.Handled = true;
            }
        }
    }
}