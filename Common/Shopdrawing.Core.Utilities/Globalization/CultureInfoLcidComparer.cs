using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Expression.Utility.Globalization
{
	public class CultureInfoLcidComparer : IEqualityComparer<CultureInfo>
	{
		public CultureInfoLcidComparer()
		{
		}

		public static bool AreEqual(CultureInfo x, CultureInfo y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
			{
				return false;
			}
			return x.LCID == y.LCID;
		}

		public bool Equals(CultureInfo x, CultureInfo y)
		{
			return CultureInfoLcidComparer.AreEqual(x, y);
		}

		public int GetHashCode(CultureInfo obj)
		{
			if (object.ReferenceEquals(obj, null))
			{
				return 0;
			}
			return obj.LCID;
		}
	}
}