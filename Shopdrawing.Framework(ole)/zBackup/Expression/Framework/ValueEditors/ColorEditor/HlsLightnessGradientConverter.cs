// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.HlsLightnessGradientConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  public class HlsLightnessGradientConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      LinearGradientBrush linearGradientBrush = (LinearGradientBrush) null;
      ColorModel source = value as ColorModel;
      if (source != null)
      {
        ColorModel colorModel = new ColorModel(source);
        colorModel.ScA = 1f;
        colorModel.HlsS = 1f;
        colorModel.HlsL = 0.5f;
        GradientStop gradientStop1 = new GradientStop(Colors.Black, 0.0);
        gradientStop1.Freeze();
        GradientStop gradientStop2 = new GradientStop(colorModel.Color, 0.5);
        gradientStop2.Freeze();
        GradientStop gradientStop3 = new GradientStop(Colors.White, 1.0);
        gradientStop3.Freeze();
        GradientStopCollection gradientStopCollection = new GradientStopCollection(3);
        gradientStopCollection.Add(gradientStop1);
        gradientStopCollection.Add(gradientStop2);
        gradientStopCollection.Add(gradientStop3);
        gradientStopCollection.Freeze();
        linearGradientBrush = new LinearGradientBrush();
        linearGradientBrush.StartPoint = new Point(0.0, 0.0);
        linearGradientBrush.EndPoint = new Point(1.0, 0.0);
        linearGradientBrush.GradientStops = gradientStopCollection;
        linearGradientBrush.Freeze();
      }
      return (object) linearGradientBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
