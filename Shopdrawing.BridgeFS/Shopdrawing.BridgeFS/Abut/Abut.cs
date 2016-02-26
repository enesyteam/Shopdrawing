using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopdrawing.BridgeFS.Abut
{
    public class Abut
    {
        [Category("General")]
        [DisplayName("Bề rộng bệ mố theo phương ngang cầu")]
        public double PileCapW { get; set; }
        [Category("General")]
        [DisplayName("Chiều dày")]
        public double PileCapH { get; set; }
        [Category("General")]
        [DisplayName("Bề rộng bệ mố theo phương dọc cầu")]
        public double PileCapL { get; set; }
    }
}
