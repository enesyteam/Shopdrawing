// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.BoolToThicknessConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class BoolToThicknessConverter : IValueConverter
  {
    private Thickness trueValue = new Thickness();
    private Thickness falseValue = new Thickness();

    public Thickness TrueValue
    {
      get
      {
        return this.trueValue;
      }
      set
      {
        this.trueValue = value;
      }
    }

    public Thickness FalseValue
    {
      get
      {
        return this.falseValue;
      }
      set
      {
        this.falseValue = value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((bool) value)
        return (object) this.trueValue;
      return (object) this.falseValue;
    }
  }
}
