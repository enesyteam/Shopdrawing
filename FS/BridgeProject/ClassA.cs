using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DaisleyHarrison.WPF.ComplexDataTemplates.UnitTest
{
    public class ClassA : Item
    {
        public List<ClassB> ListOfClassB { get; private set; }
        public List<ClassC> ListOfClassC { get; private set; }

        public ClassA()
        {
            this.ListOfClassB = new List<ClassB>();
            this.ListOfClassC = new List<ClassC>();
        }

        string _label;
        [Category("General")]
        [DefaultValue("Abutment")]
        public override string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyChanged("Label");
            }
        }
    }
}
