using System.Windows;
using System.Xml.Linq;

namespace DynamicGeometry
{
    public partial class RemotePoint : FreePoint
    {
        public override string Name
        {
            get
            {
                return base.Name + " " + base.HintText;
            }
            set
            {
                base.Name = value;
            }
        }
    }
}