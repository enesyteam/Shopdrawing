using Microsoft.Build.Utilities;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Microsoft.Expression.Project
{
	public static class ProjectAssemblyHelper
	{
		private static Hashtable assemblyToAssemblyNameCache;

		private static Hashtable pathToAssemblyNameCache;

		private static Hashtable assemblyNameToFullNameCache;

		static ProjectAssemblyHelper()
		{
			ProjectAssemblyHelper.assemblyToAssemblyNameCache = Hashtable.Synchronized(new Hashtable());
			ProjectAssemblyHelper.pathToAssemblyNameCache = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));
			ProjectAssemblyHelper.assemblyNameToFullNameCache = Hashtable.Synchronized(new Hashtable());
		}

		public static string CachedGetAssemblyFullName(AssemblyName assemblyName)
		{
			if (!ProjectAssemblyHelper.assemblyNameToFullNameCache.ContainsKey(assemblyName))
			{
				ProjectAssemblyHelper.assemblyNameToFullNameCache[assemblyName] = assemblyName.FullName;
			}
			return (string)ProjectAssemblyHelper.assemblyNameToFullNameCache[assemblyName];
		}

		public static AssemblyName CachedGetAssemblyNameFromPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			if (!ProjectAssemblyHelper.pathToAssemblyNameCache.ContainsKey(path))
			{
				ProjectAssemblyHelper.pathToAssemblyNameCache[path] = ProjectAssemblyHelper.GetAssemblyNameFromPath(path);
			}
			AssemblyName item = (AssemblyName)ProjectAssemblyHelper.pathToAssemblyNameCache[path];
			if (item != null)
			{
				ProjectAssemblyHelper.CachedGetAssemblyFullName(item);
			}
			return item;
		}

		internal static bool CanAssemblyBeFoundInReferenceFolders(AssemblyName assemblyName, string assemblyPath, FrameworkName targetFramework)
		{
			bool flag;
			if (targetFramework == null)
			{
				return false;
			}
			IEnumerable<string> strs = ToolLocationHelper.GetPathToReferenceAssemblies(targetFramework).Concat<string>(ProjectAssemblyHelper.GetAssemblyFolderExLocations(targetFramework).Distinct<string>());
			using (IEnumerator<string> enumerator = strs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					using (IEnumerator<string> enumerator1 = Directory.EnumerateFiles(enumerator.Current).GetEnumerator())
					{
						while (enumerator1.MoveNext())
						{
							string current = enumerator1.Current;
							if (!Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(current, assemblyPath))
							{
								if (!string.Equals(ProjectAssemblyHelper.TrimFiletypeFromAssemblyNameOrPath(current), assemblyName.Name, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								AssemblyName assemblyNameFromPath = ProjectAssemblyHelper.GetAssemblyNameFromPath(current);
								if (assemblyNameFromPath == null || !string.Equals(assemblyNameFromPath.FullName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								flag = true;
								return flag;
							}
							else
							{
								flag = true;
								return flag;
							}
						}
					}
				}
				return false;
			}
			return flag;
		}

		public static bool ComparePublicKeyTokens(byte[] publicKeyToken1, byte[] publicKeyToken2)
		{
			return ArrayHelper.Compare<byte>(publicKeyToken1, publicKeyToken2);
		}

		private static IEnumerable<string> CopyResourceAssemblies(Assembly cachedAssembly, string originalAssemblyDirectory)
		{
			string str = null;
			try
			{
				object[] customAttributes = cachedAssembly.GetCustomAttributes(false);
				int num = 0;
				while (num < (int)customAttributes.Length)
				{
					NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = (Attribute)customAttributes[num] as NeutralResourcesLanguageAttribute;
					if (neutralResourcesLanguageAttribute == null)
					{
						num++;
					}
					else
					{
						str = neutralResourcesLanguageAttribute.CultureName;
						break;
					}
				}
			}
			catch (Exception exception)
			{
			}
			for (CultureInfo i = CultureInfo.CurrentUICulture; !i.Equals(CultureInfo.InvariantCulture); i = i.Parent)
			{
				string str1 = i.ToString();
				if (str != null && string.Equals(str1, str, StringComparison.OrdinalIgnoreCase))
				{
					str = null;
				}
				string str2 = ProjectAssemblyHelper.TryCopyResourceAssembly(cachedAssembly, originalAssemblyDirectory, str1);
				if (str2 != null)
				{
					yield return str2;
				}
			}
			if (str != null)
			{
				string str3 = ProjectAssemblyHelper.TryCopyResourceAssembly(cachedAssembly, originalAssemblyDirectory, str);
				if (str3 != null)
				{
					yield return str3;
				}
			}
		}

		private static string EncodeHexString(byte[] buffer)
		{
			if (buffer == null)
			{
				return null;
			}
			string empty = string.Empty;
			for (int i = 0; i < (int)buffer.Length; i++)
			{
				empty = string.Concat(empty, buffer[i].ToString("x2", CultureInfo.InvariantCulture));
			}
			return empty;
		}

		private static IEnumerable<string> GetAssemblyFolderExLocations(FrameworkName targetFramework)
		{
			RegistryKey[] currentUser = new RegistryKey[] { Registry.CurrentUser, Registry.LocalMachine };
			for (int i = 0; i < (int)currentUser.Length; i++)
			{
				RegistryKey registryKey = currentUser[i];
				RegistryKey registryKey1 = null;
				string identifier = targetFramework.Identifier;
				string str = identifier;
				if (identifier != null)
				{
					if (str == ".NETFramework")
					{
						registryKey1 = RegistryHelper.OpenSubkey(registryKey, "SOFTWARE\\Microsoft\\.NETFramework", false);
					}
					else if (str == "Silverlight")
					{
						registryKey1 = RegistryHelper.OpenSubkey(registryKey, "SOFTWARE\\Microsoft\\Microsoft SDKs\\Silverlight", false);
					}
				}
				if (registryKey1 != null)
				{
					using (registryKey1)
					{
						foreach (string subkeyName in RegistryHelper.GetSubkeyNames(registryKey1))
						{
							Version version = null;
							if (string.IsNullOrEmpty(subkeyName) || subkeyName[0] != 'v' || subkeyName.Length < 4 || !Version.TryParse(subkeyName.Substring(1), out version) || version.Major > targetFramework.Version.Major || version.Major == targetFramework.Version.Major && version.Minor > targetFramework.Version.Minor)
							{
								continue;
							}
							foreach (RegistryKey subkey in RegistryHelper.GetSubkeys(registryKey1, string.Concat(subkeyName, "\\AssemblyFoldersEx")))
							{
								string str1 = null;
								using (subkey)
								{
									str1 = RegistryHelper.RetrieveRegistryValue<string>(subkey, null);
								}
								if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str1))
								{
									continue;
								}
								yield return str1;
							}
						}
					}
				}
			}
		}

		public static AssemblyName GetAssemblyName(Assembly assembly)
		{
			if (!ProjectAssemblyHelper.assemblyToAssemblyNameCache.ContainsKey(assembly))
			{
				ProjectAssemblyHelper.assemblyToAssemblyNameCache[assembly] = assembly.GetName();
			}
			AssemblyName item = (AssemblyName)ProjectAssemblyHelper.assemblyToAssemblyNameCache[assembly];
			if (item != null && !ProjectAssemblyHelper.assemblyNameToFullNameCache.ContainsKey(item))
			{
				ProjectAssemblyHelper.assemblyNameToFullNameCache[item] = assembly.FullName;
			}
			return item;
		}

		public static AssemblyName GetAssemblyNameFromPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			AssemblyName assemblyName = null;
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
			{
				try
				{
					assemblyName = AssemblyName.GetAssemblyName(path);
				}
				catch (ArgumentException argumentException)
				{
				}
				catch (IOException oException)
				{
				}
				catch (BadImageFormatException badImageFormatException)
				{
				}
				catch (OutOfMemoryException outOfMemoryException)
				{
					LowMemoryMessage.Show();
				}
			}
			return assemblyName;
		}

		public static string GetAssemblySpec(AssemblyName assemblyName)
		{
			string str = (assemblyName.CultureInfo == null || string.IsNullOrEmpty(assemblyName.CultureInfo.Name) ? "neutral" : assemblyName.CultureInfo.Name);
			string name = assemblyName.Name;
			string str1 = string.Concat(name, ", Culture=", str);
			if (!name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase))
			{
				string str2 = ProjectAssemblyHelper.EncodeHexString(assemblyName.GetPublicKeyToken());
				if (str2 != null)
				{
					str1 = string.Concat(str1, ", PublicKeyToken=", (str2.Length == 0 ? "null" : str2));
				}
			}
			return str1;
		}

		public static IEnumerable<string> GetSatelliteAssemblyNames(Assembly assembly, string originalAssemblyDirectory)
		{
			return ProjectAssemblyHelper.CopyResourceAssemblies(assembly, originalAssemblyDirectory).Reverse<string>();
		}

		public static Assembly Load(string assemblyFullName)
		{
			try
			{
				return Assembly.Load(assemblyFullName);
			}
			catch (IOException oException)
			{
			}
			catch (BadImageFormatException badImageFormatException)
			{
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
			}
			return null;
		}

		public static Assembly Load(AssemblyName assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch (IOException oException)
			{
			}
			catch (BadImageFormatException badImageFormatException)
			{
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
			}
			return null;
		}

		public static Assembly LoadFile(string path)
		{
			try
			{
				return Assembly.LoadFile(path);
			}
			catch (IOException oException)
			{
			}
			catch (BadImageFormatException badImageFormatException)
			{
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
			}
			return null;
		}

		public static Assembly LoadFrom(string path)
		{
			Assembly assembly = null;
			try
			{
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
				{
					assembly = Assembly.LoadFrom(path);
				}
			}
			catch (IOException oException)
			{
			}
			catch (BadImageFormatException badImageFormatException)
			{
			}
			catch (OutOfMemoryException outOfMemoryException)
			{
				LowMemoryMessage.Show();
			}
			return assembly;
		}

		internal static string TrimFiletypeFromAssemblyNameOrPath(string assemblyNameOrPath)
		{
			string fileName = Path.GetFileName(assemblyNameOrPath);
			if (fileName == null || !fileName.EndsWith(".DLL", StringComparison.OrdinalIgnoreCase) && !fileName.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase))
			{
				return fileName;
			}
			return Path.GetFileNameWithoutExtension(assemblyNameOrPath);
		}

		private static string TryCopyResourceAssembly(Assembly cachedAssembly, string originalAssemblyDirectory, string resourceCulture)
		{
			string str = Path.Combine(originalAssemblyDirectory, resourceCulture);
			string str1 = Path.Combine(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(cachedAssembly.Location), resourceCulture);
			string str2 = string.Concat(ProjectAssemblyHelper.GetAssemblyName(cachedAssembly).Name, ".resources.dll");
			string str3 = Path.Combine(str, str2);
			string str4 = Path.Combine(str1, str2);
			if (!File.Exists(str3))
			{
				return null;
			}
			if (Microsoft.Expression.Framework.Documents.PathHelper.ArePathsEquivalent(str3, str4))
			{
				return str4;
			}
			if (File.Exists(str4))
			{
				return null;
			}
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(str1))
			{
				Directory.CreateDirectory(str1);
			}
			File.Copy(str3, str4);
			return str4;
		}

		public static string UncachedGetAssemblyFullName(AssemblyName assemblyName)
		{
			return assemblyName.FullName;
		}
	}
}