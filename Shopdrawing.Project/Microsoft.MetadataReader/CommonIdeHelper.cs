using System;
using System.Reflection;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal static class CommonIdeHelper
	{
		public static AssemblyName GetNameFromPath(string path)
		{
			CommonIdeHelper.EmptyUniverse emptyUniverse = new CommonIdeHelper.EmptyUniverse();
			MetadataFile metadataFile = (new MetadataDispenser()).OpenFile(path);
			return AssemblyFactory.CreateAssembly(emptyUniverse, metadataFile, path).GetName();
		}

		private class EmptyUniverse : ITypeUniverse
		{
			public EmptyUniverse()
			{
			}

			public Type GetBuiltInType(System.Reflection.Adds.CorElementType elementType)
			{
				throw new NotImplementedException();
			}

			public Assembly GetSystemAssembly()
			{
				throw new NotImplementedException();
			}

			public Type GetTypeXFromName(string fullName)
			{
				throw new NotImplementedException();
			}

			public Assembly ResolveAssembly(AssemblyName name)
			{
				throw new NotImplementedException();
			}

			public Assembly ResolveAssembly(Module scope, Token tokenAssemblyRef)
			{
				throw new NotImplementedException();
			}

			public Module ResolveModule(Assembly containingAssembly, string moduleName)
			{
				throw new NotImplementedException();
			}
		}
	}
}