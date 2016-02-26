// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.AdditionConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class AdditionConverter : IValueConverter
  {
    private double offset;

    public double Offset
    {
      get
      {
        return this.offset;
      }
      set
      {
        this.offset = value;
      }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double)
        return (object) ((double) value + this.offset);
      if (value is int)
        return (object) (int) ((double) (int) value + this.offset);
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
