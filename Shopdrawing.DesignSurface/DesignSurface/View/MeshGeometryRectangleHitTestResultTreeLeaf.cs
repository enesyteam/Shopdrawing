// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.MeshGeometryRectangleHitTestResultTreeLeaf
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.Framework;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.View
{
  public class MeshGeometryRectangleHitTestResultTreeLeaf : RectangleHitTestResultTreeLeaf
  {
    private MeshGeometry3D geometryHit;
    private Material frontMaterial;
    private Material backMaterial;

    public override IEnumerable<RectangleHitTestResult> HitResults
    {
      get
      {
        MeshGeometry3D meshGeometry = this.geometryHit;
        TriangleEnumerator triangleEnumerator = new TriangleEnumerator(meshGeometry.Positions, meshGeometry.TriangleIndices);
        Point3D[] triangle = new Point3D[3];
        foreach (int[] triangleIndices in triangleEnumerator.TriangleList)
        {
          triangle[0] = meshGeometry.Positions[triangleIndices[0]];
          triangle[1] = meshGeometry.Positions[triangleIndices[1]];
          triangle[2] = meshGeometry.Positions[triangleIndices[2]];
          this.CompositeTransform.Transform(triangle);
          Point3D[] intersectedPolygon = this.TransformedFrustum.IntersectPolygon(triangle);
          if (intersectedPolygon.Length != 0)
          {
            Vector3D normal = Vector3D.CrossProduct(triangle[1] - triangle[0], triangle[2] - triangle[0]);
            Point3D center = (Point3D) Vector3D.Divide((Vector3D) intersectedPolygon[0] + (Vector3D) intersectedPolygon[1] + (Vector3D) intersectedPolygon[2], 3.0);
            Vector3D hitVector = center - this.FrustumCenterRay.Origin;
            double cullSign = Vector3D.DotProduct(normal, hitVector);
            if (Tolerances.LessThanOrClose(cullSign, 0.0) && this.frontMaterial != null || Tolerances.GreaterThan(cullSign, 0.0) && this.backMaterial != null)
            {
              double distance = (this.FrustumCenterRay.Origin - center).Length;
              Point barycentricHit;
              MeshGeometryRectangleHitTestResultTreeLeaf.ComputeLineTriangleIntersection(new Ray3D(this.FrustumCenterRay.Origin, hitVector), triangle, out barycentricHit);
              yield return (RectangleHitTestResult) new RectangleMeshGeometryHitTestResult((RectangleHitTestResultTreeLeaf) this, this.ObjectHit, this.geometryHit, distance, barycentricHit, triangleIndices);
            }
          }
        }
      }
    }

    public MeshGeometryRectangleHitTestResultTreeLeaf(RectangleHitTestResultTreeNode parent, DependencyObject objectHit, MeshGeometry3D geometryHit, BoundingVolume transformedFrustum, Ray3D frustumCenterRay, Matrix3D transform, Material frontMaterial, Material backMaterial)
      : base(parent, objectHit, transformedFrustum, frustumCenterRay, transform)
    {
      this.geometryHit = geometryHit;
      this.frontMaterial = frontMaterial;
      this.backMaterial = backMaterial;
    }

    private static bool ComputeLineTriangleIntersection(Ray3D line, Point3D[] triangle, out Point hitBarycentric)
    {
      hitBarycentric = new Point(double.NaN, double.NaN);
      Vector3D vector3D1 = triangle[1] - triangle[0];
      Vector3D vector3D2 = triangle[2] - triangle[0];
      if (Tolerances.NearZero(Vector3D.CrossProduct(vector3D1, vector3D2).LengthSquared))
        return false;
      Vector3D vector3D3 = line.Origin - triangle[0];
      Vector3D vector2_1 = Vector3D.CrossProduct(line.Direction, vector3D2);
      double d = Vector3D.DotProduct(vector3D1, vector2_1);
      if (Tolerances.NearZero(d))
        return false;
      Vector3D vector1;
      if (d > 0.0)
      {
        vector1 = line.Origin - triangle[0];
      }
      else
      {
        vector1 = triangle[0] - line.Origin;
        d = -d;
      }
      double num1 = 1.0 / d;
      double num2 = Vector3D.DotProduct(vector1, vector2_1);
      if (num2 < 0.0 || d < num2)
        return false;
      Vector3D vector2_2 = Vector3D.CrossProduct(vector1, vector3D1);
      double num3 = Vector3D.DotProduct(line.Direction, vector2_2);
      if (num3 < 0.0 || d < num2 + num3)
        return false;
      double num4 = Vector3D.DotProduct(vector3D2, vector2_2) * num1;
      double x = num2 * num1;
      double y = num3 * num1;
      hitBarycentric = new Point(x, y);
      return true;
    }
  }
}
