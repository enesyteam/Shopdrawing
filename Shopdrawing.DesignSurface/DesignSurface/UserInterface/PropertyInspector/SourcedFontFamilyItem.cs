// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SourcedFontFamilyItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.Framework.ValueEditors;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public abstract class SourcedFontFamilyItem : FontFamilyItem
  {
    private SceneNodeObjectSet sceneNodeObjectSet;
    private IDocumentContext documentContext;
    private static string defaultPreviewFontFamilyName;

    public abstract FontFamily SerializeFontFamily { get; }

    protected abstract FontFamily FontFamilySource { get; }

    protected IDocumentContext DocumentContext
    {
      get
      {
        if (this.sceneNodeObjectSet != null)
          return this.sceneNodeObjectSet.DocumentContext;
        return this.documentContext;
      }
    }

    public abstract FontFamily DisplayFontFamily { get; }

    public static string DefaultPreviewFontFamilyName
    {
      get
      {
        return SourcedFontFamilyItem.defaultPreviewFontFamilyName;
      }
      internal set
      {
        SourcedFontFamilyItem.defaultPreviewFontFamilyName = value;
      }
    }

    public bool IsFontDamaged
    {
      get
      {
        try
        {
          foreach (Typeface typeface in (IEnumerable<Typeface>) this.FontFamily.GetTypefaces())
          {
            GlyphTypeface glyphTypeface;
            typeface.TryGetGlyphTypeface(out glyphTypeface);
          }
          return false;
        }
        catch
        {
          return true;
        }
      }
    }

    protected SourcedFontFamilyItem(FontFamily fontFamily, string category, IDocumentContext documentContext)
      : base(FontFamilyItem.EnsureFamilyName(FontEmbedder.GetFontNameFromSource(fontFamily)), category, SourcedFontFamilyItem.DefaultPreviewFontFamilyName, SourcedFontFamilyItem.ConvertToWpfFontFamily(fontFamily, documentContext))
    {
      this.documentContext = documentContext;
    }

    protected SourcedFontFamilyItem(FontFamily fontFamily, string category, SceneNodeObjectSet sceneNodeObjectSet)
      : base(FontFamilyItem.EnsureFamilyName(FontEmbedder.GetFontNameFromSource(fontFamily)), category, SourcedFontFamilyItem.DefaultPreviewFontFamilyName, SourcedFontFamilyItem.ConvertToWpfFontFamily(fontFamily, sceneNodeObjectSet.DocumentContext))
    {
      this.sceneNodeObjectSet = sceneNodeObjectSet;
    }

    private static FontFamily ConvertToWpfFontFamily(FontFamily fontFamily, IDocumentContext documentContext)
    {
      if (string.IsNullOrEmpty(fontFamily.Source))
        return fontFamily;
      string fontNameFromSource = FontEmbedder.GetFontNameFromSource(fontFamily);
      string fontFamilyPath = FontEmbedder.GetFontFamilyPath(fontFamily.Source);
      string str = ((IProjectContext) documentContext.TypeResolver).FontResolver.ConvertToWpfFontName(fontNameFromSource);
      return new FontFamily(!string.IsNullOrEmpty(fontFamilyPath) ? fontFamilyPath + "#" + str : str);
    }

    public override bool Equals(object obj)
    {
      SourcedFontFamilyItem sourcedFontFamilyItem = obj as SourcedFontFamilyItem;
      if (sourcedFontFamilyItem == null || !(this.FamilyName == sourcedFontFamilyItem.FamilyName))
        return false;
      return FontEmbedder.AreFontsEqual(this.FontFamilySource, sourcedFontFamilyItem.FontFamilySource, this.DocumentContext);
    }

    public override int GetHashCode()
    {
      return this.FamilyName.GetHashCode() ^ this.FontFamilySource.GetHashCode();
    }
  }
}
