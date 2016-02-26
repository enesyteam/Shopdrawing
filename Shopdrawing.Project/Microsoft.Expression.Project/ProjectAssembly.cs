using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectAssembly
	{
		public string FullName
		{
			get;
			private set;
		}

		public bool IsImplicitlyResolved
		{
			get;
			private set;
		}

		public bool IsPlatformAssembly
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string Path
		{
			get;
			private set;
		}

		public IProjectItem ProjectItem
		{
			get;
			private set;
		}

		public Assembly ReferenceAssembly
		{
			get;
			internal set;
		}

		public Assembly RuntimeAssembly
		{
			get;
			private set;
		}

		internal ProjectAssembly(IProjectItem projectItem, string name)
		{
			this.Name = name;
			this.ProjectItem = projectItem;
			this.IsImplicitlyResolved = false;
		}

		internal ProjectAssembly(IProjectItem projectItem, Assembly runtimeAssembly, string path) : this(projectItem, runtimeAssembly, path, false)
		{
		}

		internal ProjectAssembly(IProjectItem projectItem, Assembly runtimeAssembly, string path, bool isImplicitlyResolvedAssembly)
		{
			this.Path = path;
			this.RuntimeAssembly = runtimeAssembly;
			AssemblyName assemblyName = ProjectAssemblyHelper.GetAssemblyName(this.RuntimeAssembly);
			this.Name = assemblyName.Name;
			this.FullName = ProjectAssemblyHelper.CachedGetAssemblyFullName(assemblyName);
			this.IsImplicitlyResolved = isImplicitlyResolvedAssembly;
			this.ProjectItem = projectItem;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			ProjectAssembly projectAssembly = obj as ProjectAssembly;
			if (projectAssembly == null)
			{
				return false;
			}
			return projectAssembly.FullName == this.FullName;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}