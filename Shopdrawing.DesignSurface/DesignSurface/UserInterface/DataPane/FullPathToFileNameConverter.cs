﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.FullPathToFileNameConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class FullPathToFileNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        try
        {
          return (object) Path.GetFileName(value.ToString());
        }
        catch (ArgumentException ex)
        {
        }
      }
      return (object) string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("Cannot convert from filename to full path");
    }
  }
}
