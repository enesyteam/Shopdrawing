using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media;

namespace Microsoft.Expression.Project
{
	internal abstract class ProjectItemBase : DocumentItemBase, IProjectItem, IDocumentItem, IDisposable, IMSBuildItem, IMSBuildItemInternal
	{
		private IProject project;

		private IDocumentType documentType;

		private IServiceProvider serviceProvider;

		private DelegatingPropertiesCollection propertyCollectionStore;

		private bool? isLinked = null;

		private bool isCut;

		private List<IProjectItem> children = new List<IProjectItem>();

		private IProjectItem parent;

		private static ICommandBar contextMenu;

		private Microsoft.Build.Evaluation.ProjectItem BuildItem
		{
			get;
			set;
		}

		protected virtual string BuildItemName
		{
			get
			{
				if (this.BuildItem != null)
				{
					return this.BuildItem.ItemType;
				}
				if (this.DocumentType.DefaultBuildTaskInfo == null)
				{
					return string.Empty;
				}
				return this.DocumentType.DefaultBuildTaskInfo.BuildTask;
			}
			set
			{
				if (this.BuildItem != null && this.BuildItem.ItemType != value)
				{
					this.BuildItem.ItemType = value;
					this.OnBuildItemChanged();
				}
			}
		}

		public virtual bool CanAddChildren
		{
			get
			{
				return false;
			}
		}

		public virtual IEnumerable<IProjectItem> Children
		{
			get
			{
				return this.children;
			}
		}

		public virtual IProjectItem CodeBehindItem
		{
			get
			{
				return null;
			}
		}

		public virtual bool ContainsDesignTimeResources
		{
			get
			{
				if (string.Equals(this.GetBuildItemMetadata("ContainsDesignTimeResources"), "true", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				return false;
			}
			set
			{
				this.SetMetadata("ContainsDesignTimeResources", (value ? "true" : string.Empty));
				this.SetCondition((value ? "'$(DesignTime)'=='true' OR ('$(SolutionPath)'!='' AND Exists('$(SolutionPath)') AND '$(BuildingInsideVisualStudio)'!='true' AND '$(BuildingInsideExpressionBlend)'!='true')" : string.Empty));
			}
		}

		public override IEnumerable<IDocumentItem> Descendants
		{
			get
			{
				IEnumerable<IDocumentItem> documentItems = this.children.Cast<IDocumentItem>().Concat<IDocumentItem>(this.children.SelectMany<IProjectItem, IDocumentItem>((IProjectItem child) => child.Descendants));
				return documentItems;
			}
		}

		public abstract IDocument Document
		{
			get;
		}

		public IDocumentType DocumentType
		{
			get
			{
				return this.documentType;
			}
		}

		public abstract bool FileExists
		{
			get;
		}

		public virtual ImageSource Image
		{
			get
			{
				return this.DocumentType.Image;
			}
		}

		protected virtual string Include
		{
			get
			{
				if (this.BuildItem == null)
				{
					return string.Empty;
				}
				return this.BuildItem.UnevaluatedInclude;
			}
			set
			{
				if (this.BuildItem != null)
				{
					this.BuildItem.Xml.Include = ProjectCollection.Escape(value);
				}
				this.OnBuildItemChanged();
			}
		}

		public bool IsCodeBehindItem
		{
			get
			{
				if (this.Parent == null)
				{
					return false;
				}
				return this.Equals(this.Parent.CodeBehindItem);
			}
		}

		public virtual bool IsCut
		{
			get
			{
				return this.isCut;
			}
			set
			{
				if (value != this.isCut)
				{
					this.isCut = value;
					this.OnIsCutChanged();
				}
			}
		}

		public abstract bool IsDirty
		{
			get;
		}

		public virtual bool IsLinkedFile
		{
			get
			{
				if (!this.isLinked.HasValue)
				{
					bool flag = false;
					if (this.project != null && this.project.ProjectRoot != null)
					{
						string path = this.project.ProjectRoot.Path;
						string str = base.DocumentReference.Path;
						if (!this.IsVirtual && !string.IsNullOrEmpty(str))
						{
							str = str.Trim();
							string str1 = path.Trim();
							char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
							path = str1.TrimEnd(directorySeparatorChar);
							flag = !str.StartsWith(path, StringComparison.OrdinalIgnoreCase);
						}
					}
					this.isLinked = new bool?(flag);
				}
				return this.isLinked.Value;
			}
		}

		public abstract bool IsOpen
		{
			get;
		}

		public abstract bool IsReadOnly
		{
			get;
		}

		string Microsoft.Expression.Project.IMSBuildItem.Include
		{
			get
			{
				return this.Include;
			}
			set
			{
				this.Include = value;
			}
		}

		string Microsoft.Expression.Project.IMSBuildItem.Name
		{
			get
			{
				return this.BuildItemName;
			}
			set
			{
				this.BuildItemName = value;
			}
		}

		Microsoft.Build.Evaluation.ProjectItem Microsoft.Expression.Project.IMSBuildItemInternal.BuildItem
		{
			get
			{
				return this.BuildItem;
			}
			set
			{
				this.BuildItem = value;
			}
		}

		public virtual IProjectItem Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (this.parent != value)
				{
					this.parent = value;
					this.OnParentChanged();
				}
			}
		}

