using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Shopdrawing.Core.Utilities
{
    public static partial class PointsHelper
    {
        /// <summary>
        /// Kiểm tra nếu 1 điểm thuộc 1 đường thẳng
        /// </summary>
        /// <param name="endPoint1"></param>
        /// <param name="endPoint2"></param>
        /// <param name="checkPoint"></param>
        /// <returns></returns>
        public static bool IsOnLine(Point endPoint1, Point endPoint2, Point checkPoint)
        {
            return (((double)checkPoint.Y - endPoint1.Y)) / ((double)(checkPoint.X - endPoint1.X))
                == ((double)(endPoint2.Y - endPoint1.Y)) / ((double)(endPoint2.X - endPoint1.X));
        }

        public static Point Offset(Point p, double deltaX, double deltaY)
        {
            return new Point(p.X + deltaX, p.Y + deltaY);
        
        }
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        public static Point Mid(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }
    }
}
