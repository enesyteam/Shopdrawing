// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RectangleHitTestResultTreeLeaf
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.View
{
  public abstract class RectangleHitTestResultTreeLeaf
  {
    private RectangleHitTestResultTreeNode parent;
    private DependencyObject objectHit;
    private BoundingVolume transformedFrustum;
    private Ray3D frustumCenterRay;
    private Matrix3D compositeTransform;

    public DependencyObject ObjectHit
    {
      get
      {
        return this.objectHit;
      }
    }

    public IEnumerable<DependencyObject> HitPath
    {
      get
      {
        yield return this.objectHit;
        for (RectangleHitTestResultTreeNode parent = this.parent; parent != null && (parent.Target is Model3D || parent.Target is Visual3D); parent = parent.Parent)
          yield return parent.Target;
      }
    }

    public abstract IEnumerable<RectangleHitTestResult> HitResults { get; }

    public Matrix3D CompositeTransform
    {
      get
      {
        return this.compositeTransform;
      }
    }

    protected BoundingVolume TransformedFrustum
    {
      get
      {
        return this.transformedFrustum;
      }
    }

    protected Ray3D FrustumCenterRay
    {
      get
      {
        return this.frustumCenterRay;
      }
    }

    public RectangleHitTestResultTreeLeaf(RectangleHitTestResultTreeNode parent, DependencyObject objectHit, BoundingVolume transformedFrustum, Ray3D frustumCenterRay, Matrix3D transform)
    {
      this.parent = parent;
      this.objectHit = objectHit;
      this.transformedFrustum = transformedFrustum;
      this.frustumCenterRay = frustumCenterRay;
      this.compositeTransform = transform;
    }
  }
}
