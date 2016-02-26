using System;
using System.ComponentModel;


namespace BridgeProject
{
    public interface IBridgeItem
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
