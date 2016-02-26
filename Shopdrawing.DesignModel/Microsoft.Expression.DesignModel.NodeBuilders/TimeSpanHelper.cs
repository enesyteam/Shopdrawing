using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.NodeBuilders
{
	public static class TimeSpanHelper
	{
		public static string FormatTimeSpanInvariant(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			string str;
			if (days == 0 && hours == 0 && minutes == 0 && seconds == 0 && milliseconds == 0)
			{
				return "0";
			}
			if (milliseconds == 0)
			{
				str = seconds.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				double num = (double)((double)seconds + (double)milliseconds / 1000);
				str = num.ToString(CultureInfo.InvariantCulture);
			}
			string str1 = str;
			string str2 = (days == 0 ? hours.ToString(CultureInfo.InvariantCulture) : string.Concat(days.ToString(CultureInfo.InvariantCulture), ".", hours.ToString(CultureInfo.InvariantCulture)));
			string[] strArrays = new string[] { str2, ":", minutes.ToString(CultureInfo.InvariantCulture), ":", str1 };
			return string.Concat(strArrays);
		}
	}
}