// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.LessThanEqualsConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class LessThanEqualsConverter : DependencyObject, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value != null)
        {
          if (parameter != null)
          {
            if ((double) value <= double.Parse((string) parameter, (IFormatProvider) CultureInfo.InvariantCulture))
              return (object) true;
          }
        }
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      return (object) false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
