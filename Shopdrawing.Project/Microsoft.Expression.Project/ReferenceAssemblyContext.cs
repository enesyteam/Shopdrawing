using Microsoft.Expression.Framework.Documents;
using Microsoft.MetadataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	internal sealed class ReferenceAssemblyContext : IReferenceAssemblyContext, IDisposable
	{
		private Guid identifier;

		private Microsoft.Expression.Project.ReferenceAssemblyMode referenceAssemblyMode;

		private FrameworkName targetFramework;

		private ReferenceAssemblyContext.ReferenceAssemblyUniverse defaultUniverse;

		private IDictionary<Assembly, Assembly> referenceAssemblyTable = new Dictionary<Assembly, Assembly>();

		private ReferenceAssemblyResolver referenceAssemblyResolver;

		public Guid Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public Microsoft.Expression.Project.ReferenceAssemblyMode ReferenceAssemblyMode
		{
			get
			{
				return this.referenceAssemblyMode;
			}
		}

		public FrameworkName TargetFramework
		{
			get
			{
				return this.targetFramework;
			}
		}

		public ReferenceAssemblyContext(Microsoft.Expression.Project.ReferenceAssemblyMode referenceAssemblyMode, FrameworkName targetFramework)
		{
			this.identifier = Guid.NewGuid();
			this.referenceAssemblyMode = referenceAssemblyMode;
			this.targetFramework = targetFramework;
			this.defaultUniverse = new ReferenceAssemblyContext.ReferenceAssemblyUniverse(this);
			this.defaultUniverse.OnResolveEvent += new EventHandler<ResolveAssemblyNameEventArgs>(this.DefaultUniverse_OnResolveEvent);
		}

		private void DefaultUniverse_OnResolveEvent(object sender, ResolveAssemblyNameEventArgs e)
		{
			Assembly assembly = this.ResolveReferenceAssembly(e.Name.FullName);
			if (assembly != null)
			{
				e.Target = assembly;
			}
		}

		public void Dispose()
		{
			if (this.defaultUniverse != null)
			{
				this.defaultUniverse.OnResolveEvent -= new EventHandler<ResolveAssemblyNameEventArgs>(this.DefaultUniverse_OnResolveEvent);
				this.defaultUniverse.Dispose();
				this.defaultUniverse = null;
			}
		}

		public Assembly ResolveReferenceAssembly(Assembly runtimeAssembly)
		{
			Assembly assembly = null;
			if (!this.referenceAssemblyTable.TryGetValue(runtimeAssembly, out assembly))
			{
				assembly = this.ResolveReferenceAssembly(ProjectAssemblyHelper.GetAssemblyName(runtimeAssembly));
				this.referenceAssemblyTable[runtimeAssembly] = assembly;
			}
			return assembly;
		}

		public Assembly ResolveReferenceAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			using (IEnumerator<Assembly> enumerator = this.defaultUniverse.Assemblies.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Assembly current = enumerator.Current;
					if (!AssemblyName.ReferenceMatchesDefinition(assemblyName, current.GetName()))
					{
						continue;
					}
					assembly = current;
					return assembly;
				}
				return this.ResolveReferenceAssembly(ProjectAssemblyHelper.GetAssemblySpec(assemblyName));
			}
			return assembly;
		}

		private Assembly ResolveReferenceAssembly(string assemblySpec)
		{
			string str = this.ResolveReferenceAssemblyPath(assemblySpec);
			if (string.IsNullOrEmpty(str) || !PathHelper.FileExists(str))
			{
				return null;
			}
			return this.defaultUniverse.LoadAssemblyFromFile(str);
		}

		private string ResolveReferenceAssemblyPath(string assemblySpec)
		{
			string str;
			if (this.referenceAssemblyResolver == null)
			{
				this.referenceAssemblyResolver = new ReferenceAssemblyResolver(this.targetFramework);
			}
			ReferenceAssemblyResolver referenceAssemblyResolver = this.referenceAssemblyResolver;
			string[] strArrays = new string[] { assemblySpec };
			if (referenceAssemblyResolver.ResolveAssemblyPaths(strArrays).TryGetValue(assemblySpec, out str))
			{
				return str;
			}
			return string.Empty;
		}

		public override string ToString()
		{
			return this.identifier.ToString();
		}

		internal void UpdateReferenceAssembly(ProjectAssembly projectAssembly)
		{
			if (projectAssembly.IsPlatformAssembly && projectAssembly.ReferenceAssembly == null)
			{
				Assembly runtimeAssembly = projectAssembly.RuntimeAssembly;
				if (runtimeAssembly != null)
				{
					Assembly assembly = null;
					if (!this.referenceAssemblyTable.TryGetValue(runtimeAssembly, out assembly) || assembly == null)
					{
						string empty = string.Empty;
						switch (this.referenceAssemblyMode)
						{
							case Microsoft.Expression.Project.ReferenceAssemblyMode.TargetFramework:
							{
								empty = this.ResolveReferenceAssemblyPath(projectAssembly.Name);
								break;
							}
							case Microsoft.Expression.Project.ReferenceAssemblyMode.Project:
							{
								empty = projectAssembly.Path;
								break;
							}
						}
						if (!string.IsNullOrEmpty(empty))
						{
							assembly = this.defaultUniverse.LoadAssemblyFromFile(empty);
							if (assembly != null)
							{
								this.referenceAssemblyTable[runtimeAssembly] = assembly;
							}
						}
					}
					projectAssembly.ReferenceAssembly = assembly;
				}
			}
		}

		private class ReferenceAssemblyUniverse : DefaultUniverse
		{
			private ReferenceAssemblyContext context;

			private bool mscorlibLoaded;

			public ReferenceAssemblyUniverse(ReferenceAssemblyContext context)
			{
				this.context = context;
			}

			public override Assembly GetSystemAssembly()
			{
				if (!this.mscorlibLoaded)
				{
					this.mscorlibLoaded = true;
					Assembly assembly = typeof(int).Assembly;
					Assembly assembly1 = this.context.ResolveReferenceAssembly(assembly);
					if (assembly1 == null)
					{
						assembly1 = this.context.ResolveReferenceAssembly(assembly.GetName());
					}
					if (assembly1 != null)
					{
						base.SetSystemAssembly(assembly1);
					}
				}
				return base.GetSystemAssembly();
			}
		}
	}
}