using System;
using System.Windows;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class PointVectorConverter
	{
		public static Vector FromPoint(object obj)
		{
			return PointVectorConverter.FromPoint((Point)obj);
		}

		public static Vector FromPoint(Point point)
		{
			return new Vector(point.X, point.Y);
		}

		public static Point ToPoint(Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}
	}
}