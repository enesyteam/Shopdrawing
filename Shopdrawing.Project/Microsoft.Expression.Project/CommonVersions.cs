using System;

namespace Microsoft.Expression.Project
{
	public static class CommonVersions
	{
		public readonly static Version Version2_0;

		public readonly static Version Version3_0;

		public readonly static Version Version3_5;

		public readonly static Version Version4_0;

		static CommonVersions()
		{
			CommonVersions.Version2_0 = new Version(2, 0);
			CommonVersions.Version3_0 = new Version(3, 0);
			CommonVersions.Version3_5 = new Version(3, 5);
			CommonVersions.Version4_0 = new Version(4, 0);
		}
	}
}