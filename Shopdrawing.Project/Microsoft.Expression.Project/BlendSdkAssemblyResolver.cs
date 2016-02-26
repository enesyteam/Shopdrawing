using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal sealed class BlendSdkAssemblyResolver : IAssemblyResolver
	{
		private HashSet<string> librarySignatures;

		private List<string> probingPaths = new List<string>();

		private Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

		public BlendSdkAssemblyResolver()
		{
			HashSet<string> strs = new HashSet<string>();
			strs.Add("System.Windows.Interactivity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Interactions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Interactions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Controls, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Effects, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("System.Windows.Interactivity, Version=2.0.5.0, Culture=neutral, PublicKeyToken=null");
			strs.Add("System.Windows.Interactivity, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Interactions, Version=2.0.5.0, Culture=neutral, PublicKeyToken=null");
			strs.Add("Microsoft.Expression.Interactions, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("System.Windows.Interactivity, Version=4.0.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("System.Windows.Interactivity, Version=4.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Interactions, Version=4.0.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("Microsoft.Expression.Interactions, Version=4.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Drawing, Version=4.0.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("Microsoft.Expression.Drawing, Version=4.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Controls, Version=4.0.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("Microsoft.Expression.Controls, Version=4.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Effects, Version=4.0.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("Microsoft.Expression.Effects, Version=4.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("System.Windows.Interactivity, Version=3.7.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("System.Windows.Interactivity, Version=3.7.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			strs.Add("Microsoft.Expression.Interactions, Version=3.7.5.0, Culture=neutral, PublicKeyToken=1a2c496f4b3bbc64");
			strs.Add("Microsoft.Expression.Interactions, Version=3.7.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			this.librarySignatures = strs;
			this.PushPathIfExists(BlendSdkHelper.GetInteractivityPath(BlendSdkHelper.Silverlight3));
			this.PushPathIfExists(BlendSdkHelper.GetInteractivityPath(BlendSdkHelper.Wpf35));
			this.PushPathIfExists(BlendSdkHelper.GetInteractivityPath(BlendSdkHelper.Silverlight4));
			this.PushPathIfExists(BlendSdkHelper.GetInteractivityPath(BlendSdkHelper.Wpf4));
			this.PushPathIfExists(BlendSdkHelper.GetInteractivityPath(BlendSdkHelper.WindowsPhone7));
		}

		private Assembly LoadLibraryAssembly(AssemblyName assemblyName)
		{
			Assembly assembly;
			if (assemblyName == null)
			{
				return null;
			}
			string str = ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName);
			if (this.loadedAssemblies.TryGetValue(str, out assembly))
			{
				return assembly;
			}
			if (this.librarySignatures.Contains(str))
			{
				foreach (string probingPath in this.probingPaths)
				{
					string str1 = string.Concat(PathHelper.ResolveCombinedPath(probingPath, assemblyName.Name), ".dll");
					AssemblyName assemblyName1 = ProjectAssemblyHelper.CachedGetAssemblyNameFromPath(str1);
					if (assemblyName1 == null || !ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName1).Equals(str, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					assembly = ProjectAssemblyHelper.LoadFrom(str1);
					if (assembly == null)
					{
						continue;
					}
					this.loadedAssemblies.Add(str, assembly);
					break;
				}
			}
			return assembly;
		}

		private void PushPathIfExists(string path)
		{
			if (!string.IsNullOrEmpty(path) && PathHelper.DirectoryExists(path))
			{
				this.probingPaths.Add(path);
			}
		}

		public Assembly ResolveAssembly(AssemblyName assemblyName)
		{
			return this.LoadLibraryAssembly(assemblyName);
		}
	}
}