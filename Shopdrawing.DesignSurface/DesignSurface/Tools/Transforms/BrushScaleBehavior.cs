// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushScaleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushScaleBehavior : BrushTransformBehavior
  {
    private bool scaleAroundCenter;
    private bool constrainAspectRatio;
    private CanonicalTransform initialBrushTransform;
    private Matrix initialRootToBrushMatrix;
    private Point initialPointerPosition;
    private Point currentPointerPosition;
    private Point currentCenter;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitScaleBrush;
      }
    }

    private BrushScaleAdorner ActiveAdorner
    {
      get
      {
        return (BrushScaleAdorner) base.ActiveAdorner;
      }
    }

    public BrushScaleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.initialBrushTransform = new CanonicalTransform(this.ActiveAdorner.BrushTransform);
      this.initialRootToBrushMatrix = this.ActiveView.GetComputedTransformFromRoot(this.EditingElement) * this.ActiveAdorner.GetCompleteInverseBrushTransformMatrix(false);
      this.initialPointerPosition = pointerPosition * this.initialRootToBrushMatrix;
      this.constrainAspectRatio = this.IsShiftDown;
      this.scaleAroundCenter = this.IsAltDown;
      this.currentPointerPosition = this.initialPointerPosition;
      this.UpdateCenter();
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.currentPointerPosition = dragCurrentPosition * this.initialRootToBrushMatrix;
      this.UpdateScale();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.CommitEditTransaction();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.CommitEditTransaction();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool flag = false;
      bool isAltDown = this.IsAltDown;
      if (this.scaleAroundCenter != isAltDown)
      {
        this.scaleAroundCenter = isAltDown;
        this.UpdateCenter();
        flag = true;
      }
      bool isShiftDown = this.IsShiftDown;
      if (flag || this.constrainAspectRatio != isShiftDown)
      {
        this.constrainAspectRatio = isShiftDown;
        this.UpdateScale();
      }
      return true;
    }

    private void UpdateCenter()
    {
      Rect brushBounds = this.ActiveAdorner.BrushBounds;
      if (!this.ActiveAdorner.IsStretchedToFitBoundingBox)
      {
        Rect actualBounds = this.ActiveView.GetActualBounds(this.EditingElement.ViewObject);
        double m11 = actualBounds.Width == 0.0 ? 1.0 : 1.0 / actualBounds.Width;
        double m22 = actualBounds.Height == 0.0 ? 1.0 : 1.0 / actualBounds.Height;
        Matrix matrix = new Matrix(m11, 0.0, 0.0, m22, -m11 * actualBounds.X, -m22 * actualBounds.Y);
        brushBounds.Transform(matrix);
      }
      this.currentCenter = new Point(brushBounds.X + 0.5 * brushBounds.Width, brushBounds.Y + 0.5 * brushBounds.Height);
      if (this.scaleAroundCenter)
        return;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.Left))
        this.currentCenter.X = brushBounds.Right;
      else if (this.ActiveAdorner.TestFlags(EdgeFlags.Right))
        this.currentCenter.X = brushBounds.Left;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.Top))
      {
        this.currentCenter.Y = brushBounds.Bottom;
      }
      else
      {
        if (!this.ActiveAdorner.TestFlags(EdgeFlags.Bottom))
          return;
        this.currentCenter.Y = brushBounds.Top;
      }
    }

    private void UpdateScale()
    {
      Vector scale = new Vector(1.0, 1.0);
      if (this.constrainAspectRatio)
        scale.X = scale.Y = 0.0;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight))
      {
        double num = this.initialPointerPosition.X - this.currentCenter.X;
        scale.X = Math.Abs(num) >= 1E-06 ? (this.currentPointerPosition.X - this.currentCenter.X) / num : 1.0;
      }
      if (this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom))
      {
        double num = this.initialPointerPosition.Y - this.currentCenter.Y;
        scale.Y = Math.Abs(num) >= 1E-06 ? (this.currentPointerPosition.Y - this.currentCenter.Y) / num : 1.0;
      }
      if (this.constrainAspectRatio)
      {
        double num = Math.Max(Math.Abs(scale.X), Math.Abs(scale.Y));
        scale.X = scale.X < 0.0 ? -num : num;
        scale.Y = scale.Y < 0.0 ? -num : num;
      }
      CanonicalTransform transform = new CanonicalTransform(this.initialBrushTransform);
      transform.UpdateCenter(this.ActiveAdorner.RelativeBrushCenter);
      transform.ApplyScale(scale, transform.Center, this.currentCenter);
      this.SetBrushTransform(transform);
    }
  }
}
