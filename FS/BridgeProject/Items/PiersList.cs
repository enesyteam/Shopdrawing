
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace BridgeProject.Items
{
    public class PiersList : BridgeItemBase
    {
        public ObservableCollection<Pier> Children { get; set; }
        public PiersList()
        {
            Children = new ObservableCollection<Pier>();
        }
    }
}
