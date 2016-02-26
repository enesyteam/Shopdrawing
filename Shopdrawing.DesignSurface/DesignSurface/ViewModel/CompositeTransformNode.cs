// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.CompositeTransformNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class CompositeTransformNode : TransformNode
  {
    public static readonly IPropertyId RotationProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "Rotation", MemberAccessTypes.Public);
    public static readonly IPropertyId ScaleXProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "ScaleX", MemberAccessTypes.Public);
    public static readonly IPropertyId ScaleYProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "ScaleY", MemberAccessTypes.Public);
    public static readonly IPropertyId SkewXProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "SkewX", MemberAccessTypes.Public);
    public static readonly IPropertyId SkewYProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "SkewY", MemberAccessTypes.Public);
    public static readonly IPropertyId TranslateXProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "TranslateX", MemberAccessTypes.Public);
    public static readonly IPropertyId TranslateYProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "TranslateY", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterXProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "CenterX", MemberAccessTypes.Public);
    public static readonly IPropertyId CenterYProperty = (IPropertyId) PlatformTypes.CompositeTransform.GetMember(MemberType.Property, "CenterY", MemberAccessTypes.Public);
    public static readonly CompositeTransformNode.ConcreteCompositeTransformNodeFactory Factory = new CompositeTransformNode.ConcreteCompositeTransformNodeFactory();

    public class ConcreteCompositeTransformNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new CompositeTransformNode();
      }
    }
  }
}
