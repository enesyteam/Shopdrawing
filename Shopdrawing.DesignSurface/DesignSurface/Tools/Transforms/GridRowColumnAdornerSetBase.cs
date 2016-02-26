// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GridRowColumnAdornerSetBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class GridRowColumnAdornerSetBase : AdornerSet
  {
    protected int Columns { get; set; }

    protected int Rows { get; set; }

    protected override bool NeedsRebuild
    {
      get
      {
        GridElement gridElement = (GridElement) this.Element;
        if (gridElement != null && gridElement.ColumnDefinitions.Count == this.Columns)
          return gridElement.RowDefinitions.Count != this.Rows;
        return true;
      }
    }

    public GridRowColumnAdornerSetBase(ToolBehaviorContext toolContext, BaseFrameworkElement adornedElement)
      : base(toolContext, (SceneElement) adornedElement, AdornerSetOrder.BottomOrder)
    {
      this.Columns = 0;
      this.Rows = 0;
    }

    protected void CacheRowColumnCounts()
    {
      GridElement gridElement = (GridElement) this.Element;
      if (gridElement == null || gridElement.ColumnDefinitions == null || gridElement.RowDefinitions == null)
      {
        this.Columns = 0;
        this.Rows = 0;
      }
      else
      {
        this.Columns = gridElement.ColumnDefinitions.Count;
        this.Rows = gridElement.RowDefinitions.Count;
      }
    }
  }
}
