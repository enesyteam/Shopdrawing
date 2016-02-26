using System;
using System.Windows.Media;
using DynamicGeometry;

namespace Shopdrawing.Settings
{
    public partial class DefaultStyles
    {
        
        static DefaultStyles instance = new DefaultStyles();
        public static DefaultStyles Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        #region Styles Default

        public static LineStyle BarStyle = new LineStyle()
        {
            Color = Colors.Red,
            Name = "BarStyle",
            StrokeWidth = 0.5,
            StrokeDashArray = null
        };
        public static LineStyle StructureLineStyle = new LineStyle()
        {
            Color = Colors.White,
            Name = "StructureLineStyle",
            StrokeWidth = 1,
            StrokeDashArray = null,// new DoubleCollection(new double[]{5,1}),
        };
        public static LineStyle DimensionLineStyle = new LineStyle()
        {
            Color = Colors.Gray,
            Name = "DimensionLineStyle",
            StrokeWidth = 0.5,
            StrokeDashArray = null,// new DoubleCollection(new double[]{5,1}),
        };
        public static LineStyle StructureLineStyleSelected = new LineStyle()
        {
            Color = Colors.White,
            Name = "StructureLineStyle",
            StrokeWidth = 1,
            //StrokeDashArray = new DoubleCollection(new double[]{5,1}),
        };
        TextStyle Label12 = new TextStyle()
        {
            Color = Color.FromArgb(255, 128, 128, 255),
            //Color = Color.FromArgb(255, 201, 201, 201),
            FontSize = 12.0,
            Name = "Label12"
        };
        // Dim Text
        public static TextStyle dimTextStyle = new TextStyle()
        {
            Color = Colors.Red,
            //FontSize = 100,
            Name = "LabelsStyle",
        };

        public static ShapeStyle ArrowHead = new ShapeStyle()
        {
            Fill = Brushes.Gray,
        };

        #region Dimensions
        public static string DefaultDimstyleName = "Standard";
        #endregion
        #endregion
    }
}
