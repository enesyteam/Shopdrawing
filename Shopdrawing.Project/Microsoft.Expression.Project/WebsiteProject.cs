using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.WebServer;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.VSWebsites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
	internal sealed class WebsiteProject : KnownProjectBase, IWebsiteProject, IProject, INamedProject, IDocumentItem, IDisposable, INotifyPropertyChanged, IExecutable
	{
		private const string DefaultStartPage = "default.html";

		private static int CassiniTimeout;

		private IWebServerService webServerService;

		private int? serverHandle;

		private Microsoft.Expression.Project.ProjectType projectType;

		private int? cachedVswdPort;

		private bool isExecuting;

		private IProjectItem startupItem;

		private static Guid WebProjectTypeGuid;

		public bool CanExecute
		{
			get
			{
				return true;
			}
		}

		public override ICodeDocumentType CodeDocumentType
		{
			get
			{
				return (ICodeDocumentType)this.JavascriptDocumentType;
			}
		}

		public override string DefaultNamespaceName
		{
			get
			{
				return CodeGenerator.GetApplicationNamespaceName(this.CodeDocumentType, this);
			}
		}

		private IDocumentType HtmlDocumentType
		{
			get
			{
				return base.Services.DocumentTypes()[DocumentTypeNamesHelper.Html];
			}
		}

		public override bool IsDirectory
		{
			get
			{
				return true;
			}
		}

		public bool IsExecuting
		{
			get
			{
				return this.isExecuting;
			}
		}

		private IDocumentType JavascriptDocumentType
		{
			get
			{
				return base.Services.DocumentTypes().JavaScriptDocumentType();
			}
		}

		public System.Diagnostics.ProcessStartInfo ProcessStartInfo
		{
			get
			{
				bool flag;
				string str;
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(Path.GetDirectoryName(base.ProjectRoot.Path));
				Uri potentialServerLocation = this.GetPotentialServerLocation();
				System.Diagnostics.ProcessStartInfo processStartInfo = base.Services.OutOfBrowserDeploymentService().TryPerformOutOfBrowserDeployment(this, documentReference, potentialServerLocation, out flag);
				if (processStartInfo == null || flag)
				{
					this.EnsureServer(documentReference);
				}
				if (processStartInfo == null)
				{
					str = (this.StartupItem != null ? this.StartupItem.DocumentReference.Path : base.ProjectRoot.Path);
					string sessionAddress = this.webServerService.GetSessionAddress(this.serverHandle.Value, str);
					if (sessionAddress == null)
					{
						return null;
					}
					processStartInfo = new System.Diagnostics.ProcessStartInfo((new Uri(sessionAddress, UriKind.Absolute)).AbsoluteUri)
					{
						UseShellExecute = true,
						Verb = "Open"
					};
				}
				return processStartInfo;
			}
		}

		public override IProjectType ProjectType
		{
			get
			{
				return this.projectType;
			}
		}

		public override bool ShouldReceiveProjectOutputReferences
		{
			get
			{
				return true;
			}
		}

		public string StartArguments
		{
			get
			{
				return string.Empty;
			}
		}

		public string StartProgram
		{
			get
			{
				return string.Empty;
			}
		}

		protected override IDocumentType StartupDocumentType
		{
			get
			{
				return this.HtmlDocumentType;
			}
		}

		public override IProjectItem StartupItem
		{
			get
			{
				return this.startupItem;
			}
			set
			{
				if (this.startupItem != value)
				{
					if (!base.AttemptToMakeProjectWritable())
					{
						return;
					}
					IProjectItem projectItem = this.startupItem;
					this.startupItem = value;
					base.OnStartupSceneChanged(new ProjectItemChangedEventArgs(projectItem, this.startupItem));
				}
			}
		}

		public override FrameworkName TargetFramework
		{
			get
			{
				return null;
			}
		}

		public override ICollection<string> TemplateProjectSubtypes
		{
			get
			{
				ICollection<string> templateProjectSubtypes = base.TemplateProjectSubtypes;
				templateProjectSubtypes.Add("Website");
				return templateProjectSubtypes;
			}
		}

		public override string UICulture
		{
			get
			{
				return CultureInfo.CurrentUICulture.ToString();
			}
			set
			{
			}
		}

		public string WorkingDirectory
		{
			get
			{
				return string.Empty;
			}
		}

		static WebsiteProject()
		{
			WebsiteProject.CassiniTimeout = 5000;
			WebsiteProject.WebProjectTypeGuid = new Guid("{E24C65DC-7377-472B-9ABA-BC803B73C61A}");
		}

		private WebsiteProject(IProjectStore projectStore, Microsoft.Expression.Project.ProjectType projectType, IServiceProvider serviceProvider) : base(projectStore, serviceProvider)
		{
			this.projectType = projectType;
			this.webServerService = serviceProvider.GetService<IWebServerService>();
		}

		public override IProjectItem AddAssemblyReference(string url, bool verifyFileExists)
		{
			return null;
		}

		protected override IEnumerable<IProjectItem> AddAssetFolder(string sourceFolder, string targetFolder, bool link)
		{
			sourceFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(sourceFolder);
			targetFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(targetFolder);
			IEnumerable<IProjectItem> array = Enumerable.Empty<IProjectItem>();
			ProjectManager.CopyDirectory(sourceFolder, targetFolder, true);
			IProjectItem projectItem = base.Items.FindMatchByUrl<IProjectItem>(targetFolder);
			if (projectItem != null)
			{
				array = projectItem.Descendants.OfType<IProjectItem>().ToArray<IProjectItem>();
			}
			else
			{
				IDocumentType item = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder];
				DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
				{
					DocumentType = item,
					TargetPath = targetFolder
				};
				projectItem = base.AddItem(documentCreationInfo);
			}
			this.RefreshChildren(projectItem, true);
			return projectItem.Descendants.OfType<IProjectItem>().Except<IProjectItem>(array);
		}

		public override void AddImport(string path)
		{
		}

		private IProjectItem AddItemDuringLoad(IDocumentType documentType, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IProjectItem parentItem)
		{
			IProjectItem projectItem = base.FindItem(documentReference) ?? documentType.CreateProjectItem(this, documentReference, base.Services);
			return this.AddItemDuringLoad(projectItem, parentItem);
		}

		private IProjectItem AddItemDuringLoad(IProjectItem projectItem, IProjectItem parentItem)
		{
			if (base.FindItem(projectItem.DocumentReference) == null)
			{
				base.AddProjectItem(projectItem);
				if (parentItem != null)
				{
					parentItem.AddChild(projectItem);
				}
			}
			return projectItem;
		}

		protected override bool AddProjectItem(IProjectItem projectItem, ProjectItemEventOptions options)
		{
			bool flag = base.AddProjectItem(projectItem, options);
			if (flag && projectItem.DocumentType == base.Services.DocumentTypes()[DocumentTypeNamesHelper.DeepZoom])
			{
				this.EnsureDeepZoomProjectItems(projectItem);
			}
			return flag;
		}

		public override IProjectItem AddProjectReference(IProject project)
		{
			return null;
		}

		public static IProject Create(IProjectStore projectStore, Microsoft.Expression.Project.ProjectType projectType, IServiceProvider serviceProvider)
		{
			return KnownProjectBase.TryCreate(() => new WebsiteProject(projectStore, projectType, serviceProvider));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !base.IsDisposed)
			{
				if (this.serverHandle.HasValue)
				{
					this.webServerService.StopBrowsingSession(this.serverHandle.Value);
				}
				this.serverHandle = null;
				base.Dispose(disposing);
			}
		}

		private void EnsureDeepZoomProjectItems(IProjectItem deepZoomProjectItem)
		{
			string[] directoryExtensions = DeepZoomHelper.GetDirectoryExtensions(deepZoomProjectItem.DocumentReference.Path);
			if (directoryExtensions != null)
			{
				string[] strArrays = directoryExtensions;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					Microsoft.Expression.Framework.Documents.DocumentReference documentReference = DeepZoomHelper.CreateDeepZoomDirectoryReference(deepZoomProjectItem.DocumentReference, str);
					IProjectItem projectItem = base.FindItem(documentReference);
					if (projectItem != null && !deepZoomProjectItem.Children.ToList<IProjectItem>().Contains(projectItem))
					{
						this.RemoveProjectItem(projectItem, false);
						projectItem = null;
					}
					if (projectItem == null && Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(documentReference.Path))
					{
						IProjectItem folderProjectItem = new FolderProjectItem(this, documentReference, base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder], base.Services, true, Microsoft.Expression.Project.FileTable.GetImageSource("Resources\\Folder_DeepZoom.png"));
						if (this.AddProjectItem(folderProjectItem, ProjectItemEventOptions.None))
						{
							deepZoomProjectItem.AddChild(folderProjectItem);
						}
					}
				}
			}
		}

		private void EnsureServer(Microsoft.Expression.Framework.Documents.DocumentReference serverLocation)
		{
			if (this.serverHandle.HasValue && !this.webServerService.IsServerReachable(this.serverHandle.Value, WebsiteProject.CassiniTimeout))
			{
				this.webServerService.StopBrowsingSession(this.serverHandle.Value);
				this.serverHandle = null;
			}
			ISolutionManagement currentSolution = base.Services.ProjectManager().CurrentSolution as ISolutionManagement;
			if (!this.serverHandle.HasValue && currentSolution != null)
			{
				WebServerSettings webServerSetting = new WebServerSettings(serverLocation.Path);
				if (!this.cachedVswdPort.HasValue || !this.IsPortAvailable(this.cachedVswdPort.Value))
				{
					object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "Port");
					if (projectProperty is int && this.IsPortAvailable((int)projectProperty))
					{
						webServerSetting.Port = (int)projectProperty;
					}
				}
				else
				{
					webServerSetting.Port = this.cachedVswdPort.Value;
				}
				this.serverHandle = new int?(this.webServerService.StartServer(webServerSetting));
			}
			if (this.serverHandle.HasValue && currentSolution != null)
			{
				string sessionAddress = this.webServerService.GetSessionAddress(this.serverHandle.Value);
				if (!string.IsNullOrEmpty(sessionAddress))
				{
					Uri uri = new Uri(sessionAddress, UriKind.Absolute);
					if (uri.Port >= 0)
					{
						currentSolution.SolutionSettingsManager.SetProjectProperty(this, "Port", uri.Port);
					}
				}
			}
		}

		public bool Execute()
		{
			this.SetRunning(true);
			Process process = null;
			System.Diagnostics.ProcessStartInfo processStartInfo = this.ProcessStartInfo;
			if (processStartInfo != null)
			{
				try
				{
					process = Process.Start(processStartInfo);
				}
				catch (Win32Exception win32Exception1)
				{
					Win32Exception win32Exception = win32Exception1;
					IMessageDisplayService messageDisplayService = base.Services.MessageDisplayService();
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string projectBuilderLaunchFailedDialogMessage = StringTable.ProjectBuilderLaunchFailedDialogMessage;
					object[] fileName = new object[] { processStartInfo.FileName, win32Exception.ToString() };
					messageDisplayService.ShowError(string.Format(currentCulture, projectBuilderLaunchFailedDialogMessage, fileName));
				}
			}
			if (process == null || process.HasExited)
			{
				this.SetRunning(false);
			}
			else
			{
				process.EnableRaisingEvents = true;
				process.Exited += new EventHandler(this.Process_Exited);
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectRun);
			return process != null;
		}

		public override T GetCapability<T>(string name)
		{
			string str = name;
			string str1 = str;
			if (str != null && (str1 == "CanHaveStartupItem" || str1 == "CanBeStartupProject"))
			{
				return Microsoft.Expression.Framework.Interop.TypeHelper.ConvertType<T>(true);
			}
			return base.GetCapability<T>(name);
		}

		public override IDocumentType GetDocumentType(string fileName)
		{
			IList<IDocumentType> documentTypes = new List<IDocumentType>(2)
			{
				base.Services.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.LimitedXaml],
				base.Services.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.DeepZoom]
			};
			return base.GetDocumentType(fileName, documentTypes);
		}

		private Uri GetPotentialServerLocation()
		{
			Uri uri;
			ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
			if (currentSolution != null)
			{
				object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "Port");
				if (projectProperty is int && Uri.TryCreate(string.Concat("http://localhost:", (int)projectProperty, "/"), UriKind.Absolute, out uri))
				{
					return uri;
				}
			}
			return new Uri("http://localhost/", UriKind.Absolute);
		}

		private IProjectItem GetUpdatedItem(IProjectItem item, FileInfo fileInfo, CreationOptions creationOptions)
		{
			IDocumentType documentType = this.GetDocumentType(item.DocumentReference.Path);
			if (item.DocumentType == documentType)
			{
				return null;
			}
			if (item.DocumentType == base.Services.DocumentTypes()[DocumentTypeNamesHelper.Xml] && documentType == base.Services.DocumentTypes()[DocumentTypeNamesHelper.DeepZoom])
			{
				string[] directoryExtensions = DeepZoomHelper.GetDirectoryExtensions(item.DocumentReference.Path);
				for (int i = 0; i < (int)directoryExtensions.Length; i++)
				{
					string str = directoryExtensions[i];
					Microsoft.Expression.Framework.Documents.DocumentReference documentReference = DeepZoomHelper.CreateDeepZoomDirectoryReference(item.DocumentReference, str);
					IProjectItem projectItem = base.FindItem(documentReference);
					if (projectItem != null)
					{
						this.RemoveProjectItem(projectItem, false);
					}
				}
			}
			this.RemoveProjectItem(item, false);
			DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
			{
				TargetPath = fileInfo.FullName,
				CreationOptions = creationOptions,
				DocumentType = documentType
			};
			return base.AddItem(documentCreationInfo);
		}

		protected override bool Initialize()
		{
			bool startupItem;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectPopulate, "InitializeExistingProject");
			string path = base.DocumentReference.Path;
			if (Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).CountIsMoreThan<string>(1000))
			{
				MessageBoxArgs messageBoxArg = new MessageBoxArgs()
				{
					Message = StringTable.FolderOpenTooManyFilesWarning,
					Button = MessageBoxButton.YesNo,
					Image = MessageBoxImage.Exclamation
				};
				if (base.Services.ShowSuppressibleWarning(messageBoxArg, "WebsiteTooLargeWarning", MessageBoxResult.Yes) == MessageBoxResult.No)
				{
					return false;
				}
			}
			this.LoadDirectory(path, null);
			if (this.StartupItem == null)
			{
				bool flag = false;
				VSWebsitesHelper vSWebsitesHelper = new VSWebsitesHelper();
				if (vSWebsitesHelper != null)
				{
					VSWebsitesWebsite vSWebsitesWebsite = vSWebsitesHelper.FindWebsite(path);
					if (vSWebsitesWebsite != null)
					{
						if (vSWebsitesWebsite.StartPage == null)
						{
							startupItem = true;
						}
						else
						{
							this.StartupItem = base.FindItem(Microsoft.Expression.Framework.Documents.DocumentReference.Create(vSWebsitesWebsite.StartPageFullPath));
							startupItem = this.StartupItem != null;
						}
						if (startupItem && vSWebsitesWebsite.VwdPort != 0)
						{
							this.cachedVswdPort = new int?(vSWebsitesWebsite.VwdPort);
						}
						flag = true;
					}
				}
				if (!flag)
				{
					this.StartupItem = base.FindItem(Microsoft.Expression.Framework.Documents.DocumentReference.Create(Microsoft.Expression.Framework.Documents.PathHelper.ResolveCombinedPath(path, "default.html")));
				}
			}
			PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectPopulate, "InitializeExistingProject");
			return true;
		}

		private bool IsPortAvailable(int port)
		{
			bool flag;
			try
			{
				IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
				int num = 0;
				while (num < (int)activeTcpListeners.Length)
				{
					if (activeTcpListeners[num].Port != port)
					{
						num++;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				return true;
			}
			catch (NetworkInformationException networkInformationException)
			{
				flag = false;
			}
			return flag;
		}

		private void LoadDirectory(string directoryPath, IProjectItem parentItem)
		{
			FileInfo[] files = (new DirectoryInfo(directoryPath)).GetFiles();
			for (int i = 0; i < (int)files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				if ((fileInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(fileInfo.FullName);
					IDocumentType documentType = this.GetDocumentType(fileInfo.FullName);
					this.AddItemDuringLoad(documentType, documentReference, parentItem);
				}
			}
			string[] directories = Directory.GetDirectories(directoryPath);
			for (int j = 0; j < (int)directories.Length; j++)
			{
				string str = directories[j];
				if (((new DirectoryInfo(str)).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					IDocumentType item = base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder];
					string str1 = string.Concat(str, Path.DirectorySeparatorChar);
					FolderProjectItem folderProjectItem = this.AddItemDuringLoad(item, Microsoft.Expression.Framework.Documents.DocumentReference.Create(str1), parentItem) as FolderProjectItem;
					if (this.ShouldInitializeFolder(folderProjectItem))
					{
						this.LoadDirectory(str, folderProjectItem);
					}
				}
			}
		}

		private void PlaceItemInFolder(IProjectItem projectItem)
		{
			string parentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(projectItem.DocumentReference.Path);
			if (!string.IsNullOrEmpty(parentDirectory))
			{
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(parentDirectory);
				if (base.DocumentReference.Equals(documentReference))
				{
					return;
				}
				IProjectItem item = base.Items[documentReference.GetHashCode()];
				if (item == null)
				{
					item = new FolderProjectItem(this, documentReference, base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder], base.Services);
					base.AddProjectItem(item);
				}
				if (item.CanAddChildren)
				{
					item.AddChild(projectItem);
				}
			}
		}

		private void Process_Exited(object sender, EventArgs args)
		{
			UIThreadDispatcher.Instance.Invoke(DispatcherPriority.Normal, () => this.SetRunning(false));
		}

		private void ProcessItem(IProjectItem item)
		{
			if (item.Children.Any<IProjectItem>())
			{
				item.Children.ToList<IProjectItem>().ForEach(new Action<IProjectItem>(this.ProcessItem));
			}
			ProjectItem projectItem = item as ProjectItem;
			if (projectItem != null && !item.IsVirtual && !item.IsLinkedFile)
			{
				projectItem.UpdateFileInformation();
			}
			if (!item.FileExists)
			{
				this.RemoveProjectItem(item, false);
				return;
			}
			if (projectItem != null && (projectItem.FileInformation.FileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
			{
				this.RemoveProjectItem(item, false);
			}
		}

		protected override void ProcessNewlyAddedProjectItem(IProjectItem projectItem)
		{
			this.PlaceItemInFolder(projectItem);
			base.OnItemAdded(new ProjectItemEventArgs(projectItem));
		}

		public override void Refresh()
		{
		}

		public void RefreshChildren(IDocumentItem item, bool selectNewlyCreatedItems)
		{
			if (item == null)
			{
				return;
			}
			if (item == this)
			{
				(
					from i in base.Items
					where i.Parent == null
					select i).ToList<IProjectItem>().ForEach(new Action<IProjectItem>(this.ProcessItem));
				this.RefreshDirectory(base.ProjectRoot.Path, selectNewlyCreatedItems);
				return;
			}
			IProjectItem projectItem = item as IProjectItem;
			if (projectItem == null || projectItem.Project != this)
			{
				return;
			}
			this.ProcessItem(projectItem);
			if (projectItem.CanAddChildren)
			{
				this.RefreshDirectory(projectItem.DocumentReference.Path, selectNewlyCreatedItems);
			}
		}

		private void RefreshDirectory(string directoryPath, bool selectNewlyCreatedItems)
		{
			if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryPath))
			{
				return;
			}
			CreationOptions creationOption = (selectNewlyCreatedItems ? CreationOptions.None : CreationOptions.DoNotSelectCreatedItems);
			foreach (FileInfo fileInfo in (new DirectoryInfo(directoryPath)).EnumerateFiles())
			{
				if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
				{
					continue;
				}
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference = Microsoft.Expression.Framework.Documents.DocumentReference.Create(fileInfo.FullName);
				IProjectItem projectItem = base.FindItem(documentReference);
				if (projectItem != null)
				{
					IProjectItem updatedItem = this.GetUpdatedItem(projectItem, fileInfo, creationOption);
					projectItem = (updatedItem != null ? updatedItem : projectItem);
				}
				else
				{
					IDocumentType documentType = this.GetDocumentType(fileInfo.FullName);
					DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
					{
						TargetPath = fileInfo.FullName,
						CreationOptions = creationOption,
						DocumentType = documentType
					};
					projectItem = base.CreateProjectItemIfNeeded(documentCreationInfo);
					if (projectItem == null)
					{
						continue;
					}
					this.AddProjectItem(projectItem, ProjectItemEventOptions.None);
				}
			}
			foreach (string str in Directory.EnumerateDirectories(directoryPath))
			{
				if (!this.ShouldInitializeFolder(base.FindItem(Microsoft.Expression.Framework.Documents.DocumentReference.Create(str)) as FolderProjectItem))
				{
					continue;
				}
				this.RefreshDirectory(str, selectNewlyCreatedItems);
			}
			if (directoryPath != base.ProjectRoot.Path)
			{
				Microsoft.Expression.Framework.Documents.DocumentReference documentReference1 = Microsoft.Expression.Framework.Documents.DocumentReference.Create(string.Concat(directoryPath, Path.DirectorySeparatorChar));
				if (base.FindItem(documentReference1) == null)
				{
					IProjectItem folderProjectItem = new FolderProjectItem(this, documentReference1, base.Services.DocumentTypes()[DocumentTypeNamesHelper.Folder], base.Services);
					base.AddProjectItem(folderProjectItem);
				}
			}
		}

		protected override void RemoveProjectItem(IProjectItem projectItem, bool deleteFiles)
		{
			try
			{
				if (projectItem.Children.Any<IProjectItem>())
				{
					base.RemoveProjectItems(deleteFiles, projectItem.Children.ToArray<IProjectItem>());
				}
				base.RemoveProjectItem(projectItem, deleteFiles);
				if (projectItem.IsOpen)
				{
					projectItem.CloseDocument();
				}
				base.OnItemRemoved(new ProjectItemEventArgs(projectItem));
				if (deleteFiles)
				{
					base.DeleteProjectItem(projectItem);
				}
			}
			finally
			{
				projectItem.Dispose();
			}
		}

		public override void ReportChangedItem(IProjectItem projectItem, ProjectItemEventOptions options)
		{
			if (Path.GetExtension(projectItem.DocumentReference.Path).ToUpperInvariant() == ".XAML")
			{
				IProjectItem projectItem1 = base.Items.FindMatchByUrl<IProjectItem>(base.GetResourcePathForFile(projectItem.DocumentReference.Path));
				if (projectItem1 != null)
				{
					this.RefreshChildren(projectItem1, true);
				}
			}
			base.ReportChangedItem(projectItem, options);
		}

		private void SetRunning(bool value)
		{
			this.isExecuting = value;
		}

		private bool ShouldInitializeFolder(FolderProjectItem item)
		{
			if (item == null)
			{
				return true;
			}
			return !item.IsUIBlockingFolder;
		}
	}
}