// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathSegmentUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public static class PathSegmentUtilities
  {
    public static LineSegment CreateLineSegment(Point point, bool isStroked)
    {
      return PathSegmentUtilities.CreateLineSegment(point, isStroked, new bool?());
    }

    public static LineSegment CreateLineSegment(Point point, bool isStroked, bool? isSmoothJoin)
    {
      LineSegment lineSegment = new LineSegment();
      lineSegment.Point = point;
      PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) lineSegment, isStroked, isSmoothJoin);
      return lineSegment;
    }

    public static QuadraticBezierSegment CreateQuadraticBezierSegment(Point point1, Point point2, bool isStroked)
    {
      return PathSegmentUtilities.CreateQuadraticBezierSegment(point1, point2, isStroked, new bool?());
    }

    public static QuadraticBezierSegment CreateQuadraticBezierSegment(Point point1, Point point2, bool isStroked, bool? isSmoothJoin)
    {
      QuadraticBezierSegment quadraticBezierSegment = new QuadraticBezierSegment();
      quadraticBezierSegment.Point1 = point1;
      quadraticBezierSegment.Point2 = point2;
      PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) quadraticBezierSegment, isStroked, isSmoothJoin);
      return quadraticBezierSegment;
    }

    public static BezierSegment CreateBezierSegment(Point point1, Point point2, Point point3, bool isStroked)
    {
      return PathSegmentUtilities.CreateBezierSegment(point1, point2, point3, isStroked, new bool?());
    }

    public static BezierSegment CreateBezierSegment(Point point1, Point point2, Point point3, bool isStroked, bool? isSmoothJoin)
    {
      BezierSegment bezierSegment = new BezierSegment();
      bezierSegment.Point1 = point1;
      bezierSegment.Point2 = point2;
      bezierSegment.Point3 = point3;
      PathSegmentUtilities.SetStrokeAndJoinOnSegment((PathSegment) bezierSegment, isStroked, isSmoothJoin);
      return bezierSegment;
    }

    public static void SetStrokeAndJoinOnSegment(PathSegment segment, bool isStroked, bool? isSmoothJoin)
    {
      if (!isStroked)
        segment.IsStroked = isStroked;
      if (!isSmoothJoin.HasValue || segment.IsSmoothJoin == isSmoothJoin.Value)
        return;
      segment.IsSmoothJoin = isSmoothJoin.Value;
    }

    public static bool IsEmpty(PathSegment segment)
    {
      return PathSegmentUtilities.GetPointCount(segment) == 0;
    }

    public static int GetPointCount(PathSegment segment)
    {
      if (segment is ArcSegment || segment is LineSegment)
        return 1;
      if (segment is QuadraticBezierSegment)
        return 2;
      if (segment is BezierSegment)
        return 3;
      PolyLineSegment polyLineSegment;
      if ((polyLineSegment = segment as PolyLineSegment) != null)
        return polyLineSegment.Points.Count;
      PolyQuadraticBezierSegment quadraticBezierSegment;
      if ((quadraticBezierSegment = segment as PolyQuadraticBezierSegment) != null)
        return quadraticBezierSegment.Points.Count / 2 * 2;
      PolyBezierSegment polyBezierSegment;
      if ((polyBezierSegment = segment as PolyBezierSegment) != null)
        return polyBezierSegment.Points.Count / 3 * 3;
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnknownPathSegmentType, new object[1]
      {
        (object) segment.GetType().Name
      }), "segment");
    }

    public static Point GetLastPoint(PathSegment segment)
    {
      if (PathSegmentUtilities.IsEmpty(segment))
        throw new ArgumentException(ExceptionStringTable.CannotGetLastPointFromAnEmptySegment, "segment");
      return PathSegmentUtilities.GetPoint(segment, PathSegmentUtilities.GetPointCount(segment) - 1);
    }

    public static void SetLastPoint(PathSegment segment, Point point)
    {
      if (PathSegmentUtilities.IsEmpty(segment))
        throw new ArgumentException(ExceptionStringTable.CannotGetLastPointFromAnEmptySegment, "segment");
      PathSegmentUtilities.SetPoint(segment, PathSegmentUtilities.GetPointCount(segment) - 1, point);
    }

    public static Point GetPoint(PathSegment segment, int index)
    {
      if (index < 0 || index >= PathSegmentUtilities.GetPointCount(segment))
        throw new ArgumentOutOfRangeException("index", (object) index, "Index must be between zero and the segment point count.");
      ArcSegment arcSegment;
      if ((arcSegment = segment as ArcSegment) != null)
        return arcSegment.Point;
      LineSegment lineSegment;
      if ((lineSegment = segment as LineSegment) != null)
        return lineSegment.Point;
      QuadraticBezierSegment quadraticBezierSegment1;
      if ((quadraticBezierSegment1 = segment as QuadraticBezierSegment) != null)
      {
        if (index == 0)
          return quadraticBezierSegment1.Point1;
        return quadraticBezierSegment1.Point2;
      }
      BezierSegment bezierSegment;
      if ((bezierSegment = segment as BezierSegment) != null)
      {
        if (index == 0)
          return bezierSegment.Point1;
        if (index == 1)
          return bezierSegment.Point2;
        return bezierSegment.Point3;
      }
      PolyLineSegment polyLineSegment;
      if ((polyLineSegment = segment as PolyLineSegment) != null)
        return polyLineSegment.Points[index];
      PolyQuadraticBezierSegment quadraticBezierSegment2;
      if ((quadraticBezierSegment2 = segment as PolyQuadraticBezierSegment) != null)
        return quadraticBezierSegment2.Points[index];
      PolyBezierSegment polyBezierSegment;
      if ((polyBezierSegment = segment as PolyBezierSegment) != null)
        return polyBezierSegment.Points[index];
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnknownPathSegmentType, new object[1]
      {
        (object) segment.GetType().Name
      }), "segment");
    }

    public static void SetPoint(PathSegment segment, int index, Point point)
    {
      if (index < 0 || index >= PathSegmentUtilities.GetPointCount(segment))
        throw new ArgumentOutOfRangeException("index", (object) index, "Index must be between zero and the segment point count.");
      ArcSegment arcSegment;
      if ((arcSegment = segment as ArcSegment) != null)
      {
        arcSegment.Point = point;
      }
      else
      {
        LineSegment lineSegment;
        if ((lineSegment = segment as LineSegment) != null)
        {
          lineSegment.Point = point;
        }
        else
        {
          QuadraticBezierSegment quadraticBezierSegment1;
          if ((quadraticBezierSegment1 = segment as QuadraticBezierSegment) != null)
          {
            if (index == 0)
              quadraticBezierSegment1.Point1 = point;
            else
              quadraticBezierSegment1.Point2 = point;
          }
          else
          {
            BezierSegment bezierSegment;
            if ((bezierSegment = segment as BezierSegment) != null)
            {
              if (index == 0)
                bezierSegment.Point1 = point;
              else if (index == 1)
                bezierSegment.Point2 = point;
              else
                bezierSegment.Point3 = point;
            }
            else
            {
              PolyLineSegment polyLineSegment;
              if ((polyLineSegment = segment as PolyLineSegment) != null)
              {
                polyLineSegment.Points[index] = point;
              }
              else
              {
                PolyQuadraticBezierSegment quadraticBezierSegment2;
                if ((quadraticBezierSegment2 = segment as PolyQuadraticBezierSegment) != null)
                {
                  quadraticBezierSegment2.Points[index] = point;
                }
                else
                {
                  PolyBezierSegment polyBezierSegment;
                  if ((polyBezierSegment = segment as PolyBezierSegment) != null)
                    polyBezierSegment.Points[index] = point;
                  else
                    throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnknownPathSegmentType, new object[1]
                    {
                      (object) segment.GetType().Name
                    }), "segment");
                }
              }
            }
          }
        }
      }
    }

    public static PathPointKind GetPointKind(PathSegment segment, int index)
    {
      if (index < 0 || index >= PathSegmentUtilities.GetPointCount(segment))
        throw new ArgumentOutOfRangeException("index", (object) index, "Index must be between zero and the segment point count.");
      if (segment is ArcSegment)
        return PathPointKind.Arc;
      if (segment is LineSegment || segment is PolyLineSegment)
        return PathPointKind.Line;
      if (segment is QuadraticBezierSegment || segment is PolyQuadraticBezierSegment)
        return index % 2 == 1 ? PathPointKind.Quadratic : PathPointKind.BezierHandle;
      if (segment is BezierSegment || segment is PolyBezierSegment)
        return index % 3 == 2 ? PathPointKind.Cubic : PathPointKind.BezierHandle;
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnknownPathSegmentType, new object[1]
      {
        (object) segment.GetType().Name
      }), "segment");
    }

    public static void TransformSegment(PathSegment segment, Transform transform)
    {
      if (transform == null)
        return;
      Matrix matrix = transform.Value;
      if (segment is ArcSegment)
        throw new NotSupportedException(ExceptionStringTable.ArcSegmentIsNotSupported);
      LineSegment lineSegment;
      if ((lineSegment = segment as LineSegment) != null)
      {
        lineSegment.Point *= matrix;
      }
      else
      {
        QuadraticBezierSegment quadraticBezierSegment1;
        if ((quadraticBezierSegment1 = segment as QuadraticBezierSegment) != null)
        {
          quadraticBezierSegment1.Point1 *= matrix;
          quadraticBezierSegment1.Point2 *= matrix;
        }
        else
        {
          BezierSegment bezierSegment;
          if ((bezierSegment = segment as BezierSegment) != null)
          {
            bezierSegment.Point1 *= matrix;
            bezierSegment.Point2 *= matrix;
            bezierSegment.Point3 *= matrix;
          }
          else
          {
            PolyLineSegment polyLineSegment;
            if ((polyLineSegment = segment as PolyLineSegment) != null)
            {
              PathSegmentUtilities.TransformPoints(polyLineSegment.Points, matrix);
            }
            else
            {
              PolyQuadraticBezierSegment quadraticBezierSegment2;
              if ((quadraticBezierSegment2 = segment as PolyQuadraticBezierSegment) != null)
              {
                PathSegmentUtilities.TransformPoints(quadraticBezierSegment2.Points, matrix);
              }
              else
              {
                PolyBezierSegment polyBezierSegment;
                if ((polyBezierSegment = segment as PolyBezierSegment) != null)
                  PathSegmentUtilities.TransformPoints(polyBezierSegment.Points, matrix);
                else
                  throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnknownPathSegmentType, new object[1]
                  {
                    (object) segment.GetType().Name
                  }), "segment");
              }
            }
          }
        }
      }
    }

    private static void TransformPoints(PointCollection pointCollection, Matrix matrix)
    {
      for (int index = 0; index < pointCollection.Count; ++index)
        pointCollection[index] *= matrix;
    }
  }
}
