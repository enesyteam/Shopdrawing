// Decompiled with JetBrains decompiler
// Type: MS.Internal.FloatingPointArithmetic
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace MS.Internal
{
  internal sealed class FloatingPointArithmetic
  {
    private static double doubleTolerance = 1E-20;
    private static double singleTolerance = 0.0 / 1.0;
    private static double distanceTolerance = 0.0 / 1.0;
    private static double squaredDistanceTolerance = FloatingPointArithmetic.distanceTolerance * FloatingPointArithmetic.distanceTolerance;

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
