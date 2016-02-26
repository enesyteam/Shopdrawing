using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DynamicGeometry.Geometries.Helper;

namespace DynamicGeometry.Geometries
{
    public class Rectangle : LineBase
    {
        private double _width;
        private double _height;

        [PropertyGridVisible]
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [PropertyGridVisible]
        [PropertyGridName("Dimensions")]
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public Rectangle()
        {
            this.Name = "Rectangle";
        }
        public void CreateRectangle(double width, double height)
        {
            Width = width;
            Height = height;

            List<IFigure> NewPolyLinePoints = new List<IFigure>();

            foreach (IPoint p in RectangleHelper.ReturnRectanglePoints(drawing, this))
            {
                NewPolyLinePoints.Add(p);
            }

            var newPolyLine = Factory.CreatePolyline(drawing, NewPolyLinePoints);
            drawing.Figures.Add(newPolyLine);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override string Name
        {
            get { return "Rectangle"; } //"Segment"
        }
    }
}
