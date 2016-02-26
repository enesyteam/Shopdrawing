using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Project;
using System;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class FolderNode : ProjectItemNode
	{
		private FolderProjectItem folderProjectItem;

		public override ImageSource BitmapImage
		{
			get
			{
				if (this.IsExpanded)
				{
					return this.folderProjectItem.OpenImage;
				}
				return this.folderProjectItem.Image;
			}
		}

		internal FolderNode(FolderProjectItem projectItem, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane, Microsoft.Expression.Project.UserInterface.ProjectNode projectNode) : base(projectItem, projectPane, projectNode)
		{
			this.folderProjectItem = projectItem;
		}

		protected override void ProjectItemNode_IsExpandedChanged(object sender, EventArgs e)
		{
			base.OnPropertyChanged("BitmapImage");
			base.ProjectItemNode_IsExpandedChanged(sender, e);
		}
	}
}