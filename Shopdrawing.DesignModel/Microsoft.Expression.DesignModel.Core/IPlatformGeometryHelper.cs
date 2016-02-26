using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface IPlatformGeometryHelper
	{
		object ConvertCanonicalTransform(ICanonicalTransform canonicalTransform);

		TransformGroup ConvertTransformToWpf(object value);

		bool NeedRoundupLayoutRect(IViewVisual visual);

		double Round(double value);

		Rect RoundupLayoutRect(Rect rect);
	}
}