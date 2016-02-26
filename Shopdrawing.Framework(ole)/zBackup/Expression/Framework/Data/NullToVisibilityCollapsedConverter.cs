// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.NullToVisibilityCollapsedConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public sealed class NullToVisibilityCollapsedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (parameter != null)
        flag = bool.Parse((string) parameter);
      if (flag)
        return (object) (Visibility) (value != null ? 2 : 0);
      return (object) (Visibility) (value != null ? 0 : 2);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
