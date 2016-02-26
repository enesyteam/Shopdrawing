using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
    public abstract class ColorEyedropperBase : Control
    {
        public readonly static DependencyProperty ColorProperty;

        public readonly static DependencyProperty BeginEditCommandProperty;

        public readonly static DependencyProperty ContinueEditCommandProperty;

        public readonly static DependencyProperty EndEditCommandProperty;

        public readonly static DependencyProperty CancelEditCommandProperty;

        public readonly static DependencyProperty FinishEditingCommandProperty;

        private Window feedbackWindow;

        private readonly double defaultFeedbackWindowSize = 64;

        private DispatcherTimer moveWindowTimer;

        private UnsafeNativeMethods.Win32Point currentPoint;

        private bool updatingColor;

        public ICommand BeginEditCommand
        {
            get
            {
                return (ICommand)base.GetValue(ColorEyedropperBase.BeginEditCommandProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.BeginEditCommandProperty, value);
            }
        }

        public ICommand CancelEditCommand
        {
            get
            {
                return (ICommand)base.GetValue(ColorEyedropperBase.CancelEditCommandProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.CancelEditCommandProperty, value);
            }
        }

        protected virtual bool CenterFeedbackWindow
        {
            get
            {
                return true;
            }
        }

        public Color Color
        {
            get
            {
                return (Color)base.GetValue(ColorEyedropperBase.ColorProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.ColorProperty, value);
            }
        }

        public ICommand ContinueEditCommand
        {
            get
            {
                return (ICommand)base.GetValue(ColorEyedropperBase.ContinueEditCommandProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.ContinueEditCommandProperty, value);
            }
        }

        public ICommand EndEditCommand
        {
            get
            {
                return (ICommand)base.GetValue(ColorEyedropperBase.EndEditCommandProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.EndEditCommandProperty, value);
            }
        }

        protected Window FeedbackWindow
        {
            get
            {
                return this.feedbackWindow;
            }
        }

        public ICommand FinishEditingCommand
        {
            get
            {
                return (ICommand)base.GetValue(ColorEyedropperBase.FinishEditingCommandProperty);
            }
            set
            {
                base.SetValue(ColorEyedropperBase.FinishEditingCommandProperty, value);
            }
        }

        private DispatcherTimer MoveWindowTimer
        {
            get
            {
                if (this.moveWindowTimer == null)
                {
                    this.moveWindowTimer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromMilliseconds(50)
                    };
                    this.moveWindowTimer.Tick += new EventHandler((object o, EventArgs e) => this.MoveFeedbackWindow());
                }
                return this.moveWindowTimer;
            }
        }

        static ColorEyedropperBase()
        {
            ColorEyedropperBase.ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(ColorEyedropperBase), new FrameworkPropertyMetadata((object)Colors.Transparent));
            ColorEyedropperBase.BeginEditCommandProperty = DependencyProperty.Register("BeginEditCommand", typeof(ICommand), typeof(ColorEyedropperBase), new PropertyMetadata(null));
            ColorEyedropperBase.ContinueEditCommandProperty = DependencyProperty.Register("ContinueEditCommand", typeof(ICommand), typeof(ColorEyedropperBase), new PropertyMetadata(null));
            ColorEyedropperBase.EndEditCommandProperty = DependencyProperty.Register("EndEditCommand", typeof(ICommand), typeof(ColorEyedropperBase), new PropertyMetadata(null));
            ColorEyedropperBase.CancelEditCommandProperty = DependencyProperty.Register("CancelEditCommand", typeof(ICommand), typeof(ColorEyedropperBase), new PropertyMetadata(null));
            ColorEyedropperBase.FinishEditingCommandProperty = DependencyProperty.Register("FinishEditingCommand", typeof(ICommand), typeof(ColorEyedropperBase), new PropertyMetadata(null));
        }

        protected ColorEyedropperBase()
        {
        }

        [CompilerGenerated]
        // <get_MoveWindowTimer>b__0
        private void u003cget_MoveWindowTimeru003eb__0(object o, EventArgs e)
        {
            this.MoveFeedbackWindow();
        }

        protected abstract void CancelEditing();

        protected void CancelUpdateColor()
        {
            this.updatingColor = false;
        }

        protected void CloseFeedbackWindow()
        {
            if (this.feedbackWindow != null)
            {
                this.UnhookWindow(this.feedbackWindow);
                if (Stylus.CurrentStylusDevice != null && Stylus.CurrentStylusDevice.InRange)
                {
                    this.feedbackWindow.Width = this.defaultFeedbackWindowSize;
                    this.feedbackWindow.Height = this.defaultFeedbackWindowSize;
                    this.feedbackWindow.Hide();
                    return;
                }
                this.feedbackWindow.Close();
                this.feedbackWindow = null;
            }
        }

        private void CloseFeedbackWindowAsap()
        {
            if (this.feedbackWindow != null)
            {
                Window window = this.feedbackWindow;
                this.UnhookWindow(this.feedbackWindow);
                this.feedbackWindow.Hide();
                this.feedbackWindow = null;
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => window.Close()));
            }
        }

        private void CoreUpdateColor()
        {
            Color pixelColor = ColorEyedropperBase.GetPixelColor(this.currentPoint.x, this.currentPoint.y);
            if (this.Color != pixelColor)
            {
                this.Color = pixelColor;
                ValueEditorUtils.ExecuteCommand(this.ContinueEditCommand, this, null);
            }
        }

        private void EnsureFeedbackWindowCreated()
        {
            if (this.feedbackWindow == null)
            {
                this.feedbackWindow = new Window()
                {
                    AllowsTransparency = true,
                    Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
                    WindowStyle = WindowStyle.None,
                    ShowInTaskbar = false,
                    Topmost = true,
                    Focusable = false,
                    Width = this.defaultFeedbackWindowSize,
                    Height = this.defaultFeedbackWindowSize
                };
                this.feedbackWindow.SourceInitialized += new EventHandler(this.FeedbackWindow_SourceInitialized);
            }
        }

        private void FeedbackWindow_SourceInitialized(object sender, EventArgs e)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(this.feedbackWindow) as HwndSource;
            if (hwndSource != null && hwndSource.CompositionTarget != null)
            {
                hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static Color GetPixelColor(int x, int y)
        {
            IntPtr intPtr = UnsafeNativeMethods.CreateDC("Display", null, null, IntPtr.Zero);
            int pixel = UnsafeNativeMethods.GetPixel(intPtr, x, y);
            UnsafeNativeMethods.DeleteDC(intPtr);
            Color color = Color.FromArgb(255, (byte)(pixel & 255), (byte)((pixel & 65280) >> 8), (byte)((pixel & 16711680) >> 16));
            return color;
        }

        private void MoveFeedbackWindow()
        {
            if (this.FeedbackWindow != null && this.CenterFeedbackWindow)
            {
                UnsafeNativeMethods.Win32Point cursorPosition = UnsafeNativeMethods.GetCursorPosition();
                double deviceToLogicalUnitsScalingFactorX = (double)cursorPosition.x * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
                double deviceToLogicalUnitsScalingFactorY = (double)cursorPosition.y * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
                this.FeedbackWindow.Left = deviceToLogicalUnitsScalingFactorX - this.FeedbackWindow.Width / 2;
                this.FeedbackWindow.Top = deviceToLogicalUnitsScalingFactorY - this.FeedbackWindow.Height / 2;
            }
        }

        protected void OpenFeedbackWindow()
        {
            try
            {
                this.EnsureFeedbackWindowCreated();
                this.MoveWindowTimer.Start();
                this.feedbackWindow.Show();
            }
            catch (COMException cOMException)
            {
                if (cOMException.ErrorCode == -2003303418)
                {
                    this.CloseFeedbackWindowAsap();
                }
            }
        }

        protected void ShowColorEditorTrack(bool showTrack)
        {
            ColorEditor colorEditor = null;
            for (DependencyObject i = this; i != null && colorEditor == null; i = VisualTreeHelper.GetParent(i))
            {
                colorEditor = i as ColorEditor;
            }
            if (colorEditor != null)
            {
                colorEditor.ShowTrack = showTrack;
            }
        }

        protected void StartUpdateColor(int screenX, int screenY)
        {
            this.currentPoint = new UnsafeNativeMethods.Win32Point(screenX, screenY);
            this.MoveFeedbackWindow();
            this.UpdateColor();
        }

        private void UnhookWindow(Window window)
        {
            this.MoveWindowTimer.Stop();
            window.SourceInitialized -= new EventHandler(this.FeedbackWindow_SourceInitialized);
        }

        private void UpdateColor()
        {
            if (!this.updatingColor)
            {
                this.updatingColor = true;
                using (ColorEyedropperBase.HideWindowToken hideWindowToken = new ColorEyedropperBase.HideWindowToken(this.FeedbackWindow))
                {
                    this.CoreUpdateColor();
                    this.updatingColor = false;
                }
            }
        }

        [CompilerGenerated]
        // <>c__DisplayClass3
        private sealed class u003cu003ec__DisplayClass3
        {
            public Window shouldCloseAsap;

            public u003cu003ec__DisplayClass3()
            {
            }

            // <CloseFeedbackWindowAsap>b__2
            public void u003cCloseFeedbackWindowAsapu003eb__2()
            {
                this.shouldCloseAsap.Close();
            }
        }

        private class HideWindowToken : IDisposable
        {
            private bool disposed;

            private IntPtr hwnd;

            public HideWindowToken(Window window)
            {
                if (window != null && UnsafeNativeMethods.DwmIsCompositionEnabled())
                {
                    HwndSource hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                    if (hwndSource != null)
                    {
                        IntPtr handle = hwndSource.Handle;
                        this.hwnd = hwndSource.Handle;
                        Microsoft.Expression.Framework.UserInterface.UnsafeNativeMethods.ShowWindow(this.hwnd, 0);
                    }
                }
            }

            public void Dispose()
            {
                if (!this.disposed)
                {
                    this.disposed = true;
                    GC.SuppressFinalize(this);
                    Microsoft.Expression.Framework.UserInterface.UnsafeNativeMethods.ShowWindow(this.hwnd, 4);
                }
            }
        }
    }
}