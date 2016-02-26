using System;
using System.ComponentModel;

namespace BridgeFS.Structures
{
    public class Abutment
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
