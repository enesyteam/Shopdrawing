using ICSharpCode.TreeView;
using System;
using System.ComponentModel;

namespace BridgeProject
{
    public class Bridge : SharpTreeNode, INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        string _bridgeName;
        [Category("General")]
        public virtual string BridgeName
        {
            get { return _bridgeName; }
            set { _bridgeName = value;
            NotifyChanged("BridgeName");
            }
        }
        string _mileStone;
        [Category("General")]
        public virtual string MileStone
        {
            get { return _mileStone; }
            set
            {
                _mileStone = value;
                NotifyChanged("MileStone");
            }
        }
        public Substructures Substructures { get; set; }

        #region Constructors
        public Bridge()
        {
            Substructures = new Substructures();
            Abutment a1 = new Abutment() { Name = "A1"};
            Abutment a2 = new Abutment() { Name = "A2"};
            Substructures.Abutments.Add(a1);
            Substructures.Abutments.Add(a2);

            Pier p1 = new Pier() { Name = "P1" };
            Pier p2 = new Pier() { Name = "P2" };
            Pier p3 = new Pier() { Name = "P3" };
            Substructures.Piers.Add(p1);
            Substructures.Piers.Add(p2);
            Substructures.Piers.Add(p3);
        }
        #endregion
    }
}
