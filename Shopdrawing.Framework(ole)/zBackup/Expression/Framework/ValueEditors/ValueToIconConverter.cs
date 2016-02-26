// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ValueToIconConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class ValueToIconConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 2)
      {
        IIconProvider iconProvider = values[0] as IIconProvider;
        object key = values[1];
        if (iconProvider != null && key != null)
          return (object) iconProvider.GetIconAsImageSource(key, parameter);
      }
      return (object) null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException(ExceptionStringTable.NoConvertBackForValueToIconConverter);
    }
  }
}
