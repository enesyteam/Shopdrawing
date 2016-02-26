// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.SourceControl.SourceControlStatusToIconOrToolTipConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.SourceControl
{
  public class SourceControlStatusToIconOrToolTipConverter : IValueConverter
  {
    private static Dictionary<string, ImageSource> iconCache;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is SourceControlStatus))
        return (object) null;
      string key = "status_None.png";
      string str = (string) null;
      switch ((SourceControlStatus) value)
      {
        case SourceControlStatus.Add:
          key = "add_on_16.png";
          str = StringTable.NewlyAddedFile;
          break;
        case SourceControlStatus.Rename:
        case SourceControlStatus.RemoteChange:
          key = "rename_on_16.png";
          str = StringTable.CheckedOutToMe;
          break;
        case SourceControlStatus.CheckedIn:
          key = "checked-in_on_16.png";
          str = StringTable.CheckedIn;
          break;
        case SourceControlStatus.CheckedOut:
          key = "checked-out_on_16.png";
          str = StringTable.CheckedOutToMe;
          break;
        case SourceControlStatus.Locked:
          key = "checked-in_on_16.png";
          str = StringTable.CheckedOutToMeExclusively;
          break;
      }
      if (targetType.IsAssignableFrom(typeof (string)))
        return (object) str;
      if (SourceControlStatusToIconOrToolTipConverter.iconCache == null)
        SourceControlStatusToIconOrToolTipConverter.iconCache = new Dictionary<string, ImageSource>();
      ImageSource imageSource = (ImageSource) null;
      if (!SourceControlStatusToIconOrToolTipConverter.iconCache.TryGetValue(key, out imageSource))
      {
        imageSource = FileTable.GetImageSource("Resources\\Icons\\SourceControl\\" + key);
        imageSource.Freeze();
        SourceControlStatusToIconOrToolTipConverter.iconCache.Add(key, imageSource);
      }
      return (object) imageSource;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }
  }
}
