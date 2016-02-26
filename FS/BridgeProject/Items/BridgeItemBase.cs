
using System;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class BridgeItemBase : INotifyPropertyChanged
    {
        string _name;

        public virtual string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
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

        public override string ToString()
        {
            return Name;
        }
    }
}
