using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ListBoxSelectionColorChange
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string imageFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/Images/";
           
            List<Customer> findResultsList = new List<Customer>()
            {
                new Customer()
                {
                     Title="Mr.",
                     Firstname="Pepito",
                     Lastname="Cracking",
                     DOB="12/10/2001",
                     Status = "Active",
                     FullAddress = new Address()
                     {
                          AddressLine1="10 Conolly St.",
                          Country ="Spain",
                          Postcode="E5 1PF"
                     },
                    CustomerId="1234dasdf",
                    Image=ImageHelper.BufferFromUri(string.Format("{0}/{1}",imageFolder,"f1.jpg"))
                },
                new Customer()
                {
                    Title="Mr.",
                    Firstname="John",
                    Lastname="Lock",
                    DOB="12/01/2000",
                    Status = "Active",
                    FullAddress = new Address()
                     {
                          AddressLine1="7 Blue Park",
                          Country ="EEUU",
                          Postcode="E4 1PF"
                     },
                    CustomerId="1234dasdf",
                    Image=ImageHelper.BufferFromUri(string.Format("{0}/{1}",imageFolder,"f2.jpg"))
                    
                },
                new Customer()
                {
                    Title="Mrs.",
                    Firstname="Walles",
                    Lastname="Carl",
                    DOB="12/04/2002",
                    Status = "Active",
                    FullAddress = new Address()
                     {
                          AddressLine1="7 Whitechapel",
                          Country ="UK",
                          Postcode="E1 1PF"
                     },
                    CustomerId="1234dLdG",
                    Image=ImageHelper.BufferFromUri(string.Format("{0}/{1}",imageFolder,"f3.jpg"))
                },
                new Customer()
                {
                    Title="Mrs.",
                    Firstname="Canny",
                    Lastname="Jo",
                    DOB="12/01/1999",
                    Status = "Active",
                    FullAddress = new Address()
                     {
                          AddressLine1="12 Sloane Street",
                          Country ="UK",
                          Postcode="E2 1PF"
                     },
                    CustomerId="1234dLPYR",
                    Image=ImageHelper.BufferFromUri(string.Format("{0}/{1}",imageFolder,"f4.jpg"))
                    
                },

                new Customer()
                {
                    Title="Mr.",
                    Firstname="Chuck",
                    Lastname="Norris",
                    DOB="12/02/1970",
                    Status = "Active",
                    FullAddress = new Address()
                     {
                          AddressLine1="129 Black Heaven",
                          Country ="EEUU",
                          Postcode="CW 1PF"
                     },
                    CustomerId="1234dLPYR",
                    Image=null
                    
                }
            };

            this.listBox_Results.DataContext = findResultsList;
            
        }

        
    }
}
