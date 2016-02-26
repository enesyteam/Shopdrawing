using System;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	[Flags]
	public enum ViewStoryboardApplyOptions
	{
		None = 0,
		RemoveMedia = 1,
		DisableTransforms = 2,
		DisableEffects = 4
	}
}