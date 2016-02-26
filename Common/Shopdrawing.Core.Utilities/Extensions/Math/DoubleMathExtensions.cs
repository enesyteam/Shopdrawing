using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Utility.Extensions.Math
{
	public static class DoubleMathExtensions
	{
		public static double Bound(this double value, double min, double max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			return value;
		}
	}
}