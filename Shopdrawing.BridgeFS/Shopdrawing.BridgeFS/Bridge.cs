using System;
using System.Collections.Generic;


namespace Shopdrawing.BridgeFS
{
    public class Bridge : IBridge
    {
        public List<BridgeParts> BridgeParts { get; set; }
        public IList<BridgeItem> Items { get; set;  }
    }
}
