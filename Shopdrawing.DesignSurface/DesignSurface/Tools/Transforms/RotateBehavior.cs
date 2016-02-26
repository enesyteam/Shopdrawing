// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RotateBehavior : AdornedToolBehavior
  {
    private static readonly double shiftSnapAngle = 15.0;
    private Point rootCenter;
    private double lastPointerAngle;
    private double unsnappedAngle;
    private double inverseCoordinateSpace;
    private bool snapping;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitRotate;
      }
    }

    protected double UnsnappedAngle
    {
      get
      {
        return this.unsnappedAngle;
      }
      set
      {
        this.unsnappedAngle = value;
      }
    }

    private RotateAdorner ActiveAdorner
    {
      get
      {
        return (RotateAdorner) base.ActiveAdorner;
      }
    }

    public RotateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.RotateElement);
      this.inverseCoordinateSpace = 1.0;
      SceneView activeView = this.ActiveView;
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) this.EditingElement.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty));
      this.rootCenter = this.EditingElementSet.RenderTransformOriginInElementCoordinates * this.EditingElementSet.GetTransformMatrix((IViewObject) this.ActiveView.HitTestRoot);
      Matrix matrix = Matrix.Identity;
      if (this.EditingElement != this.RootNode)
      {
        SceneNode parent = this.EditingElement.Parent;
        BaseFrameworkElement frameworkElement = (BaseFrameworkElement) null;
        while (parent != null && (frameworkElement = parent as BaseFrameworkElement) == null)
          parent = parent.Parent;
        if (frameworkElement != null && frameworkElement.Visual != null)
          matrix = activeView.GetComputedTransformToRoot((SceneElement) frameworkElement);
      }
      this.inverseCoordinateSpace = (double) Math.Sign(matrix.Determinant);
      this.lastPointerAngle = this.GetAngle(pointerPosition);
      this.unsnappedAngle = canonicalTransform.RotationAngle;
      this.EnsureEditTransaction();
      this.snapping = false;
      this.snapping = this.IsShiftDown;
      this.Initialize();
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      double angle = this.GetAngle(dragCurrentPosition);
      double num = angle - this.lastPointerAngle;
      this.lastPointerAngle = angle;
      if (num > 180.0)
        num -= 360.0;
      else if (num < -180.0)
        num += 360.0;
      this.unsnappedAngle += num;
      this.UpdateRotation();
      this.Cursor = this.ActiveAdorner.AdornerSet.GetCursor((IAdorner) this.ActiveAdorner);
      this.UpdateEditTransaction();
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.AllDone();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      this.AllDone();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    private void AllDone()
    {
      this.Finish();
      this.CommitEditTransaction();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.RotateElement);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isShiftDown = this.IsShiftDown;
      if (this.snapping != isShiftDown)
      {
        this.snapping = isShiftDown;
        this.UpdateRotation();
        this.UpdateEditTransaction();
      }
      return true;
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void Finish()
    {
    }

    protected virtual void ApplyRotation(double angle)
    {
      this.EditingElement.SetValue(this.EditingElement.Platform.Metadata.CommonProperties.RenderTransformRotationAngle, (object) angle);
    }

    private double GetAngle(Point point)
    {
      return this.inverseCoordinateSpace * Math.Atan2(point.Y - this.rootCenter.Y, point.X - this.rootCenter.X) * 180.0 / Math.PI;
    }

    private double SnapToAngle(double toSnap, double snapAngle)
    {
      return snapAngle * Math.Round(toSnap / snapAngle);
    }

    private void UpdateRotation()
    {
      double num = this.unsnappedAngle;
      if (this.snapping)
        num = this.SnapToAngle(this.unsnappedAngle, RotateBehavior.shiftSnapAngle);
      this.ApplyRotation(RoundingHelper.RoundAngle(num));
    }
  }
}
