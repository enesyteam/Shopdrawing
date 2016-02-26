using System;
using System.Windows;
using System.Windows.Controls;

namespace Shopdrawing.Controls
{
    public class ShopdrawingToolbar : ToolBar
    {
        /// <summary>
        /// TestTimer Dependency Property
        /// </summary>
        public static readonly DependencyProperty DrawingProperty =
            DependencyProperty.Register("Drawing", typeof(DynamicGeometry.Drawing), typeof(ShopdrawingToolbar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TestTimer property.  This dependency property 
        /// indicates a test timer that elapses evry one second (just for binding test).
        /// </summary>
        public DynamicGeometry.Drawing Drawing
        {
            get { return (DynamicGeometry.Drawing)GetValue(DrawingProperty); }
            set { SetValue(DrawingProperty, value); }
        }
    }
}
