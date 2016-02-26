using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Extensibility.Project
{
	internal class ProjectStore : Microsoft.Expression.Extensibility.Project.IProjectStore
	{
		private Microsoft.Expression.Project.IProjectStore projectStore;

		internal Microsoft.Expression.Project.IProjectStore InternalProjectStore
		{
			get
			{
				return this.projectStore;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.projectStore != null;
			}
		}

		public IEnumerable<string> ProjectImports
		{
			get
			{
				return this.projectStore.ProjectImports;
			}
		}

		internal ProjectStore(Microsoft.Expression.Project.IProjectStore projectStore)
		{
			this.projectStore = projectStore;
		}

		public bool AddImport(string value)
		{
			return this.projectStore.AddImport(value);
		}

		internal void Close()
		{
			this.projectStore = null;
		}

		internal static Microsoft.Expression.Extensibility.Project.IProjectStore FromProjectStore(Microsoft.Expression.Project.IProjectStore projectStore)
		{
			return new ProjectStore(projectStore);
		}
	}
}