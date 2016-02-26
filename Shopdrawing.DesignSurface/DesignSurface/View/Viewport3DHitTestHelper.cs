// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.Viewport3DHitTestHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.Framework;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.View
{
  public class Viewport3DHitTestHelper
  {
    private List<Viewport3DHitTestHelper.ViewportInformation> viewports = new List<Viewport3DHitTestHelper.ViewportInformation>();
    private BoundingVolume frustum;
    private Ray3D frustumCenterRay;

    public Viewport3DHitTestHelper(Viewport3DVisual viewportVisual, GeneralTransform viewportTransform)
    {
      this.viewports.Add(new Viewport3DHitTestHelper.ViewportInformation(viewportVisual, viewportTransform));
    }

    public Viewport3DHitTestHelper(Viewport3D viewport, GeneralTransform viewportTransform)
    {
      this.viewports.Add(new Viewport3DHitTestHelper.ViewportInformation(viewport, viewportTransform));
    }

    public Viewport3DHitTestHelper(List<Viewport3D> viewports, List<GeneralTransform> viewportTransforms)
    {
      for (int index = 0; index < viewports.Count; ++index)
        this.viewports.Add(new Viewport3DHitTestHelper.ViewportInformation(viewports[index], viewportTransforms[index]));
    }

    public RectangleHitTestResultTreeNode HitTest(HitTestParameters hitTestParameters)
    {
      PointHitTestParameters hitTestParameters1 = hitTestParameters as PointHitTestParameters;
      if (hitTestParameters1 != null)
        return this.HitTestPoint(hitTestParameters1.HitPoint);
      GeometryHitTestParameters hitTestParameters2;
      if ((hitTestParameters2 = hitTestParameters as GeometryHitTestParameters) != null)
        return this.HitTestGeometry(hitTestParameters2.HitGeometry);
      return (RectangleHitTestResultTreeNode) null;
    }

    private RectangleHitTestResultTreeNode HitTestPoint(Point point)
    {
        return this.HitTestGeometry((System.Windows.Media.Geometry)new RectangleGeometry(new Rect(point.X - 0.5, point.Y - 0.5, 1.0, 1.0)));
    }

    private RectangleHitTestResultTreeNode HitTestGeometry(System.Windows.Media.Geometry geometry)
    {
      RectangleGeometry rectangleGeometry = geometry as RectangleGeometry;
      Rect rect = rectangleGeometry != null ? rectangleGeometry.Rect : geometry.GetFlattenedPathGeometry().Bounds;
      RectangleHitTestResultTreeNode parent = new RectangleHitTestResultTreeNode((RectangleHitTestResultTreeNode) null, (DependencyObject) null);
      for (int index = 0; index < this.viewports.Count; ++index)
      {
        List<Point> frustumOutline = new List<Point>();
        Point point1 = this.viewports[index].Transform.Transform(rect.TopLeft);
        Point point2 = this.viewports[index].Transform.Transform(rect.TopRight);
        Point point3 = this.viewports[index].Transform.Transform(rect.BottomRight);
        Point point4 = this.viewports[index].Transform.Transform(rect.BottomLeft);
        frustumOutline.Add(point1);
        frustumOutline.Add(point2);
        frustumOutline.Add(point3);
        frustumOutline.Add(point4);
        this.frustum = this.BuildFrustumFromViewport(this.viewports[index], frustumOutline);
        RectangleHitTestResultTreeNode testResultTreeNode = new RectangleHitTestResultTreeNode(parent, (DependencyObject) null);
        if (this.WalkVisual3DChildren(this.viewports[index].Children, Matrix3D.Identity, testResultTreeNode))
          parent.AddChild(testResultTreeNode);
      }
      return parent;
    }

    private BoundingVolume BuildFrustumFromViewport(Viewport3DHitTestHelper.ViewportInformation viewport, List<Point> frustumOutline)
    {
      List<Ray3D> list1 = new List<Ray3D>(frustumOutline.Count);
      Point point1 = new Point(0.0, 0.0);
      for (int index = 0; index < frustumOutline.Count; ++index)
      {
        list1.Add(CameraRayHelpers.RayFromViewportPoint(viewport.Bounds, viewport.Camera, frustumOutline[index]));
        point1 += (Vector) frustumOutline[index];
      }
      Point point2 = (Point) Vector.Divide((Vector) point1, (double) frustumOutline.Count);
      Point3D point3D = new Point3D(0.0, 0.0, 0.0);
      List<Plane3D> list2 = new List<Plane3D>();
      for (int index = 0; index < frustumOutline.Count; ++index)
      {
        Vector3D normal = Vector3D.AngleBetween(list1[index].Direction, list1[(index + 1) % frustumOutline.Count].Direction) <= 0.0001 ? Vector3D.CrossProduct(list1[index].Direction, list1[(index + 1) % frustumOutline.Count].Origin - list1[index].Origin) : Vector3D.CrossProduct(list1[index].Direction, list1[(index + 1) % frustumOutline.Count].Direction);
        normal.Normalize();
        list2.Add(new Plane3D(normal, list1[index].Origin));
        point3D += (Vector3D) list1[index].Origin;
      }
      bool flag = list2[0].GetSignedDistanceFromPoint(list1[2].Origin) < 0.0;
      BoundingVolume boundingVolume = new BoundingVolume();
      for (int index = 0; index < frustumOutline.Count; ++index)
      {
        Plane3D frustumSide = list2[index];
        if (flag)
        {
          Vector3D normal = frustumSide.Normal;
          normal.Negate();
          frustumSide = new Plane3D(normal, list1[index].Origin);
        }
        boundingVolume.AddSide(frustumSide);
      }
      Ray3D ray3D = CameraRayHelpers.RayFromViewportPoint(viewport.Bounds, viewport.Camera, point2);
      this.frustumCenterRay = new Ray3D((Point3D) Vector3D.Divide((Vector3D) point3D, (double) frustumOutline.Count), ray3D.Direction);
      ProjectionCamera projectionCamera = viewport.Camera as ProjectionCamera;
      if (projectionCamera != null)
      {
        boundingVolume.NearFrustum = new Plane3D(ray3D.Direction, this.frustumCenterRay.Origin);
        if (!double.IsPositiveInfinity(projectionCamera.FarPlaneDistance))
        {
          Vector3D direction = ray3D.Direction;
          direction.Negate();
          boundingVolume.FarFrustum = new Plane3D(direction, this.frustumCenterRay.Origin + (projectionCamera.FarPlaneDistance - projectionCamera.NearPlaneDistance) * ray3D.Direction);
        }
      }
      return boundingVolume;
    }

    private bool WalkVisual3DChildren(Visual3DCollection visual3DCollection, Matrix3D currentTransform, RectangleHitTestResultTreeNode node)
    {
      bool flag1 = false;
      if (visual3DCollection == null)
        return false;
      foreach (Visual3D reference in visual3DCollection)
      {
        Matrix3D currentTransform1 = currentTransform;
        if (reference.Transform != null)
          currentTransform1.Prepend(reference.Transform.Value);
        if (this.TestSphereEnclosingBoundingBoxAgainstFrustum(VisualTreeHelper.GetContentBounds(reference), currentTransform1))
        {
          RectangleHitTestResultTreeNode testResultTreeNode = new RectangleHitTestResultTreeNode(node, (DependencyObject) reference);
          bool flag2 = false;
          ModelVisual3D modelVisual3D = reference as ModelVisual3D;
          ModelUIElement3D modelUiElement3D = reference as ModelUIElement3D;
          ContainerUIElement3D containerUiElement3D = reference as ContainerUIElement3D;
          Viewport2DVisual3D viewport2Dvisual3D = reference as Viewport2DVisual3D;
          if (modelVisual3D != null)
          {
            if (this.WalkModelTree(modelVisual3D.Content, currentTransform1, testResultTreeNode))
              flag1 = true;
            if (this.WalkVisual3DChildren(modelVisual3D.Children, currentTransform1, testResultTreeNode))
              flag1 = true;
          }
          else if (modelUiElement3D != null)
          {
            if (this.WalkModelTree(modelUiElement3D.Model, currentTransform1, testResultTreeNode))
              flag1 = true;
          }
          else if (containerUiElement3D != null)
          {
            if (this.WalkVisual3DChildren(containerUiElement3D.Children, currentTransform1, testResultTreeNode))
              flag1 = true;
          }
          else if (viewport2Dvisual3D != null && viewport2Dvisual3D.Geometry != null)
            flag2 = this.HitTestAgainstGeometry((DependencyObject) viewport2Dvisual3D, viewport2Dvisual3D.Geometry.Bounds, viewport2Dvisual3D.Transform, viewport2Dvisual3D.Geometry as MeshGeometry3D, currentTransform, viewport2Dvisual3D.Material, (Material) null, node);
          if (flag1)
            node.AddChild(testResultTreeNode);
          if (flag2)
            flag1 = true;
        }
      }
      return flag1;
    }

    private bool WalkModelTree(Model3D model3D, Matrix3D currentTransform, RectangleHitTestResultTreeNode node)
    {
      Model3DGroup model3Dgroup = model3D as Model3DGroup;
      if (model3Dgroup != null)
      {
        if (model3Dgroup.Children == null || !this.TestSphereEnclosingBoundingBoxAgainstFrustum(model3Dgroup.Bounds, currentTransform))
          return false;
        if (model3Dgroup.Transform != null)
          currentTransform.Prepend(model3Dgroup.Transform.Value);
        RectangleHitTestResultTreeNode testResultTreeNode = new RectangleHitTestResultTreeNode(node, (DependencyObject) model3Dgroup);
        bool flag = false;
        foreach (Model3D model3D1 in model3Dgroup.Children)
        {
          if (this.WalkModelTree(model3D1, currentTransform, testResultTreeNode))
            flag = true;
        }
        if (flag)
          node.AddChild(testResultTreeNode);
        return flag;
      }
      GeometryModel3D geometryModel3D;
      if ((geometryModel3D = model3D as GeometryModel3D) != null)
        return this.HitTestAgainstGeometry((DependencyObject) geometryModel3D, geometryModel3D.Bounds, geometryModel3D.Transform, geometryModel3D.Geometry as MeshGeometry3D, currentTransform, geometryModel3D.Material, geometryModel3D.BackMaterial, node);
      return false;
    }

    private bool HitTestAgainstGeometry(DependencyObject objectHit, Rect3D boundingBox, Transform3D transform, MeshGeometry3D geometry, Matrix3D currentTransform, Material frontMaterial, Material backMaterial, RectangleHitTestResultTreeNode node)
    {
      if (this.TestSphereEnclosingBoundingBoxAgainstFrustum(boundingBox, currentTransform))
      {
        if (transform != null)
          currentTransform.Prepend(transform.Value);
        if (this.TestGeometryAgainstFrustum(geometry, frontMaterial, backMaterial, this.frustum, currentTransform))
        {
          node.AddLeaf((RectangleHitTestResultTreeLeaf) new MeshGeometryRectangleHitTestResultTreeLeaf(node, objectHit, geometry, this.frustum, this.frustumCenterRay, currentTransform, frontMaterial, backMaterial));
          return true;
        }
      }
      return false;
    }

    private bool TestSphereEnclosingBoundingBoxAgainstFrustum(Rect3D boundingBox, Matrix3D currentTransform)
    {
      Vector3D vector3D = new Vector3D(boundingBox.SizeX, boundingBox.SizeY, boundingBox.SizeZ);
      Point3D origin = currentTransform.Transform(boundingBox.Location + vector3D * 0.5);
      double length = (currentTransform.Transform(boundingBox.Location + vector3D) - origin).Length;
      return this.frustum.IsSphereContainedOrIntersecting(origin, length);
    }

    private bool TestGeometryAgainstFrustum(MeshGeometry3D rawGeometry, Material frontMaterial, Material backMaterial, BoundingVolume frustum, Matrix3D transform)
    {
      if (rawGeometry == null)
        return false;
      TriangleEnumerator triangleEnumerator = new TriangleEnumerator(rawGeometry.Positions, rawGeometry.TriangleIndices);
      Point3D[] point3DArray = new Point3D[3];
      foreach (int[] numArray in triangleEnumerator.TriangleList)
      {
        point3DArray[0] = rawGeometry.Positions[numArray[0]];
        point3DArray[1] = rawGeometry.Positions[numArray[1]];
        point3DArray[2] = rawGeometry.Positions[numArray[2]];
        transform.Transform(point3DArray);
        if (frustum.IsPolygonContainedOrIntersecting(point3DArray))
        {
          double num = Vector3D.DotProduct(Vector3D.CrossProduct(point3DArray[1] - point3DArray[0], point3DArray[2] - point3DArray[0]), point3DArray[0] - this.frustumCenterRay.Origin);
          if (Tolerances.LessThanOrClose(num, 0.0) && frontMaterial != null || Tolerances.GreaterThan(num, 0.0) && backMaterial != null)
            return true;
        }
      }
      return false;
    }

    private struct ViewportInformation
    {
      private Visual3DCollection children;
      private Camera camera;
      private Size bounds;
      private GeneralTransform transform;

      public Visual3DCollection Children
      {
        get
        {
          return this.children;
        }
      }

      public Camera Camera
      {
        get
        {
          return this.camera;
        }
      }

      public Size Bounds
      {
        get
        {
          return this.bounds;
        }
      }

      public GeneralTransform Transform
      {
        get
        {
          return this.transform;
        }
      }

      public ViewportInformation(Visual3DCollection children, Camera camera, Size bounds, GeneralTransform transform)
      {
        this.children = children;
        this.camera = camera;
        this.bounds = bounds;
        this.transform = transform;
      }

      public ViewportInformation(Viewport3DVisual viewport, GeneralTransform transform)
      {
        this = new Viewport3DHitTestHelper.ViewportInformation(viewport.Children, viewport.Camera, new Size(viewport.Viewport.Width, viewport.Viewport.Height), transform);
      }

      public ViewportInformation(Viewport3D viewport, GeneralTransform transform)
      {
        this = new Viewport3DHitTestHelper.ViewportInformation(viewport.Children, viewport.Camera, new Size(viewport.ActualWidth, viewport.ActualHeight), transform);
      }
    }
  }
}
