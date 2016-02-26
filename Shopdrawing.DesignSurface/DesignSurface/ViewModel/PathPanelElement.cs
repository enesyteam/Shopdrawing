// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PathPanelElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PathPanelElement : PanelElement
  {
    public static readonly IPropertyId LayoutPathsProperty = (IPropertyId) ProjectNeutralTypes.PathPanel.GetMember(MemberType.LocalProperty, "LayoutPaths", MemberAccessTypes.Public);
    public static readonly IPropertyId StartItemIndexProperty = (IPropertyId) ProjectNeutralTypes.PathPanel.GetMember(MemberType.LocalProperty, "StartItemIndex", MemberAccessTypes.Public);
    public static readonly IPropertyId WrapItemsProperty = (IPropertyId) ProjectNeutralTypes.PathPanel.GetMember(MemberType.LocalProperty, "WrapItems", MemberAccessTypes.Public);
    public static readonly PathPanelElement.ConcretePathPanelElementFactory Factory = new PathPanelElement.ConcretePathPanelElementFactory();

    public ISceneNodeCollection<SceneNode> LayoutPaths
    {
      get
      {
        return this.GetCollectionForProperty(PathPanelElement.LayoutPathsProperty);
      }
    }

    public class ConcretePathPanelElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PathPanelElement();
      }
    }
  }
}
