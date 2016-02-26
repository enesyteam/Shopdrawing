using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    [TemplatePart(Name = "PART_HwndHost", Type = typeof(HwndHost))]
    public class AutoHideWindow : LayoutSynchronizedContentControl, IResizable, IDisposable
    {
        public readonly static RoutedEvent SlideoutStartedEvent;

        public readonly static RoutedEvent SlideoutCompletedEvent;

        public readonly static DependencyProperty IsAutoHiddenProperty;

        public readonly static DependencyProperty DockRootElementProperty;

        private bool disposed;

        public FrameworkElement DockRootElement
        {
            get
            {
                return (FrameworkElement)base.GetValue(AutoHideWindow.DockRootElementProperty);
            }
            set
            {
                base.SetValue(AutoHideWindow.DockRootElementProperty, value);
            }
        }

        static AutoHideWindow()
        {
            AutoHideWindow.SlideoutStartedEvent = EventManager.RegisterRoutedEvent("SlideoutStarted", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(AutoHideChannelControl));
            AutoHideWindow.SlideoutCompletedEvent = EventManager.RegisterRoutedEvent("SlideoutCompleted", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(AutoHideChannelControl));
            AutoHideWindow.IsAutoHiddenProperty = DependencyProperty.RegisterAttached("IsAutoHidden", typeof(bool), typeof(AutoHideWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
            AutoHideWindow.DockRootElementProperty = DependencyProperty.Register("DockRootElement", typeof(FrameworkElement), typeof(AutoHideWindow));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideWindow), new FrameworkPropertyMetadata(typeof(AutoHideWindow)));
        }

        public AutoHideWindow()
        {
            AutoHideWindow.SetIsAutoHidden(this, true);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    IDisposable templateChild = base.GetTemplateChild("PART_HwndHost") as IDisposable;
                    if (templateChild != null)
                    {
                        templateChild.Dispose();
                    }
                }
                this.disposed = true;
            }
        }

        public static bool GetIsAutoHidden(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(AutoHideWindow.IsAutoHiddenProperty);
        }

        public static void SetIsAutoHidden(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AutoHideWindow.IsAutoHiddenProperty, value);
        }

        public void UpdateBounds(double leftDelta, double topDelta, double widthDelta, double heightDelta)
        {
            Rect rect = new Rect(0, 0, base.ActualWidth, base.ActualHeight);
            Size size = new Size((base.MinWidth.IsNonreal() ? 0 : base.MinWidth), (base.MinHeight.IsNonreal() ? 0 : base.MinHeight));
            Size size1 = new Size((base.MaxWidth.IsNonreal() ? double.MaxValue : base.MaxWidth), (base.MaxHeight.IsNonreal() ? double.MaxValue : base.MaxHeight));
            rect = rect.Resize(new Vector(leftDelta, topDelta), new Vector(widthDelta, heightDelta), size, size1);
            if (widthDelta != 0)
            {
                base.Width = rect.Width;
            }
            if (heightDelta != 0)
            {
                base.Height = rect.Height;
            }
        }
    }
}