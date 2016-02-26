// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.CompositeShapeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class CompositeShapeElement : ControlElement
  {
    public static readonly IPropertyId StrokeProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "Stroke", MemberAccessTypes.Public);
    public static readonly IPropertyId FillProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "Fill", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeThicknessProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeThickness", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeMiterLimitProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeMiterLimit", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeEndLineCapProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeEndLineCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeStartLineCapProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeStartLineCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeLineJoinProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeLineJoin", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashCapProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeDashCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StretchProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "Stretch", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashOffsetProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeDashOffset", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashArrayProperty = (IPropertyId) ProjectNeutralTypes.CompositeShape.GetMember(MemberType.LocalProperty, "StrokeDashArray", MemberAccessTypes.Public);
    public new static readonly SceneNode.ConcreteSceneNodeFactory Factory = (SceneNode.ConcreteSceneNodeFactory) new CompositeShapeElement.CompositeShapeElementFactory();

    private class CompositeShapeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new CompositeShapeElement();
      }
    }
  }
}
