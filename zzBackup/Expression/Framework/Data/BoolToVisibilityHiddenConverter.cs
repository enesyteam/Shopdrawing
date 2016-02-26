using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public class BoolToVisibilityHiddenConverter : IValueConverter
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

        public BoolToVisibilityHiddenConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)o;
            if (this.invertBoolean)
            {
                flag = !flag;
            }
            return (flag ? Visibility.Visible : Visibility.Hidden);
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (Visibility)o == Visibility.Visible;
            if (this.invertBoolean)
            {
                flag = !flag;
            }
            return flag;
        }
    }
}