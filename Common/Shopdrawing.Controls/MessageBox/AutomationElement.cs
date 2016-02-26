using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Diagnostics
{
    public static class AutomationElement
    {
        public readonly static DependencyProperty IdProperty;

        static AutomationElement()
        {
            AutomationElement.IdProperty = DependencyProperty.RegisterAttached("Id", typeof(string), typeof(AutomationElement));
        }

        public static string GetId(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return (string)target.GetValue(AutomationElement.IdProperty);
        }

        public static void SetId(DependencyObject target, string value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            target.SetValue(AutomationElement.IdProperty, value);
        }
    }
}