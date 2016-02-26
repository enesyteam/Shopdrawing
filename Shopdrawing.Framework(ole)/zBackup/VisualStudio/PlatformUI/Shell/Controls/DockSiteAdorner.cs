using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class DockSiteAdorner : DockAdorner
    {
        public readonly static DependencyProperty CreatesDocumentGroupProperty;

        public readonly static DependencyProperty IsHighlightedProperty;

        public DockTarget AdornedDockTarget
        {
            get
            {
                DockAdornerWindow dockAdornerWindow = this.FindAncestor<DockAdornerWindow>();
                if (dockAdornerWindow == null)
                {
                    return null;
                }
                if (dockAdornerWindow.AdornedElement == null)
                {
                    return null;
                }
                return dockAdornerWindow.AdornedElement as DockTarget;
            }
        }

        public bool CreatesDocumentGroup
        {
            get
            {
                return (bool)base.GetValue(DockSiteAdorner.CreatesDocumentGroupProperty);
            }
            set
            {
                base.SetValue(DockSiteAdorner.CreatesDocumentGroupProperty, value);
            }
        }

        public bool IsHighlighted
        {
            get
            {
                return (bool)base.GetValue(DockSiteAdorner.IsHighlightedProperty);
            }
            set
            {
                base.SetValue(DockSiteAdorner.IsHighlightedProperty, value);
            }
        }

        static DockSiteAdorner()
        {
            DockSiteAdorner.CreatesDocumentGroupProperty = DependencyProperty.Register("CreatesDocumentGroup", typeof(bool), typeof(DockSiteAdorner), new FrameworkPropertyMetadata(false));
            DockSiteAdorner.IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(DockSiteAdorner));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSiteAdorner), new FrameworkPropertyMetadata(typeof(DockSiteAdorner)));
        }

        public DockSiteAdorner()
        {
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DockSiteAdornerAutomationPeer(this);
        }
    }
}