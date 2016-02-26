// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathGeometryEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class PathGeometryEditor
  {
    private PathGeometry path;
    private PathDiffChangeList changeList;

    public virtual PathGeometry PathGeometry
    {
      get
      {
        return this.path;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.path = value;
      }
    }

    internal bool LastFigureIsOpen
    {
      get
      {
        if (this.path.Figures.Count == 0)
          return false;
        return PathFigureUtilities.IsOpen(this.path.Figures[this.path.Figures.Count - 1]);
      }
    }

    public PathDiffChangeList PathDiffChangeList
    {
      get
      {
        return this.changeList;
      }
    }

    public PathGeometryEditor(PathGeometry path)
      : this(path, new PathDiffChangeList())
    {
    }

    public PathGeometryEditor(PathGeometry path, PathDiffChangeList changeList)
    {
      if (path == null)
        throw new ArgumentNullException("path");
      if (path.IsFrozen)
        throw new ArgumentException(ExceptionStringTable.PathIsFrozen, "path");
      this.path = path;
      this.changeList = changeList;
    }

    public PathFigureEditor CreatePathFigureEditor(int figureIndex)
    {
      return new PathFigureEditor(this.path.Figures[figureIndex], this.PathDiffChangeList, figureIndex);
    }

    public void Clear()
    {
      this.path.Figures.Clear();
    }

    public void AppendFigure(PathFigure figure)
    {
      this.path.Figures.Add(figure);
    }

    public PathFigure StartFigure(Point p)
    {
      PathFigure pathFigure = new PathFigure();
      pathFigure.StartPoint = p;
      this.path.Figures.Add(pathFigure);
      return pathFigure;
    }

    public void AppendLineSegment(Point p)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(this.path.Figures.Count - 1);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.LastFigureMustBeOpenToAppendALineSegment);
      pathFigureEditor.LineTo(p);
    }

    public void AppendLineSegment(Point p, int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.FigureMustBeOpenToAppendALineSegment);
      pathFigureEditor.LineTo(p);
    }

    public void CloseFigureWithLineSegment()
    {
      this.CloseFigureWithLineSegment(this.path.Figures.Count - 1);
    }

    public void CloseFigureWithLineSegment(int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.FigureMustBeOpenToAppendALineSegment);
      pathFigureEditor.CloseWithLineSegment();
    }

    public void AppendQuadraticBezier(Point q, Point r)
    {
      this.AppendQuadraticBezier(q, r, this.path.Figures.Count - 1);
    }

    public void AppendQuadraticBezier(Point q, Point r, int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.LastFigureMustBeOpenToAppendAQuadraticCurve);
      pathFigureEditor.QuadraticCurveTo(q, r);
    }

    public void CloseFigureWithQuadraticBezier(Point q)
    {
      this.CloseFigureWithQuadraticBezier(q, this.path.Figures.Count - 1);
    }

    public void CloseFigureWithQuadraticBezier(Point q, int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.FigureMustBeOpenToAppendAQuadraticCurve);
      pathFigureEditor.QuadraticCurveToAndCloseFigure(q);
    }

    public void AppendCubicBezier(Point q, Point r, Point s)
    {
      this.AppendCubicBezier(q, r, s, this.path.Figures.Count - 1);
    }

    public void AppendCubicBezier(Point q, Point r, Point s, int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.FigureMustBeOpenToAppendACubicCurve);
      pathFigureEditor.CubicCurveTo(q, r, s);
    }

    public void CloseFigureWithCubicBezier(Point q, Point r)
    {
      this.CloseFigureWithCubicBezier(q, r, this.path.Figures.Count - 1);
    }

    public void CloseFigureWithCubicBezier(Point q, Point r, int figureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      if (!PathFigureUtilities.IsOpen(pathFigureEditor.PathFigure))
        throw new InvalidOperationException(ExceptionStringTable.FigureMustBeOpenToAppendACubicCurve);
      pathFigureEditor.CubicCurveToAndCloseFigure(q, r);
    }

    public bool IsFigureOpen(int figureIndex)
    {
      return PathFigureUtilities.IsOpen(this.path.Figures[figureIndex]);
    }

    public Point GetPoint(int figureIndex, int pointIndex)
    {
      return this.CreatePathFigureEditor(figureIndex).GetPoint(pointIndex);
    }

    public void OpenFigure(int figureIndex)
    {
      this.OpenFigure(figureIndex, 0);
    }

    public void OpenFigure(int figureIndex, int pointIndex)
    {
      this.CreatePathFigureEditor(figureIndex).Open(pointIndex);
    }

    public void CloseFigure(int figureIndex)
    {
      this.CreatePathFigureEditor(figureIndex).CloseWithLineSegment();
    }

    public void SplitFigure(int figureIndex, int pointIndex)
    {
      int count = this.PathDiffChangeList.Changes.Count;
      PathFigure pathFigure = this.CreatePathFigureEditor(figureIndex).Split(pointIndex);
      if (this.PathDiffChangeList.Changes.Count == count + 1 && this.PathDiffChangeList.Changes[count].Action == PathActionType.Split)
      {
        this.PathDiffChangeList.Changes[count].Action = PathActionType.SplitAndAdd;
        int num1 = 0;
        int num2 = -1;
        foreach (PathStructureChange pathStructureChange in this.PathDiffChangeList.Changes[count].PathStructureChanges)
        {
          if (num2 == -1 || pathStructureChange.OldSegmentIndex < num2)
            num2 = pathStructureChange.OldSegmentIndex;
          pathStructureChange.NewSegmentIndex = num1;
          pathStructureChange.OldFigureIndex = figureIndex;
          pathStructureChange.NewFigureIndex = this.path.Figures.Count;
          ++num1;
        }
        int oldIndex = num2 <= 0 ? PathStructureChange.StartPointIndex : num2 - 1;
        this.PathDiffChangeList.Changes[count].PathStructureChanges.Add(new PathStructureChange(oldIndex, PathStructureChange.StartPointIndex, figureIndex, this.path.Figures.Count));
      }
      if (pathFigure == null)
        return;
      this.path.Figures.Add(pathFigure);
    }

    public void JoinFigure(int figureIndex, PathFigure figure)
    {
      this.CreatePathFigureEditor(figureIndex).Join(figure);
    }

    public void JoinFigure(int figureIndex, int targetFigureIndex)
    {
      PathFigureEditor pathFigureEditor = this.CreatePathFigureEditor(figureIndex);
      PathFigure figure = this.PathGeometry.Figures[targetFigureIndex];
      int count1 = pathFigureEditor.PathFigure.Segments.Count;
      int num1 = figureIndex < targetFigureIndex ? 0 : -1;
      int count2 = this.changeList.Changes.Count;
      pathFigureEditor.Join(figure);
      while (this.changeList.Changes.Count > count2)
        this.changeList.Changes.RemoveAt(this.changeList.Changes.Count - 1);
      PathAction pathAction = new PathAction();
      pathAction.Action = PathActionType.Join;
      pathAction.Figure = figureIndex;
      pathAction.PointIndex = targetFigureIndex;
      pathAction.PathStructureChanges.Add(new PathStructureChange(count1 - 1, count1, figureIndex, figureIndex + num1, (DependencyProperty) null, BezierSegment.Point1Property, PathChangeType.Copy));
      pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, count1, targetFigureIndex, figureIndex + num1, PathFigure.StartPointProperty, BezierSegment.Point2Property, PathChangeType.Copy));
      pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, count1, targetFigureIndex, figureIndex + num1, PathFigure.StartPointProperty, BezierSegment.Point3Property, PathChangeType.Move));
      int num2 = count1 + 1;
      for (int oldIndex = 0; oldIndex < this.path.Figures[targetFigureIndex].Segments.Count; ++oldIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, num2 + oldIndex, targetFigureIndex, figureIndex + num1));
      this.path.Figures.RemoveAt(targetFigureIndex);
      for (int newFigure = targetFigureIndex; newFigure < this.path.Figures.Count; ++newFigure)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.StartPointIndex, newFigure + 1, newFigure));
        int num3 = this.path.Figures[newFigure].Segments.Count;
        if (newFigure + 1 == figureIndex)
          num3 = count1;
        for (int index = 0; index < num3; ++index)
          pathAction.PathStructureChanges.Add(new PathStructureChange(index, index, newFigure + 1, newFigure));
      }
      this.changeList.Changes.Add(pathAction);
    }

    public int PromoteSegment(int figureIndex, int lastPointIndexOfSegment)
    {
      return this.CreatePathFigureEditor(figureIndex).PromoteSegment(lastPointIndexOfSegment);
    }

    public void SetPoint(int figureIndex, int pointIndex, Point point)
    {
      this.CreatePathFigureEditor(figureIndex).SetPoint(pointIndex, point);
    }

    public void RemoveFigure(int figureIndex)
    {
      PathAction pathAction = new PathAction();
      pathAction.Action = PathActionType.RemoveFigure;
      pathAction.Figure = figureIndex;
      pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.DeletedPointIndex, figureIndex, figureIndex));
      for (int oldIndex = 0; oldIndex < this.path.Figures[figureIndex].Segments.Count; ++oldIndex)
        pathAction.PathStructureChanges.Add(new PathStructureChange(oldIndex, PathStructureChange.DeletedPointIndex, figureIndex, figureIndex));
      this.path.Figures.RemoveAt(figureIndex);
      for (int newFigure = figureIndex; newFigure < this.path.Figures.Count; ++newFigure)
      {
        pathAction.PathStructureChanges.Add(new PathStructureChange(PathStructureChange.StartPointIndex, PathStructureChange.StartPointIndex, newFigure + 1, newFigure));
        for (int index = 0; index < this.path.Figures[newFigure].Segments.Count; ++index)
          pathAction.PathStructureChanges.Add(new PathStructureChange(index, index, newFigure + 1, newFigure));
      }
      this.changeList.Changes.Add(pathAction);
    }

    public int RemovePoint(int figureIndex, int pointIndex)
    {
      return this.CreatePathFigureEditor(figureIndex).RemovePoint(pointIndex);
    }

    public int RemoveLastSegmentOfFigure(int figureIndex)
    {
      return this.CreatePathFigureEditor(figureIndex).RemoveLastSegment();
    }

    public int RemoveFirstSegmentOfFigure(int figureIndex)
    {
      return this.CreatePathFigureEditor(figureIndex).RemoveFirstSegment();
    }

    public int SubdivideSegment(int figureIndex, int lastPointIndexOfSegment, double parameter)
    {
      return this.CreatePathFigureEditor(figureIndex).SubdivideSegment(lastPointIndexOfSegment, parameter);
    }

    public void Reverse()
    {
      for (int figureIndex = 0; figureIndex < this.PathGeometry.Figures.Count; ++figureIndex)
        this.CreatePathFigureEditor(figureIndex).Reverse();
    }

    public void ReverseFigure(int figureIndex)
    {
      this.CreatePathFigureEditor(figureIndex).Reverse();
    }
  }
}
