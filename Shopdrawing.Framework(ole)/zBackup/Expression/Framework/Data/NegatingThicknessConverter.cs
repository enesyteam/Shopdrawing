// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.NegatingThicknessConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.Framework.Data
{
  public class NegatingThicknessConverter : IValueConverter
  {
    public Thickness Offset { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Thickness))
        return value;
      Thickness thickness = (Thickness) value;
      return (object) new Thickness(-thickness.Left - this.Offset.Left, -thickness.Top - this.Offset.Top, -thickness.Right - this.Offset.Right, -thickness.Bottom - this.Offset.Bottom);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
