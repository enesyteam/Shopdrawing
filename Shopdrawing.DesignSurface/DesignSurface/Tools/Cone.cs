// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Cone
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal static class Cone
  {
    public static Model3DGroup CreateCone(double radius, double height, int segmentCount, Material bodyMaterial, Material bottomCapMaterial)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      model3Dgroup.Children = new Model3DCollection();
      if (bodyMaterial != null)
      {
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cone.CreateConeMesh(radius, height, segmentCount), bodyMaterial);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      if (bottomCapMaterial != null)
      {
        Transform3D transform = (Transform3D) new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(1.0, 0.0, 0.0), 180.0));
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cylinder.CreateDiskMesh(radius, segmentCount, transform), bottomCapMaterial);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      return model3Dgroup;
    }

    private static MeshGeometry3D CreateConeMesh(double radius, double height, int segmentCount)
    {
      MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
      Point3D point3D1 = new Point3D(0.0, height, 0.0);
      for (int index = 0; index < segmentCount; ++index)
      {
        double num1 = 2.0 * Math.PI * ((double) index / (double) segmentCount);
        double z1 = Math.Cos(num1);
        double num2 = Math.Sin(num1);
        Point3D point3D2 = new Point3D(radius * z1, 0.0, radius * num2);
        Vector3D vector1 = new Vector3D(-num2, 0.0, z1);
        Vector3D vector3D1 = Vector3D.CrossProduct(vector1, point3D1 - point3D2);
        meshGeometry3D.Positions.Add(point3D2);
        meshGeometry3D.Normals.Add(vector3D1);
        meshGeometry3D.TextureCoordinates.Add(new Point((1.0 + z1) / 2.0, (1.0 + num2) / 2.0));
        double num3 = 2.0 * Math.PI * (((double) index + 0.5) / (double) segmentCount);
        double z2 = Math.Cos(num3);
        vector1 = new Vector3D(-Math.Sin(num3), 0.0, z2);
        Vector3D vector3D2 = Vector3D.CrossProduct(vector1, point3D1 - point3D2);
        meshGeometry3D.Positions.Add(point3D1);
        meshGeometry3D.Normals.Add(vector3D2);
        meshGeometry3D.TextureCoordinates.Add(new Point(0.5, 0.5));
        meshGeometry3D.TriangleIndices.Add(2 * index);
        meshGeometry3D.TriangleIndices.Add(2 * index + 1);
        meshGeometry3D.TriangleIndices.Add((2 * index + 2) % (2 * segmentCount));
      }
      return meshGeometry3D;
    }
  }
}
