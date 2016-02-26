// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.SingleMarginConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.Framework.Data
{
  public class SingleMarginConverter : MarginConverter
  {
    private MarginSubProperty target;

    public MarginSubProperty TargetSubProperty
    {
      get
      {
        return this.target;
      }
      set
      {
        this.target = value;
      }
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      object obj;
      switch (this.target)
      {
        case MarginSubProperty.Left:
          obj = (object) new Thickness(this.Left - ((Thickness) value).Left, this.Top, this.Right, this.Bottom);
          break;
        case MarginSubProperty.Top:
          obj = (object) new Thickness(this.Left, this.Top - ((Thickness) value).Top, this.Right, this.Bottom);
          break;
        case MarginSubProperty.Right:
          obj = (object) new Thickness(this.Left, this.Top, this.Right - ((Thickness) value).Right, this.Bottom);
          break;
        case MarginSubProperty.Bottom:
          obj = (object) new Thickness(this.Left, this.Top, this.Right, this.Bottom - ((Thickness) value).Bottom);
          break;
        default:
          throw new ArgumentException(ExceptionStringTable.InvalidTargetSubProperty);
      }
      if (this.Converter != null)
        obj = this.Converter.ConvertBack(obj, targetType, parameter, culture);
      return obj;
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (this.Converter != null)
        value = this.Converter.Convert(value, typeof (double), parameter, culture);
      double num = (double) value;
      switch (this.target)
      {
        case MarginSubProperty.Left:
          return (object) new Thickness(num + this.Left, this.Top, this.Right, this.Bottom);
        case MarginSubProperty.Top:
          return (object) new Thickness(this.Left, num + this.Top, this.Right, this.Bottom);
        case MarginSubProperty.Right:
          return (object) new Thickness(this.Left, this.Top, num + this.Right, this.Bottom);
        case MarginSubProperty.Bottom:
          return (object) new Thickness(this.Left, this.Top, this.Right, num + this.Bottom);
        default:
          throw new ArgumentException(ExceptionStringTable.InvalidTargetSubProperty);
      }
    }
  }
}
