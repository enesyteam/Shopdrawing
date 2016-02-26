using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	public sealed class BlendAssemblyResolver : IAssemblyResolver
	{
		private object syncLock = new object();

		private bool hasEntered;

		private Dictionary<string, Assembly> blendAssemblies = new Dictionary<string, Assembly>();

		private Dictionary<string, string> delayLoadedAssemblies;

		public BlendAssemblyResolver()
		{
			UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.InitializeBlendLoadedAssemblies));
			Assembly assembly = typeof(BlendAssemblyResolver).Assembly;
			string codeBase = assembly.CodeBase;
			codeBase = codeBase.Substring(0, codeBase.LastIndexOf('/') + 1);
			string localPath = (new Uri(codeBase)).LocalPath;
			string str = ProjectAssemblyHelper.GetAssemblyName(assembly).Version.ToString();
			Dictionary<string, string> strs = new Dictionary<string, string>(8)
			{
				{ string.Concat("Microsoft.Expression.Platform.WPF, Version=", str, ", Culture=neutral, PublicKeyToken=31bf3856ad364e35"), PathHelper.ResolveCombinedPath(localPath, "Microsoft.Expression.Platform.WPF.dll") },
				{ string.Concat("Microsoft.Expression.Platform.Silverlight, Version=", str, ", Culture=neutral, PublicKeyToken=31bf3856ad364e35"), PathHelper.ResolveCombinedPath(localPath, "Microsoft.Expression.Platform.Silverlight.dll") },
				{ "ActiproSoftware.Shared.Net20, Version=1.0.102.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", null },
				{ "ActiproSoftware.SyntaxEditor.Addons.DotNet.Net20, Version=4.0.283.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", null },
				{ "ActiproSoftware.SyntaxEditor.Net20, Version=4.0.283.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", null },
				{ "ActiproSoftware.WinUICore.Net20, Version=1.0.102.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", null },
				{ "Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", PathHelper.ResolveCombinedPath(localPath, "Microsoft.Windows.Design.Interaction.dll") },
				{ "Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", PathHelper.ResolveCombinedPath(localPath, "Microsoft.Windows.Design.Extensibility.dll") }
			};
			this.delayLoadedAssemblies = strs;
		}

		private void InitializeBlendLoadedAssemblies()
		{
			lock (this.syncLock)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < (int)assemblies.Length; i++)
				{
					Assembly assembly = assemblies[i];
					try
					{
						if (!this.blendAssemblies.ContainsKey(assembly.FullName) && !assembly.IsDynamic)
						{
							this.blendAssemblies.Add(assembly.FullName, assembly);
						}
					}
					catch (NotSupportedException notSupportedException)
					{
					}
				}
			}
		}

		public Assembly ResolveAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			Assembly assembly1 = null;
			if (!this.hasEntered)
			{
				lock (this.syncLock)
				{
					try
					{
						string str = ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName);
						this.hasEntered = true;
						if (!this.blendAssemblies.TryGetValue(str, out assembly1))
						{
							foreach (KeyValuePair<string, string> delayLoadedAssembly in this.delayLoadedAssemblies)
							{
								if (!string.Equals(str, delayLoadedAssembly.Key, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								assembly1 = (delayLoadedAssembly.Value == null ? ProjectAssemblyHelper.Load(str) : ProjectAssemblyHelper.LoadFrom(delayLoadedAssembly.Value));
								if (assembly1 != null)
								{
									this.blendAssemblies.Add(str, assembly1);
								}
								assembly = assembly1;
								return assembly;
							}
						}
					}
					finally
					{
						this.hasEntered = false;
					}
					return assembly1;
				}
				return assembly;
			}
			return assembly1;
		}
	}
}