// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.SystemFontFamily
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class SystemFontFamily
  {
    private List<string> fontSources = new List<string>();
    private FontFamily fontFamily;

    public string FontFamilyName
    {
      get
      {
        return FontEmbedder.GetFontNameFromSource(this.fontFamily);
      }
    }

    public FontFamily FontFamily
    {
      get
      {
        return this.fontFamily;
      }
    }

    public List<string> FontSources
    {
      get
      {
        return this.fontSources;
      }
    }

    public bool IsNativeSilverlightFont
    {
      get
      {
        return Array.IndexOf<string>(FontEmbedder.silverlightFontNames, this.FontFamilyName) != -1;
      }
    }

    public SystemFontFamily(FontFamily fontFamily)
    {
      this.fontFamily = fontFamily;
    }
  }
}
