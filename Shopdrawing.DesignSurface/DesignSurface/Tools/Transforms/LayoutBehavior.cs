// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Designers;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class LayoutBehavior : AdornedToolBehavior
  {
    private bool firstLineHeaderDrag;
    private int effectiveLineHeaderIndex;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitMoveGridline;
      }
    }

    internal bool IsNewGridlineEnabled
    {
      get
      {
        return true;
      }
    }

    public LayoutBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.ToolBehaviorContext.SnappingEngine.Start(this.ToolBehaviorContext, (BaseFrameworkElement) null, (IList<BaseFrameworkElement>) null);
      LayoutLineHeaderAdorner lineHeaderAdorner = this.ActiveAdorner as LayoutLineHeaderAdorner;
      LayoutBackgroundAdorner backgroundAdorner = this.ActiveAdorner as LayoutBackgroundAdorner;
      if (lineHeaderAdorner != null)
      {
        GridElement gridElement = (GridElement) lineHeaderAdorner.Element;
        if (lineHeaderAdorner.IsX)
        {
          GridColumnSelectionSet columnSelectionSet = this.ActiveSceneViewModel.GridColumnSelectionSet;
          ColumnDefinitionNode selectionToSet = gridElement.ColumnDefinitions[lineHeaderAdorner.Index];
          columnSelectionSet.GridLineMode = true;
          columnSelectionSet.SetSelection(selectionToSet);
        }
        else
        {
          GridRowSelectionSet gridRowSelectionSet = this.ActiveSceneViewModel.GridRowSelectionSet;
          RowDefinitionNode selectionToSet = gridElement.RowDefinitions[lineHeaderAdorner.Index];
          gridRowSelectionSet.GridLineMode = true;
          gridRowSelectionSet.SetSelection(selectionToSet);
        }
        this.firstLineHeaderDrag = true;
        ((GridRowColumnAdornerSet) lineHeaderAdorner.AdornerSet).DraggingGridline = true;
        return true;
      }
      return backgroundAdorner != null;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      SceneView activeView = this.ActiveView;
      LayoutLineHeaderAdorner lineHeaderAdorner = this.ActiveAdorner as LayoutLineHeaderAdorner;
      GridElement grid = (GridElement) ((Adorner) this.ActiveAdorner).Element;
      GridLayoutDesigner gridLayoutDesigner = (GridLayoutDesigner) grid.LayoutDesigner;
      activeView.EnsureVisible(this.ActiveAdorner, scrollNow);
      Matrix transformFromRoot = activeView.GetComputedTransformFromRoot((SceneElement) grid);
      dragStartPosition *= transformFromRoot;
      dragCurrentPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(dragCurrentPosition);
      dragCurrentPosition *= transformFromRoot;
      if (lineHeaderAdorner == null || lineHeaderAdorner.Index <= 0)
        return false;
      this.EnsureEditTransaction();
      if (lineHeaderAdorner.IsX)
      {
        if (this.firstLineHeaderDrag)
        {
          this.effectiveLineHeaderIndex = lineHeaderAdorner.Index;
          if (dragCurrentPosition.X < dragStartPosition.X)
          {
            while (this.effectiveLineHeaderIndex > 1 && grid.GetComputedColumnWidth(this.effectiveLineHeaderIndex - 1) < 1.0 / activeView.Zoom)
              --this.effectiveLineHeaderIndex;
            this.firstLineHeaderDrag = false;
          }
          else if (dragCurrentPosition.X > dragStartPosition.X)
          {
            while (this.effectiveLineHeaderIndex < grid.ColumnDefinitions.Count - 2 && grid.GetComputedColumnWidth(this.effectiveLineHeaderIndex) < 1.0 / activeView.Zoom)
              ++this.effectiveLineHeaderIndex;
            this.firstLineHeaderDrag = false;
          }
        }
        if (!this.firstLineHeaderDrag)
        {
          int num = this.effectiveLineHeaderIndex - 1;
          double width = dragCurrentPosition.X - grid.GetComputedPositionOfColumn(num);
          gridLayoutDesigner.AdjustColumn(grid, num, width, this.IsShiftDown, !grid.ViewModel.IsInGridDesignMode);
        }
      }
      else
      {
        if (this.firstLineHeaderDrag)
        {
          this.effectiveLineHeaderIndex = lineHeaderAdorner.Index;
          if (dragCurrentPosition.Y < dragStartPosition.Y)
          {
            while (this.effectiveLineHeaderIndex > 1 && grid.GetComputedRowHeight(this.effectiveLineHeaderIndex - 1) < 1.0 / activeView.Zoom)
              --this.effectiveLineHeaderIndex;
            this.firstLineHeaderDrag = false;
          }
          else if (dragCurrentPosition.Y > dragStartPosition.Y)
          {
            while (this.effectiveLineHeaderIndex < grid.RowDefinitions.Count - 2 && grid.GetComputedRowHeight(this.effectiveLineHeaderIndex) < 1.0 / activeView.Zoom)
              ++this.effectiveLineHeaderIndex;
            this.firstLineHeaderDrag = false;
          }
        }
        if (!this.firstLineHeaderDrag)
        {
          int row = this.effectiveLineHeaderIndex - 1;
          double height = dragCurrentPosition.Y - grid.GetComputedPositionOfRow(row);
          gridLayoutDesigner.AdjustRow(grid, row, height, this.IsShiftDown, !grid.ViewModel.IsInGridDesignMode);
        }
      }
      ((Adorner) this.ActiveAdorner).AdornerSet.InvalidateRender();
      this.UpdateEditTransaction();
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      LayoutLineHeaderAdorner lineHeaderAdorner = this.ActiveAdorner as LayoutLineHeaderAdorner;
      NewGridlineAdorner newGridlineAdorner = this.ActiveAdorner as NewGridlineAdorner;
      if (lineHeaderAdorner != null)
        ((GridRowColumnAdornerSet) lineHeaderAdorner.AdornerSet).DraggingGridline = false;
      else if (newGridlineAdorner != null && this.IsNewGridlineEnabled)
      {
        GridElement grid = (GridElement) ((Adorner) this.ActiveAdorner).Element;
        GridLayoutDesigner gridLayoutDesigner = (GridLayoutDesigner) grid.LayoutDesigner;
        dragEndPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(dragEndPosition);
        Matrix transformFromRoot = this.ActiveView.GetComputedTransformFromRoot((SceneElement) grid);
        dragEndPosition *= transformFromRoot;
        if (newGridlineAdorner.IsX)
          gridLayoutDesigner.AddVerticalGridline(grid, dragEndPosition.X);
        else
          gridLayoutDesigner.AddHorizontalGridline(grid, dragEndPosition.Y);
      }
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.CommitEditTransaction();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      GridLockAdorner gridLockAdorner = this.ActiveAdorner as GridLockAdorner;
      LayoutBackgroundAdorner backgroundAdorner = this.ActiveAdorner as LayoutBackgroundAdorner;
      LayoutLineHeaderAdorner lineHeaderAdorner = this.ActiveAdorner as LayoutLineHeaderAdorner;
      GridDesignModeAdorner designModeAdorner = this.ActiveAdorner as GridDesignModeAdorner;
      NewGridlineAdorner newGridlineAdorner = this.ActiveAdorner as NewGridlineAdorner;
      LayoutEmptyBackgroundHeaderAdorner backgroundHeaderAdorner = this.ActiveAdorner as LayoutEmptyBackgroundHeaderAdorner;
      GridElement grid = (GridElement) ((Adorner) this.ActiveAdorner).Element;
      GridLayoutDesigner gridLayoutDesigner = (GridLayoutDesigner) grid.LayoutDesigner;
      pointerPosition = this.ToolBehaviorContext.SnappingEngine.SnapPoint(pointerPosition);
      Matrix transformFromRoot = this.ActiveView.GetComputedTransformFromRoot((SceneElement) grid);
      pointerPosition *= transformFromRoot;
      if (backgroundHeaderAdorner == null)
      {
        if (lineHeaderAdorner != null)
        {
          ((GridRowColumnAdornerSet) lineHeaderAdorner.AdornerSet).DraggingGridline = false;
          if (clickCount > 1)
          {
            if (lineHeaderAdorner.IsX)
              gridLayoutDesigner.RemoveColumn(grid, lineHeaderAdorner.Index);
            else
              gridLayoutDesigner.RemoveRow(grid, lineHeaderAdorner.Index);
          }
        }
        else if (gridLockAdorner != null)
        {
          using (SceneEditTransaction editTransaction = this.ActiveDocument.CreateEditTransaction(StringTable.UndoUnitToggleGridProperty))
          {
            if (gridLockAdorner.IsX)
            {
              ColumnDefinitionNode columnDefinitionNode = grid.ColumnDefinitions[gridLockAdorner.Index];
              GridLength width = columnDefinitionNode.Width;
              if (width.IsAuto)
              {
                columnDefinitionNode.ClearValue(ColumnDefinitionNode.MinWidthProperty);
                gridLayoutDesigner.SetColumnWidthToStar(grid, gridLockAdorner.Index);
              }
              else if (width.IsAbsolute)
                gridLayoutDesigner.SetColumnWidth(grid, gridLockAdorner.Index, GridLength.Auto, columnDefinitionNode.ComputedWidth);
              else
                gridLayoutDesigner.SetColumnWidth(grid, gridLockAdorner.Index, new GridLength(columnDefinitionNode.ComputedWidth), double.NaN);
            }
            else
            {
              RowDefinitionNode rowDefinitionNode = grid.RowDefinitions[gridLockAdorner.Index];
              GridLength height = rowDefinitionNode.Height;
              if (height.IsAuto)
              {
                rowDefinitionNode.ClearValue(RowDefinitionNode.MinHeightProperty);
                gridLayoutDesigner.SetRowHeightToStar(grid, gridLockAdorner.Index);
              }
              else if (height.IsAbsolute)
                gridLayoutDesigner.SetRowHeight(grid, gridLockAdorner.Index, GridLength.Auto, rowDefinitionNode.ComputedHeight);
              else
                gridLayoutDesigner.SetRowHeight(grid, gridLockAdorner.Index, new GridLength(rowDefinitionNode.ComputedHeight), double.NaN);
            }
            editTransaction.Commit();
          }
        }
        else if (backgroundAdorner != null)
        {
          if (backgroundAdorner.IsX)
          {
            GridColumnSelectionSet columnSelectionSet = this.ActiveSceneViewModel.GridColumnSelectionSet;
            ColumnDefinitionNode columnDefinitionNode = grid.ColumnDefinitions[backgroundAdorner.Index];
            columnSelectionSet.GridLineMode = false;
            if (this.IsShiftDown)
              columnSelectionSet.ToggleSelection(columnDefinitionNode);
            else
              columnSelectionSet.SetSelection(columnDefinitionNode);
          }
          else
          {
            GridRowSelectionSet gridRowSelectionSet = this.ActiveSceneViewModel.GridRowSelectionSet;
            RowDefinitionNode rowDefinitionNode = grid.RowDefinitions[backgroundAdorner.Index];
            gridRowSelectionSet.GridLineMode = false;
            if (this.IsShiftDown)
              gridRowSelectionSet.ToggleSelection(rowDefinitionNode);
            else
              gridRowSelectionSet.SetSelection(rowDefinitionNode);
          }
        }
        else if (newGridlineAdorner != null && this.IsNewGridlineEnabled)
        {
          if (newGridlineAdorner.IsX)
            gridLayoutDesigner.AddVerticalGridline(grid, pointerPosition.X);
          else
            gridLayoutDesigner.AddHorizontalGridline(grid, pointerPosition.Y);
        }
        else if (designModeAdorner != null)
          grid.ViewModel.IsInGridDesignMode = !designModeAdorner.Grid.ViewModel.IsInGridDesignMode;
      }
      this.ToolBehaviorContext.SnappingEngine.Stop();
      this.ActiveView.UpdateLayout();
      this.ActiveSceneViewModel.RefreshSelection();
      return base.OnClickEnd(pointerPosition, clickCount);
    }
  }
}
