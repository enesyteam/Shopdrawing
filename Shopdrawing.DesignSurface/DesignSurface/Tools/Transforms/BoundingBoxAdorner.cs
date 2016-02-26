// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BoundingBoxAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class BoundingBoxAdorner : EventHandlingAdorner
  {
    private bool shouldDraw;

    protected virtual bool ShouldDraw
    {
      get
      {
        return this.shouldDraw;
      }
    }

    public override bool SupportsProjectionTransforms
    {
      get
      {
        return true;
      }
    }

    public virtual Pen BorderPen
    {
      get
      {
        return this.ThinPen;
      }
    }

    public BoundingBoxAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
      this.shouldDraw = PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) this.Element.Type);
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!(eventArgs.PropertyName == "BoundingBoxAdornerIsVisible"))
        return;
      this.shouldDraw = (bool) eventArgs.Value && PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) this.Element.Type);
      this.InvalidateRender();
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      AnimatableAdornerSet animatableAdornerSet = this.AdornerSet as AnimatableAdornerSet;
      this.DrawAnimate(drawingContext, matrix, animatableAdornerSet != null ? (IEnumerable<IAdornerAnimation>) animatableAdornerSet.ActiveAnimations : (IEnumerable<IAdornerAnimation>) null);
    }

    private void DrawAnimate(DrawingContext ctx, Matrix matrix, IEnumerable<IAdornerAnimation> activeAdornerAnimations)
    {
      IAdornerAnimation adornerAnimation = activeAdornerAnimations != null ? Enumerable.FirstOrDefault<IAdornerAnimation>(activeAdornerAnimations, (Func<IAdornerAnimation, bool>) (item => item.Name == "PreviewBox")) : (IAdornerAnimation) null;
      if (!this.ShouldDraw || this.Element.Visual == null)
        return;
      Rect elementBounds = this.ElementBounds;
      Pen pen = this.BorderPen;
      if (pen.IsFrozen && adornerAnimation != null)
      {
        pen = adornerAnimation.ClientData as Pen;
        if (pen == null)
        {
          pen = this.BorderPen.Clone();
          adornerAnimation.ClientData = (object) pen;
        }
      }
      if (adornerAnimation != null)
        pen.Brush.Opacity = adornerAnimation.LerpValue;
      this.DrawBounds(ctx, elementBounds, (Brush) null, pen);
      if (!this.AdornerSet.ToolContext.Tool.ShowDimensions || !this.Element.ViewModel.ElementSelectionSet.Selection.Contains(this.Element) || this.ElementSet.IsPrimaryTransformNonAffine)
        return;
      Matrix computedTransformToRoot = this.DesignerContext.ActiveView.GetComputedTransformToRoot(this.Element);
      SizeAdorner.DrawDimension(ctx, ElementLayoutAdornerType.Right, matrix, computedTransformToRoot, pen, elementBounds, 1.0);
      SizeAdorner.DrawDimension(ctx, ElementLayoutAdornerType.Bottom, matrix, computedTransformToRoot, pen, elementBounds, 1.0);
    }

    protected void DrawBounds(DrawingContext ctx, Brush fillBrush, Pen pen)
    {
      this.DrawBounds(ctx, this.ElementBounds, fillBrush, pen);
    }

    private void DrawBounds(DrawingContext ctx, Rect bounds, Brush fillBrush, Pen pen)
    {
        System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(this.DesignerContext.ActiveView, this.Element, bounds, pen.Thickness, true);
      ctx.DrawGeometry(fillBrush, pen, rectangleGeometry);
    }
  }
}
