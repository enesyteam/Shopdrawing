// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MaterialGroupNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MaterialGroupNode : MaterialNode
  {
    public static readonly IPropertyId ChildrenProperty = (IPropertyId) PlatformTypes.MaterialGroup.GetMember(MemberType.LocalProperty, "Children", MemberAccessTypes.Public);
    public static readonly MaterialGroupNode.ConcreteMaterialGroupNodeFactory Factory = new MaterialGroupNode.ConcreteMaterialGroupNodeFactory();

    public ISceneNodeCollection<MaterialNode> Children
    {
      get
      {
        return (ISceneNodeCollection<MaterialNode>) new SceneNode.SceneNodeCollection<MaterialNode>((SceneNode) this, MaterialGroupNode.ChildrenProperty);
      }
    }

    public class ConcreteMaterialGroupNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MaterialGroupNode();
      }
    }
  }
}
