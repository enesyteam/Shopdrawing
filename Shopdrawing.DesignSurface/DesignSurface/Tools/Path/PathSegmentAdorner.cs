// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathSegmentAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PathSegmentAdorner : PathPartAdorner, IClickable
  {
    private static readonly Pen HitTestPen = new Pen((Brush) Brushes.Transparent, 8.0);

    public int LastPointIndex
    {
      get
      {
        return this.PartIndex;
      }
    }

    public PathPointKind PathPointKind
    {
      get
      {
        if (this.SegmentIndex == -1)
          return PathPointKind.Line;
        return PathSegmentUtilities.GetPointKind(this.PathGeometry.Figures[this.FigureIndex].Segments[this.SegmentIndex], this.SegmentPointIndex);
      }
    }

    protected override bool IsHighlightedOverride
    {
      get
      {
        return false;
      }
    }

    static PathSegmentAdorner()
    {
      PathSegmentAdorner.HitTestPen.Freeze();
    }

    public PathSegmentAdorner(PathAdornerSet pathAdornerSet, int figureIndex, int lastPointIndex, int segmentIndex, int segmentPointIndex)
      : base(pathAdornerSet, figureIndex, lastPointIndex, segmentIndex, segmentPointIndex)
    {
    }

    public static explicit operator PathSegment(PathSegmentAdorner pathSegmentAdorner)
    {
      PathEditorTarget pathEditorTarget = pathSegmentAdorner.PathAdornerSet.PathEditorTarget;
      return new PathSegment((SceneElement) pathEditorTarget.EditingElement, pathEditorTarget.PathEditMode, pathSegmentAdorner.FigureIndex, pathSegmentAdorner.PartIndex);
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return new PathFigureEditor(this.PathGeometry.Figures[this.FigureIndex]).Evaluate(this.SegmentIndex, this.SegmentPointIndex, 0.5) * this.PathGeometryTransformMatrix * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.IsValid)
        return;
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.PathGeometry.Figures[this.FigureIndex]);
      StreamGeometry streamGeometry = new StreamGeometry();
      StreamGeometryContext streamGeometryContext = streamGeometry.Open();
      switch (this.PathPointKind)
      {
        case PathPointKind.Start:
          throw new InvalidOperationException(ExceptionStringTable.PathSegmentAdornerIsolatedPointSegment);
        case PathPointKind.Arc:
          ArcSegment arcSegment = (ArcSegment) pathFigureEditor.PathFigure.Segments[this.SegmentIndex];
          streamGeometryContext.BeginFigure(this.GetPoint(pathFigureEditor, -1), false, false);
          streamGeometryContext.ArcTo(arcSegment.Point, arcSegment.Size, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection, true, false);
          break;
        case PathPointKind.Line:
          streamGeometryContext.BeginFigure(this.GetPoint(pathFigureEditor, -1), false, false);
          streamGeometryContext.LineTo(this.GetPoint(pathFigureEditor, 0), true, false);
          break;
        case PathPointKind.Quadratic:
          streamGeometryContext.BeginFigure(this.GetPoint(pathFigureEditor, -2), false, false);
          streamGeometryContext.QuadraticBezierTo(this.GetPoint(pathFigureEditor, -1), this.GetPoint(pathFigureEditor, 0), true, false);
          break;
        case PathPointKind.Cubic:
          streamGeometryContext.BeginFigure(this.GetPoint(pathFigureEditor, -3), false, false);
          streamGeometryContext.BezierTo(this.GetPoint(pathFigureEditor, -2), this.GetPoint(pathFigureEditor, -1), this.GetPoint(pathFigureEditor, 0), true, false);
          break;
        case PathPointKind.BezierHandle:
          throw new InvalidOperationException(ExceptionStringTable.PathSegmentAdornerLastPointIsBezier);
        default:
          throw new NotImplementedException(ExceptionStringTable.PathSegmentAdornerUnknownPathPoint);
      }
      streamGeometryContext.Close();
      MatrixTransform matrixTransform = new MatrixTransform(matrix);
      matrixTransform.Freeze();
      streamGeometry.Transform = (Transform) matrixTransform;
      streamGeometry.Freeze();
      Pen pen = this.IsActive ? this.ThickPathSegmentPen : this.ThinPathSegmentPen;
      ctx.DrawGeometry((Brush)null, pen, (System.Windows.Media.Geometry)streamGeometry);
      ctx.DrawGeometry((Brush)null, PathSegmentAdorner.HitTestPen, (System.Windows.Media.Geometry)streamGeometry);
    }

    public override PathPart ToPathPart()
    {
      return (PathPart) (PathSegment) this;
    }

    private Point GetPoint(PathFigureEditor pathFigureEditor, int relativeIndex)
    {
      return pathFigureEditor.GetPoint(this.SegmentIndex, this.SegmentPointIndex + relativeIndex) * this.PathGeometryTransformMatrix;
    }
  }
}
