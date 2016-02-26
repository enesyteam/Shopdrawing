using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public static class MouseHover
    {
        public readonly static RoutedEvent MouseHoverEvent;

        public readonly static DependencyProperty MouseHoverDelayProperty;

        public readonly static DependencyProperty IsMouseHoverTrackingEnabledProperty;

        private readonly static DependencyProperty MouseHoverMonitorProperty;

        static MouseHover()
        {
            MouseHover.MouseHoverEvent = EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MouseHover));
            MouseHover.MouseHoverDelayProperty = DependencyProperty.RegisterAttached("MouseHoverDelay", typeof(TimeSpan), typeof(MouseHover), new FrameworkPropertyMetadata((object)TimeSpan.Zero));
            MouseHover.IsMouseHoverTrackingEnabledProperty = DependencyProperty.RegisterAttached("IsMouseHoverTrackingEnabled", typeof(bool), typeof(MouseHover), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(MouseHover.OnIsMouseHoverTrackingEnabledChanged)));
            MouseHover.MouseHoverMonitorProperty = DependencyProperty.RegisterAttached("MouseHoverMonitor", typeof(MouseHover.Monitor), typeof(MouseHover), new FrameworkPropertyMetadata(null));
        }

        private static void AttachHoverMonitor(UIElement element)
        {
            MouseHover.DetachHoverMonitor(element);
            MouseHover.SetMouseHoverMonitor(element, new MouseHover.Monitor(element));
        }

        private static void DetachHoverMonitor(UIElement element)
        {
            MouseHover.Monitor mouseHoverMonitor = MouseHover.GetMouseHoverMonitor(element);
            if (mouseHoverMonitor != null)
            {
                mouseHoverMonitor.Dispose();
                MouseHover.SetMouseHoverMonitor(element, null);
            }
        }

        public static bool GetIsMouseHoverTrackingEnabled(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(MouseHover.IsMouseHoverTrackingEnabledProperty);
        }

        public static TimeSpan GetMouseHoverDelay(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (TimeSpan)element.GetValue(MouseHover.MouseHoverDelayProperty);
        }

        private static MouseHover.Monitor GetMouseHoverMonitor(UIElement element)
        {
            return (MouseHover.Monitor)element.GetValue(MouseHover.MouseHoverMonitorProperty);
        }

        private static void OnIsMouseHoverTrackingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement uIElement = obj as UIElement;
            if (uIElement == null)
            {
                throw new ArgumentException("MouseHover element must be a UIElement.");
            }
            if ((bool)args.NewValue)
            {
                MouseHover.AttachHoverMonitor(uIElement);
                return;
            }
            MouseHover.DetachHoverMonitor(uIElement);
        }

        public static void SetIsMouseHoverTrackingEnabled(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(MouseHover.IsMouseHoverTrackingEnabledProperty, value);
        }

        public static void SetMouseHoverDelay(UIElement element, TimeSpan value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(MouseHover.MouseHoverDelayProperty, value);
        }

        private static void SetMouseHoverMonitor(UIElement element, MouseHover.Monitor value)
        {
            element.SetValue(MouseHover.MouseHoverMonitorProperty, value);
        }

        private class Monitor : DisposableObject
        {
            private bool isActive;

            private bool isMouseOver;

            private UIElement Element
            {
                get;
                set;
            }

            private bool IsActive
            {
                get
                {
                    return this.isActive;
                }
                set
                {
                    if (this.isActive != value)
                    {
                        this.isActive = value;
                        this.UpdateTimerState();
                    }
                }
            }

            private bool IsMouseOver
            {
                get
                {
                    return this.isMouseOver;
                }
                set
                {
                    if (this.isMouseOver != value)
                    {
                        this.isMouseOver = value;
                        this.UpdateTimerState();
                    }
                }
            }

            private DispatcherTimer Timer
            {
                get;
                set;
            }

            public Monitor(UIElement element)
            {
                this.Element = element;
                this.Element.MouseEnter += new MouseEventHandler(this.OnMouseEnter);
                this.Element.MouseLeave += new MouseEventHandler(this.OnMouseLeave);
                ApplicationActivationMonitor.Instance.Activated += new EventHandler(this.OnActivated);
                ApplicationActivationMonitor.Instance.Deactivated += new EventHandler(this.OnDeactivated);
                this.IsMouseOver = this.Element.IsMouseOver;
                this.IsActive = ApplicationActivationMonitor.Instance.IsActive;
            }

            protected override void DisposeManagedResources()
            {
                this.StopTimer();
                this.Element.MouseEnter -= new MouseEventHandler(this.OnMouseEnter);
                this.Element.MouseLeave -= new MouseEventHandler(this.OnMouseLeave);
                ApplicationActivationMonitor.Instance.Activated -= new EventHandler(this.OnActivated);
                ApplicationActivationMonitor.Instance.Deactivated -= new EventHandler(this.OnDeactivated);
                base.DisposeManagedResources();
            }

            private void OnActivated(object sender, EventArgs e)
            {
                this.IsActive = true;
            }

            private void OnDeactivated(object sender, EventArgs e)
            {
                this.IsActive = false;
            }

            private void OnMouseEnter(object sender, MouseEventArgs e)
            {
                this.IsMouseOver = true;
            }

            private void OnMouseLeave(object sender, MouseEventArgs e)
            {
                this.IsMouseOver = false;
            }

            private void OnTimerTick(object sender, EventArgs args)
            {
                this.Element.RaiseEvent(new RoutedEventArgs(MouseHover.MouseHoverEvent));
                this.StopTimer();
            }

            private void StartTimer()
            {
                this.StopTimer();
                TimeSpan mouseHoverDelay = MouseHover.GetMouseHoverDelay(this.Element);
                this.Timer = new DispatcherTimer(mouseHoverDelay, DispatcherPriority.Input, new EventHandler(this.OnTimerTick), this.Element.Dispatcher);
            }

            private void StopTimer()
            {
                if (this.Timer != null)
                {
                    this.Timer.Stop();
                    this.Timer = null;
                }
            }

            private void UpdateTimerState()
            {
                if (this.IsMouseOver && this.IsActive)
                {
                    this.StartTimer();
                    return;
                }
                this.StopTimer();
            }
        }
    }
}