// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Geometry.CameraRayHelpers
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Geometry
{
  public static class CameraRayHelpers
  {
    public static Ray3D RayFromViewportPoint(Size viewportSize, Camera camera, Point point)
    {
      if (camera == null)
        return new Ray3D();
      PerspectiveCamera camera1;
      if ((camera1 = camera as PerspectiveCamera) != null)
        return CameraRayHelpers.RayFromPerspectiveCameraPoint(camera1, point, viewportSize);
      MatrixCamera camera2;
      if ((camera2 = camera as MatrixCamera) != null)
        return CameraRayHelpers.RayFromMatrixCameraPoint(camera2, point, viewportSize);
      OrthographicCamera camera3;
      if ((camera3 = camera as OrthographicCamera) != null)
        return CameraRayHelpers.RayFromOrthographicCameraPoint(camera3, point, viewportSize);
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnableToHandleCameraType, new object[1]
      {
        (object) camera.GetType().Name
      }), "camera");
    }

    private static Ray3D RayFromPerspectiveCameraPoint(PerspectiveCamera camera, Point point, Size viewSize)
    {
      Point3D position = camera.Position;
      Vector3D lookDirection = camera.LookDirection;
      Vector3D upDirection = camera.UpDirection;
      Transform3D transform = camera.Transform;
      double nearPlaneDistance = camera.NearPlaneDistance;
      double farPlaneDistance = camera.FarPlaneDistance;
      double num1 = camera.FieldOfView * (Math.PI / 180.0);
      Point normalizedPoint = CameraRayHelpers.GetNormalizedPoint(point, viewSize);
      double aspectRatio = CameraRayHelpers.GetAspectRatio(viewSize);
      double num2 = Math.Tan(num1 / 2.0);
      double num3 = aspectRatio / num2;
      double num4 = 1.0 / num2;
      Vector3D vector = new Vector3D(normalizedPoint.X / num4, normalizedPoint.Y / num3, -1.0);
      Matrix3D viewMatrix = CameraRayHelpers.CreateViewMatrix((Transform3D) null, ref position, ref lookDirection, ref upDirection);
      viewMatrix.Invert();
      Vector3D vector3D = viewMatrix.Transform(vector);
      Point3D point3D = position + nearPlaneDistance * vector3D;
      vector3D.Normalize();
      if (transform != null && transform != Transform3D.Identity)
      {
        point3D = transform.Transform(point3D);
        vector3D = transform.Transform(vector3D);
      }
      return new Ray3D(point3D, vector3D);
    }

    private static Ray3D RayFromOrthographicCameraPoint(OrthographicCamera camera, Point point, Size viewSize)
    {
      Point3D position = camera.Position;
      Vector3D lookDirection = camera.LookDirection;
      Vector3D upDirection = camera.UpDirection;
      Transform3D transform = camera.Transform;
      double nearPlaneDistance = camera.NearPlaneDistance;
      double farPlaneDistance = camera.FarPlaneDistance;
      double width = camera.Width;
      Point normalizedPoint = CameraRayHelpers.GetNormalizedPoint(point, viewSize);
      double aspectRatio = CameraRayHelpers.GetAspectRatio(viewSize);
      double num1 = width;
      double num2 = num1 / aspectRatio;
      Point3D point1 = new Point3D(normalizedPoint.X * (num1 / 2.0), normalizedPoint.Y * (num2 / 2.0), -nearPlaneDistance);
      Vector3D vector = new Vector3D(0.0, 0.0, -1.0);
      Matrix3D viewMatrix = CameraRayHelpers.CreateViewMatrix((Transform3D) null, ref position, ref lookDirection, ref upDirection);
      viewMatrix.Invert();
      Point3D point3D = viewMatrix.Transform(point1);
      Vector3D vector3D = viewMatrix.Transform(vector);
      if (transform != null && transform != Transform3D.Identity)
      {
        point3D = transform.Transform(point3D);
        vector3D = transform.Transform(vector3D);
      }
      return new Ray3D(point3D, vector3D);
    }

    private static Ray3D RayFromMatrixCameraPoint(MatrixCamera camera, Point point, Size viewSize)
    {
      Point normalizedPoint = CameraRayHelpers.GetNormalizedPoint(point, viewSize);
      Matrix3D matrix3D = camera.ViewMatrix * camera.ProjectionMatrix;
      if (!matrix3D.HasInverse)
        throw new NotSupportedException(ExceptionStringTable.NeedToHandleSingularMatrixCameras);
      matrix3D.Invert();
      Point4D point4D = new Point4D(normalizedPoint.X, normalizedPoint.Y, 0.0, 1.0) * matrix3D;
      Point3D origin = new Point3D(point4D.X / point4D.W, point4D.Y / point4D.W, point4D.Z / point4D.W);
      Vector3D direction = new Vector3D(matrix3D.M31 - matrix3D.M34 * origin.X, matrix3D.M32 - matrix3D.M34 * origin.Y, matrix3D.M33 - matrix3D.M34 * origin.Z);
      direction.Normalize();
      if (point4D.W < 0.0)
        direction = -direction;
      return new Ray3D(origin, direction);
    }

    private static Matrix3D CreateViewMatrix(Transform3D transform, ref Point3D position, ref Vector3D lookDirection, ref Vector3D upDirection)
    {
      Vector3D vector3D1 = -lookDirection;
      vector3D1.Normalize();
      Vector3D vector3D2 = Vector3D.CrossProduct(upDirection, vector3D1);
      vector3D2.Normalize();
      Vector3D vector1 = Vector3D.CrossProduct(vector3D1, vector3D2);
      Vector3D vector2 = (Vector3D) position;
      double offsetX = -Vector3D.DotProduct(vector3D2, vector2);
      double offsetY = -Vector3D.DotProduct(vector1, vector2);
      double offsetZ = -Vector3D.DotProduct(vector3D1, vector2);
      Matrix3D viewMatrix = new Matrix3D(vector3D2.X, vector1.X, vector3D1.X, 0.0, vector3D2.Y, vector1.Y, vector3D1.Y, 0.0, vector3D2.Z, vector1.Z, vector3D1.Z, 0.0, offsetX, offsetY, offsetZ, 1.0);
      CameraRayHelpers.PrependInverseTransform(transform, ref viewMatrix);
      return viewMatrix;
    }

    private static void PrependInverseTransform(Transform3D transform, ref Matrix3D viewMatrix)
    {
      if (transform == null || transform == Transform3D.Identity)
        return;
      Matrix3D matrix = transform.Value;
      if (!matrix.HasInverse)
      {
        viewMatrix = new Matrix3D(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
      }
      else
      {
        matrix.Invert();
        viewMatrix.Prepend(matrix);
      }
    }

    private static Point GetNormalizedPoint(Point point, Size size)
    {
      return new Point(2.0 * point.X / size.Width - 1.0, -(2.0 * point.Y / size.Height - 1.0));
    }

    private static double GetAspectRatio(Size size)
    {
      return size.Width / size.Height;
    }
  }
}
