using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class ViewPresenter : LayoutSynchronizedContentControl
    {
        public readonly static RoutedEvent ContentShowingEvent;

        public readonly static RoutedEvent ContentHidingEvent;

        public readonly static DependencyProperty ViewProperty;

        public readonly static DependencyProperty ContainingElementProperty;

        private DependencyObject currentFocusScope;

        public ViewElement ContainingElement
        {
            get
            {
                return (ViewElement)base.GetValue(ViewPresenter.ContainingElementProperty);
            }
            set
            {
                base.SetValue(ViewPresenter.ContainingElementProperty, value);
            }
        }

        public View View
        {
            get
            {
                return (View)base.GetValue(ViewPresenter.ViewProperty);
            }
            set
            {
                base.SetValue(ViewPresenter.ViewProperty, value);
            }
        }

        static ViewPresenter()
        {
            ViewPresenter.ContentShowingEvent = EventManager.RegisterRoutedEvent("ContentShowing", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));
            ViewPresenter.ContentHidingEvent = EventManager.RegisterRoutedEvent("ContentHiding", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));
            ViewPresenter.ViewProperty = DependencyProperty.Register("View", typeof(View), typeof(ViewPresenter));
            ViewPresenter.ContainingElementProperty = DependencyProperty.Register("ContainingElement", typeof(ViewElement), typeof(ViewPresenter));
            Control.IsTabStopProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Cycle));
        }

        public ViewPresenter()
        {
            base.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.OnIsVisibleChanged);
            UtilityMethods.AddPresentationSourceCleanupAction(this, () =>
            {
                base.Content = null;
                if (this.currentFocusScope != null)
                {
                    FocusManager.SetFocusedElement(this.currentFocusScope, null);
                }
            });
        }

        [CompilerGenerated]
        // <.ctor>b__0
        private void u003cu002ectoru003eb__0()
        {
            base.Content = null;
            if (this.currentFocusScope != null)
            {
                FocusManager.SetFocusedElement(this.currentFocusScope, null);
            }
        }

        [CompilerGenerated]
        // <AsyncRaiseEvent>b__2
        private object u003cAsyncRaiseEventu003eb__2(object arg)
        {
            base.RaiseEvent((RoutedEventArgs)arg);
            return null;
        }

        private void AsyncRaiseEvent(RoutedEventArgs args)
        {
            base.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object arg) =>
            {
                base.RaiseEvent((RoutedEventArgs)arg);
                return null;
            }), args);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ViewPresenterAutomationPeer(this);
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (base.IsKeyboardFocusWithin)
            {
                this.currentFocusScope = FocusManager.GetFocusScope(this);
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            bool newValue = (bool)args.NewValue;
            View dataContext = base.DataContext as View;
            if (dataContext != null)
            {
                this.AsyncRaiseEvent(new ViewEventArgs((newValue ? ViewPresenter.ContentShowingEvent : ViewPresenter.ContentHidingEvent), dataContext));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FrameworkElement.DataContextProperty && base.IsVisible)
            {
                View oldValue = e.OldValue as View;
                View newValue = e.NewValue as View;
                if (oldValue != null)
                {
                    this.AsyncRaiseEvent(new ViewEventArgs(ViewPresenter.ContentHidingEvent, oldValue));
                }
                if (newValue != null)
                {
                    this.AsyncRaiseEvent(new ViewEventArgs(ViewPresenter.ContentShowingEvent, newValue));
                }
            }
        }
    }
}