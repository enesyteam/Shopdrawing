using System;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectItemChangedEventArgs : EventArgs
	{
		private IProjectItem oldProjectItem;

		private IProjectItem newProjectItem;

		public IProjectItem NewProjectItem
		{
			get
			{
				return this.newProjectItem;
			}
		}

		public IProjectItem OldProjectItem
		{
			get
			{
				return this.oldProjectItem;
			}
		}

		public ProjectItemChangedEventArgs(IProjectItem oldProjectItem, IProjectItem newProjectItem)
		{
			this.oldProjectItem = oldProjectItem;
			this.newProjectItem = newProjectItem;
		}
	}
}