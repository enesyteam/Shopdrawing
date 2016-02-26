////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Media;
////using DynamicGeometry;
//using Shopdrawing.Utilities;

////namespace Shopdrawing.Reinforcement
////{
////    ///// <summary>
////    ///// Hướng bố trí các thanh thép
////    ///// trong một lưới thép thường gồm 2 hướng
////    ///// </summary>
////    //public enum Direction
////    //{
////    //    /// <summary>
////    //    /// Phương 1
////    //    /// </summary>
////    //    Direction1 =0,
////    //    /// <summary>
////    //    /// Phương 2
////    //    /// </summary>
////    //    Direction2 = 1,
////    //    /// <summary>
////    //    /// Phương 3 (Phương nhìn thẳng vào lưới)
////    //    /// </summary>
////    //    Direction3 = 2,
    
////    //}

////    /// <summary>
////    /// ------> Direction1
////    ///     |   |   |   |   |   |   |   |   |   |   |   |   |
////    ///     -------------------------------------------------
////    ///     |   |   |   |   |   |   |   |   |   |   |   |   |
////    ///     -------------------------------------------------
////    ///     |   |   |   |   |   |   |   |   |   |   |   |   |
////    /// Lưới thép
////    /// </summary>
////    public class Mesh : CompositeFigure, ICustomPropertyProvider, ICustomMethodProvider
////    {
////        public Direction Direction { get; set; }
////        public List<Bar> BarsOnDirection1 { get; set; }
////        public List<Bar> BarsOnDirection2 { get; set; }

////        public Point CenterPoint { get; set; }

////        #region Test
////        public double Length { get; set; }
////        public double Offset { get; set; }
////        #endregion

////        #region Constructors
////        public Mesh()
////        {
////        }
////        public Mesh(DynamicGeometry.Drawing drawing)
////        {
////            Drawing = drawing;
////        }
////        #endregion

////        #region Public Methods 
////        public void CreateCrossSection(Direction direction)
////        {
////            #region Styles
////            LineStyle BarsStyle = new LineStyle()
////            {
////                Color = Colors.Red,
////                Name = "StructureLines",
////                StrokeWidth = 0.2,
////                StrokeDashArray = null
////            };

////            TextStyle labelsStyle = new TextStyle()
////            {
////                Color = Color.FromArgb(255, 128, 128, 255),
////                //Color = Color.FromArgb(255, 201, 201, 201),
////                FontSize = 12.0,
////                Name = "LabelsStyle"
////            };
////            #endregion

////            switch (direction)
////            { 
////                case Reinforcement.Direction.Up:

////                    //
////                    Point basePoint = this.CenterPoint;
////                    var p1 = Factory.CreateFreePoint(Drawing, basePoint);
////                    var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(basePoint, Length, 0));
////                    List<IFigure> listPoints = new List<IFigure>();
////                    listPoints.Add(p1);
////                    listPoints.Add(p2);

////                    var t = Factory.CreateSegment(Drawing, listPoints);
////                    t.Style = BarsStyle;
////                    Drawing.Figures.Add(t);
////                    this.Dependencies.Add(t);

////                    Point basePoint2 = PointsHelper.Offset(basePoint, 0, Offset - 2*Settings.Cover);
////                    var p3 = Factory.CreateFreePoint(Drawing, basePoint2);
////                    var p4 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(basePoint2, Length, 0));
////                    List<IFigure> listPoints2 = new List<IFigure>();
////                    listPoints2.Add(p3);
////                    listPoints2.Add(p4);

////                    var t2 = Factory.CreateSegment(Drawing, listPoints2);
////                    t2.Style = BarsStyle;
////                    Drawing.Figures.Add(t2);
////                    this.Dependencies.Add(t2);
////                    //

////                    break;

////            }
        
////        }
////        #endregion






////        public static FrameworkElement GetIcon()
////        {
////#if TABULAPLAYER
////            return null;    // never used and my player excludes IconBuilder
////#else
////            var builder = IconBuilder
////                .BuildIcon()
////                .Polygon(
////                    new SolidColorBrush(Colors.Blue),
////                    new SolidColorBrush(Colors.Blue),
////                    new Point(0.5, 0),
////                    new Point(0.4, 0.2),
////                    new Point(0.6, 0.2))
////                .Polygon(
////                    new SolidColorBrush(Colors.Blue),
////                    new SolidColorBrush(Colors.Blue),
////                    new Point(1, 0.5),
////                    new Point(0.8, 0.4),
////                    new Point(0.8, 0.6))
////                .Line(Color.FromArgb(255, 0, 0, 255), 0.5, 0, 0.5, 1)
////                .Line(Color.FromArgb(255, 0, 0, 255), 0, 0.5, 1, 0.5);
////            for (double i = 0.1; i < 1; i += 0.2)
////            {
////                builder.Line(Color.FromArgb(100, 0, 0, 255), i, 0, i, 1);
////                builder.Line(Color.FromArgb(100, 0, 0, 255), 0, i, 1, i);
////            }
////            return builder.Canvas;
////#endif
////        }
////        public IEnumerable<IValueProvider> GetProperties()
////        {
////            return PropertyDiscoveryStrategy.GetValuesFromProperties(this, "Visible", "Locked", "ShowGrid");
////        }

////        public IEnumerable<IOperationDescription> GetMethods()
////        {
////            return Enumerable.Empty<IOperationDescription>();
////        }


////    }
////}
