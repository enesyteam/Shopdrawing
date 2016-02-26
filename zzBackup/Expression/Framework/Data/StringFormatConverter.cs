// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.StringFormatConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  [Localizability(LocalizationCategory.Text)]
  public class StringFormatConverter : IMultiValueConverter, IValueConverter
  {
    private string format;

    public string Format
    {
      get
      {
        return this.format;
      }
      set
      {
        this.format = value;
      }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return this.Convert(new object[1]
      {
        value
      }, targetType, parameter, culture);
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.format.Replace("{}", ""), values);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return (object[]) null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }
  }
}
