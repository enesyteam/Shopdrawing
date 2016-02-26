// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.MultiplyConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class MultiplyConverter : IValueConverter
  {
    private double multiplyValue;

    public double MultiplyValue
    {
      get
      {
        return this.multiplyValue;
      }
      set
      {
        this.multiplyValue = value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) ((!(value is int) ? (double) value : (double) (int) value) / this.multiplyValue);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) ((!(value is int) ? (!(value is uint) ? (double) value : (double) (uint) value) : (double) (int) value) * this.multiplyValue);
    }
  }
}
