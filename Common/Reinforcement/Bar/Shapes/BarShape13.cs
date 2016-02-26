using System;
using System.Collections.Generic;
using DynamicGeometry;
using Shopdrawing.Utilities;
using System.Windows;
using System.ComponentModel;
using M = System.Math;

namespace Shopdrawing.Reinforcement
{
    /// <summary>
    /// BarShape13
    /// See BS 8666:2005 for more detail
    /// </summary>
    public class BarShape13 : BarShapeBase
    {
        public BarShape13(Drawing drawing, Dimensions dimensions, ViewDirection viewDirection, Point _insertPoint)
        {
            Drawing = drawing;
            Dimensions = dimensions;
            ViewDirection = viewDirection;

            Parts = new List<IFigure>();
            DependencyPoints = new List<Point>();
        }

        #region overrides
        public override void Create()
        {
            Parts.Clear();
            Dependencies.Clear();
            Children.Clear();
            switch (BarStyle)
            {
                case Reinforcement.BarStyle.SingleLine:
                    // Part1
                    // Chú ý khi gọi phương thức GetPart1Dependencies()
                    // ứng với trường hợp Transform = Rotate180, Rotate90CCW, Rotate90CW
                    // Thì góc Rotate đã bị thay đổi 1 lần
                    // Do đó trong phương thức GetPart1Dependencies()
                    // khi thay đổi góc Rotate chỉ thay đổi 1/2 là đủ
                    if (GetPart1Dependencies() != null)
                    {
                        var part1 = Factory.CreateSegment(Drawing, GetPart1Dependencies());
                        part1.Style = base.Style;
                        Parts.Add(part1);
                    }
                    // Part2
                    if (GetPart2Dependencies() != null)
                    {
                        if (ViewDirection != Reinforcement.ViewDirection.Left)
                        {
                            var part2 = Factory.CreateArc(Drawing, GetPart2Dependencies());
                            part2.Clockwise = ArcPartClockWise;
                            part2.Style = base.Style;
                            Parts.Add(part2);
                        }
                        else
                        {
                            var part2 = Factory.CreateSegment(Drawing, GetPart2Dependencies());
                            part2.Style = base.Style;
                            Parts.Add(part2);
                        }
                    }
                    // Part3
                    if (GetPart3Dependencies() != null)
                    {
                        var part3 = Factory.CreateSegment(Drawing, GetPart3Dependencies());
                        part3.Style = base.Style;
                        Parts.Add(part3);
                    }

                    // Cross Part
                    if (GetCrossPartDependencies() != null)
                    {
                        var crossPart = Factory.CreateCircle(Drawing, GetCrossPartDependencies());
                        crossPart.Style = base.Style;
                        Parts.Add(crossPart);
                    }

                    if (Parts != null)
                    {
                        foreach (IFigure f in Parts)
                        {
                            Children.Add(f);
                            foreach (IPoint p in f.Dependencies)
                            {
                                Dependencies.Add(p);
                                DependencyPoints.Add(p.Coordinates);
                            }
                        }
                    }
                    break;
                case Reinforcement.BarStyle.BoundLine:
                    break;
                case Reinforcement.BarStyle.FilledLine:
                    break;
            
            }
            
        }

