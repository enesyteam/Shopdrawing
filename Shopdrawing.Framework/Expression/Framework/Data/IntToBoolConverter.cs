using Microsoft.Expression.Framework;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class IntToBoolConverter : IValueConverter
    {
        public IntToBoolConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o == MixedProperty.Mixed)
            {
                return true;
            }
            return ((int)o == 0 ? false : true);
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)o ? true : false);
        }
    }
}