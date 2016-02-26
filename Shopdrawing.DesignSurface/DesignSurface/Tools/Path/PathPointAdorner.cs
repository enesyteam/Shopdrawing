// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PathPointAdorner : PathPartAdorner, IClickable
  {
    private static readonly double HalfSize = 2.5;
    private static readonly Vector HalfSizeVector = new Vector(PathPointAdorner.HalfSize, PathPointAdorner.HalfSize);

    public int PointIndex
    {
      get
      {
        return this.PartIndex;
      }
    }

    public Point Point
    {
      get
      {
        return new PathFigureEditor(this.PathAdornerSet.PathGeometry.Figures[this.FigureIndex]).GetPoint(this.SegmentIndex, this.SegmentPointIndex);
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

    private Brush ActivePointBrush
    {
      get
      {
        switch (this.PathAdornerSet.PathEditorTarget.PathEditMode)
        {
          case PathEditMode.MotionPath:
            return FeedbackHelper.GetActiveBrush(AdornerType.MotionPath);
          case PathEditMode.ClippingPath:
            return FeedbackHelper.GetActiveBrush(AdornerType.ClipPath);
          default:
            return FeedbackHelper.GetActiveBrush();
        }
      }
    }

    public PathPointAdorner(PathAdornerSet pathAdornerSet, int figureIndex, int pointIndex, int segmentIndex, int segmentPointIndex)
      : base(pathAdornerSet, figureIndex, pointIndex, segmentIndex, segmentPointIndex)
    {
      PathFigureEditor pathFigureEditor = new PathFigureEditor(pathAdornerSet.PathGeometry.Figures[figureIndex]);
    }

    public static explicit operator PathPoint(PathPointAdorner pathPointAdorner)
    {
      PathEditorTarget pathEditorTarget = pathPointAdorner.PathAdornerSet.PathEditorTarget;
      return new PathPoint((SceneElement) pathEditorTarget.EditingElement, pathEditorTarget.PathEditMode, pathPointAdorner.FigureIndex, pathPointAdorner.PointIndex);
    }

    public override PathPart ToPathPart()
    {
      return (PathPart) (PathPoint) this;
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.Point * this.PathGeometryTransformMatrix * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.IsValid)
        return;
      Brush brush = this.IsActive ? this.ActivePointBrush : this.InactiveBrush;
      Point clickablePoint = this.GetClickablePoint(matrix);
      Pen pen = this.IsHighlighted ? this.ThickPathPen : this.ThinPathPen;
      ctx.DrawRectangle(brush, pen, new Rect(clickablePoint - PathPointAdorner.HalfSizeVector, clickablePoint + PathPointAdorner.HalfSizeVector));
    }
  }
}
