////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Media;
////using System.Windows.Shapes;
////using DynamicGeometry;
////using Shopdrawing.Utilities;

////namespace Shopdrawing.Reinforcement
////{
////    /// <summary>
////    /// Lưới cốt thép hình vuông (Hướng nhìn: Direction3)
////    /// </summary>
////    public class RectangleMesh : MeshBase, IHaveCrossSection, IEnumerable<BarPlanView>
////    {
////        public RectangleMeshLinesCollection MeshLines { get; set; }

////        /// <summary>
////        /// Mặt cắt lưới theo phương ngang
////        /// </summary>
////        public MeshSection HorizontalMeshSection { get; set; }
////        /// <summary>
////        /// Mặt cắt lưới theo phương đứng
////        /// </summary>
////        public MeshSection VerticalMeshSection { get; set; }

////        public List<BarPlanView> Items { get; set; }

////        #region Constructors
////        public RectangleMesh(DynamicGeometry.Drawing drawing, MeshContainer meshContainer)
////        {
////            this.Drawing = drawing;
////            this.MeshContainer = meshContainer;
////            Items = new List<BarPlanView>();

////            //CreateMesh();
////        }

////        public RectangleMesh(DynamicGeometry.Drawing drawing, 
////            MeshSection horizontalMeshSection, MeshSection verticalMeshSection, Point insertPoint)
////            //: base(drawing, horizontalMeshSection, verticalMeshSection)
////        {
////            this.Drawing = drawing;
////            this.HorizontalMeshSection = horizontalMeshSection;
////            this.VerticalMeshSection = verticalMeshSection;
////            this.InsertPoint = insertPoint;

////            Items = new List<BarPlanView>();

////            //CreateRectangleMesh();
////        }


////        public LineStyle MeshStyle = new LineStyle()
////            {
////                Color = Colors.Red,
////                Name = "meshStyle",
////                StrokeWidth = 0.5,
////                StrokeDashArray = null
////            };

////        [PropertyGridVisible]
////        public RectangleMeshLinesCollection CreateMesh2()
////        {
////            MeshStyle = new LineStyle()
////            {
////                Color = Colors.Red,
////                Name = "meshStyle",
////                StrokeWidth = 0.5,
////                StrokeDashArray = null
////            };

////            MeshLines = new RectangleMeshLinesCollection(this.Drawing, this.MeshContainer);
////            MeshLines.Style = MeshStyle;
////            MeshLines.CreateRectangleMeshLines();

////            Children.Add(MeshLines);
////            Actions.Add(Drawing, MeshLines);
////            return MeshLines;
////        }

////        #endregion

////        private bool visible = false;
////        public override bool Visible
////        {
////            get
////            {
////                return visible;
////            }
////            set
////            {
////                visible = value;
////                MeshLines.Visible = value;
////                if (value && this.Drawing != null)
////                {
////                    UpdateVisual();
////                }
////            }
////        }

//////        public static FrameworkElement GetIcon()
//////        {
//////#if TABULAPLAYER
//////            return null;    // never used and my player excludes IconBuilder
//////#else
//////            var builder = IconBuilder
//////                .BuildIcon()
//////                .Polygon(
//////                    new SolidColorBrush(Colors.Blue),
//////                    new SolidColorBrush(Colors.Blue),
//////                    new Point(0.5, 0),
//////                    new Point(0.4, 0.2),
//////                    new Point(0.6, 0.2))
//////                .Polygon(
//////                    new SolidColorBrush(Colors.Blue),
//////                    new SolidColorBrush(Colors.Blue),
//////                    new Point(1, 0.5),
//////                    new Point(0.8, 0.4),
//////                    new Point(0.8, 0.6))
//////                .Line(Color.FromArgb(255, 0, 0, 255), 0.5, 0, 0.5, 1)
//////                .Line(Color.FromArgb(255, 0, 0, 255), 0, 0.5, 1, 0.5);
//////            for (double i = 0.1; i < 1; i += 0.2)
//////            {
//////                builder.Line(Color.FromArgb(100, 0, 0, 255), i, 0, i, 1);
//////                builder.Line(Color.FromArgb(100, 0, 0, 255), 0, i, 1, i);
//////            }
//////            return builder.Canvas;
//////#endif
//////        }

////        public override string ToString()
////        {
////            return "Rectangle Mesh";
////        }


