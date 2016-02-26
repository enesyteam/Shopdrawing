using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.Framework.Controls
{
    public sealed class FreezeBehavior
    {
        public readonly static DependencyProperty FreezeProperty;

        public readonly static DependencyProperty FreezeResourcesProperty;

        static FreezeBehavior()
        {
            FreezeBehavior.FreezeProperty = DependencyProperty.RegisterAttached("Freeze", typeof(bool), typeof(FreezeBehavior), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FreezeBehavior.FreezeBehavior_FreezePropertyChanged)));
            FreezeBehavior.FreezeResourcesProperty = DependencyProperty.RegisterAttached("FreezeResources", typeof(bool), typeof(FreezeBehavior), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FreezeBehavior.FreezeBehavior_FreezeResourcesPropertyChanged)));
        }

        private FreezeBehavior()
        {
        }

        private static void FrameworkContentElement_Initialized(object sender, EventArgs e)
        {
            FrameworkContentElement frameworkContentElement = (FrameworkContentElement)sender;
            frameworkContentElement.Initialized -= new EventHandler(FreezeBehavior.FrameworkContentElement_Initialized);
            ResourceDictionary resources = frameworkContentElement.Resources;
            if (resources != null)
            {
                FreezeBehavior.FreezeResources(resources);
            }
        }

        private static void FrameworkElement_Initialized(object sender, EventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            frameworkElement.Initialized -= new EventHandler(FreezeBehavior.FrameworkElement_Initialized);
            ResourceDictionary resources = frameworkElement.Resources;
            if (resources != null)
            {
                FreezeBehavior.FreezeResources(resources);
            }
        }

        private static void FreezeBehavior_FreezePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                return;
            }
            Freezable freezable = target as Freezable;
            if (freezable != null && freezable.CanFreeze)
            {
                freezable.Freeze();
            }
        }

        private static void FreezeBehavior_FreezeResourcesPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                return;
            }
            ResourceDictionary resources = null;
            FrameworkElement frameworkElement = target as FrameworkElement;
            FrameworkContentElement frameworkContentElement = target as FrameworkContentElement;
            if (frameworkElement != null)
            {
                if (frameworkElement.IsInitialized)
                {
                    resources = frameworkElement.Resources;
                }
                else
                {
                    frameworkElement.Initialized += new EventHandler(FreezeBehavior.FrameworkElement_Initialized);
                }
            }
            else if (frameworkContentElement != null)
            {
                if (frameworkContentElement.IsInitialized)
                {
                    resources = frameworkContentElement.Resources;
                }
                else
                {
                    frameworkContentElement.Initialized += new EventHandler(FreezeBehavior.FrameworkContentElement_Initialized);
                }
            }
            if (resources != null)
            {
                FreezeBehavior.FreezeResources(resources);
            }
        }

        public static void FreezeResources(ResourceDictionary resources)
        {
            object[] objArray = new object[resources.Keys.Count];
            resources.Keys.CopyTo(objArray, 0);
            object[] objArray1 = objArray;
            for (int i = 0; i < (int)objArray1.Length; i++)
            {
                Freezable item = resources[objArray1[i]] as Freezable;
                if (item != null && item.CanFreeze)
                {
                    item.Freeze();
                }
            }
            foreach (ResourceDictionary mergedDictionary in resources.MergedDictionaries)
            {
                FreezeBehavior.FreezeResources(mergedDictionary);
            }
        }

        public static void SetFreeze(DependencyObject target, bool value)
        {
            target.SetValue(FreezeBehavior.FreezeProperty, value);
        }

        public static void SetFreezeResources(DependencyObject target, bool value)
        {
            target.SetValue(FreezeBehavior.FreezeResourcesProperty, value);
        }
    }
}