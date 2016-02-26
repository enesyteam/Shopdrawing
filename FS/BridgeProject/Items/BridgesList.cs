using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class BridgesList : BridgeItemBase
    {
        public ObservableCollection<Bridge> Children { get; set; }
        public BridgesList()
        {
            Children = new ObservableCollection<Bridge>();
        }
    }
}
