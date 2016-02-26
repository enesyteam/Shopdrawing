using System;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.Core
{
	public interface ICanonicalTransform
	{
		double CenterX
		{
			get;
			set;
		}

		double CenterY
		{
			get;
			set;
		}

		double RotationAngle
		{
			get;
			set;
		}

		double ScaleX
		{
			get;
			set;
		}

		double ScaleY
		{
			get;
			set;
		}

		double SkewX
		{
			get;
			set;
		}

		double SkewY
		{
			get;
			set;
		}

		System.Windows.Media.TransformGroup TransformGroup
		{
			get;
		}

		double TranslationX
		{
			get;
			set;
		}

		double TranslationY
		{
			get;
			set;
		}
	}
}