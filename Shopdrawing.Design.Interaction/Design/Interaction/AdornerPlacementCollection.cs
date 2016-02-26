// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerPlacementCollection
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.Interaction;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Windows.Design.Interaction
{
  public class AdornerPlacementCollection : ObservableCollection<IAdornerPlacement>
  {
    private Vector _topLeft;
    private Vector _size;

    internal Vector TopLeft
    {
      get
      {
        return this._topLeft;
      }
      set
      {
        this._topLeft = value;
      }
    }

    internal Vector Size
    {
      get
      {
        return this._size;
      }
      set
      {
        this._size = value;
      }
    }

    internal virtual void ComputePlacement(AdornerCoordinateSpace space, UIElement adorner, ViewItem adornedElement, Vector zoom, System.Windows.Size finalSize)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      double height = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      double width = 0.0;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      System.Windows.Size viewFinalSize = finalSize;
      foreach (IAdornerPlacement adornerPlacement in (Collection<IAdornerPlacement>) this)
      {
        foreach (AdornerPlacementValue adornerPlacementValue in adornerPlacement.GetSizeTerms(space, adorner, adornedElement, zoom, viewFinalSize))
        {
          switch (adornerPlacementValue.Term)
          {
            case AdornerPlacementDimension.Left:
              num3 += adornerPlacementValue.Contribution;
              flag3 = true;
              continue;
            case AdornerPlacementDimension.Top:
              num1 += adornerPlacementValue.Contribution;
              flag1 = true;
              continue;
            case AdornerPlacementDimension.Right:
              num4 += adornerPlacementValue.Contribution;
              flag3 = true;
              continue;
            case AdornerPlacementDimension.Bottom:
              num2 += adornerPlacementValue.Contribution;
              flag2 = true;
              continue;
            case AdornerPlacementDimension.Width:
              width += adornerPlacementValue.Contribution;
              continue;
            case AdornerPlacementDimension.Height:
              height += adornerPlacementValue.Contribution;
              continue;
            default:
              continue;
          }
        }
      }
      if (width < 0.0)
        width = 0.0;
      if (height < 0.0)
        height = 0.0;
      System.Windows.Size computedAdornerSize = new System.Windows.Size(width, height);
      foreach (IAdornerPlacement adornerPlacement in (Collection<IAdornerPlacement>) this)
      {
        foreach (AdornerPlacementValue adornerPlacementValue in adornerPlacement.GetPositionTerms(space, adorner, adornedElement, zoom, computedAdornerSize))
        {
          switch (adornerPlacementValue.Term)
          {
            case AdornerPlacementDimension.Left:
              num3 += adornerPlacementValue.Contribution;
              flag3 = true;
              continue;
            case AdornerPlacementDimension.Top:
              num1 += adornerPlacementValue.Contribution;
              flag1 = true;
              continue;
            case AdornerPlacementDimension.Right:
              num4 += adornerPlacementValue.Contribution;
              flag3 = true;
              continue;
            case AdornerPlacementDimension.Bottom:
              num2 += adornerPlacementValue.Contribution;
              flag2 = true;
              continue;
            case AdornerPlacementDimension.Width:
              width += adornerPlacementValue.Contribution;
              continue;
            case AdornerPlacementDimension.Height:
              height += adornerPlacementValue.Contribution;
              continue;
            default:
              continue;
          }
        }
      }
      double num5 = 0.0;
      if (flag3 && flag4)
        num5 = num4 - num3;
      double num6 = 0.0;
      if (flag1 && flag2)
        num6 = num2 - num1;
      double num7 = num5 + width;
      double num8 = num6 + height;
      this._topLeft.X = !flag4 || flag3 ? num3 : num4 - num7;
      this._size.X = num7;
      this._topLeft.Y = !flag2 || flag1 ? num1 : num2 - num8;
      this._size.Y = num8;
    }

    public void PositionRelativeToAdornerHeight(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToAdornerHeight(factor, offset, (DependencyObject) null));
    }

    public void PositionRelativeToAdornerHeight(double factor, double offset, DependencyObject relativeTo)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToAdornerHeight(factor, offset, relativeTo));
    }

    public void PositionRelativeToAdornerWidth(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToAdornerWidth(factor, offset, (DependencyObject) null));
    }

    public void PositionRelativeToAdornerWidth(double factor, double offset, DependencyObject relativeTo)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToAdornerWidth(factor, offset, relativeTo));
    }

    public void PositionRelativeToContentHeight(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToContentHeight(factor, offset, (ViewItem) null));
    }

    public void PositionRelativeToContentHeight(double factor, double offset, ViewItem relativeTo)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToContentHeight(factor, offset, relativeTo));
    }

    public void PositionRelativeToContentWidth(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToContentWidth(factor, offset, (ViewItem) null));
    }

    public void PositionRelativeToContentWidth(double factor, double offset, ViewItem relativeTo)
    {
      this.Add((IAdornerPlacement) new PositionRelativeToContentWidth(factor, offset, relativeTo));
    }

    public void SizeRelativeToAdornerDesiredHeight(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToAdornerDesiredHeight(factor, offset, (DependencyObject) null));
    }

    public void SizeRelativeToAdornerDesiredHeight(double factor, double offset, DependencyObject relativeTo)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToAdornerDesiredHeight(factor, offset, relativeTo));
    }

    public void SizeRelativeToAdornerDesiredWidth(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToAdornerDesiredWidth(factor, offset, (DependencyObject) null));
    }

    public void SizeRelativeToAdornerDesiredWidth(double factor, double offset, DependencyObject relativeTo)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToAdornerDesiredWidth(factor, offset, relativeTo));
    }

    public void SizeRelativeToContentHeight(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToContentHeight(factor, offset, (ViewItem) null));
    }

    public void SizeRelativeToContentHeight(double factor, double offset, ViewItem relativeTo)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToContentHeight(factor, offset, relativeTo));
    }

    public void SizeRelativeToContentWidth(double factor, double offset)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToContentWidth(factor, offset, (ViewItem) null));
    }

    public void SizeRelativeToContentWidth(double factor, double offset, ViewItem relativeTo)
    {
      this.Add((IAdornerPlacement) new SizeRelativeToContentWidth(factor, offset, relativeTo));
    }
  }
}
