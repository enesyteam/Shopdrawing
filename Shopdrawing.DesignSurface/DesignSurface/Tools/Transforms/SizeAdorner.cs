// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.SizeAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.Tools.Path;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class SizeAdorner : AnchorPointAdorner, IClickable
  {
    private static readonly Brush dimensionBackgroundBrush = (Brush) Brushes.White;
    private bool shouldDraw = true;
    private const double size = 5.0;
    private const double dimensionTextMargin = 4.0;
    private const double dimensionLineOffset = 17.5;
    private const double dimensionTextExtraOffset = 10.0;
    private const double markStartOffset = 7.0;
    private const double markEndOffset = 22.0;

    public static double Size
    {
      get
      {
        return 5.0;
      }
    }

    protected override Rect TargetRect
    {
      get
      {
        ISubElementAdornerSet elementAdornerSet = this.AdornerSet as ISubElementAdornerSet;
        if (elementAdornerSet == null)
          return base.TargetRect;
        return elementAdornerSet.TargetRect;
      }
    }

    public SizeAdorner(AdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!(eventArgs.PropertyName == "SizeAdornerVisibility"))
        return;
      this.shouldDraw = (bool) eventArgs.Value;
      this.InvalidateRender();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.GetOffsetAnchorPoint(matrix, 2.5);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.shouldDraw)
        return;
      bool flag = this.AdornerSet is RectangleGeometryAdornerSetBase;
      Point anchorPoint = this.GetAnchorPoint(matrix);
      Brush brush = this.IsActive ? (flag ? FeedbackHelper.GetActiveBrush(AdornerType.ClipPath) : this.ActiveBrush) : this.InactiveBrush;
      Pen pen = flag ? FeedbackHelper.GetThinPen(AdornerType.ClipPath) : this.ThinPen;
      Rect targetRect = this.TargetRect;
      AdornerRenderLocation location = AdornerRenderLocation.Outside;
      if (this.DesignerContext.SelectionManager.ElementSelectionSet != null && this.DesignerContext.SelectionManager.ElementSelectionSet.Selection.Count > 1 && !this.ElementSet.AdornsMultipleElements)
        location = AdornerRenderLocation.Inside;
      SizeAdorner.DrawSizeAdorner(ctx, anchorPoint, this.EdgeFlags, matrix, brush, pen, targetRect, location);
      if (!this.IsActive)
        return;
      Matrix transformMatrix = this.ElementSet.GetTransformMatrix(this.DesignerContext.ActiveView.ViewRoot);
      SizeAdorner.DrawDimensions(ctx, matrix, transformMatrix, pen, targetRect, this.EdgeFlags);
    }

    public static void DrawSizeAdorner(DrawingContext drawingContext, Point anchorPoint, EdgeFlags edgeFlags, Matrix matrix, Brush brush, Pen pen, Rect bounds, AdornerRenderLocation location)
    {
      if (SizeAdorner.SkipAdorner(edgeFlags, matrix, bounds))
        return;
      Vector vector1 = new Vector(1.0, 0.0) * matrix;
      Vector vector2 = new Vector(0.0, 1.0) * matrix;
      vector1.Normalize();
      vector2.Normalize();
      double num1 = matrix.Determinant < 0.0 ? -1.0 : 1.0;
      Point point = anchorPoint;
      Matrix matrix1 = new Matrix(0.0, 1.0 * num1, -1.0 * num1, 0.0, 0.0, 0.0);
      Matrix matrix2 = new Matrix(0.0, -1.0 * num1, 1.0 * num1, 0.0, 0.0, 0.0);
      Vector vector3 = vector1 * matrix1;
      Vector vector4 = vector2 * matrix2;
      Vector vector5 = vector1 * 8.0;
      Vector vector6 = vector2 * 8.0;
      if (location == AdornerRenderLocation.Inside)
      {
        switch (edgeFlags & EdgeFlags.TopOrBottom)
        {
          case EdgeFlags.None:
            vector1 = vector4;
            anchorPoint -= 0.5 * (5.0 - pen.Thickness) * vector2;
            point += vector2 * 0.5 * 8.0;
            break;
          case EdgeFlags.Top:
            vector2 = vector3;
            anchorPoint += 0.5 * pen.Thickness * vector2;
            point += vector2 * 8.0;
            break;
          case EdgeFlags.Bottom:
            vector2 = vector3;
            anchorPoint -= (5.0 - 0.5 * pen.Thickness) * vector2;
            break;
        }
        switch (edgeFlags & EdgeFlags.LeftOrRight)
        {
          case EdgeFlags.None:
            anchorPoint -= 0.5 * (5.0 - pen.Thickness) * vector1;
            point += vector1 * 0.5 * 8.0;
            break;
          case EdgeFlags.Left:
            anchorPoint += 0.5 * pen.Thickness * vector1;
            point += vector1 * 8.0;
            break;
          case EdgeFlags.Right:
            anchorPoint -= (5.0 - 0.5 * pen.Thickness) * vector1;
            break;
        }
      }
      else
      {
        switch (edgeFlags & EdgeFlags.TopOrBottom)
        {
          case EdgeFlags.None:
            vector1 = vector4;
            anchorPoint -= 0.5 * (5.0 - pen.Thickness) * vector2;
            point += vector2 * 0.5 * 8.0;
            break;
          case EdgeFlags.Top:
            vector2 = vector3;
            anchorPoint -= (5.0 - 0.5 * pen.Thickness) * vector2;
            break;
          case EdgeFlags.Bottom:
            vector2 = vector3;
            anchorPoint += 0.5 * pen.Thickness * vector2;
            point += vector2 * 8.0;
            break;
        }
        switch (edgeFlags & EdgeFlags.LeftOrRight)
        {
          case EdgeFlags.None:
            anchorPoint -= 0.5 * (5.0 - pen.Thickness) * vector1;
            point += vector1 * 0.5 * 8.0;
            break;
          case EdgeFlags.Left:
            anchorPoint -= (5.0 - 0.5 * pen.Thickness) * vector1;
            break;
          case EdgeFlags.Right:
            anchorPoint += 0.5 * pen.Thickness * vector1;
            point += vector1 * 8.0;
            break;
        }
      }
      StreamGeometry streamGeometry1 = new StreamGeometry();
      StreamGeometryContext streamGeometryContext1 = streamGeometry1.Open();
      streamGeometryContext1.BeginFigure(point - vector5, true, true);
      streamGeometryContext1.LineTo(point - vector5 - vector6, false, false);
      streamGeometryContext1.LineTo(point - vector6, false, false);
      streamGeometryContext1.LineTo(point, false, false);
      streamGeometryContext1.Close();
      streamGeometry1.Freeze();
      drawingContext.DrawGeometry((Brush)Brushes.Transparent, (Pen)null, (System.Windows.Media.Geometry)streamGeometry1);
      double num2 = 5.0 - pen.Thickness;
      Vector vector7 = vector1 * num2;
      Vector vector8 = vector2 * num2;
      StreamGeometry streamGeometry2 = new StreamGeometry();
      StreamGeometryContext streamGeometryContext2 = streamGeometry2.Open();
      streamGeometryContext2.BeginFigure(anchorPoint, true, true);
      streamGeometryContext2.LineTo(anchorPoint + vector8, true, false);
      streamGeometryContext2.LineTo(anchorPoint + vector7 + vector8, true, false);
      streamGeometryContext2.LineTo(anchorPoint + vector7, true, false);
      streamGeometryContext2.Close();
      streamGeometry2.Freeze();
      drawingContext.DrawGeometry(brush, pen, (System.Windows.Media.Geometry)streamGeometry2);
    }

    public static bool SkipAdorner(EdgeFlags edgeFlags, Matrix matrix, Rect bounds)
    {
      System.Windows.Size ofTransformedRect = Adorner.GetSizeOfTransformedRect(bounds, matrix);
      double num1 = 5.0;
      double num2 = 12.5;
      if (ofTransformedRect.Width < num1 && ofTransformedRect.Height < num1)
      {
        if (edgeFlags != EdgeFlags.BottomRight)
          return true;
      }
      else if (edgeFlags != EdgeFlags.Top && edgeFlags != EdgeFlags.Bottom && (edgeFlags != EdgeFlags.Right && ofTransformedRect.Width < num1) || edgeFlags != EdgeFlags.Left && edgeFlags != EdgeFlags.Right && (edgeFlags != EdgeFlags.Bottom && ofTransformedRect.Height < num1) || ((edgeFlags == EdgeFlags.Top || edgeFlags == EdgeFlags.Bottom) && (ofTransformedRect.Width < num2 && ofTransformedRect.Width >= num1) || (edgeFlags == EdgeFlags.Left || edgeFlags == EdgeFlags.Right) && (ofTransformedRect.Height < num2 && ofTransformedRect.Height >= num1)))
        return true;
      return false;
    }

    public static void DrawDimensions(DrawingContext drawingContext, Matrix matrix, Matrix rootMatrix, Pen pen, Rect bounds, EdgeFlags edgeFlags)
    {
      if ((edgeFlags & EdgeFlags.LeftOrRight) != EdgeFlags.None)
      {
        ElementLayoutAdornerType edge = (edgeFlags & EdgeFlags.Top) != EdgeFlags.None ? ElementLayoutAdornerType.Top : ElementLayoutAdornerType.Bottom;
        SizeAdorner.DrawDimension(drawingContext, edge, matrix, rootMatrix, pen, bounds, 1.0);
      }
      if ((edgeFlags & EdgeFlags.TopOrBottom) == EdgeFlags.None)
        return;
      ElementLayoutAdornerType edge1 = (edgeFlags & EdgeFlags.Left) != EdgeFlags.None ? ElementLayoutAdornerType.Left : ElementLayoutAdornerType.Right;
      SizeAdorner.DrawDimension(drawingContext, edge1, matrix, rootMatrix, pen, bounds, 1.0);
    }

    public static void DrawDimension(DrawingContext drawingContext, ElementLayoutAdornerType edge, Matrix matrix, Matrix rootMatrix, Pen pen, Rect bounds, double scale)
    {
      Point point1;
      Point point2;
      switch (edge)
      {
        case ElementLayoutAdornerType.Left:
          point1 = bounds.TopLeft;
          point2 = bounds.BottomLeft;
          break;
        case ElementLayoutAdornerType.Top:
          point1 = bounds.TopLeft;
          point2 = bounds.TopRight;
          break;
        case ElementLayoutAdornerType.Right:
          point1 = bounds.TopRight;
          point2 = bounds.BottomRight;
          break;
        case ElementLayoutAdornerType.Bottom:
          point1 = bounds.BottomLeft;
          point2 = bounds.BottomRight;
          break;
        default:
          throw new NotSupportedException();
      }
      double length = ((point2 - point1) * rootMatrix).Length;
      Point point3 = bounds.TopLeft + 0.5 * (bounds.BottomRight - bounds.TopLeft);
      Point point4 = point1 * matrix;
      Point point5 = point2 * matrix;
      Point point6 = point3 * matrix;
      Vector vector1 = point4 + 0.5 * (point5 - point4) - point6;
      vector1.Normalize();
      Vector vector2 = point5 - point4;
      vector2.Normalize();
      Vector overflowDirection = new Vector(vector2.Y, -vector2.X);
      if (overflowDirection * vector1 < 0.0)
        overflowDirection = -overflowDirection;
      Point point7 = point4 - 0.5 * pen.Thickness * vector2;
      Point point8 = point5 + 0.5 * pen.Thickness * vector2;
      Vector vector3 = scale * 7.0 * overflowDirection;
      Vector vector4 = scale * 22.0 * overflowDirection;
      drawingContext.DrawLine(pen, point7 + vector3, point7 + vector4);
      drawingContext.DrawLine(pen, point8 + vector3, point8 + vector4);
      Point a = point4 + scale * 17.5 * overflowDirection;
      Point b = point5 + scale * 17.5 * overflowDirection;
      string text = length.ToString("0.###", (IFormatProvider) CultureInfo.CurrentCulture);
      SizeAdorner.DrawLineAndText(drawingContext, a, b, overflowDirection, pen, pen.Brush, text, scale);
    }

    public static void DrawLineAndText(DrawingContext drawingContext, Point a, Point b, Vector overflowDirection, Pen pen, Brush textBrush, string text, double scale)
    {
      if (string.IsNullOrEmpty(text))
      {
        drawingContext.DrawLine(pen, a, b);
      }
      else
      {
        Vector vector1 = b - a;
        Vector vector2 = new Vector(vector1.Y, -vector1.X);
        vector2.Normalize();
        vector2.X = Math.Round(vector2.X, 3);
        vector2.Y = Math.Round(vector2.Y, 3);
        if (vector2.Y > 0.0 || vector2.Y == 0.0 && vector2.X < 0.0)
          vector2 = -vector2;
        FontFamily fontFamily = (FontFamily) Application.Current.FindResource((object) SystemFonts.IconFontFamilyKey);
        double num1 = (double) Application.Current.FindResource((object) SystemFonts.IconFontSizeKey);
        Typeface typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, scale * num1, textBrush);
        Vector vector3 = new Vector(-vector2.Y, vector2.X);
        double width = formattedText.Width;
        double height = formattedText.Height;
        Point point1 = new Point(0.5 * (a.X + b.X), 0.5 * (a.Y + b.Y));
        Point point2 = point1 - 0.5 * (vector3 * width - vector2 * height);
        double num2 = scale * 4.0;
        if (vector1.Length < width + 6.0 * num2)
        {
          drawingContext.DrawLine(pen, a, b);
          if (Math.Abs(overflowDirection * vector2) > 0.01)
            point2 += scale * 10.0 * overflowDirection;
          else
            point2 = b + 0.5 * width * (overflowDirection - vector3) + 4.0 * num2 * overflowDirection + 0.5 * height * vector2;
        }
        else
        {
          vector1.Normalize();
          vector1 *= 0.5 * width + num2;
          drawingContext.DrawLine(pen, a, point1 - vector1);
          drawingContext.DrawLine(pen, point1 + vector1, b);
        }
        if (vector2.Y == 0.0)
        {
          double num3 = -vector2.X * formattedText.Baseline;
          point2.X = Math.Round(point2.X + num3) - num3;
        }
        Matrix matrix = new Matrix(vector3.X, vector3.Y, -vector2.X, -vector2.Y, point2.X, point2.Y);
        drawingContext.PushTransform((Transform) new MatrixTransform(matrix));
        drawingContext.DrawRectangle(SizeAdorner.dimensionBackgroundBrush, (Pen) null, new Rect(-0.5 * num2, 0.0, width + num2, height));
        drawingContext.DrawText(formattedText, new Point());
        drawingContext.Pop();
      }
    }
  }
}
