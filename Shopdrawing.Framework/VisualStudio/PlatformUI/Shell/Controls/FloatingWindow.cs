using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class FloatingWindow : FloatingElement, IResizable
    {
        public readonly static RoutedEvent IsFloatingWindowDragWithinChangedEvent;

        public readonly static DependencyProperty IsFloatingWindowDragWithinProperty;

        public readonly static RoutedEvent LocationChangedEvent;

        public readonly static DependencyProperty CornerRadiusProperty;

        public readonly static DependencyProperty HasMaximizeButtonProperty;

        public readonly static DependencyProperty HasMinimizeButtonProperty;

        public readonly static DependencyProperty IsFloatingProperty;

        public readonly static DependencyProperty HasMultipleOnScreenViewsProperty;

        public int CornerRadius
        {
            get
            {
                return (int)base.GetValue(FloatingWindow.CornerRadiusProperty);
            }
            set
            {
                base.SetValue(FloatingWindow.CornerRadiusProperty, value);
            }
        }

        public bool HasMaximizeButton
        {
            get
            {
                return (bool)base.GetValue(FloatingWindow.HasMaximizeButtonProperty);
            }
            set
            {
                base.SetValue(FloatingWindow.HasMaximizeButtonProperty, value);
            }
        }

        public bool HasMinimizeButton
        {
            get
            {
                return (bool)base.GetValue(FloatingWindow.HasMinimizeButtonProperty);
            }
            set
            {
                base.SetValue(FloatingWindow.HasMinimizeButtonProperty, value);
            }
        }

        static FloatingWindow()
        {
            FloatingWindow.IsFloatingWindowDragWithinChangedEvent = EventManager.RegisterRoutedEvent("IsFloatingWindowDragWithinChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FloatingWindow));
            FloatingWindow.IsFloatingWindowDragWithinProperty = DependencyProperty.RegisterAttached("IsFloatingWindowDragWithin", typeof(bool), typeof(FloatingWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FloatingWindow.OnIsFloatingWindowDragWithinChanged)));
            FloatingWindow.LocationChangedEvent = EventManager.RegisterRoutedEvent("LocationChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FloatingWindow));
            FloatingWindow.CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(int), typeof(FloatingWindow), new FrameworkPropertyMetadata((object)0, new PropertyChangedCallback(FloatingWindow.OnCornerRadiusChanged)));
            FloatingWindow.HasMaximizeButtonProperty = DependencyProperty.Register("HasMaximizeButton", typeof(bool), typeof(FloatingWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FloatingWindow.OnWindowStyleChanged)));
            FloatingWindow.HasMinimizeButtonProperty = DependencyProperty.Register("HasMinimizeButton", typeof(bool), typeof(FloatingWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FloatingWindow.OnWindowStyleChanged)));
            FloatingWindow.IsFloatingProperty = DependencyProperty.RegisterAttached("IsFloating", typeof(bool), typeof(FloatingWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
            FloatingWindow.HasMultipleOnScreenViewsProperty = DependencyProperty.RegisterAttached("HasMultipleOnScreenViews", typeof(bool), typeof(FloatingWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(typeof(FloatingWindow)));
            Window.LeftProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(FloatingWindow.OnLeftOrTopChanged)));
            Window.TopProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(new PropertyChangedCallback(FloatingWindow.OnLeftOrTopChanged)));
        }

        public FloatingWindow()
        {
            base.WindowStyle = WindowStyle.None;
            base.ResizeMode = ResizeMode.NoResize;
            FloatingWindow.SetIsFloating(this, true);
            AutomationProperties.SetName(this, "FloatingWindow");
            DependencyProperty leftProperty = Window.LeftProperty;
            Binding binding = new Binding()
            {
                Path = new PropertyPath("FloatingLeft", new object[0]),
                Mode = BindingMode.TwoWay
            };
            base.SetBinding(leftProperty, binding);
            DependencyProperty topProperty = Window.TopProperty;
            Binding binding1 = new Binding()
            {
                Path = new PropertyPath("FloatingTop", new object[0]),
                Mode = BindingMode.TwoWay
            };
            base.SetBinding(topProperty, binding1);
            DependencyProperty widthProperty = FrameworkElement.WidthProperty;
            Binding binding2 = new Binding()
            {
                Path = new PropertyPath("FloatingWidth", new object[0]),
                Mode = BindingMode.TwoWay
            };
            base.SetBinding(widthProperty, binding2);
            DependencyProperty heightProperty = FrameworkElement.HeightProperty;
            Binding binding3 = new Binding()
            {
                Path = new PropertyPath("FloatingHeight", new object[0]),
                Mode = BindingMode.TwoWay
            };
            base.SetBinding(heightProperty, binding3);
            base.SizeChanged += new SizeChangedEventHandler((object argument0, SizeChangedEventArgs argument1) => this.UpdateCornerRadius());
        }

        [CompilerGenerated]
        // <.ctor>b__4
        private void u003cu002ectoru003eb__4(object obj, SizeChangedEventArgs sizeChangedEventArg)
        {
            this.UpdateCornerRadius();
        }

        public static bool GetHasMultipleOnScreenViews(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(FloatingWindow.HasMultipleOnScreenViewsProperty);
        }

        public static bool GetIsFloating(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(FloatingWindow.IsFloatingProperty);
        }

        private IntPtr HwndSourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 274 && wParam.ToInt32() == 61536)
            {
                if (lParam.ToInt32() == 0)
                {
                    IntPtr owner = (new WindowInteropHelper(this)).Owner;
                    if (owner != IntPtr.Zero)
                    {
                        NativeMethods.SendMessage(owner, msg, wParam, lParam);
                        handled = true;
                        return IntPtr.Zero;
                    }
                }
            }
            else if (msg == 36)
            {
                FloatingWindow.WmGetMinMaxInfo(hWnd, lParam);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((FloatingWindow)obj).UpdateCornerRadius();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FloatingWindowAutomationPeer(this);
        }

        private static void OnIsFloatingWindowDragWithinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement uIElement = obj as UIElement;
            if (uIElement != null)
            {
                uIElement.RaiseEvent(new RoutedEventArgs(FloatingWindow.IsFloatingWindowDragWithinChangedEvent));
            }
        }

        private static void OnLeftOrTopChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((FloatingWindow)obj).RaiseEvent(new RoutedEventArgs(FloatingWindow.LocationChangedEvent));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            ((IKeyboardInputSink)hwndSource).RegisterKeyboardInputSink(new FloatingWindow.MnemonicForwardingKeyboardInputSink(this));
            hwndSource.AddHook(new HwndSourceHook(this.HwndSourceHook));
            this.UpdateCornerRadius();
            base.OnSourceInitialized(e);
        }

        private static void OnWindowStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FloatingWindow floatingWindow = (FloatingWindow)obj;
            HwndSource hwndSource = PresentationSource.FromVisual(floatingWindow) as HwndSource;
            if (hwndSource != null)
            {
                floatingWindow.UpdateWindowStyle(hwndSource.Handle);
            }
        }

        public static void SetHasMultipleOnScreenViews(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(FloatingWindow.HasMultipleOnScreenViewsProperty, value);
        }

        public static void SetIsFloating(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(FloatingWindow.IsFloatingProperty, value);
        }

        internal static void SetIsFloatingWindowDragWithin(IEnumerable<DependencyObject> values, bool value)
        {
            foreach (DependencyObject dependencyObject in values)
            {
                dependencyObject.SetValue(FloatingWindow.IsFloatingWindowDragWithinProperty, value);
            }
        }

        public void UpdateBounds(double leftDelta, double topDelta, double widthDelta, double heightDelta)
        {
            Size size = new Size((base.MinWidth.IsNonreal() ? 0 : base.MinWidth), (base.MinHeight.IsNonreal() ? 0 : base.MinHeight));
            Size size1 = new Size((base.MaxWidth.IsNonreal() ? double.MaxValue : base.MaxWidth), (base.MaxHeight.IsNonreal() ? double.MaxValue : base.MaxHeight));
            Rect rect = new Rect(base.Left, base.Top, base.Width, base.Height);
            Rect rect1 = rect.Resize(new Vector(leftDelta, topDelta), new Vector(widthDelta, heightDelta), size, size1);
            base.Left = rect1.Left;
            base.Top = rect1.Top;
            base.Width = rect1.Width;
            base.Height = rect1.Height;
        }

        private void UpdateCornerRadius()
        {
            RECT rECT;
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource != null)
            {
                if (this.CornerRadius == 0)
                {
                    NativeMethods.SetWindowRgn(hwndSource.Handle, IntPtr.Zero, true);
                    return;
                }
                NativeMethods.GetWindowRect(hwndSource.Handle, out rECT);
                int cornerRadius = 2 * this.CornerRadius;
                IntPtr intPtr = NativeMethods.CreateRoundRectRgn(0, 0, rECT.Width + 1, rECT.Height + 1, cornerRadius, cornerRadius);
                NativeMethods.SetWindowRgn(hwndSource.Handle, intPtr, true);
            }
        }

        private void UpdateWindowStyle(IntPtr hwnd)
        {
            int windowLong = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL.STYLE);
            windowLong = (!this.HasMaximizeButton ? windowLong & -65537 : windowLong | 65536);
            windowLong = (!this.HasMinimizeButton ? windowLong & -131073 : windowLong | 131072);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL.STYLE, windowLong);
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 2);
            if (intPtr != IntPtr.Zero)
            {
                MONITORINFO mONITORINFO = new MONITORINFO()
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO))
                };
                NativeMethods.GetMonitorInfo(intPtr, ref mONITORINFO);
                RECT rECT = mONITORINFO.rcWork;
                RECT rECT1 = mONITORINFO.rcMonitor;
                structure.ptMaxPosition.x = Math.Abs(rECT.Left - rECT1.Left);
                structure.ptMaxPosition.y = Math.Abs(rECT.Top - rECT1.Top);
                structure.ptMaxSize.x = Math.Abs(rECT.Right - rECT.Left);
                structure.ptMaxSize.y = Math.Abs(rECT.Bottom - rECT.Top);
            }
            Marshal.StructureToPtr(structure, lParam, true);
        }

        private class MnemonicForwardingKeyboardInputSink : UIElement, IKeyboardInputSink
        {
            IKeyboardInputSite System.Windows.Interop.IKeyboardInputSink.KeyboardInputSite
            {
                get;
                set;
            }

            private Window Window
            {
                get;
                set;
            }

            public MnemonicForwardingKeyboardInputSink(Window window)
            {
                this.Window = window;
            }

            bool System.Windows.Interop.IKeyboardInputSink.HasFocusWithin()
            {
                return false;
            }

            bool System.Windows.Interop.IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers)
            {
                switch (msg.message)
                {
                    case 262:
                    case 263:
                        {
                            string str = new string((char)((int)msg.wParam), 1);
                            if (str == null || str.Length <= 0)
                            {
                                break;
                            }
                            IntPtr owner = (new WindowInteropHelper(this.Window)).Owner;
                            HwndSource hwndSource = HwndSource.FromHwnd(owner);
                            if (hwndSource == null || !AccessKeyManager.IsKeyRegistered(hwndSource, str))
                            {
                                break;
                            }
                            AccessKeyManager.ProcessKey(hwndSource, str, false);
                            return true;
                        }
                }
                return false;
            }

            IKeyboardInputSite System.Windows.Interop.IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink)
            {
                throw new NotSupportedException();
            }

            bool System.Windows.Interop.IKeyboardInputSink.TabInto(TraversalRequest request)
            {
                return false;
            }

            bool System.Windows.Interop.IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
            {
                return false;
            }

            bool System.Windows.Interop.IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
            {
                return false;
            }
        }
    }
}