using Microsoft.Expression.Framework;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class BoolToCheckStateConverter : IValueConverter
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

        public BoolToCheckStateConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o == MixedProperty.Mixed)
            {
                return null;
            }
            bool flag = (bool)o;
            if (this.negate)
            {
                flag = !flag;
            }
            return new bool?(flag);
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            bool value = false;
            if (o is bool?)
            {
                bool? nullable = (bool?)o;
                if (nullable.HasValue)
                {
                    value = nullable.Value;
                }
            }
            else if (o is bool)
            {
                value = (bool)o;
            }
            if (!this.negate)
            {
                return value;
            }
            return !value;
        }
    }
}