		public IProject Project
		{
			get
			{
				return this.project;
			}
		}

		public virtual string ProjectRelativeDocumentReference
		{
			get
			{
				string displayName = base.DocumentReference.DisplayName;
				for (IProjectItem i = this.Parent; i != null; i = i.Parent)
				{
					if (i is FolderProjectItem)
					{
						displayName = string.Concat(i.DocumentReference.DisplayName, Path.DirectorySeparatorChar, displayName);
					}
				}
				char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
				displayName = displayName.TrimEnd(directorySeparatorChar);
				displayName = string.Concat(Path.DirectorySeparatorChar, displayName);
				return displayName;
			}
		}

		public IPropertiesCollection Properties
		{
			get
			{
				if (this.propertyCollectionStore == null)
				{
					this.propertyCollectionStore = new DelegatingPropertiesCollection(new DelegatingPropertiesCollection.GetProperty(this.GetItemProperty), new DelegatingPropertiesCollection.SetProperty(this.SetItemProperty));
				}
				return this.propertyCollectionStore;
			}
		}

		public IEnumerable<IProjectItem> ReferencingProjectItems
		{
			get
			{
				return this.Project.Items.Where<IProjectItem>(new Func<IProjectItem, bool>(this.ReferencedBy));
			}
		}

		protected override bool SafeToRename
		{
			get
			{
				return this.Project.Items.FindMatchByUrl<IProjectItem>(base.DocumentReference.Path) == null;
			}
		}

