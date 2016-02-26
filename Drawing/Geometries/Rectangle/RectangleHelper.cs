using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicGeometry.Geometries.Helper
{
    public static class RectangleHelper
    {
        public static List<IPoint> ReturnRectanglePoints(Drawing d, Rectangle r)
        {
            List<IPoint> resultPoints = new List<IPoint>();
            System.Windows.Point p1 = new System.Windows.Point(0, 0);
            System.Windows.Point p2 = new System.Windows.Point(r.Width, 0);
            System.Windows.Point p3 = new System.Windows.Point(r.Width, -r.Height);
            System.Windows.Point p4 = new System.Windows.Point(0, -r.Height);

            var p11 = Factory.CreateFreePoint(d, p1);
            var p12 = Factory.CreateFreePoint(d, p2);
            var p13 = Factory.CreateFreePoint(d, p3);
            var p14 = Factory.CreateFreePoint(d, p4);

            resultPoints.Add(p11);
            resultPoints.Add(p12);
            resultPoints.Add(p13);
            resultPoints.Add(p14);
            resultPoints.Add(p11);

            return resultPoints;
        }
    }
}
