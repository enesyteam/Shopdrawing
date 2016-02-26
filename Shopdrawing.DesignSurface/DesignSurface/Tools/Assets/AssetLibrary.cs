// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssetLibrary
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal sealed class AssetLibrary : IAssetLibrary, IDisposable
  {
    private bool needsUpdate = true;
    private Dictionary<AssetLibrary.AssetProviderKey, AssetProvider> AssetProviders = new Dictionary<AssetLibrary.AssetProviderKey, AssetProvider>();
    private const string ActiveUserThemeUnsetValue = "<Unset>";
    private const string ActiveUserThemeProviderNameProjectSetting = "ActiveUserThemeProviderNameProjectSetting";
    private bool initialized;
    private int openCount;
    private IProject targetProject;
    private bool activeUserThemeProviderCached;
    private IUserThemeProvider activeUserThemeProvider;
    private IProjectManager projectManager;
    private IViewService viewService;
    private IPrototypingService prototypingService;
    private SolutionSettingsManager solutionSettingsManager;
    private static bool performanceMarkerStarted;

    private ProjectAssetProvider ProjectAssetProvider
    {
      get
      {
        return (ProjectAssetProvider) this.AssetProviders[AssetLibrary.AssetProviderKey.ProjectAssetProvider];
      }
    }

    private AssetAggregator ProjectResourceAssetAggregator
    {
      get
      {
        return (AssetAggregator) this.AssetProviders[AssetLibrary.AssetProviderKey.ProjectResourceAssetAggregator];
      }
    }

    internal static bool DisableAsyncUpdate { get; set; }

    internal static bool DisableAsyncUpdateCache { get; set; }

    public DesignerContext DesignerContext { get; private set; }

    public IEnumerable<Asset> Assets
    {
      get
      {
        foreach (AssetProvider assetProvider in this.AssetProviders.Values)
        {
          foreach (Asset asset in (Collection<Asset>) assetProvider.Assets)
            yield return asset;
        }
      }
    }

    public IUserThemeProvider ActiveUserThemeProvider
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        if (!this.activeUserThemeProviderCached && this.TargetProject != null)
        {
          string name = this.ActiveUserThemeProviderName;
          if (name == "<Unset>")
            name = (string) null;
          this.activeUserThemeProvider = this.FindUserThemeProvider(name);
          this.activeUserThemeProviderCached = this.activeUserThemeProvider != null || Enumerable.All<IUserThemeProvider>(this.FindAssetProviders<IUserThemeProvider>(), (Func<IUserThemeProvider, bool>) (provider => !((AssetProvider) provider).NeedsUpdate));
        }
        if (!this.activeUserThemeProviderCached)
          return (IUserThemeProvider) null;
        return this.activeUserThemeProvider;
      }
      set
      {
        if (this.activeUserThemeProvider == value)
          return;
        this.activeUserThemeProvider = value;
        this.ActiveUserThemeProviderName = AssetLibrary.GetUserThemeName(value);
        this.activeUserThemeProviderCached = true;
        if (this.ActiveUserThemeProviderChanged == null)
          return;
        this.ActiveUserThemeProviderChanged();
      }
    }

    private string ActiveUserThemeProviderName
    {
      get
      {
        string str = (string) null;
        if (this.SolutionSettingsManager != null && this.DesignerContext.ActiveProject != null)
          str = (string) this.SolutionSettingsManager.GetProjectProperty((INamedProject) this.DesignerContext.ActiveProject, "ActiveUserThemeProviderNameProjectSetting");
        return str ?? this.GetDefaultStyleName();
      }
      set
      {
        if (this.SolutionSettingsManager == null || this.DesignerContext.ActiveProject == null)
          return;
        this.SolutionSettingsManager.SetProjectProperty((INamedProject) this.DesignerContext.ActiveProject, "ActiveUserThemeProviderNameProjectSetting", (object) (value ?? "<Unset>"));
      }
    }

    private SolutionSettingsManager SolutionSettingsManager
    {
      get
      {
        if (this.solutionSettingsManager == null)
        {
          IProjectManager service = this.DesignerContext.Services.GetService<IProjectManager>();
          if (service != null)
          {
            ISolution currentSolution = service.CurrentSolution;
            if (currentSolution != null)
              this.solutionSettingsManager = currentSolution.SolutionSettingsManager;
          }
        }
        return this.solutionSettingsManager;
      }
    }

    public bool NeedsUpdate
    {
      get
      {
        return this.needsUpdate;
      }
      private set
      {
        if (this.needsUpdate == value)
          return;
        this.needsUpdate = value;
        if (this.NeedsUpdateChanged == null)
          return;
        this.NeedsUpdateChanged();
      }
    }

    public bool IsOpened
    {
      get
      {
        return this.openCount > 0;
      }
    }

    private IProject TargetProject
    {
      get
      {
        return this.targetProject;
      }
      set
      {
        if (this.targetProject != null)
        {
          this.targetProject.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
          this.targetProject.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
        }
        this.targetProject = value;
        if (this.targetProject != null)
        {
          this.targetProject.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
          this.targetProject.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
        }
        this.RefreshProjectItems();
        this.NotifyAssetLibraryChanged(AssetLibraryDamages.Categories | AssetLibraryDamages.ProjectChanged);
      }
    }

    public event Action<AssetLibraryDamages> AssetLibraryChanged;

    public event Action ActiveUserThemeProviderChanged;

    public event Action NeedsUpdateChanged;

    public AssetLibrary(DesignerContext DesignerContext)
    {
      this.DesignerContext = DesignerContext;
    }

    public void OnOpen()
    {
      ++this.openCount;
      this.Initialize();
      this.InvokeAsync(new Action(this.UpdateAssetsWorker));
    }

    public void OnClose()
    {
      --this.openCount;
    }

    public IEnumerable<T> FindAssetProviders<T>() where T : class
    {
      if (!this.initialized)
        this.Initialize();
      Queue<AssetProvider> queue = new Queue<AssetProvider>((IEnumerable<AssetProvider>) this.AssetProviders.Values);
      while (queue.Count > 0)
      {
        AssetProvider provider = queue.Dequeue();
        AssetAggregator aggregator = provider as AssetAggregator;
        if (aggregator != null)
          EnumerableExtensions.ForEach<AssetProvider>((IEnumerable<AssetProvider>) aggregator.AssetProviders, (Action<AssetProvider>) (item => queue.Enqueue(item)));
        T result = provider as T;
        if ((object) result != null)
        {
          if (provider.NeedsUpdate)
            provider.Update();
          yield return result;
        }
      }
    }

    public Asset FindAsset(Asset asset)
    {
      foreach (AssetProvider assetProvider in this.AssetProviders.Values)
      {
        if (assetProvider.NeedsUpdate)
          assetProvider.Update();
        int index = assetProvider.Assets.BinarySearch(asset, Asset.DefaultComparer);
        if (index >= 0)
          return assetProvider.Assets[index];
      }
      return (Asset) null;
    }

    public IUserThemeProvider FindUserThemeProvider(string name)
    {
      if (!string.IsNullOrEmpty(name))
        return Enumerable.FirstOrDefault<IUserThemeProvider>(this.FindAssetProviders<IUserThemeProvider>(), (Func<IUserThemeProvider, bool>) (provider =>
        {
          if (name.Equals(provider.ThemeName))
            return provider.CanInsert(this.TargetProject);
          return false;
        }));
      return (IUserThemeProvider) null;
    }

    internal static string GetUserThemeName(IUserThemeProvider themeProvider)
    {
      if (themeProvider != null)
        return themeProvider.ThemeName;
      return (string) null;
    }

    private void Initialize()
    {
      if (this.initialized)
        return;
      this.initialized = true;
      this.AddAssetProvider(AssetLibrary.AssetProviderKey.AssemblyAssetAggregator, (AssetProvider) new AssemblyAssetAggregator(this.DesignerContext));
      this.AddAssetProvider(AssetLibrary.AssetProviderKey.ResourceAssetAggregator, (AssetProvider) new ResourceAssetAggregator(this.DesignerContext));
      this.AddAssetProvider(AssetLibrary.AssetProviderKey.UserThemeAssetAggregator, (AssetProvider) new UserThemeAssetAggregator(this.DesignerContext));
      this.AddAssetProvider(AssetLibrary.AssetProviderKey.ProjectResourceAssetAggregator, (AssetProvider) new AssetAggregator());
      this.AddAssetProvider(AssetLibrary.AssetProviderKey.ProjectAssetProvider, (AssetProvider) new ProjectAssetProvider(this.DesignerContext));
      this.TargetProject = this.DesignerContext.ActiveProject;
      this.projectManager = this.DesignerContext.ProjectManager;
      this.viewService = this.DesignerContext.ViewService;
      this.prototypingService = this.DesignerContext.PrototypingService;
      this.projectManager.SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      this.projectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
      this.viewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      if (this.prototypingService == null)
        return;
      this.prototypingService.ScreenTypeChanged += new EventHandler<ScreenTypeChangedEventArgs>(this.PrototypingService_ScreenTypeChanged);
    }

    public void Dispose()
    {
      if (this.initialized)
      {
        foreach (AssetProvider assetProvider in this.AssetProviders.Values)
        {
          assetProvider.NeedsUpdateChanged -= new EventHandler(this.ProviderNeedsUpdateChanged);
          assetProvider.AssetsChanged -= new EventHandler(this.ProviderAssetsChanged);
          assetProvider.Dispose();
        }
        this.AssetProviders.Clear();
        this.projectManager.SolutionOpened -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
        this.projectManager.SolutionClosed -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
        this.viewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
        if (this.prototypingService != null)
          this.prototypingService.ScreenTypeChanged -= new EventHandler<ScreenTypeChangedEventArgs>(this.PrototypingService_ScreenTypeChanged);
        this.projectManager = (IProjectManager) null;
        this.viewService = (IViewService) null;
        this.prototypingService = (IPrototypingService) null;
        this.DesignerContext = (DesignerContext) null;
      }
      GC.SuppressFinalize((object) this);
    }

    private void AddAssetProvider(AssetLibrary.AssetProviderKey key, AssetProvider provider)
    {
      this.AssetProviders[key] = provider;
      provider.AssetsChanged += new EventHandler(this.ProviderAssetsChanged);
      provider.NeedsUpdateChanged += new EventHandler(this.ProviderNeedsUpdateChanged);
    }

    private void ProviderAssetsChanged(object sender, EventArgs e)
    {
      this.InvokeAsync(new Action(this.UpdateAssetsWorker));
      this.NotifyAssetLibraryChanged(AssetLibraryDamages.Assets);
    }

    private void ProviderNeedsUpdateChanged(object sender, EventArgs e)
    {
      if (!((AssetProvider) sender).NeedsUpdate)
        return;
      this.NeedsUpdate = true;
      this.InvokeAsync(new Action(this.UpdateAssetsWorker));
    }

    private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
    {
      this.DesignerContext.ProjectManager.ProjectOpened += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
      this.TargetProject = this.DesignerContext.ActiveProject;
    }

    private void ProjectManager_SolutionClosed(object sender, SolutionEventArgs e)
    {
      this.DesignerContext.ProjectManager.ProjectOpened -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
      this.TargetProject = this.DesignerContext.ActiveProject;
      this.solutionSettingsManager = (SolutionSettingsManager) null;
    }

    private void ProjectManager_ProjectOpened(object sender, ProjectEventArgs e)
    {
      this.TargetProject = this.DesignerContext.ActiveProject;
    }

    private void PrototypingService_ScreenTypeChanged(object sender, ScreenTypeChangedEventArgs e)
    {
      this.NotifyAssetLibraryChanged(AssetLibraryDamages.AssetCategories);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (this.DesignerContext.ActiveProject == this.TargetProject)
        return;
      this.TargetProject = this.DesignerContext.ActiveProject;
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      this.InvokeAsync((Action) (() => this.RefreshProjectItems()));
    }

    private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
    {
      this.InvokeAsync((Action) (() => this.TryAddItem(e.ProjectItem)));
    }

    private void InvokeAsync(Action action)
    {
      if (AssetLibrary.DisableAsyncUpdate)
        action();
      else
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Delegate) action);
    }

    private void UpdateAssetsWorker()
    {
      if (this.openCount > 0 && this.NeedsUpdate && this.TargetProject != null)
      {
        bool flag = false;
        foreach (AssetProvider assetProvider in Enumerable.Reverse<AssetProvider>(this.FindAssetProviders<AssetProvider>()))
        {
          if (assetProvider.NeedsUpdate)
          {
            using (PerformanceUtility.PerformanceSequence(PerformanceEvent.AssetLibraryUpdateAssets))
            {
              if (assetProvider.Update())
              {
                if (!AssetLibrary.DisableAsyncUpdate)
                {
                  this.NotifyAssetLibraryChanged(AssetLibraryDamages.Assets);
                  this.InvokeAsync(new Action(this.UpdateAssetsWorker));
                  return;
                }
              }
            }
          }
          flag |= assetProvider.NeedsUpdate;
        }
        this.NeedsUpdate = flag;
      }
      if (this.needsUpdate)
        return;
      AssetLibrary.ResetAssetLoadPerformanceMarker(false);
    }

    private void NotifyAssetLibraryChanged(AssetLibraryDamages damages)
    {
      if (this.AssetLibraryChanged == null)
        return;
      this.AssetLibraryChanged(damages);
    }

    private static void ResetAssetLoadPerformanceMarker(bool shouldStart)
    {
      if (AssetLibrary.performanceMarkerStarted)
      {
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.AssetLibraryUpdateEntireLibrary);
        AssetLibrary.performanceMarkerStarted = false;
      }
      if (!shouldStart)
        return;
      AssetLibrary.performanceMarkerStarted = true;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.AssetLibraryUpdateEntireLibrary);
    }

    private void RefreshProjectItems()
    {
      AssetLibrary.ResetAssetLoadPerformanceMarker(this.targetProject != null);
      this.ProjectResourceAssetAggregator.ClearProviders();
      if (this.targetProject != null && this.targetProject.Items != null)
        EnumerableExtensions.ForEach<IProjectItem>((IEnumerable<IProjectItem>) this.targetProject.Items, (Action<IProjectItem>) (projectItem => this.TryAddItem(projectItem)));
      this.activeUserThemeProviderCached = false;
      this.InvokeAsync(new Action(this.UpdateAssetsWorker));
      this.NotifyAssetLibraryChanged(AssetLibraryDamages.Categories | AssetLibraryDamages.Assets);
    }

    private void TryAddItem(IProjectItem projectItem)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(projectItem.Project);
      if (projectContext == null)
        return;
      IProjectDocument document1 = projectContext.GetDocument(DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
      if (document1 != null && document1.DocumentType == ProjectDocumentType.ResourceDictionary && this.FindProvider(projectItem) == null)
      {
        IDocument document2 = projectItem.Document;
        this.ProjectResourceAssetAggregator.AddProvider((AssetProvider) new ResourceDictionaryAssetProvider(this.DesignerContext.ResourceManager.GetContentProviderForResourceDictionary(projectItem)));
      }
      this.ProjectAssetProvider.NeedsUpdate = true;
    }

    private ResourceDictionaryAssetProvider FindProvider(IProjectItem projectItem)
    {
      return Enumerable.FirstOrDefault<ResourceDictionaryAssetProvider>(this.FindAssetProviders<ResourceDictionaryAssetProvider>(), (Func<ResourceDictionaryAssetProvider, bool>) (provider => provider.DoesProjectItemMatch(projectItem)));
    }

    private string GetDefaultStyleName()
    {
      string str = (string) null;
      if (this.DesignerContext != null && this.DesignerContext.ActiveProject != null && this.DesignerContext.ActiveProject.GetCapability<bool>("ExpressionBlendPrototypingEnabled"))
      {
        IUserThemeProvider userThemeProvider = (IUserThemeProvider) Enumerable.FirstOrDefault<ResourceDictionaryAssetProvider>(this.FindAssetProviders<ResourceDictionaryAssetProvider>(), (Func<ResourceDictionaryAssetProvider, bool>) (provider => provider.ResourceDictionaryUsage.HasFlag((Enum) ResourceDictionaryUsage.PrototypingDefaultStyles)));
        if (userThemeProvider != null)
          str = userThemeProvider.ThemeName;
      }
      return str;
    }

    public StyleAsset FindActiveUserThemeAsset(ITypeId type)
    {
      if (this.DesignerContext.ActiveProject != null && this.ActiveUserThemeProvider != null && this.ActiveUserThemeProvider.CanInsert(this.DesignerContext.ActiveProject))
      {
        IXamlProject xamlProject = this.DesignerContext.ActiveProject as IXamlProject;
        if (xamlProject == null || xamlProject.ProjectContext == null)
          return (StyleAsset) null;
        IType type1 = xamlProject.ProjectContext.ResolveType(type);
        if (type1 != null)
          return StyleAsset.Find((IEnumerable) this.ActiveUserThemeProvider.ThemeAssets, (ITypeId) type1);
      }
      return (StyleAsset) null;
    }

    public static bool ApplyActiveUserThemeStyle(SceneNode node)
    {
      IType type = node == null ? (IType) null : node.Type;
      if (type == null)
        return false;
      StyleAsset activeUserThemeAsset = node.DesignerContext.AssetLibrary.FindActiveUserThemeAsset((ITypeId) type);
      if (activeUserThemeAsset != null)
        return activeUserThemeAsset.ApplyStyle(node);
      return false;
    }

    private enum AssetProviderKey
    {
      AssemblyAssetAggregator,
      ResourceAssetAggregator,
      UserThemeAssetAggregator,
      ProjectResourceAssetAggregator,
      ProjectAssetProvider,
    }
  }
}
