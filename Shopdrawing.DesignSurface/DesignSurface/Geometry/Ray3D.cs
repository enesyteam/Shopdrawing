// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Ray3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  public struct Ray3D
  {
    private Point3D origin;
    private Vector3D direction;

    public Point3D Origin
    {
      get
      {
        return this.origin;
      }
      set
      {
        this.origin = value;
      }
    }

    public Vector3D Direction
    {
      get
      {
        return this.direction;
      }
      set
      {
        this.direction = value;
      }
    }

    public Ray3D(Point3D origin, Vector3D direction)
    {
      this.origin = origin;
      this.direction = direction;
    }

    public Ray3D Transform(Matrix3D matrix)
    {
      return new Ray3D(matrix.Transform(this.origin), matrix.Transform(this.direction));
    }

    public Point3D Evaluate(double t)
    {
      return this.origin + this.direction * t;
    }
  }
}
