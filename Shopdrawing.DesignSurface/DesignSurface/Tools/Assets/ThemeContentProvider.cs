// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.ThemeContentProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class ThemeContentProvider
  {
    private Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider> projectThemeCache;
    private DesignerContext designerContext;

    public ThemeContentProvider(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.projectThemeCache = new Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider>();
      this.designerContext.ProjectManager.ProjectClosed += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosed);
      this.designerContext.ProjectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
    }

    private static DocumentReference GetFileThemeReference(string rootDirectory, string currentTheme)
    {
      return DocumentReference.Create(Path.Combine(rootDirectory, currentTheme) + ".xaml");
    }

    public DocumentNode GetThemeResourceFromAssembly(IProjectContext projectContext, IAssembly themeRuntimeAssembly, IAssembly themeTargetAssembly, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      using (TemporaryCursor.SetWaitCursor())
      {
        foreach (string str1 in ThemeContentProvider.GetThemeNames(projectContext))
        {
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "themes/{0}.xaml", new object[1]
          {
            (object) str1.ToLowerInvariant()
          });
          IProjectContext projectContext1 = (IProjectContext) new ThemeContentProvider.TargetThemeAssemblyProjectContext(projectContext, themeRuntimeAssembly, this.designerContext.AssemblyService);
          DocumentReference theme = DocumentReference.Create(Path.Combine(themeTargetAssembly.Name, str2));
          DocumentNode resourceInternal = this.GetResourceInternal(this.projectThemeCache, projectContext.Platform.ThemeManager, projectContext1, theme, themeTargetAssembly, str2, resourceKey, out auxillaryResources);
          if (resourceInternal != null)
            return resourceInternal;
        }
        if (projectContext.Platform.ThemeManager.AllowFallbackToPlatform)
          return this.GetThemeResourceFromPlatform(projectContext.Platform, resourceKey, out auxillaryResources);
        auxillaryResources = (IList<DocumentCompositeNode>) null;
        return (DocumentNode) null;
      }
    }

    public DocumentNode GetThemeResourceFromProject(IProject project, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(project);
      if (projectContext == null)
        return (DocumentNode) null;
      string rootDirectory = Path.Combine(project.ProjectRoot.Path, "Themes");
      foreach (string currentTheme in ThemeContentProvider.GetThemeNames(projectContext))
      {
        DocumentReference fileThemeReference = ThemeContentProvider.GetFileThemeReference(rootDirectory, currentTheme);
        IProjectItem projectItem = project.FindItem(fileThemeReference);
        if (projectItem != null)
        {
          ResourceDictionaryContentProvider resourceDictionary = this.designerContext.ResourceManager.GetContentProviderForResourceDictionary(projectItem);
          if (resourceDictionary != null)
          {
            DocumentNode projectResource = this.FindProjectResource(project, resourceDictionary, projectItem.DocumentReference.Path, resourceKey, out auxillaryResources);
            if (projectResource != null)
              return projectResource;
          }
        }
      }
      return (DocumentNode) null;
    }

    public DocumentNode GetThemeResourceFromPlatform(IPlatform platform, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      FolderBasedThemeManager basedThemeManager = platform.ThemeManager as FolderBasedThemeManager;
      if (basedThemeManager == null)
        return (DocumentNode) null;
      using (TemporaryCursor.SetWaitCursor())
      {
        Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider> orCreateCache = DesignSurfacePlatformCaches.GetOrCreateCache<Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider>>(platform.Metadata, DesignSurfacePlatformCaches.ThemeContentProviderCache);
        DocumentReference fileThemeReference = ThemeContentProvider.GetFileThemeReference(basedThemeManager.ThemeFolder, basedThemeManager.CurrentTheme);
        return this.GetResourceInternal(orCreateCache, (ThemeManager) basedThemeManager, (IProjectContext) null, fileThemeReference, (IAssembly) null, (string) null, resourceKey, out auxillaryResources);
      }
    }

    private static string[] GetThemeNames(IProjectContext projectContext)
    {
      string currentTheme = projectContext.Platform.ThemeManager.CurrentTheme;
      string strB = "generic";
      string[] strArray;
      if (string.Compare(currentTheme, strB, StringComparison.OrdinalIgnoreCase) != 0)
        strArray = new string[2]
        {
          currentTheme,
          strB
        };
      else
        strArray = new string[1]
        {
          strB
        };
      return strArray;
    }

    private DocumentNode GetResourceInternal(Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider> themeCache, ThemeManager manager, IProjectContext projectContext, DocumentReference theme, IAssembly themeAssembly, string themeAssemblyPath, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      ThemeContentProvider.SystemThemeContentProvider cachedThemeContent = this.GetCachedThemeContent(themeCache, manager, projectContext, theme, themeAssembly, themeAssemblyPath, this.designerContext.TextBufferService);
      DocumentNode documentNode = (DocumentNode) null;
      List<string> resolvedUris = new List<string>();
      if (cachedThemeContent != null)
        documentNode = ThemeContentProvider.FindResource((ResourceDictionaryContentProvider) cachedThemeContent, themeAssemblyPath, (Func<ResourceDictionaryContentProvider, Func<DocumentNode, object>>) (i => new Func<DocumentNode, object>(((ThemeContentProvider.SystemThemeContentProvider) i).CreateInstance)), (Func<Uri, string, string>) ((relativeUri, sourcePath) =>
        {
          if (themeAssembly != null && relativeUri != (Uri) null)
          {
            string str1 = "pack://application:,,,/" + themeAssembly.Name + ";component/";
            Uri uri1 = new Uri(str1);
            Uri uri2 = KnownProjectBase.MakeDesignTimeUri(relativeUri, str1, sourcePath, themeAssembly.Name, (IEnumerable<IProject>) null);
            if (uri2 != (Uri) null && uri2.IsAbsoluteUri)
            {
              Uri uri3 = uri1.MakeRelativeUri(uri2);
              if (uri3 != (Uri) null)
              {
                string str2 = uri3.OriginalString.ToLowerInvariant();
                if (resolvedUris.Contains(str2))
                  return (string) null;
                resolvedUris.Add(str2);
                return str2;
              }
            }
          }
          return (string) null;
        }), (Func<string, ResourceDictionaryContentProvider>) (resolvedUri =>
        {
          if (themeAssembly != null && !string.IsNullOrEmpty(resolvedUri))
            return (ResourceDictionaryContentProvider) this.GetCachedThemeContent(themeCache, manager, projectContext, DocumentReference.Create(Path.Combine(themeAssembly.Name, resolvedUri)), themeAssembly, resolvedUri, this.designerContext.TextBufferService);
          return (ResourceDictionaryContentProvider) null;
        }), resourceKey, out auxillaryResources);
      return documentNode;
    }

    private DocumentNode FindProjectResource(IProject project, ResourceDictionaryContentProvider currentThemeProvider, string currentThemePath, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      List<string> resolvedUris = new List<string>();
      return ThemeContentProvider.FindResource(currentThemeProvider, currentThemePath, (Func<ResourceDictionaryContentProvider, Func<DocumentNode, object>>) (provider => (Func<DocumentNode, object>) (resourceKeyNode =>
      {
        DocumentNode rootNode = currentThemeProvider.Document.RootNode;
        using (StandaloneInstanceBuilderContext instanceBuilderContext = new StandaloneInstanceBuilderContext(rootNode.Context, this.designerContext))
        {
          using (instanceBuilderContext.DisablePostponedResourceEvaluation())
          {
            instanceBuilderContext.ViewNodeManager.RootNodePath = new DocumentNodePath(rootNode, resourceKeyNode);
            instanceBuilderContext.ViewNodeManager.Instantiate(instanceBuilderContext.ViewNodeManager.Root);
            return instanceBuilderContext.ViewNodeManager.ValidRootInstance;
          }
        }
      })), (Func<Uri, string, string>) ((relativeUri, sourcePath) =>
      {
        Uri uri = project.MakeDesignTimeUri(relativeUri, sourcePath);
        if (!(uri != (Uri) null))
          return (string) null;
        string originalString = uri.OriginalString;
        if (resolvedUris.Contains(originalString))
          return (string) null;
        resolvedUris.Add(originalString);
        return originalString;
      }), (Func<string, ResourceDictionaryContentProvider>) (resolvedUri =>
      {
        if (!string.IsNullOrEmpty(resolvedUri))
        {
          IProjectItem projectItem = project.FindItem(DocumentReference.Create(resolvedUri));
          if (projectItem != null)
            return this.designerContext.ResourceManager.GetContentProviderForResourceDictionary(projectItem);
        }
        return (ResourceDictionaryContentProvider) null;
      }), resourceKey, out auxillaryResources);
    }

    private static DocumentNode FindResource(ResourceDictionaryContentProvider themeContent, string originalThemePath, Func<ResourceDictionaryContentProvider, Func<DocumentNode, object>> provideInstanceForThemeContent, Func<Uri, string, string> provideResolvedUriForMergedDictionary, Func<string, ResourceDictionaryContentProvider> provideContentForMergedDictionary, object resourceKey, out IList<DocumentCompositeNode> auxillaryResources)
    {
      DocumentNode root = (DocumentNode) null;
      auxillaryResources = (IList<DocumentCompositeNode>) null;
      Func<DocumentNode, object> func = provideInstanceForThemeContent(themeContent);
      foreach (DocumentNode documentNode1 in themeContent.Items)
      {
        DocumentCompositeNode entryNode = documentNode1 as DocumentCompositeNode;
        if (entryNode != null)
        {
          DocumentNode documentNode2 = ResourceNodeHelper.GetResourceEntryKey(entryNode);
          if (documentNode2 == null)
          {
            DocumentCompositeNode documentCompositeNode = entryNode.Properties[DictionaryEntryNode.ValueProperty] as DocumentCompositeNode;
            if (documentCompositeNode != null)
            {
              if (documentCompositeNode.Type.Metadata.ImplicitDictionaryKeyProperty != null)
                documentNode2 = documentCompositeNode.Properties[documentCompositeNode.Type.Metadata.ImplicitDictionaryKeyProperty];
              if (documentNode2 == null && documentCompositeNode.TypeResolver.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
                documentNode2 = documentCompositeNode.Properties[(IPropertyId) documentCompositeNode.Type.Metadata.NameProperty];
            }
          }
          if (documentNode2 != null)
          {
            object objB = func(documentNode2);
            if (object.Equals(resourceKey, objB))
            {
              root = entryNode.Properties[DictionaryEntryNode.ValueProperty];
              break;
            }
          }
        }
      }
      if (root == null && originalThemePath != null)
      {
        foreach (DocumentNode documentNode in themeContent.Items)
        {
          DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
          if (documentCompositeNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentCompositeNode.Type))
          {
            Uri uriValue = documentCompositeNode.GetUriValue(ResourceDictionaryNode.SourceProperty);
            if (uriValue != (Uri) null)
            {
              string originalThemePath1 = provideResolvedUriForMergedDictionary(uriValue, originalThemePath);
              if (originalThemePath1 != null)
              {
                ResourceDictionaryContentProvider themeContent1 = provideContentForMergedDictionary(originalThemePath1);
                if (themeContent1 != null)
                {
                  root = ThemeContentProvider.FindResource(themeContent1, originalThemePath1, provideInstanceForThemeContent, provideResolvedUriForMergedDictionary, provideContentForMergedDictionary, resourceKey, out auxillaryResources);
                  if (root != null)
                    return root;
                }
              }
            }
          }
        }
      }
      if (root != null)
        auxillaryResources = Microsoft.Expression.DesignSurface.Utility.ResourceHelper.FindReferencedResources(root);
      return root;
    }

    internal static ITextBuffer LoadReference(DocumentReference documentReference, ITextBufferService textBufferService, out Encoding encoding)
    {
      encoding = (Encoding) null;
      string text;
      using (Stream stream = documentReference.GetStream(FileAccess.Read))
      {
        if (stream == null)
          return (ITextBuffer) null;
        try
        {
          text = DocumentReference.ReadDocumentContents(stream, out encoding);
        }
        catch (IOException ex)
        {
          return (ITextBuffer) null;
        }
      }
      ITextBuffer textBuffer = textBufferService.CreateTextBuffer();
      textBuffer.SetText(0, textBuffer.Length, text);
      return textBuffer;
    }

    private ThemeContentProvider.SystemThemeContentProvider GetCachedThemeContent(Dictionary<ThemeContentProvider.PlatformSpecificDocumentReference, ThemeContentProvider.SystemThemeContentProvider> themeCache, ThemeManager manager, IProjectContext projectContext, DocumentReference reference, IAssembly themeAssembly, string themeAssemblyPath, ITextBufferService textBufferService)
    {
      ThemeContentProvider.PlatformSpecificDocumentReference key = new ThemeContentProvider.PlatformSpecificDocumentReference(reference, projectContext != null ? projectContext.TargetFramework : (FrameworkName) null);
      ThemeContentProvider.SystemThemeContentProvider themeContentProvider;
      if (!themeCache.TryGetValue(key, out themeContentProvider))
      {
        Encoding encoding;
        ITextBuffer textBuffer = themeAssembly != null ? ThemeManager.LoadResource(themeAssembly, themeAssemblyPath, textBufferService, out encoding) : ThemeContentProvider.LoadReference(reference, textBufferService, out encoding);
        IDocumentLocator documentLocator = DocumentReferenceLocator.GetDocumentLocator(reference);
        IDocumentContext userContext = projectContext == null ? (IDocumentContext) null : (IDocumentContext) new DocumentContext(projectContext, documentLocator);
        XamlDocument theme = manager.GetTheme(documentLocator, themeAssembly != null, userContext, textBuffer, encoding);
        if (theme != null)
        {
          bool flag1 = false;
          try
          {
            if (projectContext != null)
            {
              if (!projectContext.IsCapabilitySet(PlatformCapability.IsWpf))
              {
                if (themeAssembly != null)
                {
                  bool flag2 = false;
                  foreach (IAssembly assembly in (projectContext.Platform.Metadata as PlatformTypes).DefaultAssemblyReferences)
                  {
                    if (assembly == themeAssembly)
                    {
                      flag2 = true;
                      break;
                    }
                  }
                  if (flag2)
                  {
                    flag1 = true;
                    AnimationEditor.ConvertFromToAnimations(theme.RootNode);
                  }
                }
              }
            }
          }
          catch
          {
            if (flag1)
              theme = manager.GetTheme(documentLocator, themeAssembly != null, userContext, textBuffer, encoding);
          }
          themeContentProvider = new ThemeContentProvider.SystemThemeContentProvider(this.designerContext, theme);
          themeCache[key] = themeContentProvider;
        }
      }
      return themeContentProvider;
    }

    private void ProjectManager_SolutionClosed(object sender, SolutionEventArgs e)
    {
      this.projectThemeCache.Clear();
    }

    private void ProjectManager_ProjectClosed(object sender, ProjectEventArgs e)
    {
      this.projectThemeCache.Clear();
    }

    private class SystemThemeContentProvider : ResourceDictionaryContentProvider
    {
      private IInstanceBuilderContext context;

      public SystemThemeContentProvider(DesignerContext designerContext, XamlDocument document)
        : base(designerContext, document)
      {
        this.context = (IInstanceBuilderContext) new StandaloneInstanceBuilderContext(document.DocumentContext, designerContext);
      }

      public object CreateInstance(DocumentNode targetNode)
      {
        using (this.context.DisablePostponedResourceEvaluation())
        {
          this.context.ViewNodeManager.RootNodePath = new DocumentNodePath(this.Document.RootNode, targetNode);
          this.context.ViewNodeManager.Instantiate(this.context.ViewNodeManager.Root);
        }
        return this.context.ViewNodeManager.ValidRootInstance;
      }
    }

    private struct PlatformSpecificDocumentReference
    {
      private DocumentReference documentReference;
      private FrameworkName frameworkName;

      public PlatformSpecificDocumentReference(DocumentReference documentReference, FrameworkName frameworkName)
      {
        this.documentReference = documentReference;
        this.frameworkName = frameworkName;
      }
    }

    private class TargetThemeAssemblyProjectContext : TypeResolver, IProjectContext, IDocumentRootResolver, ITypeResolver, IMetadataResolver, IServiceProvider
    {
      private IProjectContext sourceContext;
      private IAssembly targetAssembly;
      private IAssemblyService assemblyService;
      private List<IAssembly> referencedAssemblies;
      private XmlnsDefinitionMap namespaceMap;

      public IPlatform Platform
      {
        get
        {
          return this.sourceContext.Platform;
        }
      }

      public override IAssembly ProjectAssembly
      {
        get
        {
          return this.targetAssembly;
        }
      }

      public ICollection<IProjectDocument> Documents
      {
        get
        {
          return this.sourceContext.Documents;
        }
      }

      public IDocumentRoot ApplicationRoot
      {
        get
        {
          return this.sourceContext.ApplicationRoot;
        }
      }

      public IProjectDocument Application
      {
        get
        {
          return this.sourceContext.Application;
        }
      }

      public IProjectDocument LocalApplication
      {
        get
        {
          return this.sourceContext.LocalApplication;
        }
      }

      public string ProjectName
      {
        get
        {
          return this.sourceContext.ProjectName;
        }
      }

      public override string ProjectPath
      {
        get
        {
          return this.sourceContext.ProjectPath;
        }
      }

      public ObservableCollection<IProjectFont> ProjectFonts
      {
        get
        {
          return this.sourceContext.ProjectFonts;
        }
      }

      public IFontResolver FontResolver
      {
        get
        {
          return this.sourceContext.FontResolver;
        }
      }

      public override string RootNamespace
      {
        get
        {
          return this.sourceContext.RootNamespace;
        }
      }

      public FrameworkName TargetFramework
      {
        get
        {
          return this.sourceContext.TargetFramework;
        }
      }

      public override ICollection<IAssembly> AssemblyReferences
      {
        get
        {
          if (this.referencedAssemblies == null)
            this.InitializeReferences();
          return (ICollection<IAssembly>) this.referencedAssemblies;
        }
      }

      public override IXmlNamespaceTypeResolver ProjectNamespaces
      {
        get
        {
          if (this.namespaceMap == null)
            this.InitializeReferences();
          return (IXmlNamespaceTypeResolver) this.namespaceMap;
        }
      }

      event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentOpened
      {
        add
        {
        }
        remove
        {
        }
      }

      event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosing
      {
        add
        {
        }
        remove
        {
        }
      }

      event EventHandler<ProjectDocumentEventArgs> IProjectContext.DocumentClosed
      {
        add
        {
        }
        remove
        {
        }
      }

      event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChangedEarly
      {
        add
        {
        }
        remove
        {
        }
      }

      event EventHandler<TypesChangedEventArgs> ITypeResolver.TypesChanged
      {
        add
        {
        }
        remove
        {
        }
      }

      public TargetThemeAssemblyProjectContext(IProjectContext sourceContext, IAssembly targetAssembly, IAssemblyService assemblyService)
      {
        this.sourceContext = sourceContext;
        this.targetAssembly = targetAssembly;
        this.assemblyService = assemblyService;
        this.Initialize(sourceContext.Platform.Metadata);
      }

      private void InitializeReferences()
      {
        this.referencedAssemblies = new List<IAssembly>((IEnumerable<IAssembly>) this.sourceContext.AssemblyReferences);
        using (IEnumerator<IAssembly> enumerator = AssemblyHelper.LoadReferencedAssemblies(this.targetAssembly, new Func<AssemblyName, Assembly>(this.assemblyService.ResolveAssembly), this.Platform.Metadata).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            IAssembly loadedAssembly = enumerator.Current;
            if (this.referencedAssemblies.Find((Predicate<IAssembly>) (i => i.CompareTo(loadedAssembly))) == null)
              this.referencedAssemblies.Add(loadedAssembly);
          }
        }
        this.Platform.Metadata.RefreshAssemblies((ITypeResolver) this, Enumerable.Empty<Assembly>());
        this.namespaceMap = this.Platform.Metadata.CreateXmlnsDefinitionMap((ITypeResolver) this, (IEnumerable<IAssembly>) this.referencedAssemblies, this.targetAssembly);
      }

      public IAssembly GetDesignAssembly(IAssembly assembly)
      {
        return this.sourceContext.GetDesignAssembly(assembly);
      }

      public IProjectDocument GetDocument(IDocumentRoot documentRoot)
      {
        return this.sourceContext.GetDocument(documentRoot);
      }

      public IProjectDocument GetDocument(IDocumentLocator documentReference)
      {
        return this.sourceContext.GetDocument(documentReference);
      }

      public IProjectDocument OpenDocument(string path)
      {
        return this.sourceContext.OpenDocument(path);
      }

      public string MakeResourceReference(string resourceReference, IDocumentLocator referringDocument)
      {
        return this.sourceContext.MakeResourceReference(resourceReference, referringDocument);
      }

      public Uri MakeDesignTimeUri(Uri uri, string documentUrl)
      {
        return this.sourceContext.MakeDesignTimeUri(uri, documentUrl);
      }

      public IDocumentRoot GetDocumentRoot(string path)
      {
        return this.sourceContext.GetDocumentRoot(path);
      }

      public override bool IsCapabilitySet(PlatformCapability capability)
      {
        return this.sourceContext.IsCapabilitySet(capability);
      }

      public override object GetCapabilityValue(PlatformCapability capability)
      {
        return this.sourceContext.GetCapabilityValue(capability);
      }

      public object GetService(Type serviceType)
      {
        if (serviceType.IsAssignableFrom(this.GetType()))
          return (object) this;
        return this.sourceContext.GetService(serviceType);
      }

      public bool IsTypeSupported(ITypeId type)
      {
        return this.sourceContext.IsTypeSupported(type);
      }

      public bool IsTypeInSolution(IType type)
      {
        return this.sourceContext.IsTypeInSolution(type);
      }

      [SpecialName]
      ITypeMetadataFactory IProjectContext.MetadataFactory
      {
          get { return this.MetadataFactory; }
      }
    }
  }
}
