// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutEmptyBackgroundHeaderAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class LayoutEmptyBackgroundHeaderAdorner : LayoutBackgroundAdorner, IClickable
  {
    public LayoutEmptyBackgroundHeaderAdorner(AdornerSet adornerSet, bool isX)
      : base(adornerSet, isX, 0)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix);
      if (this.IsX)
        return new Point((pointBegin.X + pointEnd.X) / 2.0, pointEnd.Y - 5.0) * matrix;
      return new Point(pointEnd.X - 5.0, (pointBegin.Y + pointEnd.Y) / 2.0) * matrix;
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix);
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      drawingContext.PushTransform((Transform) matrixTransform);
      drawingContext.DrawRectangle((Brush) LayoutBackgroundAdorner.UnselectedBrush, (Pen) null, new Rect(pointBegin, pointEnd));
      drawingContext.Pop();
    }

    private void GetPoints(out Point pointBegin, out Point pointEnd, Matrix matrix)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      pointBegin = new Point();
      pointEnd = new Point();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = elementBounds.Left;
        pointEnd.X = elementBounds.Right;
        pointBegin.Y = elementBounds.Top - 8.0 / canonicalTransform.ScaleY;
        pointEnd.Y = elementBounds.Top - 1.0 / canonicalTransform.ScaleY;
      }
      else
      {
        pointBegin.Y = elementBounds.Top;
        pointEnd.Y = elementBounds.Bottom;
        pointBegin.X = elementBounds.Left - 8.0 / canonicalTransform.ScaleX;
        pointEnd.X = elementBounds.Left - 1.0 / canonicalTransform.ScaleX;
      }
    }
  }
}
