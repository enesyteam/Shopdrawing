// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.EqualsConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class EqualsConverter : DependencyObject, IValueConverter
  {
    private object defaultValue = (object) false;
    private object matchValue = (object) true;

    public object DefaultValue
    {
      get
      {
        return this.defaultValue;
      }
      set
      {
        this.defaultValue = value;
      }
    }

    public object MatchValue
    {
      get
      {
        return this.matchValue;
      }
      set
      {
        this.matchValue = value;
      }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (object.Equals(value, parameter))
        return this.matchValue;
      return this.defaultValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException();
    }
  }
}
