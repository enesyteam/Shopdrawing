// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ShapeElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ShapeElement : BaseFrameworkElement
  {
    public static readonly IPropertyId StrokeProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "Stroke", MemberAccessTypes.Public);
    public static readonly IPropertyId FillProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "Fill", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeThicknessProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeThickness", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeMiterLimitProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeMiterLimit", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeEndLineCapProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeEndLineCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeStartLineCapProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeStartLineCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeLineJoinProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeLineJoin", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashCapProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeDashCap", MemberAccessTypes.Public);
    public static readonly IPropertyId StretchProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "Stretch", MemberAccessTypes.Public);
    public static readonly IPropertyId GeometryTransformProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "GeometryTransform", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashOffsetProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeDashOffset", MemberAccessTypes.Public);
    public static readonly IPropertyId StrokeDashArrayProperty = (IPropertyId) PlatformTypes.Shape.GetMember(MemberType.LocalProperty, "StrokeDashArray", MemberAccessTypes.Public);
    public static readonly ShapeElement.ShapeElementFactory Factory = new ShapeElement.ShapeElementFactory();

    public class ShapeElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ShapeElement();
      }
    }
  }
}
