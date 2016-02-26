// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.Plane3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  public struct Plane3D
  {
    private Vector3D normal;
    private double distance;

    public Vector3D Normal
    {
      get
      {
        return this.normal;
      }
      set
      {
        this.normal = value;
      }
    }

    public double Distance
    {
      get
      {
        return this.distance;
      }
      set
      {
        this.distance = value;
      }
    }

    public Plane3D(Vector3D normal, double distance)
    {
      this.normal = normal;
      this.distance = distance;
    }

    public Plane3D(Vector3D normal, Point3D pointOnPlane)
    {
      this.normal = normal;
      this.distance = -(normal.X * pointOnPlane.X + normal.Y * pointOnPlane.Y + normal.Z * pointOnPlane.Z);
    }

    public double GetSignedDistanceFromPoint(Point3D point)
    {
      return Vector3D.DotProduct((Vector3D) point, this.normal) + this.distance;
    }

    public Plane3D Transform(Matrix3D transform)
    {
      Point3D point3D = transform.Transform((Point3D) (this.normal * -this.distance));
      this.normal = transform.Transform(this.normal);
      this.normal.Normalize();
      this.distance = -(this.normal.X * point3D.X + this.normal.Y * point3D.Y + this.normal.Z * point3D.Z);
      return this;
    }

    public bool IntersectWithLine(Point3D start, Point3D end, out Point3D result)
    {
      Vector3D direction = end - start;
      double t;
      if (this.IntersectCore(start, direction, out t))
      {
        result = new Point3D(start.X + direction.X * t, start.Y + direction.Y * t, start.Z + direction.Z * t);
        return true;
      }
      result = new Point3D(0.0, 0.0, 0.0);
      return false;
    }

    public bool IntersectWithRay(Ray3D ray, out double t)
    {
      return this.IntersectCore(ray.Origin, ray.Direction, out t);
    }

    private bool IntersectCore(Point3D start, Vector3D direction, out double t)
    {
      t = Vector3D.DotProduct(this.normal, direction);
      if (Math.Abs(t) < 4.94065645841247E-324)
      {
        t = double.NaN;
        return false;
      }
      t = -(Vector3D.DotProduct(this.normal, (Vector3D) start) + this.distance) / t;
      return true;
    }
  }
}
