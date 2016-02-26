// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ValueConverter`2
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class ValueConverter<TSource, TTarget> : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is TSource) && (value != null || typeof (TSource).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ValueNotOfType, new object[1]
        {
          (object) typeof (TSource).FullName
        }));
      if (!targetType.IsAssignableFrom(typeof (TTarget)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_TargetNotExtendingType, new object[1]
        {
          (object) typeof (TTarget).FullName
        }));
      return (object) this.Convert((TSource) value, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is TTarget) && (value != null || typeof (TTarget).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ValueNotOfType, new object[1]
        {
          (object) typeof (TTarget).FullName
        }));
      if (!targetType.IsAssignableFrom(typeof (TSource)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_TargetNotExtendingType, new object[1]
        {
          (object) typeof (TSource).FullName
        }));
      return (object) this.ConvertBack((TTarget) value, parameter, culture);
    }

    protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ConverterFunctionNotDefined, new object[1]
      {
        (object) "Convert"
      }));
    }

    protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ConverterFunctionNotDefined, new object[1]
      {
        (object) "ConvertBack"
      }));
    }
  }
}
