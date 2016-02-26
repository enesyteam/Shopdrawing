// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ShortToDoubleConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class ShortToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double result;
      if (value.GetType().IsAssignableFrom(typeof (short)))
          result = System.Convert.ToDouble(value, (IFormatProvider)CultureInfo.CurrentCulture);
      else
        double.TryParse(value.ToString(), out result);
      return (object) result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      short result;
      if (value.GetType().IsAssignableFrom(typeof (double)))
          result = System.Convert.ToInt16(value, (IFormatProvider)CultureInfo.CurrentCulture);
      else
        short.TryParse(value.ToString(), out result);
      return (object) result;
    }
  }
}
