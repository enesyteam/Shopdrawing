// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.DocumentResourceContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public sealed class DocumentResourceContainer : ResourceContainer
  {
    private ResourceDictionaryContentProvider contentProvider;

    public override IDocumentContext DocumentContext
    {
      get
      {
        if (this.contentProvider.Document != null)
          return this.contentProvider.Document.DocumentContext;
        return (IDocumentContext) null;
      }
    }

    public override string Name
    {
      get
      {
        if (this.contentProvider.ProjectItem.ContainsDesignTimeResources)
          return StringTable.DesignTimeResourcesContainerName;
        return this.contentProvider.ProjectItem.DocumentReference.DisplayName;
      }
    }

    public override object ToolTip
    {
      get
      {
        return (object) this.ProjectItem.DocumentReference.Path;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        this.EnsureEditable();
        return this.contentProvider.ViewModel;
      }
    }

    public override SceneDocument Document
    {
      get
      {
        if (this.ProjectItem.Document == null && this.ProjectItem.FileExists)
          this.ProjectItem.OpenDocument(false);
        return this.ProjectItem.Document as SceneDocument;
      }
    }

    public override DocumentReference DocumentReference
    {
      get
      {
        return this.ProjectItem.DocumentReference;
      }
    }

    public IProjectItem ProjectItem
    {
      get
      {
        return this.contentProvider.ProjectItem;
      }
    }

    public override ResourceDictionaryNode ResourceDictionaryNode
    {
      get
      {
        ResourceDictionaryNode resourceDictionaryNode = (ResourceDictionaryNode) null;
        if (this.ViewModel != null)
        {
          resourceDictionaryNode = this.ViewModel.RootNode as ResourceDictionaryNode;
          ApplicationSceneNode applicationSceneNode = this.ViewModel.RootNode as ApplicationSceneNode;
          if (applicationSceneNode != null)
            resourceDictionaryNode = applicationSceneNode.Resources;
        }
        return resourceDictionaryNode;
      }
    }

    public override SceneNode Node
    {
      get
      {
        return this.ViewModel.RootNode;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        return this.DocumentNode.Marker;
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        if (this.IsEditable)
          return this.Node.DocumentNode;
        if (this.contentProvider.Document != null)
          return this.contentProvider.Document.RootNode;
        return (DocumentNode) null;
      }
    }

    public override ISupportsResources ResourcesCollection
    {
      get
      {
        return ResourceNodeHelper.GetResourcesCollection(this.contentProvider.Document.RootNode);
      }
    }

    public override bool IsEditable
    {
      get
      {
        if (this.contentProvider.Document != null && this.contentProvider.Document.IsEditable)
          return this.contentProvider.View != null;
        return false;
      }
    }

    public DocumentResourceContainer(ResourceManager manager, IProjectItem item)
      : base(manager)
    {
      this.contentProvider = manager.GetContentProviderForResourceDictionary(item);
      this.contentProvider.ItemsChanged += new CollectionChangeEventHandler(this.ContentProvider_ResourcesChanged);
      this.contentProvider.KeyChanged += new EventHandler<EventArgs<DocumentNode>>(this.ContentProvider_KeyChanged);
      this.Refresh();
    }

    public override void EnsureEditable()
    {
      base.EnsureEditable();
      if (this.contentProvider.Document == null)
        this.contentProvider.ProjectItem.OpenDocument(false);
      if (this.contentProvider.View != null || this.contentProvider.Document == null)
        return;
      ISceneViewHost sceneViewHost = (ISceneViewHost) this.ProjectContext.GetService(typeof (ISceneViewHost));
      if (sceneViewHost == null)
        return;
      this.contentProvider.View = sceneViewHost.OpenView((IDocumentRoot) this.contentProvider.Document, false);
    }

    public override void EnsureResourceDictionaryNode()
    {
      ApplicationSceneNode applicationSceneNode = this.ViewModel.RootNode as ApplicationSceneNode;
      if (applicationSceneNode == null || applicationSceneNode.Resources != null)
        return;
      ResourceDictionaryNode resourceDictionaryNode = ResourceDictionaryNode.Factory.Instantiate(this.ViewModel);
      applicationSceneNode.Resources = resourceDictionaryNode;
    }

    private void ContentProvider_ResourcesChanged(object sender, CollectionChangeEventArgs e)
    {
      this.Refresh();
    }

    private void ContentProvider_KeyChanged(object sender, EventArgs<DocumentNode> e)
    {
      foreach (ResourceItem resourceItem in (Collection<ResourceItem>) this.ResourceItems)
      {
        if (resourceItem.DocumentNode == e.Value)
        {
          ResourceEntryItem resourceEntryItem = resourceItem as ResourceEntryItem;
          if (resourceEntryItem != null)
          {
            resourceEntryItem.OnKeyChanged();
            break;
          }
        }
      }
    }

    private void Refresh()
    {
      List<DocumentNode> list = new List<DocumentNode>(this.contentProvider.Items);
      list.RemoveAll((Predicate<DocumentNode>) (target =>
      {
        DocumentCompositeNode documentCompositeNode1 = target as DocumentCompositeNode;
        if (documentCompositeNode1 == null || !PlatformTypes.DictionaryEntry.Equals((object) documentCompositeNode1.Type))
          return false;
        DocumentNode documentNode = documentCompositeNode1.Properties[DictionaryEntryNode.ValueProperty];
        DocumentCompositeNode documentCompositeNode2 = documentNode as DocumentCompositeNode;
        if (documentNode == null || PlatformTypes.Storyboard.IsAssignableFrom((ITypeId) documentNode.Type))
          return true;
        if (documentCompositeNode2 != null)
          return documentCompositeNode2.GetValue<bool>(DesignTimeProperties.IsDataSourceProperty);
        return false;
      }));
      bool flag = this.ResourceItems.Count == list.Count;
      for (int index = 0; flag && index < list.Count; ++index)
        flag = flag && this.ResourceItems[index].DocumentNode == list[index];
      if (!flag)
      {
        foreach (ResourceItem oldItem in (Collection<ResourceItem>) this.ResourceItems)
          this.ResourceManager.OnItemRemoved(oldItem);
        this.ResourceItems.Clear();
        foreach (DocumentNode documentNode in list)
        {
          DocumentCompositeNode node = documentNode as DocumentCompositeNode;
          if (node != null)
            this.ResourceItems.Add(this.ResourceManager.GetResourceItem((ResourceContainer) this, node));
        }
      }
      this.OnPropertyChanged("DocumentHasErrors");
    }

    public override void Close()
    {
      this.contentProvider.ItemsChanged -= new CollectionChangeEventHandler(this.ContentProvider_ResourcesChanged);
      this.contentProvider.KeyChanged -= new EventHandler<EventArgs<DocumentNode>>(this.ContentProvider_KeyChanged);
      base.Close();
    }
  }
}