////        public SectionDirection SectionDirection { get; set; }
////        public void GetHorizontalSection(){}
////        public void GetVerticalSection() { }

////        #region Vẽ mặt bằng lưới
////        public RectangleMeshDirection DrawDirection { get; set; }
////        public void CreateRectangleMesh()
////        {
////            #region Horizontal Bars
////            var hCount = HorizontalMeshSection.Count;
////            var hLength = HorizontalMeshSection.Length;
////            var hStep = HorizontalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                for (int i = 0; i < hCount; i++)
////                {
////                    var linePoints = new List<IFigure>();

////                    var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, i * hStep, 0));
////                    var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, 0, -hLength));

////                    linePoints.Add(p1, p2);

////                    var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                    bar.Style = MeshStyle;
////                    Items.Add(bar);
////                    Children.Add(bar);
////                    Dependencies.Add(p1, p2);
////                    Actions.Add(Drawing, bar);
////                }
////            }
////            #endregion

////            #region Vertical Bars
////            var vCount = VerticalMeshSection.Count;
////            var vLength = VerticalMeshSection.Length;
////            var vStep = VerticalMeshSection.UniformSpacing;
////            using (Drawing.ActionManager.CreateTransaction())
////            {
////                for (int i = 0; i < vCount; i++)
////                {
////                    var linePoints = new List<IFigure>();

////                    var p1 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(InsertPoint, 0, -i * vStep));
////                    var p2 = Factory.CreateFreePoint(Drawing, PointsHelper.Offset(p1.Coordinates, vLength, 0));

////                    linePoints.Add(p1, p2);

////                    var bar = BarCreator.CreateBarPlanView(Drawing, linePoints);
////                    bar.Style = MeshStyle;
////                    Items.Add(bar);
////                    Children.Add(bar);
////                    Dependencies.Add(p1, p2);
////                    Actions.Add(Drawing, bar);
////                }
////            }
////            #endregion



////            GetBounds();
////        }
////        #endregion

////        //public Point InsertPoint { get; set; }
////        public Point LowerLeft { get; set; }
////        public Point UpperLeft { get; set; }
////        public Point LowerRight { get; set; }
////        public Point UpperRight { get; set; }
////        public List<Point> PointsBound { get; set; }
////        public void GetBounds()
////        {
////            double minX, minY, maxX, maxY;
////            List<IPoint> lps = new List<IPoint>();
////            List<double> Xs = new List<double>();
////            List<double> Ys = new List<double>();

////            foreach (IPoint p in this.Dependencies)
////            {
////                Xs.Add(p.Coordinates.X);
////                Ys.Add(p.Coordinates.Y);
////            }
////            minX = Xs.Min(); minY = Ys.Min();
////            maxX = Xs.Max(); maxY = Ys.Max();

////            LowerLeft = new Point(minX, minY);
////            UpperLeft = new Point(minX, maxY);
////            LowerRight = new Point(maxX, minY);
////            UpperRight = new Point(maxX, maxY);
////            PointsBound = new List<Point>();
////            PointsBound.Add(LowerLeft, UpperLeft, UpperRight, LowerRight, LowerLeft);

////            //foreach (Point p in PointsBound)
////            //{
////            //    Actions.Add(Drawing, Factory.CreateFreePoint(Drawing, p));
////            //}
////        }

////        //public override void OnAddingToDrawing(DynamicGeometry.Drawing drawing)
////        //{
////        //    foreach (BarPlanView b in Items)
////        //        Children.Add(b);
////        //}
////        ////public override IFigure HitTest(Point point)
////        ////{
////        ////    List<Point> pList = new List<System.Windows.Point>();

////        ////    //foreach (IPoint p in this.Dependencies)
////        ////    //{
////        ////    //    pList.Add(p.Coordinates);
////        ////    //}


////        ////    bool isInside = PointsBound.IsPointInPolygon(point);
////        ////    return isInside ? this : null;
////        ////}
////        // IEnumerable<Bar> Members
////        public IEnumerator<BarPlanView> GetEnumerator()
////        {
////            return Items.GetEnumerator();
////        }
////        //IEnumerable Members
////        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
////        {
////            throw new NotImplementedException();
////        }

////    }

////    public enum RectangleMeshDirection
////    { 
////        Horizontal=0, Vertical=1
////    }
////}
