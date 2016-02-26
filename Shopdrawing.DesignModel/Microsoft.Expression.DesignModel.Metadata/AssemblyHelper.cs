using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class AssemblyHelper
	{
		private static Hashtable assemblyNameCache;

		private static Dictionary<Assembly, bool> assemblies;

		private static string ProgramFiles
		{
			get
			{
				return AssemblyHelper.NormalizePath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			}
		}

		private static string SystemRoot
		{
			get
			{
				return AssemblyHelper.NormalizePath(Environment.GetEnvironmentVariable("SystemRoot") ?? Environment.SystemDirectory);
			}
		}

		static AssemblyHelper()
		{
			AssemblyHelper.assemblyNameCache = Hashtable.Synchronized(new Hashtable());
			AssemblyHelper.assemblies = new Dictionary<Assembly, bool>();
		}

		public static AssemblyName GetAssemblyName(Assembly assembly)
		{
			if (!AssemblyHelper.assemblyNameCache.ContainsKey(assembly))
			{
				AssemblyHelper.assemblyNameCache[assembly] = assembly.GetName();
			}
			return (AssemblyName)AssemblyHelper.assemblyNameCache[assembly];
		}

		public static AssemblyName GetAssemblyName(IAssembly assembly)
		{
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly == null)
			{
				return null;
			}
			return AssemblyHelper.GetAssemblyName(reflectionAssembly);
		}

		public static Assembly GetReflectionAssembly(IAssembly assembly)
		{
			IReflectionAssembly reflectionAssembly = assembly as IReflectionAssembly;
			if (reflectionAssembly == null)
			{
				return null;
			}
			return reflectionAssembly.ReflectionAssembly;
		}

		public static Type[] GetTypes(IAssembly assembly)
		{
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly == null)
			{
				return Type.EmptyTypes;
			}
			return reflectionAssembly.GetTypes();
		}

		public static bool IsPlatformAssembly(IAssembly assembly)
		{
			Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly runtimeAssembly = assembly as Microsoft.Expression.DesignModel.Metadata.RuntimeAssembly;
			if (runtimeAssembly == null)
			{
				return false;
			}
			return runtimeAssembly.IsPlatformAssembly;
		}

		public static bool IsSystemAssembly(IAssembly assembly)
		{
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly == null)
			{
				return false;
			}
			return AssemblyHelper.IsSystemAssembly(reflectionAssembly);
		}

		public static bool IsSystemAssembly(Assembly assembly)
		{
			bool flag;
			bool flag1;
			if (!AssemblyHelper.assemblies.TryGetValue(assembly, out flag))
			{
				string empty = string.Empty;
				try
				{
					empty = assembly.Location;
					flag = (empty.StartsWith(AssemblyHelper.SystemRoot, StringComparison.OrdinalIgnoreCase) || empty.StartsWith(AssemblyHelper.ProgramFiles, StringComparison.OrdinalIgnoreCase) ? true : false);
					AssemblyHelper.assemblies[assembly] = flag;
					return flag;
				}
				catch
				{
					flag1 = false;
				}
				return flag1;
			}
			return flag;
		}

		public static IEnumerable<IAssembly> LoadReferencedAssemblies(IAssembly assembly, Func<AssemblyName, Assembly> assemblyResolverCallback, IPlatformTypes platformTypes)
		{
			List<IAssembly> assemblies = new List<IAssembly>();
			Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
			if (reflectionAssembly != null)
			{
				AssemblyName[] referencedAssemblies = reflectionAssembly.GetReferencedAssemblies();
				if (referencedAssemblies != null)
				{
					AssemblyName[] assemblyNameArray = referencedAssemblies;
					for (int i = 0; i < (int)assemblyNameArray.Length; i++)
					{
						Assembly assembly1 = assemblyResolverCallback(assemblyNameArray[i]);
						if (assembly1 != null)
						{
							assemblies.Add(platformTypes.CreateAssembly(assembly1, AssemblySource.Unknown));
						}
					}
				}
			}
			return assemblies;
		}

		private static string NormalizePath(string path)
		{
			if (path == null)
			{
				return string.Empty;
			}
			if (!path.EndsWith("\\", StringComparison.Ordinal))
			{
				path = string.Concat(path, "\\");
			}
			return path;
		}

		public static IAssemblyId ParseAssemblyId(string fullName)
		{
			return new AssemblyId(fullName);
		}
	}
}