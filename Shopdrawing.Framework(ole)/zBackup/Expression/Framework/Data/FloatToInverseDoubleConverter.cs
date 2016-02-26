// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.FloatToInverseDoubleConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class FloatToInverseDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double result;
      if (value.GetType().IsAssignableFrom(typeof (float)))
          result = System.Convert.ToDouble(value, (IFormatProvider)CultureInfo.CurrentCulture);
      else
        double.TryParse(value.ToString(), out result);
      return (object) (1.0 - result);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      float result;
      if (value.GetType().IsAssignableFrom(typeof (double)))
          result = System.Convert.ToSingle(value, (IFormatProvider)CultureInfo.CurrentCulture);
      else
        float.TryParse(value.ToString(), out result);
      return (object) (1.0 - (double) result);
    }
  }
}
