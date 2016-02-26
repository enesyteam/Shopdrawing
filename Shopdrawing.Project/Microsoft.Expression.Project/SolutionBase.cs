using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.ServiceExtensions.ErrorHandling;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	internal abstract class SolutionBase : DocumentItemBase, ISolutionManagement, ISolution, IDocumentItem, IDisposable, INotifyPropertyChanged, IFileWatcher
	{
		protected List<INamedProject> projects = new List<INamedProject>();

		private ReadOnlyCollection<INamedProject> readOnlyProjects;

		protected IServiceProvider serviceProvider;

		private IExecutable startupProject;

		private static int disableReloadPrompt;

		private Microsoft.Expression.Project.SolutionWatcher solutionWatcher;

		private ProjectFileInformation solutionFileInformation;

		private bool updateSolutionAndProjects;

		protected bool reloadDocuments;

		private bool disposed;

		protected SolutionBase.ProjectBuildOrderLogger projectBuildOrderLogger = new SolutionBase.ProjectBuildOrderLogger();

		public IEnumerable<INamedProject> AllProjects
		{
			get
			{
				return this.readOnlyProjects;
			}
		}

		public abstract bool CanAddProjects
		{
			get;
		}

		public override IEnumerable<IDocumentItem> Descendants
		{
			get
			{
				return this.Projects.Cast<IDocumentItem>().Concat<IDocumentItem>(this.Projects.SelectMany<IProject, IDocumentItem>((IProject project) => project.Descendants));
			}
		}

		protected IExternalChanges ExternalChanges
		{
			get
			{
				return this.Services.ProjectManager() as IExternalChanges;
			}
		}

		public bool IsClosingAllProjects
		{
			get
			{
				return JustDecompileGenerated_get_IsClosingAllProjects();
			}
			set
			{
				JustDecompileGenerated_set_IsClosingAllProjects(value);
			}
		}

		private bool JustDecompileGenerated_IsClosingAllProjects_k__BackingField;

		public virtual bool JustDecompileGenerated_get_IsClosingAllProjects()
		{
			return this.JustDecompileGenerated_IsClosingAllProjects_k__BackingField;
		}

		private void JustDecompileGenerated_set_IsClosingAllProjects(bool value)
		{
			this.JustDecompileGenerated_IsClosingAllProjects_k__BackingField = value;
		}

		protected bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		public virtual bool IsIssueTrackingAvailable
		{
			get
			{
				return false;
			}
		}

		protected bool IsSafeClose
		{
			get;
			private set;
		}

		public virtual bool IsSourceControlActive
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsUnderSourceControl
		{
			get
			{
				return false;
			}
		}

		public abstract IProjectBuildContext ProjectBuildContext
		{
			get;
		}

		public IEnumerable<IProject> Projects
		{
			get
			{
				if (this.projects == null)
				{
					return null;
				}
				return 
					from project in this.projects
					where project is IProject
					select (IProject)project;
			}
		}

		internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public ProjectFileInformation SolutionFileInformation
		{
			get
			{
				return this.solutionFileInformation;
			}
			set
			{
				this.solutionFileInformation = value;
			}
		}

		public abstract Microsoft.Expression.Project.SolutionSettingsManager SolutionSettingsManager
		{
			get;
		}

		public string SolutionTypeDescription
		{
			get
			{
				return StringTable.ProjectSolutionTypeDescription;
			}
		}

		internal Microsoft.Expression.Project.SolutionWatcher SolutionWatcher
		{
			get
			{
				if (this.IsDisposed)
				{
					return null;
				}
				if (this.solutionWatcher == null)
				{
					this.solutionWatcher = new Microsoft.Expression.Project.SolutionWatcher(this);
				}
				return this.solutionWatcher;
			}
		}

		public IExecutable StartupProject
		{
			get
			{
				if (this.startupProject == null)
				{
					foreach (INamedProject project in this.Projects)
					{
						IProject project1 = project as IProject;
						if (project1 == null || !project1.GetCapability<bool>("CanBeStartupProject"))
						{
							continue;
						}
						this.StartupProject = project1 as IExecutable;
						break;
					}
				}
				return this.startupProject;
			}
			set
			{
				this.startupProject = value;
				((ProjectManager)this.Services.ProjectManager()).OnStartupProjectChanged(EventArgs.Empty);
			}
		}

		static SolutionBase()
		{
		}

		internal SolutionBase(IServiceProvider serviceProvider, Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(documentReference)
		{
			this.serviceProvider = serviceProvider;
			this.readOnlyProjects = new ReadOnlyCollection<INamedProject>(this.projects);
			this.IsClosingAllProjects = false;
			this.IsSafeClose = false;
		}

		protected void AddProject(INamedProject project)
		{
			if (project != null)
			{
				this.projects.Add(project);
				this.OnProjectAdded(new NamedProjectEventArgs(project));
			}
		}

		public virtual void AddProjectOutputReferences(IEnumerable<INamedProject> createdProjects)
		{
		}

		public void CheckDelayedChange()
		{
			if (!this.ExternalChanges.IsDelayed && this.ShouldProcessDelayedChange())
			{
				UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.ProcessDelayedChange));
			}
		}

		public virtual void CheckForChangedOrDeletedItems()
		{
			this.updateSolutionAndProjects = true;
			if (this.IsDisposed || this.ExternalChanges.IsDelayed)
			{
				return;
			}
			using (IDisposable disposable = this.ExternalChanges.DelayNotification())
			{
				this.RefreshSolution();
				List<UnknownProject> unknownProjects = new List<UnknownProject>();
				foreach (INamedProject allProject in this.AllProjects)
				{
					UnknownProject unknownProject = allProject as UnknownProject;
					if (unknownProject == null || !unknownProject.ProjectFileInformation.HasComeIntoExistence() && !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(unknownProject.DocumentReference.Path))
					{
						continue;
					}
					unknownProjects.Add(unknownProject);
				}
				foreach (UnknownProject unknownProject1 in unknownProjects)
				{
					this.ReloadProject(unknownProject1);
				}
				this.updateSolutionAndProjects = false;
			}
		}

		public void Close(bool isUserInitiated)
		{
			this.IsSafeClose = isUserInitiated;
			this.Dispose(true);
		}

		private void CloseAllProjects()
		{
			this.IsClosingAllProjects = true;
			for (int i = this.projects.Count - 1; i >= 0; i--)
			{
				this.CloseProject(this.projects[i]);
			}
			this.IsClosingAllProjects = false;
		}

		private void CloseProject(INamedProject project)
		{
			IProjectItem projectItem2 = null;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectClose);
			try
			{
				IProject project1 = project as IProject;
				if (project1 != null)
				{
					((ProjectManager)this.Services.ProjectManager()).OnProjectClosing(new ProjectEventArgs(project1));
				}
				ItemSelectionSet itemSelectionSet = this.Services.ProjectManager().ItemSelectionSet;
				itemSelectionSet.RemoveSelection((IDocumentItem selection) => {
					IProjectItem projectItem = selection as IProjectItem;
					IProjectItem projectItem1 = projectItem;
					projectItem2 = projectItem;
					if (projectItem1 == null)
					{
						return false;
					}
					return projectItem2.Project == project;
				});
				itemSelectionSet.RemoveSelection(project);
				((ProjectManager)this.Services.ProjectManager()).UpdateCurrentWorkingDirectory(null);
				project.Dispose();
			}
			finally
			{
				this.RemoveProject(project);
				if (this.StartupProject == project)
				{
					this.StartupProject = null;
				}
				this.OnAnyProjectClosed(new NamedProjectEventArgs(project));
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectClose);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.IsDisposed)
			{
				try
				{
					if (this.solutionWatcher != null)
					{
						this.solutionWatcher.DisableWatchingForChanges();
					}
					this.Services.ProjectManager().ItemSelectionSet.Clear();
					this.CloseAllProjects();
				}
				finally
				{
					if (this.solutionWatcher != null)
					{
						this.solutionWatcher.Dispose();
						this.solutionWatcher = null;
					}
				}
				this.projects = null;
				this.disposed = true;
			}
		}

		public INamedProject FindProject(Microsoft.Expression.Framework.Documents.DocumentReference projectReference)
		{
			if (projectReference == null || this.projects == null)
			{
				return null;
			}
			return this.projects.FirstOrDefault<INamedProject>((INamedProject p) => projectReference.Path == p.DocumentReference.Path);
		}

		public INamedProject FindProjectByName(string projectName)
		{
			INamedProject namedProject = null;
			foreach (INamedProject project in this.Projects)
			{
				if (string.CompareOrdinal(project.DocumentReference.DisplayName, projectName) != 0)
				{
					continue;
				}
				namedProject = project;
				break;
			}
			return namedProject;
		}

		public IProject FindProjectContainingItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			IProject project = null;
			foreach (IProject project1 in this.Projects)
			{
				if (project1.FindItem(documentReference) == null)
				{
					continue;
				}
				project = project1;
				break;
			}
			return project;
		}

		public IProject FindProjectContainingOpenItem(Microsoft.Expression.Framework.Documents.DocumentReference documentReference)
		{
			bool flag = false;
			foreach (IView view in this.Services.ViewService().Views)
			{
				DocumentView documentView = view as DocumentView;
				if (documentView == null || documentView.Document == null || StringComparer.OrdinalIgnoreCase.Compare(documentReference.Path, documentView.Document.DocumentReference.Path) != 0)
				{
					continue;
				}
				flag = true;
				break;
			}
			if (!flag)
			{
				return null;
			}
			return this.FindProjectContainingItem(documentReference);
		}

		private ICodeDocumentType GetCodeDocumentTypeFromProject(IProjectStore projectStore)
		{
			ICodeDocumentType codeDocumentType;
			string extension = Path.GetExtension(projectStore.DocumentReference.Path);
			using (IEnumerator<IDocumentType> enumerator = this.Services.DocumentTypes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ICodeDocumentType current = enumerator.Current as ICodeDocumentType;
					if (current == null || StringComparer.OrdinalIgnoreCase.Compare(extension, current.ProjectFileExtension) != 0)
					{
						continue;
					}
					codeDocumentType = current;
					return codeDocumentType;
				}
				return null;
			}
			return codeDocumentType;
		}

		internal static IEnumerable<IProjectItem> GetDirtyProjectItems(IEnumerable<IProject> projects)
		{
			return (
				from IProject project in projects
				from IProjectItem projectItem in project.Items
				select new { project = project, projectItem = projectItem }).Where((argument0) => {
				if (!argument0.projectItem.IsOpen)
				{
					return false;
				}
				return argument0.projectItem.IsDirty;
			}).Select((argument1) => argument1.projectItem);
		}

		protected INamedProject InitializeProject(IProjectStore projectStore)
		{
			INamedProject namedProject;
			INamedProject unlicensedProject = null;
			try
			{
				IProjectType projectTypeForProject = this.Services.ProjectTypeManager().GetProjectTypeForProject(projectStore);
				if (projectTypeForProject != null)
				{
					IProjectCreateError projectCreateError = projectTypeForProject.CanCreateProject(projectStore);
					if (projectCreateError != null)
					{
						projectTypeForProject = this.Services.ProjectTypeManager().UnknownProjectType;
					}
					if (projectTypeForProject is UnknownProjectType && SolutionBase.IsReloadPromptEnabled())
					{
						InvalidProjectStore invalidProjectStore = projectStore as InvalidProjectStore;
						if (invalidProjectStore == null || string.IsNullOrEmpty(invalidProjectStore.InvalidStateDescription))
						{
							this.PromptWithUnsupportedProjectDetails(projectStore, projectCreateError);
						}
						else
						{
							IMessageDisplayService messageDisplayService = this.Services.MessageDisplayService();
							ErrorArgs errorArg = new ErrorArgs();
							CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
							string unsupportedProjectWithDescription = StringTable.UnsupportedProjectWithDescription;
							object[] displayName = new object[] { projectStore.DocumentReference.DisplayName, invalidProjectStore.InvalidStateDescription };
							errorArg.Message = string.Format(currentUICulture, unsupportedProjectWithDescription, displayName);
							errorArg.AutomationId = "OpenProjectErrorDialog";
							messageDisplayService.ShowError(errorArg);
						}
					}
					LicenseState licenseState = LicensingHelper.IsProjectLicensed(projectStore, this.serviceProvider);
					if (!licenseState.IsExpired)
					{
						if (!licenseState.FullyLicensed)
						{
							LicensingHelper.UnlicensedProjectLoadAttempted();
						}
						unlicensedProject = projectTypeForProject.CreateProject(projectStore, this.GetCodeDocumentTypeFromProject(projectStore), this.serviceProvider);
					}
					else
					{
						LicensingHelper.UnlicensedProjectLoadAttempted();
						unlicensedProject = new UnlicensedProject(projectStore, this.serviceProvider);
					}
					return unlicensedProject;
				}
				else
				{
					namedProject = null;
				}
			}
			catch (Exception exception)
			{
				if (unlicensedProject != null)
				{
					projectStore.Dispose();
					unlicensedProject.Dispose();
					unlicensedProject = null;
				}
				throw;
			}
			return namedProject;
		}

		internal static bool IsReloadPromptEnabled()
		{
			return SolutionBase.disableReloadPrompt <= 0;
		}

		public bool Load()
		{
			bool flag;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SolutionLoad);
			try
			{
				Microsoft.Expression.Project.SolutionWatcher solutionWatcher = this.SolutionWatcher;
				flag = this.LoadInternal();
			}
			finally
			{
				PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SolutionLoad);
			}
			return flag;
		}

		protected abstract bool LoadInternal();

		void Microsoft.Expression.Project.IFileWatcher.Deactivate()
		{
			if (this.solutionWatcher != null)
			{
				this.solutionWatcher.DisableWatchingForChanges();
			}
		}

		void Microsoft.Expression.Project.IFileWatcher.Reactivate()
		{
			Microsoft.Expression.Project.SolutionWatcher solutionWatcher = this.SolutionWatcher;
			if (solutionWatcher != null)
			{
				solutionWatcher.CheckForChangesAndReenable();
			}
		}

		INamedProject Microsoft.Expression.Project.ISolutionManagement.AddProject(IProjectStore projectStore)
		{
			return this.OpenProject(projectStore);
		}

		void Microsoft.Expression.Project.ISolutionManagement.CloseAllProjects()
		{
			this.CloseAllProjects();
		}

		void Microsoft.Expression.Project.ISolutionManagement.CloseProject(INamedProject project)
		{
			this.CloseProject(project);
		}

		void Microsoft.Expression.Project.ISolutionManagement.DeactivateWatchers()
		{
			foreach (IFileWatcher fileWatcher in this.Projects.OfType<IFileWatcher>())
			{
				fileWatcher.Deactivate();
			}
			((IFileWatcher)this).Deactivate();
		}

		void Microsoft.Expression.Project.ISolutionManagement.ReactivateWatchers()
		{
			((IFileWatcher)this).Reactivate();
			if (this.Projects == null || this.Projects.CountIs<IProject>(0))
			{
				return;
			}
			IProject[] array = this.Projects.ToArray<IProject>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				IFileWatcher fileWatcher = array[i] as IFileWatcher;
				if (fileWatcher != null)
				{
					fileWatcher.Reactivate();
				}
			}
			if (!BuildManager.Building)
			{
				AssemblyService assemblyService = (AssemblyService)this.serviceProvider.AssemblyService();
				if (assemblyService == null)
				{
					return;
				}
				List<string> strs = assemblyService.UpdateCacheWithExternalChanges();
				if (strs.Count > 0)
				{
					foreach (MSBuildBasedProject mSBuildBasedProject in this.Projects.OfType<MSBuildBasedProject>())
					{
						mSBuildBasedProject.UpdateAssemblyReferences(strs);
					}
				}
			}
		}

		bool Microsoft.Expression.Project.ISolutionManagement.Save(bool promptBeforeSaving, bool saveActiveDocument)
		{
			return this.Save(promptBeforeSaving, saveActiveDocument);
		}

		void Microsoft.Expression.Project.ISolutionManagement.SaveCopy(string newRootPath, string newSolutionName)
		{
			this.SaveCopy(newRootPath, newSolutionName);
		}

		protected static void MoveProjects(SolutionBase from, SolutionBase to)
		{
			for (int i = from.projects.Count - 1; i >= 0; i--)
			{
				INamedProject item = from.projects[i];
				from.RemoveProject(item);
				to.AddProject(item);
			}
		}

		private void OnAnyProjectClosed(NamedProjectEventArgs e)
		{
			if (this.AnyProjectClosed != null)
			{
				this.AnyProjectClosed(this, e);
			}
			IProject namedProject = e.NamedProject as IProject;
			if (namedProject != null)
			{
				this.OnProjectClosed(new ProjectEventArgs(namedProject));
			}
		}

		private void OnAnyProjectOpened(NamedProjectEventArgs e)
		{
			if (this.AnyProjectOpened != null)
			{
				this.AnyProjectOpened(this, e);
			}
			IProject namedProject = e.NamedProject as IProject;
			if (namedProject != null)
			{
				this.OnProjectOpened(new ProjectEventArgs(namedProject));
			}
		}

		protected virtual void OnProjectAdded(NamedProjectEventArgs e)
		{
			IProject namedProject = e.NamedProject as IProject;
			if (namedProject != null)
			{
				namedProject.ProjectChanged += new EventHandler<ProjectEventArgs>(this.Project_ProjectChanged);
			}
		}

		internal void OnProjectClosed(ProjectEventArgs e)
		{
			((ProjectManager)this.Services.ProjectManager()).OnProjectClosed(e);
		}

		private void OnProjectOpened(ProjectEventArgs e)
		{
			((ProjectManager)this.Services.ProjectManager()).OnProjectOpened(e);
		}

		protected virtual void OnProjectRemoved(NamedProjectEventArgs e)
		{
			IProject namedProject = e.NamedProject as IProject;
			if (namedProject != null)
			{
				namedProject.ProjectChanged -= new EventHandler<ProjectEventArgs>(this.Project_ProjectChanged);
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OpenInitialScene()
		{
			IProject startupProject = this.StartupProject as IProject;
			if (startupProject == null || startupProject is IWebsiteProject || startupProject is WebApplicationProject)
			{
				return;
			}
			if (startupProject.StartupItem != null)
			{
				if (this.Services.ViewService().Views.OfType<IDocumentView>().FirstOrDefault<IDocumentView>((IDocumentView documentView) => documentView.Document.DocumentReference == startupProject.StartupItem.DocumentReference) != null)
				{
					return;
				}
			}
			IProjectItem startupItem = startupProject.StartupItem;
			if (startupItem == null || !startupItem.DocumentType.CanView)
			{
				foreach (IProjectItem item in startupProject.Items)
				{
					if (item.DocumentType != ((ProjectManager)this.Services.ProjectManager()).KnownDocumentTypes.SceneDocumentType || !item.DocumentType.CanView)
					{
						continue;
					}
					startupItem = item;
					break;
				}
			}
			if (startupItem != null)
			{
				startupItem.OpenView(true);
			}
		}

		public virtual void OpenInitialViews()
		{
			this.OpenInitialScene();
		}

		protected INamedProject OpenProject(IProjectStore projectStore)
		{
			INamedProject namedProject = null;
			ErrorHandling.HandleBasicExceptions(() => namedProject = this.InitializeProject(projectStore), (Exception exception) => {
				this.Services.MessageDisplayService().ShowError(string.Format(CultureInfo.CurrentCulture, StringTable.DialogFailedMessage, new object[] { string.Format(CultureInfo.CurrentCulture, StringTable.DialogOpenFailedMessage, new object[] { projectStore.DocumentReference.DisplayName }), exception.Message }));
				namedProject = null;
			});
			if (namedProject != null)
			{
				try
				{
					this.OpenProjectInternal(namedProject);
				}
				catch
				{
					namedProject.Dispose();
					namedProject = null;
					throw;
				}
			}
			return namedProject;
		}

		protected void OpenProjectInternal(INamedProject project)
		{
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectManagerOpenProject, "projects.Add");
			this.AddProject(project);
			this.OnAnyProjectOpened(new NamedProjectEventArgs(project));
		}

		protected virtual void PersistSolutionSettings()
		{
		}

		private void ProcessDelayedChange()
		{
			if (this.ExternalChanges.IsDelayed)
			{
				return;
			}
			if (this.updateSolutionAndProjects)
			{
				this.CheckForChangedOrDeletedItems();
			}
			foreach (INamedProject allProject in this.AllProjects)
			{
				KnownProjectBase knownProjectBase = allProject as KnownProjectBase;
				if (knownProjectBase == null)
				{
					continue;
				}
				knownProjectBase.UpdateAssembliesAndChangedItemsIfPossible();
			}
		}

		private void Project_ProjectChanged(object o, ProjectEventArgs e)
		{
			MessageBoxResult messageBoxResult;
			if (this.IsDisposed)
			{
				return;
			}
			if (e.Project == null)
			{
				return;
			}
			string projectChangedDialogMessage = StringTable.ProjectChangedDialogMessage;
			string path = e.Project.DocumentReference.Path;
			if (!SolutionBase.IsReloadPromptEnabled())
			{
				messageBoxResult = MessageBoxResult.Yes;
			}
			else
			{
				MessageBoxArgs messageBoxArg = new MessageBoxArgs();
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				object[] shortApplicationName = new object[] { path, this.Services.ExpressionInformationService().ShortApplicationName };
				messageBoxArg.Message = string.Format(currentCulture, projectChangedDialogMessage, shortApplicationName);
				messageBoxArg.Button = MessageBoxButton.YesNo;
				messageBoxArg.Image = MessageBoxImage.Exclamation;
				MessageBoxArgs messageBoxArg1 = messageBoxArg;
				messageBoxResult = this.Services.MessageDisplayService().ShowMessage(messageBoxArg1);
			}
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				IProject project = e.Project;
				if (project != null && this.Save(false) && this.RefreshProject(project) && this.Services.ProjectManager().ItemSelectionSet.Count == 0)
				{
					this.Services.ProjectManager().ItemSelectionSet.SetSelection(this);
				}
			}
		}

		private void PromptWithUnsupportedProjectDetails(IProjectStore projectStore, IProjectCreateError projectCreateError)
		{
			string invalidStoreUnsupportedError = StringTable.InvalidStoreUnsupportedError;
			string identifier = null;
			if (projectStore is InvalidProjectStore)
			{
				string safeExtension = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(projectStore.DocumentReference.Path);
				if (!string.IsNullOrEmpty(safeExtension))
				{
					identifier = string.Concat("DoNotWarnAboutExtension", safeExtension.ToUpperInvariant());
				}
			}
			else if (projectCreateError != null)
			{
				invalidStoreUnsupportedError = projectCreateError.Message;
				identifier = projectCreateError.Identifier;
			}
			else if (projectStore.StoreVersion != CommonVersions.Version4_0)
			{
				CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
				string invalidStoreBadToolsVersion = StringTable.InvalidStoreBadToolsVersion;
				object[] objArray = new object[] { "4.0" };
				invalidStoreUnsupportedError = string.Format(currentUICulture, invalidStoreBadToolsVersion, objArray);
				identifier = "DoNotWarnAboutBadToolsVersion";
			}
			else if (!ProjectStoreHelper.IsKnownLanguage(projectStore))
			{
				invalidStoreUnsupportedError = StringTable.InvalidStoreUnknownLanguage;
				identifier = "DoNotWarnAboutUnknownLanguage";
			}
			else if (!WindowsExecutableProjectType.IsSupportedOutputType(projectStore))
			{
				invalidStoreUnsupportedError = StringTable.InvalidStoreUnsupportedOutputType;
				identifier = "DoNotWarnAboutUnsupportedOutputType";
			}
			else if (ProjectStoreHelper.IsSketchFlowProject(projectStore))
			{
				FrameworkName targetFrameworkName = ProjectStoreHelper.GetTargetFrameworkName(projectStore);
				if (targetFrameworkName == null || targetFrameworkName.Version != CommonVersions.Version4_0)
				{
					CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
					string invalidStoreUnsupportedSketchflow = StringTable.InvalidStoreUnsupportedSketchflow;
					object[] objArray1 = new object[] { (targetFrameworkName.Identifier == "Silverlight" ? "Silverlight 4" : ".NET Framework 4.0") };
					invalidStoreUnsupportedError = string.Format(cultureInfo, invalidStoreUnsupportedSketchflow, objArray1);
					identifier = "DoNotWarnAboutUnsupportedSketchflowProject";
				}
			}
			CultureInfo currentUICulture1 = CultureInfo.CurrentUICulture;
			string unsupportedProjectWithDescription = StringTable.UnsupportedProjectWithDescription;
			object[] displayName = new object[] { projectStore.DocumentReference.DisplayName, invalidStoreUnsupportedError };
			string str = string.Format(currentUICulture1, unsupportedProjectWithDescription, displayName);
			if (string.IsNullOrEmpty(identifier))
			{
				this.Services.MessageDisplayService().ShowError(str);
				return;
			}
			IServiceProvider services = this.Services;
			MessageBoxArgs messageBoxArg = new MessageBoxArgs()
			{
				Button = MessageBoxButton.OK,
				Image = MessageBoxImage.Exclamation,
				Message = str,
				AutomationId = "UnsupportedProjectErrorDialog"
			};
			services.ShowSuppressibleWarning(messageBoxArg, identifier, MessageBoxResult.OK);
		}

		public bool RefreshProject(IProject project)
		{
			KnownProjectBase knownProjectBase = project as KnownProjectBase;
			if (knownProjectBase == null)
			{
				return false;
			}
			bool flag = false;
			ErrorHandling.HandleBasicExceptions(() => {
				knownProjectBase.Refresh();
				flag = true;
			}, (Exception exception) => {
				this.ReloadProject(project);
				this.OpenInitialViews();
			});
			return flag;
		}

		private void RefreshSolution()
		{
			MessageBoxResult messageBoxResult;
			if (this.SolutionFileInformation != null && this.SolutionFileInformation.HasChanged())
			{
				string solutionChangedDialogMessage = StringTable.SolutionChangedDialogMessage;
				string path = base.DocumentReference.Path;
				if (!SolutionBase.IsReloadPromptEnabled())
				{
					messageBoxResult = MessageBoxResult.Yes;
				}
				else
				{
					MessageBoxArgs messageBoxArg = new MessageBoxArgs();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					object[] shortApplicationName = new object[] { path, this.Services.ExpressionInformationService().ShortApplicationName };
					messageBoxArg.Message = string.Format(currentCulture, solutionChangedDialogMessage, shortApplicationName);
					messageBoxArg.Button = MessageBoxButton.YesNo;
					messageBoxArg.Image = MessageBoxImage.Exclamation;
					MessageBoxArgs messageBoxArg1 = messageBoxArg;
					messageBoxResult = this.Services.MessageDisplayService().ShowMessage(messageBoxArg1);
				}
				if (messageBoxResult == MessageBoxResult.Yes && this.Save(false))
				{
					this.Services.ExceptionHandler(() => {
						this.CloseAllProjects();
						((ProjectManager)this.Services.ProjectManager()).OnSolutionClosed(new SolutionEventArgs(this));
						this.LoadInternal();
						((ProjectManager)this.Services.ProjectManager()).OnSolutionOpened(new SolutionEventArgs(this));
						this.reloadDocuments = true;
						if (this.IsUnderSourceControl)
						{
							ISourceControlProvider sourceControlProvider = this.Services.SourceControlProvider();
							if (sourceControlProvider != null)
							{
								sourceControlProvider.OpenProject(base.DocumentReference.Path, false);
							}
						}
						if (this.Services.ProjectManager().ItemSelectionSet.Count == 0)
						{
							this.Services.ProjectManager().ItemSelectionSet.SetSelection(this);
						}
					}, () => string.Format(CultureInfo.CurrentCulture, StringTable.DialogRefreshFailedMessage, new object[] { base.DocumentReference.Path }));
				}
				this.UpdateFileInformation();
			}
		}

		private void ReloadProject(INamedProject project)
		{
			IProjectStore projectStore = null;
			try
			{
				this.CloseProject(project);
				((ProjectManager)this.Services.ProjectManager()).OnSolutionClosed(new SolutionEventArgs(this));
				projectStore = ProjectStoreHelper.CreateProjectStore(project.DocumentReference, this.serviceProvider, ProjectStoreHelper.DefaultProjectCreationChain);
				this.OpenProject(projectStore);
				((ProjectManager)this.Services.ProjectManager()).OnSolutionOpened(new SolutionEventArgs(this));
			}
			catch
			{
				if (projectStore != null)
				{
					projectStore.Dispose();
				}
				throw;
			}
		}

		protected void RemoveProject(INamedProject project)
		{
			if (project != null && this.projects.Contains(project))
			{
				this.projects.Remove(project);
				this.OnProjectRemoved(new NamedProjectEventArgs(project));
			}
		}

		public bool Save(bool promptBeforeSaving)
		{
			return this.Save(promptBeforeSaving, true);
		}

		private bool Save(bool promptBeforeSaving, bool saveActiveDocument)
		{
			if (!this.SaveProjectItems(promptBeforeSaving, saveActiveDocument))
			{
				return false;
			}
			this.PersistSolutionSettings();
			return true;
		}

		protected virtual void SaveCopy(string newRootPath, string newSolutionName)
		{
			string path = base.DocumentReference.Path;
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(path))
			{
				path = Path.GetDirectoryName(path);
			}
			ProjectManager.CopyDirectory(path, Path.Combine(newRootPath, newSolutionName));
		}

		private bool SaveProjectItems(bool promptBeforeSaving, bool saveActiveDocument)
		{
			Func<IProjectItem, Microsoft.Expression.Framework.Documents.DocumentReference> documentReference = null;
			Func<IProjectItem, bool> func = null;
			bool flag = true;
			ErrorHandling.HandleBasicExceptions(() => {
				IEnumerable<IProjectItem> dirtyProjectItems = SolutionBase.GetDirtyProjectItems(this.Projects);
				if (!saveActiveDocument)
				{
					IDocumentView activeView = this.Services.ViewService().ActiveView as IDocumentView;
					if (activeView != null)
					{
						IDocument document = activeView.Document;
						dirtyProjectItems = 
							from item in dirtyProjectItems
							where item.Document != document
							select item;
					}
				}
				if (dirtyProjectItems.Any<IProjectItem>())
				{
					if (promptBeforeSaving)
					{
						SaveFilesDialog saveFilesDialog = new SaveFilesDialog(this.Services, dirtyProjectItems);
						saveFilesDialog.InitializeDialog();
						saveFilesDialog.ShowDialog();
						if (saveFilesDialog.Result == ProjectDialog.ProjectDialogResult.Cancel)
						{
							flag = false;
							return;
						}
						if (saveFilesDialog.Result == ProjectDialog.ProjectDialogResult.Discard)
						{
							flag = true;
							return;
						}
					}
					if (this.IsSourceControlActive)
					{
						IEnumerable<IProjectItem> projectItems = dirtyProjectItems;
						if (documentReference == null)
						{
							documentReference = (IProjectItem item) => item.DocumentReference;
						}
						SourceControlHelper.UpdateSourceControl(projectItems.Select<IProjectItem, Microsoft.Expression.Framework.Documents.DocumentReference>(documentReference), UpdateSourceControlActions.Checkout, this.Services);
					}
					IEnumerable<IProjectItem> projectItems1 = dirtyProjectItems;
					if (func == null)
					{
						func = (IProjectItem item) => !item.SaveDocumentFile();
					}
					flag = !projectItems1.Any<IProjectItem>(func);
				}
			}, (Exception exception) => this.Services.ShowErrorMessage(StringTable.SaveAllFailedDialogMessage, exception));
			return flag;
		}

		private bool ShouldProcessDelayedChange()
		{
			bool flag;
			if (this.updateSolutionAndProjects)
			{
				return true;
			}
			using (IEnumerator<INamedProject> enumerator = this.AllProjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KnownProjectBase current = enumerator.Current as KnownProjectBase;
					if (current == null || !current.ShouldProcessDelayedChange())
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

		protected void UpdateFileInformation()
		{
			this.SolutionFileInformation = new ProjectFileInformation(base.DocumentReference.Path);
		}

		public event EventHandler<NamedProjectEventArgs> AnyProjectClosed;

		public event EventHandler<NamedProjectEventArgs> AnyProjectOpened;

		public event PropertyChangedEventHandler PropertyChanged;

		public sealed class DisableReloadPromptToken : IDisposable
		{
			public DisableReloadPromptToken()
			{
				SolutionBase.disableReloadPrompt = SolutionBase.disableReloadPrompt + 1;
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			public void Dispose(bool disposing)
			{
				if (disposing)
				{
					SolutionBase.disableReloadPrompt = SolutionBase.disableReloadPrompt - 1;
				}
			}
		}

		protected class ProjectBuildOrderLogger : ILogger
		{
			private List<string> projectOrder;

			private IEventSource eventSource;

			string Microsoft.Build.Framework.ILogger.Parameters
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			LoggerVerbosity Microsoft.Build.Framework.ILogger.Verbosity
			{
				get
				{
					return LoggerVerbosity.Normal;
				}
				set
				{
				}
			}

			internal List<string> ProjectOrder
			{
				get
				{
					return this.projectOrder;
				}
			}

			public ProjectBuildOrderLogger()
			{
			}

			private void EventSource_BuildFinished(object sender, BuildFinishedEventArgs e)
			{
				if (this.eventSource != null)
				{
					this.eventSource.ProjectFinished -= new ProjectFinishedEventHandler(this.EventSource_ProjectFinished);
					this.eventSource.BuildFinished -= new BuildFinishedEventHandler(this.EventSource_BuildFinished);
					this.eventSource = null;
				}
			}

			private void EventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
			{
				if (!this.projectOrder.Contains(e.ProjectFile) && e.Succeeded)
				{
					this.projectOrder.Add(e.ProjectFile);
				}
			}

			void Microsoft.Build.Framework.ILogger.Initialize(IEventSource eventSource)
			{
				eventSource.ProjectFinished += new ProjectFinishedEventHandler(this.EventSource_ProjectFinished);
				eventSource.BuildFinished += new BuildFinishedEventHandler(this.EventSource_BuildFinished);
				this.eventSource = eventSource;
			}

			void Microsoft.Build.Framework.ILogger.Shutdown()
			{
			}
		}

		protected struct SolutionProperty
		{
			public string Name;

			public object Value;

			public SolutionProperty(string name, object value)
			{
				this.Name = name;
				this.Value = value;
			}

			public T GetValueAs<T>()
			{
				try
				{
					T t = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(this.Value);
					return t;
				}
				catch (NotSupportedException notSupportedException)
				{
				}
				catch (InvalidCastException invalidCastException)
				{
				}
				return default(T);
			}
		}
	}
}