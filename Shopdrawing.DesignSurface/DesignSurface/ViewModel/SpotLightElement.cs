// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SpotLightElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class SpotLightElement : LightElement
  {
    public static readonly IPropertyId InnerConeAngleProperty = (IPropertyId) PlatformTypes.SpotLight.GetMember(MemberType.LocalProperty, "InnerConeAngle", MemberAccessTypes.Public);
    public static readonly IPropertyId OuterConeAngleProperty = (IPropertyId) PlatformTypes.SpotLight.GetMember(MemberType.LocalProperty, "OuterConeAngle", MemberAccessTypes.Public);
    public static readonly IPropertyId DirectionProperty = (IPropertyId) PlatformTypes.SpotLight.GetMember(MemberType.LocalProperty, "Direction", MemberAccessTypes.Public);
    public static readonly SpotLightElement.ConcreteSpotLightElementFactory Factory = new SpotLightElement.ConcreteSpotLightElementFactory();

    public class ConcreteSpotLightElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new SpotLightElement();
      }
    }
  }
}
