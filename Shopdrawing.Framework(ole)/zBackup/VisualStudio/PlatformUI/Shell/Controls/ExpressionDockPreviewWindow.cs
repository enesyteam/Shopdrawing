using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class ExpressionDockPreviewWindow : Window, IDockPreviewWindow
    {
        public readonly static DependencyProperty IsFillPreviewProperty;

        public bool IsFillPreview
        {
            get
            {
                return (bool)base.GetValue(ExpressionDockPreviewWindow.IsFillPreviewProperty);
            }
            set
            {
                base.SetValue(ExpressionDockPreviewWindow.IsFillPreviewProperty, value);
            }
        }

        static ExpressionDockPreviewWindow()
        {
            ExpressionDockPreviewWindow.IsFillPreviewProperty = DependencyProperty.Register("IsFillPreview", typeof(bool), typeof(ExpressionDockPreviewWindow), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpressionDockPreviewWindow), new FrameworkPropertyMetadata(typeof(ExpressionDockPreviewWindow)));
        }

        public ExpressionDockPreviewWindow()
        {
            base.AllowsTransparency = true;
            base.ResizeMode = ResizeMode.NoResize;
            base.WindowStyle = WindowStyle.None;
            base.ShowInTaskbar = false;
            base.ShowActivated = false;
            base.Topmost = true;
            base.Focusable = false;
        }

        void Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.Close()
        {
            base.Close();
        }

        //double Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.get_Left()
        //{
        //    return base.Left;
        //}

        //double Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.get_Top()
        //{
        //    return base.Top;
        //}

        //void Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.Hide()
        //{
        //    base.Hide();
        //}

        //void Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.set_Left(double num)
        //{
        //    base.Left = num;
        //}

        //void Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.set_Top(double num)
        //{
        //    base.Top = num;
        //}

        void Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow.Show()
        {
            base.Show();
        }
    }
}