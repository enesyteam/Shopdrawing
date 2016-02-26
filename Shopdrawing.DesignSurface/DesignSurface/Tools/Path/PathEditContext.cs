// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Path.PathEditContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Path
{
  public class PathEditContext
  {
    private int figureIndex;
    private int partIndex;

    public int FigureIndex
    {
      get
      {
        return this.figureIndex;
      }
    }

    public int PartIndex
    {
      get
      {
        return this.partIndex;
      }
    }

    public PathEditContext(int figureIndex, int partIndex)
    {
      this.figureIndex = figureIndex;
      this.partIndex = partIndex;
    }

    public PathFigure GetPathFigure(PathGeometry pathGeometry)
    {
      PathFigureCollection figures = pathGeometry.Figures;
      if (this.figureIndex < figures.Count)
        return figures[this.figureIndex];
      return (PathFigure) null;
    }
  }
}
