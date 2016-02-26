using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace ListBoxSelectionColorChange
{
    [ValueConversion(typeof(Address), typeof(string))]
    public class AddressToFullAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {

            if (value == null)
              return null;

            Address address=value as Address;

            if (address != null)
            {
               return string.Format("{0}, {1}, {2}",address.AddressLine1,address.Postcode,address.Country);
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }



    }
}
