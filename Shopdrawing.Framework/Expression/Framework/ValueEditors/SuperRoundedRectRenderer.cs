// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.SuperRoundedRectRenderer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors
{
  public class SuperRoundedRectRenderer
  {
    private static Thickness zeroThickness = new Thickness(0.0);
    private StreamGeometry geometry;

    public bool IsValid
    {
      get
      {
        return this.geometry != null;
      }
    }

    public void Render(DrawingContext dc, Rect renderRect, Brush brush, CornerRadius cornerRadius)
    {
      if (brush == null || Tolerances.NearZero(renderRect.Width) || Tolerances.NearZero(renderRect.Height))
        return;
      this.ValidateGeometry(renderRect, cornerRadius);
      dc.DrawGeometry(brush, (Pen) null, (Geometry) this.geometry);
    }

    public void InvalidateGeometry()
    {
      this.geometry = (StreamGeometry) null;
    }

    public void ValidateGeometry(Rect renderRect, CornerRadius cornerRadius)
    {
      if (this.geometry != null)
        return;
      this.geometry = new StreamGeometry();
      StreamGeometryContext ctx = this.geometry.Open();
      SuperRoundedRectRenderer.GenerateGeometry(ctx, renderRect, new SuperRoundedRectRenderer.Radii(cornerRadius, SuperRoundedRectRenderer.zeroThickness, false));
      ctx.Close();
      this.geometry.Freeze();
    }

    private static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, SuperRoundedRectRenderer.Radii radii)
    {
      Point point1 = new Point(radii.LeftTop, 0.0);
      Point point2 = new Point(rect.Width - radii.RightTop, 0.0);
      Point point3 = new Point(rect.Width, radii.TopRight);
      Point point4 = new Point(rect.Width, rect.Height - radii.BottomRight);
      Point point5 = new Point(rect.Width - radii.RightBottom, rect.Height);
      Point point6 = new Point(radii.LeftBottom, rect.Height);
      Point point7 = new Point(0.0, rect.Height - radii.BottomLeft);
      Point point8 = new Point(0.0, radii.TopLeft);
      if (point1.X > point2.X)
      {
        double num = radii.LeftTop / (radii.LeftTop + radii.RightTop) * rect.Width;
        point1.X = num;
        point2.X = num;
      }
      if (point3.Y > point4.Y)
      {
        double num = radii.TopRight / (radii.TopRight + radii.BottomRight) * rect.Height;
        point3.Y = num;
        point4.Y = num;
      }
      if (point5.X < point6.X)
      {
        double num = radii.LeftBottom / (radii.LeftBottom + radii.RightBottom) * rect.Width;
        point5.X = num;
        point6.X = num;
      }
      if (point7.Y < point8.Y)
      {
        double num = radii.TopLeft / (radii.TopLeft + radii.BottomLeft) * rect.Height;
        point7.Y = num;
        point8.Y = num;
      }
      Vector vector = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
      point1 += vector;
      point2 += vector;
      point3 += vector;
      point4 += vector;
      point5 += vector;
      point6 += vector;
      point7 += vector;
      point8 += vector;
      ctx.BeginFigure(point1, true, true);
      ctx.LineTo(point2, true, false);
      double num1 = rect.TopRight.X - point2.X;
      double num2 = point3.Y - rect.TopRight.Y;
      if (!Tolerances.NearZero(num1) || !Tolerances.NearZero(num2))
        ctx.ArcTo(point3, new Size(num1, num2), 0.0, false, SweepDirection.Clockwise, true, false);
      ctx.LineTo(point4, true, false);
      double num3 = rect.BottomRight.X - point5.X;
      double num4 = rect.BottomRight.Y - point4.Y;
      if (!Tolerances.NearZero(num3) || !Tolerances.NearZero(num4))
        ctx.ArcTo(point5, new Size(num3, num4), 0.0, false, SweepDirection.Clockwise, true, false);
      ctx.LineTo(point6, true, false);
      double num5 = point6.X - rect.BottomLeft.X;
      double num6 = rect.BottomLeft.Y - point7.Y;
      if (!Tolerances.NearZero(num5) || !Tolerances.NearZero(num6))
        ctx.ArcTo(point7, new Size(num5, num6), 0.0, false, SweepDirection.Clockwise, true, false);
      ctx.LineTo(point8, true, false);
      double num7 = point1.X - rect.TopLeft.X;
      double num8 = point8.Y - rect.TopLeft.Y;
      if (Tolerances.NearZero(num7) && Tolerances.NearZero(num8))
        return;
      ctx.ArcTo(point1, new Size(num7, num8), 0.0, false, SweepDirection.Clockwise, true, false);
    }

    private struct Radii
    {
      internal double LeftTop;
      internal double TopLeft;
      internal double TopRight;
      internal double RightTop;
      internal double RightBottom;
      internal double BottomRight;
      internal double BottomLeft;
      internal double LeftBottom;

      internal Radii(CornerRadius radii, Thickness borders, bool outer)
      {
        double num1 = 0.5 * borders.Left;
        double num2 = 0.5 * borders.Top;
        double num3 = 0.5 * borders.Right;
        double num4 = 0.5 * borders.Bottom;
        if (outer)
        {
          if (Tolerances.NearZero(radii.TopLeft))
          {
            this.LeftTop = this.TopLeft = 0.0;
          }
          else
          {
            this.LeftTop = radii.TopLeft + num1;
            this.TopLeft = radii.TopLeft + num2;
          }
          if (Tolerances.NearZero(radii.TopRight))
          {
            this.TopRight = this.RightTop = 0.0;
          }
          else
          {
            this.TopRight = radii.TopRight + num2;
            this.RightTop = radii.TopRight + num3;
          }
          if (Tolerances.NearZero(radii.BottomRight))
          {
            this.RightBottom = this.BottomRight = 0.0;
          }
          else
          {
            this.RightBottom = radii.BottomRight + num3;
            this.BottomRight = radii.BottomRight + num4;
          }
          if (Tolerances.NearZero(radii.BottomLeft))
          {
            this.BottomLeft = this.LeftBottom = 0.0;
          }
          else
          {
            this.BottomLeft = radii.BottomLeft + num4;
            this.LeftBottom = radii.BottomLeft + num1;
          }
        }
        else
        {
          this.LeftTop = Math.Max(0.0, radii.TopLeft - num1);
          this.TopLeft = Math.Max(0.0, radii.TopLeft - num2);
          this.TopRight = Math.Max(0.0, radii.TopRight - num2);
          this.RightTop = Math.Max(0.0, radii.TopRight - num3);
          this.RightBottom = Math.Max(0.0, radii.BottomRight - num3);
          this.BottomRight = Math.Max(0.0, radii.BottomRight - num4);
          this.BottomLeft = Math.Max(0.0, radii.BottomLeft - num4);
          this.LeftBottom = Math.Max(0.0, radii.BottomLeft - num1);
        }
      }
    }
  }
}
