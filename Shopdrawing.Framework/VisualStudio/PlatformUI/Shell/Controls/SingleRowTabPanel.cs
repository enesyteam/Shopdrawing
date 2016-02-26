using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
    public class SingleRowTabPanel : ReorderTabPanel
    {
        private readonly static DependencyProperty CalculatedTabSizeProperty;

        protected readonly static DependencyPropertyKey IsFirstPropertyKey;

        protected readonly static DependencyPropertyKey IsLastPropertyKey;

        public readonly static DependencyProperty IsFirstProperty;

        public readonly static DependencyProperty IsLastProperty;

        static SingleRowTabPanel()
        {
            SingleRowTabPanel.CalculatedTabSizeProperty = DependencyProperty.RegisterAttached("CalculatedTabSize", typeof(Size), typeof(SingleRowTabPanel));
            SingleRowTabPanel.IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(bool), typeof(SingleRowTabPanel), new FrameworkPropertyMetadata(false));
            SingleRowTabPanel.IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(bool), typeof(SingleRowTabPanel), new FrameworkPropertyMetadata(false));
            SingleRowTabPanel.IsFirstProperty = SingleRowTabPanel.IsFirstPropertyKey.DependencyProperty;
            SingleRowTabPanel.IsLastProperty = SingleRowTabPanel.IsLastPropertyKey.DependencyProperty;
        }

        public SingleRowTabPanel()
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = 0;
            foreach (UIElement internalChild in base.InternalChildren)
            {
                Size calculatedTabSize = SingleRowTabPanel.GetCalculatedTabSize(internalChild);
                internalChild.Arrange(new Rect(width, 0, calculatedTabSize.Width, calculatedTabSize.Height));
                width = width + calculatedTabSize.Width;
            }
            return finalSize;
        }

        private static void CalculateTruncationThreshold(List<double> values, double sizeToRemove, out double truncationThreshold, out double truncatedValue)
        {
            values.Sort();
            for (int i = 1; i < values.Count; i++)
            {
                double item = values[values.Count - i];
                double num = values[values.Count - i - 1];
                double num1 = item - num;
                if (sizeToRemove - num1 * (double)i < 0)
                {
                    truncationThreshold = item;
                    truncatedValue = item - sizeToRemove / (double)i;
                    return;
                }
                sizeToRemove = sizeToRemove - num1 * (double)i;
            }
            truncationThreshold = values[0];
            truncatedValue = truncationThreshold - sizeToRemove / (double)values.Count;
        }

        private static Size GetCalculatedTabSize(UIElement element)
        {
            return (Size)element.GetValue(SingleRowTabPanel.CalculatedTabSizeProperty);
        }

        public static bool GetIsFirst(UIElement element)
        {
            return (bool)element.GetValue(SingleRowTabPanel.IsFirstProperty);
        }

        public static bool GetIsLast(UIElement element)
        {
            return (bool)element.GetValue(SingleRowTabPanel.IsLastProperty);
        }

        private static double GetMinimumWidth(UIElement child)
        {
            double minWidth = 0;
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                minWidth = frameworkElement.MinWidth;
            }
            return minWidth;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double num;
            double num1;
            this.UpdateFirstAndLast();
            double num2 = 0;
            double width = 0;
            List<double> nums = new List<double>();
            foreach (UIElement internalChild in base.InternalChildren)
            {
                internalChild.Measure(availableSize);
                Size desiredSize = internalChild.DesiredSize;
                SingleRowTabPanel.SetCalculatedTabSize(internalChild, desiredSize);
                num2 = Math.Max(num2, desiredSize.Height);
                nums.Add(desiredSize.Width);
                width = width + desiredSize.Width;
            }
            num2 = Math.Min(availableSize.Height, num2);
            if (width <= availableSize.Width)
            {
                return new Size(width, num2);
            }
            SingleRowTabPanel.CalculateTruncationThreshold(nums, width - availableSize.Width, out num, out num1);
            foreach (UIElement uIElement in base.InternalChildren)
            {
                Size size = uIElement.DesiredSize;
                double width1 = size.Width;
                double minimumWidth = SingleRowTabPanel.GetMinimumWidth(uIElement);
                if (width1 >= num)
                {
                    width1 = Math.Max(minimumWidth, num1);
                }
                Size size1 = new Size(width1, size.Height);
                SingleRowTabPanel.SetCalculatedTabSize(uIElement, size1);
                uIElement.Measure(size1);
            }
            return new Size(availableSize.Width, num2);
        }

        private static void SetCalculatedTabSize(UIElement element, Size size)
        {
            element.SetValue(SingleRowTabPanel.CalculatedTabSizeProperty, size);
        }

        protected static void SetIsFirst(UIElement element, bool value)
        {
            element.SetValue(SingleRowTabPanel.IsFirstPropertyKey, value);
        }

        protected static void SetIsLast(UIElement element, bool value)
        {
            element.SetValue(SingleRowTabPanel.IsLastPropertyKey, value);
        }

        private void UpdateFirstAndLast()
        {
            int count = base.InternalChildren.Count;
            int num = base.InternalChildren.Count - 1;
            for (int i = 0; i < count; i++)
            {
                SingleRowTabPanel.SetIsFirst(base.InternalChildren[i], i == 0);
                SingleRowTabPanel.SetIsLast(base.InternalChildren[i], i == num);
            }
        }
    }
}