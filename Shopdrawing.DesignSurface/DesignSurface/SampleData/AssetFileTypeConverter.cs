// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.AssetFileTypeConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class AssetFileTypeConverter : TypeConverter
  {
    private string assetRootFolder;
    private IEnumerable<string> fileExtensions;

    public AssetFileTypeConverter(string assetRootFolder, IEnumerable<string> fileExtensions)
    {
      this.assetRootFolder = assetRootFolder;
      this.fileExtensions = fileExtensions;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (!(sourceType == typeof (string)))
        return base.CanConvertFrom(context, sourceType);
      return true;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      string str = value as string;
      if (str == null)
        return base.ConvertFrom(context, culture, value);
      string assetFilePath = this.GetAssetFilePath(str);
      if (string.IsNullOrEmpty(assetFilePath))
        throw new FormatException();
      return (object) assetFilePath;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (!(destinationType == typeof (string)))
        return base.CanConvertTo(context, destinationType);
      return true;
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof (string))
      {
        string str = value as string;
        if (str != null)
          return (object) str;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }

    private string GetAssetFilePath(string value)
    {
      string str = value;
      if (str.IndexOf('.') < 0)
        return (string) null;
      switch (str[0])
      {
        case '/':
        case '\\':
        case '.':
          return (string) null;
        default:
          if (value.IndexOf(':') >= 0)
            return (string) null;
          if (this.fileExtensions != null)
          {
            string fileExtension = Path.GetExtension(str);
            if (Enumerable.FirstOrDefault<string>(this.fileExtensions, (Func<string, bool>) (ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase))) == null)
              return (string) null;
          }
          string path = Path.Combine(this.assetRootFolder, str);
          if (!Microsoft.Expression.Framework.Documents.PathHelper.FileExists(path))
            return (string) null;
          return path;
      }
    }
  }
}
