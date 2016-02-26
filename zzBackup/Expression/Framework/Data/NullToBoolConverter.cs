using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public class NullToBoolConverter : IValueConverter
    {
        public NullToBoolConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return o != null;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}