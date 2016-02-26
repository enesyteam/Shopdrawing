using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	internal class FolderStandIn : FolderProjectItem
	{
		private static ImageSource closedFolder;

		private static ImageSource openFolder;

		private static ICommandBar contextMenu;

		public override bool CanAddChildren
		{
			get
			{
				return false;
			}
		}

		protected override ImageSource ClosedFolder
		{
			get
			{
				if (FolderStandIn.closedFolder == null)
				{
					FolderStandIn.closedFolder = FileTable.GetImageSource("Resources\\referenceFolder_closed_16x16.png");
					FolderStandIn.closedFolder.Freeze();
				}
				return FolderStandIn.closedFolder;
			}
		}

		public override bool IsVirtual
		{
			get
			{
				return true;
			}
		}

		protected override ImageSource OpenedFolder
		{
			get
			{
				if (FolderStandIn.openFolder == null)
				{
					FolderStandIn.openFolder = FileTable.GetImageSource("Resources\\referenceFolder_open_16x16.png");
					FolderStandIn.openFolder.Freeze();
				}
				return FolderStandIn.openFolder;
			}
		}

		public FolderStandIn(IProject project, string name, IServiceProvider serviceProvider) : base(project, Microsoft.Expression.Framework.Documents.DocumentReference.Create(Path.Combine(project.ProjectRoot.Path, name)), new FolderDocumentType(), serviceProvider)
		{
		}

		public override ICommandBar GetContextMenu(ICommandBarCollection commandBarCollection)
		{
			if (FolderStandIn.contextMenu == null)
			{
				FolderStandIn.contextMenu = commandBarCollection.AddContextMenu("Project_FolderStandinContextMenu");
				FolderStandIn.contextMenu.Items.AddButton("Project_AddReference", StringTable.ProjectItemContextMenuAddReference);
				FolderStandIn.contextMenu.Items.AddDynamicMenu("Project_AddProjectReference", StringTable.ProjectItemContextMenuAddProjectReference);
			}
			return FolderStandIn.contextMenu;
		}
	}
}