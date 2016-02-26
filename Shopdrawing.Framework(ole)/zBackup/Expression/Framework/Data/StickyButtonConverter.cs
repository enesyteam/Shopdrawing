// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.StickyButtonConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public abstract class StickyButtonConverter : IValueConverter
  {
    public abstract object TrueValue { get; }

    public abstract object FalseValue { get; }

    public abstract object IndeterminateValue { get; }

    protected virtual bool AreEqual(object value, object referenceValue)
    {
      return value.Equals(referenceValue);
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      if (o == null)
        return (object) new bool?(false);
      if (this.AreEqual(o, this.IndeterminateValue))
        return (object) new bool?();
      if (this.AreEqual(o, this.TrueValue))
        return (object) new bool?(true);
      return (object) new bool?(false);
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      bool? nullable = new bool?();
      if (o is bool?)
        nullable = (bool?) o;
      else if (o is bool)
        nullable = new bool?((bool) o);
      if (!nullable.HasValue)
        return this.IndeterminateValue;
      if (nullable.Value)
        return this.TrueValue;
      return this.FalseValue;
    }
  }
}
