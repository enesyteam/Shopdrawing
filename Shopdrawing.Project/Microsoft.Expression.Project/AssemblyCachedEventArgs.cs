using System;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class AssemblyCachedEventArgs : EventArgs
	{
		private string originalAssemblyLocation;

		private Assembly cachedAssembly;

		public Assembly CachedAssembly
		{
			get
			{
				return this.cachedAssembly;
			}
		}

		public string OriginalAssemblyLocation
		{
			get
			{
				return this.originalAssemblyLocation;
			}
		}

		public AssemblyCachedEventArgs(string originalAssemblyLocation, Assembly cachedAssembly)
		{
			this.originalAssemblyLocation = originalAssemblyLocation;
			this.cachedAssembly = cachedAssembly;
		}
	}
}