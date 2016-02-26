using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Serialization;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
    public class ExpressionView : View, ICustomXmlSerializable, IDependencyObjectCustomSerializerAccess, INotifyPropertyChanged
    {
        public readonly static DependencyProperty IsAutoHideableProperty;

        public readonly static DependencyProperty IsTitleHiddenProperty;

        private readonly static DependencyPropertyKey DockingOrientationPropertyKey;

        public readonly static DependencyProperty DockingOrientationProperty;

        public readonly static DependencyProperty IsSizableWhenFloatingProperty;

        public readonly static DependencyProperty IsForcedInvisibleProperty;

        public readonly static DependencyProperty IsDesiredVisibleProperty;

        public readonly static DependencyProperty WasSelectedBeforeAutoHideProperty;

        private bool isForcingVisibility;

        [NonXamlSerialized]
        public Size? DefaultFloatSize
        {
            get;
            set;
        }

        [NonXamlSerialized]
        public Orientation? DockingOrientation
        {
            get
            {
                return (Orientation?)base.GetValue(ExpressionView.DockingOrientationProperty);
            }
            private set
            {
                base.SetValue(ExpressionView.DockingOrientationPropertyKey, value);
            }
        }

        public string GripperAutomationId
        {
            get
            {
                return string.Concat("GRIPPER_", base.Name);
            }
        }

        [NonXamlSerialized]
        public bool IsAutoHideable
        {
            get
            {
                return ExpressionView.GetIsAutoHideable(this);
            }
            set
            {
                ExpressionView.SetIsAutoHideable(this, value);
            }
        }

        [DefaultValue(false)]
        public bool IsDesiredVisible
        {
            get
            {
                return (bool)base.GetValue(ExpressionView.IsDesiredVisibleProperty);
            }
            set
            {
                base.SetValue(ExpressionView.IsDesiredVisibleProperty, value);
            }
        }

        [DefaultValue(false)]
        public bool IsForcedInvisible
        {
            get
            {
                return (bool)base.GetValue(ExpressionView.IsForcedInvisibleProperty);
            }
            set
            {
                base.SetValue(ExpressionView.IsForcedInvisibleProperty, value);
            }
        }

        [NonXamlSerialized]
        public bool IsHorizontalGripperVisible
        {
            get
            {
                if (!this.IsTitleHidden)
                {
                    return false;
                }
                Orientation? dockingOrientation = this.DockingOrientation;
                if (dockingOrientation.GetValueOrDefault() != Orientation.Vertical)
                {
                    return true;
                }
                return !dockingOrientation.HasValue;
            }
        }

        [NonXamlSerialized]
        public bool IsSizableWhenFloating
        {
            get
            {
                return ExpressionView.GetIsSizableWhenFloating(this);
            }
            set
            {
                ExpressionView.SetIsSizableWhenFloating(this, value);
            }
        }

        [NonXamlSerialized]
        public bool IsTitleHidden
        {
            get
            {
                return (bool)base.GetValue(ExpressionView.IsTitleHiddenProperty);
            }
            set
            {
                base.SetValue(ExpressionView.IsTitleHiddenProperty, value);
            }
        }

        [NonXamlSerialized]
        public bool IsVerticalGripperVisible
        {
            get
            {
                if (!this.IsTitleHidden)
                {
                    return false;
                }
                Orientation? dockingOrientation = this.DockingOrientation;
                if (dockingOrientation.GetValueOrDefault() != Orientation.Vertical)
                {
                    return false;
                }
                return dockingOrientation.HasValue;
            }
        }

        [NonXamlSerialized]
        public new bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }
            set
            {
                base.IsVisible = value;
            }
        }

        public bool WasSelectedBeforeAutoHide
        {
            get
            {
                return (bool)base.GetValue(ExpressionView.WasSelectedBeforeAutoHideProperty);
            }
            set
            {
                base.SetValue(ExpressionView.WasSelectedBeforeAutoHideProperty, value);
            }
        }

        static ExpressionView()
        {
            ExpressionView.IsAutoHideableProperty = DependencyProperty.RegisterAttached("IsAutoHideable", typeof(bool), typeof(ExpressionView), new PropertyMetadata(false));
            ExpressionView.IsTitleHiddenProperty = DependencyProperty.Register("IsTitleHidden", typeof(bool), typeof(ExpressionView), new PropertyMetadata(false));
            ExpressionView.DockingOrientationPropertyKey = DependencyProperty.RegisterReadOnly("DockingOrientation", typeof(Orientation?), typeof(ExpressionView), new PropertyMetadata(null));
            ExpressionView.DockingOrientationProperty = ExpressionView.DockingOrientationPropertyKey.DependencyProperty;
            ExpressionView.IsSizableWhenFloatingProperty = DependencyProperty.RegisterAttached("IsSizableWhenFloating", typeof(bool), typeof(ExpressionView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
            ExpressionView.IsForcedInvisibleProperty = DependencyProperty.Register("IsForcedInvisible", typeof(bool), typeof(ExpressionView), new PropertyMetadata(false, new PropertyChangedCallback(ExpressionView.OnVisibilityChanged)));
            ExpressionView.IsDesiredVisibleProperty = DependencyProperty.Register("IsDesiredVisible", typeof(bool), typeof(ExpressionView), new PropertyMetadata(false, new PropertyChangedCallback(ExpressionView.OnVisibilityChanged)));
            ExpressionView.WasSelectedBeforeAutoHideProperty = DependencyProperty.RegisterAttached("WasSelectedBeforeAutoHide", typeof(bool), typeof(ExpressionView), new PropertyMetadata(false));
            ViewElement.IsVisibleProperty.OverrideMetadata(typeof(ExpressionView), new FrameworkPropertyMetadata(new PropertyChangedCallback(ExpressionView.OnIsVisibleChanged)));
        }

        public ExpressionView()
        {
            base.ParentChanged += new EventHandler(this.ParentChangedHandler);
        }

        public override ICustomXmlSerializer CreateSerializer()
        {
            return new ExpressionViewCustomSerializer(this);
        }

        public static bool GetIsAutoHideable(View target)
        {
            return (bool)target.GetValue(ExpressionView.IsAutoHideableProperty);
        }

        public static bool GetIsSizableWhenFloating(DependencyObject target)
        {
            return (bool)target.GetValue(ExpressionView.IsSizableWhenFloatingProperty);
        }

        bool Microsoft.VisualStudio.PlatformUI.Shell.IDependencyObjectCustomSerializerAccess.ShouldSerializeProperty(DependencyProperty dp)
        {
            if (dp == View.TitleProperty)
            {
                return false;
            }
            return true;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ExpressionView isVisible = (ExpressionView)obj;
            if (!isVisible.isForcingVisibility)
            {
                isVisible.IsDesiredVisible = isVisible.IsVisible;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == View.ContentProperty)
            {
                this.UpdateContentAdvancedLayoutProperties();
            }
            if (e.Property == ExpressionView.IsTitleHiddenProperty || e.Property == ExpressionView.DockingOrientationProperty)
            {
                this.NotifyPropertyChanged("IsVerticalGripperVisible");
                this.NotifyPropertyChanged("IsHorizontalGripperVisible");
            }
        }

        private static void OnVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ExpressionView expressionView = (ExpressionView)obj;
            expressionView.isForcingVisibility = true;
            expressionView.IsVisible = (!expressionView.IsDesiredVisible ? false : !expressionView.IsForcedInvisible);
            expressionView.isForcingVisibility = false;
        }

        private void ParentChangedHandler(object sender, EventArgs e)
        {
            this.UpdateContentAdvancedLayoutProperties();
        }

        public static void SetIsAutoHideable(View target, bool value)
        {
            target.SetValue(ExpressionView.IsAutoHideableProperty, value);
        }

        public static void SetIsSizableWhenFloating(DependencyObject target, object value)
        {
            target.SetValue(ExpressionView.IsSizableWhenFloatingProperty, value);
        }

        private void UpdateContentAdvancedLayoutProperties()
        {
            ViewGroup parent = base.Parent;
            while (parent != null && !(parent is DockGroup))
            {
                parent = parent.Parent;
            }
            DockGroup dockGroup = parent as DockGroup;
            if (dockGroup == null)
            {
                this.DockingOrientation = null;
            }
            else
            {
                this.DockingOrientation = new Orientation?(dockGroup.Orientation);
            }
            IAdvancedLayoutPanel content = base.Content as IAdvancedLayoutPanel;
            if (content != null)
            {
                content.DockingOrientation = this.DockingOrientation;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}