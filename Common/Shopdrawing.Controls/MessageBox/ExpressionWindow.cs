//using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Microsoft.Expression.Framework.Controls
{
    public class ExpressionWindow : Window
    {
        public virtual bool IsOverridingWindowsChrome
        {
            get
            {
                return true;
            }
        }

        public ExpressionWindow()
        {
        }

        private void Close(object sender, EventArgs e)
        {
            base.Close();
        }

        private void DoDragMove(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void ExpressionWindow_SourceInitialized(object sender, EventArgs e)
        {
            //if (base.ResizeMode == ResizeMode.CanResize || base.ResizeMode == ResizeMode.CanResizeWithGrip)
            //{
            //    HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            //    int windowLong = UnsafeNativeMethods.GetWindowLong(hwndSource.Handle, -16);
            //    uint num = -196609;
            //    UnsafeNativeMethods.SetWindowLong(hwndSource.Handle, -16, (int)(windowLong & num));
            //}
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsOverridingWindowsChrome)
            {
                FrameworkElement templateChild = (FrameworkElement)base.GetTemplateChild("Caption");
                if (templateChild != null)
                {
                    templateChild.MouseLeftButtonDown += new MouseButtonEventHandler(this.DoDragMove);
                }
                Button button = (Button)base.GetTemplateChild("Close");
                if (button != null)
                {
                    button.Click += new RoutedEventHandler(this.Close);
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (this.IsOverridingWindowsChrome)
            {
                base.SetResourceReference(FrameworkElement.StyleProperty, "WindowsChromeOverride");
                base.SourceInitialized += new EventHandler(this.ExpressionWindow_SourceInitialized);
            }
            base.OnInitialized(e);
        }
    }
}