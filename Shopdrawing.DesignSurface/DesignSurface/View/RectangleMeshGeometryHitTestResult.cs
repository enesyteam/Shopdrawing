// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RectangleMeshGeometryHitTestResult
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.View
{
  public class RectangleMeshGeometryHitTestResult : RectangleHitTestResult
  {
    private MeshGeometry3D meshHit;
    private int vertexIndex1;
    private int vertexIndex2;
    private int vertexIndex3;
    private double vertexWeight2;
    private double vertexWeight3;

    public MeshGeometry3D MeshHit
    {
      get
      {
        return this.meshHit;
      }
    }

    public Point3D PointHit
    {
      get
      {
        double num1 = this.vertexWeight2;
        double num2 = this.vertexWeight3;
        Point3D point3D1 = this.meshHit.Positions[this.vertexIndex1];
        Point3D point3D2 = this.meshHit.Positions[this.vertexIndex2];
        Point3D point3D3 = this.meshHit.Positions[this.vertexIndex3];
        return this.CompositeTransform.Transform(new Point3D(point3D1.X + num1 * (point3D2.X - point3D1.X) + num2 * (point3D3.X - point3D1.X), point3D1.Y + num1 * (point3D2.Y - point3D1.Y) + num2 * (point3D3.Y - point3D1.Y), point3D1.Z + num1 * (point3D2.Z - point3D1.Z) + num2 * (point3D3.Z - point3D1.Z)));
      }
    }

    public RectangleMeshGeometryHitTestResult(RectangleHitTestResultTreeLeaf hitTreeLeaf, DependencyObject objectHit, MeshGeometry3D meshHit, double distanceToRectangleCenter, Point barycentricHit, int[] triangleIndices)
      : base(hitTreeLeaf, objectHit, distanceToRectangleCenter)
    {
      this.meshHit = meshHit;
      this.vertexWeight2 = barycentricHit.X;
      this.vertexWeight3 = barycentricHit.Y;
      this.vertexIndex1 = triangleIndices[0];
      this.vertexIndex2 = triangleIndices[1];
      this.vertexIndex3 = triangleIndices[2];
    }
  }
}
