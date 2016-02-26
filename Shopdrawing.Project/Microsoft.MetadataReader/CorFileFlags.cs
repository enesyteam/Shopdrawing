using System;

namespace Microsoft.MetadataReader
{
	[Flags]
	internal enum CorFileFlags
	{
		ContainsMetaData,
		ContainsNoMetaData
	}
}