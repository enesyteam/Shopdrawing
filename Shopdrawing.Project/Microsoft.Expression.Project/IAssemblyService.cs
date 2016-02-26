using System;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IAssemblyService
	{
		IAssemblyResolver GetPlatformResolver(string frameworkSpec);

		bool IsInstalledAssembly(string path);

		void RegisterLibraryResolver(IAssemblyResolver assemblyResolver);

		void RegisterPlatformResolver(string frameworkSpec, IAssemblyResolver assemblyResolver);

		Assembly ResolveAssembly(AssemblyName assemblyName);

		Assembly ResolveLibraryAssembly(AssemblyName assemblyName);

		Assembly ResolvePlatformAssembly(AssemblyName assemblyName);

		void UnregisterLibraryResolver(IAssemblyResolver assemblyResolver);

		void UnregisterPlatformResolver(string frameworkSpec);
	}
}