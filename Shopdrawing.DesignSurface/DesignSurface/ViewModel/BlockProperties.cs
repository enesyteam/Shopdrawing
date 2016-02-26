// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BlockProperties
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class BlockProperties
  {
    public static readonly IPropertyId FontWeightProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStyleProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId ForegroundProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId LineHeightProperty = (IPropertyId) PlatformTypes.Block.GetMember(MemberType.LocalProperty, "LineHeight", MemberAccessTypes.Public);
  }
}
