// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathTangentAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PathTangentAdorner : PathPartAdorner, IClickable
  {
    private static readonly double radius = 2.0;
    private PathPointAdorner pathPointAdorner;
    private PathSegmentAdorner pathSegmentAdorner;

    public int EndPointIndex
    {
      get
      {
        return this.PartIndex;
      }
    }

    public int StartPointIndex
    {
      get
      {
        return this.pathPointAdorner.PointIndex;
      }
    }

    public Point StartPoint
    {
      get
      {
        return this.pathPointAdorner.Point;
      }
    }

    public Point EndPoint
    {
      get
      {
        return PathSegmentUtilities.GetPoint(this.PathGeometry.Figures[this.FigureIndex].Segments[this.SegmentIndex], this.SegmentPointIndex);
      }
    }

    public Vector Tangent
    {
      get
      {
        return this.EndPoint - this.StartPoint;
      }
    }

    protected override bool IsHighlightedOverride
    {
      get
      {
        if (this.PathAdornerSet.HighlightSegmentIndex == this.SegmentIndex)
          return this.PathAdornerSet.HighlightSegmentPointIndex == this.SegmentPointIndex;
        return false;
      }
    }

    public PathTangentAdorner(PathAdornerSet pathAdornerSet, int figureIndex, int endPointIndex, int segmentIndex, int segmentPointIndex, PathPointAdorner pathPointAdorner, PathSegmentAdorner pathSegmentAdorner)
      : base(pathAdornerSet, figureIndex, endPointIndex, segmentIndex, segmentPointIndex)
    {
      this.Initialize(figureIndex, endPointIndex, segmentIndex, segmentPointIndex, pathPointAdorner, pathSegmentAdorner);
    }

    public void Initialize(int figureIndex, int endPointIndex, int segmentIndex, int segmentPointIndex, PathPointAdorner pathPointAdorner, PathSegmentAdorner pathSegmentAdorner)
    {
      this.Initialize(figureIndex, endPointIndex, segmentIndex, segmentPointIndex);
      PathFigureEditor pathFigureEditor = new PathFigureEditor(this.PathAdornerSet.PathGeometry.Figures[figureIndex]);
      this.pathPointAdorner = pathPointAdorner;
      this.pathSegmentAdorner = pathSegmentAdorner;
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.EndPoint * this.PathGeometryTransformMatrix * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.pathPointAdorner.IsActive && !this.pathSegmentAdorner.IsActive && !this.IsHighlighted || (!this.IsValid || !this.pathPointAdorner.IsValid || !(this.StartPoint != this.EndPoint)))
        return;
      matrix = this.PathGeometryTransformMatrix * matrix;
      Point point0 = this.StartPoint * matrix;
      Point point = this.EndPoint * matrix;
      Pen pen = this.IsHighlighted ? this.ThickPathPen : this.ThinPathPen;
      ctx.DrawLine(pen, point0, point);
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      ctx.DrawEllipse(brush, pen, point, PathTangentAdorner.radius, PathTangentAdorner.radius);
    }

    public override PathPart ToPathPart()
    {
      return (PathPart) null;
    }
  }
}
