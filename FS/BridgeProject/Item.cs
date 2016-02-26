using BridgeProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest
{
    public class Item :  ProjectItem, IItem
    {
        string _label;
        [Category("General")]
        [DefaultValue("UnTitled")]
        public virtual string Label 
        {
            get { return _label; }
            set { _label = value;
            NotifyChanged("Label");
            }
        }

        public string ToolTip { get; set; }
    }
}
