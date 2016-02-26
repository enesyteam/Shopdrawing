using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class DockGroupAdorner : DockAdorner
    {
        public readonly static DependencyProperty DockSiteTypeProperty;

        public readonly static DependencyProperty IsFirstProperty;

        public readonly static DependencyProperty IsLastProperty;

        public DockSiteType DockSiteType
        {
            get
            {
                return (DockSiteType)base.GetValue(DockGroupAdorner.DockSiteTypeProperty);
            }
            set
            {
                base.SetValue(DockGroupAdorner.DockSiteTypeProperty, value);
            }
        }

        public bool IsFirst
        {
            get
            {
                return (bool)base.GetValue(DockGroupAdorner.IsFirstProperty);
            }
            set
            {
                base.SetValue(DockGroupAdorner.IsFirstProperty, value);
            }
        }

        public bool IsLast
        {
            get
            {
                return (bool)base.GetValue(DockGroupAdorner.IsLastProperty);
            }
            set
            {
                base.SetValue(DockGroupAdorner.IsLastProperty, value);
            }
        }

        static DockGroupAdorner()
        {
            DockGroupAdorner.DockSiteTypeProperty = DependencyProperty.Register("DockSiteType", typeof(DockSiteType), typeof(DockGroupAdorner), new PropertyMetadata(DockSiteType.Default));
            DockGroupAdorner.IsFirstProperty = DependencyProperty.Register("IsFirst", typeof(bool), typeof(DockGroupAdorner), new FrameworkPropertyMetadata(false));
            DockGroupAdorner.IsLastProperty = DependencyProperty.Register("IsLast", typeof(bool), typeof(DockGroupAdorner), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupAdorner), new FrameworkPropertyMetadata(typeof(DockGroupAdorner)));
        }

        public DockGroupAdorner()
        {
        }

        protected override void UpdateContentCore()
        {
            base.UpdateContentCore();
            DockTarget adornedElement = base.AdornedElement as DockTarget;
            if (adornedElement != null)
            {
                this.DockSiteType = adornedElement.DockSiteType;
                SplitterItem splitterItem = adornedElement.FindAncestor<SplitterItem>();
                if (splitterItem != null)
                {
                    this.IsFirst = SplitterPanel.GetIsFirst(splitterItem);
                    this.IsLast = SplitterPanel.GetIsLast(splitterItem);
                }
            }
        }
    }
}