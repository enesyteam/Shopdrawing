using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	public class ProjectNode : HierarchicalNode, INotifyPropertyChanged
	{
		private INamedProject project;

		private ProjectItemNode referencesFolder;

		private ICommandBar projectContextMenu;

		public ICommand ActivateCommand
		{
			get
			{
				return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnActivateCommand));
			}
		}

		public virtual ImageSource BitmapImage
		{
			get
			{
				return FileTable.GetImageSource("Resources\\file_unknownProject_on_16x16.png");
			}
		}

		public override string DisplayName
		{
			get
			{
				if (LicensingHelper.IsProjectLicensed(((ProjectBase)this.project).ProjectStore, base.Services).IsExpired)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string unlicensedProjectDisplayNameTemplate = StringTable.UnlicensedProjectDisplayNameTemplate;
					object[] name = new object[] { this.project.Name };
					return string.Format(currentCulture, unlicensedProjectDisplayNameTemplate, name);
				}
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				string unsupportedProjectDisplayNameTemplate = StringTable.UnsupportedProjectDisplayNameTemplate;
				object[] objArray = new object[] { this.project.Name };
				return string.Format(cultureInfo, unsupportedProjectDisplayNameTemplate, objArray);
			}
			set
			{
			}
		}

		public virtual ImageSource FrameworkImage
		{
			get
			{
				return null;
			}
		}

		public virtual string FrameworkString
		{
			get
			{
				return null;
			}
		}

		public string FullPath
		{
			get
			{
				return this.project.DocumentReference.Path;
			}
		}

		public virtual bool IsStartup
		{
			get
			{
				return false;
			}
		}

		internal INamedProject Project
		{
			get
			{
				return this.project;
			}
		}

		public ProjectItemNode ReferencesFolder
		{
			get
			{
				if (this.referencesFolder == null)
				{
					foreach (HierarchicalNode child in base.Children)
					{
						if (child.Children.Count <= 0 || !((ProjectItemNode)child.Children[0]).ProjectItem.IsReference)
						{
							continue;
						}
						this.referencesFolder = (ProjectItemNode)child;
						break;
					}
				}
				return this.referencesFolder;
			}
		}

		public virtual string ToolTip
		{
			get
			{
				return PathHelper.CompressPathForDisplay(this.FullPath, 50);
			}
		}

		public virtual bool UseFrameworkToolTip
		{
			get
			{
				return false;
			}
		}

		internal ProjectNode(INamedProject project, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane) : base(project, projectPane)
		{
			this.project = project;
			base.IsExpandedChanged += new EventHandler(this.ProjectItemNode_IsExpandedChanged);
		}

		internal virtual void AddItemNode(IProjectItem item)
		{
			ProjectItemNode projectItemNode = ProjectItemNode.Create(item, base.ProjectPane, this);
			HierarchicalNode hierarchicalNode = this;
			if (item.Parent != null)
			{
				IProjectItem parent = item.Parent;
				if (parent != null)
				{
					ProjectItemNode projectItemNode1 = this.FindItemNode(parent);
					if (projectItemNode1 != null)
					{
						hierarchicalNode = projectItemNode1;
					}
				}
			}
			hierarchicalNode.AddChild(projectItemNode);
		}

		protected void AddItemsRecursively(IProjectItem parent)
		{
			this.AddItemNode(parent);
			foreach (IProjectItem child in parent.Children)
			{
				this.AddItemsRecursively(child);
			}
		}

		protected void AddSortedItemNode(HierarchicalNode node)
		{
			base.AddChild(node);
		}

		public override int CompareTo(HierarchicalNode treeItem)
		{
			ProjectNode projectNode = treeItem as ProjectNode;
			if (projectNode == null)
			{
				return 0;
			}
			return AlphabeticThenNumericComparer.Compare(this.DisplayName, projectNode.DisplayName, CultureInfo.CurrentCulture);
		}

		private void DestroyContextMenu()
		{
			this.projectContextMenu = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.IsExpandedChanged -= new EventHandler(this.ProjectItemNode_IsExpandedChanged);
				base.Dispose(disposing);
			}
		}

		protected override void ExpandAllParents()
		{
		}

		internal ProjectItemNode FindItemNode(IProjectItem item)
		{
			return ProjectNode.FindItemNodeRecursive(item, base.Children);
		}

		private static ProjectItemNode FindItemNodeRecursive(IProjectItem item, VirtualizingTreeItemCollection<HierarchicalNode> collection)
		{
			ProjectItemNode projectItemNode;
			using (IEnumerator<ProjectItemNode> enumerator = collection.OfType<ProjectItemNode>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ProjectItemNode current = enumerator.Current;
					if (current.ProjectItem != item)
					{
						ProjectItemNode projectItemNode1 = ProjectNode.FindItemNodeRecursive(item, current.Children);
						if (projectItemNode1 == null)
						{
							continue;
						}
						projectItemNode = projectItemNode1;
						return projectItemNode;
					}
					else
					{
						projectItemNode = current;
						return projectItemNode;
					}
				}
				return null;
			}
			return projectItemNode;
		}

		private void OnActivateCommand()
		{
			this.IsExpanded = !this.IsExpanded;
		}

		protected override void OnCreateContextMenuCommand()
		{
			if (base.Services.ProjectManager().CurrentSolution != null && base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
			{
				this.DestroyContextMenu();
			}
			if (this.projectContextMenu == null)
			{
				ICommandBarService commandBarService = base.Services.CommandBarService();
				commandBarService.CommandBars.Remove("Project_ProjectContextMenu");
				this.projectContextMenu = commandBarService.CommandBars.AddContextMenu("Project_ProjectContextMenu");
				this.projectContextMenu.Items.AddCheckBox("Project_SetStartupProject");
				this.projectContextMenu.Items.AddButton("Project_EditExternally", StringTable.ProjectItemContextMenuEditExternally);
				this.projectContextMenu.Items.AddButton("Project_EditVisualStudio", StringTable.ProjectItemContextMenuEditVisualStudio);
				this.projectContextMenu.Items.AddSeparator();
				this.projectContextMenu.Items.AddButton("Project_ConvertToSilverlight4", StringTable.ProjectItemContextMenuConvertSilverlight4);
				this.projectContextMenu.Items.AddButton("Project_ConvertToDotNet4", StringTable.ProjectItemContextMenuConvertFramework4);
				this.projectContextMenu.Items.AddSeparator();
				ProjectManager.AddSourceControlMenuItems(this.projectContextMenu.Items);
				this.projectContextMenu.Items.AddSeparator();
				if (base.Services.ProjectManager().CurrentSolution is WebProjectSolution)
				{
					this.projectContextMenu.Items.AddButton("Project_TestProject");
					this.projectContextMenu.Items.AddSeparator();
				}
				this.projectContextMenu.Items.AddButton("Application_AddNewItem", StringTable.ProjectItemContextMenuAddNewItem);
				this.projectContextMenu.Items.AddButton("Project_AddExistingItem", StringTable.ProjectItemContextMenuAddExistingItem);
				this.projectContextMenu.Items.AddButton("Project_LinkToExistingItem", StringTable.ProjectItemContextMenuLinkToExistingItem);
				this.projectContextMenu.Items.AddSeparator();
				this.projectContextMenu.Items.AddButton("Project_AddReference", StringTable.ProjectItemContextMenuAddReference);
				this.projectContextMenu.Items.AddDynamicMenu("Project_AddProjectReference", StringTable.ProjectItemContextMenuAddProjectReference);
				this.projectContextMenu.Items.AddSeparator();
				this.projectContextMenu.Items.AddButton("Project_NewFolder", StringTable.ProjectItemContextMenuNewFolder);
				this.projectContextMenu.Items.AddButton("Project_Refresh", StringTable.ProjectItemContextMenuRefresh);
				this.projectContextMenu.Items.AddSeparator();
				this.projectContextMenu.Items.AddButton("Project_Paste", StringTable.ProjectItemContextMenuPaste);
				this.projectContextMenu.Items.AddSeparator();
				this.projectContextMenu.Items.AddButton("Project_ExploreProject", StringTable.ProjectItemContextMenuExplore);
			}
			this.ContextMenu = (System.Windows.Controls.ContextMenu)this.projectContextMenu;
		}

		private void ProjectItemNode_IsExpandedChanged(object sender, EventArgs e)
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
			return this.project.ToString();
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