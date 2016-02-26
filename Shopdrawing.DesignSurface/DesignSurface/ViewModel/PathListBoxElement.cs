// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PathListBoxElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PathListBoxElement : ItemsControlElement
  {
    public static readonly IPropertyId LayoutPathsProperty = (IPropertyId) ProjectNeutralTypes.PathListBox.GetMember(MemberType.LocalProperty, "LayoutPaths", MemberAccessTypes.Public);
    public static readonly IPropertyId StartItemIndexProperty = (IPropertyId) ProjectNeutralTypes.PathListBox.GetMember(MemberType.LocalProperty, "StartItemIndex", MemberAccessTypes.Public);
    public static readonly IPropertyId WrapItemsProperty = (IPropertyId) ProjectNeutralTypes.PathListBox.GetMember(MemberType.LocalProperty, "WrapItems", MemberAccessTypes.Public);
    public static readonly PathListBoxElement.ConcretePathListBoxElementFactory Factory = new PathListBoxElement.ConcretePathListBoxElementFactory();

    public ISceneNodeCollection<SceneNode> LayoutPaths
    {
      get
      {
        return this.GetCollectionForProperty(PathListBoxElement.LayoutPathsProperty);
      }
    }

    public class ConcretePathListBoxElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PathListBoxElement();
      }
    }
  }
}
