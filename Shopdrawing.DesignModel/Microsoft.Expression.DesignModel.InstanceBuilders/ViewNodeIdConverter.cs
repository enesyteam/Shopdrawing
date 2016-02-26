using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class ViewNodeIdConverter : TypeConverter
	{
		private const char SeparatorToken = ',';

		public ViewNodeIdConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			int num;
			string str = value as string;
			if (str != null)
			{
				int num1 = str.IndexOf(',');
				if (num1 > 0)
				{
					Guid empty = Guid.Empty;
					try
					{
						empty = new Guid(str.Substring(0, num1));
					}
					catch (FormatException formatException)
					{
					}
					if (empty != Guid.Empty && int.TryParse(str.Substring(num1 + 1), out num))
					{
						return new ViewNodeId(num, empty);
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			ViewNodeId viewNodeId = value as ViewNodeId;
			if (viewNodeId == null)
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			string str = viewNodeId.ViewNodeIndex.ToString(CultureInfo.InvariantCulture);
			string str1 = viewNodeId.SerializationContextIndex.ToString();
			StringBuilder stringBuilder = new StringBuilder(str.Length + str1.Length + 1);
			stringBuilder.Append(str1);
			stringBuilder.Append(',');
			stringBuilder.Append(str);
			return stringBuilder.ToString();
		}
	}
}