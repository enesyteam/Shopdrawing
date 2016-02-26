using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Extensibility.Project
{
	internal class Project : Microsoft.Expression.Extensibility.Project.IProject
	{
		private Microsoft.Expression.Project.IProject project;

		internal Microsoft.Expression.Project.IProject InternalProject
		{
			get
			{
				return this.project;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.project != null;
			}
		}

		public IEnumerable<Microsoft.Expression.Extensibility.Project.IProjectItem> Items
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				return 
					from item in this.project.Items
					select Microsoft.Expression.Extensibility.Project.ProjectItem.FromProjectItem(this, item);
			}
		}

		public string Path
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}
				return this.project.DocumentReference.Path;
			}
		}

		public Microsoft.Expression.Extensibility.Project.IProjectStore ProjectStore
		{
			get
			{
				return Microsoft.Expression.Extensibility.Project.ProjectStore.FromProjectStore(this.project.ProjectStore);
			}
		}

		public FrameworkName TargetFramework
		{
			get
			{
				return this.project.TargetFramework;
			}
		}

		internal Project(Microsoft.Expression.Project.IProject project)
		{
			this.project = project;
		}

		public Microsoft.Expression.Extensibility.Project.IProjectItem AddItem(string documentItemPath, string itemType)
		{
			if (!this.IsValid)
			{
				return null;
			}
			Microsoft.Expression.Project.IProject project = this.project;
			DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
			{
				SourcePath = documentItemPath,
				CreationOptions = CreationOptions.None,
				BuildTaskInfo = new BuildTaskInfo(itemType, null)
			};
			return Microsoft.Expression.Extensibility.Project.ProjectItem.FromProjectItem(this, project.AddItem(documentCreationInfo));
		}

		public void AddReference(string reference)
		{
			if (!this.IsValid)
			{
				return;
			}
			this.project.AddAssemblyReference(reference, false);
		}

		internal void Close()
		{
			this.project = null;
		}

		internal static Microsoft.Expression.Extensibility.Project.IProject FromProject(Microsoft.Expression.Project.IProject project)
		{
			return new Microsoft.Expression.Extensibility.Project.Project(project);
		}
	}
}