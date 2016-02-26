using System;
using System.Reflection;

namespace System.Reflection.Adds
{
	internal static class Helpers
	{
		public static Type EnsureResolve(Type type)
		{
			while (true)
			{
				ITypeProxy typeProxy = type as ITypeProxy;
				if (typeProxy == null)
				{
					break;
				}
				type = typeProxy.GetResolvedType();
			}
			return type;
		}

		public static ITypeUniverse Universe(Type type)
		{
			ITypeProxy typeProxy = type as ITypeProxy;
			if (typeProxy != null)
			{
				return typeProxy.TypeUniverse;
			}
			IAssembly2 assembly = type.Assembly as IAssembly2;
			if (assembly == null)
			{
				return null;
			}
			return assembly.TypeUniverse;
		}
	}
}