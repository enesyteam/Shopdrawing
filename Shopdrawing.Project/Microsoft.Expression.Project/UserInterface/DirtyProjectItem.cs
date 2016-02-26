using System;

namespace Microsoft.Expression.Project.UserInterface
{
	public class DirtyProjectItem : ItemModel
	{
		private string displayName;

		private string owningProject;

		private bool isProject;

		public override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public override bool IsHeaderItem
		{
			get
			{
				return this.isProject;
			}
		}

		public string OwningProject
		{
			get
			{
				return this.owningProject;
			}
		}

		internal DirtyProjectItem(string displayName, string owningProject, bool isProject)
		{
			this.isProject = isProject;
			this.displayName = displayName;
			this.owningProject = owningProject;
		}
	}
}