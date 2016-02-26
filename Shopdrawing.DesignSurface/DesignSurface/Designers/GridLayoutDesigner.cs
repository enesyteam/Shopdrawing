// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.GridLayoutDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Layout;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class GridLayoutDesigner : LayoutDesigner
  {
    private static IPropertyId[] gridLayoutProperties = new IPropertyId[4]
    {
      GridElement.RowProperty,
      GridElement.ColumnProperty,
      GridElement.RowSpanProperty,
      GridElement.ColumnSpanProperty
    };

    public GridLayoutDesigner()
      : base((IEnumerable<IPropertyId>) GridLayoutDesigner.gridLayoutProperties)
    {
    }

    protected override LayoutOperation CreateLayoutOperation(BaseFrameworkElement child)
    {
      return (LayoutOperation) new GridLayoutOperation((ILayoutDesigner) this, child);
    }

    public override void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      if (!child.IsAttached)
        overridesToIgnore = LayoutOverrides.All;
      overridesToIgnore |= LayoutOverrides.GridBox;
      base.SetChildRect(child, this.PrepareLayoutRect(rect), layoutOverrides, overridesToIgnore, nonExplicitOverrides, setRectMode);
    }

    public override void SetRootSize(BaseFrameworkElement root, Size size, bool setWidth, bool setHeight)
    {
      root.ViewModel.DefaultView.UpdateLayout();
      if (!root.ViewModel.IsInGridDesignMode && root is GridElement)
      {
        size = RoundingHelper.RoundSize(size);
        using (GridLayoutDesigner.TryCanvasDesignMode(root, size, setWidth, setHeight))
        {
          BaseFrameworkElement sizeElement = this.GetSizeElement(root);
          if (setHeight)
            sizeElement.Height = size.Height;
          if (!setWidth)
            return;
          sizeElement.Width = size.Width;
        }
      }
      else
        base.SetRootSize(root, size, setWidth, setHeight);
    }

    public static IDisposable TryCanvasDesignMode(BaseFrameworkElement element, Size size, bool setWidth, bool setHeight)
    {
      GridElement grid = element as GridElement;
      if (grid != null && !grid.ViewModel.IsInGridDesignMode && !grid.ViewModel.OverrideGridDesignMode)
        return (IDisposable) new GridLayoutDesigner.CanvasDesignModeToken(grid, size, setWidth, setHeight);
      return (IDisposable) null;
    }

    private void AdjustLastColumnRow(GridElement grid, Size size, bool setWidth, bool setHeight)
    {
      Size size1 = this.GetSizeElement((BaseFrameworkElement) grid).GetComputedTightBounds().Size;
      Size size2 = grid.GetComputedTightBounds().Size;
      if (setWidth && grid.ColumnDefinitions.Count > 1)
      {
        double positionOfColumn = grid.GetComputedPositionOfColumn(grid.ColumnDefinitions.Count - 1);
        double num = Math.Max(0.0, size.Width - size1.Width + size2.Width);
        this.AdjustColumn(grid, grid.ColumnDefinitions.Count - 1, Math.Max(num - positionOfColumn, 0.0), true, false);
      }
      if (!setHeight || grid.RowDefinitions.Count <= 1)
        return;
      double computedPositionOfRow = grid.GetComputedPositionOfRow(grid.RowDefinitions.Count - 1);
      double num1 = Math.Max(0.0, size.Height - size1.Height + size2.Height);
      this.AdjustRow(grid, grid.RowDefinitions.Count - 1, Math.Max(num1 - computedPositionOfRow, 0.0), true, false);
    }

    private void SetColumnWidthCore(GridElement grid, int col, double width)
    {
      width = RoundingHelper.RoundLength(width);
      ColumnDefinitionNode columnDefinitionNode = grid.ColumnDefinitions[col];
      if (columnDefinitionNode.Width.IsAuto)
        columnDefinitionNode.MinWidth = width;
      else
        columnDefinitionNode.Width = new GridLength(width);
      grid.ComputedColumnWidthCache[col] = width;
    }

    private void SetGridWidthCore(GridElement grid, double width)
    {
      BaseFrameworkElement sizeElement = this.GetSizeElement((BaseFrameworkElement) grid);
      if (sizeElement.ViewModel.ViewRoot == sizeElement)
      {
        if (!double.IsNaN(sizeElement.Width))
        {
          sizeElement.Width = width;
        }
        else
        {
          if (double.IsNaN((double) sizeElement.GetLocalOrDefaultValue(DesignTimeProperties.DesignWidthProperty)))
            return;
          sizeElement.SetValue(DesignTimeProperties.DesignWidthProperty, (object) width);
        }
      }
      else
      {
        ILayoutDesigner designerForChild = sizeElement.ViewModel.GetLayoutDesignerForChild((SceneElement) sizeElement, true);
        Rect rect = designerForChild.GetChildRect(sizeElement);
        rect = new Rect(rect.Left, rect.Top, width, rect.Height);
        designerForChild.SetChildRect(sizeElement, rect, true, false);
      }
    }

    public void AdjustColumn(GridElement grid, int col, double width, bool adjustGridWidth, bool preserveRects)
    {
      double computedWidth1 = grid.ColumnDefinitions[col].ComputedWidth;
      double num = 0.0;
      grid.CacheComputedColumnWidths();
      List<int> widthStarIndices = this.GetWidthStarIndices(grid);
      List<LayoutCacheRecord> layoutCache = (List<LayoutCacheRecord>) null;
      if (preserveRects)
        layoutCache = this.GetCurrentRects((BaseFrameworkElement) grid);
      width = Math.Max(0.0, width);
      if (col + 1 < grid.ColumnDefinitions.Count)
      {
        num = grid.ColumnDefinitions[col + 1].ComputedWidth;
        width = Math.Min(computedWidth1 + num, width);
      }
      else if (!adjustGridWidth)
        return;
      using (this.DeferTokenForGridDesignMode(grid))
      {
        this.SetColumnWidthCore(grid, col, width);
        if (!adjustGridWidth)
        {
          double width1 = Math.Max(computedWidth1 + num - width, 0.0);
          this.SetColumnWidthCore(grid, col + 1, width1);
          grid.ViewModel.Document.OnUpdatedEditTransaction();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (Tolerances.GreaterThan(grid.ColumnDefinitions[col].ComputedWidth, width))
          {
            width = grid.ColumnDefinitions[col].ComputedWidth;
            double width2 = Math.Max(computedWidth1 + num - width, 0.0);
            this.SetColumnWidthCore(grid, col, width);
            this.SetColumnWidthCore(grid, col + 1, width2);
          }
          else if (Tolerances.GreaterThan(grid.ColumnDefinitions[col + 1].ComputedWidth, width1))
          {
            double computedWidth2 = grid.ColumnDefinitions[col + 1].ComputedWidth;
            width = Math.Max(computedWidth1 + num - computedWidth2, 0.0);
            this.SetColumnWidthCore(grid, col, width);
            this.SetColumnWidthCore(grid, col + 1, computedWidth2);
          }
        }
        else
        {
          double width1 = this.GetSizeElement((BaseFrameworkElement) grid).GetComputedTightBounds().Width;
          this.SetGridWidthCore(grid, Math.Max(width1 + width - computedWidth1, 0.0));
          grid.ViewModel.Document.OnUpdatedEditTransaction();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (Tolerances.GreaterThan(grid.ColumnDefinitions[col].ComputedWidth, width))
          {
            width = grid.ColumnDefinitions[col].ComputedWidth;
            double width2 = Math.Max(width1 + width - computedWidth1, 0.0);
            this.SetColumnWidthCore(grid, col, width);
            this.SetGridWidthCore(grid, width2);
          }
        }
        this.NormalizeWidthStars(grid, widthStarIndices);
        grid.UncacheComputedColumnWidths();
        if (!preserveRects)
          return;
        grid.ViewModel.Document.OnUpdatedEditTransaction();
        grid.ViewModel.DefaultView.UpdateLayout();
        this.SetCurrentRects((BaseFrameworkElement) grid, layoutCache);
      }
    }

    private void SetRowHeightCore(GridElement grid, int row, double height)
    {
      height = RoundingHelper.RoundLength(height);
      RowDefinitionNode rowDefinitionNode = grid.RowDefinitions[row];
      if (rowDefinitionNode.Height.IsAuto)
        rowDefinitionNode.MinHeight = height;
      else
        rowDefinitionNode.Height = new GridLength(height);
      grid.ComputedRowHeightCache[row] = height;
    }

    private void SetGridHeightCore(GridElement grid, double height)
    {
      BaseFrameworkElement sizeElement = this.GetSizeElement((BaseFrameworkElement) grid);
      if (sizeElement.ViewModel.RootNode == sizeElement)
      {
        if (!double.IsNaN(sizeElement.Height))
        {
          sizeElement.Height = height;
        }
        else
        {
          if (double.IsNaN((double) sizeElement.GetLocalOrDefaultValue(DesignTimeProperties.DesignHeightProperty)))
            return;
          sizeElement.SetValue(DesignTimeProperties.DesignHeightProperty, (object) height);
        }
      }
      else
      {
        ILayoutDesigner designerForChild = sizeElement.ViewModel.GetLayoutDesignerForChild((SceneElement) sizeElement, true);
        Rect rect = designerForChild.GetChildRect(sizeElement);
        rect = new Rect(rect.Left, rect.Top, rect.Width, height);
        designerForChild.SetChildRect(sizeElement, rect, false, true);
      }
    }

    public void AdjustRow(GridElement grid, int row, double height, bool adjustGridHeight, bool preserveRects)
    {
      double computedHeight1 = grid.RowDefinitions[row].ComputedHeight;
      double num = 0.0;
      grid.CacheComputedRowHeights();
      List<int> heightStarIndices = this.GetHeightStarIndices(grid);
      List<LayoutCacheRecord> layoutCache = (List<LayoutCacheRecord>) null;
      if (preserveRects)
        layoutCache = this.GetCurrentRects((BaseFrameworkElement) grid);
      height = Math.Max(0.0, height);
      if (row + 1 < grid.RowDefinitions.Count)
      {
        num = grid.RowDefinitions[row + 1].ComputedHeight;
        height = Math.Min(computedHeight1 + num, height);
      }
      else if (!adjustGridHeight)
        return;
      using (this.DeferTokenForGridDesignMode(grid))
      {
        this.SetRowHeightCore(grid, row, height);
        if (!adjustGridHeight)
        {
          double height1 = Math.Max(computedHeight1 + num - height, 0.0);
          this.SetRowHeightCore(grid, row + 1, height1);
          grid.ViewModel.Document.OnUpdatedEditTransaction();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (Tolerances.GreaterThan(grid.RowDefinitions[row].ComputedHeight, height))
          {
            height = grid.RowDefinitions[row].ComputedHeight;
            double height2 = Math.Max(computedHeight1 + num - height, 0.0);
            this.SetRowHeightCore(grid, row, height);
            this.SetRowHeightCore(grid, row + 1, height2);
          }
          else if (Tolerances.GreaterThan(grid.RowDefinitions[row + 1].ComputedHeight, height1))
          {
            double computedHeight2 = grid.RowDefinitions[row + 1].ComputedHeight;
            height = Math.Max(computedHeight1 + num - computedHeight2, 0.0);
            this.SetRowHeightCore(grid, row, height);
            this.SetRowHeightCore(grid, row + 1, computedHeight2);
          }
        }
        else
        {
          double height1 = this.GetSizeElement((BaseFrameworkElement) grid).GetComputedTightBounds().Height;
          this.SetGridHeightCore(grid, Math.Max(height1 + height - computedHeight1, 0.0));
          grid.ViewModel.Document.OnUpdatedEditTransaction();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (Tolerances.GreaterThan(grid.RowDefinitions[row].ComputedHeight, height))
          {
            height = grid.RowDefinitions[row].ComputedHeight;
            double height2 = Math.Max(height1 + height - computedHeight1, 0.0);
            this.SetRowHeightCore(grid, row, height);
            this.SetGridHeightCore(grid, height2);
          }
        }
        this.NormalizeHeightStars(grid, heightStarIndices);
        grid.UncacheComputedRowHeights();
        if (!preserveRects)
          return;
        grid.ViewModel.Document.OnUpdatedEditTransaction();
        grid.ViewModel.DefaultView.UpdateLayout();
        this.SetCurrentRects((BaseFrameworkElement) grid, layoutCache);
      }
    }

    private List<int> GetWidthStarIndices(GridElement gridElement)
    {
      List<int> list = new List<int>();
      for (int index = 0; index < gridElement.ColumnDefinitions.Count; ++index)
      {
        if (gridElement.ColumnDefinitions[index].Width.IsStar)
          list.Add(index);
      }
      return list;
    }

    private void NormalizeWidthStars(GridElement grid, List<int> widthStarIndices)
    {
      double num1 = 0.0;
      foreach (int column in widthStarIndices)
        num1 += grid.GetComputedColumnWidth(column);
      foreach (int column in widthStarIndices)
      {
        double num2 = RoundingHelper.RoundLength(num1 == 0.0 ? 1.0 : grid.GetComputedColumnWidth(column) / num1);
        grid.ColumnDefinitions[column].Width = new GridLength(num2, GridUnitType.Star);
      }
    }

    private List<int> GetHeightStarIndices(GridElement gridElement)
    {
      List<int> list = new List<int>();
      for (int index = 0; index < gridElement.RowDefinitions.Count; ++index)
      {
        if (gridElement.RowDefinitions[index].Height.IsStar)
          list.Add(index);
      }
      return list;
    }

    private void NormalizeHeightStars(GridElement grid, List<int> heightStarIndices)
    {
      double num1 = 0.0;
      foreach (int row in heightStarIndices)
        num1 += grid.GetComputedRowHeight(row);
      foreach (int row in heightStarIndices)
      {
        double num2 = RoundingHelper.RoundLength(num1 == 0.0 ? 1.0 : grid.GetComputedRowHeight(row) / num1);
        grid.RowDefinitions[row].Height = new GridLength(num2, GridUnitType.Star);
      }
    }

    private void EnsureOneColumn(GridElement grid)
    {
      if (grid.ColumnDefinitions.Count != 0)
        return;
      grid.ColumnDefinitions.Insert(0, ColumnDefinitionNode.Factory.Instantiate(grid.ViewModel));
      grid.ComputedColumnWidthCache.Insert(0, grid.GetComputedTightBounds().Width);
    }

    private void EnsureOneRow(GridElement grid)
    {
      if (grid.RowDefinitions.Count != 0)
        return;
      grid.RowDefinitions.Insert(0, RowDefinitionNode.Factory.Instantiate(grid.ViewModel));
      grid.ComputedRowHeightCache.Insert(0, grid.GetComputedTightBounds().Height);
    }

    public void AddVerticalGridline(GridElement grid, double position)
    {
      using (SceneEditTransaction editTransaction = grid.ViewModel.CreateEditTransaction(StringTable.UndoUnitAddGridline))
      {
        using (grid.ViewModel.ForceBaseValue())
        {
          List<LayoutCacheRecord> currentRects = this.GetCurrentRects((BaseFrameworkElement) grid);
          int columnBeforePosition = grid.GetComputedColumnBeforePosition(position);
          double positionOfColumn1 = grid.GetComputedPositionOfColumn(columnBeforePosition);
          double positionOfColumn2 = grid.GetComputedPositionOfColumn(columnBeforePosition + 1);
          bool flag = grid.ColumnDefinitions.Count == 0 || grid.ColumnDefinitions[columnBeforePosition].Width.IsStar;
          grid.CacheComputedColumnWidths();
          this.EnsureOneColumn(grid);
          grid.ColumnDefinitions.Insert(columnBeforePosition + 1, ColumnDefinitionNode.Factory.Instantiate(grid.ViewModel));
          position = Math.Max(position, positionOfColumn1);
          if (position > positionOfColumn2)
          {
            grid.ComputedColumnWidthCache[columnBeforePosition] = positionOfColumn2 - positionOfColumn1;
            grid.ComputedColumnWidthCache.Insert(columnBeforePosition + 1, position - positionOfColumn2);
          }
          else
          {
            grid.ComputedColumnWidthCache[columnBeforePosition] = position - positionOfColumn1;
            grid.ComputedColumnWidthCache.Insert(columnBeforePosition + 1, positionOfColumn2 - position);
          }
          editTransaction.Update();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (flag)
          {
            List<int> widthStarIndices = this.GetWidthStarIndices(grid);
            this.NormalizeWidthStars(grid, widthStarIndices);
          }
          else
          {
            grid.ColumnDefinitions[columnBeforePosition].Width = new GridLength(grid.ComputedColumnWidthCache[columnBeforePosition]);
            grid.ColumnDefinitions[columnBeforePosition].ClearValue(ColumnDefinitionNode.MinWidthProperty);
            grid.ColumnDefinitions[columnBeforePosition + 1].Width = new GridLength(grid.ComputedColumnWidthCache[columnBeforePosition + 1]);
            grid.ColumnDefinitions[columnBeforePosition + 1].ClearValue(ColumnDefinitionNode.MinWidthProperty);
          }
          editTransaction.Update();
          grid.ViewModel.RefreshSelection();
          this.SetCurrentRects((BaseFrameworkElement) grid, currentRects);
          grid.UncacheComputedColumnWidths();
          editTransaction.Commit();
        }
      }
      grid.ViewModel.DefaultView.AdornerLayer.InvalidateAdornersStructure((SceneElement) grid);
    }

    public void AddHorizontalGridline(GridElement grid, double position)
    {
      using (SceneEditTransaction editTransaction = grid.ViewModel.CreateEditTransaction(StringTable.UndoUnitAddGridline))
      {
        using (grid.ViewModel.ForceBaseValue())
        {
          List<LayoutCacheRecord> currentRects = this.GetCurrentRects((BaseFrameworkElement) grid);
          int rowBeforePosition = grid.GetComputedRowBeforePosition(position);
          double computedPositionOfRow1 = grid.GetComputedPositionOfRow(rowBeforePosition);
          double computedPositionOfRow2 = grid.GetComputedPositionOfRow(rowBeforePosition + 1);
          bool flag = grid.RowDefinitions.Count == 0 || grid.RowDefinitions[rowBeforePosition].Height.IsStar;
          grid.CacheComputedRowHeights();
          this.EnsureOneRow(grid);
          grid.RowDefinitions.Insert(rowBeforePosition + 1, RowDefinitionNode.Factory.Instantiate(grid.ViewModel));
          position = Math.Max(position, computedPositionOfRow1);
          if (position > computedPositionOfRow2)
          {
            grid.ComputedRowHeightCache[rowBeforePosition] = computedPositionOfRow2 - computedPositionOfRow1;
            grid.ComputedRowHeightCache.Insert(rowBeforePosition + 1, position - computedPositionOfRow2);
          }
          else
          {
            grid.ComputedRowHeightCache[rowBeforePosition] = position - computedPositionOfRow1;
            grid.ComputedRowHeightCache.Insert(rowBeforePosition + 1, computedPositionOfRow2 - position);
          }
          editTransaction.Update();
          grid.ViewModel.DefaultView.UpdateLayout();
          if (flag)
          {
            List<int> heightStarIndices = this.GetHeightStarIndices(grid);
            this.NormalizeHeightStars(grid, heightStarIndices);
          }
          else
          {
            grid.RowDefinitions[rowBeforePosition].Height = new GridLength(grid.ComputedRowHeightCache[rowBeforePosition]);
            grid.RowDefinitions[rowBeforePosition].ClearValue(RowDefinitionNode.MinHeightProperty);
            grid.RowDefinitions[rowBeforePosition + 1].Height = new GridLength(grid.ComputedRowHeightCache[rowBeforePosition + 1]);
            grid.RowDefinitions[rowBeforePosition + 1].ClearValue(RowDefinitionNode.MinHeightProperty);
          }
          editTransaction.Update();
          grid.ViewModel.RefreshSelection();
          this.SetCurrentRects((BaseFrameworkElement) grid, currentRects);
          grid.UncacheComputedRowHeights();
          editTransaction.Commit();
        }
      }
      grid.ViewModel.DefaultView.AdornerLayer.InvalidateAdornersStructure((SceneElement) grid);
    }

    public void RemoveColumn(GridElement grid, int index)
    {
      using (SceneEditTransaction editTransaction = grid.ViewModel.CreateEditTransaction(StringTable.UndoUnitRemoveGridline))
      {
        grid.ViewModel.GridColumnSelectionSet.Clear();
        grid.ViewModel.AnimationEditor.DeleteAllAnimations((SceneNode) grid.ColumnDefinitions[index]);
        using (grid.ViewModel.ForceBaseValue())
        {
          List<LayoutCacheRecord> currentRects = this.GetCurrentRects((BaseFrameworkElement) grid);
          bool isStar1 = grid.ColumnDefinitions[index - 1].Width.IsStar;
          bool isStar2 = grid.ColumnDefinitions[index].Width.IsStar;
          List<int> widthStarIndices = this.GetWidthStarIndices(grid);
          grid.CacheComputedColumnWidths();
          List<double> columnWidthCache;
          int index1;
          (columnWidthCache = grid.ComputedColumnWidthCache)[index1 = index - 1] = columnWidthCache[index1] + grid.ComputedColumnWidthCache[index];
          grid.ComputedColumnWidthCache.RemoveAt(index);
          grid.ColumnDefinitions.RemoveAt(index);
          if (isStar2)
            widthStarIndices.Remove(index);
          if (isStar1 || isStar2)
          {
            for (int index2 = 0; index2 < widthStarIndices.Count; ++index2)
            {
              if (widthStarIndices[index2] > index)
              {
                List<int> list;
                int index3;
                (list = widthStarIndices)[index3 = index2] = list[index3] - 1;
              }
            }
            this.NormalizeWidthStars(grid, widthStarIndices);
          }
          if (!isStar1)
          {
            grid.ColumnDefinitions[index - 1].Width = new GridLength(grid.ComputedColumnWidthCache[index - 1]);
            grid.ColumnDefinitions[index - 1].ClearValue(ColumnDefinitionNode.MinWidthProperty);
          }
          editTransaction.Update();
          grid.ViewModel.RefreshSelection();
          this.SetCurrentRects((BaseFrameworkElement) grid, currentRects);
          grid.UncacheComputedColumnWidths();
          GridLayoutDesigner.ReselectGridToRestoreAdorners(grid);
          editTransaction.Commit();
        }
      }
    }

    public void RemoveRow(GridElement grid, int index)
    {
      using (SceneEditTransaction editTransaction = grid.ViewModel.CreateEditTransaction(StringTable.UndoUnitRemoveGridline))
      {
        grid.ViewModel.GridRowSelectionSet.Clear();
        grid.ViewModel.AnimationEditor.DeleteAllAnimations((SceneNode) grid.RowDefinitions[index]);
        using (grid.ViewModel.ForceBaseValue())
        {
          List<LayoutCacheRecord> currentRects = this.GetCurrentRects((BaseFrameworkElement) grid);
          bool isStar1 = grid.RowDefinitions[index - 1].Height.IsStar;
          bool isStar2 = grid.RowDefinitions[index].Height.IsStar;
          List<int> heightStarIndices = this.GetHeightStarIndices(grid);
          grid.CacheComputedRowHeights();
          List<double> computedRowHeightCache;
          int index1;
          (computedRowHeightCache = grid.ComputedRowHeightCache)[index1 = index - 1] = computedRowHeightCache[index1] + grid.ComputedRowHeightCache[index];
          grid.ComputedRowHeightCache.RemoveAt(index);
          grid.RowDefinitions.RemoveAt(index);
          if (isStar2)
            heightStarIndices.Remove(index);
          if (isStar1 || isStar2)
          {
            for (int index2 = 0; index2 < heightStarIndices.Count; ++index2)
            {
              if (heightStarIndices[index2] > index)
              {
                List<int> list;
                int index3;
                (list = heightStarIndices)[index3 = index2] = list[index3] - 1;
              }
            }
            this.NormalizeHeightStars(grid, heightStarIndices);
          }
          if (!isStar1)
          {
            grid.RowDefinitions[index - 1].Height = new GridLength(grid.ComputedRowHeightCache[index - 1]);
            grid.RowDefinitions[index - 1].ClearValue(RowDefinitionNode.MinHeightProperty);
          }
          editTransaction.Update();
          grid.ViewModel.RefreshSelection();
          this.SetCurrentRects((BaseFrameworkElement) grid, currentRects);
          grid.UncacheComputedRowHeights();
          GridLayoutDesigner.ReselectGridToRestoreAdorners(grid);
          editTransaction.Commit();
        }
      }
    }

    private static void ReselectGridToRestoreAdorners(GridElement grid)
    {
      ISceneInsertionPoint sceneInsertionPoint = grid.ViewModel.ActiveSceneInsertionPoint;
      if (sceneInsertionPoint == null || grid == sceneInsertionPoint.SceneElement)
        return;
      grid.ViewModel.ElementSelectionSet.SetSelection((SceneElement) grid);
    }

    public void SetColumnWidth(GridElement grid, int index, GridLength width, double minWidth)
    {
      using (this.DeferTokenForGridDesignMode(grid))
      {
        List<int> widthStarIndices = this.GetWidthStarIndices(grid);
        bool isStar = grid.ColumnDefinitions[index].Width.IsStar;
        width = new GridLength(RoundingHelper.RoundLength(width.Value), width.GridUnitType);
        grid.ColumnDefinitions[index].Width = width;
        if (double.IsNaN(minWidth))
          grid.ColumnDefinitions[index].ClearValue(ColumnDefinitionNode.MinWidthProperty);
        else
          grid.ColumnDefinitions[index].MinWidth = minWidth;
        if (!isStar || width.IsStar)
          return;
        widthStarIndices.Remove(index);
        this.NormalizeWidthStars(grid, widthStarIndices);
      }
    }

    public void SetColumnWidthToStar(GridElement grid, int index)
    {
      using (this.DeferTokenForGridDesignMode(grid))
      {
        List<int> widthStarIndices = this.GetWidthStarIndices(grid);
        if (!widthStarIndices.Contains(index))
          widthStarIndices.Add(index);
        this.NormalizeWidthStars(grid, widthStarIndices);
      }
    }

    public void SetRowHeight(GridElement grid, int index, GridLength height, double minHeight)
    {
      using (this.DeferTokenForGridDesignMode(grid))
      {
        List<int> heightStarIndices = this.GetHeightStarIndices(grid);
        bool isStar = grid.RowDefinitions[index].Height.IsStar;
        height = new GridLength(RoundingHelper.RoundLength(height.Value), height.GridUnitType);
        grid.RowDefinitions[index].Height = height;
        if (double.IsNaN(minHeight))
          grid.RowDefinitions[index].ClearValue(RowDefinitionNode.MinHeightProperty);
        else
          grid.RowDefinitions[index].MinHeight = minHeight;
        if (!isStar || height.IsStar)
          return;
        heightStarIndices.Remove(index);
        this.NormalizeHeightStars(grid, heightStarIndices);
      }
    }

    public void SetRowHeightToStar(GridElement grid, int index)
    {
      using (this.DeferTokenForGridDesignMode(grid))
      {
        List<int> heightStarIndices = this.GetHeightStarIndices(grid);
        if (!heightStarIndices.Contains(index))
          heightStarIndices.Add(index);
        this.NormalizeHeightStars(grid, heightStarIndices);
      }
    }

    private IDisposable DeferTokenForGridDesignMode(GridElement grid)
    {
      if (grid.ViewModel.IsInGridDesignMode)
        return grid.ViewModel.AnimationEditor.DeferKeyFraming();
      return grid.ViewModel.ForceBaseValue();
    }

    public override void FillChild(BaseFrameworkElement element)
    {
      GridElement grid = element.Parent as GridElement;
      if (grid == null)
        throw new ArgumentException(ExceptionStringTable.LayoutBehaviorElementNotInGrid);
      Rect gridRectFromGridBox = LayoutUtilities.GetComputedGridRectFromGridBox(LayoutUtilities.GetComputedGridBoxContainingRect(this.GetChildRect(element), grid), grid);
      this.SetChildRect(element, this.PrepareLayoutRect(gridRectFromGridBox), LayoutOverrides.None, LayoutOverrides.All, LayoutOverrides.All);
    }

    private class CanvasDesignModeToken : IDisposable
    {
      private Size size;
      private GridElement grid;
      private List<LayoutCacheRecord> rects;
      private GridLayoutDesigner gridLayoutDesigner;
      private IDisposable enforceGridDesignModeToken;
      private IDisposable deferTokenForGridDesignMode;

      public CanvasDesignModeToken(GridElement grid, Size size, bool setWidth, bool setHeight)
      {
        this.grid = grid;
        this.size = size;
        this.gridLayoutDesigner = new GridLayoutDesigner();
        this.enforceGridDesignModeToken = this.grid.ViewModel.EnforceGridDesignMode;
        this.deferTokenForGridDesignMode = this.gridLayoutDesigner.DeferTokenForGridDesignMode(this.grid);
        this.grid.ViewModel.DefaultView.UpdateLayout();
        using (LayoutRoundingHelper.TurnOffLayoutRounding((BaseFrameworkElement) this.grid))
          this.rects = this.gridLayoutDesigner.GetCurrentRects((BaseFrameworkElement) this.grid);
        this.gridLayoutDesigner.AdjustLastColumnRow(this.grid, this.size, setWidth, setHeight);
      }

      public void Dispose()
      {
        this.grid.ViewModel.Document.OnUpdatedEditTransaction();
        this.grid.ViewModel.DefaultView.UpdateLayout();
        this.gridLayoutDesigner.SetCurrentRects((BaseFrameworkElement) this.grid, this.rects);
        this.deferTokenForGridDesignMode.Dispose();
        this.enforceGridDesignModeToken.Dispose();
      }
    }
  }
}
