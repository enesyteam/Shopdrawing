using System;

namespace Microsoft.MetadataReader
{
	[Flags]
	internal enum CorManifestResourceFlags
	{
		mrPublic = 1,
		mrPrivate = 2,
		mrVisibilityMask = 7
	}
}