// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Assets.AssemblyAssetAggregator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Tools.Assets
{
  internal class AssemblyAssetAggregator : AssetAggregator
  {
    private static bool useThreads = true;
    private Dictionary<IProjectContext, List<AssemblyAssetProvider>> cachedProjectProviders = new Dictionary<IProjectContext, List<AssemblyAssetProvider>>();
    private List<AssemblyAssetAggregator.ProviderWorker> workers = new List<AssemblyAssetAggregator.ProviderWorker>();
    private DesignerContext designerContext;
    private IProjectManager projectManager;
    private IViewService viewService;
    private IProjectContext activeProject;

    internal static bool UseThreads
    {
      get
      {
        return AssemblyAssetAggregator.useThreads;
      }
      set
      {
        AssemblyAssetAggregator.useThreads = value;
      }
    }

    public AssemblyAssetAggregator(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.projectManager = this.designerContext.ProjectManager;
      this.projectManager.ProjectClosed += new EventHandler<ProjectEventArgs>(this.OnProjectClosed);
      this.viewService = this.designerContext.ViewService;
      this.viewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      if (designerContext.ActiveDocument == null)
        return;
      this.activeProject = designerContext.ActiveProjectContext;
      this.OnProjectActivating();
    }

    protected override void InternalDispose(bool disposing)
    {
      base.InternalDispose(disposing);
      if (!disposing)
        return;
      this.projectManager.ProjectClosed -= new EventHandler<ProjectEventArgs>(this.OnProjectClosed);
      this.viewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.workers.ForEach((Action<AssemblyAssetAggregator.ProviderWorker>) (worker => worker.Cancel()));
      this.workers.Clear();
      this.workers = (List<AssemblyAssetAggregator.ProviderWorker>) null;
    }

    private void SwitchProjects(IProjectContext newProject)
    {
      if (this.activeProject == newProject)
        return;
      this.OnProjectDeactivating();
      this.activeProject = newProject;
      this.OnProjectActivating();
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      if (this.designerContext.ActiveDocument == null)
        return;
      this.SwitchProjects(this.designerContext.ActiveProjectContext);
    }

    private void OnProjectActivating()
    {
      if (this.activeProject == null)
        return;
      this.RegisterAssemblies();
      INotifyCollectionChanged collectionChanged = this.activeProject.AssemblyReferences as INotifyCollectionChanged;
      if (collectionChanged == null)
        return;
      collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ProjectReferences_CollectionChanged);
    }

    private void OnProjectDeactivating()
    {
      if (this.activeProject == null)
        return;
      INotifyCollectionChanged collectionChanged = this.activeProject.AssemblyReferences as INotifyCollectionChanged;
      if (collectionChanged != null)
        collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ProjectReferences_CollectionChanged);
      this.UnregisterAssemblies();
      this.activeProject = (IProjectContext) null;
    }

    private void RegisterAssemblies()
    {
      if (this.activeProject == null)
        return;
      List<AssemblyAssetProvider> list1;
      if (this.cachedProjectProviders.TryGetValue(this.activeProject, out list1))
      {
        foreach (AssetProvider provider in list1)
          this.AddProvider(provider);
      }
      else
      {
        list1 = new List<AssemblyAssetProvider>();
        foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.activeProject.AssemblyReferences)
        {
          string name = assembly.Name;
          if (!assembly.IsResolvedImplicitAssembly && string.Compare(name, "System", StringComparison.OrdinalIgnoreCase) != 0 && (string.Compare(name, "System.Core", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(name, "mscorlib", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(name, "System.Data", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(name, "System.Xml", StringComparison.OrdinalIgnoreCase) != 0 && (string.Compare(name, "WindowsBase", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(name, "System.Xml.Linq", StringComparison.OrdinalIgnoreCase) != 0)) && (string.Compare(name, "System.Xaml", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(name, "System.Data.DataSetExtensions", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(name, "System.Activities", StringComparison.OrdinalIgnoreCase) != 0))
          {
            bool isMainAssembly = assembly.Equals((object) this.activeProject.ProjectAssembly);
            AssemblyAssetProvider assemblyAssetProvider = new AssemblyAssetProvider(this.activeProject, assembly, isMainAssembly);
            this.AddProvider((AssetProvider) assemblyAssetProvider);
            list1.Add(assemblyAssetProvider);
          }
        }
        this.cachedProjectProviders.Add(this.activeProject, list1);
      }
      Tuple<List<AssemblyAssetProvider>, AssemblyAssetAggregator.ProviderCache> tuple = this.BuildGlobalProviders();
      List<AssemblyAssetProvider> list2 = tuple.Item1;
      AssemblyAssetAggregator.ProviderCache providerCache = tuple.Item2;
      if (!AssemblyAssetAggregator.UseThreads || !EnumerableExtensions.CountIsMoreThan<AssemblyAssetProvider>((IEnumerable<AssemblyAssetProvider>) list2, 0))
        return;
      AssemblyAssetAggregator.ProviderWorker providerWorker = new AssemblyAssetAggregator.ProviderWorker(providerCache);
      providerWorker.DoWork += new DoWorkEventHandler(this.OnWorkerDoWork);
      providerWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
      this.workers.Add(providerWorker);
      providerWorker.RunWorkerAsync((object) list2);
    }

    private void ProviderCache_Disposing(object sender, EventArgs e)
    {
      AssemblyAssetAggregator.ProviderCache providerCache = (AssemblyAssetAggregator.ProviderCache) sender;
      providerCache.Disposing -= new EventHandler(this.ProviderCache_Disposing);
      AssemblyAssetAggregator.ProviderWorker providerWorker = Enumerable.FirstOrDefault<AssemblyAssetAggregator.ProviderWorker>((IEnumerable<AssemblyAssetAggregator.ProviderWorker>) this.workers, (Func<AssemblyAssetAggregator.ProviderWorker, bool>) (w => w.ProviderCache == providerCache));
      if (providerWorker == null)
        return;
      providerWorker.Cancel();
      this.workers.Remove(providerWorker);
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      AssemblyAssetAggregator.ProviderWorker providerWorker = (AssemblyAssetAggregator.ProviderWorker) sender;
      providerWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
      if (this.workers == null)
        return;
      this.workers.Remove(providerWorker);
    }

    private void OnWorkerDoWork(object sender, DoWorkEventArgs e)
    {
      AssemblyAssetAggregator.ProviderWorker providerWorker = sender as AssemblyAssetAggregator.ProviderWorker;
      providerWorker.DoWork -= new DoWorkEventHandler(this.OnWorkerDoWork);
      List<AssemblyAssetProvider> list = (List<AssemblyAssetProvider>) e.Argument;
      if (Enumerable.FirstOrDefault<AssemblyAssetProvider>((IEnumerable<AssemblyAssetProvider>) list) == null)
        return;
      AppDomain domain = AppDomain.CreateDomain("AssetToolDomain", (Evidence) null, new AppDomainSetup()
      {
        DisallowBindingRedirects = true
      });
      try
      {
        using (ProjectPathHelper.TemporaryDirectory temporaryDirectory = new ProjectPathHelper.TemporaryDirectory())
        {
          using (List<AssemblyAssetProvider>.Enumerator enumerator = list.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              AssemblyAssetProvider provider = enumerator.Current;
              if (providerWorker.CancellationPending)
                break;
              if (!provider.AsynchronizedInitialize(domain, temporaryDirectory.Path, (IBackgroundWorkerWrapper) providerWorker))
                providerWorker.InvokeUIThread(DispatcherPriority.Background, (Action) (() => this.RemoveProvider((AssetProvider) provider)));
            }
          }
        }
      }
      finally
      {
        AppDomain.Unload(domain);
      }
    }

    public static IEnumerable<string> GetDesignFileNames(string filename, bool includeVisualStudio)
    {
      List<string> list1 = new List<string>(3);
      list1.Add(".design");
      list1.Add(".expression.design");
      if (includeVisualStudio)
        list1.Add(".visualstudio.design");
      List<string> list2 = new List<string>(list1.Count * 2);
      string directoryName = Path.GetDirectoryName(filename);
      string sourceDirectory = Path.Combine(directoryName, "Design");
      foreach (string metadataSuffix in list1)
      {
        string metadataAssembly1 = KnownProjectBase.FindBestMetadataAssembly(directoryName, filename, metadataSuffix);
        if (metadataAssembly1 != null)
          list2.Add(metadataAssembly1);
        string metadataAssembly2 = KnownProjectBase.FindBestMetadataAssembly(sourceDirectory, filename, metadataSuffix);
        if (metadataAssembly2 != null)
          list2.Add(metadataAssembly2);
      }
      return (IEnumerable<string>) list2;
    }

    private Tuple<List<AssemblyAssetProvider>, AssemblyAssetAggregator.ProviderCache> BuildGlobalProviders()
    {
      HashSet<string> hashSet1 = new HashSet<string>();
      foreach (IAssembly assembly in (IEnumerable<IAssembly>) this.activeProject.AssemblyReferences)
      {
        if (assembly.IsLoaded && !assembly.IsResolvedImplicitAssembly)
          hashSet1.Add(assembly.Name);
      }
      AssemblyAssetAggregator.ProviderCache providerCache = (AssemblyAssetAggregator.ProviderCache) this.activeProject.Platform.Metadata.GetPlatformCache(DesignSurfacePlatformCaches.AssemblyAssetProviders);
      List<AssemblyAssetProvider> list = new List<AssemblyAssetProvider>();
      if (providerCache != null)
      {
        foreach (AssemblyAssetProvider assemblyAssetProvider in (List<AssemblyAssetProvider>) providerCache)
        {
          if (!hashSet1.Contains(assemblyAssetProvider.AssemblyName))
          {
            this.AddProvider((AssetProvider) assemblyAssetProvider);
            EnumerableExtensions.ForEach<TypeAsset>(Enumerable.OfType<TypeAsset>((IEnumerable) assemblyAssetProvider.Assets), (Action<TypeAsset>) (asset => asset.OnProjectChanged()));
          }
        }
      }
      else
      {
        providerCache = new AssemblyAssetAggregator.ProviderCache();
        providerCache.Disposing += new EventHandler(this.ProviderCache_Disposing);
        foreach (string path in new HashSet<string>(BlendSdkHelper.GetExtensionDirectories(this.activeProject.TargetFramework)))
        {
          string[] files = Directory.GetFiles(path, "*.dll");
          HashSet<string> hashSet2 = new HashSet<string>();
          foreach (string filename in files)
          {
            foreach (string str in AssemblyAssetAggregator.GetDesignFileNames(filename, true))
              hashSet2.Add(str);
          }
          foreach (string filename in files)
          {
            if (!hashSet2.Contains(filename.ToUpperInvariant()))
            {
              IAssemblyLoggingService assemblyLoggingService = (IAssemblyLoggingService) null;
              if (this.designerContext.Services != null)
                assemblyLoggingService = this.designerContext.Services.GetService<IAssemblyLoggingService>();
              AssemblyAssetProvider assemblyAssetProvider = new AssemblyAssetProvider(this.activeProject, assemblyLoggingService, filename);
              providerCache.Add(assemblyAssetProvider);
              list.Add(assemblyAssetProvider);
              this.AddProvider((AssetProvider) assemblyAssetProvider);
            }
          }
        }
        if (!AssetLibrary.DisableAsyncUpdateCache)
          this.activeProject.Platform.Metadata.SetPlatformCache(DesignSurfacePlatformCaches.AssemblyAssetProviders, (object) providerCache);
      }
      return new Tuple<List<AssemblyAssetProvider>, AssemblyAssetAggregator.ProviderCache>(list, providerCache);
    }

    private void UnregisterAssemblies()
    {
      this.ClearProviders();
    }

    private void ProjectReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (this.activeProject == null)
        return;
      this.cachedProjectProviders.Clear();
      this.UnregisterAssemblies();
      this.RegisterAssemblies();
    }

    private void OnProjectClosed(object sender, ProjectEventArgs e)
    {
      IXamlProject xamlProject = e.Project as IXamlProject;
      if (xamlProject == null || xamlProject.ProjectContext == null || this.activeProject != xamlProject.ProjectContext)
        return;
      this.OnProjectDeactivating();
      this.cachedProjectProviders.Remove(xamlProject.ProjectContext);
    }

    private class ProviderWorker : BackgroundWorker, IBackgroundWorkerWrapper
    {
      private object uiThreadDispatcherLock = new object();
      private UIThreadDispatcher uiThreadDispatcher;

      private UIThreadDispatcher UIThreadDispatcher
      {
        get
        {
          lock (this.uiThreadDispatcherLock)
            return this.uiThreadDispatcher;
        }
        set
        {
          lock (this.uiThreadDispatcherLock)
            this.uiThreadDispatcher = value;
        }
      }

      public AssemblyAssetAggregator.ProviderCache ProviderCache { get; private set; }

      public ProviderWorker(AssemblyAssetAggregator.ProviderCache providerCache)
      {
        this.ProviderCache = providerCache;
        this.WorkerSupportsCancellation = true;
        this.UIThreadDispatcher = UIThreadDispatcher.Instance;
      }

      public void Cancel()
      {
        this.CancelAsync();
        this.UIThreadDispatcher = (UIThreadDispatcher) null;
        this.ProviderCache = (AssemblyAssetAggregator.ProviderCache) null;
      }

      public void InvokeUIThread(DispatcherPriority priority, Action action)
      {
        if (this.CancellationPending)
          return;
        UIThreadDispatcher dispatcher = this.UIThreadDispatcher;
        if (dispatcher == null)
          return;
        dispatcher.Invoke(priority, (Action) (() =>
        {
          dispatcher = this.UIThreadDispatcher;
          if (dispatcher == null || this.CancellationPending)
            return;
          action();
        }));
      }

      [SpecialName]
      bool Microsoft.Expression.DesignSurface.Tools.Assets.IBackgroundWorkerWrapper.CancellationPending
      {
          get { return base.CancellationPending; }
      }
    }

    private class ProviderCache : List<AssemblyAssetProvider>, IDisposable
    {
      public event EventHandler Disposing;

      public void Dispose()
      {
        if (this.Disposing != null)
          this.Disposing((object) this, EventArgs.Empty);
        foreach (AssetProvider assetProvider in (List<AssemblyAssetProvider>) this)
          assetProvider.Dispose();
        this.Clear();
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
