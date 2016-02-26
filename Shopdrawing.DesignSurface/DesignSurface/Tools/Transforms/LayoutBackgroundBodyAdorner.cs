// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutBackgroundBodyAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class LayoutBackgroundBodyAdorner : LayoutBackgroundAdorner
  {
    public LayoutBackgroundBodyAdorner(AdornerSet adornerSet, bool isX, int index)
      : base(adornerSet, isX, index)
    {
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      bool flag = false;
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd);
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
      ctx.DrawRectangle(flag ? (Brush) LayoutBackgroundAdorner.SelectedBrush : (Brush) LayoutBackgroundAdorner.UnselectedBrush, (Pen) null, new Rect(pointBegin, pointEnd));
      ctx.Pop();
    }

    private void GetPoints(out Point pointBegin, out Point pointEnd)
    {
      pointBegin = new Point();
      pointEnd = new Point();
      Rect elementBounds = this.ElementBounds;
      if (this.IsX)
      {
        pointBegin.X = this.Grid.GetComputedPositionOfColumn(this.Index);
        pointEnd.X = this.Grid.GetComputedPositionOfColumn(this.Index + 1);
        pointBegin.Y = elementBounds.Top;
        pointEnd.Y = elementBounds.Bottom;
      }
      else
      {
        pointBegin.Y = this.Grid.GetComputedPositionOfRow(this.Index);
        pointEnd.Y = this.Grid.GetComputedPositionOfRow(this.Index + 1);
        pointBegin.X = elementBounds.Left;
        pointEnd.X = elementBounds.Right;
      }
    }
  }
}
