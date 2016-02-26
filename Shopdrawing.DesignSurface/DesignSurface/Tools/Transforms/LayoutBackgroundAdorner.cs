// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LayoutBackgroundAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  public abstract class LayoutBackgroundAdorner : LayoutAdorner
  {
    protected static SolidColorBrush UnselectedBrush = new SolidColorBrush(Color.FromArgb((byte) 1, (byte) 1, (byte) 1, (byte) 1));
    protected static SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromArgb((byte) 100, (byte) 100, (byte) 100, (byte) 100));
    protected const int ClickablePointOffsetAwayFromIcon = 20;
    protected const int ClickablePointOffsetAwayFromArtboard = 5;
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

    static LayoutBackgroundAdorner()
    {
      LayoutBackgroundAdorner.UnselectedBrush.Freeze();
      LayoutBackgroundAdorner.SelectedBrush.Freeze();
    }

    protected LayoutBackgroundAdorner(AdornerSet adornerSet, bool isX, int index)
      : base(adornerSet, isX)
    {
      this.index = index;
    }
  }
}
