// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.GridLengthConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class GridLengthConverter : IValueConverter
  {
    private bool inverted;

    public bool Inverted
    {
      get
      {
        return this.inverted;
      }
      set
      {
        this.inverted = value;
      }
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      if (!this.inverted)
        return (object) ((GridLength) o).Value;
      return (object) new GridLength((double) o, GridUnitType.Pixel);
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      if (!this.inverted)
        return (object) new GridLength((double) o, GridUnitType.Pixel);
      return (object) ((GridLength) o).Value;
    }
  }
}
