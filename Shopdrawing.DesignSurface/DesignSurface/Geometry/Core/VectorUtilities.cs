// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.VectorUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public static class VectorUtilities
  {
    public static Matrix GetMatrixFromTransform(GeneralTransform generalTransform)
    {
      Transform transform = generalTransform as Transform;
      if (transform != null)
        return transform.Value;
      GeneralTransformGroup generalTransformGroup = generalTransform as GeneralTransformGroup;
      if (generalTransformGroup != null)
      {
        Matrix identity = Matrix.Identity;
        foreach (GeneralTransform generalTransform1 in generalTransformGroup.Children)
          identity *= VectorUtilities.GetMatrixFromTransform(generalTransform1);
        return identity;
      }
      if (generalTransform == null)
        throw new InvalidOperationException(StringTable.GeneralTransformIsNotAffineException);
      return Matrix.Identity;
    }

    public static bool AngleIsGreaterThan(Vector a, Vector b, double angleInRadian)
    {
      if (angleInRadian <= 0.0 || angleInRadian >= Math.PI)
        throw new ArgumentOutOfRangeException("angleInRadian", (object) angleInRadian, "Angle threshold must be strictly between zero and Pi.");
      double num1 = Math.Cos(angleInRadian);
      double num2 = a.X * b.X + a.Y * b.Y;
      if (num2 >= 0.0)
      {
        if (num1 <= 0.0)
          return false;
        return num2 * num2 < num1 * num1 * a.LengthSquared * b.LengthSquared;
      }
      if (num1 >= 0.0)
        return true;
      return num2 * num2 > num1 * num1 * a.LengthSquared * b.LengthSquared;
    }

    public static Point Midpoint(Point a, Point b)
    {
      return new Point((a.X + b.X) / 2.0, (a.Y + b.Y) / 2.0);
    }

    public static Point WeightedAverage(Point a, Point b, double t)
    {
      double num = 1.0 - t;
      return new Point(num * a.X + t * b.X, num * a.Y + t * b.Y);
    }

    public static bool ArePathPointsVeryClose(Point a, Point b)
    {
      double num1 = b.X - a.X;
      double num2 = b.Y - a.Y;
      return num1 * num1 + num2 * num2 <= FloatingPointArithmetic.PathPointDistanceTolerance;
    }

    public static bool ComputeClosestPointOnTransformedLineSegment(Point point, Point a, Point b, Matrix matrix, double toleranceSquared, out double resultParameter, out Point resultPoint, out double resultDistanceSquared)
    {
      Vector a1 = (b - a) * matrix;
      Vector b1 = point - a * matrix;
      double lengthSquared = a1.LengthSquared;
      resultParameter = lengthSquared < FloatingPointArithmetic.SquaredDistanceTolerance ? 0.0 : VectorUtilities.Dot(a1, b1) / lengthSquared;
      if (resultParameter <= 0.0)
      {
        resultParameter = 0.0;
        resultPoint = a;
      }
      else if (resultParameter >= 1.0)
      {
        resultParameter = 1.0;
        resultPoint = b;
      }
      else
        resultPoint = VectorUtilities.WeightedAverage(a, b, resultParameter);
      resultDistanceSquared = (resultPoint * matrix - point).LengthSquared;
      return resultDistanceSquared <= toleranceSquared;
    }

    public static double Dot(Vector a, Vector b)
    {
      return a.X * b.X + a.Y * b.Y;
    }

    public static double Distance(Point a, Point b)
    {
      double num1 = a.X - b.X;
      double num2 = a.Y - b.Y;
      return Math.Sqrt(num1 * num1 + num2 * num2);
    }

    public static double SquaredDistance(Point a, Point b)
    {
      double num1 = a.X - b.X;
      double num2 = a.Y - b.Y;
      return num1 * num1 + num2 * num2;
    }

    internal static bool ArePolylinesClose(List<Point> p, double[] lengthP, int firstP, int lastP, List<Point> q, double[] lengthQ, int firstQ, int lastQ, double distanceTolerance, ref int firstBadVertexInQ)
    {
      double num1 = distanceTolerance * distanceTolerance;
      int index1 = firstP;
      double num2 = lengthP[firstP];
      double num3 = lengthQ[firstQ];
      for (int index2 = firstQ + 1; index2 < lastQ; ++index2)
      {
        while (index1 <= lastP && lengthQ[index2] - num3 > lengthP[index1] - num2)
          ++index1;
        if (index1 > lastP)
        {
          for (int index3 = index2; index3 < lastQ; ++index3)
          {
            if (VectorUtilities.SquaredDistance(p[lastP], q[index3]) > num1)
            {
              firstBadVertexInQ = index3;
              return false;
            }
          }
          return true;
        }
        Vector vector = (p[index1] - p[index1 - 1]) * ((lengthQ[index2] - num3 - lengthP[index1 - 1] + num2) / (lengthP[index1] - lengthP[index1 - 1]));
        if (VectorUtilities.SquaredDistance(p[index1 - 1] + vector, q[index2]) > num1)
        {
          firstBadVertexInQ = index2;
          return false;
        }
      }
      return true;
    }

    public static double[] GetCumulatedChordLength(List<Point> points, int firstIndex, int lastIndex)
    {
      if (firstIndex > lastIndex || firstIndex < 0 || (lastIndex < 0 || firstIndex >= points.Count) || lastIndex >= points.Count)
        throw new ArgumentException("The specified indices are invalid.");
      double[] numArray = new double[lastIndex - firstIndex + 1];
      double num = 0.0;
      numArray[0] = num;
      int index1 = firstIndex + 1;
      int index2 = 1;
      while (index1 <= lastIndex)
      {
        num += VectorUtilities.Distance(points[index1 - 1], points[index1]);
        numArray[index2] = num;
        ++index1;
        ++index2;
      }
      return numArray;
    }

    public static Vector UnitNormal(Vector v)
    {
      Vector vector = new Vector(v.Y, -v.X);
      vector.Normalize();
      return vector;
    }

    public static bool HaveOppositeDirections(Vector a, Vector b)
    {
      double lengthSquared1 = a.LengthSquared;
      double lengthSquared2 = b.LengthSquared;
      if (lengthSquared1 >= FloatingPointArithmetic.SquaredDistanceTolerance && lengthSquared2 >= FloatingPointArithmetic.SquaredDistanceTolerance)
        return a * b / Math.Sqrt(lengthSquared1 * lengthSquared2) < -0.99999;
      return false;
    }

    public static bool ComputeClosestPointOnLineSegment(Point point, Point a, Point b, double toleranceSquared, out double resultParameter, out Point resultPoint, out double resultDistanceSquared)
    {
      return VectorUtilities.ComputeClosestPointOnTransformedLineSegment(point, a, b, Matrix.Identity, toleranceSquared, out resultParameter, out resultPoint, out resultDistanceSquared);
    }

    public static bool IsZero(Vector a)
    {
      return a.LengthSquared < FloatingPointArithmetic.SquaredDistanceTolerance;
    }
  }
}
