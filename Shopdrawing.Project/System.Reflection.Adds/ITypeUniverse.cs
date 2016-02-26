using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal interface ITypeUniverse
	{
		Type GetBuiltInType(System.Reflection.Adds.CorElementType elementType);

		Assembly GetSystemAssembly();

		Type GetTypeXFromName(string fullName);

		Assembly ResolveAssembly(AssemblyName name);

		Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef);

		Module ResolveModule(Assembly containingAssembly, string moduleName);
	}
}