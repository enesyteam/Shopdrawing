// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.UserInterface.ExpandoSplitter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Expression.Framework.UserInterface
{
  public class ExpandoSplitter : Thumb
  {
    private bool IsAdjustingHorizontal
    {
      get
      {
        return this.VerticalAlignment == VerticalAlignment.Stretch;
      }
    }

    protected override void OnInitialized(EventArgs e)
    {
      this.DragDelta += new DragDeltaEventHandler(this.ExpandoSplitter_DragDelta);
      base.OnInitialized(e);
    }

    private void ExpandoSplitter_DragDelta(object sender, DragDeltaEventArgs e)
    {
      if (!(this.Parent is Grid))
        return;
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.ResizePalette);
      this.MoveSplitter(e.HorizontalChange, e.VerticalChange);
    }

    private double GetActualLength(DefinitionBase definition)
    {
      ColumnDefinition columnDefinition = definition as ColumnDefinition;
      if (columnDefinition != null)
        return columnDefinition.ActualWidth;
      return ((RowDefinition) definition).ActualHeight;
    }

    private GridLength GetSpecifiedLength(DefinitionBase definition)
    {
      ColumnDefinition columnDefinition = definition as ColumnDefinition;
      if (columnDefinition != null)
        return columnDefinition.Width;
      return ((RowDefinition) definition).Height;
    }

    private double GetMinLength(DefinitionBase definition)
    {
      ColumnDefinition columnDefinition = definition as ColumnDefinition;
      if (columnDefinition != null)
        return columnDefinition.MinWidth;
      return ((RowDefinition) definition).MinHeight;
    }

    private double GetMaxLength(DefinitionBase definition)
    {
      ColumnDefinition columnDefinition = definition as ColumnDefinition;
      if (columnDefinition != null)
        return columnDefinition.MaxWidth;
      return ((RowDefinition) definition).MaxHeight;
    }

    private DefinitionBase SearchForAdjustableDefinition(int index, bool searchForwards, bool shrinkingInSearchDirection)
    {
      Grid grid = this.Parent as Grid;
      if (grid == null)
        return (DefinitionBase) null;
      ExpandoList expandoList = grid.TemplatedParent as ExpandoList;
      IList list = this.IsAdjustingHorizontal ? (IList) grid.ColumnDefinitions : (IList) grid.RowDefinitions;
      while (index >= 0 && index < list.Count)
      {
        DefinitionBase definition = (DefinitionBase) list[index];
        if (expandoList != null && ((ExpandoListItemModel) expandoList.Items[index]).IsExpanded && (!shrinkingInSearchDirection || !Tolerances.AreClose(this.GetActualLength(definition), this.GetMinLength(definition))))
          return definition;
        index += searchForwards ? 1 : -1;
      }
      return (DefinitionBase) null;
    }

    private void GetDefinitions(out DefinitionBase definition1, out DefinitionBase definition2, bool shrinkingForwards)
    {
      int index;
      if (this.IsAdjustingHorizontal)
      {
        index = Grid.GetColumn((UIElement) this);
        if (this.HorizontalAlignment == HorizontalAlignment.Left)
          --index;
      }
      else
      {
        index = Grid.GetRow((UIElement) this);
        if (this.VerticalAlignment == VerticalAlignment.Top)
          --index;
      }
      definition1 = this.SearchForAdjustableDefinition(index, false, !shrinkingForwards);
      definition2 = this.SearchForAdjustableDefinition(index + 1, true, shrinkingForwards);
    }

    private void GetDeltaConstraints(DefinitionBase definition1, DefinitionBase definition2, out double minDelta, out double maxDelta)
    {
      double actualLength1 = this.GetActualLength(definition1);
      double minLength1 = this.GetMinLength(definition1);
      double maxLength1 = this.GetMaxLength(definition1);
      double actualLength2 = this.GetActualLength(definition2);
      double minLength2 = this.GetMinLength(definition2);
      double maxLength2 = this.GetMaxLength(definition2);
      minDelta = -Math.Min(actualLength1 - minLength1, maxLength2 - actualLength2);
      maxDelta = Math.Min(maxLength1 - actualLength1, actualLength2 - minLength2);
    }

    private void SetDefinitionLength(DefinitionBase definition, GridLength length)
    {
      definition.SetValue(definition is ColumnDefinition ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, (object) length);
    }

    private void MoveSplitter(double horizontalChange, double verticalChange)
    {
      double val1 = this.IsAdjustingHorizontal ? horizontalChange : verticalChange;
      DefinitionBase definition1;
      DefinitionBase definition2;
      this.GetDefinitions(out definition1, out definition2, val1 > 0.0);
      if (definition1 == null || definition2 == null)
        return;
      double actualLength1 = this.GetActualLength(definition1);
      double actualLength2 = this.GetActualLength(definition2);
      double minDelta;
      double maxDelta;
      this.GetDeltaConstraints(definition1, definition2, out minDelta, out maxDelta);
      double num1 = Math.Min(Math.Max(val1, minDelta), maxDelta);
      double num2 = actualLength1 + num1;
      double num3 = actualLength2 - num1;
      double num4 = this.GetSpecifiedLength(definition1).Value + this.GetSpecifiedLength(definition2).Value;
      this.SetDefinitionLength(definition1, new GridLength(num4 * num2 / (num2 + num3), GridUnitType.Star));
      this.SetDefinitionLength(definition2, new GridLength(num4 * num3 / (num2 + num3), GridUnitType.Star));
    }
  }
}
