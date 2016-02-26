using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectItemNode : HierarchicalNode, INotifyPropertyChanged
	{
		private IProjectItem projectItem;

		private Microsoft.Expression.Project.UserInterface.ProjectNode projectNode;

		private KnownProjectNode knownProjectNode;

		private bool isFilterCached;

		private bool isFiltered;

		private bool isRenaming;

		private bool preparedForRename;

		private string potentialName;

		private static ImageSource missingFileIcon;

		private static List<string> wpfSupportedImages;

		public System.Windows.Input.ICommand ActivateCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnActivateCommand));
			}
		}

		public System.Windows.Input.ICommand AttemptRenameCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnAttemptRenameCommand));
			}
		}

		public virtual ImageSource BitmapImage
		{
			get
			{
				return this.projectItem.Image;
			}
		}

		public bool CanBeRenamed
		{
			get
			{
				if (this.projectItem.IsCodeBehindItem || this.projectItem.DocumentReference.Path.Length > 260 || !this.projectItem.DocumentReference.IsValidPathFormat || this.projectItem.IsVirtual || !this.PrototypingRenameAllowed || this.knownProjectNode == null || this.projectItem is FolderProjectItem && ((FolderProjectItem)this.projectItem).IsUIBlockingFolder)
				{
					return false;
				}
				return this.projectItem.DocumentReference.Path.StartsWith(((IProject)this.knownProjectNode.Project).ProjectRoot.Path);
			}
		}

		public System.Windows.Input.ICommand CancelRename
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCancelRename));
			}
		}

		public System.Windows.Input.ICommand CommitRename
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnCommitRename));
			}
		}

		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(this.projectItem.DocumentReference.EditableDisplayName))
				{
					if (!this.IsDirty)
					{
						return this.projectItem.DocumentReference.DisplayName;
					}
					return string.Concat(this.projectItem.DocumentReference.DisplayName, " *");
				}
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string customItemDisplayNameFormat = StringTable.CustomItemDisplayNameFormat;
				object[] editableDisplayName = new object[] { this.projectItem.DocumentReference.EditableDisplayName, this.projectItem.DocumentReference.DisplayName };
				string str = string.Format(currentCulture, customItemDisplayNameFormat, editableDisplayName);
				if (this.IsDirty)
				{
					str = string.Concat(str, " *");
				}
				return str;
			}
			set
			{
			}
		}

		public bool Exists
		{
			get
			{
				if (this.projectItem is FolderProjectItem)
				{
					return true;
				}
				return this.projectItem.FileExists;
			}
		}

		public string FullPath
		{
			get
			{
				return this.projectItem.DocumentReference.Path;
			}
		}

		public bool IsCut
		{
			get
			{
				return this.projectItem.IsCut;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.projectItem.IsDirty;
			}
		}

		public bool IsOpen
		{
			get
			{
				bool flag;
				using (IEnumerator<IView> enumerator = base.Services.ViewService().Views.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!this.IsViewOfItem(enumerator.Current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
		}

		public bool IsRenaming
		{
			get
			{
				return this.isRenaming;
			}
			private set
			{
				this.isRenaming = value;
				base.OnPropertyChanged("IsRenaming");
			}
		}

		public bool IsStartupItem
		{
			get
			{
				if (this.knownProjectNode == null)
				{
					return false;
				}
				return ((IProject)this.knownProjectNode.Project).StartupItem == this.ProjectItem;
			}
		}

		public override bool IsVisible
		{
			get
			{
				if (base.ProjectPane.SearchTransaction == null)
				{
					return this.projectItem.Visible;
				}
				if (this.isFilterCached)
				{
					return !this.isFiltered;
				}
				base.ProjectPane.SearchTransaction.EnsureNodeTracked(this);
				if ((!this.projectItem.Visible ? false : this.DisplayName.ToUpperInvariant().Contains(base.ProjectPane.SearchTransaction.FilterString.ToUpperInvariant())))
				{
					this.isFiltered = false;
				}
				else
				{
					bool flag = false;
					foreach (ProjectItemNode child in base.Children)
					{
						if (!child.IsVisible)
						{
							continue;
						}
						flag = true;
						break;
					}
					this.isFiltered = !flag;
				}
				this.isFilterCached = true;
				return !this.isFiltered;
			}
		}

		public bool IsWpfSupportedImage
		{
			get
			{
				return ProjectItemNode.IsWpfSupportedImageType(this.FullPath);
			}
		}

		public ImageSource MissingFileIcon
		{
			get
			{
				if (ProjectItemNode.missingFileIcon == null)
				{
					ProjectItemNode.missingFileIcon = Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\missing_file_warning_on_12x12.png");
					ProjectItemNode.missingFileIcon.Freeze();
				}
				return ProjectItemNode.missingFileIcon;
			}
		}

		public string PotentialName
		{
			get
			{
				return this.potentialName;
			}
			set
			{
				this.potentialName = value;
			}
		}

		internal IProjectItem ProjectItem
		{
			get
			{
				return this.projectItem;
			}
		}

		internal Microsoft.Expression.Project.UserInterface.ProjectNode ProjectNode
		{
			get
			{
				return this.projectNode;
			}
		}

		private bool PrototypingRenameAllowed
		{
			get
			{
				IPrototypingProjectService prototypingProjectService = base.Services.PrototypingProjectService();
				if (prototypingProjectService == null)
				{
					return true;
				}
				return prototypingProjectService.CanRename(this.projectItem);
			}
		}

		public System.Windows.Input.ICommand SelectAndPrepareRenameCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnSelectAndPrepareRenameCommand));
			}
		}

		protected override bool ShouldReturnFocus
		{
			get
			{
				if (this.IsRenaming)
				{
					return false;
				}
				return base.ShouldReturnFocus;
			}
		}

		public bool ShowAsLinked
		{
			get
			{
				if (this.projectItem is AssemblyReferenceProjectItem)
				{
					return false;
				}
				return this.projectItem.IsLinkedFile;
			}
		}

		public string ToolTip
		{
			get
			{
				return this.FullPath;
			}
		}

		static ProjectItemNode()
		{
			List<string> strs = new List<string>()
			{
				".bmp",
				".ico",
				".png",
				".jpg",
				".jpeg",
				".gif",
				".tif",
				".tiff"
			};
			ProjectItemNode.wpfSupportedImages = strs;
		}

		protected ProjectItemNode(IProjectItem projectItem, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane, Microsoft.Expression.Project.UserInterface.ProjectNode projectNode) : base(projectItem, projectPane)
		{
			this.projectItem = projectItem;
			base.ProjectPane = projectPane;
			this.projectNode = projectNode;
			this.knownProjectNode = this.projectNode as KnownProjectNode;
			base.Services.ViewService().ViewOpened += new ViewEventHandler(this.ViewService_ViewOpened);
			base.Services.ViewService().ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
			this.projectItem.IsDirtyChanged += new EventHandler(this.ProjectItem_IsDirtyChanged);
			this.projectItem.IsCutChanged += new EventHandler(this.ProjectItem_IsCutChanged);
			this.projectItem.ParentChanged += new EventHandler(this.ProjectItem_ParentChanged);
			this.projectItem.FileInformationChanged += new FileSystemEventHandler(this.ProjectItem_FileInformationChanged);
			ProjectItemNode projectItemNode = this;
			base.IsExpandedChanged += new EventHandler(projectItemNode.ProjectItemNode_IsExpandedChanged);
			if (this.knownProjectNode != null)
			{
				((IProject)this.knownProjectNode.Project).StartupItemChanged += new EventHandler<ProjectItemChangedEventArgs>(this.Project_StartupSceneChanged);
			}
		}

		internal void BeginRename()
		{
			this.potentialName = this.ProjectItem.DocumentReference.DisplayName;
			this.IsRenaming = true;
			base.OnPropertyChanged("PotentialName");
		}

		private string CleanFileName(string newName)
		{
			if (!this.projectItem.IsDirectory)
			{
				string safeExtension = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(this.projectItem.DocumentReference.DisplayName);
				if (safeExtension != Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(newName))
				{
					newName = Path.ChangeExtension(newName, safeExtension);
				}
			}
			else
			{
				newName = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(newName);
			}
			return newName;
		}

		public override int CompareTo(HierarchicalNode treeItem)
		{
			ProjectItemNode projectItemNode = treeItem as ProjectItemNode;
			if (projectItemNode == null)
			{
				return -1;
			}
			if (this.ProjectItem is FolderStandIn)
			{
				return -1;
			}
			if (projectItemNode.ProjectItem is FolderStandIn)
			{
				return 1;
			}
			IDocumentType item = base.Services.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.Folder];
			if (this.ProjectItem.DocumentType == item)
			{
				if (projectItemNode.ProjectItem.DocumentType != item)
				{
					return -1;
				}
				return AlphabeticThenNumericComparer.Compare(this.DisplayName, projectItemNode.DisplayName, CultureInfo.CurrentCulture);
			}
			if (projectItemNode.ProjectItem.DocumentType == item)
			{
				return 1;
			}
			try
			{
				if (Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(this.FullPath) == Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(projectItemNode.FullPath))
				{
					int num = AlphabeticThenNumericComparer.Compare(Path.GetFileNameWithoutExtension(this.FullPath), Path.GetFileNameWithoutExtension(projectItemNode.FullPath), CultureInfo.CurrentCulture);
					return num;
				}
			}
			catch (ArgumentException argumentException)
			{
			}
			return ProjectPathHelper.CompareForDisplay(this.DisplayName, projectItemNode.DisplayName, CultureInfo.CurrentCulture);
		}

		internal static ProjectItemNode Create(IProjectItem projectItem, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane, Microsoft.Expression.Project.UserInterface.ProjectNode projectNode)
		{
			ProjectItemNode projectItemNode = null;
			FolderProjectItem folderProjectItem = projectItem as FolderProjectItem;
			if (folderProjectItem == null)
			{
				projectItemNode = new ProjectItemNode(projectItem, projectPane, projectNode);
			}
			else
			{
				projectItemNode = new FolderNode(folderProjectItem, projectPane, projectNode);
			}
			return projectItemNode;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (base.ProjectPane != null)
				{
					base.Services.ViewService().ViewOpened -= new ViewEventHandler(this.ViewService_ViewOpened);
					base.Services.ViewService().ViewClosed -= new ViewEventHandler(this.ViewService_ViewClosed);
					base.ProjectPane = null;
				}
				if (this.projectItem != null)
				{
					this.projectItem.IsDirtyChanged -= new EventHandler(this.ProjectItem_IsDirtyChanged);
					this.projectItem.IsCutChanged -= new EventHandler(this.ProjectItem_IsCutChanged);
					this.projectItem.ParentChanged -= new EventHandler(this.ProjectItem_ParentChanged);
					this.projectItem.FileInformationChanged -= new FileSystemEventHandler(this.ProjectItem_FileInformationChanged);
					this.projectItem = null;
				}
				if (this.knownProjectNode != null)
				{
					((IProject)this.knownProjectNode.Project).StartupItemChanged -= new EventHandler<ProjectItemChangedEventArgs>(this.Project_StartupSceneChanged);
				}
				this.projectNode = null;
				this.knownProjectNode = null;
				ProjectItemNode projectItemNode = this;
				base.IsExpandedChanged -= new EventHandler(projectItemNode.ProjectItemNode_IsExpandedChanged);
				base.Dispose(disposing);
			}
		}

		protected override void ExpandAllParents()
		{
			for (HierarchicalNode i = base.Parent; i != null && i is ProjectItemNode; i = i.Parent)
			{
				i.IsExpanded = true;
			}
		}

		public bool IsViewOfItem(IView view)
		{
			IDocumentView documentView = view as IDocumentView;
			if (documentView == null || documentView.Document == null)
			{
				return false;
			}
			return documentView.Document.DocumentReference == this.projectItem.DocumentReference;
		}

		public static bool IsWpfSupportedImageType(string fullPath)
		{
			return ProjectItemNode.wpfSupportedImages.Contains<string>(Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(fullPath), StringComparer.OrdinalIgnoreCase);
		}

		private void OnActivateCommand()
		{
			PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ActivateProjectItem);
			INamedProject namedProject = base.Services.ProjectManager().CurrentSolution.FindProjectContainingOpenItem(this.projectItem.DocumentReference);
			if (namedProject != null && namedProject != this.ProjectItem.Project)
			{
				base.Services.MessageDisplayService().ShowError(StringTable.ProjectItemAlreadyOpenMessage);
				return;
			}
			IView activeView = base.Services.ViewService().ActiveView;
			if (!this.projectItem.DocumentType.CanView)
			{
				if (this.projectItem is FolderStandIn || this.projectItem.DocumentType == base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder])
				{
					this.IsExpanded = !this.IsExpanded;
					return;
				}
				if (activeView != null && this.projectItem.DocumentType.CanInsertTo(this.projectItem, activeView))
				{
					this.projectItem.DocumentType.AddToDocument(this.projectItem, activeView);
					return;
				}
				if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(this.projectItem.DocumentReference.Path))
				{
					CommandTarget commandTarget = base.Services.ProjectManager() as CommandTarget;
					if (commandTarget != null)
					{
						string str = "Project_EditExternally";
						commandTarget.SetCommandProperty(str, "TargetDocument", this.ProjectItem);
						commandTarget.Execute(str, CommandInvocationSource.Palette);
					}
				}
			}
			else
			{
				using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
				{
					this.projectItem.OpenView(true);
				}
			}
		}

		private void OnAttemptRenameCommand()
		{
			if (this.preparedForRename)
			{
				if (base.Services.ProjectManager().ItemSelectionSet.Count != 1)
				{
					base.Services.ProjectManager().ItemSelectionSet.SetSelection(this.ProjectItem);
					base.OnSelectionChanged();
				}
				else if (this.CanBeRenamed)
				{
					this.BeginRename();
				}
				this.preparedForRename = false;
			}
		}

		private void OnCancelRename()
		{
			this.IsRenaming = false;
		}

		private void OnCommitRename()
		{
			if (this.isRenaming && this.projectItem != null)
			{
				this.IsRenaming = false;
				try
				{
					string str = this.potentialName.Trim();
					this.ValidatePotentialFileName(str);
					str = this.CleanFileName(str);
					str = Path.Combine(Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(this.projectItem.DocumentReference.Path), str);
					if (this.projectItem.IsDirectory)
					{
						str = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(str);
					}
					if (!this.projectItem.Project.RenameProjectItem(this.projectItem, DocumentReference.Create(str)))
					{
						this.BeginRename();
					}
				}
				catch (ProjectItemRenameException projectItemRenameException)
				{
					base.Services.MessageDisplayService().ShowError(projectItemRenameException.Message);
					this.BeginRename();
				}
			}
		}

		protected override void OnCreateContextMenuCommand()
		{
			this.ContextMenu = (System.Windows.Controls.ContextMenu)this.ProjectItem.GetContextMenu(base.Services.CommandBarService().CommandBars);
		}

		public override void OnDragBegin(DragBeginEventArgs e)
		{
			if (Mouse.Captured == null && !this.IsRenaming)
			{
				DataObject dataObject = new DataObject();
				dataObject.SetData("BlendProjectItem", base.Services.ProjectManager().ItemSelectionSet, true);
				try
				{
					DragDrop.DoDragDrop(e.DragSource, dataObject, DragDropEffects.Move);
				}
				catch (COMException cOMException)
				{
				}
			}
		}

		internal void OnRenamed()
		{
			if (base.Parent != null)
			{
				HierarchicalNode parent = base.Parent;
				parent.RemoveChild(this);
				parent.AddChild(this);
			}
			base.OnPropertyChanged("DisplayName");
			base.OnPropertyChanged("FullPath");
			base.OnPropertyChanged("ToolTip");
		}

		private void OnSelectAndPrepareRenameCommand()
		{
			this.preparedForRename = base.Services.ProjectManager().ItemSelectionSet.Selection.Contains<IDocumentItem>(this.ProjectItem);
			if (!this.preparedForRename)
			{
				base.Services.ProjectManager().ItemSelectionSet.SetSelection(this.ProjectItem);
				base.OnSelectionChanged();
			}
		}

		private void Project_StartupSceneChanged(object sender, ProjectItemChangedEventArgs e)
		{
			if (e.OldProjectItem == this.projectItem || e.NewProjectItem == this.projectItem)
			{
				base.OnPropertyChanged("IsStartupItem");
			}
		}

		private void ProjectItem_FileInformationChanged(object sender, FileSystemEventArgs e)
		{
			if ((e.ChangeType & WatcherChangeTypes.Created) == WatcherChangeTypes.Created || (e.ChangeType & WatcherChangeTypes.Deleted) == WatcherChangeTypes.Deleted)
			{
				base.OnPropertyChanged("Exists");
			}
		}

		private void ProjectItem_IsCutChanged(object sender, EventArgs e)
		{
			base.OnPropertyChanged("IsCut");
		}

		private void ProjectItem_IsDirtyChanged(object sender, EventArgs e)
		{
			base.OnPropertyChanged("IsDirty");
			base.OnPropertyChanged("DisplayName");
		}

		private void ProjectItem_ParentChanged(object sender, EventArgs e)
		{
			IProjectItem parent = this.ProjectItem.Parent;
			if (parent == null)
			{
				base.Parent.RemoveChild(this);
				this.projectNode.AddChild(this);
				return;
			}
			HierarchicalNode hierarchicalNode = this.projectNode.FindItemNode(parent);
			base.Parent.RemoveChild(this);
			hierarchicalNode.AddChild(this);
		}

		protected virtual void ProjectItemNode_IsExpandedChanged(object sender, EventArgs e)
		{
			if (base.ProjectPane.SearchTransaction != null)
			{
				HierarchicalNode hierarchicalNode = sender as HierarchicalNode;
				if (hierarchicalNode.IsExpanded)
				{
					base.ProjectPane.SearchTransaction.ExpandNode(hierarchicalNode);
					return;
				}
				base.ProjectPane.SearchTransaction.CollapseNode(hierarchicalNode);
			}
		}

		public void ResetVisibilityFilter()
		{
			this.isFilterCached = false;
		}

		public void ResetVisibilityFilter(bool isNewFilterASubstring, bool isOldFilterASubstring)
		{
			if (this.isFilterCached)
			{
				if (isNewFilterASubstring && this.isFiltered)
				{
					this.isFilterCached = false;
					return;
				}
				if (isOldFilterASubstring && !this.isFiltered)
				{
					this.isFilterCached = false;
					return;
				}
				if (!isOldFilterASubstring && !isNewFilterASubstring)
				{
					this.isFilterCached = false;
				}
			}
		}

		protected override void SetSelection(bool value)
		{
			if (base.ProjectPane != null && base.ProjectPane.SearchTransaction != null && value)
			{
				base.ProjectPane.SearchTransaction.ExpressInterestInNode(this);
			}
			base.SetSelection(value);
		}

		public override string ToString()
		{
			return this.ProjectItem.ToString();
		}

		internal override void UpdateSelection()
		{
			base.OnSelectionChanged();
			foreach (ProjectItemNode child in base.Children)
			{
				child.UpdateSelection();
			}
		}

		private void ValidatePotentialFileName(string potentialFileName)
		{
			if (string.IsNullOrEmpty(potentialFileName))
			{
				throw new ProjectItemRenameException(ProjectItemRenameError.EmptyString, this.projectItem, potentialFileName);
			}
			if (potentialFileName[0] == '.')
			{
				throw new ProjectItemRenameException(ProjectItemRenameError.StartsWithPeriod, this.projectItem, potentialFileName);
			}
			if (potentialFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				throw new ProjectItemRenameException(ProjectItemRenameError.ContainsInvalidCharacters, this.projectItem, potentialFileName);
			}
			string str = Path.GetFileNameWithoutExtension(potentialFileName).Trim();
			char[] chrArray = new char[] { '.' };
			string upper = str.Trim(chrArray).ToUpper(CultureInfo.CurrentCulture);
			if (ProjectDialog.ReservedNames.Contains<string>(upper))
			{
				throw new ProjectItemRenameException(ProjectItemRenameError.IsReservedName, this.projectItem, potentialFileName);
			}
			if (potentialFileName.Length > 260)
			{
				throw new ProjectItemRenameException(new PathTooLongException(), ProjectItemRenameError.Exception, this.projectItem, potentialFileName);
			}
		}

		private void ViewService_ViewClosed(object sender, ViewEventArgs e)
		{
			if (this.IsViewOfItem(e.View))
			{
				base.OnPropertyChanged("IsOpen");
			}
		}

		private void ViewService_ViewOpened(object sender, ViewEventArgs e)
		{
			if (this.IsViewOfItem(e.View))
			{
				base.OnPropertyChanged("IsOpen");
			}
		}

		public void VisibilityChanged()
		{
			base.OnPropertyChanged("IsVisible");
		}
	}
}