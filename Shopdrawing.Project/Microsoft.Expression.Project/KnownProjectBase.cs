// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Project.KnownProjectBase
// Assembly: Microsoft.Expression.Project, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 80357D9B-A7D7-4011-8FBC-3E1052652ADC
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Project.dll

using Microsoft.Build.Evaluation;
using Microsoft.Expression.Extensibility.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Collections;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.UserInterface;
using Microsoft.VisualStudio.Silverlight;
using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
    public abstract class KnownProjectBase : ProjectBase, IProject, INamedProject, IDocumentItem, IDisposable, INotifyPropertyChanged, IFileWatcher
    {
        private HashSet<string> cachedAssemblyResolveFailures = new HashSet<string>();
        private HashSet<KnownProjectBase.DesignTimeAssemblyLookupInformation> dirtyDesignMetadataAssemblies = new HashSet<KnownProjectBase.DesignTimeAssemblyLookupInformation>();
        private Dictionary<string, Assembly> designAssembliesForSourceAssemblies = new Dictionary<string, Assembly>();
        private HashSet<string> currentlyLoadingDesignAssemblies = new HashSet<string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        private List<string> deferredAssemblyAddRequests = new List<string>();
        private static readonly string ResourceFolderExtension = "_Files";
        private static Dictionary<string, Assembly> designAssemblies = new Dictionary<string, Assembly>();
        private static byte[] sharedLibraryPublicKey = new byte[8]
    {
      (byte) 49,
      (byte) 191,
      (byte) 56,
      (byte) 86,
      (byte) 173,
      (byte) 54,
      (byte) 78,
      (byte) 53
    };
        private static byte[] microsoftPublicKey = new byte[8]
    {
      (byte) 176,
      (byte) 63,
      (byte) 95,
      (byte) 127,
      (byte) 17,
      (byte) 213,
      (byte) 10,
      (byte) 58
    };
        private static byte[] ecmaPublicKey = new byte[8]
    {
      (byte) 183,
      (byte) 122,
      (byte) 92,
      (byte) 86,
      (byte) 25,
      (byte) 52,
      (byte) 224,
      (byte) 137
    };
        private static readonly string[] KnownCapabilities = new string[17]
    {
      "ExpressionBlendPrototypingEnabled",
      "ExpressionBlendPrototypeHarness",
      "CanAddAssemblyReference",
      "CanAddProjectReference",
      "CanAddLinks",
      "CanHaveStartupItem",
      "SupportsDatabinding",
      "SupportsMediaElementControl",
      "SupportsHyperlinkButtonControl",
      "PlatformSimpleName",
      "SupportsUIElementEffectProperty",
      "SupportsAssetLibraryBehaviorsItems",
      "SupportsTransparentAssemblyCache",
      "SupportsEnableOutOfBrowser",
      "CanBeStartupProject",
      "SourceControlBound",
      "DeployedProject"
    };
        private static Dictionary<string, byte[]> designAssemblyBlackList = new Dictionary<string, byte[]>()
    {
      {
        "System",
        KnownProjectBase.ecmaPublicKey
      },
      {
        "System.Drawing",
        KnownProjectBase.microsoftPublicKey
      },
      {
        "Microsoft.Windows",
        KnownProjectBase.microsoftPublicKey
      },
      {
        "Microsoft.VisualStudio",
        KnownProjectBase.microsoftPublicKey
      },
      {
        "Microsoft.VisualStudio.Shell",
        KnownProjectBase.microsoftPublicKey
      },
      {
        "Microsoft.ReportViewer",
        KnownProjectBase.microsoftPublicKey
      },
      {
        "System.Data.Entity",
        KnownProjectBase.ecmaPublicKey
      },
      {
        "System.Data.Services",
        KnownProjectBase.ecmaPublicKey
      },
      {
        "System.Web.DynamicData",
        KnownProjectBase.sharedLibraryPublicKey
      },
      {
        "System.Web.Entity",
        KnownProjectBase.ecmaPublicKey
      },
      {
        "System.Web.Extensions",
        KnownProjectBase.sharedLibraryPublicKey
      }
    };
        private IndexedHashSet<IProjectItem> items;
        private ReadOnlyIndexedHashSet<IProjectItem> readOnlyItems;
        internal ProjectWatcher projectWatcher;
        private bool updateAssemblies;
        private bool updateChangedOrDeletedItems;
        private int assemblyNotificationNesting;
        private KnownProjectBase.AssemblyCollection referencedAssemblies;
        private Assembly currentlyLoadingDesignAssembly;
        private bool referenceAssemblyContextInitialized;
        private Microsoft.Expression.Project.ReferenceAssemblyContext referenceAssemblyContext;
        private static IMetadataStore metadataStore;

        protected int ImplicitReferencedAssembliesCount { get; set; }

        protected bool ProjectAssemblyReferencesDirty { get; set; }

        protected BuildTaskInfoPopulator BuildTaskInfoPopulator { get; set; }

        private AssemblyService AssemblyService
        {
            get
            {
                return (AssemblyService)base.Services.AssemblyService();
            }
        }

        public IAssemblyCollection ReferencedAssemblies
        {
            get
            {
                return (IAssemblyCollection)this.referencedAssemblies;
            }
        }

        public abstract string UICulture { get; set; }

        public abstract ICodeDocumentType CodeDocumentType { get; }

        public abstract string DefaultNamespaceName { get; }

        public abstract IProjectItem StartupItem { get; set; }

        protected abstract IDocumentType StartupDocumentType { get; }

        public abstract IProjectType ProjectType { get; }

        public virtual string PropertiesPath
        {
            get
            {
                if (this.CodeDocumentType != null)
                    return this.CodeDocumentType.PropertiesPath;
                return (string)null;
            }
        }

        public abstract FrameworkName TargetFramework { get; }

        public virtual string TemplateProjectType
        {
            get
            {
                return (string)null;
            }
        }

        public virtual ICollection<string> TemplateProjectSubtypes
        {
            get
            {
                ICollection<string> collection = (ICollection<string>)new HashSet<string>();
                string capability = this.GetCapability<string>("PlatformSimpleName");
                if (!string.IsNullOrEmpty(capability))
                    collection.Add(capability);
                return collection;
            }
        }

        public string RelativePath
        {
            get
            {
                return this.DocumentReference.GetRelativePath(ServiceExtensions.ProjectManager(this.Services).CurrentSolution.DocumentReference);
            }
        }

        public virtual bool ShouldProduceProjectOutputReferences
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShouldReceiveProjectOutputReferences
        {
            get
            {
                return false;
            }
        }

        Microsoft.Expression.Framework.IReadOnlyCollection<IProjectItem> IProject.Items
        {
            get
            {
                if (this.IsDisposed)
                    return (Microsoft.Expression.Framework.IReadOnlyCollection<IProjectItem>)new ReadOnlyIndexedHashSet<IProjectItem>((IIndexedHashSet<IProjectItem>)new IndexedHashSet<IProjectItem>());
                return (Microsoft.Expression.Framework.IReadOnlyCollection<IProjectItem>)this.readOnlyItems;
            }
        }

        protected ReadOnlyIndexedHashSet<IProjectItem> Items
        {
            get
            {
                return this.readOnlyItems;
            }
        }

        public override IEnumerable<IDocumentItem> Descendants
        {
            get
            {
                return Enumerable.Cast<IDocumentItem>((IEnumerable)this.readOnlyItems);
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<IProject> ReferencedProjects
        {
            get
            {
                ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
                if (currentSolution == null)
                {
                    return Enumerable.Empty<IProject>();
                }
                IEnumerable<IProject> referencedAssemblies =
                    from assembly in this.ReferencedAssemblies
                    where assembly.ProjectItem is ProjectReferenceProjectItem
                    join project in currentSolution.Projects on assembly.ProjectItem.DocumentReference.GetHashCode() equals project.DocumentReference.GetHashCode()
                    select project;
                return referencedAssemblies;
            }
        }

        protected IExternalChanges ExternalChanges
        {
            get
            {
                return base.Services.ProjectManager() as IExternalChanges;
            }
        }

        private bool DeferredAssemblyRequestsNeedUpdated
        {
            get
            {
                if (this.deferredAssemblyAddRequests != null)
                    return EnumerableExtensions.CountIsMoreThan<string>((IEnumerable<string>)this.deferredAssemblyAddRequests, 0);
                return false;
            }
        }

        private bool ImplicitReferencedAssembliesNeedUpdated
        {
            get
            {
                IProjectManager projectManager = base.Services.ProjectManager();
                if (projectManager == null)
                {
                    return false;
                }
                return projectManager.ImplicitAssemblyReferences.Count > this.ImplicitReferencedAssembliesCount;
            }
        }

        public string Configuration
        {
            get
            {
                string str = this.ProjectStore.GetProperty("Configuration");
                if (string.IsNullOrEmpty(str))
                    str = "Debug";
                return str;
            }
        }

        public new IProjectStore ProjectStore
        {
            get
            {
                return base.ProjectStore;
            }
        }

        public ProjectAssembly TargetAssembly
        {
            get
            {
                return this.referencedAssemblies[(object)this];
            }
            protected set
            {
                if (value == null)
                    this.referencedAssemblies.Remove((object)this);
                else
                    this.referencedAssemblies[(object)this] = value;
            }
        }

        public virtual string FullTargetPath
        {
            get
            {
                string property1 = this.ProjectStore.GetProperty("OutputPath");
                string property2 = this.ProjectStore.GetProperty("TargetFileName");
                if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(property1) || !Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(property2))
                    return (string)null;
                return Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(this.DocumentReference.Path), Path.Combine(property1, property2));
            }
        }

        public IReferenceAssemblyContext ReferenceAssemblyContext
        {
            get
            {
                if (!this.referenceAssemblyContextInitialized)
                {
                    this.referenceAssemblyContext = this.AssemblyService.CreateReferenceAssemblyContext((IProject)this);
                    if (this.referenceAssemblyContext != null)
                    {
                        foreach (ProjectAssembly projectAssembly in (IEnumerable<ProjectAssembly>)this.ReferencedAssemblies)
                            this.referenceAssemblyContext.UpdateReferenceAssembly(projectAssembly);
                    }
                    this.referenceAssemblyContextInitialized = true;
                }
                return (IReferenceAssemblyContext)this.referenceAssemblyContext;
            }
        }

        public static IMetadataStore MetadataStore
        {
            get
            {
                return KnownProjectBase.metadataStore;
            }
            set
            {
                KnownProjectBase.metadataStore = value;
            }
        }

        internal ProjectWatcher ProjectWatcher
        {
            get
            {
                if (this.IsDisposed)
                    return (ProjectWatcher)null;
                if (this.projectWatcher == null)
                    this.projectWatcher = new ProjectWatcher(this);
                return this.projectWatcher;
            }
        }

        public event EventHandler<ProjectItemEventArgs> ItemAdded;

        public event EventHandler<ProjectItemEventArgs> ItemRemoved;

        public event EventHandler<ProjectItemRenamedEventArgs> ItemRenamed;

        public event EventHandler<ProjectItemEventArgs> ItemChanged;

        public event EventHandler<ProjectItemEventArgs> ItemDeleted;

        public event EventHandler<ProjectEventArgs> ProjectChanged;

        public event EventHandler<ProjectEventArgs> ProcessingProjectChanges;

        public event EventHandler<ProjectEventArgs> ProcessingProjectChangesComplete;

        public event EventHandler<ProjectItemEventArgs> ItemOpened;

        public event EventHandler<ProjectItemEventArgs> ItemClosing;

        public event EventHandler<ProjectItemEventArgs> ItemClosed;

        public event EventHandler<ProjectItemChangedEventArgs> StartupItemChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected KnownProjectBase(IProjectStore projectStore, IServiceProvider serviceProvider)
            : base(projectStore, serviceProvider)
        {
            this.ImplicitReferencedAssembliesCount = 2147483647;
            this.items = new IndexedHashSet<IProjectItem>(new DocumentItemUrlComparer<IProjectItem>());
            this.readOnlyItems = new ReadOnlyIndexedHashSet<IProjectItem>(this.items);
            this.referencedAssemblies = new KnownProjectBase.AssemblyCollection();
            ((ProjectManager)base.Services.ProjectManager()).SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.AppDomain_AssemblyResolve);
            this.BuildTaskInfoPopulator = new BuildTaskInfoPopulator();
        }

        protected static byte[] GetMicrosoftPublicKey()
        {
            return (byte[])KnownProjectBase.microsoftPublicKey.Clone();
        }

        protected abstract bool Initialize();

        protected static KnownProjectBase TryCreate(Func<KnownProjectBase> constructorCall)
        {
            KnownProjectBase knownProjectBase;
            try
            {
                knownProjectBase = constructorCall();
            }
            catch
            {
                throw;
            }
            try
            {
                if (!knownProjectBase.Initialize())
                {
                    knownProjectBase.Dispose();
                    knownProjectBase = (KnownProjectBase)null;
                }
            }
            catch
            {
                knownProjectBase.Dispose();
                throw;
            }
            return knownProjectBase;
        }

        protected abstract IEnumerable<IProjectItem> AddAssetFolder(string sourceFolder, string targetFolder, bool link);

        public abstract void Refresh();

        public abstract IProjectItem AddAssemblyReference(string path, bool verifyFileExists);

        public abstract IProjectItem AddProjectReference(IProject project);

        public abstract void AddImport(string path);

        public bool AttemptToMakeProjectWritable()
        {
            if (!Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(this.DocumentReference.Path))
                return ProjectPathHelper.AttemptToMakeWritable(this.DocumentReference, this.Services);
            return true;
        }

        public void AddAssemblyReferenceDeferred(string path)
        {
            if (this.deferredAssemblyAddRequests == null || this.deferredAssemblyAddRequests.Contains(path))
                return;
            this.deferredAssemblyAddRequests.Add(path);
        }

        public bool RemoveItems(bool deleteFiles, params IProjectItem[] projectItems)
        {
            IProjectItem[] projectItemArray = Enumerable.ToArray<IProjectItem>(Enumerable.Where<IProjectItem>((IEnumerable<IProjectItem>)projectItems, (Func<IProjectItem, bool>)(projectItem => projectItem != null)));
            if (EnumerableExtensions.CountIs<IProjectItem>((IEnumerable<IProjectItem>)projectItemArray, 0))
                return true;
            if (!this.AttemptToMakeProjectWritable())
                return false;
            this.RemoveProjectItems(deleteFiles, projectItemArray);
            return this.ImplicitSave();
        }

        public virtual IDocumentType GetDocumentType(string fileName)
        {
            return this.GetDocumentType(fileName, (IList<IDocumentType>)null);
        }

        protected IDocumentType GetDocumentType(string fileName, IList<IDocumentType> overrideChain)
        {
            try
            {
                Path.GetExtension(fileName);
            }
            catch (ArgumentException ex)
            {
                return (IDocumentType)null;
            }
            List<IDocumentType> list = new List<IDocumentType>();
            foreach (IDocumentType documentType in (IEnumerable<IDocumentType>)DocumentServiceExtensions.DocumentTypes(this.Services))
            {
                if (documentType.IsDocumentTypeOf(fileName))
                    list.Add(documentType);
            }
            if (EnumerableExtensions.CountIsMoreThan<IDocumentType>((IEnumerable<IDocumentType>)list, 0))
            {
                IDocumentType matchingDocumentType = this.FindFirstMatchingDocumentType((IList<IDocumentType>)list, overrideChain);
                if (matchingDocumentType != null)
                    return matchingDocumentType;
                IDocumentType defaultDocumentType = this.FindFirstDefaultDocumentType((IList<IDocumentType>)list);
                if (defaultDocumentType != null)
                    return defaultDocumentType;
            }
            return DocumentServiceExtensions.DocumentTypes(this.Services).UnknownDocumentType;
        }

        private IDocumentType FindFirstDefaultDocumentType(IList<IDocumentType> respondingDocumentTypes)
        {
            foreach (IDocumentType documentType in (IEnumerable<IDocumentType>)respondingDocumentTypes)
            {
                if (documentType.IsDefaultTypeForExtension)
                    return documentType;
            }
            return (IDocumentType)null;
        }

        private IDocumentType FindFirstMatchingDocumentType(IList<IDocumentType> respondingDocumentTypes, IList<IDocumentType> precedenceChain)
        {
            if (precedenceChain != null)
            {
                foreach (IDocumentType documentType1 in (IEnumerable<IDocumentType>)precedenceChain)
                {
                    foreach (IDocumentType documentType2 in (IEnumerable<IDocumentType>)respondingDocumentTypes)
                    {
                        if (documentType1 == documentType2)
                            return documentType1;
                    }
                }
            }
            return (IDocumentType)null;
        }

        private IEnumerable<DocumentCreationInfo> FillOutDocumentCreationInfo(IEnumerable<DocumentCreationInfo> creationInfo)
        {
            IEnumerable<DocumentCreationInfo> documentCreationInfos = creationInfo.Select<DocumentCreationInfo, DocumentCreationInfo>((DocumentCreationInfo info) =>
            {
                if (string.IsNullOrEmpty(info.TargetPath) && !string.IsNullOrEmpty(info.SourcePath))
                {
                    string targetFolder = info.TargetFolder;
                    if (string.IsNullOrEmpty(targetFolder))
                    {
                        targetFolder = base.Services.ProjectManager().TargetFolderForProject(this);
                    }
                    try
                    {
                        info.TargetPath = Path.Combine(targetFolder, Path.GetFileName(info.SourcePath));
                    }
                    catch (ArgumentException argumentException)
                    {
                    }
                }
                if (string.IsNullOrEmpty(info.SourcePath))
                {
                    info.SourcePath = info.TargetPath;
                }
                if (info.DocumentType == null)
                {
                    info.DocumentType = this.GetDocumentType(info.SourcePath);
                }
                return info;
            });
            return this.BuildTaskInfoPopulator.PopulateBuildTaskInfo(documentCreationInfos, this);
        }

        private IEnumerable<DocumentCreationInfo> AddCodeBehindFiles(IEnumerable<DocumentCreationInfo> creationInfo, string codeBehindExtension)
        {
            return Enumerable.Union<DocumentCreationInfo>(creationInfo, Enumerable.Where<DocumentCreationInfo>(Enumerable.Select<DocumentCreationInfo, DocumentCreationInfo>(Enumerable.Where<DocumentCreationInfo>(creationInfo, (Func<DocumentCreationInfo, bool>)(info => info.TargetPath.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))), (Func<DocumentCreationInfo, DocumentCreationInfo>)(info =>
            {
                info.TargetPath = Path.ChangeExtension(info.TargetPath, codeBehindExtension);
                info.SourcePath = Path.ChangeExtension(info.SourcePath, codeBehindExtension);
                info.DocumentType = this.GetDocumentType(info.TargetPath);
                info.BuildTaskInfo = info.DocumentType == null ? (BuildTaskInfo)null : info.DocumentType.DefaultBuildTaskInfo;
                return info;
            })), (Func<DocumentCreationInfo, bool>)(info => Microsoft.Expression.Framework.Documents.PathHelper.FileExists(info.SourcePath))), (IEqualityComparer<DocumentCreationInfo>)new KnownProjectBase.TargetComparer());
        }

        private bool ConflictsWithServer(IEnumerable<DocumentCreationInfo> creationInfo)
        {
            if (!base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
            {
                return false;
            }
            DocumentCreationInfo[] array = creationInfo.Where<DocumentCreationInfo>((DocumentCreationInfo info) =>
            {
                if (PathHelper.FileExists(info.TargetPath))
                {
                    return false;
                }
                return base.Services.SourceControlProvider().ExistsOnServer(info.TargetPath);
            }).ToArray<DocumentCreationInfo>();
            if ((int)array.Length == 0)
            {
                return false;
            }
            if ((int)array.Length != 1)
            {
                StringBuilder stringBuilder = new StringBuilder(128);
                DocumentCreationInfo[] documentCreationInfoArray = array;
                for (int i = 0; i < (int)documentCreationInfoArray.Length; i++)
                {
                    DocumentCreationInfo documentCreationInfo = documentCreationInfoArray[i];
                    stringBuilder.AppendLine(PathHelper.GetFileOrDirectoryName(documentCreationInfo.TargetPath));
                }
                IMessageDisplayService messageDisplayService = base.Services.MessageDisplayService();
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string serverConflictMultipleMessage = StringTable.ServerConflictMultipleMessage;
                object[] str = new object[] { stringBuilder.ToString() };
                messageDisplayService.ShowError(string.Format(currentCulture, serverConflictMultipleMessage, str));
            }
            else
            {
                IMessageDisplayService messageDisplayService1 = base.Services.MessageDisplayService();
                CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                string serverConflictSingleMessage = StringTable.ServerConflictSingleMessage;
                object[] fileOrDirectoryName = new object[] { PathHelper.GetFileOrDirectoryName(array[0].TargetPath) };
                messageDisplayService1.ShowError(string.Format(cultureInfo, serverConflictSingleMessage, fileOrDirectoryName));
            }
            return true;
        }

        private bool AllowOverwrites(IEnumerable<DocumentCreationInfo> creationInfo)
        {
            bool flag = false;
            DocumentCreationInfo[] array = creationInfo.Where<DocumentCreationInfo>((DocumentCreationInfo info) =>
            {
                if ((info.CreationOptions & CreationOptions.SilentlyOverwriteReadOnly) == CreationOptions.SilentlyOverwriteReadOnly)
                {
                    return false;
                }
                if (string.Compare(info.SourcePath, info.TargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return false;
                }
                if (!PathHelper.FileExists(info.TargetPath))
                {
                    return false;
                }
                if ((File.GetAttributes(info.TargetPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    if ((info.CreationOptions & CreationOptions.DoNotAllowOverwrites) == CreationOptions.DoNotAllowOverwrites)
                    {
                        flag = true;
                    }
                    return true;
                }
                if ((info.CreationOptions & CreationOptions.SilentlyOverwrite) == CreationOptions.SilentlyOverwrite)
                {
                    return false;
                }
                if ((info.CreationOptions & CreationOptions.DoNotAllowOverwrites) == CreationOptions.DoNotAllowOverwrites)
                {
                    flag = true;
                }
                return true;
            }).ToArray<DocumentCreationInfo>();
            if ((int)array.Length > 0)
            {
                OverwriteFilesDialog overwriteFilesDialog = new OverwriteFilesDialog(
                    from info in (IEnumerable<DocumentCreationInfo>)array
                    select info.TargetPath, !flag, base.Services.ExpressionInformationService());
                overwriteFilesDialog.InitializeDialog();
                bool? nullable = overwriteFilesDialog.ShowDialog();
                if (flag)
                {
                    return false;
                }
                if (nullable.HasValue)
                {
                    bool? nullable1 = nullable;
                    if ((nullable1.GetValueOrDefault() ? true : !nullable1.HasValue))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        public IProjectItem AddItem(DocumentCreationInfo creationInfo)
        {
            return Enumerable.FirstOrDefault<IProjectItem>(this.AddItems(EnumerableExtensions.AsEnumerable<DocumentCreationInfo>(creationInfo)));
        }

        public IEnumerable<IProjectItem> AddItems(IEnumerable<DocumentCreationInfo> creationInfo)
        {
            IEnumerable<IProjectItem> projectItems;
            using (IDisposable disposable = TemporaryCursor.SetWaitCursor())
            {
                creationInfo = this.FillOutDocumentCreationInfo(creationInfo).ToArray<DocumentCreationInfo>();
                creationInfo = this.AddCodeBehindFiles(creationInfo, string.Concat(".xaml", this.CodeDocumentType.DefaultFileExtension)).ToArray<DocumentCreationInfo>();
                if (creationInfo.CountIs<DocumentCreationInfo>(0))
                {
                    projectItems = Enumerable.Empty<IProjectItem>();
                }
                else if (this.ShouldCancelOnReadOnly(creationInfo))
                {
                    projectItems = Enumerable.Empty<IProjectItem>();
                }
                else if (this.ConflictsWithServer(creationInfo) || !this.AllowOverwrites(creationInfo))
                {
                    projectItems = Enumerable.Empty<IProjectItem>();
                }
                else
                {
                    List<IProjectItem> projectItems1 = new List<IProjectItem>();
                    List<IProjectItem> projectItems2 = new List<IProjectItem>();
                    IProjectManager directory = base.Services.ProjectManager();
                    IEnumerable<DocumentReference> documentReferences =
                        from info in creationInfo
                        where !info.TargetPath.Equals(info.SourcePath, StringComparison.OrdinalIgnoreCase)
                        select DocumentReference.Create(info.TargetPath);
                    this.UpdateSourceControl(documentReferences, UpdateSourceControlActions.Checkout);
                    foreach (DocumentCreationInfo documentCreationInfo in creationInfo)
                    {
                        if ((documentCreationInfo.CreationOptions & CreationOptions.LinkSourceFile) == CreationOptions.LinkSourceFile && !this.GetCapability<bool>("CanAddLinks") || !this.CopyIfNecessary(documentCreationInfo))
                        {
                            continue;
                        }
                        IProjectItem projectItem = this.CreateProjectItemIfNeeded(documentCreationInfo);
                        if (projectItem == null)
                        {
                            continue;
                        }
                        ProjectItemEventOptions projectItemEventOption = ProjectItemEventOptions.None;
                        if ((documentCreationInfo.CreationOptions & CreationOptions.SilentlyOverwrite) == CreationOptions.SilentlyOverwrite)
                        {
                            projectItemEventOption = ProjectItemEventOptions.Silent;
                        }
                        if (!this.AddProjectItem(projectItem, projectItemEventOption))
                        {
                            projectItems2.Add(projectItem);
                        }
                        else
                        {
                            projectItems1.Add(projectItem);
                        }
                        if (Path.GetExtension(documentCreationInfo.SourcePath).ToUpperInvariant() == ".XAML")
                        {
                            string resourcePathForFile = this.GetResourcePathForFile(documentCreationInfo.SourcePath);
                            if (PathHelper.DirectoryExists(resourcePathForFile))
                            {
                                projectItems1.AddRange(this.AddAssetFolder(resourcePathForFile, this.GetResourcePathForFile(documentCreationInfo.TargetPath), false));
                            }
                        }
                        if ((documentCreationInfo.CreationOptions & CreationOptions.DoNotSetDefaultImportPath) != CreationOptions.DoNotSetDefaultImportPath)
                        {
                            directory.DefaultImportPath = PathHelper.GetDirectory(documentCreationInfo.SourcePath);
                        }
                        if ((documentCreationInfo.CreationOptions & CreationOptions.DoNotSelectCreatedItems) != CreationOptions.DoNotSelectCreatedItems)
                        {
                            directory.ItemSelectionSet.Clear();
                            directory.ItemSelectionSet.ToggleSelection(projectItem);
                        }
                        if ((documentCreationInfo.CreationOptions & CreationOptions.DesignTimeResource) != CreationOptions.DesignTimeResource)
                        {
                            continue;
                        }
                        projectItem.ContainsDesignTimeResources = true;
                    }
                    if (projectItems1.Count > 0)
                    {
                        this.ImplicitSave();
                        this.UpdateSourceControl(projectItems1, UpdateSourceControlActions.PendAdd);
                    }
                    projectItems1.AddRange(projectItems2);
                    projectItems = projectItems1;
                }
            }
            return projectItems;
        }

        private bool ShouldCancelOnReadOnly(IEnumerable<DocumentCreationInfo> creationInfo)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(Enumerable.FirstOrDefault<DocumentCreationInfo>(creationInfo, (Func<DocumentCreationInfo, bool>)(info => this.Items[Microsoft.Expression.Framework.Documents.PathHelper.GenerateHashFromPath(info.TargetPath)] == null)).TargetPath))
                flag = !this.AttemptToMakeProjectWritable();
            return flag;
        }

        private bool CopyIfNecessary(DocumentCreationInfo creationInfo)
        {
            IProjectItem projectItem = this.Items[Microsoft.Expression.Framework.Documents.PathHelper.GenerateHashFromPath(creationInfo.TargetPath)];
            if (projectItem != null && projectItem.IsLinkedFile)
            {
                MessagingServiceExtensions.DisplayFailureMessage(this.Services, string.Format((IFormatProvider)CultureInfo.CurrentCulture, StringTable.DialogAddFailedMessage, new object[1]
        {
          (object) creationInfo.TargetPath
        }), StringTable.DialogMessageLinkAlreadyExists);
                return false;
            }
            if ((creationInfo.CreationOptions & CreationOptions.LinkSourceFile) == CreationOptions.LinkSourceFile)
                return true;
            if (string.Compare(creationInfo.SourcePath, creationInfo.TargetPath, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            try
            {
                Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(creationInfo.TargetFolder);
                Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(creationInfo.TargetPath);
                if (creationInfo.TargetFolder != null && !Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(creationInfo.TargetFolder))
                    Directory.CreateDirectory(creationInfo.TargetFolder);
                if (Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(creationInfo.SourcePath) && !Microsoft.Expression.Framework.Documents.PathHelper.FileOrDirectoryExists(creationInfo.TargetPath))
                    Directory.CreateDirectory(creationInfo.TargetPath);
                if (!Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(creationInfo.SourcePath))
                    File.Copy(creationInfo.SourcePath, creationInfo.TargetPath, true);
                Microsoft.Expression.Framework.Documents.PathHelper.ClearFileOrDirectoryReadOnlyAttribute(creationInfo.TargetPath);
            }
            catch (Exception ex)
            {
                if (!ErrorHandling.ShouldHandleExceptions(ex))
                {
                    throw;
                }
                else
                {
                    if ((creationInfo.CreationOptions & CreationOptions.SilentlyHandleIOExceptions) != CreationOptions.SilentlyHandleIOExceptions)
                        MessagingServiceExtensions.DisplayFailureMessage(this.Services, string.Format((IFormatProvider)CultureInfo.CurrentCulture, StringTable.DialogAddFailedMessage, new object[1]
            {
              (object) creationInfo.TargetPath
            }), ex.Message);
                    return false;
                }
            }
            return true;
        }

        protected string GetResourcePathForFile(string path)
        {
            return Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(Path.Combine(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(path), this.GetResourceFolderName(path)));
        }

        private string GetResourceFolderName(string path)
        {
            return Path.GetFileNameWithoutExtension(path) + KnownProjectBase.ResourceFolderExtension;
        }

        protected IProjectItem CreateProjectItemIfNeeded(DocumentCreationInfo creationInfo)
        {
            IProjectItem projectItem = this.Items[Microsoft.Expression.Framework.Documents.PathHelper.GenerateHashFromPath(creationInfo.TargetPath)];
            if (projectItem == null)
            {
                projectItem = (creationInfo.CreationOptions & CreationOptions.LinkSourceFile) != CreationOptions.LinkSourceFile ? creationInfo.DocumentType.CreateProjectItem((IProject)this, DocumentReference.Create(creationInfo.TargetPath), this.Services) : creationInfo.DocumentType.CreateProjectItem((IProject)this, DocumentReference.Create(creationInfo.SourcePath), this.Services);
                this.ProcessNewlyCreatedProjectItem(projectItem, creationInfo);
            }
            return projectItem;
        }

        protected virtual void ProcessNewlyCreatedProjectItem(IProjectItem projectItem, DocumentCreationInfo creationInfo)
        {
        }

        protected abstract void ProcessNewlyAddedProjectItem(IProjectItem projectItem);

        protected void UpdateSourceControl(IEnumerable<IProjectItem> projectItems, UpdateSourceControlActions updateSourceControlAction)
        {
            if (projectItems == null)
                throw new ArgumentNullException("projectItems");
            this.UpdateSourceControl(Enumerable.Select<IProjectItem, DocumentReference>(projectItems, (Func<IProjectItem, DocumentReference>)(item => item.DocumentReference)), updateSourceControlAction);
        }

        protected void UpdateSourceControl(IEnumerable<DocumentReference> paths, UpdateSourceControlActions updateSourceControlAction)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            HandlerBasedProjectActionContext handlerBasedProjectActionContext = new HandlerBasedProjectActionContext(base.Services)
            {
                IsSourceControlledHandler = new Func<DocumentReference, bool>(this.IsWithinProjectRoot)
            };
            SourceControlHelper.UpdateSourceControl(paths, updateSourceControlAction, handlerBasedProjectActionContext);
        }

        public void PendAddItems(IEnumerable<IProjectItem> projectItems)
        {
            this.UpdateSourceControl(projectItems, UpdateSourceControlActions.PendAdd);
        }

        public virtual bool IsValidItemTemplate(IProjectItemTemplate itemTemplate)
        {
            Version version = this.TargetFramework == (FrameworkName)null ? (Version)null : this.TargetFramework.Version;
            if (!string.IsNullOrEmpty(itemTemplate.ProjectType) && itemTemplate.ProjectType != this.TemplateProjectType)
                return false;
            ICollection<string> templateProjectSubtypes = this.TemplateProjectSubtypes;
            if (!string.IsNullOrEmpty(itemTemplate.ProjectSubType) && templateProjectSubtypes != null && (Enumerable.Count<string>((IEnumerable<string>)templateProjectSubtypes) > 0 && !templateProjectSubtypes.Contains(itemTemplate.ProjectSubType)))
                return false;
            if (!string.IsNullOrEmpty(itemTemplate.ProjectSubTypes) && templateProjectSubtypes != null && Enumerable.Count<string>((IEnumerable<string>)templateProjectSubtypes) > 0)
            {
                if (!Enumerable.Contains<string>((IEnumerable<string>)itemTemplate.ProjectSubTypes.Split(';'), Enumerable.First<string>((IEnumerable<string>)templateProjectSubtypes)))
                    return false;
            }
            Version result1;
            Version result2;
            return (string.IsNullOrEmpty(itemTemplate.MinimumFrameworkVersion) || !Version.TryParse(itemTemplate.MinimumFrameworkVersion, out result1) || !(version == (Version)null) && !(result1 > version)) && (string.IsNullOrEmpty(itemTemplate.MaximumFrameworkVersion) || !Version.TryParse(itemTemplate.MaximumFrameworkVersion, out result2) || !(version == (Version)null) && !(result2 < version));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !base.IsDisposed)
            {
                if (this.projectWatcher != null)
                {
                    this.projectWatcher.DisableWatchingForChanges();
                }
                this.updateAssemblies = false;
                this.updateChangedOrDeletedItems = false;
                this.deferredAssemblyAddRequests = null;
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.AppDomain_AssemblyResolve);
                this.AssemblyService.ReleaseReferenceAssemblyContext(this.referenceAssemblyContext);
                this.referenceAssemblyContext = null;
                try
                {
                    try
                    {
                        IProjectItem projectItem = this.Items.FirstOrDefault<IProjectItem>((IProjectItem item) => item.Document == base.Services.DocumentService().ActiveDocument);
                        foreach (IProjectItem projectItem1 in (IEnumerable<IProjectItem>)this.Items)
                        {
                            if (projectItem1 == projectItem || !projectItem1.IsOpen)
                            {
                                continue;
                            }
                            projectItem1.CloseDocument();
                        }
                        if (projectItem != null && projectItem.IsOpen)
                        {
                            projectItem.CloseDocument();
                        }
                    }
                    catch (Exception exception)
                    {
                        throw;
                    }
                }
                finally
                {
                    this.items.Clear();
                    this.items = null;
                    if (this.projectWatcher != null)
                    {
                        this.projectWatcher.Dispose();
                        this.projectWatcher = null;
                    }
                }
                base.Dispose(disposing);
            }
        }

        public virtual bool IsValidStartupItem(IProjectItem projectItem)
        {
            if (projectItem != null)
                return projectItem.DocumentType == this.StartupDocumentType;
            return false;
        }

        protected void CreateFile(string filePath, string fileContents)
        {
            if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.UTF8);
                streamWriter.Write(fileContents);
                streamWriter.Close();
            }
        }

        protected internal bool ImplicitSave()
        {
            bool flag;
            try
            {
                PathHelper.ClearFileOrDirectoryReadOnlyAttribute(base.DocumentReference.Path);
                PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectSave);
                this.DisableWatchingForChanges();
                try
                {
                    this.SaveProjectFileInternal();
                }
                finally
                {
                    this.EnableWatchingForChanges();
                }
                PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectSave);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (!ErrorHandling.ShouldHandleExceptions(exception))
                {
                    throw;
                }
                IMessageDisplayService messageDisplayService = base.Services.MessageDisplayService();
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string projectSaveProjectFailedMessage = StringTable.ProjectSaveProjectFailedMessage;
                object[] path = new object[] { base.DocumentReference.Path, exception.Message };
                messageDisplayService.ShowError(string.Format(currentCulture, projectSaveProjectFailedMessage, path));
                return false;
            }
            return flag;
        }

        protected virtual void SaveProjectFileInternal()
        {
        }

        protected bool AddProjectItem(IProjectItem projectItem)
        {
            return this.AddProjectItem(projectItem, ProjectItemEventOptions.None);
        }

        protected virtual bool AddProjectItem(IProjectItem projectItem, ProjectItemEventOptions options)
        {
            PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectAddProjectItem);
            try
            {
                if (!this.items.Add(projectItem))
                {
                    ProjectItem projectItem1 = projectItem as ProjectItem;
                    if (projectItem1 != null)
                        projectItem1.UpdateFileInformation();
                    this.ReportChangedItem(projectItem, options);
                    return false;
                }
                this.ProcessNewlyAddedProjectItem(projectItem);
                return true;
            }
            finally
            {
                PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectAddProjectItem);
            }
        }

        protected void RemoveProjectItems(bool deleteFiles, params IProjectItem[] projectItems)
        {
            foreach (IProjectItem projectItem in projectItems)
                this.RemoveProjectItem(projectItem, deleteFiles);
        }

        protected virtual void RemoveProjectItem(IProjectItem projectItem, bool deleteFiles)
        {
            if (this.StartupItem == projectItem && (this.GetCapability<bool>("CanHaveStartupItem") || this.ProjectType.Identifier.Equals(ProjectTypeNamesHelper.Silverlight)))
                this.StartupItem = (IProjectItem)null;
            if (projectItem.Parent != null)
                projectItem.Parent.RemoveChild(projectItem);
            this.items.Remove(projectItem);
        }

        private IEnumerable<MoveInfo> GenerateMoveInfoForChildren(IProjectItem projectItem, string newPath)
        {
            if (projectItem.DocumentReference.IsValidPathFormat)
            {
                foreach (IProjectItem projectItem1 in projectItem.Children)
                {
                    if (projectItem1.DocumentReference.IsValidPathFormat)
                    {
                        string currentParentPath = projectItem.DocumentReference.Path;
                        string currentParentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(currentParentPath);
                        string newParentDirectory = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectory(newPath);
                        string currentChildPath = projectItem1.DocumentReference.Path;
                        string newChildPath = newParentDirectory + currentChildPath.Substring(currentParentDirectory.Length);
                        if (!projectItem.IsDirectory)
                        {
                            string withoutExtension = Path.GetFileNameWithoutExtension(currentParentPath);
                            string fileOrDirectoryName = Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(currentChildPath);
                            Microsoft.Expression.Framework.Documents.PathHelper.GetSafeExtension(currentChildPath);
                            if (fileOrDirectoryName.StartsWith(withoutExtension, StringComparison.OrdinalIgnoreCase))
                            {
                                newChildPath = Microsoft.Expression.Framework.Documents.PathHelper.GetParentDirectory(newChildPath);
                                newChildPath = newChildPath + Path.GetFileNameWithoutExtension(newPath) + fileOrDirectoryName.Substring(withoutExtension.Length);
                                if (projectItem1.IsDirectory)
                                    newChildPath = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(newChildPath);
                            }
                        }
                        yield return new MoveInfo()
                        {
                            Source = projectItem1.DocumentReference.Path,
                            Destination = newChildPath
                        };
                        foreach (MoveInfo moveInfo in this.GenerateMoveInfoForChildren(projectItem1, newChildPath))
                            yield return moveInfo;
                    }
                }
            }
        }

        private IList<MoveResult> MoveProjectItem(IProjectItem projectItem, DocumentReference newPath)
        {
            IList<MoveResult> moveResults;
            try
            {
                if (newPath != projectItem.DocumentReference)
                {
                    if (this.FindItem(newPath) != null || PathHelper.FileOrDirectoryExists(newPath.Path))
                    {
                        throw new ProjectItemRenameException(ProjectItemRenameError.DuplicateName, projectItem, newPath.Path);
                    }
                    if (!this.PromptUserWhenReferencingItemsExist(string.Join(" ,", (
                        from item in projectItem.ReferencingProjectItems
                        select item.Document.Caption).ToArray<string>())))
                    {
                        moveResults = null;
                    }
                    else if (this.AttemptToMakeProjectWritable())
                    {
                        List<MoveInfo> moveInfos = new List<MoveInfo>();
                        MoveInfo moveInfo = new MoveInfo()
                        {
                            Source = projectItem.DocumentReference.Path,
                            Destination = newPath.Path
                        };
                        moveInfos.Add(moveInfo);
                        moveInfos.AddRange(this.GenerateMoveInfoForChildren(projectItem, newPath.Path));
                        moveResults = (!base.Services.ProjectManager().CurrentSolution.IsSourceControlActive ? ProjectPathHelper.MoveSafe(moveInfos, MoveOptions.None, true) : this.SourceControlMove(moveInfos));
                    }
                    else
                    {
                        moveResults = null;
                    }
                }
                else
                {
                    List<MoveResult> moveResults1 = new List<MoveResult>();
                    MoveResult moveResult = new MoveResult()
                    {
                        Source = newPath.Path,
                        Destination = newPath.Path,
                        MovedSuccessfully = true
                    };
                    moveResults1.Add(moveResult);
                    moveResults = moveResults1;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (ErrorHandling.ShouldHandleExceptions(exception))
                {
                    throw new ProjectItemRenameException(exception, ProjectItemRenameError.Exception, projectItem, newPath.Path);
                }
                throw;
            }
            return moveResults;
        }

        private IList<MoveResult> SourceControlMove(IEnumerable<MoveInfo> moveRequests)
        {
            bool[] flagArray;
            int num = moveRequests.Count<MoveInfo>();
            List<string> strs = new List<string>(num);
            List<string> strs1 = new List<string>(num);
            IList<MoveResult> moveResults = new List<MoveResult>();
            foreach (MoveInfo moveRequest in moveRequests)
            {
                if (!PathHelper.FileExists(moveRequest.Source))
                {
                    if (!PathHelper.DirectoryExists(moveRequest.Source))
                    {
                        continue;
                    }
                    if (!PathHelper.FileOrDirectoryExists(moveRequest.Destination))
                    {
                        Directory.CreateDirectory(moveRequest.Destination);
                        MoveResult moveResult = new MoveResult()
                        {
                            Source = moveRequest.Source,
                            Destination = moveRequest.Destination,
                            MovedSuccessfully = true
                        };
                        moveResults.Add(moveResult);
                    }
                    else if (!PathHelper.FileExists(moveRequest.Destination))
                    {
                        MoveResult moveResult1 = new MoveResult()
                        {
                            Source = moveRequest.Source,
                            Destination = moveRequest.Destination,
                            MovedSuccessfully = true
                        };
                        moveResults.Add(moveResult1);
                    }
                    else
                    {
                        MoveResult moveResult2 = new MoveResult()
                        {
                            Source = moveRequest.Source,
                            Destination = moveRequest.Destination,
                            MovedSuccessfully = false
                        };
                        moveResults.Add(moveResult2);
                    }
                }
                else
                {
                    strs.Add(moveRequest.Source);
                    strs1.Add(moveRequest.Destination);
                }
            }
            base.Services.SourceControlProvider().Rename(strs.ToArray(), strs1.ToArray(), true, out flagArray);
            for (int i = 0; i < (int)flagArray.Length; i++)
            {
                MoveResult moveResult3 = new MoveResult()
                {
                    Source = strs[i],
                    Destination = strs1[i],
                    MovedSuccessfully = flagArray[i]
                };
                moveResults.Add(moveResult3);
            }
            return moveResults;
        }

        private bool PromptUserWhenReferencingItemsExist(string referencingItemsString)
        {
            bool flag = true;
            if (!string.IsNullOrEmpty(referencingItemsString))
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string projectItemRenameItemInUse = StringTable.ProjectItemRenameItemInUse;
                object[] objArray = new object[] { referencingItemsString };
                string str = string.Format(currentCulture, projectItemRenameItemInUse, objArray);
                UIThreadDispatcher.Instance.Invoke(DispatcherPriority.ApplicationIdle, () =>
                {
                    MessageBoxArgs messageBoxArg = new MessageBoxArgs()
                    {
                        Message = str,
                        Button = MessageBoxButton.YesNo,
                        Image = MessageBoxImage.Exclamation
                    };
                    MessageBoxResult messageBoxResult = this.Services.MessageDisplayService().ShowMessage(messageBoxArg);
                    flag = messageBoxResult == MessageBoxResult.Yes;
                });
            }
            return flag;
        }

        public void UpdateProjectItemDisplayName(IProjectItem projectItem, string newDisplayName)
        {
            projectItem.DocumentReference.EditableDisplayName = newDisplayName;
            this.OnItemRenamed(new ProjectItemRenamedEventArgs(projectItem, projectItem.DocumentReference));
        }

        public bool RenameProjectItem(IProjectItem oldProjectItem, DocumentReference newDocumentReference)
        {
            if (oldProjectItem == null || newDocumentReference == null)
            {
                return false;
            }
            if (oldProjectItem.DocumentReference.Equals(newDocumentReference))
            {
                return true;
            }
            IList<MoveResult> moveResults = null;
            moveResults = this.MoveProjectItem(oldProjectItem, newDocumentReference);
            if (moveResults == null || moveResults.Count == 0)
            {
                return false;
            }
            if (PathHelper.DirectoryExists(oldProjectItem.DocumentReference.Path) && !Directory.EnumerateFiles(oldProjectItem.DocumentReference.Path, "*.*", SearchOption.AllDirectories).Any<string>())
            {
                try
                {
                    Directory.Delete(oldProjectItem.DocumentReference.Path, true);
                }
                catch (IOException oException)
                {
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                }
            }
            List<IProjectItem> projectItems = new List<IProjectItem>();
            IEnumerable<MoveResult> movedSuccessfully =
                from moveResult in moveResults
                where moveResult.MovedSuccessfully
                select moveResult;
            IEnumerable<MoveResult> movedSuccessfully1 =
                from moveResult in moveResults
                where !moveResult.MovedSuccessfully
                select moveResult;
            foreach (MoveResult moveResult1 in movedSuccessfully)
            {
                IProjectItem projectItem = this.items[PathHelper.GenerateHashFromPath(moveResult1.Source)];
                if (projectItem == null || !this.UpdateProjectItemFileName(projectItem, DocumentReference.Create(moveResult1.Destination)))
                {
                    continue;
                }
                projectItems.Add(projectItem);
            }
            List<DocumentCreationInfo> documentCreationInfos = new List<DocumentCreationInfo>();
            List<IProjectItem> projectItems1 = new List<IProjectItem>();
            Func<IProjectItem, DocumentCreationInfo> func = (IProjectItem item) =>
            {
                IMSBuildItemInternal mSBuildItemInternal = item as IMSBuildItemInternal;
                BuildTaskInfo buildTaskInfo = null;
                if (mSBuildItemInternal != null && mSBuildItemInternal.BuildItem != null)
                {
                    Dictionary<string, string> strs = new Dictionary<string, string>();
                    foreach (ProjectMetadata directMetadatum in mSBuildItemInternal.BuildItem.DirectMetadata)
                    {
                        strs.Add(directMetadatum.Name, directMetadatum.UnevaluatedValue);
                    }
                    buildTaskInfo = new BuildTaskInfo(mSBuildItemInternal.BuildItem.ItemType, strs);
                }
                return new DocumentCreationInfo()
                {
                    BuildTaskInfo = buildTaskInfo,
                    DocumentType = item.DocumentType,
                    TargetPath = item.DocumentReference.Path
                };
            };
            foreach (MoveResult moveResult2 in movedSuccessfully1)
            {
                IProjectItem projectItem1 = this.items[PathHelper.GenerateHashFromPath(moveResult2.Source)];
                if (projectItem1 == null)
                {
                    continue;
                }
                documentCreationInfos.Add(func(projectItem1));
                foreach (IProjectItem child in projectItem1.Children)
                {
                    if (!projectItems.Contains(child))
                    {
                        continue;
                    }
                    documentCreationInfos.Add(func(child));
                }
                projectItems1.Add(projectItem1);
            }
            this.RemoveItems(false, projectItems1.ToArray());
            if (documentCreationInfos.Count > 0)
            {
                this.AddItems(documentCreationInfos);
            }
            if (base.Services.ProjectManager().CurrentSolution.IsSourceControlActive)
            {
                SourceControlStatusCache.UpdateStatus(projectItems, base.Services.SourceControlProvider());
            }
            return true;
        }

        public bool UpdateProjectItemFileName(IProjectItem oldProjectItem, DocumentReference newDocumentReference)
        {
            DocumentReference documentReference = oldProjectItem.DocumentReference;
            newDocumentReference.EditableDisplayName = documentReference.EditableDisplayName;
            if (!this.items.Remove(oldProjectItem))
                throw new InvalidOperationException("Trying to update an item name on a ProjectItem that is NOT in the project's item collection");
            ((DocumentItemBase)oldProjectItem).Rename(newDocumentReference);
            this.items.Add(oldProjectItem);
            this.RenameProjectItemInternal(oldProjectItem, newDocumentReference);
            this.OnItemRenamed(new ProjectItemRenamedEventArgs(oldProjectItem, documentReference));
            this.ImplicitSave();
            return true;
        }

        protected virtual void RenameProjectItemInternal(IProjectItem oldProjectItem, DocumentReference newDocumentReference)
        {
        }

        public IProjectItem FindItem(DocumentReference documentReference)
        {
            return DocumentItemExtensions.FindMatchByUrl<IProjectItem>((IEnumerable<IProjectItem>)this.Items, documentReference.Path);
        }

        public virtual void ReportChangedItem(IProjectItem projectItem, ProjectItemEventOptions options)
        {
            this.OnItemChanged(new ProjectItemEventArgs(projectItem, options));
        }

        protected void DeleteProjectItem(IProjectItem itemToDelete)
        {
            if (base.IsWithinProjectRoot(itemToDelete.DocumentReference))
            {
                this.ProjectWatcher.DisableWatchingForChanges();
                try
                {
                    string path = itemToDelete.DocumentReference.Path;
                    if (PathHelper.FileExists(path))
                    {
                        File.SetAttributes(path, FileAttributes.Normal);
                        ProjectPathHelper.DeleteWithUndo(path);
                        ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
                        if (currentSolution != null && currentSolution.IsSourceControlActive)
                        {
                            SourceControlStatus cachedStatus = SourceControlStatusCache.GetCachedStatus(itemToDelete);
                            if (cachedStatus != SourceControlStatus.None && cachedStatus != SourceControlStatus.Delete)
                            {
                                ISourceControlProvider sourceControlProvider = base.Services.SourceControlProvider();
                                string[] strArrays = new string[] { itemToDelete.DocumentReference.Path };
                                if (sourceControlProvider.Remove(strArrays) == SourceControlSuccess.Success)
                                {
                                    SourceControlStatusCache.SetCachedStatus(itemToDelete, SourceControlStatus.None);
                                }
                            }
                        }
                    }
                    else if (PathHelper.DirectoryExists(path))
                    {
                        ProjectPathHelper.DeleteWithUndo(path);
                    }
                    this.ReportDeletedItem(itemToDelete);
                }
                finally
                {
                    this.EnableWatchingForChanges();
                }
            }
        }

        protected virtual void ReportDeletedItem(IProjectItem deletedItem)
        {
            this.OnItemDeleted(new ProjectItemEventArgs(deletedItem));
        }

        public void ReportChangedProject()
        {
            this.OnProjectChanged(new ProjectEventArgs((IProject)this));
        }

        private string GetDesignTimeUri(string resourceReference)
        {
            foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>)this.Items)
            {
                if (resourceReference == projectItem.ProjectRelativeDocumentReference)
                    return projectItem.DocumentReference.Path;
            }
            return (string)null;
        }

        public Uri MakeDesignTimeUri(Uri uri, string documentPath)
        {
            string relativePath;
            Uri uri1;
            string name;
            string str;
            Uri uri2 = uri;
            string path = base.ProjectRoot.Path;
            string str1 = documentPath;
            if (this.TargetAssembly == null)
            {
                name = null;
            }
            else
            {
                name = this.TargetAssembly.Name;
            }
            Uri uri3 = KnownProjectBase.MakeDesignTimeUri(uri2, path, str1, name, this.ReferencedProjects);
            if (uri3.IsAbsoluteUri && !string.IsNullOrEmpty(uri3.LocalPath) && uri3.IsFile)
            {
                try
                {
                    DocumentReference documentReference = DocumentReference.Create(base.ProjectRoot.Path);
                    relativePath = documentReference.GetRelativePath(DocumentReference.Create(Path.GetFullPath(uri3.LocalPath)));
                }
                catch (NotSupportedException notSupportedException)
                {
                    relativePath = null;
                    uri3 = uri;
                }
                catch (ArgumentException argumentException)
                {
                    relativePath = null;
                    uri3 = uri;
                }
                catch (PathTooLongException pathTooLongException)
                {
                    relativePath = null;
                    uri3 = uri;
                }
                catch (SecurityException securityException)
                {
                    relativePath = null;
                    uri3 = uri;
                }
                if (relativePath != null)
                {
                    if (!Path.IsPathRooted(relativePath))
                    {
                        relativePath = string.Concat(Path.DirectorySeparatorChar, relativePath);
                    }
                    string designTimeUri = this.GetDesignTimeUri(relativePath);
                    if (designTimeUri != null)
                    {
                        uri3 = new Uri(designTimeUri);
                    }
                    else if (uri != null && !ProjectItemBase.IsComponentResourceReference(uri.OriginalString))
                    {
                        IProjectOutputReferenceResolver currentSolution = base.Services.ProjectManager().CurrentSolution as IProjectOutputReferenceResolver;
                        if (currentSolution != null)
                        {
                            Uri deploymentResolvedRoot = currentSolution.GetDeploymentResolvedRoot(this, out uri1);
                            if (deploymentResolvedRoot != null)
                            {
                                Uri uri4 = null;
                                if (uri.OriginalString.StartsWith("../", StringComparison.OrdinalIgnoreCase))
                                {
                                    string relativePath1 = base.ProjectRoot.GetRelativePath(DocumentReference.Create(PathHelper.ResolveRelativePath(documentPath, uri.OriginalString)));
                                    uri4 = new Uri(relativePath1, UriKind.RelativeOrAbsolute);
                                }
                                else if (uri.OriginalString.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                                {
                                    string str2 = uri.OriginalString.Remove(0, "/".Length);
                                    if (!string.IsNullOrEmpty(str2))
                                    {
                                        uri4 = new Uri(str2, UriKind.RelativeOrAbsolute);
                                    }
                                }
                                if (uri4 != null)
                                {
                                    Uri uri5 = uri4;
                                    string path1 = base.ProjectRoot.Path;
                                    string localPath = deploymentResolvedRoot.LocalPath;
                                    if (this.TargetAssembly == null)
                                    {
                                        str = null;
                                    }
                                    else
                                    {
                                        str = this.TargetAssembly.Name;
                                    }
                                    Uri uri6 = KnownProjectBase.MakeDesignTimeUri(uri5, path1, localPath, str, this.ReferencedProjects);
                                    if (uri6 != null && uri1 != null && uri6.LocalPath.StartsWith(uri1.LocalPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        uri3 = uri6;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return uri3;
        }

        public static Uri MakeDesignTimeUri(Uri uri, string projectPath, string documentPath, string projectAssemblyName, IEnumerable<IProject> referencedProjects)
        {
            Uri result1;
            if (projectPath != null)
            {
                Uri result2 = (Uri)null;
                if (Uri.TryCreate(Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(projectPath), UriKind.Absolute, out result2))
                {
                    if (!uri.IsAbsoluteUri)
                    {
                        if (uri.OriginalString.Trim().Length > 0)
                        {
                            if ((int)uri.OriginalString[0] == 47 || (int)uri.OriginalString[0] == 92)
                            {
                                result1 = KnownProjectBase.ParseUri(uri, result2, projectAssemblyName, referencedProjects);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(documentPath))
                                {
                                    if (!Uri.TryCreate(documentPath, UriKind.Absolute, out result2))
                                        return uri;
                                }
                                string relativeUri;
                                try
                                {
                                    relativeUri = Uri.UnescapeDataString(uri.OriginalString);
                                }
                                catch (UriFormatException ex)
                                {
                                    relativeUri = uri.OriginalString;
                                }
                                if (!Uri.TryCreate(result2, relativeUri, out result1))
                                    result1 = uri;
                            }
                        }
                        else
                            result1 = uri;
                    }
                    else
                        result1 = !(uri.Scheme == "pack") || !(uri.Host == "application:,,,") && !(uri.Host == "siteoforigin:,,,") ? uri : KnownProjectBase.ParseUri(uri, result2, projectAssemblyName, referencedProjects);
                }
                else
                    result1 = uri;
            }
            else
                result1 = uri;
            return result1;
        }

        private static Uri ParseUri(Uri uri, Uri baseUri, string projectAssemblyName, IEnumerable<IProject> referencedProjects)
        {
            string str;
            if (uri.IsAbsoluteUri)
            {
                str = uri.LocalPath;
            }
            else
            {
                try
                {
                    str = Uri.UnescapeDataString(uri.OriginalString);
                }
                catch (UriFormatException ex)
                {
                    str = uri.OriginalString;
                }
            }
            Uri result1 = (Uri)null;
            string uriString = str.Substring(1);
            string[] strArray1 = uriString.Split(new char[2]
      {
        '/',
        '\\'
      });
            string[] strArray2 = strArray1[0].Split(';');
            if (strArray2.Length == 2 && string.Compare(strArray2[1], "component", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (string.Compare(strArray2[0], projectAssemblyName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Uri relativeUri = new Uri(uriString.Substring(strArray1[0].Length + 1), UriKind.Relative);
                    if (!Uri.TryCreate(baseUri, relativeUri, out result1))
                        result1 = uri;
                }
                else
                {
                    if (referencedProjects != null)
                    {
                        foreach (IProject project in referencedProjects)
                        {
                            if (string.Compare(strArray2[0], project.TargetAssembly.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                result1 = project.MakeDesignTimeUri(uri, (string)null);
                                break;
                            }
                        }
                    }
                    if (result1 == (Uri)null)
                        result1 = uri;
                }
            }
            else
            {
                Uri result2 = (Uri)null;
                if (!Uri.TryCreate(uriString, UriKind.Relative, out result2) || !Uri.TryCreate(baseUri, result2, out result1))
                    result1 = uri;
            }
            return result1;
        }

        protected void OnItemChanged(ProjectItemEventArgs e)
        {
            IProjectItem projectItem = e.ProjectItem;
            projectItem.DocumentType.OnItemInvalidating(projectItem, projectItem.DocumentReference);
            if (this.ItemChanged == null || this.IsDisposed)
                return;
            this.ItemChanged((object)this, e);
        }

        protected void OnProjectChanged(ProjectEventArgs e)
        {
            if (this.ProjectChanged == null || this.IsDisposed)
                return;
            this.ProjectChanged((object)this, e);
        }

        protected void OnProcessingProjectChanges(ProjectEventArgs e)
        {
            if (this.ProcessingProjectChanges == null || this.IsDisposed)
                return;
            this.ProcessingProjectChanges((object)this, e);
        }

        protected void OnProcessingProjectChangesComplete(ProjectEventArgs e)
        {
            if (this.ProcessingProjectChangesComplete == null || this.IsDisposed)
                return;
            this.ProcessingProjectChangesComplete((object)this, e);
        }

        protected void OnItemDeleted(ProjectItemEventArgs e)
        {
            IProjectItem projectItem = e.ProjectItem;
            projectItem.DocumentType.OnItemInvalidating(projectItem, projectItem.DocumentReference);
            if (this.ItemDeleted == null || this.IsDisposed)
                return;
            this.ItemDeleted((object)this, e);
        }

        protected void OnItemAdded(ProjectItemEventArgs e)
        {
            if (this.ItemAdded == null || this.IsDisposed)
                return;
            this.ItemAdded((object)this, e);
        }

        protected void OnItemRemoved(ProjectItemEventArgs e)
        {
            IProjectItem projectItem = e.ProjectItem;
            projectItem.DocumentType.OnItemInvalidating(projectItem, projectItem.DocumentReference);
            if (this.ItemRemoved == null || this.IsDisposed)
                return;
            this.ItemRemoved((object)this, e);
        }

        public void OnItemOpened(ProjectItemEventArgs e)
        {
            if (this.ItemOpened == null || this.IsDisposed)
                return;
            this.ItemOpened((object)this, e);
        }

        public void OnItemClosing(ProjectItemEventArgs e)
        {
            if (this.ItemClosing == null || this.IsDisposed)
                return;
            this.ItemClosing((object)this, e);
        }

        public void OnItemClosed(ProjectItemEventArgs e)
        {
            if (this.ItemClosed == null || this.IsDisposed)
                return;
            this.ItemClosed((object)this, e);
        }

        protected void OnItemRenamed(ProjectItemRenamedEventArgs e)
        {
            IProjectItem projectItem = e.ProjectItem;
            projectItem.DocumentType.OnItemInvalidating(projectItem, e.OldName);
            if (this.ItemRenamed == null || this.IsDisposed)
                return;
            this.ItemRenamed((object)this, e);
        }

        protected void OnStartupSceneChanged(ProjectItemChangedEventArgs e)
        {
            if (this.StartupItemChanged == null || this.IsDisposed)
                return;
            this.StartupItemChanged((object)this, e);
        }

        internal bool ShouldProcessDelayedChange()
        {
            if (!this.updateAssemblies && !this.updateChangedOrDeletedItems && !this.ImplicitReferencedAssembliesNeedUpdated)
                return this.DeferredAssemblyRequestsNeedUpdated;
            return true;
        }

        internal void UpdateAssembliesAndChangedItemsIfPossible()
        {
            this.OnProcessingProjectChanges(new ProjectEventArgs((IProject)this));
            try
            {
                while (this.ShouldProcessDelayedChange() && (!this.ExternalChanges.IsDelayed && !this.IsDisposed) && this.ProjectStore != null)
                {
                    if (this.DeferredAssemblyRequestsNeedUpdated)
                    {
                        List<string> list = this.deferredAssemblyAddRequests;
                        this.deferredAssemblyAddRequests = new List<string>();
                        using (this.ExternalChanges.DelayNotification())
                        {
                            foreach (string path in list)
                                this.AddAssemblyReference(path, true);
                        }
                    }
                    if (this.updateAssemblies || this.ImplicitReferencedAssembliesNeedUpdated)
                    {
                        this.updateAssemblies = false;
                        this.UpdateAssemblyReferencesImmediately();
                    }
                    if (this.updateChangedOrDeletedItems)
                    {
                        this.updateChangedOrDeletedItems = false;
                        this.UpdateChangedOrDeletedItems();
                    }
                }
            }
            finally
            {
                this.OnProcessingProjectChangesComplete(new ProjectEventArgs((IProject)this));
            }
        }

        protected virtual void UpdateAssemblyReferencesImmediately()
        {
        }

        protected void NonIncrementalAssemblyUpdate(Action updateAction)
        {
            ++this.assemblyNotificationNesting;
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            try
            {
                if (this.assemblyNotificationNesting == 1)
                {
                    PerformanceUtility.StartPerformanceSequence(PerformanceEvent.UpdateAssemblyReferencesInternal);
                    try
                    {
                        this.referencedAssemblies.OnCollectionChanging(e);
                    }
                    finally
                    {
                        this.referencedAssemblies.IsNotifying = false;
                    }
                }
                if (this.IsDisposed)
                    return;
                updateAction();
            }
            finally
            {
                --this.assemblyNotificationNesting;
                if (this.assemblyNotificationNesting <= 0)
                {
                    this.EnsureDesignMetadataLoaded(true);
                    this.referencedAssemblies.IsNotifying = true;
                    PerformanceUtility.EndPerformanceSequence(PerformanceEvent.UpdateAssemblyReferencesInternal);
                    this.referencedAssemblies.OnCollectionChanged(e);
                    PerformanceUtility.EndPerformanceSequence(PerformanceEvent.UpdateAssemblyReferences);
                }
            }
        }

        protected void CheckForUpdatedAssemblies()
        {
            this.updateAssemblies = true;
            this.UpdateAssembliesAndChangedItemsIfPossible();
        }

        internal void CheckForChangedOrDeletedItems()
        {
            this.updateChangedOrDeletedItems = true;
            this.UpdateAssembliesAndChangedItemsIfPossible();
        }

        internal ProjectAssembly GetReferencedAssembly(IProjectItem projectItem)
        {
            return this.referencedAssemblies[(object)projectItem];
        }

        protected void InternalAddImplicitAssemblyReference(Assembly runtimeAssembly, string path, bool isImplicitlyResolved, bool isPlatformAssembly)
        {
            if (this.referencedAssemblies.GetAssemblyKeyByName(ProjectAssemblyHelper.GetAssemblyName(runtimeAssembly).Name, this.TargetAssembly) != null)
                return;
            ProjectAssembly assembly = new ProjectAssembly((IProjectItem)null, runtimeAssembly, path, isImplicitlyResolved);
            assembly.IsPlatformAssembly = isPlatformAssembly;
            this.referencedAssemblies[(object)runtimeAssembly.FullName] = assembly;
            this.UpdateReferenceAssembly(assembly);
        }

        protected void InternalAddAssemblyReference(IProjectItem projectItem)
        {
            ProjectAssembly assembly = this.GetAssembly(projectItem);
            if (assembly == null || this.TargetAssembly != null && this.TargetAssembly.Name == assembly.Name)
                return;
            object assemblyKeyByName = this.referencedAssemblies.GetAssemblyKeyByName(assembly.Name, this.TargetAssembly);
            if (assemblyKeyByName != null)
                this.referencedAssemblies.Remove(assemblyKeyByName);
            this.referencedAssemblies[(object)assembly.ProjectItem] = assembly;
        }

        public Assembly GetDesignAssembly(string assemblyName)
        {
            Assembly assembly = (Assembly)null;
            if (!string.IsNullOrEmpty(assemblyName))
                this.designAssembliesForSourceAssemblies.TryGetValue(assemblyName, out assembly);
            return assembly;
        }

        protected virtual ProjectAssembly GetAssembly(IProjectItem projectItem)
        {
            throw new InvalidOperationException();
        }

        protected void InternalRemoveAssemblyReference(IProjectItem projectItem)
        {
            this.referencedAssemblies.Remove((object)projectItem);
        }

        protected void InternalRemoveAssemblyReference(ProjectAssembly assembly)
        {
            if (assembly.ProjectItem == null)
                this.referencedAssemblies.Remove((object)assembly.FullName);
            else
                this.InternalRemoveAssemblyReference(assembly.ProjectItem);
        }

        protected ProjectAssembly GetAssembly(string path, IProjectItem associatedProjectItem)
        {
            if (!path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                path += ".dll";
            string location = path;
            AssemblyInformation assemblyInformation = this.AssemblyService.ResolveAssembly(path);
            if (!string.IsNullOrEmpty(assemblyInformation.ShadowCopyLocation))
                path = assemblyInformation.ShadowCopyLocation;
            if (assemblyInformation.Assembly == (Assembly)null)
            {
                string withoutExtension = Path.GetFileNameWithoutExtension(path);
                return new ProjectAssembly(associatedProjectItem, withoutExtension);
            }
            ProjectAssembly projectAssembly = new ProjectAssembly(associatedProjectItem, assemblyInformation.Assembly, path);
            projectAssembly.IsPlatformAssembly = assemblyInformation.IsPlatformAssembly;
            if (!assemblyInformation.WasShadowCopied)
            {
                AssemblyName name = ProjectAssemblyHelper.GetAssemblyName(assemblyInformation.Assembly);
                if (!Enumerable.Any<KeyValuePair<string, byte[]>>((IEnumerable<KeyValuePair<string, byte[]>>)KnownProjectBase.designAssemblyBlackList, (Func<KeyValuePair<string, byte[]>, bool>)(keyPair =>
                {
                    if (keyPair.Key.Equals(name.Name))
                        return ProjectAssemblyHelper.ComparePublicKeyTokens(keyPair.Value, name.GetPublicKeyToken());
                    return false;
                })))
                    this.dirtyDesignMetadataAssemblies.Add(new KnownProjectBase.DesignTimeAssemblyLookupInformation(projectAssembly.RuntimeAssembly, location));
            }
            else
                this.dirtyDesignMetadataAssemblies.Add(new KnownProjectBase.DesignTimeAssemblyLookupInformation(projectAssembly.RuntimeAssembly, location));
            if (this.referencedAssemblies.IsNotifying)
                this.EnsureDesignMetadataLoaded(true);
            return projectAssembly;
        }

        private Assembly ResolveProjectAssembly(AssemblyName assemblyName)
        {
            foreach (ProjectAssembly projectAssembly in (IEnumerable<ProjectAssembly>)this.ReferencedAssemblies)
            {
                if (projectAssembly != null && assemblyName.Name == projectAssembly.Name && (projectAssembly.RuntimeAssembly != (Assembly)null && ProjectAssemblyHelper.ComparePublicKeyTokens(ProjectAssemblyHelper.GetAssemblyName(projectAssembly.RuntimeAssembly).GetPublicKeyToken(), assemblyName.GetPublicKeyToken())))
                    return projectAssembly.RuntimeAssembly;
            }
            if (this.currentlyLoadingDesignAssembly != (Assembly)null && string.Equals(this.currentlyLoadingDesignAssembly.FullName, assemblyName.FullName, StringComparison.Ordinal))
                return this.currentlyLoadingDesignAssembly;
            Assembly assembly;
            if (KnownProjectBase.designAssemblies.TryGetValue(assemblyName.FullName, out assembly))
                return assembly;
            Assembly satelliteAssembly = this.AssemblyService.GetCachedSatelliteAssembly(assemblyName);
            if (satelliteAssembly != (Assembly)null)
                return satelliteAssembly;
            return (Assembly)null;
        }

        protected internal void UpdateReferenceAssembly(ProjectAssembly assembly)
        {
            if (!this.referenceAssemblyContextInitialized || this.referenceAssemblyContext == null)
                return;
            this.referenceAssemblyContext.UpdateReferenceAssembly(assembly);
        }

        protected virtual Assembly ResolveImplicitProjectAssembly(AssemblyName assemblyName)
        {
            return (Assembly)null;
        }

        private Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Name))
                return this.ResolveAssembly(new AssemblyName(args.Name));
            return (Assembly)null;
        }

        private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(this.AppDomain_AssemblyResolve);
            ((ProjectManager)base.Services.ProjectManager()).SolutionOpened -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
        }

        internal Assembly ResolveAssembly(AssemblyName assemblyName)
        {
            Assembly assembly = (Assembly)null;
            Thread currentThread = Thread.CurrentThread;
            bool flag = currentThread != null && !currentThread.IsBackground;
            if (flag)
                PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ProjectAssemblyResolveHandler);
            if (!string.IsNullOrEmpty(assemblyName.Name))
            {
                assembly = this.ResolveProjectAssembly(assemblyName);
                if (assembly == (Assembly)null && !assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                {
                    if (this.ProjectAssemblyReferencesDirty)
                    {
                        this.cachedAssemblyResolveFailures.Clear();
                        this.ProjectAssemblyReferencesDirty = false;
                    }
                    if (this.cachedAssemblyResolveFailures.Contains(assemblyName.FullName))
                        return (Assembly)null;
                    assembly = this.ResolveImplicitProjectAssembly(assemblyName);
                    if (assembly == (Assembly)null)
                        this.cachedAssemblyResolveFailures.Add(assemblyName.FullName);
                }
            }
            if (flag)
                PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ProjectAssemblyResolveHandler);
            return assembly;
        }

        protected void EnsureDesignMetadataLoaded(bool showErrors)
        {
            List<KnownProjectBase.DesignTimeAssemblyLookupInformation> list = Enumerable.ToList<KnownProjectBase.DesignTimeAssemblyLookupInformation>((IEnumerable<KnownProjectBase.DesignTimeAssemblyLookupInformation>)this.dirtyDesignMetadataAssemblies);
            this.currentlyLoadingDesignAssemblies.Clear();
            foreach (KnownProjectBase.DesignTimeAssemblyLookupInformation lookupInformation in list)
            {
                if ((this.LoadDesignMetadata(lookupInformation.RuntimeAssembly, lookupInformation.Location, showErrors) & KnownProjectBase.LoadMetadataResult.ErrorLoading) == KnownProjectBase.LoadMetadataResult.NotFound)
                    this.dirtyDesignMetadataAssemblies.Remove(lookupInformation);
            }
        }

        private KnownProjectBase.LoadMetadataResult LoadDesignMetadata(Assembly cachedAssembly, string originalAssemblyLocation, bool showErrors)
        {
            string directoryNameOrRoot = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(originalAssemblyLocation);
            string str = Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(cachedAssembly.Location);
            if (cachedAssembly.GlobalAssemblyCache || !this.AssemblyService.AssemblyCache.VerifyDirectory(cachedAssembly.Location))
                str = directoryNameOrRoot;
            string sourceDirectory = Path.Combine(directoryNameOrRoot, "Design");
            string destinationDirectory = Path.Combine(str, "Design");
            return this.TryLoadMetadata(cachedAssembly, originalAssemblyLocation, directoryNameOrRoot, str, ".Design", showErrors) | this.TryLoadMetadata(cachedAssembly, originalAssemblyLocation, sourceDirectory, destinationDirectory, ".Design", showErrors) | this.TryLoadMetadata(cachedAssembly, originalAssemblyLocation, directoryNameOrRoot, str, ".Expression.Design", showErrors) | this.TryLoadMetadata(cachedAssembly, originalAssemblyLocation, sourceDirectory, destinationDirectory, ".Expression.Design", showErrors);
        }

        public static string FindBestMetadataAssembly(string sourceDirectory, string controlAssemblyPath, string metadataSuffix)
        {
            if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(sourceDirectory))
                return (string)null;
            string str = (string)null;
            Version version1 = (Version)null;
            Version version2 = ProjectAssemblyHelper.GetAssemblyName(typeof(IProvideAttributeTable).Assembly).Version;
            string designAssemblyPrefix = Path.GetFileNameWithoutExtension(controlAssemblyPath) + metadataSuffix;
            IEnumerable<string> results = (IEnumerable<string>)null;
            ErrorHandling.HandleBasicExceptions((Action)(() => results = (IEnumerable<string>)Directory.GetFiles(sourceDirectory, designAssemblyPrefix + "*.dll", SearchOption.TopDirectoryOnly)), (Action<Exception>)(e => { }));
            if (results == null)
                return (string)null;
            foreach (string path in results)
            {
                string withoutExtension = Path.GetFileNameWithoutExtension(path);
                if (withoutExtension.Equals(designAssemblyPrefix, StringComparison.OrdinalIgnoreCase) || withoutExtension.StartsWith(designAssemblyPrefix + ".", StringComparison.OrdinalIgnoreCase))
                {
                    Version referencedDesignVersion = KnownProjectBase.GetReferencedDesignVersion(path);
                    if (referencedDesignVersion != (Version)null)
                    {
                        if (version2.Major == referencedDesignVersion.Major && referencedDesignVersion.CompareTo(version2) <= 0 && (version1 == (Version)null || referencedDesignVersion.CompareTo(version1) > 0))
                        {
                            version1 = referencedDesignVersion;
                            str = path;
                        }
                    }
                    else if (version1 == (Version)null)
                        str = path;
                }
            }
            return str;
        }

        private KnownProjectBase.LoadMetadataResult TryLoadMetadata(Assembly sourceAssembly, string controlAssemblyPath, string sourceDirectory, string destinationDirectory, string metadataSuffix, bool showErrors)
        {
            string designAssemblySourcePath = KnownProjectBase.FindBestMetadataAssembly(sourceDirectory, controlAssemblyPath, metadataSuffix);
            if (designAssemblySourcePath == null)
                return KnownProjectBase.LoadMetadataResult.NotFound;
            if (this.currentlyLoadingDesignAssemblies.Contains(designAssemblySourcePath))
                return KnownProjectBase.LoadMetadataResult.Success;
            this.currentlyLoadingDesignAssemblies.Add(designAssemblySourcePath);
            string designAssemblyDestinationPath = Path.Combine(destinationDirectory, Path.GetFileName(designAssemblySourcePath));
            if (!sourceDirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase) && !ErrorHandling.HandleBasicExceptions((Action)(() =>
            {
                if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);
                if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(designAssemblyDestinationPath))
                    File.Copy(designAssemblySourcePath, designAssemblyDestinationPath);
                File.SetAttributes(designAssemblyDestinationPath, FileAttributes.Normal);
            }), (Action<Exception>)(exception => this.LogException(designAssemblySourcePath, exception, showErrors))))
                return KnownProjectBase.LoadMetadataResult.ErrorLoading;
            if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(designAssemblyDestinationPath))
                return KnownProjectBase.LoadMetadataResult.NotFound;
            try
            {
                this.currentlyLoadingDesignAssembly = sourceAssembly;
                return this.LoadMetadata(sourceAssembly, designAssemblyDestinationPath, sourceDirectory, showErrors) ? KnownProjectBase.LoadMetadataResult.Success : KnownProjectBase.LoadMetadataResult.ErrorLoading;
            }
            finally
            {
                this.currentlyLoadingDesignAssembly = (Assembly)null;
            }
        }

        private static Version GetReferencedDesignVersion(string path)
        {
            AssemblyMetadataHelper.IMetaDataDispenserEx dispenser = AssemblyMetadataHelper.GetDispenser();
            AssemblyMetadataHelper.IMetaDataAssemblyImport assemblyImport = AssemblyMetadataHelper.OpenScope(dispenser, path);
            try
            {
                if (assemblyImport != null)
                {
                    AssemblyNameVersion[] referenceNameVersion = AssemblyMetadataHelper.GetAssemblyReferenceNameVersion(assemblyImport);
                    if (referenceNameVersion != null)
                    {
                        foreach (AssemblyNameVersion assemblyNameVersion in referenceNameVersion)
                        {
                            if (assemblyNameVersion.Name.Equals("Microsoft.Windows.Design.Extensibility", StringComparison.OrdinalIgnoreCase))
                                return assemblyNameVersion.Version;
                        }
                    }
                }
            }
            finally
            {
                if (assemblyImport != null)
                    AssemblyMetadataHelper.ReleaseAssemblyImport(assemblyImport);
                if (dispenser != null)
                    AssemblyMetadataHelper.ReleaseDispenser(dispenser);
            }
            return (Version)null;
        }

        private bool LoadMetadata(Assembly sourceAssembly, string designAssemblyPath, string sourceDirectory, bool showErrors)
        {
            Assembly assembly1;
            try
            {
                assembly1 = Assembly.LoadFile(designAssemblyPath);
            }
            catch (OutOfMemoryException ex)
            {
                LowMemoryMessage.Show();
                return false;
            }
            catch (Exception ex)
            {
                this.LogException(designAssemblyPath, ex, showErrors);
                return false;
            }
            if (assembly1 == (Assembly)null)
                return false;
            Assembly assembly2;
            if (KnownProjectBase.designAssemblies.TryGetValue(assembly1.FullName, out assembly2) && assembly2 == assembly1)
            {
                this.designAssembliesForSourceAssemblies[sourceAssembly.FullName] = assembly1;
                return true;
            }
            this.AssemblyService.TryCacheSatelliteAssembly(assembly1, sourceDirectory);
            this.LoadDependentMetadataAssemblies(assembly1, sourceDirectory, showErrors);
            object[] customAttributes;
            try
            {
                customAttributes = assembly1.GetCustomAttributes(typeof(ProvideMetadataAttribute), false);
            }
            catch (Exception ex)
            {
                return false;
            }
            foreach (object obj in customAttributes)
            {
                ProvideMetadataAttribute metadataAttribute = obj as ProvideMetadataAttribute;
                if (metadataAttribute != null)
                {
                    if (typeof(IProvideAttributeTable).IsAssignableFrom(metadataAttribute.MetadataProviderType))
                    {
                        try
                        {
                            IProvideAttributeTable provideAttributeTable = (IProvideAttributeTable)Activator.CreateInstance(metadataAttribute.MetadataProviderType);
                            if (KnownProjectBase.MetadataStore != null)
                                KnownProjectBase.MetadataStore.AddAttributeTable(provideAttributeTable.AttributeTable);
                        }
                        catch (Exception ex)
                        {
                            this.LogException(designAssemblyPath, ex, showErrors);
                            return false;
                        }
                    }
                }
            }
            KnownProjectBase.designAssemblies[assembly1.FullName] = assembly1;
            this.designAssembliesForSourceAssemblies[sourceAssembly.FullName] = assembly1;
            return true;
        }

        private void LoadDependentMetadataAssemblies(Assembly assembly, string sourceDirectory, bool showErrors)
        {
            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                AssemblyName referencedAssemblyName = assemblyName;
                if (this.ResolveProjectAssembly(referencedAssemblyName) == (Assembly)null)
                    ErrorHandling.HandleBasicExceptions((Action)(() =>
                    {
                        string path = Path.Combine(sourceDirectory, referencedAssemblyName.Name + ".dll");
                        if (this.currentlyLoadingDesignAssemblies.Contains(path) || !Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
                            return;
                        Assembly assembly1 = this.AssemblyService.ResolveAssembly(path).Assembly;
                        if (!(assembly1 != (Assembly)null))
                            return;
                        this.currentlyLoadingDesignAssemblies.Add(path);
                        KnownProjectBase.designAssemblies[referencedAssemblyName.FullName] = assembly1;
                        this.LoadDependentMetadataAssemblies(assembly1, sourceDirectory, showErrors);
                    }), (Action<Exception>)(exception => this.LogException(referencedAssemblyName.Name, exception, showErrors)));
            }
        }

        private void LogException(string designAssemblyPath, Exception e, bool showErrors)
        {
            if (!showErrors)
            {
                return;
            }
            IMessageLoggingService messageLoggingService = base.Services.MessageLoggingService();
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string unableToLoadDesignMetadataAssembly = StringTable.UnableToLoadDesignMetadataAssembly;
            object[] objArray = new object[] { designAssemblyPath };
            messageLoggingService.WriteLine(string.Format(currentCulture, unableToLoadDesignMetadataAssembly, objArray));
            base.Services.MessageLoggingService().WriteLine(e.Message);
            ReflectionTypeLoadException reflectionTypeLoadException = e as ReflectionTypeLoadException;
            if (reflectionTypeLoadException != null)
            {
                Exception[] loaderExceptions = reflectionTypeLoadException.LoaderExceptions;
                for (int i = 0; i < (int)loaderExceptions.Length; i++)
                {
                    Exception exception = loaderExceptions[i];
                    base.Services.MessageLoggingService().WriteLine(exception.Message);
                }
            }
        }

        private void UpdateChangedOrDeletedItems()
        {
            if (base.IsDisposed)
            {
                return;
            }
            using (IDisposable disposable = this.ExternalChanges.DelayNotification())
            {
                if (!base.ProjectFileInformation.HasChanged())
                {
                    List<IProjectItem> projectItems = new List<IProjectItem>();
                    List<IProjectItem> projectItems1 = new List<IProjectItem>();
                    foreach (IProjectItem item in (IEnumerable<IProjectItem>)this.Items)
                    {
                        if (PathHelper.FileExists(item.DocumentReference.Path) || PathHelper.DirectoryExists(item.DocumentReference.Path))
                        {
                            ProjectItem projectItem = item as ProjectItem;
                            if (projectItem == null || !projectItem.FileInformation.HasChanged())
                            {
                                continue;
                            }
                            projectItems1.Add(item);
                        }
                        else
                        {
                            if (item.IsVirtual || item.IsReference)
                            {
                                continue;
                            }
                            projectItems.Add(item);
                        }
                    }
                    SourceControlStatusCache.UpdateStatus(projectItems1, base.Services.SourceControlProvider());
                    this.UpdateFileInformationForItems(projectItems1);
                    this.UpdateFileInformationForItems(projectItems);
                    foreach (IProjectItem projectItem1 in projectItems1)
                    {
                        this.ReportChangedItem(projectItem1, ProjectItemEventOptions.None);
                    }
                    foreach (IProjectItem projectItem2 in projectItems)
                    {
                        this.ReportDeletedItem(projectItem2);
                    }
                }
                else
                {
                    this.ReportChangedProject();
                    if (!base.IsDisposed)
                    {
                        base.ProjectFileInformation = new ProjectFileInformation(base.DocumentReference.Path);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void UpdateFileInformationForItems(IEnumerable<IProjectItem> items)
        {
            foreach (IProjectItem projectItem1 in items)
            {
                ProjectItem projectItem2 = projectItem1 as ProjectItem;
                if (projectItem2 != null)
                    projectItem2.UpdateFileInformation();
            }
        }

        public void DisableWatchingForChanges()
        {
            ProjectWatcher projectWatcher = this.ProjectWatcher;
            if (projectWatcher == null)
                return;
            projectWatcher.DisableWatchingForChanges();
        }

        public void EnableWatchingForChanges()
        {
            ProjectWatcher projectWatcher = this.ProjectWatcher;
            if (projectWatcher == null)
                return;
            projectWatcher.UpdateFileInformation();
            projectWatcher.EnableWatchingForChanges();
        }

        void IFileWatcher.Reactivate()
        {
            ProjectWatcher projectWatcher = this.ProjectWatcher;
            if (projectWatcher == null)
                return;
            projectWatcher.CheckForChangesAndReenable();
        }

        void IFileWatcher.Deactivate()
        {
            ProjectWatcher projectWatcher = this.ProjectWatcher;
            if (projectWatcher == null)
                return;
            projectWatcher.DisableWatchingForChanges();
        }

        protected bool IsKnownCapability(string name)
        {
            return Enumerable.SingleOrDefault<string>((IEnumerable<string>)KnownProjectBase.KnownCapabilities, (Func<string, bool>)(capability => capability.Equals(name, StringComparison.OrdinalIgnoreCase))) != null;
        }

        public virtual T GetCapability<T>(string name)
        {
            IProject project;
            T t;
            if (!this.IsKnownCapability(name))
            {
                throw new ArgumentOutOfRangeException(string.Concat("name=", name), name, string.Concat("Unknown project capability: ", name));
            }
            if (base.Services.ProjectAdapterService() != null)
            {
                IProjectCapabilityAdapter projectCapabilityAdapter = base.Services.ProjectAdapterService().FindAdapter<IProjectCapabilityAdapter>(this, out project);
                if (projectCapabilityAdapter != null && projectCapabilityAdapter.GetCapability<T>(project, name, out t))
                {
                    return t;
                }
            }
            string str = name;
            string str1 = str;
            if (str != null && (str1 == "SupportsDatabinding" || str1 == "SupportsMediaElementControl" || str1 == "SupportsHyperlinkButtonControl" || str1 == "SupportsUIElementEffectProperty" || str1 == "SupportsAssetLibraryBehaviorsItems"))
            {
                return TypeHelper.ConvertType<T>(true);
            }
            return default(T);
        }

        public virtual bool SetCapability<T>(string name, T value)
        {
            if (!this.IsKnownCapability(name))
                throw new ArgumentOutOfRangeException("name");
            return object.Equals((object)this.GetCapability<T>(name), (object)value);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return this.DocumentReference.Path;
        }

        private sealed class AssemblyCollection : IAssemblyCollection, ICollection<ProjectAssembly>, IEnumerable<ProjectAssembly>, IEnumerable, INotifyCollectionChanges, INotifyCollectionChanged
        {
            private List<KeyValuePair<object, ProjectAssembly>> dictionary;

            public bool IsNotifying { get; set; }

            public ProjectAssembly this[object key]
            {
                get
                {
                    int index = this.IndexOf(key);
                    if (index < 0)
                        return (ProjectAssembly)null;
                    return this.dictionary[index].Value;
                }
                set
                {
                    int index = this.IndexOf(key);
                    if (index >= 0)
                    {
                        NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)this.dictionary[index].Value, index);
                        this.OnCollectionChanging(e);
                        this.dictionary.RemoveAt(index);
                        this.OnCollectionChanged(e);
                    }
                    else
                        index = this.Count;
                    if (value == null)
                        return;
                    NotifyCollectionChangedEventArgs e1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object)value, index);
                    this.OnCollectionChanging(e1);
                    this.dictionary.Insert(index, new KeyValuePair<object, ProjectAssembly>(key, value));
                    this.OnCollectionChanged(e1);
                }
            }

            public int Count
            {
                get
                {
                    return this.dictionary.Count;
                }
            }

            bool ICollection<ProjectAssembly>.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanging;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public AssemblyCollection()
            {
                this.dictionary = new List<KeyValuePair<object, ProjectAssembly>>();
                this.IsNotifying = true;
            }

            public int IndexOf(object key)
            {
                for (int index = 0; index < this.dictionary.Count; ++index)
                {
                    if (key == this.dictionary[index].Key)
                        return index;
                }
                return -1;
            }

            public int IndexOf(ProjectAssembly assembly)
            {
                for (int index = 0; index < this.dictionary.Count; ++index)
                {
                    if (assembly.Equals((object)this.dictionary[index].Value))
                        return index;
                }
                return -1;
            }

            public object GetAssemblyKeyByName(string assemblyName, ProjectAssembly targetAssembly)
            {
                return Enumerable.FirstOrDefault<KeyValuePair<object, ProjectAssembly>>((IEnumerable<KeyValuePair<object, ProjectAssembly>>)this.dictionary, (Func<KeyValuePair<object, ProjectAssembly>, bool>)(assembly =>
                {
                    if (assembly.Value.Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                        return assembly.Value != targetAssembly;
                    return false;
                })).Key;
            }

            public void Remove(object key)
            {
                int index = this.IndexOf(key);
                if (index < 0)
                    return;
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object)this.dictionary[index].Value, index);
                this.OnCollectionChanging(e);
                this.dictionary.RemoveAt(index);
                this.OnCollectionChanged(e);
            }

            public void OnCollectionChanging(NotifyCollectionChangedEventArgs e)
            {
                if (!this.IsNotifying || this.CollectionChanging == null)
                    return;
                this.CollectionChanging((object)this, e);
            }

            public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                if (!this.IsNotifying || this.CollectionChanged == null)
                    return;
                this.CollectionChanged((object)this, e);
            }

            void ICollection<ProjectAssembly>.Add(ProjectAssembly item)
            {
                throw new InvalidOperationException();
            }

            public void Clear()
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                this.OnCollectionChanging(e);
                this.dictionary.Clear();
                this.OnCollectionChanged(e);
            }

            public bool Contains(ProjectAssembly item)
            {
                return this.IndexOf(item) >= 0;
            }

            public ProjectAssembly Find(string name)
            {
                name = ProjectAssemblyHelper.TrimFiletypeFromAssemblyNameOrPath(name);
                for (int index = 0; index < this.dictionary.Count; ++index)
                {
                    if (this.dictionary[index].Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return this.dictionary[index].Value;
                }
                return (ProjectAssembly)null;
            }

            void ICollection<ProjectAssembly>.CopyTo(ProjectAssembly[] array, int arrayIndex)
            {
                for (int index = 0; index < this.dictionary.Count; ++index)
                    array[index + arrayIndex] = this.dictionary[index].Value;
            }

            bool ICollection<ProjectAssembly>.Remove(ProjectAssembly item)
            {
                throw new InvalidOperationException();
            }

            public IEnumerator<ProjectAssembly> GetEnumerator()
            {
                foreach (KeyValuePair<object, ProjectAssembly> keyValuePair in this.dictionary)
                    yield return keyValuePair.Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)this.GetEnumerator();
            }
        }

        private class TargetComparer : IEqualityComparer<DocumentCreationInfo>
        {
            public bool Equals(DocumentCreationInfo x, DocumentCreationInfo y)
            {
                return string.Compare(x.TargetPath, y.TargetPath, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public int GetHashCode(DocumentCreationInfo obj)
            {
                if (obj.TargetPath == null)
                    return 0;
                return obj.TargetPath.GetHashCode();
            }
        }

        private struct DesignTimeAssemblyLookupInformation
        {
            public Assembly RuntimeAssembly { get; private set; }

            public string Location { get; private set; }

            public DesignTimeAssemblyLookupInformation(Assembly runtimeAssembly, string location)
            {
                this = new KnownProjectBase.DesignTimeAssemblyLookupInformation();
                this.RuntimeAssembly = runtimeAssembly;
                this.Location = location;
            }
        }

        [Flags]
        private enum LoadMetadataResult
        {
            NotFound = 0,
            Success = 1,
            ErrorLoading = 2,
        }
    }
}
