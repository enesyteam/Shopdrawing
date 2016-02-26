using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeProject.Items
{
    public class Point3d
    {
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Point3d(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }
        public override string ToString()
        {
            return "{X=" + X + "," + "Y=" + Y + "," + "Z=" + Z+"}";
        }
    }
}
