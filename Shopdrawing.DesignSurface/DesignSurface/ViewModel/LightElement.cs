// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.LightElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class LightElement : Model3DElement
  {
    public static readonly IPropertyId ColorProperty = (IPropertyId) PlatformTypes.Light.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
    public static readonly IPropertyId ConstantAttenuationProperty = (IPropertyId) PlatformTypes.PointLightBase.GetMember(MemberType.LocalProperty, "ConstantAttenuation", MemberAccessTypes.Public);
    public static readonly IPropertyId LinearAttenuationProperty = (IPropertyId) PlatformTypes.PointLightBase.GetMember(MemberType.LocalProperty, "LinearAttenuation", MemberAccessTypes.Public);
    public static readonly IPropertyId PositionProperty = (IPropertyId) PlatformTypes.PointLightBase.GetMember(MemberType.LocalProperty, "Position", MemberAccessTypes.Public);
    public static readonly IPropertyId QuadraticAttenuationProperty = (IPropertyId) PlatformTypes.PointLightBase.GetMember(MemberType.LocalProperty, "QuadraticAttenuation", MemberAccessTypes.Public);
    public static readonly IPropertyId RangeProperty = (IPropertyId) PlatformTypes.PointLightBase.GetMember(MemberType.LocalProperty, "Range", MemberAccessTypes.Public);
  }
}
