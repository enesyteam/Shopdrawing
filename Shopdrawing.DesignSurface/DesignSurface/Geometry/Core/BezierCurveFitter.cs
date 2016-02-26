// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.BezierCurveFitter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class BezierCurveFitter
  {
    private bool enforceEndTangent = true;
    private double distanceTolerance;
    private List<Point> sample;
    private double[] chordLength;
    private PathFigure figure;
    private bool enforceStartTangent;
    private Vector startTangent;
    private Vector endTangent;
    private static int[][] pascalTriangle;

    static BezierCurveFitter()
    {
      int[][] numArray1 = new int[4][];
      int[][] numArray2 = numArray1;
      int index = 0;
      int[] numArray3 = new int[4];
      numArray3[0] = 1;
      int[] numArray4 = numArray3;
      numArray2[index] = numArray4;
      numArray1[1] = new int[4]
      {
        1,
        1,
        0,
        0
      };
      numArray1[2] = new int[4]
      {
        1,
        2,
        1,
        0
      };
      numArray1[3] = new int[4]
      {
        1,
        3,
        3,
        1
      };
      BezierCurveFitter.pascalTriangle = numArray1;
    }

    public PathGeometry OpenFit(List<Point> input, bool inputMayContainRepeats, double cornerThreshold, double distanceTolerance, bool onlyCubics)
    {
      return this.OpenFit(input, inputMayContainRepeats, cornerThreshold, distanceTolerance, onlyCubics, false, new Vector(0.0, 0.0), false, new Vector(0.0, 0.0));
    }

    public PathGeometry OpenFit(List<Point> input, bool inputMayContainRepeats, double cornerThreshold, double distanceTolerance, bool onlyCubics, bool enforceStartTangent, Vector startTangent, bool enforceEndTangent, Vector endTangent)
    {
      if (cornerThreshold <= 0.0 || cornerThreshold >= Math.PI)
        throw new ArgumentOutOfRangeException("cornerThreshold", (object) cornerThreshold, "Corner threshold must be strictly between zero and Pi.");
      if (distanceTolerance <= 0.0)
        throw new ArgumentOutOfRangeException("distanceTolerance", (object) distanceTolerance, "Distance tolerance must be strictly greater than zero.");
      this.enforceStartTangent = enforceStartTangent;
      this.enforceEndTangent = enforceEndTangent;
      if (this.enforceStartTangent && startTangent.LengthSquared < FloatingPointArithmetic.SquaredDistanceTolerance && (this.enforceEndTangent && endTangent.LengthSquared < FloatingPointArithmetic.SquaredDistanceTolerance))
        throw new ArgumentException(ExceptionStringTable.CannotEnforceZeroLengthTangents);
      if (this.enforceEndTangent)
      {
        endTangent.Normalize();
        this.endTangent = endTangent;
      }
      if (this.enforceStartTangent)
      {
        startTangent.Normalize();
        this.startTangent = startTangent;
      }
      this.sample = input;
      if (inputMayContainRepeats && input.Count > 1)
      {
        this.sample = new List<Point>(input.Count);
        this.sample.Add(input[0]);
        for (int index = 1; index < input.Count; ++index)
        {
          if (!VectorUtilities.ArePathPointsVeryClose(input[index], input[index - 1]))
            this.sample.Add(input[index]);
        }
      }
      PathGeometry path = new PathGeometry();
      PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(path);
      if (this.sample.Count > 0)
      {
        pathGeometryEditor.StartFigure(this.sample[0]);
        this.figure = path.Figures[0];
      }
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.figure);
      if (this.sample.Count == 2)
      {
        if (VectorUtilities.Distance(this.sample[0], this.sample[1]) >= distanceTolerance)
        {
          if (onlyCubics)
            pathFigureEditor.LinearCubicCurveTo(this.sample[1]);
          else
            pathFigureEditor.LineTo(this.sample[1]);
        }
      }
      else if (this.sample.Count > 2)
      {
        int lastIndex = this.sample.Count - 1;
        this.chordLength = VectorUtilities.GetCumulatedChordLength(this.sample, 0, lastIndex);
        this.distanceTolerance = distanceTolerance;
        double cosThreshold = Math.Cos(cornerThreshold);
        int num = 0;
        int index = 1;
        while (true)
        {
          while (index >= lastIndex || this.IsSamplePointACorner(index, cosThreshold))
          {
            if (index == num + 1)
            {
              if (onlyCubics)
                pathFigureEditor.LinearCubicCurveTo(this.sample[index]);
              else
                pathFigureEditor.LineTo(this.sample[index]);
            }
            else
              this.OpenFit2DFromTo(num, this.GetUnitTangentVectorFirst(num), index, this.GetUnitTangentVectorLast(index), onlyCubics);
            num = index;
            ++index;
            if (num >= lastIndex)
              goto label_33;
          }
          ++index;
        }
      }
label_33:
      return path;
    }

    public PathGeometry ClosedFit(List<Point> input, bool inputMayContainRepeats, double cornerThreshold, double distanceTolerance, bool onlyCubics)
    {
      if (input.Count < 3)
        throw new ArgumentException(ExceptionStringTable.InputCollectionMustContainAtLeastThreePoints, "input");
      PathGeometry path = this.OpenFit(input, inputMayContainRepeats, cornerThreshold, distanceTolerance, onlyCubics);
      PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(path);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(path.Figures[0]);
      if (onlyCubics)
      {
        pathFigureEditor.CloseWithCubicBezier();
        this.figure = path.Figures[0];
        this.SetupCollinearHandlesConstraint(0, false);
      }
      else
        pathFigureEditor.CloseWithLineSegment();
      return path;
    }

    private void OpenFit2DFromTo(int first, Vector unitTangentFirst, int last, Vector unitTangentLast, bool onlyCubics)
    {
      int length = last - first + 1;
      int num1 = length - 1;
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.figure);
      if (length == 2)
      {
        if (onlyCubics)
        {
          double num2 = VectorUtilities.Distance(this.sample[first], this.sample[last]) / 3.0;
          Point p1 = this.sample[first] + unitTangentFirst * num2;
          Point p2 = this.sample[last] - unitTangentLast * num2;
          pathFigureEditor.CubicCurveTo(p1, p2, this.sample[last]);
        }
        else
          pathFigureEditor.LineTo(this.sample[last]);
      }
      else if (length == 3)
      {
        int index1 = first + 1;
        Vector vector1 = this.sample[first] - this.sample[index1];
        Vector vector2 = this.sample[last] - this.sample[index1];
        Vector vector3 = vector1;
        vector3.Normalize();
        Vector vector4 = vector2;
        vector4.Normalize();
        Vector vector5 = vector3 + vector4;
        Vector vector6;
        if (VectorUtilities.IsZero(vector5))
        {
          vector6 = this.sample[last] - this.sample[first];
          vector6.Normalize();
        }
        else
          vector6 = VectorUtilities.UnitNormal(vector5);
        if (VectorUtilities.Dot(vector6, this.sample[last] - this.sample[first]) < 0.0)
          vector6 *= -1.0;
        this.OpenFit2DFromTo(first, unitTangentFirst, index1, vector6, onlyCubics);
        int index2 = PathFigureUtilities.PointCount(this.figure) - 1;
        this.OpenFit2DFromTo(index1, vector6, last, unitTangentLast, onlyCubics);
        this.SetupCollinearHandlesConstraint(index2, onlyCubics);
      }
      else
      {
        double[][] numArray1 = new double[length][];
        for (int index = 0; index < length; ++index)
          numArray1[index] = new double[4];
        double num2 = 1.0 / (this.chordLength[last] - this.chordLength[first]);
        double[] numArray2 = new double[length];
        for (int index = 0; index <= num1; ++index)
          numArray2[index] = (this.chordLength[first + index] - this.chordLength[first]) * num2;
        double[] numArray3 = new double[4];
        numArray3[0] = 1.0;
        for (int index1 = 0; index1 <= num1; ++index1)
        {
          numArray3[1] = 1.0 - numArray2[index1];
          for (int index2 = 2; index2 <= 3; ++index2)
            numArray3[index2] = numArray3[index2 - 1] * numArray3[1];
          numArray1[index1][0] = numArray3[3];
          double num3 = numArray2[index1];
          int index3 = 1;
          while (index3 <= 3)
          {
            numArray1[index1][index3] = (double) BezierCurveFitter.pascalTriangle[3][index3] * num3 * numArray3[3 - index3];
            ++index3;
            num3 *= numArray2[index1];
          }
        }
        double[][] numArray4 = new double[4][];
        for (int index = 0; index < 4; ++index)
          numArray4[index] = new double[4];
        for (int index1 = 0; index1 <= 3; ++index1)
        {
          for (int index2 = 0; index2 <= index1; ++index2)
          {
            for (int index3 = 0; index3 <= num1; ++index3)
              numArray4[index1][index2] += numArray1[index3][index2] * numArray1[index3][index1];
            if (index1 != index2)
              numArray4[index2][index1] = numArray4[index1][index2];
          }
        }
        double[][] m = new double[2][]
        {
          new double[2]
          {
            numArray4[1][1],
            numArray4[1][2] * VectorUtilities.Dot(unitTangentFirst, unitTangentLast)
          },
          new double[2]
          {
            numArray4[1][2],
            numArray4[2][2]
          }
        };
        double[] v = new double[2];
        Vector[] vectorArray = new Vector[4];
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 <= num1; ++index2)
          {
            vectorArray[index1].X += numArray1[index2][index1] * this.sample[index2 + first].X;
            vectorArray[index1].Y += numArray1[index2][index1] * this.sample[index2 + first].Y;
          }
        }
        Vector vector1 = new Vector(this.sample[first].X, this.sample[first].Y);
        Vector vector2 = new Vector(this.sample[last].X, this.sample[last].Y);
        Vector b1 = (numArray4[1][0] + numArray4[1][1]) * vector1 + (numArray4[1][2] + numArray4[1][3]) * vector2 - vectorArray[1];
        v[0] = -VectorUtilities.Dot(unitTangentFirst, b1);
        Vector b2 = (numArray4[2][0] + numArray4[2][1]) * vector1 + (numArray4[2][2] + numArray4[2][3]) * vector2 - vectorArray[2];
        v[1] = -VectorUtilities.Dot(unitTangentLast, b2);
        bool flag = BezierCurveFitter.Solve2By2LinearSystem(m, v);
        int firstBadVertexInQ = 0;
        if (flag && v[0] > 0.0 && v[1] < 0.0)
        {
          Point[] controlPoints = new Point[4];
          controlPoints[0] = this.sample[first];
          controlPoints[1] = controlPoints[0] + v[0] * unitTangentFirst;
          controlPoints[3] = this.sample[last];
          controlPoints[2] = controlPoints[3] + v[1] * unitTangentLast;
          List<Point> list = new List<Point>(128);
          BezierCurveFlattener.FlattenCubic(controlPoints, this.distanceTolerance, list, false);
          double[] cumulatedChordLength = VectorUtilities.GetCumulatedChordLength(list, 0, list.Count - 1);
          if (VectorUtilities.ArePolylinesClose(list, cumulatedChordLength, 0, list.Count - 1, this.sample, this.chordLength, first, last, this.distanceTolerance, ref firstBadVertexInQ))
          {
            pathFigureEditor.CubicCurveTo(controlPoints[1], controlPoints[2], controlPoints[3]);
            return;
          }
        }
        int num4 = (first + last) / 2;
        Vector tangentVectorAtSplit = this.GetUnitTangentVectorAtSplit(num4);
        this.OpenFit2DFromTo(first, unitTangentFirst, num4, tangentVectorAtSplit, onlyCubics);
        int index4 = PathFigureUtilities.PointCount(this.figure) - 1;
        this.OpenFit2DFromTo(num4, tangentVectorAtSplit, last, unitTangentLast, onlyCubics);
        this.SetupCollinearHandlesConstraint(index4, onlyCubics);
      }
    }

    private void SetupCollinearHandlesConstraint(int index, bool enforceConstraint)
    {
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.figure);
      if (!pathFigureEditor.HasDownstreamBezierHandleNeighbor(index) || !pathFigureEditor.HasUpstreamBezierHandleNeighbor(index))
        return;
      Point point = pathFigureEditor.GetPoint(index);
      Vector a = pathFigureEditor.GetPoint(index - 1) - point;
      Vector b = pathFigureEditor.GetPoint(index + 1) - point;
      if (!enforceConstraint || VectorUtilities.HaveOppositeDirections(a, b))
        return;
      double length1 = a.Length;
      double length2 = b.Length;
      if (length1 > length2)
      {
        a.Normalize();
        b = -a * length2;
        pathFigureEditor.SetPoint(index + 1, point + b);
      }
      else
      {
        b.Normalize();
        Vector vector = -b * length1;
        pathFigureEditor.SetPoint(index - 1, point - vector);
      }
    }

    private Vector GetUnitTangentVectorFirst(int index)
    {
      Vector vector1;
      if (index == 0 && this.enforceStartTangent)
      {
        vector1 = this.startTangent;
      }
      else
      {
        vector1 = this.sample[index + 1] - this.sample[index];
        double num = index == 0 ? 4.0 * this.distanceTolerance : this.distanceTolerance;
        for (int index1 = index + 2; index1 < this.sample.Count && (this.sample[index1] - this.sample[index]).Length < num; ++index1)
        {
          Vector vector2 = vector1;
          vector1 += this.sample[index1] - this.sample[index];
          if (vector1.Length < vector2.Length)
          {
            vector1 = vector2;
            break;
          }
        }
      }
      vector1.Normalize();
      return vector1;
    }

    private Vector GetUnitTangentVectorLast(int index)
    {
      Vector vector1;
      if (index == this.sample.Count - 1 && this.enforceEndTangent)
      {
        vector1 = this.endTangent;
      }
      else
      {
        vector1 = this.sample[index] - this.sample[index - 1];
        double num = index == this.sample.Count - 1 ? 4.0 * this.distanceTolerance : this.distanceTolerance;
        for (int index1 = index - 2; index1 >= 0 && (this.sample[index] - this.sample[index1]).Length < num; --index1)
        {
          Vector vector2 = vector1;
          vector1 += this.sample[index] - this.sample[index1];
          if (vector1.Length < vector2.Length)
          {
            vector1 = vector2;
            break;
          }
        }
      }
      vector1.Normalize();
      return vector1;
    }

    private Vector GetUnitTangentVectorAtSplit(int index)
    {
      Vector vector1 = this.sample[index + 1] - this.sample[index - 1];
      double num = 2.0 * this.distanceTolerance;
      int index1 = index + 2;
      for (int index2 = index - 2; index1 < this.sample.Count && index2 >= 0 && (this.sample[index1] - this.sample[index2]).Length < num; --index2)
      {
        Vector vector2 = vector1;
        vector1 += this.sample[index1] - this.sample[index2];
        if (vector1.Length < vector2.Length)
        {
          vector1 = vector2;
          break;
        }
        ++index1;
      }
      vector1.Normalize();
      return vector1;
    }

    private static bool Solve2By2LinearSystem(double[][] m, double[] v)
    {
      double k = m[0][0] * m[1][1] - m[0][1] * m[1][0];
      if (FloatingPointArithmetic.IsVerySmall(k))
        return false;
      double num1 = v[0];
      double num2 = v[1];
      v[0] = (num1 * m[1][1] - m[0][1] * num2) / k;
      v[1] = (m[0][0] * num2 - num1 * m[1][0]) / k;
      return true;
    }

    private bool IsSamplePointACorner(int i, double cosThreshold)
    {
      Vector a = this.sample[i] - this.sample[i - 1];
      Vector b = this.sample[i + 1] - this.sample[i];
      double num = VectorUtilities.Dot(a, b);
      if (num >= 0.0)
      {
        if (cosThreshold <= 0.0)
          return false;
        return num * num < cosThreshold * cosThreshold * a.LengthSquared * b.LengthSquared;
      }
      if (cosThreshold >= 0.0)
        return true;
      return num * num > cosThreshold * cosThreshold * a.LengthSquared * b.LengthSquared;
    }
  }
}
