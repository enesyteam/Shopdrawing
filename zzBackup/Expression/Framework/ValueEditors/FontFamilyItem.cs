// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.FontFamilyItem
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class FontFamilyItem : INotifyPropertyChanged
  {
    private static readonly SerialAsyncProcess BackgroundFontPreviewRenderer = new SerialAsyncProcess((IAsyncMechanism) new CurrentDispatcherAsyncMechanism(DispatcherPriority.ContextIdle));
    private string unescapedFamilyName;
    private string familyName;
    private string categoryName;
    private FontFamily fontFamily;
    private string previewName;
    private bool? isFontReadable;
    private bool isFontCorrupt;
    private FontFamilyItem.PreviewFontState previewFontState;

    public string FamilyName
    {
      get
      {
        return this.familyName;
      }
    }

    public string UnescapedFamilyName
    {
      get
      {
        return this.unescapedFamilyName;
      }
    }

    public FontFamily FontFamily
    {
      get
      {
        return this.fontFamily;
      }
    }

    public string PreviewFamilyName
    {
      get
      {
        if (this.previewFontState == FontFamilyItem.PreviewFontState.Initial)
        {
          this.BeginBackgroundPreviewFont();
          return this.previewName;
        }
        if (this.isFontCorrupt)
          return this.previewName;
        return this.familyName;
      }
    }

    public int SortOverride
    {
      get
      {
        return (int) this.familyName[0] == 64 ? 1 : 0;
      }
    }

    public bool IsFontReadable
    {
      get
      {
        if (!this.isFontReadable.HasValue)
          this.isFontReadable = new bool?(!this.IsSymbolFont());
        return this.isFontReadable.Value;
      }
    }

    public bool IsFontPreviewReadable
    {
      get
      {
        if (this.previewFontState == FontFamilyItem.PreviewFontState.Rendered)
          return this.IsFontReadable;
        return true;
      }
    }

    public string CategoryName
    {
      get
      {
        return this.categoryName;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    static FontFamilyItem()
    {
      FontFamilyItem.BackgroundFontPreviewRenderer.Complete += new EventHandler(FontFamilyItem.BackgroundFontPreviewRenderer_AllDone);
      FontFamilyItem.BackgroundFontPreviewRenderer.Killed += new EventHandler(FontFamilyItem.BackgroundFontPreviewRenderer_AllDone);
    }

    public FontFamilyItem(string familyName, string categoryName)
      : this(familyName, categoryName, FontGallery.DefaultPreviewFontFamilyName, (FontFamily) null)
    {
    }

    public FontFamilyItem(string familyName, string categoryName, string previewName, FontFamily fontFamily)
    {
      if (fontFamily == null)
      {
        familyName = FontFamilyItem.EnsureFamilyName(familyName);
        fontFamily = new FontFamily(familyName);
      }
      this.unescapedFamilyName = Uri.UnescapeDataString(familyName);
      this.familyName = familyName;
      this.categoryName = categoryName;
      this.previewName = previewName;
      this.fontFamily = fontFamily;
    }

    private static void BackgroundFontPreviewRenderer_AllDone(object sender, EventArgs e)
    {
      FontFamilyItem.BackgroundFontPreviewRenderer.Clear();
    }

    public static string EnsureFamilyName(string familyName)
    {
      try
      {
        Uri.UnescapeDataString(familyName);
      }
      catch (UriFormatException ex)
      {
        familyName = Uri.EscapeUriString(familyName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
      }
      return familyName;
    }

    public override string ToString()
    {
      return this.UnescapedFamilyName;
    }

    private void BeginBackgroundPreviewFont()
    {
      FontFamilyItem.BackgroundFontPreviewRenderer.Add((AsyncProcess) new DelegateAsyncProcess(new Action<object, DoWorkEventArgs>(this.DoBackgroundPreviewFont)));
      if (FontFamilyItem.BackgroundFontPreviewRenderer.IsAlive)
        return;
      FontFamilyItem.BackgroundFontPreviewRenderer.Begin();
    }

    private bool IsSymbolFont()
    {
      foreach (Typeface typeface in (IEnumerable<Typeface>) this.fontFamily.GetTypefaces())
      {
        try
        {
          GlyphTypeface glyphTypeface;
          if (typeface.TryGetGlyphTypeface(out glyphTypeface))
            return glyphTypeface.Symbol;
        }
        catch
        {
        }
      }
      return false;
    }

    private void DoBackgroundPreviewFont(object sender, DoWorkEventArgs args)
    {
      this.TipOverPreviewFamilyName();
    }

    public override bool Equals(object obj)
    {
      FontFamilyItem fontFamilyItem = obj as FontFamilyItem;
      if (fontFamilyItem == null)
        return base.Equals(obj);
      if (this.familyName == fontFamilyItem.familyName)
        return this.categoryName == fontFamilyItem.categoryName;
      return false;
    }

    public override int GetHashCode()
    {
      return this.familyName.GetHashCode();
    }

    public void TipOverPreviewFamilyName()
    {
      try
      {
        Typeface typeface = new Typeface(this.familyName);
        GlyphTypeface glyphTypeface = (GlyphTypeface) null;
        typeface.TryGetGlyphTypeface(out glyphTypeface);
        this.previewFontState = FontFamilyItem.PreviewFontState.Rendered;
        this.OnPropertyChanged("PreviewFamilyName");
        if (this.IsFontReadable)
          return;
        this.OnPropertyChanged("IsFontPreviewReadable");
      }
      catch
      {
        this.isFontCorrupt = true;
        this.previewFontState = FontFamilyItem.PreviewFontState.Rendered;
        this.OnPropertyChanged("PreviewFamilyName");
        this.OnPropertyChanged("IsFontPreviewReadable");
      }
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private enum PreviewFontState
    {
      Initial,
      Rendered,
    }

    public class Converter : TypeConverter
    {
      public static FontFamilyItem.Converter Singleton = new FontFamilyItem.Converter();

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
        return sourceType == typeof (string);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        return destinationType == typeof (string);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        string familyName = value as string;
        if (value != null)
          return (object) new FontFamilyItem(familyName, string.Empty);
        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        FontFamilyItem fontFamilyItem = value as FontFamilyItem;
        if (destinationType == typeof (string) && fontFamilyItem != null)
          return (object) fontFamilyItem.FamilyName;
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
  }
}
