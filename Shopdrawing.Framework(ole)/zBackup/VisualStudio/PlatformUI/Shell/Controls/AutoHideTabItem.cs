using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class AutoHideTabItem : Button
    {
        public readonly static DependencyProperty IsAutoHideWindowShownProperty;

        public bool IsAutoHideWindowShown
        {
            get
            {
                return (bool)base.GetValue(AutoHideTabItem.IsAutoHideWindowShownProperty);
            }
            set
            {
                base.SetValue(AutoHideTabItem.IsAutoHideWindowShownProperty, value);
            }
        }

        static AutoHideTabItem()
        {
            AutoHideTabItem.IsAutoHideWindowShownProperty = DependencyProperty.Register("IsAutoHideWindowShown", typeof(bool), typeof(AutoHideTabItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(AutoHideTabItem.OnIsAutoHideWindowShownChanged)));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideTabItem), new FrameworkPropertyMetadata(typeof(AutoHideTabItem)));
        }

        public AutoHideTabItem()
        {
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AutoHideTabItemAutomationPeer(this);
        }

        private static void OnIsAutoHideWindowShownChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoHideTabItem autoHideTabItem = (AutoHideTabItem)obj;
            if (!autoHideTabItem.IsConnectedToPresentationSource())
            {
                return;
            }
            if (autoHideTabItem.IsAutoHideWindowShown)
            {
                ViewCommands.ShowAutoHiddenView.Execute(autoHideTabItem.DataContext, autoHideTabItem);
            }
        }
    }
}