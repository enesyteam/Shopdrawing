// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.LayoutPathNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class LayoutPathNode : SceneNode
  {
    public static readonly IPropertyId SourceElementProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "SourceElement", MemberAccessTypes.Public);
    public static readonly IPropertyId DistributionProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Distribution", MemberAccessTypes.Public);
    public static readonly IPropertyId CapacityProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Capacity", MemberAccessTypes.Public);
    public static readonly IPropertyId ActualCapacityProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "ActualCapacity", MemberAccessTypes.Public);
    public static readonly IPropertyId PaddingProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Padding", MemberAccessTypes.Public);
    public static readonly IPropertyId OrientationProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Orientation", MemberAccessTypes.Public);
    public static readonly IPropertyId StartProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Start", MemberAccessTypes.Public);
    public static readonly IPropertyId SpanProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "Span", MemberAccessTypes.Public);
    public static readonly IPropertyId IsValidProperty = (IPropertyId) ProjectNeutralTypes.LayoutPath.GetMember(MemberType.LocalProperty, "IsValid", MemberAccessTypes.Public);
    public static readonly LayoutPathNode.ConcreteLayoutPathNodeFactory Factory = new LayoutPathNode.ConcreteLayoutPathNodeFactory();

    public class ConcreteLayoutPathNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new LayoutPathNode();
      }
    }
  }
}
