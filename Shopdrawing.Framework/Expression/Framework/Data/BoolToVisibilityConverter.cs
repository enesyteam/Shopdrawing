using Microsoft.Expression.Framework;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o == MixedProperty.Mixed)
            {
                return Visibility.Visible;
            }
            return ((bool)o ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)o == Visibility.Visible ? true : false);
        }
    }
}