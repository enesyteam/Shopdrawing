////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Media;
////using System.Windows.Shapes;
////using DynamicGeometry;
////using M = System.Math;

////using Shopdrawing.Utilities;
////using System;

////namespace Shopdrawing.Reinforcement
////{
////    public class RectangleMeshLinesCollection : MeshBase, IEnumerable<BarPlanView>
////    {
////        public IFigure MeshBound { get; set; }

////        public List<BarPlanView> Items { get; set; }

////        public List<BarPlanView> HorizontalBars { get; set; }
////        public List<BarPlanView> VerticalBars { get; set; }

////        public LineStyle StructureLine { get; set; }

////        private bool ShowMeshContainer = false;
////        [PropertyGridVisible]
////        [PropertyGridName("Show Bounds")]
////        public bool ShowAxes
////        {
////            get
////            {
////                return ShowMeshContainer;
////            }
////            set
////            {
////                ShowMeshContainer = value;
////                if (value && this.Drawing != null)
////                {
////                    MeshBound.Visible = value;
////                }
////            }
////        }


////        #region Constructors
////        public RectangleMeshLinesCollection()
////        {
////        }
////        public RectangleMeshLinesCollection(DynamicGeometry.Drawing drawing)
////        {
////            Drawing = drawing;
////        }
////        public RectangleMeshLinesCollection(DynamicGeometry.Drawing drawing, MeshContainer meshContainer)
////        {
////            Drawing = drawing;
////            MeshContainer = meshContainer;
////            //ShowMeshContainer = false;

////            Items = new List<BarPlanView>();
////            VerticalBars = new List<BarPlanView>();
////            HorizontalBars = new List<BarPlanView>();
////            // 
////            HorizontalBarsSpacing = 10;
////            VerticalBarsSpacing = 10;
////        }

////        #endregion

////        #region Public Methods

////        //
////        public IEnumerable<double> GetVisibleXVerticalBars()
////        {
////            //               |------------------->
////            List<double> result = new List<double>();
////            for (var x = MeshContainer.Focus.X;
////                    x <= MeshContainer.Focus.X + MeshContainer.Width / 2 - MeshContainer.Cover;
////                    x += VerticalBarsSpacing)
////            {
////                if (x == MeshContainer.Focus.X) continue;
////                result.Add(x);
////            }

////            // Middle
////            result.Add(MeshContainer.Focus.X);

////            //<----------------|
////            for (var x = MeshContainer.Focus.X;
////                    x >= MeshContainer.Focus.X - MeshContainer.Width / 2 + MeshContainer.Cover;
////                    x -= VerticalBarsSpacing)
////            {
////                if (x == MeshContainer.Focus.X) continue;
////                result.Add(x);
////            }
////            return result;
////        }
////        public IEnumerable<double> GetVisibleYHorizontalBars()
////        {
////            List<double> result = new List<double>();

////            for (var y = MeshContainer.Focus.Y;
////                y <= MeshContainer.Focus.Y + MeshContainer.Height / 2 - MeshContainer.Cover;
////                y += HorizontalBarsSpacing)
////            {
////                if (y == MeshContainer.Focus.Y) continue;
////                result.Add(y);
////            }

////            // Middle
////            result.Add(MeshContainer.Focus.Y);

////            for (var y = MeshContainer.Focus.Y;
////                y >= MeshContainer.Focus.Y - MeshContainer.Height / 2 + MeshContainer.Cover;
////                y -= HorizontalBarsSpacing)
////            {
////                if (y == MeshContainer.Focus.Y) continue;
////                result.Add(y);
////            }

////            return result;
////        }


////        public void CreateRectangleMeshLines()
////        {
////            var xPoints = GetVisibleXVerticalBars();
////            var yPoints = GetVisibleYHorizontalBars();
////            foreach (var x in xPoints)
////            {
////                var p1 = Factory.CreateFreePoint(Drawing,
////                    new Point(x, MeshContainer.Focus.Y - MeshContainer.Height / 2 + MeshContainer.Cover));
////                var p2 = Factory.CreateFreePoint(Drawing,
////                    new Point(x, MeshContainer.Focus.Y + MeshContainer.Height / 2 - MeshContainer.Cover));
////                List<IFigure> listPoints = new List<IFigure>();
////                listPoints.Add(p1, p2);

////                var b = BarCreator.CreateBarPlanView(Drawing, listPoints);
////                b.Length = PointsHelper.Distance(p1.Coordinates, p2.Coordinates);
////                b.Name = "B1";
////                b.Style = this.Style;
////                VerticalBars.Add(b);
////                Items.Add(b);
////                //foreach(IPoint p in b.Dependencies)
////                //    this.Dependencies.Add(p);
////            }

////            foreach (var y in yPoints)
////            {
////                var p1 = Factory.CreateFreePoint(Drawing,
////                    new Point(MeshContainer.Focus.X - MeshContainer.Width / 2 + MeshContainer.Cover, y));
////                var p2 = Factory.CreateFreePoint(Drawing,
////                    new Point(MeshContainer.Focus.X + MeshContainer.Width / 2 - MeshContainer.Cover, y));
////                List<IFigure> listPoints = new List<IFigure>();
////                listPoints.Add(p1);
////                listPoints.Add(p2);

