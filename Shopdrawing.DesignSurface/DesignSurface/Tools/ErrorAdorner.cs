// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ErrorAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class ErrorAdorner : Adorner, IClickable
  {
    private const double BorderThickness = 4.0;
    private const double Margin = 12.0;
    private const double IconSize = 24.0;

    protected ErrorAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.ElementBounds.TopLeft * matrix;
    }

    internal static Brush CreateErrorAdornerBrush(Color borderBrushColor)
    {
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      linearGradientBrush.StartPoint = new Point(-0.377, 318.681);
      linearGradientBrush.EndPoint = new Point(1.885, 316.767);
      linearGradientBrush.SpreadMethod = GradientSpreadMethod.Repeat;
      linearGradientBrush.MappingMode = BrushMappingMode.Absolute;
      linearGradientBrush.GradientStops.Add(new GradientStop(borderBrushColor, 0.389));
      linearGradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0.399));
      linearGradientBrush.Freeze();
      return (Brush) linearGradientBrush;
    }

    protected void DrawAdorner(DrawingContext context, Matrix matrix, SceneElement adornedElement, Brush borderBrush, DrawingImage icon, string message)
    {
      Rect actualBounds = adornedElement.ViewModel.DefaultView.GetActualBounds(adornedElement.ViewTargetElement);
      if (actualBounds.IsEmpty)
        return;
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      System.Windows.Media.Geometry clipGeometry = (System.Windows.Media.Geometry)new RectangleGeometry(actualBounds);
      clipGeometry.Freeze();
      context.PushOpacity(0.5);
      System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(actualBounds, matrix, 4.0);
      context.DrawGeometry((Brush) Brushes.White, (Pen) null, rectangleGeometry);
      context.Pop();
      context.PushTransform((Transform) matrixTransform);
      context.PushClip(clipGeometry);
      double num1 = 1.0 / this.DesignerContext.ActiveView.Zoom;
      double num2 = num1 * 12.0;
      double num3 = num1 * 24.0;
      Rect rectangle = new Rect(actualBounds.Left + num2, actualBounds.Top + num2, num3, num3);
      context.DrawImage((ImageSource) icon, rectangle);
      FontFamily fontFamily = (FontFamily) Application.Current.FindResource((object) SystemFonts.IconFontFamilyKey);
      double num4 = (double) Application.Current.FindResource((object) SystemFonts.IconFontSizeKey);
      Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      FormattedText formattedText = new FormattedText(message, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, num4 * num1, (Brush) Brushes.Black);
      formattedText.MaxTextWidth = Math.Min(3579139.0, Math.Max(0.0, actualBounds.Width - num3 - 3.0 * num2));
      Point origin = new Point(actualBounds.Left + num3 + 2.0 * num2, actualBounds.Top + num2 + Math.Max(0.0, (num3 - formattedText.Height) / 2.0));
      context.DrawText(formattedText, origin);
      context.Pop();
      context.Pop();
      context.PushOpacity(0.5);
      Pen pen = new Pen(borderBrush, 4.0);
      pen.Freeze();
      context.DrawGeometry((Brush) null, pen, rectangleGeometry);
      context.Pop();
    }
  }
}
