using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Project.Conversion
{
	[ValueConversion(typeof(List<ConversionType>), typeof(List<string>))]
	public class ConversionTypeConverter : IValueConverter
	{
		public ConversionTypeConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			List<string> strs = new List<string>();
			foreach (ConversionType conversionType in (List<ConversionType>)value)
			{
				switch (conversionType)
				{
					case ConversionType.ProjectSilverlight3:
					{
						strs.Add("3.0");
						continue;
					}
					case ConversionType.ProjectSilverlight4:
					{
						strs.Add("4.0");
						continue;
					}
					case ConversionType.ProjectWpf30:
					{
						strs.Add("0.0");
						continue;
					}
					case ConversionType.ProjectWpf35:
					{
						strs.Add("3.5");
						continue;
					}
					case ConversionType.ProjectWpf40:
					{
						strs.Add("4.0");
						continue;
					}
					default:
					{
						goto case ConversionType.ProjectWpf30;
					}
				}
			}
			return strs;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}