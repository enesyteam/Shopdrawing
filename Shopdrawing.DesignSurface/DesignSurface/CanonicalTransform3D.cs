// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CanonicalTransform3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface
{
  public class CanonicalTransform3D : ICloneable
  {
    private static DependencyProperty EulerAnglesProperty;
    private Transform3DGroup transformGroup;
    private Point3D center;
    private Vector3D scale;
    private Vector3D translation;

    public Point3D Center
    {
      get
      {
        return this.center;
      }
      set
      {
        this.SetCenter(value);
      }
    }

    public double CenterX
    {
      get
      {
        return this.center.X;
      }
      set
      {
        this.SetCenter(new Point3D(value, this.center.Y, this.center.Z));
      }
    }

    public double CenterY
    {
      get
      {
        return this.center.Y;
      }
      set
      {
        this.SetCenter(new Point3D(this.center.X, value, this.center.Z));
      }
    }

    public double CenterZ
    {
      get
      {
        return this.center.Z;
      }
      set
      {
        this.SetCenter(new Point3D(this.center.X, this.center.Y, value));
      }
    }

    public Vector3D Scale
    {
      get
      {
        return this.scale;
      }
      set
      {
        this.SetScale(value);
      }
    }

    public ScaleTransform3D ScaleTransform
    {
      get
      {
        return this.transformGroup.Children[1] as ScaleTransform3D;
      }
      set
      {
        this.SetScale(new Vector3D(value.ScaleX, value.ScaleY, value.ScaleZ));
      }
    }

    public double ScaleX
    {
      get
      {
        return this.scale.X;
      }
      set
      {
        this.SetScale(new Vector3D(value, this.scale.Y, this.scale.Z));
      }
    }

    public double ScaleY
    {
      get
      {
        return this.scale.Y;
      }
      set
      {
        this.SetScale(new Vector3D(this.scale.X, value, this.scale.Z));
      }
    }

    public double ScaleZ
    {
      get
      {
        return this.scale.Z;
      }
      set
      {
        this.SetScale(new Vector3D(this.scale.X, this.scale.Y, value));
      }
    }

    public Vector3D RotationAngles
    {
      get
      {
        return (Vector3D) this.RotateTransform.GetValue(CanonicalTransform3D.EulerAnglesProperty);
      }
      set
      {
        this.RotateTransform.SetValue(CanonicalTransform3D.EulerAnglesProperty, (object) value);
        this.RotateTransform.Rotation = (Rotation3D) new AxisAngleRotation3D(RoundingHelper.RoundDirection(this.RotationQuaternion.Axis), RoundingHelper.RoundAngle(this.RotationQuaternion.Angle));
      }
    }

    public double RotationAngleX
    {
      get
      {
        return this.RotationAngles.X;
      }
      set
      {
        this.SetRotation(value, CanonicalTransform3D.XYZEnum.X);
      }
    }

    public double RotationAngleY
    {
      get
      {
        return this.RotationAngles.Y;
      }
      set
      {
        this.SetRotation(value, CanonicalTransform3D.XYZEnum.Y);
      }
    }

    public double RotationAngleZ
    {
      get
      {
        return this.RotationAngles.Z;
      }
      set
      {
        this.SetRotation(value, CanonicalTransform3D.XYZEnum.Z);
      }
    }

    public Quaternion RotationQuaternion
    {
      get
      {
        return Helper3D.QuaternionFromEulerAngles(this.RotationAngles);
      }
    }

    public RotateTransform3D RotateTransform
    {
      get
      {
        return this.transformGroup.Children[2] as RotateTransform3D;
      }
      set
      {
        throw new InvalidOperationException(ExceptionStringTable.ThisShouldNeverBeCalledButIsNeccesaryForReferenceSteps);
      }
    }

    public Vector3D Translation
    {
      get
      {
        return this.translation;
      }
      set
      {
        this.SetTranslation(value);
      }
    }

    public TranslateTransform3D TranslateTransform
    {
      get
      {
        return this.transformGroup.Children[4] as TranslateTransform3D;
      }
      set
      {
        this.SetTranslation(new Vector3D(value.OffsetX, value.OffsetY, value.OffsetZ));
      }
    }

    public double TranslationX
    {
      get
      {
        return this.Translation.X;
      }
      set
      {
        this.SetTranslation(new Vector3D(value, this.translation.Y, this.translation.Z));
      }
    }

    public double TranslationY
    {
      get
      {
        return this.Translation.Y;
      }
      set
      {
        this.SetTranslation(new Vector3D(this.translation.X, value, this.translation.Z));
      }
    }

    public double TranslationZ
    {
      get
      {
        return this.Translation.Z;
      }
      set
      {
        this.SetTranslation(new Vector3D(this.translation.X, this.translation.Y, value));
      }
    }

    public CanonicalTransform3D()
    {
      this.Initialize();
    }

    public CanonicalTransform3D(CanonicalTransform3D transformToCopy)
    {
      if (transformToCopy == (CanonicalTransform3D) null)
      {
        this.Initialize();
      }
      else
      {
        this.center = transformToCopy.center;
        this.scale = transformToCopy.scale;
        this.translation = transformToCopy.translation;
        this.InitializeTransformGroup();
        this.RotationAngles = transformToCopy.RotationAngles;
      }
    }

    public CanonicalTransform3D(Transform3D transform)
    {
      if (transform == null)
        this.Initialize();
      else
        this.ReadTransform(transform, false);
    }

    public CanonicalTransform3D(Matrix3D mat)
      : this((Transform3D) new MatrixTransform3D(mat))
    {
    }

    private CanonicalTransform3D(Transform3D transform, bool useIfChangeable)
    {
      if (transform == null)
        this.Initialize();
      else
        this.ReadTransform(transform, useIfChangeable);
    }

    public static explicit operator Transform3D(CanonicalTransform3D value)
    {
      if (value != (CanonicalTransform3D) null)
        return (Transform3D) value.ToTransform();
      return (Transform3D) null;
    }

    public static explicit operator CanonicalTransform3D(Transform3D value)
    {
      return new CanonicalTransform3D(value, true);
    }

    public static bool operator ==(CanonicalTransform3D ct1, CanonicalTransform3D ct2)
    {
      bool flag1 = ct1 == null;
      bool flag2 = ct2 == null;
      if (flag1 && flag2)
        return true;
      if (!flag1 && !flag2 && (ct1.center == ct2.center && ct1.scale == ct2.scale) && ct1.RotationAngles == ct2.RotationAngles)
        return ct1.translation == ct2.translation;
      return false;
    }

    public static bool operator !=(CanonicalTransform3D ct1, CanonicalTransform3D ct2)
    {
      return !(ct1 == ct2);
    }

    internal static void Initialize(DesignerContext designerContext)
    {
      CanonicalTransform3D.EulerAnglesProperty = (DependencyProperty) DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.EulerAnglesProperty, (IPlatformMetadata) designerContext.DesignerDefaultPlatformService.DefaultPlatform.Metadata).DependencyProperty;
    }

    public override bool Equals(object obj)
    {
      return this == obj as CanonicalTransform3D;
    }

    public static bool Compare(CanonicalTransform3D ct1, CanonicalTransform3D ct2)
    {
      return ct1 == ct2;
    }

    public override string ToString()
    {
      return "toCenter(" + (object) -this.center.X + ", " + (string) (object) -this.center.Y + ", " + (string) (object) -this.center.Z + ") scale(" + (string) (object) this.scale.X + ", " + (string) (object) this.scale.Y + ", " + (string) (object) this.scale.Z + ") rotationValues(" + (string) (object) this.RotationAngles.X + ", " + (string) (object) this.RotationAngles.Y + ", " + (string) (object) this.RotationAngles.Z + ") backFromCenter(" + (string) (object) this.center.X + ", " + (string) (object) this.center.Y + ", " + (string) (object) this.center.Z + ") translate(" + (string) (object) this.translation.X + ", " + (string) (object) this.translation.Y + ", " + (string) (object) this.translation.Z + ")";
    }

    public override int GetHashCode()
    {
      return this.center.GetHashCode() ^ this.scale.GetHashCode() ^ this.RotationAngles.GetHashCode() ^ this.translation.GetHashCode();
    }

    public Transform3DGroup ToTransform()
    {
      return this.transformGroup;
    }

    public object Clone()
    {
      return (object) new CanonicalTransform3D(this);
    }

    public void ChangeCenter(Point3D newCenter)
    {
      Point3D point3D1 = new Point3D(0.0, 0.0, 0.0);
      Point3D point3D2 = point3D1 * this.ToTransform().Value;
      this.Center = newCenter;
      Point3D point3D3 = point3D1 * this.ToTransform().Value;
      this.Translation += point3D2 - point3D3;
    }

    public void ApplyScale(Vector3D scale, Point3D fixedPoint)
    {
      this.Translation += new Vector3D((1.0 - scale.X) * (fixedPoint.X - this.center.X), (1.0 - scale.Y) * (fixedPoint.Y - this.center.Y), (1.0 - scale.Z) * (fixedPoint.Z - this.center.Z)) * this.ToTransform().Value;
      this.Scale = new Vector3D(this.scale.X * scale.X, this.scale.Y * scale.Y, this.scale.Z * scale.Z);
    }

    public static bool IsCanonical(Transform3D transform)
    {
      Transform3DGroup transform3Dgroup = transform as Transform3DGroup;
      if (transform3Dgroup != null && transform3Dgroup.Children.Count == 5)
      {
        TranslateTransform3D translateTransform3D1 = transform3Dgroup.Children[0] as TranslateTransform3D;
        ScaleTransform3D scaleTransform3D = transform3Dgroup.Children[1] as ScaleTransform3D;
        RotateTransform3D rotateTransform3D = transform3Dgroup.Children[2] as RotateTransform3D;
        TranslateTransform3D translateTransform3D2 = transform3Dgroup.Children[3] as TranslateTransform3D;
        TranslateTransform3D translateTransform3D3 = transform3Dgroup.Children[4] as TranslateTransform3D;
        if (translateTransform3D1 != null && scaleTransform3D != null && (rotateTransform3D != null && translateTransform3D2 != null) && (translateTransform3D3 != null && translateTransform3D1.OffsetX == -translateTransform3D2.OffsetX && (translateTransform3D1.OffsetY == -translateTransform3D2.OffsetY && translateTransform3D1.OffsetZ == -translateTransform3D2.OffsetZ)))
          return true;
      }
      return false;
    }

    private void Initialize()
    {
      this.center = new Point3D(0.0, 0.0, 0.0);
      this.scale = new Vector3D(1.0, 1.0, 1.0);
      this.translation = new Vector3D(0.0, 0.0, 0.0);
      this.InitializeTransformGroup();
      this.RotationAngles = new Vector3D(0.0, 0.0, 0.0);
    }

    private void InitializeTransformGroup()
    {
      if (this.transformGroup == null)
      {
        Transform3D[] transform3DArray = new Transform3D[5]
        {
          (Transform3D) new TranslateTransform3D(-this.center.X, -this.center.Y, -this.center.Z),
          (Transform3D) new ScaleTransform3D(this.scale),
          (Transform3D) new RotateTransform3D(),
          (Transform3D) new TranslateTransform3D(this.center.X, this.center.Y, this.center.Z),
          (Transform3D) new TranslateTransform3D(this.translation)
        };
        this.transformGroup = new Transform3DGroup();
        this.transformGroup.Children = new Transform3DCollection((IEnumerable<Transform3D>) transform3DArray);
      }
      else
      {
        TranslateTransform3D translateTransform3D1 = (TranslateTransform3D) this.transformGroup.Children[0];
        translateTransform3D1.OffsetX = -this.center.X;
        translateTransform3D1.OffsetY = -this.center.Y;
        translateTransform3D1.OffsetZ = -this.center.Z;
        ScaleTransform3D scaleTransform3D = (ScaleTransform3D) this.transformGroup.Children[1];
        scaleTransform3D.ScaleX = this.scale.X;
        scaleTransform3D.ScaleY = this.scale.Y;
        scaleTransform3D.ScaleZ = this.scale.Z;
        TranslateTransform3D translateTransform3D2 = (TranslateTransform3D) this.transformGroup.Children[3];
        translateTransform3D2.OffsetX = this.center.X;
        translateTransform3D2.OffsetY = this.center.Y;
        translateTransform3D2.OffsetZ = this.center.Z;
        TranslateTransform3D translateTransform3D3 = (TranslateTransform3D) this.transformGroup.Children[4];
        translateTransform3D3.OffsetX = this.translation.X;
        translateTransform3D3.OffsetY = this.translation.Y;
        translateTransform3D3.OffsetZ = this.translation.Z;
      }
    }

    private void SetCenter(Point3D center)
    {
      this.center = center;
      this.InitializeTransformGroup();
    }

    private void SetScale(Vector3D scale)
    {
      this.scale = scale;
      this.InitializeTransformGroup();
    }

    private void SetRotation(double value, CanonicalTransform3D.XYZEnum xyz)
    {
      Vector3D rotationAngles = this.RotationAngles;
      switch (xyz)
      {
        case CanonicalTransform3D.XYZEnum.X:
          rotationAngles.X = value;
          break;
        case CanonicalTransform3D.XYZEnum.Y:
          rotationAngles.Y = value;
          break;
        case CanonicalTransform3D.XYZEnum.Z:
          rotationAngles.Z = value;
          break;
      }
      this.RotationAngles = rotationAngles;
    }

    private void SetTranslation(Vector3D translate)
    {
      this.translation = translate;
      this.InitializeTransformGroup();
    }

    private Quaternion MatrixToQuaternion(Matrix3D matrix)
    {
      double d = 1.0 + matrix.M11 + matrix.M22 + matrix.M33;
      double w;
      double x;
      double y;
      double z;
      if (d > 1E-09)
      {
        double num = 0.5 / Math.Sqrt(d);
        w = 0.25 / num;
        x = (matrix.M23 - matrix.M32) * num;
        y = (matrix.M31 - matrix.M13) * num;
        z = (matrix.M12 - matrix.M21) * num;
      }
      else if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M22)
      {
        double num = 2.0 * Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
        x = 0.25 * num;
        y = (matrix.M21 + matrix.M12) / num;
        z = (matrix.M31 + matrix.M13) / num;
        w = (matrix.M32 - matrix.M23) / num;
      }
      else if (matrix.M22 > matrix.M33)
      {
        double num = 2.0 * Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
        x = (matrix.M21 + matrix.M12) / num;
        y = 0.25 * num;
        z = (matrix.M32 + matrix.M23) / num;
        w = (matrix.M31 - matrix.M13) / num;
      }
      else
      {
        double num = 2.0 * Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
        x = (matrix.M31 + matrix.M13) / num;
        y = (matrix.M32 + matrix.M23) / num;
        z = 0.25 * num;
        w = (matrix.M21 - matrix.M12) / num;
      }
      Quaternion quaternion = new Quaternion(x, y, z, w);
      quaternion.Normalize();
      return quaternion;
    }

    private void ReadTransform(Transform3D transform, bool useIfChangeable)
    {
      if (this.ReadCanonicalForm(transform, useIfChangeable))
        return;
      this.ConvertGenericTransform(transform);
    }

    private bool ReadCanonicalForm(Transform3D transform, bool useIfChangeable)
    {
      if (!CanonicalTransform3D.IsCanonical(transform))
        return false;
      Transform3DGroup transform3Dgroup = (Transform3DGroup) transform;
      ScaleTransform3D scaleTransform3D = transform3Dgroup.Children[1] as ScaleTransform3D;
      RotateTransform3D rotateTransform3D = transform3Dgroup.Children[2] as RotateTransform3D;
      TranslateTransform3D translateTransform3D1 = transform3Dgroup.Children[3] as TranslateTransform3D;
      TranslateTransform3D translateTransform3D2 = transform3Dgroup.Children[4] as TranslateTransform3D;
      this.center = new Point3D(translateTransform3D1.OffsetX, translateTransform3D1.OffsetY, translateTransform3D1.OffsetZ);
      this.scale = new Vector3D(scaleTransform3D.ScaleX, scaleTransform3D.ScaleY, scaleTransform3D.ScaleZ);
      this.translation = new Vector3D(translateTransform3D2.OffsetX, translateTransform3D2.OffsetY, translateTransform3D2.OffsetZ);
      if (useIfChangeable && !transform3Dgroup.IsFrozen)
      {
        this.transformGroup = transform3Dgroup;
      }
      else
      {
        this.InitializeTransformGroup();
        Vector3D? nullable = (Vector3D?) rotateTransform3D.GetValue(CanonicalTransform3D.EulerAnglesProperty);
        bool flag = nullable.HasValue;
        AxisAngleRotation3D axisAngleRotation3D = rotateTransform3D.Rotation as AxisAngleRotation3D;
        Quaternion orientation = Quaternion.Identity;
        if (axisAngleRotation3D != null)
          orientation = new Quaternion(axisAngleRotation3D.Axis, axisAngleRotation3D.Angle);
        if (flag)
        {
          Quaternion quaternion = Helper3D.QuaternionFromEulerAngles(nullable.Value);
          if (!Tolerances.AreClose(quaternion.Angle, orientation.Angle) || !Tolerances.AreClose(quaternion.Axis, orientation.Axis))
            flag = false;
        }
        if (!flag)
          nullable = new Vector3D?(Helper3D.EulerAnglesFromQuaternion(orientation));
        this.RotationAngles = nullable.Value;
      }
      return true;
    }

    private void ConvertGenericTransform(Transform3D transform)
    {
      Point3D point3D1 = new Point3D(0.0, 0.0, 0.0);
      this.Initialize();
      ScaleTransform3D scaleTransform3D = transform as ScaleTransform3D;
      if (scaleTransform3D != null)
      {
        this.scale = new Vector3D(scaleTransform3D.ScaleX, scaleTransform3D.ScaleY, scaleTransform3D.ScaleZ);
        this.center = new Point3D(scaleTransform3D.CenterX, scaleTransform3D.CenterY, scaleTransform3D.CenterZ);
      }
      else
      {
        Point3D point3D2 = point3D1 * transform.Value;
        this.translation = new Vector3D(point3D2.X, point3D2.Y, point3D2.Z);
        Matrix3D rotation;
        Vector3D scale;
        Matrix3DOperations.DecomposeIntoRotationAndScale(transform.Value, out rotation, out scale);
        this.scale = scale;
        this.RotationAngles = Helper3D.EulerAnglesFromQuaternion(this.MatrixToQuaternion(rotation));
      }
      this.InitializeTransformGroup();
    }

    private enum XYZEnum
    {
      X,
      Y,
      Z,
    }
  }
}
