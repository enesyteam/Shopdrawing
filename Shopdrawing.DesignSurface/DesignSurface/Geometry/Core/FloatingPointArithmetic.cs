// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.FloatingPointArithmetic
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public sealed class FloatingPointArithmetic
  {
    private static double doubleTolerance = 1E-20;
    private static double singleTolerance = 0.0 / 1.0;
    private static double distanceTolerance = 0.0 / 1.0;
    private static double squaredDistanceTolerance = FloatingPointArithmetic.distanceTolerance * FloatingPointArithmetic.distanceTolerance;
    private static double pathPointDistanceTolerance = 1E-07;
    private static double angleTolerance = Math.PI / 720.0;

    public static double DoubleTolerance
    {
      get
      {
        return FloatingPointArithmetic.doubleTolerance;
      }
    }

    public static double SingleTolerance
    {
      get
      {
        return FloatingPointArithmetic.singleTolerance;
      }
    }

    public static double DistanceTolerance
    {
      get
      {
        return FloatingPointArithmetic.distanceTolerance;
      }
    }

    public static double SquaredDistanceTolerance
    {
      get
      {
        return FloatingPointArithmetic.squaredDistanceTolerance;
      }
    }

    public static double PathPointDistanceTolerance
    {
      get
      {
        return FloatingPointArithmetic.pathPointDistanceTolerance;
      }
    }

    public static double AngleTolerance
    {
      get
      {
        return FloatingPointArithmetic.angleTolerance;
      }
    }

    private FloatingPointArithmetic()
    {
    }

    public static bool IsVerySmall(double k)
    {
      return Math.Abs(k) < FloatingPointArithmetic.DoubleTolerance;
    }

    public static bool IsVerySmall(float k)
    {
      return (double) Math.Abs(k) < FloatingPointArithmetic.SingleTolerance;
    }

    public static bool IsInClosedInterval(double x, double a, double b)
    {
      if (x >= a)
        return x <= b;
      return false;
    }

    public static float ToSingle(double d)
    {
      return (float) d;
    }

    public static double Hypotenuse(double x, double y)
    {
      return Math.Sqrt(x * x + y * y);
    }

    public static double DoubleFromMantissaAndExponent(double x, int exp)
    {
      return x * Math.Pow(2.0, (double) exp);
    }

    public static bool IsFiniteDouble(double x)
    {
      if (!double.IsInfinity(x))
        return !double.IsNaN(x);
      return false;
    }
  }
}
