// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Cube
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal static class Cube
  {
    public static Model3DGroup CreateCube(Rect3D rect, Material material)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      model3Dgroup.Children = new Model3DCollection();
      if (!rect.IsEmpty)
      {
        GeometryModel3D geometryModel3D = new GeometryModel3D((Geometry3D) Cube.CreateCubeMesh(rect), material);
        model3Dgroup.Children.Add((Model3D) geometryModel3D);
      }
      return model3Dgroup;
    }

    private static MeshGeometry3D CreateCubeMesh(Rect3D rect)
    {
      Point3D point3D1 = new Point3D(rect.X, rect.Y, rect.Z);
      Point3D point3D2 = new Point3D(rect.X, rect.Y, rect.Z + rect.SizeZ);
      Point3D point3D3 = new Point3D(rect.X, rect.Y + rect.SizeY, rect.Z);
      Point3D point3D4 = new Point3D(rect.X, rect.Y + rect.SizeY, rect.Z + rect.SizeZ);
      Point3D point3D5 = new Point3D(rect.X + rect.SizeX, rect.Y, rect.Z);
      Point3D point3D6 = new Point3D(rect.X + rect.SizeX, rect.Y, rect.Z + rect.SizeZ);
      Point3D point3D7 = new Point3D(rect.X + rect.SizeX, rect.Y + rect.SizeY, rect.Z);
      Point3D point3D8 = new Point3D(rect.X + rect.SizeX, rect.Y + rect.SizeY, rect.Z + rect.SizeZ);
      Vector3D normal1 = new Vector3D(-1.0, 0.0, 0.0);
      Vector3D normal2 = new Vector3D(0.0, -1.0, 0.0);
      Vector3D normal3 = new Vector3D(0.0, 0.0, -1.0);
      Vector3D normal4 = new Vector3D(1.0, 0.0, 0.0);
      Vector3D normal5 = new Vector3D(0.0, 1.0, 0.0);
      Vector3D normal6 = new Vector3D(0.0, 0.0, 1.0);
      MeshGeometry3D mesh = new MeshGeometry3D();
      Cube.AddFace(mesh, point3D4, point3D8, point3D6, point3D2, normal6);
      Cube.AddFace(mesh, point3D7, point3D3, point3D1, point3D5, normal3);
      Cube.AddFace(mesh, point3D8, point3D7, point3D5, point3D6, normal4);
      Cube.AddFace(mesh, point3D3, point3D4, point3D2, point3D1, normal1);
      Cube.AddFace(mesh, point3D3, point3D7, point3D8, point3D4, normal5);
      Cube.AddFace(mesh, point3D5, point3D1, point3D2, point3D6, normal2);
      return mesh;
    }

    private static void AddFace(MeshGeometry3D mesh, Point3D topLeft, Point3D topRight, Point3D bottomRight, Point3D bottomLeft, Vector3D normal)
    {
      int count = mesh.Positions.Count;
      mesh.Positions.Add(topLeft);
      mesh.Positions.Add(topRight);
      mesh.Positions.Add(bottomRight);
      mesh.Positions.Add(bottomLeft);
      mesh.Normals.Add(normal);
      mesh.Normals.Add(normal);
      mesh.Normals.Add(normal);
      mesh.Normals.Add(normal);
      mesh.TextureCoordinates.Add(new Point(0.0, 0.0));
      mesh.TextureCoordinates.Add(new Point(1.0, 0.0));
      mesh.TextureCoordinates.Add(new Point(1.0, 1.0));
      mesh.TextureCoordinates.Add(new Point(0.0, 1.0));
      mesh.TriangleIndices.Add(count + 1);
      mesh.TriangleIndices.Add(count);
      mesh.TriangleIndices.Add(count + 3);
      mesh.TriangleIndices.Add(count + 2);
      mesh.TriangleIndices.Add(count + 1);
      mesh.TriangleIndices.Add(count + 3);
    }
  }
}
