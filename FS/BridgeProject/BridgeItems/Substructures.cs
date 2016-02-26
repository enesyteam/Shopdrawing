using System;
using System.Collections.Generic;

namespace BridgeProject
{
    public class Substructures : BridgeItem
    {
        public List<Abutment> Abutments { get; set; }
        public List<Pier> Piers { get; set; }

        public Substructures()
        {
            Abutments = new List<Abutment>();
            Piers = new List<Pier>();
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
                NotifyChanged("Name");
            }
        }
    }
}
