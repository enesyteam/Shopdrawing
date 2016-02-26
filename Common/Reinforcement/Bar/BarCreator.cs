using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Shopdrawing.Reinforcement;
using DynamicGeometry;

namespace Shopdrawing.Reinforcement
{
    public partial class BarCreator
    {
        public static Bar CreateBar(DynamicGeometry.Drawing drawing, IList<IFigure> dependencies)
        {
            return new Bar() { Drawing = drawing, Dependencies = dependencies };
        }

        ///// <summary>
        ///// Mặt cắt ngang thanh
        ///// </summary>
        ///// <param name="drawing"></param>
        ///// <param name="dependencies"></param>
        ///// <returns></returns>
        //public static BarCrossView CreateBarCross(DynamicGeometry.Drawing drawing, IList<IFigure> dependencies)
        //{
        //    return new BarCrossView() { Drawing = drawing, Dependencies = dependencies };
        //}

        ///// <summary>
        ///// Thanh trên mặt bằng
        ///// </summary>
        ///// <param name="drawing"></param>
        ///// <param name="dependencies"></param>
        ///// <returns></returns>
        //public static PlanView CreateBarPlanView(DynamicGeometry.Drawing drawing, IList<IFigure> dependencies)
        //{
        //    return new PlanView() { Drawing = drawing, Dependencies = dependencies };
        //}
    }
}
