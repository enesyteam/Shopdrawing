using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class DefaultPlatformReferenceContext : IPlatformReferenceContext
	{
		private FrameworkName targetFramework;

		public bool KeepAlive
		{
			get
			{
				return true;
			}
		}

		public FrameworkName TargetFramework
		{
			get
			{
				return this.targetFramework;
			}
		}

		public DefaultPlatformReferenceContext(FrameworkName targetFramework)
		{
			this.targetFramework = targetFramework;
		}

		public Assembly ResolveReferenceAssembly(Assembly runtimeAssembly)
		{
			return null;
		}

		public Assembly ResolveReferenceAssembly(AssemblyName assemblyName)
		{
			return null;
		}

		public override string ToString()
		{
			return string.Concat("Default: ", this.TargetFramework.ToString());
		}
	}
}