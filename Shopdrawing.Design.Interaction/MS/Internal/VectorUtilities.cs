// Decompiled with JetBrains decompiler
// Type: MS.Internal.VectorUtilities
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal
{
  internal static class VectorUtilities
  {
    public static Matrix GetMatrixFromTransform(GeneralTransform generalTransform)
    {
      Transform transform = generalTransform as Transform;
      if (transform != null)
        return transform.Value;
      return Matrix.Identity;
    }

    public static bool AngleIsGreaterThan(Vector a, Vector b, double angleInRadian)
    {
      if (angleInRadian > 0.0)
        ;
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

    public static bool AreVeryClose(Point a, Point b)
    {
      return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y) <= FloatingPointArithmetic.SquaredDistanceTolerance;
    }

    public static Vector Unscale(Vector start, Vector scale)
    {
      return new Vector(start.X / scale.X, start.Y / scale.Y);
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
      if (firstIndex <= lastIndex && firstIndex >= 0 && (lastIndex >= 0 && firstIndex < points.Count))
      {
        int count = points.Count;
      }
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

    internal static Vector Scale(Vector start, Vector scale)
    {
      return new Vector(start.X * scale.X, start.Y * scale.Y);
    }

    internal static Vector InvertScale(Vector scale)
    {
      return new Vector(scale.X == 0.0 ? 0.0 : 1.0 / scale.X, scale.Y == 0.0 ? 0.0 : 1.0 / scale.Y);
    }

    internal static Vector RemoveMirror(Vector currentScale)
    {
      return new Vector(Math.Abs(currentScale.X), Math.Abs(currentScale.Y));
    }

    internal static Rect Scale(Vector vector, Rect rect)
    {
      Matrix matrix = new Matrix(vector.X, 0.0, 0.0, vector.Y, 0.0, 0.0);
      return new Rect(rect.TopLeft * matrix, rect.BottomRight * matrix);
    }
  }
}
