// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ResourceManager
  {
    private ObservableCollection<ResourceContainer> localResourceContainers = new ObservableCollection<ResourceContainer>();
    private ObservableCollection<DocumentResourceContainer> documentResourceContainers = new ObservableCollection<DocumentResourceContainer>();
    private ObservableCollection<DocumentResourceContainer> visibleDocumentResourceContainers = new ObservableCollection<DocumentResourceContainer>();
    private ObservableCollection<ResourceContainer> filteredResourceContainers = new ObservableCollection<ResourceContainer>();
    private NotifyingSelectionSet<ResourceEntryBase> selectedResources = new NotifyingSelectionSet<ResourceEntryBase>((System.Collections.Generic.Comparer<ResourceEntryBase>) new ResourceEntryBase.Comparer());
    private IDictionary<IProjectItem, ResourceDictionaryContentProvider> providers = (IDictionary<IProjectItem, ResourceDictionaryContentProvider>) new Dictionary<IProjectItem, ResourceDictionaryContentProvider>();
    private IDictionary<ResourceDictionaryContentProvider, bool> isUpdatingProvider = (IDictionary<ResourceDictionaryContentProvider, bool>) new Dictionary<ResourceDictionaryContentProvider, bool>();
    private uint viewStateChangeStamp = uint.MaxValue;
    private uint documentChangeStamp = uint.MaxValue;
    private DesignerContext designerContext;
    private BaseFrameworkElement primarySelection;
    private List<IProject> targetProjects;
    private Dictionary<DocumentNodeMarker, SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>>> propertyReferenceChangedHandlers;
    private Dictionary<IDocumentContext, DocumentNodeMarkerSortedList> watchedPropertyRoots;
    private Dictionary<IDocumentContext, SceneViewModel> watchedViewModels;
    private uint extraChangeStamp;
    private SceneNodeSubscription<object, object> basisSubscription;
    private SceneNodeSubscription<ResourceContainer, ResourceItem> resourcesSubscription;
    private bool isFiltering;
    private SelectedElementsResourceObserver filteredResources;
    private bool listenForProjectChanges;

    internal ResourcePane ResourcePane
    {
      get
      {
        return (ResourcePane) this.designerContext.WindowService.PaletteRegistry["Designer_ResourcePane"].Content;
      }
    }

    public ObservableCollection<ResourceContainer> LocalResourceContainers
    {
      get
      {
        return this.localResourceContainers;
      }
    }

    public ObservableCollection<DocumentResourceContainer> DocumentResourceContainers
    {
      get
      {
        return this.documentResourceContainers;
      }
    }

    public ObservableCollection<DocumentResourceContainer> VisibleDocumentResourceContainers
    {
      get
      {
        return this.visibleDocumentResourceContainers;
      }
    }

    public ObservableCollection<ResourceContainer> FilteredResourceContainers
    {
      get
      {
        return this.filteredResourceContainers;
      }
    }

    public ResourceContainer TopLevelResourceContainer
    {
      get
      {
        ResourceContainer activeRootContainer = this.ActiveRootContainer;
        SceneDocument applicationSceneDocument = this.ApplicationSceneDocument;
        if (applicationSceneDocument != null)
        {
          ResourceContainer resourceContainer = this.FindResourceContainer(applicationSceneDocument.DocumentContext.DocumentUrl);
          if (resourceContainer != null)
            return resourceContainer;
        }
        return activeRootContainer;
      }
    }

    public bool IsFiltering
    {
      get
      {
        return this.isFiltering;
      }
      set
      {
        if (this.isFiltering == value)
          return;
        this.SelectedItems.Clear();
        if (this.isFiltering)
        {
          this.UpdateFilteredResources((SceneViewModel) null, SceneViewModel.ViewStateBits.None);
          this.isFiltering = value;
        }
        else
        {
          this.isFiltering = value;
          this.UpdateFilteredResources(this.ActiveSceneViewModel, SceneViewModel.ViewStateBits.None);
        }
      }
    }

    public IEnumerable<ResourceContainer> ActiveResourceContainers
    {
      get
      {
        ResourceContainer rootContainer = this.ActiveRootContainer;
        if (rootContainer != null && !this.LocalResourceContainers.Contains(rootContainer))
          yield return rootContainer;
        foreach (ResourceContainer resourceContainer in (Collection<ResourceContainer>) this.LocalResourceContainers)
          yield return resourceContainer;
      }
    }

    public SceneNodeSubscription<ResourceContainer, ResourceItem> ResourcesSubscription
    {
      get
      {
        return this.resourcesSubscription;
      }
    }

    internal SelectionSet<ResourceEntryBase, OrderedList<ResourceEntryBase>> SelectedItems
    {
      get
      {
        return (SelectionSet<ResourceEntryBase, OrderedList<ResourceEntryBase>>) this.selectedResources;
      }
    }

    public IList<ResourceContainer> SelectedResourceContainers
    {
      get
      {
        return this.selectedResources.GetFilteredSelection<ResourceContainer>();
      }
    }

    public IList<ResourceItem> SelectedResourceItems
    {
      get
      {
        return this.selectedResources.GetFilteredSelection<ResourceItem>();
      }
    }

    public ResourceContainer ActiveRootContainer
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.RootNode != null)
        {
          foreach (ResourceContainer resourceContainer in (Collection<ResourceContainer>) this.localResourceContainers)
          {
            if (resourceContainer.DocumentNode != null && resourceContainer.DocumentNode == this.ActiveSceneViewModel.RootNode.DocumentNode)
              return resourceContainer;
          }
          foreach (ResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
          {
            if (resourceContainer.DocumentNode != null && resourceContainer.DocumentNode == this.ActiveSceneViewModel.RootNode.DocumentNode)
              return resourceContainer;
          }
        }
        return (ResourceContainer) null;
      }
    }

    public uint ResourceChangeStamp
    {
      get
      {
        return this.ResourcesSubscription.PathNodesChangeStamp + this.extraChangeStamp;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    private SceneDocument ApplicationSceneDocument
    {
      get
      {
        return SceneDocument.GetApplicationDocument(this.ActiveProjectContext);
      }
    }

    private IProjectContext ActiveProjectContext
    {
      get
      {
        SceneView activeView = this.designerContext.ActiveView;
        return activeView != null ? activeView.Document.ProjectContext : (IProjectContext) null;
      }
    }

    internal IEnumerable<IProject> TargetProjects
    {
      get
      {
        return (IEnumerable<IProject>) new ReadOnlyCollection<IProject>((IList<IProject>) this.targetProjects);
      }
      private set
      {
        this.ListenForProjectChanges = false;
        this.targetProjects.Clear();
        if (value != null)
        {
          foreach (IProject project in value)
            this.targetProjects.Add(project);
        }
        this.selectedResources.Clear();
        foreach (ResourceDictionaryContentProvider dictionaryContentProvider in (IEnumerable<ResourceDictionaryContentProvider>) this.providers.Values)
          dictionaryContentProvider.Close();
        this.providers.Clear();
        foreach (ResourceContainer container in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
          this.OnContainerRemoved(container);
        this.documentResourceContainers.Clear();
        this.ListenForProjectChanges = true;
        foreach (IProject project in this.targetProjects)
        {
          project.ItemOpened += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
          foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) project.Items)
            this.TryAddItem(projectItem);
        }
        this.RecalculateDocumentContainersEnabled();
        if (this.TargetProjectsChanged == null)
          return;
        this.TargetProjectsChanged((object) this, EventArgs.Empty);
      }
    }

    public bool ListenForProjectChanges
    {
      get
      {
        return this.listenForProjectChanges;
      }
      set
      {
        if (this.listenForProjectChanges == value)
          return;
        foreach (IProject project in this.targetProjects)
        {
          if (value)
          {
            project.ItemAdded += new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
            project.ItemRemoved += new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
            project.ItemChanged += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
            project.ItemClosed += new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
          }
          else
          {
            project.ItemAdded -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemAdded);
            project.ItemRemoved -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemRemoved);
            project.ItemChanged -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
            project.ItemClosed -= new EventHandler<ProjectItemEventArgs>(this.Project_ItemChanged);
          }
        }
        this.listenForProjectChanges = value;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        return this.designerContext.ActiveSceneViewModel;
      }
    }

    internal event EventHandler<EventArgs<ResourceContainer>> ContainerRemoved;

    internal event EventHandler<EventArgs<ResourceItem>> ItemRemoved;

    internal event EventHandler<EventArgs> TargetProjectsChanged;

    internal ResourceManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.SelectionManager.ActiveSceneSwitched += new EventHandler(this.SelectionManager_ActiveSceneSwitched);
      this.targetProjects = new List<IProject>();
      this.designerContext.ProjectManager.SolutionOpened += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      this.designerContext.ProjectManager.SolutionClosed += new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
      this.designerContext.ProjectManager.ProjectClosing += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      this.designerContext.ViewService.ViewOpened += new ViewEventHandler(this.ViewService_ViewOpened);
      this.designerContext.ViewService.ViewClosed += new ViewEventHandler(this.ViewService_ViewClosed);
      this.propertyReferenceChangedHandlers = new Dictionary<DocumentNodeMarker, SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>>>();
      this.watchedViewModels = new Dictionary<IDocumentContext, SceneViewModel>();
      this.watchedPropertyRoots = new Dictionary<IDocumentContext, DocumentNodeMarkerSortedList>();
      this.basisSubscription = new SceneNodeSubscription<object, object>();
      this.basisSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentSelfAndDescendant, (ISearchPredicate) new DelegatePredicate((Predicate<SceneNode>) (obj =>
        {
          if (obj.Type.Metadata.ResourcesProperty != null)
          {
            ResourceDictionaryNode resourceDictionaryNode = ResourceManager.ProvideResourcesForElement(obj);
            if (obj == this.ActiveSceneViewModel.RootNode || resourceDictionaryNode != null && (resourceDictionaryNode.Count > 0 || resourceDictionaryNode.MergedDictionaries.Count > 0))
              return true;
          }
          return false;
        }), SearchScope.NodeTreeDescendant))
      });
      this.resourcesSubscription = new SceneNodeSubscription<ResourceContainer, ResourceItem>();
      this.resourcesSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep((SearchAxis) new DelegateAxis(new DelegateAxis.EnumerationHandler(this.ResourceItemEnumerator), SearchScope.NodeTreeDescendant))
      });
      this.resourcesSubscription.SetBasisNodeInsertedHandler(new SceneNodeSubscription<ResourceContainer, ResourceItem>.BasisNodeInsertedHandler(this.ResourcesSubscription_ResourceContainerInsertedHandler));
      this.resourcesSubscription.BasisNodeRemoved += new SceneNodeSubscription<ResourceContainer, ResourceItem>.BasisNodeRemovedListener(this.ResourcesSubscription_ResourceContainerRemoved);
      this.resourcesSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<ResourceContainer, ResourceItem>.PathNodeInsertedHandler(this.ResourcesSubscription_ResourceItemInsertedHandler));
      this.resourcesSubscription.PathNodeRemoved += new SceneNodeSubscription<ResourceContainer, ResourceItem>.PathNodeRemovedListener(this.ResourcesSubscription_ResourceItemRemoved);
      this.resourcesSubscription.PathNodeContentChanged += new SceneNodeSubscription<ResourceContainer, ResourceItem>.PathNodeContentChangedListener(this.ResourcesSubscription_ResourceItemContentChanged);
      this.filteredResources = new SelectedElementsResourceObserver(this);
    }

    public ResourceDictionaryContentProvider GetContentProviderForResourceDictionary(IProjectItem item)
    {
      ResourceDictionaryContentProvider dictionaryContentProvider;
      if (!this.providers.TryGetValue(item, out dictionaryContentProvider))
      {
        dictionaryContentProvider = new ResourceDictionaryContentProvider(this.designerContext, item);
        this.providers[item] = dictionaryContentProvider;
      }
      return dictionaryContentProvider;
    }

    public ResourceDictionaryContentProvider FindContentProviderForResourceDictionary(SceneDocument resourceDictionary)
    {
      foreach (KeyValuePair<IProjectItem, ResourceDictionaryContentProvider> keyValuePair in (IEnumerable<KeyValuePair<IProjectItem, ResourceDictionaryContentProvider>>) this.providers)
      {
        if (keyValuePair.Key.Document == resourceDictionary)
          return keyValuePair.Value;
      }
      return (ResourceDictionaryContentProvider) null;
    }

    private void SelectionManager_ActiveSceneSwitched(object sender, EventArgs e)
    {
      this.basisSubscription.CurrentViewModel = this.ActiveSceneViewModel;
      this.resourcesSubscription.CurrentViewModel = this.ActiveSceneViewModel;
      this.UpdateFilteredResources(this.ActiveSceneViewModel, SceneViewModel.ViewStateBits.None);
    }

    private void ProjectManager_SolutionOpened(object sender, SolutionEventArgs e)
    {
      this.designerContext.ProjectManager.ProjectOpened += new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
      this.TargetProjects = this.designerContext.ProjectManager.CurrentSolution.Projects;
    }

    private void ProjectManager_SolutionClosed(object sender, SolutionEventArgs e)
    {
      this.designerContext.ProjectManager.ProjectOpened -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectOpened);
      this.TargetProjects = (IEnumerable<IProject>) null;
    }

    private void ProjectManager_ProjectOpened(object sender, ProjectEventArgs e)
    {
      this.TargetProjects = this.designerContext.ProjectManager.CurrentSolution.Projects;
    }

    private void ProjectManager_ProjectClosing(object sender, ProjectEventArgs e)
    {
      IProject project = e.Project;
      int index = 0;
      while (index < this.documentResourceContainers.Count)
      {
        DocumentResourceContainer resourceContainer = this.documentResourceContainers[index];
        if (resourceContainer.ProjectItem.Project == project)
        {
          this.OnContainerRemoved((ResourceContainer) resourceContainer);
          this.documentResourceContainers.RemoveAt(index);
        }
        else
          ++index;
      }
      List<IProjectItem> list = new List<IProjectItem>();
      foreach (IProjectItem projectItem in (IEnumerable<IProjectItem>) this.providers.Keys)
      {
        if (projectItem.Project == project)
          list.Add(projectItem);
      }
      foreach (IProjectItem key in list)
      {
        ResourceDictionaryContentProvider dictionaryContentProvider;
        if (this.providers.TryGetValue(key, out dictionaryContentProvider))
        {
          dictionaryContentProvider.Close();
          this.providers.Remove(key);
        }
      }
      this.RecalculateDocumentContainersEnabled();
    }

    private void Project_ItemRemoved(object sender, ProjectItemEventArgs e)
    {
      for (int index = 0; index < this.documentResourceContainers.Count; ++index)
      {
        DocumentResourceContainer resourceContainer = this.documentResourceContainers[index];
        if (resourceContainer.ProjectItem == e.ProjectItem)
        {
          this.OnContainerRemoved((ResourceContainer) resourceContainer);
          this.documentResourceContainers.RemoveAt(index);
          break;
        }
      }
      ResourceDictionaryContentProvider dictionaryContentProvider;
      if (this.providers.TryGetValue(e.ProjectItem, out dictionaryContentProvider))
      {
        dictionaryContentProvider.Close();
        this.providers.Remove(e.ProjectItem);
      }
      this.RecalculateDocumentContainersEnabled();
    }

    private void Project_ItemChanged(object sender, ProjectItemEventArgs e)
    {
      bool flag;
      ResourceDictionaryContentProvider provider;
      if (!this.providers.TryGetValue(e.ProjectItem, out provider) || this.isUpdatingProvider.TryGetValue(provider, out flag))
        return;
      this.isUpdatingProvider.Add(provider, true);
      provider.Close();
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (() =>
      {
        if (this.designerContext.ProjectManager == null || this.designerContext.ProjectManager.ActiveBuildTarget == null)
          return;
        if (this.providers.ContainsKey(e.ProjectItem))
          provider.ProjectItem = e.ProjectItem;
        this.isUpdatingProvider.Remove(provider);
      }));
    }

    private void Project_ItemAdded(object sender, ProjectItemEventArgs e)
    {
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) new DispatcherOperationCallback(this.OnProjectItemAdded), (object) e.ProjectItem);
    }

    private object OnProjectItemAdded(object item)
    {
      IProjectItem projectItem = (IProjectItem) item;
      IProjectDocument projectDocument = this.TryAddItem(projectItem);
      ResourceContainer container = (ResourceContainer) null;
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(projectItem.Project);
      if (projectContext != null && projectDocument != null && (projectDocument.DocumentType == ProjectDocumentType.ResourceDictionary && !projectItem.ContainsDesignTimeResources))
      {
        IProjectDocument application = projectContext.Application;
        if (application != null)
          container = this.FindResourceContainer(application.Path);
        if (container == null)
        {
          ResourceContainer resourceContainer = this.TopLevelResourceContainer;
          if (resourceContainer != null && resourceContainer.ProjectContext == projectContext)
            container = resourceContainer;
        }
        if (container != null)
          this.LinkToResource(container, projectItem.DocumentReference);
      }
      this.RecalculateDocumentContainersEnabled();
      return (object) null;
    }

    private void ViewService_ViewOpened(object sender, ViewEventArgs e)
    {
      SceneView sceneView = e.View as SceneView;
      if (sceneView == null)
        return;
      this.UpdateProviderViews(sceneView.Document, sceneView);
      this.UpdateWatchedViewModels(sceneView);
    }

    private void ViewService_ViewClosed(object sender, ViewEventArgs e)
    {
      SceneView sceneView = e.View as SceneView;
      if (sceneView == null)
        return;
      this.UpdateProviderViews(sceneView.Document, (SceneView) null);
      this.RemoveWatchedViewModel(sceneView.Document.DocumentContext);
    }

    private void UpdateProviderViews(SceneDocument targetDocument, SceneView newValue)
    {
      foreach (KeyValuePair<IProjectItem, ResourceDictionaryContentProvider> keyValuePair in (IEnumerable<KeyValuePair<IProjectItem, ResourceDictionaryContentProvider>>) this.providers)
      {
        if (keyValuePair.Key.Document == targetDocument)
          keyValuePair.Value.View = newValue;
      }
    }

    private IProjectDocument TryAddItem(IProjectItem projectItem)
    {
      IProjectContext projectContext = (IProjectContext) ProjectXamlContext.GetProjectContext(projectItem.Project);
      if (projectContext == null)
        return (IProjectDocument) null;
      IProjectDocument document = projectContext.GetDocument(DocumentReferenceLocator.GetDocumentLocator(projectItem.DocumentReference));
      if (document != null && (document.DocumentType == ProjectDocumentType.ResourceDictionary || document.DocumentType == ProjectDocumentType.Application))
      {
        foreach (DocumentResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
        {
          if (resourceContainer.ProjectItem == projectItem)
            return document;
        }
        DocumentResourceContainer resourceContainer1 = new DocumentResourceContainer(this, projectItem);
        this.documentResourceContainers.Add(resourceContainer1);
        resourceContainer1.ResourceItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnDocumentResourceContainerItemsChanged);
      }
      return document;
    }

    public static IEnumerable<DocumentNode> GetMergedDictionaries(ResourceContainer container)
    {
      DocumentCompositeNode mergedDictionaries = container.ResourcesCollection.Resources != null ? container.ResourcesCollection.Resources.Properties[ResourceDictionaryNode.MergedDictionariesProperty] as DocumentCompositeNode : (DocumentCompositeNode) null;
      if (mergedDictionaries != null && mergedDictionaries.SupportsChildren)
      {
        foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) mergedDictionaries.Children)
          yield return documentNode;
      }
    }

    private void RecalculateDocumentContainersEnabled()
    {
      SceneDocument applicationSceneDocument = this.ApplicationSceneDocument;
      List<ResourceContainer> list1 = new List<ResourceContainer>();
      if (applicationSceneDocument != null && applicationSceneDocument.IsEditable)
      {
        foreach (ResourceContainer resourceContainer1 in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
        {
          if (resourceContainer1.DocumentNode == applicationSceneDocument.DocumentRoot.RootNode)
          {
            list1.Add(resourceContainer1);
          }
          else
          {
            DocumentResourceContainer resourceContainer2 = resourceContainer1 as DocumentResourceContainer;
            if (resourceContainer2 != null && resourceContainer2.ProjectContext == applicationSceneDocument.ProjectContext && resourceContainer2.ProjectItem.ContainsDesignTimeResources)
              list1.Add(resourceContainer1);
          }
        }
      }
      else if (this.ActiveProjectContext != null)
      {
        foreach (DocumentResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
        {
          if (resourceContainer.Document != null && resourceContainer.ProjectContext == this.ActiveProjectContext && resourceContainer.ProjectItem.ContainsDesignTimeResources)
            list1.Add((ResourceContainer) resourceContainer);
        }
      }
      Dictionary<ResourceContainer, HashSet<Uri>> dictionary = new Dictionary<ResourceContainer, HashSet<Uri>>();
      foreach (ResourceContainer resourceContainer in list1)
      {
        HashSet<Uri> hashSet = (HashSet<Uri>) null;
        foreach (DocumentNode documentNode in ResourceManager.GetMergedDictionaries(resourceContainer))
        {
          DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            Uri uriValue = documentCompositeNode.GetUriValue(ResourceDictionaryNode.SourceProperty);
            if (uriValue != (Uri) null)
            {
              if (hashSet == null)
                hashSet = new HashSet<Uri>();
              if (!hashSet.Contains(uriValue))
                hashSet.Add(uriValue);
            }
          }
        }
        if (hashSet != null)
          dictionary.Add(resourceContainer, hashSet);
      }
      ResourceContainer activeRootContainer = this.ActiveRootContainer;
      IProjectContext activeProjectContext = this.ActiveProjectContext;
      List<DocumentResourceContainer> list2 = new List<DocumentResourceContainer>();
      foreach (DocumentResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
      {
        bool flag = resourceContainer.Document != null && resourceContainer.DocumentContext != null && (resourceContainer.ProjectContext == activeProjectContext || list1.Contains((ResourceContainer) resourceContainer));
        if (!flag)
        {
          foreach (ResourceContainer key in Enumerable.Where<ResourceContainer>((IEnumerable<ResourceContainer>) list1, (Func<ResourceContainer, bool>) (container => container.Document != null)))
          {
            IProject project = ProjectHelper.GetProject(this.designerContext.ProjectManager, key.DocumentContext);
            HashSet<Uri> hashSet;
            if ((key.ProjectContext == resourceContainer.ProjectContext || ProjectHelper.DoesProjectReferencesContainTarget(project, resourceContainer.ProjectContext)) && dictionary.TryGetValue(key, out hashSet))
            {
              string[] strArray = new string[2]
              {
                resourceContainer.ProjectContext.MakeResourceReference(resourceContainer.DocumentReference.Path, DocumentReferenceLocator.GetDocumentLocator(key.DocumentReference)),
                key.DocumentReference.GetRelativePath(resourceContainer.DocumentReference)
              };
              foreach (string uriString in strArray)
              {
                if (uriString != null)
                {
                  Uri result = (Uri) null;
                  if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out result) && hashSet.Contains(result))
                  {
                    flag = true;
                    break;
                  }
                }
              }
              if (flag)
                break;
            }
          }
        }
        if (flag)
          list2.Add(resourceContainer);
        resourceContainer.IsInScope = list1.Contains((ResourceContainer) resourceContainer) || resourceContainer == activeRootContainer;
      }
      bool flag1 = this.visibleDocumentResourceContainers.Count == list2.Count;
      for (int index = 0; index < this.visibleDocumentResourceContainers.Count && flag1; ++index)
      {
        if (list2[index] != this.visibleDocumentResourceContainers[index])
          flag1 = false;
      }
      if (flag1)
        return;
      ++this.extraChangeStamp;
      this.visibleDocumentResourceContainers.Clear();
      foreach (DocumentResourceContainer resourceContainer in list2)
        this.visibleDocumentResourceContainers.Add(resourceContainer);
    }

    private void OnDocumentResourceContainerItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      ++this.extraChangeStamp;
    }

    public void Unload()
    {
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.SelectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      this.designerContext.SelectionManager.ActiveSceneSwitched -= new EventHandler(this.SelectionManager_ActiveSceneSwitched);
      this.designerContext.ProjectManager.SolutionOpened -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionOpened);
      this.designerContext.ProjectManager.SolutionClosed -= new EventHandler<SolutionEventArgs>(this.ProjectManager_SolutionClosed);
      this.designerContext.ProjectManager.ProjectClosing -= new EventHandler<ProjectEventArgs>(this.ProjectManager_ProjectClosing);
      this.designerContext.ViewService.ViewOpened -= new ViewEventHandler(this.ViewService_ViewOpened);
      this.designerContext.ViewService.ViewClosed -= new ViewEventHandler(this.ViewService_ViewClosed);
    }

    internal void OnContainerRemoved(ResourceContainer container)
    {
      if (this.selectedResources.IsSelected((ResourceEntryBase) container))
        this.selectedResources.RemoveSelection((ResourceEntryBase) container);
      List<ResourceEntryBase> list = new List<ResourceEntryBase>();
      foreach (ResourceItem resourceItem in (IEnumerable<ResourceItem>) this.SelectedResourceItems)
      {
        if (resourceItem.Container == container)
          list.Add((ResourceEntryBase) resourceItem);
      }
      this.selectedResources.RemoveSelection((ICollection<ResourceEntryBase>) list);
      if (container is DocumentResourceContainer)
        container.ResourceItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnDocumentResourceContainerItemsChanged);
      if (this.ContainerRemoved != null)
        this.ContainerRemoved((object) this, new EventArgs<ResourceContainer>(container));
      container.Close();
    }

    internal void OnItemRemoved(ResourceItem oldItem)
    {
      if (this.selectedResources.IsSelected((ResourceEntryBase) oldItem))
        this.selectedResources.RemoveSelection((ResourceEntryBase) oldItem);
      if (this.ItemRemoved == null)
        return;
      this.ItemRemoved((object) this, new EventArgs<ResourceItem>(oldItem));
    }

    private ResourceContainer ResourcesSubscription_ResourceContainerInsertedHandler(object sender, SceneNode newBasisNode)
    {
      ResourceContainer resourceContainer1 = (ResourceContainer) null;
      bool flag = false;
      if (newBasisNode.Type.Metadata.ResourcesProperty != null)
      {
        resourceContainer1 = (ResourceContainer) new NodeResourceContainer(this, newBasisNode);
      }
      else
      {
        foreach (DocumentResourceContainer resourceContainer2 in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
        {
          if (resourceContainer2.IsEditable && resourceContainer2.ViewModel == newBasisNode.ViewModel)
          {
            resourceContainer1 = (ResourceContainer) resourceContainer2;
            resourceContainer1.IsInScope = true;
            flag = true;
            break;
          }
        }
      }
      if (resourceContainer1 != null && !flag)
        this.localResourceContainers.Add(resourceContainer1);
      return resourceContainer1;
    }

    private void ResourcesSubscription_ResourceContainerRemoved(object sender, SceneNode oldBasisNode, ResourceContainer oldResourceContainer)
    {
      if (oldResourceContainer == null)
        return;
      if (oldResourceContainer is NodeResourceContainer)
      {
        this.localResourceContainers.Remove(oldResourceContainer);
        this.OnContainerRemoved(oldResourceContainer);
      }
      else
        oldResourceContainer.IsInScope = false;
    }

    private ResourceItem ResourcesSubscription_ResourceItemInsertedHandler(object sender, SceneNode basisNode, ResourceContainer basisContainer, SceneNode newPathNode)
    {
      ResourceItem resourceItem1 = (ResourceItem) null;
      if (basisContainer is NodeResourceContainer)
      {
        DocumentCompositeNode node = newPathNode.DocumentNode as DocumentCompositeNode;
        if (node != null)
        {
          ResourceEntryBase.Comparer comparer = new ResourceEntryBase.Comparer();
          DocumentCompositeNode parent = newPathNode.DocumentNode.Parent;
          int index = 0;
          resourceItem1 = this.GetResourceItem(basisContainer, node);
          foreach (ResourceItem resourceItem2 in (Collection<ResourceItem>) basisContainer.ResourceItems)
          {
            if (comparer.Compare((ResourceEntryBase) resourceItem1, (ResourceEntryBase) resourceItem2) >= 0)
              ++index;
            else
              break;
          }
          if (index < basisContainer.ResourceItems.Count)
            basisContainer.ResourceItems.Insert(index, resourceItem1);
          else
            basisContainer.ResourceItems.Add(resourceItem1);
        }
      }
      return resourceItem1;
    }

    internal ResourceContainer GetResourceContainer(DocumentNode node)
    {
      if (node.Parent == null || !PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) node.Parent.Type))
        return (ResourceContainer) null;
      DocumentNode documentNode = (DocumentNode) node.Parent.Parent ?? (DocumentNode) node.Parent;
      foreach (ResourceContainer resourceContainer in this.ActiveResourceContainers)
      {
        if (resourceContainer.DocumentNode == documentNode)
          return resourceContainer;
      }
      foreach (ResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.DocumentResourceContainers)
      {
        if (resourceContainer.DocumentNode == documentNode)
          return resourceContainer;
      }
      return (ResourceContainer) null;
    }

    internal ResourceItem GetResourceItem(ResourceContainer basisContainer, DocumentCompositeNode node)
    {
      return !PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) node.Type) ? (ResourceItem) new ResourceDictionaryItem(this, basisContainer, node) : ResourceEntryItem.GetTypedItem(this, basisContainer, new ResourceModel(node));
    }

    internal ResourceItem GetResourceItem(DocumentCompositeNode node)
    {
      ResourceContainer basisContainer = this.GetResourceContainer((DocumentNode) node);
      if (basisContainer == null && this.ActiveSceneViewModel != null && (PlatformTypes.DictionaryEntry.Equals((object) node.Type) && node.Parent != null))
      {
        SceneViewModel viewModel = this.ActiveSceneViewModel.GetViewModel(node.DocumentRoot, false);
        if (viewModel != null)
          basisContainer = (ResourceContainer) new NodeResourceContainer(this, viewModel.GetSceneNode((DocumentNode) node.Parent));
      }
      if (basisContainer == null)
        return (ResourceItem) null;
      return this.GetResourceItem(basisContainer, node);
    }

    private void ResourcesSubscription_ResourceItemRemoved(object sender, SceneNode basisNode, ResourceContainer basisContainer, SceneNode oldPathNode, ResourceItem oldItem)
    {
      if (!(basisContainer is NodeResourceContainer))
        return;
      basisContainer.ResourceItems.Remove(oldItem);
      this.OnItemRemoved(oldItem);
    }

    private void ResourcesSubscription_ResourceItemContentChanged(object sender, SceneNode pathNode, ResourceItem item, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      ResourceEntryItem resourceEntryItem = item as ResourceEntryItem;
      if (resourceEntryItem == null)
        return;
      resourceEntryItem.OnKeyChanged();
    }

    private IEnumerable<SceneNode> ResourceItemEnumerator(SceneNode pivot)
    {
      ResourceDictionaryNode searchDictionary = ResourceManager.ProvideResourcesForElement(pivot);
      if (searchDictionary != null)
      {
        foreach (DictionaryEntryNode dictionaryEntryNode in searchDictionary)
        {
          if (dictionaryEntryNode.Value != null)
          {
            DocumentCompositeNode resourceValue = dictionaryEntryNode.Value.DocumentNode as DocumentCompositeNode;
            if (!PlatformTypes.Storyboard.IsAssignableFrom((ITypeId) dictionaryEntryNode.Value.Type) && (resourceValue == null || !resourceValue.GetValue<bool>(DesignTimeProperties.IsDataSourceProperty)))
              yield return (SceneNode) dictionaryEntryNode;
          }
        }
        foreach (ResourceDictionaryNode resourceDictionaryNode in (IEnumerable<ResourceDictionaryNode>) searchDictionary.MergedDictionaries)
          yield return (SceneNode) resourceDictionaryNode;
      }
    }

    internal static DocumentCompositeNode ProvideResourcesForDocumentNode(DocumentCompositeNode documentNode)
    {
      if (documentNode == null)
        return (DocumentCompositeNode) null;
      IPropertyId resourcesProperty = documentNode.Type.Metadata.ResourcesProperty;
      DocumentCompositeNode documentCompositeNode = resourcesProperty != null ? documentNode.Properties[resourcesProperty] as DocumentCompositeNode : (DocumentCompositeNode) null;
      if (documentCompositeNode == null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentNode.Type))
        documentCompositeNode = documentNode;
      return documentCompositeNode;
    }

    internal static ResourceDictionaryNode ProvideResourcesForElement(SceneNode node)
    {
      DocumentCompositeNode documentCompositeNode = ResourceManager.ProvideResourcesForDocumentNode(node.DocumentNode as DocumentCompositeNode);
      if (documentCompositeNode == null)
        return (ResourceDictionaryNode) null;
      return (node.ViewModel.IsExternal((DocumentNode) documentCompositeNode) ? node.ViewModel.GetViewModel(documentCompositeNode.DocumentRoot, false).GetSceneNode((DocumentNode) documentCompositeNode) : node.ViewModel.GetSceneNode((DocumentNode) documentCompositeNode)) as ResourceDictionaryNode;
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.OnSceneUpdate(args);
    }

    public void OnSceneUpdate(SceneUpdatePhaseEventArgs args)
    {
      if (args.ViewModel != null && (int) args.ViewModel.ChangeStamp == (int) this.viewStateChangeStamp && (int) args.DocumentChangeStamp == (int) this.documentChangeStamp)
        return;
      if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable) || this.primarySelection != null && this.primarySelection.ViewModel != args.ViewModel)
      {
        this.primarySelection = (BaseFrameworkElement) null;
        ++this.extraChangeStamp;
        if (this.ActiveSceneViewModel == null)
        {
          this.basisSubscription.CurrentViewModel = (SceneViewModel) null;
          this.resourcesSubscription.CurrentViewModel = (SceneViewModel) null;
          this.UpdateFilteredResources((SceneViewModel) null, SceneViewModel.ViewStateBits.None);
          return;
        }
      }
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        this.RecalculatePrimarySelection();
      if (args.Document != null)
      {
        if (args.RootNodeChanged || args.Document == null || !args.Document.IsEditable)
        {
          List<ResourceEntryBase> list = (List<ResourceEntryBase>) null;
          foreach (ResourceEntryBase resourceEntryBase in this.selectedResources.Selection)
          {
            if (resourceEntryBase.DocumentNode == null || resourceEntryBase.DocumentNode.DocumentRoot == null)
            {
              if (list == null)
                list = new List<ResourceEntryBase>();
              list.Add(resourceEntryBase);
            }
          }
          if (list != null)
            this.selectedResources.RemoveSelection((ICollection<ResourceEntryBase>) list);
        }
        List<SceneNode> list1 = new List<SceneNode>(1);
        if (this.ActiveSceneViewModel.RootNode is IResourceContainer)
          list1.Add(this.ActiveSceneViewModel.RootNode);
        this.basisSubscription.SetSceneRootNodeAsTheBasisNode(this.ActiveSceneViewModel);
        if (this.primarySelection != null)
          list1.Add((SceneNode) this.primarySelection);
        this.basisSubscription.Update(this.ActiveSceneViewModel, args.DocumentChanges, args.DocumentChangeStamp);
        this.resourcesSubscription.ChainUpdate(this.ActiveSceneViewModel, this.basisSubscription.PathNodeList, (IEnumerable<SceneNode>) list1, args.DocumentChanges, args.DocumentChangeStamp);
        foreach (SceneChange sceneChange in SceneChange.Changes(args.DocumentChanges, this.ActiveSceneViewModel.RootNode, (Type) null))
        {
          PropertyReferenceSceneChange e = sceneChange as PropertyReferenceSceneChange;
          if (e != null)
            this.OnPropertyReferenceSceneChange(e);
        }
      }
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        this.RecalculateInScopeContainers();
      this.UpdateFilteredResources(this.ActiveSceneViewModel, args.DirtyViewState);
      if (args.ViewModel == null || args.Document == null)
        return;
      this.viewStateChangeStamp = args.ViewModel.ChangeStamp;
      this.documentChangeStamp = args.DocumentChangeStamp;
    }

    private void UpdateFilteredResources(SceneViewModel viewModel, SceneViewModel.ViewStateBits viewStateBits)
    {
      if (!this.IsFiltering)
        return;
      this.filteredResources.Update(viewModel, viewStateBits);
      if (this.filteredResources.ViewModel == null)
      {
        this.filteredResourceContainers.Clear();
      }
      else
      {
        if (this.filteredResourceContainers.Count != 0)
          return;
        this.filteredResourceContainers.Add((ResourceContainer) this.filteredResources);
      }
    }

    private void OnPropertyReferenceSceneChange(PropertyReferenceSceneChange e)
    {
      if (e.PropertyChanged == null || e.Target == null || e.PropertyChanged.Count != 1)
        return;
      SceneElement sceneElement = e.Target as SceneElement;
      if (sceneElement == null)
        return;
      IPropertyId nameProperty = sceneElement.NameProperty;
      if (nameProperty == null || e.PropertyChanged[0] != nameProperty)
        return;
      SceneNodeSubscription<ResourceContainer, ResourceItem>.BasisNodeInfo basisNode = this.ResourcesSubscription.FindBasisNode(e.Target);
      if (basisNode.Info != null)
        basisNode.Info.RefreshName();
      if (!this.IsFiltering)
        return;
      this.filteredResources.RefreshName();
    }

    public void RegisterPropertyChangedEventHandler(DocumentNodeMarker resourceMarker, IDocumentContext context, PropertyReference propertyReference, PropertyReferenceChangedEventHandler handler)
    {
      SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>> sortedList;
      if (!this.propertyReferenceChangedHandlers.TryGetValue(resourceMarker, out sortedList))
      {
        sortedList = new SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>>();
        this.propertyReferenceChangedHandlers[resourceMarker] = sortedList;
      }
      List<PropertyReferenceChangedEventHandler> list;
      if (!sortedList.TryGetValue(propertyReference, out list))
      {
        list = new List<PropertyReferenceChangedEventHandler>();
        sortedList[propertyReference] = list;
      }
      list.Add(handler);
      this.AddWatchedPropertyRoot(context, resourceMarker);
    }

    public void UnregisterPropertyChangedEventHandler(DocumentNodeMarker resourceMarker, IDocumentContext context, PropertyReference propertyReference, PropertyReferenceChangedEventHandler handler)
    {
      SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>> sortedList = this.propertyReferenceChangedHandlers[resourceMarker];
      List<PropertyReferenceChangedEventHandler> list = sortedList[propertyReference];
      list.Remove(handler);
      if (list.Count == 0)
        sortedList.Remove(propertyReference);
      if (sortedList.Count != 0)
        return;
      this.propertyReferenceChangedHandlers.Remove(resourceMarker);
      this.RemoveWatchedPropertyRoot(context, resourceMarker);
    }

    private void RemoveWatchedViewModel(IDocumentContext context)
    {
      SceneViewModel viewModel;
      if (!this.watchedViewModels.TryGetValue(context, out viewModel))
        return;
      this.RemoveWatchedViewModel(context, viewModel);
    }

    private void AddWatchedPropertyRoot(IDocumentContext context, DocumentNodeMarker resourceMarker)
    {
      DocumentNodeMarkerSortedList markerSortedList;
      if (!this.watchedPropertyRoots.TryGetValue(context, out markerSortedList))
      {
        markerSortedList = new DocumentNodeMarkerSortedList();
        this.watchedPropertyRoots[context] = markerSortedList;
      }
      if (markerSortedList.FindFirstPosition(resourceMarker) < 0)
        markerSortedList.Add(resourceMarker);
      if (this.watchedViewModels.ContainsKey(context))
        return;
      foreach (IView view in (IEnumerable<IView>) this.designerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && sceneView.ViewModel.Document.DocumentContext == context)
        {
          this.AddWatchedViewModel(context, sceneView.ViewModel);
          break;
        }
      }
    }

    private void RemoveWatchedPropertyRoot(IDocumentContext context, DocumentNodeMarker resourceMarker)
    {
      DocumentNodeMarkerSortedList markerSortedList;
      if (!this.watchedPropertyRoots.TryGetValue(context, out markerSortedList))
        return;
      if (markerSortedList.FindFirstPosition(resourceMarker) >= 0)
        markerSortedList.Remove(resourceMarker);
      if (markerSortedList.Count != 0)
        return;
      this.watchedPropertyRoots.Remove(context);
      SceneViewModel viewModel;
      if (!this.watchedViewModels.TryGetValue(context, out viewModel))
        return;
      this.RemoveWatchedViewModel(context, viewModel);
    }

    private void AddWatchedViewModel(IDocumentContext context, SceneViewModel viewModel)
    {
      viewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.OnWatchedSceneUpdated);
      this.watchedViewModels[context] = viewModel;
    }

    private void RemoveWatchedViewModel(IDocumentContext context, SceneViewModel viewModel)
    {
      this.watchedViewModels.Remove(context);
      viewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.OnWatchedSceneUpdated);
    }

    private void UpdateWatchedViewModels(SceneView newView)
    {
      SceneViewModel viewModel = newView.ViewModel;
      if (this.watchedViewModels.ContainsKey(viewModel.Document.DocumentContext) || !this.watchedPropertyRoots.ContainsKey(viewModel.Document.DocumentContext))
        return;
      this.AddWatchedViewModel(viewModel.Document.DocumentContext, viewModel);
    }

    private void OnWatchedSceneUpdated(object sender, SceneUpdatePhaseEventArgs args)
    {
      DocumentNodeMarkerSortedList markerSortedList;
      if (!this.watchedPropertyRoots.TryGetValue(args.ViewModel.Document.DocumentContext, out markerSortedList))
        return;
      OrderedList<PropertyReference> changes = new OrderedList<PropertyReference>((IComparer<PropertyReference>) new PropertyReference.Comparer());
      DocumentNodeMarker index = (DocumentNodeMarker) null;
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in args.DocumentChanges.Intersect((DocumentNodeMarkerSortedListBase) markerSortedList, DocumentNodeMarkerSortedListBase.Flags.ContainedBy))
      {
        DocumentNodeMarker ancestor = markerSortedList.MarkerAt(intersectionResult.RightHandSideIndex);
        if (ancestor != index)
        {
          if (index != null)
            this.FirePropertyReferenceChangedHandlersInternal(changes, this.propertyReferenceChangedHandlers[index], args.DirtyViewState);
          changes.Clear();
          index = ancestor;
        }
        Stack<ReferenceStep> input = DocumentNodeMarkerUtilities.PropertyReferencePath(args.DocumentChanges.MarkerAt(intersectionResult.LeftHandSideIndex), ancestor);
        if (input.Count > 0)
        {
          PropertyReference propertyReference = new PropertyReference(input);
          int num = changes.BinarySearch(propertyReference);
          if (num < 0)
            changes.Insert(~num, propertyReference);
        }
      }
      if (index == null)
        return;
      this.FirePropertyReferenceChangedHandlersInternal(changes, this.propertyReferenceChangedHandlers[index], args.DirtyViewState);
    }

    private void FirePropertyReferenceChangedHandlersInternal(OrderedList<PropertyReference> changes, SortedList<PropertyReference, List<PropertyReferenceChangedEventHandler>> handlers, SceneViewModel.ViewStateBits viewState)
    {
      int index1 = 0;
      int index2 = 0;
      for (; index1 < handlers.Count; ++index1)
      {
        while (index2 < changes.Count && index1 < handlers.Count && changes[index2].CompareTo((object) handlers.Keys[index1]) < 0)
        {
          if (changes[index2].IsPrefixOf(handlers.Keys[index1]))
          {
            this.InvokeHandlers(handlers.Values[index1], handlers.Keys[index1], viewState);
            ++index1;
          }
          else
            ++index2;
        }
        if (index2 >= changes.Count || index1 >= handlers.Count)
          break;
        if (changes[index2].CompareTo((object) handlers.Keys[index1]) == 0)
          this.InvokeHandlers(handlers.Values[index1], handlers.Keys[index1], viewState);
        else if (handlers.Keys[index1].IsPrefixOf(changes[index2]))
          this.InvokeHandlers(handlers.Values[index1], changes[index2], viewState);
      }
    }

    private void InvokeHandlers(List<PropertyReferenceChangedEventHandler> handlers, PropertyReference propertyReference, SceneViewModel.ViewStateBits viewState)
    {
      List<PropertyReferenceChangedEventHandler> list = new List<PropertyReferenceChangedEventHandler>((IEnumerable<PropertyReferenceChangedEventHandler>) handlers);
      PropertyReferenceChangedEventArgs e = new PropertyReferenceChangedEventArgs(viewState, propertyReference);
      foreach (PropertyReferenceChangedEventHandler changedEventHandler in list)
        changedEventHandler((object) this, e);
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.documentChangeStamp = uint.MaxValue;
      this.viewStateChangeStamp = uint.MaxValue;
      this.RecalculatePrimarySelection();
      this.RecalculateInScopeContainers();
      this.RecalculateDocumentContainersEnabled();
      SceneView sceneView = e.OldView as SceneView;
      if (sceneView != null && sceneView.ViewModel != null)
      {
        foreach (ResourceContainer resourceContainer in (Collection<DocumentResourceContainer>) this.documentResourceContainers)
        {
          if (resourceContainer.DocumentNode != null && sceneView.ViewModel.RootNode != null && resourceContainer.DocumentNode == sceneView.ViewModel.RootNode.DocumentNode)
            resourceContainer.IsInScope = false;
        }
      }
      if (this.ActiveSceneViewModel == null || this.ActiveRootContainer == null)
        return;
      this.ActiveRootContainer.IsInScope = true;
    }

    internal void LinkToResource(ResourceContainer container, DocumentReference dictionaryItem)
    {
      if (this.FindDictionaryItem(container, dictionaryItem) != null || container.DocumentReference == dictionaryItem)
        return;
      if (container.ViewModel.XamlDocument.HasTextEdits)
      {
        using (SceneEditTransaction editTransaction = container.ViewModel.CreateEditTransaction(StringTable.CommitTextEditsUndo))
        {
          container.ViewModel.XamlDocument.CommitTextEdits();
          editTransaction.Commit();
        }
      }
      if (container.ViewModel.XamlDocument.IsEditable)
      {
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.LinkToResourceDictionary);
        ResourceDictionaryNode resourceDictionaryNode = ResourceDictionaryNode.Factory.Instantiate(container.ViewModel);
        DocumentReference documentReference = container.ViewModel.Document.DocumentReference;
        string uriString = container.ProjectContext.MakeResourceReference(dictionaryItem.Path, DocumentReferenceLocator.GetDocumentLocator(container.DocumentReference));
        if (string.IsNullOrEmpty(uriString))
          uriString = dictionaryItem.Path;
        resourceDictionaryNode.Source = new Uri(uriString, UriKind.RelativeOrAbsolute);
        ResourceManager.ResourceContainerModel parent = new ResourceManager.ResourceContainerModel(container.ViewModel.Document.DocumentReference.Path, (ResourceManager.ResourceContainerModel) null);
        List<IProjectItem> list = new List<IProjectItem>();
        ResourceManager.ResourceContainerModel resourceContainerModel1 = (ResourceManager.ResourceContainerModel) null;
        try
        {
          resourceContainerModel1 = this.FindCyclicalDepenencies(ProjectHelper.GetProject(this.designerContext.ProjectManager, container.DocumentContext), new ResourceManager.ResourceContainerModel(dictionaryItem.Path, parent), (IList<IProjectItem>) list);
        }
        finally
        {
          foreach (IProjectItem projectItem in list)
            projectItem.CloseDocument();
        }
        if (resourceContainerModel1 != null)
        {
          string str1 = string.Empty;
          string str2 = string.Empty;
          for (ResourceManager.ResourceContainerModel resourceContainerModel2 = resourceContainerModel1; resourceContainerModel2.Parent != null; resourceContainerModel2 = resourceContainerModel2.Parent)
            str1 = Path.GetFileName(resourceContainerModel2.Location) + Environment.NewLine + str1;
          string str3 = Path.GetFileName(parent.Location) + Environment.NewLine + str1;
          this.designerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceDictionaryInstanceBuilderCycleDisplay, new object[2]
          {
            (object) dictionaryItem.DisplayName,
            (object) str3
          }));
        }
        else
        {
          using (SceneEditTransaction editTransaction = container.ViewModel.CreateEditTransaction(StringTable.UndoUnitLinkToResourceDictionary))
          {
            container.EnsureResourceDictionaryNode();
            container.ResourceDictionaryNode.MergedDictionaries.Add(resourceDictionaryNode);
            editTransaction.Commit();
          }
          ++this.extraChangeStamp;
        }
      }
      else
        this.designerContext.MessageDisplayService.ShowError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ResourceDictionaryMergeFailed_TargetDictionaryHasErrorsMessage, new object[2]
        {
          (object) dictionaryItem.DisplayName,
          (object) container.DocumentReference.DisplayName
        }));
    }

    private DocumentCompositeNode GetDocumentRootNode(IProject project, DocumentReference projectItemPath, out IProjectItem openedProjectItemDocument)
    {
      openedProjectItemDocument = (IProjectItem) null;
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) null;
      IProjectItem projectItem = project.FindItem(projectItemPath);
      if (projectItem != null)
      {
        if (!projectItem.IsOpen)
        {
          projectItem.OpenDocument(true);
          if (projectItem.IsOpen)
            openedProjectItemDocument = projectItem;
        }
        if (projectItem.IsOpen)
        {
          SceneDocument sceneDocument = projectItem.Document as SceneDocument;
          if (sceneDocument != null)
          {
            XamlDocument xamlDocument = (XamlDocument) sceneDocument.XamlDocument;
            if (xamlDocument != null)
              documentCompositeNode = xamlDocument.RootNode as DocumentCompositeNode;
          }
        }
      }
      return documentCompositeNode;
    }

    internal void FindAllReachableDictionaries(ResourceContainer resourceContainer, ICollection<ResourceDictionaryItem> linkedDictionaries)
    {
      if (resourceContainer == null)
        return;
      DocumentCompositeNode documentCompositeNode1 = ResourceManager.ProvideResourcesForDocumentNode(resourceContainer.DocumentNode as DocumentCompositeNode);
      if (documentCompositeNode1 == null)
        return;
      DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[ResourceDictionaryNode.MergedDictionariesProperty] as DocumentCompositeNode;
      if (documentCompositeNode2 == null)
        return;
      for (int index = 0; index < documentCompositeNode2.Children.Count; ++index)
      {
        DocumentCompositeNode dictionary = documentCompositeNode2.Children[index] as DocumentCompositeNode;
        if (dictionary != null)
        {
          ResourceDictionaryItem resourceDictionaryItem1 = new ResourceDictionaryItem(this, resourceContainer, dictionary);
          bool flag = false;
          if (!string.IsNullOrEmpty(resourceDictionaryItem1.DesignTimeSource) && resourceContainer.ProjectContext.OpenDocument(resourceDictionaryItem1.DesignTimeSource) != null)
          {
            flag = true;
            foreach (ResourceDictionaryItem resourceDictionaryItem2 in (IEnumerable<ResourceDictionaryItem>) linkedDictionaries)
            {
              if (!string.IsNullOrEmpty(resourceDictionaryItem2.DesignTimeSource) && StringComparer.OrdinalIgnoreCase.Compare(resourceDictionaryItem2.DesignTimeSource, resourceDictionaryItem1.DesignTimeSource) == 0)
              {
                flag = false;
                break;
              }
            }
          }
          if (flag)
          {
            linkedDictionaries.Add(resourceDictionaryItem1);
            if (!string.IsNullOrEmpty(resourceDictionaryItem1.DesignTimeSource))
            {
              foreach (DocumentResourceContainer resourceContainer1 in (Collection<DocumentResourceContainer>) this.DocumentResourceContainers)
              {
                if (StringComparer.OrdinalIgnoreCase.Compare(resourceContainer1.DocumentReference.Path, resourceDictionaryItem1.DesignTimeSource) == 0)
                  this.FindAllReachableDictionaries((ResourceContainer) resourceContainer1, linkedDictionaries);
              }
            }
          }
        }
      }
    }

    internal ResourceContainer FindResourceContainer(string documentReference)
    {
      ResourceContainer resourceContainer1 = (ResourceContainer) null;
      foreach (ResourceContainer resourceContainer2 in (Collection<DocumentResourceContainer>) this.DocumentResourceContainers)
      {
        if (StringComparer.OrdinalIgnoreCase.Compare(documentReference, resourceContainer2.DocumentReference.Path) == 0)
        {
          resourceContainer1 = resourceContainer2;
          break;
        }
      }
      if (resourceContainer1 == null && this.ActiveRootContainer != null && StringComparer.OrdinalIgnoreCase.Compare(documentReference, this.ActiveRootContainer.DocumentReference.Path) == 0)
        resourceContainer1 = this.ActiveRootContainer;
      return resourceContainer1;
    }

    internal DocumentNode FindDictionaryItem(ResourceContainer targetContainer, DocumentReference targetReference)
    {
      DocumentReference documentReference = targetContainer.DocumentReference;
      string uriString = targetContainer.ProjectContext.MakeResourceReference(targetReference.Path, DocumentReferenceLocator.GetDocumentLocator(documentReference));
      if (!string.IsNullOrEmpty(uriString))
      {
        Uri uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
        DocumentCompositeNode documentCompositeNode1 = ResourceManager.ProvideResourcesForDocumentNode(targetContainer.DocumentNode as DocumentCompositeNode);
        if (documentCompositeNode1 == null)
          return (DocumentNode) null;
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[ResourceDictionaryNode.MergedDictionariesProperty] as DocumentCompositeNode;
        if (documentCompositeNode2 == null)
          return (DocumentNode) null;
        for (int index = 0; index < documentCompositeNode2.Children.Count; ++index)
        {
          DocumentCompositeNode dictionary = documentCompositeNode2.Children[index] as DocumentCompositeNode;
          if (dictionary != null)
          {
            ResourceDictionaryItem resourceDictionaryItem = new ResourceDictionaryItem(this, targetContainer, dictionary);
            if (!string.IsNullOrEmpty(resourceDictionaryItem.DesignTimeSource) && object.Equals((object) uri, (object) resourceDictionaryItem.SourceUri))
              return resourceDictionaryItem.DocumentNode;
          }
        }
      }
      return (DocumentNode) null;
    }

    private ResourceManager.ResourceContainerModel FindCyclicalDepenencies(IProject project, ResourceManager.ResourceContainerModel parentContainer, IList<IProjectItem> projectItemsOpened)
    {
      ResourceManager.ResourceContainerModel resourceContainerModel1 = (ResourceManager.ResourceContainerModel) null;
      IProjectItem openedProjectItemDocument;
      DocumentCompositeNode documentRootNode = this.GetDocumentRootNode(project, DocumentReference.Create(parentContainer.Location), out openedProjectItemDocument);
      if (documentRootNode != null && documentRootNode.DescendantNodes != null)
      {
        if (openedProjectItemDocument != null)
          projectItemsOpened.Add(openedProjectItemDocument);
        IProject project1 = ProjectHelper.GetProject(this.designerContext.ProjectManager, documentRootNode.Context);
        foreach (DocumentNode documentNode in documentRootNode.DescendantNodes)
        {
          if (PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentNode.Type))
          {
            DocumentCompositeNode documentCompositeNode = documentNode as DocumentCompositeNode;
            if (documentCompositeNode != null)
            {
              Uri uriValue = documentCompositeNode.GetUriValue(ResourceDictionaryNode.SourceProperty);
              if (uriValue != (Uri) null)
              {
                DocumentReference documentReference = DocumentReference.Create(parentContainer.Location);
                Uri uri1 = project1.MakeDesignTimeUri(uriValue, documentReference.Path);
                if (uri1.IsAbsoluteUri)
                {
                  string localPath1 = uri1.LocalPath;
                  ResourceManager.ResourceContainerModel parentContainer1 = new ResourceManager.ResourceContainerModel(localPath1, parentContainer);
                  for (ResourceManager.ResourceContainerModel resourceContainerModel2 = parentContainer; resourceContainerModel2 != null; resourceContainerModel2 = resourceContainerModel2.Parent)
                  {
                    Uri uri2 = project1.MakeDesignTimeUri(new Uri(resourceContainerModel2.Location, UriKind.RelativeOrAbsolute), documentReference.Path);
                    if (uri2.IsAbsoluteUri)
                    {
                      string localPath2 = uri2.LocalPath;
                      if (string.Compare(localPath1, localPath2, StringComparison.OrdinalIgnoreCase) == 0)
                      {
                        resourceContainerModel1 = parentContainer1;
                        break;
                      }
                    }
                  }
                  if (resourceContainerModel1 == null)
                    resourceContainerModel1 = this.FindCyclicalDepenencies(project1, parentContainer1, projectItemsOpened);
                  if (resourceContainerModel1 != null)
                    break;
                }
              }
            }
          }
        }
      }
      return resourceContainerModel1;
    }

    private void RecalculatePrimarySelection()
    {
      this.primarySelection = (BaseFrameworkElement) null;
      SceneView sceneView = this.designerContext.ViewService.ActiveView as SceneView;
      if (sceneView == null || !sceneView.IsValid || !sceneView.IsDesignSurfaceVisible)
        return;
      SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
      for (SceneNode sceneNode = elementSelectionSet.Count == 1 ? (SceneNode) elementSelectionSet.PrimarySelection : (SceneNode) null; sceneNode != null && this.primarySelection == null; sceneNode = sceneNode.Parent)
        this.primarySelection = sceneNode as BaseFrameworkElement;
    }

    private void RecalculateInScopeContainers()
    {
      List<SceneNode> list = new List<SceneNode>();
      SceneView sceneView = this.designerContext.ViewService.ActiveView as SceneView;
      if (sceneView != null && sceneView.IsValid && sceneView.IsDesignSurfaceVisible)
      {
        SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
        SceneNode sceneNode = (SceneNode) null;
        SceneNode rootNode = this.ActiveSceneViewModel.RootNode;
        if (elementSelectionSet != null)
          sceneNode = elementSelectionSet.Selection.Count != 0 ? (SceneNode) SceneElementHelper.FindLowestAncestorOfCollection((ICollection<SceneElement>) elementSelectionSet.Selection) : rootNode;
        if (sceneNode == null)
          sceneNode = rootNode;
        if (sceneNode != null)
        {
          SceneViewModel viewModel = sceneNode.ViewModel;
          for (DocumentNodePath documentNodePath = sceneNode.DocumentNodePath; documentNodePath != null; documentNodePath = documentNodePath.GetParent())
            list.Add(viewModel.GetSceneNode(documentNodePath.Node));
        }
      }
      foreach (ResourceContainer resourceContainer1 in (Collection<ResourceContainer>) this.localResourceContainers)
      {
        NodeResourceContainer resourceContainer2 = resourceContainer1 as NodeResourceContainer;
        if (resourceContainer2 != null)
          resourceContainer2.IsInScope = list.Contains(resourceContainer2.Node);
      }
    }

    internal IEnumerable<DocumentCompositeNode> GetResourcesInSelectionScope(ITypeId typeId, ResourceResolutionFlags flags)
    {
      return this.GetResourcesInElementsScope((IList<ResourceContainer>) new List<ResourceContainer>(this.ActiveResourceContainers), typeId, flags);
    }

    internal IEnumerable<DocumentCompositeNode> GetResourcesInRootScope(ITypeId typeId, ResourceResolutionFlags flags)
    {
      List<ResourceContainer> list = new List<ResourceContainer>(1);
      if (this.ActiveRootContainer != null)
        list.Add(this.ActiveRootContainer);
      return this.GetResourcesInElementsScope((IList<ResourceContainer>) list, typeId, flags);
    }

    internal IEnumerable<DocumentCompositeNode> GetResourcesInElementsScope(IList<ResourceContainer> resourceContainers, ITypeId typeId, ResourceResolutionFlags flags)
    {
      Dictionary<DocumentNode, DocumentNode> alreadyReturned = (Dictionary<DocumentNode, DocumentNode>) null;
      if ((flags & ResourceResolutionFlags.UniqueKeysOnly) != (ResourceResolutionFlags) 0)
        alreadyReturned = new Dictionary<DocumentNode, DocumentNode>((IEqualityComparer<DocumentNode>) DocumentNodeEqualityComparer.Instance);
      bool walkApplicationRoot = (flags & ResourceResolutionFlags.IncludeApplicationResources) == ResourceResolutionFlags.IncludeApplicationResources;
      bool uniqueResources = (flags & ResourceResolutionFlags.UniqueKeysOnly) == ResourceResolutionFlags.UniqueKeysOnly;
      foreach (DocumentCompositeNode entryNode in this.GetResourcesInElementsScope(resourceContainers, typeId, walkApplicationRoot, uniqueResources))
      {
        if (alreadyReturned == null || ResourceManager.AddIfMissing((IDictionary<DocumentNode, DocumentNode>) alreadyReturned, entryNode))
          yield return entryNode;
      }
    }

    private static bool AddIfMissing(IDictionary<DocumentNode, DocumentNode> dictionary, DocumentCompositeNode entryNode)
    {
      DocumentNode valueNode = entryNode.Properties[DictionaryEntryNode.ValueProperty];
      DocumentNode keyOrImplicitKey = ResourceManager.GetResourceKeyOrImplicitKey(entryNode, valueNode);
      if (keyOrImplicitKey == null || dictionary.ContainsKey(keyOrImplicitKey))
        return false;
      dictionary.Add(keyOrImplicitKey, valueNode);
      return true;
    }

    private static DocumentNode GetResourceKeyOrImplicitKey(DocumentCompositeNode entryNode, DocumentNode valueNode)
    {
      DocumentNode documentNode = (DocumentNode) null;
      if (valueNode != null)
      {
        documentNode = ResourceNodeHelper.GetResourceEntryKey(entryNode);
        if (documentNode == null)
        {
          DocumentCompositeNode documentCompositeNode = valueNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            IPropertyId dictionaryKeyProperty = documentCompositeNode.Type.Metadata.ImplicitDictionaryKeyProperty;
            if (dictionaryKeyProperty != null)
              documentNode = documentCompositeNode.Properties[dictionaryKeyProperty];
          }
        }
      }
      return documentNode;
    }

    private IEnumerable<DocumentCompositeNode> GetResourcesInElementsScope(IList<ResourceContainer> resourceContainers, ITypeId typeId, bool walkApplicationRoot, bool uniqueResourcesOnly)
    {
      List<DocumentCompositeNode> alreadyVisited = new List<DocumentCompositeNode>();
      IEnumerable<Uri> applicationReferencedResourceDictionaries = (IEnumerable<Uri>) null;
      IEnumerable<IDocumentRoot> applicationAndDesignTimeResources = Enumerable.Select<DocumentResourceContainer, IDocumentRoot>(Enumerable.Where<DocumentResourceContainer>((IEnumerable<DocumentResourceContainer>) this.VisibleDocumentResourceContainers, (Func<DocumentResourceContainer, bool>) (resourceContainer =>
      {
        if (resourceContainer.ProjectItem.ContainsDesignTimeResources)
          return true;
        if (this.ApplicationSceneDocument != null)
          return resourceContainer.DocumentReference == this.ApplicationSceneDocument.DocumentReference;
        return false;
      })), (Func<DocumentResourceContainer, IDocumentRoot>) (resourceContainer => resourceContainer.Document.DocumentRoot));
      for (int i = resourceContainers.Count - 1; i >= 0; --i)
      {
        ResourceContainer container = resourceContainers[i];
        IDocumentRootResolver documentRootResolver = (IDocumentRootResolver) container.ProjectContext;
        if (container.IsInScope)
        {
          if (walkApplicationRoot)
          {
            bool flag = false;
            foreach (IDocumentRoot documentRoot in applicationAndDesignTimeResources)
            {
              DocumentCompositeNode node = documentRoot.RootNode as DocumentCompositeNode;
              DocumentResourceContainer resourceContainer = container as DocumentResourceContainer;
              if (resourceContainer != null && node != null)
              {
                if (applicationReferencedResourceDictionaries == null)
                  applicationReferencedResourceDictionaries = ResourceNodeHelper.FindReferencedDictionaries(node);
                Uri result;
                if (Uri.TryCreate(resourceContainer.DocumentReference.Path, UriKind.Absolute, out result) && Enumerable.Contains<Uri>(applicationReferencedResourceDictionaries, result))
                {
                  flag = true;
                  break;
                }
              }
            }
            if (flag)
              continue;
          }
          DocumentCompositeNode containerDocumentNode = container.DocumentNode as DocumentCompositeNode;
          if (containerDocumentNode != null)
            alreadyVisited.Add(containerDocumentNode);
          List<DocumentCompositeNode> resourcesAtCurrentScope = new List<DocumentCompositeNode>();
          Dictionary<DocumentNode, DocumentCompositeNode> uniqueResourcesAtCurrentScope = new Dictionary<DocumentNode, DocumentCompositeNode>((IEqualityComparer<DocumentNode>) DocumentNodeEqualityComparer.Instance);
          foreach (ResourceDictionaryItem resourceDictionaryItem in Enumerable.OfType<ResourceDictionaryItem>((IEnumerable) container.ResourceItems))
          {
            IDocumentRoot documentRoot = (IDocumentRoot) null;
            try
            {
              if (!string.IsNullOrEmpty(resourceDictionaryItem.DesignTimeSource))
                documentRoot = documentRootResolver.GetDocumentRoot(resourceDictionaryItem.DesignTimeSource);
            }
            catch (IOException ex)
            {
            }
            catch (NotSupportedException ex)
            {
            }
            if (documentRoot != null && documentRoot.IsEditable && (documentRoot.RootNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentRoot.RootNode.Type)))
            {
              foreach (DocumentCompositeNode entryNode in this.GetResourceNodes((DocumentCompositeNode) documentRoot.RootNode, typeId, documentRootResolver, (IList<DocumentCompositeNode>) alreadyVisited))
              {
                DocumentNode valueNode = entryNode.Properties[DictionaryEntryNode.ValueProperty];
                DocumentNode keyOrImplicitKey = ResourceManager.GetResourceKeyOrImplicitKey(entryNode, valueNode);
                if (keyOrImplicitKey != null)
                {
                  resourcesAtCurrentScope.Add(entryNode);
                  uniqueResourcesAtCurrentScope[keyOrImplicitKey] = entryNode;
                }
              }
            }
          }
          foreach (ResourceEntryItem resourceEntryItem in Enumerable.OfType<ResourceEntryItem>((IEnumerable) container.ResourceItems))
          {
            if (typeId.IsAssignableFrom((ITypeId) resourceEntryItem.EffectiveTypeId))
            {
              DocumentNode valueNode = resourceEntryItem.Resource.ValueNode;
              DocumentNode keyOrImplicitKey = ResourceManager.GetResourceKeyOrImplicitKey(resourceEntryItem.Resource.ResourceNode, valueNode);
              if (keyOrImplicitKey != null)
              {
                resourcesAtCurrentScope.Add(resourceEntryItem.Resource.ResourceNode);
                uniqueResourcesAtCurrentScope[keyOrImplicitKey] = resourceEntryItem.Resource.ResourceNode;
              }
            }
          }
          foreach (DocumentCompositeNode entryNode in resourcesAtCurrentScope)
          {
            DocumentNode valueNode = entryNode.Properties[DictionaryEntryNode.ValueProperty];
            DocumentNode keyNode = ResourceManager.GetResourceKeyOrImplicitKey(entryNode, valueNode);
            DocumentCompositeNode uniqueValue = (DocumentCompositeNode) null;
            if (!uniqueResourcesOnly || uniqueResourcesAtCurrentScope.TryGetValue(keyNode, out uniqueValue) && uniqueValue == entryNode)
              yield return entryNode;
          }
        }
      }
      if (walkApplicationRoot)
      {
        foreach (IDocumentRoot documentRoot in applicationAndDesignTimeResources)
        {
          DocumentCompositeNode compositeRoot = documentRoot.RootNode as DocumentCompositeNode;
          if (compositeRoot != null && !alreadyVisited.Contains(compositeRoot))
          {
            ISupportsResources supportsResources = ResourceNodeHelper.GetResourcesCollection((DocumentNode) compositeRoot);
            if (supportsResources != null && supportsResources.Resources != null)
            {
              List<DocumentCompositeNode> resourcesAtCurrentScope = new List<DocumentCompositeNode>();
              Dictionary<DocumentNode, DocumentCompositeNode> uniqueResourcesAtCurrentScope = new Dictionary<DocumentNode, DocumentCompositeNode>((IEqualityComparer<DocumentNode>) DocumentNodeEqualityComparer.Instance);
              DocumentCompositeNode dictionaryNode = supportsResources.Resources;
              foreach (DocumentCompositeNode entryNode in this.GetResourceNodes(dictionaryNode, typeId, this.ActiveSceneViewModel.DocumentRootResolver, (IList<DocumentCompositeNode>) alreadyVisited))
              {
                DocumentNode valueNode = entryNode.Properties[DictionaryEntryNode.ValueProperty];
                DocumentNode keyOrImplicitKey = ResourceManager.GetResourceKeyOrImplicitKey(entryNode, valueNode);
                if (keyOrImplicitKey != null)
                {
                  resourcesAtCurrentScope.Add(entryNode);
                  uniqueResourcesAtCurrentScope[keyOrImplicitKey] = entryNode;
                }
              }
              foreach (DocumentCompositeNode entryNode in resourcesAtCurrentScope)
              {
                DocumentNode valueNode = entryNode.Properties[DictionaryEntryNode.ValueProperty];
                DocumentNode keyNode = ResourceManager.GetResourceKeyOrImplicitKey(entryNode, valueNode);
                DocumentCompositeNode uniqueValue = (DocumentCompositeNode) null;
                if (!uniqueResourcesOnly || uniqueResourcesAtCurrentScope.TryGetValue(keyNode, out uniqueValue) && uniqueValue == entryNode)
                  yield return entryNode;
              }
            }
          }
        }
      }
    }

    private IEnumerable<DocumentCompositeNode> GetResourceNodes(DocumentCompositeNode dictionaryNode, ITypeId filterTypeId, IDocumentRootResolver documentRootResolver, IList<DocumentCompositeNode> alreadyVisited)
    {
      if (!alreadyVisited.Contains(dictionaryNode))
      {
        alreadyVisited.Add(dictionaryNode);
        if (dictionaryNode.TypeResolver.ResolveProperty(ResourceDictionaryNode.MergedDictionariesProperty) != null)
        {
          DocumentCompositeNode mergedDictionariesNode = dictionaryNode.Properties[ResourceDictionaryNode.MergedDictionariesProperty] as DocumentCompositeNode;
          if (mergedDictionariesNode != null && mergedDictionariesNode.SupportsChildren)
          {
            foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) mergedDictionariesNode.Children)
            {
              DocumentCompositeNode childAsCompositeNode = documentNode as DocumentCompositeNode;
              if (childAsCompositeNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) childAsCompositeNode.Type))
              {
                Uri uri = childAsCompositeNode.GetUriValue(ResourceDictionaryNode.SourceProperty);
                if (uri != (Uri) null)
                {
                  uri = documentNode.Context.MakeDesignTimeUri(uri);
                  if (uri != (Uri) null)
                  {
                    IDocumentRoot root = (IDocumentRoot) null;
                    try
                    {
                      root = documentRootResolver.GetDocumentRoot(uri.OriginalString);
                    }
                    catch (IOException ex)
                    {
                    }
                    if (root != null && PlatformTypes.PlatformsCompatible(root.TypeResolver.PlatformMetadata, dictionaryNode.PlatformMetadata) && (root.RootNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) root.RootNode.Type)))
                    {
                      DocumentCompositeNode rootNode = root.RootNode as DocumentCompositeNode;
                      if (rootNode != null && PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) rootNode.Type))
                      {
                        foreach (DocumentCompositeNode documentCompositeNode in this.GetResourceNodes(rootNode, filterTypeId, documentRootResolver, alreadyVisited))
                          yield return documentCompositeNode;
                      }
                    }
                  }
                }
              }
            }
          }
        }
        if (dictionaryNode.SupportsChildren)
        {
          foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) dictionaryNode.Children)
          {
            DocumentCompositeNode compositeNode = documentNode as DocumentCompositeNode;
            if (compositeNode != null)
            {
              DocumentNode valueNode = compositeNode.Properties[DictionaryEntryNode.ValueProperty];
              if (filterTypeId.IsAssignableFrom((ITypeId) valueNode.Type))
                yield return compositeNode;
            }
          }
        }
      }
    }

    private class ResourceContainerModel
    {
      private string location;
      private ResourceManager.ResourceContainerModel parent;

      internal string Location
      {
        get
        {
          return this.location;
        }
      }

      internal ResourceManager.ResourceContainerModel Parent
      {
        get
        {
          return this.parent;
        }
      }

      internal ResourceContainerModel(string location, ResourceManager.ResourceContainerModel parent)
      {
        this.location = location;
        this.parent = parent;
      }
    }
  }
}
