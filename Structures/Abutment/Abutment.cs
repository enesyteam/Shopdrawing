using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DynamicGeometry;
using Shopdrawing.Reinforcement;
using Shopdrawing.Settings;
using Shopdrawing.Utilities;

namespace Shopdrawing.Structures
{
    public class Abutment : CompositeFigure
    {
        [Browsable(false)]
        public MainAbutmentSection MainSection { get; set; }
        public Abutment(Drawing drawing)
        {
            Drawing = drawing;
            MainSection = new MainAbutmentSection(Drawing) { SkewAngle = 90 };
            MainSection.CreateMainSection();
            this.Children.Add(MainSection);
            
        }
        public override string ToString()
        {
            return "Abutment";
        }

        #region override
        public override IFigure HitTest(Point point)
        {
            List<Point> abutBoundPoints = new List<Point>();
            foreach (IPoint p in this.MainSection.Dependencies)
            {
                abutBoundPoints.Add(p.Coordinates);
            }
            bool isInside = abutBoundPoints.IsPointInPolygon(point);
            return isInside ? this : null;
        }
        #endregion
    }
}
