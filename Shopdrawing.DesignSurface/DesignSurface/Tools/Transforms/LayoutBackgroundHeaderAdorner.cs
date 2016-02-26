// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutBackgroundHeaderAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class LayoutBackgroundHeaderAdorner : LayoutBackgroundAdorner, IClickable
  {
    public LayoutBackgroundHeaderAdorner(AdornerSet adornerSet, bool isX, int index)
      : base(adornerSet, isX, index)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix, true);
      if (this.IsX)
        return new Point((pointBegin.X + pointEnd.X - 20.0) / 2.0, pointEnd.Y - 5.0) * matrix;
      return new Point(pointEnd.X - 5.0, (pointBegin.Y + pointEnd.Y - 20.0) / 2.0) * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      bool flag = false;
      if (this.IsX)
      {
        GridColumnSelectionSet columnSelectionSet = ((GridRowColumnAdornerSet) this.AdornerSet).GridColumnSelectionSet;
        if (columnSelectionSet != null)
          flag = !columnSelectionSet.GridLineMode && columnSelectionSet.IsSelected(this.Grid.ColumnDefinitions[this.Index]);
      }
      else
      {
        GridRowSelectionSet gridRowSelectionSet = ((GridRowColumnAdornerSet) this.AdornerSet).GridRowSelectionSet;
        if (gridRowSelectionSet != null)
          flag = !gridRowSelectionSet.GridLineMode && gridRowSelectionSet.IsSelected(this.Grid.RowDefinitions[this.Index]);
      }
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      ctx.PushTransform((Transform) matrixTransform);
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd, matrix, true);
      ctx.DrawRectangle(flag ? (Brush) LayoutBackgroundAdorner.SelectedBrush : (Brush) LayoutBackgroundAdorner.UnselectedBrush, (Pen) null, new Rect(pointBegin, pointEnd));
      this.GetPoints(out pointBegin, out pointEnd, matrix, false);
      ctx.DrawRectangle(flag ? (Brush) LayoutBackgroundAdorner.SelectedBrush : (Brush) LayoutBackgroundAdorner.UnselectedBrush, (Pen) null, new Rect(pointBegin, pointEnd));
      ctx.Pop();
    }

    private void GetPoints(out Point pointBegin, out Point pointEnd, Matrix matrix, bool outerRegion)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) new MatrixTransform(matrix));
      pointBegin = new Point();
      pointEnd = new Point();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = this.Grid.GetComputedPositionOfColumn(this.Index);
        pointEnd.X = this.Grid.GetComputedPositionOfColumn(this.Index + 1);
        if (outerRegion)
        {
          pointBegin.Y = elementBounds.Top - 37.0 / canonicalTransform.ScaleY;
          pointEnd.Y = elementBounds.Top - 17.0 / canonicalTransform.ScaleY;
        }
        else
        {
          pointBegin.Y = elementBounds.Top - 8.0 / canonicalTransform.ScaleY;
          pointEnd.Y = elementBounds.Top;
        }
      }
      else
      {
        pointBegin.Y = this.Grid.GetComputedPositionOfRow(this.Index);
        pointEnd.Y = this.Grid.GetComputedPositionOfRow(this.Index + 1);
        if (outerRegion)
        {
          pointBegin.X = elementBounds.Left - 37.0 / canonicalTransform.ScaleX;
          pointEnd.X = elementBounds.Left - 17.0 / canonicalTransform.ScaleX;
        }
        else
        {
          pointBegin.X = elementBounds.Left - 8.0 / canonicalTransform.ScaleX;
          pointEnd.X = elementBounds.Left;
        }
      }
    }
  }
}
