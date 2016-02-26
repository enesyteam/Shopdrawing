using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	[Flags]
	public enum LayoutOverrides
	{
		None = 0,
		HorizontalAlignment = 1,
		VerticalAlignment = 2,
		CenterHorizontalAlignment = 4,
		CenterVerticalAlignment = 8,
		Width = 16,
		Height = 32,
		GridBox = 128,
		HorizontalMargin = 256,
		VerticalMargin = 512,
		Margin = 768,
		CenterHorizontalMargin = 1024,
		CenterVerticalMargin = 2048,
		RecomputeDefault = 4096,
		All = 65535
	}
}