// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerColors
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Interaction;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public static class AdornerColors
  {
    private static readonly ResourceKey _alignmentMarkBrushKey = AdornerColors.CreateKey("AlignmentMarkBrushKey");
    private static readonly ResourceKey _alignmentMarkColorKey = AdornerColors.CreateKey("AlignmentMarkColorKey");
    private static readonly ResourceKey _elementBorderBrushKey = AdornerColors.CreateKey("ElementBorderBrushKey");
    private static readonly ResourceKey _elementBorderColorKey = AdornerColors.CreateKey("ElementBorderColorKey");
    private static readonly ResourceKey _glyphFillBrushKey = AdornerColors.CreateKey("GlyphFillBrushKey");
    private static readonly ResourceKey _glyphFillColorKey = AdornerColors.CreateKey("GlyphFillColorKey");
    private static readonly ResourceKey _handleBorderColorKey = AdornerColors.CreateKey("HandleBorderColorKey");
    private static readonly ResourceKey _handleBorderBrushKey = AdornerColors.CreateKey("HandleBorderBrushKey");
    private static readonly ResourceKey _handleFillColorKey = AdornerColors.CreateKey("HandleFillColorKey");
    private static readonly ResourceKey _handleFillBrushKey = AdornerColors.CreateKey("HandleFillBrushKey");
    private static readonly ResourceKey _handleFillHoverColorKey = AdornerColors.CreateKey("HandleFillHoverColorKey");
    private static readonly ResourceKey _handleEmptyFillColorKey = AdornerColors.CreateKey("HandleEmptyFillColorKey");
    private static readonly ResourceKey _handleEmptyFillBrushKey = AdornerColors.CreateKey("HandleEmptyFillBrushKey");
    private static readonly ResourceKey _handleFillHoverBrushKey = AdornerColors.CreateKey("HandleFillHoverBrushKey");
    private static readonly ResourceKey _handleFillPressedColorKey = AdornerColors.CreateKey("HandleFillPressedColorKey");
    private static readonly ResourceKey _handleFillPressedBrushKey = AdornerColors.CreateKey("HandleFillPressedBrushKey");
    private static readonly ResourceKey _moveHandleContentBrushKey = AdornerColors.CreateKey("MoveHandleContentBrushKey");
    private static readonly ResourceKey _moveHandleContentColorKey = AdornerColors.CreateKey("MoveHandleContentColorKey");
    private static readonly ResourceKey _moveHandleFillBrushKey = AdornerColors.CreateKey("MoveHandleFillBrushKey");
    private static readonly ResourceKey _moveHandleFillColorKey = AdornerColors.CreateKey("MoveHandleFillColorKey");
    private static readonly ResourceKey _moveHandleFillHoverBrushKey = AdornerColors.CreateKey("MoveHandleFillHoverBrushKey");
    private static readonly ResourceKey _moveHandleFillHoverColorKey = AdornerColors.CreateKey("MoveHandleFillHoverColorKey");
    private static readonly ResourceKey _railFillBrushKey = AdornerColors.CreateKey("RailFillBrushKey");
    private static readonly ResourceKey _railFillColorKey = AdornerColors.CreateKey("RailFillColorKey");
    private static readonly ResourceKey _selectionFrameBorderBrushKey = AdornerColors.CreateKey("SelectionFrameBorderBrushKey");
    private static readonly ResourceKey _selectionFrameBorderColorKey = AdornerColors.CreateKey("SelectionFrameBorderColorKey");
    private static readonly ResourceKey _selectionFrameFillBrushKey = AdornerColors.CreateKey("SelectionFrameFillBrushKey");
    private static readonly ResourceKey _selectionFrameFillColorKey = AdornerColors.CreateKey("SelectionFrameFillColorKey");
    private static readonly ResourceKey _simpleWashBrushKey = AdornerColors.CreateKey("SimpleWashBrushKey");
    private static readonly ResourceKey _simpleWashColorKey = AdornerColors.CreateKey("SimpleWashColorKey");
    private static readonly ResourceKey _toggledGlyphFillBrushKey = AdornerColors.CreateKey("ToggledGlyphFillBrushKey");
    private static readonly ResourceKey _toggledGlyphFillColorKey = AdornerColors.CreateKey("ToggledGlyphFillColorKey");

    public static Brush AlignmentMarkBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.AlignmentMarkBrushKey);
      }
    }

    public static ResourceKey AlignmentMarkBrushKey
    {
      get
      {
        return AdornerColors._alignmentMarkBrushKey;
      }
    }

    public static Color AlignmentMarkColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.AlignmentMarkColorKey);
      }
    }

    public static ResourceKey AlignmentMarkColorKey
    {
      get
      {
        return AdornerColors._alignmentMarkColorKey;
      }
    }

    public static Brush ElementBorderBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.ElementBorderBrushKey);
      }
    }

    public static ResourceKey ElementBorderBrushKey
    {
      get
      {
        return AdornerColors._elementBorderBrushKey;
      }
    }

    public static Color ElementBorderColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.ElementBorderColorKey);
      }
    }

    public static ResourceKey ElementBorderColorKey
    {
      get
      {
        return AdornerColors._elementBorderColorKey;
      }
    }

    public static Brush GlyphFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.GlyphFillBrushKey);
      }
    }

    public static ResourceKey GlyphFillBrushKey
    {
      get
      {
        return AdornerColors._glyphFillBrushKey;
      }
    }

    public static Color GlyphFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.GlyphFillColorKey);
      }
    }

    public static ResourceKey GlyphFillColorKey
    {
      get
      {
        return AdornerColors._glyphFillColorKey;
      }
    }

    public static Brush HandleBorderBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.HandleBorderBrushKey);
      }
    }

    public static ResourceKey HandleBorderBrushKey
    {
      get
      {
        return AdornerColors._handleBorderBrushKey;
      }
    }

    public static Color HandleBorderColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.HandleBorderColorKey);
      }
    }

    public static ResourceKey HandleBorderColorKey
    {
      get
      {
        return AdornerColors._handleBorderColorKey;
      }
    }

    public static Brush HandleFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.HandleFillBrushKey);
      }
    }

    public static ResourceKey HandleFillBrushKey
    {
      get
      {
        return AdornerColors._handleFillBrushKey;
      }
    }

    public static Color HandleFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.HandleFillColorKey);
      }
    }

    public static ResourceKey HandleFillColorKey
    {
      get
      {
        return AdornerColors._handleFillColorKey;
      }
    }

    public static Brush HandleEmptyFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.HandleEmptyFillBrushKey);
      }
    }

    public static ResourceKey HandleEmptyFillBrushKey
    {
      get
      {
        return AdornerColors._handleEmptyFillBrushKey;
      }
    }

    public static Color HandleEmptyFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.HandleEmptyFillColorKey);
      }
    }

    public static ResourceKey HandleEmptyFillColorKey
    {
      get
      {
        return AdornerColors._handleEmptyFillColorKey;
      }
    }

    public static Brush HandleFillHoverBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.HandleFillHoverBrushKey);
      }
    }

    public static ResourceKey HandleFillHoverBrushKey
    {
      get
      {
        return AdornerColors._handleFillHoverBrushKey;
      }
    }

    public static Color HandleFillHoverColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.HandleFillHoverColorKey);
      }
    }

    public static ResourceKey HandleFillHoverColorKey
    {
      get
      {
        return AdornerColors._handleFillHoverColorKey;
      }
    }

    public static Brush HandleFillPressedBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.HandleFillPressedBrushKey);
      }
    }

    public static ResourceKey HandleFillPressedBrushKey
    {
      get
      {
        return AdornerColors._handleFillPressedBrushKey;
      }
    }

    public static Color HandleFillPressedColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.HandleFillPressedColorKey);
      }
    }

    public static ResourceKey HandleFillPressedColorKey
    {
      get
      {
        return AdornerColors._handleFillPressedColorKey;
      }
    }

    public static Brush MoveHandleContentBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.MoveHandleContentBrushKey);
      }
    }

    public static ResourceKey MoveHandleContentBrushKey
    {
      get
      {
        return AdornerColors._moveHandleContentBrushKey;
      }
    }

    public static Color MoveHandleContentColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.MoveHandleContentColorKey);
      }
    }

    public static ResourceKey MoveHandleContentColorKey
    {
      get
      {
        return AdornerColors._moveHandleContentColorKey;
      }
    }

    public static Brush MoveHandleFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.MoveHandleFillBrushKey);
      }
    }

    public static ResourceKey MoveHandleFillBrushKey
    {
      get
      {
        return AdornerColors._moveHandleFillBrushKey;
      }
    }

    public static Color MoveHandleFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.MoveHandleFillColorKey);
      }
    }

    public static ResourceKey MoveHandleFillColorKey
    {
      get
      {
        return AdornerColors._moveHandleFillColorKey;
      }
    }

    public static Brush MoveHandleFillHoverBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.MoveHandleFillHoverBrushKey);
      }
    }

    public static ResourceKey MoveHandleFillHoverBrushKey
    {
      get
      {
        return AdornerColors._moveHandleFillHoverBrushKey;
      }
    }

    public static Color MoveHandleFillHoverColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.MoveHandleFillHoverColorKey);
      }
    }

    public static ResourceKey MoveHandleFillHoverColorKey
    {
      get
      {
        return AdornerColors._moveHandleFillHoverColorKey;
      }
    }

    public static Brush RailFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.RailFillBrushKey);
      }
    }

    public static ResourceKey RailFillBrushKey
    {
      get
      {
        return AdornerColors._railFillBrushKey;
      }
    }

    public static Color RailFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.RailFillColorKey);
      }
    }

    public static ResourceKey RailFillColorKey
    {
      get
      {
        return AdornerColors._railFillColorKey;
      }
    }

    public static Brush SelectionFrameBorderBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.SelectionFrameBorderBrushKey);
      }
    }

    public static ResourceKey SelectionFrameBorderBrushKey
    {
      get
      {
        return AdornerColors._selectionFrameBorderBrushKey;
      }
    }

    public static Color SelectionFrameBorderColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.SelectionFrameBorderColorKey);
      }
    }

    public static ResourceKey SelectionFrameBorderColorKey
    {
      get
      {
        return AdornerColors._selectionFrameBorderColorKey;
      }
    }

    public static Brush SelectionFrameFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.SelectionFrameFillBrushKey);
      }
    }

    public static ResourceKey SelectionFrameFillBrushKey
    {
      get
      {
        return AdornerColors._selectionFrameFillBrushKey;
      }
    }

    public static Color SelectionFrameFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.SelectionFrameFillColorKey);
      }
    }

    public static ResourceKey SelectionFrameFillColorKey
    {
      get
      {
        return AdornerColors._selectionFrameFillColorKey;
      }
    }

    public static Brush SimpleWashBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.SimpleWashBrushKey);
      }
    }

    public static ResourceKey SimpleWashBrushKey
    {
      get
      {
        return AdornerColors._simpleWashBrushKey;
      }
    }

    public static Color SimpleWashColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.SimpleWashColorKey);
      }
    }

    public static ResourceKey SimpleWashColorKey
    {
      get
      {
        return AdornerColors._simpleWashColorKey;
      }
    }

    public static Brush ToggledGlyphFillBrush
    {
      get
      {
        return AdornerColors.GetBrush(AdornerColors.ToggledGlyphFillBrushKey);
      }
    }

    public static ResourceKey ToggledGlyphFillBrushKey
    {
      get
      {
        return AdornerColors._toggledGlyphFillBrushKey;
      }
    }

    public static Color ToggledGlyphFillColor
    {
      get
      {
        return AdornerColors.GetColor(AdornerColors.ToggledGlyphFillColorKey);
      }
    }

    public static ResourceKey ToggledGlyphFillColorKey
    {
      get
      {
        return AdornerColors._toggledGlyphFillColorKey;
      }
    }

    static AdornerColors()
    {
      AdornerResources.RegisterResources((LoadResourcesCallback) (() => (ResourceDictionary) new AdornerColorResourceDictionary()));
    }

    private static ResourceKey CreateKey(string name)
    {
      return AdornerResources.CreateResourceKey(typeof (AdornerColors), name);
    }

    private static Brush GetBrush(ResourceKey key)
    {
      return (Brush) AdornerResources.FindResource(key);
    }

    private static Color GetColor(ResourceKey key)
    {
      return (Color) AdornerResources.FindResource(key);
    }
  }
}
