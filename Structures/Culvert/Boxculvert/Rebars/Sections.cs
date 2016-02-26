////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Windows;
////using DynamicGeometry;

////namespace Shopdrawing.Structures.Culvert
////{
////    public class BoxculvertRebarsSections : CompositeFigure
////    {
////        private double _length;
////        public double Length
////        {
////            get { return _length; }
////            set { _length = value; }
////        }
////        private BoxculvertCrossSectionBars _crossSection;
////        public BoxculvertCrossSectionBars CrossSection
////        {
////            get { return _crossSection; }
////            set { _crossSection = value; }
////        }

////        #region Constructors
////        public BoxculvertRebarsSections()
////        {
////        }
////        public BoxculvertRebarsSections(BoxculvertCrossSectionBars section, double length)
////            : base()
////        {
////            this.CrossSection = section;
////            this.Length = length;
////        }

////        #endregion

////        public override string ToString()
////        {
////            return "Rebars Sections";
////        }

////    }
////}
