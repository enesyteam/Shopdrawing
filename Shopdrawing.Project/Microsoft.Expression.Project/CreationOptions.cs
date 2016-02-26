using System;

namespace Microsoft.Expression.Project
{
	[Flags]
	public enum CreationOptions
	{
		None = 0,
		SilentlyOverwrite = 1,
		SilentlyOverwriteReadOnly = 2,
		SilentlyHandleIOExceptions = 4,
		DoNotAllowOverwrites = 8,
		LinkSourceFile = 16,
		DoNotSelectCreatedItems = 256,
		DoNotSetDefaultImportPath = 4096,
		AlwaysUseDefaultBuildTask = 65536,
		DesignTimeResource = 1048576
	}
}