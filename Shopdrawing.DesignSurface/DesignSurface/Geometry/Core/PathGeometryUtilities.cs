// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathGeometryUtilities
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public static class PathGeometryUtilities
  {
    public static PathGeometry Copy(PathGeometry original, bool applyTransform)
    {
      PathGeometry pathGeometry = new PathGeometry();
      if (pathGeometry.FillRule != original.FillRule)
        pathGeometry.FillRule = original.FillRule;
      if (!applyTransform && original.Transform != null && !original.Transform.Value.IsIdentity)
        pathGeometry.Transform = original.Transform.Clone();
      Transform transformToApply = applyTransform ? original.Transform : (Transform) null;
      foreach (PathFigure original1 in original.Figures)
        pathGeometry.Figures.Add(PathFigureUtilities.Copy(original1, transformToApply));
      return pathGeometry;
    }

    public static PathGeometry RemoveMapping(PathGeometry original, bool preserveOriginal)
    {
      if (original == null || original.Figures.Count == 0)
        return original;
      PathGeometry pathGeometry = preserveOriginal ? PathGeometryUtilities.Copy(original, false) : original;
      foreach (PathFigure pathFigure in pathGeometry.Figures)
      {
        pathFigure.ClearValue(PathFigureUtilities.FigureMappingProperty);
        foreach (DependencyObject dependencyObject in pathFigure.Segments)
          dependencyObject.ClearValue(PathFigureUtilities.SegmentMappingProperty);
      }
      return pathGeometry;
    }

    public static bool EnsureOnlySingleSegmentsInGeometry(PathGeometry geometry)
    {
      if (geometry == null)
        return false;
      bool flag = false;
      foreach (PathFigure original in geometry.Figures)
      {
        if (PathFigureUtilities.EnsureOnlySingleSegmentsInFigure(original))
          flag = true;
      }
      return flag;
    }

    public static bool CollapseSingleSegmentsToPolySegments(PathGeometry geometry)
    {
      if (geometry == null)
        return false;
      bool flag = false;
      foreach (PathFigure original in geometry.Figures)
      {
        if (PathFigureUtilities.CollapseSingleSegmentsToPolySegments(original))
          flag = true;
      }
      return flag;
    }

    public static int TotalPointCount(PathGeometry geometry)
    {
      int num = 0;
      foreach (PathFigure figure in geometry.Figures)
        num += PathFigureUtilities.PointCount(figure);
      return num;
    }

    public static int TotalSegmentCount(PathGeometry geometry)
    {
      int num = 0;
      foreach (PathFigure figure in geometry.Figures)
        num += PathFigureUtilities.SegmentCount(figure);
      return num;
    }

    public static bool IsEmpty(PathGeometry geometry)
    {
      return geometry.Figures.Count == 0;
    }

    public static Rect TightExtent(PathGeometry geometry)
    {
      return PathGeometryUtilities.TightExtent(geometry, Matrix.Identity);
    }

    public static Rect TightExtent(PathGeometry geometry, Matrix matrix)
    {
      if (geometry.Transform != null)
        matrix = geometry.Transform.Value * matrix;
      Rect empty = Rect.Empty;
      foreach (PathFigure figure in geometry.Figures)
        empty.Union(PathFigureUtilities.TightExtent(figure, matrix));
      return empty;
    }

    public static PathGeometry GetPathGeometryFromGeometry(System.Windows.Media.Geometry geometry)
    {
      PathGeometry pathGeometry = geometry as PathGeometry;
      if (pathGeometry == null)
      {
        pathGeometry = new PathGeometry();
        if (geometry != null)
          pathGeometry.AddGeometry(geometry);
        StreamGeometry streamGeometry = geometry as StreamGeometry;
        if (streamGeometry != null && pathGeometry.FillRule != streamGeometry.FillRule)
          pathGeometry.FillRule = streamGeometry.FillRule;
      }
      return pathGeometry;
    }

    public static PathGeometry TransformGeometry(System.Windows.Media.Geometry geometry, Transform transform)
    {
      PathGeometry pathGeometry1 = new PathGeometry();
      pathGeometry1.AddGeometry(geometry);
      pathGeometry1.Transform = transform;
      PathGeometry pathGeometry2 = new PathGeometry();
      pathGeometry2.AddGeometry((System.Windows.Media.Geometry)pathGeometry1);
      return pathGeometry2;
    }
  }
}
