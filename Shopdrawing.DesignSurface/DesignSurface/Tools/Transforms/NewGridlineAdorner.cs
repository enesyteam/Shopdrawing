// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.NewGridlineAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class NewGridlineAdorner : LayoutAdorner, IClickable
  {
    private double position = double.NaN;
    private Point lastPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
    private static Brush NewGridlineBrush = (Brush) Brushes.Orange;
    private static Pen NewGridlinePen = new Pen(NewGridlineAdorner.NewGridlineBrush, 1.0);
    private bool isMouseOver;

    static NewGridlineAdorner()
    {
      NewGridlineAdorner.NewGridlinePen.Freeze();
    }

    public NewGridlineAdorner(AdornerSet adornerSet, bool isX)
      : base(adornerSet, isX)
    {
    }

    public override void OnMouseEnter()
    {
      this.isMouseOver = true;
      this.InvalidateRender();
      this.AdornerSet.Update();
    }

    public override void OnMouseLeave()
    {
      this.isMouseOver = false;
      this.InvalidateRender();
      this.AdornerSet.Update();
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
    {
      Point hitPoint = hitTestParams.HitPoint;
      if (Tolerances.AreClose(hitPoint, this.lastPoint))
        return base.HitTestCore(hitTestParams);
      this.lastPoint = hitPoint;
      double num;
      if (((LayoutBehavior) this.AdornerSet.Behavior).IsNewGridlineEnabled && Mouse.LeftButton != MouseButtonState.Pressed && Mouse.RightButton != MouseButtonState.Pressed)
      {
        Matrix inverseMatrix = ElementUtilities.GetInverseMatrix(this.AdornerSet.Matrix);
        Point point = hitPoint * inverseMatrix;
        Point pointBegin;
        Point pointEnd;
        this.GetPoints(out pointBegin, out pointEnd, this.AdornerSet.Matrix);
        num = !new Rect(pointBegin, pointEnd).Contains(point) ? double.NaN : (!this.IsX ? point.Y : point.X);
      }
      else
        num = double.NaN;
      if (!this.position.Equals(num))
      {
        this.position = num;
        if (this.isMouseOver)
        {
          this.InvalidateRender();
          this.AdornerSet.Update();
        }
      }
      return base.HitTestCore(hitTestParams);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix);
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      ctx.PushTransform((Transform) matrixTransform);
      ctx.PushOpacity(0.25);
      ctx.DrawRectangle(this.ActiveBrush, (Pen) null, new Rect(pointBegin, pointEnd));
      ctx.Pop();
      ctx.Pop();
      if (!this.isMouseOver || double.IsNaN(this.position))
        return;
      this.GetLinePoints(matrix, out pointBegin, out pointEnd);
      ctx.DrawLine(NewGridlineAdorner.NewGridlinePen, pointBegin, pointEnd);
      this.DrawEndpoint(ctx, NewGridlineAdorner.NewGridlineBrush, NewGridlineAdorner.NewGridlinePen, pointBegin, matrix);
    }

    private void DrawEndpoint(DrawingContext drawingContext, Brush brush, Pen pen, Point point, Matrix matrix)
    {
      Matrix matrix1 = this.TruncateMatrix(matrix);
      matrix1.Translate(point.X, point.Y);
      if (!this.IsX)
        matrix1.RotatePrepend(-90.0);
      MatrixTransform matrixTransform = new MatrixTransform(matrix1);
      matrixTransform.Freeze();
      drawingContext.PushTransform((Transform) matrixTransform);
      drawingContext.DrawGeometry(brush, pen, LayoutAdorner.TriangleGeometry);
      drawingContext.Pop();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix);
      if (this.IsX)
        return new Point(0.75 * pointBegin.X + 0.25 * pointEnd.X, 0.5 * (pointBegin.Y + pointEnd.Y)) * matrix;
      return new Point(0.5 * (pointBegin.X + pointEnd.X), 0.75 * pointBegin.Y + 0.25 * pointEnd.Y) * matrix;
    }

    protected void GetPoints(out Point pointBegin, out Point pointEnd, Matrix matrix)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      pointBegin = new Point();
      pointEnd = new Point();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = elementBounds.Left;
        pointEnd.X = elementBounds.Right;
        pointBegin.Y = elementBounds.Top - 17.0 / canonicalTransform.ScaleY;
        pointEnd.Y = elementBounds.Top - 8.0 / canonicalTransform.ScaleY;
      }
      else
      {
        pointBegin.Y = elementBounds.Top;
        pointEnd.Y = elementBounds.Bottom;
        pointBegin.X = elementBounds.Left - 17.0 / canonicalTransform.ScaleX;
        pointEnd.X = elementBounds.Left - 8.0 / canonicalTransform.ScaleX;
      }
    }

    protected void GetLinePoints(Matrix matrix, out Point pointBegin, out Point pointEnd)
    {
      pointBegin = new Point();
      pointEnd = new Point();
      Matrix matrix1 = this.TruncateMatrix(matrix);
      Vector vector1 = new Vector();
      Vector vector2 = new Vector();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = pointEnd.X = this.position;
        pointBegin.Y = elementBounds.Top;
        pointEnd.Y = elementBounds.Bottom;
        vector1.Y = -12.5;
      }
      else
      {
        pointBegin.Y = pointEnd.Y = this.position;
        pointBegin.X = elementBounds.Left;
        pointEnd.X = elementBounds.Right;
        vector1.X = -12.5;
      }
      pointBegin = pointBegin * matrix + vector1 * matrix1;
      pointEnd = pointEnd * matrix + vector2 * matrix1;
    }
  }
}
