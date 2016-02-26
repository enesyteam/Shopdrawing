using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Microsoft.Expression.Utility.Extensions.Math
{
	public static class RectExtensions
	{
		public static Point GetCenter(this Rect rect)
		{
			return rect.Location + ((Vector)rect.Size / 2);
		}

		public static double GetCenterX(this Rect rect)
		{
			return rect.Left + rect.Width / 2;
		}

		public static double GetCenterY(this Rect rect)
		{
			return rect.Top + rect.Height / 2;
		}

		public static double GetProportionalScale(this Rect rect, Size maxSize)
		{
			double[] width = new double[] { 1, maxSize.Width / rect.Width, maxSize.Height / rect.Height };
			return width.Min();
		}

		public static Rect Union(this IEnumerable<Rect> rects)
		{
			return rects.Aggregate<Rect>(new Func<Rect, Rect, Rect>(Rect.Union));
		}
	}
}