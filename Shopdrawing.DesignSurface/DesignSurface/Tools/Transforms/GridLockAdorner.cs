// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GridLockAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public class GridLockAdorner : LayoutLockAdorner
  {
    private static ImageSource LockImageCache = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_lock_on_16x16.png");
    private static ImageSource UnlockImageCache = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_unlock_on_16x16.png");
    private static ImageSource AutoImageCache = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_auto_on_12x12.png");
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

    protected GridLength GridLength
    {
      get
      {
        return !this.IsX ? this.Grid.RowDefinitions[this.Index].Height : this.Grid.ColumnDefinitions[this.Index].Width;
      }
    }

    protected override double Value
    {
      get
      {
        if (this.LayoutLockState != LayoutLockState.Neither)
          return this.GridLength.Value;
        return double.NaN;
      }
    }

    protected override LayoutLockState LayoutLockState
    {
      get
      {
        GridLength gridLength = this.GridLength;
        if (gridLength.IsAbsolute)
          return LayoutLockState.Locked;
        return gridLength.IsStar ? LayoutLockState.Unlocked : LayoutLockState.Neither;
      }
    }

    protected override ImageSource LockImage
    {
      get
      {
        return GridLockAdorner.LockImageCache;
      }
    }

    protected override ImageSource UnlockImage
    {
      get
      {
        return GridLockAdorner.UnlockImageCache;
      }
    }

    protected override ImageSource AutoImage
    {
      get
      {
        return GridLockAdorner.AutoImageCache;
      }
    }

    protected override bool ParentRelative
    {
      get
      {
        return false;
      }
    }

    public override object ToolTip
    {
      get
      {
        switch (this.LayoutLockState)
        {
          case LayoutLockState.Locked:
            return (object) StringTable.GridLockPixelTooltip;
          case LayoutLockState.Unlocked:
            return (object) StringTable.GridLockStarTooltip;
          case LayoutLockState.Neither:
            return (object) StringTable.GridLockAutoTooltip;
          default:
            return (object) null;
        }
      }
    }

    public GridLockAdorner(AdornerSet adornerSet, bool isX, int index)
      : base(adornerSet, isX)
    {
      this.index = index;
    }

    protected override void OnIsActiveChanged()
    {
    }

    protected override Point GetCenter(Matrix matrix)
    {
      Point point = new Point();
      Vector vector = new Vector();
      if (this.IsX)
      {
        double positionOfColumn1 = this.Grid.GetComputedPositionOfColumn(this.Index);
        double positionOfColumn2 = this.Grid.GetComputedPositionOfColumn(this.Index + 1);
        point.X = (positionOfColumn1 + positionOfColumn2) / 2.0;
        vector.Y = this.ElementBounds.Top - 27.0;
      }
      else
      {
        double computedPositionOfRow1 = this.Grid.GetComputedPositionOfRow(this.Index);
        double computedPositionOfRow2 = this.Grid.GetComputedPositionOfRow(this.Index + 1);
        point.Y = (computedPositionOfRow1 + computedPositionOfRow2) / 2.0;
        vector.X = this.ElementBounds.Left - 27.0;
      }
      return point * matrix + vector * this.TruncateMatrix(matrix);
    }
  }
}
