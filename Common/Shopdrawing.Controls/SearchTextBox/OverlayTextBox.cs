using System;
using System.Windows;
using System.Windows.Controls;

namespace Shopdrawing.Controls
{
    public class OverlayTextBox : TextBox
    {
        public readonly static DependencyProperty OverlayTextProperty;

        private readonly static DependencyPropertyKey HasTextPropertyKey;

        public readonly static DependencyProperty HasTextProperty;

        public bool HasText
        {
            get
            {
                return (bool)base.GetValue(OverlayTextBox.HasTextProperty);
            }
        }

        public string OverlayText
        {
            get
            {
                return (string)base.GetValue(OverlayTextBox.OverlayTextProperty);
            }
            set
            {
                base.SetValue(OverlayTextBox.OverlayTextProperty, value);
            }
        }

        static OverlayTextBox()
        {
            OverlayTextBox.OverlayTextProperty = DependencyProperty.Register("OverlayText", typeof(string), typeof(OverlayTextBox), new FrameworkPropertyMetadata(""));
            OverlayTextBox.HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof(bool), typeof(OverlayTextBox), new FrameworkPropertyMetadata(false));
            OverlayTextBox.HasTextProperty = OverlayTextBox.HasTextPropertyKey.DependencyProperty;
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayTextBox), new FrameworkPropertyMetadata(typeof(OverlayTextBox)));
            TextBox.TextProperty.OverrideMetadata(typeof(OverlayTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(OverlayTextBox.OnTextChangedCallback)));
        }

        public OverlayTextBox()
        {
        }

        public static void OnTextChangedCallback(object sender, DependencyPropertyChangedEventArgs args)
        {
            OverlayTextBox overlayTextBox = sender as OverlayTextBox;
            if (overlayTextBox != null)
            {
                bool flag = !string.IsNullOrEmpty(overlayTextBox.Text);
                if (flag != overlayTextBox.HasText)
                {
                    overlayTextBox.SetValue(OverlayTextBox.HasTextPropertyKey, flag);
                }
            }
        }
    }
}