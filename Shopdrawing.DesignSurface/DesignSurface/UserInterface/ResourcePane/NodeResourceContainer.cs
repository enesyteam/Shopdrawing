// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.NodeResourceContainer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  public sealed class NodeResourceContainer : ResourceContainer
  {
    private SceneNode node;
    private DocumentNodeMarker marker;

    public override string Name
    {
      get
      {
        return this.node.DisplayName;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        return this.node.ViewModel;
      }
    }

    public override ResourceDictionaryNode ResourceDictionaryNode
    {
      get
      {
        return ResourceManager.ProvideResourcesForElement(this.node);
      }
    }

    public override object ToolTip
    {
      get
      {
        return (object) this.node.TargetType.Name;
      }
    }

    public string UniqueId
    {
      get
      {
        return this.node.UniqueID;
      }
    }

    public override ISupportsResources ResourcesCollection
    {
      get
      {
        return ResourceNodeHelper.GetResourcesCollection(this.node.DocumentNode);
      }
    }

    public override SceneDocument Document
    {
      get
      {
        return this.ViewModel.Document;
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
        return this.node;
      }
    }

    public override DocumentNode DocumentNode
    {
      get
      {
        return this.node.DocumentNode;
      }
    }

    public override DocumentNodeMarker Marker
    {
      get
      {
        return this.marker;
      }
    }

    public NodeResourceContainer(ResourceManager manager, SceneNode node)
      : base(manager)
    {
      this.node = node;
      this.marker = node.DocumentNode.Marker;
    }

    public override void EnsureResourceDictionaryNode()
    {
      if (this.ResourceDictionaryNode != null)
        return;
      IPropertyId resourcesProperty = this.node.Type.Metadata.ResourcesProperty;
      if (resourcesProperty == null)
        return;
      ResourceDictionaryNode resourceDictionaryNode = ResourceDictionaryNode.Factory.Instantiate(this.ViewModel);
      this.node.SetValueAsSceneNode(resourcesProperty, (SceneNode) resourceDictionaryNode);
    }
  }
}
