// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Cylinder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal static class Cylinder
  {
    public static Model3DGroup CreateCylinder(double radius, double height, int segmentCount, Material bodyMaterial, Material bottomCapMaterial, Material topCapMaterial)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      model3Dgroup.Children = new Model3DCollection();
      if (bodyMaterial != null)
      {
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cylinder.CreateCylinderMesh(radius, height, segmentCount), bodyMaterial);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      if (bottomCapMaterial != null)
      {
        Transform3D transform = (Transform3D) new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(1.0, 0.0, 0.0), 180.0));
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cylinder.CreateDiskMesh(radius, segmentCount, transform), bottomCapMaterial);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      if (topCapMaterial != null)
      {
        Transform3D transform = (Transform3D) new TranslateTransform3D(new Vector3D(0.0, height, 0.0));
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cylinder.CreateDiskMesh(radius, segmentCount, transform), topCapMaterial);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      return model3Dgroup;
    }

    private static MeshGeometry3D CreateCylinderMesh(double radius, double height, int segmentCount)
    {
      MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
      for (int index = 0; index <= segmentCount; ++index)
      {
        double num1 = (double) index / (double) segmentCount;
        double num2 = 2.0 * Math.PI * num1;
        double x = Math.Cos(num2);
        double z = Math.Sin(num2);
        meshGeometry3D.Positions.Add(new Point3D(radius * x, 0.0, radius * z));
        meshGeometry3D.Positions.Add(new Point3D(radius * x, height, radius * z));
        meshGeometry3D.Normals.Add(new Vector3D(x, 0.0, z));
        meshGeometry3D.Normals.Add(new Vector3D(x, 0.0, z));
        meshGeometry3D.TextureCoordinates.Add(new Point(1.0 - num1, 1.0));
        meshGeometry3D.TextureCoordinates.Add(new Point(1.0 - num1, 0.0));
        if (index > 0)
        {
          int num3 = 2 * index;
          meshGeometry3D.TriangleIndices.Add(num3 - 2);
          meshGeometry3D.TriangleIndices.Add(num3 - 1);
          meshGeometry3D.TriangleIndices.Add(num3 + 1);
          meshGeometry3D.TriangleIndices.Add(num3 - 2);
          meshGeometry3D.TriangleIndices.Add(num3 + 1);
          meshGeometry3D.TriangleIndices.Add(num3);
        }
      }
      return meshGeometry3D;
    }

    internal static MeshGeometry3D CreateDiskMesh(double radius, int segmentCount, Transform3D transform)
    {
      MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
      Vector3D vector3D = transform.Transform(new Vector3D(0.0, 1.0, 0.0));
      for (int index = 0; index < segmentCount; ++index)
      {
        double num1 = 2.0 * Math.PI * ((double) index / (double) segmentCount);
        double num2 = Math.Cos(num1);
        double num3 = Math.Sin(num1);
        meshGeometry3D.Positions.Add(transform.Transform(new Point3D(radius * num2, 0.0, radius * num3)));
        meshGeometry3D.Normals.Add(vector3D);
        meshGeometry3D.TextureCoordinates.Add(new Point((1.0 + num2) / 2.0, (1.0 + num3) / 2.0));
        meshGeometry3D.TriangleIndices.Add(index);
        meshGeometry3D.TriangleIndices.Add(segmentCount);
        meshGeometry3D.TriangleIndices.Add((index + 1) % segmentCount);
      }
      meshGeometry3D.Positions.Add(transform.Transform(new Point3D(0.0, 0.0, 0.0)));
      meshGeometry3D.Normals.Add(vector3D);
      meshGeometry3D.TextureCoordinates.Add(new Point(0.5, 0.5));
      return meshGeometry3D;
    }
  }
}
