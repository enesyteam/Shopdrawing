using System;

namespace Microsoft.Expression.Project
{
	[Flags]
	public enum ProjectItemEventOptions
	{
		None,
		SilentIfDirty,
		SilentIfOpen,
		Silent
	}
}