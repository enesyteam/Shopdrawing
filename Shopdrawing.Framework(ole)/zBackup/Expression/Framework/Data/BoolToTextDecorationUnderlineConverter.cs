// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.BoolToTextDecorationUnderlineConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class BoolToTextDecorationUnderlineConverter : IValueConverter
  {
    private static TextDecorationCollection UnderlineDecorations = TextDecorations.Underline;
    private static TextDecorationCollection emptyDecorations;

    private static TextDecorationCollection EmptyDecorations
    {
      get
      {
        if (BoolToTextDecorationUnderlineConverter.emptyDecorations == null)
        {
          BoolToTextDecorationUnderlineConverter.emptyDecorations = new TextDecorationCollection();
          BoolToTextDecorationUnderlineConverter.emptyDecorations.Freeze();
        }
        return BoolToTextDecorationUnderlineConverter.emptyDecorations;
      }
    }

    public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) null;
    }

    public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
    {
      if ((bool) o)
        return (object) BoolToTextDecorationUnderlineConverter.UnderlineDecorations;
      return (object) BoolToTextDecorationUnderlineConverter.EmptyDecorations;
    }
  }
}
