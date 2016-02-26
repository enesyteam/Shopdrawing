using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal class ProjectItem : ProjectItemBase, IDocumentContainer
	{
		private IDocument document;

		private ProjectFileInformation fileInformation;

		private bool closingProjectItem;

		public override IProjectItem CodeBehindItem
		{
			get
			{
				return (
					from child in this.Children.OfType<IMSBuildItem>()
					where !string.IsNullOrEmpty(child.GetMetadata(ProjectItem.MSBuildDependentUponMetadata))
					select child).FirstOrDefault<IMSBuildItem>() as IProjectItem;
			}
		}

		public override IDocument Document
		{
			get
			{
				return this.document;
			}
		}

		public override bool FileExists
		{
			get
			{
				return Microsoft.Expression.Framework.Documents.PathHelper.FileExists(base.DocumentReference.Path);
			}
		}

		internal ProjectFileInformation FileInformation
		{
			get
			{
				return this.fileInformation;
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (this.document == null)
				{
					return false;
				}
				return this.document.IsDirty;
			}
		}

		public override bool IsOpen
		{
			get
			{
				return this.document != null;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return Microsoft.Expression.Framework.Documents.PathHelper.IsFileOrDirectoryReadOnly(base.DocumentReference.Path);
			}
		}

		internal static string MSBuildDependentUponMetadata
		{
			get
			{
				return "DependentUpon";
			}
		}

		public ProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
			this.UpdateFileInformation();
			base.Project.ItemDeleted += new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
			base.Project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
			base.Project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
		}

		public override void AddChild(IProjectItem child)
		{
			base.AddChild(child);
		}

		public override void CloseDocument()
		{
			if (this.IsOpen)
			{
				this.closingProjectItem = true;
				try
				{
					base.Services.DocumentService().CloseDocument(this.document);
				}
				finally
				{
					this.closingProjectItem = false;
				}
			}
		}

		public override bool CreateDocument(string initialContents)
		{
			bool flag;
			if (this.document != null)
			{
				throw new InvalidOperationException();
			}
			if (!base.DocumentType.CanCreate)
			{
				throw new InvalidOperationException();
			}
			try
			{
				IDocument document = base.DocumentType.CreateDocument(this, base.Project, initialContents);
				this.SetDocument(document);
				bool flag1 = this.SaveDocumentFile();
				KnownProjectBase project = base.Project as KnownProjectBase;
				if (project != null)
				{
					project.OnItemOpened(new ProjectItemEventArgs(this));
				}
				return flag1;
			}
			catch (IOException oException1)
			{
				IOException oException = oException1;
				ErrorArgs errorArg = new ErrorArgs()
				{
					Message = StringTable.ProjectItemOpenDocumentErrorMessage,
					Exception = oException,
					AutomationId = "ProjectItemOpenDocumentErrorDialog"
				};
				base.Services.MessageDisplayService().ShowError(errorArg);
				flag = false;
			}
			return flag;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.Project.ItemDeleted -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
				base.Project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
				base.Project.ItemRenamed -= new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
			}
			base.Dispose(disposing);
		}

		private void Document_IsDirtyChanged(object sender, EventArgs e)
		{
			this.OnIsDirtyChanged();
		}

		private void DocumentManager_DocumentClosed(object sender, DocumentEventArgs e)
		{
			if (e.Document == this.document)
			{
				KnownProjectBase project = base.Project as KnownProjectBase;
				if (project != null)
				{
					project.OnItemClosing(new ProjectItemEventArgs(this));
				}
				bool flag = false;
				if (!this.closingProjectItem && !this.FileExists && !base.IsCodeBehindItem)
				{
					flag = true;
				}
				base.DocumentType.CloseDocument(this, base.Project);
				base.Services.DocumentService().DocumentClosed -= new DocumentEventHandler(this.DocumentManager_DocumentClosed);
				base.Project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
				this.document.IsDirtyChanged -= new EventHandler(this.Document_IsDirtyChanged);
				this.document.Container = null;
				this.document = null;
				if (flag && base.Project.Items.Contains(this))
				{
					IProjectItem codeBehindItem = this.CodeBehindItem;
					if (codeBehindItem != null)
					{
						IProject project1 = base.Project;
						IProjectItem[] projectItemArray = new IProjectItem[] { codeBehindItem };
						project1.RemoveItems(true, projectItemArray);
					}
					IProject project2 = base.Project;
					IProjectItem[] projectItemArray1 = new IProjectItem[] { this };
					project2.RemoveItems(false, projectItemArray1);
				}
				this.OnIsDirtyChanged();
				if (project != null)
				{
					project.OnItemClosed(new ProjectItemEventArgs(this));
				}
			}
		}

		void Microsoft.Expression.Framework.Documents.IDocumentContainer.BeginCheckDocumentStatus(IDocument document)
		{
			ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
			if (currentSolution != null && currentSolution.IsSourceControlActive && SourceControlStatusCache.GetCachedStatus(this) == SourceControlStatus.CheckedIn)
			{
				ISourceControlProvider sourceControlProvider = base.Services.SourceControlProvider();
				string[] path = new string[] { base.DocumentReference.Path };
				if (sourceControlProvider.Checkout(path) == SourceControlSuccess.Success)
				{
					SourceControlStatusCache.SetCachedStatus(this, SourceControlStatus.CheckedOut);
				}
			}
		}

		void Microsoft.Expression.Framework.Documents.IDocumentContainer.BeginDocumentSave(IDocument document)
		{
			KnownProjectBase project = base.Project as KnownProjectBase;
			if (project != null)
			{
				project.DisableWatchingForChanges();
			}
		}

		void Microsoft.Expression.Framework.Documents.IDocumentContainer.DocumentSaveCompleted(IDocument document, bool saveSucceeded)
		{
			this.UpdateFileInformation();
			KnownProjectBase project = base.Project as KnownProjectBase;
			if (project != null)
			{
				project.EnableWatchingForChanges();
				if (saveSucceeded)
				{
					IProjectItem codeBehindItem = this.CodeBehindItem;
					if (codeBehindItem != null && !codeBehindItem.FileExists && codeBehindItem.IsOpen)
					{
						DocumentUtilities.SaveDocument(codeBehindItem.Document, true, true, base.Services.MessageDisplayService());
					}
				}
			}
		}

		public override bool OpenDocument(bool isReadOnly, bool suppressUI)
		{
			bool flag;
			if (this.document != null)
			{
				return true;
			}
			KnownProjectBase project = base.Project as KnownProjectBase;
			if (project != null && project.IsDisposed)
			{
				return false;
			}
			try
			{
				IDocument document = base.DocumentType.OpenDocument(this, base.Project, isReadOnly);
				this.SetDocument(document);
				if (project != null)
				{
					project.OnItemOpened(new ProjectItemEventArgs(this));
				}
				return true;
			}
			catch (IOException oException1)
			{
				IOException oException = oException1;
				if (!suppressUI)
				{
					ErrorArgs errorArg = new ErrorArgs()
					{
						Message = StringTable.ProjectItemOpenDocumentErrorMessage,
						Exception = oException,
						AutomationId = "ProjectItemOpenDocumentErrorDialog"
					};
					base.Services.MessageDisplayService().ShowError(errorArg);
				}
				flag = false;
			}
			return flag;
		}

		private void Project_ItemChanged(object sender, ProjectItemEventArgs e)
		{
			if (this.document != null)
			{
				this.document.ProjectItemChanged(e.ProjectItem.DocumentReference);
			}
		}

		private void Project_ItemDeleted(object sender, ProjectItemEventArgs e)
		{
			if (this.document != null)
			{
				this.document.ProjectItemRemoved(e.ProjectItem.DocumentReference);
			}
		}

		private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (this.document != null)
			{
				this.document.ProjectItemRemoved(e.ProjectItem.DocumentReference);
			}
		}

		private void Project_ItemRenamed(object sender, ProjectItemRenamedEventArgs e)
		{
			if (this.document != null)
			{
				this.document.ProjectItemRenamed(e.OldName, e.ProjectItem.DocumentReference);
			}
		}

		public override bool SaveDocumentFile()
		{
			if (!this.IsOpen)
			{
				return true;
			}
			return DocumentUtilities.SaveDocument(this.Document, true, true, base.Services.MessageDisplayService());
		}

		private void SetDocument(IDocument document)
		{
			this.document = document;
			this.document.Container = this;
			base.Services.DocumentService().DocumentClosed += new DocumentEventHandler(this.DocumentManager_DocumentClosed);
			this.document.IsDirtyChanged += new EventHandler(this.Document_IsDirtyChanged);
			base.Services.DocumentService().OpenDocument(this.document);
			base.Project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
		}

		public void UpdateFileInformation()
		{
			bool flag = (this.FileInformation == null ? false : this.FileInformation.Exists);
			if (!(base.DocumentReference != null) || string.IsNullOrEmpty(base.DocumentReference.Path))
			{
				this.fileInformation = new ProjectFileInformation(null);
			}
			else
			{
				this.fileInformation = new ProjectFileInformation(base.DocumentReference.Path);
			}
			if (flag && !this.FileInformation.Exists)
			{
				this.OnFileInformationChanged(WatcherChangeTypes.Deleted);
				return;
			}
			if (!flag && this.FileInformation.Exists)
			{
				this.OnFileInformationChanged(WatcherChangeTypes.Created);
			}
		}
	}
}