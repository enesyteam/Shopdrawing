using System;
using System.Collections.Generic;
using System.Windows;

namespace Shopdrawing.Reinforcement
{
    public class RectangleBarGrid : BarGridBase
    {
        public Point CenterPoint { get; set; }
        public Bar HorizontalBar { get; set; }
        public Bar VerticalBar { get; set; }

        public double HorizontalBarSpacing { get; set; }
        public double VerticalBarSpacing { get; set; }

        public RectangleBarGrid()
        {
        }
    }
}
