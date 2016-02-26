using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;

namespace Microsoft.Expression.Extensibility.Project
{
	internal class ProjectItem : Microsoft.Expression.Extensibility.Project.IProjectItem
	{
		private Microsoft.Expression.Project.IProjectItem projectItem;

		private Microsoft.Expression.Extensibility.Project.IProject project;

		public string FullPath
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				return this.projectItem.DocumentReference.Path;
			}
		}

		public bool IsValid
		{
			get
			{
				return ((Microsoft.Expression.Extensibility.Project.Project)this.project).IsValid;
			}
		}

		public string Name
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				return this.projectItem.DocumentReference.DisplayName;
			}
		}

		internal ProjectItem(Microsoft.Expression.Extensibility.Project.IProject project, Microsoft.Expression.Project.IProjectItem projectItem)
		{
			this.projectItem = projectItem;
			this.project = project;
		}

		internal static Microsoft.Expression.Extensibility.Project.IProjectItem FromProjectItem(Microsoft.Expression.Extensibility.Project.IProject project, Microsoft.Expression.Project.IProjectItem projectItem)
		{
			return new Microsoft.Expression.Extensibility.Project.ProjectItem(project, projectItem);
		}
	}
}