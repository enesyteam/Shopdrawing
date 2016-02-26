using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Reflection.Adds
{
	internal static class TypeNameParser
	{
		private readonly static char[] compoundTypeNameCharacters;

		static TypeNameParser()
		{
			System.Reflection.Adds.TypeNameParser.compoundTypeNameCharacters = new char[] { '+', ',', '[', '*', '&' };
		}

		private static Assembly DetermineAssembly(AssemblyName assemblyName, Module defaultTokenResolver, ITypeUniverse universe)
		{
			Module module = defaultTokenResolver;
			if (assemblyName == null)
			{
				if (defaultTokenResolver == null)
				{
					throw new ArgumentException(MetadataStringTable.DefaultTokenResolverRequired);
				}
				return module.Assembly;
			}
			if (universe == null)
			{
				throw new ArgumentException(MetadataStringTable.HostSpecifierMissing);
			}
			Assembly assembly = universe.ResolveAssembly(assemblyName);
			if (assembly == null)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string universeCannotResolveAssembly = MetadataStringTable.UniverseCannotResolveAssembly;
				object[] objArray = new object[] { assemblyName };
				throw new ArgumentException(string.Format(invariantCulture, universeCannotResolveAssembly, objArray));
			}
			return assembly;
		}

		public static bool IsCompoundType(string name)
		{
			return name.IndexOfAny(System.Reflection.Adds.TypeNameParser.compoundTypeNameCharacters) > 0;
		}

		public static Type ParseTypeName(ITypeUniverse universe, Module module, string input)
		{
			return System.Reflection.Adds.TypeNameParser.ParseTypeName(universe, module, input, true);
		}

		public static Type ParseTypeName(ITypeUniverse universe, Module module, string input, bool throwOnError)
		{
			Func<AssemblyName, Assembly> func = (AssemblyName assemblyName) => System.Reflection.Adds.TypeNameParser.DetermineAssembly(assemblyName, module, universe);
			Func<Assembly, string, bool, Type> func1 = (Assembly assembly, string simpleTypeName, bool ignoreCase) => {
				bool flag = false;
				if (assembly != null)
				{
					return assembly.GetType(simpleTypeName, flag, ignoreCase);
				}
				return module.GetType(simpleTypeName, flag, ignoreCase);
			};
			return Type.GetType(input, func, func1, throwOnError);
		}
	}
}