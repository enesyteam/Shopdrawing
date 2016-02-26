using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Framework.Controls
{
    public static class AnimatedExpander
    {
        public readonly static DependencyProperty IsAnimatedProperty;

        static AnimatedExpander()
        {
            AnimatedExpander.IsAnimatedProperty = DependencyProperty.RegisterAttached("IsAnimated", typeof(bool), typeof(AnimatedExpander), new PropertyMetadata(new PropertyChangedCallback(AnimatedExpander.OnIsAnimatedChanged)));
        }

        private static void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            if (expander != null)
            {
                FrameworkElement frameworkElement = (FrameworkElement)expander.Template.FindName("ExpandSite", expander);
                if (frameworkElement != null)
                {
                    Storyboard item = (Storyboard)expander.Template.Resources["Open"];
                    Storyboard storyboard = (Storyboard)expander.Template.Resources["Close"];
                    if (item != null)
                    {
                        item.Stop(frameworkElement);
                    }
                    if (storyboard != null)
                    {
                        storyboard.Seek(frameworkElement, new TimeSpan((long)0), TimeSeekOrigin.BeginTime);
                        frameworkElement.BeginStoryboard(storyboard, HandoffBehavior.SnapshotAndReplace, true);
                    }
                }
            }
        }

        private static void expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            if (expander != null)
            {
                expander.UpdateLayout();
                FrameworkElement frameworkElement = (FrameworkElement)expander.Template.FindName("ExpandSite", expander);
                if (frameworkElement != null)
                {
                    frameworkElement.Visibility = Visibility.Visible;
                    frameworkElement.UpdateLayout();
                    Storyboard item = (Storyboard)expander.Template.Resources["Open"];
                    Storyboard storyboard = (Storyboard)expander.Template.Resources["Close"];
                    if (storyboard != null)
                    {
                        storyboard.Stop(frameworkElement);
                    }
                    if (item != null)
                    {
                        item.Seek(frameworkElement, new TimeSpan((long)0), TimeSeekOrigin.BeginTime);
                        frameworkElement.BeginStoryboard(item, HandoffBehavior.SnapshotAndReplace, true);
                    }
                }
            }
        }

        public static bool GetIsAnimated(DependencyObject target)
        {
            return (bool)target.GetValue(AnimatedExpander.IsAnimatedProperty);
        }

        private static void OnIsAnimatedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Expander expander = sender as Expander;
            if (expander != null && (bool)e.NewValue)
            {
                expander.Expanded += new RoutedEventHandler(AnimatedExpander.expander_Expanded);
                expander.Collapsed += new RoutedEventHandler(AnimatedExpander.expander_Collapsed);
            }
        }

        public static void SetIsAnimated(DependencyObject target, bool value)
        {
            target.SetValue(AnimatedExpander.IsAnimatedProperty, value);
        }
    }
}