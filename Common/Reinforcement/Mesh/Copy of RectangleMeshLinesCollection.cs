////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Media;
////using System.Windows.Shapes;
////using DynamicGeometry;
////using M = System.Math;

////namespace Shopdrawing.Reinforcement
////{
////    public class RectangleMeshLinesCollection : MeshLinesCollection
////    {
////        List<Line> Lines = new List<Line>();

////        public MeshContainer MeshContainer { get; set; }

////        #region Các phương thức tìm điểm
////        public double MinimalVisibleX { get; set; }
////        public double MinimalVisibleY { get; set; }
////        public double MaximalVisibleX { get; set; }
////        public double MaximalVisibleY { get; set; }
////        public IEnumerable<double> GetVisibleXPoints()
////        {
////            for (var x = M.Ceiling(MinimalVisibleX); x <= M.Floor(MaximalVisibleX); x++)
////            {
////                yield return x;
////            }
////        }
////        public IEnumerable<double> GetVisibleYPoints()
////        {
////            for (var y = M.Ceiling(MinimalVisibleY); y <= M.Floor(MaximalVisibleY); y++)
////            {
////                yield return y;
////            }
////        }
////        #endregion

////        public override bool Visible
////        {
////            get
////            {
////                return base.Visible;
////            }
////            set
////            {
////                base.Visible = value;
////                foreach (var line in Lines)
////                {
////                    line.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
////                }
////            }
////        }

////        //public override void GetRegionToDraw()
////        //{
////        //    MinimalVisibleX = MeshContainer.Focus.X - MeshContainer.Width / 2;
////        //    MaximalVisibleX = MeshContainer.Focus.X + MeshContainer.Width / 2;
////        //    MinimalVisibleY = MeshContainer.Focus.Y - MeshContainer.Height / 2;
////        //    MaximalVisibleY = MeshContainer.Focus.Y + MeshContainer.Height / 2;

////        //    StringBuilder sb = new StringBuilder();
////        //    sb.AppendLine("Từ điểm: X= " + MinimalVisibleX + " Y= " + MinimalVisibleY);
////        //    sb.AppendLine("Đến điểm: X= " + MaximalVisibleX + " Y= " + MaximalVisibleY);
////        //    MessageBox.Show(sb.ToString());
            
////        //}

////        public override void UpdateVisual()
////        {
////            CoordinateSystem coordinateSystem = Drawing.CoordinateSystem;
////            var xPoints = this.GetVisibleXPoints();
////            var yPoints = this.GetVisibleYPoints();


////            int count = xPoints.Count() + yPoints.Count();
////            if (Lines.Count < count)
////            {
////                AddNewLines(count - Lines.Count, coordinateSystem);
////            }
////            else if (Lines.Count > count)
////            {
////                RemoveExcessLines(Lines.Count - count, coordinateSystem);
////            }

////            int i = 0;
////            foreach (var x in xPoints)
////            {
////                MoveLineX(Lines[i++], x, coordinateSystem);
////            }
////            foreach (var y in yPoints)
////            {
////                MoveLineY(Lines[i++], y, coordinateSystem);
////            }
////        }

////        public override void OnAddingToCanvas(Canvas newContainer)
////        {
////            foreach (var line in Lines)
////            {
////                if (line.Parent == null)
////                {
////                    newContainer.Children.Add(line);
////                }
////            }
////        }

////        public override void OnRemovingFromCanvas(Canvas leavingContainer)
////        {
////            foreach (var line in Lines)
////            {
////                leavingContainer.Children.Remove(line);
////            }
////        }

////        void MoveLineX(Line line, double x, CoordinateSystem coordinateSystem)
////        {
////            line.Move(
////                x,
////                coordinateSystem.MinimalVisibleY,
////                x,
////                coordinateSystem.MaximalVisibleY,
////                coordinateSystem);
////        }

////        void MoveLineY(Line line, double y, CoordinateSystem coordinateSystem)
////        {
////            line.Move(
////                coordinateSystem.MinimalVisibleX,
////                y,
////                coordinateSystem.MaximalVisibleX,
////                y,
////                coordinateSystem);
////        }

////        void RemoveExcessLines(int count, CoordinateSystem coordinateSystem)
////        {
////            for (int i = Lines.Count - count; i < Lines.Count; i++)
////            {
////                Drawing.Canvas.Children.Remove(Lines[i]);
////            }
////            Lines.RemoveRange(Lines.Count - count, count);
////        }

////        void AddNewLines(int count, CoordinateSystem coordinateSystem)
////        {
////            for (int i = 0; i < count; i++)
////            {
////                var newLine = new Line()
////                {
////                    Visibility = this.Visible.ToVisibility()
////                };
////                Canvas.SetZIndex(newLine, (int)ZOrder.Grid);
////                Lines.Add(newLine);
////                Drawing.Canvas.Children.Add(newLine);
////                newLine.Apply(this.Style.GetWpfStyle());
////            }
////        }

////        public override void ApplyStyle()
////        {
////        }
////    }
////}
