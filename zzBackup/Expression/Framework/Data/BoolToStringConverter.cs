using Microsoft.Expression.Framework;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
    public sealed class BoolToStringConverter : IValueConverter
    {
        private string trueString = "True";

        private string falseString = "False";

        private string indeterminateString = string.Empty;

        public string FalseString
        {
            get
            {
                return this.falseString;
            }
            set
            {
                this.falseString = value;
            }
        }

        public string IndeterminateString
        {
            get
            {
                return this.indeterminateString;
            }
            set
            {
                this.indeterminateString = value;
            }
        }

        public string TrueString
        {
            get
            {
                return this.trueString;
            }
            set
            {
                this.trueString = value;
            }
        }

        public BoolToStringConverter()
        {
        }

        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            if (o == MixedProperty.Mixed)
            {
                return this.indeterminateString;
            }
            if (!(bool)o)
            {
                return this.falseString;
            }
            return this.trueString;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)o == this.trueString ? true : false);
        }
    }
}