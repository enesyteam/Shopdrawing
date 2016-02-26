using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal static class PropertySortValue
	{
		public readonly static int NoValue;

		private readonly static int DesignTimeSeedSortValue;

		private static HashSet<int> sortValues;

		static PropertySortValue()
		{
			PropertySortValue.NoValue = -1;
			PropertySortValue.DesignTimeSeedSortValue = 1073741823;
			PropertySortValue.sortValues = new HashSet<int>();
			PropertySortValue.sortValues.Add(PropertySortValue.NoValue);
			PropertySortValue.sortValues.Add(0);
		}

		private static int GetUniqueSortValue(string name, int offset)
		{
			int num = offset + (name[0] << '\u0016' | Math.Abs(name.GetHashCode()) & 4194303);
			while (PropertySortValue.sortValues.Contains(num))
			{
				num++;
			}
			return num;
		}

		public static bool IsDesignTimeProperty(int sortValue)
		{
			if (sortValue < PropertySortValue.DesignTimeSeedSortValue)
			{
				return false;
			}
			return PropertySortValue.sortValues.Contains(sortValue);
		}

		public static int RegisterDesignTimeProperty(IPropertyId property)
		{
			int uniqueSortValue = PropertySortValue.GetUniqueSortValue(string.Concat("__BLEND", property.Name), PropertySortValue.DesignTimeSeedSortValue);
			PropertySortValue.sortValues.Add(uniqueSortValue);
			return uniqueSortValue;
		}

		public static int RegisterProperty(IProperty property)
		{
			int uniqueSortValue = PropertySortValue.GetUniqueSortValue(property.UniqueName, 1);
			PropertySortValue.sortValues.Add(uniqueSortValue);
			return uniqueSortValue;
		}
	}
}