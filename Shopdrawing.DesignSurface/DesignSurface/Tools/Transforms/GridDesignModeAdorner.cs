// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.GridDesignModeAdorner
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
  public class GridDesignModeAdorner : LayoutLockAdorner
  {
    private static ImageSource LockImageCache = FileTable.GetImageSource("Resources\\Icons\\Assets\\asset_grid_12x12_on.png");
    private static ImageSource UnlockImageCache = FileTable.GetImageSource("Resources\\Icons\\Assets\\asset_canvas_12x12_on.png");

    public GridElement Grid
    {
      get
      {
        return (GridElement) this.Element;
      }
    }

    protected override double Value
    {
      get
      {
        return double.NaN;
      }
    }

    protected override LayoutLockState LayoutLockState
    {
      get
      {
        return this.Grid.ViewModel.IsInGridDesignMode ? LayoutLockState.Locked : LayoutLockState.Unlocked;
      }
    }

    protected override ImageSource LockImage
    {
      get
      {
        return GridDesignModeAdorner.LockImageCache;
      }
    }

    protected override ImageSource UnlockImage
    {
      get
      {
        return GridDesignModeAdorner.UnlockImageCache;
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
        if (this.LayoutLockState != LayoutLockState.Locked)
          return (object) StringTable.GridDesignModeCanvasTooltip;
        return (object) StringTable.GridDesignModeGridTooltip;
      }
    }

    private double CenterOffset
    {
      get
      {
        return 12.5;
      }
    }

    public GridDesignModeAdorner(AdornerSet adornerSet)
      : base(adornerSet, true)
    {
    }

    protected override Point GetCenter(Matrix matrix)
    {
      return new Point() * matrix + new Vector()
      {
        Y = (this.ElementBounds.Top - this.CenterOffset),
        X = (this.ElementBounds.Left - this.CenterOffset)
      } * this.TruncateMatrix(matrix);
    }
  }
}
