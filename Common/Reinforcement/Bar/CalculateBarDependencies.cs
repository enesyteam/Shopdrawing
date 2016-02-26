using System;
using DynamicGeometry;
using System.ComponentModel;
using System.Windows;
using System.Collections.Generic;
//using DynamicGeometry;
using Shopdrawing.Utilities;

namespace Shopdrawing.Reinforcement
{
    public partial class Bar : CompositeFigure
    {
        #region Calculate Bar dependencies base on Shape
        /// <summary>
        /// See BS 8666:2005
        /// </summary>
        /// <returns></returns>
        public List<IFigure> CalculateBarDependencies()
        {
            List<IFigure> result = new List<DynamicGeometry.IFigure>();
            Point basePoint = new System.Windows.Point(0, 0);
            result.Add(Factory.CreateFreePoint(this.Drawing, basePoint)); // Điểm chèn ban đầu

            switch (this.ShapeCode)
            {
                case BarShape2D.Shape00:
                    {
                        switch (ViewDirection)
                        {
                            case Reinforcement.ViewDirection.Top:
                                {
                                    switch (Transform)
                                    {
                                        case Reinforcement.Transform.Normal:
                                            result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(basePoint, this.Dimensions.A, 0)));
                                            return result;
                                        case Reinforcement.Transform.Rotate90CW:
                                            result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(basePoint, 0, -this.Dimensions.A)));
                                            return result;
                                        case Reinforcement.Transform.Rotate90CCW:
                                            result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(basePoint, 0, this.Dimensions.A)));
                                            return result;
                                        case Reinforcement.Transform.Rotate180:
                                            result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(basePoint, -this.Dimensions.A, 0)));
                                            return result;
                                        case Reinforcement.Transform.FlipHorizontal: // ??
                                            return null;
                                        case Reinforcement.Transform.FlipVertical: // ??
                                            return null;
                                        default: return null;
                                    }
                                }
                            case Reinforcement.ViewDirection.Cross:
                                {
                                    result.Add(Factory.CreateFreePoint(this.Drawing, PointsHelper.Offset(basePoint, this.Diameter, 0)));
                                    return result;
                                }
                            default: return null;
                        
                        }
                        
                       
                    }

                    // so on...
                default: return null;
                    
            }


        }
        #endregion
    }
}
