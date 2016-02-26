using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class DocumentTabPanel : ReorderTabPanel
    {
        public readonly static RoutedEvent SelectedItemHiddenEvent;

        private readonly static DependencyPropertyKey HasHiddenItemsPropertyKey;

        public readonly static DependencyProperty HasHiddenItemsProperty;

        public bool HasHiddenItems
        {
            get
            {
                return (bool)base.GetValue(DocumentTabPanel.HasHiddenItemsProperty);
            }
            protected set
            {
                base.SetValue(DocumentTabPanel.HasHiddenItemsPropertyKey, value);
            }
        }

        static DocumentTabPanel()
        {
            DocumentTabPanel.SelectedItemHiddenEvent = EventManager.RegisterRoutedEvent("SelectedElementHidden", RoutingStrategy.Bubble, typeof(EventHandler<SelectedItemHiddenEventArgs>), typeof(DocumentTabPanel));
            DocumentTabPanel.HasHiddenItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasHiddenItems", typeof(bool), typeof(DocumentTabPanel), new PropertyMetadata(false));
            DocumentTabPanel.HasHiddenItemsProperty = DocumentTabPanel.HasHiddenItemsPropertyKey.DependencyProperty;
        }

        public DocumentTabPanel()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool width;
            bool height;
            double num = 0;
            double num1 = 0;
            bool flag = false;
            bool flag1 = false;
            bool width1 = false;
            int num2 = -1;
            int num3 = 0;
            TabControl tabControl = this.FindAncestor<TabControl>();
            UIElement item = null;
            if (tabControl.SelectedIndex >= 0 && tabControl.SelectedIndex < base.InternalChildren.Count)
            {
                item = base.InternalChildren[tabControl.SelectedIndex];
            }
            else if (base.InternalChildren.Count > 0)
            {
                tabControl.SelectedIndex = 0;
                item = base.InternalChildren[0];
            }
            foreach (UIElement internalChild in base.InternalChildren)
            {
                TabItem tabItem = internalChild as TabItem;
                num2++;
                if (!this.HasLogicalOrientation || this.LogicalOrientation != Orientation.Vertical)
                {
                    if (flag)
                    {
                        width = true;
                    }
                    else
                    {
                        Size desiredSize = internalChild.DesiredSize;
                        width = num + desiredSize.Width > finalSize.Width;
                    }
                    flag = width;
                    if (item != null)
                    {
                        Size size = item.DesiredSize;
                        width1 = num + size.Width <= finalSize.Width;
                    }
                }
                else
                {
                    if (flag)
                    {
                        height = true;
                    }
                    else
                    {
                        Size desiredSize1 = internalChild.DesiredSize;
                        height = num1 + desiredSize1.Height > finalSize.Height;
                    }
                    flag = height;
                    if (item != null)
                    {
                        Size size1 = item.DesiredSize;
                        width1 = num1 + size1.Height <= finalSize.Height;
                    }
                }
                if (width1)
                {
                    num3 = num2;
                }
                if (!flag)
                {
                    internalChild.Visibility = Visibility.Visible;
                    if (!this.HasLogicalOrientation || this.LogicalOrientation != Orientation.Vertical)
                    {
                        double width2 = internalChild.DesiredSize.Width;
                        internalChild.Arrange(new Rect(num, num1, width2, finalSize.Height));
                        num = num + width2;
                    }
                    else
                    {
                        double height1 = internalChild.DesiredSize.Height;
                        internalChild.Arrange(new Rect(num, num1, finalSize.Width, height1));
                        num1 = num1 + height1;
                    }
                }
                else
                {
                    internalChild.Visibility = Visibility.Hidden;
                    if (tabControl.SelectedItem != tabItem.Content)
                    {
                        continue;
                    }
                    flag1 = true;
                }
            }
            Size height2 = new Size();
            if (!this.HasLogicalOrientation || this.LogicalOrientation != Orientation.Vertical)
            {
                height2.Width = num;
                height2.Height = finalSize.Height;
            }
            else
            {
                height2.Width = finalSize.Width;
                height2.Height = num1;
            }
            this.HasHiddenItems = flag;
            if (flag1)
            {
                this.RaiseSelectedElementHidden(tabControl, num3);
            }
            return height2;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size width = new Size();
            double num = 0;
            double height = 0;
            double num1 = 0;
            double num2 = 0;
            foreach (UIElement internalChild in base.InternalChildren)
            {
                internalChild.Measure(availableSize);
                num2 = Math.Max(internalChild.DesiredSize.Height, num2);
                num1 = Math.Max(internalChild.DesiredSize.Width, num1);
                num = num + internalChild.DesiredSize.Width;
                height = height + internalChild.DesiredSize.Height;
            }
            if (!this.HasLogicalOrientation || this.LogicalOrientation != Orientation.Vertical)
            {
                width.Width = availableSize.Width;
                width.Height = num2;
            }
            else
            {
                width.Width = num1;
                width.Height = availableSize.Height;
            }
            return width;
        }

        private void RaiseSelectedElementHidden(TabControl parent, int lastVisiblePosition)
        {
            base.RaiseEvent(new SelectedItemHiddenEventArgs(DocumentTabPanel.SelectedItemHiddenEvent, parent, lastVisiblePosition));
        }
    }
}