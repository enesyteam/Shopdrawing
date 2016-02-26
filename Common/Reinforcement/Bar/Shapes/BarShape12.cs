using System;
using System.Collections.Generic;
using DynamicGeometry;
using Shopdrawing.Utilities;
using System.Windows;
using System.ComponentModel;

namespace Shopdrawing.Reinforcement
{
    /// <summary>
    /// Same to BarShape11
    /// </summary>
    public class BarShape12 : BarShapeBase
    {
        public BarShape12(Drawing drawing, Dimensions dimensions, ViewDirection viewDirection)
        {
            Drawing = drawing;
            Dimensions = dimensions;
            ViewDirection = viewDirection;

            Parts = new List<IFigure>();
        }

        #region overrides
        public override void Create()
        {
            Parts.Clear();
            // Part1
            if (GetPart1Dependencies() != null)
            {
                var part1 = Factory.CreateSegment(Drawing, GetPart1Dependencies());
                if (part1 != null) Parts.Add(part1);
            }
            // Part2
            if (GetPart2Dependencies() != null)
            {
                var part2 = Factory.CreateSegment(Drawing, GetPart2Dependencies());
                if (part2 != null) Parts.Add(part2);
            }

            // Cross Part
            if (GetCrossPartDependencies() != null)
            {
                var crossPart = Factory.CreateCircle(Drawing, GetCrossPartDependencies());
                Parts.Add(crossPart);
            }

            Dependencies.Clear();
            Children.Clear();
            if (Parts != null)
            {
                foreach (IFigure f in Parts)
                {
                    Children.Add(f);
                    foreach (IPoint p in f.Dependencies)
                        Dependencies.Add(p);
                }
            }

        }

        /// <summary>
        /// Thay đổi dependencies
        /// </summary>
        public override void ReDraw()
        {
            //if (Parts != null)
            //{
            //    switch (Parts.Count)
            //    {
            //        case 1: // crossview
            //            {
            //                break;
            //            }
            //        case 2: //
            //    }
            //}
        }
        /// <summary>
        /// Get dependencies for part1
        /// </summary>
        /// <returns></returns>
        public IList<IFigure> GetPart1Dependencies()
        {
            List<IFigure> result = new List<IFigure>();
            result.Add(Factory.CreateFreePoint(this.Drawing, InsertPoint)); // Điểm chèn ban đầu
            switch (this.ViewDirection)
            {
                case Reinforcement.ViewDirection.Top:
                case Reinforcement.ViewDirection.Cross:
                    {
                        return null;
                    }
                case Reinforcement.ViewDirection.Front:
                case Reinforcement.ViewDirection.Left:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                            case Reinforcement.Transform.FlipHorizontal:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, -this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipVertical:
                            case Reinforcement.Transform.Rotate180:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, this.Dimensions.A, 0)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, -this.Dimensions.A, 0)));
                                    return result;
                                }

                            default: return null;
                        }
                    }

                default: return null;
            }
        }
        /// <summary>
        /// Get dependencies for part1
        /// </summary>
        /// <returns></returns>
        public IList<IFigure> GetPart2Dependencies()
        {
            List<IFigure> result = new List<IFigure>();
            switch (this.ViewDirection)
            {
                case Reinforcement.ViewDirection.Top:
                case Reinforcement.ViewDirection.Front:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, -this.Dimensions.A))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, this.Dimensions.B, -this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipHorizontal:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, -this.Dimensions.A))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, -this.Dimensions.B, -this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, this.Dimensions.A))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, this.Dimensions.B, this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, this.Dimensions.A, 0))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, this.Dimensions.A, this.Dimensions.B)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, -this.Dimensions.A, 0))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, -this.Dimensions.A, -this.Dimensions.B)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate180:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, 0, this.Dimensions.A))); // Điểm chèn ban đầu
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(InsertPoint, -this.Dimensions.B, this.Dimensions.A)));
                                    return result;
                                }
                            default: return null;
                        }

                    }

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
            return "BarShape11";
        }
    }
        #endregion

}
