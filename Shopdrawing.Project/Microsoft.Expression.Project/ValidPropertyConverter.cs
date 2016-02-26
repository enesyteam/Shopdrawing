using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Project
{
	[ValueConversion(typeof(IProjectTemplate), typeof(List<ProjectPropertyValue>))]
	public class ValidPropertyConverter : IValueConverter
	{
		public ValidPropertyConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}
			if (value == null)
			{
				return null;
			}
			return ((IProjectTemplate)value).ValidPropertyValues(parameter.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}