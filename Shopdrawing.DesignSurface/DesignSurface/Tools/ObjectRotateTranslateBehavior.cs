// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.ObjectRotateTranslateBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class ObjectRotateTranslateBehavior : AdornedToolBehavior3D
  {
    private ObjectRotateTranslateBehavior.MovementMode mouseMovementMode = ObjectRotateTranslateBehavior.MovementMode.Rotate;
    private Point3D hitPoint;

    protected override string UndoUnitNameString
    {
      get
      {
        return StringTable.UndoUnitRotateTranslateObject;
      }
    }

    protected override AdornedToolBehavior3D.DragBehaviors DragBehavior
    {
      get
      {
        return AdornedToolBehavior3D.DragBehaviors.UseSelectionSet;
      }
    }

    public ObjectRotateTranslateBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
      this.PopSelfOnButtonUp = false;
    }

    public ObjectRotateTranslateBehavior(ToolBehaviorContext toolContext, bool popSelfOnButtonUp)
      : base(toolContext)
    {
      this.PopSelfOnButtonUp = popSelfOnButtonUp;
    }

    protected override void ButtonDownAction()
    {
      this.hitPoint = this.GetHitPoint();
      this.SetMovementMode();
    }

    protected override void UpdateAltKeyState()
    {
      this.SetMovementMode();
    }

    protected override void UpdateControlKeyState()
    {
      this.SetMovementMode();
    }

    private void SetMovementMode()
    {
      this.mouseMovementMode = ObjectRotateTranslateBehavior.MovementMode.TranslateXY;
      if (this.ControlKeyDepressed)
      {
        this.mouseMovementMode = ObjectRotateTranslateBehavior.MovementMode.Rotate;
      }
      else
      {
        if (!this.AltKeyDepressed)
          return;
        this.mouseMovementMode = ObjectRotateTranslateBehavior.MovementMode.TranslateXZ;
      }
    }

    protected override void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta)
    {
      Camera camera = (Camera) selected3DElement.Viewport.Camera.ViewObject.PlatformSpecificObject;
      Matrix3D matrix3D1 = Helper3D.CameraRotationMatrix(camera);
      Matrix3D matrix3D2 = camera.Transform.Value;
      if (matrix3D2.HasInverse)
      {
        matrix3D2.Invert();
        matrix3D1 *= matrix3D2;
      }
      Vector3D vector1 = new Vector3D(matrix3D1.M11, matrix3D1.M21, matrix3D1.M31);
      Vector3D vector2_1 = new Vector3D(matrix3D1.M12, matrix3D1.M22, matrix3D1.M32);
      Vector3D vector2_2 = new Vector3D(matrix3D1.M13, matrix3D1.M23, matrix3D1.M33);
      Base3DElement base3Delement = selected3DElement.Parent as Base3DElement;
      Matrix3D matrix3D3 = Matrix3D.Identity;
      if (base3Delement != null)
      {
        matrix3D3 = base3Delement.GetComputedTransformFromRoot3DElementToElement();
        matrix3D3.Invert();
      }
      if (this.mouseMovementMode == ObjectRotateTranslateBehavior.MovementMode.Rotate)
      {
        mousePositionDelta /= 2.0;
        Vector3D axisOfRotation = Vector3D.CrossProduct(new Vector3D(-mousePositionDelta.X, mousePositionDelta.Y, 0.0), vector2_2);
        double length = axisOfRotation.Length;
        if (length <= 0.0)
          return;
        Vector3D vector3D = Helper3D.EulerAnglesFromQuaternion(new Quaternion(axisOfRotation, length) * Helper3D.QuaternionFromEulerAngles(selected3DElement.CanonicalRotationAngles));
        vector3D = new Vector3D(RoundingHelper.RoundAngle(vector3D.X), RoundingHelper.RoundAngle(vector3D.Y), RoundingHelper.RoundAngle(vector3D.Z));
        selected3DElement.CanonicalRotationAngles = vector3D;
      }
      else
      {
        Vector3D vector3D1 = new Vector3D(selected3DElement.CanonicalTranslationX, selected3DElement.CanonicalTranslationY, selected3DElement.CanonicalTranslationZ);
        Point lastMousePosition = this.LastMousePosition;
        Point endPoint = lastMousePosition + mousePositionDelta;
        Vector3D vector3D2;
        if (this.mouseMovementMode == ObjectRotateTranslateBehavior.MovementMode.TranslateXY)
        {
          Plane3D plane = new Plane3D(Vector3D.CrossProduct(vector1, vector2_1), this.hitPoint);
          Vector3D vector = Helper3D.VectorBetweenPointsOnPlane((Viewport3D) selected3DElement.Viewport.ViewObject.PlatformSpecificObject, plane, lastMousePosition, endPoint);
          Vector3D vector3D3 = matrix3D3.Transform(vector);
          vector3D2 = vector3D1 + vector3D3;
        }
        else
        {
          double scale = this.Scale;
          vector3D2 = vector3D1 + scale * -mousePositionDelta.Y * vector2_2;
        }
        selected3DElement.CanonicalTranslationX = RoundingHelper.RoundLength(vector3D2.X);
        selected3DElement.CanonicalTranslationY = RoundingHelper.RoundLength(vector3D2.Y);
        selected3DElement.CanonicalTranslationZ = RoundingHelper.RoundLength(vector3D2.Z);
      }
    }

    private enum MovementMode
    {
      TranslateXY,
      TranslateXZ,
      Rotate,
    }
  }
}
