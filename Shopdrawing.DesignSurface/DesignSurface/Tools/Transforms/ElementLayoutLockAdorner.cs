// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ElementLayoutLockAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ElementLayoutLockAdorner : LayoutLockAdorner
  {
    private static ImageSource LockImageH = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_marginLockH_on_16x16.png");
    private static ImageSource LockImageV = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_marginLockV_on_16x16.png");
    private static ImageSource UnlockImageH = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_marginUnlockH_on_16x16.png");
    private static ImageSource UnlockImageV = FileTable.GetImageSource("Resources\\Icons\\Adorners\\adorner_marginUnlockV_on_16x16.png");
    private ElementLayoutAdornerType type;

    public ElementLayoutAdornerType Type
    {
      get
      {
        return this.type;
      }
    }

    public BaseFrameworkElement Element
    {
      get
      {
        return (BaseFrameworkElement) base.Element;
      }
    }

    protected override double Value
    {
      get
      {
        return 0.0;
      }
    }

    protected override LayoutLockState LayoutLockState
    {
      get
      {
        HorizontalAlignment horizontalAlignment = (HorizontalAlignment) this.Element.GetComputedValue(BaseFrameworkElement.HorizontalAlignmentProperty);
        VerticalAlignment verticalAlignment = (VerticalAlignment) this.Element.GetComputedValue(BaseFrameworkElement.VerticalAlignmentProperty);
        bool flag;
        switch (this.Type)
        {
          case ElementLayoutAdornerType.Left:
            flag = horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Right;
            break;
          case ElementLayoutAdornerType.Top:
            flag = verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Bottom;
            break;
          case ElementLayoutAdornerType.Right:
            flag = horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Left;
            break;
          case ElementLayoutAdornerType.Bottom:
            flag = verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Top;
            break;
          default:
            throw new NotSupportedException();
        }
        return !flag ? LayoutLockState.Locked : LayoutLockState.Unlocked;
      }
    }

    protected override ImageSource LockImage
    {
      get
      {
        if (!this.IsX)
          return ElementLayoutLockAdorner.LockImageV;
        return ElementLayoutLockAdorner.LockImageH;
      }
    }

    protected override ImageSource UnlockImage
    {
      get
      {
        if (!this.IsX)
          return ElementLayoutLockAdorner.UnlockImageV;
        return ElementLayoutLockAdorner.UnlockImageH;
      }
    }

    protected override bool ParentRelative
    {
      get
      {
        return true;
      }
    }

    public ElementLayoutLockAdorner(AdornerSet adornerSet, ElementLayoutAdornerType type)
      : base(adornerSet, type == ElementLayoutAdornerType.Left || type == ElementLayoutAdornerType.Right)
    {
      this.type = type;
    }

    protected override Point GetCenter(Matrix matrix)
    {
      IViewVisual viewVisual = this.Element.Visual as IViewVisual;
      Rect rect = viewVisual != null ? viewVisual.GetLayoutSlot() : Rect.Empty;
      Matrix fromVisualParent = this.Element.GetComputedTransformFromVisualParent();
      Rect childRect = this.DesignerContext.ActiveSceneViewModel.GetLayoutDesignerForChild((SceneElement) this.Element, true).GetChildRect(this.Element);
      double y = (childRect.Top + childRect.Bottom) / 2.0;
      double x = (childRect.Left + childRect.Right) / 2.0;
      Point point;
      switch (this.type)
      {
        case ElementLayoutAdornerType.Left:
          point = new Point(rect.Left, y);
          break;
        case ElementLayoutAdornerType.Top:
          point = new Point(x, rect.Top);
          break;
        case ElementLayoutAdornerType.Right:
          point = new Point(rect.Right, y);
          break;
        case ElementLayoutAdornerType.Bottom:
          point = new Point(x, rect.Bottom);
          break;
        default:
          throw new NotSupportedException();
      }
      matrix = fromVisualParent * matrix;
      return point * matrix;
    }
  }
}
