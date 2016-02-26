// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutLineAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class LayoutLineAdorner : LayoutAdorner
  {
    private int index;
    private bool dimmed;

    public GridElement Grid
    {
      get
      {
        return (GridElement) this.Element;
      }
    }

    public int Index
    {
      get
      {
        return this.index;
      }
    }

    public LayoutLineAdorner(AdornerSet adornerSet, bool isX, int index, bool dimmed)
      : base(adornerSet, isX)
    {
      this.index = index;
      this.dimmed = dimmed;
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(matrix, out pointBegin, out pointEnd);
      Pen pen = this.dimmed ? FeedbackHelper.GetThinPen(AdornerType.Inactive) : this.ThinPen;
      drawingContext.DrawLine(pen, pointBegin, pointEnd);
    }

    private void GetPoints(Matrix matrix, out Point pointBegin, out Point pointEnd)
    {
      pointBegin = new Point();
      pointEnd = new Point();
      Matrix matrix1 = this.TruncateMatrix(matrix);
      Vector vector1 = new Vector();
      Vector vector2 = new Vector();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = pointEnd.X = this.Grid.GetComputedPositionOfColumn(this.Index);
        pointBegin.Y = elementBounds.Top;
        pointEnd.Y = elementBounds.Bottom;
        if (!this.dimmed)
          vector1.Y = -8.0;
      }
      else
      {
        pointBegin.Y = pointEnd.Y = this.Grid.GetComputedPositionOfRow(this.Index);
        pointBegin.X = elementBounds.Left;
        pointEnd.X = elementBounds.Right;
        if (!this.dimmed)
          vector1.X = -8.0;
      }
      pointBegin = pointBegin * matrix + vector1 * matrix1;
      pointEnd = pointEnd * matrix + vector2 * matrix1;
    }
  }
}
