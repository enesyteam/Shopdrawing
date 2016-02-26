using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class NotConverter : IValueConverter
    {
        public NotConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)o;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return !ValueConverterUtilities.AssureBool(o, false);
        }
    }
}