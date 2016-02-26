using System;
using System.Collections.Generic;

namespace Shopdrawing.BridgeFS
{
    public interface IBridge
    {
        List<BridgeParts> BridgeParts { get; set; }
    }
}
