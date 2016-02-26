using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectItemEventArgs : EventArgs
	{
		public ProjectItemEventOptions Options
		{
			get;
			private set;
		}

		public IProjectItem ProjectItem
		{
			get;
			private set;
		}

		public ProjectItemEventArgs(IProjectItem projectItem) : this(projectItem, ProjectItemEventOptions.None)
		{
		}

		public ProjectItemEventArgs(IProjectItem projectItem, ProjectItemEventOptions options)
		{
			this.ProjectItem = projectItem;
			this.Options = options;
		}
	}
}