using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Project
{
	internal class ProjectWatcher : FileWatcherBase
	{
		private KnownProjectBase project;

		public ProjectWatcher(KnownProjectBase project)
		{
			this.project = project;
			base.CreateFileWatcher(project.ProjectRoot.Path);
		}

		protected override void CheckForChangedOrDeletedItems()
		{
			this.project.CheckForChangedOrDeletedItems();
		}

		public override void UpdateFileInformation()
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectWatcherUpdateFileInformation);
			foreach (IProjectItem item in ((IProject)this.project).Items)
			{
				ProjectItem projectItem = item as ProjectItem;
				if (projectItem == null)
				{
					continue;
				}
				projectItem.UpdateFileInformation();
			}
			if (this.project != null && this.project.DocumentReference != null)
			{
				this.project.ProjectFileInformation = new ProjectFileInformation(this.project.DocumentReference.Path);
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectWatcherUpdateFileInformation);
		}
	}
}