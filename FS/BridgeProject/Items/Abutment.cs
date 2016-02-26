
using System;
using System.ComponentModel;

namespace BridgeProject.Items
{
    public class Abutment : BridgeItemBase, ICanGetQuantities
    {
        public Point3d Center { get; set; }

        #region Caption
        [Category("Pile's Cap")]
        [DisplayName("Bề rộng bệ mố")]
        [Description("Kích thước bệ mố theo phương ngang cầu")]
        public double PileCapW { get; set; }

        [Category("Pile's Cap")]
        [DisplayName("Chiều dài bệ mố")]
        [Description("Kích thước bệ mố theo phương dọc cầu")]
        public double PileCapL { get; set; }

        [Category("Pile's Cap")]
        [DisplayName("Chiều cao bệ mố")]
        [Description("Kích thước bệ mố theo phương đứng cầu")]
        public double PileCapH { get; set; }
        #endregion

        #region Body
        [Category("Body Wall")]
        [DisplayName("Bề rộng thân mố")]
        [Description("Kích thước thân mố theo phương ngang cầu")]
        public double BodyW { get; set; }

        [Category("Body Wall")]
        [DisplayName("Chiều dài thân mố")]
        [Description("Kích thước thân mố theo phương dọc cầu")]
        public double BodyL { get; set; }

        [Category("Body Wall")]
        [DisplayName("Chiều cao thân mố")]
        [Description("Kích thước thân mố theo phương đứng cầu")]
        public double BodyH { get; set; }
        #endregion

        #region Head
        [Category("Head Wall")]
        [DisplayName("Bề rộng tường đầu")]
        [Description("Kích thước tường đầu mố theo phương ngang cầu")]
        public double HeadW { get; set; }

        [Category("Head Wall")]
        [DisplayName("Chiều dài tường đầu")]
        [Description("Kích thước tường đầu mố theo phương dọc cầu")]
        public double HeadL { get; set; }

        [Category("Head Wall")]
        [DisplayName("Chiều cao tường đầu")]
        [Description("Kích thước tường đầu mố theo phương đứng cầu")]
        public double HeadH { get; set; }
        #endregion

        #region Conts
        public Abutment()
        {
            Center = new Point3d(0, 0, 0);
        }
        #endregion

        public void GetQuantities()
        { 
        
        }
    }
}
