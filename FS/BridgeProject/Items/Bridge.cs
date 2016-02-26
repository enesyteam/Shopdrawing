using System;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class Bridge : BridgeItemBase, IHaveCenterPoint
    {
        public Point3d Center { get; set; }
        [Category("General")]
        [DisplayName("Lý trình")]
        [Description("Lý trình cầu")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DefaultValue(false)]
        public MileStone MileStone { get; set; }

        public Bridge()
        {
            MileStone = new MileStone(0,0);
            Center = new Point3d(0,0,0);
            Substructures = new Substructures();
            Abutment a1 = new Abutment() { Name = "A1"};
            Abutment a2 = new Abutment() { Name = "A2" };
            Abutment a3 = new Abutment() { Name = "A3" };
            Substructures.Abutments.Children.Add(a1);
            Substructures.Abutments.Children.Add(a2);
            Substructures.Abutments.Children.Add(a3);

            for (int i = 1; i <= 13; i++)
            {
                Pier p = new Pier() { Name = "P" +i};
                Substructures.Piers.Children.Add(p);
            }
        }

        public Substructures Substructures { get; set; }
    }
}
