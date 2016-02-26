// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SnapLineAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal sealed class SnapLineAdorner : Adorner
  {
    private static Pen SolidSnapLinePen = new Pen((Brush) Brushes.White, 1.0);
    private static Pen DottedSnapLinePen;
    private static Brush MarginBrush;
    private SceneElement targetElement;
    private Rect targetBounds;
    private List<SnapLine> snapLines;

    static SnapLineAdorner()
    {
      SnapLineAdorner.SolidSnapLinePen.Freeze();
      SnapLineAdorner.DottedSnapLinePen = new Pen((Brush) Brushes.Red, 1.0);
      SnapLineAdorner.DottedSnapLinePen.DashStyle = new DashStyle((IEnumerable<double>) new double[2]
      {
        4.0,
        4.0
      }, 0.0);
      SnapLineAdorner.DottedSnapLinePen.Freeze();
      SnapLineAdorner.MarginBrush = (Brush) new SolidColorBrush(Color.FromScRgb(0.25f, 1f, 0.0f, 0.0f));
      SnapLineAdorner.MarginBrush.Freeze();
    }

    public SnapLineAdorner(SnapLineAdornerSet adornerSet)
      : base((AdornerSet) adornerSet)
    {
    }

    public void ReplaceSnapLines(SceneElement targetElement, Rect targetBounds, List<SnapLine> snapLines)
    {
      this.targetElement = targetElement;
      this.targetBounds = targetBounds;
      this.snapLines = snapLines;
      this.InvalidateRender();
      this.AdornerSet.Update();
    }

    public void UpdateTargetBounds(Rect targetBounds)
    {
      this.targetBounds = targetBounds;
      this.InvalidateRender();
      this.AdornerSet.Update();
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (this.snapLines == null)
        return;
      FrameworkElement element = this.targetElement == null || !this.targetElement.IsViewObjectValid ? (FrameworkElement) null : this.targetElement.ViewObject.PlatformSpecificObject as FrameworkElement;
      if (element != null)
        this.targetBounds = ElementUtilities.GetActualBoundsInParent(element);
      foreach (SnapLine snapLine in this.snapLines)
      {
        if (this.BoundsMatchSnapLine(this.targetBounds, snapLine))
        {
          Point p1 = snapLine.P1;
          Point p2 = snapLine.P2;
          if (snapLine.IsContainerLine || snapLine.Location != snapLine.LocationRelativeToTarget)
          {
            Point cornerPoint1;
            Point cornerPoint2;
            this.GetTargetCorners(this.targetBounds, snapLine.Orientation, snapLine.LocationRelativeToTarget, out cornerPoint1, out cornerPoint2);
            Rect rect = new Rect(p1, p2);
            rect.Union(cornerPoint1);
            rect.Union(cornerPoint2);
            if (Math.Abs(snapLine.OffsetRelativeToTarget) < 0.001)
            {
              p1 = rect.TopLeft * matrix;
              Point point1 = rect.BottomRight * matrix;
              drawingContext.DrawLine(SnapLineAdorner.SolidSnapLinePen, p1, point1);
              drawingContext.DrawLine(SnapLineAdorner.DottedSnapLinePen, p1, point1);
            }
            else
            {
              StreamGeometry streamGeometry = new StreamGeometry();
              StreamGeometryContext streamGeometryContext = streamGeometry.Open();
              streamGeometryContext.BeginFigure(rect.TopLeft * matrix, true, true);
              streamGeometryContext.PolyLineTo((IList<Point>) new Point[3]
              {
                rect.TopRight * matrix,
                rect.BottomRight * matrix,
                rect.BottomLeft * matrix
              }, 0 != 0, 0 != 0);
              streamGeometryContext.Close();
              streamGeometry.Freeze();
              drawingContext.DrawGeometry(SnapLineAdorner.MarginBrush, (Pen)null, (System.Windows.Media.Geometry)streamGeometry);
            }
          }
          else
          {
            this.ExtendLine(this.targetBounds, snapLine.Orientation, ref p1, ref p2);
            p1 *= matrix;
            Point point1 = p2 * matrix;
            Vector vector = (snapLine.Orientation == SnapLineOrientation.Horizontal ? new Vector(0.0, 1.0) : new Vector(1.0, 0.0)) * matrix;
            if (vector.X != 0.0 || vector.Y != 0.0)
            {
              vector.Normalize();
              double num = (snapLine.LocationRelativeToTarget == SnapLineLocation.Minimum ? -1.0 : 1.0) * SnapLineAdorner.SolidSnapLinePen.Thickness / 2.0;
              p1 += num * vector;
              point1 += num * vector;
            }
            drawingContext.DrawLine(SnapLineAdorner.SolidSnapLinePen, p1, point1);
            drawingContext.DrawLine(SnapLineAdorner.DottedSnapLinePen, p1, point1);
          }
        }
      }
    }

    private bool BoundsMatchSnapLine(Rect bounds, SnapLine snapLine)
    {
      if (snapLine.Location == SnapLineLocation.Baseline)
        return true;
      Point cornerPoint1;
      Point cornerPoint2;
      this.GetTargetCorners(bounds, snapLine.Orientation, snapLine.LocationRelativeToTarget, out cornerPoint1, out cornerPoint2);
      double signedDistance1 = snapLine.GetSignedDistance(cornerPoint1);
      double signedDistance2 = snapLine.GetSignedDistance(cornerPoint2);
      if (Math.Abs(signedDistance1 + snapLine.OffsetRelativeToTarget) < 0.01)
        return Math.Abs(signedDistance2 + snapLine.OffsetRelativeToTarget) < 0.01;
      return false;
    }

    private void GetTargetCorners(Rect bounds, SnapLineOrientation orientation, SnapLineLocation location, out Point cornerPoint1, out Point cornerPoint2)
    {
      if (orientation == SnapLineOrientation.Horizontal)
      {
        if (location == SnapLineLocation.Minimum)
        {
          cornerPoint1 = bounds.TopLeft;
          cornerPoint2 = bounds.TopRight;
        }
        else
        {
          cornerPoint1 = bounds.BottomLeft;
          cornerPoint2 = bounds.BottomRight;
        }
      }
      else if (location == SnapLineLocation.Minimum)
      {
        cornerPoint1 = bounds.TopLeft;
        cornerPoint2 = bounds.BottomLeft;
      }
      else
      {
        cornerPoint1 = bounds.TopRight;
        cornerPoint2 = bounds.BottomRight;
      }
    }

    private void ExtendLine(Rect bounds, SnapLineOrientation orientation, ref Point p1, ref Point p2)
    {
      if (orientation == SnapLineOrientation.Horizontal)
      {
        p1.X = Math.Min(p1.X, bounds.Left);
        p2.X = Math.Max(p2.X, bounds.Right);
      }
      else
      {
        p1.Y = Math.Min(p1.Y, bounds.Top);
        p2.Y = Math.Max(p2.Y, bounds.Bottom);
      }
    }
  }
}
