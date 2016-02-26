// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Documents.ProjectXamlContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using Microsoft.Expression.Project.ServiceExtensions;
using Microsoft.Expression.Project.ServiceExtensions.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Documents
{
  internal sealed class ProjectXamlContext : ProjectContext, ISceneViewHost, IAttachedPropertiesProvider
  {
    private IProjectManager projectManager;
    private IAssemblyService assemblyService;
    private IProject project;
    private IPlatformService platformService;
    private IPlatform platform;
    private IViewService viewService;
    private ObservableCollection<IProjectDocument> documents;
    private IDesignerDefaultPlatformService designerDefaultPlatformService;
    private ObservableCollection<IProjectItem> projectFontDocuments;
    private Dictionary<IProjectItem, List<ProjectFont>> projectItemToFontFamilyMap;
    private Dictionary<string, List<IProjectItem>> fontFamilyToProjectItemMap;
    private ObservableCollection<IProjectFont> projectFonts;
    private Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontResolver fontResolver;
    private Dictionary<string, Dictionary<Uri, Uri>> designTimeUriCache;
    private bool openDocumentsReadOnly;
    private IAssembly projectAssembly;
    private ProjectXamlContext.ContextAssemblyCollection assemblyReferences;
    private SampleDataAwareNamespaceTypeResolver namespaces;
    private SampleDataCollection sampleData;

    public override IPlatform Platform
    {
      get
      {
        return this.platform;
      }
    }

    public string MSBuildExtensionsPath
    {
      get
      {
        MSBuildBasedProject buildBasedProject = this.project as MSBuildBasedProject;
        if (buildBasedProject == null)
          return string.Empty;
        return buildBasedProject.GetEvaluatedPropertyValue("MSBuildExtensionsPath");
      }
    }

    public SampleDataCollection SampleData
    {
      get
      {
        return this.sampleData;
      }
    }

    public SchemaManager SchemaManager { get; private set; }

    public override IAssembly ProjectAssembly
    {
      get
      {
        return this.projectAssembly;
      }
    }

    public override ICollection<IAssembly> AssemblyReferences
    {
      get
      {
        return (ICollection<IAssembly>) this.assemblyReferences;
      }
    }

    public override ICollection<IProjectDocument> Documents
    {
      get
      {
        return (ICollection<IProjectDocument>) this.documents;
      }
    }

    public override IProjectDocument Application
    {
      get
      {
        IProjectDocument localApplication = this.LocalApplication;
        if (localApplication == null)
        {
          IProject project1 = this.projectManager.CurrentSolution != null ? this.projectManager.CurrentSolution.StartupProject as IProject : (IProject) null;
          if (project1 != null && project1 != this.project && ProjectHelper.DoesProjectReferenceHierarchyContainTarget(project1, (IProjectContext) this))
          {
            IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project1);
            if (projectContext != null && PlatformTypes.PlatformsCompatible(projectContext.PlatformMetadata, this.PlatformMetadata))
              localApplication = projectContext.LocalApplication;
          }
          if (localApplication == null && this.projectManager.CurrentSolution != null)
          {
            foreach (IProject project2 in this.projectManager.CurrentSolution.Projects)
            {
              if (project2 != this.project && project2 != project1 && ProjectHelper.DoesProjectReferenceHierarchyContainTarget(project2, (IProjectContext) this))
              {
                IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project2);
                if (projectContext != null && PlatformTypes.PlatformsCompatible(projectContext.PlatformMetadata, this.PlatformMetadata))
                {
                  localApplication = projectContext.LocalApplication;
                  if (localApplication != null)
                    break;
                }
              }
            }
          }
        }
        return localApplication;
      }
    }

    public override IProjectDocument LocalApplication
    {
      get
      {
        foreach (ProjectXamlContext.ProjectDocument projectDocument in (IEnumerable<IProjectDocument>) this.Documents)
        {
          if (projectDocument.DocumentType == ProjectDocumentType.Application && this.project.Items.Contains(projectDocument.ProjectItem) && (this.projectManager.CurrentSolution != null && !this.projectManager.CurrentSolution.IsClosingAllProjects))
            return this.OpenProjectDocument(projectDocument.ProjectItem);
        }
        return (IProjectDocument) null;
      }
    }

    public override string ProjectName
    {
      get
      {
        return this.project.Name;
      }
    }

    public override string ProjectPath
    {
      get
      {
        return this.project.DocumentReference.Path;
      }
    }

    public override IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return (IXmlNamespaceTypeResolver) this.namespaces;
      }
    }

    public IAttachedPropertiesMetadata AttachedProperties { get; private set; }

    public override string RootNamespace
    {
      get
      {
        MSBuildBasedProject buildBasedProject = this.project as MSBuildBasedProject;
        if (buildBasedProject != null)
        {
          ICodeDocumentType codeDocumentType = buildBasedProject.CodeDocumentType;
          if (codeDocumentType != null && codeDocumentType.Name == "VB" && buildBasedProject.HasProperty("RootNamespace"))
            return buildBasedProject.GetEvaluatedPropertyValue("RootNamespace");
        }
        return (string) null;
      }
    }

    public override FrameworkName TargetFramework
    {
      get
      {
        return this.project.TargetFramework;
      }
    }

    protected override ICollection<IAssembly> PlatformAssemblies
    {
      get
      {
        return (ICollection<IAssembly>) new HashSet<IAssembly>(Enumerable.Where<IAssembly>((IEnumerable<IAssembly>) this.assemblyReferences, (Func<IAssembly, bool>) (assembly =>
        {
          if (assembly.IsLoaded)
            return this.assemblyService.ResolvePlatformAssembly(AssemblyHelper.GetAssemblyName(assembly)) != (Assembly) null;
          return false;
        })));
      }
    }

    protected override ICollection<IAssembly> LibraryAssemblies
    {
      get
      {
        return (ICollection<IAssembly>) new HashSet<IAssembly>(Enumerable.Where<IAssembly>((IEnumerable<IAssembly>) this.assemblyReferences, (Func<IAssembly, bool>) (assembly =>
        {
          if (!assembly.IsLoaded)
            return false;
          if (!(this.assemblyService.ResolveLibraryAssembly(AssemblyHelper.GetAssemblyName(assembly)) != (Assembly) null))
            return this.assemblyService.IsInstalledAssembly(assembly.Location);
          return true;
        })));
      }
    }

    public override ObservableCollection<IProjectFont> ProjectFonts
    {
      get
      {
        return this.projectFonts;
      }
    }

    public override IFontResolver FontResolver
    {
      get
      {
        return (IFontResolver) this.fontResolver;
      }
    }

    public override IDocumentRoot ApplicationRoot
    {
      get
      {
        IProjectDocument application = this.Application;
        if (application != null)
        {
          SceneDocument sceneDocument = application.Document as SceneDocument;
          if (sceneDocument != null)
            return (IDocumentRoot) sceneDocument.XamlDocument;
        }
        return (IDocumentRoot) null;
      }
    }

    private IEnumerable<IProjectItem> SolutionItems
    {
      get
      {
        foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.project.Items)
          yield return projectItem;
        IProjectManager projectManager = this.projectManager;
        if (projectManager.CurrentSolution != null)
        {
          foreach (IProject project in this.project.ReferencedProjects)
          {
            foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) project.Items)
              yield return projectItem;
          }
        }
      }
    }

    public string ProjectXml
    {
      get
      {
        MSBuildBasedProject buildBasedProject = this.project as MSBuildBasedProject;
        if (buildBasedProject != null)
          return buildBasedProject.ProjectXml;
        return (string) null;
      }
    }

    private IEnumerable<IProjectContext> ReferencedProjectsInSolution
    {
      get
      {
        yield return (IProjectContext) this;
        foreach (IXamlProject xamlProject in Enumerable.OfType<IXamlProject>((IEnumerable) this.project.ReferencedProjects))
        {
          if (xamlProject.ProjectContext != null)
            yield return xamlProject.ProjectContext;
        }
      }
    }

    public ProjectXamlContext(IProject project, IServiceProvider serviceProvider, bool openDocumentsReadOnly)
    {
      this.project = project;
      this.platformService = (IPlatformService) serviceProvider.GetService(typeof (IPlatformService));
      IPlatformCreator platformCreator = this.platformService.GetPlatformCreator(project.TargetFramework.Identifier);
      IReferenceAssemblyContext referenceAssemblyContext = (this.project as KnownProjectBase).ReferenceAssemblyContext;
      IPlatformReferenceContext platformReferenceContext = referenceAssemblyContext != null ? (IPlatformReferenceContext) new ProjectXamlContext.PlatformReferenceContext(referenceAssemblyContext) : (IPlatformReferenceContext) null;
      IPlatformRuntimeContext platformRuntimeContext = (IPlatformRuntimeContext) new PlatformRuntimeContext(project.TargetFramework.FullName, serviceProvider);
      if (project.TargetFramework.Identifier != "Silverlight")
      {
        this.platform = platformCreator.CreatePlatform(platformRuntimeContext, platformReferenceContext, this.platformService, (IPlatformHost) new PlatformHost());
      }
      else
      {
        try
        {
          this.platform = platformCreator.CreatePlatform(platformRuntimeContext, platformReferenceContext, this.platformService, (IPlatformHost) new PlatformHost());
        }
        catch (COMException ex)
        {
          ServiceExtensions.MessageDisplayService(serviceProvider).ShowError(StringTable.IncompatibleSilverlightVersion);
          throw;
        }
        catch (MemberAccessException ex)
        {
          ServiceExtensions.MessageDisplayService(serviceProvider).ShowError(StringTable.IncompatibleSilverlightVersion);
          throw;
        }
        catch (TypeLoadException ex)
        {
          ServiceExtensions.MessageDisplayService(serviceProvider).ShowError(StringTable.IncompatibleSilverlightVersion);
          throw;
        }
      }
      this.InitializeXamlContext(serviceProvider, openDocumentsReadOnly);
    }

    public ProjectXamlContext(IProject project, IPlatform platform, IServiceProvider serviceProvider, bool openDocumentsReadOnly)
    {
      this.platform = platform;
      this.project = project;
      this.InitializeXamlContext(serviceProvider, openDocumentsReadOnly);
    }

    private void InitializeXamlContext(IServiceProvider serviceProvider, bool openDocumentsReadOnly)
    {
      IPlatformContextChanger platformContextChanger = serviceProvider.GetService(typeof (IPlatformContextChanger)) as IPlatformContextChanger;
      if (platformContextChanger != null)
        platformContextChanger.UpdatePlatformOnSolutionChanged((IProjectContext) this);
      this.Initialize(this.Platform.Metadata);
      this.projectManager = (IProjectManager) serviceProvider.GetService(typeof (IProjectManager));
      this.assemblyService = (IAssemblyService) serviceProvider.GetService(typeof (IAssemblyService));
      this.viewService = (IViewService) serviceProvider.GetService(typeof (IViewService));
      this.designerDefaultPlatformService = (IDesignerDefaultPlatformService) serviceProvider.GetService(typeof (IDesignerDefaultPlatformService));
      this.documents = new ObservableCollection<IProjectDocument>();
      this.projectFontDocuments = new ObservableCollection<IProjectItem>();
      this.projectItemToFontFamilyMap = new Dictionary<IProjectItem, List<ProjectFont>>();
      this.fontFamilyToProjectItemMap = new Dictionary<string, List<IProjectItem>>();
      this.projectFonts = new ObservableCollection<IProjectFont>();
      bool useGdiFontNames = this.IsCapabilitySet(PlatformCapability.UsesGdiFontNames);
      this.fontResolver = new Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.FontResolver(serviceProvider, useGdiFontNames);
      this.designTimeUriCache = new Dictionary<string, Dictionary<Uri, Uri>>();
      this.openDocumentsReadOnly = openDocumentsReadOnly;
      this.assemblyReferences = new ProjectXamlContext.ContextAssemblyCollection(this, this.project.ReferencedAssemblies);
      this.UpdateProjectAssembly();
      this.InitializeAttachedProperties((IAttachedPropertyMetadataFactory) serviceProvider.GetService(typeof (IAttachedPropertyMetadataFactory)));
      this.sampleData = new SampleDataCollection((ProjectContext) this, (IMSBuildProject) new MSBuildProject(this.project), (IExpressionInformationService) serviceProvider.GetService(typeof (IExpressionInformationService)));
      this.sampleData.LoadFromProject(this.project.Items);
      this.RebuildNamespaceMaps();
      this.Platform.RefreshProjectSpecificMetadata((ITypeResolver) this, this.MetadataFactory);
      DocumentServiceExtensions.DocumentTypes(serviceProvider);
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.project.Items)
      {
        IProjectDocument projectDocument = (IProjectDocument) this.CreateProjectDocument(projectItem);
        if (projectDocument != null)
          this.documents.Add(projectDocument);
        this.CheckFontAdded(projectItem);
      }
      this.project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
      this.project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
      this.project.ItemOpened += new EventHandler<ProjectItemEventArgs>(this.Project_ItemOpened);
      this.project.ItemClosing += new EventHandler<ProjectItemEventArgs>(this.Project_ItemClosing);
      this.project.ItemClosed += new EventHandler<ProjectItemEventArgs>(this.Project_ItemClosed);
      this.project.ItemRenamed += new EventHandler<ProjectItemRenamedEventArgs>(this.Project_ItemRenamed);
      this.projectManager.ProjectClosed += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosed);
      this.projectManager.SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      this.projectManager.ProjectClosing += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      this.SchemaManager = new SchemaManager(this);
    }

    internal static ProjectXamlContext GetProjectContext(IProject project)
    {
      IXamlProject xamlProject = project as IXamlProject;
      if (xamlProject == null)
        return (ProjectXamlContext) null;
      return ProjectXamlContext.FromProjectContext(xamlProject.ProjectContext);
    }

    internal static ProjectXamlContext FromProjectContext(IProjectContext projectContext)
    {
      if (projectContext == null)
        return (ProjectXamlContext) null;
      return (ProjectXamlContext) projectContext.GetService(typeof (ProjectXamlContext));
    }

    internal static ProjectXamlContext FromDocumentNode(DocumentNode documentNode)
    {
      if (documentNode == null)
        return (ProjectXamlContext) null;
      return ProjectXamlContext.FromProjectContext((IProjectContext) documentNode.TypeResolver);
    }

    internal static ProjectXamlContext FromSceneNode(SceneNode sceneNode)
    {
      if (sceneNode == null)
        return (ProjectXamlContext) null;
      return ProjectXamlContext.FromProjectContext(sceneNode.ProjectContext);
    }

    public override Uri MakeDesignTimeUri(Uri uri, string referringDocumentUrl)
    {
      if (string.IsNullOrEmpty(referringDocumentUrl))
        return this.project.MakeDesignTimeUri(uri, referringDocumentUrl);
      Dictionary<Uri, Uri> dictionary = (Dictionary<Uri, Uri>) null;
      Uri uri1;
      if (this.designTimeUriCache.TryGetValue(referringDocumentUrl, out dictionary) && dictionary.TryGetValue(uri, out uri1))
        return uri1;
      uri1 = this.project.MakeDesignTimeUri(uri, referringDocumentUrl);
      if (dictionary == null)
      {
        dictionary = new Dictionary<Uri, Uri>();
        this.designTimeUriCache.Add(referringDocumentUrl, dictionary);
      }
      dictionary.Add(uri, uri1);
      return uri1;
    }

    public bool EnsureAssemblyReferenceMatches(IProjectContext sourceContext, IAssembly assembly)
    {
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(sourceContext);
      if (projectXamlContext == null)
        return false;
      Microsoft.Expression.Project.ProjectAssembly projectAssembly1 = Enumerable.FirstOrDefault<Microsoft.Expression.Project.ProjectAssembly>((IEnumerable<Microsoft.Expression.Project.ProjectAssembly>) projectXamlContext.project.ReferencedAssemblies, (Func<Microsoft.Expression.Project.ProjectAssembly, bool>) (referencedAssembly => assembly.CompareTo(referencedAssembly.RuntimeAssembly)));
      if (projectAssembly1 == null)
        return this.EnsureAssemblyReferenced(assembly.Name);
      Microsoft.Expression.Project.ProjectAssembly projectAssembly2 = Enumerable.FirstOrDefault<Microsoft.Expression.Project.ProjectAssembly>((IEnumerable<Microsoft.Expression.Project.ProjectAssembly>) this.project.ReferencedAssemblies, (Func<Microsoft.Expression.Project.ProjectAssembly, bool>) (referencedAssembly => assembly.CompareTo(referencedAssembly.RuntimeAssembly)));
      if (projectAssembly2 != null && (!projectAssembly2.IsImplicitlyResolved || projectAssembly1.IsImplicitlyResolved))
        return true;
      if (projectAssembly1.ProjectItem == null)
        return this.EnsureAssemblyReferenced(assembly.Name);
      return this.EnsureAssemblyReferenced(projectAssembly1.ProjectItem.DocumentReference.Path);
    }

    public void EnsureAssemblyReferencedDeferred(string assemblyPath)
    {
      this.EnsureAssemblyReferencedInternal(assemblyPath, true);
    }

    public override bool EnsureAssemblyReferenced(string assemblyPath)
    {
      return this.EnsureAssemblyReferencedInternal(assemblyPath, false);
    }

    private bool EnsureAssemblyReferencedInternal(string assemblyPath, bool shouldDeferIfImplicit)
    {
      IProject project = (IProject) null;
      IProjectDocument application = this.Application;
      if (application != null)
      {
        IDocument document = application.Document as IDocument;
        if (document != null)
        {
          IProjectItem projectItem = document.Container as IProjectItem;
          if (projectItem != null && projectItem.Project != this.project)
            project = projectItem.Project;
        }
      }
      bool flag = true;
      Microsoft.Expression.Project.ProjectAssembly projectAssembly1 = this.project.ReferencedAssemblies.Find(Path.GetFileName(assemblyPath));
      if (projectAssembly1 == null)
        flag = this.project.AddAssemblyReference(assemblyPath, true) != null;
      else if (projectAssembly1.IsImplicitlyResolved)
      {
        if (shouldDeferIfImplicit)
        {
          this.project.AddAssemblyReferenceDeferred(assemblyPath);
          flag = true;
        }
        else
          flag = this.project.AddAssemblyReference(assemblyPath, true) != null;
      }
      if (project != null)
      {
        Microsoft.Expression.Project.ProjectAssembly projectAssembly2 = project.ReferencedAssemblies.Find(Path.GetFileName(assemblyPath));
        if (projectAssembly2 == null)
          flag = flag && project.AddAssemblyReference(assemblyPath, true) != null;
        else if (projectAssembly2.IsImplicitlyResolved)
        {
          if (shouldDeferIfImplicit)
          {
            this.project.AddAssemblyReferenceDeferred(assemblyPath);
            flag = true;
          }
          else
            flag = this.project.AddAssemblyReference(assemblyPath, true) != null;
        }
      }
      return flag;
    }

    public override string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
    {
      DocumentReference documentReference = DocumentReferenceLocator.GetDocumentReference(referringDocument);
      string resource = (string) null;
      bool flag = false;
      foreach (IProjectItem projectItem in this.SolutionItems)
      {
        if (projectItem.DocumentReference.Path == resourceReference)
        {
          resource = projectItem.GetResourceReference(documentReference);
          flag = true;
          break;
        }
      }
      if (!flag && this.projectManager.CurrentSolution.StartupProject is IWebsiteProject)
      {
        IProject targetProject = (IProject) this.projectManager.CurrentSolution.StartupProject;
        IProjectOutputReferenceResolver referenceResolver = this.projectManager.CurrentSolution as IProjectOutputReferenceResolver;
        if (referenceResolver != null)
        {
          IProjectOutputReferenceInformation outputReferenceInfo = referenceResolver.GetProjectOutputReferenceInfo(targetProject, this.project);
          if (outputReferenceInfo != null)
          {
            foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) targetProject.Items)
            {
              if (projectItem.DocumentReference.Path == resourceReference && outputReferenceInfo != null)
              {
                string deploymentPath = outputReferenceInfo.CreateDeploymentPath(targetProject, this.project);
                if (!string.IsNullOrEmpty(deploymentPath))
                  resource = "/" + DocumentReference.Create(deploymentPath).GetRelativePath(projectItem.DocumentReference);
              }
            }
          }
        }
      }
      return this.SanitizeResourceReference(resource);
    }

    private string SanitizeResourceReference(string resource)
    {
      if (this.IsCapabilitySet(PlatformCapability.ShouldSanitizeResourceReferences) && resource != null)
        resource = resource.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      return resource;
    }

    public override IProjectDocument OpenDocument(string path)
    {
      IProjectDocument projectDocument = (IProjectDocument) null;
      IProjectItem solutionItem = this.FindSolutionItem(path);
      if (solutionItem != null)
        projectDocument = this.OpenProjectDocument(solutionItem);
      return projectDocument;
    }

    public T GetCapability<T>(string name)
    {
      return this.project.GetCapability<T>(name);
    }

    public override IAssembly GetDesignAssembly(IAssembly assembly)
    {
      KnownProjectBase knownProjectBase = this.project as KnownProjectBase;
      if (knownProjectBase != null)
      {
        Assembly designAssembly = knownProjectBase.GetDesignAssembly(assembly.FullName);
        if (designAssembly != (Assembly) null)
          return this.Platform.Metadata.CreateAssembly(designAssembly, AssemblySource.User);
      }
      return base.GetDesignAssembly(assembly);
    }

    public override IType GetType(IXmlNamespace xmlNamespace, string typeName)
    {
      IType type = this.sampleData.GetSampleType(xmlNamespace, typeName) ?? base.GetType(xmlNamespace, typeName);
      if (type == null && xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
        type = this.Platform.Metadata.GetDesignTimeType((ITypeResolver) this, xmlNamespace, typeName);
      return type;
    }

    public override IType GetType(Type type)
    {
      return this.sampleData.GetSampleType(type) ?? base.GetType(type);
    }

    public override IDocumentRoot GetDocumentRoot(string path)
    {
      IProjectDocument projectDocument = this.OpenDocument(path);
      if (projectDocument != null)
      {
        SceneDocument sceneDocument = projectDocument.Document as SceneDocument;
        if (sceneDocument != null)
          return (IDocumentRoot) sceneDocument.XamlDocument;
      }
      return (IDocumentRoot) null;
    }

    public SceneView OpenView(IDocumentRoot documentRoot, bool makeActive)
    {
      SceneView orOpenView = this.FindOrOpenView(documentRoot);
      if (orOpenView != null && makeActive)
        this.viewService.ActiveView = (IView) orOpenView;
      return orOpenView;
    }

    public override string ToString()
    {
      return this.project.ToString();
    }

    protected override void OnAssemblyCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      this.RebuildNamespaceMaps();
      base.OnAssemblyCollectionChanged(e);
      this.Platform.RefreshProjectSpecificMetadata((ITypeResolver) this, this.MetadataFactory);
    }

    private void OnFontDocumentAdded(IProjectItem fontDocument)
    {
      this.fontResolver.AddProjectFont(fontDocument.DocumentReference.Path);
      this.AddProjectFonts(fontDocument);
    }

    private void AddProjectFonts(IProjectItem fontDocument)
    {
      ICollection<FontFamily> fonts = this.GetFonts(fontDocument);
      List<ProjectFont> list1 = new List<ProjectFont>();
      foreach (FontFamily fontFamily in (IEnumerable<FontFamily>) fonts)
      {
        string fontNameFromSource = FontEmbedder.GetFontNameFromSource(fontFamily);
        ProjectFont projectFont1 = (ProjectFont) null;
        List<IProjectItem> list2;
        if (this.fontFamilyToProjectItemMap.TryGetValue(fontNameFromSource, out list2))
        {
          list2.Add(fontDocument);
          foreach (ProjectFont projectFont2 in (Collection<IProjectFont>) this.projectFonts)
          {
            if (projectFont2.FontFamilyName == fontNameFromSource)
            {
              projectFont1 = projectFont2;
              break;
            }
          }
        }
        else
        {
          projectFont1 = new ProjectFont(fontFamily, fontDocument);
          this.projectFonts.Add((IProjectFont) projectFont1);
          list2 = new List<IProjectItem>();
          list2.Add(fontDocument);
          this.fontFamilyToProjectItemMap[fontNameFromSource] = list2;
        }
        list1.Add(projectFont1);
        projectFont1.FontFamilies.Add(fontFamily);
        projectFont1.FontItems.Add(fontDocument);
      }
      this.projectItemToFontFamilyMap[fontDocument] = list1;
    }

    private void OnFontDocumentRemoved(IProjectItem fontDocument)
    {
      this.fontResolver.RemoveProjectFont(fontDocument.DocumentReference.Path);
      this.RemoveProjectFonts(fontDocument);
    }

    private void RemoveProjectFonts(IProjectItem fontDocument)
    {
      foreach (ProjectFont projectFont in this.projectItemToFontFamilyMap[fontDocument])
      {
        List<IProjectItem> list = this.fontFamilyToProjectItemMap[projectFont.FontFamilyName];
        list.Remove(fontDocument);
        int index = projectFont.FontItems.IndexOf(fontDocument);
        if (index != -1)
        {
          projectFont.FontItems.RemoveAt(index);
          projectFont.FontFamilies.RemoveAt(index);
        }
        if (list.Count == 0)
        {
          this.fontFamilyToProjectItemMap.Remove(projectFont.FontFamilyName);
          this.projectFonts.Remove((IProjectFont) projectFont);
        }
        if (projectFont.FontItems.Count > 0 && projectFont.FontDocumentReference == fontDocument.DocumentReference)
          projectFont.Initialize(projectFont.FontFamilies[0], projectFont.FontItems[0]);
      }
      this.projectItemToFontFamilyMap.Remove(fontDocument);
    }

    private ICollection<FontFamily> GetFonts(IProjectItem fontDocument)
    {
      List<FontFamily> list = new List<FontFamily>();
      string cachedFont = this.fontResolver.GetCachedFont(fontDocument.DocumentReference.Path);
      bool useGdiFontNames = this.IsCapabilitySet(PlatformCapability.UsesGdiFontNames);
      foreach (string familyName in FontEmbedder.GetFontNamesInFile(cachedFont, useGdiFontNames))
      {
        string str = FontFamilyHelper.EnsureFamilyName(familyName);
        list.Add(new FontFamily(FontEmbedder.MakeSilverlightFontReference(cachedFont) + "#" + str));
      }
      return (ICollection<FontFamily>) list;
    }

    private void InitializeAttachedProperties(IAttachedPropertyMetadataFactory factory)
    {
      this.AttachedProperties = factory.Create((IProjectContext) this, this.platformService, this.assemblyService);
      using (IAttachedPropertiesAccessToken propertiesAccessToken = this.AttachedProperties.Access())
        propertiesAccessToken.BeginInitializeAsync();
      this.TypesChanged += new EventHandler<TypesChangedEventArgs>(this.OnTypesChanged);
    }

    private void OnTypesChanged(object sender, TypesChangedEventArgs e)
    {
      this.AttachedProperties.OnTypesChanged(e);
    }

    private void CheckFontAdded(IProjectItem newProjectItem)
    {
      if (!(newProjectItem.DocumentType is FontDocumentType) || !newProjectItem.FileExists)
        return;
      this.projectFontDocuments.Add(newProjectItem);
      this.OnFontDocumentAdded(newProjectItem);
    }

    private void CheckFontRemoved(IProjectItem oldProjectItem)
    {
      if (!this.projectFontDocuments.Remove(oldProjectItem))
        return;
      this.OnFontDocumentRemoved(oldProjectItem);
    }

    private void CheckFontRenamed(IProjectItem projectItem, string oldFileName)
    {
      if (!this.projectFontDocuments.Contains(projectItem))
        return;
      this.fontResolver.RemoveProjectFont(oldFileName);
      this.fontResolver.AddProjectFont(projectItem.DocumentReference.Path);
      this.RemoveProjectFonts(projectItem);
      this.AddProjectFonts(projectItem);
    }

    private void UpdateProjectAssembly()
    {
      this.projectAssembly = (IAssembly) null;
      Microsoft.Expression.Project.ProjectAssembly targetAssembly = this.project.TargetAssembly;
      if (targetAssembly == null)
        return;
      this.projectAssembly = this.assemblyReferences.Find(targetAssembly);
    }

    private List<Assembly> GetDesignTimeAssemblies()
    {
      List<Assembly> list = new List<Assembly>();
      ISatelliteAssemblyResolver assemblyResolver = this.assemblyService as ISatelliteAssemblyResolver;
      KnownProjectBase knownProjectBase = this.project as KnownProjectBase;
      foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.AssemblyReferences)
      {
        AssemblyName assemblyName = (AssemblyName) null;
        if (knownProjectBase != null && assembly.FullName != null)
        {
          Assembly designAssembly = knownProjectBase.GetDesignAssembly(assembly.FullName);
          if (designAssembly != (Assembly) null)
          {
            list.Add(designAssembly);
            assemblyName = new AssemblyName(designAssembly.FullName);
          }
        }
        if (assemblyResolver != null)
        {
          if (assembly.FullName != null)
            list.AddRange(assemblyResolver.GetCachedSatelliteAssembliesForMain(new AssemblyName(assembly.FullName)));
          if (assemblyName != null)
            list.AddRange(assemblyResolver.GetCachedSatelliteAssembliesForMain(assemblyName));
        }
      }
      return list;
    }

    private void RebuildNamespaceMaps()
    {
      this.RefreshAssemblies();
      this.namespaces = new SampleDataAwareNamespaceTypeResolver((IXmlNamespaceTypeResolver) this.platform.Metadata.CreateXmlnsDefinitionMap((ITypeResolver) this, this.GetProjectAssemblies(), this.projectAssembly), this.RootNamespace);
      this.RefreshUnbuiltTypeDescriptions();
      this.designTimeUriCache.Clear();
      DesignSurfaceMetadata.InitializeMetadata();
    }

    public void RefreshAssemblies()
    {
      IPlatform platform = this.Platform;
      IEnumerable<Assembly> designTimeAssemblies = (IEnumerable<Assembly>) this.GetDesignTimeAssemblies();
      platform.Metadata.RefreshAssemblies((ITypeResolver) this, designTimeAssemblies);
      if (this.designerDefaultPlatformService == null)
        return;
      IPlatformTypes metadata = this.designerDefaultPlatformService.DefaultPlatform.Metadata;
      if (string.Equals(metadata.TargetFramework.Identifier, platform.Metadata.TargetFramework.Identifier))
        return;
      metadata.RefreshAssemblies((ITypeResolver) null, designTimeAssemblies);
    }

    public void RefreshUnbuiltTypeDescriptions()
    {
      string rootNamespace = this.RootNamespace;
      this.UnbuiltTypeInfo.Clear();
      foreach (IProjectItem projectItem in this.SolutionItems)
      {
        if (projectItem.DocumentType is XamlDocumentType)
        {
          string path = projectItem.DocumentReference.Path;
          SceneDocument sceneDocument = projectItem.Document as SceneDocument;
          Microsoft.Expression.Project.ProjectAssembly targetAssembly = projectItem.Project.TargetAssembly;
          if (targetAssembly != null)
          {
            IAssembly assembly = this.assemblyReferences.Find(targetAssembly);
            if (assembly != null && !(projectItem.Properties["BuildAction"] == "None"))
            {
              string clrNamespace = (string) null;
              string typeName = (string) null;
              string key = (string) null;
              IXmlNamespace xmlNamespace = (IXmlNamespace) null;
              ITypeId baseType = PlatformTypes.UserControl;
              if (sceneDocument != null && sceneDocument.DocumentRoot != null)
              {
                ClassAttributes rootClassAttributes = sceneDocument.DocumentRoot.RootClassAttributes;
                if (rootClassAttributes != null)
                {
                  string qualifiedClassName = rootClassAttributes.QualifiedClassName;
                  clrNamespace = TypeHelper.GetClrNamespacePart(qualifiedClassName);
                  typeName = TypeHelper.GetTypeNamePart(qualifiedClassName);
                  xmlNamespace = this.ProjectNamespaces.GetNamespace(assembly, clrNamespace);
                  key = this.GetKey((IAssemblyId) assembly, qualifiedClassName);
                  if (sceneDocument.DocumentRoot.RootNode != null)
                    baseType = (ITypeId) sceneDocument.DocumentRoot.RootNode.Type;
                }
              }
              else if (Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
              {
                try
                {
                  using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                  {
                    DocumentContext documentContext = DocumentContextHelper.CreateDocumentContext(this.project, (IProjectContext) this, DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
                    string xamlClassAttribute;
                    ITypeId type = XamlRootNodeSniffer.SniffRootNodeType((Stream) fileStream, (IDocumentContext) documentContext, out xamlClassAttribute);
                    if (!string.IsNullOrEmpty(xamlClassAttribute))
                    {
                      if (this.IsCapabilitySet(PlatformCapability.SupportsXClassRootNamespace) && !string.IsNullOrEmpty(rootNamespace))
                        xamlClassAttribute = rootNamespace + "." + xamlClassAttribute;
                      clrNamespace = TypeHelper.GetClrNamespacePart(xamlClassAttribute);
                      typeName = TypeHelper.GetTypeNamePart(xamlClassAttribute);
                      xmlNamespace = this.ProjectNamespaces.GetNamespace(assembly, clrNamespace);
                      key = this.GetKey((IAssemblyId) assembly, xamlClassAttribute);
                      if (!this.PlatformMetadata.IsNullType(type))
                        baseType = type;
                    }
                  }
                }
                catch (IOException ex)
                {
                }
              }
              if (typeName != null)
              {
                UnbuiltTypeDescription unbuiltTypeDescription = new UnbuiltTypeDescription(assembly, baseType, clrNamespace, typeName, xmlNamespace);
                unbuiltTypeDescription.XamlSourcePath = path;
                if (!this.UnbuiltTypeInfo.ContainsKey(key))
                  this.UnbuiltTypeInfo.Add(key, unbuiltTypeDescription);
              }
            }
          }
        }
      }
    }

    private SceneView FindOrOpenView(IDocumentRoot documentRoot)
    {
      foreach (IView view in (IEnumerable<IView>) this.viewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && sceneView.Document.DocumentRoot == documentRoot)
          return sceneView;
      }
      IProjectItem solutionItem = this.FindSolutionItem(documentRoot);
      if (solutionItem != null)
        return solutionItem.OpenView(false) as SceneView;
      return (SceneView) null;
    }

    private IProjectItem FindSolutionItem(IDocumentRoot documentRoot)
    {
      IProjectItem projectItem = this.FindProjectItem(documentRoot, this.project);
      if (projectItem == null)
      {
        foreach (IProject project in this.projectManager.CurrentSolution.Projects)
        {
          if (project != this.project)
          {
            projectItem = this.FindProjectItem(documentRoot, project);
            if (projectItem != null)
              break;
          }
        }
      }
      return projectItem;
    }

    private IProjectItem FindProjectItem(IDocumentRoot documentRoot, IProject project)
    {
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) project.Items)
      {
        if (this.GetDocumentType(projectItem) != ProjectDocumentType.Invalid)
        {
          SceneDocument sceneDocument = projectItem.Document as SceneDocument;
          if (sceneDocument != null && sceneDocument.DocumentRoot == documentRoot)
            return projectItem;
        }
      }
      return (IProjectItem) null;
    }

    private IProjectItem FindSolutionItem(string path)
    {
      DocumentReference documentReference = DocumentReference.Create(path);
      IProjectItem projectItem = this.project.FindItem(documentReference);
      if (projectItem == null)
      {
        foreach (IProject project in this.projectManager.CurrentSolution.Projects)
        {
          if (project != this.project)
          {
            projectItem = project.FindItem(documentReference);
            if (projectItem != null)
              break;
          }
        }
      }
      return projectItem;
    }

    private IProjectDocument OpenProjectDocument(IProjectItem projectItem)
    {
      ProjectXamlContext.ProjectDocument projectDocument = this.FindProjectDocument(projectItem);
      if (projectDocument != null && projectItem.Document == null)
      {
        if (!projectItem.FileExists)
          return (IProjectDocument) null;
        projectItem.OpenDocument(this.openDocumentsReadOnly);
        if (projectItem.Document == null)
          projectDocument = (ProjectXamlContext.ProjectDocument) null;
      }
      return (IProjectDocument) projectDocument;
    }

    private ProjectDocumentType GetDocumentType(IProjectItem projectItem)
    {
      ProjectDocumentType projectDocumentType = ProjectDocumentType.Invalid;
      if (projectItem.DocumentType is XamlDocumentType)
      {
        if (projectItem.Document != null)
          projectDocumentType = ((SceneDocument) projectItem.Document).ProjectDocumentType;
        else if (projectItem.FileExists)
        {
          try
          {
            using (FileStream fileStream = new FileStream(projectItem.DocumentReference.Path, FileMode.Open, FileAccess.Read))
            {
              DocumentContext documentContext = DocumentContextHelper.CreateDocumentContext(this.project, (IProjectContext) this, DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
              string xamlClassAttribute;
              ITypeId rootType = XamlRootNodeSniffer.SniffRootNodeType((Stream) fileStream, (IDocumentContext) documentContext, out xamlClassAttribute);
              if (rootType != null)
                projectDocumentType = SceneDocument.GetProjectDocumentTypeFromType(rootType);
            }
          }
          catch (Exception ex)
          {
            if (!ErrorHandling.ShouldHandleExceptions(ex))
              throw;
          }
        }
      }
      return projectDocumentType;
    }

    private ProjectXamlContext.ProjectDocument CreateProjectDocument(IProjectItem projectItem)
    {
      ProjectDocumentType documentType = this.GetDocumentType(projectItem);
      if (documentType != ProjectDocumentType.Invalid)
        return new ProjectXamlContext.ProjectDocument(projectItem, documentType);
      return (ProjectXamlContext.ProjectDocument) null;
    }

    private ProjectXamlContext.ProjectDocument FindProjectDocument(IProjectItem projectItem)
    {
      foreach (ProjectXamlContext.ProjectDocument projectDocument in (Collection<IProjectDocument>) this.documents)
      {
        if (projectDocument.ProjectItem == projectItem)
          return projectDocument;
      }
      return this.CreateProjectDocument(projectItem);
    }

    public override IProjectDocument GetDocument(IDocumentLocator DocumentLocator)
    {
      foreach (ProjectXamlContext.ProjectDocument projectDocument in (Collection<IProjectDocument>) this.documents)
      {
        if (projectDocument.ProjectItem.DocumentReference == DocumentReferenceLocator.GetDocumentReference(DocumentLocator))
          return (IProjectDocument) projectDocument;
      }
      return (IProjectDocument) null;
    }

    public override IProjectDocument GetDocument(IDocumentRoot documentRoot)
    {
      foreach (ProjectXamlContext.ProjectDocument projectDocument in (Collection<IProjectDocument>) this.documents)
      {
        SceneDocument sceneDocument = projectDocument.ProjectItem.Document as SceneDocument;
        if (sceneDocument != null && sceneDocument.DocumentRoot == documentRoot)
          return (IProjectDocument) projectDocument;
      }
      return (IProjectDocument) null;
    }

    private IEnumerable<IAssembly> GetProjectAssemblies()
    {
      foreach (Microsoft.Expression.Project.ProjectAssembly projectAssembly in (IEnumerable<Microsoft.Expression.Project.ProjectAssembly>) this.project.ReferencedAssemblies)
      {
        Assembly assembly = projectAssembly.RuntimeAssembly;
        if (assembly != (Assembly) null)
          yield return this.Platform.Metadata.CreateAssembly(assembly, AssemblySource.Unknown);
      }
    }

    private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
    {
      this.CheckFontAdded(e.ProjectItem);
      IProjectDocument projectDocument = (IProjectDocument) this.CreateProjectDocument(e.ProjectItem);
      if (projectDocument != null)
        this.documents.Add(projectDocument);
      if (e.ProjectItem.DocumentType is XamlDocumentType)
        this.RefreshUnbuiltTypeDescriptions();
      this.designTimeUriCache.Clear();
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      this.CheckFontRemoved(e.ProjectItem);
      ProjectXamlContext.ProjectDocument projectDocument = this.FindProjectDocument(e.ProjectItem);
      if (projectDocument != null)
      {
        this.documents.Remove((IProjectDocument) projectDocument);
        if (e.ProjectItem.DocumentType is XamlDocumentType)
          this.RefreshUnbuiltTypeDescriptions();
      }
      this.designTimeUriCache.Clear();
    }

    private void Project_ItemOpened(object sender, ProjectItemEventArgs e)
    {
      ProjectXamlContext.ProjectDocument projectDocument = this.FindProjectDocument(e.ProjectItem);
      if (projectDocument == null)
        return;
      if (!this.documents.Contains((IProjectDocument) projectDocument))
        this.documents.Add((IProjectDocument) projectDocument);
      this.OnDocumentOpened((IProjectDocument) projectDocument);
    }

    private void Project_ItemClosing(object sender, ProjectItemEventArgs e)
    {
      ProjectXamlContext.ProjectDocument projectDocument = this.FindProjectDocument(e.ProjectItem);
      if (projectDocument == null)
        return;
      this.OnDocumentClosing((IProjectDocument) projectDocument);
    }

    private void Project_ItemClosed(object sender, ProjectItemEventArgs e)
    {
      ProjectXamlContext.ProjectDocument projectDocument = this.FindProjectDocument(e.ProjectItem);
      if (projectDocument == null)
        return;
      this.OnDocumentClosed((IProjectDocument) projectDocument);
    }

    private void Project_ItemRenamed(object sender, ProjectItemRenamedEventArgs e)
    {
      this.CheckFontRenamed(e.ProjectItem, e.OldName.Path);
      this.designTimeUriCache.Clear();
      if (!(e.ProjectItem.DocumentType is XamlDocumentType))
        return;
      this.RefreshUnbuiltTypeDescriptions();
    }

    private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
    {
      this.RefreshUnbuiltTypeDescriptions();
    }

    private void ProjectManager_ProjectClosing(object sender, ProjectEventArgs e)
    {
      if (e.Project != this.project)
        return;
      this.sampleData.Save();
    }

    private void ProjectManager_ProjectClosed(object sender, ProjectEventArgs e)
    {
      if (e.Project != this.project)
        return;
      this.projectManager.SolutionOpened -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      this.projectManager.ProjectClosed -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosed);
      this.projectManager.ProjectClosing -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      if (this.AttachedProperties != null)
      {
        this.AttachedProperties.CancelScanAssembliesAsync();
        this.AttachedProperties.Dispose();
        this.AttachedProperties = (IAttachedPropertiesMetadata) null;
      }
      this.TypesChanged -= new EventHandler<TypesChangedEventArgs>(this.OnTypesChanged);
      this.SampleData.Close();
      if (this.platformService != null)
        this.platformService.GetPlatformCreator(this.project.TargetFramework.Identifier).ReleasePlatform(this.Platform);
      if (this.SchemaManager == null)
        return;
      this.SchemaManager.Dispose();
      this.SchemaManager = (SchemaManager) null;
    }

    private string PlatformCapabilityToProjectCapability(PlatformCapability platformCapability)
    {
      switch (platformCapability)
      {
        case PlatformCapability.SupportsDatabinding:
          return "SupportsDatabinding";
        case PlatformCapability.SupportsMediaElementControl:
          return "SupportsMediaElementControl";
        case PlatformCapability.SupportsHyperlinkButtonControl:
          return "SupportsHyperlinkButtonControl";
        case PlatformCapability.SupportsAssetLibraryBehaviorsItems:
          return "SupportsAssetLibraryBehaviorsItems";
        case PlatformCapability.SupportsUIElementEffectProperty:
          return "SupportsUIElementEffectProperty";
        case PlatformCapability.SupportsTransparentAssemblyCache:
          return "SupportsTransparentAssemblyCache";
        case PlatformCapability.SupportsEnableOutOfBrowser:
          return "SupportsEnableOutOfBrowser";
        case PlatformCapability.PlatformSimpleName:
          return "PlatformSimpleName";
        default:
          throw new ArgumentOutOfRangeException("platformCapability", (object) platformCapability, "PlatformCapabilityToProjectCapability: Unknown Project Capability");
      }
    }

    public override bool IsCapabilitySet(PlatformCapability capability)
    {
      switch (capability)
      {
        case PlatformCapability.SupportsDatabinding:
        case PlatformCapability.SupportsMediaElementControl:
        case PlatformCapability.SupportsHyperlinkButtonControl:
        case PlatformCapability.SupportsAssetLibraryBehaviorsItems:
        case PlatformCapability.SupportsUIElementEffectProperty:
          if (this.project == null)
            return base.IsCapabilitySet(capability);
          if (base.IsCapabilitySet(capability))
            return this.project.GetCapability<bool>(this.PlatformCapabilityToProjectCapability(capability));
          return false;
        case PlatformCapability.VsmInToolkit:
          if (this.IsCapabilitySet(PlatformCapability.IsWpf) && this.TargetFramework.Version >= new Version(4, 0))
            return false;
          return base.IsCapabilitySet(capability);
        case PlatformCapability.VsmEverywhereByDefault:
          if (this.IsCapabilitySet(PlatformCapability.IsWpf) && this.TargetFramework.Version >= new Version(4, 0))
            return true;
          return base.IsCapabilitySet(capability);
        case PlatformCapability.SupportsVisualStateManager:
          if (!this.PlatformMetadata.IsNullType((ITypeId) this.ResolveType(ProjectNeutralTypes.VisualStateManager)))
            return true;
          if (this.IsCapabilitySet(PlatformCapability.VsmInToolkit))
            return ToolkitHelper.GetToolkitPath() != null;
          return false;
        case PlatformCapability.SupportPrototyping:
          return this.GetCapability<bool>("ExpressionBlendPrototypingEnabled");
        default:
          return base.IsCapabilitySet(capability);
      }
    }

    public override object GetCapabilityValue(PlatformCapability capability)
    {
      switch (capability)
      {
        case PlatformCapability.SupportsTransparentAssemblyCache:
          if (this.project != null)
            return (object) this.project.GetCapability<bool?>(this.PlatformCapabilityToProjectCapability(capability));
          return base.GetCapabilityValue(capability);
        case PlatformCapability.PlatformSimpleName:
          object obj = (object) null;
          if (this.project != null)
            obj = (object) this.project.GetCapability<string>(this.PlatformCapabilityToProjectCapability(capability));
          if (obj == null)
            obj = base.GetCapabilityValue(capability);
          return obj;
        default:
          return base.GetCapabilityValue(capability);
      }
    }

    public override object GetService(Type serviceType)
    {
      object obj = base.GetService(serviceType);
      if (obj == null)
      {
        if (typeof (IProject).IsAssignableFrom(serviceType))
          obj = (object) this.project;
        else if (typeof (SampleDataCollection).IsAssignableFrom(serviceType))
          obj = (object) this.SampleData;
      }
      return obj;
    }

    public override bool IsTypeInSolution(IType type)
    {
      if (type.RuntimeAssembly != null)
        return Enumerable.Any<IProjectContext>(this.ReferencedProjectsInSolution, (Func<IProjectContext, bool>) (context => type.RuntimeAssembly.Equals((object) context.ProjectAssembly)));
      return false;
    }

    private sealed class ContextAssemblyCollection : ICollection<IAssembly>, IEnumerable<IAssembly>, IEnumerable, INotifyCollectionChanges, INotifyCollectionChanged
    {
      private ProjectXamlContext projectContext;
      private IAssemblyCollection assemblies;
      private List<IAssembly> assemblyReferences;

      public int Count
      {
        get
        {
          return this.assemblyReferences.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public event NotifyCollectionChangedEventHandler CollectionChanging;

      public event NotifyCollectionChangedEventHandler CollectionChanged;

      public ContextAssemblyCollection(ProjectXamlContext projectContext, IAssemblyCollection assemblies)
      {
        this.projectContext = projectContext;
        this.assemblies = assemblies;
        this.Reset();
        INotifyCollectionChanges collectionChanges = (INotifyCollectionChanges) this.assemblies;
        if (collectionChanges == null)
          return;
        collectionChanges.CollectionChanging += new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanging);
        collectionChanges.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AssemblyCollection_CollectionChanged);
      }

      public IAssembly Find(Microsoft.Expression.Project.ProjectAssembly assembly)
      {
        foreach (IAssembly assembly1 in this.assemblyReferences)
        {
          if (assembly.Name == assembly1.Name)
            return assembly1;
        }
        return (IAssembly) null;
      }

      public void Add(IAssembly item)
      {
        throw new NotSupportedException();
      }

      public void Clear()
      {
        throw new NotSupportedException();
      }

      public bool Contains(IAssembly item)
      {
        return this.assemblyReferences.Contains(item);
      }

      public void CopyTo(IAssembly[] array, int arrayIndex)
      {
        this.assemblyReferences.CopyTo(array, arrayIndex);
      }

      public bool Remove(IAssembly item)
      {
        throw new NotSupportedException();
      }

      public IEnumerator<IAssembly> GetEnumerator()
      {
        return (IEnumerator<IAssembly>) this.assemblyReferences.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private void Reset()
      {
        this.assemblyReferences = new List<IAssembly>();
        foreach (Microsoft.Expression.Project.ProjectAssembly assembly in (IEnumerable<Microsoft.Expression.Project.ProjectAssembly>) this.assemblies)
          this.assemblyReferences.Add(this.CreateAssemblyReference(assembly));
      }

      private void AssemblyCollection_CollectionChanging(object sender, NotifyCollectionChangedEventArgs e)
      {
        e = this.UseAssemblyReference(e);
        this.projectContext.OnAssemblyCollectionChanging(e);
        if (this.CollectionChanging == null)
          return;
        this.CollectionChanging((object) this, e);
      }

      private void AssemblyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        e = this.UseAssemblyReference(e);
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            int newStartingIndex = e.NewStartingIndex;
            IEnumerator enumerator1 = e.NewItems.GetEnumerator();
            try
            {
              while (enumerator1.MoveNext())
              {
                IAssembly assembly = (IAssembly) enumerator1.Current;
                this.assemblyReferences.Insert(newStartingIndex, assembly);
                ++newStartingIndex;
              }
              break;
            }
            finally
            {
              IDisposable disposable = enumerator1 as IDisposable;
              if (disposable != null)
                disposable.Dispose();
            }
          case NotifyCollectionChangedAction.Remove:
            IEnumerator enumerator2 = e.OldItems.GetEnumerator();
            try
            {
              while (enumerator2.MoveNext())
                this.assemblyReferences.Remove((IAssembly) enumerator2.Current);
              break;
            }
            finally
            {
              IDisposable disposable = enumerator2 as IDisposable;
              if (disposable != null)
                disposable.Dispose();
            }
          case NotifyCollectionChangedAction.Reset:
            this.Reset();
            break;
        }
        this.projectContext.UpdateProjectAssembly();
        this.projectContext.OnAssemblyCollectionChanged(e);
        if (this.CollectionChanged == null)
          return;
        this.CollectionChanged((object) this, e);
      }

      [Conditional("DEBUG")]
      private void CheckInvariants()
      {
        using (IEnumerator<Microsoft.Expression.Project.ProjectAssembly> enumerator = this.assemblies.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Microsoft.Expression.Project.ProjectAssembly assembly = enumerator.Current;
            try
            {
              Enumerable.First<IAssembly>((IEnumerable<IAssembly>) this.assemblyReferences, (Func<IAssembly, bool>) (a => a.Name.Equals(assembly.Name)));
            }
            catch (Exception ex)
            {
            }
          }
        }
      }

      private IAssembly CreateAssemblyReference(Microsoft.Expression.Project.ProjectAssembly assembly)
      {
        Assembly runtimeAssembly = assembly.RuntimeAssembly;
        if (runtimeAssembly != (Assembly) null)
          return this.projectContext.Platform.Metadata.CreateAssembly(runtimeAssembly, AssemblySource.Unknown, assembly.IsImplicitlyResolved);
        return this.projectContext.Platform.Metadata.CreateAssembly(assembly.Name);
      }

      private IList CreateAssemblyReferences(IList assemblies)
      {
        ArrayList arrayList = new ArrayList(assemblies.Count);
        foreach (Microsoft.Expression.Project.ProjectAssembly assembly in (IEnumerable) assemblies)
          arrayList.Add((object) this.CreateAssemblyReference(assembly));
        return (IList) arrayList;
      }

      private NotifyCollectionChangedEventArgs UseAssemblyReference(NotifyCollectionChangedEventArgs e)
      {
        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            return new NotifyCollectionChangedEventArgs(e.Action, this.CreateAssemblyReferences(e.NewItems), e.NewStartingIndex);
          case NotifyCollectionChangedAction.Remove:
            return new NotifyCollectionChangedEventArgs(e.Action, this.CreateAssemblyReferences(e.OldItems), e.OldStartingIndex);
          case NotifyCollectionChangedAction.Reset:
            return e;
          default:
            throw new NotSupportedException();
        }
      }
    }

    private sealed class ProjectDocument : IProjectDocument
    {
      private IProjectItem projectItem;
      private ProjectDocumentType documentType;

      public IProjectItem ProjectItem
      {
        get
        {
          return this.projectItem;
        }
      }

      public string Path
      {
        get
        {
          return this.projectItem.DocumentReference.Path;
        }
      }

      public ProjectDocumentType DocumentType
      {
        get
        {
          return this.documentType;
        }
      }

      public IDocumentRoot DocumentRoot
      {
        get
        {
          SceneDocument sceneDocument = this.projectItem.Document as SceneDocument;
          if (sceneDocument == null)
            return (IDocumentRoot) null;
          return (IDocumentRoot) sceneDocument.XamlDocument;
        }
      }

      public object Document
      {
        get
        {
          return (object) this.projectItem.Document;
        }
      }

      public bool IsDirty
      {
        get
        {
          return this.projectItem.IsDirty;
        }
      }

      public ProjectDocument(IProjectItem projectItem, ProjectDocumentType documentType)
      {
        this.projectItem = projectItem;
        this.documentType = documentType;
      }

      public override string ToString()
      {
        return this.projectItem.ToString();
      }
    }

    private sealed class PlatformReferenceContext : IPlatformReferenceContext
    {
      private IReferenceAssemblyContext referenceAssemblyContext;

      public FrameworkName TargetFramework
      {
        get
        {
          return this.referenceAssemblyContext.TargetFramework;
        }
      }

      public bool KeepAlive
      {
        get
        {
          return this.referenceAssemblyContext.ReferenceAssemblyMode == ReferenceAssemblyMode.TargetFramework;
        }
      }

      public PlatformReferenceContext(IReferenceAssemblyContext referenceAssemblyContext)
      {
        this.referenceAssemblyContext = referenceAssemblyContext;
      }

      public Assembly ResolveReferenceAssembly(Assembly runtimeAssembly)
      {
        return this.referenceAssemblyContext.ResolveReferenceAssembly(runtimeAssembly);
      }

      public Assembly ResolveReferenceAssembly(AssemblyName assemblyName)
      {
        return this.referenceAssemblyContext.ResolveReferenceAssembly(assemblyName);
      }

      public override bool Equals(object obj)
      {
        ProjectXamlContext.PlatformReferenceContext referenceContext = obj as ProjectXamlContext.PlatformReferenceContext;
        if (referenceContext != null)
          return this.referenceAssemblyContext.Identifier == referenceContext.referenceAssemblyContext.Identifier;
        return false;
      }

      public override int GetHashCode()
      {
        return this.referenceAssemblyContext.Identifier.GetHashCode();
      }

      public override string ToString()
      {
        return "ProjectXamlContext: " + (object) this.TargetFramework.ToString() + ", Mode: " + (string) (object) this.referenceAssemblyContext.ReferenceAssemblyMode + ", Universe: " + (string) (this.referenceAssemblyContext.ReferenceAssemblyMode != ReferenceAssemblyMode.None ? (object) this.referenceAssemblyContext.Identifier.ToString() : (object) "None");
      }
    }
  }
}
