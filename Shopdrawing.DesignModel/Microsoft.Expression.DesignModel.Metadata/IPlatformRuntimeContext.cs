using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IPlatformRuntimeContext
	{
		Assembly ResolveRuntimeAssembly(AssemblyName assemblyName);
	}
}