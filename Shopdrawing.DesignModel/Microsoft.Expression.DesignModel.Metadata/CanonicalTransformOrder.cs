using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class CanonicalTransformOrder
	{
		public readonly static int ScaleIndex;

		public readonly static int SkewIndex;

		public readonly static int RotateIndex;

		public readonly static int TranslateIndex;

		public readonly static int TransformCount;

		static CanonicalTransformOrder()
		{
			CanonicalTransformOrder.ScaleIndex = 0;
			CanonicalTransformOrder.SkewIndex = 1;
			CanonicalTransformOrder.RotateIndex = 2;
			CanonicalTransformOrder.TranslateIndex = 3;
			CanonicalTransformOrder.TransformCount = 4;
		}
	}
}