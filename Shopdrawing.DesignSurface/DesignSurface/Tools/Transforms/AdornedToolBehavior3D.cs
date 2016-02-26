// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.AdornedToolBehavior3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal abstract class AdornedToolBehavior3D : AdornedToolBehavior
  {
    protected static readonly Point3D origin = new Point3D(0.0, 0.0, 0.0);
    protected static readonly double sqrt2div2 = Math.Sqrt(2.0) / 2.0;
    protected static readonly double tolerance = 1E-14;
    private bool popSelfOnButtonUp = true;
    private bool altDown;
    private bool controlDown;
    private bool shiftDown;
    private Base3DElement selected3DElement;
    private double scale;
    private Matrix rootToViewport3DMatrix;
    private Point pointerInViewportCoordinates;
    private Point lastMousePosition;

    protected Adorner3D ActiveAdorner
    {
      get
      {
        return (Adorner3D) base.ActiveAdorner;
      }
    }

    protected Base3DElement Selected3DElement
    {
      get
      {
        return this.selected3DElement;
      }
    }

    protected Point LastMousePosition
    {
      get
      {
        return this.lastMousePosition;
      }
    }

    protected bool PopSelfOnButtonUp
    {
      get
      {
        return this.popSelfOnButtonUp;
      }
      set
      {
        this.popSelfOnButtonUp = value;
      }
    }

    protected double Scale
    {
      get
      {
        return this.scale;
      }
      private set
      {
        this.scale = value;
      }
    }

    protected Point PointerInWindowCoordinates
    {
      get
      {
        return this.pointerInViewportCoordinates;
      }
    }

    protected bool AltKeyDepressed
    {
      get
      {
        return this.altDown;
      }
    }

    protected bool ControlKeyDepressed
    {
      get
      {
        return this.controlDown;
      }
    }

    protected bool ShiftKeyDepressed
    {
      get
      {
        return this.shiftDown;
      }
    }

    protected Matrix TransformToViewport3D
    {
      get
      {
        return this.rootToViewport3DMatrix;
      }
    }

    protected virtual AdornedToolBehavior3D.DragBehaviors DragBehavior
    {
      get
      {
        return AdornedToolBehavior3D.DragBehaviors.UseAdornedElement;
      }
    }

    public override string ActionString
    {
      get
      {
        return this.UndoUnitNameString;
      }
    }

    protected abstract string UndoUnitNameString { get; }

    public AdornedToolBehavior3D(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected virtual void UpdateShiftKeyState()
    {
    }

    protected virtual void UpdateAltKeyState()
    {
    }

    protected virtual void UpdateControlKeyState()
    {
    }

    protected abstract void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta);

    protected abstract void ButtonDownAction();

    protected override sealed bool OnButtonDown(Point pointerPosition)
    {
      if (this.ActiveAdorner != null)
        this.selected3DElement = this.EditingElement as Base3DElement;
      if (this.selected3DElement == null)
        this.selected3DElement = this.ActiveSceneViewModel.ElementSelectionSet.PrimarySelection as Base3DElement;
      if (this.selected3DElement == null || this.selected3DElement.Viewport.Visual == null)
        return false;
      this.Scale = Helper3D.UnitsPerPixel((Viewport3D) this.Selected3DElement.Viewport.ViewObject.PlatformSpecificObject, this.Selected3DElement);
      this.rootToViewport3DMatrix = ElementUtilities.GetComputedTransform(this.selected3DElement.Viewport.Visual.PlatformSpecificObject as Visual, (Visual) this.ActiveSceneViewModel.DefaultView.ViewRootContainer);
      this.pointerInViewportCoordinates = pointerPosition * this.rootToViewport3DMatrix;
      this.shiftDown = this.IsShiftDown;
      this.controlDown = this.IsControlDown;
      this.altDown = this.IsAltDown;
      this.ButtonDownAction();
      this.lastMousePosition = this.pointerInViewportCoordinates;
      this.EnsureEditTransaction();
      return true;
    }

    internal override bool ShouldMotionlessAutoScroll(Point mousePoint, Rect artboardBoundary)
    {
      return false;
    }

    protected override bool OnDrag(Point dragStartPosition, Point dragCurrentPosition, bool scrollNow)
    {
      bool flag = false;
      Point point = dragCurrentPosition * this.rootToViewport3DMatrix;
      switch (this.DragBehavior)
      {
        case AdornedToolBehavior3D.DragBehaviors.UseAdornedElement:
          if (this.selected3DElement != null)
          {
            this.UpdateModelFromMouse(this.selected3DElement, point - this.lastMousePosition);
            flag = true;
            break;
          }
          break;
        case AdornedToolBehavior3D.DragBehaviors.UseSelectionSet:
          using (IEnumerator<Base3DElement> enumerator = this.ActiveSceneViewModel.ElementSelectionSet.GetFilteredSelection<Base3DElement>().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              this.UpdateModelFromMouse(enumerator.Current, point - this.lastMousePosition);
              flag = true;
            }
            break;
          }
      }
      this.lastMousePosition = point;
      if (this.IsEditTransactionOpen)
        this.UpdateEditTransaction();
      return flag;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      return this.Finish();
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      return this.Finish();
    }

    protected virtual bool Finish()
    {
      this.selected3DElement = (Base3DElement) null;
      this.CommitEditTransaction();
      if (this.popSelfOnButtonUp)
        this.PopSelf();
      return true;
    }

    protected override bool OnKey(KeyEventArgs args)
    {
      bool isShiftDown = this.IsShiftDown;
      bool isAltDown = this.IsAltDown;
      bool isControlDown = this.IsControlDown;
      if (this.shiftDown != isShiftDown)
      {
        this.shiftDown = isShiftDown;
        this.UpdateShiftKeyState();
      }
      else if (this.altDown != isAltDown)
      {
        this.altDown = isAltDown;
        this.UpdateAltKeyState();
      }
      else if (this.controlDown != isControlDown)
      {
        this.controlDown = isControlDown;
        this.UpdateControlKeyState();
      }
      else
      {
        if (!this.IsEditTransactionOpen)
          return base.OnKey(args);
        return false;
      }
      return true;
    }

    protected Point3D GetHitPoint()
    {
      RectangleMeshGeometryHitTestResult geometryHitTestResult = (this.ActiveAdorner == null ? new Viewport3DHitTestHelper((Viewport3D) this.selected3DElement.Viewport.ViewObject.PlatformSpecificObject, (GeneralTransform) Transform.Identity) : new Viewport3DHitTestHelper(this.ActiveView.AdornerLayer.GetAdornerSet3DContainer(this.ActiveAdorner.Element.Viewport).OrthographicAdorningViewport3D, (GeneralTransform) Transform.Identity)).HitTest((HitTestParameters) new PointHitTestParameters(this.pointerInViewportCoordinates)).ClosestHitTestResult as RectangleMeshGeometryHitTestResult;
      if (geometryHitTestResult == null)
        return new Point3D(0.0, 0.0, 0.0);
      return geometryHitTestResult.PointHit;
    }

    internal static Point Point3DInViewport3D(Viewport3D viewport, Matrix3D viewportToWorld, Point3D point)
    {
      Camera camera = viewport.Camera;
      Point3D point1 = viewportToWorld.Transform(point);
      Point3D point3D = Helper3D.CameraRotationTranslationMatrix(camera).Transform(point1);
      Point4D point2 = new Point4D(point3D.X, point3D.Y, point3D.Z, 1.0);
      Point4D point4D = AdornedToolBehavior3D.ProjectionMatrix(viewport.ActualWidth, viewport.ActualHeight, camera).Transform(point2);
      Point point3 = Math.Abs(point4D.W) >= 0.0 / 1.0 ? new Point(point4D.X / point4D.W, point4D.Y / point4D.W) : new Point(0.0, 0.0);
      return new Point((point3.X + 1.0) * viewport.ActualWidth / 2.0, viewport.ActualHeight * ((1.0 - point3.Y) / 2.0));
    }

    internal static Point3D ProjectionPoint3DTranslatedToMatchingOrthographicPosition(Viewport3D viewport, Matrix3D pointToWorldTransform, OrthographicCamera ortho, Point3D point)
    {
      Camera camera = viewport.Camera;
      Point3D point1 = pointToWorldTransform.Transform(point);
      Point3D point3D = Helper3D.CameraRotationTranslationMatrix(camera).Transform(point1);
      Point4D point2 = new Point4D(point3D.X, point3D.Y, point3D.Z, 1.0);
      Point4D point4D1 = AdornedToolBehavior3D.ProjectionMatrix(viewport.ActualWidth, viewport.ActualHeight, camera).Transform(point2);
      Point4D point3 = new Point4D(point4D1.X / point4D1.W, point4D1.Y / point4D1.W, 0.0, 1.0);
      Matrix3D matrix3D1 = AdornedToolBehavior3D.ProjectionMatrix(viewport.ActualWidth, viewport.ActualHeight, (Camera) ortho);
      if (Math.Abs(matrix3D1.Determinant) > 1E-16)
        matrix3D1.Invert();
      Point4D point4D2 = matrix3D1.Transform(point3);
      Point3D point4 = new Point3D(point4D2.X / point4D2.W, point4D2.Y / point4D2.W, point3D.Z);
      Matrix3D matrix3D2 = Helper3D.CameraRotationTranslationMatrix((Camera) ortho);
      matrix3D2.Invert();
      return matrix3D2.Transform(point4);
    }

    protected static Vector Vector3DInViewport3D(Base3DElement target, Vector3D vector)
    {
      Viewport3D viewport = (Viewport3D) target.Viewport.ViewObject.PlatformSpecificObject;
      Matrix3D viewport3DtoElement = target.GetComputedTransformFromViewport3DToElement();
      Point point = AdornedToolBehavior3D.Point3DInViewport3D(viewport, viewport3DtoElement, new Point3D(0.0, 0.0, 0.0));
      Vector vector1 = AdornedToolBehavior3D.Point3DInViewport3D(viewport, viewport3DtoElement, (Point3D) vector) - point;
      if (vector1.Length < 0.0 / 1.0)
        return new Vector(0.0, 1.0);
      return vector1;
    }

    protected static Matrix3D ProjectionMatrix(double width, double height, Camera camera)
    {
      Matrix3D matrix3D = new Matrix3D();
      PerspectiveCamera perspectiveCamera = camera as PerspectiveCamera;
      if (perspectiveCamera != null)
      {
        double num1 = width / height;
        double num2 = 1.0 / Math.Tan(Math.PI / 180.0 * perspectiveCamera.FieldOfView / 2.0);
        double num3 = perspectiveCamera.NearPlaneDistance - perspectiveCamera.FarPlaneDistance;
        double num4 = -1.0;
        if (!double.IsPositiveInfinity(perspectiveCamera.FarPlaneDistance))
          num4 = perspectiveCamera.FarPlaneDistance / num3;
        matrix3D.M11 = num2;
        matrix3D.M22 = num1 * num2;
        matrix3D.M33 = num4;
        matrix3D.M34 = -1.0;
        matrix3D.OffsetZ = perspectiveCamera.NearPlaneDistance * num4;
        matrix3D.M44 = 0.0;
      }
      else
      {
        OrthographicCamera orthographicCamera;
        if ((orthographicCamera = camera as OrthographicCamera) != null)
        {
          double num1 = width / height;
          double width1 = orthographicCamera.Width;
          double num2 = orthographicCamera.Width / num1;
          double num3 = -1.0;
          if (!double.IsPositiveInfinity(orthographicCamera.FarPlaneDistance))
            num3 = 1.0 / (orthographicCamera.NearPlaneDistance - orthographicCamera.FarPlaneDistance);
          matrix3D.M11 = 2.0 / orthographicCamera.Width;
          matrix3D.M22 = 2.0 / num2;
          matrix3D.M33 = num3;
          matrix3D.OffsetZ = orthographicCamera.NearPlaneDistance * num3;
          matrix3D.M44 = 1.0;
        }
        else
        {
          MatrixCamera matrixCamera;
          if ((matrixCamera = camera as MatrixCamera) != null)
            matrix3D = matrixCamera.ViewMatrix;
        }
      }
      return matrix3D;
    }

    protected enum DragBehaviors
    {
      UseAdornedElement,
      UseSelectionSet,
    }
  }
}
