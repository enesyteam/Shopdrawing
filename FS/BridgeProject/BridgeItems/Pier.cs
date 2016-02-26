using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeProject
{
    public class Pier : SubstructureItem
    {
        string _name = "Pier";
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
