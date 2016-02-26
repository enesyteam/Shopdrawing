using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public class BoolToVisibilityCollapsedConverter : IValueConverter
    {
        private bool invertBoolean;

        public bool InvertBoolean
        {
            get
            {
                return this.invertBoolean;
            }
            set
            {
                this.invertBoolean = value;
            }
        }

        public BoolToVisibilityCollapsedConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if (o is bool?)
            {
                if (((bool?)o).Value ^ this.invertBoolean)
                {
                    visibility = Visibility.Visible;
                }
            }
            else if (o is bool && (bool)o ^ this.invertBoolean)
            {
                visibility = Visibility.Visible;
            }
            return visibility;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)o == Visibility.Visible ^ this.invertBoolean;
        }
    }
}