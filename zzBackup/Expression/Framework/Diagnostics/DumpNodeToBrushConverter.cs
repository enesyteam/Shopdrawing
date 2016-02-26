// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.DumpNodeToBrushConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class DumpNodeToBrushConverter : IValueConverter
  {
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      DumpNode dumpNode = (DumpNode) value;
      if (value == null || dumpNode.Visual is Window)
        return (object) null;
      VisualBrush visualBrush = new VisualBrush(dumpNode.Visual);
      visualBrush.Stretch = Stretch.Uniform;
      return (object) visualBrush;
    }
  }
}
