using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public InverseVisibilityConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool)o ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)o != Visibility.Visible;
        }
    }
}