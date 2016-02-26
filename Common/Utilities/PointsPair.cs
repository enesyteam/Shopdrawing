using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Shopdrawing.Utilities
{
    public class PointsPair
    {
        public PointsPair(Point lowerLeft, Point upperRight)
        { 
        
        }
        public Point LowerLeftPoint { get; set; }
        public Point UpperRightPoint { get; set; }

        public bool Contains(Point p)
        {
            return p.X <= UpperRightPoint.X && p.X >= LowerLeftPoint.X
                && p.Y <= UpperRightPoint.Y && p.Y >= LowerLeftPoint.Y;

        }
    }
}
