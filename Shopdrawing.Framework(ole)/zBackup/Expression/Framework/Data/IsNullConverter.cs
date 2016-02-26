using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public class IsNullConverter : IValueConverter
    {
        public IsNullConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}