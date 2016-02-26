
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class Substructures : BridgeItemBase
    {
        public AbutmentsList Abutments { get; set; }
        public PiersList Piers { get; set; }

        public Substructures()
        {
            Abutments = new AbutmentsList() { Name = "Abutments"};
            Piers = new PiersList() { Name = "Piers" };
        }

        public override string ToString()
        {
            return "(" + Abutments.Children.Count + " Abutments; " + Piers.Children.Count + " Piers)";
        }

        string _name = "Substructures";
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                base.Name = value;
            }
        }
    }
}
