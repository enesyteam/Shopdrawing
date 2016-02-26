using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeProject
{
    public class Abutment : SubstructureItem
    {
        string _name = "Abutment";
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
        [Category("Dimensions")]
        [DisplayName("Bề rộng bệ")]
        public double PileCapWidth { get; set; }
        [Category("Dimensions")]
        [DisplayName("Chiều dài bệ")]
        public double PileCapLength { get; set; }
        [Category("Dimensions")]
        [DisplayName("Chiều dày bệ")]
        [DefaultValue(2000)]
        public double PileCapHeight { get; set; }

        public Abutment()
        {
            PileCapWidth = 10500;
            PileCapLength = 6000;
            PileCapHeight = 2000;
        }
    }
}
