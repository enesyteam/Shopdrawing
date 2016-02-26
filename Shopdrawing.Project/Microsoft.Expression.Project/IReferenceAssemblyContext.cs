using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public interface IReferenceAssemblyContext
	{
		Guid Identifier
		{
			get;
		}

		Microsoft.Expression.Project.ReferenceAssemblyMode ReferenceAssemblyMode
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