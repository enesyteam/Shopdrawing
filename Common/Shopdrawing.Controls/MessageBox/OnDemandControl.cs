using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.Controls
{
    public class OnDemandControl : Control
    {
        public readonly static DependencyProperty OnDemandTemplateProperty;

        private bool templateIsApplied;

        public ControlTemplate OnDemandTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(OnDemandControl.OnDemandTemplateProperty);
            }
            set
            {
                base.SetValue(OnDemandControl.OnDemandTemplateProperty, value);
            }
        }

        static OnDemandControl()
        {
            OnDemandControl.OnDemandTemplateProperty = DependencyProperty.Register("OnDemandTemplate", typeof(ControlTemplate), typeof(OnDemandControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDemandControl.OnDemandTemplatePropertyChanged)));
            UIElement.VisibilityProperty.OverrideMetadata(typeof(OnDemandControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDemandControl.VisibilityPropertyChanged)));
        }

        public OnDemandControl()
        {
        }

        private static void OnDemandTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnDemandControl newValue = d as OnDemandControl;
            if (newValue != null && (newValue.templateIsApplied || newValue.Visibility == Visibility.Visible))
            {
                newValue.templateIsApplied = true;
                newValue.Template = (ControlTemplate)e.NewValue;
            }
        }

        private void OnVisibilityChanged(Visibility newValue)
        {
            if (newValue == Visibility.Visible && !this.templateIsApplied)
            {
                base.Template = this.OnDemandTemplate;
                this.templateIsApplied = true;
            }
        }

        private static void VisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnDemandControl onDemandControl = d as OnDemandControl;
            if (onDemandControl != null)
            {
                onDemandControl.OnVisibilityChanged((Visibility)e.NewValue);
            }
        }
    }
}