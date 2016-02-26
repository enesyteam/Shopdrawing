// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialGradientBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class RadialGradientBehavior : BrushTransformBehavior
  {
    private Matrix initialElementToContextMatrix;
    private Matrix initialContextToElementMatrix;
    private Matrix rootToBrushMatrix;
    private Matrix brushToRootMatrix;
    private Point initialCenterInBrush;
    private Point initialCenterInDocument;
    private double dragOffset;
    private Point initialPointerPosition;
    private double radiusX;
    private double radiusY;
    private string actionString;

    private RadialGradientAdorner ActiveAdorner
    {
      get
      {
        return (RadialGradientAdorner) base.ActiveAdorner;
      }
    }

    public override string ActionString
    {
      get
      {
        return this.actionString;
      }
    }

    public RadialGradientBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.initialElementToContextMatrix = this.ActiveView.GetComputedTransformToRoot(this.EditingElement);
      this.initialContextToElementMatrix = ElementUtilities.GetInverseMatrix(this.initialElementToContextMatrix);
      this.initialCenterInBrush = this.ActiveAdorner.BrushCenter;
      this.rootToBrushMatrix = this.initialContextToElementMatrix * this.ActiveAdorner.GetCompleteInverseBrushTransformMatrix(true);
      this.brushToRootMatrix = ElementUtilities.GetInverseMatrix(this.rootToBrushMatrix);
      this.initialCenterInDocument = this.ActiveAdorner.TransformPoint(this.initialCenterInBrush, true) * this.initialElementToContextMatrix;
      this.initialPointerPosition = pointerPosition;
      Point startPoint;
      Point endPoint;
      if (this.ActiveAdorner.GetBrushEndpoints(out startPoint, out endPoint))
      {
        double offset = 1.0;
        if (this.ActiveAdorner.Kind == RadialGradientAdornerKind.GradientOriginPoint)
          offset = 0.0;
        Point S = startPoint * this.brushToRootMatrix;
        Point E = endPoint * this.brushToRootMatrix;
        Vector perpendicular;
        this.dragOffset = GradientStopBehavior.VectorProjection(S, E, pointerPosition, offset, out perpendicular);
        this.dragOffset = Math.Abs(this.dragOffset * (E - S).Length);
      }
      this.CancelEditTransaction();
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      switch (this.ActiveAdorner.Kind)
      {
        case RadialGradientAdornerKind.GradientOriginPoint:
          this.actionString = StringTable.UndoUnitRadialGradientOrigin;
          if (!this.HasMouseMovedAfterDown)
            this.CopyPrimaryBrushToSelection();
          Point point = this.initialCenterInBrush;
          if (Tolerances.HaveMoved(dragCurrentPosition, this.initialCenterInDocument, this.ActiveView.Zoom))
          {
            Point positionInBrush = this.ActiveAdorner.InverseTransformPoint(dragCurrentPosition * this.initialContextToElementMatrix, true) * this.brushToRootMatrix;
            BrushTransformBehavior.OffsetDragPoint(ref positionInBrush, this.initialCenterInBrush * this.brushToRootMatrix, this.dragOffset, 0.0);
            point = RoundingHelper.RoundPosition(positionInBrush * this.rootToBrushMatrix);
          }
          if (this.ActiveAdorner.GradientOriginPoint != point)
          {
            this.SetBrushValue(RadialGradientBrushNode.GradientOriginProperty, (object) point);
            this.UpdateEditTransaction();
            break;
          }
          break;
        case RadialGradientAdornerKind.RadiusPoint:
          this.actionString = StringTable.UndoUnitScaleRadialGradient;
          if (!this.HasMouseMovedAfterDown)
          {
            this.CopyPrimaryBrushToSelection();
            this.radiusX = (double) this.GetBrushValue(RadialGradientBrushNode.RadiusXProperty);
            this.radiusY = (double) this.GetBrushValue(RadialGradientBrushNode.RadiusYProperty);
            if (Math.Abs(this.radiusX) < 0.01)
              this.radiusX = 0.01;
            if (Math.Abs(this.radiusY) < 0.01)
              this.radiusY = 0.01;
          }
          Vector perpendicular;
          double num1 = GradientStopBehavior.VectorProjection(this.ActiveAdorner.BrushCenter * this.brushToRootMatrix, this.initialPointerPosition, dragCurrentPosition, 0.0, out perpendicular);
          double num2 = RoundingHelper.RoundLength(this.radiusX * num1);
          double num3 = RoundingHelper.RoundLength(this.radiusY * num1);
          this.SetBrushValue(RadialGradientBrushNode.RadiusXProperty, (object) num2);
          this.SetBrushValue(RadialGradientBrushNode.RadiusYProperty, (object) num3);
          this.UpdateEditTransaction();
          break;
      }
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
      if (clickCount > 1)
      {
        this.actionString = StringTable.UndoUnitResetRadialGradient;
        Point point = new Point(0.5, 0.5);
        double num1 = 1.0;
        double num2 = 1.0;
        if (this.ActiveAdorner.IsStretchedToFitBoundingBox)
        {
          point = new Point(0.5, 0.5);
          num1 = num2 = 0.5;
        }
        else
        {
          BaseFrameworkElement frameworkElement = this.EditingElement as BaseFrameworkElement;
          if (frameworkElement == null)
          {
            StyleNode styleNode = this.EditingElement as StyleNode;
            if (styleNode != null)
              frameworkElement = styleNode.TargetElement;
          }
          if (frameworkElement != null)
          {
            Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
            num1 = RoundingHelper.RoundLength(computedTightBounds.Width / 2.0);
            num2 = RoundingHelper.RoundLength(computedTightBounds.Height / 2.0);
            point = RoundingHelper.RoundPosition(new Point(computedTightBounds.X + num1, computedTightBounds.Y + num2));
          }
        }
        this.CopyPrimaryBrushToSelection();
        this.ClearBrushTransform();
        if ((Point) this.GetBrushValue(RadialGradientBrushNode.GradientOriginProperty) != point)
          this.SetBrushValue(RadialGradientBrushNode.GradientOriginProperty, (object) point);
        if ((Point) this.GetBrushValue(RadialGradientBrushNode.CenterProperty) != point)
          this.SetBrushValue(RadialGradientBrushNode.CenterProperty, (object) point);
        if ((double) this.GetBrushValue(RadialGradientBrushNode.RadiusXProperty) != num1)
          this.SetBrushValue(RadialGradientBrushNode.RadiusXProperty, (object) num1);
        if ((double) this.GetBrushValue(RadialGradientBrushNode.RadiusYProperty) != num2)
          this.SetBrushValue(RadialGradientBrushNode.RadiusYProperty, (object) num2);
      }
      this.CommitEditTransaction();
      return base.OnClickEnd(pointerPosition, clickCount);
    }
  }
}
