using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Commands;
using Microsoft.Expression.Project.Conversion;
using Microsoft.Expression.Project.Licensing;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	internal sealed class ProjectManager : CommandTarget, IProjectManager, IExternalChanges
	{
		private const string LastImportSpecialFolderName = "LastImportFolder";

		private const string LastOpenProjectSpecialFolderName = "LastOpenProjectFolder";

		private const string LastNewProjectSpecialFolderName = "LastNewProjectFolder";

		private const string LastSampleSpecialFolderName = "LastSampleProjectFolder";

		internal const string BaseUserFolder = "Expression\\Blend 4";

		private const string RecentProjectsConfigurationString = "RecentProjects";

		private IConfigurationObject configuration;

		private SpecialFolderManager defaultImportFolder;

		private SpecialFolderManager defaultNewProjectFolder;

		private SpecialFolderManager defaultOpenProjectFolder;

		private SpecialFolderManager defaultSampleProjectFolder;

		private ISolutionManagement currentSolution;

		private Dictionary<Assembly, string> implicitAssemblyReferences = new Dictionary<Assembly, string>();

		internal static string NewProjectDefaultProjectName;

		private readonly static string DefaultProjectFolder;

		private readonly static string DefaultSampleFolder;

		private Microsoft.Expression.Project.KnownDocumentTypes knownDocumentTypes;

		private IServiceProvider serviceProvider;

		private bool projectConfigurationInitialized;

		private Microsoft.Expression.Project.Build.BuildManager buildManager;

		private ITemplateManager templateManager;

		private ProjectManager.CaseInsensitiveStringList recentProjects = new ProjectManager.CaseInsensitiveStringList();

		private Microsoft.Expression.Project.ItemSelectionSet itemSelectionSet;

		private bool applicationIsActive;

		private int delayNotificationCount;

		private ProjectSystemOptionsModel optionsModel = new ProjectSystemOptionsModel();

		private string initialWorkingDirectory;

		public IProjectBuildContext ActiveBuildTarget
		{
			get
			{
				if (this.CurrentSolution == null)
				{
					return null;
				}
				return this.CurrentSolution.ProjectBuildContext;
			}
		}

		public Microsoft.Expression.Project.Build.BuildManager BuildManager
		{
			get
			{
				return this.buildManager;
			}
		}

		public ISolution CurrentSolution
		{
			get
			{
				return JustDecompileGenerated_get_CurrentSolution();
			}
			set
			{
				JustDecompileGenerated_set_CurrentSolution(value);
			}
		}

		public ISolution JustDecompileGenerated_get_CurrentSolution()
		{
			return this.currentSolution;
		}

		public void JustDecompileGenerated_set_CurrentSolution(ISolution value)
		{
			this.currentSolution = (ISolutionManagement)value;
		}

		public ICodeDocumentType DefaultCodeDocumentType
		{
			get
			{
				return this.knownDocumentTypes.DefaultCodeDocumentType;
			}
		}

		public string DefaultImportPath
		{
			get
			{
				return this.defaultImportFolder.Path;
			}
			set
			{
				this.defaultImportFolder.Path = value;
			}
		}

		public string DefaultNewProjectPath
		{
			get
			{
				return this.defaultNewProjectFolder.Path;
			}
			set
			{
				this.defaultNewProjectFolder.Path = value;
			}
		}

		public string DefaultNewSamplePath
		{
			get
			{
				return this.defaultSampleProjectFolder.Path;
			}
			set
			{
				this.defaultSampleProjectFolder.Path = value;
			}
		}

		public string DefaultOpenProjectPath
		{
			get
			{
				return this.defaultOpenProjectFolder.Path;
			}
			set
			{
				this.defaultOpenProjectFolder.Path = value;
			}
		}

		public Microsoft.Expression.Framework.IReadOnlyCollection<Assembly> ImplicitAssemblyReferences
		{
			get
			{
				return new ReadOnlyList<Assembly>(this.implicitAssemblyReferences.Keys.ToList<Assembly>());
			}
		}

		public Microsoft.Expression.Project.ItemSelectionSet ItemSelectionSet
		{
			get
			{
				return this.itemSelectionSet;
			}
		}

		internal Microsoft.Expression.Project.KnownDocumentTypes KnownDocumentTypes
		{
			get
			{
				return this.knownDocumentTypes;
			}
		}

		internal static int MaximumRecentProjectsCount
		{
			get
			{
				return 10;
			}
		}

		bool Microsoft.Expression.Framework.IExternalChanges.IsDelayed
		{
			get
			{
				return this.delayNotificationCount > 0;
			}
		}

		public ProjectSystemOptionsModel OptionsModel
		{
			get
			{
				return this.optionsModel;
			}
		}

		public string[] RecentProjects
		{
			get
			{
				return this.recentProjects.ToArray();
			}
		}

		internal IServiceProvider Services
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public ITemplateManager TemplateManager
		{
			get
			{
				return this.templateManager;
			}
		}

		static ProjectManager()
		{
			ProjectManager.NewProjectDefaultProjectName = "UntitledProject";
			ProjectManager.DefaultProjectFolder = Path.Combine("Expression\\Blend 4", "Projects");
			ProjectManager.DefaultSampleFolder = Path.Combine("Expression\\Blend 4", "Samples");
		}

		private ProjectManager()
		{
		}

		public ProjectManager(IServiceProvider serviceProvider, IConfigurationObject configuration)
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectManagerConstructor);
			this.serviceProvider = serviceProvider;
			IExpressionMefHostingService service = this.serviceProvider.GetService<IExpressionMefHostingService>();
			this.initialWorkingDirectory = Environment.CurrentDirectory;
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectManagerConstructor, "KnownDocumentTypes");
			this.knownDocumentTypes = new Microsoft.Expression.Project.KnownDocumentTypes(this.Services.DocumentTypeManager());
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectManagerConstructor, "InitializeSpecialFolderManagers");
			this.configuration = configuration;
			this.InitializeSpecialFolderManagers();
			this.buildManager = new Microsoft.Expression.Project.Build.BuildManager(serviceProvider);
			this.buildManager.BuildStarting += new EventHandler<BuildStartingEventArgs>(this.BuildManager_BuildStarting);
			this.Services.DocumentService().DocumentClosed += new DocumentEventHandler(this.DocumentManager_DocumentClosed);
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectManagerConstructor, "InitializeCommands");
			this.InitializeCommands();
			this.templateManager = new Microsoft.Expression.Project.TemplateManager(this.Services);
			if (service != null)
			{
				service.AddInternalPart(this.templateManager);
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectManagerConstructor);
			this.itemSelectionSet = new Microsoft.Expression.Project.ItemSelectionSet();
			if (Application.Current != null)
			{
				this.applicationIsActive = true;
				Application.Current.Activated += new EventHandler(this.CurrentApplication_Activated);
				Application.Current.Deactivated += new EventHandler(this.CurrentApplication_Deactivated);
			}
			ISourceControlService sourceControlService = this.Services.SourceControlService();
			if (sourceControlService != null)
			{
				sourceControlService.SetLoggingInformation(StringTable.CheckResultsPanelMessage);
			}
		}

		public void AddImplicitAssemblyReference(Assembly assembly, string originalAssemblyLocation)
		{
			if (!this.implicitAssemblyReferences.ContainsKey(assembly))
			{
				this.implicitAssemblyReferences[assembly] = originalAssemblyLocation;
				UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.OnIsDelayedChanged));
			}
		}

		public INamedProject AddProject(IProjectStore projectStore)
		{
			if (!LicensingHelper.IsProjectLicensed(projectStore, this.serviceProvider).FullyLicensed)
			{
				LicensingHelper.UnlicensedProjectLoadAttempted();
				LicensingHelper.ShowLicensingDialogIfNecessary(this.serviceProvider);
			}
			if (this.CurrentSolution.FindProject(projectStore.DocumentReference) != null || this.CurrentSolution.FindProjectByName(projectStore.DocumentReference.DisplayName) != null)
			{
				IMessageDisplayService messageDisplayService = this.Services.MessageDisplayService();
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string cannotAddNamedProjectToSolutionMessage = StringTable.CannotAddNamedProjectToSolutionMessage;
				object[] displayName = new object[] { projectStore.DocumentReference.DisplayName };
				messageDisplayService.ShowError(string.Format(currentCulture, cannotAddNamedProjectToSolutionMessage, displayName));
				return null;
			}
			if (!this.EnsureSolutionTypeIsCorrect())
			{
				return null;
			}
			INamedProject namedProject = this.currentSolution.AddProject(projectStore);
			if (namedProject != null)
			{
				this.ItemSelectionSet.SetSelection(namedProject);
			}
			return namedProject;
		}

		internal void AddRecentProject(string projectUrl)
		{
			this.recentProjects.Remove(projectUrl);
			this.recentProjects.Insert(0, projectUrl);
			Microsoft.Expression.Framework.Documents.PathHelper.ShellAddPathToRecentDocuments(projectUrl);
			UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.SaveConfiguration));
		}

		internal static void AddSourceControlMenuItems(ICommandBarItemCollection commandItems)
		{
			commandItems.AddButton("Project_AddToSourceControl", StringTable.SourceControlContextMenuAdd);
			commandItems.AddButton("Project_CheckOut", StringTable.SourceControlContextMenuCheckOutSingle);
			commandItems.AddButton("Project_CheckOutRecursive", StringTable.SourceControlContextMenuCheckOut);
			commandItems.AddButton("Project_CheckIn", StringTable.SourceControlContextMenuCheckIn);
			commandItems.AddButton("Project_UndoPendingChanges", StringTable.SourceControlContextMenuUndoPendingChanges);
			commandItems.AddButton("Project_GetLatestVersion", StringTable.SourceControlContextMenuGetLatestVersion);
			commandItems.AddButton("Project_GetSpecificVersionSelected", StringTable.SourceControlContextMenuGetSpecificVersion);
			commandItems.AddButton("Project_ViewHistory", StringTable.SourceControlContextMenuViewHistory);
		}

		internal void ApplicationActivated()
		{
			if (!this.applicationIsActive)
			{
				this.applicationIsActive = true;
				if (this.currentSolution != null)
				{
					this.currentSolution.ReactivateWatchers();
				}
			}
		}

		internal void ApplicationDeactivated()
		{
			if (this.applicationIsActive)
			{
				this.applicationIsActive = false;
				if (this.currentSolution != null)
				{
					this.currentSolution.DeactivateWatchers();
				}
			}
		}

		private void BuildManager_BuildStarting(object sender, BuildStartingEventArgs e)
		{
			ProjectManager.BuildListener buildListener = new ProjectManager.BuildListener(this);
		}

		public bool CloseSolution()
		{
			return this.CloseSolution(false);
		}

		public bool CloseSolution(bool applicationClosing)
		{
			bool flag;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SolutionClose);
			if (this.CurrentSolution != null)
			{
				this.OnSolutionClosing(new SolutionEventArgs(this.CurrentSolution));
			}
			try
			{
				if (applicationClosing && Microsoft.Expression.Project.Build.BuildManager.Finalizing)
				{
					flag = false;
				}
				else if (this.CurrentSolution != null)
				{
					bool flag1 = false;
					if (Microsoft.Expression.Project.Build.BuildManager.Building)
					{
						MessageBoxArgs messageBoxArg = new MessageBoxArgs()
						{
							Message = StringTable.CannotCloseDuringBuildErrorMessage,
							Button = MessageBoxButton.YesNo,
							Image = MessageBoxImage.Asterisk
						};
						if (this.Services.MessageDisplayService().ShowMessage(messageBoxArg) != MessageBoxResult.No)
						{
							if (applicationClosing)
							{
								Microsoft.Expression.Project.Build.BuildManager.NotificationsDisabled = true;
							}
							Microsoft.Expression.Project.Build.BuildManager.CancelBuild();
						}
						else
						{
							flag1 = true;
						}
					}
					if (!flag1)
					{
						if (!this.currentSolution.Save(true))
						{
							flag1 = true;
							Microsoft.Expression.Project.Build.BuildManager.NotificationsDisabled = false;
						}
						else
						{
							this.currentSolution.Close(true);
							this.PostSolutionCloseCleanup();
						}
					}
					if (applicationClosing && !flag1)
					{
						Microsoft.Expression.Project.Build.BuildManager.NotificationsDisabled = true;
					}
					flag = !flag1;
				}
				else
				{
					if (applicationClosing)
					{
						Microsoft.Expression.Project.Build.BuildManager.NotificationsDisabled = true;
					}
					flag = true;
				}
			}
			finally
			{
				PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SolutionClose);
			}
			return flag;
		}

		internal void CloseSolutionInternal()
		{
			this.CloseSolutionInternal(false);
		}

		internal void CloseSolutionInternal(bool errorStateShutdown)
		{
			if (this.CurrentSolution == null)
			{
				return;
			}
			if (Microsoft.Expression.Project.Build.BuildManager.Building)
			{
				Microsoft.Expression.Project.Build.BuildManager.CancelBuild();
			}
			try
			{
				this.currentSolution.Close(!errorStateShutdown);
			}
			finally
			{
				this.PostSolutionCloseCleanup();
			}
		}

		internal static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite)
		{
			List<string> strs = new List<string>();
			List<string> strs1 = new List<string>();
			ProjectManager.GetDirectoryList(sourceDirectory, strs, strs1);
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(destinationDirectory))
			{
				Directory.CreateDirectory(destinationDirectory);
			}
			foreach (string str in strs1)
			{
				string str1 = str.Substring(sourceDirectory.Length);
				char[] directorySeparatorChar = new char[] { Path.DirectorySeparatorChar };
				str1 = str1.TrimStart(directorySeparatorChar);
				Directory.CreateDirectory(Path.Combine(destinationDirectory, str1));
			}
			foreach (string str2 in strs)
			{
				string str3 = str2.Substring(sourceDirectory.Length);
				char[] chrArray = new char[] { Path.DirectorySeparatorChar };
				string str4 = Path.Combine(destinationDirectory, str3.TrimStart(chrArray));
				File.Copy(str2, str4, overwrite);
				Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(str4);
			}
		}

		internal static void CopyDirectory(string sourceDirectory, string destinationDirectory)
		{
			ProjectManager.CopyDirectory(sourceDirectory, destinationDirectory, false);
		}

		public IEnumerable<INamedProject> CreateProjectTemplate(string projectPath, string projectName, IProjectTemplate projectTemplate, IEnumerable<TemplateArgument> templateArguments)
		{
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectCreate);
			IEnumerable<INamedProject> namedProjects = this.CreateProjectTemplateInternal(projectPath, projectName, projectTemplate, templateArguments);
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectCreate);
			return namedProjects;
		}

		private IEnumerable<INamedProject> CreateProjectTemplateInternal(string projectPath, string projectName, IProjectTemplate projectTemplate, IEnumerable<TemplateArgument> templateArguments)
		{
			IEnumerable<INamedProject> namedProjects = null;
			if (this.CurrentSolution == null)
			{
				projectPath = Path.Combine(projectPath, projectName);
				if (projectTemplate == null || projectTemplate.HasProjectFile)
				{
					string solutionPathFileName = ProjectManager.GetSolutionPathFileName(projectPath, projectName);
					this.CurrentSolution = VisualStudioSolution.CreateSolution(this.serviceProvider, DocumentReference.Create(solutionPathFileName));
				}
				else
				{
					this.CurrentSolution = new WebProjectSolution(this.serviceProvider, DocumentReference.Create(projectPath));
				}
				if (this.CurrentSolution == null)
				{
					return null;
				}
				if (projectTemplate != null)
				{
					namedProjects = projectTemplate.CreateProjects(projectName, projectPath, templateArguments, this.serviceProvider);
				}
				this.OnActiveBuildTargetChanged(EventArgs.Empty);
				this.OnSolutionOpened(new SolutionEventArgs(this.CurrentSolution));
				this.AddRecentProject(this.CurrentSolution.DocumentReference.Path);
			}
			else if (projectTemplate != null)
			{
				namedProjects = projectTemplate.CreateProjects(projectName, projectPath, templateArguments, this.serviceProvider);
			}
			this.currentSolution.AddProjectOutputReferences(namedProjects);
			if (namedProjects != null && namedProjects.Any<INamedProject>() && projectTemplate != null && projectTemplate.BuildOnLoad)
			{
				this.BuildManager.Build(this.currentSolution.ProjectBuildContext, null, true);
			}
			return namedProjects;
		}

		private ISolutionManagement CreateSolution(IServiceProvider serviceProvider, DocumentReference documentReference)
		{
			if (Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(documentReference.Path).ToUpperInvariant() == ".SLN")
			{
				return new VisualStudioSolution(serviceProvider, documentReference);
			}
			if (Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(documentReference.Path))
			{
				return new WebProjectSolution(serviceProvider, documentReference);
			}
			string str = this.FindContainingSolution(documentReference);
			if (str == null)
			{
				return new MSBuildProjectSolution(serviceProvider, documentReference);
			}
			return new VisualStudioSolution(serviceProvider, DocumentReference.Create(str));
		}

		private void CurrentApplication_Activated(object sender, EventArgs e)
		{
			UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.ApplicationActivated));
		}

		private void CurrentApplication_Deactivated(object sender, EventArgs e)
		{
			this.ApplicationDeactivated();
		}

		private void DeactivateExternalChangeNotification()
		{
			bool flag = this.delayNotificationCount == 0;
			ProjectManager projectManager = this;
			projectManager.delayNotificationCount = projectManager.delayNotificationCount + 1;
			if (flag)
			{
				this.OnIsDelayedChanged();
			}
		}

		public string DetermineOriginalImplicitAssemblyLocation(Assembly assembly)
		{
			if (!(assembly != null) || !this.implicitAssemblyReferences.ContainsKey(assembly))
			{
				return null;
			}
			return this.implicitAssemblyReferences[assembly];
		}

		private void DocumentManager_DocumentClosed(object sender, DocumentEventArgs e)
		{
			IProject document = e.Document as IProject;
			if (document != null && this.CurrentSolution.Projects.Contains<IProject>(document))
			{
				this.currentSolution.CloseProject(document);
			}
		}

		private bool EnsureSolutionTypeIsCorrect()
		{
			bool flag;
			if (!(this.CurrentSolution is SingleProjectSolution) || this.CurrentSolution.Projects.CountIs<IProject>(0))
			{
				return true;
			}
			ISolutionManagement solutionManagement = null;
			try
			{
				solutionManagement = VisualStudioSolution.MigrateSolution(this.serviceProvider, this.CurrentSolution as SingleProjectSolution);
				if (solutionManagement == null)
				{
					return false;
				}
				this.CurrentSolution = solutionManagement;
				this.OnSolutionMigrated(new SolutionEventArgs(this.currentSolution));
				this.OnActiveBuildTargetChanged(EventArgs.Empty);
				return true;
			}
			catch (Exception exception)
			{
				if (!ErrorHandling.ShouldHandleExceptions(exception))
				{
					throw;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		private string FindContainingSolution(DocumentReference projectReference)
		{
			string str = Path.ChangeExtension(projectReference.Path, ".sln");
			if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str) && this.SolutionContainsProject(str, projectReference))
			{
				return str;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(projectReference.Path));
			FileInfo[] files = directoryInfo.GetFiles("*.sln");
			if ((int)files.Length == 1 && this.SolutionContainsProject(files[0].FullName, projectReference))
			{
				return files[0].FullName;
			}
			if (directoryInfo.Parent != null)
			{
				FileInfo[] fileInfoArray = directoryInfo.Parent.GetFiles("*.sln");
				if ((int)fileInfoArray.Length == 1 && this.SolutionContainsProject(fileInfoArray[0].FullName, projectReference))
				{
					return fileInfoArray[0].FullName;
				}
			}
			return null;
		}

		private static void GetDirectoryList(string sourceDirectory, List<string> files, List<string> folders)
		{
			string[] strArrays = Directory.GetFiles(sourceDirectory);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				files.Add(strArrays[i]);
			}
			string[] directories = Directory.GetDirectories(sourceDirectory);
			for (int j = 0; j < (int)directories.Length; j++)
			{
				string str = directories[j];
				folders.Add(str);
				ProjectManager.GetDirectoryList(str, files, folders);
			}
		}

		private string GetProjectListAsString()
		{
			int num;
			StringBuilder stringBuilder = new StringBuilder();
			int num1 = 0;
			List<string>.Enumerator enumerator = this.recentProjects.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					stringBuilder.Append(enumerator.Current);
					stringBuilder.Append("|");
					num = num1 + 1;
					num1 = num;
				}
				while (num < ProjectManager.MaximumRecentProjectsCount);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		private string[] GetProjectListFromString(string temp)
		{
			return temp.Split(new char[] { '|' });
		}

		public static string GetSolutionPathFileName(string projectPath, string projectName)
		{
			return string.Concat(Path.Combine(projectPath, projectName), ".sln");
		}

		private void InitializeCommands()
		{
			base.AddCommand("Application_NewProject", new ProjectNewCommand(this.serviceProvider, this.configuration));
			base.AddCommand("Application_CloseAllDocuments", new CloseAllDocumentsCommand(this.serviceProvider, true));
			base.AddCommand("Application_CloseOtherDocuments", new CloseAllDocumentsCommand(this.serviceProvider, false));
			base.AddCommand("Project_NewFolder", new NewFolderCommand(this.serviceProvider));
			base.AddCommand("Project_Build", new BuildCommand(this.serviceProvider));
			base.AddCommand("Project_Clean", new CleanCommand(this.serviceProvider));
			base.AddCommand("Project_Rebuild", new RebuildCommand(this.serviceProvider));
			base.AddCommand("Project_TestProject", new TestProjectCommand(this.serviceProvider));
			base.AddCommand("Project_CloseProject", new CloseProjectCommand(this.serviceProvider));
			base.AddCommand("Project_SaveSolutionCopy", new SaveSolutionCopyCommand(this.serviceProvider));
			base.AddCommand("Project_OpenProject", new OpenProjectCommand(this.serviceProvider));
			base.AddCommand("Project_AddNewProject", new AddNewProjectCommand(this.serviceProvider, this.configuration));
			base.AddCommand("Project_AddExistingProject", new AddExistingProjectCommand(this.serviceProvider));
			base.AddCommand("Project_AddExistingWebsite", new AddExistingWebsiteCommand(this.serviceProvider));
			base.AddCommand("Project_ExploreProject", new ExploreProjectCommand(this.serviceProvider));
			base.AddCommand("Application_SaveAll", new SaveAllCommand(this.serviceProvider));
			base.AddCommand("Project_AddExistingItem", new AddExistingItemCommand(this.serviceProvider));
			base.AddCommand("Project_AddExistingItemOfType", new AddExistingItemOfTypeCommand(this.serviceProvider, null));
			base.AddCommand("Project_LinkToExistingItem", new LinkToExistingItemCommand(this.serviceProvider));
			base.AddCommand("Project_AddProjectReference", new AddProjectReferenceCommand(this.serviceProvider));
			base.AddCommand("Project_DeleteProjectItem", new DeleteProjectItemCommand(this.serviceProvider));
			base.AddCommand("Project_RemoveProjectItem", new RemoveProjectItemCommand(this.serviceProvider));
			base.AddCommand("Project_SetStartupScene", new SetStartupSceneCommand(this.serviceProvider));
			base.AddCommand("Project_SetStartupSceneMenu", new SetStartupSceneMenuCommand(this.serviceProvider));
			base.AddCommand("Project_SetStartupProject", new SetStartupProjectCommand(this.serviceProvider));
			base.AddCommand("Project_InsertIntoActiveDocument", new InsertIntoActiveDocumentCommand(this.serviceProvider));
			base.AddCommand("Project_EditExternally", new EditExternallyCommand(this.serviceProvider));
			base.AddCommand("Project_Refresh", new RefreshCommand(this.serviceProvider));
			for (int i = 0; i < ProjectManager.MaximumRecentProjectsCount; i++)
			{
				base.AddCommand(string.Concat("Project_OpenRecentProject_", i.ToString(CultureInfo.InvariantCulture)), new OpenRecentProject(this.serviceProvider, i));
			}
			CutBuffer cutBuffer = new CutBuffer(this.serviceProvider);
			base.AddCommand("Project_Paste", new PasteCommand(this.serviceProvider, cutBuffer));
			base.AddCommand("Project_Copy", new CutCopyCommand(this.serviceProvider, false, cutBuffer));
			base.AddCommand("Project_Cut", new CutCopyCommand(this.serviceProvider, true, cutBuffer));
			base.AddCommand("Project_AddToSourceControl", new AddToSourceControlCommand(this.serviceProvider));
			base.AddCommand("Project_CheckOut", new CheckOutCommand(false, this.serviceProvider));
			base.AddCommand("Project_CheckOutRecursive", new CheckOutCommand(true, this.serviceProvider));
			base.AddCommand("Project_CheckIn", new CheckInCommand(this.serviceProvider));
			base.AddCommand("Project_ViewHistory", new ViewHistoryCommand(this.serviceProvider));
			base.AddCommand("Project_GetLatestVersion", new GetCommand(this.serviceProvider, true, SourceControlGetOption.Latest));
			base.AddCommand("Project_GetSpecificVersionSelected", new GetCommand(this.serviceProvider, true, SourceControlGetOption.UserSpecifiedVersion));
			base.AddCommand("Project_UndoPendingChanges", new UndoPendingChangesCommand(this.serviceProvider));
			base.AddCommand("Project_RefreshStatus", new RefreshStatusCommand(this.serviceProvider));
			base.AddCommand("Project_GoOnline", new GoOnlineCommand(this.serviceProvider));
			base.AddCommand("Project_ResolveConflicts", new ResolveCommand(this.serviceProvider));
			base.AddCommand("Project_ConvertToSilverlight4", new UpgradeProjectCommand(new SilverlightProjectConverter(null, this.serviceProvider), ConversionType.ProjectSilverlight3, ConversionType.ProjectSilverlight4, this.serviceProvider));
			base.AddCommand("Project_ConvertToDotNet4", new UpgradeProjectCommand(new WpfProjectConverter(null, this.serviceProvider), ConversionType.ProjectWpf35, ConversionType.ProjectWpf40, this.serviceProvider));
		}

		public void InitializeFromKnownProjects(string[] args)
		{
			bool flag;
			string empty;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectSystemInitialization);
			this.projectConfigurationInitialized = true;
			string str = null;
			ICommandLineService service = (ICommandLineService)this.serviceProvider.GetService(typeof(ICommandLineService));
			string[] arguments = service.GetArguments(string.Empty, args);
			if (arguments != null)
			{
				if ((int)arguments.Length != 1)
				{
					if ((int)arguments.Length > 1)
					{
						this.Services.MessageDisplayService().ShowError(StringTable.InvaldCommandLineArgumentsDialogMessage);
					}
				}
				else if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(arguments[0]))
				{
					str = arguments[0];
				}
				else if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(arguments[0]))
				{
					IMessageDisplayService messageDisplayService = this.Services.MessageDisplayService();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string invalidCommandLineArgumentsBadProjectMessage = StringTable.InvalidCommandLineArgumentsBadProjectMessage;
					object[] objArray = new object[] { arguments[0] };
					messageDisplayService.ShowError(string.Format(currentCulture, invalidCommandLineArgumentsBadProjectMessage, objArray));
				}
				else
				{
					this.Services.MessageDisplayService().ShowError(string.Format(CultureInfo.CurrentCulture, StringTable.InvalidCommandLineArgumentsWebsitesNotSupportedMessage, new object[0]));
				}
			}
			if (str != null)
			{
				DocumentReference documentReference = DocumentReference.Create(str);
				if (!documentReference.IsValidPathFormat || !Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(documentReference.Path))
				{
					IMessageDisplayService messageDisplayService1 = this.Services.MessageDisplayService();
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string invalidCommandLineArgumentsBadFileMessage = StringTable.InvalidCommandLineArgumentsBadFileMessage;
					object[] objArray1 = new object[] { str };
					messageDisplayService1.ShowError(string.Format(cultureInfo, invalidCommandLineArgumentsBadFileMessage, objArray1));
				}
				if (this.IsValidProjectReference(documentReference))
				{
					MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
					IConfigurationService configurationService = this.serviceProvider.GetService<IConfigurationService>();
					if (this.OptionsModel != null && configurationService != null && Microsoft.Expression.Framework.Documents.PathHelper.FileExists(documentReference.Path) && this.OptionsModel.ShowSecurityWarning)
					{
						MessageBoxArgs messageBoxArg = new MessageBoxArgs()
						{
							CheckBoxMessage = StringTable.UnknownFileSecurityCheckboxMessage,
							Message = StringTable.UnknownFileSecurityWarningMessage,
							Button = MessageBoxButton.YesNo,
							Image = MessageBoxImage.Exclamation
						};
						MessageBoxArgs messageBoxArg1 = messageBoxArg;
						messageBoxResult = this.Services.MessageDisplayService().ShowMessage(messageBoxArg1, out flag);
						if (flag)
						{
							this.OptionsModel.ShowSecurityWarning = false;
							this.OptionsModel.Save(configurationService["Options.ProjectSystemOptions"]);
							configurationService.Save();
						}
					}
					string[] strArrays = service.GetArguments("file", args);
					if (messageBoxResult == MessageBoxResult.Yes && this.OpenSolution(documentReference, true, strArrays == null) != null)
					{
						IProject project = this.CurrentSolution.FindProject(documentReference) as IProject;
						if (project != null && strArrays != null)
						{
							string[] strArrays1 = strArrays;
							for (int i = 0; i < (int)strArrays1.Length; i++)
							{
								string str1 = strArrays1[i];
								bool flag1 = false;
								try
								{
									empty = Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(project.ProjectRoot.Path, str1);
								}
								catch (ArgumentException argumentException)
								{
									empty = string.Empty;
									flag1 = true;
								}
								if (!flag1)
								{
									IProjectItem projectItem = project.Items.FindMatchByUrl<IProjectItem>(empty);
									if (projectItem == null || projectItem.OpenView(true) == null)
									{
										flag1 = true;
									}
								}
								if (flag1)
								{
									IMessageDisplayService messageDisplayService2 = this.Services.MessageDisplayService();
									CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
									string invalidCommandLineArgumentsBadFileMessage1 = StringTable.InvalidCommandLineArgumentsBadFileMessage;
									object[] objArray2 = new object[] { str1 };
									messageDisplayService2.ShowError(string.Format(currentCulture1, invalidCommandLineArgumentsBadFileMessage1, objArray2));
								}
							}
						}
					}
				}
				else if (this.CurrentSolution != null)
				{
					bool flag2 = false;
					foreach (IProject project1 in this.CurrentSolution.Projects)
					{
						if (!this.serviceProvider.OpenFile(str))
						{
							continue;
						}
						flag2 = true;
						break;
					}
					if (!flag2)
					{
						IMessageDisplayService messageDisplayService3 = this.Services.MessageDisplayService();
						CultureInfo cultureInfo1 = CultureInfo.CurrentCulture;
						string invalidCommandLineArgumentsBadFileMessage2 = StringTable.InvalidCommandLineArgumentsBadFileMessage;
						object[] objArray3 = new object[] { str };
						messageDisplayService3.ShowError(string.Format(cultureInfo1, invalidCommandLineArgumentsBadFileMessage2, objArray3));
					}
				}
				else
				{
					ErrorArgs errorArg = new ErrorArgs();
					CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
					string openProjectOrSolutionInvalidTypeMessage = StringTable.OpenProjectOrSolutionInvalidTypeMessage;
					object[] objArray4 = new object[] { str };
					errorArg.Message = string.Format(currentCulture2, openProjectOrSolutionInvalidTypeMessage, objArray4);
					errorArg.AutomationId = "OpenSolutionErrorDialog";
					this.Services.MessageDisplayService().ShowError(errorArg);
				}
			}
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectSystemInitialization, "OnInitialized");
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectSystemInitialization);
		}

		public void InitializeRecentProjects()
		{
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectSystemInitialization, "InitializeRecentProjects");
			string property = (string)this.configuration.GetProperty("RecentProjects", "");
			if (!string.IsNullOrEmpty(property))
			{
				this.recentProjects.Clear();
				string[] projectListFromString = this.GetProjectListFromString(property);
				for (int i = 0; i < (int)projectListFromString.Length; i++)
				{
					string str = projectListFromString[i];
					this.recentProjects.Add(str);
					if (this.recentProjects.Count >= ProjectManager.MaximumRecentProjectsCount)
					{
						break;
					}
				}
			}
			PerformanceUtility.MarkInterimStep(PerformanceEvent.ProjectSystemInitialization, "Done Opening Projects");
		}

		private void InitializeSpecialFolderManagers()
		{
			this.defaultImportFolder = new SpecialFolderManager("LastImportFolder", this.configuration);
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (string.IsNullOrEmpty(folderPath))
			{
				folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}
			this.defaultOpenProjectFolder = new SpecialFolderManager("LastOpenProjectFolder", this.configuration, Path.Combine(folderPath, ProjectManager.DefaultProjectFolder));
			this.defaultNewProjectFolder = new SpecialFolderManager("LastNewProjectFolder", this.configuration, Path.Combine(folderPath, ProjectManager.DefaultProjectFolder));
			this.defaultSampleProjectFolder = new SpecialFolderManager("LastSampleProjectFolder", this.configuration, Path.Combine(folderPath, ProjectManager.DefaultSampleFolder));
			string argument = this.Services.CommandLineService().GetArgument("userpath");
			if (argument != null)
			{
				this.defaultNewProjectFolder.Path = argument;
				this.defaultOpenProjectFolder.Path = argument;
			}
		}

		private bool IsValidProjectReference(DocumentReference projectReference)
		{
			if (!projectReference.IsValidPathFormat)
			{
				return false;
			}
			if (Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(projectReference.Path))
			{
				return true;
			}
			string safeExtension = Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(projectReference.Path);
			if (safeExtension.Equals(".sln", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			IDocumentType item = this.Services.DocumentTypes()[DocumentTypeNamesHelper.ProjectReference];
			string[] fileExtensions = item.FileExtensions;
			for (int i = 0; i < (int)fileExtensions.Length; i++)
			{
				if (safeExtension.Equals(fileExtensions[i], StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static void MarkFilesWithNormalAttribute(string folder)
		{
			string[] files = Directory.GetFiles(folder);
			for (int i = 0; i < (int)files.Length; i++)
			{
				File.SetAttributes(files[i], FileAttributes.Normal);
			}
			string[] directories = Directory.GetDirectories(folder);
			for (int j = 0; j < (int)directories.Length; j++)
			{
				ProjectManager.MarkFilesWithNormalAttribute(directories[j]);
			}
		}

		IDisposable Microsoft.Expression.Framework.IExternalChanges.DelayNotification()
		{
			return new ProjectManager.DelayNotificationDisposable(this);
		}

		internal void OnActiveBuildTargetChanged(EventArgs e)
		{
			if (this.ActiveBuildTargetChanged != null)
			{
				this.ActiveBuildTargetChanged(this, e);
			}
		}

		private void OnIsDelayedChanged()
		{
			if (!((IExternalChanges)this).IsDelayed && this.currentSolution != null)
			{
				this.currentSolution.CheckDelayedChange();
			}
		}

		internal void OnProjectClosed(ProjectEventArgs e)
		{
			IProject project = e.Project;
			if (project != null)
			{
				project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
			}
			if (this.ProjectClosed != null)
			{
				this.ProjectClosed(this, e);
			}
		}

		internal void OnProjectClosing(ProjectEventArgs e)
		{
			if (this.ProjectClosing != null)
			{
				this.ProjectClosing(this, e);
			}
		}

		internal void OnProjectOpened(ProjectEventArgs e)
		{
			IProject project = e.Project;
			if (project != null)
			{
				project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
			}
			if (this.ProjectOpened != null)
			{
				this.ProjectOpened(this, e);
			}
		}

		internal void OnSolutionClosed(SolutionEventArgs e)
		{
			if (this.SolutionClosed != null)
			{
				this.SolutionClosed(this, e);
			}
		}

		internal void OnSolutionClosing(SolutionEventArgs e)
		{
			if (this.SolutionClosing != null)
			{
				this.SolutionClosing(this, e);
			}
		}

		internal void OnSolutionMigrated(SolutionEventArgs e)
		{
			if (this.SolutionMigrated != null)
			{
				this.SolutionMigrated(this, e);
			}
		}

		internal void OnSolutionOpened(SolutionEventArgs e)
		{
			if (this.SolutionOpened != null)
			{
				this.SolutionOpened(this, e);
			}
		}

		internal void OnStartupProjectChanged(EventArgs e)
		{
			IExecutable startupProject;
			if (this.CurrentSolution != null)
			{
				startupProject = this.CurrentSolution.StartupProject;
			}
			else
			{
				startupProject = null;
			}
			this.UpdateCurrentWorkingDirectory(startupProject);
			if (this.StartupProjectChanged != null)
			{
				this.StartupProjectChanged(this, e);
			}
		}

		public ISolution OpenSolution(DocumentReference solutionOrProjectReference, bool addToRecentList, bool openInitialScene)
		{
			ISolution currentSolution;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SolutionOpen);
			using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
			{
				try
				{
					try
					{
						if (this.IsValidProjectReference(solutionOrProjectReference))
						{
							if (this.CurrentSolution != null)
							{
								if (this.CurrentSolution.DocumentReference.Path == solutionOrProjectReference.Path || this.CurrentSolution.FindProject(solutionOrProjectReference) != null)
								{
									currentSolution = this.CurrentSolution;
									return currentSolution;
								}
								else if (!this.CloseSolution())
								{
									currentSolution = null;
									return currentSolution;
								}
							}
							this.currentSolution = this.CreateSolution(this.serviceProvider, solutionOrProjectReference);
							if (this.currentSolution.Load())
							{
								LicensingHelper.ShowLicensingDialogIfNecessary(this.Services);
								if (openInitialScene)
								{
									IProject startupProject = this.currentSolution.StartupProject as IProject;
									if (startupProject != null)
									{
										this.UpdateCurrentWorkingDirectory(startupProject as IExecutable);
									}
								}
								if (this.ItemSelectionSet.Count == 0)
								{
									this.itemSelectionSet.SetSelection(this.CurrentSolution);
								}
								if (addToRecentList)
								{
									this.AddRecentProject(this.CurrentSolution.DocumentReference.Path);
								}
								this.OnSolutionOpened(new SolutionEventArgs(this.CurrentSolution));
								this.OnActiveBuildTargetChanged(EventArgs.Empty);
								if (openInitialScene)
								{
									this.currentSolution.OpenInitialViews();
								}
							}
							else
							{
								this.currentSolution.Close(false);
								this.currentSolution = null;
							}
						}
						else
						{
							ErrorArgs errorArg = new ErrorArgs();
							CultureInfo currentCulture = CultureInfo.CurrentCulture;
							string openProjectOrSolutionInvalidTypeMessage = StringTable.OpenProjectOrSolutionInvalidTypeMessage;
							object[] path = new object[] { solutionOrProjectReference.Path };
							errorArg.Message = string.Format(currentCulture, openProjectOrSolutionInvalidTypeMessage, path);
							errorArg.AutomationId = "OpenSolutionErrorDialog";
							this.Services.MessageDisplayService().ShowError(errorArg);
							currentSolution = null;
							return currentSolution;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (!ErrorHandling.ShouldHandleExceptions(exception))
						{
							throw;
						}
						ErrorArgs errorArg1 = new ErrorArgs();
						CultureInfo cultureInfo = CultureInfo.CurrentCulture;
						string openProjectOrSolutionErrorDialogMessage = StringTable.OpenProjectOrSolutionErrorDialogMessage;
						object[] objArray = new object[] { solutionOrProjectReference.Path, exception.Message };
						errorArg1.Message = string.Format(cultureInfo, openProjectOrSolutionErrorDialogMessage, objArray);
						errorArg1.AutomationId = "OpenSolutionErrorDialog";
						this.Services.MessageDisplayService().ShowError(errorArg1);
						if (this.CurrentSolution != null)
						{
							this.currentSolution.Close(false);
							this.currentSolution = null;
						}
					}
				}
				finally
				{
					PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SolutionOpen);
					using (SolutionBase.DisableReloadPromptToken disableReloadPromptToken = new SolutionBase.DisableReloadPromptToken())
					{
						using (ConversionSupressor conversionSupressor = new ConversionSupressor())
						{
							SolutionBase solutionBase = this.CurrentSolution as SolutionBase;
							if (solutionBase != null)
							{
								solutionBase.CheckForChangedOrDeletedItems();
							}
						}
					}
				}
				return this.CurrentSolution;
			}
			return currentSolution;
		}

		private void PostSolutionCloseCleanup()
		{
			SolutionEventArgs solutionEventArg = new SolutionEventArgs(this.currentSolution);
			this.currentSolution = null;
			this.OnSolutionClosed(solutionEventArg);
			this.Services.MessageLoggingService().Clear();
			this.UpdateConfiguration();
			this.OnActiveBuildTargetChanged(EventArgs.Empty);
		}

		internal void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (this.ItemSelectionSet.IsSelected(e.ProjectItem))
			{
				this.ItemSelectionSet.RemoveSelection(e.ProjectItem);
			}
		}

		private void ReactivateExternalChangeNotification()
		{
			ProjectManager projectManager = this;
			projectManager.delayNotificationCount = projectManager.delayNotificationCount - 1;
			if (this.delayNotificationCount == 0)
			{
				this.OnIsDelayedChanged();
			}
		}

		internal void RemoveRecentProject(string projectUrl)
		{
			this.recentProjects.Remove(projectUrl);
			UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.SaveConfiguration));
		}

		internal void RemoveRecentProject(string projectUrl, bool updateConfiguration)
		{
			if (updateConfiguration)
			{
				this.RemoveRecentProject(projectUrl);
				return;
			}
			this.recentProjects.Remove(projectUrl);
		}

		private void SaveConfiguration()
		{
			try
			{
				IConfigurationService service = this.serviceProvider.GetService<IConfigurationService>();
				if (service != null)
				{
					this.UpdateConfiguration();
					service.Save();
				}
			}
			catch (Exception exception)
			{
				throw;
			}
		}

		private bool SolutionContainsProject(string solutionPath, DocumentReference projectReference)
		{
			VisualStudioSolution.ProjectMetadata projectMetadatum;
			bool flag;
			using (FileStream fileStream = File.OpenRead(solutionPath))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
				{
					string str = null;
					do
					{
					Label3:
						string str1 = streamReader.ReadLine();
						str = str1;
						if (str1 == null)
						{
							return false;
						}
						else if (str.StartsWith("Project(", StringComparison.Ordinal))
						{
							projectMetadatum = VisualStudioSolution.ProjectMetadata.Create(str, solutionPath);
						}
						else
						{
							goto Label3;
						}
					}
					while (projectMetadatum == null || !projectMetadatum.RelativePath.Equals(DocumentReference.Create(solutionPath).GetRelativePath(projectReference), StringComparison.OrdinalIgnoreCase));
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public string TargetFolderForProject(IProject project)
		{
			if (project == null)
			{
				return null;
			}
			string path = null;
			IProjectItem projectItem = this.ItemSelectionSet.Selection.SingleOrNull<IDocumentItem>() as IProjectItem;
			if (projectItem != null && projectItem.Project == project)
			{
				if (!projectItem.CanAddChildren)
				{
					IProjectItem parent = projectItem.Parent;
					path = (parent != null ? Path.GetDirectoryName(parent.DocumentReference.Path) : project.ProjectRoot.Path);
				}
				else
				{
					path = projectItem.DocumentReference.Path;
				}
			}
			if (path == null && project.ProjectRoot != null)
			{
				path = project.ProjectRoot.Path;
			}
			return path;
		}

		internal void UpdateConfiguration()
		{
			if (this.projectConfigurationInitialized && this.configuration != null)
			{
				this.configuration.SetProperty("RecentProjects", this.GetProjectListAsString());
			}
		}

		internal void UpdateCurrentWorkingDirectory(IExecutable executable)
		{
			if (executable == null || executable.WorkingDirectory == null || !Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(executable.WorkingDirectory))
			{
				try
				{
					CurrentDirectoryHelper.SetWorkingDirectory(this.initialWorkingDirectory);
				}
				catch (IOException oException)
				{
				}
			}
			else
			{
				try
				{
					CurrentDirectoryHelper.SetWorkingDirectory(executable.WorkingDirectory);
				}
				catch (IOException oException1)
				{
				}
			}
		}

		public event EventHandler ActiveBuildTargetChanged;

		public event EventHandler<ProjectEventArgs> ProjectClosed;

		public event EventHandler<ProjectEventArgs> ProjectClosing;

		public event EventHandler<ProjectEventArgs> ProjectOpened;

		public event EventHandler<SolutionEventArgs> SolutionClosed;

		public event EventHandler<SolutionEventArgs> SolutionClosing;

		public event EventHandler<SolutionEventArgs> SolutionMigrated;

		public event EventHandler<SolutionEventArgs> SolutionOpened;

		public event EventHandler StartupProjectChanged;

		private sealed class BuildListener
		{
			private ProjectManager projectManager;

			private IDisposable delayNotification;

			public BuildListener(ProjectManager projectManager)
			{
				this.projectManager = projectManager;
				this.projectManager.BuildManager.BuildCompleted += new EventHandler<BuildCompletedEventArgs>(this.BuildManager_BuildCompleted);
				this.delayNotification = ((IExternalChanges)this.projectManager).DelayNotification();
			}

			private void BuildManager_BuildCompleted(object sender, BuildCompletedEventArgs e)
			{
				if (this.delayNotification != null)
				{
					this.projectManager.BuildManager.BuildCompleted -= new EventHandler<BuildCompletedEventArgs>(this.BuildManager_BuildCompleted);
					this.delayNotification.Dispose();
					this.delayNotification = null;
				}
			}
		}

		private class CaseInsensitiveStringList : List<string>
		{
			internal CaseInsensitiveStringList()
			{
			}

			internal void Add(string stringValue)
			{
				base.Add(stringValue);
			}

			internal bool Contains(string stringValue)
			{
				return this.FindIndex(stringValue) >= 0;
			}

			private int FindIndex(string stringValue)
			{
				return base.FindIndex((string target) => StringComparer.OrdinalIgnoreCase.Compare(target, stringValue) == 0);
			}

			internal void Remove(string stringValue)
			{
				int num = this.FindIndex(stringValue);
				if (num >= 0)
				{
					base.RemoveAt(num);
				}
			}
		}

		private sealed class DelayNotificationDisposable : IDisposable
		{
			private ProjectManager projectManager;

			public DelayNotificationDisposable(ProjectManager projectManager)
			{
				this.projectManager = projectManager;
				this.projectManager.DeactivateExternalChangeNotification();
			}

			public void Dispose()
			{
				if (this.projectManager != null)
				{
					this.projectManager.ReactivateExternalChangeNotification();
				}
				this.projectManager = null;
			}
		}
	}
}