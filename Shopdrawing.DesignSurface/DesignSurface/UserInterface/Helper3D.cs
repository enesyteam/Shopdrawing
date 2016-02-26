// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Helper3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public static class Helper3D
  {
    public const double DefaultCameraScaleFactor = 0.8;
    public const double QuaternionTolerance = 0.0001;

    public static Viewport3D UnitTestCreateViewportContainingModel(Model3D model, Camera camera, double width, double height, bool clipToBounds)
    {
      Viewport3D viewport3D = Helper3D.UnitTestCreateViewport3D(width, height, clipToBounds);
      ModelVisual3D world = new ModelVisual3D();
      Helper3D.UnitTestAddLights(world);
      if (model != null)
        world.Content = model;
      viewport3D.Children.Add((Visual3D) world);
      Helper3D.UnitTestAddCameraToViewport3D(camera, viewport3D, width, height, model);
      return viewport3D;
    }

    private static Viewport3D UnitTestCreateViewport3D(double width, double height, bool clipToBounds)
    {
      Viewport3D viewport3D = new Viewport3D();
      if (!double.IsNaN(width))
      {
        if (width < 1.0)
          width = 100.0;
        viewport3D.Width = width;
      }
      if (!double.IsNaN(height))
      {
        if (height < 1.0)
          height = 100.0;
        viewport3D.Height = height;
      }
      viewport3D.ClipToBounds = clipToBounds;
      return viewport3D;
    }

    private static void UnitTestAddLights(ModelVisual3D world)
    {
      AmbientLight ambientLight = new AmbientLight(Color.FromRgb((byte) 127, (byte) 127, (byte) 127));
      world.Children.Add((Visual3D) Helper3D.UnitTestCreateModelVisual3DContainer((Model3D) ambientLight));
      Vector3D direction = new Vector3D(0.0, 0.0, -1.0);
      direction.Normalize();
      DirectionalLight directionalLight = new DirectionalLight(Color.FromRgb((byte) 63, (byte) 63, (byte) 63), direction);
      directionalLight.Transform = (Transform3D) new TranslateTransform3D(new Vector3D(0.0, 0.0, 3.0));
      world.Children.Add((Visual3D) Helper3D.UnitTestCreateModelVisual3DContainer((Model3D) directionalLight));
    }

    private static void UnitTestAddCameraToViewport3D(Camera camera, Viewport3D viewport, double width, double height, Model3D model)
    {
      List<Model3D> models = new List<Model3D>();
      models.Add(model);
      if (camera == null)
      {
        if (double.IsNaN(height) || double.IsNaN(width))
          viewport.Camera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, 1.0, models);
        else
          viewport.Camera = Helper3D.CreateEnclosingPerspectiveCamera(45.0, height / width, models);
      }
      else
        viewport.Camera = camera;
    }

    private static ModelVisual3D UnitTestCreateModelVisual3DContainer(Model3D model3D)
    {
      return new ModelVisual3D()
      {
        Content = model3D
      };
    }

    public static Camera CreateEnclosingPerspectiveCamera(double horizontalFieldOfView, double aspectRatio, Rect3D bounds, double scaleRatio)
    {
      double num1 = horizontalFieldOfView * Math.PI / 180.0;
      double num2 = Math.Atan(Math.Tan(num1 / 2.0) / aspectRatio) * 2.0;
      Vector3D vector3D;
      if (bounds.IsEmpty)
      {
        vector3D = new Vector3D(0.0, 0.0, 0.0);
        bounds = new Rect3D(-1.0, -1.0, -1.0, 2.0, 2.0, 2.0);
      }
      else
        vector3D = new Vector3D(bounds.X + bounds.SizeX / 2.0, bounds.Y + bounds.SizeY / 2.0, bounds.Z + bounds.SizeZ / 2.0);
      PerspectiveCamera perspectiveCamera = new PerspectiveCamera();
      perspectiveCamera.FieldOfView = horizontalFieldOfView;
      perspectiveCamera.UpDirection = new Vector3D(0.0, 1.0, 0.0);
      double num3 = bounds.SizeX / 2.0;
      double num4 = bounds.SizeY / 2.0;
      double num5 = Math.Max(num3 / scaleRatio / Math.Tan(num1 / 2.0), num4 / scaleRatio / Math.Tan(num2 / 2.0));
      perspectiveCamera.Position = new Point3D(vector3D.X, vector3D.Y, bounds.Z + bounds.SizeZ + num5);
      perspectiveCamera.LookDirection = new Point3D(vector3D.X, vector3D.Y, vector3D.Z) - perspectiveCamera.Position;
      perspectiveCamera.NearPlaneDistance = 0.1;
      perspectiveCamera.FarPlaneDistance = Math.Max(100.0, 3.0 * num5);
      return (Camera) perspectiveCamera;
    }

    internal static Camera CreateEnclosingPerspectiveCamera(double horizontalFieldOfView, double aspectRatio, Model3D model)
    {
      return Helper3D.CreateEnclosingPerspectiveCamera(horizontalFieldOfView, aspectRatio, model != null ? model.Bounds : Rect3D.Empty, 0.8);
    }

    internal static Camera CreateEnclosingPerspectiveCamera(double horizontalFieldOfView, double aspectRatio, List<Model3D> models)
    {
      Rect3D empty = Rect3D.Empty;
      foreach (Model3D model3D in models)
      {
        if (model3D != null)
          empty.Union(model3D.Bounds);
      }
      return Helper3D.CreateEnclosingPerspectiveCamera(horizontalFieldOfView, aspectRatio, empty, 0.8);
    }

    public static double UnitsPerPixel(Viewport3D viewport, Base3DElement targetModel)
    {
      Matrix3D matrix3D1 = Matrix3D.Identity;
      Base3DElement base3Delement = targetModel.Parent as Base3DElement;
      if (base3Delement != null)
        matrix3D1 = base3Delement.GetComputedTransformFromViewport3DToElement();
      Point3D point3D = matrix3D1.Transform(new Point3D(0.0, 0.0, 0.0));
      double num = (matrix3D1.Transform(new Point3D(0.1, 0.1, 0.1)) - point3D).Length / 0.1;
      Matrix3D matrix3D2 = targetModel.Transform.Value * matrix3D1;
      Point3D targetPoint = new Point3D(matrix3D2.OffsetX, matrix3D2.OffsetY, matrix3D2.OffsetZ);
      return Helper3D.UnitsPerPixel(viewport, targetPoint) / num;
    }

    public static double UnitsPerPixel(Viewport3D viewport, Point3D targetPoint)
    {
      double num1 = 1.0;
      ProjectionCamera projectionCamera = viewport.Camera as ProjectionCamera;
      if (projectionCamera == null)
        return 1.0;
      Vector3D vector3D = targetPoint - projectionCamera.Position;
      PerspectiveCamera perspectiveCamera = projectionCamera as PerspectiveCamera;
      OrthographicCamera orthographicCamera = projectionCamera as OrthographicCamera;
      if (perspectiveCamera != null)
      {
        double fieldOfView = perspectiveCamera.FieldOfView;
        num1 = 2.0 * vector3D.Length * Math.Tan(Math.PI / 180.0 * (fieldOfView / 2.0));
      }
      else if (orthographicCamera != null)
        num1 = orthographicCamera.Width;
      double num2 = num1;
      if (viewport.ActualWidth > 0.0)
        num2 /= viewport.ActualWidth;
      return num2;
    }

    public static Matrix3D CameraRotationMatrix(Camera camera)
    {
      Matrix3D matrix3D = Helper3D.CameraRotationTranslationMatrix(camera);
      matrix3D.OffsetX = 0.0;
      matrix3D.OffsetY = 0.0;
      matrix3D.OffsetZ = 0.0;
      return matrix3D;
    }

    public static Matrix3D CameraRotationTranslationMatrix(Camera camera)
    {
      ProjectionCamera projectionCamera = camera as ProjectionCamera;
      Matrix3D matrix3D;
      if (projectionCamera != null)
      {
        Vector3D vector2 = (Vector3D) projectionCamera.Position;
        Vector3D vector3D1 = -projectionCamera.LookDirection;
        vector3D1.Normalize();
        Vector3D vector3D2 = Vector3D.CrossProduct(projectionCamera.UpDirection, vector3D1);
        vector3D2.Normalize();
        Vector3D vector1 = Vector3D.CrossProduct(vector3D1, vector3D2);
        double offsetX = -Vector3D.DotProduct(vector3D2, vector2);
        double offsetY = -Vector3D.DotProduct(vector1, vector2);
        double offsetZ = -Vector3D.DotProduct(vector3D1, vector2);
        matrix3D = new Matrix3D(vector3D2.X, vector1.X, vector3D1.X, 0.0, vector3D2.Y, vector1.Y, vector3D1.Y, 0.0, vector3D2.Z, vector1.Z, vector3D1.Z, 0.0, offsetX, offsetY, offsetZ, 1.0);
      }
      else
      {
        matrix3D = ((MatrixCamera) camera).ViewMatrix;
        matrix3D.OffsetX = 0.0;
        matrix3D.OffsetY = 0.0;
        matrix3D.OffsetZ = 0.0;
        matrix3D.M44 = 1.0;
      }
      return matrix3D;
    }

    public static Quaternion QuaternionFromRotation3D(Rotation3D rotation3D)
    {
      if (rotation3D == null)
        return Quaternion.Identity;
      QuaternionRotation3D quaternionRotation3D = rotation3D as QuaternionRotation3D;
      Quaternion quaternion;
      if (quaternionRotation3D != null)
      {
        quaternion = quaternionRotation3D.Quaternion;
      }
      else
      {
        AxisAngleRotation3D axisAngleRotation3D = (AxisAngleRotation3D) rotation3D;
        quaternion = new Quaternion(axisAngleRotation3D.Axis, axisAngleRotation3D.Angle);
      }
      return quaternion;
    }

    public static Vector3D EulerAnglesFromQuaternion(Quaternion orientation)
    {
      Quaternion quaternion = orientation;
      quaternion.Normalize();
      double d = 2.0 * (quaternion.W * quaternion.Y - quaternion.X * quaternion.Z);
      double num1;
      double num2;
      double num3;
      if (d > 0.9999)
      {
        num1 = 2.0 * Math.Atan2(quaternion.X, quaternion.Y);
        num2 = Math.PI / 2.0;
        num3 = 0.0;
      }
      else if (d < -0.9999)
      {
        num1 = -2.0 * Math.Atan2(quaternion.X, quaternion.Y);
        num2 = -1.0 * Math.PI / 2.0;
        num3 = 0.0;
      }
      else
      {
        num2 = Math.Asin(d);
        num1 = Math.Atan2(2.0 * (quaternion.W * quaternion.X + quaternion.Y * quaternion.Z), 2.0 * (quaternion.W * quaternion.W + quaternion.Z * quaternion.Z) - 1.0);
        num3 = Math.Atan2(2.0 * (quaternion.X * quaternion.Y + quaternion.W * quaternion.Z), 2.0 * (quaternion.W * quaternion.W + quaternion.X * quaternion.X) - 1.0);
      }
      return new Vector3D(num1 * 180.0 / Math.PI, num2 * 180.0 / Math.PI, num3 * 180.0 / Math.PI);
    }

    public static Quaternion QuaternionFromEulerAngles(Vector3D eulerAngles)
    {
      double num1 = eulerAngles.X * Math.PI / 180.0;
      double num2 = eulerAngles.Y * Math.PI / 180.0;
      double num3 = eulerAngles.Z * Math.PI / 180.0;
      double num4 = Math.Cos(num1 / 2.0);
      double num5 = Math.Cos(num2 / 2.0);
      double num6 = Math.Cos(num3 / 2.0);
      double num7 = Math.Sin(num1 / 2.0);
      double num8 = Math.Sin(num2 / 2.0);
      double num9 = Math.Sin(num3 / 2.0);
      return new Quaternion(num7 * num5 * num6 - num4 * num8 * num9, num4 * num8 * num6 + num7 * num5 * num9, num4 * num5 * num9 - num7 * num8 * num6, num4 * num5 * num6 + num7 * num8 * num9);
    }

    public static Vector3D VectorBetweenPointsOnPlane(Viewport3D viewport, Plane3D plane, Point initialPoint, Point endPoint)
    {
      Size viewportSize = new Size(viewport.ActualWidth, viewport.ActualHeight);
      Ray3D ray1 = CameraRayHelpers.RayFromViewportPoint(viewportSize, viewport.Camera, initialPoint);
      Ray3D ray2 = CameraRayHelpers.RayFromViewportPoint(viewportSize, viewport.Camera, endPoint);
      double t1;
      double t2;
      if (!plane.IntersectWithRay(ray1, out t1) || !plane.IntersectWithRay(ray2, out t2))
        return new Vector3D(0.0, 0.0, 0.0);
      return ray2.Evaluate(t2) - ray1.Evaluate(t1);
    }

    public static MeshGeometry3D TranslateGeometryModel3D(MeshGeometry3D model, Point3D center)
    {
      MeshGeometry3D meshGeometry3D = model.Clone();
      for (int index = 0; index < model.Positions.Count; ++index)
        meshGeometry3D.Positions[index] = new Point3D(model.Positions[index].X - center.X, model.Positions[index].Y - center.Y, model.Positions[index].Z - center.Z);
      return meshGeometry3D;
    }
  }
}
