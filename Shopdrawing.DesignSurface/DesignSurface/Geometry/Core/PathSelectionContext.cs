// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Core.PathSelectionContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.Geometry.Core
{
  public class PathSelectionContext
  {
    public int FigureIndex { get; private set; }

    public int SegmentIndex { get; private set; }

    public int PointIndex { get; private set; }

    public bool IsSegmentSelected
    {
      get
      {
        return this.SegmentIndex != -1;
      }
    }

    public void SetHitSegment(int figure, int segment)
    {
      this.FigureIndex = figure;
      this.SegmentIndex = segment;
      this.PointIndex = -1;
    }

    public void SetHitPoint(int figure, int point)
    {
      this.FigureIndex = figure;
      this.SegmentIndex = -1;
      this.PointIndex = point;
    }
  }
}
