// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public static class LayoutUtilities
  {
    private static int cLayoutMode;

    public static bool InLayoutMode
    {
      get
      {
        return LayoutUtilities.cLayoutMode > 0;
      }
    }

    public static GridBox GetGridBoxFromElement(SceneElement element)
    {
      int colBegin = Math.Max(0, (int) element.GetComputedValue(GridElement.ColumnProperty));
      int colEnd = colBegin + Math.Max(1, (int) element.GetComputedValue(GridElement.ColumnSpanProperty));
      int rowBegin = Math.Max(0, (int) element.GetComputedValue(GridElement.RowProperty));
      int rowEnd = rowBegin + Math.Max(1, (int) element.GetComputedValue(GridElement.RowSpanProperty));
      return new GridBox(colBegin, colEnd, rowBegin, rowEnd);
    }

    public static GridBox GetComputedGridBoxContainingRect(Rect rect, GridElement grid)
    {
      int columnBeforePosition = grid.GetComputedColumnBeforePosition(rect.Left);
      int columnAfterPosition = grid.GetComputedColumnAfterPosition(rect.Right);
      int rowBeforePosition = grid.GetComputedRowBeforePosition(rect.Top);
      int rowAfterPosition = grid.GetComputedRowAfterPosition(rect.Bottom);
      if (columnAfterPosition > columnBeforePosition + 1 && rect.Right < grid.GetComputedPositionOfColumn(columnAfterPosition - 1) + grid.GetComputedColumnWidth(columnAfterPosition - 1) / 2.0)
        --columnAfterPosition;
      if (columnAfterPosition > columnBeforePosition + 1 && rect.Left > grid.GetComputedPositionOfColumn(columnBeforePosition) + grid.GetComputedColumnWidth(columnBeforePosition) / 2.0)
        ++columnBeforePosition;
      if (columnAfterPosition == columnBeforePosition + 1 && columnAfterPosition < grid.ColumnDefinitions.Count && grid.GetComputedPositionOfColumn(columnAfterPosition) < (rect.Left + rect.Right) / 2.0)
      {
        ++columnBeforePosition;
        ++columnAfterPosition;
      }
      if (rowAfterPosition > rowBeforePosition + 1 && rect.Bottom < grid.GetComputedPositionOfRow(rowAfterPosition - 1) + grid.GetComputedRowHeight(rowAfterPosition - 1) / 2.0)
        --rowAfterPosition;
      if (rowAfterPosition > rowBeforePosition + 1 && rect.Top > grid.GetComputedPositionOfRow(rowBeforePosition) + grid.GetComputedRowHeight(rowBeforePosition) / 2.0)
        ++rowBeforePosition;
      if (rowAfterPosition == rowBeforePosition + 1 && rowAfterPosition < grid.RowDefinitions.Count && grid.GetComputedPositionOfRow(rowAfterPosition) < (rect.Top + rect.Bottom) / 2.0)
      {
        ++rowBeforePosition;
        ++rowAfterPosition;
      }
      return new GridBox(columnBeforePosition, columnAfterPosition, rowBeforePosition, rowAfterPosition);
    }

    public static Rect GetComputedGridRectFromGridBox(GridBox gridBox, GridElement grid)
    {
      double positionOfColumn1 = grid.GetComputedPositionOfColumn(Math.Max(0, Math.Min(gridBox.ColBegin, grid.ColumnDefinitions.Count - 1)));
      double positionOfColumn2 = grid.GetComputedPositionOfColumn(Math.Min(Math.Max(1, grid.ColumnDefinitions.Count), gridBox.ColEnd));
      double computedPositionOfRow1 = grid.GetComputedPositionOfRow(Math.Max(0, Math.Min(gridBox.RowBegin, grid.RowDefinitions.Count - 1)));
      double computedPositionOfRow2 = grid.GetComputedPositionOfRow(Math.Min(Math.Max(1, grid.RowDefinitions.Count), gridBox.RowEnd));
      return new Rect(positionOfColumn1, computedPositionOfRow1, positionOfColumn2 - positionOfColumn1, computedPositionOfRow2 - computedPositionOfRow1);
    }

    public static LayoutOverrides GetLayoutOverrides(SceneElement element)
    {
      return (LayoutOverrides) element.GetComputedValue(DesignTimeProperties.LayoutOverridesProperty);
    }

    public static void SetLayoutOverrides(SceneElement element, LayoutOverrides flags)
    {
      if (flags == LayoutOverrides.None)
        element.ClearValue(DesignTimeProperties.LayoutOverridesProperty);
      else
        element.SetValue(DesignTimeProperties.LayoutOverridesProperty, (object) flags);
    }

    public static void EnterLayoutMode()
    {
      ++LayoutUtilities.cLayoutMode;
    }

    public static void ExitLayoutMode()
    {
      --LayoutUtilities.cLayoutMode;
    }

    public static bool PropertyIsExpression(SceneNode element, IPropertyId property)
    {
      DocumentNodePath documentNodePath = (DocumentNodePath) null;
      using (element.ViewModel.ForceBaseValue())
        documentNodePath = element.GetLocalValueAsDocumentNode(property);
      if (documentNodePath != null && documentNodePath.Node != null)
        return documentNodePath.Node.Type.IsExpression;
      return false;
    }

    public static LayoutSettings GetLayoutSettingsFromElement(SceneElement element)
    {
      LayoutSettings layoutSettings = new LayoutSettings();
      layoutSettings.LayoutOverrides = (LayoutOverrides) element.GetComputedValue(DesignTimeProperties.LayoutOverridesProperty);
      if (element.Parent is GridElement)
      {
        layoutSettings.GridBox = LayoutUtilities.GetGridBoxFromElement(element);
        if (LayoutUtilities.PropertyIsExpression((SceneNode) element, GridElement.ColumnProperty) || LayoutUtilities.PropertyIsExpression((SceneNode) element, GridElement.ColumnSpanProperty) || (LayoutUtilities.PropertyIsExpression((SceneNode) element, GridElement.RowProperty) || LayoutUtilities.PropertyIsExpression((SceneNode) element, GridElement.RowSpanProperty)))
          layoutSettings.LayoutOverrides |= LayoutOverrides.GridBox;
      }
      layoutSettings.HorizontalAlignment = (HorizontalAlignment) element.GetComputedValue(BaseFrameworkElement.HorizontalAlignmentProperty);
      if (LayoutUtilities.PropertyIsExpression((SceneNode) element, BaseFrameworkElement.HorizontalAlignmentProperty))
        layoutSettings.LayoutOverrides |= LayoutOverrides.HorizontalAlignment;
      layoutSettings.VerticalAlignment = (VerticalAlignment) element.GetComputedValue(BaseFrameworkElement.VerticalAlignmentProperty);
      if (LayoutUtilities.PropertyIsExpression((SceneNode) element, BaseFrameworkElement.VerticalAlignmentProperty))
        layoutSettings.LayoutOverrides |= LayoutOverrides.VerticalAlignment;
      layoutSettings.Width = (double) element.GetComputedValue(BaseFrameworkElement.WidthProperty);
      if (LayoutUtilities.PropertyIsExpression((SceneNode) element, BaseFrameworkElement.WidthProperty))
        layoutSettings.LayoutOverrides |= LayoutOverrides.Width;
      layoutSettings.Height = (double) element.GetComputedValue(BaseFrameworkElement.HeightProperty);
      if (LayoutUtilities.PropertyIsExpression((SceneNode) element, BaseFrameworkElement.HeightProperty))
        layoutSettings.LayoutOverrides |= LayoutOverrides.Height;
      layoutSettings.Margin = (Thickness) element.GetComputedValueAsWpf(BaseFrameworkElement.MarginProperty);
      if (LayoutUtilities.PropertyIsExpression((SceneNode) element, BaseFrameworkElement.MarginProperty))
        layoutSettings.LayoutOverrides |= LayoutOverrides.Margin;
      return layoutSettings;
    }

    public static void DetectLayoutOverrides(SceneElement element, PropertyReference propertyReference)
    {
      if (!element.IsAttached || LayoutUtilities.InLayoutMode)
        return;
      ReferenceStep referenceStep = propertyReference[0];
      LayoutOverrides layoutOverrides1 = LayoutOverrides.None;
      if (referenceStep.Equals((object) BaseFrameworkElement.HorizontalAlignmentProperty))
        layoutOverrides1 |= LayoutOverrides.HorizontalAlignment;
      else if (referenceStep.Equals((object) BaseFrameworkElement.VerticalAlignmentProperty))
        layoutOverrides1 |= LayoutOverrides.VerticalAlignment;
      else if (referenceStep.Equals((object) BaseFrameworkElement.MarginProperty))
        layoutOverrides1 |= LayoutOverrides.Margin;
      else if (referenceStep.Equals((object) BaseFrameworkElement.WidthProperty))
        layoutOverrides1 |= LayoutOverrides.Width;
      else if (referenceStep.Equals((object) BaseFrameworkElement.HeightProperty))
        layoutOverrides1 |= LayoutOverrides.Height;
      if (layoutOverrides1 == LayoutOverrides.None)
        return;
      LayoutOverrides layoutOverrides2 = LayoutUtilities.GetLayoutOverrides(element);
      LayoutOverrides flags = layoutOverrides2 & ~layoutOverrides1;
      if (layoutOverrides2 == flags)
        return;
      LayoutUtilities.SetLayoutOverrides(element, flags);
    }
  }
}
