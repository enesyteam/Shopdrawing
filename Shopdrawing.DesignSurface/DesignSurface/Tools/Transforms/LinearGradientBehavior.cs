// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.LinearGradientBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class LinearGradientBehavior : BrushTransformBehavior
  {
    private Matrix rootToBrushMatrix;
    private Matrix brushToRootMatrix;
    private Point centerInRootCoordinates;
    private double radiusInRootCoordinates;
    private string actionString;
    private Point dragCurrentPosition;
    private Key? dragCurrentKey;
    private bool? dragCurrentKeyIsUp;
    private Point initialStartPoint;
    private Point initialEndPoint;
    private double dragOffset;

    public override string ActionString
    {
      get
      {
        return this.actionString;
      }
    }

    private LinearGradientAdorner ActiveAdorner
    {
      get
      {
        return (LinearGradientAdorner) base.ActiveAdorner;
      }
    }

    public LinearGradientBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.dragCurrentKey = new Key?();
      this.dragCurrentKeyIsUp = new bool?();
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      Matrix computedTransformToRoot = this.ActiveView.GetComputedTransformToRoot(this.EditingElement);
      this.rootToBrushMatrix = ElementUtilities.GetInverseMatrix(computedTransformToRoot);
      this.rootToBrushMatrix *= this.ActiveAdorner.GetCompleteInverseBrushTransformMatrix(true);
      this.brushToRootMatrix = ElementUtilities.GetInverseMatrix(this.rootToBrushMatrix);
      if (this.ActiveAdorner.Kind == LinearGradientAdornerKind.StartRotation || this.ActiveAdorner.Kind == LinearGradientAdornerKind.EndRotation)
      {
        Point startPoint = this.ActiveAdorner.StartPoint;
        Point endPoint = this.ActiveAdorner.EndPoint;
        Point point = startPoint * computedTransformToRoot;
        Vector vector = 0.5 * (endPoint * computedTransformToRoot - point);
        this.centerInRootCoordinates = point + vector;
        this.radiusInRootCoordinates = vector.Length;
      }
      else if (this.ActiveAdorner.GetBrushEndpoints(out this.initialStartPoint, out this.initialEndPoint))
      {
        double offset = 0.0;
        if (this.ActiveAdorner.Kind == LinearGradientAdornerKind.EndPoint)
          offset = 1.0;
        Point S = this.initialStartPoint * this.brushToRootMatrix;
        Point E = this.initialEndPoint * this.brushToRootMatrix;
        Vector perpendicular;
        this.dragOffset = GradientStopBehavior.VectorProjection(S, E, pointerPosition, offset, out perpendicular);
        this.dragOffset = Math.Abs(this.dragOffset * (E - S).Length);
      }
      return true;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      this.dragCurrentPosition = dragCurrentPosition;
      switch (this.ActiveAdorner.Kind)
      {
        case LinearGradientAdornerKind.StartPoint:
        case LinearGradientAdornerKind.EndPoint:
          this.UpdatePosition();
          break;
        case LinearGradientAdornerKind.StartRotation:
        case LinearGradientAdornerKind.EndRotation:
          this.UpdateRotation();
          break;
      }
      this.ActiveView.EnsureVisible((IAdorner) this.ActiveAdorner, scrollNow);
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      this.CommitEditTransaction();
      this.dragCurrentKey = new Key?();
      this.dragCurrentKeyIsUp = new bool?();
      return base.OnDragEnd(dragStartPosition, dragEndPosition);
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (clickCount > 1)
      {
        this.CancelEditTransaction();
        this.actionString = StringTable.UndoUnitResetLinearGradient;
        Point point1 = new Point();
        Point point2 = new Point();
        if (this.ActiveAdorner.IsStretchedToFitBoundingBox)
        {
          point1 = new Point(0.5, 0.0);
          point2 = new Point(0.5, 1.0);
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
            point1 = RoundingHelper.RoundPosition(new Point((computedTightBounds.Left + computedTightBounds.Right) / 2.0, computedTightBounds.Top));
            point2 = RoundingHelper.RoundPosition(new Point((computedTightBounds.Left + computedTightBounds.Right) / 2.0, computedTightBounds.Bottom));
          }
        }
        this.CopyPrimaryBrushToSelection();
        this.ClearBrushTransform();
        if ((Point) this.GetBrushValue(LinearGradientBrushNode.StartPointProperty) != point1)
          this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) point1);
        if ((Point) this.GetBrushValue(LinearGradientBrushNode.EndPointProperty) != point2)
          this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) point2);
        this.UpdateEditTransaction();
      }
      this.CommitEditTransaction();
      return base.OnClickEnd(pointerPosition, clickCount);
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      if (this.IsDragging)
      {
        Key? nullable1 = this.dragCurrentKey;
        Key key = args.Key;
        if ((nullable1.GetValueOrDefault() != key ? 0 : (nullable1.HasValue ? true : false)) != 0)
        {
          bool? nullable2 = this.dragCurrentKeyIsUp;
          bool isUp = args.IsUp;
          if ((nullable2.GetValueOrDefault() != isUp ? 0 : (nullable2.HasValue ? true : false)) != 0)
            return true;
        }
        this.dragCurrentKey = new Key?(args.Key);
        this.dragCurrentKeyIsUp = new bool?(args.IsUp);
        if (args.Key == Key.LeftShift || args.Key == Key.RightShift || args.Key == Key.System && (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt))
        {
          switch (this.ActiveAdorner.Kind)
          {
            case LinearGradientAdornerKind.StartPoint:
            case LinearGradientAdornerKind.EndPoint:
              this.UpdatePosition();
              break;
            case LinearGradientAdornerKind.StartRotation:
            case LinearGradientAdornerKind.EndRotation:
              this.UpdateRotation();
              break;
          }
        }
      }
      return true;
    }

    private void UpdatePosition()
    {
      Point positionInBrush = this.dragCurrentPosition * this.rootToBrushMatrix;
      if (this.IsShiftDown)
      {
        Vector vector1 = this.initialEndPoint - this.initialStartPoint;
        double length = vector1.Length;
        if (length == 0.0)
        {
          positionInBrush = this.initialStartPoint;
        }
        else
        {
          Vector vector2 = vector1 / length;
          positionInBrush = this.initialStartPoint + vector2 * (positionInBrush - this.initialStartPoint) * vector2;
        }
      }
      Point point = RoundingHelper.RoundPosition(new Point(this.initialStartPoint.X + this.initialEndPoint.X - positionInBrush.X, this.initialStartPoint.Y + this.initialEndPoint.Y - positionInBrush.Y));
      if (this.ActiveAdorner.Kind == LinearGradientAdornerKind.StartPoint)
      {
        positionInBrush *= this.brushToRootMatrix;
        BrushTransformBehavior.OffsetDragPoint(ref positionInBrush, this.initialEndPoint * this.brushToRootMatrix, this.dragOffset, 0.001 / this.ActiveView.Zoom);
        positionInBrush = RoundingHelper.RoundPosition(positionInBrush * this.rootToBrushMatrix);
        this.actionString = StringTable.UndoUnitLinearGradientStart;
        if (!this.HasMouseMovedAfterDown)
          this.CopyPrimaryBrushToSelection();
        this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) positionInBrush);
        if (this.IsAltDown)
          this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) point);
      }
      else
      {
        positionInBrush *= this.brushToRootMatrix;
        BrushTransformBehavior.OffsetDragPoint(ref positionInBrush, this.initialStartPoint * this.brushToRootMatrix, this.dragOffset, 0.001 / this.ActiveView.Zoom);
        positionInBrush = RoundingHelper.RoundPosition(positionInBrush * this.rootToBrushMatrix);
        this.actionString = StringTable.UndoUnitLinearGradientEnd;
        if (!this.HasMouseMovedAfterDown)
          this.CopyPrimaryBrushToSelection();
        this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) positionInBrush);
        if (this.IsAltDown)
          this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) point);
      }
      this.UpdateEditTransaction();
    }

    private void UpdateRotation()
    {
      this.actionString = StringTable.UndoUnitLinearGradientRotation;
      if (!this.HasMouseMovedAfterDown)
        this.CopyPrimaryBrushToSelection();
      Vector vector = this.dragCurrentPosition - this.centerInRootCoordinates;
      double num = Math.Atan2(vector.Y, vector.X);
      if (this.IsShiftDown)
        num = Math.Round(num * 57.2957795130823 / 15.0) * 15.0 * (Math.PI / 180.0);
      vector = new Vector(this.radiusInRootCoordinates * Math.Cos(num), this.radiusInRootCoordinates * Math.Sin(num));
      Point point1 = this.centerInRootCoordinates + vector;
      Point point2 = this.centerInRootCoordinates - vector;
      Point point3 = RoundingHelper.RoundPosition(point1 * this.rootToBrushMatrix);
      Point point4 = RoundingHelper.RoundPosition(point2 * this.rootToBrushMatrix);
      if (this.ActiveAdorner.Kind == LinearGradientAdornerKind.EndRotation)
      {
        Point point5 = point3;
        point3 = point4;
        point4 = point5;
      }
      this.SetBrushValue(LinearGradientBrushNode.StartPointProperty, (object) point3);
      this.SetBrushValue(LinearGradientBrushNode.EndPointProperty, (object) point4);
      this.UpdateEditTransaction();
      this.Cursor = this.ActiveAdorner.AdornerSet.GetCursor((IAdorner) this.ActiveAdorner);
    }
  }
}
