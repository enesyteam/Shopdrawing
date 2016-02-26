// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.SwitchConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Data
{
  [ContentProperty("Cases")]
  public class SwitchConverter : DependencyObject, IValueConverter
  {
    private static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof (object), typeof (SwitchConverter));
    private List<SwitchCase> cases;

    public List<SwitchCase> Cases
    {
      get
      {
        return this.cases;
      }
    }

    public object DefaultValue
    {
      get
      {
        return this.GetValue(SwitchConverter.DefaultValueProperty);
      }
      set
      {
        this.SetValue(SwitchConverter.DefaultValueProperty, value);
      }
    }

    public SwitchConverter()
    {
      this.cases = new List<SwitchCase>();
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (SwitchCase switchCase in this.Cases)
      {
        if (object.Equals(switchCase.In, o))
          return switchCase.Out;
      }
      return this.DefaultValue;
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException(ExceptionStringTable.SwitchConverterIsOneWay);
    }
  }
}
