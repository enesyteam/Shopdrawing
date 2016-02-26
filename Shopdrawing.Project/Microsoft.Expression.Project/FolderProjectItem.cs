using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	internal class FolderProjectItem : ProjectItemBase
	{
		private static ImageSource closedFolder;

		private static ImageSource openFolder;

		private ImageSource overrideImage;

		private static ICommandBar contextMenu;

		protected override string BuildItemName
		{
			get
			{
				return "Folder";
			}
			set
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string settingBuildItemNameOnWrongProjectItemType = ExceptionStringTable.SettingBuildItemNameOnWrongProjectItemType;
				object[] name = new object[] { base.GetType().Name };
				throw new InvalidOperationException(string.Format(currentCulture, settingBuildItemNameOnWrongProjectItemType, name));
			}
		}

		public override bool CanAddChildren
		{
			get
			{
				return !this.IsUIBlockingFolder;
			}
		}

		protected virtual ImageSource ClosedFolder
		{
			get
			{
				if (FolderProjectItem.closedFolder == null)
				{
					FolderProjectItem.closedFolder = FileTable.GetImageSource("Resources\\folder_closed_16x16.png");
					FolderProjectItem.closedFolder.Freeze();
				}
				return FolderProjectItem.closedFolder;
			}
		}

		public override IDocument Document
		{
			get
			{
				return null;
			}
		}

		public override bool FileExists
		{
			get
			{
				return PathHelper.DirectoryExists(base.DocumentReference.Path);
			}
		}

		public override ImageSource Image
		{
			get
			{
				if (this.overrideImage == null)
				{
					return this.ClosedFolder;
				}
				return this.overrideImage;
			}
		}

		public override bool IsDirectory
		{
			get
			{
				return true;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		public override bool IsOpen
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal bool IsUIBlockingFolder
		{
			get;
			private set;
		}

		protected virtual ImageSource OpenedFolder
		{
			get
			{
				if (FolderProjectItem.openFolder == null)
				{
					FolderProjectItem.openFolder = FileTable.GetImageSource("Resources\\folder_open_16x16.png");
					FolderProjectItem.openFolder.Freeze();
				}
				return FolderProjectItem.openFolder;
			}
		}

		public virtual ImageSource OpenImage
		{
			get
			{
				if (this.overrideImage == null)
				{
					return this.OpenedFolder;
				}
				return this.overrideImage;
			}
		}

		public FolderProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : this(project, documentReference, documentType, serviceProvider, false, null)
		{
		}

		public FolderProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider, bool isBlockingFolder) : this(project, documentReference, documentType, serviceProvider, isBlockingFolder, null)
		{
		}

		public FolderProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider, bool isBlockingFolder, ImageSource image) : base(project, documentReference, documentType, serviceProvider)
		{
			this.IsUIBlockingFolder = isBlockingFolder;
			this.overrideImage = image;
		}

		private static void DestroyContextMenu()
		{
			FolderProjectItem.contextMenu = null;
		}

		public override ICommandBar GetContextMenu(ICommandBarCollection commandBarCollection)
		{
			if (this.IsUIBlockingFolder)
			{
				return null;
			}
			if (base.Services.ProjectManager().CurrentSolution != null && base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
			{
				FolderProjectItem.DestroyContextMenu();
			}
			if (FolderProjectItem.contextMenu == null)
			{
				FolderProjectItem.contextMenu = commandBarCollection.AddContextMenu("Project_FolderProjectItemContextMenu");
				FolderProjectItem.contextMenu.Items.AddButton("Application_AddNewItem", StringTable.ProjectItemContextMenuAddNewItem);
				FolderProjectItem.contextMenu.Items.AddButton("Project_AddExistingItem", StringTable.ProjectItemContextMenuAddExistingItem);
				FolderProjectItem.contextMenu.Items.AddButton("Project_LinkToExistingItem", StringTable.ProjectItemContextMenuLinkToExistingItem);
				FolderProjectItem.contextMenu.Items.AddSeparator();
				ProjectManager.AddSourceControlMenuItems(FolderProjectItem.contextMenu.Items);
				FolderProjectItem.contextMenu.Items.AddSeparator();
				FolderProjectItem.contextMenu.Items.AddButton("Project_NewFolder", StringTable.ProjectItemContextMenuNewFolder);
				FolderProjectItem.contextMenu.Items.AddButton("Project_Refresh", StringTable.ProjectItemContextMenuRefresh);
				FolderProjectItem.contextMenu.Items.AddSeparator();
				FolderProjectItem.contextMenu.Items.AddButton("Project_Cut", StringTable.ProjectItemContextMenuCut);
				FolderProjectItem.contextMenu.Items.AddButton("Project_Copy", StringTable.ProjectItemContextMenuCopy);
				FolderProjectItem.contextMenu.Items.AddButton("Project_Paste", StringTable.ProjectItemContextMenuPaste);
				FolderProjectItem.contextMenu.Items.AddButton("Project_RenameProjectItem", StringTable.ProjectItemContextMenuRename);
				FolderProjectItem.contextMenu.Items.AddSeparator();
				FolderProjectItem.contextMenu.Items.AddButton("Project_DeleteProjectItem", StringTable.ProjectItemContextMenuDelete);
				FolderProjectItem.contextMenu.Items.AddSeparator();
				FolderProjectItem.contextMenu.Items.AddButton("Project_ExploreProject", StringTable.ProjectItemContextMenuExplore);
			}
			return FolderProjectItem.contextMenu;
		}
	}
}