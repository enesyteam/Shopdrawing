using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DaisleyHarrison.WPF.ComplexDataTemplates
{
    public interface IBindingGroup
    {
        Type ElementType { get; }
        IEnumerable Items { get; }
        string Parameter { get; }
    }
}
