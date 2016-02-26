// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutLineHeaderAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public sealed class LayoutLineHeaderAdorner : LayoutAdorner, IClickable
  {
    private int index;

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

    public LayoutLineHeaderAdorner(AdornerSet adornerSet, bool isX, int index)
      : base(adornerSet, isX)
    {
      this.index = index;
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.GetCenter(matrix, this.TruncateMatrix(matrix));
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      bool flag;
      if (this.IsX)
      {
        GridColumnSelectionSet columnSelectionSet = ((GridRowColumnAdornerSet) this.AdornerSet).GridColumnSelectionSet;
        flag = columnSelectionSet.GridLineMode && columnSelectionSet.IsSelected(this.Grid.ColumnDefinitions[this.Index]);
      }
      else
      {
        GridRowSelectionSet gridRowSelectionSet = ((GridRowColumnAdornerSet) this.AdornerSet).GridRowSelectionSet;
        flag = gridRowSelectionSet.GridLineMode && gridRowSelectionSet.IsSelected(this.Grid.RowDefinitions[this.Index]);
      }
      Brush brush = flag ? this.ActiveBrush : this.InactiveBrush;
      Matrix matrix1 = this.TruncateMatrix(matrix);
      Point center = this.GetCenter(matrix, matrix1);
      matrix1.Translate(center.X, center.Y);
      if (!this.IsX)
        matrix1.RotatePrepend(-90.0);
      MatrixTransform matrixTransform = new MatrixTransform(matrix1);
      matrixTransform.Freeze();
      drawingContext.PushTransform((Transform) matrixTransform);
      drawingContext.DrawGeometry(brush, this.ThinPen, LayoutAdorner.TriangleGeometry);
      drawingContext.Pop();
    }

    protected override void OnIsActiveChanged()
    {
    }

    private Point GetCenter(Matrix matrix, Matrix truncatedMatrix)
    {
      Point point = new Point();
      Vector vector = new Vector();
      if (this.IsX)
      {
        point.X = this.Grid.GetComputedPositionOfColumn(this.Index);
        vector.Y = this.ElementBounds.Top - 8.0 - 4.5;
      }
      else
      {
        point.Y = this.Grid.GetComputedPositionOfRow(this.Index);
        vector.X = this.ElementBounds.Left - 8.0 - 4.5;
      }
      return point * matrix + vector * truncatedMatrix;
    }
  }
}
