// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ButtonProperties
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public static class ButtonProperties
  {
    public static readonly IPropertyId IsDefaultedProperty = (IPropertyId) PlatformTypes.Button.GetMember(MemberType.LocalProperty, "IsDefaulted", MemberAccessTypes.Public);
    public static readonly IPropertyId IsFocusedProperty = (IPropertyId) PlatformTypes.Button.GetMember(MemberType.LocalProperty, "IsFocused", MemberAccessTypes.Public);
    public static readonly IPropertyId IsPressedProperty = (IPropertyId) PlatformTypes.ButtonBase.GetMember(MemberType.LocalProperty, "IsPressed", MemberAccessTypes.Public);
    public static readonly IPropertyId IsCheckedProperty = (IPropertyId) PlatformTypes.ToggleButton.GetMember(MemberType.LocalProperty, "IsChecked", MemberAccessTypes.Public);
    public static readonly IPropertyId CommandProperty = (IPropertyId) PlatformTypes.ToggleButton.GetMember(MemberType.LocalProperty, "Command", MemberAccessTypes.Public);
  }
}
