using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface ISatelliteAssemblyResolver
	{
		IEnumerable<Assembly> GetCachedSatelliteAssembliesForMain(AssemblyName assemblyName);

		Assembly GetCachedSatelliteAssembly(AssemblyName assemblyName);
	}
}