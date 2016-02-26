using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeProject.Items
{
    public class MileStone : INotifyPropertyChanged
    {
        double _km;
        public double Km
        {
            get { return _km; }
            set
            {
                _km = value;
                RaisePropertyChanged("Km"); }
        }
        public double M { get; set; }
        public override string ToString()
        {
            return "Km" + Km + "+" +  M.ToString();
        }
        public MileStone(double km, double m)
        {
            Km = km;
            M=m;
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