		protected internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public virtual bool Visible
		{
			get
			{
				string buildItemMetadata = this.GetBuildItemMetadata("Visible");
				if (!string.IsNullOrEmpty(buildItemMetadata) && buildItemMetadata.Equals("false", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				return true;
			}
		}

		static ProjectItemBase()
		{
		}

		protected ProjectItemBase(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(documentReference)
		{
			this.serviceProvider = serviceProvider;
			this.project = project;
			this.documentType = documentType;
		}

		public virtual void AddChild(IProjectItem child)
		{
			for (IProjectItem i = this.Parent; i != null; i = i.Parent)
			{
				if (i == child)
				{
					return;
				}
			}
			IProjectItem parent = child.Parent;
			if (parent != null)
			{
				parent.RemoveChild(child);
			}
			this.children.Add(child);
			child.Parent = this;
		}

		public virtual void CloseDocument()
		{
			throw new NotSupportedException();
		}

		public virtual bool CreateDocument(string initialContents)
		{
			throw new NotSupportedException();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.propertyCollectionStore != null)
			{
				this.propertyCollectionStore.Dispose();
				this.propertyCollectionStore = null;
			}
		}

		private string GetBuildItemMetadata(string name)
		{
			if (this.BuildItem == null || !this.BuildItem.HasMetadata(name))
			{
				return string.Empty;
			}
			return this.BuildItem.GetMetadataValue(name);
		}

		public virtual ICommandBar GetContextMenu(ICommandBarCollection commandBarCollection)
		{
			if (ProjectItemBase.contextMenu == null)
			{
				ProjectItemBase.contextMenu = commandBarCollection.AddContextMenu("Project_ProjectItemContextMenu");
				ProjectItemBase.contextMenu.Items.AddCheckBox("Project_SetStartupScene", StringTable.ProjectItemContextMenuStartupScene);
				ProjectItemBase.contextMenu.Items.AddButton("Project_OpenView", StringTable.ProjectItemContextMenuOpen);
				ProjectItemBase.contextMenu.Items.AddButton("Project_InsertIntoActiveDocument", StringTable.ProjectItemContextMenuInsert);
				ProjectItemBase.contextMenu.Items.AddButton("Project_EditExternally", StringTable.ProjectItemContextMenuEditExternally);
				ProjectItemBase.contextMenu.Items.AddButton("Project_EditVisualStudio", StringTable.ProjectItemContextMenuEditVisualStudio);
				ProjectItemBase.contextMenu.Items.AddSeparator();
				ProjectManager.AddSourceControlMenuItems(ProjectItemBase.contextMenu.Items);
				ProjectItemBase.contextMenu.Items.AddSeparator();
				ProjectItemBase.contextMenu.Items.AddButton("Project_Cut", StringTable.ProjectItemContextMenuCut);
				ProjectItemBase.contextMenu.Items.AddButton("Project_Copy", StringTable.ProjectItemContextMenuCopy);
				ProjectItemBase.contextMenu.Items.AddButton("Project_RenameProjectItem", StringTable.ProjectItemContextMenuRename);
				ProjectItemBase.contextMenu.Items.AddSeparator();
				ProjectItemBase.contextMenu.Items.AddButton("Project_DeleteProjectItem", StringTable.ProjectItemContextMenuDelete);
				ProjectItemBase.contextMenu.Items.AddButton("Project_RemoveProjectItem", StringTable.ProjectItemContextMenuRemove);
				ProjectItemBase.contextMenu.Items.AddSeparator();
				ProjectItemBase.contextMenu.Items.AddButton("Project_ExploreProject", StringTable.ProjectItemContextMenuExplore);
				if (this.serviceProvider.CommandService().Commands.Contains("SketchFlow_MakeIntoNavigationScreen"))
				{
					ProjectItemBase.contextMenu.Items.AddSeparator();
					ProjectItemBase.contextMenu.Items.AddButton("SketchFlow_MakeIntoNavigationScreen");
					ProjectItemBase.contextMenu.Items.AddButton("SketchFlow_MakeIntoCompositionScreen");
				}
			}
			return ProjectItemBase.contextMenu;
		}

		private string GetItemProperty(string propertyName)
		{
			if (propertyName != "BuildAction")
			{
				return null;
			}
			return this.BuildItemName;
		}

		public string GetResourceReference(Microsoft.Expression.Framework.Documents.DocumentReference referencingDocument)
		{
			if (referencingDocument != null && referencingDocument.Equals(this.Project.ProjectRoot))
			{
				return this.ProjectRelativeDocumentReference.TrimStart(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectorySeparatorCharacters());
			}
			return (new ResourceReferenceCreator(this)).CreateResourceReferenceFromDocument(referencingDocument);
		}

		public static bool IsComponentResourceReference(string resourceReference)
		{
			if (ProjectItemBase.isComponentUriHelper(resourceReference, "pack://application:,,,/"))
			{
				return true;
			}
			if (ProjectItemBase.isComponentUriHelper(resourceReference, "/"))
			{
				return true;
			}
			return false;
		}

		public virtual bool IsComponentUri(string resourceReference)
		{
			return ProjectItemBase.IsComponentResourceReference(resourceReference);
		}

		private static bool isComponentUriHelper(string resourceReference, string expectedPrefix)
		{
			if (resourceReference == null)
			{
				return false;
			}
			if (resourceReference.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
			{
				string str = resourceReference.Substring(expectedPrefix.Length);
				char[] chrArray = new char[] { '/', '\\' };
				string[] strArrays = str.Split(chrArray)[0].Split(new char[] { ';' });
				if (strArrays[(int)strArrays.Length - 1].Equals("component", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		string Microsoft.Expression.Project.IMSBuildItem.GetMetadata(string name)
		{
			return this.GetBuildItemMetadata(name);
		}

		void Microsoft.Expression.Project.IMSBuildItem.SetMetadata(string name, string value)
		{
			this.SetMetadata(name, value);
		}

		private void OnBuildItemChanged()
		{
			IMSBuildBasedProject project = this.Project as IMSBuildBasedProject;
			if (project != null)
			{
				project.OnBuildItemChanged();
			}
		}

		internal virtual void OnFileInformationChanged(WatcherChangeTypes changeTypes)
		{
			if (this.FileInformationChanged != null)
			{
				this.FileInformationChanged(this, new FileSystemEventArgs(changeTypes, string.Empty, string.Empty));
			}
		}

		protected virtual void OnIsCutChanged()
		{
			if (this.IsCutChanged != null)
			{
				this.IsCutChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnIsDirtyChanged()
		{
			if (this.IsDirtyChanged != null)
			{
				this.IsDirtyChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnIsReadOnlyChanged()
		{
			if (this.IsReadOnlyChanged != null)
			{
				this.IsReadOnlyChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnParentChanged()
		{
			if (this.ParentChanged != null)
			{
				this.ParentChanged(this, EventArgs.Empty);
			}
		}

		public virtual bool OpenDocument(bool isReadOnly, bool suppressUI)
		{
			throw new NotSupportedException();
		}

		public bool OpenDocument(bool isReadOnly)
		{
			return this.OpenDocument(isReadOnly, false);
		}

		public IDocumentView OpenView(bool makeActive)
		{
			if (!this.DocumentType.CanView)
			{
				return null;
			}
			if (!this.IsOpen && !this.OpenDocument(false, false))
			{
				return null;
			}
			IViewService viewService = this.Services.ViewService();
			IDocumentView documentView = viewService.Views.OfType<IDocumentView>().FirstOrDefault<IDocumentView>((IDocumentView v) => v.Document == this.Document);
			if (documentView == null)
			{
				documentView = this.Document.CreateDefaultView();
				if (documentView != null)
				{
					viewService.OpenView(documentView);
				}
			}
			if (documentView != null && makeActive)
			{
				viewService.ActiveView = documentView;
			}
			return documentView;
		}

		private bool ReferencedBy(IProjectItem projectItem)
		{
			if (projectItem == this)
			{
				return false;
			}
			bool flag = false;
			if (projectItem.DocumentType.CanView)
			{
				bool isOpen = false;
				if (!projectItem.IsOpen)
				{
					projectItem.OpenDocument(false, true);
					isOpen = projectItem.IsOpen;
				}
				if (projectItem.IsOpen)
				{
					flag = projectItem.Document.ReferencesDocument(base.DocumentReference);
				}
				if (isOpen)
				{
					projectItem.CloseDocument();
				}
			}
			return flag;
		}

		public virtual void RemoveChild(IProjectItem child)
		{
			child.Parent = null;
			this.children.Remove(child);
		}

		public virtual bool SaveDocumentFile()
		{
			throw new NotSupportedException();
		}

		protected void SetCondition(string condition)
		{
			if (this.BuildItem == null)
			{
				return;
			}
			this.BuildItem.Xml.Condition = condition;
			this.OnBuildItemChanged();
		}

		private void SetItemProperty(string propertyName, string value)
		{
			if (propertyName == "BuildAction")
			{
				this.BuildItemName = value;
			}
		}

		protected void SetMetadata(string name, string value)
		{
			if (this.BuildItem == null)
			{
				return;
			}
			this.BuildItem.SetMetadataValue(name, value);
			this.OnBuildItemChanged();
		}

		public override string ToString()
		{
			return base.DocumentReference.Path;
		}

		public event FileSystemEventHandler FileInformationChanged;

		public event EventHandler IsCutChanged;

		public event EventHandler IsDirtyChanged;

		public event EventHandler IsReadOnlyChanged;

		public event EventHandler ParentChanged;
	}
}