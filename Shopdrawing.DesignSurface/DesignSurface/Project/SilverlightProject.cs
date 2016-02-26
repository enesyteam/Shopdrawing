using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Interop;
using Microsoft.Expression.Framework.WebServer;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using Microsoft.Expression.Project.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Expression.DesignSurface.Project
{
    internal sealed class SilverlightProject : XamlProject
    {
        private const string WindowsCEPropertyName = "WindowsCE";

        private static int CassiniTimeout;

        private readonly static string InitialSceneName;

        private readonly static string[] PossibleStartupSceneXamlTypes;

        private IProjectItem startupScene;

        private bool? isControlLibrary;

        private int? serverHandle;

        private IWebServerService webServerService;

        private bool? cachedSupportTransparentAssemblyCacheCapability = null;

        private bool? cachedSupportEnableOutOfBrowserCapability = null;

        private bool? cachedUsePlatformExtensions;

        private bool? cachedEnableOutOfBrowser;

        private XDocument outOfBrowserSettingsDocument;

        private DateTime outOfBrowserSettingsDocumentLastWrite = DateTime.MinValue;

        private readonly static string OutOfBrowserSettingsFile;

     
        protected override bool DefinesOwnMscorlib
        {
            get
            {
                return true;
            }
        }

        public bool ElevatedOutOfBrowser
        {
            get
            {
                XAttribute elevatedOutOfBrowserElement = this.GetElevatedOutOfBrowserElement();
                if (elevatedOutOfBrowserElement == null)
                {
                    return false;
                }
                return string.Equals(elevatedOutOfBrowserElement.Value, "Required", StringComparison.Ordinal);
            }
            set
            {
                bool flag = true;
                string str = this.EnsureOutOfBrowserSettingsFile(ref flag);
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                XAttribute elevatedOutOfBrowserElement = this.GetElevatedOutOfBrowserElement();
                if (this.outOfBrowserSettingsDocument == null)
                {
                    return;
                }
                if (elevatedOutOfBrowserElement == null)
                {
                    XElement xElement = this.outOfBrowserSettingsDocument.Element("OutOfBrowserSettings");
                    if (xElement == null)
                    {
                        xElement = new XElement("OutOfBrowserSettings");
                        this.outOfBrowserSettingsDocument.Root.Add(xElement);
                    }
                    XElement xElement1 = xElement.Element("OutOfBrowserSettings.SecuritySettings");
                    if (xElement1 == null)
                    {
                        xElement1 = new XElement("OutOfBrowserSettings.SecuritySettings");
                        xElement.Add(xElement1);
                    }
                    XElement xElement2 = xElement1.Element("SecuritySettings");
                    if (xElement2 == null)
                    {
                        xElement2 = new XElement("SecuritySettings");
                        xElement1.Add(xElement2);
                    }
                    elevatedOutOfBrowserElement = new XAttribute("ElevatedPermissions", "temp");
                    xElement2.Add(elevatedOutOfBrowserElement);
                }
                else if (string.Equals(elevatedOutOfBrowserElement.Value, "Required", StringComparison.Ordinal) && value || string.Equals(elevatedOutOfBrowserElement.Value, "NotRequired", StringComparison.Ordinal) && !value)
                {
                    return;
                }
                if (!value)
                {
                    elevatedOutOfBrowserElement.Value = "NotRequired";
                }
                else
                {
                    elevatedOutOfBrowserElement.Value = "Required";
                }
                using (ProjectPathHelper.TemporaryDirectory temporaryDirectory = new ProjectPathHelper.TemporaryDirectory())
                {
                    string str1 = PathHelper.ResolveCombinedPath(temporaryDirectory.Path, SilverlightProject.OutOfBrowserSettingsFile);
                    this.outOfBrowserSettingsDocument.Save(str1);
                    DocumentCreationInfo documentCreationInfo = new DocumentCreationInfo()
                    {
                        TargetPath = str,
                        SourcePath = str1,
                        CreationOptions = CreationOptions.SilentlyOverwrite | CreationOptions.SilentlyOverwriteReadOnly | CreationOptions.DoNotSelectCreatedItems | CreationOptions.DoNotSetDefaultImportPath
                    };
                    if (base.AddItem(documentCreationInfo) != null)
                    {
                        this.outOfBrowserSettingsDocumentLastWrite = File.GetLastWriteTime(str);
                    }
                }
            }
        }

        internal bool EnableOutOfBrowser
        {
            get
            {
                if (!this.cachedEnableOutOfBrowser.HasValue)
                {
                    bool? nullablePropertyValue = this.GetNullablePropertyValue("EnableOutOfBrowser");
                    this.cachedEnableOutOfBrowser = new bool?((nullablePropertyValue.HasValue ? nullablePropertyValue.Value : false));
                }
                return this.cachedEnableOutOfBrowser.Value;
            }
            set
            {
                string lowerInvariant;
                bool flag = false;
                bool? nullablePropertyValue = this.GetNullablePropertyValue("EnableOutOfBrowser");
                if (!nullablePropertyValue.HasValue || !object.Equals(value, nullablePropertyValue.Value))
                {
                    if (!base.AttemptToMakeProjectWritable())
                    {
                        return;
                    }
                    PathHelper.ClearFileOrDirectoryReadOnlyAttribute(base.DocumentReference.Path);
                    this.cachedEnableOutOfBrowser = new bool?(value);
                    string str = value.ToString().ToLowerInvariant();
                    base.ProjectStore.SetProperty("EnableOutOfBrowser", str);
                    flag = true;
                }
                if (value && this.EnsureOutOfBrowserSettingsFile(ref flag) == null)
                {
                    if (nullablePropertyValue.HasValue)
                    {
                        lowerInvariant = nullablePropertyValue.ToString().ToLowerInvariant();
                    }
                    else
                    {
                        lowerInvariant = null;
                    }
                    base.ProjectStore.SetProperty("EnableOutOfBrowser", lowerInvariant);
                }
                if (flag)
                {
                    base.ImplicitSave();
                }
            }
        }

        public override string FullTargetPath
        {
            get
            {
                string fullTargetPath = base.FullTargetPath;
                if (string.IsNullOrEmpty(fullTargetPath))
                {
                    return fullTargetPath;
                }
                if (!TypeHelper.ConvertType<bool>(base.ProjectStore.GetProperty("XapOutputs")))
                {
                    return fullTargetPath;
                }
                string property = base.ProjectStore.GetProperty("XapFilename");
                if (string.IsNullOrEmpty(property))
                {
                    return Path.ChangeExtension(fullTargetPath, ".xap");
                }
                return PathHelper.ResolveCombinedPath(PathHelper.GetParentDirectory(fullTargetPath), property);
            }
        }

        protected override bool IsControlLibrary
        {
            get
            {
                if (!this.isControlLibrary.HasValue)
                {
                    string evaluatedPropertyValue = base.GetEvaluatedPropertyValue("SilverlightApplication");
                    this.isControlLibrary = new bool?(!string.Equals(evaluatedPropertyValue, "true", StringComparison.OrdinalIgnoreCase));
                }
                return this.isControlLibrary.Value;
            }
        }

        public override ProcessStartInfo ProcessStartInfo
        {
            get
            {
                bool flag;
                string str;
                if (!base.HasProperty("OutputPath"))
                {
                    return null;
                }
                DocumentReference documentReference = DocumentReference.CreateFromRelativePath(Path.GetDirectoryName(base.DocumentReference.Path), base.GetEvaluatedPropertyValue("OutputPath"));
                Uri potentialServerLocation = this.GetPotentialServerLocation();
                ProcessStartInfo processStartInfo = base.Services.OutOfBrowserDeploymentService().TryPerformOutOfBrowserDeployment(this, documentReference, potentialServerLocation, out flag);
                if (processStartInfo == null || flag)
                {
                    this.EnsureServer(documentReference);
                }
                if (processStartInfo == null)
                {
                    str = (base.HasProperty("TestPageFileName") ? this.webServerService.GetSessionAddress(this.serverHandle.Value, DocumentReference.CreateFromRelativePath(documentReference.Path, base.GetEvaluatedPropertyValue("TestPageFileName")).Path) : this.webServerService.GetSessionAddress(this.serverHandle.Value, documentReference.Path));
                    if (str == null)
                    {
                        return null;
                    }
                    processStartInfo = new ProcessStartInfo((new Uri(str, UriKind.Absolute)).AbsoluteUri)
                    {
                        UseShellExecute = true,
                        Verb = "Open"
                    };
                }
                return processStartInfo;
            }
        }

        public override bool ShouldProduceProjectOutputReferences
        {
            get
            {
                return true;
            }
        }

        public override string StartArguments
        {
            get
            {
                return string.Empty;
            }
        }

        public override string StartProgram
        {
            get
            {
                if (this.IsControlLibrary)
                {
                    return null;
                }
                return string.Empty;
            }
        }

        public override IProjectItem StartupItem
        {
            get
            {
                if (this.IsControlLibrary)
                {
                    return null;
                }
                if (this.startupScene == null)
                {
                    foreach (IProjectItem item in (IEnumerable<IProjectItem>)base.Items)
                    {
                        IMSBuildItem mSBuildItem = (IMSBuildItem)item;
                        if (!(item.Properties["BuildAction"] == "Page") || this.startupScene != null && string.Compare(mSBuildItem.Include, SilverlightProject.InitialSceneName, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            continue;
                        }
                        string path = item.DocumentReference.Path;
                        if (!PathHelper.FileExists(path))
                        {
                            continue;
                        }
                        using (StreamReader streamReader = File.OpenText(path))
                        {
                            char[] chrArray = new char[128];
                            streamReader.Read(chrArray, 0, (int)chrArray.Length);
                            chrArray[(int)chrArray.Length - 1] = '\0';
                            string str = new string(chrArray);
                            str = str.Trim();
                            bool flag = true;
                            string[] possibleStartupSceneXamlTypes = SilverlightProject.PossibleStartupSceneXamlTypes;
                            for (int i = 0; i < (int)possibleStartupSceneXamlTypes.Length; i++)
                            {
                                if (str.StartsWith(possibleStartupSceneXamlTypes[i], StringComparison.Ordinal))
                                {
                                    this.startupScene = item;
                                    if (string.Compare(mSBuildItem.Include, SilverlightProject.InitialSceneName, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (!flag)
                            {
                                break;
                            }
                        }
                    }
                }
                return this.startupScene;
            }
            set
            {
                if (this.startupScene != value && !this.IsControlLibrary)
                {
                    if (!base.AttemptToMakeProjectWritable())
                    {
                        return;
                    }
                    IProjectItem projectItem = this.startupScene;
                    this.startupScene = value;
                    base.OnStartupSceneChanged(new ProjectItemChangedEventArgs(projectItem, this.startupScene));
                }
            }
        }

        public string SupportedCultures
        {
            get
            {
                string evaluatedPropertyValue = base.GetEvaluatedPropertyValue("SupportedCultures");
                if (evaluatedPropertyValue == null)
                {
                    return string.Empty;
                }
                return evaluatedPropertyValue;
            }
            set
            {
                base.ProjectStore.SetProperty("SupportedCultures", value);
            }
        }

        public override ICollection<string> TemplateProjectSubtypes
        {
            get
            {
                ICollection<string> templateProjectSubtypes = base.TemplateProjectSubtypes;
                templateProjectSubtypes.Add("Silverlight");
                return templateProjectSubtypes;
            }
        }

        internal bool UsePlatformExtensions
        {
            get
            {
                if (!this.cachedUsePlatformExtensions.HasValue)
                {
                    bool? nullablePropertyValue = this.GetNullablePropertyValue("UsePlatformExtensions");
                    this.cachedUsePlatformExtensions = new bool?((nullablePropertyValue.HasValue ? nullablePropertyValue.Value : false));
                }
                return this.cachedUsePlatformExtensions.Value;
            }
            set
            {
                if (!object.Equals(value, this.GetNullablePropertyValue("UsePlatformExtensions")))
                {
                    if (!base.AttemptToMakeProjectWritable())
                    {
                        return;
                    }
                    PathHelper.ClearFileOrDirectoryReadOnlyAttribute(base.DocumentReference.Path);
                    this.cachedUsePlatformExtensions = new bool?(value);
                    string lowerInvariant = value.ToString().ToLowerInvariant();
                    base.ProjectStore.SetProperty("UsePlatformExtensions", lowerInvariant);
                    base.ImplicitSave();
                }
            }
        }

        public override string WorkingDirectory
        {
            get
            {
                return string.Empty;
            }
        }

        static SilverlightProject()
        {
            SilverlightProject.CassiniTimeout = 15000;
            SilverlightProject.InitialSceneName = "MainPage.xaml";
            SilverlightProject.PossibleStartupSceneXamlTypes = new string[] { "<UserControl", "<phoneNavigation:PhoneApplicationPage" };
            SilverlightProject.OutOfBrowserSettingsFile = "OutOfBrowserSettings.xml";
        }

        private SilverlightProject(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
            : base(projectStore, codeDocumentType, projectType, serviceProvider)
        {
            this.webServerService = serviceProvider.GetService<IWebServerService>();
        }

       
        public static new IProject Create(IProjectStore projectStore, ICodeDocumentType codeDocumentType, IProjectType projectType, IServiceProvider serviceProvider)
        {
            return KnownProjectBase.TryCreate(() => new SilverlightProject(projectStore, codeDocumentType, projectType, serviceProvider));
        }

        private BuildTaskInfoPopulator CreateLargeImageBuildTaskPopulator()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "CopyToOutputDirectory", "PreserveNewest" }
            };
            BuildTaskInfo buildTaskInfo = new BuildTaskInfo("Content", strs);
            return XamlProject.CreateLargeImagePopulator((this.IsControlLibrary ? StringTable.ImageScalabilityWarningSilverlightControlLibrary : StringTable.ImageScalabilityWarningSilverlightApplication), buildTaskInfo, base.Services);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.serverHandle.HasValue)
                {
                    this.webServerService.StopBrowsingSession(this.serverHandle.Value);
                }
                this.serverHandle = null;
            }
            base.Dispose(disposing);
        }

        protected override bool DoesErrorConditionExistInternal(string errorCondition, object parameter)
        {
            string str = errorCondition;
            if (str == null || !(str == "PotentiallyInvalidTransparentCache"))
            {
                return base.DoesErrorConditionExistInternal(errorCondition, parameter);
            }
            if ((bool)parameter)
            {
                return false;
            }
            return this.UsePlatformExtensions;
        }

        private string EnsureOutOfBrowserSettingsFile(ref bool shouldSaveProjectFile)
        {
            string str;
            string outOfBrowserSettingsFileLocation = this.GetOutOfBrowserSettingsFileLocation();
            if (string.IsNullOrEmpty(outOfBrowserSettingsFileLocation))
            {
                if (!base.AttemptToMakeProjectWritable())
                {
                    return null;
                }
                PathHelper.ClearFileOrDirectoryReadOnlyAttribute(base.DocumentReference.Path);
                string str1 = (this.PropertiesPath != null ? Path.Combine(this.PropertiesPath, SilverlightProject.OutOfBrowserSettingsFile) : SilverlightProject.OutOfBrowserSettingsFile);
                base.ProjectStore.SetProperty("OutOfBrowserSettingsFile", str1);
                shouldSaveProjectFile = true;
                outOfBrowserSettingsFileLocation = PathHelper.ResolveRelativePath(base.ProjectRoot.Path, str1);
            }
            if (!PathHelper.FileOrDirectoryExists(outOfBrowserSettingsFileLocation))
            {
                TemplateItemHelper templateItemHelper = new TemplateItemHelper(this, null, base.Services);
                IProjectItemTemplate projectItemTemplate = templateItemHelper.FindTemplateItem("OutOfBrowserSettings");
                if (projectItemTemplate != null)
                {
                    try
                    {
                        List<IProjectItem> projectItems = new List<IProjectItem>();
                        string str2 = PathHelper.EnsurePathEndsInDirectorySeparator(PathHelper.GetDirectoryNameOrRoot(outOfBrowserSettingsFileLocation));
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outOfBrowserSettingsFileLocation);
                        IEnumerable<IProjectItem> projectItems1 = templateItemHelper.AddProjectItemsForTemplateItem(projectItemTemplate, fileNameWithoutExtension, str2, CreationOptions.DoNotSelectCreatedItems, out projectItems);
                        if (projectItems1 == null || !projectItems1.CountIs<IProjectItem>(1))
                        {
                            str = null;
                        }
                        else
                        {
                            return outOfBrowserSettingsFileLocation;
                        }
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        if (!(exception is NotSupportedException) && !ErrorHandling.ShouldHandleExceptions(exception))
                        {
                            throw;
                        }
                        ErrorArgs errorArg = new ErrorArgs()
                        {
                            Exception = exception
                        };
                        base.Services.MessageDisplayService().ShowError(errorArg);
                        str = null;
                    }
                    return str;
                }
            }
            return outOfBrowserSettingsFileLocation;
        }

        private void EnsureServer(DocumentReference serverLocation)
        {
            if (this.serverHandle.HasValue && !this.webServerService.IsServerReachable(this.serverHandle.Value, SilverlightProject.CassiniTimeout))
            {
                this.webServerService.StopBrowsingSession(this.serverHandle.Value);
                this.serverHandle = null;
            }
            ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
            if (!this.serverHandle.HasValue && currentSolution != null)
            {
                WebServerSettings webServerSetting = new WebServerSettings(serverLocation.Path);
                object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "Port");
                if (projectProperty is int && this.IsPortAvailable((int)projectProperty))
                {
                    webServerSetting.Port = (int)projectProperty;
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

        public override T GetCapability<T>(string name)
        {
            string str = name;
            string str1 = str;
            if (str != null)
            {
                if (str1 == "CanHaveStartupItem")
                {
                    return TypeHelper.ConvertType<T>(false);
                }
                if (str1 == "SupportsTransparentAssemblyCache")
                {
                    if (!this.cachedSupportTransparentAssemblyCacheCapability.HasValue)
                    {
                        this.cachedSupportTransparentAssemblyCacheCapability = new bool?(!this.IsAlchemySupported());
                    }
                    return TypeHelper.ConvertType<T>(this.cachedSupportTransparentAssemblyCacheCapability.Value);
                }
                if (str1 == "SupportsEnableOutOfBrowser")
                {
                    if (!this.cachedSupportEnableOutOfBrowserCapability.HasValue)
                    {
                        this.cachedSupportEnableOutOfBrowserCapability = new bool?(!this.IsAlchemySupported());
                    }
                    return TypeHelper.ConvertType<T>(this.cachedSupportEnableOutOfBrowserCapability.Value);
                }
                if (str1 == "CanBeStartupProject")
                {
                    if (this.IsControlLibrary)
                    {
                        return TypeHelper.ConvertType<T>(false);
                    }
                    return base.GetCapability<T>(name);
                }
            }
            return base.GetCapability<T>(name);
        }

        private XAttribute GetElevatedOutOfBrowserElement()
        {
            this.RefreshOutOfBrowserXDocumentIfNewer();
            if (this.outOfBrowserSettingsDocument == null)
            {
                return null;
            }
            return (
                from setting in this.outOfBrowserSettingsDocument.Elements("OutOfBrowserSettings")
                from outOfBrowserSecuritySetting in setting.Elements("OutOfBrowserSettings.SecuritySettings")
                from securitySetting in outOfBrowserSecuritySetting.Elements("SecuritySettings")
                from elevatedAttribute in securitySetting.Attributes("ElevatedPermissions")
                select elevatedAttribute).FirstOrDefault<XAttribute>();
        }

        private bool? GetNullablePropertyValue(string propertyName)
        {
            bool flag;
            string evaluatedPropertyValue = base.GetEvaluatedPropertyValue(propertyName);
            bool? nullable = null;
            if (bool.TryParse(evaluatedPropertyValue, out flag))
            {
                nullable = new bool?(flag);
            }
            return nullable;
        }

        private string GetOutOfBrowserSettingsFileLocation()
        {
            string evaluatedPropertyValue = base.GetEvaluatedPropertyValue("OutOfBrowserSettingsFile");
            if (string.IsNullOrEmpty(evaluatedPropertyValue))
            {
                return null;
            }
            return PathHelper.ResolveRelativePath(base.ProjectRoot.Path, evaluatedPropertyValue);
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

        protected override bool Initialize()
        {
            if (!base.Initialize())
            {
                return false;
            }
            base.BuildTaskInfoPopulator = this.CreateLargeImageBuildTaskPopulator();
            return true;
        }

        private bool IsAlchemySupported()
        {
            bool flag;
            if (bool.TryParse(base.GetEvaluatedPropertyValue("WindowsCE"), out flag))
            {
                return flag;
            }
            return false;
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

        protected override bool IsValidAssemblyReference(string fullAssemblyPath, bool verifyFileExists)
        {
            bool flag = true;
            try
            {
                if (PathHelper.FileOrDirectoryExists(fullAssemblyPath))
                {
                    flag = SilverlightProjectHelper.IsSilverlightAssembly(fullAssemblyPath);
                }
            }
            catch (BadImageFormatException badImageFormatException)
            {
                flag = true;
            }
            if (!flag)
            {
                MessageBoxArgs messageBoxArg = new MessageBoxArgs();
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                string addReferenceNotBuiltAgainstSilverlightErrorMessage = StringTable.AddReferenceNotBuiltAgainstSilverlightErrorMessage;
                object[] fileOrDirectoryName = new object[] { PathHelper.GetFileOrDirectoryName(fullAssemblyPath) };
                messageBoxArg.Message = string.Format(invariantCulture, addReferenceNotBuiltAgainstSilverlightErrorMessage, fileOrDirectoryName);
                messageBoxArg.Button = MessageBoxButton.OK;
                messageBoxArg.Image = MessageBoxImage.Exclamation;
                MessageBoxArgs messageBoxArg1 = messageBoxArg;
                base.Services.MessageDisplayService().ShowMessage(messageBoxArg1);
            }
            if (!flag)
            {
                return false;
            }
            return base.IsValidAssemblyReference(fullAssemblyPath, verifyFileExists);
        }

        protected override void OnProjectItemCreated(IProjectItem item)
        {
            IDocumentTypeCollection documentTypeCollections = base.Services.DocumentTypes();
            if (item.DocumentType == documentTypeCollections[DocumentTypeNamesHelper.Xaml] || item.DocumentType == documentTypeCollections[DocumentTypeNamesHelper.ApplicationDefinition])
            {
                IMSBuildItem mSBuildItem = item as IMSBuildItem;
                if (mSBuildItem != null)
                {
                    mSBuildItem.SetMetadata("Generator", "MSBuild:MarkupCompilePass1");
                }
            }
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            this.cachedUsePlatformExtensions = null;
            this.cachedEnableOutOfBrowser = null;
            this.cachedSupportTransparentAssemblyCacheCapability = null;
            this.cachedSupportEnableOutOfBrowserCapability = null;
        }

        private void RefreshOutOfBrowserXDocumentIfNewer()
        {
            string outOfBrowserSettingsFileLocation = this.GetOutOfBrowserSettingsFileLocation();
            if (string.IsNullOrEmpty(outOfBrowserSettingsFileLocation))
            {
                return;
            }
            DateTime lastWriteTime = File.GetLastWriteTime(outOfBrowserSettingsFileLocation);
            if (this.outOfBrowserSettingsDocumentLastWrite != lastWriteTime)
            {
                this.outOfBrowserSettingsDocumentLastWrite = lastWriteTime;
                try
                {
                    this.outOfBrowserSettingsDocument = XDocument.Load(outOfBrowserSettingsFileLocation);
                }
                catch (IOException oException)
                {
                }
                catch (XmlException xmlException)
                {
                }
            }
        }

        public override bool ShouldPerformOutOfBrowserDeployment()
        {
            ISolution currentSolution = base.Services.ProjectManager().CurrentSolution;
            if (currentSolution == null)
            {
                return base.ShouldPerformOutOfBrowserDeployment();
            }
            object projectProperty = currentSolution.SolutionSettingsManager.GetProjectProperty(this, "PreviewOutOfBrowserEnabled");
            if (!this.EnableOutOfBrowser || !(projectProperty is bool))
            {
                return false;
            }
            return (bool)projectProperty;
        }
    }
}