        /// <summary>
        /// Thay đổi dependencies
        /// </summary>
        public override void ReDraw()
        {
        }
        /// <summary>
        /// Get dependencies for part1
        /// </summary>
        /// <returns></returns>
        protected IList<IFigure> GetPart1Dependencies()
        {
            switch (ViewDirection)
            {
                case Reinforcement.ViewDirection.Front:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, this.InsertPoint)); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * this.Dimensions.B) * M.Cos(RotateRAD), 
                                        -(this.Dimensions.C - 0.5 * this.Dimensions.B) * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipHorizontal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, this.InsertPoint)); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        (this.Dimensions.C - 0.5 * this.Dimensions.B) * M.Cos(RotateRAD),
                                        (this.Dimensions.C - 0.5 * this.Dimensions.B) * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, this.InsertPoint)); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 
                                        -(this.Dimensions.C -0.5*Dimensions.B)* M.Cos(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    Rotate += 90; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    Rotate += 45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    Rotate += -45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            default: return null;
                        }
                    }
                case Reinforcement.ViewDirection.Left:
                case Reinforcement.ViewDirection.Top:
                    return null;
                default: return null;
            }
        }
        /// <summary>
        /// Get dependencies for part2
        /// </summary>
        /// <returns></returns>
        protected IList<IFigure> GetPart2Dependencies()
        {
            switch (ViewDirection)
            {
                case Reinforcement.ViewDirection.Front:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 
                                        -(Dimensions.C - 0.5*Dimensions.B)*M.Cos(RotateRAD) + 0.5*Dimensions.B*M.Sin(RotateRAD),
                                        -(Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - 0.5 * Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD),
                                        -(Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) + Dimensions.B * M.Sin(RotateRAD),
                                        -(Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - Dimensions.B * M.Cos(RotateRAD)))); 
                                    return result;
                                }
                            case Reinforcement.Transform.FlipHorizontal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        (this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) + 0.5*this.Dimensions.B * M.Sin(RotateRAD),
                                        (this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - 0.5*this.Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint,
                                        (Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD),
                                        (Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 
                                        (this.Dimensions.C -0.5*Dimensions.B)* M.Cos(RotateRAD) + this.Dimensions.B * M.Sin(RotateRAD),
                                        (this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - this.Dimensions.B * M.Cos(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) - 0.5*Dimensions.B*M.Sin(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) + 0.5 * Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) - Dimensions.B * M.Sin(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) + Dimensions.B * M.Cos(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            default: return null;
                        }
                    }
                case Reinforcement.ViewDirection.Left:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                            case Reinforcement.Transform.FlipHorizontal:
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint,
                                       -0.5 * Dimensions.B * M.Sin(RotateRAD), -0.5 * Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint,
                                       0.5 * Dimensions.B * M.Sin(RotateRAD), 0.5 * Dimensions.B * M.Cos(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    Rotate += 90; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    Rotate += 45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    Rotate += -45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            default: return null;
                        }
                    }
                case Reinforcement.ViewDirection.Cross:
                case Reinforcement.ViewDirection.Top:
                    return null;
                default: return null;
            }
            
        }
        /// <summary>
        /// Trả về chiều vẽ cho các đối tượng Arc thuộc hình dạng này
        /// </summary>
        protected bool ArcPartClockWise
        {
            get 
            {
                switch (Transform)
                { 
                    case Reinforcement.Transform.FlipVertical:
                    case Reinforcement.Transform.FlipHorizontal:
                        return true;
                    case Reinforcement.Transform.Normal:
                    case Reinforcement.Transform.Rotate180:
                    case Reinforcement.Transform.Rotate90CCW:
                    case Reinforcement.Transform.Rotate90CW:
                        return false;

                    default: return false;
                }
            }
        
        }
        /// <summary>
        /// Get dependencies for part3
        /// </summary>
        /// <returns></returns>
        protected IList<IFigure> GetPart3Dependencies()
        {
            switch (ViewDirection)
            {
                case Reinforcement.ViewDirection.Front:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                            -(Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) + Dimensions.B * M.Sin(RotateRAD),
                                            -(Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - Dimensions.B * M.Cos(RotateRAD))));

                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                            -(Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) + Dimensions.B * M.Sin(RotateRAD) + (Dimensions.A - 0.5*Dimensions.B)*M.Cos(RotateRAD),
                                            -(Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - Dimensions.B * M.Cos(RotateRAD) + (Dimensions.A - 0.5 * Dimensions.B) * M.Sin(RotateRAD))));

                                    return result;
                                }
                            case Reinforcement.Transform.FlipHorizontal:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        (this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) + this.Dimensions.B * M.Sin(RotateRAD),
                                        (this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) - this.Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, this.Dimensions.C * M.Cos(RotateRAD) + this.Dimensions.B * M.Sin(RotateRAD) - this.Dimensions.A * M.Cos(RotateRAD),
                                        this.Dimensions.C * M.Sin(RotateRAD) - this.Dimensions.B * M.Cos(RotateRAD) - this.Dimensions.A * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) - Dimensions.B * M.Sin(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) + Dimensions.B * M.Cos(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint,
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Cos(RotateRAD) - Dimensions.B * M.Sin(RotateRAD)
                                        + (Dimensions.A + 0.5*Dimensions.B)*M.Cos(RotateRAD),
                                        -(this.Dimensions.C - 0.5 * Dimensions.B) * M.Sin(RotateRAD) + Dimensions.B * M.Cos(RotateRAD)
                                        + (Dimensions.A + 0.5 * Dimensions.B) * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    goto case Reinforcement.Transform.Normal;
                                }
                            default: return null;
                        }
                    }
                case Reinforcement.ViewDirection.Top:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                            case Reinforcement.Transform.FlipHorizontal:
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    List<IFigure> result = new List<IFigure>();
                                    result.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint, 
                                        -0.5 * Dimensions.A* M.Cos(RotateRAD), -0.5 * Dimensions.A* M.Sin(RotateRAD))));
                                    result.Add(Factory.CreateFreePoint(Drawing, PointsHelper.Offset(this.InsertPoint,
                                        0.5 * Dimensions.A * M.Cos(RotateRAD), 0.5 * Dimensions.A * M.Sin(RotateRAD))));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    Rotate += 90; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    Rotate += 45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    Rotate += -45; // Gọi phương thức này 2 lần
                                    goto case Reinforcement.Transform.Normal;
                                }
                            default: return null;
                        }
                    }
                case Reinforcement.ViewDirection.Left:
                    return null;
                default: return null;
            }
        }

        public override void ReCreate()
        {
            if (Parts != null)
            {
                foreach (IFigure part in Parts)
                {
                    part.OnRemovingFromCanvas(Drawing.Canvas);
                }
                Parts.Clear();
            }
            this.Dependencies.Clear();
            this.Children.Clear();
            Create();
            foreach (IFigure part in Parts)
            {
                part.OnAddingToCanvas(Drawing.Canvas);

                foreach (IPoint p in part.Dependencies)
                    Dependencies.Add(p);
            }
            UpdateVisual();
        }

        public override string ToString()
        {
            return "BarShape13";
        }
        public override IFigure HitTest(Point point)
        {
            bool isInside = this.DependencyPoints.IsPointInPolygon(point);
            return isInside ? this : null;
        }
        private List<Point> DependencyPoints { get; set; }
    }
        #endregion

}
