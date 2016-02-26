using Microsoft.Expression.Framework.UserInterface;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.Framework.Workspaces.Extension
{
    public class ExpressionFloatingWindow : FloatingWindow
    {
        private readonly static DependencyPropertyKey IsSizablePropertyKey;

        public readonly static DependencyProperty IsSizableProperty;

        public bool IsSizable
        {
            get
            {
                return (bool)base.GetValue(ExpressionFloatingWindow.IsSizableProperty);
            }
            private set
            {
                base.SetValue(ExpressionFloatingWindow.IsSizablePropertyKey, value);
            }
        }

        static ExpressionFloatingWindow()
        {
            ExpressionFloatingWindow.IsSizablePropertyKey = DependencyProperty.RegisterReadOnly("IsSizable", typeof(bool), typeof(ExpressionFloatingWindow), new PropertyMetadata(true));
            ExpressionFloatingWindow.IsSizableProperty = ExpressionFloatingWindow.IsSizablePropertyKey.DependencyProperty;
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpressionFloatingWindow), new FrameworkPropertyMetadata(typeof(ExpressionFloatingWindow)));
        }

        public ExpressionFloatingWindow(bool isSizable)
        {
            this.IsSizable = isSizable;
            base.SizeToContent = (this.IsSizable ? SizeToContent.Manual : SizeToContent.WidthAndHeight);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            FocusScopeManager.DenyNextFocusChange();
        }
    }
}