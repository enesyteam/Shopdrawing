
using System;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class Pier : BridgeItemBase, IHaveCenterPoint
    {
        public Point3d Center { get; set; }
        public Pier()
        {
            Center = new Point3d(0,0,0);
        }
    }
}
