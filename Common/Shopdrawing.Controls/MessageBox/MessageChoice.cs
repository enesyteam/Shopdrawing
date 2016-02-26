using System;

namespace Microsoft.Expression.Framework.Controls
{
	[Flags]
	public enum MessageChoice
	{
		None = 0,
		OK = 1,
		Cancel = 2,
		Yes = 4,
		No = 8,
		Help = 16
	}
}