using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shopdrawing.BridgeFS.Abut
{
    /// <summary>
    /// Interaction logic for AbutDimensionWindow.xaml
    /// </summary>
    public partial class AbutDimensionWindow : Window
    {
        public static readonly DependencyProperty AbutProperty = DependencyProperty.Register("Abut", typeof(Abut), typeof(AbutDimensionWindow));
        public Abut Abut
        {
            get { return (Abut)this.GetValue(AbutProperty); }
            set { this.SetValue(AbutProperty, value); }
        }
        public AbutDimensionWindow()
        {
            DataContext = this;
            InitializeComponent();
            Abut = new Abut() { PileCapH = 2, PileCapL = 5, PileCapW = 10.5};
        }


    }
}
