// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.TriangleEnumerator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  public class TriangleEnumerator
  {
    private Point3DCollection positions;
    private Int32Collection indices;
    private bool indexedTriangle;

    public IEnumerable<int[]> TriangleList
    {
      get
      {
        if (this.positions != null)
        {
          int[] triangle = new int[3];
          if (this.indexedTriangle)
          {
            int i = 0;
            while (i < this.indices.Count - 2)
            {
              triangle[0] = this.indices[i];
              triangle[1] = this.indices[i + 1];
              triangle[2] = this.indices[i + 2];
              if (triangle[0] < 0 || triangle[0] >= this.positions.Count || (triangle[1] < 0 || triangle[1] >= this.positions.Count) || (triangle[2] < 0 || triangle[2] >= this.positions.Count))
                break;
              yield return triangle;
              i += 3;
            }
          }
          else
          {
            int i = 0;
            while (i < this.positions.Count - 2)
            {
              triangle[0] = i;
              triangle[1] = i + 1;
              triangle[2] = i + 2;
              yield return triangle;
              i += 3;
            }
          }
        }
      }
    }

    public TriangleEnumerator(Point3DCollection positions, Int32Collection indices)
    {
      this.positions = positions;
      this.indices = indices;
      this.indexedTriangle = this.indices != null && this.indices.Count > 0;
    }
  }
}
