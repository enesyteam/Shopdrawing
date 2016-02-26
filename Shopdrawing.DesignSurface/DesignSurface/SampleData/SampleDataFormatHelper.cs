// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataFormatHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public static class SampleDataFormatHelper
  {
    private static List<TypeFormatInfo> defaults = new List<TypeFormatInfo>()
    {
      new TypeFormatInfo(SampleBasicType.Boolean, (string) null, (string) null),
      new TypeFormatInfo(SampleBasicType.Date, (string) null, (string) null),
      new TypeFormatInfo(SampleBasicType.Image, (string) null, Path.Combine(TemplateManager.TranslatedFolder("SampleDataResources"), "Images")),
      new TypeFormatInfo(SampleBasicType.String, StringTable.SampleDataConfigurationLoremIpsumFormat, "4,8"),
      new TypeFormatInfo(SampleBasicType.Number, (string) null, "2")
    };

    public static string GetDefaultFormat(SampleBasicType sampleType)
    {
      return SampleDataFormatHelper.defaults.Find((Predicate<TypeFormatInfo>) (info => info.SampleType == sampleType)).Format;
    }

    public static string GetDefaultFormatParameters(SampleBasicType sampleType)
    {
      return SampleDataFormatHelper.defaults.Find((Predicate<TypeFormatInfo>) (info => info.SampleType == sampleType)).FormatParameters;
    }

    public static string NormalizeFormat(SampleBasicType sampleType, string format, bool nullPreferred)
    {
      if (sampleType == null)
        return format;
      TypeFormatInfo typeFormatInfo = SampleDataFormatHelper.defaults.Find((Predicate<TypeFormatInfo>) (info => info.SampleType == sampleType));
      if (nullPreferred && (format == null || typeFormatInfo.Format == format))
        return (string) null;
      if (!nullPreferred && format == null)
        return typeFormatInfo.Format;
      return format;
    }

    public static string NormalizeFormatParameters(SampleBasicType sampleType, string formatParameters, bool nullPreferred)
    {
      if (sampleType == null)
        return formatParameters;
      TypeFormatInfo typeFormatInfo = SampleDataFormatHelper.defaults.Find((Predicate<TypeFormatInfo>) (info => info.SampleType == sampleType));
      if (nullPreferred && (formatParameters == null || typeFormatInfo.FormatParameters == formatParameters))
        return (string) null;
      if (!nullPreferred && formatParameters == null)
        return typeFormatInfo.FormatParameters;
      return formatParameters;
    }
  }
}
