// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerFonts
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Interaction;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public static class AdornerFonts
  {
    private static readonly ResourceKey _fontFamilyKey = AdornerFonts.CreateKey("FontFamilyKey");
    private static readonly ResourceKey _fontSizeKey = AdornerFonts.CreateKey("FontSizeKey");

    public static ResourceKey FontFamilyKey
    {
      get
      {
        return AdornerFonts._fontFamilyKey;
      }
    }

    public static ResourceKey FontSizeKey
    {
      get
      {
        return AdornerFonts._fontSizeKey;
      }
    }

    public static FontFamily FontFamily
    {
      get
      {
        return AdornerFonts.GetFontFamily(AdornerFonts.FontFamilyKey);
      }
    }

    public static double FontSize
    {
      get
      {
        return AdornerFonts.GetFontSize(AdornerFonts.FontSizeKey);
      }
    }

    static AdornerFonts()
    {
      AdornerResources.RegisterResources((LoadResourcesCallback) (() => (ResourceDictionary) new AdornerFontResourceDictionary()));
    }

    private static ResourceKey CreateKey(string name)
    {
      return AdornerResources.CreateResourceKey(typeof (AdornerFonts), name);
    }

    private static double GetFontSize(ResourceKey key)
    {
      return (double) AdornerResources.FindResource(key);
    }

    private static FontFamily GetFontFamily(ResourceKey key)
    {
      return (FontFamily) AdornerResources.FindResource(key);
    }
  }
}
