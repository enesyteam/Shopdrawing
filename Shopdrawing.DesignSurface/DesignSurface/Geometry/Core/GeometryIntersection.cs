// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.GeometryIntersection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class GeometryIntersection
  {
    private static readonly Pen stroke = new Pen((Brush) Brushes.Black, 1.0);
    private System.Windows.Media.Geometry geometry;

    public GeometryIntersection(System.Windows.Media.Geometry geometry)
    {
      this.geometry = geometry;
    }

    public GeometryIntersection(Rect rect)
    {
        this.geometry = (System.Windows.Media.Geometry)new RectangleGeometry(rect);
    }

    public bool IntersectsPoint(Point point)
    {
      return this.geometry.FillContains(point);
    }

    public bool IntersectsPathSegment(Point startPoint, PathSegment pathSegment, IntersectionDetail intersectionDetail)
    {
      return (new PathGeometry()
      {
        Figures = {
          new PathFigure()
          {
            StartPoint = startPoint,
            Segments = {
              pathSegment
            }
          }
        }
      }.StrokeContainsWithDetail(GeometryIntersection.stroke, this.geometry) & intersectionDetail) != IntersectionDetail.NotCalculated;
    }
  }
}
