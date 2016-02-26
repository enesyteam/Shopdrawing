using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Project.UserInterface
{
	[ValueConversion(typeof(ICollection), typeof(Visibility))]
	public class MultipleItemsToVisibilityVisibleConverter : IValueConverter
	{
		public MultipleItemsToVisibilityVisibleConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ICollection collections = value as ICollection;
			if (collections != null && collections.Count >= 2)
			{
				return Visibility.Visible;
			}
			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}