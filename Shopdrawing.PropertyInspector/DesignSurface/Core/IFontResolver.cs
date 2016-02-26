// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Core.IFontResolver
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignModel.Core
{
  public interface IFontResolver
  {
    string FontCacheDirectory { get; }

    string ResolveFont(string fontFamilySource, object fontStretch, object fontStyle, object fontWeight, IDocumentContext documentContext);

    string ConvertToWpfFontName(string gdiFontName);

    string ConvertToGdiFontName(string wpfFontName);
  }
}
