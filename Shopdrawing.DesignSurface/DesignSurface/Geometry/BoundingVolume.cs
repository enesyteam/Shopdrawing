// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.BoundingVolume
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  public class BoundingVolume
  {
    private Plane3D? nearFrustum = new Plane3D?();
    private Plane3D? farFrustum = new Plane3D?();
    private Point3D[] triangleStore = new Point3D[9];
    private Point3D[] newTriangleStore = new Point3D[9];
    private List<Plane3D> frustumSides;
    private int outPoints;
    private Point3D intersectedPoint;

    public Plane3D NearFrustum
    {
      set
      {
        this.nearFrustum = new Plane3D?(value);
      }
    }

    public Plane3D FarFrustum
    {
      set
      {
        this.farFrustum = new Plane3D?(value);
      }
    }

    public BoundingVolume()
    {
      this.frustumSides = new List<Plane3D>(4);
    }

    public BoundingVolume(List<Plane3D> frustumSides, Plane3D? nearFrustum, Plane3D? farFrustum)
    {
      this.frustumSides = frustumSides;
      this.nearFrustum = nearFrustum;
      this.farFrustum = farFrustum;
    }

    public BoundingVolume Clone()
    {
      return new BoundingVolume(new List<Plane3D>((IEnumerable<Plane3D>) this.frustumSides), this.nearFrustum, this.farFrustum);
    }

    public void Transform(Matrix3D transform)
    {
      for (int index = 0; index < this.frustumSides.Count; ++index)
        this.frustumSides[index] = this.frustumSides[index].Transform(transform);
      if (this.nearFrustum.HasValue)
        this.nearFrustum = new Plane3D?(this.nearFrustum.Value.Transform(transform));
      if (!this.farFrustum.HasValue)
        return;
      this.farFrustum = new Plane3D?(this.farFrustum.Value.Transform(transform));
    }

    public void AddSide(Plane3D frustumSide)
    {
      this.frustumSides.Add(frustumSide);
    }

    public bool IsSphereContainedOrIntersecting(Point3D origin, double radius)
    {
      if (this.nearFrustum.HasValue && this.nearFrustum.Value.GetSignedDistanceFromPoint(origin) < -radius || this.farFrustum.HasValue && this.farFrustum.Value.GetSignedDistanceFromPoint(origin) < -radius)
        return false;
      foreach (Plane3D plane3D in this.frustumSides)
      {
        if (plane3D.GetSignedDistanceFromPoint(origin) < -radius)
          return false;
      }
      return true;
    }

    public bool IsPolygonContainedOrIntersecting(Point3D[] polygon)
    {
      return this.IntersectPolygonInternal(polygon) != 0;
    }

    public Point3D[] IntersectPolygon(Point3D[] polygon)
    {
      int length = this.IntersectPolygonInternal(polygon);
      Point3D[] point3DArray = new Point3D[length];
      for (int index = 0; index < length; ++index)
        point3DArray[index] = this.triangleStore[index];
      return point3DArray;
    }

    private void AddPointCheckForDuplicate(int k)
    {
      if (this.outPoints > 0 && this.newTriangleStore[this.outPoints - 1] == this.triangleStore[k])
        return;
      this.newTriangleStore[this.outPoints++] = this.triangleStore[k];
    }

    private void AddIntersectionCheckForDuplicate()
    {
      if (this.outPoints > 0 && this.newTriangleStore[this.outPoints - 1] == this.intersectedPoint)
        return;
      this.newTriangleStore[this.outPoints++] = this.intersectedPoint;
    }

    private int IntersectPolygonInternal(Point3D[] polygon)
    {
      int num = polygon.Length;
      this.outPoints = 0;
      bool flag1 = false;
      if (this.nearFrustum.HasValue)
      {
        foreach (Point3D point in polygon)
        {
          if (this.nearFrustum.Value.GetSignedDistanceFromPoint(point) >= 0.0)
          {
            flag1 = true;
            break;
          }
        }
      }
      else
        flag1 = true;
      bool flag2 = false;
      if (this.farFrustum.HasValue)
      {
        foreach (Point3D point in polygon)
        {
          if (this.farFrustum.Value.GetSignedDistanceFromPoint(point) >= 0.0)
          {
            flag2 = true;
            break;
          }
        }
      }
      else
        flag2 = true;
      if (!flag1 || !flag2)
        return 0;
      for (int index = 0; index < num; ++index)
        this.triangleStore[index] = polygon[index];
      foreach (Plane3D plane3D in this.frustumSides)
      {
        this.outPoints = 0;
        for (int k = 0; k < num; ++k)
        {
          int index = (k + 1) % num;
          bool flag3 = plane3D.GetSignedDistanceFromPoint(this.triangleStore[k]) >= 0.0;
          bool flag4 = plane3D.GetSignedDistanceFromPoint(this.triangleStore[index]) >= 0.0;
          if (flag3)
          {
            if (flag4)
            {
              this.AddPointCheckForDuplicate(k);
            }
            else
            {
              this.AddPointCheckForDuplicate(k);
              if (!plane3D.IntersectWithLine(this.triangleStore[k], this.triangleStore[index], out this.intersectedPoint))
              {
                this.outPoints = 0;
                break;
              }
              this.AddIntersectionCheckForDuplicate();
            }
          }
          else if (flag4)
          {
            if (!plane3D.IntersectWithLine(this.triangleStore[k], this.triangleStore[index], out this.intersectedPoint))
            {
              this.outPoints = 0;
              break;
            }
            this.AddIntersectionCheckForDuplicate();
          }
        }
        if (this.outPoints < 3)
          return 0;
        Point3D[] point3DArray = this.triangleStore;
        this.triangleStore = this.newTriangleStore;
        this.newTriangleStore = point3DArray;
        num = this.outPoints;
      }
      return num;
    }
  }
}
