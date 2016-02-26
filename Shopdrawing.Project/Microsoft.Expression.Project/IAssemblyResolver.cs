using System.Reflection;

namespace Microsoft.Expression.Project
{
	public interface IAssemblyResolver
	{
		Assembly ResolveAssembly(AssemblyName assemblyName);
	}
}