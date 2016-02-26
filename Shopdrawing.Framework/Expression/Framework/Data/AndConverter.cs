// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.AndConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class AndConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (object obj in values)
      {
        if (!(obj is bool) || !(bool) obj)
          return (object) false;
      }
      return (object) true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
