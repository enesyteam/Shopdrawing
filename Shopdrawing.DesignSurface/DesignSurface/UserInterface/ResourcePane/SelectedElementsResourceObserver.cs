// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.SelectedElementsResourceObserver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Subscription;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public sealed class SelectedElementsResourceObserver : ResourceContainer
  {
    private Dictionary<ResourceItem, int> itemsCache = new Dictionary<ResourceItem, int>();
    private SearchStep descendantEnumerator = new SearchStep(SearchAxis.SceneDescendant);
    private ObservableCollectionWorkaround<ResourceItem> resourceItems = new ObservableCollectionWorkaround<ResourceItem>();
    private IComparer<ResourceItem> resourceItemComparer = (IComparer<ResourceItem>) new SelectedElementsResourceObserver.ResourceItemComparer();
    private DocumentNodeMarkerSortedListOf<DocumentNodePath> lastSelection = new DocumentNodeMarkerSortedListOf<DocumentNodePath>();
    private List<DocumentNode> oldNodesAsync = new List<DocumentNode>();
    private List<DocumentNode> newNodesAsync = new List<DocumentNode>();
    private XamlProjectSubscription subscription;
    private SceneViewModel lastViewModel;
    private SelectedElementsResourceObserver.UpdateState updateState;

    private SceneElementSelectionSet Selection
    {
      get
      {
        return this.lastViewModel.ElementSelectionSet;
      }
    }

    public override string Name
    {
      get
      {
        if (this.Selection.Count == 0)
          return StringTable.ResourcePaneNoElementsSelected;
        if (this.Selection.Count == 1)
          return this.Selection.PrimarySelection.DisplayName;
        return StringTable.ResourcePaneMultipleElementsSelected;
      }
    }

    public override ObservableCollection<ResourceItem> ResourceItems
    {
      get
      {
        return (ObservableCollection<ResourceItem>) this.resourceItems;
      }
    }

    public override SceneDocument Document
    {
      get
      {
        if (this.ResourceManager.ActiveSceneViewModel == null)
          return (SceneDocument) null;
        return this.ResourceManager.ActiveSceneViewModel.Document;
      }
    }

    private IList<ResourceEntryBase> ResourceEntries
    {
      get
      {
        List<ResourceEntryBase> list = new List<ResourceEntryBase>(this.ResourceItems.Count);
        foreach (ResourceItem resourceItem in (Collection<ResourceItem>) this.ResourceItems)
          list.Add((ResourceEntryBase) resourceItem);
        return (IList<ResourceEntryBase>) list;
      }
    }

    public ITypeId Type
    {
      get
      {
        if (this.Selection.Count == 1)
          return (ITypeId) this.Selection.PrimarySelection.Type;
        return PlatformTypes.Object;
      }
    }

    public string UniqueId
    {
      get
      {
        if (this.Selection.Count == 1)
        {
          BaseFrameworkElement frameworkElement = this.Selection.PrimarySelection as BaseFrameworkElement;
          if (frameworkElement != null)
            return frameworkElement.UniqueID;
        }
        return this.Name;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        return this.lastViewModel;
      }
    }

    public override ResourceDictionaryNode ResourceDictionaryNode
    {
      get
      {
        return (ResourceDictionaryNode) null;
      }
    }

    public override object ToolTip
    {
      get
      {
        return (object) this.Name;
      }
    }

    public override ISupportsResources ResourcesCollection
    {
      get
      {
        return (ISupportsResources) null;
      }
    }

    public override DocumentReference DocumentReference
    {
      get
      {
        return this.ViewModel.Document.DocumentReference;
      }
    }

    public override SceneNode Node
    {
      get
      {
        return (SceneNode) null;
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        if (this.ViewModel == null || this.ViewModel.RootNode == null)
          return (DocumentNode) null;
        return this.ViewModel.RootNode.DocumentNode;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        if (this.DocumentNode == null)
          return (DocumentNodeMarker) null;
        return this.DocumentNode.Marker;
      }
    }

    public SelectedElementsResourceObserver(ResourceManager manager)
      : base(manager)
    {
    }

    public void Update(SceneViewModel targetViewModel, SceneViewModel.ViewStateBits viewStateBits)
    {
      bool flag1 = (viewStateBits & SceneViewModel.ViewStateBits.ElementSelection) != SceneViewModel.ViewStateBits.None;
      if (targetViewModel != null && !targetViewModel.Document.IsEditable)
        targetViewModel = (SceneViewModel) null;
      if (targetViewModel != this.lastViewModel)
      {
        if (this.subscription == null || targetViewModel == null || this.subscription.ProjectContext != targetViewModel.ProjectContext)
        {
          if (this.subscription != null)
          {
            this.subscription.PathNodeInserted -= new XamlProjectSubscription.DocumentNodeInsertedHandler(this.Subscription_PathNodeInserted);
            this.subscription.PathNodeRemoved -= new XamlProjectSubscription.DocumentNodeRemovedHandler(this.Subscription_PathNodeRemoved);
            this.subscription.CatastrophicUpdate -= new EventHandler(this.Subscription_Refresh);
            this.subscription.Dispose();
            this.subscription = (XamlProjectSubscription) null;
          }
          if (targetViewModel != null)
          {
            this.subscription = new XamlProjectSubscription(this.ResourceManager.DesignerContext, targetViewModel.ProjectContext, new SearchPath(new SearchStep[2]
            {
              new SearchStep((SearchAxis) new DelegateAxis(new DelegateAxis.EnumerationHandler(this.FilterScopeEnumerator), SearchScope.NodeTreeSelf)),
              new SearchStep((SearchAxis) new DelegateAxis(new DelegateAxis.EnumerationHandler(this.FilteredResourceEnumerator), SearchScope.NodeTreeDescendant))
            }), new XamlProjectSubscription.DocumentNodeFilter(this.FilteredDocumentNodeEnumerator));
            this.subscription.PathNodeInserted += new XamlProjectSubscription.DocumentNodeInsertedHandler(this.Subscription_PathNodeInserted);
            this.subscription.PathNodeRemoved += new XamlProjectSubscription.DocumentNodeRemovedHandler(this.Subscription_PathNodeRemoved);
            this.subscription.CatastrophicUpdate += new EventHandler(this.Subscription_Refresh);
          }
        }
        else
          this.subscription.Clear();
        this.ClearItems();
        this.lastViewModel = targetViewModel;
        this.lastSelection.Clear();
        flag1 = true;
      }
      if (this.lastViewModel == null || !flag1)
        return;
      DocumentNodeMarkerSortedListOf<DocumentNodePath> newBasisNodes = new DocumentNodeMarkerSortedListOf<DocumentNodePath>(this.Selection.Count);
      foreach (SceneNode sceneNode in this.Selection.Selection)
      {
        DocumentNodeMarker marker = sceneNode.DocumentNode.Marker;
        DocumentNodePath documentNodePath = this.lastSelection.Find(marker) ?? sceneNode.DocumentNodePath;
        newBasisNodes.Add(marker, documentNodePath);
      }
      bool flag2 = true;
      foreach (DocumentNodeMarker marker in this.lastSelection.Markers)
      {
        if (newBasisNodes.Find(marker) == null)
        {
          flag2 = false;
          break;
        }
      }
      if (!flag2)
      {
        this.subscription.Clear();
        this.ClearItems();
      }
      this.subscription.SetBasisNodes(newBasisNodes);
      this.lastSelection = newBasisNodes;
      this.subscription.Update();
      this.ResourceManager.SelectedItems.RemoveSelection((ResourceEntryBase) this);
      this.OnPropertyChanged("Name");
      this.OnPropertyChanged("Type");
      this.OnPropertyChanged("UniqueId");
      this.OnPropertyChanged("ToolTip");
    }

    public override void EnsureResourceDictionaryNode()
    {
    }

    private void AddRefItem(ResourceItem item)
    {
      if (!this.itemsCache.ContainsKey(item))
      {
        this.itemsCache.Add(item, 1);
        int num = this.resourceItems.BinarySearch(item, this.resourceItemComparer);
        if (num >= 0)
          return;
        this.resourceItems.Insert(~num, item);
      }
      else
        this.itemsCache[item] = this.itemsCache[item] + 1;
    }

    private bool RemoveItem(ResourceItem item)
    {
      if (!this.itemsCache.ContainsKey(item))
        return false;
      if (this.itemsCache[item] == 1)
      {
        this.itemsCache.Remove(item);
        this.ResourceItems.Remove(item);
        this.ResourceManager.SelectedItems.RemoveSelection((ResourceEntryBase) item);
      }
      else
        this.itemsCache[item] = this.itemsCache[item] - 1;
      return true;
    }

    private void ClearItems()
    {
      this.itemsCache.Clear();
      this.ResourceManager.SelectedItems.RemoveSelection((ICollection<ResourceEntryBase>) this.ResourceEntries);
      this.ResourceItems.Clear();
    }

    private void Subscription_Refresh(object sender, EventArgs e)
    {
      this.ClearItems();
      this.EnsureAsyncUpdate(SelectedElementsResourceObserver.UpdateState.Catastrophic);
    }

    private void Subscription_PathNodeRemoved(IXamlSubscription sender, DocumentNode oldNode, DocumentNodePath watchedRoot)
    {
      if (!(oldNode.TargetType == typeof (DictionaryEntry)))
        return;
      this.oldNodesAsync.Add(oldNode);
      this.EnsureAsyncUpdate(SelectedElementsResourceObserver.UpdateState.Incremental);
    }

    private void Subscription_PathNodeInserted(IXamlSubscription sender, DocumentNode newNode, DocumentNodePath watchedRoot)
    {
      if (!(newNode.TargetType == typeof (DictionaryEntry)))
        return;
      this.newNodesAsync.Add(newNode);
      this.EnsureAsyncUpdate(SelectedElementsResourceObserver.UpdateState.Incremental);
    }

    private void EnsureAsyncUpdate(SelectedElementsResourceObserver.UpdateState updateState)
    {
      if (this.updateState >= updateState)
        return;
      if (this.updateState == SelectedElementsResourceObserver.UpdateState.None)
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.Send, new Action(this.UpdateAsync));
      this.updateState = updateState;
    }

    private void UpdateAsync()
    {
      SelectedElementsResourceObserver.UpdateState updateState = this.updateState;
      this.updateState = SelectedElementsResourceObserver.UpdateState.None;
      List<DocumentNode> list1 = this.oldNodesAsync;
      List<DocumentNode> list2 = this.newNodesAsync;
      this.oldNodesAsync = new List<DocumentNode>();
      this.newNodesAsync = new List<DocumentNode>();
      if (updateState == SelectedElementsResourceObserver.UpdateState.Catastrophic)
        this.Update(this.ViewModel, SceneViewModel.ViewStateBits.EntireScene);
      foreach (DocumentNode documentNode in list2)
      {
        foreach (ResourceContainer resourceContainer in this.ResourceManager.ActiveSceneViewModel == null || documentNode.DocumentRoot != this.ResourceManager.ActiveSceneViewModel.DocumentRoot ? (IEnumerable) this.ResourceManager.DocumentResourceContainers : (IEnumerable) this.ResourceManager.LocalResourceContainers)
        {
          if (resourceContainer.DocumentNode != null && ResourceNodeHelper.GetResourcesCollection(resourceContainer.DocumentNode).Resources == documentNode.Parent)
          {
            foreach (ResourceItem resourceItem in (Collection<ResourceItem>) resourceContainer.ResourceItems)
            {
              if (resourceItem.DocumentNode == documentNode)
              {
                this.AddRefItem(resourceItem);
                break;
              }
            }
          }
        }
      }
      foreach (DocumentNode documentNode in list1)
      {
        foreach (ResourceItem resourceItem in (Collection<ResourceItem>) this.ResourceItems)
        {
          if (resourceItem.DocumentNode == documentNode)
          {
            this.RemoveItem(resourceItem);
            break;
          }
        }
      }
    }

    private IEnumerable<SceneNode> FilteredResourceEnumerator(SceneNode pivot)
    {
      if (DocumentNodeUtilities.IsStaticResource(pivot.DocumentNode) || DocumentNodeUtilities.IsDynamicResource(pivot.DocumentNode))
      {
        yield return pivot;
      }
      else
      {
        DocumentNode parent = (DocumentNode) pivot.DocumentNode.Parent;
        if (parent != null && parent.TargetType == typeof (DictionaryEntry))
          yield return pivot.ViewModel.GetSceneNode(parent);
      }
    }

    private IEnumerable<DocumentNode> FilteredDocumentNodeEnumerator(DocumentNode pivot)
    {
      if (pivot.Parent != null && pivot.Parent.TargetType == typeof (DictionaryEntry))
        yield return (DocumentNode) pivot.Parent;
    }

    private IEnumerable<SceneNode> FilterScopeEnumerator(SceneNode pivot)
    {
      SceneElement pivotElement = pivot as SceneElement;
      bool ignoreContent = pivotElement != null && pivot.ViewModel == this.lastViewModel && this.lastViewModel.ElementSelectionSet.IsSelected(pivotElement);
      yield return pivot;
      DocumentCompositeNode compositeNode = pivot.DocumentNode as DocumentCompositeNode;
      if (compositeNode != null)
      {
        foreach (IPropertyId propertyKey in (IEnumerable<IProperty>) compositeNode.Properties.Keys)
        {
          if (!ignoreContent || !pivot.ContentProperties.Contains(propertyKey))
          {
            SceneNode value = pivot.GetLocalValueAsSceneNode(propertyKey);
            if (value == null)
            {
              DocumentNode nodeValue = ((DocumentCompositeNode) pivot.DocumentNode).Properties[propertyKey];
              if (nodeValue != null)
                yield return pivot.ViewModel.GetSceneNode(nodeValue);
            }
            else
            {
              yield return value;
              foreach (SceneNode sceneNode in this.descendantEnumerator.Query(value))
                yield return sceneNode;
            }
          }
        }
        if (compositeNode.SupportsChildren)
        {
          ISceneNodeCollection<SceneNode> children = pivot.GetChildren();
          for (int i = 0; i < children.Count; ++i)
          {
            yield return children[i];
            foreach (SceneNode sceneNode in this.descendantEnumerator.Query(children[i]))
              yield return sceneNode;
          }
        }
      }
    }

    private enum UpdateState
    {
      None,
      Incremental,
      Catastrophic,
    }

    private class ResourceItemComparer : IComparer<ResourceItem>
    {
      private IComparer<ResourceEntryBase> comparer;

      public ResourceItemComparer()
      {
        this.comparer = (IComparer<ResourceEntryBase>) new ResourceEntryBase.Comparer();
      }

      public int Compare(ResourceItem a, ResourceItem b)
      {
        ResourceEntryItem resourceEntryItem1 = a as ResourceEntryItem;
        ResourceEntryItem resourceEntryItem2 = b as ResourceEntryItem;
        if (resourceEntryItem1 != null && resourceEntryItem2 != null)
        {
          int num = string.Compare(resourceEntryItem1.Key, resourceEntryItem2.Key, StringComparison.Ordinal);
          if (num != 0)
            return num;
        }
        return this.comparer.Compare((ResourceEntryBase) a, (ResourceEntryBase) b);
      }
    }
  }
}
