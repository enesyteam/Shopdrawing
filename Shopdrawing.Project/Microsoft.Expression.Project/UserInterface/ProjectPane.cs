using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Project.UserInterface
{
	public sealed class ProjectPane : DockPanel, IDisposable, INotifyPropertyChanged, ITreeViewItemProvider<HierarchicalNode>
	{
		private IServiceProvider serviceProvider;

		private CommandTarget commandTarget;

		private FileDropUtility dropUtility;

		private DispatcherTimer filterTimer;

		private string filterString;

		private bool enableSearch;

		private Microsoft.Expression.Project.UserInterface.SearchTransaction searchTransaction;

		private DelegateCommand clearFilterStringCommand;

		private ClearableTextBox searchBox;

		private HierarchicalNode projectInsertion;

		private HierarchicalNode root;

		private VirtualizingTreeItemFlattener<HierarchicalNode> flattener;

		private bool disposed;

		private Dictionary<ProjectItemNode, bool> VisibilityDictionary = new Dictionary<ProjectItemNode, bool>();

		private readonly static string IsExpanded;

		private ProjectPaneWorkaroundVirtualizingStackPanel viewItemList;

		public System.Windows.Input.ICommand ClearFilterStringCommand
		{
			get
			{
				if (this.clearFilterStringCommand == null)
				{
					this.clearFilterStringCommand = new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.OnClearFilterStringCommand));
				}
				return this.clearFilterStringCommand;
			}
		}

		public bool EnableSearch
		{
			get
			{
				return this.enableSearch;
			}
			set
			{
				if (this.enableSearch != value)
				{
					this.enableSearch = value;
					this.OnPropertyChanged("EnableSearch");
				}
			}
		}

		public string FilterString
		{
			get
			{
				return this.filterString;
			}
			set
			{
				if (this.filterString != value)
				{
					this.filterString = value;
					if (this.filterString == null)
					{
						this.filterTimer_Tick(this.filterTimer, null);
						return;
					}
					this.filterTimer.Stop();
					this.filterTimer.Start();
				}
			}
		}

		public bool IsSourceControlActive
		{
			get
			{
				ISolution currentSolution = this.Services.ProjectManager().CurrentSolution;
				if (currentSolution == null)
				{
					return false;
				}
				return currentSolution.IsSourceControlActive;
			}
		}

		public ReadOnlyObservableCollection<HierarchicalNode> ItemList
		{
			get
			{
				return this.flattener.ItemList;
			}
		}

		public VirtualizingTreeItemCollection<HierarchicalNode> Projects
		{
			get
			{
				return this.projectInsertion.Children;
			}
		}

		public HierarchicalNode RootItem
		{
			get
			{
				return this.root;
			}
		}

		private ClearableTextBox SearchBox
		{
			get
			{
				if (this.searchBox == null)
				{
					this.searchBox = LogicalTreeHelper.FindLogicalNode(this, "SearchBox") as ClearableTextBox;
				}
				return this.searchBox;
			}
		}

		internal Microsoft.Expression.Project.UserInterface.SearchTransaction SearchTransaction
		{
			get
			{
				return this.searchTransaction;
			}
		}

		internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public int SolutionDepth
		{
			get
			{
				return this.projectInsertion.Depth;
			}
		}

		private ProjectPaneWorkaroundVirtualizingStackPanel ViewItemList
		{
			get
			{
				if (this.viewItemList == null)
				{
					ItemsControl itemsControl = LogicalTreeHelper.FindLogicalNode(this, "ViewItemList") as ItemsControl;
					if (itemsControl != null)
					{
						List<DependencyObject> dependencyObjects = new List<DependencyObject>()
						{
							itemsControl
						};
						for (int i = 0; i < dependencyObjects.Count && this.viewItemList == null; i++)
						{
							int num = 0;
							while (num < VisualTreeHelper.GetChildrenCount(dependencyObjects[i]))
							{
								DependencyObject child = VisualTreeHelper.GetChild(dependencyObjects[i], num);
								if (!(child is ProjectPaneWorkaroundVirtualizingStackPanel))
								{
									dependencyObjects.Add(child);
									num++;
								}
								else
								{
									this.viewItemList = child as ProjectPaneWorkaroundVirtualizingStackPanel;
									break;
								}
							}
						}
					}
				}
				return this.viewItemList;
			}
		}

		static ProjectPane()
		{
			ProjectPane.IsExpanded = "IsExpanded";
		}

		public ProjectPane(IServiceProvider serviceProvider)
		{
			this.root = new RootHierarchicalNode(this)
			{
				IsExpanded = true
			};
			this.flattener = new VirtualizingTreeItemFlattener<HierarchicalNode>(this, false);
			this.projectInsertion = this.root;
			this.filterTimer = new DispatcherTimer();
			this.filterTimer.Tick += new EventHandler(this.filterTimer_Tick);
			this.filterTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			this.serviceProvider = serviceProvider;
			ProjectManager projectManager = this.Services.ProjectManager() as ProjectManager;
			if (projectManager != null)
			{
				projectManager.SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
				projectManager.SolutionClosing += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosing);
				projectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
				projectManager.SolutionMigrated += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionMigrated);
				projectManager.ItemSelectionSet.SelectionChanged += new EventHandler(this.ItemSelectionSet_SelectionChanged);
			}
			int num = 0;
			IDocumentType[] unknownDocumentType = new IDocumentType[this.Services.DocumentTypes().Count + 1];
			foreach (IDocumentType documentType in this.Services.DocumentTypes())
			{
				int num1 = num;
				num = num1 + 1;
				unknownDocumentType[num1] = documentType;
			}
			unknownDocumentType[num] = this.Services.DocumentTypes().UnknownDocumentType;
			this.dropUtility = new FileDropUtility(projectManager, this, unknownDocumentType);
			this.Services.ViewService().ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
			this.commandTarget = new ProjectPane.ProjectUserInterfaceCommandTarget(this);
			this.Services.CommandService().AddTarget(this.commandTarget);
			SourceControlStatusCache.StatusUpdated += new EventHandler(this.SourceControlStatusCache_StatusUpdated);
		}

		private void CleanupClosedSolution(ISolution solution)
		{
			ISolutionManagement solutionManagement = solution as ISolutionManagement;
			if (solutionManagement != null)
			{
				solutionManagement.AnyProjectOpened -= new EventHandler<NamedProjectEventArgs>(this.Solution_AnyProjectOpened);
				solutionManagement.AnyProjectClosed -= new EventHandler<NamedProjectEventArgs>(this.Solution_AnyProjectClosed);
			}
			this.ClearProjectCollection();
			this.ClearSolutionCollection();
		}

		private void ClearProjectCollection()
		{
			for (int i = this.projectInsertion.Children.Count - 1; i >= 0; i--)
			{
				ProjectNode item = this.projectInsertion.Children[i] as ProjectNode;
				if (item != null)
				{
					this.projectInsertion.RemoveChild(item);
					item.Dispose();
				}
			}
		}

		private void ClearSolutionCollection()
		{
			for (int i = this.root.Children.Count - 1; i >= 0; i--)
			{
				SolutionNode item = this.root.Children[i] as SolutionNode;
				if (item != null)
				{
					this.root.RemoveChild(item);
					item.Dispose();
				}
			}
			this.projectInsertion = this.root;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				IProjectManager projectManager = this.Services.ProjectManager();
				if (projectManager != null)
				{
					projectManager.SolutionOpened -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
					projectManager.SolutionClosing -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosing);
					projectManager.SolutionClosed -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
					projectManager.SolutionMigrated -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionMigrated);
					projectManager.ItemSelectionSet.SelectionChanged -= new EventHandler(this.ItemSelectionSet_SelectionChanged);
				}
				IViewService viewService = this.Services.ViewService();
				if (viewService != null)
				{
					viewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
				}
				if (this.root != null)
				{
					this.root.Dispose();
				}
				this.disposed = true;
			}
		}

		private void FilterProject(ProjectNode project)
		{
			foreach (HierarchicalNode child in project.Children)
			{
				this.FilterProjectItem((ProjectItemNode)child);
			}
		}

		private void FilterProjectItem(ProjectItemNode node)
		{
			bool flag;
			bool isVisible = node.IsVisible;
			if (!this.VisibilityDictionary.TryGetValue(node, out flag))
			{
				this.VisibilityDictionary.Add(node, isVisible);
				node.VisibilityChanged();
			}
			else if (flag != isVisible)
			{
				this.VisibilityDictionary[node] = isVisible;
				node.VisibilityChanged();
			}
			if (isVisible)
			{
				foreach (ProjectItemNode child in node.Children)
				{
					this.FilterProjectItem(child);
				}
			}
		}

		private void FilterProjects()
		{
			foreach (ProjectNode project in this.Projects)
			{
				this.FilterProject(project);
			}
		}

		private void filterTimer_Tick(object sender, EventArgs e)
		{
			((DispatcherTimer)sender).Stop();
			PerformanceUtility.MeasurePerformanceUntilIdle(PerformanceEvent.FilterProjectPane);
			if (this.searchTransaction == null)
			{
				if (!string.IsNullOrEmpty(this.filterString))
				{
					this.searchTransaction = new Microsoft.Expression.Project.UserInterface.SearchTransaction(this.filterString, this.projectInsertion);
				}
			}
			else if (this.searchTransaction != null)
			{
				this.searchTransaction.UpdateFilterString(this.filterString);
				if (string.IsNullOrEmpty(this.filterString))
				{
					this.searchTransaction = null;
					this.SearchBox.Text = null;
				}
			}
			this.FilterProjects();
			this.flattener.RebuildList(false);
		}

		private void HandleDragOverEvent(DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			bool flag = true;
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] supportedFiles = this.dropUtility.GetSupportedFiles(e.Data);
				int num = 0;
				while (num < (int)supportedFiles.Length)
				{
					if (((new FileInfo(supportedFiles[num])).Attributes & FileAttributes.Directory) == FileAttributes.Directory)
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag && this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.Count<IProject>() == 1)
			{
				e.Effects = DragDropEffects.Move;
			}
			e.Handled = true;
		}

		private void ItemSelectionSet_SelectionChanged(object sender, EventArgs e)
		{
			if (this.RootItem.Children.Count > 0)
			{
				SolutionNode item = this.RootItem.Children[0] as SolutionNode;
				if (item != null)
				{
					item.UpdateSelection();
				}
			}
			foreach (ProjectNode project in this.Projects)
			{
				project.UpdateSelection();
			}
		}

		private void OnClearFilterStringCommand()
		{
			this.FilterString = null;
			IView activeView = this.serviceProvider.ViewService().ActiveView;
			if (activeView != null)
			{
				activeView.ReturnFocus();
			}
		}

		protected override void OnDragLeave(DragEventArgs e)
		{
			this.HandleDragOverEvent(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			this.HandleDragOverEvent(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			SafeDataObject safeDataObject = new SafeDataObject(e.Data);
			if (safeDataObject != null && safeDataObject.GetDataPresent(DataFormats.FileDrop))
			{
				string[] supportedFiles = this.dropUtility.GetSupportedFiles(e.Data);
				List<string> strs = new List<string>();
				string[] strArrays = supportedFiles;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					if (((new FileInfo(str)).Attributes & FileAttributes.Directory) != FileAttributes.Directory)
					{
						strs.Add(str);
					}
				}
				if (strs.Count > 0)
				{
					IProject project = this.Services.ProjectManager().ItemSelectionSet.SelectedProjects.SingleOrNull<IProject>();
					if (project != null)
					{
						project.AddItems(
							from item in strs
							select new DocumentCreationInfo()
							{
								SourcePath = item
							});
					}
				}
			}
		}

		protected override void OnInitialized(EventArgs e)
		{
			if (base.Children.Count == 0)
			{
				FrameworkElement element = FileTable.GetElement("Resources\\ProjectPane.xaml");
				element.DataContext = this;
				base.Children.Add(element);
			}
			base.OnInitialized(e);
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void PersistProjectNodeState(ISolutionManagement solution)
		{
			foreach (ProjectNode projectNode in this.projectInsertion.Children.OfType<ProjectNode>())
			{
				if (projectNode.IsExpanded)
				{
					continue;
				}
				solution.SolutionSettingsManager.SetProjectProperty(projectNode.Project, ProjectPane.IsExpanded, false);
			}
		}

		private void ProjectManager_SolutionClosed(object sender, SolutionEventArgs e)
		{
			this.FilterString = null;
			this.CleanupClosedSolution(e.Solution);
			this.EnableSearch = false;
			this.OnPropertyChanged("RootItem");
		}

		private void ProjectManager_SolutionClosing(object sender, SolutionEventArgs e)
		{
			ISolutionManagement solution = e.Solution as ISolutionManagement;
			if (solution != null)
			{
				this.PersistProjectNodeState(solution);
			}
		}

		private void ProjectManager_SolutionMigrated(object sender, SolutionEventArgs e)
		{
			this.CleanupClosedSolution(e.Solution);
			this.ProjectManager_SolutionOpened(sender, e);
		}

		private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
		{
			ProjectNode projectNode;
			ISolutionManagement solution = e.Solution as ISolutionManagement;
			if (solution != null)
			{
				solution.AnyProjectOpened += new EventHandler<NamedProjectEventArgs>(this.Solution_AnyProjectOpened);
				solution.AnyProjectClosed += new EventHandler<NamedProjectEventArgs>(this.Solution_AnyProjectClosed);
			}
			this.projectInsertion = new SolutionNode(e.Solution, this);
			this.root.AddChild(this.projectInsertion);
			foreach (INamedProject allProject in ((ISolutionManagement)e.Solution).AllProjects)
			{
				IProject project = allProject as IProject;
				if (project == null)
				{
					projectNode = new ProjectNode(allProject, this);
				}
				else
				{
					projectNode = new KnownProjectNode(project, this);
				}
				if (solution != null)
				{
					object projectProperty = solution.SolutionSettingsManager.GetProjectProperty(allProject, ProjectPane.IsExpanded);
					if (projectProperty != null && projectProperty is bool)
					{
						projectNode.IsExpanded = (bool)projectProperty;
					}
					solution.SolutionSettingsManager.ClearProjectProperty(allProject, ProjectPane.IsExpanded);
				}
				this.projectInsertion.AddChild(projectNode);
			}
			this.EnableSearch = true;
			this.OnPropertyChanged("RootItem");
		}

		public void ScrollIntoView(HierarchicalNode node)
		{
			if (this.ViewItemList != null)
			{
				int num = this.ItemList.IndexOf(node);
				if (num >= 0)
				{
					this.ViewItemList.BringIndexIntoViewWorkaround(num);
				}
			}
		}

		private void SelectItemForDocument(IDocument document)
		{
			foreach (IProject project in this.Services.ProjectManager().CurrentSolution.Projects.OfType<IProject>())
			{
				IProjectItem projectItem = project.FindItem(document.DocumentReference);
				if (projectItem == null)
				{
					continue;
				}
				this.Services.ProjectManager().ItemSelectionSet.SetSelection(projectItem);
				break;
			}
		}

		private void Solution_AnyProjectClosed(object sender, NamedProjectEventArgs e)
		{
			for (int i = 0; i < this.projectInsertion.Children.Count; i++)
			{
				ProjectNode item = this.projectInsertion.Children[i] as ProjectNode;
				if (item != null && e.NamedProject == item.Project)
				{
					this.projectInsertion.RemoveChild(item);
					this.OnPropertyChanged("RootItem");
					item.Dispose();
					return;
				}
			}
		}

		private void Solution_AnyProjectOpened(object sender, NamedProjectEventArgs e)
		{
			ProjectNode projectNode;
			try
			{
				IProject namedProject = e.NamedProject as IProject;
				if (namedProject == null)
				{
					projectNode = new ProjectNode(e.NamedProject, this);
				}
				else
				{
					projectNode = new KnownProjectNode(namedProject, this);
				}
				this.projectInsertion.AddChild(projectNode);
			}
			finally
			{
				this.OnPropertyChanged("RootItem");
			}
		}

		private void SourceControlStatusCache_StatusUpdated(object sender, EventArgs args)
		{
			this.flattener.RebuildList(false);
		}

		private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
		{
			IDocumentView newView = e.NewView as IDocumentView;
			if (newView != null)
			{
				this.SelectItemForDocument(newView.Document);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private class ProjectUserInterfaceCommandTarget : CommandTarget
		{
			public ProjectUserInterfaceCommandTarget(ProjectPane projectPane)
			{
				base.AddCommand("Project_RenameProjectItem", new RenameProjectItemCommand(projectPane));
			}
		}
	}
}