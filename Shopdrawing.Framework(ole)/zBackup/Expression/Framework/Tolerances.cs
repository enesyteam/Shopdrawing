// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Tolerances
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.Framework
{
  public static class Tolerances
  {
    private static readonly double HitSquaredDistanceTolerance = 9.0;
    private static readonly double MoveSquaredDistanceTolerance = 9.0;
    private static readonly double FittingDistanceTolerance = 5.0;
    private static readonly double FitCornerThreshold = Math.PI / 2.0;
    private static readonly double FlatteningTolerance = 0.01;
    private static readonly double Epsilon = 2.22044604925031E-16;
    private static readonly double ZeroThreshold = 2.22044604925031E-15;

    public static double CurveFitCornerThreshold
    {
      get
      {
        return Tolerances.FitCornerThreshold;
      }
    }

    public static double CurveFlatteningTolerance
    {
      get
      {
        return Tolerances.FlatteningTolerance;
      }
    }

    public static bool DoPointsHit(double squaredDistance, double zoom)
    {
      return squaredDistance <= Tolerances.HitSquaredDistanceTolerance / (zoom * zoom);
    }

    public static bool DoPointsHit(Point point1, Point point2, double zoom)
    {
      return Tolerances.DoPointsHit((point1 - point2).LengthSquared, zoom);
    }

    public static bool HaveMoved(double squaredDistance, double zoom)
    {
      return squaredDistance >= Tolerances.MoveSquaredDistanceTolerance / (zoom * zoom);
    }

    public static bool HaveMoved(Point point1, Point point2, double zoom)
    {
      return Tolerances.HaveMoved((point1 - point2).LengthSquared, zoom);
    }

    public static double CurveFittingDistanceTolerance(double zoom)
    {
      return Tolerances.FittingDistanceTolerance / zoom;
    }

    public static bool AreClose(Point point1, Point point2)
    {
      if (Tolerances.AreClose(point1.X, point2.X))
        return Tolerances.AreClose(point1.Y, point2.Y);
      return false;
    }

    public static bool AreClose(Vector vector1, Vector vector2)
    {
      if (Tolerances.AreClose(vector1.X, vector2.X))
        return Tolerances.AreClose(vector1.Y, vector2.Y);
      return false;
    }

    public static bool AreClose(Vector3D vector1, Vector3D vector2)
    {
      if (Tolerances.AreClose(vector1.X, vector2.X) && Tolerances.AreClose(vector1.Y, vector2.Y))
        return Tolerances.AreClose(vector1.Z, vector2.Z);
      return false;
    }

    public static bool AreClose(double value1, double value2)
    {
      if (value1 == value2)
        return true;
      double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * Tolerances.Epsilon;
      double num2 = value1 - value2;
      if (-num1 < num2)
        return num1 > num2;
      return false;
    }

    public static bool NearZero(Point point)
    {
      if (Tolerances.NearZero(point.X))
        return Tolerances.NearZero(point.Y);
      return false;
    }

    public static bool NearZero(Vector vector)
    {
      if (Tolerances.NearZero(vector.X))
        return Tolerances.NearZero(vector.Y);
      return false;
    }

    public static bool NearZero(double d)
    {
      return Math.Abs(d) < Tolerances.ZeroThreshold;
    }

    public static bool GreaterThan(double value1, double value2)
    {
      if (value1 > value2)
        return !Tolerances.AreClose(value1, value2);
      return false;
    }

    public static bool GreaterThanOrClose(double value1, double value2)
    {
      if (value1 <= value2)
        return Tolerances.AreClose(value1, value2);
      return true;
    }

    public static bool LessThan(double value1, double value2)
    {
      if (value1 < value2)
        return !Tolerances.AreClose(value1, value2);
      return false;
    }

    public static bool LessThanOrClose(double value1, double value2)
    {
      if (value1 >= value2)
        return Tolerances.AreClose(value1, value2);
      return true;
    }

    public static bool IsUniform(CornerRadius cornerRadius)
    {
      if (Tolerances.AreClose(cornerRadius.TopLeft, cornerRadius.TopRight) && Tolerances.AreClose(cornerRadius.TopLeft, cornerRadius.BottomRight))
        return Tolerances.AreClose(cornerRadius.TopLeft, cornerRadius.BottomLeft);
      return false;
    }
  }
}
