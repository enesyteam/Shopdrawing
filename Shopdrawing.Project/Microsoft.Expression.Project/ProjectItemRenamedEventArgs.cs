using Microsoft.Expression.Framework.Documents;
using System;

namespace Microsoft.Expression.Project
{
	public sealed class ProjectItemRenamedEventArgs : EventArgs
	{
		private IProjectItem projectItem;

		private DocumentReference oldName;

		public DocumentReference OldName
		{
			get
			{
				return this.oldName;
			}
		}

		public IProjectItem ProjectItem
		{
			get
			{
				return this.projectItem;
			}
		}

		public ProjectItemRenamedEventArgs(IProjectItem projectItem, DocumentReference oldName)
		{
			this.projectItem = projectItem;
			this.oldName = oldName;
		}
	}
}