using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicGeometry;

namespace Shopdrawing.Structures.Culvert
{
    public class Boxculvert : CompositeFigure
    {
        private double _length;
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private BoxCulvertSection _crossSection;
        public BoxCulvertSection CrossSection
        {
            get { return _crossSection; }
            set { _crossSection = value; }
        }

        #region Constructors
        public Boxculvert()
        {      
        }
        public Boxculvert(BoxCulvertSection section, double length)
            : base()
        {
            this.CrossSection = section;
            this.Length = length;
        }

        #endregion

        public override string ToString()
        {
            return "Boxculvert";
        }
    }
}
