// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.IncrementalFitter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class IncrementalFitter
  {
    private IncrementalFitter.IncrementalFittingData current = new IncrementalFitter.IncrementalFittingData();
    private PathGeometry path = new PathGeometry();
    private double curveTolerance = 5.0;
    private double cornerTolerance = 90.0;

    public PathGeometry Path
    {
      get
      {
        PathGeometryEditor pathEditor = new PathGeometryEditor(this.path.Clone());
        this.FitSegments(pathEditor, this.current);
        return pathEditor.PathGeometry;
      }
      set
      {
        this.path = value.Clone();
        this.current = new IncrementalFitter.IncrementalFittingData();
      }
    }

    public double CurveTolerance
    {
      get
      {
        return this.curveTolerance;
      }
      set
      {
        this.curveTolerance = value;
      }
    }

    public double CornerTolerance
    {
      get
      {
        return this.cornerTolerance;
      }
      set
      {
        this.cornerTolerance = value;
      }
    }

    public double Length
    {
      get
      {
        if (this.path.Figures.Count != 1)
          return 0.0;
        List<Point> list = new PathFigureEditor(this.path.Figures[0]).Flatten(0.2);
        double num = 0.0;
        for (int index = 1; index < list.Count; ++index)
          num += VectorUtilities.Distance(list[index - 1], list[index]);
        return num;
      }
    }

    public void Clear()
    {
      this.current = new IncrementalFitter.IncrementalFittingData();
      new PathGeometryEditor(this.path).Clear();
    }

    public void Add(Point pt, long time)
    {
      if (this.current.points.Count > 0 && VectorUtilities.ArePathPointsVeryClose(this.current.points[this.current.points.Count - 1], pt))
        return;
      this.current.points.Add(pt);
      int count = this.current.points.Count;
      if (count >= 3)
      {
        this.current.endTangent.X = 0.0;
        this.current.endTangent.Y = 0.0;
        for (int index = Math.Max(0, count - 6); index < count - 1; ++index)
          this.current.endTangent += (this.current.points[count - 1] - this.current.points[index]) / (double) (count - 1 - Math.Max(0, count - 6));
        if (this.IsSamplePointACorner(this.current.points[count - 3], this.current.points[count - 2], this.current.points[count - 1], Math.Cos(2.0 * Math.PI * (this.cornerTolerance / 360.0))))
        {
          this.current.points.RemoveAt(this.current.points.Count - 1);
          this.current.endTangent = new Vector(0.0, 0.0);
          this.FitSegments(new PathGeometryEditor(this.path), this.current);
          this.current.cornerSegment = true;
          this.current.prevBezier = (IncrementalFitter.IncrementalFittingData) null;
          this.current.points.Clear();
          this.current.points.Add(pt);
        }
      }
      if (this.current.points.Count < 4)
        return;
      Point[] bezierFit1 = this.ComputeBezierFit(this.current);
      if (this.ComputeMaxError(this.current.points, bezierFit1) <= this.curveTolerance)
        return;
      if (this.current.prevBezier != null)
      {
        PathGeometryEditor pathGeometryEditor = new PathGeometryEditor(this.path);
        Point[] bezierFit2 = this.ComputeBezierFit(this.current.prevBezier);
        if (PathGeometryUtilities.IsEmpty(pathGeometryEditor.PathGeometry))
          pathGeometryEditor.StartFigure(bezierFit2[0]);
        pathGeometryEditor.AppendCubicBezier(bezierFit2[1], bezierFit2[2], bezierFit2[3]);
      }
      this.current.prevBezier = (IncrementalFitter.IncrementalFittingData) this.current.Clone();
      this.current.prevBezier.prevBezier = (IncrementalFitter.IncrementalFittingData) null;
      this.current.cornerSegment = false;
      this.current.startTangent = bezierFit1[2] - bezierFit1[3];
      this.current.startTangent.Normalize();
      this.current.points.Clear();
      this.current.points.Add(pt);
    }

    private void FitSegments(PathGeometryEditor pathEditor, IncrementalFitter.IncrementalFittingData data)
    {
      Point[] pointArray = (Point[]) null;
      if (data.points.Count > 3)
        pointArray = this.ComputeBezierFit(data);
      if (data.prevBezier != null)
      {
        Point[] bezierFit = this.ComputeBezierFit(data.prevBezier);
        if (PathGeometryUtilities.IsEmpty(pathEditor.PathGeometry))
          pathEditor.StartFigure(bezierFit[0]);
        pathEditor.AppendCubicBezier(bezierFit[1], bezierFit[2], bezierFit[3]);
      }
      if (data.points.Count == 2)
      {
        if (PathGeometryUtilities.IsEmpty(pathEditor.PathGeometry))
          pathEditor.StartFigure(data.points[0]);
        pathEditor.AppendLineSegment(data.points[1]);
      }
      else if (data.points.Count == 3)
      {
        if (PathGeometryUtilities.IsEmpty(pathEditor.PathGeometry))
          pathEditor.StartFigure(data.points[0]);
        pathEditor.AppendQuadraticBezier(data.points[1], data.points[2]);
      }
      else
      {
        if (data.points.Count <= 3)
          return;
        if (PathGeometryUtilities.IsEmpty(pathEditor.PathGeometry))
          pathEditor.StartFigure(pointArray[0]);
        pathEditor.AppendCubicBezier(pointArray[1], pointArray[2], pointArray[3]);
      }
    }

    private Point[] ComputeBezierFit(IncrementalFitter.IncrementalFittingData data)
    {
      if (data == null || data.points.Count < 4)
        return (Point[]) null;
      Point[] pointArray;
      if (data.cornerSegment)
        pointArray = data.endTangent.X != 0.0 || data.endTangent.Y != 0.0 ? this.ComputeFitWithEndTangent(data.points, data.endTangent) : this.ComputeInitialFit(data.points);
      else if (data.prevBezier != null)
      {
        Vector vector = new Vector(0.0, 0.0);
        int count = data.prevBezier.points.Count;
        if (count > 8)
        {
          for (int index1 = 0; index1 < 2; ++index1)
          {
            for (int index2 = Math.Max(0, count - 6); index2 < count - 1; ++index2)
              vector += (data.points[index1] - data.prevBezier.points[index2]) / (double) (count - 1 - Math.Max(0, count - 6) * 3);
          }
        }
        else
          vector = data.points[1] - data.prevBezier.points[data.prevBezier.points.Count - 1];
        if (vector.Length == 0.0)
          vector = data.points[1] - data.points[0];
        vector.Normalize();
        data.prevBezier.endTangent = vector;
        data.startTangent = vector;
        pointArray = data.endTangent.X != 0.0 || data.endTangent.Y != 0.0 ? this.ComputeFitWithTwoTangents(data.points, data.startTangent, data.endTangent) : this.ComputeFitWithStartTangent(data.points, data.startTangent);
      }
      else
        pointArray = data.endTangent.X != 0.0 || data.endTangent.Y != 0.0 ? this.ComputeFitWithTwoTangents(data.points, data.startTangent, data.endTangent) : this.ComputeFitWithStartTangent(data.points, data.startTangent);
      return pointArray;
    }

    private Point EvaluateBezier(Point[] bezier, double t)
    {
      double num1 = 1.0 - t;
      double num2 = num1 * num1 * num1;
      double num3 = 3.0 * num1 * num1 * t;
      double num4 = 3.0 * num1 * t * t;
      double num5 = t * t * t;
      return new Point()
      {
        X = bezier[0].X * num2 + bezier[1].X * num3 + bezier[2].X * num4 + bezier[3].X * num5,
        Y = bezier[0].Y * num2 + bezier[1].Y * num3 + bezier[2].Y * num4 + bezier[3].Y * num5
      };
    }

    private double[] ComputeParameterization(List<Point> points)
    {
      double[] numArray = new double[points.Count];
      numArray[0] = 0.0;
      for (int index = 1; index < points.Count; ++index)
        numArray[index] = numArray[index - 1] + VectorUtilities.Distance(points[index - 1], points[index]);
      double num = numArray[numArray.Length - 1];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] /= num;
      return numArray;
    }

    private double ComputeMaxError(List<Point> points, Point[] bezier)
    {
      double num1 = 0.0;
      if (bezier == null)
        return double.MaxValue;
      double[] parameterization = this.ComputeParameterization(points);
      for (int index = 0; index < points.Count; ++index)
      {
        Vector vector = this.EvaluateBezier(bezier, parameterization[index]) - points[index];
        double num2 = VectorUtilities.Dot(vector, vector);
        if (num2 >= num1)
          num1 = num2;
      }
      return num1;
    }

    private Point[] ComputeInitialFit(List<Point> points)
    {
      if (points.Count < 4)
        return new Point[4];
      double[] parameterization = this.ComputeParameterization(points);
      Point[] pointArray = new Point[4];
      pointArray[0] = points[0];
      pointArray[3] = points[points.Count - 1];
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      double num5 = 0.0;
      double num6 = 0.0;
      double num7 = 0.0;
      double num8 = 0.0;
      Point point1 = new Point(0.0, 0.0);
      Point point2 = new Point(0.0, 0.0);
      for (int index = 1; index < parameterization.Length - 1; ++index)
      {
        double num9 = 1.0 - parameterization[index];
        double num10 = num9 * num9 * num9;
        double num11 = 3.0 * num9 * num9 * parameterization[index];
        double num12 = 3.0 * num9 * parameterization[index] * parameterization[index];
        double num13 = parameterization[index] * parameterization[index] * parameterization[index];
        num1 += num11 * num11;
        num4 += num12 * num12;
        num2 += num11 * num12;
        num3 += num12 * num11;
        point1.X += points[index].X * num11;
        point1.Y += points[index].Y * num11;
        point2.X += points[index].X * num12;
        point2.Y += points[index].Y * num12;
        num5 += num10 * num11;
        num7 += num13 * num11;
        num6 += num10 * num12;
        num8 += num13 * num12;
      }
      point1.X = point1.X - pointArray[0].X * num5 - pointArray[3].X * num7;
      point1.Y = point1.Y - pointArray[0].Y * num5 - pointArray[3].Y * num7;
      point2.X = point2.X - pointArray[0].X * num6 - pointArray[3].X * num8;
      point2.Y = point2.Y - pointArray[0].Y * num6 - pointArray[3].Y * num8;
      double num14 = num1 * num4 - num2 * num2;
      pointArray[1].X = (point1.X * num4 - point2.X * num2) / num14;
      pointArray[1].Y = (point1.Y * num4 - point2.Y * num2) / num14;
      pointArray[2].X = (point2.X * num1 - point1.X * num2) / num14;
      pointArray[2].Y = (point2.Y * num1 - point1.Y * num2) / num14;
      return pointArray;
    }

    private Point[] ComputeFitWithStartTangent(List<Point> points, Vector tangent)
    {
      if (points.Count < 4)
        return new Point[4];
      double[] parameterization = this.ComputeParameterization(points);
      Point[] pointArray = new Point[4];
      pointArray[0] = points[0];
      pointArray[3] = points[points.Count - 1];
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      double num5 = 0.0;
      double num6 = 0.0;
      double num7 = 0.0;
      for (int index = 1; index < parameterization.Length - 1; ++index)
      {
        double num8 = 1.0 - parameterization[index];
        double num9 = num8 * num8 * num8;
        double num10 = 3.0 * num8 * num8 * parameterization[index];
        double num11 = 3.0 * num8 * parameterization[index] * parameterization[index];
        double num12 = parameterization[index] * parameterization[index] * parameterization[index];
        num1 += num10 * num10;
        num4 += num11 * num11;
        num2 += num10 * num11;
        num3 += num11 * num10;
        num5 += ((points[index].X - pointArray[0].X * (num9 + num10) - pointArray[3].X * num12) * tangent.X + (points[index].Y - pointArray[0].Y * (num9 + num10) - pointArray[3].Y * num12) * tangent.Y) * num10;
        num6 += (points[index].X - pointArray[0].X * (num9 + num10) - pointArray[3].X * num12) * num11;
        num7 += (points[index].Y - pointArray[0].Y * (num9 + num10) - pointArray[3].Y * num12) * num11;
      }
      double num13 = num1 * VectorUtilities.Dot(tangent, tangent);
      double num14 = num2 * tangent.X;
      double num15 = num3 * tangent.Y;
      double num16 = num13 - num15 * (num15 / num4) - num14 * (num14 / num4);
      double num17 = (num5 - num7 * (num15 / num4) - num6 * (num14 / num4)) / num16;
      double num18 = (num6 - num17 * num14) / num4;
      double num19 = (num7 - num17 * num15) / num4;
      pointArray[1].X = pointArray[0].X + tangent.X * num17;
      pointArray[1].Y = pointArray[0].Y + tangent.Y * num17;
      pointArray[2].X = num18;
      pointArray[2].Y = num19;
      return pointArray;
    }

    private Point[] ComputeFitWithEndTangent(List<Point> points, Vector tangent)
    {
      if (points.Count < 4)
        return new Point[4];
      double[] parameterization = this.ComputeParameterization(points);
      Point[] pointArray = new Point[4];
      pointArray[3] = points[0];
      pointArray[0] = points[points.Count - 1];
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      double num5 = 0.0;
      double num6 = 0.0;
      double num7 = 0.0;
      for (int index = 1; index < parameterization.Length - 1; ++index)
      {
        double num8 = 1.0 - parameterization[index];
        double num9 = num8 * num8 * num8;
        double num10 = 3.0 * num8 * num8 * parameterization[index];
        double num11 = 3.0 * num8 * parameterization[index] * parameterization[index];
        double num12 = parameterization[index] * parameterization[index] * parameterization[index];
        num1 += num11 * num11;
        num4 += num10 * num10;
        num2 += num11 * num10;
        num3 += num10 * num11;
        num5 += ((points[index].X - pointArray[0].X * (num12 + num11) - pointArray[3].X * num9) * tangent.X + (points[index].Y - pointArray[0].Y * (num12 + num11) - pointArray[3].Y * num9) * tangent.Y) * num11;
        num6 += (points[index].X - pointArray[0].X * (num12 + num11) - pointArray[3].X * num9) * num10;
        num7 += (points[index].Y - pointArray[0].Y * (num12 + num11) - pointArray[3].Y * num9) * num10;
      }
      double num13 = num1 * VectorUtilities.Dot(tangent, tangent);
      double num14 = num2 * tangent.X;
      double num15 = num3 * tangent.Y;
      double num16 = num13 - num15 * (num15 / num4) - num14 * (num14 / num4);
      double num17 = (num5 - num7 * (num15 / num4) - num6 * (num14 / num4)) / num16;
      double num18 = (num6 - num17 * num14) / num4;
      double num19 = (num7 - num17 * num15) / num4;
      pointArray[2].X = pointArray[0].X + tangent.X * num17;
      pointArray[2].Y = pointArray[0].Y + tangent.Y * num17;
      pointArray[1].X = num18;
      pointArray[1].Y = num19;
      pointArray[0] = points[0];
      pointArray[3] = points[points.Count - 1];
      return pointArray;
    }

    private Point[] ComputeFitWithTwoTangents(List<Point> points, Vector start, Vector end)
    {
      if (points.Count < 4)
        return new Point[4];
      double[] parameterization = this.ComputeParameterization(points);
      Point[] pointArray = new Point[4];
      pointArray[0] = points[0];
      pointArray[3] = points[points.Count - 1];
      double num1;
      double num2;
      if (Math.Abs(start.X) == Math.Abs(end.X) && Math.Abs(start.Y) == Math.Abs(end.Y))
      {
        num1 = 1.0 / 3.0;
        num2 = 2.0 / 3.0;
      }
      else
      {
        double num3 = 0.0;
        double num4 = 0.0;
        double num5 = 0.0;
        double num6 = 0.0;
        double num7 = 0.0;
        for (int index = 1; index < parameterization.Length - 1; ++index)
        {
          double num8 = 1.0 - parameterization[index];
          double num9 = num8 * num8 * num8;
          double num10 = 3.0 * num8 * num8 * parameterization[index];
          double num11 = 3.0 * num8 * parameterization[index] * parameterization[index];
          double num12 = parameterization[index] * parameterization[index] * parameterization[index];
          num3 += VectorUtilities.Dot(start, start) * num10 * num10;
          num4 += VectorUtilities.Dot(start, end) * num10 * num11;
          num5 += VectorUtilities.Dot(end, end) * num11 * num11;
          num6 += ((points[index].X - pointArray[0].X * (num9 + num10) - pointArray[3].X * (num11 + num12)) * start.X + (points[index].Y - pointArray[0].Y * (num9 + num10) - pointArray[3].Y * (num11 + num12)) * start.Y) * num10;
          num7 += ((points[index].X - pointArray[0].X * (num9 + num10) - pointArray[3].X * (num11 + num12)) * end.X + (points[index].Y - pointArray[0].Y * (num9 + num10) - pointArray[3].Y * (num11 + num12)) * end.Y) * num11;
        }
        double num13 = num3 * num5 - num4 * num4;
        num1 = (num6 * num5 - num7 * num4) / num13;
        num2 = (num7 * num3 - num6 * num4) / num13;
      }
      pointArray[1].X = pointArray[0].X + start.X * num1;
      pointArray[1].Y = pointArray[0].Y + start.Y * num1;
      pointArray[2].X = pointArray[3].X + end.X * num2;
      pointArray[2].Y = pointArray[3].Y + end.Y * num2;
      return pointArray;
    }

    private bool IsSamplePointACorner(Point start, Point corner, Point end, double cosThreshold)
    {
      Vector a = corner - start;
      Vector b = end - corner;
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

    private class IncrementalFittingData : ICloneable
    {
      public List<Point> points = new List<Point>();
      public bool cornerSegment = true;
      public List<double> timing = new List<double>();
      public Vector startTangent;
      public Vector endTangent;
      public IncrementalFitter.IncrementalFittingData prevBezier;

      public object Clone()
      {
        IncrementalFitter.IncrementalFittingData incrementalFittingData = new IncrementalFitter.IncrementalFittingData();
        incrementalFittingData.points = new List<Point>((IEnumerable<Point>) this.points);
        incrementalFittingData.startTangent = this.startTangent;
        incrementalFittingData.endTangent = this.endTangent;
        incrementalFittingData.cornerSegment = this.cornerSegment;
        if (this.prevBezier != null)
          incrementalFittingData.prevBezier = (IncrementalFitter.IncrementalFittingData) this.prevBezier.Clone();
        incrementalFittingData.timing = new List<double>((IEnumerable<double>) this.timing);
        return (object) incrementalFittingData;
      }
    }
  }
}
