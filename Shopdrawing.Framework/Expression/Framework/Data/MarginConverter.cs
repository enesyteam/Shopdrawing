// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.MarginConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class MarginConverter : IValueConverter
  {
    private double left;
    private double top;
    private double right;
    private double bottom;
    private IValueConverter converter;

    public double Left
    {
      get
      {
        return this.left;
      }
      set
      {
        this.left = value;
      }
    }

    public double Top
    {
      get
      {
        return this.top;
      }
      set
      {
        this.top = value;
      }
    }

    public double Right
    {
      get
      {
        return this.right;
      }
      set
      {
        this.right = value;
      }
    }

    public double Bottom
    {
      get
      {
        return this.bottom;
      }
      set
      {
        this.bottom = value;
      }
    }

    public IValueConverter Converter
    {
      get
      {
        return this.converter;
      }
      set
      {
        this.converter = value;
      }
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(ExceptionStringTable.ConvertBackNotImplementedOnMarginConverter);
    }

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(ExceptionStringTable.ConvertNotImplementedOnMarginConverter);
    }
  }
}
