using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Microsoft.Expression.Project
{
	internal sealed class AssemblyService : IAssemblyService, ISatelliteAssemblyResolver
	{
		private IServiceProvider serviceProvider;

		private Dictionary<string, AssemblyService.AssemblyInfo> loadedAssemblies = new Dictionary<string, AssemblyService.AssemblyInfo>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, Assembly> satelliteAssemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, List<Assembly>> satelliteAssembliesForMain = new Dictionary<string, List<Assembly>>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, Assembly> installedAssemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

		private IDictionary<string, IAssemblyResolver> platformAssemblyResolverCache = new Dictionary<string, IAssemblyResolver>();

		private SortedList<AssemblyService.FrameworkSpec, IAssemblyResolver> platformAssemblyResolverTable = new SortedList<AssemblyService.FrameworkSpec, IAssemblyResolver>();

		private List<IAssemblyResolver> libraryAssemblyResolvers = new List<IAssemblyResolver>();

		private static string systemPath;

		private static string programFilesPath;

		private static string programFilesX86Path;

		private static string blendPath;

		private IDictionary<ReferenceAssemblyContext, int> referenceAssemblyContextTable = new Dictionary<ReferenceAssemblyContext, int>();

		private Microsoft.Expression.Project.AssemblyCache assemblyCache = new Microsoft.Expression.Project.AssemblyCache();

		public Microsoft.Expression.Project.AssemblyCache AssemblyCache
		{
			get
			{
				return this.assemblyCache;
			}
		}

		static AssemblyService()
		{
			AssemblyService.systemPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			AssemblyService.programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			AssemblyService.programFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			AssemblyService.blendPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		}

		public AssemblyService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
		}

		public ReferenceAssemblyContext CreateReferenceAssemblyContext(IProject project)
		{
			ReferenceAssemblyContext referenceAssemblyContext;
			ReferenceAssemblyContext referenceAssemblyContext1;
			ProjectType projectType = project.ProjectType as ProjectType;
			if (projectType != null)
			{
				ReferenceAssemblyMode referenceAssemblyMode = projectType.GetReferenceAssemblyMode(project);
				if (referenceAssemblyMode != ReferenceAssemblyMode.None)
				{
					if (referenceAssemblyMode == ReferenceAssemblyMode.TargetFramework)
					{
						using (IEnumerator<ReferenceAssemblyContext> enumerator = this.referenceAssemblyContextTable.Keys.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ReferenceAssemblyContext current = enumerator.Current;
								if (current.ReferenceAssemblyMode != ReferenceAssemblyMode.TargetFramework || !current.TargetFramework.Equals(project.TargetFramework))
								{
									continue;
								}
								this.referenceAssemblyContextTable[current] = this.referenceAssemblyContextTable[current] + 1;
								referenceAssemblyContext1 = current;
								return referenceAssemblyContext1;
							}
							referenceAssemblyContext = new ReferenceAssemblyContext(referenceAssemblyMode, project.TargetFramework);
							this.referenceAssemblyContextTable.Add(referenceAssemblyContext, 1);
							return referenceAssemblyContext;
						}
						return referenceAssemblyContext1;
					}
					referenceAssemblyContext = new ReferenceAssemblyContext(referenceAssemblyMode, project.TargetFramework);
					this.referenceAssemblyContextTable.Add(referenceAssemblyContext, 1);
					return referenceAssemblyContext;
				}
			}
			return null;
		}

		private Assembly CreateShadowCopy(string requestedAssemblyLocation, FileInfo fileInfo, out string shadowCopyLocation)
		{
			shadowCopyLocation = string.Empty;
			string str = this.AssemblyCache.CreateDirectory();
			string fileName = Path.GetFileName(requestedAssemblyLocation);
			shadowCopyLocation = Path.Combine(str, fileName);
			File.Copy(requestedAssemblyLocation, shadowCopyLocation);
			string str1 = Path.ChangeExtension(fileName, ".pdb");
			string str2 = Path.Combine(Path.GetDirectoryName(requestedAssemblyLocation), str1);
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str2))
			{
				File.Copy(str2, Path.Combine(str, str1));
			}
			string[] files = Directory.GetFiles(Path.GetDirectoryName(requestedAssemblyLocation), "*.lic");
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str3 = files[i];
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str3))
				{
					File.Copy(str3, Path.Combine(str, Path.GetFileName(str3)));
				}
			}
			Assembly assembly = ProjectAssemblyHelper.LoadFile(shadowCopyLocation);
			if (assembly == null)
			{
				return null;
			}
			this.TryCacheSatelliteAssembly(assembly, Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(requestedAssemblyLocation));
			if (!assembly.GlobalAssemblyCache)
			{
				this.loadedAssemblies[requestedAssemblyLocation] = new AssemblyService.AssemblyInfo(fileInfo, shadowCopyLocation, assembly);
				if (this.AssemblyCached != null)
				{
					this.AssemblyCached(null, new AssemblyCachedEventArgs(requestedAssemblyLocation, assembly));
				}
			}
			if (this.serviceProvider.AssemblyLoggingService() != null)
			{
				this.serviceProvider.AssemblyLoggingService().Log(new ShadowCopyAssemblyEvent(requestedAssemblyLocation, assembly));
			}
			if (this.AssemblyCopied != null)
			{
				this.AssemblyCopied(null, new AssemblyCopiedEventArgs(requestedAssemblyLocation, assembly, str));
			}
			return assembly;
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly assembly = null;
			if (!string.IsNullOrEmpty(args.Name))
			{
				assembly = this.ResolveAssembly(new AssemblyName(args.Name));
			}
			return assembly;
		}

		public void Dispose()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
			foreach (KeyValuePair<ReferenceAssemblyContext, int> keyValuePair in this.referenceAssemblyContextTable)
			{
				keyValuePair.Key.Dispose();
			}
			this.referenceAssemblyContextTable.Clear();
		}

		public IEnumerable<Assembly> GetCachedSatelliteAssembliesForMain(AssemblyName assemblyName)
		{
			List<Assembly> assemblies;
			if (this.satelliteAssembliesForMain.TryGetValue(assemblyName.FullName, out assemblies))
			{
				return assemblies;
			}
			return Enumerable.Empty<Assembly>();
		}

		public Assembly GetCachedSatelliteAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			if (this.satelliteAssemblies.TryGetValue(ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName), out assembly))
			{
				return assembly;
			}
			return null;
		}

		public IAssemblyResolver GetPlatformResolver(string frameworkSpec)
		{
			IAssemblyResolver item = null;
			if (!this.platformAssemblyResolverCache.TryGetValue(frameworkSpec, out item))
			{
				AssemblyService.FrameworkSpec frameworkSpec1 = new AssemblyService.FrameworkSpec(frameworkSpec);
				foreach (AssemblyService.FrameworkSpec key in this.platformAssemblyResolverTable.Keys)
				{
					if (!key.Match(frameworkSpec1))
					{
						continue;
					}
					item = this.platformAssemblyResolverTable[key];
					this.platformAssemblyResolverCache[frameworkSpec] = item;
					break;
				}
			}
			return item;
		}

		public bool IsInstalledAssembly(string path)
		{
			return this.IsInstalledAssemblyPath(path);
		}

		private bool IsInstalledAssemblyPath(string path)
		{
			if (path.StartsWith(AssemblyService.programFilesPath, StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(AssemblyService.programFilesX86Path) && path.StartsWith(AssemblyService.programFilesX86Path, StringComparison.OrdinalIgnoreCase) || path.StartsWith(AssemblyService.blendPath, StringComparison.OrdinalIgnoreCase) || path.StartsWith(RuntimeEnvironment.GetRuntimeDirectory(), StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return path.StartsWith(AssemblyService.systemPath, StringComparison.OrdinalIgnoreCase);
		}

		public void RegisterLibraryResolver(IAssemblyResolver assemblyResolver)
		{
			this.libraryAssemblyResolvers.Add(assemblyResolver);
		}

		public void RegisterPlatformResolver(string frameworkSpec, IAssemblyResolver assemblyResolver)
		{
			this.platformAssemblyResolverCache.Clear();
			this.platformAssemblyResolverTable.Add(new AssemblyService.FrameworkSpec(frameworkSpec), assemblyResolver);
		}

		public void ReleaseReferenceAssemblyContext(ReferenceAssemblyContext referenceAssemblyContext)
		{
			int num;
			if (referenceAssemblyContext != null && this.referenceAssemblyContextTable.TryGetValue(referenceAssemblyContext, out num))
			{
				this.referenceAssemblyContextTable[referenceAssemblyContext] = num - 1;
				if (this.referenceAssemblyContextTable[referenceAssemblyContext] == 0 && referenceAssemblyContext.ReferenceAssemblyMode != ReferenceAssemblyMode.TargetFramework)
				{
					referenceAssemblyContext.Dispose();
					this.referenceAssemblyContextTable.Remove(referenceAssemblyContext);
				}
			}
		}

		public Assembly ResolveAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			Assembly assembly1 = this.ResolvePlatformAssembly(assemblyName);
			if (assembly1 != null)
			{
				return assembly1;
			}
			Assembly assembly2 = this.ResolveLibraryAssembly(assemblyName);
			if (assembly2 != null)
			{
				return assembly2;
			}
			Assembly assembly3 = this.ResolveInstalledAssembly(assemblyName);
			if (assembly3 != null)
			{
				return assembly3;
			}
			IProjectManager service = (IProjectManager)this.serviceProvider.GetService(typeof(IProjectManager));
			if (service != null && service.CurrentSolution != null)
			{
				List<IProject>.Enumerator enumerator = service.CurrentSolution.Projects.ToList<IProject>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Assembly assembly4 = ((KnownProjectBase)enumerator.Current).ResolveAssembly(assemblyName);
						if (assembly4 == null)
						{
							continue;
						}
						assembly = assembly4;
						return assembly;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return assembly;
			}
			return null;
		}

		public AssemblyInformation ResolveAssembly(string path)
		{
			AssemblyName assemblyName = ProjectAssemblyHelper.CachedGetAssemblyNameFromPath(path);
			Assembly assembly = this.ResolvePlatformAssembly(assemblyName);
			if (assembly != null)
			{
				return new AssemblyInformation(assembly)
				{
					IsPlatformAssembly = true
				};
			}
			Assembly assembly1 = this.ResolveLibraryAssembly(assemblyName);
			if (assembly1 != null)
			{
				return new AssemblyInformation(assembly1);
			}
			Assembly assembly2 = this.ResolveInstalledAssembly(path);
			if (assembly2 != null)
			{
				return new AssemblyInformation(assembly2);
			}
			return this.ResolveShadowCopyAssembly(path);
		}

		private Assembly ResolveInstalledAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			string str = ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName);
			Dictionary<string, Assembly>.ValueCollection.Enumerator enumerator = this.installedAssemblies.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Assembly current = enumerator.Current;
					if (current.FullName != str)
					{
						continue;
					}
					assembly = current;
					return assembly;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return assembly;
		}

		private Assembly ResolveInstalledAssembly(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Assembly assembly = null;
			if (this.installedAssemblies.TryGetValue(path, out assembly))
			{
				return assembly;
			}
			if (this.IsInstalledAssembly(path))
			{
				assembly = ProjectAssemblyHelper.LoadFrom(path);
			}
			if (assembly != null)
			{
				this.installedAssemblies.Add(path, assembly);
			}
			return assembly;
		}

		public Assembly ResolveLibraryAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			if (assemblyName != null)
			{
				List<IAssemblyResolver>.Enumerator enumerator = this.libraryAssemblyResolvers.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Assembly assembly1 = enumerator.Current.ResolveAssembly(assemblyName);
						if (assembly1 == null)
						{
							continue;
						}
						assembly = assembly1;
						return assembly;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return assembly;
			}
			return null;
		}

		public Assembly ResolvePlatformAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			if (assemblyName != null)
			{
				using (IEnumerator<IAssemblyResolver> enumerator = this.platformAssemblyResolverTable.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Assembly assembly1 = enumerator.Current.ResolveAssembly(assemblyName);
						if (assembly1 == null)
						{
							continue;
						}
						assembly = assembly1;
						return assembly;
					}
					return null;
				}
				return assembly;
			}
			return null;
		}

		private AssemblyInformation ResolveShadowCopyAssembly(string path)
		{
			AssemblyService.AssemblyInfo assemblyInfo;
			AssemblyInformation assemblyInformation;
			try
			{
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
				{
					FileInfo fileInfo = new FileInfo(path);
					if (!this.loadedAssemblies.TryGetValue(path, out assemblyInfo) || !(assemblyInfo.OriginalAssemblyLastWriteTime == fileInfo.LastWriteTime))
					{
						string empty = string.Empty;
						Assembly assembly = this.CreateShadowCopy(path, fileInfo, out empty);
						AssemblyInformation assemblyInformation1 = new AssemblyInformation(assembly)
						{
							WasShadowCopied = true,
							ShadowCopyLocation = empty
						};
						assemblyInformation = assemblyInformation1;
						return assemblyInformation;
					}
					else
					{
						AssemblyInformation assemblyInformation2 = new AssemblyInformation(assemblyInfo.CachedAssembly)
						{
							ShadowCopyLocation = assemblyInfo.ShadowCopyLocation
						};
						assemblyInformation = assemblyInformation2;
						return assemblyInformation;
					}
				}
			}
			catch (IOException oException)
			{
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
			}
			catch (NotSupportedException notSupportedException)
			{
			}
			catch (ArgumentException argumentException)
			{
			}
			return new AssemblyInformation(null);
		}

		public void TryCacheSatelliteAssembly(Assembly baseAssembly, string originalAssemblyDirectory)
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach (string satelliteAssemblyName in ProjectAssemblyHelper.GetSatelliteAssemblyNames(baseAssembly, originalAssemblyDirectory))
			{
				Assembly assembly = ProjectAssemblyHelper.LoadFile(satelliteAssemblyName);
				if (assembly == null)
				{
					continue;
				}
				assemblies.Add(assembly);
				this.satelliteAssemblies[assembly.FullName] = assembly;
			}
			this.satelliteAssembliesForMain[baseAssembly.FullName] = assemblies;
		}

		public void UnregisterLibraryResolver(IAssemblyResolver assemblyResolver)
		{
			this.libraryAssemblyResolvers.Remove(assemblyResolver);
		}

		public void UnregisterPlatformResolver(string frameworkSpec)
		{
			this.platformAssemblyResolverCache.Clear();
			this.platformAssemblyResolverTable.Remove(new AssemblyService.FrameworkSpec(frameworkSpec));
		}

		public List<string> UpdateCacheWithExternalChanges()
		{
			string str;
			List<AssemblyService.CurrentAssemblyInfo> currentAssemblyInfos = new List<AssemblyService.CurrentAssemblyInfo>();
			foreach (AssemblyService.AssemblyInfo value in this.loadedAssemblies.Values)
			{
				FileInfo fileInfo = new FileInfo(value.OriginalAssembyPath);
				if (!fileInfo.Exists)
				{
					continue;
				}
				try
				{
					if (value.OriginalAssemblyLastWriteTime != fileInfo.LastWriteTime)
					{
						currentAssemblyInfos.Add(new AssemblyService.CurrentAssemblyInfo(value.OriginalAssembyPath, fileInfo));
					}
				}
				catch (BadImageFormatException badImageFormatException)
				{
				}
				catch (IOException oException)
				{
				}
				catch (UnauthorizedAccessException unauthorizedAccessException)
				{
				}
			}
			List<string> strs = new List<string>(currentAssemblyInfos.Count);
			foreach (AssemblyService.CurrentAssemblyInfo currentAssemblyInfo in currentAssemblyInfos)
			{
				try
				{
					this.CreateShadowCopy(currentAssemblyInfo.AssemblyLocation, currentAssemblyInfo.FileInfo, out str);
					strs.Add(currentAssemblyInfo.AssemblyLocation);
				}
				catch (BadImageFormatException badImageFormatException1)
				{
				}
				catch (IOException oException1)
				{
				}
				catch (UnauthorizedAccessException unauthorizedAccessException1)
				{
				}
			}
			return strs;
		}

		public event AssemblyCachedHandler AssemblyCached;

		public event AssemblyCopiedHandler AssemblyCopied;

		private sealed class AssemblyInfo
		{
			private string fullName;

			private DateTime lastWriteTime;

			private string shadowCopyLocation;

			private Assembly assembly;

			public Assembly CachedAssembly
			{
				get
				{
					return this.assembly;
				}
			}

			public DateTime OriginalAssemblyLastWriteTime
			{
				get
				{
					return this.lastWriteTime;
				}
			}

			public string OriginalAssembyPath
			{
				get
				{
					return this.fullName;
				}
			}

			public string ShadowCopyLocation
			{
				get
				{
					return this.shadowCopyLocation;
				}
			}

			public AssemblyInfo(FileInfo fileInfo, string shadowCopyLocation, Assembly assembly)
			{
				this.fullName = fileInfo.FullName;
				this.lastWriteTime = fileInfo.LastWriteTime;
				this.shadowCopyLocation = shadowCopyLocation;
				this.assembly = assembly;
			}
		}

		private struct CurrentAssemblyInfo
		{
			private FileInfo fileInfo;

			private string assemblyLocation;

			public string AssemblyLocation
			{
				get
				{
					return this.assemblyLocation;
				}
			}

			public FileInfo FileInfo
			{
				get
				{
					return this.fileInfo;
				}
			}

			public CurrentAssemblyInfo(string assemblyLocation, FileInfo fileInfo)
			{
				this.assemblyLocation = assemblyLocation;
				this.fileInfo = fileInfo;
			}
		}

		private sealed class FrameworkSpec : IComparable<AssemblyService.FrameworkSpec>
		{
			private string identifier;

			private string version;

			private string profile;

			public FrameworkSpec(string value)
			{
				string[] strArrays = value.Split(new char[] { ',' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string[] strArrays1 = strArrays[i].Trim().Split(new char[] { '=' });
					if ((int)strArrays1.Length == 1)
					{
						this.identifier = strArrays1[0];
					}
					else if ((int)strArrays1.Length == 2)
					{
						string str = strArrays1[0];
						string str1 = str;
						if (str != null)
						{
							if (str1 == "Version")
							{
								this.version = (strArrays1[1].StartsWith("v", StringComparison.Ordinal) ? strArrays1[1].Substring(1) : strArrays1[1]);
							}
							else if (str1 == "Profile")
							{
								this.profile = strArrays1[1];
							}
						}
					}
				}
			}

			public int CompareTo(AssemblyService.FrameworkSpec other)
			{
				int num = string.Compare(other.identifier, this.identifier, StringComparison.Ordinal);
				if (num != 0)
				{
					return num;
				}
				int num1 = string.Compare(other.version, this.version, StringComparison.Ordinal);
				if (num1 != 0)
				{
					return num1;
				}
				int num2 = string.Compare(other.profile, this.profile, StringComparison.Ordinal);
				if (num2 != 0)
				{
					return num2;
				}
				return 0;
			}

			public override bool Equals(object obj)
			{
				AssemblyService.FrameworkSpec frameworkSpec = obj as AssemblyService.FrameworkSpec;
				if (frameworkSpec == null || !(this.identifier == frameworkSpec.identifier) || !(this.version == frameworkSpec.version))
				{
					return false;
				}
				return this.profile == frameworkSpec.profile;
			}

			public override int GetHashCode()
			{
				return this.identifier.GetHashCode() ^ this.version.GetHashCode() ^ this.profile.GetHashCode();
			}

			public bool Match(AssemblyService.FrameworkSpec other)
			{
				if (string.Compare(this.identifier, other.identifier, StringComparison.Ordinal) == 0 && (string.Compare(this.profile, other.profile, StringComparison.Ordinal) == 0 || string.IsNullOrEmpty(this.profile)) && (string.Compare(this.version, other.version, StringComparison.Ordinal) == 0 || string.IsNullOrEmpty(this.version)))
				{
					return true;
				}
				return false;
			}

			public override string ToString()
			{
				string str = this.identifier;
				if (!string.IsNullOrEmpty(this.version))
				{
					str = string.Concat(str, ", Version=", this.version);
				}
				if (!string.IsNullOrEmpty(this.profile))
				{
					str = string.Concat(str, ", Profile=", this.profile);
				}
				return str;
			}
		}
	}
}