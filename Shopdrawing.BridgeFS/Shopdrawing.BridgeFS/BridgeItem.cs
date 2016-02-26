using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shopdrawing.BridgeFS
{
    public class BridgeItem : IBridgeItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int Order { get; set; }
        public string ItemName { get; set; }
        public Unit ItemUnit { get; set; }
        public UnitType ItemUnitType { get; set; }
    }
}
