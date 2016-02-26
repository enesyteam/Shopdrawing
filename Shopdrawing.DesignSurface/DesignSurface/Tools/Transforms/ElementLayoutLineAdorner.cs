// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ElementLayoutLineAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ElementLayoutLineAdorner : LayoutAdorner
  {
    private static DashStyle LineDashStyle = new DashStyle((IEnumerable<double>) new double[2]
    {
      2.0,
      5.0
    }, 0.0);
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

    public bool IsAuto
    {
      get
      {
        HorizontalAlignment horizontalAlignment = (HorizontalAlignment) this.Element.GetComputedValue(BaseFrameworkElement.HorizontalAlignmentProperty);
        VerticalAlignment verticalAlignment = (VerticalAlignment) this.Element.GetComputedValue(BaseFrameworkElement.VerticalAlignmentProperty);
        switch (this.Type)
        {
          case ElementLayoutAdornerType.Left:
            return horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Right;
          case ElementLayoutAdornerType.Top:
            return verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Bottom;
          case ElementLayoutAdornerType.Right:
            return horizontalAlignment == HorizontalAlignment.Center || horizontalAlignment == HorizontalAlignment.Left;
          case ElementLayoutAdornerType.Bottom:
            return verticalAlignment == VerticalAlignment.Center || verticalAlignment == VerticalAlignment.Top;
          default:
            throw new NotSupportedException();
        }
      }
    }

    static ElementLayoutLineAdorner()
    {
      ElementLayoutLineAdorner.LineDashStyle.Freeze();
    }

    public ElementLayoutLineAdorner(AdornerSet adornerSet, ElementLayoutAdornerType type)
      : base(adornerSet, type == ElementLayoutAdornerType.Left || type == ElementLayoutAdornerType.Right)
    {
      this.type = type;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (this.Element.ParentElement == null || this.Element.Visual == null)
        return;
      Point pointBegin;
      Point pointEnd;
      this.GetPoints(out pointBegin, out pointEnd);
      string text = string.Empty;
      if (!this.IsAuto)
      {
        Matrix computedTransformToRoot = this.DesignerContext.ActiveView.GetComputedTransformToRoot((SceneElement) this.Element);
        text = ((pointEnd - pointBegin) * computedTransformToRoot).Length.ToString("0.###", (IFormatProvider) CultureInfo.CurrentCulture);
      }
      Point a = pointBegin * matrix;
      Point b = pointEnd * matrix;
      Pen pen1 = this.ThinPen.Clone();
      pen1.Brush.Opacity = 0.5;
      if (this.IsAuto && VectorUtilities.SquaredDistance(a, b) < 10000000000.0)
        pen1.DashStyle = ElementLayoutLineAdorner.LineDashStyle;
      pen1.Freeze();
      Vector overflowDirection = b - a;
      overflowDirection.Normalize();
      SizeAdorner.DrawLineAndText(ctx, a, b, overflowDirection, pen1, this.ActiveBrush, text, 1.0);
      if (this.type != ElementLayoutAdornerType.Left)
        return;
      Transform transform1 = (Transform) this.Element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty);
      Transform transform2 = (Transform) null;
      IPropertyId propertyKey = (IPropertyId) this.Element.ProjectContext.ResolveProperty(BaseFrameworkElement.LayoutTransformProperty);
      if (propertyKey != null)
        transform2 = (Transform) this.Element.GetComputedValueAsWpf(propertyKey);
      if ((transform1 == null || transform1.Value.IsIdentity) && (transform2 == null || transform2.Value.IsIdentity))
        return;
      RectangleGeometry rectangleGeometry = new RectangleGeometry(this.DesignerContext.ActiveSceneViewModel.GetLayoutDesignerForChild((SceneElement) this.Element, true).GetChildRect(this.Element), 0.0, 0.0, (Transform) new MatrixTransform(this.Element.GetComputedTransformFromVisualParent() * matrix));
      rectangleGeometry.Freeze();
      Pen pen2 = this.ThinPen.Clone();
      pen2.Brush.Opacity = 0.5;
      pen2.Freeze();
      ctx.DrawGeometry((Brush)null, pen2, (System.Windows.Media.Geometry)rectangleGeometry);
    }

    private void GetPoints(out Point pointBegin, out Point pointEnd)
    {
      IViewVisual viewVisual = this.Element.Visual as IViewVisual;
      Rect rect = viewVisual != null ? viewVisual.GetLayoutSlot() : Rect.Empty;
      Matrix fromVisualParent = this.Element.GetComputedTransformFromVisualParent();
      Rect childRect = this.DesignerContext.ActiveSceneViewModel.GetLayoutDesignerForChild((SceneElement) this.Element, true).GetChildRect(this.Element);
      double y = (childRect.Top + childRect.Bottom) / 2.0;
      double x = (childRect.Left + childRect.Right) / 2.0;
      switch (this.type)
      {
        case ElementLayoutAdornerType.Left:
          pointBegin = new Point(childRect.Left, y);
          pointEnd = new Point(rect.Left, y);
          break;
        case ElementLayoutAdornerType.Top:
          pointBegin = new Point(x, childRect.Top);
          pointEnd = new Point(x, rect.Top);
          break;
        case ElementLayoutAdornerType.Right:
          pointBegin = new Point(childRect.Right, y);
          pointEnd = new Point(rect.Right, y);
          break;
        case ElementLayoutAdornerType.Bottom:
          pointBegin = new Point(x, childRect.Bottom);
          pointEnd = new Point(x, rect.Bottom);
          break;
        default:
          throw new NotSupportedException();
      }
      pointBegin *= fromVisualParent;
      pointEnd *= fromVisualParent;
    }
  }
}
