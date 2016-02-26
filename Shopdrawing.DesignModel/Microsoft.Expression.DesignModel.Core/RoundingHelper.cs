using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class RoundingHelper
	{
		private const int decimalPlacesForLength = 3;

		private const int decimalPlacesForScale = 3;

		private const int decimalPlacesForAngle = 3;

		public const double LengthEpsilon = 0.001;

		public static double RoundAngle(double value)
		{
			return Math.Round(value, 3);
		}

		public static Vector3D RoundDirection(Vector3D value)
		{
			return new Vector3D(RoundingHelper.RoundLength(value.X), RoundingHelper.RoundLength(value.Y), RoundingHelper.RoundLength(value.Z));
		}

		public static double RoundLength(double value)
		{
			return Math.Round(value, 3);
		}

		public static Point RoundPosition(Point value)
		{
			return new Point(RoundingHelper.RoundLength(value.X), RoundingHelper.RoundLength(value.Y));
		}

		public static Point3D RoundPosition(Point3D value)
		{
			return new Point3D(RoundingHelper.RoundLength(value.X), RoundingHelper.RoundLength(value.Y), RoundingHelper.RoundLength(value.Z));
		}

		public static Rect RoundRect(Rect value)
		{
			if (value.IsEmpty)
			{
				return value;
			}
			return new Rect(RoundingHelper.RoundLength(value.X), RoundingHelper.RoundLength(value.Y), RoundingHelper.RoundLength(value.Width), RoundingHelper.RoundLength(value.Height));
		}

		public static double RoundScale(double value)
		{
			return Math.Round(value, 3);
		}

		public static Size RoundSize(Size value)
		{
			return new Size(RoundingHelper.RoundLength(value.Width), RoundingHelper.RoundLength(value.Height));
		}

		public static double RoundToDoublePrecision(double value, int digits)
		{
			double num = Math.Abs(value);
			if (num >= 1)
			{
				digits = digits - (int)Math.Ceiling(Math.Log10(num));
			}
			if (digits >= 0)
			{
				value = Math.Round(value, digits);
			}
			return value;
		}
	}
}