
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class AbutmentsList : BridgeItemBase
    {
        public ObservableCollection<Abutment> Children { get; set; }
        public AbutmentsList()
        {
            Children = new ObservableCollection<Abutment>();
        }
    }
}
