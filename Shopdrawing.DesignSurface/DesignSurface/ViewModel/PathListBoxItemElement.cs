// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.PathListBoxItemElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class PathListBoxItemElement : ContentControlElement
  {
    public static readonly IPropertyId LayoutPathIndexProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "LayoutPathIndex", MemberAccessTypes.Public);
    public static readonly IPropertyId GlobalChildIndexProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "GlobalChildIndex", MemberAccessTypes.Public);
    public static readonly IPropertyId LocalChildIndexProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "LocalChildIndex", MemberAccessTypes.Public);
    public static readonly IPropertyId GlobalChildOffsetProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "GlobalChildOffset", MemberAccessTypes.Public);
    public static readonly IPropertyId LocalChildOffsetProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "LocalChildOffset", MemberAccessTypes.Public);
    public static readonly IPropertyId NormalAngleProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "NormalAngle", MemberAccessTypes.Public);
    public static readonly IPropertyId OrientationAngleProperty = (IPropertyId) ProjectNeutralTypes.PathListBoxItem.GetMember(MemberType.LocalProperty, "OrientationAngle", MemberAccessTypes.Public);
    public static readonly PathListBoxItemElement.ConcretePathListBoxItemElementFactory Factory = new PathListBoxItemElement.ConcretePathListBoxItemElementFactory();

    public class ConcretePathListBoxItemElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new PathListBoxItemElement();
      }
    }
  }
}
