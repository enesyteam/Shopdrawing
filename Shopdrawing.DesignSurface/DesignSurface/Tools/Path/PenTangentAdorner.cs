// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PenTangentAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  internal sealed class PenTangentAdorner : PathPartAdorner, IClickable
  {
    private static readonly double radius = 2.0;
    private PathPointAdorner pathPointAdorner;

    public Point StartPoint
    {
      get
      {
        return this.pathPointAdorner.Point;
      }
    }

    protected override bool IsHighlightedOverride
    {
      get
      {
        return false;
      }
    }

    public PenTangentAdorner(PathPointAdorner pathPointAdorner)
      : base(pathPointAdorner.PathAdornerSet, pathPointAdorner.FigureIndex, pathPointAdorner.PointIndex, pathPointAdorner.SegmentIndex, pathPointAdorner.SegmentPointIndex)
    {
      this.pathPointAdorner = pathPointAdorner;
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return (this.StartPoint + ((PenAdornerSet) this.AdornerSet).LastTangent) * this.PathGeometryTransformMatrix * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      Vector lastTangent = ((PenAdornerSet) this.AdornerSet).LastTangent;
      if (VectorUtilities.IsZero(lastTangent) || !this.pathPointAdorner.IsValid)
        return;
      matrix = this.PathGeometryTransformMatrix * matrix;
      Point point0 = this.StartPoint * matrix;
      Point point = (this.StartPoint + lastTangent) * matrix;
      ctx.DrawLine(this.ThinPen, point0, point);
      ctx.DrawEllipse(this.ActiveBrush, this.ThinPen, point, PenTangentAdorner.radius, PenTangentAdorner.radius);
    }

    public override PathPart ToPathPart()
    {
      return (PathPart) null;
    }
  }
}
