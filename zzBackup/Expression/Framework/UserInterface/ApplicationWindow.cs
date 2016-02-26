using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Feedback;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.UserInterface
{
    internal class ApplicationWindow : Window
    {
        private const double MinimumWidth = 400;

        private const double MinimumHeight = 100;

        private const double DefaultWindowWidth = 1024;

        private const double DefaultWindowHeight = 768;

        private const string ConfigurationPropertyLeft = "Left";

        private const string ConfigurationPropertyTop = "Top";

        private const string ConfigurationPropertyWidth = "Width";

        private const string ConfigurationPropertyHeight = "Height";

        private const string ConfigurationPropertyWindowState = "WindowState";

        private readonly static DependencyProperty ThemeObserverBrushProperty;

        public IConfigurationObject Configuration
        {
            get;
            private set;
        }

        public IFeedbackService FeedbackService
        {
            get;
            private set;
        }

        static ApplicationWindow()
        {
            ApplicationWindow.ThemeObserverBrushProperty = DependencyProperty.Register("ThemeObserverBrush", typeof(Brush), typeof(ApplicationWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ApplicationWindow.ThemePropertyChanged)));
            EventManager.RegisterClassHandler(typeof(FloatingWindow), UIElement.KeyDownEvent, new KeyEventHandler(ApplicationWindow.OnWindowKeyDown));
            EventManager.RegisterClassHandler(typeof(AutoHideWindow), UIElement.KeyDownEvent, new KeyEventHandler(ApplicationWindow.OnWindowKeyDown));
            EventManager.RegisterClassHandler(typeof(ViewFrame), UIElement.KeyDownEvent, new KeyEventHandler(ApplicationWindow.OnPaletteKeyDown));
            EventManager.RegisterClassHandler(typeof(TabGroupControl), UIElement.KeyDownEvent, new KeyEventHandler(ApplicationWindow.OnPaletteKeyDown));
        }

        public ApplicationWindow(IConfigurationObject configuration, IFeedbackService feedbackService)
        {
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
            base.Name = "WindowService";
            base.MinWidth = 400;
            base.MinHeight = 100;
            base.Width = 1024;
            base.Height = 768;
            this.Configuration = configuration;
            this.FeedbackService = feedbackService;
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            base.UseLayoutRounding = DpiHelper.DeviceToLogicalUnitsScalingFactorX == 1;
            base.SetResourceReference(ApplicationWindow.ThemeObserverBrushProperty, SystemColors.WindowFrameBrushKey);
            PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ApplicationWindowConstructor);
        }

        private static Window GetOwningWindow(Window child)
        {
            Window rootVisual = null;
            IntPtr owner = (new WindowInteropHelper(child)).Owner;
            if (owner != IntPtr.Zero)
            {
                HwndSource hwndSource = HwndSource.FromHwnd(owner);
                if (hwndSource != null)
                {
                    rootVisual = hwndSource.RootVisual as Window;
                }
            }
            return rootVisual;
        }

        public static void HandleTabRequest(KeyEventArgs e)
        {
            FrameworkElement focusedElement = Keyboard.FocusedElement as FrameworkElement;
            if (focusedElement != null && KeyboardNavigation.GetIsTabStop(focusedElement))
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                    return;
                }
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                {
                    focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    e.Handled = true;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (this.Configuration != null)
                {
                    UnsafeNativeMethods.WINDOWPLACEMENT wINDOWPLACEMENT = new UnsafeNativeMethods.WINDOWPLACEMENT()
                    {
                        Length = Marshal.SizeOf(typeof(UnsafeNativeMethods.WINDOWPLACEMENT))
                    };
                    if (UnsafeNativeMethods.GetWindowPlacement((new WindowInteropHelper(this)).Handle, out wINDOWPLACEMENT))
                    {
                        Point point = WindowHelper.TransformFromDevice(this, new Point((double)wINDOWPLACEMENT.NormalPosition.Left, (double)wINDOWPLACEMENT.NormalPosition.Top));
                        Point point1 = WindowHelper.TransformFromDevice(this, new Point((double)wINDOWPLACEMENT.NormalPosition.Right, (double)wINDOWPLACEMENT.NormalPosition.Bottom));
                        this.Configuration.SetProperty("Height", point1.Y - point.Y);
                        this.Configuration.SetProperty("Width", point1.X - point.X);
                        this.Configuration.SetProperty("Left", point.X);
                        this.Configuration.SetProperty("Top", point.Y);
                        if (wINDOWPLACEMENT.ShowCmd != 3)
                        {
                            this.Configuration.SetProperty("WindowState", WindowState.Normal);
                        }
                        else
                        {
                            this.Configuration.SetProperty("WindowState", WindowState.Maximized);
                        }
                    }
                }
            }
            catch
            {
            }
            base.OnClosing(e);
            if (e.Cancel)
            {
                this.OnClosingCanceled(EventArgs.Empty);
                return;
            }
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.SystemEvents_UserPreferenceChanged);
        }

        private void OnClosingCanceled(EventArgs e)
        {
            if (this.ClosingCanceled != null)
            {
                this.ClosingCanceled(this, e);
            }
        }

        private static void OnPaletteKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && !e.Handled)
            {
                ApplicationWindow.HandleTabRequest(e);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.SetInitialWindowPlacement();
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.SystemEvents_UserPreferenceChanged);
        }

        private static void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && !e.Handled)
            {
                ApplicationWindow.HandleTabRequest(e);
            }
            if (!e.Handled)
            {
                Window window = sender as Window;
                if (window != null)
                {
                    Window owningWindow = ApplicationWindow.GetOwningWindow(window);
                    if (owningWindow != null)
                    {
                        owningWindow.RaiseEvent(e);
                    }
                }
            }
        }

        public void SetInitialWindowPlacement()
        {
            try
            {
                if (this.Configuration == null || !this.Configuration.HasProperty("Left") || !this.Configuration.HasProperty("Width") || !this.Configuration.HasProperty("Top") || !this.Configuration.HasProperty("Height") || !this.Configuration.HasProperty("WindowState"))
                {
                    UnsafeNativeMethods.WINDOWPLACEMENT top = new UnsafeNativeMethods.WINDOWPLACEMENT()
                    {
                        Length = Marshal.SizeOf(typeof(UnsafeNativeMethods.WINDOWPLACEMENT))
                    };
                    if (UnsafeNativeMethods.GetWindowPlacement((new WindowInteropHelper(this)).Handle, out top))
                    {
                        UnsafeNativeMethods.RECT rECT = new UnsafeNativeMethods.RECT();
                        if (UnsafeNativeMethods.SystemParametersInfo(48, 0, ref rECT, 0))
                        {
                            if (top.NormalPosition.Top < rECT.Top)
                            {
                                top.NormalPosition.Top = rECT.Top;
                            }
                            if (top.NormalPosition.Left < rECT.Left)
                            {
                                top.NormalPosition.Left = rECT.Left;
                            }
                            if (top.NormalPosition.Bottom > rECT.Bottom)
                            {
                                top.NormalPosition.Bottom = rECT.Bottom;
                            }
                            if (top.NormalPosition.Right > rECT.Right)
                            {
                                top.NormalPosition.Right = rECT.Right;
                            }
                            if (top.NormalPosition.Left > top.NormalPosition.Right)
                            {
                                top.NormalPosition.Left = rECT.Left;
                            }
                            if (top.NormalPosition.Top > top.NormalPosition.Bottom)
                            {
                                top.NormalPosition.Top = rECT.Top;
                            }
                            top.ShowCmd = 3;
                            this.SetWindowPlacement(ref top);
                        }
                    }
                }
                else
                {
                    double property = (double)this.Configuration.GetProperty("Left");
                    double num = (double)this.Configuration.GetProperty("Top");
                    double property1 = (double)this.Configuration.GetProperty("Width");
                    double num1 = (double)this.Configuration.GetProperty("Height");
                    WindowState windowState = (WindowState)this.Configuration.GetProperty("WindowState");
                    Point device = WindowHelper.TransformToDevice(this, new Point(property, num));
                    Point point = WindowHelper.TransformToDevice(this, new Point(property + property1, num + num1));
                    UnsafeNativeMethods.WINDOWPLACEMENT x = new UnsafeNativeMethods.WINDOWPLACEMENT()
                    {
                        Length = Marshal.SizeOf(typeof(UnsafeNativeMethods.WINDOWPLACEMENT)),
                        Flags = 0
                    };
                    x.MaxPosition.X = -1;
                    x.MaxPosition.Y = -1;
                    x.MinPosition.X = -1;
                    x.MinPosition.Y = -1;
                    x.NormalPosition.Left = (int)device.X;
                    x.NormalPosition.Top = (int)device.Y;
                    x.NormalPosition.Right = (int)point.X;
                    x.NormalPosition.Bottom = (int)point.Y;
                    if (windowState != WindowState.Maximized)
                    {
                        x.ShowCmd = 1;
                    }
                    else
                    {
                        x.ShowCmd = 3;
                    }
                    this.SetWindowPlacement(ref x);
                }
                if (this.FeedbackService != null)
                {
                    this.FeedbackService.SetData(37, (int)base.ActualWidth);
                    this.FeedbackService.SetData(38, (int)base.ActualHeight);
                }
            }
            catch
            {
            }
        }

        private void SetWindowPlacement(ref UnsafeNativeMethods.WINDOWPLACEMENT windowPlacement)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            int showCmd = windowPlacement.ShowCmd;
            windowPlacement.ShowCmd = 0;
            UnsafeNativeMethods.SetWindowPlacement(handle, ref windowPlacement);
            if (!UnsafeNativeMethods.GetWindowPlacement(handle, out windowPlacement))
            {
                base.Left = SystemParameters.WorkArea.X;
                base.Top = SystemParameters.WorkArea.Y;
                base.Width = SystemParameters.WorkArea.Width;
                base.Height = SystemParameters.WorkArea.Height;
                return;
            }
            Point point = WindowHelper.TransformFromDevice(this, new Point((double)windowPlacement.NormalPosition.Left, (double)windowPlacement.NormalPosition.Top));
            Point point1 = WindowHelper.TransformFromDevice(this, new Point((double)windowPlacement.NormalPosition.Right, (double)windowPlacement.NormalPosition.Bottom));
            base.Left = point.X;
            base.Top = point.Y;
            base.Width = point1.X - point.X;
            base.Height = point1.Y - point.Y;
            windowPlacement.ShowCmd = showCmd;
            UnsafeNativeMethods.SetWindowPlacement(handle, ref windowPlacement);
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General || e.Category == UserPreferenceCategory.Desktop)
            {
                Application current = Application.Current;
                if (current != null)
                {
                    foreach (Window window in current.Windows)
                    {
                        WindowHelper.UpdateWindowPlacement(window);
                    }
                }
            }
        }

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationWindow applicationWindow = d as ApplicationWindow;
            if (applicationWindow != null && applicationWindow.ThemeChanged != null)
            {
                applicationWindow.ThemeChanged(applicationWindow, EventArgs.Empty);
            }
        }

        public event EventHandler ClosingCanceled;

        public event EventHandler ThemeChanged;
    }
}