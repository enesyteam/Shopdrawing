// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.ComposingConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class ComposingConverter : IValueConverter
  {
    private List<IValueConverter> converters = new List<IValueConverter>();

    public List<IValueConverter> Converters
    {
      get
      {
        return this.converters;
      }
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      for (int index = 0; index < this.converters.Count; ++index)
        o = this.converters[index].Convert(o, targetType, parameter, culture);
      return o;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      for (int index = this.converters.Count - 1; index >= 0; --index)
        value = this.converters[index].ConvertBack(value, targetType, parameter, culture);
      return value;
    }
  }
}
