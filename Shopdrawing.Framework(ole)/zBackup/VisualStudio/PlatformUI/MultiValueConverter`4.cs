// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.MultiValueConverter`4
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class MultiValueConverter<TSource1, TSource2, TSource3, TTarget> : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 3)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_InsufficientSourceParameters, new object[1]
        {
          (object) 3
        }));
      foreach (object obj in values)
      {
        if (obj == DependencyProperty.UnsetValue)
          return (object) default (TTarget);
      }
      MultiValueHelper.CheckValue<TSource1>(values, 0);
      MultiValueHelper.CheckValue<TSource2>(values, 1);
      MultiValueHelper.CheckValue<TSource3>(values, 2);
      if (!targetType.IsAssignableFrom(typeof (TTarget)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_TargetNotExtendingType, new object[1]
        {
          (object) typeof (TTarget).FullName
        }));
      return (object) this.Convert((TSource1) values[0], (TSource2) values[1], (TSource3) values[2], parameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      if (targetTypes.Length != 3)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_InsufficientTypeParameters, new object[1]
        {
          (object) 3
        }));
      if (!(value is TTarget) && (value != null || typeof (TTarget).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ValueNotOfType, new object[1]
        {
          (object) typeof (TTarget).FullName
        }));
      MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
      MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
      MultiValueHelper.CheckType<TSource3>(targetTypes, 2);
      TSource1 out1;
      TSource2 out2;
      TSource3 out3;
      this.ConvertBack((TTarget) value, out out1, out out2, out out3, parameter, culture);
      return new object[3]
      {
        (object) out1,
        (object) out2,
        (object) out3
      };
    }

    protected virtual TTarget Convert(TSource1 value1, TSource2 value2, TSource3 value3, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ConverterFunctionNotDefined, new object[1]
      {
        (object) "Convert"
      }));
    }

    protected virtual void ConvertBack(TTarget value, out TSource1 out1, out TSource2 out2, out TSource3 out3, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.Error_ConverterFunctionNotDefined, new object[1]
      {
        (object) "ConvertBack"
      }));
    }
  }
}
