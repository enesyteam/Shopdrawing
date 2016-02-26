using System;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class AssemblyCopiedEventArgs : AssemblyCachedEventArgs
	{
		private string shadowCopyDirectory;

		public string ShadowCopyDirectory
		{
			get
			{
				return this.shadowCopyDirectory;
			}
		}

		public AssemblyCopiedEventArgs(string originalAssemblyLocation, Assembly cachedAssembly, string shadowCopyDirectory) : base(originalAssemblyLocation, cachedAssembly)
		{
			this.shadowCopyDirectory = shadowCopyDirectory;
		}
	}
}