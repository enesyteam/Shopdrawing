// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.GridElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class GridElement : PanelElement
  {
    public static readonly IPropertyId ColumnDefinitionsProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.LocalProperty, "ColumnDefinitions", MemberAccessTypes.Public);
    public static readonly IPropertyId ColumnProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.AttachedProperty, "Column", MemberAccessTypes.Public);
    public static readonly IPropertyId ColumnSpanProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.AttachedProperty, "ColumnSpan", MemberAccessTypes.Public);
    public static readonly IPropertyId RowDefinitionsProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.LocalProperty, "RowDefinitions", MemberAccessTypes.Public);
    public static readonly IPropertyId RowProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.AttachedProperty, "Row", MemberAccessTypes.Public);
    public static readonly IPropertyId RowSpanProperty = (IPropertyId) PlatformTypes.Grid.GetMember(MemberType.AttachedProperty, "RowSpan", MemberAccessTypes.Public);
    public static readonly GridElement.ConcreteGridElementFactory Factory = new GridElement.ConcreteGridElementFactory();
    private List<double> computedColumnWidthCache;
    private List<double> computedRowHeightCache;

    public ISceneNodeCollection<ColumnDefinitionNode> ColumnDefinitions
    {
      get
      {
        return (ISceneNodeCollection<ColumnDefinitionNode>) new SceneNode.SceneNodeCollection<ColumnDefinitionNode>((SceneNode) this, GridElement.ColumnDefinitionsProperty);
      }
    }

    public ISceneNodeCollection<RowDefinitionNode> RowDefinitions
    {
      get
      {
        return (ISceneNodeCollection<RowDefinitionNode>) new SceneNode.SceneNodeCollection<RowDefinitionNode>((SceneNode) this, GridElement.RowDefinitionsProperty);
      }
    }

    public List<double> ComputedColumnWidthCache
    {
      get
      {
        return this.computedColumnWidthCache;
      }
    }

    public List<double> ComputedRowHeightCache
    {
      get
      {
        return this.computedRowHeightCache;
      }
    }

    public void CacheComputedColumnWidths()
    {
      List<double> list = new List<double>(this.ColumnDefinitions.Count);
      foreach (ColumnDefinitionNode columnDefinitionNode in (IEnumerable<ColumnDefinitionNode>) this.ColumnDefinitions)
        list.Add(columnDefinitionNode.ComputedWidth);
      this.computedColumnWidthCache = list;
    }

    public void UncacheComputedColumnWidths()
    {
      this.computedColumnWidthCache = (List<double>) null;
    }

    public void CacheComputedRowHeights()
    {
      List<double> list = new List<double>(this.RowDefinitions.Count);
      foreach (RowDefinitionNode rowDefinitionNode in (IEnumerable<RowDefinitionNode>) this.RowDefinitions)
        list.Add(rowDefinitionNode.ComputedHeight);
      this.computedRowHeightCache = list;
    }

    public void UncacheComputedRowHeights()
    {
      this.computedRowHeightCache = (List<double>) null;
    }

    public double GetComputedColumnWidth(int column)
    {
      if (this.computedColumnWidthCache != null)
      {
        if (this.computedColumnWidthCache.Count == 0)
          return this.GetComputedTightBounds().Width;
        return this.computedColumnWidthCache[column];
      }
      if (this.ColumnDefinitions.Count == 0)
        return this.GetComputedTightBounds().Width;
      return this.ColumnDefinitions[column].ComputedWidth;
    }

    public double GetComputedRowHeight(int row)
    {
      if (this.computedRowHeightCache != null)
      {
        if (this.computedRowHeightCache.Count == 0)
          return this.GetComputedTightBounds().Height;
        return this.computedRowHeightCache[row];
      }
      if (this.RowDefinitions.Count == 0)
        return this.GetComputedTightBounds().Height;
      return this.RowDefinitions[row].ComputedHeight;
    }

    public int GetComputedColumnBeforePosition(double position)
    {
      int column = 1;
      for (double computedColumnWidth = this.GetComputedColumnWidth(0); column < this.ColumnDefinitions.Count && (position == computedColumnWidth && this.GetComputedColumnWidth(column) == 0.0 || position >= computedColumnWidth); ++column)
        computedColumnWidth += this.GetComputedColumnWidth(column);
      return column - 1;
    }

    public int GetComputedColumnAfterPosition(double position)
    {
      int num1 = 0;
      for (double num2 = 0.0; num1 < Math.Max(1, this.ColumnDefinitions.Count) && position > num2; ++num1)
        num2 += this.GetComputedColumnWidth(num1);
      return Math.Max(1, num1);
    }

    public int GetComputedRowBeforePosition(double position)
    {
      int row = 1;
      for (double computedRowHeight = this.GetComputedRowHeight(0); row < this.RowDefinitions.Count && (position == computedRowHeight && this.GetComputedRowHeight(row) == 0.0 || position >= computedRowHeight); ++row)
        computedRowHeight += this.GetComputedRowHeight(row);
      return row - 1;
    }

    public int GetComputedRowAfterPosition(double position)
    {
      int num1 = 0;
      for (double num2 = 0.0; num1 < Math.Max(1, this.RowDefinitions.Count) && position > num2; ++num1)
        num2 += this.GetComputedRowHeight(num1);
      return Math.Max(1, num1);
    }

    public double GetComputedPositionOfColumn(int column)
    {
      int column1 = 0;
      double num = 0.0;
      for (; column1 < column; ++column1)
        num += this.GetComputedColumnWidth(column1);
      return num;
    }

    public double GetComputedPositionOfRow(int row)
    {
      int row1 = 0;
      double num = 0.0;
      for (; row1 < row; ++row1)
        num += this.GetComputedRowHeight(row1);
      return num;
    }

    public class ConcreteGridElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new GridElement();
      }
    }
  }
}
