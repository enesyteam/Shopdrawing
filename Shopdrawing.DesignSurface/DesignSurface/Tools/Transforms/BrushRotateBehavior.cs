// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushRotateBehavior
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
  internal sealed class BrushRotateBehavior : BrushTransformBehavior
  {
    private static readonly double shiftSnapAngle = 15.0;
    private CanonicalTransform initialBrushTransform;
    private double unsnappedAngle;
    private bool snapping;
    private double lastPointerAngle;
    private Point brushCenter;
    private Vector startVector;
    private Matrix pixelToBrushTransform;
    private double inverseCoordinateSpace;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitRotateBrush;
      }
    }

    private BrushRotateAdorner ActiveAdorner
    {
      get
      {
        return (BrushRotateAdorner) base.ActiveAdorner;
      }
    }

    public BrushRotateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.pixelToBrushTransform = this.ActiveView.GetComputedTransformFromRoot(this.EditingElement) * this.ActiveAdorner.GetCompleteInverseBrushTransformMatrix(false);
      this.brushCenter = this.ActiveAdorner.RelativeBrushCenter;
      this.inverseCoordinateSpace = (double) Math.Sign(this.ActiveAdorner.BrushTransform.Value.Determinant);
      this.startVector = pointerPosition * this.pixelToBrushTransform - this.brushCenter;
      this.startVector.Normalize();
      this.lastPointerAngle = this.GetAngle(this.startVector);
      this.initialBrushTransform = new CanonicalTransform(this.ActiveAdorner.BrushTransform);
      this.unsnappedAngle = this.initialBrushTransform.RotationAngle;
      this.EnsureEditTransaction();
      this.snapping = this.IsShiftDown;
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      Vector sweep = dragCurrentPosition * this.pixelToBrushTransform - this.brushCenter;
      if (sweep.Length > 0.0)
        sweep.Normalize();
      double angle = this.GetAngle(sweep);
      double num = angle - this.lastPointerAngle;
      this.lastPointerAngle = angle;
      if (num > 180.0)
        num -= 360.0;
      else if (num < -180.0)
        num += 360.0;
      this.unsnappedAngle += num;
      this.UpdateRotation();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner);
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
      if (this.snapping != isShiftDown)
      {
        this.snapping = isShiftDown;
        this.UpdateRotation();
      }
      return true;
    }

    private double GetAngle(Vector sweep)
    {
      return this.inverseCoordinateSpace * 180.0 / Math.PI * Math.Atan2(sweep.Y, sweep.X);
    }

    private double SnapToAngle(double toSnap, double snapAngle)
    {
      return snapAngle * Math.Round(toSnap / snapAngle);
    }

    private void UpdateRotation()
    {
      double num = this.unsnappedAngle;
      if (this.snapping)
        num = this.SnapToAngle(this.unsnappedAngle, BrushRotateBehavior.shiftSnapAngle);
      Point relativeBrushCenter = this.ActiveAdorner.RelativeBrushCenter;
      CanonicalTransform transform = new CanonicalTransform(this.initialBrushTransform);
      transform.UpdateCenter(relativeBrushCenter);
      transform.ApplyRotation(num - this.initialBrushTransform.RotationAngle, relativeBrushCenter);
      this.SetBrushTransform(transform);
    }
  }
}
