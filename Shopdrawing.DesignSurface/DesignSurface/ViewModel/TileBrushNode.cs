// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TileBrushNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class TileBrushNode : BrushNode
  {
    public static readonly IPropertyId StretchProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "Stretch", MemberAccessTypes.Public);
    public static readonly IPropertyId TileModeProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "TileMode", MemberAccessTypes.Public);
    public static readonly IPropertyId ViewboxProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "Viewbox", MemberAccessTypes.Public);
    public static readonly IPropertyId ViewboxUnitsProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "ViewboxUnits", MemberAccessTypes.Public);
    public static readonly IPropertyId ViewportProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "Viewport", MemberAccessTypes.Public);
    public static readonly IPropertyId ViewportUnitsProperty = (IPropertyId) PlatformTypes.TileBrush.GetMember(MemberType.LocalProperty, "ViewportUnits", MemberAccessTypes.Public);
    public static readonly TileBrushNode.ConcreteTileBrushNodeFactory Factory = new TileBrushNode.ConcreteTileBrushNodeFactory();

    public class ConcreteTileBrushNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new TileBrushNode();
      }
    }
  }
}
