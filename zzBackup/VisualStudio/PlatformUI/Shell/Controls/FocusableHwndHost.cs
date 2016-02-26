using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public abstract class FocusableHwndHost : HwndHost, IKeyboardInputSink
    {
        private IDisposable _hwndSourceTracker;

        private HwndSource _hwndSource;

        private bool _takingWpfFocus;

        private bool _takingWin32Focus;

        private DispatcherOperation _pendingSetFocusOperation;

        private bool CanFocus
        {
            get
            {
                IntPtr handle = base.Handle;
                bool flag = NativeMethods.IsWindowVisible(base.Handle);
                bool flag1 = NativeMethods.IsWindowEnabled(base.Handle);
                if (flag)
                {
                    return flag1;
                }
                return false;
            }
        }

        internal IntPtr LastFocusedHwnd
        {
            get;
            private set;
        }

        protected FocusableHwndHost()
        {
            FocusTracker.Instance.TrackFocus += new EventHandler<TrackFocusEventArgs>(this.OnTrackFocus);
            PresentationSource.AddSourceChangedHandler(this, new SourceChangedEventHandler(this.OnSourceChanged));
        }

        [CompilerGenerated]
        // <SchedulePendingSetFocusOperation>b__0
        private object u003cSchedulePendingSetFocusOperationu003eb__0(object unused)
        {
            DispatcherOperation dispatcherOperation = null;
            if (Keyboard.FocusedElement == this)
            {
                if (!this.CanFocus)
                {
                    dispatcherOperation = this.SchedulePendingSetFocusOperation(DispatcherPriority.Input);
                }
                else
                {
                    this._takingWin32Focus = true;
                    NativeMethods.SetFocus(base.Handle);
                    this._takingWin32Focus = false;
                }
            }
            this._pendingSetFocusOperation = dispatcherOperation;
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FocusTracker.Instance.TrackFocus -= new EventHandler<TrackFocusEventArgs>(this.OnTrackFocus);
                PresentationSource.RemoveSourceChangedHandler(this, new SourceChangedEventHandler(this.OnSourceChanged));
                this.StopHwndSourceTracking();
            }
            base.Dispose(disposing);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (base.IsArrangeValid)
            {
                Rect rect = new Rect(0, 0, base.ActualWidth, base.ActualHeight);
                if (rect.Contains(hitTestParameters.HitPoint))
                {
                    return new PointHitTestResult(this, hitTestParameters.HitPoint);
                }
            }
            return base.HitTestCore(hitTestParameters);
        }

        private bool IsSelfOrDescendentWindow(IntPtr window)
        {
            if (!(base.Handle != IntPtr.Zero) || !(window != IntPtr.Zero))
            {
                return false;
            }
            if (base.Handle == window)
            {
                return true;
            }
            return NativeMethods.IsChild(base.Handle, window);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (!e.Handled && e.NewFocus == this)
            {
                IntPtr handle = base.Handle;
                if (!this._takingWpfFocus)
                {
                    if (this.CanFocus)
                    {
                        this._takingWin32Focus = true;
                        NativeMethods.SetFocus(base.Handle);
                        this._takingWin32Focus = false;
                    }
                    else if (this._pendingSetFocusOperation == null)
                    {
                        this._pendingSetFocusOperation = this.SchedulePendingSetFocusOperation(DispatcherPriority.Loaded);
                    }
                    e.Handled = true;
                }
            }
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            this.UnregisterSourceKeyboardInputSink();
            PresentationSource newSource = e.NewSource;
            this.RegisterSourceKeyboardInputSink(newSource as IKeyboardInputSink);
            this._hwndSource = newSource as HwndSource;
            this.StartHwndSourceTracking();
        }

        private void OnTrackFocus(object sender, TrackFocusEventArgs e)
        {
            if (this.IsSelfOrDescendentWindow(e.HwndGainFocus))
            {
                if (!this._takingWin32Focus && Keyboard.FocusedElement != this && !this.IsSelfOrDescendentWindow(e.HwndLoseFocus))
                {
                    this.SetFocusToHwndHost(false, e.HwndGainFocus);
                    return;
                }
            }
            else if (e.HwndGainFocus != IntPtr.Zero && this.IsSelfOrDescendentWindow(e.HwndLoseFocus) && !this._takingWpfFocus && this._hwndSource != null && e.HwndGainFocus == this._hwndSource.Handle && this._hwndSource.RootVisual != null)
            {
                FocusManager.SetFocusedElement(this._hwndSource.RootVisual, null);
            }
        }

        protected virtual void RegisterSourceKeyboardInputSink(IKeyboardInputSink sourceKeyboardInputSink)
        {
            IKeyboardInputSink keyboardInputSink = this;
            if (sourceKeyboardInputSink != null)
            {
                keyboardInputSink.KeyboardInputSite = sourceKeyboardInputSink.RegisterKeyboardInputSink(this);
            }
        }

        private DispatcherOperation SchedulePendingSetFocusOperation(DispatcherPriority priority)
        {
            DispatcherOperation dispatcherOperation1 = base.Dispatcher.BeginInvoke(priority, new DispatcherOperationCallback((object unused) =>
            {
                DispatcherOperation dispatcherOperation = null;
                if (Keyboard.FocusedElement == this)
                {
                    if (!this.CanFocus)
                    {
                        dispatcherOperation = this.SchedulePendingSetFocusOperation(DispatcherPriority.Input);
                    }
                    else
                    {
                        this._takingWin32Focus = true;
                        NativeMethods.SetFocus(base.Handle);
                        this._takingWin32Focus = false;
                    }
                }
                this._pendingSetFocusOperation = dispatcherOperation;
                return null;
            }), null);
            return dispatcherOperation1;
        }

        private void SetFocusToHwndHost(bool allowFocusToDelegateToHostedWindow, IntPtr hwndGainFocus)
        {
            if (this._hwndSource != null && this._hwndSource.RootVisual != null && NativeMethods.GetFocus() != this._hwndSource.Handle)
            {
                FocusManager.SetFocusedElement(this._hwndSource.RootVisual, null);
            }
            this._takingWpfFocus = !allowFocusToDelegateToHostedWindow;
            this.LastFocusedHwnd = hwndGainFocus;
            base.Focus();
            this.LastFocusedHwnd = IntPtr.Zero;
            this._takingWpfFocus = false;
        }

        private void StartHwndSourceTracking()
        {
            this.StopHwndSourceTracking();
            if (this._hwndSource != null)
            {
                this._hwndSourceTracker = HwndSourceTracker.AddSource(this._hwndSource);
            }
        }

        private void StopHwndSourceTracking()
        {
            if (this._hwndSourceTracker != null)
            {
                this._hwndSourceTracker.Dispose();
                this._hwndSourceTracker = null;
            }
        }

        public bool TabInto(TraversalRequest request)
        {
            this.SetFocusToHwndHost(true, IntPtr.Zero);
            return true;
        }

        protected virtual void UnregisterSourceKeyboardInputSink()
        {
            IKeyboardInputSink keyboardInputSink = this;
            if (keyboardInputSink.KeyboardInputSite != null)
            {
                keyboardInputSink.KeyboardInputSite.Unregister();
            }
        }
    }
}