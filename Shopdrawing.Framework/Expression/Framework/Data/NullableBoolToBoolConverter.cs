using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class NullableBoolToBoolConverter : IValueConverter
    {
        private bool negate;

        public bool Negate
        {
            get
            {
                return this.negate;
            }
            set
            {
                this.negate = value;
            }
        }

        public NullableBoolToBoolConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (!this.Negate)
            {
                return ValueConverterUtilities.AssureBool(value, false);
            }
            return !ValueConverterUtilities.AssureBool(value, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (!this.Negate)
            {
                return flag;
            }
            return !flag;
        }
    }
}