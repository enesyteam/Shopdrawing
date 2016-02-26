// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathFigureUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public static class PathFigureUtilities
  {
    public static DependencyProperty FigureMappingProperty = DependencyProperty.RegisterAttached("FigureMapping", typeof (int), typeof (PathFigureUtilities), new PropertyMetadata((object) -1));
    public static DependencyProperty SegmentMappingProperty = DependencyProperty.RegisterAttached("SegmentMapping", typeof (int), typeof (PathFigureUtilities), new PropertyMetadata((object) -1));

    public static PathFigure Copy(PathFigure original, Transform transformToApply)
    {
      PathFigure pathFigure = new PathFigure();
      if (pathFigure.IsFilled != original.IsFilled)
        pathFigure.IsFilled = original.IsFilled;
      if (pathFigure.IsClosed != original.IsClosed)
        pathFigure.IsClosed = original.IsClosed;
      int num = (int) original.GetValue(PathFigureUtilities.FigureMappingProperty);
      pathFigure.SetValue(PathFigureUtilities.FigureMappingProperty, (object) num);
      pathFigure.StartPoint = transformToApply == null ? original.StartPoint : original.StartPoint * transformToApply.Value;
      foreach (PathSegment pathSegment in original.Segments)
      {
        PathSegment segment = pathSegment.CloneCurrentValue();
        if (transformToApply != null)
          PathSegmentUtilities.TransformSegment(segment, transformToApply);
        pathFigure.Segments.Add(segment);
      }
      return pathFigure;
    }

    public static bool EnsureOnlySingleSegmentsInFigure(PathFigure original)
    {
      bool flag = false;
      PathSegmentCollection segmentCollection = new PathSegmentCollection();
      foreach (PathSegment pathSegment in original.Segments)
      {
        PolyLineSegment polyLineSegment = pathSegment as PolyLineSegment;
        PolyQuadraticBezierSegment quadraticBezierSegment1 = pathSegment as PolyQuadraticBezierSegment;
        PolyBezierSegment polyBezierSegment = pathSegment as PolyBezierSegment;
        if (polyLineSegment != null)
        {
          foreach (Point point in polyLineSegment.Points)
          {
            LineSegment lineSegment = PathSegmentUtilities.CreateLineSegment(point, polyLineSegment.IsStroked, new bool?(polyLineSegment.IsSmoothJoin));
            segmentCollection.Add((PathSegment) lineSegment);
          }
          flag = true;
        }
        else if (quadraticBezierSegment1 != null)
        {
          int index = 0;
          while (index < quadraticBezierSegment1.Points.Count - 1)
          {
            QuadraticBezierSegment quadraticBezierSegment2 = PathSegmentUtilities.CreateQuadraticBezierSegment(quadraticBezierSegment1.Points[index], quadraticBezierSegment1.Points[index + 1], quadraticBezierSegment1.IsStroked, new bool?(quadraticBezierSegment1.IsSmoothJoin));
            segmentCollection.Add((PathSegment) quadraticBezierSegment2);
            index += 2;
          }
          flag = true;
        }
        else if (polyBezierSegment != null)
        {
          int index = 0;
          while (index < polyBezierSegment.Points.Count - 2)
          {
            BezierSegment bezierSegment = PathSegmentUtilities.CreateBezierSegment(polyBezierSegment.Points[index], polyBezierSegment.Points[index + 1], polyBezierSegment.Points[index + 2], polyBezierSegment.IsStroked, new bool?(polyBezierSegment.IsSmoothJoin));
            segmentCollection.Add((PathSegment) bezierSegment);
            index += 3;
          }
          flag = true;
        }
        else
          segmentCollection.Add(pathSegment);
      }
      if (flag)
        original.Segments = segmentCollection;
      return flag;
    }

    private static bool IsSignificantChangeBetweenSegments(PathSegment original, PathSegment newSegment)
    {
      if (original.IsStroked == newSegment.IsStroked)
        return original.IsSmoothJoin != newSegment.IsSmoothJoin;
      return true;
    }

    public static bool CollapseSingleSegmentsToPolySegments(PathFigure original)
    {
      bool flag = false;
      PathSegmentCollection segmentCollection = new PathSegmentCollection();
      for (int index = 0; index < original.Segments.Count; ++index)
      {
        LineSegment lineSegment = original.Segments[index] as LineSegment;
        QuadraticBezierSegment quadraticBezierSegment1 = original.Segments[index] as QuadraticBezierSegment;
        BezierSegment bezierSegment1 = original.Segments[index] as BezierSegment;
        int num = index;
        if (lineSegment != null)
        {
          PointCollection pointCollection = (PointCollection) null;
          for (; index + 1 < original.Segments.Count && original.Segments[index + 1] is LineSegment && !PathFigureUtilities.IsSignificantChangeBetweenSegments((PathSegment) lineSegment, original.Segments[index + 1]); ++index)
          {
            if (pointCollection == null)
            {
              pointCollection = new PointCollection();
              pointCollection.Add(lineSegment.Point);
            }
            pointCollection.Add(((LineSegment) original.Segments[index + 1]).Point);
          }
          if (index != num)
          {
            PolyLineSegment polyLineSegment = new PolyLineSegment();
            polyLineSegment.Points = pointCollection;
            PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) polyLineSegment, lineSegment.IsStroked, new bool?(lineSegment.IsSmoothJoin));
            segmentCollection.Add((PathSegment) polyLineSegment);
            flag = true;
          }
          else
            segmentCollection.Add((PathSegment) lineSegment);
        }
        else if (quadraticBezierSegment1 != null)
        {
          PointCollection pointCollection = (PointCollection) null;
          QuadraticBezierSegment quadraticBezierSegment2;
          for (; index + 1 < original.Segments.Count && (quadraticBezierSegment2 = original.Segments[index + 1] as QuadraticBezierSegment) != null && !PathFigureUtilities.IsSignificantChangeBetweenSegments((PathSegment) quadraticBezierSegment1, (PathSegment) quadraticBezierSegment2); ++index)
          {
            if (pointCollection == null)
            {
              pointCollection = new PointCollection();
              pointCollection.Add(quadraticBezierSegment1.Point1);
              pointCollection.Add(quadraticBezierSegment1.Point2);
            }
            pointCollection.Add(quadraticBezierSegment2.Point1);
            pointCollection.Add(quadraticBezierSegment2.Point2);
          }
          if (index != num)
          {
            PolyQuadraticBezierSegment quadraticBezierSegment3 = new PolyQuadraticBezierSegment();
            quadraticBezierSegment3.Points = pointCollection;
            PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) quadraticBezierSegment3, quadraticBezierSegment1.IsStroked, new bool?(quadraticBezierSegment1.IsSmoothJoin));
            segmentCollection.Add((PathSegment) quadraticBezierSegment3);
            flag = true;
          }
          else
            segmentCollection.Add((PathSegment) quadraticBezierSegment1);
        }
        else if (bezierSegment1 != null)
        {
          PointCollection pointCollection = (PointCollection) null;
          BezierSegment bezierSegment2;
          for (; index + 1 < original.Segments.Count && (bezierSegment2 = original.Segments[index + 1] as BezierSegment) != null && !PathFigureUtilities.IsSignificantChangeBetweenSegments((PathSegment) bezierSegment1, (PathSegment) bezierSegment2); ++index)
          {
            if (pointCollection == null)
            {
              pointCollection = new PointCollection();
              pointCollection.Add(bezierSegment1.Point1);
              pointCollection.Add(bezierSegment1.Point2);
              pointCollection.Add(bezierSegment1.Point3);
            }
            pointCollection.Add(bezierSegment2.Point1);
            pointCollection.Add(bezierSegment2.Point2);
            pointCollection.Add(bezierSegment2.Point3);
          }
          if (index != num)
          {
            PolyBezierSegment polyBezierSegment = new PolyBezierSegment();
            polyBezierSegment.Points = pointCollection;
            PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) polyBezierSegment, bezierSegment1.IsStroked, new bool?(bezierSegment1.IsSmoothJoin));
            segmentCollection.Add((PathSegment) polyBezierSegment);
            flag = true;
          }
          else
            segmentCollection.Add((PathSegment) bezierSegment1);
        }
        else
          segmentCollection.Add(original.Segments[index]);
      }
      if (flag)
        original.Segments = segmentCollection;
      return flag;
    }

    public static void GetSegmentFromPointIndex(PathFigure pathFigure, int pointIndex, out int segmentIndex, out int segmentPointIndex)
    {
      PathSegmentCollection segments = pathFigure.Segments;
      if (pointIndex == 0)
      {
        if (pathFigure.IsClosed && PathFigureUtilities.IsCloseSegmentDegenerate(pathFigure))
        {
          segmentIndex = pathFigure.Segments.Count - 1;
          segmentPointIndex = PathSegmentUtilities.GetPointCount(segments[segmentIndex]) - 1;
        }
        else
        {
          segmentIndex = 0;
          segmentPointIndex = -1;
        }
      }
      else
      {
        segmentIndex = 0;
        segmentPointIndex = 0;
        int num = 1;
        int pointCount = PathSegmentUtilities.GetPointCount(segments[segmentIndex]);
        while (pointCount < pointIndex)
        {
          num = pointCount + 1;
          pointCount += PathSegmentUtilities.GetPointCount(segments[++segmentIndex]);
        }
        segmentPointIndex = pointIndex - num;
      }
    }

    public static void SetPoint(PathFigure pathFigure, int pointIndex, Point point)
    {
      if (pointIndex == 0 || pointIndex == PathFigureUtilities.PointCount(pathFigure) && PathFigureUtilities.IsClosed(pathFigure) && !PathFigureUtilities.IsCloseSegmentDegenerate(pathFigure))
      {
        pathFigure.StartPoint = point;
      }
      else
      {
        PathSegmentCollection segments = pathFigure.Segments;
        int segmentIndex;
        int segmentPointIndex;
        PathFigureUtilities.GetSegmentFromPointIndex(pathFigure, pointIndex, out segmentIndex, out segmentPointIndex);
        PathSegmentUtilities.SetPoint(segments[segmentIndex], segmentPointIndex, point);
      }
    }

    public static Point GetPoint(PathFigure pathFigure, int index)
    {
      return PathFigureUtilities.GetPoint(pathFigure, index, false);
    }

    public static Point GetPoint(PathFigure pathFigure, int index, bool correspondingPoint)
    {
      if (index == 0)
        return pathFigure.StartPoint;
      PathSegmentCollection segments = pathFigure.Segments;
      int segmentIndex;
      int segmentPointIndex;
      PathFigureUtilities.GetSegmentFromPointIndex(pathFigure, index, out segmentIndex, out segmentPointIndex);
      PathSegment segment = segments[segmentIndex];
      if (correspondingPoint && segment is BezierSegment)
      {
        if (segmentPointIndex == 1)
          segmentPointIndex = 2;
        else if (segmentPointIndex == 0)
        {
          if (segmentIndex == 0)
            return pathFigure.StartPoint;
          if (index > 0)
          {
            PathFigureUtilities.GetSegmentFromPointIndex(pathFigure, index - 1, out segmentIndex, out segmentPointIndex);
            segment = segments[segmentIndex];
          }
        }
      }
      return PathSegmentUtilities.GetPoint(segment, segmentPointIndex);
    }

    public static Rect TightExtent(PathFigure figure, Matrix matrix)
    {
      PathGeometry pathGeometry = new PathGeometry();
      pathGeometry.Figures.Add(figure);
      pathGeometry.Transform = (Transform) new MatrixTransform(matrix);
      return pathGeometry.Bounds;
    }

    public static bool IsCloseSegmentDegenerate(PathFigure figure)
    {
      if (figure.Segments.Count == 0)
        return false;
      Point lastPoint = PathSegmentUtilities.GetLastPoint(figure.Segments[figure.Segments.Count - 1]);
      return VectorUtilities.ArePathPointsVeryClose(PathFigureUtilities.FirstPoint(figure), lastPoint);
    }

    public static bool IsClosed(PathFigure figure)
    {
      return figure.IsClosed;
    }

    public static bool IsOpen(PathFigure figure)
    {
      return !PathFigureUtilities.IsClosed(figure);
    }

    public static bool IsIsolatedPoint(PathFigure figure)
    {
      return figure.Segments.Count == 0;
    }

    public static int PointCount(PathFigure figure)
    {
      int num = 1;
      foreach (PathSegment segment in figure.Segments)
        num += PathSegmentUtilities.GetPointCount(segment);
      if (PathFigureUtilities.IsClosed(figure) && PathFigureUtilities.IsCloseSegmentDegenerate(figure))
        --num;
      return num;
    }

    public static int SegmentCount(PathFigure figure)
    {
      return figure.Segments.Count;
    }

    public static Point FirstPoint(PathFigure figure)
    {
      return figure.StartPoint;
    }
  }
}
