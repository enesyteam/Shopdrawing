// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateBehavior3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RotateBehavior3D : AdornedToolBehavior3D
  {
    private static readonly double shiftSnapAngle = 15.0;
    private const double Epsilon = 1E-06;
    private Vector mouseMovementAxis;
    private double lastUnsnappedAngle;
    private Vector3D rotationAxis;
    private Quaternion initialQuaternion;
    private Quaternion previousQuaternion;
    private double? previousAngle;

    protected RotateAdorner3D ActiveAdorner
    {
      get
      {
        return (RotateAdorner3D) base.ActiveAdorner;
      }
    }

    protected override string UndoUnitNameString
    {
      get
      {
        return StringTable.UndoUnitRotateObject;
      }
    }

    public RotateBehavior3D(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void ButtonDownAction()
    {
      Vector3D vector = new Vector3D();
      switch (this.ActiveAdorner.Axis)
      {
        case Adorner3D.TransformVia.XAxis:
          vector = new Vector3D(0.0, -AdornedToolBehavior3D.sqrt2div2, AdornedToolBehavior3D.sqrt2div2);
          break;
        case Adorner3D.TransformVia.YAxis:
          vector = new Vector3D(AdornedToolBehavior3D.sqrt2div2, 0.0, -AdornedToolBehavior3D.sqrt2div2);
          break;
        case Adorner3D.TransformVia.ZAxis:
          vector = new Vector3D(-AdornedToolBehavior3D.sqrt2div2, AdornedToolBehavior3D.sqrt2div2, 0.0);
          break;
        default:
          vector = new Vector3D(0.0, AdornedToolBehavior3D.sqrt2div2, -AdornedToolBehavior3D.sqrt2div2);
          break;
      }
      this.mouseMovementAxis = AdornedToolBehavior3D.Vector3DInViewport3D(this.Selected3DElement, vector);
      this.mouseMovementAxis.Normalize();
      this.rotationAxis = this.ActiveAdorner.RotationAxis;
      this.initialQuaternion = Helper3D.QuaternionFromEulerAngles(this.Selected3DElement.CanonicalRotationAngles);
      this.previousQuaternion = this.initialQuaternion;
      this.previousAngle = new double?();
      this.lastUnsnappedAngle = 0.0;
    }

    protected override void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta)
    {
      this.lastUnsnappedAngle += this.ActiveView.Zoom * this.mouseMovementAxis * mousePositionDelta;
      double angleInDegrees1 = this.lastUnsnappedAngle;
      if (this.ShiftKeyDepressed)
        angleInDegrees1 = RotateBehavior3D.shiftSnapAngle * Math.Round(angleInDegrees1 / RotateBehavior3D.shiftSnapAngle);
      double angleInDegrees2 = !this.previousAngle.HasValue ? angleInDegrees1 : angleInDegrees1 - this.previousAngle.Value;
      this.previousAngle = new double?(angleInDegrees1);
      Vector3D unitEulerAngles = RotateBehavior3D.GetUnitEulerAngles(Helper3D.EulerAnglesFromQuaternion(this.previousQuaternion * new Quaternion(this.rotationAxis, angleInDegrees2)) - selected3DElement.CanonicalRotationAngles);
      this.previousQuaternion = this.initialQuaternion * new Quaternion(this.rotationAxis, angleInDegrees1);
      Vector3D vector3D = selected3DElement.CanonicalRotationAngles + unitEulerAngles;
      vector3D = new Vector3D(RoundingHelper.RoundAngle(vector3D.X), RoundingHelper.RoundAngle(vector3D.Y), RoundingHelper.RoundAngle(vector3D.Z));
      selected3DElement.CanonicalRotationAngles = vector3D;
    }

    private static Vector3D GetUnitEulerAngles(Vector3D eulerAngles)
    {
      if (eulerAngles.X >= 180.000001)
        eulerAngles.X = RotateBehavior3D.Remainder(eulerAngles.X + 180.0, 360.0) - 180.0;
      if (eulerAngles.Y >= 180.000001)
        eulerAngles.Y = RotateBehavior3D.Remainder(eulerAngles.Y + 180.0, 360.0) - 180.0;
      if (eulerAngles.Z >= 180.000001)
        eulerAngles.Z = RotateBehavior3D.Remainder(eulerAngles.Z + 180.0, 360.0) - 180.0;
      if (eulerAngles.X <= -180.000001)
        eulerAngles.X = RotateBehavior3D.Remainder(eulerAngles.X - 180.0, 360.0) + 180.0;
      if (eulerAngles.Y <= -180.000001)
        eulerAngles.Y = RotateBehavior3D.Remainder(eulerAngles.Y - 180.0, 360.0) + 180.0;
      if (eulerAngles.Z <= -180.000001)
        eulerAngles.Z = RotateBehavior3D.Remainder(eulerAngles.Z - 180.0, 360.0) + 180.0;
      return eulerAngles;
    }

    private static double Remainder(double num, double denom)
    {
      return num - Math.Truncate(num / denom) * denom;
    }

    private delegate Vector3D Vector3DTransformer(Vector3D input);
  }
}
