// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.BaseGradientConverter
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
  public abstract class BaseGradientConverter : IValueConverter
  {
    protected abstract void UpdateModelForGradientMin(ColorModel model);

    protected abstract void UpdateModelForGradientMax(ColorModel model);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Brush brush1 = (Brush) null;
      ColorModel source = value as ColorModel;
      if (source != null)
      {
        ColorModel model = new ColorModel(source);
        model.ScA = 1f;
        this.UpdateModelForGradientMin(model);
        GradientStop gradientStop1 = new GradientStop(model.Color, 0.0);
        gradientStop1.Freeze();
        this.UpdateModelForGradientMax(model);
        GradientStop gradientStop2 = new GradientStop(model.Color, 1.0);
        gradientStop2.Freeze();
        GradientStopCollection gradientStopCollection = new GradientStopCollection(2);
        gradientStopCollection.Add(gradientStop1);
        gradientStopCollection.Add(gradientStop2);
        gradientStopCollection.Freeze();
        LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
        linearGradientBrush.StartPoint = new Point(0.0, 0.0);
        linearGradientBrush.EndPoint = new Point(1.0, 0.0);
        linearGradientBrush.GradientStops = gradientStopCollection;
        linearGradientBrush.Freeze();
        brush1 = (Brush) linearGradientBrush;
        Brush brush2 = parameter as Brush;
        if (brush2 != null)
        {
          brush2.Freeze();
          RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 100.0, 10.0));
          rectangleGeometry.Freeze();
          GeometryDrawing geometryDrawing1 = new GeometryDrawing(brush2, (Pen) null, (Geometry) rectangleGeometry);
          brush2.Freeze();
          GeometryDrawing geometryDrawing2 = new GeometryDrawing(brush1, (Pen) null, (Geometry) rectangleGeometry);
          geometryDrawing2.Freeze();
          DrawingGroup drawingGroup = new DrawingGroup();
          drawingGroup.Children.Add((Drawing) geometryDrawing1);
          drawingGroup.Children.Add((Drawing) geometryDrawing2);
          drawingGroup.Freeze();
          DrawingBrush drawingBrush = new DrawingBrush((Drawing) drawingGroup);
          drawingBrush.Freeze();
          brush1 = (Brush) drawingBrush;
        }
      }
      return (object) brush1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
