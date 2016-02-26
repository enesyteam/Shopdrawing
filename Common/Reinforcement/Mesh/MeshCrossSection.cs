////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Media;
////using System.Windows.Shapes;
////using DynamicGeometry;
////using M = System.Math;

//using Shopdrawing.Utilities;
////using System;

////namespace Shopdrawing.Reinforcement
////{
////    public class MeshCrossSection : CompositeFigure, IEnumerable<Bar>
////    {
////        public MeshContainer MeshContainer { get; set; }

////        public SectionDirection SectionDirection { get; set; }
////        public RectangleMesh Parent { get; set; }

////        public List<Bar> Items { get; set; }

////        public List<Bar> HorizontalBars { get; set; }
////        public List<BarCrossView> VerticalBars { get; set; }


////        public double HorizontalBarsSpacing { get; set; }
////        public double VerticalBarsSpacing { get; set; }

////        #region Constructors
////        public MeshCrossSection()
////        {
////        }
////        public MeshCrossSection(DynamicGeometry.Drawing drawing)
////        {
////            Drawing = drawing;


////        }
////        public MeshCrossSection(DynamicGeometry.Drawing drawing, RectangleMesh parent)
////        {
////            Drawing = drawing;
////            Parent = parent;

////            Items = new List<Bar>();
////            VerticalBars = new List<BarCrossView>();
////            HorizontalBars = new List<Bar>();
////            // 
////            this.HorizontalBarsSpacing = Parent.MeshLines.HorizontalBarsSpacing;
////            this.VerticalBarsSpacing = Parent.MeshLines.VerticalBarsSpacing;
////        }

////        #endregion

////        #region Public Methods

////        //
////        public IEnumerable<double> GetVisibleXVerticalBars(double horizontalLength)
////        {
////            //               |------------------->
////            List<double> result = new List<double>();
////            for (var x = MeshContainer.Focus.X;
////                    x <= MeshContainer.Focus.X + horizontalLength / 2 - MeshContainer.Cover;
////                    x += VerticalBarsSpacing)
////            {
////                if (x == MeshContainer.Focus.X) continue;
////                result.Add(x);
////            }

////            // Middle
////            result.Add(MeshContainer.Focus.X);

////            //<----------------|
////            for (var x = MeshContainer.Focus.X;
////                    x >= MeshContainer.Focus.X - horizontalLength / 2 + MeshContainer.Cover;
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
////            double length = SectionDirection == Reinforcement.SectionDirection.HorizontalSection ?
////                MeshContainer.Width : MeshContainer.Height;

////            //double diameter = SectionDirection == Reinforcement.SectionDirection.HorizontalSection ?
////            //    Parent.MeshLines.VerticalBars[0].Diameter : Parent.MeshLines.HorizontalBars[0].Diameter;
////            double diameter = 10;


////            var xPoints = GetVisibleXVerticalBars(length);
////            foreach (var x in xPoints)
////            {
////                var p1 = Factory.CreateFreePoint(Drawing,
////                    new Point(x, 0));
////                var p2 = Factory.CreateFreePoint(Drawing,
////                    PointsHelper.Offset(p1.Coordinates, diameter, 0));
////                List<IFigure> listPoints = new List<IFigure>();
////                listPoints.Add(p1);
////                listPoints.Add(p2);

////                var b = BarCreator.CreateBarCross(Drawing, listPoints);
////                b.Name = "B1";
////                b.Style = this.Style;
////                VerticalBars.Add(b);
////            }


////        }
////        public override void OnAddingToDrawing(DynamicGeometry.Drawing drawing)
////        {
////            foreach (BarCrossView b in VerticalBars)
////            {
////                Children.Add(b);
////            }
////        }

////        public override string ToString()
////        {
////            return "Rectangle Mesh";
////        }

////        #endregion



////        // IEnumerable<Bar> Members
////        public IEnumerator<Bar> GetEnumerator()
////        {
////            return Items.GetEnumerator();
////        }

////        //IEnumerable Members
////        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
////        {
////            throw new NotImplementedException();
////        }

////    }
////}
