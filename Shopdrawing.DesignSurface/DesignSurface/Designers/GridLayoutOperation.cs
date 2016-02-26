// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.GridLayoutOperation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Designers
{
  public class GridLayoutOperation : LayoutOperation
  {
    private GridElement parentGrid;

    public GridLayoutOperation(ILayoutDesigner designer, BaseFrameworkElement child)
      : base(designer, child)
    {
      this.parentGrid = child.ParentElement as GridElement;
      if (this.parentGrid == null)
        throw new ArgumentException(ExceptionStringTable.LayoutBehaviorElementNotInGrid);
    }

    protected override void ComputeIdealSlotRect()
    {
      this.Settings.GridBox = LayoutUtilities.GetComputedGridBoxContainingRect(this.ChildRect, this.parentGrid);
      this.SlotRect = LayoutUtilities.GetComputedGridRectFromGridBox(this.Settings.GridBox, this.parentGrid);
    }

    protected override void ComputeSlotRectOverrides()
    {
      if (this.SettingsFromElement.GridBox.ColBegin == this.Settings.GridBox.ColBegin && this.SettingsFromElement.GridBox.ColSpan == this.Settings.GridBox.ColSpan && (this.SettingsFromElement.GridBox.RowBegin == this.Settings.GridBox.RowBegin && this.SettingsFromElement.GridBox.RowSpan == this.Settings.GridBox.RowSpan))
        return;
      this.Settings.LayoutOverrides |= LayoutOverrides.GridBox;
      this.Settings.GridBox = this.SettingsFromElement.GridBox;
    }

    protected override void SetSlotRectChanges()
    {
      if ((this.WidthConstraintMode & LayoutConstraintMode.CanvasLike) == LayoutConstraintMode.NonOverlappingGridlike && (this.SettingsFromElement.HorizontalAlignment != HorizontalAlignment.Center || (this.OverridesToIgnore & LayoutOverrides.CenterHorizontalAlignment) != LayoutOverrides.None))
      {
        this.Settings.GridBox.ColBegin = this.SetPropertyChanges<int>(this.Settings.GridBox.ColBegin, this.SettingsFromElement.GridBox.ColBegin, LayoutOverrides.GridBox, GridElement.ColumnProperty);
        this.Settings.GridBox.ColSpan = this.SetPropertyChanges<int>(this.Settings.GridBox.ColSpan, this.SettingsFromElement.GridBox.ColSpan, LayoutOverrides.GridBox, GridElement.ColumnSpanProperty);
      }
      if ((this.HeightConstraintMode & LayoutConstraintMode.CanvasLike) != LayoutConstraintMode.NonOverlappingGridlike || this.SettingsFromElement.VerticalAlignment == VerticalAlignment.Center && (this.OverridesToIgnore & LayoutOverrides.CenterVerticalAlignment) == LayoutOverrides.None)
        return;
      this.Settings.GridBox.RowBegin = this.SetPropertyChanges<int>(this.Settings.GridBox.RowBegin, this.SettingsFromElement.GridBox.RowBegin, LayoutOverrides.GridBox, GridElement.RowProperty);
      this.Settings.GridBox.RowSpan = this.SetPropertyChanges<int>(this.Settings.GridBox.RowSpan, this.SettingsFromElement.GridBox.RowSpan, LayoutOverrides.GridBox, GridElement.RowSpanProperty);
    }
  }
}
