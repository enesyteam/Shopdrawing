// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class BrushNode : SceneNode
  {
    public static readonly IPropertyId TransformProperty = (IPropertyId) PlatformTypes.Brush.GetMember(MemberType.LocalProperty, "Transform", MemberAccessTypes.Public);
    public static readonly IPropertyId RelativeTransformProperty = (IPropertyId) PlatformTypes.Brush.GetMember(MemberType.LocalProperty, "RelativeTransform", MemberAccessTypes.Public);
    public static readonly IPropertyId OpacityProperty = (IPropertyId) PlatformTypes.Brush.GetMember(MemberType.LocalProperty, "Opacity", MemberAccessTypes.Public);
    public static readonly IPropertyId VisualBrushVisualProperty = (IPropertyId) PlatformTypes.VisualBrush.GetMember(MemberType.LocalProperty, "Visual", MemberAccessTypes.Public);
    public static readonly BrushNode.ConcreteBrushNodeFactory Factory = new BrushNode.ConcreteBrushNodeFactory();

    public class ConcreteBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BrushNode();
      }
    }
  }
}
