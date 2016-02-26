using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IPlatformReferenceContext
	{
		bool KeepAlive
		{
			get;
		}

		FrameworkName TargetFramework
		{
			get;
		}

		Assembly ResolveReferenceAssembly(Assembly runtimeAssembly);

		Assembly ResolveReferenceAssembly(AssemblyName assemblyName);
	}
}