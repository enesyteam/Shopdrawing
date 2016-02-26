// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceDictionaryContentProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public class ResourceDictionaryContentProvider
  {
    private XamlDocument resourceDictionary;
    private IProjectItem projectItem;
    private DesignerContext designerContext;
    private SceneView view;
    private SceneNodeSubscription<object, object> resourceSubscription;
    private bool needsRebuild;

    public virtual XamlDocument Document
    {
      get
      {
        return this.resourceDictionary;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        if (this.view == null)
          return (SceneViewModel) null;
        return this.view.ViewModel;
      }
    }

    public IProjectItem ProjectItem
    {
      get
      {
        return this.projectItem;
      }
      internal set
      {
        this.CloseInternal(false);
        this.projectItem = value;
        if (this.projectItem != null)
        {
          if (!this.projectItem.IsOpen)
            this.projectItem.OpenDocument(false, true);
          if (this.projectItem.IsOpen)
          {
            SceneDocument sceneDocument = this.projectItem.Document as SceneDocument;
            this.resourceDictionary = (XamlDocument) sceneDocument.XamlDocument;
            foreach (IView view in (IEnumerable<IView>) this.designerContext.ViewService.Views)
            {
              SceneView sceneView = view as SceneView;
              if (sceneView != null && sceneView.Document == sceneDocument)
              {
                this.View = sceneView;
                break;
              }
            }
          }
        }
        if (this.resourceDictionary != null && this.resourceDictionary.DocumentContext != null)
        {
          this.resourceDictionary.TypesChanged += new EventHandler(this.DocumentChanged);
          this.resourceDictionary.RootNodeChanged += new EventHandler(this.DocumentChanged);
        }
        this.OnItemsChanged();
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public SceneView View
    {
      get
      {
        return this.view;
      }
      set
      {
        if (this.ViewModel != null)
          this.ViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
        this.view = value;
        this.resourceSubscription.CurrentViewModel = this.ViewModel;
        if (this.ViewModel == null)
          return;
        this.ViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdatePhase);
        this.OnViewUpdated(new SceneUpdatePhaseEventArgs(this.ViewModel, true, false));
      }
    }

    public IEnumerable<DocumentNode> Items
    {
      get
      {
        if (this.Document != null && this.Document.IsEditable)
        {
          this.needsRebuild = false;
          DocumentNode rootNode = this.Document.RootNode;
          ISupportsResources supportsResources = ResourceNodeHelper.GetResourcesCollection(rootNode);
          if (supportsResources != null && supportsResources.Resources != null)
          {
            if (supportsResources.Resources.TypeResolver.ResolveProperty(ResourceDictionaryNode.MergedDictionariesProperty) != null)
            {
              DocumentCompositeNode mergedDictionaries = supportsResources.Resources.Properties[ResourceDictionaryNode.MergedDictionariesProperty] as DocumentCompositeNode;
              if (mergedDictionaries != null && mergedDictionaries.SupportsChildren)
              {
                foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) mergedDictionaries.Children)
                {
                  if (PlatformTypes.ResourceDictionary.IsAssignableFrom((ITypeId) documentNode.Type))
                    yield return documentNode;
                }
              }
            }
            if (supportsResources.Resources.SupportsChildren)
            {
              foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) supportsResources.Resources.Children)
              {
                if (PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) documentNode.Type))
                  yield return documentNode;
              }
            }
          }
        }
      }
    }

    public event CollectionChangeEventHandler ItemsChanged;

    public event EventHandler<EventArgs<DocumentNode>> KeyChanged;

    internal ResourceDictionaryContentProvider(DesignerContext designerContext, IProjectItem projectItem)
    {
      this.InitCommon(designerContext);
      this.ProjectItem = projectItem;
    }

    internal ResourceDictionaryContentProvider(DesignerContext designerContext, XamlDocument resourceDictionary)
    {
      this.InitCommon(designerContext);
      this.resourceDictionary = resourceDictionary;
    }

    private void InitCommon(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.resourceSubscription = new SceneNodeSubscription<object, object>();
      this.resourceSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep((SearchAxis) new DelegateAxis(new DelegateAxis.EnumerationHandler(ResourceDictionaryContentProvider.ResourceEntryEnumerator), SearchScope.NodeTreeSelf))
      });
      this.resourceSubscription.PathNodeInserted += new SceneNodeSubscription<object, object>.PathNodeInsertedListener(this.ResourcesSubscription_PathNodeInserted);
      this.resourceSubscription.PathNodeRemoved += new SceneNodeSubscription<object, object>.PathNodeRemovedListener(this.ResourcesSubscription_PathNodeRemoved);
      this.resourceSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.ResourcesSubscription_PathNodeChanged);
    }

    internal void Close()
    {
      this.CloseInternal(true);
    }

    private void CloseInternal(bool notifyItemsChanged)
    {
      this.View = (SceneView) null;
      if (this.resourceDictionary != null)
      {
        this.resourceDictionary.TypesChanged -= new EventHandler(this.DocumentChanged);
        this.resourceDictionary.RootNodeChanged -= new EventHandler(this.DocumentChanged);
        this.resourceDictionary = (XamlDocument) null;
      }
      if (!notifyItemsChanged)
        return;
      this.OnItemsChanged();
    }

    protected void Refresh()
    {
      this.OnItemsChanged();
    }

    internal bool EnsureLinked(SceneViewModel sceneViewModel)
    {
      return this.EnsureLinked(sceneViewModel, (ResourceContainer) null);
    }

    internal bool EnsureLinked(SceneViewModel sceneViewModel, ResourceContainer targetContainer)
    {
      sceneViewModel.Document.OnUpdatedEditTransaction();
      DocumentReference documentReference = this.projectItem.DocumentReference;
      Uri uri = new Uri(documentReference.Path, UriKind.RelativeOrAbsolute);
      ResourceManager resourceManager = this.designerContext.ResourceManager;
      ResourceContainer activeRootContainer = resourceManager.ActiveRootContainer;
      if (activeRootContainer == null)
        return false;
      List<ResourceDictionaryItem> list = new List<ResourceDictionaryItem>();
      resourceManager.FindAllReachableDictionaries(activeRootContainer, (ICollection<ResourceDictionaryItem>) list);
      if (resourceManager.TopLevelResourceContainer != null && resourceManager.TopLevelResourceContainer != activeRootContainer)
        resourceManager.FindAllReachableDictionaries(resourceManager.TopLevelResourceContainer, (ICollection<ResourceDictionaryItem>) list);
      foreach (ResourceDictionaryItem resourceDictionaryItem in list)
      {
        if (object.Equals((object) resourceDictionaryItem.SourceUri, (object) uri) || object.Equals((object) resourceDictionaryItem.DesignTimeSourceUri, (object) uri))
          return true;
      }
      if (documentReference != resourceManager.ActiveRootContainer.DocumentReference && documentReference != resourceManager.TopLevelResourceContainer.DocumentReference)
        resourceManager.LinkToResource(targetContainer ?? resourceManager.TopLevelResourceContainer, documentReference);
      return true;
    }

    private void OnItemsChanged()
    {
      if (this.ItemsChanged == null)
        return;
      this.ItemsChanged((object) this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, (object) this));
    }

    private void OnKeyChanged(DocumentNode entry)
    {
      if (this.KeyChanged == null)
        return;
      this.KeyChanged((object) this, new EventArgs<DocumentNode>(entry));
    }

    private void ResourcesSubscription_PathNodeInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode, object newContent)
    {
      if (!(newPathNode is DictionaryEntryNode) && !(newPathNode is ResourceDictionaryNode))
        return;
      this.needsRebuild = true;
    }

    private void ResourcesSubscription_PathNodeRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, object oldContent)
    {
      if (!(oldPathNode is DictionaryEntryNode) && !(oldPathNode is ResourceDictionaryNode))
        return;
      this.needsRebuild = true;
    }

    private void ResourcesSubscription_PathNodeChanged(object sender, SceneNode pathNode, object item, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (!damage.IsPropertyChange)
        return;
      DictionaryEntryNode dictionaryEntryNode = pathNode as DictionaryEntryNode;
      if (dictionaryEntryNode == null)
        return;
      bool flag = DictionaryEntryNode.KeyProperty.Equals((object) damage.PropertyKey);
      if (!flag && damage.ParentNode != null && (damage.ParentNode == dictionaryEntryNode.Value.DocumentNode && this.ViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey)) && damage.PropertyKey == damage.ParentNode.NameProperty)
        flag = true;
      if (!flag)
        return;
      this.OnKeyChanged(pathNode.DocumentNode);
    }

    private void ViewModel_LateSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.OnViewUpdated(args);
    }

    private void DocumentChanged(object sender, EventArgs e)
    {
      this.OnItemsChanged();
    }

    private void OnViewUpdated(SceneUpdatePhaseEventArgs args)
    {
      if (this.view != null && this.Document != null && this.Document.IsEditable)
      {
        ISupportsResources resourcesCollection = ResourceNodeHelper.GetResourcesCollection(this.Document.RootNode);
        if (resourcesCollection != null)
        {
          DocumentNode node = (DocumentNode) resourcesCollection.Resources;
          if (node != null)
          {
            this.resourceSubscription.SetBasisNodes(this.ViewModel, (IEnumerable<SceneNode>) new List<SceneNode>()
            {
              this.ViewModel.GetSceneNode(node)
            });
            this.resourceSubscription.Update(this.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
          }
        }
      }
      else
      {
        this.needsRebuild = true;
        this.resourceSubscription.SetBasisNodes(this.ViewModel, (IEnumerable<SceneNode>) new List<SceneNode>());
      }
      if (!this.needsRebuild)
        return;
      this.OnItemsChanged();
    }

    private static IEnumerable<SceneNode> ResourceEntryEnumerator(SceneNode target)
    {
      ResourceDictionaryNode resourceDictionaryNode = target as ResourceDictionaryNode;
      if (resourceDictionaryNode != null)
      {
        foreach (ResourceDictionaryNode resourceDictionaryNode1 in (IEnumerable<ResourceDictionaryNode>) resourceDictionaryNode.MergedDictionaries)
          yield return (SceneNode) resourceDictionaryNode1;
        foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) target.GetChildren())
          yield return sceneNode;
      }
    }
  }
}