////                var b = BarCreator.CreateBarPlanView(Drawing, listPoints);
////                b.Length = PointsHelper.Distance(p1.Coordinates, p2.Coordinates);
////                b.Name = "B2";
////                b.Style = this.Style;
////                HorizontalBars.Add(b);
////                Items.Add(b);
////                //foreach (IPoint p in b.Dependencies)
////                //    this.Dependencies.Add(p);
////            }
        
////        }
////        public override void OnAddingToDrawing(DynamicGeometry.Drawing drawing)
////        {
////            foreach (BarPlanView b in VerticalBars)
////            {
////                Children.Add(b);
////            }

////            foreach (BarPlanView b in HorizontalBars)
////            {
////                Children.Add(b);
////            }
////            // Bounds
////            StructureLine = new LineStyle() 
////            {
////                Color = Colors.LightGray,
////                Name = "StructureLines",
////                StrokeWidth = 0.25,
////                StrokeDashArray = new DoubleCollection(new double[]{2, 2})
            
////            };
////            var p1 = Factory.CreateFreePoint(Drawing, 
////                PointsHelper.Offset(MeshContainer.Focus, -MeshContainer.Width/2, -MeshContainer.Height/2));
////            var p2 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, -MeshContainer.Width / 2, MeshContainer.Height / 2));
////            var p3 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, MeshContainer.Width / 2, MeshContainer.Height / 2));
////            var p4 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, MeshContainer.Width / 2, -MeshContainer.Height / 2));
////            List<IFigure> pointsList = new List<IFigure>();
////            pointsList.Add(p1);
////            pointsList.Add(p2);
////            pointsList.Add(p3);
////            pointsList.Add(p4);
////            pointsList.Add(p1);
////            var polyline = Factory.CreatePolyline(Drawing, pointsList);
////            polyline.Style = StructureLine;
////            //polyline.Visible = this.Selected;
////            Children.Add(polyline);
////            MeshBound = polyline;
////            MeshBound.Dependencies = pointsList;
////            MeshBound.Visible = ShowMeshContainer;
////            //polyline.Visible = ShowMeshContainer;
////            foreach (IFigure f in polyline.Dependencies)
////            {
////                this.Dependencies.Add(f);
////            }
////        }
////        public override bool Selected
////        {
////            get
////            {
                
////                return base.Selected;
////            }
////            set
////            {
////                base.Selected = value;
////                MeshBound.Visible = value;
////            }
////        }


////        public override string ToString()
////        {
////            return "Rectangle Mesh";
////        }

////        #endregion

////        #region Tests
////        //[PropertyGridVisible]
////        //[PropertyGridName("Test1")]
////        //public void Test1()
////        //{
////        //    StringBuilder sb = new StringBuilder();
////        //    foreach (var b in VerticalBars)
////        //    {
////        //        sb.AppendLine(b.FullName + ": " + b.Length);
            
////        //    }
////        //    MessageBox.Show(sb.ToString());
        
////        //}
////        [PropertyGridVisible]
////        [PropertyGridName("Test2")]
////        public void Test2()
////        {
////            var p1 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, -MeshContainer.Width / 2 - 10, -MeshContainer.Height / 2 - 10));
////            var p2 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, -MeshContainer.Width / 2 - 10, MeshContainer.Height / 2 + 10));
////            var p3 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, MeshContainer.Width / 2 + 10, MeshContainer.Height / 2 + 10));
////            var p4 = Factory.CreateFreePoint(Drawing,
////                PointsHelper.Offset(MeshContainer.Focus, MeshContainer.Width / 2 + 10, -MeshContainer.Height / 2 - 10));
////            List<IFigure> pointsList = new List<IFigure>();
////            pointsList.Add(p1);
////            pointsList.Add(p2);
////            pointsList.Add(p3);
////            pointsList.Add(p4);
////            pointsList.Add(p1);
////            var polyline = Factory.CreatePolyline(Drawing, pointsList);


////            //var drawing = oldArc.Drawing;
////            //newArc.Style = oldArc.Style;
////            //newArc.Clockwise = oldArc.Clockwise;
////            //Actions.ReplaceWithNew(MeshBound, polyline);
////            //Actions.Add(Drawing, polyline);
////            Actions.ReplaceWithNew(MeshBound, polyline);
////            //this.Children.Replace(MeshBound, polyline);
////            //drawing.RaiseUserIsAddingFigures(new DynamicGeometry.Drawing.UIAFEventArgs() 
////            //{ Figures = polyline.AsEnumerable<IFigure>() });
                
////        }
////        #endregion
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

////        public override IFigure HitTest(Point point)
////        {
////            List<Point> pList = new List<System.Windows.Point>();

////            foreach (IPoint p in this.Dependencies)
////            {
////                pList.Add(p.Coordinates);
////            }
////            bool isInside = pList.IsPointInPolygon(point);
////            return isInside ? this : null;
////        }
////    }
////}
