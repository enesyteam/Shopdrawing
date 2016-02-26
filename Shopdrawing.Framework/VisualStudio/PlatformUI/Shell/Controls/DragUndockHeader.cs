using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class DragUndockHeader : ContentControl
    {
        private Point originalScreenPoint;

        private Point lastScreenPoint;

        public readonly static RoutedEvent DragStartedEvent;

        public readonly static RoutedEvent DragAbsoluteEvent;

        public readonly static RoutedEvent DragCompletedAbsoluteEvent;

        public readonly static RoutedEvent DragDeltaEvent;

        public readonly static RoutedEvent DragHeaderClickedEvent;

        public readonly static RoutedEvent DragHeaderContextMenuEvent;

        public readonly static DependencyProperty DragSensitivityProperty;

        public readonly static DependencyProperty ViewFrameworkElementProperty;

        public readonly static DependencyProperty ViewElementProperty;

        private readonly static DependencyPropertyKey IsDraggingPropertyKey;

        public readonly static DependencyProperty IsDraggingProperty;

        public readonly static DependencyProperty IsWindowTitleBarProperty;

        public readonly static DependencyProperty IsDragEnabledProperty;

        public double DragSensitivity
        {
            get
            {
                return (double)base.GetValue(DragUndockHeader.DragSensitivityProperty);
            }
            set
            {
                base.SetValue(DragUndockHeader.DragSensitivityProperty, value);
            }
        }

        public bool IsDragEnabled
        {
            get
            {
                return (bool)base.GetValue(DragUndockHeader.IsDragEnabledProperty);
            }
            set
            {
                base.SetValue(DragUndockHeader.IsDragEnabledProperty, value);
            }
        }

        public bool IsDragging
        {
            get
            {
                return (bool)base.GetValue(DragUndockHeader.IsDraggingProperty);
            }
            protected set
            {
                base.SetValue(DragUndockHeader.IsDraggingPropertyKey, value);
            }
        }

        public bool IsWindowTitleBar
        {
            get
            {
                return (bool)base.GetValue(DragUndockHeader.IsWindowTitleBarProperty);
            }
            set
            {
                base.SetValue(DragUndockHeader.IsWindowTitleBarProperty, value);
            }
        }

        public ViewElement ViewElement
        {
            get
            {
                return (ViewElement)base.GetValue(DragUndockHeader.ViewElementProperty);
            }
            set
            {
                base.SetValue(DragUndockHeader.ViewElementProperty, value);
            }
        }

        public FrameworkElement ViewFrameworkElement
        {
            get
            {
                return (FrameworkElement)base.GetValue(DragUndockHeader.ViewFrameworkElementProperty);
            }
            set
            {
                base.SetValue(DragUndockHeader.ViewFrameworkElementProperty, value);
            }
        }

        static DragUndockHeader()
        {
            DragUndockHeader.DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));
            DragUndockHeader.DragAbsoluteEvent = EventManager.RegisterRoutedEvent("DragAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DragUndockHeader));
            DragUndockHeader.DragCompletedAbsoluteEvent = EventManager.RegisterRoutedEvent("DragCompletedAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteCompletedEventArgs>), typeof(DragUndockHeader));
            DragUndockHeader.DragDeltaEvent = Thumb.DragDeltaEvent.AddOwner(typeof(DragUndockHeader));
            DragUndockHeader.DragHeaderClickedEvent = EventManager.RegisterRoutedEvent("DragHeaderClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragUndockHeader));
            DragUndockHeader.DragHeaderContextMenuEvent = EventManager.RegisterRoutedEvent("DragHeaderContextMenu", RoutingStrategy.Bubble, typeof(EventHandler<DragUndockHeaderContextMenuEventArgs>), typeof(DragUndockHeader));
            DragUndockHeader.DragSensitivityProperty = DependencyProperty.Register("DragSensitivity", typeof(double), typeof(DragUndockHeader), new FrameworkPropertyMetadata((5)));
            DragUndockHeader.ViewFrameworkElementProperty = DependencyProperty.Register("ViewFrameworkElement", typeof(FrameworkElement), typeof(DragUndockHeader));
            DragUndockHeader.ViewElementProperty = DependencyProperty.Register("ViewElement", typeof(ViewElement), typeof(DragUndockHeader));
            DragUndockHeader.IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(DragUndockHeader), new FrameworkPropertyMetadata(false));
            DragUndockHeader.IsDraggingProperty = DragUndockHeader.IsDraggingPropertyKey.DependencyProperty;
            DragUndockHeader.IsWindowTitleBarProperty = DependencyProperty.Register("IsWindowTitleBar", typeof(bool), typeof(DragUndockHeader));
            DragUndockHeader.IsDragEnabledProperty = DependencyProperty.Register("IsDragEnabled", typeof(bool), typeof(DragUndockHeader), new FrameworkPropertyMetadata(true));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DragUndockHeader), new FrameworkPropertyMetadata(typeof(DragUndockHeader)));
        }

        public DragUndockHeader()
        {
        }

        private void BeginDragging(Point screenPoint)
        {
            if (base.CaptureMouse())
            {
                this.IsDragging = true;
                this.originalScreenPoint = screenPoint;
                this.lastScreenPoint = screenPoint;
            }
        }

        public void CancelDrag()
        {
            if (this.IsDragging)
            {
                this.ReleaseCapture();
                this.RaiseDragCompletedAbsolute(this.lastScreenPoint, false);
            }
        }

        private void CompleteDrag()
        {
            if (this.IsDragging)
            {
                this.ReleaseCapture();
                this.RaiseDragCompletedAbsolute(this.lastScreenPoint, true);
            }
        }

        private bool IsOutsideSensitivity(Point point)
        {
            bool flag = false;
            DraggedTabInfo draggedTabInfo = DockManager.Instance.DraggedTabInfo;
            if (draggedTabInfo != null)
            {
                flag = draggedTabInfo.TabStripRect.Contains(point);
            }
            point.Offset(-this.originalScreenPoint.X, -this.originalScreenPoint.Y);
            if (flag)
            {
                return false;
            }
            if (Math.Abs(point.X) > this.DragSensitivity)
            {
                return true;
            }
            return Math.Abs(point.Y) > this.DragSensitivity;
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = true;
            Visual originalSource = (Visual)e.OriginalSource;
            Point point = originalSource.TransformToVisual(this).Transform(new Point(e.CursorLeft, e.CursorTop));
            base.RaiseEvent(new DragUndockHeaderContextMenuEventArgs(DragUndockHeader.DragHeaderContextMenuEvent, point));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            bool flag = false;
            TabItem tabItem = this.FindAncestor<TabItem>();
            if (tabItem != null)
            {
                DraggedTabInfo draggedTabInfo = DockManager.Instance.DraggedTabInfo;
                if (draggedTabInfo != null && draggedTabInfo.DraggedViewElement == this.ViewElement)
                {
                    flag = true;
                    ReorderTabPanel parent = tabItem.Parent as ReorderTabPanel ?? VisualTreeHelper.GetParent(tabItem) as ReorderTabPanel;
                    if (parent != null && draggedTabInfo != null && (draggedTabInfo.TabStrip != parent || parent.Children.Count != draggedTabInfo.TabRects.Count))
                    {
                        draggedTabInfo.TabStrip = parent;
                        draggedTabInfo.TabStrip.IsNotificationNeeded = true;
                    }
                }
            }
            bool flag1 = (DockManager.Instance.UndockingInformation == null ? false : DockManager.Instance.UndockingInformation.Element == this.ViewElement);
            if (NativeMethods.IsLeftButtonPressed() && (flag1 || flag))
            {
                this.BeginDragging((flag1 ? DockManager.Instance.UndockingInformation.Location : NativeMethods.GetCursorPos()));
            }
        }

        protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseCapturedChanged(e);
            if (!base.IsMouseCaptured)
            {
                this.CancelDrag();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsConnectedToPresentationSource() && this.IsDragEnabled)
            {
                this.BeginDragging(base.PointToScreen(e.GetPosition(this)));
                this.RaiseHeaderClicked();
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (base.IsMouseCaptured && this.IsDragging && this.IsConnectedToPresentationSource())
            {
                this.lastScreenPoint = base.PointToScreen(e.GetPosition(this));
                this.CompleteDrag();
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (base.IsMouseCaptured && this.IsDragging && this.IsConnectedToPresentationSource())
            {
                Point screen = base.PointToScreen(e.GetPosition(this));
                base.RaiseEvent(new DragDeltaEventArgs(screen.X - this.lastScreenPoint.X, screen.Y - this.lastScreenPoint.Y));
                this.RaiseDragAbsolute(screen);
                if (this.IsOutsideSensitivity(screen))
                {
                    this.RaiseDragStarted(screen);
                }
                this.lastScreenPoint = screen;
            }
        }

        protected void RaiseDragAbsolute(Point point)
        {
            base.RaiseEvent(new DragAbsoluteEventArgs(DragUndockHeader.DragAbsoluteEvent, point));
        }

        protected void RaiseDragCompletedAbsolute(Point point, bool isCompleted)
        {
            base.RaiseEvent(new DragAbsoluteCompletedEventArgs(DragUndockHeader.DragCompletedAbsoluteEvent, point, isCompleted));
        }

        protected void RaiseDragStarted(Point point)
        {
            base.RaiseEvent(new DragAbsoluteEventArgs(DragUndockHeader.DragStartedEvent, point));
        }

        protected void RaiseHeaderClicked()
        {
            base.RaiseEvent(new RoutedEventArgs(DragUndockHeader.DragHeaderClickedEvent));
        }

        private void ReleaseCapture()
        {
            if (this.IsDragging)
            {
                base.ClearValue(DragUndockHeader.IsDraggingPropertyKey);
                if (base.IsMouseCaptured)
                {
                    base.ReleaseMouseCapture();
                }
            }
        }
    }
}