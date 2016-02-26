// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Torus
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal static class Torus
  {
    public static Model3DGroup CreateTorus(double spineRadius, double fleshRadius, int spineSegmentCount, int skinPanelCount, Material material)
    {
      return Torus.CreateTorusSection(spineRadius, fleshRadius, spineSegmentCount, spineSegmentCount, skinPanelCount, material);
    }

    public static Model3DGroup CreateQuarterTorus(double spineRadius, double fleshRadius, int spineSegmentCount, int skinPanelCount, Material material)
    {
      spineSegmentCount += (4 - spineSegmentCount % 4) % 4;
      int partialSpineSegmentCount = spineSegmentCount / 4;
      return Torus.CreateTorusSection(spineRadius, fleshRadius, partialSpineSegmentCount, spineSegmentCount, skinPanelCount, material);
    }

    private static Model3DGroup CreateTorusSection(double spineRadius, double fleshRadius, int partialSpineSegmentCount, int totalSpineSegmentCount, int skinPanelCount, Material material)
    {
      GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Torus.CreateTorusMesh(spineRadius, fleshRadius, partialSpineSegmentCount, totalSpineSegmentCount, skinPanelCount), material);
      return new Model3DGroup()
      {
        Children = new Model3DCollection()
        {
          (Model3D) geometryModel3D
        }
      };
    }

    private static MeshGeometry3D CreateTorusMesh(double spineRadius, double fleshRadius, int spineSegmentCount, int totalSpineSegmentCount, int skinPanelCount)
    {
      MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
      int num1 = 0;
      for (int index1 = 0; index1 <= spineSegmentCount; ++index1)
      {
        double x = (double) index1 / (double) totalSpineSegmentCount;
        double num2 = 2.0 * Math.PI * x;
        Vector3D vector3D1 = new Vector3D(Math.Cos(num2), Math.Sin(num2), 0.0);
        for (int index2 = 0; index2 <= skinPanelCount; ++index2)
        {
          double num3 = (double) index2 / (double) skinPanelCount;
          double num4 = 2.0 * Math.PI * num3;
          Vector3D vector3D2 = vector3D1 * Math.Cos(num4) + new Vector3D(0.0, 0.0, Math.Sin(num4));
          Point3D point3D = new Point3D(0.0, 0.0, 0.0) + spineRadius * vector3D1 + fleshRadius * vector3D2;
          meshGeometry3D.Positions.Add(point3D);
          meshGeometry3D.Normals.Add(vector3D2);
          meshGeometry3D.TextureCoordinates.Add(new Point(x, 1.0 - num3));
          if (index1 != 0 && index2 != 0)
          {
            int num5 = num1;
            int num6 = num1 - 1;
            int num7 = num1 - skinPanelCount - 2;
            int num8 = num1 - skinPanelCount - 1;
            meshGeometry3D.TriangleIndices.Add(num5);
            meshGeometry3D.TriangleIndices.Add(num7);
            meshGeometry3D.TriangleIndices.Add(num6);
            meshGeometry3D.TriangleIndices.Add(num5);
            meshGeometry3D.TriangleIndices.Add(num8);
            meshGeometry3D.TriangleIndices.Add(num7);
          }
          ++num1;
        }
      }
      return meshGeometry3D;
    }
  }
}
