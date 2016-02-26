// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ObjectDataProviderSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ObjectDataProviderSceneNode : DataSourceProviderSceneNode
  {
    public static readonly IPropertyId ObjectTypeProperty = (IPropertyId) PlatformTypes.ObjectDataProvider.GetMember(MemberType.LocalProperty, "ObjectType", MemberAccessTypes.Public);
    public static readonly IPropertyId ObjectInstanceProperty = (IPropertyId) PlatformTypes.ObjectDataProvider.GetMember(MemberType.LocalProperty, "ObjectInstance", MemberAccessTypes.Public);
    public static readonly ObjectDataProviderSceneNode.ConcreteObjectDataProviderSceneNodeFactory Factory = new ObjectDataProviderSceneNode.ConcreteObjectDataProviderSceneNodeFactory();

    public Type ObjectType
    {
      get
      {
        return (Type) this.GetLocalValue(ObjectDataProviderSceneNode.ObjectTypeProperty);
      }
      set
      {
        this.SetLocalValue(ObjectDataProviderSceneNode.ObjectTypeProperty, (object) value);
      }
    }

    public class ConcreteObjectDataProviderSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ObjectDataProviderSceneNode();
      }

      public ObjectDataProviderSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (ObjectDataProviderSceneNode) this.Instantiate(viewModel, PlatformTypes.ObjectDataProvider);
      }
    }
  }
}
