using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class SolutionNode : HierarchicalNode
	{
		private static ICommandBar solutionContextMenu;

		private ISolution solution;

		private int projectCount;

		public ImageSource BitmapImage
		{
			get
			{
				return FileTable.GetImageSource("Resources\\file_solution_on_16x16.png");
			}
		}

		public override string DisplayName
		{
			get
			{
				string empty = string.Empty;
				if (this.solution != null && this.solution.DocumentReference != null)
				{
					if (base.Children.Count != 1)
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string projectPaneSolutionTitle = StringTable.ProjectPaneSolutionTitle;
						object[] fileNameWithoutExtension = new object[] { Path.GetFileNameWithoutExtension(this.solution.DocumentReference.DisplayName), base.Children.Count };
						empty = string.Format(currentCulture, projectPaneSolutionTitle, fileNameWithoutExtension);
					}
					else
					{
						CultureInfo cultureInfo = CultureInfo.CurrentCulture;
						string projectPaneSolutionTitleSingle = StringTable.ProjectPaneSolutionTitleSingle;
						object[] objArray = new object[] { Path.GetFileNameWithoutExtension(this.solution.DocumentReference.DisplayName) };
						empty = string.Format(cultureInfo, projectPaneSolutionTitleSingle, objArray);
					}
				}
				return empty;
			}
			set
			{
			}
		}

		public string FullPath
		{
			get
			{
				return this.solution.DocumentReference.Path;
			}
		}

		public string ToolTip
		{
			get
			{
				return this.solution.DocumentReference.Path;
			}
		}

		static SolutionNode()
		{
		}

		internal SolutionNode(ISolution solution, Microsoft.Expression.Project.UserInterface.ProjectPane projectPane) : base(solution, projectPane)
		{
			this.solution = solution;
			this.IsExpanded = true;
			base.Children.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SolutionNode_CollectionChanged);
		}

		public override int CompareTo(HierarchicalNode treeItem)
		{
			return 0;
		}

		private static void DestroyContextMenu()
		{
			SolutionNode.solutionContextMenu = null;
		}

		protected override void Dispose(bool disposing)
		{
			base.Children.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SolutionNode_CollectionChanged);
			base.Dispose(disposing);
		}

		protected override void ExpandAllParents()
		{
		}

		private static void InitializeContextMenu(ICommandBarService commandBarService)
		{
			commandBarService.CommandBars.Remove("Project_SolutionContextMenu");
			SolutionNode.solutionContextMenu = commandBarService.CommandBars.AddContextMenu("Project_SolutionContextMenu");
			SolutionNode.solutionContextMenu.Items.AddButton("Project_EditExternally", StringTable.ProjectItemContextMenuEditExternally);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_EditVisualStudio", StringTable.ProjectItemContextMenuEditVisualStudio);
			SolutionNode.solutionContextMenu.Items.AddSeparator();
			ProjectManager.AddSourceControlMenuItems(SolutionNode.solutionContextMenu.Items);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_RefreshStatus", StringTable.SourceControlContextMenuRefreshStatus);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_GoOnline", StringTable.SourceControlContextMenuGoOnline);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_ResolveConflicts", StringTable.SourceControlContextMenuResolve);
			SolutionNode.solutionContextMenu.Items.AddSeparator();
			SolutionNode.solutionContextMenu.Items.AddButton("Project_AddNewProject", StringTable.SolutionItemContextMenuAddNewProject);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_AddExistingProject", StringTable.SolutionItemContextMenuAddExistingProject);
			SolutionNode.solutionContextMenu.Items.AddButton("Project_AddExistingWebsite", StringTable.SolutionItemContextMenuAddExistingWebsite);
			SolutionNode.solutionContextMenu.Items.AddSeparator();
			SolutionNode.solutionContextMenu.Items.AddButton("Project_Build");
			SolutionNode.solutionContextMenu.Items.AddButton("Project_Rebuild");
			SolutionNode.solutionContextMenu.Items.AddButton("Project_Clean");
			SolutionNode.solutionContextMenu.Items.AddButton("Project_TestProject");
			SolutionNode.solutionContextMenu.Items.AddSeparator();
			SolutionNode.solutionContextMenu.Items.AddButton("Project_ExploreProject", StringTable.ProjectItemContextMenuExplore);
		}

		protected override void OnCreateContextMenuCommand()
		{
			if (base.Services.ProjectManager().CurrentSolution != null && base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
			{
				SolutionNode.DestroyContextMenu();
			}
			if (SolutionNode.solutionContextMenu == null)
			{
				SolutionNode.InitializeContextMenu(base.Services.CommandBarService());
			}
			this.ContextMenu = (System.Windows.Controls.ContextMenu)SolutionNode.solutionContextMenu;
		}

		private void SolutionNode_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.projectCount != base.Children.Count)
			{
				this.projectCount = base.Children.Count;
				base.OnPropertyChanged("DisplayName");
			}
		}
	}
}