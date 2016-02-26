// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ApplicationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ApplicationSceneNode : SceneNode, IResourceContainer
  {
    public static readonly ApplicationSceneNode.ConcreteApplicationSceneNodeFactory Factory = new ApplicationSceneNode.ConcreteApplicationSceneNodeFactory();

    public ResourceDictionaryNode Resources
    {
      get
      {
        return ResourceManager.ProvideResourcesForElement((SceneNode) this);
      }
      set
      {
        this.SetValueAsSceneNode(this.Type.Metadata.ResourcesProperty, (SceneNode) value);
      }
    }

    public bool AreResourcesSupported
    {
      get
      {
        return this.Platform.Metadata.ResolveProperty(ApplicationMetadata.ResourcesProperty) != null;
      }
    }

    public class ConcreteApplicationSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ApplicationSceneNode();
      }
    }
  }
}
