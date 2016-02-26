// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.CameraOrbitToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class CameraOrbitToolBehavior : ToolBehavior
  {
    private static readonly double shiftSnapAngle = 15.0;
    private CameraOrbitToolBehavior.MovementMode mouseMovementMode = CameraOrbitToolBehavior.MovementMode.Rotate;
    private CameraElement cameraElement;
    private Vector3D cameraInitialUp;
    private Point lastPoint;
    private Point3D cameraInitialPosition;
    private Point3D cameraInitialLookAt;
    private double scale;
    private Matrix rootToViewport3DMatrix;
    private Point pointerInViewportCoordinates;
    private double totalAzimuthDelta;
    private double totalElevationDelta;
    private bool isActive;
    private bool isAltDown;
    private bool isControlDown;

    public override string ActionString
    {
      get
      {
        return StringTable.UndoUnitRotateTranslateCamera;
      }
    }

    public CameraOrbitToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override bool OnButtonDown(Point pointerPosition)
    {
      this.isActive = false;
      Viewport3DElement firstHitViewport3D = this.ActiveView.GetFirstHitViewport3D(pointerPosition);
      if (firstHitViewport3D == null || firstHitViewport3D.ViewObject.PlatformSpecificObject == null)
        return false;
      this.cameraElement = firstHitViewport3D.Camera;
      if (this.cameraElement == null || !this.cameraElement.IsSelectable)
        return false;
      this.scale = Helper3D.UnitsPerPixel((Viewport3D) firstHitViewport3D.ViewObject.PlatformSpecificObject, new Point3D(0.0, 0.0, 0.0));
      this.rootToViewport3DMatrix = ElementUtilities.GetComputedTransform(firstHitViewport3D.Visual != null ? firstHitViewport3D.Visual.PlatformSpecificObject as Visual : (Visual) null, (Visual) this.ActiveSceneViewModel.DefaultView.ViewRootContainer);
      this.pointerInViewportCoordinates = pointerPosition * this.rootToViewport3DMatrix;
      this.SetMovementMode();
      this.cameraInitialUp = (Vector3D) this.cameraElement.GetComputedValue(ProjectionCameraElement.UpDirectionProperty);
      this.cameraInitialPosition = (Point3D) this.cameraElement.GetComputedValue(ProjectionCameraElement.PositionProperty);
      this.cameraInitialLookAt = this.cameraInitialPosition + (Vector3D) this.cameraElement.GetComputedValue(ProjectionCameraElement.LookDirectionProperty);
      this.totalAzimuthDelta = 0.0;
      this.totalElevationDelta = 0.0;
      this.lastPoint = this.pointerInViewportCoordinates;
      this.EnsureEditTransaction();
      this.isActive = true;
      return true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      return this.FinishUpdate();
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return this.FinishUpdate();
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      if (!this.isActive)
        return false;
      Point point = dragCurrentPosition * this.rootToViewport3DMatrix;
      this.UpdateModelFromMouse(point - this.lastPoint);
      this.lastPoint = point;
      if (this.IsEditTransactionOpen)
        this.UpdateEditTransaction();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isAltDown = this.IsAltDown;
      bool isControlDown = this.IsControlDown;
      if (this.isAltDown != isAltDown)
      {
        this.isAltDown = isAltDown;
        this.SetMovementMode();
      }
      else if (this.isControlDown != isControlDown)
      {
        this.isControlDown = isControlDown;
        this.SetMovementMode();
      }
      else
      {
        if (!this.IsEditTransactionOpen)
          return base.OnKey(args);
        return false;
      }
      return true;
    }

    protected override void OnResume()
    {
      this.Cursor = ToolCursors.OrbitCursor;
    }

    private void SetMovementMode()
    {
      this.mouseMovementMode = CameraOrbitToolBehavior.MovementMode.Rotate;
      if (this.isControlDown)
      {
        this.mouseMovementMode = CameraOrbitToolBehavior.MovementMode.TranslateXY;
      }
      else
      {
        if (!this.isAltDown)
          return;
        this.mouseMovementMode = CameraOrbitToolBehavior.MovementMode.TranslateZ;
      }
    }

    private bool FinishUpdate()
    {
      if (!this.isActive)
        return false;
      this.isActive = false;
      this.CommitEditTransaction();
      this.cameraElement = (CameraElement) null;
      return true;
    }

    private void UpdateModelFromMouse(Vector mousePositionDelta)
    {
      if (this.cameraElement == null || !(this.cameraElement is ProjectionCameraElement))
        return;
      if (this.mouseMovementMode == CameraOrbitToolBehavior.MovementMode.Rotate)
      {
        this.totalAzimuthDelta += -mousePositionDelta.X / 2.0;
        this.totalElevationDelta += -mousePositionDelta.Y / 2.0;
        double angle1 = this.totalAzimuthDelta;
        if (this.IsShiftDown)
          angle1 = CameraOrbitToolBehavior.shiftSnapAngle * Math.Round(angle1 / CameraOrbitToolBehavior.shiftSnapAngle);
        RotateTransform3D rotateTransform3D1 = new RotateTransform3D(Vector3D.DotProduct((Vector3D) this.cameraElement.GetComputedValue(ProjectionCameraElement.UpDirectionProperty), this.cameraInitialUp) >= 0.0 ? (Rotation3D) new AxisAngleRotation3D(this.cameraInitialUp, angle1) : (Rotation3D) new AxisAngleRotation3D(-this.cameraInitialUp, angle1), this.cameraInitialLookAt);
        Point3D point = rotateTransform3D1.Transform(this.cameraInitialPosition);
        Vector3D vector3D1 = rotateTransform3D1.Transform(this.cameraInitialUp);
        Vector3D axis = Vector3D.CrossProduct(this.cameraInitialLookAt - point, vector3D1);
        double angle2 = this.totalElevationDelta;
        if (this.IsShiftDown)
          angle2 = CameraOrbitToolBehavior.shiftSnapAngle * Math.Round(angle2 / CameraOrbitToolBehavior.shiftSnapAngle);
        if (axis.LengthSquared == 0.0)
          return;
        RotateTransform3D rotateTransform3D2 = new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(axis, angle2), this.cameraInitialLookAt);
        Point3D point3D1 = rotateTransform3D2.Transform(point);
        Vector3D vector3D2 = rotateTransform3D2.Transform(vector3D1);
        Point3D point3D2 = RoundingHelper.RoundPosition(point3D1);
        Vector3D vector3D3 = RoundingHelper.RoundDirection(vector3D2);
        Vector3D vector3D4 = RoundingHelper.RoundDirection(this.cameraInitialLookAt - point3D2);
        this.cameraElement.SetValue(ProjectionCameraElement.PositionProperty, (object) point3D2);
        this.cameraElement.SetValue(ProjectionCameraElement.UpDirectionProperty, (object) vector3D3);
        this.cameraElement.SetValue(ProjectionCameraElement.LookDirectionProperty, (object) vector3D4);
      }
      else
      {
        Matrix3D matrix3D = Helper3D.CameraRotationMatrix((Camera) this.cameraElement.ViewObject.PlatformSpecificObject);
        if (this.mouseMovementMode == CameraOrbitToolBehavior.MovementMode.TranslateXY)
        {
          Vector3D vector3D1 = new Vector3D(matrix3D.M11, matrix3D.M21, matrix3D.M31);
          Vector3D vector3D2 = new Vector3D(matrix3D.M12, matrix3D.M22, matrix3D.M32);
          this.cameraInitialPosition += this.scale * (-mousePositionDelta.X * vector3D1 + mousePositionDelta.Y * vector3D2);
        }
        else
        {
          Vector3D vector3D1 = new Vector3D(matrix3D.M13, matrix3D.M23, matrix3D.M33);
          this.cameraInitialPosition += this.scale * mousePositionDelta.Y * vector3D1;
          Vector3D vector3D2 = RoundingHelper.RoundDirection(this.cameraInitialLookAt - this.cameraInitialPosition);
          this.cameraElement.SetValue(ProjectionCameraElement.LookDirectionProperty, (object) vector3D2);
        }
        this.cameraInitialPosition = RoundingHelper.RoundPosition(this.cameraInitialPosition);
        this.cameraElement.SetValue(ProjectionCameraElement.PositionProperty, (object) this.cameraInitialPosition);
      }
    }

    private enum MovementMode
    {
      TranslateXY,
      TranslateZ,
      Rotate,
    }
  }
}
