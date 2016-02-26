using Microsoft.Expression.Framework.Documents;
using System;
using System.IO;

namespace Microsoft.Expression.Project
{
	internal class SolutionWatcher : FileWatcherBase
	{
		private SolutionBase solution;

		public SolutionWatcher(SolutionBase solution)
		{
			this.solution = solution;
			base.CreateFileWatcher(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(solution.DocumentReference.Path));
			base.Watcher.IncludeSubdirectories = false;
		}

		protected override void CheckForChangedOrDeletedItems()
		{
			if (this.solution != null)
			{
				this.solution.CheckForChangedOrDeletedItems();
			}
		}

		public override void UpdateFileInformation()
		{
			if (this.solution != null && this.solution.DocumentReference != null)
			{
				this.solution.SolutionFileInformation = new ProjectFileInformation(this.solution.DocumentReference.Path);
			}
		}
	}
}