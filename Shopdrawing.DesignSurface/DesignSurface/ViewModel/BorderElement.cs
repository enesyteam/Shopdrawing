// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BorderElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class BorderElement : DecoratorElement
  {
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.Border.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId BorderBrushProperty = (IPropertyId) PlatformTypes.Border.GetMember(MemberType.LocalProperty, "BorderBrush", MemberAccessTypes.Public);
    public static readonly IPropertyId BorderThicknessProperty = (IPropertyId) PlatformTypes.Border.GetMember(MemberType.LocalProperty, "BorderThickness", MemberAccessTypes.Public);
    public static readonly BorderElement.ConcreteBorderFactory Factory = new BorderElement.ConcreteBorderFactory();

    public class ConcreteBorderFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new BorderElement();
      }
    }
  }
}
