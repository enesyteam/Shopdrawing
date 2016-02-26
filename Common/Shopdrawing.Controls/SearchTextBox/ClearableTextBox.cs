using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Shopdrawing.Controls
{
    public class ClearableTextBox : UserControl
    {
        public readonly static DependencyProperty ClearTextFieldCommandProperty;

        public readonly static DependencyProperty OverlayTextProperty;

        public readonly static DependencyProperty TextProperty;

        public readonly static DependencyProperty FocusOnLoadedProperty;

        public readonly static RoutedEvent TextChangedEvent;

        private OverlayTextBox textField;

        private Button clearButton;

        private bool focusedAfterLoaded;

        private Button ClearButton
        {
            get
            {
                return this.clearButton;
            }
            set
            {
                if (this.clearButton != null)
                {
                    this.clearButton.Click -= new RoutedEventHandler(this.ClearButton_Click);
                }
                this.clearButton = value;
                if (this.clearButton != null)
                {
                    this.clearButton.Click += new RoutedEventHandler(this.ClearButton_Click);
                }
            }
        }

        public ICommand ClearTextFieldCommand
        {
            get
            {
                return base.GetValue(ClearableTextBox.ClearTextFieldCommandProperty) as ICommand;
            }
            set
            {
                base.SetValue(ClearableTextBox.ClearTextFieldCommandProperty, value);
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

        public string OverlayText
        {
            get
            {
                return base.GetValue(ClearableTextBox.OverlayTextProperty) as string;
            }
            set
            {
                base.SetValue(ClearableTextBox.OverlayTextProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return base.GetValue(ClearableTextBox.TextProperty) as string;
            }
            set
            {
                base.SetValue(ClearableTextBox.TextProperty, value);
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
                if (this.textField != null)
                {
                    this.textField.KeyDown += new KeyEventHandler(this.TextField_KeyDown);
                    this.textField.TextChanged += new TextChangedEventHandler(this.OnTextChanged);
                }
            }
        }

        static ClearableTextBox()
        {
            ClearableTextBox.ClearTextFieldCommandProperty = DependencyProperty.Register("ClearTextFieldCommand", typeof(ICommand), typeof(ClearableTextBox));
            ClearableTextBox.OverlayTextProperty = DependencyProperty.Register("OverlayText", typeof(string), typeof(ClearableTextBox));
            ClearableTextBox.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ClearableTextBox));
            ClearableTextBox.FocusOnLoadedProperty = DependencyProperty.Register("FocusOnLoaded", typeof(bool), typeof(ClearableTextBox));
            ClearableTextBox.TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ClearableTextBox));
        }

        public ClearableTextBox()
        {
            base.Loaded += new RoutedEventHandler(this.ClearableTextBox_Loaded);
        }

        private void ClearableTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.FocusOnLoaded && this.TextField != null)
            {
                this.TextField.Focus();
                this.focusedAfterLoaded = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Text = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DependencyObject child = VisualTreeHelper.GetChild(this, 0);
            this.TextField = LogicalTreeHelper.FindLogicalNode(child, "PART_ContentHost") as OverlayTextBox;
            this.ClearButton = LogicalTreeHelper.FindLogicalNode(child, "ClearSearchButton") as Button;
            if (this.FocusOnLoaded && !this.focusedAfterLoaded)
            {
                UIThreadDispatcher.Instance.BeginInvoke<bool>(DispatcherPriority.Loaded, () => this.TextField.Focus());
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            base.RaiseEvent(new RoutedEventArgs(ClearableTextBox.TextChangedEvent));
        }

        private void TextField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && !string.IsNullOrEmpty(this.Text))
            {
                this.Text = null;
                if (this.ClearTextFieldCommand != null)
                {
                    this.ClearTextFieldCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        public event RoutedEventHandler TextChanged
        {
            add
            {
                base.AddHandler(ClearableTextBox.TextChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(ClearableTextBox.TextChangedEvent, value);
            }
        }
    }
}