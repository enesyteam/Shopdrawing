// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.BezierCurveFlattener
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public static class BezierCurveFlattener
  {
    public static void FlattenCubic(Point[] controlPoints, double errorTolerance, List<Point> resultPolyline, bool connect)
    {
      if (!connect || resultPolyline.Count == 0)
        resultPolyline.Add(controlPoints[0]);
      if (BezierCurveFlattener.IsCubicChordMonotone(controlPoints, errorTolerance * errorTolerance))
      {
        BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener differencingCubicFlattener = new BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener(controlPoints, errorTolerance, errorTolerance, false);
        Point p = new Point();
        while (differencingCubicFlattener.Next(ref p))
          resultPolyline.Add(p);
      }
      else
      {
        double x = controlPoints[3].X - controlPoints[2].X + controlPoints[1].X - controlPoints[0].X;
        double y = controlPoints[3].Y - controlPoints[2].Y + controlPoints[1].Y - controlPoints[0].Y;
        double num = 1.0 / errorTolerance;
        uint depth = BezierCurveFlattener.Log8UnsignedInt32((uint) (FloatingPointArithmetic.Hypotenuse(x, y) * num + 0.5));
        if (depth > 0U)
          --depth;
        if (depth > 0U)
          BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints, depth, 0.75 * num, resultPolyline);
        else
          BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints, 0.75 * num, resultPolyline);
      }
      resultPolyline.Add(controlPoints[3]);
    }

    public static void FlattenQuadratic(Point[] controlPoints, double errorTolerance, List<Point> resultPolyline, bool connect)
    {
      BezierCurveFlattener.FlattenCubic(new Point[4]
      {
        controlPoints[0],
        VectorUtilities.WeightedAverage(controlPoints[0], controlPoints[1], 2.0 / 3.0),
        VectorUtilities.WeightedAverage(controlPoints[1], controlPoints[2], 1.0 / 3.0),
        controlPoints[2]
      }, errorTolerance, resultPolyline, connect);
    }

    public static void FlattenCubic(Point[] controlPoints, double errorTolerance, List<Point> resultPolyline, bool connect, List<double> resultParameters)
    {
      if (!connect || resultPolyline.Count == 0)
      {
        resultPolyline.Add(controlPoints[0]);
        resultParameters.Add(0.0);
      }
      if (BezierCurveFlattener.IsCubicChordMonotone(controlPoints, errorTolerance * errorTolerance))
      {
        BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener differencingCubicFlattener = new BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener(controlPoints, errorTolerance, errorTolerance, true);
        Point p = new Point();
        double u = 0.0;
        while (differencingCubicFlattener.Next(ref p, ref u))
        {
          resultPolyline.Add(p);
          resultParameters.Add(u);
        }
      }
      else
      {
        double x = controlPoints[3].X - controlPoints[2].X + controlPoints[1].X - controlPoints[0].X;
        double y = controlPoints[3].Y - controlPoints[2].Y + controlPoints[1].Y - controlPoints[0].Y;
        double num = 1.0 / errorTolerance;
        uint depth = BezierCurveFlattener.Log8UnsignedInt32((uint) (FloatingPointArithmetic.Hypotenuse(x, y) * num + 0.5));
        if (depth > 0U)
          --depth;
        if (depth > 0U)
          BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints, depth, 0.0, 1.0, 0.75 * num, resultPolyline, resultParameters);
        else
          BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints, 0.0, 1.0, 0.75 * num, resultPolyline, resultParameters);
      }
      resultPolyline.Add(controlPoints[3]);
      resultParameters.Add(1.0);
    }

    public static void FlattenQuadratic(Point[] controlPoints, double errorTolerance, List<Point> resultPolyline, bool connect, List<double> resultParameters)
    {
      BezierCurveFlattener.FlattenCubic(new Point[4]
      {
        controlPoints[0],
        VectorUtilities.WeightedAverage(controlPoints[0], controlPoints[1], 2.0 / 3.0),
        VectorUtilities.WeightedAverage(controlPoints[1], controlPoints[2], 1.0 / 3.0),
        controlPoints[2]
      }, errorTolerance, resultPolyline, connect, resultParameters);
    }

    public static bool CompareAlgorithmsOnChordMonotoneCubic(Point[] controlPoints, double errorTolerance)
    {
      if (!BezierCurveFlattener.IsCubicChordMonotone(controlPoints, errorTolerance * errorTolerance))
        return false;
      List<Point> list1 = new List<Point>(16);
      List<Point> list2 = new List<Point>(16);
      list1.Add(controlPoints[0]);
      list2.Add(controlPoints[0]);
      double x = controlPoints[3].X - controlPoints[2].X + controlPoints[1].X - controlPoints[0].X;
      double y = controlPoints[3].Y - controlPoints[2].Y + controlPoints[1].Y - controlPoints[0].Y;
      double num = 1.0 / errorTolerance;
      uint depth = BezierCurveFlattener.Log8UnsignedInt32((uint) (FloatingPointArithmetic.Hypotenuse(x, y) * num + 0.5));
      if (depth > 0U)
        --depth;
      if (depth > 0U)
        BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints, depth, 0.75 * num, list1);
      else
        BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints, 0.75 * num, list1);
      BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener differencingCubicFlattener = new BezierCurveFlattener.AdaptiveForwardDifferencingCubicFlattener(controlPoints, errorTolerance, errorTolerance, false);
      Point p = new Point();
      while (differencingCubicFlattener.Next(ref p))
        list2.Add(p);
      list1.Add(controlPoints[3]);
      list2.Add(controlPoints[3]);
      double[] cumulatedChordLength1 = VectorUtilities.GetCumulatedChordLength(list1, 0, list1.Count - 1);
      double[] cumulatedChordLength2 = VectorUtilities.GetCumulatedChordLength(list2, 0, list2.Count - 1);
      int firstBadVertexInQ = 0;
      return VectorUtilities.ArePolylinesClose(list1, cumulatedChordLength1, 0, list1.Count - 1, list2, cumulatedChordLength2, 0, list2.Count - 1, errorTolerance, ref firstBadVertexInQ);
    }

    private static uint Log8UnsignedInt32(uint i)
    {
      uint num = 0U;
      while (i > 0U)
      {
        i >>= 3;
        ++num;
      }
      return num;
    }

    private static uint Log4UnsignedInt32(uint i)
    {
      uint num = 0U;
      while (i > 0U)
      {
        i >>= 2;
        ++num;
      }
      return num;
    }

    private static uint Log4Double(double d)
    {
      uint num = 0U;
      while (d > 1.0)
      {
        d *= 0.25;
        ++num;
      }
      return num;
    }

    private static void DoCubicMidpointSubdivision(Point[] controlPoints, uint depth, double inverseErrorTolerance, List<Point> resultPolyline)
    {
      Point[] controlPoints1 = new Point[4]
      {
        controlPoints[0],
        controlPoints[1],
        controlPoints[2],
        controlPoints[3]
      };
      Point[] controlPoints2 = new Point[4];
      controlPoints2[3] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints1[2] = VectorUtilities.Midpoint(controlPoints1[2], controlPoints1[1]);
      controlPoints1[1] = VectorUtilities.Midpoint(controlPoints1[1], controlPoints1[0]);
      controlPoints2[2] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints1[2] = VectorUtilities.Midpoint(controlPoints1[2], controlPoints1[1]);
      controlPoints2[1] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints2[0] = controlPoints1[3];
      --depth;
      if (depth > 0U)
      {
        BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints1, depth, inverseErrorTolerance, resultPolyline);
        resultPolyline.Add(controlPoints2[0]);
        BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints2, depth, inverseErrorTolerance, resultPolyline);
      }
      else
      {
        BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints1, inverseErrorTolerance, resultPolyline);
        resultPolyline.Add(controlPoints2[0]);
        BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints2, inverseErrorTolerance, resultPolyline);
      }
    }

    private static void DoCubicMidpointSubdivision(Point[] controlPoints, uint depth, double leftParameter, double rightParameter, double inverseErrorTolerance, List<Point> resultPolyline, List<double> resultParameters)
    {
      Point[] controlPoints1 = new Point[4]
      {
        controlPoints[0],
        controlPoints[1],
        controlPoints[2],
        controlPoints[3]
      };
      Point[] controlPoints2 = new Point[4];
      controlPoints2[3] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints1[2] = VectorUtilities.Midpoint(controlPoints1[2], controlPoints1[1]);
      controlPoints1[1] = VectorUtilities.Midpoint(controlPoints1[1], controlPoints1[0]);
      controlPoints2[2] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints1[2] = VectorUtilities.Midpoint(controlPoints1[2], controlPoints1[1]);
      controlPoints2[1] = controlPoints1[3];
      controlPoints1[3] = VectorUtilities.Midpoint(controlPoints1[3], controlPoints1[2]);
      controlPoints2[0] = controlPoints1[3];
      --depth;
      double num = (leftParameter + rightParameter) * 0.5;
      if (depth > 0U)
      {
        BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints1, depth, leftParameter, num, inverseErrorTolerance, resultPolyline, resultParameters);
        resultPolyline.Add(controlPoints2[0]);
        resultParameters.Add(num);
        BezierCurveFlattener.DoCubicMidpointSubdivision(controlPoints2, depth, num, rightParameter, inverseErrorTolerance, resultPolyline, resultParameters);
      }
      else
      {
        BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints1, leftParameter, num, inverseErrorTolerance, resultPolyline, resultParameters);
        resultPolyline.Add(controlPoints2[0]);
        resultParameters.Add(num);
        BezierCurveFlattener.DoCubicForwardDifferencing(controlPoints2, num, rightParameter, inverseErrorTolerance, resultPolyline, resultParameters);
      }
    }

    private static void DoCubicForwardDifferencing(Point[] controlPoints, double inverseErrorTolerance, List<Point> resultPolyline)
    {
      double num1 = controlPoints[1].X - controlPoints[0].X;
      double num2 = controlPoints[1].Y - controlPoints[0].Y;
      double num3 = controlPoints[2].X - controlPoints[1].X;
      double num4 = controlPoints[2].Y - controlPoints[1].Y;
      double num5 = controlPoints[3].X - controlPoints[2].X;
      double num6 = controlPoints[3].Y - controlPoints[2].Y;
      double num7 = num3 - num1;
      double num8 = num4 - num2;
      double num9 = num5 - num3;
      double num10 = num6 - num4;
      double num11 = num9 - num7;
      double num12 = num10 - num8;
      Vector vector = controlPoints[3] - controlPoints[0];
      double length = vector.Length;
      double num13 = length < FloatingPointArithmetic.DistanceTolerance ? Math.Max(0.0, Math.Max(VectorUtilities.Distance(controlPoints[1], controlPoints[0]), VectorUtilities.Distance(controlPoints[2], controlPoints[0]))) : Math.Max(0.0, Math.Max(Math.Abs((num7 * vector.Y - num8 * vector.X) / length), Math.Abs((num9 * vector.Y - num10 * vector.X) / length)));
      uint num14 = 0U;
      if (num13 > 0.0)
      {
        double d = num13 * inverseErrorTolerance;
        num14 = d < (double) int.MaxValue ? BezierCurveFlattener.Log4UnsignedInt32((uint) (d + 0.5)) : BezierCurveFlattener.Log4Double(d);
      }
      int exp1 = -(int) num14;
      int exp2 = exp1 + exp1;
      int exp3 = exp2 + exp1;
      double num15 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num7, exp2);
      double num16 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num8, exp2);
      double num17 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(6.0 * num11, exp3);
      double num18 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(6.0 * num12, exp3);
      double num19 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num1, exp1) + num15 + 1.0 / 6.0 * num17;
      double num20 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num2, exp1) + num16 + 1.0 / 6.0 * num18;
      double num21 = 2.0 * num15 + num17;
      double num22 = 2.0 * num16 + num18;
      double x = controlPoints[0].X;
      double y = controlPoints[0].Y;
      Point point = new Point(0.0, 0.0);
      int num23 = 1 << (int) num14;
      for (int index = 1; index < num23; ++index)
      {
        x += num19;
        y += num20;
        point.X = x;
        point.Y = y;
        resultPolyline.Add(point);
        num19 += num21;
        num20 += num22;
        num21 += num17;
        num22 += num18;
      }
    }

    private static void DoCubicForwardDifferencing(Point[] controlPoints, double leftParameter, double rightParameter, double inverseErrorTolerance, List<Point> resultPolyline, List<double> resultParameters)
    {
      double num1 = controlPoints[1].X - controlPoints[0].X;
      double num2 = controlPoints[1].Y - controlPoints[0].Y;
      double num3 = controlPoints[2].X - controlPoints[1].X;
      double num4 = controlPoints[2].Y - controlPoints[1].Y;
      double num5 = controlPoints[3].X - controlPoints[2].X;
      double num6 = controlPoints[3].Y - controlPoints[2].Y;
      double num7 = num3 - num1;
      double num8 = num4 - num2;
      double num9 = num5 - num3;
      double num10 = num6 - num4;
      double num11 = num9 - num7;
      double num12 = num10 - num8;
      Vector vector = controlPoints[3] - controlPoints[0];
      double length = vector.Length;
      double num13 = length < FloatingPointArithmetic.DistanceTolerance ? Math.Max(0.0, Math.Max(VectorUtilities.Distance(controlPoints[1], controlPoints[0]), VectorUtilities.Distance(controlPoints[2], controlPoints[0]))) : Math.Max(0.0, Math.Max(Math.Abs((num7 * vector.Y - num8 * vector.X) / length), Math.Abs((num9 * vector.Y - num10 * vector.X) / length)));
      uint num14 = 0U;
      if (num13 > 0.0)
      {
        double d = num13 * inverseErrorTolerance;
        num14 = d < (double) int.MaxValue ? BezierCurveFlattener.Log4UnsignedInt32((uint) (d + 0.5)) : BezierCurveFlattener.Log4Double(d);
      }
      int exp1 = -(int) num14;
      int exp2 = exp1 + exp1;
      int exp3 = exp2 + exp1;
      double num15 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num7, exp2);
      double num16 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num8, exp2);
      double num17 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(6.0 * num11, exp3);
      double num18 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(6.0 * num12, exp3);
      double num19 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num1, exp1) + num15 + 1.0 / 6.0 * num17;
      double num20 = FloatingPointArithmetic.DoubleFromMantissaAndExponent(3.0 * num2, exp1) + num16 + 1.0 / 6.0 * num18;
      double num21 = 2.0 * num15 + num17;
      double num22 = 2.0 * num16 + num18;
      double x = controlPoints[0].X;
      double y = controlPoints[0].Y;
      Point point = new Point(0.0, 0.0);
      int num23 = 1 << (int) num14;
      double num24 = num23 > 0 ? (rightParameter - leftParameter) / (double) num23 : 0.0;
      double num25 = leftParameter;
      for (int index = 1; index < num23; ++index)
      {
        x += num19;
        y += num20;
        point.X = x;
        point.Y = y;
        resultPolyline.Add(point);
        num25 += num24;
        resultParameters.Add(num25);
        num19 += num21;
        num20 += num22;
        num21 += num17;
        num22 += num18;
      }
    }

    private static bool IsCubicChordMonotone(Point[] controlPoints, double squaredTolerance)
    {
      double num1 = VectorUtilities.SquaredDistance(controlPoints[0], controlPoints[3]);
      if (num1 <= squaredTolerance)
        return false;
      Vector a = controlPoints[3] - controlPoints[0];
      Vector b1 = controlPoints[1] - controlPoints[0];
      double num2 = VectorUtilities.Dot(a, b1);
      if (num2 < 0.0 || num2 > num1)
        return false;
      Vector b2 = controlPoints[2] - controlPoints[0];
      double num3 = VectorUtilities.Dot(a, b2);
      return num3 >= 0.0 && num3 <= num1 && num2 <= num3;
    }

    private class AdaptiveForwardDifferencingCubicFlattener
    {
      private int numSteps = 1;
      private double dParameter = 1.0;
      private double aX;
      private double aY;
      private double bX;
      private double bY;
      private double cX;
      private double cY;
      private double dX;
      private double dY;
      private double flatnessTolerance;
      private double distanceTolerance;
      private bool doParameters;
      private double parameter;

      internal AdaptiveForwardDifferencingCubicFlattener(Point[] controlPoints, double flatnessTolerance, double distanceTolerance, bool doParameters)
      {
        this.flatnessTolerance = 3.0 * flatnessTolerance;
        this.distanceTolerance = distanceTolerance;
        this.doParameters = doParameters;
        this.aX = -controlPoints[0].X + 3.0 * (controlPoints[1].X - controlPoints[2].X) + controlPoints[3].X;
        this.aY = -controlPoints[0].Y + 3.0 * (controlPoints[1].Y - controlPoints[2].Y) + controlPoints[3].Y;
        this.bX = 3.0 * (controlPoints[0].X - 2.0 * controlPoints[1].X + controlPoints[2].X);
        this.bY = 3.0 * (controlPoints[0].Y - 2.0 * controlPoints[1].Y + controlPoints[2].Y);
        this.cX = 3.0 * (-controlPoints[0].X + controlPoints[1].X);
        this.cY = 3.0 * (-controlPoints[0].Y + controlPoints[1].Y);
        this.dX = controlPoints[0].X;
        this.dY = controlPoints[0].Y;
      }

      private AdaptiveForwardDifferencingCubicFlattener()
      {
      }

      internal bool Next(ref Point p)
      {
        while (this.MustSubdivide(this.flatnessTolerance))
          this.HalveStepSize();
        if ((this.numSteps & 1) == 0)
        {
          while (this.numSteps > 1 && !this.MustSubdivide(this.flatnessTolerance * 0.25))
            this.DoubleStepSize();
        }
        this.IncrementDifferences();
        p.X = this.dX;
        p.Y = this.dY;
        return this.numSteps != 0;
      }

      internal bool Next(ref Point p, ref double u)
      {
        while (this.MustSubdivide(this.flatnessTolerance))
          this.HalveStepSize();
        if ((this.numSteps & 1) == 0)
        {
          while (this.numSteps > 1 && !this.MustSubdivide(this.flatnessTolerance * 0.25))
            this.DoubleStepSize();
        }
        this.IncrementDifferencesAndParameter();
        p.X = this.dX;
        p.Y = this.dY;
        u = this.parameter;
        return this.numSteps != 0;
      }

      private void DoubleStepSize()
      {
        this.aX *= 8.0;
        this.aY *= 8.0;
        this.bX *= 4.0;
        this.bY *= 4.0;
        this.cX = this.cX + this.cX;
        this.cY = this.cY + this.cY;
        if (this.doParameters)
          this.dParameter *= 2.0;
        this.numSteps >>= 1;
      }

      private void HalveStepSize()
      {
        this.aX *= 0.125;
        this.aY *= 0.125;
        this.bX *= 0.25;
        this.bY *= 0.25;
        this.cX *= 0.5;
        this.cY *= 0.5;
        if (this.doParameters)
          this.dParameter *= 0.5;
        this.numSteps <<= 1;
      }

      private void IncrementDifferences()
      {
        this.dX = this.aX + this.bX + this.cX + this.dX;
        this.dY = this.aY + this.bY + this.cY + this.dY;
        this.cX = this.aX + this.aX + this.aX + this.bX + this.bX + this.cX;
        this.cY = this.aY + this.aY + this.aY + this.bY + this.bY + this.cY;
        this.bX = this.aX + this.aX + this.aX + this.bX;
        this.bY = this.aY + this.aY + this.aY + this.bY;
        --this.numSteps;
      }

      private void IncrementDifferencesAndParameter()
      {
        this.dX = this.aX + this.bX + this.cX + this.dX;
        this.dY = this.aY + this.bY + this.cY + this.dY;
        this.cX = this.aX + this.aX + this.aX + this.bX + this.bX + this.cX;
        this.cY = this.aY + this.aY + this.aY + this.bY + this.bY + this.cY;
        this.bX = this.aX + this.aX + this.aX + this.bX;
        this.bY = this.aY + this.aY + this.aY + this.bY;
        --this.numSteps;
        this.parameter += this.dParameter;
      }

      private bool MustSubdivide(double flatnessTolerance)
      {
        double num1 = -(this.aY + this.bY + this.cY);
        double num2 = this.aX + this.bX + this.cX;
        double num3 = Math.Abs(num1) + Math.Abs(num2);
        if (num3 <= this.distanceTolerance)
          return false;
        double num4 = num3 * flatnessTolerance;
        return Math.Abs(this.cX * num1 + this.cY * num2) > num4 || Math.Abs((this.bX + this.cX + this.cX) * num1 + (this.bY + this.cY + this.cY) * num2) > num4;
      }
    }
  }
}
