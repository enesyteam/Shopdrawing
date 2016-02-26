using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI.Shell
{
    public class NakedView : ViewElement
    {
        public readonly static DependencyProperty ContentProperty;

        public readonly static DependencyProperty NameProperty;

        public readonly static DependencyProperty IsActiveProperty;

        [NonXamlSerialized]
        public FrameworkElement Content
        {
            get
            {
                return (FrameworkElement)base.GetValue(NakedView.ContentProperty);
            }
            set
            {
                base.SetValue(NakedView.ContentProperty, value);
            }
        }

        [NonXamlSerialized]
        public bool IsActive
        {
            get
            {
                return (bool)base.GetValue(NakedView.IsActiveProperty);
            }
            set
            {
                base.SetValue(NakedView.IsActiveProperty, value);
            }
        }

        public string Name
        {
            get
            {
                return (string)base.GetValue(NakedView.NameProperty);
            }
            set
            {
                base.SetValue(NakedView.NameProperty, value);
            }
        }

        static NakedView()
        {
            NakedView.ContentProperty = DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(NakedView));
            NakedView.NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(NakedView));
            NakedView.IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(NakedView));
        }

        public NakedView()
        {
            base.IsVisible = true;
            AutoHideRoot.SetIsAutoHideCenter(this, true);
        }

        public override string ToString()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            object[] name = new object[] { base.GetType().Name, this.Name };
            return string.Format(invariantCulture, "{0}, Name = {1}", name);
        }
    }
}