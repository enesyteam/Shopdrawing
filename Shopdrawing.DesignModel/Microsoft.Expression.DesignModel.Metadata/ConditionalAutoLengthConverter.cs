using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public class ConditionalAutoLengthConverter : LengthConverter
	{
		public ConditionalAutoLengthConverter()
		{
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (ConditionalAutoLengthConverter.IsSilverlightProject(context))
			{
				string str = value as string;
				if (str != null && ConditionalAutoLengthConverter.IsEquivalentToAuto(str))
				{
					throw new FormatException();
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			object obj = base.ConvertTo(context, culture, value, destinationType);
			if (ConditionalAutoLengthConverter.IsSilverlightProject(context))
			{
				string str = value as string;
				if (str != null && ConditionalAutoLengthConverter.IsEquivalentToAuto(str))
				{
					obj = "";
				}
			}
			return obj;
		}

		private static bool IsEquivalentToAuto(string str)
		{
			if (str.Equals("AUTO", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return str.Equals(CultureInfo.CurrentCulture.NumberFormat.NaNSymbol, StringComparison.CurrentCulture);
		}

		private static bool IsSilverlightProject(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				SilverlightConverterService service = (SilverlightConverterService)context.GetService(typeof(SilverlightConverterService));
				if (service != null)
				{
					return service.IsSilverlightProject;
				}
			}
			return false;
		}
	}
}