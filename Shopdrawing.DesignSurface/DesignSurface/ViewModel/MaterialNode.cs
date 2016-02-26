// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.MaterialNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class MaterialNode : SceneNode
  {
    public static readonly IPropertyId SpecularBrushProperty = (IPropertyId) PlatformTypes.SpecularMaterial.GetMember(MemberType.LocalProperty, "Brush", MemberAccessTypes.Public);
    public static readonly IPropertyId SpecularColorProperty = (IPropertyId) PlatformTypes.SpecularMaterial.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
    public static readonly IPropertyId SpecularPowerProperty = (IPropertyId) PlatformTypes.SpecularMaterial.GetMember(MemberType.LocalProperty, "SpecularPower", MemberAccessTypes.Public);
    public static readonly IPropertyId DiffusBrushProperty = (IPropertyId) PlatformTypes.DiffuseMaterial.GetMember(MemberType.LocalProperty, "Brush", MemberAccessTypes.Public);
    public static readonly IPropertyId DiffusColorProperty = (IPropertyId) PlatformTypes.DiffuseMaterial.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
    public static readonly IPropertyId DiffusAmbientColorProperty = (IPropertyId) PlatformTypes.DiffuseMaterial.GetMember(MemberType.LocalProperty, "AmbientColor", MemberAccessTypes.Public);
    public static readonly IPropertyId EmissiveBrushProperty = (IPropertyId) PlatformTypes.EmissiveMaterial.GetMember(MemberType.LocalProperty, "Brush", MemberAccessTypes.Public);
    public static readonly IPropertyId EmissiveColorProperty = (IPropertyId) PlatformTypes.EmissiveMaterial.GetMember(MemberType.LocalProperty, "Color", MemberAccessTypes.Public);
    public static readonly MaterialNode.ConcreteMaterialNodeFactory Factory = new MaterialNode.ConcreteMaterialNodeFactory();

    public class ConcreteMaterialNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new MaterialNode();
      }
    }
  }
}
