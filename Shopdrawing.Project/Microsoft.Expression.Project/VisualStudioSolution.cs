// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Project.VisualStudioSolution
// Assembly: Microsoft.Expression.Project, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 80357D9B-A7D7-4011-8FBC-3E1052652ADC
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Project.dll

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Configuration;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.IssueTracking;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project.Build;
using Microsoft.Expression.Project.Extensions;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Messaging;
using Microsoft.Expression.Project.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace Microsoft.Expression.Project
{
    [VisualStudioSolution]
    internal class VisualStudioSolution : SolutionBase, IProjectOutputReferenceResolver, IProjectBuildContext, IBuildWorker
    {
        private static readonly string VisualStudio2005Format = "Microsoft Visual Studio Solution File, Format Version 9.00";
        private static readonly string VisualStudio2008Format = "Microsoft Visual Studio Solution File, Format Version 10.00";
        private static readonly string VisualStudio2010Format = "Microsoft Visual Studio Solution File, Format Version 11.00";
        private static readonly string VisualStudioFormatComment = "# Visual Studio 2010";
        private static readonly string ConfigurationDebugAnyCpu = "Debug|Any CPU";
        private static readonly string ConfigurationReleaseAnyCpu = "Release|Any CPU";
        private static readonly string SolutionConfigurationPlatforms = "SolutionConfigurationPlatforms";
        private static readonly string ProjectConfigurationPlatforms = "ProjectConfigurationPlatforms";
        private static readonly char[] VerticalBar = new char[1]
    {
      '|'
    };
        private static readonly char[] SemiColon = new char[1]
    {
      ';'
    };
        private static readonly string[] SupportedPlatforms = new string[4]
    {
      "Any CPU",
      "x86",
      "x64",
      "Itanium"
    };
        private static readonly string[] BuildablePlatforms = new string[5]
    {
      "Any CPU",
      "x86",
      "x64",
      "Itanium",
      "Mixed Platforms"
    };
        private static readonly string SuoProjectList = "ProjectList";
        private static readonly string SuoProjectGuidProperty = "GUID";
        private static readonly string SuoIsStartupProjectProperty = "IsStartup";
        private static readonly string SuoStartupHtmlProperty = "StartupHtml";
        private static readonly string SuoOpenViewCollection = "OpenViews";
        private static readonly string SuoHasPersistedOpenViews = "HasPersistedOpenViews";
        private static readonly string SuoOpenViewSolutionRelativePathProperty = "SolutionRelativePath";
        private static readonly string SuoActiveViewProperty = "ActiveView";
        private static readonly string SuoSolutionSettings = "SolutionSettings";
        private HashSet<VisualStudioSolution.SolutionSection> solutionFolderProjectSections = new HashSet<VisualStudioSolution.SolutionSection>();
        private Dictionary<DocumentReference, VisualStudioSolution.SolutionSection> projectSections = new Dictionary<DocumentReference, VisualStudioSolution.SolutionSection>();
        private Dictionary<DocumentReference, VisualStudioSolution.ProjectMetadata> projectMetadata = new Dictionary<DocumentReference, VisualStudioSolution.ProjectMetadata>();
        private Dictionary<string, VisualStudioSolution.SolutionSection> globalSections = new Dictionary<string, VisualStudioSolution.SolutionSection>();
        private VisualStudioSolution.VisualStudioSolutionFormat solutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.UnknownFormat;
        private string solutionFormatComment = VisualStudioSolution.VisualStudioFormatComment;
        private IErrorTaskCollection buildErrors = (IErrorTaskCollection)new ErrorTaskCollection();
        private SolutionConfigurationManager configurationManager;
        private VisualStudioSolution.ProjectOutputReferenceManager projectOutputReferenceManager;
        private bool hookedTfs;
        private SolutionSettingsManager solutionSettingsManager;

        public override SolutionSettingsManager SolutionSettingsManager
        {
            get
            {
                if (this.solutionSettingsManager == null)
                    this.solutionSettingsManager = new SolutionSettingsManager((ISolution)this, this.configurationManager[VisualStudioSolution.SuoSolutionSettings]);
                return this.solutionSettingsManager;
            }
        }

        private IConfigurationObject SolutionConfiguration
        {
            get
            {
                return this.configurationManager["SolutionConfiguration"];
            }
        }

        private IConfigurationObjectCollection PersistedOpenDocumentViews
        {
            get
            {
                return ConfigurationExtensions.GetOrCreateConfigurationObjectCollectionProperty(this.SolutionConfiguration, VisualStudioSolution.SuoOpenViewCollection);
            }
        }

        public override bool CanAddProjects
        {
            get
            {
                return true;
            }
        }

        public VisualStudioSolution.VisualStudioSolutionFormat SolutionFormat
        {
            get
            {
                return this.solutionFormat;
            }
            set
            {
                this.solutionFormat = value;
            }
        }

        public override Version VersionNumber
        {
            get
            {
                switch (this.SolutionFormat)
                {
                    case VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2005Format:
                        return new Version(9, 0);
                    case VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2008Format:
                        return new Version(10, 0);
                    case VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2010Format:
                        return new Version(11, 0);
                    default:
                        return new Version(0, 0);
                }
            }
        }

        public string SolutionFormatComment
        {
            get
            {
                return this.solutionFormatComment;
            }
            set
            {
                this.solutionFormatComment = value;
            }
        }

        public override bool IsUnderSourceControl
        {
            get
            {
                return !string.IsNullOrEmpty(this.SoureControlProviderName);
            }
        }

        public override bool IsIssueTrackingAvailable
        {
            get
            {
                return base.Services.IssueTrackingProvider() != null;
            }
        }

        public override bool IsSourceControlActive
        {
            get
            {
                ISourceControlProvider sourceControlProvider = base.Services.SourceControlProvider();
                if (!this.IsUnderSourceControl || sourceControlProvider == null)
                {
                    return false;
                }
                return sourceControlProvider.GetOnlineStatus() == SourceControlOnlineStatus.Online;
            }
        }

        private string SoureControlProviderName
        {
            get
            {
                ISourceControlService sourceControlService = ServiceExtensions.SourceControlService(this.Services);
                if (sourceControlService != null)
                {
                    foreach (string key in sourceControlService.RegisteredProviders)
                    {
                        if (this.globalSections.ContainsKey(key))
                            return key;
                    }
                }
                return string.Empty;
            }
        }

        public override IProjectBuildContext ProjectBuildContext
        {
            get
            {
                return (IProjectBuildContext)this;
            }
        }

        public ProjectInstance ProjectInstance
        {
            get
            {
                return (ProjectInstance)null;
            }
        }

        public IErrorTaskCollection BuildErrors
        {
            get
            {
                return this.buildErrors;
            }
        }

        public IBuildWorker BuildWorker
        {
            get
            {
                return (IBuildWorker)this;
            }
        }

        public string FullTargetPath
        {
            get
            {
                if (this.StartupProject != null)
                    return this.StartupProject.StartProgram;
                return (string)null;
            }
        }

        public virtual string Configuration
        {
            get
            {
                return (string)null;
            }
        }

        public bool NeedsBuilding
        {
            get
            {
                return true;
            }
        }

        string IProjectBuildContext.DisplayName
        {
            get
            {
                return this.DocumentReference.DisplayName;
            }
        }

        internal VisualStudioSolution(IServiceProvider serviceProvider, DocumentReference documentReference)
            : base(serviceProvider, documentReference)
        {
            this.configurationManager = new SolutionConfigurationManager(this);
            this.projectOutputReferenceManager = new VisualStudioSolution.ProjectOutputReferenceManager();
        }

        public Uri GetDeploymentResolvedRoot(IProject sourceProject)
        {
            Uri targetProjectRoot;
            return this.GetDeploymentResolvedRoot(sourceProject, out targetProjectRoot);
        }

        public Uri GetDeploymentResolvedRoot(IProject sourceProject, out Uri targetProjectRoot)
        {
            targetProjectRoot = (Uri)null;
            IProjectOutputReferenceInformation referenceInformation = (IProjectOutputReferenceInformation)null;
            Guid? targetGuidForReference = new Guid?();
            Guid? nullable = new Guid?();
            if (this.StartupProject is IProject)
                nullable = new Guid?(((IProject)this.StartupProject).ProjectGuid);
            if (nullable.HasValue)
            {
                referenceInformation = this.projectOutputReferenceManager.GetProjectOutputReferenceInfo(nullable.Value, sourceProject.ProjectGuid);
                if (referenceInformation != null)
                    targetGuidForReference = nullable;
            }
            if (referenceInformation == null || !targetGuidForReference.HasValue)
            {
                Guid? matchingTargetForSource = this.projectOutputReferenceManager.FindFirstMatchingTargetForSource(sourceProject.ProjectGuid);
                if (matchingTargetForSource.HasValue)
                {
                    targetGuidForReference = new Guid?(matchingTargetForSource.Value);
                    referenceInformation = this.projectOutputReferenceManager.GetProjectOutputReferenceInfo(matchingTargetForSource.Value, sourceProject.ProjectGuid);
                }
            }
            if (referenceInformation != null && targetGuidForReference.HasValue)
            {
                IProject targetProject = Enumerable.FirstOrDefault<IProject>(this.Projects, (Func<IProject, bool>)(project => project.ProjectGuid.Equals(targetGuidForReference.Value)));
                if (targetProject != null)
                {
                    string deploymentPath = referenceInformation.CreateDeploymentPath(targetProject, sourceProject);
                    if (!string.IsNullOrEmpty(deploymentPath))
                    {
                        targetProjectRoot = new Uri(targetProject.ProjectRoot.Path, UriKind.RelativeOrAbsolute);
                        return new Uri(deploymentPath);
                    }
                }
            }
            return (Uri)null;
        }

        public IProjectOutputReferenceInformation GetProjectOutputReferenceInfo(IProject targetProject, IProject sourceProject)
        {
            return this.projectOutputReferenceManager.GetProjectOutputReferenceInfo(targetProject.ProjectGuid, sourceProject.ProjectGuid);
        }

        public override void CheckForChangedOrDeletedItems()
        {
            base.CheckForChangedOrDeletedItems();
            if (!this.reloadDocuments)
                return;
            this.OpenInitialViews();
            this.reloadDocuments = false;
        }

        public override void OpenInitialViews()
        {
            if (!this.SolutionConfiguration.GetPropertyOrDefault<bool>(VisualStudioSolution.SuoHasPersistedOpenViews))
            {
                base.OpenInitialScene();
                return;
            }
            IDocumentView documentView = null;
            foreach (IConfigurationObject configurationObject in this.PersistedOpenDocumentViews.OfType<IConfigurationObject>().Reverse<IConfigurationObject>())
            {
                string propertyOrDefault = configurationObject.GetPropertyOrDefault<string>(VisualStudioSolution.SuoOpenViewSolutionRelativePathProperty);
                if (string.IsNullOrEmpty(propertyOrDefault))
                {
                    continue;
                }
                DocumentReference documentReference = DocumentReference.CreateFromRelativePath(PathHelper.GetParentDirectory(base.DocumentReference.Path), propertyOrDefault);
                IProject project = base.FindProjectContainingItem(documentReference);
                if (project == null)
                {
                    continue;
                }
                IProjectItem projectItem = project.FindItem(documentReference);
                if (projectItem == null)
                {
                    continue;
                }
                IDocumentView documentView1 = projectItem.OpenView(false);
                if (documentView1 == null || !configurationObject.GetPropertyOrDefault<bool>(VisualStudioSolution.SuoActiveViewProperty))
                {
                    continue;
                }
                documentView = documentView1;
            }
            if (documentView != null)
            {
                base.Services.DocumentService().ActiveDocument = documentView.Document;
                base.Services.ViewService().ActiveView = documentView;
            }
        }

        internal static ISolutionManagement MigrateSolution(IServiceProvider serviceProvider, SingleProjectSolution oldSolution)
        {
            string path1 = Enumerable.First<IProject>(oldSolution.Projects).ProjectRoot.Path;
            string name = Enumerable.First<IProject>(oldSolution.Projects).Name;
            string path2 = Path.ChangeExtension(Path.Combine(path1, name), ".sln");
            int num = 0;
            for (; Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path2); path2 = Path.ChangeExtension(Path.Combine(path1, name + num.ToString((IFormatProvider)CultureInfo.CurrentCulture)), ".sln"))
                ++num;
            ISolutionManagement solution = VisualStudioSolution.CreateSolution(serviceProvider, DocumentReference.Create(path2));
            if (solution == null)
                return (ISolutionManagement)null;
            SolutionBase.MoveProjects((SolutionBase)oldSolution, (SolutionBase)solution);
            oldSolution.Dispose();
            return solution;
        }

        internal static ISolutionManagement CreateSolution(IServiceProvider serviceProvider, DocumentReference solutionReference)
        {
            if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(solutionReference.Path))
                throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, StringTable.SolutionFileAlreadyExists, new object[1]
        {
          (object) solutionReference.Path
        }));
            string path = solutionReference.Path;
            string directoryName = Path.GetDirectoryName(path);
            if (!Microsoft.Expression.Framework.Documents.PathHelper.DirectoryExists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (DirectoryNotFoundException ex)
                {
                    return (ISolutionManagement)null;
                }
            }
            DocumentReference documentReference = DocumentReference.Create(path);
            return (ISolutionManagement)new VisualStudioSolution(serviceProvider, documentReference)
            {
                solutionFormat = (!Microsoft.Expression.Project.Build.BuildManager.IsBuildEngineVersionAtLeast(4, 0) ? (!Microsoft.Expression.Project.Build.BuildManager.IsBuildEngineVersionAtLeast(3, 5) ? VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2005Format : VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2008Format) : VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2010Format),
                solutionFormatComment = VisualStudioSolution.VisualStudioFormatComment,
                globalSections = {
          {
            VisualStudioSolution.SolutionConfigurationPlatforms,
            new VisualStudioSolution.SolutionSection("GlobalSection(SolutionConfigurationPlatforms) = preSolution")
            {
              Properties = {
                new SolutionBase.SolutionProperty(VisualStudioSolution.ConfigurationDebugAnyCpu, (object) VisualStudioSolution.ConfigurationDebugAnyCpu),
                new SolutionBase.SolutionProperty(VisualStudioSolution.ConfigurationReleaseAnyCpu, (object) VisualStudioSolution.ConfigurationReleaseAnyCpu)
              }
            }
          },
          {
            VisualStudioSolution.ProjectConfigurationPlatforms,
            new VisualStudioSolution.SolutionSection("GlobalSection(ProjectConfigurationPlatforms) = postSolution")
          }
        }
            };
        }

        private static VisualStudioSolution.SolutionSection CreateProjectSection(IProjectStore projectStore, DocumentReference solutionDocumentReference)
        {
            string str;
            switch (ProjectStoreHelper.GetProjectLanguage(projectStore))
            {
                case ProjectLanguage.CPlusPlus:
                    str = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
                    break;
                case ProjectLanguage.CSharp:
                    str = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
                    break;
                case ProjectLanguage.FSharp:
                    str = "{F2A71F9B-5D33-465A-A702-920D77279786}";
                    break;
                case ProjectLanguage.VisualBasic:
                    str = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
                    break;
                default:
                    str = !Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(projectStore.DocumentReference.Path) ? Guid.Empty.ToString("B").ToUpperInvariant() : "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
                    break;
            }
            return new VisualStudioSolution.SolutionSection(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", (object)str, (object)projectStore.DocumentReference.DisplayNameShort, (object)solutionDocumentReference.GetRelativePath(projectStore.DocumentReference), (object)VisualStudioSolution.GetOrCreateProjectGuid(projectStore)));
        }

        private static List<SolutionBase.SolutionProperty> CreateProjectConfigurationProperties(string projectGuid, IProject project, List<SolutionBase.SolutionProperty> solutionConfigurations, params SolutionBase.SolutionProperty[] configs)
        {
            List<SolutionBase.SolutionProperty> list = new List<SolutionBase.SolutionProperty>(configs.Length * 2);
            string str1 = projectGuid.ToUpperInvariant();
            foreach (SolutionBase.SolutionProperty solutionProperty1 in configs)
            {
                string[] strArray1 = solutionProperty1.Value.ToString().Split(VisualStudioSolution.VerticalBar);
                if (strArray1.Length == 2)
                {
                    string str2 = (string)null;
                    if (Array.Exists<string>(VisualStudioSolution.SupportedPlatforms, new Predicate<string>(strArray1[1].Equals)))
                    {
                        str2 = solutionProperty1.Value.ToString();
                    }
                    else
                    {
                        foreach (SolutionBase.SolutionProperty solutionProperty2 in solutionConfigurations)
                        {
                            string[] strArray2 = solutionProperty2.Value.ToString().Split(VisualStudioSolution.VerticalBar);
                            if (strArray2.Length == 2 && Array.Exists<string>(VisualStudioSolution.SupportedPlatforms, new Predicate<string>(strArray2[1].Equals)) && strArray2[0] == strArray1[0])
                            {
                                str2 = solutionProperty2.Value.ToString();
                                break;
                            }
                        }
                    }
                    if (str2 != null)
                    {
                        list.Add(new SolutionBase.SolutionProperty(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1}.ActiveCfg", new object[2]
            {
              (object) str1,
              solutionProperty1.Value
            }), (object)str2));
                        if (Array.Exists<string>(VisualStudioSolution.BuildablePlatforms, new Predicate<string>(strArray1[1].Equals)))
                            list.Add(new SolutionBase.SolutionProperty(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1}.Build.0", new object[2]
              {
                (object) str1,
                solutionProperty1.Value
              }), (object)str2));
                        if (project != null && project.GetCapability<bool>("DeployedProject"))
                            list.Add(new SolutionBase.SolutionProperty(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1}.Deploy.0", new object[2]
              {
                (object) str1,
                solutionProperty1.Value
              }), (object)str2));
                    }
                }
            }
            return list;
        }

        protected override void PersistSolutionSettings()
        {
            using (new WatcherSuspender((ISolutionManagement)this))
                this.PersistSettings();
        }

        private void PersistSolutionFile()
        {
            this.PersistSolutionFile((IProjectActionContext)new HandlerBasedProjectActionContext(this.Services)
            {
                ExceptionHandler = (Func<DocumentReference, Exception, bool>)((doc, e) => false)
            });
        }

        private void PersistSolutionFile(IProjectActionContext context)
        {
            using (new WatcherSuspender((ISolutionManagement)this))
            {
                if (!ProjectPathHelper.AttemptToMakeWritable(this.DocumentReference, context))
                    return;
                try
                {
                    using (FileStream fileStream = new FileStream(this.DocumentReference.Path, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream, Encoding.UTF8))
                        {
                            streamWriter.WriteLine();
                            switch (this.SolutionFormat)
                            {
                                case VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2005Format:
                                    streamWriter.WriteLine(VisualStudioSolution.VisualStudio2005Format);
                                    ProjectLog.LogSuccess(this.DocumentReference.Path, StringTable.SetSolutionFormatAction, (object)VisualStudioSolution.VisualStudio2005Format);
                                    break;
                                case VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2010Format:
                                    streamWriter.WriteLine(VisualStudioSolution.VisualStudio2010Format);
                                    ProjectLog.LogSuccess(this.DocumentReference.Path, StringTable.SetSolutionFormatAction, (object)VisualStudioSolution.VisualStudio2010Format);
                                    break;
                                default:
                                    streamWriter.WriteLine(VisualStudioSolution.VisualStudio2008Format);
                                    ProjectLog.LogSuccess(this.DocumentReference.Path, StringTable.SetSolutionFormatAction, (object)VisualStudioSolution.VisualStudio2008Format);
                                    break;
                            }
                            streamWriter.WriteLine(this.SolutionFormatComment);
                            foreach (VisualStudioSolution.SolutionSection section in this.solutionFolderProjectSections)
                                streamWriter.Write(this.WriteSolutionSection(section, 0));
                            foreach (VisualStudioSolution.SolutionSection section in this.projectSections.Values)
                                streamWriter.Write(this.WriteSolutionSection(section, 0));
                            streamWriter.WriteLine("Global");
                            foreach (VisualStudioSolution.SolutionSection section in this.globalSections.Values)
                                streamWriter.Write(this.WriteSolutionSection(section, 1));
                            streamWriter.WriteLine("EndGlobal");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProjectLog.LogError(this.DocumentReference.Path, ex, StringTable.SaveAction);
                    if (context.HandleException(this.DocumentReference, ex))
                        return;
                    throw;
                }
                this.UpdateFileInformation();
                ProjectLog.LogSuccess(this.DocumentReference.Path, StringTable.SaveAction);
            }
        }

        protected override void OnProjectAdded(NamedProjectEventArgs e)
        {
            if (!this.projectSections.ContainsKey(e.NamedProject.DocumentReference))
                this.AddProjectEntries(e.NamedProject as IProject, ((ProjectBase)e.NamedProject).ProjectStore);
            base.OnProjectAdded(e);
        }

        public override void AddProjectOutputReferences(IEnumerable<INamedProject> createdProjects)
        {
            IEnumerable<IProject> source = Enumerable.OfType<IProject>((IEnumerable)createdProjects);
            if (source == null)
                return;
            IProject projectOutputTarget = Enumerable.FirstOrDefault<IProject>(source, (Func<IProject, bool>)(project => project.ShouldReceiveProjectOutputReferences));
            IEnumerable<IProject> projectOutputSources = Enumerable.Where<IProject>(source, (Func<IProject, bool>)(project => project.ShouldProduceProjectOutputReferences));
            if (projectOutputTarget == null)
                return;
            this.CreateProjectOutputReference(projectOutputTarget, projectOutputSources);
        }

        private void CreateProjectOutputReference(IProject projectOutputTarget, IEnumerable<IProject> projectOutputSources)
        {
            foreach (IProject project in projectOutputSources)
            {
                VisualStudioSolution.ProjectOutputReferenceInformation info = new VisualStudioSolution.ProjectOutputReferenceInformation(project.ProjectGuid, "ClientBin", false);
                this.projectOutputReferenceManager.RegisterProjectOutputReference(projectOutputTarget.ProjectGuid, info);
            }
            projectOutputTarget.ProjectGuid.ToString("B");
            if (!this.projectSections.ContainsKey(projectOutputTarget.DocumentReference))
                this.AddProjectEntries(projectOutputTarget, ((ProjectBase)projectOutputTarget).ProjectStore);
            VisualStudioSolution.SolutionSection propertiesSection = this.FindProjectOutputReferencePropertiesSection(this.projectSections[projectOutputTarget.DocumentReference], "WebsiteProperties");
            if (propertiesSection == null)
                return;
            string str1 = "\"";
            foreach (IProject silverlightApp in projectOutputSources)
                str1 += this.CreateProjOutputReferenceString(silverlightApp);
            string str2 = str1 + "\"";
            propertiesSection.Properties.Add(new SolutionBase.SolutionProperty("ProjOutputReferences", (object)str2));
            this.PersistSolutionFile();
        }

        private VisualStudioSolution.SolutionSection FindProjectOutputReferencePropertiesSection(VisualStudioSolution.SolutionSection section, string projOutputReferencePropertiesSection)
        {
            foreach (VisualStudioSolution.SolutionSection solutionSection in section.NestedSections)
            {
                if (solutionSection.SectionName.Equals(projOutputReferencePropertiesSection, StringComparison.OrdinalIgnoreCase))
                    return solutionSection;
            }
            return (VisualStudioSolution.SolutionSection)null;
        }

        private void AddProjectEntries(IProject project, IProjectStore projectStore)
        {
            bool flag = Microsoft.Expression.Framework.Documents.PathHelper.IsDirectory(projectStore.DocumentReference.Path);
            this.projectSections.Add(projectStore.DocumentReference, VisualStudioSolution.CreateProjectSection(projectStore, this.DocumentReference));
            this.globalSections[VisualStudioSolution.ProjectConfigurationPlatforms].Properties.AddRange((IEnumerable<SolutionBase.SolutionProperty>)VisualStudioSolution.CreateProjectConfigurationProperties(VisualStudioSolution.GetOrCreateProjectGuid(projectStore), project, this.globalSections[VisualStudioSolution.SolutionConfigurationPlatforms].Properties, this.globalSections[VisualStudioSolution.SolutionConfigurationPlatforms].Properties.ToArray()));
            if (flag)
            {
                VisualStudioSolution.SolutionSection solutionSection = new VisualStudioSolution.SolutionSection("ProjectSection(WebsiteProperties) = preProject");
                solutionSection.Properties.Add(new SolutionBase.SolutionProperty("TargetFrameworkMoniker", (object)"\".NETFramework,Version%3Dv4.0\""));
                this.projectSections[projectStore.DocumentReference].NestedSections.Add(solutionSection);
                if (this.IsUnderSourceControl)
                {
                    solutionSection.Properties.Add(new SolutionBase.SolutionProperty("SccProjectName", (object)"SAK"));
                    solutionSection.Properties.Add(new SolutionBase.SolutionProperty("SccAuxPath", (object)"SAK"));
                    solutionSection.Properties.Add(new SolutionBase.SolutionProperty("SccLocalPath", (object)"SAK"));
                    solutionSection.Properties.Add(new SolutionBase.SolutionProperty("SccProvider", (object)"SAK"));
                }
            }
            this.PersistSolutionFile();
        }

        private string CreateProjOutputReferenceString(IProject silverlightApp)
        {
            return silverlightApp.ProjectGuid.ToString("B") + "|ClientBin|false;";
        }

        private string WriteSolutionSection(VisualStudioSolution.SolutionSection section, int indent)
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            stringBuilder.Append('\t', indent++);
            stringBuilder.AppendLine(section.SectionHeader);
            foreach (SolutionBase.SolutionProperty solutionProperty in section.Properties)
            {
                stringBuilder.Append('\t', indent);
                stringBuilder.AppendFormat((IFormatProvider)CultureInfo.InvariantCulture, "{0} = {1}", new object[2]
        {
          (object) solutionProperty.Name,
          solutionProperty.Value
        });
                stringBuilder.AppendLine();
            }
            foreach (VisualStudioSolution.SolutionSection section1 in section.NestedSections)
                stringBuilder.Append(this.WriteSolutionSection(section1, indent));
            stringBuilder.Append('\t', --indent);
            stringBuilder.Append("End");
            stringBuilder.AppendLine(section.SectionType);
            return stringBuilder.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (base.IsSafeClose)
                {
                    this.PersistSolutionSettings();
                }
                if (base.Services.AssemblyLoggingService() != null)
                {
                    base.Services.AssemblyLoggingService().Log(new AppDomainListEvent());
                }
                ISourceControlProvider sourceControlProvider = base.Services.SourceControlProvider();
                if (sourceControlProvider != null && this.hookedTfs)
                {
                    sourceControlProvider.OnlineStatusChanged -= new EventHandler<SourceControlOnlineEventArgs>(this.SourceControlProvider_OnlineStatusChanged);
                    sourceControlProvider.SetOnlineStatus(SourceControlOnlineStatus.None);
                }
                ISourceControlService sourceControlService = base.Services.SourceControlService();
                if (sourceControlService != null)
                {
                    sourceControlService.ActiveProvider = null;
                }
                base.Dispose(disposing);
            }
            finally
            {
                Microsoft.Expression.Project.Build.BuildManager.ProjectCollection.UnloadAllProjects();
            }
        }

        private void PersistSettings()
        {
            IConfigurationObject solutionConfiguration = this.SolutionConfiguration;
            solutionConfiguration.Clear();
            IConfigurationObjectCollection configurationObjectCollections = solutionConfiguration.CreateConfigurationObjectCollection();
            foreach (IProject project in base.Projects)
            {
                IConfigurationObject configurationObject = solutionConfiguration.CreateConfigurationObject();
                configurationObject.SetProperty(VisualStudioSolution.SuoProjectGuidProperty, project.ProjectGuid);
                configurationObject.SetProperty(VisualStudioSolution.SuoIsStartupProjectProperty, base.StartupProject == project);
                if (project is WebsiteProject && project.StartupItem != null)
                {
                    string relativePath = project.DocumentReference.GetRelativePath(project.StartupItem.DocumentReference);
                    configurationObject.SetProperty(VisualStudioSolution.SuoStartupHtmlProperty, relativePath);
                }
                configurationObjectCollections.Add(configurationObject);
            }
            solutionConfiguration.SetProperty(VisualStudioSolution.SuoProjectList, configurationObjectCollections);
            this.PersistOpenDocumentViews(solutionConfiguration);
            this.SolutionSettingsManager.FilterProjectSettings(this.projects);
            if (PathHelper.FileOrDirectoryExists(base.DocumentReference.Path))
            {
                this.configurationManager.Save();
            }
        }

        private void PersistOpenDocumentViews(IConfigurationObject rootObject)
        {
            IConfigurationObjectCollection persistedOpenDocumentViews = this.PersistedOpenDocumentViews;
            persistedOpenDocumentViews.Clear();
            this.SolutionConfiguration.SetProperty(VisualStudioSolution.SuoHasPersistedOpenViews, true);
            IViewCollection orderedViews = base.Services.OrderedViewProvider().OrderedViews;
            if (orderedViews == null)
            {
                return;
            }
            foreach (IDocumentView documentView in orderedViews.OfType<IDocumentView>())
            {
                IConfigurationObject configurationObject = rootObject.CreateConfigurationObject();
                string str = this.CreateSolutionRelativePath(documentView.Document.DocumentReference);
                configurationObject.SetProperty(VisualStudioSolution.SuoOpenViewSolutionRelativePathProperty, str);
                if (documentView == base.Services.ViewService().ActiveView)
                {
                    configurationObject.SetProperty(VisualStudioSolution.SuoActiveViewProperty, true);
                }
                persistedOpenDocumentViews.Add(configurationObject);
            }
        }

        private string CreateSolutionRelativePath(DocumentReference fileReference)
        {
            return this.DocumentReference.GetRelativePath(fileReference);
        }

        protected override void SaveCopy(string newRootPath, string newSolutionName)
        {
            base.SaveCopy(newRootPath, newSolutionName);
            string fileName = Path.GetFileName(this.DocumentReference.Path);
            string path1 = Path.Combine(newRootPath, newSolutionName);
            File.Move(Path.Combine(path1, fileName), Path.ChangeExtension(Path.Combine(path1, newSolutionName), ".sln"));
            string str = Path.Combine(path1, Path.ChangeExtension(fileName, ".suo"));
            if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str))
                return;
            string destFileName = Path.Combine(path1, Path.ChangeExtension(newSolutionName, ".suo"));
            File.Move(str, destFileName);
        }

        protected override bool LoadInternal()
        {
            bool flag;
            this.projectMetadata.Clear();
            this.projectSections.Clear();
            this.globalSections.Clear();
            using (FileStream fileStream = File.OpenRead(base.DocumentReference.Path))
            {
                using (VisualStudioSolution.LineReader lineReader = new VisualStudioSolution.LineReader(fileStream, Encoding.Default))
                {
                    try
                    {
                        this.ParseHeader(lineReader);
                        lineReader.IgnoreComments = true;
                        while (true)
                        {
                            string str = lineReader.ReadLine();
                            string str1 = str;
                            if (str == null)
                            {
                                break;
                            }
                            if (str1.StartsWith("Project(", StringComparison.Ordinal))
                            {
                                VisualStudioSolution.ProjectMetadata projectMetadatum = VisualStudioSolution.ProjectMetadata.Create(str1, base.DocumentReference.Path);
                                if (projectMetadatum == null)
                                {
                                    CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
                                    string solutionFormatInvalidError = StringTable.SolutionFormatInvalidError;
                                    object[] currentLine = new object[] { lineReader.CurrentLine };
                                    throw new SolutionParseException(string.Format(currentUICulture, solutionFormatInvalidError, currentLine));
                                }
                                if (projectMetadatum.ProjectTypeGuid == "{2150E333-8FDC-42A3-9474-1A3956D46DE8}")
                                {
                                    MessageBoxArgs messageBoxArg = new MessageBoxArgs();
                                    CultureInfo currentCulture = CultureInfo.CurrentCulture;
                                    string solutionFolderUnsupportedMessage = StringTable.SolutionFolderUnsupportedMessage;
                                    object[] relativePath = new object[] { projectMetadatum.RelativePath };
                                    messageBoxArg.Message = string.Format(currentCulture, solutionFolderUnsupportedMessage, relativePath);
                                    messageBoxArg.Button = MessageBoxButton.OK;
                                    messageBoxArg.Image = MessageBoxImage.Exclamation;
                                    MessageBoxArgs messageBoxArg1 = messageBoxArg;
                                    base.Services.ShowSuppressibleWarning(messageBoxArg1, "DontShowSolutionFolderWarning", MessageBoxResult.OK);
                                    this.solutionFolderProjectSections.Add(this.ParseSection(lineReader, new VisualStudioSolution.SolutionSection(str1)));
                                }
                                else if (!this.projectSections.ContainsKey(projectMetadatum.DocumentReference))
                                {
                                    this.projectMetadata.Add(projectMetadatum.DocumentReference, projectMetadatum);
                                    this.projectSections.Add(projectMetadatum.DocumentReference, this.ParseSection(lineReader, new VisualStudioSolution.SolutionSection(str1)));
                                    this.RegisterProjectOutputReferencesIfNecessary(projectMetadatum);
                                }
                                else
                                {
                                    MessageBoxArgs messageBoxArg2 = new MessageBoxArgs();
                                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                                    string projectAlreadyInSolutionMessage = StringTable.ProjectAlreadyInSolutionMessage;
                                    object[] projectName = new object[] { projectMetadatum.ProjectName };
                                    messageBoxArg2.Message = string.Format(cultureInfo, projectAlreadyInSolutionMessage, projectName);
                                    messageBoxArg2.Button = MessageBoxButton.OK;
                                    messageBoxArg2.Image = MessageBoxImage.Exclamation;
                                    messageBoxArg2.AutomationId = "OpenSolutionErrorDialog";
                                    MessageBoxArgs messageBoxArg3 = messageBoxArg2;
                                    base.Services.MessageDisplayService().ShowMessage(messageBoxArg3);
                                }
                            }
                            else if (str1 == "Global")
                            {
                                this.ParseSection(lineReader, new VisualStudioSolution.SolutionSection(str1));
                            }
                        }
                        if (!this.globalSections.ContainsKey(VisualStudioSolution.ProjectConfigurationPlatforms))
                        {
                            this.globalSections.Add(VisualStudioSolution.ProjectConfigurationPlatforms, new VisualStudioSolution.SolutionSection("GlobalSection(ProjectConfigurationPlatforms) = postSolution"));
                        }
                        if (!this.globalSections.ContainsKey(VisualStudioSolution.SolutionConfigurationPlatforms))
                        {
                            VisualStudioSolution.SolutionSection solutionSection = new VisualStudioSolution.SolutionSection("GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                            solutionSection.Properties.Add(new SolutionBase.SolutionProperty(VisualStudioSolution.ConfigurationDebugAnyCpu, VisualStudioSolution.ConfigurationDebugAnyCpu));
                            solutionSection.Properties.Add(new SolutionBase.SolutionProperty(VisualStudioSolution.ConfigurationReleaseAnyCpu, VisualStudioSolution.ConfigurationReleaseAnyCpu));
                            this.globalSections.Add(VisualStudioSolution.SolutionConfigurationPlatforms, solutionSection);
                        }
                    }
                    catch (SolutionParseException solutionParseException1)
                    {
                        SolutionParseException solutionParseException = solutionParseException1;
                        IMessageDisplayService messageDisplayService = base.Services.MessageDisplayService();
                        CultureInfo currentUICulture1 = CultureInfo.CurrentUICulture;
                        string solutionCannotBeOpenedError = StringTable.SolutionCannotBeOpenedError;
                        object[] message = new object[] { solutionParseException.Message };
                        messageDisplayService.ShowError(string.Format(currentUICulture1, solutionCannotBeOpenedError, message));
                        HostLogger.LogFormattedError(this, solutionParseException.Message, "", lineReader.CurrentLine, 0, base.DocumentReference.DisplayName);
                        flag = false;
                        return flag;
                    }
                }
                this.AttemptToActivateTFSService();
                this.configurationManager.Load();
                if (!this.InstantiateAndUpgradeProjects())
                {
                    return false;
                }
                if (this.IsSourceControlActive)
                {
                    SourceControlStatusCache.ClearStatusCache();
                    SourceControlStatusCache.UpdateStatus(this.Descendants.AppendItem<IDocumentItem>(this), base.Services.SourceControlProvider());
                }
                this.LoadSettings();
                if (!this.hookedTfs)
                {
                    ISourceControlProvider sourceControlProvider = base.Services.SourceControlProvider();
                    if (sourceControlProvider != null)
                    {
                        sourceControlProvider.OnlineStatusChanged += new EventHandler<SourceControlOnlineEventArgs>(this.SourceControlProvider_OnlineStatusChanged);
                        this.hookedTfs = true;
                    }
                }
                return true;
            }
            return flag;
        }

        private bool InstantiateAndUpgradeProjects()
        {
            bool flag;
            Func<List<IProjectStore>, List<IProjectStore>> values = (List<IProjectStore> stores) =>
            {
                foreach (VisualStudioSolution.ProjectMetadata value in this.projectMetadata.Values)
                {
                    IProjectStore projectStore = this.CreateProjectStore(value);
                    if (projectStore == null)
                    {
                        continue;
                    }
                    stores.Add(projectStore);
                }
                return stores;
            };
            Action<List<IProjectStore>> action = (List<IProjectStore> stores) =>
            {
                foreach (IProjectStore store in stores)
                {
                    store.Dispose();
                }
                stores.Clear();
            };
            List<IProjectStore> projectStores = new List<IProjectStore>();
            try
            {
                values(projectStores);
                ISolutionManagement solutionManagement = this;
                IEnumerable<IProjectStore> projectStores1 = projectStores;
                IProjectStore projectStore1 = null;
                if ((new UpgradeWizard(solutionManagement, projectStores1, projectStore1, () =>
                {
                    this.SolutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2010Format;
                    this.SolutionFormatComment = VisualStudioSolution.VisualStudioFormatComment;
                    this.PersistSolutionFile(new ProjectUpgradeContext(this.Services));
                    action(projectStores);
                    values(projectStores);
                }, base.Services)).Upgrade())
                {
                    this.OpenProjects(projectStores);
                    return true;
                }
                else
                {
                    action(projectStores);
                    flag = false;
                }
            }
            catch
            {
                action(projectStores);
                throw;
            }
            return flag;
        }

        private void AttemptToActivateTFSService()
        {
            if (!this.IsUnderSourceControl)
                return;
            string controlProviderName = this.SoureControlProviderName;
            if (string.IsNullOrEmpty(controlProviderName))
                return;
            ISourceControlService sourceControlService = ServiceExtensions.SourceControlService(this.Services);
            if (sourceControlService == null)
                return;
            sourceControlService.ActiveProvider = sourceControlService[controlProviderName];
            if (sourceControlService.ActiveProvider != null)
            {
                int num = (int)sourceControlService.ActiveProvider.OpenProject(this.DocumentReference.Path, false);
            }
            IIssueTrackingService issueTrackingService = ServiceExtensions.IssueTrackingService(this.Services);
            if (issueTrackingService == null)
                return;
            issueTrackingService.ActiveProvider = issueTrackingService[controlProviderName];
        }

        private void SourceControlProvider_OnlineStatusChanged(object sender, SourceControlOnlineEventArgs args)
        {
            UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                SourceControlStatusCache.ClearStatusCache();
                if (args.OnlineStatus != SourceControlOnlineStatus.Online)
                    return;
                SourceControlStatusCache.UpdateStatus(EnumerableExtensions.AppendItem<IDocumentItem>(this.Descendants, (IDocumentItem)this), ServiceExtensions.SourceControlProvider(this.Services));
            }));
        }

        private void RegisterProjectOutputReferencesIfNecessary(VisualStudioSolution.ProjectMetadata currentProjectMetadata)
        {
            foreach (VisualStudioSolution.ProjectOutputReferenceInformation info in (IEnumerable<VisualStudioSolution.ProjectOutputReferenceInformation>)this.FindProjectOutputReferences(currentProjectMetadata.DocumentReference))
            {
                Guid? guid = VisualStudioSolution.CreateGuid(currentProjectMetadata.ProjectGuid);
                if (guid.HasValue)
                    this.projectOutputReferenceManager.RegisterProjectOutputReference(guid.Value, info);
            }
        }

        private IList<VisualStudioSolution.ProjectOutputReferenceInformation> FindProjectOutputReferences(DocumentReference documentReference)
        {
            List<VisualStudioSolution.ProjectOutputReferenceInformation> list = new List<VisualStudioSolution.ProjectOutputReferenceInformation>();
            VisualStudioSolution.SolutionSection propertiesSection = this.FindProjectOutputReferencePropertiesSection(this.projectSections[documentReference], "WebsiteProperties");
            if (propertiesSection != null)
            {
                foreach (SolutionBase.SolutionProperty solutionProperty in propertiesSection.Properties)
                {
                    if (solutionProperty.Name.Equals("ProjOutputReferences", StringComparison.OrdinalIgnoreCase))
                    {
                        string str1 = solutionProperty.Value as string;
                        if (str1 != null)
                        {
                            string str2 = str1.Trim('"').TrimStart('"').TrimEnd(';');
                            char[] chArray = new char[1]
              {
                ';'
              };
                            foreach (string projectOutputString in str2.Split(chArray))
                                list.Add(new VisualStudioSolution.ProjectOutputReferenceInformation(projectOutputString));
                        }
                    }
                }
            }
            return (IList<VisualStudioSolution.ProjectOutputReferenceInformation>)list;
        }

        private static string GetOrCreateProjectGuid(IProjectStore projectStore)
        {
            Guid? guid = VisualStudioSolution.CreateGuid(projectStore.GetProperty("ProjectGuid"));
            if (guid.HasValue)
                return guid.Value.ToString("B");
            string str = Guid.NewGuid().ToString("B");
            projectStore.SetProperty("ProjectGuid", str);
            return str;
        }

        private static Guid? CreateGuid(string guidString)
        {
            Guid result;
            if (Guid.TryParse(guidString, out result))
                return new Guid?(result);
            return new Guid?();
        }

        private IProjectStore CreateProjectStore(VisualStudioSolution.ProjectMetadata projectMetadata)
        {
            return ProjectStoreHelper.CreateProjectStore(projectMetadata.DocumentReference, this.Services, (IEnumerable<ProjectCreator>)ProjectStoreHelper.ResilientProjectCreationChain);
        }

        private void OpenProjects(List<IProjectStore> projectStores)
        {
            bool flag;
            IProjectStore[] array = projectStores.ToArray();
            for (int i = 0; i < (int)array.Length; i++)
            {
                IProjectStore projectStore = array[i];
                if (projectStore is MigratingMSBuildStore)
                {
                    DocumentReference documentReference = projectStore.DocumentReference;
                    projectStore.Dispose();
                    projectStore = ProjectStoreHelper.CreateProjectStore(documentReference, base.Services, ProjectStoreHelper.DefaultProjectCreationChain);
                }
                if (base.OpenProject(projectStore) != null)
                {
                    Guid? nullable = VisualStudioSolution.CreateGuid(projectStore.GetProperty("ProjectGuid"));
                    if (!nullable.HasValue)
                    {
                        VisualStudioSolution.ProjectMetadata item = this.projectMetadata[projectStore.DocumentReference];
                        Guid? nullable1 = VisualStudioSolution.CreateGuid(item.ProjectGuid);
                        if (nullable1.HasValue)
                        {
                            Guid? nullable2 = nullable1;
                            Guid? nullable3 = nullable;
                            if (nullable2.HasValue != nullable3.HasValue)
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = (!nullable2.HasValue ? false : nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault());
                            }
                            if (flag)
                            {
                                Guid value = nullable1.Value;
                                projectStore.SetProperty("ProjectGuid", value.ToString());
                            }
                        }
                    }
                }
                else
                {
                    projectStore.Dispose();
                }
            }
        }

        private void ParseHeader(VisualStudioSolution.LineReader reader)
        {
            string str = reader.ReadLine();
            if (str == VisualStudioSolution.VisualStudio2005Format)
                this.solutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2005Format;
            else if (str == VisualStudioSolution.VisualStudio2008Format)
                this.solutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2008Format;
            else if (str == VisualStudioSolution.VisualStudio2010Format)
            {
                this.solutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.VisualStudio2010Format;
            }
            else
            {
                this.solutionFormat = VisualStudioSolution.VisualStudioSolutionFormat.UnknownFormat;
                if (MessagingServiceExtensions.ShowSuppressibleWarning(this.Services, new MessageBoxArgs()
                {
                    Message = string.Format((IFormatProvider)CultureInfo.CurrentCulture, StringTable.UnknownSolutionWarningMessage, new object[1]
          {
            (object) this.DocumentReference.DisplayName
          }),
                    CheckBoxMessage = StringTable.UnknownSolutionWarningCheckboxMessage,
                    Button = MessageBoxButton.YesNo,
                    Image = MessageBoxImage.Exclamation,
                    AutomationId = "UnknownSolutionWarningDialog"
                }, "AlwaysOpenUnsupportedSolutions", MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    throw new SolutionParseException(StringTable.SolutionFileParseErrorMessage);
            }
            int num = reader.Peek();
            if (num == -1 || (int)(ushort)num != 35)
                return;
            this.solutionFormatComment = reader.ReadLine();
        }

        private VisualStudioSolution.SolutionSection ParseSection(VisualStudioSolution.LineReader reader, VisualStudioSolution.SolutionSection section)
        {
            if (section.SectionHeader.StartsWith("GlobalSection(", StringComparison.Ordinal) && !this.globalSections.ContainsKey(section.SectionName))
                this.globalSections.Add(section.SectionName, section);
            string str1 = "End" + section.SectionType;
            string str2 = section.SectionType + "Section";
            string str3;
            while ((str3 = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(str3))
                {
                    if (str3.StartsWith(str1, StringComparison.Ordinal))
                        return section;
                    if (str3.StartsWith(str2, StringComparison.Ordinal))
                    {
                        section.NestedSections.Add(this.ParseSection(reader, new VisualStudioSolution.SolutionSection(str3)));
                    }
                    else
                    {
                        try
                        {
                            section.Properties.Add(this.ParseProperty(str3));
                        }
                        catch (SolutionParseException ex)
                        {
                        }
                    }
                }
            }
            return section;
        }

        private SolutionBase.SolutionProperty ParseProperty(string property)
        {
            int length = property.IndexOf('=');
            if (length == -1)
                throw new SolutionParseException();
            SolutionBase.SolutionProperty solutionProperty;
            solutionProperty.Name = property.Substring(0, length).Trim();
            int num;
            solutionProperty.Value = length != property.Length - 1 ? (object)property.Substring(num = length + 1).Trim() : (object)string.Empty;
            return solutionProperty;
        }

        private void LoadSettings()
        {
            Guid? nullable = new Guid?();
            Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
            foreach (IConfigurationObject configurationObject in (IEnumerable)ConfigurationExtensions.GetOrCreateConfigurationObjectCollectionProperty(this.SolutionConfiguration, VisualStudioSolution.SuoProjectList))
            {
                Guid propertyOrDefault1 = ConfigurationExtensions.GetPropertyOrDefault<Guid>(configurationObject, VisualStudioSolution.SuoProjectGuidProperty);
                if (ConfigurationExtensions.GetPropertyOrDefault<bool>(configurationObject, VisualStudioSolution.SuoIsStartupProjectProperty))
                    nullable = new Guid?(propertyOrDefault1);
                string propertyOrDefault2 = ConfigurationExtensions.GetPropertyOrDefault<string>(configurationObject, VisualStudioSolution.SuoStartupHtmlProperty);
                if (!string.IsNullOrEmpty(propertyOrDefault2))
                    dictionary.Add(propertyOrDefault1, propertyOrDefault2);
            }
            foreach (Guid htmlProjectGuid in dictionary.Keys)
                this.SetStartupHtml(htmlProjectGuid, dictionary[htmlProjectGuid]);
            bool flag = false;
            if (nullable.HasValue)
                flag = this.SetStartupProject(nullable.Value);
            if (flag)
                return;
            this.SetStartupProjectFromVisualStudioConfiguration();
        }

        private void SetStartupProjectFromVisualStudioConfiguration()
        {
            string str = Path.ChangeExtension(this.DocumentReference.Path, ".suo");
            if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(str) || !StructuredStorage.HasStorage(str))
                return;
            Dictionary<string, SolutionBase.SolutionProperty> dictionary = new Dictionary<string, SolutionBase.SolutionProperty>();
            using (StructuredStorage structuredStorage = StructuredStorage.OpenStorage(str, FileAccess.Read))
            {
                if (structuredStorage == null)
                    return;
                Stream input = structuredStorage.OpenStreamForRead("SolutionConfiguration");
                if (input == null)
                    return;
                using (BinaryReader reader = new BinaryReader(input, Encoding.Unicode))
                {
                    long length = reader.BaseStream.Length;
                    while (input.Position < length)
                    {
                        try
                        {
                            SolutionBase.SolutionProperty solutionProperty = this.ParseSolutionOption(reader);
                            dictionary.Add(solutionProperty.Name, solutionProperty);
                        }
                        catch (SolutionParseException ex)
                        {
                            return;
                        }
                        catch (COMException ex)
                        {
                            return;
                        }
                    }
                }
            }
            if (!dictionary.ContainsKey("StartupProject"))
                return;
            this.SetStartupProject(dictionary["StartupProject"].Value.ToString());
        }

        private bool SetStartupProject(Guid startupProjectGuid)
        {
            IProject matchByGuid = KnownProjectExtensions.FindMatchByGuid(this.Projects, startupProjectGuid);
            if (matchByGuid != null)
            {
                IExecutable executable = matchByGuid as IExecutable;
                if (executable != null)
                {
                    this.StartupProject = executable;
                    return true;
                }
            }
            return false;
        }

        private bool SetStartupProject(string startupProjectGuidString)
        {
            Guid? nullable = new Guid?();
            try
            {
                nullable = new Guid?(new Guid(startupProjectGuidString));
            }
            catch (FormatException ex)
            {
            }
            catch (OverflowException ex)
            {
            }
            if (nullable.HasValue)
                return this.SetStartupProject(nullable.Value);
            return false;
        }

        private void SetStartupHtml(Guid htmlProjectGuid, string startupHtmlPage)
        {
            IProject matchByGuid = KnownProjectExtensions.FindMatchByGuid(this.Projects, htmlProjectGuid);
            if (matchByGuid == null)
                return;
            DocumentReference fromRelativePath = DocumentReference.CreateFromRelativePath(matchByGuid.ProjectRoot.Path, startupHtmlPage);
            matchByGuid.StartupItem = matchByGuid.FindItem(fromRelativePath);
        }

        private SolutionBase.SolutionProperty ParseSolutionOption(BinaryReader reader)
        {
            SolutionBase.SolutionProperty solutionProperty = new SolutionBase.SolutionProperty((string)null, (object)null);
            uint num1 = reader.ReadUInt32();
            solutionProperty.Name = new string(reader.ReadChars((int)num1), 0, (int)num1 - 1);
            if ((int)reader.ReadChar() != 61)
                throw new SolutionParseException("Invalid options format.");
            uint num2 = (uint)reader.ReadUInt16();
            if (!Enum.IsDefined(typeof(NativeMethods.VariantTypes), (object)num2))
                throw new SolutionParseException("Invalid variant type.");
            switch ((NativeMethods.VariantTypes)num2)
            {
                case NativeMethods.VariantTypes.VT_I2:
                    solutionProperty.Value = (object)reader.ReadInt16();
                    break;
                case NativeMethods.VariantTypes.VT_I4:
                    solutionProperty.Value = (object)reader.ReadInt32();
                    break;
                case NativeMethods.VariantTypes.VT_BSTR:
                    uint num3 = reader.ReadUInt32();
                    solutionProperty.Value = (object)new string(reader.ReadChars((int)num3), 0, (int)num3);
                    break;
            }
            if ((int)reader.ReadChar() != 59)
                throw new SolutionParseException("Invalid options format.");
            return solutionProperty;
        }

        public Microsoft.Expression.Project.Build.BuildResult Build(IEnumerable<ILogger> loggers, params string[] targetNames)
        {
            Microsoft.Expression.Project.Build.BuildResult buildResult = Microsoft.Expression.Project.Build.BuildResult.Failed;
            this.projectBuildOrderLogger.ProjectOrder.Clear();
            IEnumerable<ILogger> enumerable1;
            if (loggers == null)
                enumerable1 = (IEnumerable<ILogger>)new List<ILogger>()
        {
          (ILogger) this.projectBuildOrderLogger
        };
            else
                enumerable1 = EnumerableExtensions.AppendItem<ILogger>(loggers, (ILogger)this.projectBuildOrderLogger);
            IEnumerable<ILogger> enumerable2 = enumerable1;
            Microsoft.Build.Execution.BuildManager.DefaultBuildManager.BeginBuild(new BuildParameters(Microsoft.Expression.Project.Build.BuildManager.ProjectCollection)
            {
                Loggers = enumerable2
            });
            BuildSubmission buildSubmission = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.PendBuildRequest(new BuildRequestData(this.DocumentReference.Path, (IDictionary<string, string>)new Dictionary<string, string>()
      {
        {
          MSBuildBasedProject.BuildingInBlendPropertyName,
          "true"
        }
      }, (string)null, targetNames, (HostServices)null));
            Microsoft.Build.Execution.BuildResult msBuildResult = (Microsoft.Build.Execution.BuildResult)null;
            try
            {
                buildSubmission.ExecuteAsync((BuildSubmissionCompleteCallback)null, (object)null);
                Microsoft.Build.Execution.BuildManager.DefaultBuildManager.EndBuild();
                msBuildResult = buildSubmission.BuildResult;
            }
            catch (OutOfMemoryException ex)
            {
                LowMemoryMessage.Show();
            }
            if (msBuildResult != null && Enumerable.All<string>((IEnumerable<string>)targetNames, (Func<string, bool>)(target =>
            {
                if (msBuildResult.HasResultsForTarget(target))
                    return msBuildResult[target].ResultCode == TargetResultCode.Success;
                return false;
            })))
                buildResult = Microsoft.Expression.Project.Build.BuildResult.Succeeded;
            return buildResult;
        }

        public void BuildCompleted(Microsoft.Expression.Project.Build.BuildResult buildResult)
        {
            if (this.IsDisposed)
                return;
            foreach (string url in this.projectBuildOrderLogger.ProjectOrder)
            {
                MSBuildBasedProject buildBasedProject = DocumentItemExtensions.FindMatchByUrl<IProject>(this.Projects, url) as MSBuildBasedProject;
                if (buildBasedProject != null)
                    buildBasedProject.UpdateAssemblyReferences((List<string>)null);
            }
            this.PerformWebsiteDeployment();
        }

        private void PerformWebsiteDeployment()
        {
            foreach (IProject project1 in Enumerable.OfType<IWebsiteProject>((IEnumerable)this.Projects))
            {
                foreach (string str1 in Enumerable.Select(Enumerable.Where(Enumerable.SelectMany(Enumerable.Where<VisualStudioSolution.SolutionSection>((IEnumerable<VisualStudioSolution.SolutionSection>)this.projectSections[project1.DocumentReference].NestedSections, (Func<VisualStudioSolution.SolutionSection, bool>)(section => StringComparer.OrdinalIgnoreCase.Compare(section.SectionName, "WebsiteProperties") == 0)), (Func<VisualStudioSolution.SolutionSection, IEnumerable<SolutionBase.SolutionProperty>>)(section => (IEnumerable<SolutionBase.SolutionProperty>)section.Properties), (section, property) => new
                {
                    section = section,
                    property = property
                }), param0 => StringComparer.OrdinalIgnoreCase.Compare(param0.property.Name, "ProjOutputReferences") == 0), param0 => param0.property.GetValueAs<string>()))
                {
                    char[] chArray = new char[1]
          {
            '"'
          };
                    foreach (string str2 in str1.TrimStart(chArray).TrimEnd(chArray).Split(VisualStudioSolution.SemiColon))
                    {
                        string[] strArray = str2.Split(VisualStudioSolution.VerticalBar);
                        if (strArray.Length >= 2)
                        {
                            IProject project2 = (IProject)null;
                            string str3 = strArray[1];
                            if (Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(str3))
                            {
                                Guid result;
                                if (Guid.TryParse(strArray[0], out result))
                                    project2 = KnownProjectExtensions.FindMatchByGuid(this.Projects, result);
                                if (project2 != null)
                                {
                                    if (strArray.Length > 2)
                                    {
                                        bool flag = TypeHelper.ConvertType<bool>((object)strArray[2]);
                                        string configuration = project2.Configuration;
                                        if (flag && configuration != null && Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(configuration))
                                            str3 = Path.Combine(str3, configuration);
                                    }
                                    string path1 = project1.ProjectRoot.Path;
                                    if (Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(path1))
                                    {
                                        string str4 = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(path1, str3);
                                        string fullTargetPath = project2.FullTargetPath;
                                        if (Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(fullTargetPath))
                                        {
                                            List<string> list1 = new List<string>();
                                            if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(fullTargetPath))
                                                list1.Add(fullTargetPath);
                                            foreach (string str5 in Directory.EnumerateFiles(Microsoft.Expression.Framework.Documents.PathHelper.GetDirectoryNameOrRoot(fullTargetPath), "*.zip", SearchOption.TopDirectoryOnly))
                                                list1.Add(str5);
                                            IList<MoveInfo> list2 = (IList<MoveInfo>)new List<MoveInfo>();
                                            foreach (string path2 in list1)
                                            {
                                                DocumentReference fromRelativePath = DocumentReference.CreateFromRelativePath(str4, Microsoft.Expression.Framework.Documents.PathHelper.GetFileOrDirectoryName(path2));
                                                list2.Add(new MoveInfo()
                                                {
                                                    Source = path2,
                                                    Destination = fromRelativePath.Path
                                                });
                                            }
                                            if (list2.Count > 0)
                                            {
                                                ProjectPathHelper.CopySafe((IEnumerable<MoveInfo>)list2, MoveOptions.OverwriteDestination, false);
                                                this.RefreshDeploymentDirectory((IWebsiteProject)project1, str4);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshDeploymentDirectory(IWebsiteProject websiteProject, string destDirectory)
        {
            if (!PathHelper.DirectoryExists(destDirectory))
            {
                return;
            }
            DocumentReference documentReference = DocumentReference.Create(destDirectory);
            IDocumentItem documentItem = websiteProject.FindItem(documentReference);
            if (documentItem == null)
            {
                IDocumentType item = base.Services.DocumentTypeManager().DocumentTypes[DocumentTypeNamesHelper.Folder];
                DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
                {
                    TargetPath = documentReference.Path,
                    DocumentType = item,
                    CreationOptions = CreationOptions.DoNotSelectCreatedItems
                };
                documentItem = websiteProject.AddItem(documentCreationInfo);
                if (documentItem == null)
                {
                    return;
                }
            }
            websiteProject.RefreshChildren(documentItem, false);
        }

        private class ProjectOutputReferenceManager
        {
            private Dictionary<Guid, Dictionary<Guid, IProjectOutputReferenceInformation>> designTimeUriInfos = new Dictionary<Guid, Dictionary<Guid, IProjectOutputReferenceInformation>>();

            public void RegisterProjectOutputReference(Guid targetGuid, VisualStudioSolution.ProjectOutputReferenceInformation info)
            {
                if (!info.initializedProperly)
                    return;
                Dictionary<Guid, IProjectOutputReferenceInformation> dictionary;
                if (!this.designTimeUriInfos.ContainsKey(targetGuid))
                {
                    dictionary = new Dictionary<Guid, IProjectOutputReferenceInformation>();
                    this.designTimeUriInfos.Add(targetGuid, dictionary);
                }
                else
                    dictionary = this.designTimeUriInfos[targetGuid];
                if (dictionary.ContainsKey(info.SourceGuid))
                    return;
                dictionary.Add(info.SourceGuid, (IProjectOutputReferenceInformation)info);
            }

            public IProjectOutputReferenceInformation GetProjectOutputReferenceInfo(Guid targetGuid, Guid sourceGuid)
            {
                if (this.designTimeUriInfos.ContainsKey(targetGuid) && this.designTimeUriInfos[targetGuid].ContainsKey(sourceGuid))
                    return this.designTimeUriInfos[targetGuid][sourceGuid];
                return (IProjectOutputReferenceInformation)null;
            }

            public Guid? FindFirstMatchingTargetForSource(Guid sourceGuid)
            {
                foreach (Guid index in this.designTimeUriInfos.Keys)
                {
                    if (this.designTimeUriInfos[index].ContainsKey(sourceGuid))
                        return new Guid?(index);
                }
                return new Guid?();
            }
        }

        public enum VisualStudioSolutionFormat
        {
            VisualStudio2005Format,
            VisualStudio2008Format,
            VisualStudio2010Format,
            UnknownFormat,
        }

        private class LineReader : StreamReader
        {
            private int currentLine;

            public int CurrentLine
            {
                get
                {
                    return this.currentLine;
                }
            }

            public bool IgnoreComments { get; set; }

            public LineReader(Stream stream, Encoding encoding)
                : base(stream, encoding)
            {
            }

            public override string ReadLine()
            {
                string str = base.ReadLine();
                if (str == null)
                    return (string)null;
                ++this.currentLine;
                if (str.Length < 1 || this.IgnoreComments && (int)str[0] == 35)
                    return this.ReadLine();
                return str.Trim();
            }
        }

        private class SolutionSection
        {
            private List<SolutionBase.SolutionProperty> properties = new List<SolutionBase.SolutionProperty>();
            private List<VisualStudioSolution.SolutionSection> nestedSections = new List<VisualStudioSolution.SolutionSection>();
            private string sectionHeader;
            private string sectionType;
            private string sectionName;

            internal List<SolutionBase.SolutionProperty> Properties
            {
                get
                {
                    return this.properties;
                }
            }

            internal List<VisualStudioSolution.SolutionSection> NestedSections
            {
                get
                {
                    return this.nestedSections;
                }
            }

            internal string SectionHeader
            {
                get
                {
                    return this.sectionHeader;
                }
            }

            internal string SectionType
            {
                get
                {
                    return this.sectionType;
                }
            }

            internal string SectionName
            {
                get
                {
                    return this.sectionName;
                }
            }

            internal SolutionSection(string sectionHeader)
            {
                this.sectionHeader = sectionHeader;
                int length = sectionHeader.IndexOf('(');
                if (length == -1)
                {
                    this.sectionName = sectionHeader;
                    this.sectionType = sectionHeader;
                }
                else
                {
                    this.sectionType = sectionHeader.Substring(0, length).Trim();
                    int num = sectionHeader.IndexOf(')');
                    if (num == -1)
                        throw new SolutionParseException(StringTable.SolutionFileParseErrorMessage);
                    this.sectionName = sectionHeader.Substring(length + 1, num - length - 1).Trim();
                }
            }
        }

        private sealed class ProjectOutputReferenceInformation : IProjectOutputReferenceInformation
        {
            internal bool initializedProperly;

            public Guid SourceGuid { get; private set; }

            public string RelativeOutputPath { get; private set; }

            public bool DeployToConfigurationSpecificFolder { get; private set; }

            public ProjectOutputReferenceInformation(Guid targetGuid, string relativeOutputPath, bool deployToConfigurationSpecificFolder)
            {
                if (relativeOutputPath == null || !Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(relativeOutputPath) || !Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(relativeOutputPath))
                {
                    this.initializedProperly = false;
                }
                else
                {
                    this.SourceGuid = targetGuid;
                    this.RelativeOutputPath = relativeOutputPath;
                    this.DeployToConfigurationSpecificFolder = deployToConfigurationSpecificFolder;
                    this.initializedProperly = true;
                }
            }

            public ProjectOutputReferenceInformation(string projectOutputString)
            {
                this.initializedProperly = this.TryInitializeInformation(projectOutputString);
                if (this.initializedProperly)
                    return;
                this.SourceGuid = Guid.Empty;
                this.RelativeOutputPath = (string)null;
                this.DeployToConfigurationSpecificFolder = false;
            }

            public string CreateDeploymentPath(IProject targetProject, IProject sourceProject)
            {
                if (!this.initializedProperly)
                    return (string)null;
                string rootFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(targetProject.ProjectRoot.Path, this.RelativeOutputPath));
                if (this.DeployToConfigurationSpecificFolder && sourceProject != null && !string.IsNullOrEmpty(sourceProject.Configuration))
                    rootFolder = Microsoft.Expression.Framework.Documents.PathHelper.EnsurePathEndsInDirectorySeparator(Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(rootFolder, sourceProject.Configuration));
                return rootFolder;
            }

            private bool TryInitializeInformation(string projectOutputString)
            {
                string[] strArray = projectOutputString.Split('|');
                if (strArray.Length != 3)
                    return false;
                Guid? guid = VisualStudioSolution.CreateGuid(strArray[0]);
                if (!guid.HasValue)
                    return false;
                this.SourceGuid = guid.Value;
                if (!Microsoft.Expression.Framework.Documents.PathHelper.IsValidPath(strArray[1]) || !Microsoft.Expression.Framework.Documents.PathHelper.IsPathRelative(strArray[1]))
                    return false;
                this.RelativeOutputPath = strArray[1];
                bool result;
                bool flag = bool.TryParse(strArray[2], out result);
                this.DeployToConfigurationSpecificFolder = result;
                return flag;
            }
        }

        internal sealed class ProjectMetadata
        {
            internal const string SolutionFolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
            private string projectTypeGuid;
            private string projectName;
            private string relativePath;
            private string projectGuid;
            private string originalString;
            private string solutionPath;
            private DocumentReference fullPath;

            internal string ProjectTypeGuid
            {
                get
                {
                    return this.projectTypeGuid;
                }
            }

            internal string ProjectName
            {
                get
                {
                    return this.projectName;
                }
            }

            internal string RelativePath
            {
                get
                {
                    return this.relativePath;
                }
            }

            internal string ProjectGuid
            {
                get
                {
                    return this.projectGuid;
                }
            }

            internal string OriginalString
            {
                get
                {
                    return this.originalString;
                }
            }

            internal DocumentReference DocumentReference
            {
                get
                {
                    if (this.fullPath == (DocumentReference)null)
                        this.fullPath = DocumentReference.Create(this.FixProjectPath(this.RelativePath));
                    return this.fullPath;
                }
            }

            private ProjectMetadata(string projectTypeGuid, string projectName, string relativePath, string projectGuid, string solutionPath)
            {
                this.projectTypeGuid = projectTypeGuid;
                this.projectName = projectName;
                this.relativePath = relativePath;
                this.projectGuid = projectGuid;
                this.solutionPath = solutionPath;
            }

            internal static VisualStudioSolution.ProjectMetadata Create(string line, string solutionPath)
            {
                if (line == null)
                    throw new ArgumentNullException("line");
                if (solutionPath == null)
                    throw new ArgumentNullException("solutionPath");
                Match match = new Regex("^\\s*Project\\(\"(?<PROJECTTYPEGUID>.*)\"\\)\\s*=\\s*\"(?<PROJECTNAME>.*)\"\\s*,\\s*\"(?<RELATIVEPATH>.*)\"\\s*,\\s*\"(?<PROJECTGUID>.*)\"$").Match(line);
                if (!match.Success)
                    return (VisualStudioSolution.ProjectMetadata)null;
                string str1 = match.Groups["PROJECTTYPEGUID"].Value;
                string str2 = match.Groups["PROJECTNAME"].Value;
                string str3 = match.Groups["RELATIVEPATH"].Value;
                string str4 = match.Groups["PROJECTGUID"].Value;
                return new VisualStudioSolution.ProjectMetadata(str1 == null ? string.Empty : str1.Trim(), str2 == null ? string.Empty : str2, str3 == null ? string.Empty : str3, str4 == null ? string.Empty : str4.Trim(), solutionPath)
                {
                    originalString = line
                };
            }

            private string FixProjectPath(string projectPath)
            {
                if (projectPath == null)
                    return (string)null;
                if (Microsoft.Expression.Framework.Documents.PathHelper.ValidateAndFixPathIfPossible(ref projectPath) && this.solutionPath != null)
                    projectPath = Microsoft.Expression.Framework.Documents.PathHelper.ResolveRelativePath(Path.GetDirectoryName(this.solutionPath), projectPath);
                return Microsoft.Expression.Framework.Documents.PathHelper.TrimTrailingDirectorySeparators(projectPath);
            }
        }
    }
}
