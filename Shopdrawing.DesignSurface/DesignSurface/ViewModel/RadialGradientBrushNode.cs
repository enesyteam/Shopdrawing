// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.RadialGradientBrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class RadialGradientBrushNode : GradientBrushNode
  {
    public static readonly IPropertyId CenterProperty = (IPropertyId) PlatformTypes.RadialGradientBrush.GetMember(MemberType.LocalProperty, "Center", MemberAccessTypes.Public);
    public static readonly IPropertyId GradientOriginProperty = (IPropertyId) PlatformTypes.RadialGradientBrush.GetMember(MemberType.LocalProperty, "GradientOrigin", MemberAccessTypes.Public);
    public static readonly IPropertyId RadiusXProperty = (IPropertyId) PlatformTypes.RadialGradientBrush.GetMember(MemberType.LocalProperty, "RadiusX", MemberAccessTypes.Public);
    public static readonly IPropertyId RadiusYProperty = (IPropertyId) PlatformTypes.RadialGradientBrush.GetMember(MemberType.LocalProperty, "RadiusY", MemberAccessTypes.Public);
    public static readonly RadialGradientBrushNode.ConcreteRadialGradientBrushNodeFactory Factory = new RadialGradientBrushNode.ConcreteRadialGradientBrushNodeFactory();

    public class ConcreteRadialGradientBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new RadialGradientBrushNode();
      }
    }
  }
}
