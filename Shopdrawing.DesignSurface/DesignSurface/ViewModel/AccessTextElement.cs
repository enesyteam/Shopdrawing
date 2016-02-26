// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.AccessTextElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class AccessTextElement : BaseFrameworkElement
  {
    public static readonly IPropertyId ForegroundProperty = (IPropertyId) PlatformTypes.AccessText.GetMember(MemberType.LocalProperty, "Foreground", MemberAccessTypes.Public);
    public static readonly IPropertyId BackgroundProperty = (IPropertyId) PlatformTypes.AccessText.GetMember(MemberType.LocalProperty, "Background", MemberAccessTypes.Public);
    public static readonly IPropertyId TextProperty = (IPropertyId) PlatformTypes.AccessText.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
    public static readonly IPropertyId FontFamilyProperty = (IPropertyId) PlatformTypes.AccessText.GetMember(MemberType.LocalProperty, "FontFamily", MemberAccessTypes.Public);
    public static readonly AccessTextElement.AccessTextElementFactory Factory = new AccessTextElement.AccessTextElementFactory();

    public class AccessTextElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new AccessTextElement();
      }
    }
  }
}
