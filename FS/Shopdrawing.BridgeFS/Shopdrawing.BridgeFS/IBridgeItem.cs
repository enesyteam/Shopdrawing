using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopdrawing.BridgeFS
{
    public interface IBridgeItem
    {
        int Order { get; set; }
        string ItemName { get; set; }
        Unit ItemUnit { get; set; }
        UnitType ItemUnitType { get; set; }
    }
}
