using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest
{
    public class ClassC : Item
    {
        public List<Item> Pickles { get; private set; }
        public List<Item> IceCream { get; private set; }

        public ClassC()
        {
            this.Pickles = new List<Item>();
            this.IceCream = new List<Item>();
        }
    }
}
