// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ControlElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class ControlElement : BaseFrameworkElement
  {
    public static readonly IPropertyId TemplateProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "Template", MemberAccessTypes.Public);
    public static readonly IPropertyId HorizontalContentAlignmentProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "HorizontalContentAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId VerticalContentAlignmentProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "VerticalContentAlignment", MemberAccessTypes.Public);
    public static readonly IPropertyId PaddingProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "Padding", MemberAccessTypes.Public);
    public static readonly IPropertyId ForegroundProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId BorderBrushProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "BorderBrush", MemberAccessTypes.Public);
    public static readonly IPropertyId BorderThicknessProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "BorderThickness", MemberAccessTypes.Public);
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly IPropertyId FontSizeProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontSize", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStretchProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontStretch", MemberAccessTypes.Public);
    public static readonly IPropertyId FontStyleProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontStyle", MemberAccessTypes.Public);
    public static readonly IPropertyId FontWeightProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "FontWeight", MemberAccessTypes.Public);
    public static readonly IPropertyId IsTabStopProperty = (IPropertyId) PlatformTypes.Control.GetMember(MemberType.LocalProperty, "IsTabStop", MemberAccessTypes.Public);
    public static readonly ControlElement.ConcreteControlElementFactory Factory = new ControlElement.ConcreteControlElementFactory();

    public class ConcreteControlElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ControlElement();
      }
    }
  }
}
