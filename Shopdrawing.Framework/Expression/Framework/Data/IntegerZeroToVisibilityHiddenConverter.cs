// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.IntegerZeroToVisibilityHiddenConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class IntegerZeroToVisibilityHiddenConverter : IValueConverter
  {
    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      if (o is int && (int) o == 0)
        return (object) Visibility.Hidden;
      return (object) Visibility.Visible;
    }
  }
}
