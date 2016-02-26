using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
    public class AutoHideRoot : ViewGroup
    {
        public readonly static DependencyProperty IsAutoHideCenterProperty;

        static AutoHideRoot()
        {
            AutoHideRoot.IsAutoHideCenterProperty = DependencyProperty.RegisterAttached("AutoHideCenter", typeof(bool), typeof(AutoHideRoot));
        }

        public AutoHideRoot()
        {
        }

        public static AutoHideRoot Create()
        {
            return ViewElementFactory.Current.CreateAutoHideRoot();
        }

        public static bool GetIsAutoHideCenter(ViewElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(AutoHideRoot.IsAutoHideCenterProperty);
        }

        public override bool IsChildAllowed(ViewElement element)
        {
            if (element is AutoHideChannel)
            {
                return true;
            }
            return element is DockRoot;
        }

        public static void SetIsAutoHideCenter(ViewElement element, bool center)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AutoHideRoot.IsAutoHideCenterProperty, center);
        }
    }
}