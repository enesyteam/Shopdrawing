// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathFigureEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class PathFigureEditor
  {
    private PathFigure figure;
    private PathDiffChangeList changeList;
    private int figureIndex;

    public PathFigure PathFigure
    {
      get
      {
        return this.figure;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.figure = value;
      }
    }

    private PathSegmentCollection PathSegments
    {
      get
      {
        return this.PathFigure.Segments;
      }
      set
      {
        this.PathFigure.Segments = value;
      }
    }

    public PathFigureEditor(PathFigure figure)
      : this(figure, new PathDiffChangeList(), 0)
    {
    }

    public PathFigureEditor(PathFigure figure, PathDiffChangeList changeList, int figureIndex)
    {
      this.PathFigure = figure;
      this.changeList = changeList;
      this.figureIndex = figureIndex;
    }

    public bool IsIndexValid(int index)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        return true;
      if (index >= 0)
        return index < PathFigureUtilities.PointCount(this.PathFigure);
      return false;
    }

    public Point GetPoint(int index)
    {
      this.ValidateIndex(ref index);
      return PathFigureUtilities.GetPoint(this.PathFigure, index);
    }

    public Point GetPoint(int segmentIndex, int segmentPointIndex)
    {
      this.ValidateIndices(ref segmentIndex, ref segmentPointIndex);
      if (segmentIndex == -1)
        return this.PathFigure.StartPoint;
      return PathSegmentUtilities.GetPoint(this.PathSegments[segmentIndex], segmentPointIndex);
    }

    public void SetPoint(int index, Point point)
    {
      this.ValidateIndex(ref index);
      int pointIndex = PathFigureUtilities.PointCount(this.PathFigure);
      if (index == 0 && PathFigureUtilities.IsClosed(this.PathFigure) && PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure))
        PathFigureUtilities.SetPoint(this.PathFigure, pointIndex, point);
      PathFigureUtilities.SetPoint(this.PathFigure, index, point);
    }

    public Point EvaluateSegment(int index, double segmentParameter)
    {
      index = this.GetLastIndexOfUpstreamSegment(index);
      this.ValidateIndex(ref index);
      switch (this.GetPointKind(index))
      {
        case PathPointKind.Line:
          return this.GetPointOnLineSegment(index, segmentParameter);
        case PathPointKind.Quadratic:
          return this.GetPointOnQuadraticSegment(index, segmentParameter);
        case PathPointKind.Cubic:
          return this.GetPointOnCubicSegment(index, segmentParameter);
        default:
          throw new InvalidOperationException(ExceptionStringTable.NotAValidPathPointKindForEndOfASegment);
      }
    }

    public Vector GetTangent(int index, double segmentParameter)
    {
      index = this.GetLastIndexOfUpstreamSegment(index);
      this.ValidateIndex(ref index);
      switch (this.GetPointKind(index))
      {
        case PathPointKind.Line:
          return this.GetTangentToLineSegment(index, segmentParameter);
        case PathPointKind.Quadratic:
          return this.GetTangentToQuadraticSegment(index, segmentParameter);
        case PathPointKind.Cubic:
          return this.GetTangentToCubicSegment(index, segmentParameter);
        default:
          throw new InvalidOperationException(ExceptionStringTable.NotAValidPathPointKindForEndOfASegment);
      }
    }

    public PathPointKind GetPointKind(int index)
    {
      this.ValidateIndex(ref index);
      return this.GetPointKindFromValidatedIndex(index);
    }

    public PathPointKind GetPointKind(int segmentIndex, int segmentPointIndex)
    {
      this.ValidateIndices(ref segmentIndex, ref segmentPointIndex);
      return this.GetPointKindFromValidatedIndices(segmentIndex, segmentPointIndex);
    }

    public bool IsNode(int index)
    {
      return this.GetPointKind(index) != PathPointKind.BezierHandle;
    }

    public bool IsFirstCubicBezierHandle(int index)
    {
      if (this.GetPointKind(index) == PathPointKind.BezierHandle && this.IsIndexValid(index + 2))
        return this.GetPointKind(index + 2) == PathPointKind.Cubic;
      return false;
    }

    public bool IsSecondCubicBezierHandle(int index)
    {
      if (this.GetPointKind(index) == PathPointKind.BezierHandle && this.IsIndexValid(index + 1))
        return this.GetPointKind(index + 1) == PathPointKind.Cubic;
      return false;
    }

    public bool HasUpstreamBezierHandleNeighbor(int index)
    {
      if (this.IsNode(index) && this.IsIndexValid(index - 1))
        return this.GetPointKind(index - 1) == PathPointKind.BezierHandle;
      return false;
    }

    public bool HasDownstreamBezierHandleNeighbor(int index)
    {
      if (this.IsNode(index) && this.IsIndexValid(index + 1))
        return this.GetPointKind(index + 1) == PathPointKind.BezierHandle;
      return false;
    }

    public int GetFirstIndexOfUpstreamSegment(int index)
    {
      this.ValidateIndex(ref index);
      while (index > 0 || PathFigureUtilities.IsClosed(this.PathFigure))
      {
        --index;
        if (this.IsNode(index))
          break;
      }
      return index;
    }

    public int GetLastIndexOfUpstreamSegment(int index)
    {
      this.ValidateIndex(ref index);
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      bool flag = PathFigureUtilities.IsClosed(this.PathFigure);
      while ((index < num - 1 || flag) && !this.IsNode(index))
        ++index;
      return index;
    }

    public int GetFirstIndexOfDownstreamSegment(int index)
    {
      this.ValidateIndex(ref index);
      bool flag = PathFigureUtilities.IsClosed(this.PathFigure);
      while ((index > 0 || flag) && !this.IsNode(index))
        --index;
      return index;
    }

    public int GetLastIndexOfDownstreamSegment(int index)
    {
      this.ValidateIndex(ref index);
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      bool flag = PathFigureUtilities.IsClosed(this.PathFigure);
      while (index < num - 1 || flag)
      {
        ++index;
        if (this.IsNode(index))
          break;
      }
      return index;
    }

    public double GetClosestPointOfUpstreamSegment(int index, Point position, Matrix matrix, double flatteningTolerance, out Point closestPoint, out double distanceSquared)
    {
      if (flatteningTolerance <= 0.0)
        throw new ArgumentException("Flattening tolerance must be strictly greater than zero.", "flatteningTolerance");
      index = this.GetLastIndexOfUpstreamSegment(index);
      switch (this.GetPointKind(index))
      {
        case PathPointKind.Start:
          closestPoint = this.GetPoint(0);
          distanceSquared = (closestPoint * matrix - position).LengthSquared;
          return 0.0;
        case PathPointKind.Arc:
          closestPoint = this.GetPoint(index);
          distanceSquared = (closestPoint * matrix - position).LengthSquared;
          return 1.0;
        case PathPointKind.Line:
          return this.GetClosestPointOnLineSegment(index, position, matrix, out closestPoint, out distanceSquared);
        case PathPointKind.Quadratic:
          return this.GetClosestPointOnQuadraticSegment(index, position, matrix, flatteningTolerance, out closestPoint, out distanceSquared);
        case PathPointKind.Cubic:
          return this.GetClosestPointOnCubicSegment(index, position, matrix, flatteningTolerance, out closestPoint, out distanceSquared);
        default:
          throw new InvalidOperationException(ExceptionStringTable.NotAValidPathPointKindForEndOfASegment);
      }
    }

    public List<Point> Flatten(double tolerance)
    {
      List<Point> resultPolyline = new List<Point>(PathFigureUtilities.PointCount(this.PathFigure));
      resultPolyline.Add(PathFigureUtilities.FirstPoint(this.PathFigure));
      if (PathFigureUtilities.IsIsolatedPoint(this.PathFigure))
        return resultPolyline;
      Point[] controlPoints = new Point[4];
      Point b = PathFigureUtilities.FirstPoint(this.PathFigure);
      for (int index = 0; index < this.PathSegments.Count; ++index)
      {
        PathSegment pathSegment = this.PathSegments[index];
        LineSegment lineSegment = pathSegment as LineSegment;
        if (lineSegment != null)
        {
          resultPolyline.Add(lineSegment.Point);
          b = lineSegment.Point;
        }
        else
        {
          QuadraticBezierSegment quadraticBezierSegment = pathSegment as QuadraticBezierSegment;
          if (quadraticBezierSegment != null)
          {
            controlPoints[0] = b;
            controlPoints[1] = quadraticBezierSegment.Point1;
            controlPoints[2] = quadraticBezierSegment.Point2;
            BezierCurveFlattener.FlattenQuadratic(controlPoints, tolerance, resultPolyline, true);
            b = quadraticBezierSegment.Point2;
          }
          else
          {
            BezierSegment bezierSegment = pathSegment as BezierSegment;
            if (bezierSegment != null)
            {
              controlPoints[0] = b;
              controlPoints[1] = bezierSegment.Point1;
              controlPoints[2] = bezierSegment.Point2;
              controlPoints[3] = bezierSegment.Point3;
              BezierCurveFlattener.FlattenCubic(controlPoints, tolerance, resultPolyline, true);
              b = bezierSegment.Point3;
            }
            else if (this.PathFigure.IsClosed)
            {
              Point a = PathFigureUtilities.FirstPoint(this.PathFigure);
              if (!VectorUtilities.ArePathPointsVeryClose(a, b))
                resultPolyline.Add(a);
            }
          }
        }
      }
      return resultPolyline;
    }

    public void ApplyTransform(Matrix matrix)
    {
      if (matrix.IsIdentity)
        return;
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      for (int index = 0; index < num; ++index)
      {
        Point point1 = this.GetPoint(index);
        Point point2 = matrix.Transform(point1);
        this.SetPoint(index, point2);
      }
    }

    public void CloseWithLineSegment()
    {
      if (!this.CanCloseFigure())
        return;
      this.LineToAndCloseFigure();
      if (!PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure) || !(this.PathSegments[this.PathSegments.Count - 1] is LineSegment))
        return;
      this.PathSegments.RemoveAt(this.PathSegments.Count - 1);
      this.changeList.Changes[this.changeList.Changes.Count - 1].PathStructureChanges.Add(new PathStructureChange(this.PathSegments.Count, PathStructureChange.DeletedPointIndex));
    }

    public void CloseWithCubicBezier()
    {
      if (!this.CanCloseFigure())
        return;
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      Point point = this.GetPoint(num - 1);
      Point b = PathFigureUtilities.FirstPoint(this.PathFigure);
      if (VectorUtilities.ArePathPointsVeryClose(point, b) && num > 2)
      {
        this.figure.IsClosed = true;
        this.AddCloseChange(false);
      }
      else
        this.CubicCurveToAndCloseFigure(point + (b - point) / 3.0, point + 2.0 * (b - point) / 3.0);
    }

    public Point Evaluate(int segmentIndex, int lastPointIndex, double t)
    {
      Point point1 = new Point();
      switch (this.GetPointKind(segmentIndex, lastPointIndex))
      {
        case PathPointKind.Arc:
          return this.GetPoint(segmentIndex, lastPointIndex);
        case PathPointKind.Line:
          return VectorUtilities.WeightedAverage(this.GetPoint(segmentIndex, lastPointIndex - 1), this.GetPoint(segmentIndex, lastPointIndex), t);
        case PathPointKind.Quadratic:
          Point point2 = this.GetPoint(segmentIndex, lastPointIndex - 2);
          Point point3 = this.GetPoint(segmentIndex, lastPointIndex - 1);
          Point point4 = this.GetPoint(segmentIndex, lastPointIndex);
          return VectorUtilities.WeightedAverage(VectorUtilities.WeightedAverage(point2, point3, t), VectorUtilities.WeightedAverage(point3, point4, t), t);
        case PathPointKind.Cubic:
          Point point5 = this.GetPoint(segmentIndex, lastPointIndex - 3);
          Point point6 = this.GetPoint(segmentIndex, lastPointIndex - 2);
          Point point7 = this.GetPoint(segmentIndex, lastPointIndex - 1);
          Point point8 = this.GetPoint(segmentIndex, lastPointIndex);
          if (VectorUtilities.ArePathPointsVeryClose(point5, point6) && VectorUtilities.ArePathPointsVeryClose(point7, point8))
            return VectorUtilities.WeightedAverage(point5, point8, t);
          Point point9 = VectorUtilities.WeightedAverage(point6, point7, t);
          Point a = VectorUtilities.WeightedAverage(point5, point6, t);
          Point b = VectorUtilities.WeightedAverage(point7, point8, t);
          return VectorUtilities.WeightedAverage(VectorUtilities.WeightedAverage(a, point9, t), VectorUtilities.WeightedAverage(point9, b, t), t);
        default:
          throw new NotImplementedException(ExceptionStringTable.PathSegmentAdornerUnknownPathPoint);
      }
    }

    internal void MovePoint(int index, Point point)
    {
      this.ValidateIndex(ref index);
      if (this.GetPointKindFromValidatedIndex(index) == PathPointKind.BezierHandle)
        throw new ArgumentException(ExceptionStringTable.GivenIndexMustNotReferToABezierTangentHandle);
      bool flag1 = PathFigureUtilities.IsClosed(this.PathFigure) && PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure);
      Point point1 = this.GetPoint(index);
      Vector vector = point - point1;
      PathFigureUtilities.SetPoint(this.PathFigure, index, point);
      bool flag2 = false;
      bool flag3 = false;
      int index1 = 0;
      BezierSegment bezierSegment1 = (BezierSegment) null;
      if (index > 0)
      {
        int segmentIndex;
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, index, out segmentIndex, out segmentPointIndex);
        PathSegment segment = this.figure.Segments[segmentIndex];
        bezierSegment1 = segment as BezierSegment;
        if (bezierSegment1 != null && segmentPointIndex == 2)
          flag2 = true;
        if (segmentPointIndex + 1 == PathSegmentUtilities.GetPointCount(segment) && segmentIndex + 1 < this.figure.Segments.Count)
        {
          flag3 = true;
          index1 = segmentIndex + 1;
        }
      }
      else
      {
        if (this.figure.Segments.Count > 0)
        {
          flag3 = true;
          index1 = 0;
        }
        if (flag1)
        {
          PathSegment segment = this.PathSegments[this.PathSegments.Count - 1];
          bezierSegment1 = segment as BezierSegment;
          if (bezierSegment1 != null)
          {
            bezierSegment1.Point3 += vector;
            flag2 = true;
          }
          else
            PathSegmentUtilities.SetLastPoint(segment, point);
        }
      }
      if (flag2)
        bezierSegment1.Point2 += vector;
      if (!flag3)
        return;
      BezierSegment bezierSegment2 = this.figure.Segments[index1] as BezierSegment;
      if (bezierSegment2 == null)
        return;
      bezierSegment2.Point1 += vector;
    }

    internal void MoveTangent(int index, Point point, bool enforceSmoothness)
    {
      this.ValidateIndex(ref index);
      if (this.GetPointKindFromValidatedIndex(index) != PathPointKind.BezierHandle)
        throw new ArgumentException(ExceptionStringTable.GivenIndexDoesNotReferToABezierTangentHandle);
      PathFigureUtilities.SetPoint(this.PathFigure, index, point);
      if (!enforceSmoothness)
        return;
      if (this.IsFirstCubicBezierHandle(index) && this.GetPointKind(index - 1) == PathPointKind.Cubic)
      {
        Point point1 = this.GetPoint(index - 1);
        Point point2 = this.GetPoint(index - 2);
        this.ConstrainTangentPoint(point, point1, ref point2);
        this.SetPoint(index - 2, point2);
      }
      if (!this.IsSecondCubicBezierHandle(index) || !this.IsIndexValid(index + 4) || this.GetPointKind(index + 4) != PathPointKind.Cubic)
        return;
      Point point3 = this.GetPoint(index + 1);
      Point point4 = this.GetPoint(index + 2);
      this.ConstrainTangentPoint(point, point3, ref point4);
      this.SetPoint(index + 2, point4);
    }

    internal void Clear()
    {
      this.figure.ClearValue(PathFigure.IsClosedProperty);
      this.figure.ClearValue(PathFigure.StartPointProperty);
      this.figure.Segments.Clear();
    }

    internal void MoveTo(Point point)
    {
      if (!PathFigureUtilities.IsOpen(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallMoveToWhenFigureIsNotOpen);
      if (PathFigureUtilities.SegmentCount(this.PathFigure) != 0)
        throw new InvalidOperationException(ExceptionStringTable.CannotCallMoveToWhenFigureAlreadyHasSegments);
      this.figure.StartPoint = point;
    }

    internal void LineTo(Point point)
    {
      if (!PathFigureUtilities.IsOpen(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallLineToWhenFigureIsNotOpen);
      this.figure.Segments.Add((PathSegment) PathSegmentUtilities.CreateLineSegment(point, true));
      PathAppendLineAction appendLineAction = new PathAppendLineAction();
      appendLineAction.Point = point;
      appendLineAction.Figure = this.figureIndex;
      appendLineAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) appendLineAction);
    }

    internal void LineToAndCloseFigure()
    {
      if (!PathFigureUtilities.IsOpen(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallLineToAndCloseFigureWhenFigureIsNotOpen);
      this.figure.IsClosed = true;
      this.AddCloseChange(false);
    }

    internal void QuadraticCurveTo(Point p1, Point p2)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallQuadraticCurveToAndCloseFigureWhenFigureIsClosed);
      this.figure.Segments.Add((PathSegment) PathSegmentUtilities.CreateQuadraticBezierSegment(p1, p2, true));
      PathAppendQuadraticBezierAction quadraticBezierAction = new PathAppendQuadraticBezierAction();
      quadraticBezierAction.Point1 = p1;
      quadraticBezierAction.Point2 = p2;
      quadraticBezierAction.Figure = this.figureIndex;
      quadraticBezierAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) quadraticBezierAction);
    }

    internal void QuadraticCurveToAndCloseFigure(Point p1)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallQuadraticCurveToAndCloseFigureWhenFigureIsClosed);
      QuadraticBezierSegment quadraticBezierSegment = PathSegmentUtilities.CreateQuadraticBezierSegment(p1, this.GetPoint(0), true);
      this.figure.Segments.Add((PathSegment) quadraticBezierSegment);
      PathAppendQuadraticBezierAction quadraticBezierAction = new PathAppendQuadraticBezierAction();
      quadraticBezierAction.Point1 = p1;
      quadraticBezierAction.Point2 = quadraticBezierSegment.Point2;
      quadraticBezierAction.Figure = this.figureIndex;
      quadraticBezierAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) quadraticBezierAction);
      this.CloseWithLineSegment();
    }

    internal void CubicCurveTo(Point p1, Point p2, Point p3)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallCubicCurveToWhenFigureIsClosed);
      this.figure.Segments.Add((PathSegment) PathSegmentUtilities.CreateBezierSegment(p1, p2, p3, true));
      PathAppendBezierAction appendBezierAction = new PathAppendBezierAction();
      appendBezierAction.Point1 = p1;
      appendBezierAction.Point2 = p2;
      appendBezierAction.Point3 = p3;
      appendBezierAction.Figure = this.figureIndex;
      appendBezierAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) appendBezierAction);
    }

    internal void LinearCubicCurveTo(Point p3)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallLinearCubicCurveToWhenFigureIsClosed);
      Point point = this.GetPoint(PathFigureUtilities.PointCount(this.PathFigure) - 1);
      Point point1 = point + (p3 - point) / 3.0;
      Point point2 = point + 2.0 * (p3 - point) / 3.0;
      this.figure.Segments.Add((PathSegment) PathSegmentUtilities.CreateBezierSegment(point1, point2, p3, true));
      PathAppendBezierAction appendBezierAction = new PathAppendBezierAction();
      appendBezierAction.Point1 = point1;
      appendBezierAction.Point2 = point2;
      appendBezierAction.Point3 = p3;
      appendBezierAction.Figure = this.figureIndex;
      appendBezierAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) appendBezierAction);
    }

    internal void CubicCurveToAndCloseFigure(Point p1, Point p2)
    {
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.CannotCallCubicCurveToAndCloseFigureWhenFigureIsClosed);
      BezierSegment bezierSegment = PathSegmentUtilities.CreateBezierSegment(p1, p2, this.GetPoint(0), true);
      this.figure.Segments.Add((PathSegment) bezierSegment);
      PathAppendBezierAction appendBezierAction = new PathAppendBezierAction();
      appendBezierAction.Point1 = p1;
      appendBezierAction.Point2 = p2;
      appendBezierAction.Point3 = bezierSegment.Point3;
      appendBezierAction.Figure = this.figureIndex;
      appendBezierAction.Segment = this.figure.Segments.Count - 1;
      this.changeList.Changes.Add((PathAction) appendBezierAction);
      this.CloseWithLineSegment();
    }

    internal int Rotate()
    {
      PathAction pathAction = new PathAction();
      pathAction.Action = PathActionType.Rotate;
      pathAction.Figure = this.figureIndex;
      int count = this.PathSegments.Count;
      bool flag1 = PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure);
      bool flag2 = this.PathSegments[0] is LineSegment;
      if (!flag1)
        this.PathSegments.Add((PathSegment) PathSegmentUtilities.CreateLineSegment(this.PathFigure.StartPoint, true));
      PathSegment segment = this.PathSegments[0];
      this.PathFigure.StartPoint = PathSegmentUtilities.GetLastPoint(segment);
      this.PathSegments.RemoveAt(0);
      int num = PathFigureUtilities.PointCount(this.PathFigure) - 1;
      if (!flag2)
        this.PathSegments.Add(segment);
      if (flag2 && flag1)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.DeletedPointIndex));
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, PathStructureChange.StartPointIndex));
      }
      else if (flag2 && !flag1)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, this.PathSegments.Count - 1));
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, PathStructureChange.StartPointIndex));
      }
      else if (!flag2 && flag1)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, this.PathSegments.Count - 1));
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, PathStructureChange.StartPointIndex));
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.DeletedPointIndex));
      }
      else if (!flag2 && !flag1)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, this.PathSegments.Count - 1));
        pathAction.PathStructureChanges.Add(new PathStructureChange(0, PathStructureChange.StartPointIndex));
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, this.PathSegments.Count - 2));
      }
      for (int oldIndex = 1; oldIndex < count; ++oldIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, (oldIndex - 1 + this.PathSegments.Count) % this.PathSegments.Count));
      this.changeList.Changes.Add(pathAction);
      return num;
    }

    internal int RemovePoint(int pointIndex)
    {
      this.ValidateIndex(ref pointIndex);
      if (!this.IsNode(pointIndex))
        throw new ArgumentException(ExceptionStringTable.CanOnlyRemoveNodePointsFromAFigure, "pointIndex");
      if (PathFigureUtilities.IsOpen(this.PathFigure))
      {
        if (pointIndex == PathFigureUtilities.PointCount(this.PathFigure) - 1)
          return this.RemoveLastSegment();
        if (pointIndex == 0)
          return this.RemoveFirstSegment();
      }
      if (PathFigureUtilities.IsClosed(this.PathFigure))
      {
        if (PathFigureUtilities.IsIsolatedPoint(this.PathFigure))
          return 0;
        if (pointIndex == 0)
        {
          if (PathFigureUtilities.SegmentCount(this.PathFigure) == 1 && PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure))
            return this.RemoveFirstSegment();
          return this.RemovePoint(this.Rotate());
        }
      }
      int count = this.PathSegments.Count;
      int segmentIndex;
      int segmentPointIndex;
      PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, pointIndex, out segmentIndex, out segmentPointIndex);
      PathAction pathAction = new PathAction();
      pathAction.Figure = this.figureIndex;
      pathAction.Segment = segmentIndex;
      pathAction.Action = PathActionType.DeletePoint;
      pathAction.PointIndex = pointIndex;
      PathSegment segment1 = this.PathSegments[segmentIndex];
      PathSegment segment2 = (PathSegment) null;
      if (segmentIndex + 1 < this.PathSegments.Count)
        segment2 = this.PathSegments[segmentIndex + 1];
      if (segment1 is QuadraticBezierSegment || segment2 is QuadraticBezierSegment || (segment1 is ArcSegment || segment2 is ArcSegment))
        return 0;
      int num1 = 0;
      int num2 = 1;
      this.PathSegments.RemoveAt(segmentIndex);
      int num3 = num1 + PathSegmentUtilities.GetPointCount(segment1);
      pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, PathStructureChange.DeletedPointIndex));
      bool flag = false;
      if (segment2 != null)
      {
        this.PathSegments.RemoveAt(segmentIndex);
        ++num2;
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, PathStructureChange.DeletedPointIndex));
        num3 += PathSegmentUtilities.GetPointCount(segment2);
      }
      else
      {
        segment2 = (PathSegment) PathSegmentUtilities.CreateLineSegment(this.PathFigure.StartPoint, true);
        flag = true;
      }
      LineSegment lineSegment1 = segment1 as LineSegment;
      LineSegment lineSegment2 = segment2 as LineSegment;
      BezierSegment bezierSegment1 = segment1 as BezierSegment;
      BezierSegment bezierSegment2 = segment2 as BezierSegment;
      PathSegment segment3 = (PathSegment) null;
      if (bezierSegment1 != null && bezierSegment2 != null)
      {
        segment3 = (PathSegment) PathSegmentUtilities.CreateBezierSegment(bezierSegment1.Point1, bezierSegment2.Point2, bezierSegment2.Point3, bezierSegment2.IsStroked);
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, BezierSegment.Point1Property));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, BezierSegment.Point2Property));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, BezierSegment.Point3Property));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, PathStructureChange.DeletedPointIndex));
      }
      else if (bezierSegment1 != null && lineSegment2 != null)
      {
        segment3 = (PathSegment) PathSegmentUtilities.CreateBezierSegment(bezierSegment1.Point1, lineSegment2.Point, lineSegment2.Point, lineSegment2.IsStroked);
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, BezierSegment.Point1Property));
        if (flag)
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.Copy));
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.Copy));
        }
        else
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, LineSegment.PointProperty, BezierSegment.Point2Property));
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, LineSegment.PointProperty, BezierSegment.Point3Property));
        }
      }
      else if (lineSegment1 != null && bezierSegment2 != null)
      {
        Point point1;
        bool isStroked;
        if (segmentIndex > 0)
        {
          PathSegment segment4 = this.PathSegments[segmentIndex - 1];
          point1 = PathSegmentUtilities.GetLastPoint(segment4);
          isStroked = segment4.IsStroked;
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex - 1, segmentIndex, LineSegment.PointProperty, BezierSegment.Point1Property, PathChangeType.Copy));
        }
        else
        {
          point1 = this.PathFigure.StartPoint;
          isStroked = true;
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.Copy));
        }
        segment3 = (PathSegment) PathSegmentUtilities.CreateBezierSegment(point1, bezierSegment2.Point2, bezierSegment2.Point3, isStroked);
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, BezierSegment.Point2Property));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex, BezierSegment.Point3Property));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, PathStructureChange.DeletedPointIndex));
      }
      else if (lineSegment1 != null && lineSegment2 != null && !flag)
      {
        segment3 = (PathSegment) lineSegment2;
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex + 1, segmentIndex));
      }
      if (segment3 != null)
      {
        --num2;
        this.PathSegments.Insert(segmentIndex, segment3);
        num3 -= PathSegmentUtilities.GetPointCount(segment3);
      }
      if (num2 > 0)
      {
        for (int newIndex = segmentIndex + 1; newIndex + num2 < count; ++newIndex)
          pathAction.PathStructureChanges.Add(new PathStructureChange(newIndex + num2, newIndex));
      }
      this.changeList.Changes.Add(pathAction);
      return num3;
    }

    internal int RemoveFirstSegment()
    {
      if (PathFigureUtilities.IsIsolatedPoint(this.PathFigure))
        return 0;
      int num1 = 0;
      PathAction pathAction = new PathAction();
      if (PathFigureUtilities.IsClosed(this.PathFigure))
      {
        Point startPoint = this.PathFigure.StartPoint;
        this.PathFigure.IsClosed = false;
        Point lastPoint = PathSegmentUtilities.GetLastPoint(this.PathSegments[this.PathSegments.Count - 1]);
        if (!VectorUtilities.ArePathPointsVeryClose(startPoint, lastPoint))
        {
          this.PathSegments.Add((PathSegment) PathSegmentUtilities.CreateLineSegment(startPoint, true));
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, this.PathSegments.Count - 1, PathChangeType.Copy));
        }
        else
        {
          int num2 = num1 + 1;
        }
      }
      this.PathFigure.StartPoint = PathSegmentUtilities.GetLastPoint(this.PathSegments[0]);
      int pointCount = PathSegmentUtilities.GetPointCount(this.PathSegments[0]);
      this.PathSegments.RemoveAt(0);
      pathAction.Segment = 0;
      pathAction.Action = PathActionType.DeleteSegment;
      pathAction.Figure = this.figureIndex;
      pathAction.PathStructureChanges.Add(new PathStructureChange(0, PathStructureChange.StartPointIndex));
      pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.DeletedPointIndex));
      for (int newIndex = 0; newIndex < this.PathSegments.Count; ++newIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(newIndex + 1, newIndex));
      this.changeList.Changes.Add(pathAction);
      return pointCount;
    }

    internal int RemoveLastSegment()
    {
      if (PathFigureUtilities.IsIsolatedPoint(this.PathFigure))
        return 0;
      int index = this.PathSegments.Count - 1;
      int num;
      if (PathFigureUtilities.IsOpen(this.PathFigure))
      {
        num = PathSegmentUtilities.GetPointCount(this.PathSegments[index]);
        this.PathSegments.RemoveAt(index);
        this.changeList.Changes.Add(new PathAction()
        {
          Segment = index,
          Action = PathActionType.DeleteSegment,
          Figure = this.figureIndex,
          PathStructureChanges = {
            new PathStructureChange(index, PathStructureChange.DeletedPointIndex)
          }
        });
      }
      else
      {
        Point lastPoint = PathSegmentUtilities.GetLastPoint(this.PathSegments[index]);
        this.PathFigure.IsClosed = false;
        num = 0;
        PathAction pathAction = new PathAction();
        pathAction.Segment = index;
        pathAction.Action = PathActionType.DeleteSegment;
        pathAction.Figure = this.figureIndex;
        if (VectorUtilities.ArePathPointsVeryClose(PathFigureUtilities.FirstPoint(this.PathFigure), lastPoint))
        {
          num += PathSegmentUtilities.GetPointCount(this.PathSegments[index]);
          this.PathSegments.RemoveAt(index);
          pathAction.PathStructureChanges.Add(new PathStructureChange(index, PathStructureChange.DeletedPointIndex));
        }
        this.changeList.Changes.Add(pathAction);
      }
      return num;
    }

    internal void Open(int pointIndex)
    {
      this.ValidateIndex(ref pointIndex);
      Point startPoint = this.PathFigure.StartPoint;
      Point lastPoint = PathSegmentUtilities.GetLastPoint(this.PathSegments[this.PathSegments.Count - 1]);
      int count1 = this.PathSegments.Count;
      this.PathFigure.IsClosed = false;
      PathAction pathAction = new PathAction();
      pathAction.PointIndex = pointIndex;
      pathAction.Figure = this.figureIndex;
      pathAction.Action = PathActionType.Open;
      if (!VectorUtilities.ArePathPointsVeryClose(lastPoint, startPoint))
        this.PathSegments.Add((PathSegment) PathSegmentUtilities.CreateLineSegment(startPoint, true));
      if (pointIndex != 0)
      {
        int segmentIndex;
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, pointIndex, out segmentIndex, out segmentPointIndex);
        PathSegmentCollection segmentCollection = new PathSegmentCollection();
        int count2 = this.PathSegments.Count;
        this.PathFigure.StartPoint = PathFigureUtilities.GetPoint(this.PathFigure, pointIndex);
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, PathStructureChange.StartPointIndex));
        for (int oldIndex = segmentIndex + 1; oldIndex < count2; ++oldIndex)
        {
          if (oldIndex < count1)
            pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, segmentCollection.Count));
          else
            pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentCollection.Count));
          segmentCollection.Add(this.PathSegments[oldIndex]);
        }
        for (int oldIndex = 0; oldIndex <= segmentIndex; ++oldIndex)
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, segmentCollection.Count));
          segmentCollection.Add(this.PathSegments[oldIndex]);
        }
        this.PathSegments = segmentCollection;
      }
      this.changeList.Changes.Add(pathAction);
    }

    internal PathFigure Split(int pointIndex)
    {
      this.ValidateIndex(ref pointIndex);
      PathFigure pathFigure = (PathFigure) null;
      if (PathFigureUtilities.IsClosed(this.PathFigure))
      {
        this.Open(pointIndex);
      }
      else
      {
        pathFigure = new PathFigure();
        int segmentIndex;
        if (pointIndex == 0)
        {
          segmentIndex = -1;
        }
        else
        {
          int segmentPointIndex;
          PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, pointIndex, out segmentIndex, out segmentPointIndex);
        }
        Point point = PathFigureUtilities.GetPoint(this.PathFigure, pointIndex);
        pathFigure.StartPoint = point;
        PathAction pathAction = new PathAction();
        pathAction.Action = PathActionType.Split;
        pathAction.Figure = this.figureIndex;
        pathAction.PointIndex = pointIndex;
        int count = this.PathSegments.Count;
        for (int oldIndex = segmentIndex + 1; oldIndex < count; ++oldIndex)
        {
          PathSegment pathSegment = this.PathSegments[segmentIndex + 1];
          this.PathSegments.RemoveAt(segmentIndex + 1);
          pathFigure.Segments.Add(pathSegment);
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, PathStructureChange.DeletedPointIndex));
        }
        this.changeList.Changes.Add(pathAction);
      }
      return pathFigure;
    }

    internal void Join(PathFigure figure)
    {
      Point point1 = PathFigureUtilities.GetPoint(this.PathFigure, PathFigureUtilities.PointCount(this.PathFigure) - 1);
      Point point2 = PathFigureUtilities.FirstPoint(figure);
      this.CubicCurveTo(point1, point2, point2);
      for (int index = 0; index < figure.Segments.Count; ++index)
        this.PathSegments.Add(figure.Segments[index]);
    }

    internal int PromoteSegment(int pointIndex)
    {
      int count = this.PathSegments.Count;
      PathAction pathAction = new PathAction();
      int segmentIndex;
      if (pointIndex == 0)
      {
        if (this.PathFigure.IsClosed && !PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure))
        {
          this.PathSegments.Add((PathSegment) PathSegmentUtilities.CreateLineSegment(this.PathFigure.StartPoint, true));
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathFigureUtilities.SegmentCount(this.PathFigure) - 1, PathChangeType.Copy));
        }
        segmentIndex = PathFigureUtilities.SegmentCount(this.PathFigure) - 1;
      }
      else
      {
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, pointIndex, out segmentIndex, out segmentPointIndex);
      }
      PathSegment segment = this.PathSegments[segmentIndex];
      int pointCount = PathSegmentUtilities.GetPointCount(segment);
      int num = pointCount;
      if (!(segment is BezierSegment))
      {
        Point[] fromSegmentIndex = this.ExtractCubicControlPointsFromSegmentIndex(segmentIndex);
        BezierSegment bezierSegment = PathSegmentUtilities.CreateBezierSegment(fromSegmentIndex[1], fromSegmentIndex[2], fromSegmentIndex[3], segment.IsStroked);
        this.PathSegments.RemoveAt(segmentIndex);
        this.PathSegments.Insert(segmentIndex, (PathSegment) bezierSegment);
        pathAction.Segment = segmentIndex;
        pathAction.Action = PathActionType.PromoteSegment;
        pathAction.Figure = this.figureIndex;
        pathAction.PointIndex = pointIndex;
        if (segmentIndex < count)
        {
          if (segmentIndex == 0)
            pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.Copy));
          else
            pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex - 1, segmentIndex, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.Copy));
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.Copy));
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.Move));
        }
        this.changeList.Changes.Add(pathAction);
        num = PathSegmentUtilities.GetPointCount((PathSegment) bezierSegment);
      }
      if (pointIndex == 0)
        return 0;
      return pointIndex + (num - pointCount);
    }

    internal int SubdivideSegment(int lastPointIndexOfSegment, double parameter)
    {
      this.ValidateIndex(ref lastPointIndexOfSegment);
      if (!this.IsNode(lastPointIndexOfSegment) || PathFigureUtilities.IsOpen(this.PathFigure) && lastPointIndexOfSegment == 0)
        throw new ArgumentException(ExceptionStringTable.SpecifiedPointIsNotEndOfASegment, "lastPointIndexOfSegment");
      if (parameter <= 0.0 || parameter >= 1.0)
        throw new ArgumentOutOfRangeException("parameter", (object) parameter, "Parameter value must be strictly between 0 and 1.");
      switch (this.GetPointKindFromValidatedIndex(lastPointIndexOfSegment))
      {
        case PathPointKind.Line:
          return this.SubdivideLine(lastPointIndexOfSegment, parameter);
        case PathPointKind.Quadratic:
          return this.SubdivideQuadratic(lastPointIndexOfSegment, parameter);
        case PathPointKind.Cubic:
          return this.SubdivideCubic(lastPointIndexOfSegment, parameter);
        default:
          throw new InvalidOperationException(ExceptionStringTable.OnlyKnowHowToSubdivideLinearQuadraticAndCubicSegments);
      }
    }

    internal void Reverse()
    {
      if (this.PathSegments.Count == 0)
        return;
      PathAction pathAction = new PathAction();
      pathAction.Action = PathActionType.Reverse;
      pathAction.Figure = this.figureIndex;
      PathSegmentCollection segmentCollection = this.PathSegments.Clone();
      if (PathFigureUtilities.IsClosed(this.PathFigure))
        PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure);
      int newIndex = segmentCollection.Count - 1;
      Point point = this.PathFigure.StartPoint;
      int oldIndex1 = PathStructureChange.StartPointIndex;
      DependencyProperty oldPointProperty = PathFigure.StartPointProperty;
      for (int oldIndex2 = 0; oldIndex2 < this.PathSegments.Count; ++oldIndex2)
      {
        PathSegment pathSegment = this.PathSegments[oldIndex2];
        LineSegment lineSegment1 = pathSegment as LineSegment;
        if (lineSegment1 != null)
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, newIndex, oldPointProperty, LineSegment.PointProperty));
          LineSegment lineSegment2 = PathSegmentUtilities.CreateLineSegment(point, lineSegment1.IsStroked);
          segmentCollection[newIndex] = (PathSegment) lineSegment2;
          --newIndex;
          point = lineSegment1.Point;
          oldPointProperty = LineSegment.PointProperty;
          oldIndex1 = oldIndex2;
        }
        QuadraticBezierSegment quadraticBezierSegment1 = pathSegment as QuadraticBezierSegment;
        if (quadraticBezierSegment1 != null)
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, newIndex, QuadraticBezierSegment.Point1Property, QuadraticBezierSegment.Point1Property));
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, newIndex, oldPointProperty, QuadraticBezierSegment.Point2Property));
          QuadraticBezierSegment quadraticBezierSegment2 = PathSegmentUtilities.CreateQuadraticBezierSegment(quadraticBezierSegment1.Point1, point, quadraticBezierSegment1.IsStroked);
          segmentCollection[newIndex] = (PathSegment) quadraticBezierSegment2;
          --newIndex;
          point = quadraticBezierSegment1.Point2;
          oldPointProperty = QuadraticBezierSegment.Point2Property;
          oldIndex1 = oldIndex2;
        }
        BezierSegment bezierSegment1 = pathSegment as BezierSegment;
        if (bezierSegment1 != null)
        {
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, newIndex, BezierSegment.Point1Property, BezierSegment.Point2Property));
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, newIndex, BezierSegment.Point2Property, BezierSegment.Point1Property));
          pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, newIndex, oldPointProperty, BezierSegment.Point3Property));
          BezierSegment bezierSegment2 = PathSegmentUtilities.CreateBezierSegment(bezierSegment1.Point2, bezierSegment1.Point1, point, bezierSegment1.IsStroked);
          segmentCollection[newIndex] = (PathSegment) bezierSegment2;
          --newIndex;
          point = bezierSegment1.Point3;
          oldPointProperty = BezierSegment.Point3Property;
          oldIndex1 = oldIndex2;
        }
      }
      this.PathFigure.StartPoint = point;
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, PathStructureChange.StartPointIndex, oldPointProperty, PathFigure.StartPointProperty));
      LineSegment lineSegment = segmentCollection[segmentCollection.Count - 1] as LineSegment;
      if (this.PathFigure.IsClosed && lineSegment != null && VectorUtilities.ArePathPointsVeryClose(lineSegment.Point, PathFigureUtilities.FirstPoint(this.PathFigure)))
      {
        segmentCollection.RemoveAt(segmentCollection.Count - 1);
        foreach (PathStructureChange pathStructureChange in pathAction.PathStructureChanges)
        {
          if (pathStructureChange.NewSegmentIndex == segmentCollection.Count)
            pathStructureChange.NewSegmentIndex = PathStructureChange.DeletedPointIndex;
        }
      }
      this.PathSegments = segmentCollection;
      this.changeList.Changes.Add(pathAction);
    }

    internal List<PathSelectionContext> HitDetect(System.Windows.Media.Geometry query, int figureIndex, bool includePartialSegmentHits)
    {
      IntersectionDetail intersectionDetail = includePartialSegmentHits ? IntersectionDetail.Intersects : IntersectionDetail.FullyInside;
      List<PathSelectionContext> list = new List<PathSelectionContext>();
      GeometryIntersection geometryIntersection = new GeometryIntersection(query);
      if (PathFigureUtilities.SegmentCount(this.PathFigure) == 0)
        return list;
      bool flag1 = PathFigureUtilities.IsClosed(this.figure);
      bool flag2 = flag1 && PathFigureUtilities.IsCloseSegmentDegenerate(this.figure);
      Point point = this.PathFigure.StartPoint;
      int num = 0;
      if (geometryIntersection.IntersectsPoint(point))
      {
        PathSelectionContext selectionContext = new PathSelectionContext();
        selectionContext.SetHitPoint(figureIndex, 0);
        list.Add(selectionContext);
      }
      for (int index = 0; index < this.PathSegments.Count; ++index)
      {
        bool flag3 = index == this.PathSegments.Count - 1;
        PathSegment pathSegment = this.PathSegments[index];
        num += PathSegmentUtilities.GetPointCount(pathSegment);
        if (geometryIntersection.IntersectsPathSegment(point, pathSegment, intersectionDetail))
        {
          PathSelectionContext selectionContext = new PathSelectionContext();
          if (flag3 && flag1 && flag2)
            num = 0;
          selectionContext.SetHitSegment(figureIndex, num);
          list.Add(selectionContext);
        }
        point = PathSegmentUtilities.GetLastPoint(pathSegment);
        if ((!flag3 || !flag2) && geometryIntersection.IntersectsPoint(point))
        {
          PathSelectionContext selectionContext = new PathSelectionContext();
          selectionContext.SetHitPoint(figureIndex, num);
          list.Add(selectionContext);
        }
      }
      if (flag1 && !flag2 && geometryIntersection.IntersectsPathSegment(point, (PathSegment) new LineSegment(this.figure.StartPoint, true), intersectionDetail))
      {
        PathSelectionContext selectionContext = new PathSelectionContext();
        selectionContext.SetHitSegment(figureIndex, 0);
        list.Add(selectionContext);
      }
      return list;
    }

    private bool CanCloseFigure()
    {
      if (!PathFigureUtilities.IsClosed(this.PathFigure))
        return !PathFigureUtilities.IsIsolatedPoint(this.PathFigure);
      return false;
    }

    private void ValidateIndex(ref int index)
    {
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      if (PathFigureUtilities.IsClosed(this.PathFigure))
      {
        index %= num;
        if (index >= 0)
          return;
        index += num;
      }
      else if (index < 0 || index >= num)
        throw new ArgumentOutOfRangeException("index", (object) index, "Index must be between 0 and PointCount - 1 when accessing points of an open figure.");
    }

    private int GetPointCountHelper(int segmentIndex)
    {
      if (segmentIndex == -1)
        return 1;
      return PathSegmentUtilities.GetPointCount(this.PathSegments[segmentIndex]);
    }

    private void ValidateIndices(ref int segmentIndex, ref int segmentPointIndex)
    {
      if (segmentIndex < -1 || segmentIndex >= this.PathSegments.Count)
        throw new ArgumentOutOfRangeException("segmentIndex", (object) segmentIndex, "segmentIndex must always refer to a valid segment");
      if (PathFigureUtilities.IsClosed(this.PathFigure))
      {
        while (segmentPointIndex < 0)
        {
          --segmentIndex;
          if (segmentIndex == -2)
            segmentIndex = this.PathSegments.Count - 1;
          segmentPointIndex += this.GetPointCountHelper(segmentIndex);
        }
        while (segmentPointIndex >= this.GetPointCountHelper(segmentIndex))
        {
          segmentPointIndex -= this.GetPointCountHelper(segmentIndex);
          ++segmentIndex;
          if (segmentIndex == this.PathSegments.Count)
            segmentIndex = -1;
        }
      }
      else
      {
        while (segmentPointIndex < 0)
        {
          --segmentIndex;
          if (segmentIndex == -2)
          {
            segmentIndex = -1;
            segmentPointIndex = -1;
            break;
          }
          segmentPointIndex += this.GetPointCountHelper(segmentIndex);
        }
        while (segmentPointIndex >= this.GetPointCountHelper(segmentIndex))
        {
          segmentPointIndex -= this.GetPointCountHelper(segmentIndex);
          ++segmentIndex;
          if (segmentIndex == this.PathSegments.Count)
          {
            segmentIndex = -1;
            segmentPointIndex = -1;
            break;
          }
        }
        if (segmentPointIndex < 0 || segmentPointIndex >= this.GetPointCountHelper(segmentIndex))
          throw new ArgumentOutOfRangeException("segmentPointIndex", (object) segmentPointIndex, "segmentPointIndex must be between 0 and PointCount - 1 when accessing points of an open figure.");
      }
    }

    private PathPointKind GetPointKindFromValidatedIndex(int index)
    {
      int num = PathFigureUtilities.PointCount(this.PathFigure);
      if (index == 0)
      {
        if (!PathFigureUtilities.IsClosed(this.PathFigure))
          return PathPointKind.Start;
        if (!PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure))
          return PathPointKind.Line;
        index = num;
      }
      int segmentIndex;
      int segmentPointIndex;
      PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, index, out segmentIndex, out segmentPointIndex);
      return PathSegmentUtilities.GetPointKind(this.PathSegments[segmentIndex], segmentPointIndex);
    }

    private PathPointKind GetPointKindFromValidatedIndices(int segmentIndex, int segmentPointIndex)
    {
      if (segmentIndex == -1)
      {
        if (!PathFigureUtilities.IsClosed(this.PathFigure))
          return PathPointKind.Start;
        if (!PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure))
          return PathPointKind.Line;
        segmentIndex = this.PathSegments.Count - 1;
        segmentPointIndex = PathSegmentUtilities.GetPointCount(this.PathSegments[segmentIndex]) - 1;
      }
      return PathSegmentUtilities.GetPointKind(this.PathSegments[segmentIndex], segmentPointIndex);
    }

    private Point GetPointFromValidatedIndex(int index)
    {
      return this.GetPoint(index);
    }

    private void ConstrainTangentPoint(Point newTangentPoint, Point nodePoint, ref Point otherTangentPoint)
    {
      Vector a = newTangentPoint - nodePoint;
      double num = VectorUtilities.Distance(otherTangentPoint, nodePoint);
      if (VectorUtilities.IsZero(a))
        return;
      Vector vector = a;
      vector.Normalize();
      otherTangentPoint = nodePoint - num * vector;
    }

    private Point GetPointOnCubicSegment(int index, double parameter)
    {
      int index1 = index - 3;
      this.ValidateIndex(ref index1);
      Point fromValidatedIndex1 = this.GetPointFromValidatedIndex(index1);
      Point fromValidatedIndex2 = this.GetPointFromValidatedIndex(index1 + 1);
      Point fromValidatedIndex3 = this.GetPointFromValidatedIndex(index1 + 2);
      Point fromValidatedIndex4 = this.GetPointFromValidatedIndex(index);
      Point a = VectorUtilities.WeightedAverage(fromValidatedIndex1, fromValidatedIndex2, parameter);
      Point point = VectorUtilities.WeightedAverage(fromValidatedIndex2, fromValidatedIndex3, parameter);
      Point b = VectorUtilities.WeightedAverage(fromValidatedIndex3, fromValidatedIndex4, parameter);
      return VectorUtilities.WeightedAverage(VectorUtilities.WeightedAverage(a, point, parameter), VectorUtilities.WeightedAverage(point, b, parameter), parameter);
    }

    private Point GetPointOnQuadraticSegment(int index, double parameter)
    {
      int index1 = index - 2;
      this.ValidateIndex(ref index1);
      Point fromValidatedIndex1 = this.GetPointFromValidatedIndex(index1);
      Point fromValidatedIndex2 = this.GetPointFromValidatedIndex(index1 + 1);
      Point fromValidatedIndex3 = this.GetPointFromValidatedIndex(index);
      return VectorUtilities.WeightedAverage(VectorUtilities.WeightedAverage(fromValidatedIndex1, fromValidatedIndex2, parameter), VectorUtilities.WeightedAverage(fromValidatedIndex2, fromValidatedIndex3, parameter), parameter);
    }

    private Point GetPointOnLineSegment(int index, double parameter)
    {
      int index1 = index - 1;
      this.ValidateIndex(ref index1);
      return VectorUtilities.WeightedAverage(this.GetPointFromValidatedIndex(index1), this.GetPointFromValidatedIndex(index), parameter);
    }

    private Vector GetTangentToCubicSegment(int index, double parameter)
    {
      int index1 = index - 3;
      this.ValidateIndex(ref index1);
      Point fromValidatedIndex1 = this.GetPointFromValidatedIndex(index1);
      Point fromValidatedIndex2 = this.GetPointFromValidatedIndex(index1 + 1);
      Point fromValidatedIndex3 = this.GetPointFromValidatedIndex(index1 + 2);
      Point fromValidatedIndex4 = this.GetPointFromValidatedIndex(index);
      Vector vector1 = fromValidatedIndex2 - fromValidatedIndex1;
      Vector vector2 = fromValidatedIndex3 - fromValidatedIndex2;
      Vector vector3 = fromValidatedIndex4 - fromValidatedIndex3;
      Vector vector4 = vector1 * (1.0 - parameter) + vector2 * parameter;
      Vector vector5 = vector2 * (1.0 - parameter) + vector3 * parameter;
      Vector vector6 = vector4 * (1.0 - parameter) + vector5 * parameter;
      vector6.Normalize();
      return vector6;
    }

    private Vector GetTangentToQuadraticSegment(int index, double parameter)
    {
      int index1 = index - 2;
      this.ValidateIndex(ref index1);
      Point fromValidatedIndex1 = this.GetPointFromValidatedIndex(index1);
      Point fromValidatedIndex2 = this.GetPointFromValidatedIndex(index1 + 1);
      Point fromValidatedIndex3 = this.GetPointFromValidatedIndex(index);
      Vector vector1 = fromValidatedIndex2 - fromValidatedIndex1;
      Vector vector2 = fromValidatedIndex3 - fromValidatedIndex2;
      Vector vector3 = vector1 * (1.0 - parameter) + vector2 * parameter;
      vector3.Normalize();
      return vector3;
    }

    private Vector GetTangentToLineSegment(int index, double parameter)
    {
      int index1 = index - 1;
      this.ValidateIndex(ref index1);
      Point fromValidatedIndex = this.GetPointFromValidatedIndex(index1);
      Vector vector = this.GetPointFromValidatedIndex(index) - fromValidatedIndex;
      vector.Normalize();
      return vector;
    }

    private double GetClosestPointOnLineSegment(int index, Point position, Matrix matrix, out Point closestPoint, out double distanceSquared)
    {
      Point point1 = this.GetPoint(index - 1);
      Point point2 = this.GetPoint(index);
      double toleranceSquared = double.PositiveInfinity;
      double resultParameter;
      VectorUtilities.ComputeClosestPointOnTransformedLineSegment(position, point1, point2, matrix, toleranceSquared, out resultParameter, out closestPoint, out distanceSquared);
      return resultParameter;
    }

    private double GetClosestPointOnQuadraticSegment(int index, Point position, Matrix matrix, double flatteningTolerance, out Point closestPoint, out double distanceSquared)
    {
      Point[] controlPoints = new Point[3]
      {
        this.GetPoint(index - 2),
        this.GetPoint(index - 1),
        this.GetPoint(index)
      };
      List<Point> list1 = new List<Point>(16);
      List<double> list2 = new List<double>(16);
      BezierCurveFlattener.FlattenQuadratic(controlPoints, flatteningTolerance, list1, false, list2);
      return PathFigureEditor.GetClosestPointOnFlattenedSegment(list1, list2, position, matrix, out closestPoint, out distanceSquared);
    }

    private double GetClosestPointOnCubicSegment(int index, Point position, Matrix matrix, double flatteningTolerance, out Point closestPoint, out double distanceSquared)
    {
      Point[] controlPoints = new Point[4]
      {
        this.GetPoint(index - 3),
        this.GetPoint(index - 2),
        this.GetPoint(index - 1),
        this.GetPoint(index)
      };
      List<Point> list1 = new List<Point>(16);
      List<double> list2 = new List<double>(16);
      BezierCurveFlattener.FlattenCubic(controlPoints, flatteningTolerance, list1, false, list2);
      return PathFigureEditor.GetClosestPointOnFlattenedSegment(list1, list2, position, matrix, out closestPoint, out distanceSquared);
    }

    private static double GetClosestPointOnFlattenedSegment(List<Point> polyline, List<double> bezierParameters, Point position, Matrix matrix, out Point closestPoint, out double distanceSquared)
    {
      double num = 0.0;
      closestPoint = polyline[0];
      distanceSquared = (position - closestPoint * matrix).LengthSquared;
      for (int index = 1; index < polyline.Count; ++index)
      {
        double resultParameter;
        Point resultPoint;
        double resultDistanceSquared;
        if (VectorUtilities.ComputeClosestPointOnTransformedLineSegment(position, polyline[index - 1], polyline[index], matrix, distanceSquared, out resultParameter, out resultPoint, out resultDistanceSquared))
        {
          num = bezierParameters[index - 1] + resultParameter * (bezierParameters[index] - bezierParameters[index - 1]);
          closestPoint = resultPoint;
          distanceSquared = resultDistanceSquared;
        }
      }
      return num;
    }

    private int SubdivideLine(int lastPointIndexOfSegment, double parameter)
    {
      int num = (int) this.GetPointKind(lastPointIndexOfSegment);
      bool flag = lastPointIndexOfSegment == 0 && PathFigureUtilities.IsClosed(this.PathFigure) && !PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure);
      int count = this.PathSegments.Count;
      int segmentIndex;
      if (lastPointIndexOfSegment == 0)
      {
        segmentIndex = this.PathSegments.Count;
      }
      else
      {
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, lastPointIndexOfSegment, out segmentIndex, out segmentPointIndex);
      }
      LineSegment lineSegment = PathSegmentUtilities.CreateLineSegment(VectorUtilities.WeightedAverage(this.GetPoint(lastPointIndexOfSegment - 1), this.GetPoint(lastPointIndexOfSegment), parameter), true);
      this.PathSegments.Insert(segmentIndex, (PathSegment) lineSegment);
      PathAction pathAction = new PathAction();
      pathAction.Segment = segmentIndex;
      pathAction.Parameter = parameter;
      pathAction.Action = PathActionType.InsertPoint;
      pathAction.Figure = this.figureIndex;
      pathAction.PointIndex = lastPointIndexOfSegment;
      if (flag)
      {
        if (segmentIndex > 0)
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex - 1, segmentIndex, PathChangeType.InferSubdivision));
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, PathChangeType.InferSubdivision));
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.StartPointIndex, PathChangeType.InferSubdivision));
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.DeletedPointIndex));
      }
      else
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, PathChangeType.InferSubdivision));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex + 1, PathChangeType.InferSubdivision));
        if (segmentIndex == 0)
          pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, PathChangeType.InferSubdivision));
        else
          pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex - 1, segmentIndex, PathChangeType.InferSubdivision));
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, PathStructureChange.DeletedPointIndex));
      }
      for (int oldIndex = segmentIndex + 1; oldIndex < count; ++oldIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, oldIndex + 1));
      this.changeList.Changes.Add(pathAction);
      return lastPointIndexOfSegment == 0 ? 0 : 1;
    }

    private int SubdivideQuadratic(int lastPointIndexOfSegment, double parameter)
    {
      int index = lastPointIndexOfSegment - 2;
      this.ValidateIndex(ref index);
      int count = this.PathSegments.Count;
      int segmentIndex;
      if (lastPointIndexOfSegment == 0)
      {
        segmentIndex = PathFigureUtilities.SegmentCount(this.PathFigure) - 1;
      }
      else
      {
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, lastPointIndexOfSegment, out segmentIndex, out segmentPointIndex);
      }
      QuadraticBezierSegment quadraticBezierSegment1 = (QuadraticBezierSegment) this.PathSegments[segmentIndex];
      Point fromValidatedIndex = this.GetPointFromValidatedIndex(index);
      Point point1 = quadraticBezierSegment1.Point1;
      Point point2_1 = quadraticBezierSegment1.Point2;
      Point point = VectorUtilities.WeightedAverage(fromValidatedIndex, point1, parameter);
      Point b = VectorUtilities.WeightedAverage(point1, point2_1, parameter);
      Point point2_2 = VectorUtilities.WeightedAverage(point, b, parameter);
      quadraticBezierSegment1.Point1 = b;
      QuadraticBezierSegment quadraticBezierSegment2 = PathSegmentUtilities.CreateQuadraticBezierSegment(point, point2_2, quadraticBezierSegment1.IsStroked);
      this.PathSegments.Insert(segmentIndex, (PathSegment) quadraticBezierSegment2);
      PathAction pathAction = new PathAction();
      pathAction.Segment = segmentIndex;
      pathAction.Parameter = parameter;
      pathAction.Action = PathActionType.InsertPoint;
      pathAction.Figure = this.figureIndex;
      pathAction.PointIndex = lastPointIndexOfSegment;
      if (segmentIndex == 0)
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, segmentIndex, PathChangeType.InferSubdivision));
      else
        pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex - 1, segmentIndex, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, segmentIndex + 1, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, PathStructureChange.DeletedPointIndex));
      for (int oldIndex = segmentIndex + 1; oldIndex < count; ++oldIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, oldIndex + 1));
      this.changeList.Changes.Add(pathAction);
      return lastPointIndexOfSegment == 0 ? 0 : 2;
    }

    private int SubdivideCubic(int lastPointIndexOfSegment, double parameter)
    {
      int index = lastPointIndexOfSegment - 3;
      bool flag = lastPointIndexOfSegment == 0 && PathFigureUtilities.IsClosed(this.PathFigure) && !PathFigureUtilities.IsCloseSegmentDegenerate(this.PathFigure);
      this.ValidateIndex(ref index);
      int count = this.PathSegments.Count;
      int segmentIndex;
      if (lastPointIndexOfSegment == 0)
      {
        segmentIndex = PathFigureUtilities.SegmentCount(this.PathFigure) - 1;
      }
      else
      {
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(this.PathFigure, lastPointIndexOfSegment, out segmentIndex, out segmentPointIndex);
      }
      BezierSegment bezierSegment1 = (BezierSegment) this.PathSegments[segmentIndex];
      Point fromValidatedIndex = this.GetPointFromValidatedIndex(index);
      Point point1 = bezierSegment1.Point1;
      Point point2 = bezierSegment1.Point2;
      Point point3_1 = bezierSegment1.Point3;
      Point point3_2;
      Point point3;
      if (VectorUtilities.ArePathPointsVeryClose(fromValidatedIndex, point1) && VectorUtilities.ArePathPointsVeryClose(point2, point3_1))
      {
        Point point4;
        Point point5 = point4 = VectorUtilities.WeightedAverage(fromValidatedIndex, point3_1, parameter);
        point3_2 = point4;
        point3 = point4;
      }
      else
      {
        Point point4 = VectorUtilities.WeightedAverage(point1, point2, parameter);
        point1 = VectorUtilities.WeightedAverage(fromValidatedIndex, point1, parameter);
        Point b1 = VectorUtilities.WeightedAverage(point2, point3_1, parameter);
        point3 = VectorUtilities.WeightedAverage(point1, point4, parameter);
        Point b2 = VectorUtilities.WeightedAverage(point4, b1, parameter);
        point3_2 = VectorUtilities.WeightedAverage(point3, b2, parameter);
        bezierSegment1.Point1 = b2;
        bezierSegment1.Point2 = b1;
      }
      PathAction pathAction = new PathAction();
      pathAction.Segment = segmentIndex;
      pathAction.Parameter = parameter;
      pathAction.Action = PathActionType.InsertPoint;
      pathAction.Figure = this.figureIndex;
      pathAction.PointIndex = lastPointIndexOfSegment;
      int oldIndex1 = segmentIndex == 0 ? PathStructureChange.StartPointIndex : segmentIndex - 1;
      int oldIndex2 = flag ? PathStructureChange.StartPointIndex : segmentIndex;
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex + 1, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex1, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point2Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex2, segmentIndex + 1, (DependencyProperty) null, BezierSegment.Point3Property, PathChangeType.InferSubdivision));
      pathAction.PathStructureChanges.Add(new PathStructureChange(segmentIndex, PathStructureChange.DeletedPointIndex));
      for (int oldIndex3 = segmentIndex + 1; oldIndex3 < count; ++oldIndex3)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex3, oldIndex3 + 1));
      this.changeList.Changes.Add(pathAction);
      BezierSegment bezierSegment2 = PathSegmentUtilities.CreateBezierSegment(point1, point3, point3_2, bezierSegment1.IsStroked);
      this.PathSegments.Insert(segmentIndex, (PathSegment) bezierSegment2);
      return lastPointIndexOfSegment == 0 ? 0 : 3;
    }

    private Point[] ExtractCubicControlPointsFromSegmentIndex(int segmentIndex)
    {
      PathSegment pathSegment = this.PathSegments[segmentIndex];
      Point point = segmentIndex != 0 ? PathSegmentUtilities.GetLastPoint(this.PathSegments[segmentIndex - 1]) : this.PathFigure.StartPoint;
      Point[] pointArray = new Point[4];
      LineSegment lineSegment = pathSegment as LineSegment;
      QuadraticBezierSegment quadraticBezierSegment = pathSegment as QuadraticBezierSegment;
      BezierSegment bezierSegment = pathSegment as BezierSegment;
      if (lineSegment != null)
      {
        pointArray[0] = point;
        pointArray[3] = lineSegment.Point;
        pointArray[1] = VectorUtilities.WeightedAverage(pointArray[0], pointArray[3], 1.0 / 3.0);
        pointArray[2] = VectorUtilities.WeightedAverage(pointArray[0], pointArray[3], 2.0 / 3.0);
      }
      else if (quadraticBezierSegment != null)
      {
        pointArray[0] = point;
        pointArray[1] = quadraticBezierSegment.Point1;
        pointArray[3] = quadraticBezierSegment.Point2;
        pointArray[2] = VectorUtilities.WeightedAverage(pointArray[1], pointArray[3], 1.0 / 3.0);
        pointArray[1] = VectorUtilities.WeightedAverage(pointArray[0], pointArray[1], 2.0 / 3.0);
      }
      else if (bezierSegment != null)
      {
        pointArray[0] = point;
        pointArray[1] = bezierSegment.Point1;
        pointArray[2] = bezierSegment.Point2;
        pointArray[3] = bezierSegment.Point3;
      }
      return pointArray;
    }

    private void AddCloseChange(bool removeLastSegment)
    {
      PathAction pathAction = new PathAction();
      pathAction.Action = PathActionType.Close;
      pathAction.Figure = this.figureIndex;
      if (removeLastSegment)
        pathAction.PathStructureChanges.Add(new PathStructureChange(this.PathSegments.Count - 1, PathStructureChange.DeletedPointIndex));
      this.changeList.Changes.Add(pathAction);
    }
  }
}
