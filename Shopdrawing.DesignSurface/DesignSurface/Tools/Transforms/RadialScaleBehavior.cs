// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialScaleBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class RadialScaleBehavior : BrushTransformBehavior
  {
    private bool constrainAspectRatio;
    private Matrix rootToBrushMatrix;
    private Point initialPointerPosition;
    private double radiusX;
    private double radiusY;
    private Point currentPointerPosition;

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

    public RadialScaleBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.rootToBrushMatrix = this.ActiveView.GetComputedTransformFromRoot(this.EditingElement) * this.ActiveAdorner.GetCompleteInverseBrushTransformMatrix(true);
      this.initialPointerPosition = pointerPosition * this.rootToBrushMatrix;
      this.constrainAspectRatio = this.IsShiftDown;
      this.currentPointerPosition = this.initialPointerPosition;
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.currentPointerPosition = dragCurrentPosition * this.rootToBrushMatrix;
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
      bool isShiftDown = this.IsShiftDown;
      if (this.constrainAspectRatio != isShiftDown)
      {
        this.constrainAspectRatio = isShiftDown;
        this.UpdateScale();
      }
      return true;
    }

    private void UpdateScale()
    {
      SceneElement primarySelection = this.ActiveView.ElementSelectionSet.PrimarySelection;
      if (primarySelection == null)
        return;
      PropertyReference propertyReference = this.GetBrushPropertyReference((SceneNode) primarySelection);
      if (propertyReference == null)
        return;
      this.EnsureEditTransaction();
      if (!this.HasMouseMovedAfterDown)
        this.CopyPrimaryBrushToSelection();
      ReferenceStep referenceStep1 = (ReferenceStep) this.ActiveView.ProjectContext.ResolveProperty(RadialGradientBrushNode.RadiusXProperty);
      ReferenceStep referenceStep2 = (ReferenceStep) this.ActiveView.ProjectContext.ResolveProperty(RadialGradientBrushNode.RadiusYProperty);
      if (!this.HasMouseMovedAfterDown)
      {
        object computedValue = primarySelection.GetComputedValue(propertyReference);
        this.radiusX = (double) referenceStep1.GetCurrentValue(computedValue);
        this.radiusY = (double) referenceStep2.GetCurrentValue(computedValue);
      }
      Vector vector = new Vector(0.0, 0.0);
      bool flag = false;
      if (this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight))
      {
        vector.X = this.currentPointerPosition.X - this.initialPointerPosition.X;
        if (this.ActiveAdorner.TestFlags(EdgeFlags.Left) && this.radiusX > 0.0 || this.ActiveAdorner.TestFlags(EdgeFlags.Right) && this.radiusX < 0.0)
          vector.X *= -1.0;
        if (this.constrainAspectRatio && Math.Abs(this.radiusX) > 0.001)
        {
          vector.Y = vector.X * this.radiusY / this.radiusX;
          flag = true;
        }
      }
      if (this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom))
      {
        vector.Y = this.currentPointerPosition.Y - this.initialPointerPosition.Y;
        if (this.ActiveAdorner.TestFlags(EdgeFlags.Top) && this.radiusY > 0.0 || this.ActiveAdorner.TestFlags(EdgeFlags.Bottom) && this.radiusY < 0.0)
          vector.Y *= -1.0;
        if (this.constrainAspectRatio && Math.Abs(this.radiusY) > 0.001)
        {
          vector.X = vector.Y * this.radiusX / this.radiusY;
          flag = true;
        }
      }
      vector.X = RoundingHelper.RoundLength(vector.X + this.radiusX);
      vector.Y = RoundingHelper.RoundLength(vector.Y + this.radiusY);
      if (this.ActiveAdorner.TestFlags(EdgeFlags.LeftOrRight) || flag)
        this.SetBrushValue(RadialGradientBrushNode.RadiusXProperty, (object) vector.X);
      if (this.ActiveAdorner.TestFlags(EdgeFlags.TopOrBottom) || flag)
        this.SetBrushValue(RadialGradientBrushNode.RadiusYProperty, (object) vector.Y);
      this.UpdateEditTransaction();
    }
  }
}
