using System;
using System.Collections.Generic;
using DynamicGeometry;
using Shopdrawing.Utilities;
using System.Windows;
using System.ComponentModel;

namespace Shopdrawing.Reinforcement
{
    public class BarShape00 : BarShapeBase
    {
        public BarShape00(Drawing drawing, Dimensions dimensions, ViewDirection viewDirection)
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
            result.Add(Factory.CreateFreePoint(this.Drawing, this.InsertPoint)); // Điểm chèn ban đầu
            switch (this.ViewDirection)
            {
                case Reinforcement.ViewDirection.Top:
                case Reinforcement.ViewDirection.Front:
                    {
                        switch (Transform)
                        {
                            case Reinforcement.Transform.Normal:
                            case Reinforcement.Transform.FlipVertical:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, this.Dimensions.A, 0)));
                                    return result;
                                }
                            case Reinforcement.Transform.FlipHorizontal:
                            case Reinforcement.Transform.Rotate180:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, -this.Dimensions.A, 0)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CCW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 0, this.Dimensions.A)));
                                    return result;
                                }
                            case Reinforcement.Transform.Rotate90CW:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(this.InsertPoint, 0, -this.Dimensions.A)));
                                    return result;
                                }

                            default: return null;
                        }
                    
                    }
                case Reinforcement.ViewDirection.Cross:
                case Reinforcement.ViewDirection.Left:
                    {
                        return null;
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
            return "BarShape00";
        }
    }
        #endregion

}
