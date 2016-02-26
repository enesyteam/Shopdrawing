// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.RectangleHitTestResult
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.View
{
  public abstract class RectangleHitTestResult
  {
    private RectangleHitTestResultTreeLeaf hitTreeLeaf;
    private DependencyObject objectHit;
    private double distanceToRectangleCenter;

    public DependencyObject ObjectHit
    {
      get
      {
        return this.objectHit;
      }
    }

    public double DistanceToRectangleCenter
    {
      get
      {
        return this.distanceToRectangleCenter;
      }
    }

    public IEnumerable<DependencyObject> HitPath
    {
      get
      {
        return this.hitTreeLeaf.HitPath;
      }
    }

    public Matrix3D CompositeTransform
    {
      get
      {
        return this.hitTreeLeaf.CompositeTransform;
      }
    }

    public RectangleHitTestResult(RectangleHitTestResultTreeLeaf hitTreeLeaf, DependencyObject objectHit, double distanceToRectangleCenter)
    {
      this.hitTreeLeaf = hitTreeLeaf;
      this.objectHit = objectHit;
      this.distanceToRectangleCenter = distanceToRectangleCenter;
    }
  }
}
