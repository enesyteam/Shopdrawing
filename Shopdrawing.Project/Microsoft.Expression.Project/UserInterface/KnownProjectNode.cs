using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class KnownProjectNode : ProjectNode, INotifyPropertyChanged
	{
		private IProject project;

		private ProjectItemNode referencesFolder;

		public override ImageSource BitmapImage
		{
			get
			{
				if (((IProject)base.Project).CodeDocumentType == null)
				{
					return Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\file_unknownProject_on_16x16.png");
				}
				return ((IProject)base.Project).CodeDocumentType.ProjectIcon;
			}
		}

		public override string DisplayName
		{
			get
			{
				return base.Project.Name;
			}
		}

		public override ImageSource FrameworkImage
		{
			get
			{
				FrameworkName targetFramework = this.project.TargetFramework;
				if (targetFramework == null)
				{
					return null;
				}
				string identifier = targetFramework.Identifier;
				string str = identifier;
				if (identifier != null)
				{
					if (str == ".NETFramework")
					{
						return Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\NetFramework.png");
					}
					if (str == "Silverlight")
					{
						return Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\Silverlight.png");
					}
				}
				return null;
			}
		}

		public override string FrameworkString
		{
			get
			{
				FrameworkName targetFramework = this.project.TargetFramework;
				if (targetFramework == null)
				{
					return null;
				}
				string identifier = targetFramework.Identifier;
				string str = identifier;
				if (identifier != null)
				{
					if (str == ".NETFramework")
					{
						CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
						string dotNetProjectTooltipText = StringTable.DotNetProjectTooltipText;
						object[] version = new object[] { targetFramework.Version };
						return string.Format(currentUICulture, dotNetProjectTooltipText, version);
					}
					if (str == "Silverlight")
					{
						CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
						string silverlightProjectTooltipText = StringTable.SilverlightProjectTooltipText;
						object[] objArray = new object[] { targetFramework.Version };
						return string.Format(cultureInfo, silverlightProjectTooltipText, objArray);
					}
				}
				return null;
			}
		}

		public override bool IsStartup
		{
			get
			{
				ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
				if (currentSolution == null)
				{
					return false;
				}
				INamedProject startupProject = currentSolution.StartupProject as INamedProject;
				return base.Project == startupProject;
			}
		}

		public override bool UseFrameworkToolTip
		{
			get
			{
				FrameworkName targetFramework = this.project.TargetFramework;
				if (targetFramework == null)
				{
					return false;
				}
				if (targetFramework.Identifier == ".NETFramework")
				{
					return true;
				}
				return targetFramework.Identifier == "Silverlight";
			}
		}

		internal KnownProjectNode(IProject project, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane) : base(project, projectPane)
		{
			this.project = project;
			this.project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
			this.project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
			this.project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
			this.project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
			this.project.ItemDeleted += new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
			this.project.PropertyChanged += new PropertyChangedEventHandler(this.Project_PropertyChanged);
			if (this.project.GetCapability<bool>("CanAddAssemblyReference") || this.project.GetCapability<bool>("CanAddProjectReference"))
			{
				this.referencesFolder = ProjectItemNode.Create(new FolderStandIn(this.project, StringTable.ProjectNodeReferencesFolderName, base.Services), projectPane, this);
				base.AddSortedItemNode(this.referencesFolder);
				this.referencesFolder.IsExpanded = false;
			}
			this.AddAllProjectItems();
			this.IsExpanded = true;
			((ProjectManager)base.Services.ProjectManager()).StartupProjectChanged += new EventHandler(this.ProjectManager_StartupProjectChanged);
		}

		private void AddAllProjectItems()
		{
			foreach (IProjectItem item in this.project.Items)
			{
				if (item.Parent != null)
				{
					continue;
				}
				base.AddItemsRecursively(item);
			}
		}

		internal override void AddItemNode(IProjectItem item)
		{
			if (!(item is AssemblyReferenceProjectItem) || this.referencesFolder == null)
			{
				base.AddItemNode(item);
				return;
			}
			this.referencesFolder.AddChild(ProjectItemNode.Create(item, base.ProjectPane, this));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.referencesFolder != null)
				{
					this.referencesFolder = null;
				}
				base.Services.ProjectManager().StartupProjectChanged -= new EventHandler(this.ProjectManager_StartupProjectChanged);
				if (this.project != null)
				{
					this.project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
					this.project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
					this.project.ItemRenamed -= new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
					this.project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
					this.project.ItemDeleted -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemDeleted);
					this.project.PropertyChanged -= new PropertyChangedEventHandler(this.Project_PropertyChanged);
				}
				base.Dispose(disposing);
			}
		}

		private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
		{
			this.AddItemNode(e.ProjectItem);
		}

		private void Project_ItemChanged(object sender, ProjectItemEventArgs e)
		{
			IProjectItem projectItem = e.ProjectItem;
			if (projectItem.IsOpen)
			{
				MessageBoxResult messageBoxResult = MessageBoxResult.No;
				string projectItemChangedDialogFileDirtyMessage = null;
				if (projectItem.IsDirty)
				{
					if ((e.Options & ProjectItemEventOptions.SilentIfDirty) != ProjectItemEventOptions.SilentIfDirty)
					{
						projectItemChangedDialogFileDirtyMessage = StringTable.ProjectItemChangedDialogFileDirtyMessage;
					}
					else
					{
						messageBoxResult = MessageBoxResult.Yes;
					}
				}
				else if ((e.Options & ProjectItemEventOptions.SilentIfOpen) != ProjectItemEventOptions.SilentIfOpen)
				{
					projectItemChangedDialogFileDirtyMessage = StringTable.ProjectItemChangedDialogFileCleanMessage;
				}
				else
				{
					messageBoxResult = MessageBoxResult.Yes;
				}
				if (!SolutionBase.IsReloadPromptEnabled())
				{
					messageBoxResult = MessageBoxResult.Yes;
					projectItemChangedDialogFileDirtyMessage = null;
				}
				if (!string.IsNullOrEmpty(projectItemChangedDialogFileDirtyMessage))
				{
					MessageBoxArgs messageBoxArg = new MessageBoxArgs();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					object[] path = new object[] { projectItem.DocumentReference.Path, base.Services.ExpressionInformationService().ShortApplicationName };
					messageBoxArg.Message = string.Format(currentCulture, projectItemChangedDialogFileDirtyMessage, path);
					messageBoxArg.Button = MessageBoxButton.YesNo;
					messageBoxArg.Image = MessageBoxImage.Exclamation;
					MessageBoxArgs messageBoxArg1 = messageBoxArg;
					messageBoxResult = base.Services.MessageDisplayService().ShowMessage(messageBoxArg1);
				}
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					bool isReadOnly = projectItem.Document.IsReadOnly;
					bool document = projectItem.Document == base.Services.DocumentService().ActiveDocument;
					bool flag = false;
					foreach (IView view in base.Services.ViewService().Views)
					{
						IDocumentView documentView = view as IDocumentView;
						if (documentView == null || documentView.Document != projectItem.Document)
						{
							continue;
						}
						flag = true;
						break;
					}
					projectItem.CloseDocument();
					projectItem.OpenDocument(isReadOnly);
					if (projectItem.IsOpen && flag)
					{
						projectItem.OpenView(document);
					}
				}
			}
		}

		private void Project_ItemDeleted(object sender, ProjectItemEventArgs e)
		{
			IProjectItem projectItem = e.ProjectItem;
			if (projectItem != null && projectItem.Document != null)
			{
				projectItem.Document.SourceChanged();
			}
		}

		private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (base.Services.ProjectManager().ItemSelectionSet.IsSelected(e.ProjectItem))
			{
				base.Services.ProjectManager().ItemSelectionSet.RemoveSelection(e.ProjectItem);
			}
			ProjectItemNode projectItemNode = base.FindItemNode(e.ProjectItem);
			if (projectItemNode != null)
			{
				if (projectItemNode.Parent != null)
				{
					projectItemNode.Parent.RemoveChild(projectItemNode);
				}
				projectItemNode.Dispose();
			}
		}

		private void Project_ItemRenamed(object sender, ProjectItemRenamedEventArgs e)
		{
			ProjectItemNode projectItemNode = base.FindItemNode(e.ProjectItem);
			if (projectItemNode != null)
			{
				projectItemNode.OnRenamed();
			}
		}

		private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		}

		private void ProjectManager_StartupProjectChanged(object sender, EventArgs e)
		{
			base.OnPropertyChanged("IsStartup");
		}

		internal override void UpdateSelection()
		{
			base.OnSelectionChanged();
			foreach (HierarchicalNode child in base.Children)
			{
				child.UpdateSelection();
			}
		}
	}
}