// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.Base3DElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using System;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class Base3DElement : SceneElement
  {
    public static readonly IPropertyId RotateTransform3DRotationProperty = (IPropertyId) PlatformTypes.RotateTransform3D.GetMember(MemberType.LocalProperty, "Rotation", MemberAccessTypes.Public);
    private PropertyReference transformPropertyReference;

    public Viewport3DElement Viewport
    {
      get
      {
        SceneElement sceneElement = (SceneElement) this;
        Viewport3DElement viewport3Delement;
        do
        {
          sceneElement = sceneElement.ParentElement;
          viewport3Delement = sceneElement as Viewport3DElement;
        }
        while (viewport3Delement == null && sceneElement != null);
        return viewport3Delement;
      }
    }

    public int DepthFromViewport3D
    {
      get
      {
        int num = 0;
        for (SceneElement sceneElement = (SceneElement) this; sceneElement.ParentElement != null && !(sceneElement is Viewport3DElement); sceneElement = sceneElement.ParentElement)
        {
          if (sceneElement is Model3DGroupElement || sceneElement is Visual3DElement)
            ++num;
        }
        return num;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        bool flag = base.IsSelectable;
        if (flag && this.ViewModel.AnimationEditor.ActiveVisualTrigger != null)
          flag = false;
        return flag;
      }
    }

    public abstract Transform3D Transform { get; set; }

    public abstract Rect3D LocalSpaceBounds { get; }

    public abstract IPropertyId TransformPropertyId { get; }

    public ScaleTransform3D CanonicalScaleTransform3D
    {
      get
      {
        return (ScaleTransform3D) this.GetComputedValue(this.Transform3DScaleTransform);
      }
      set
      {
        this.SetValue(this.Transform3DScaleTransform, (object) value);
      }
    }

    public double CanonicalScaleX
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DScaleX);
      }
      set
      {
        this.SetValue(this.Transform3DScaleX, (object) value);
      }
    }

    public double CanonicalScaleY
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DScaleY);
      }
      set
      {
        this.SetValue(this.Transform3DScaleY, (object) value);
      }
    }

    public double CanonicalScaleZ
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DScaleZ);
      }
      set
      {
        this.SetValue(this.Transform3DScaleZ, (object) value);
      }
    }

    public RotateTransform3D CanonicalRotateTransform3D
    {
      get
      {
        return (RotateTransform3D) this.GetComputedValue(this.Transform3DRotateTransform);
      }
      set
      {
        this.SetValue(this.Transform3DRotateTransform, (object) value);
      }
    }

    public Vector3D CanonicalRotationAngles
    {
      get
      {
        if (!this.ViewModel.AnimationEditor.IsKeyFraming || this.ViewModel.IsForcingBaseValue)
          return (Vector3D) this.GetComputedValue(this.TransformRotationAngles);
        Rotation3D rotation = this.CanonicalRotateTransform3D.Rotation;
        if (rotation != null)
          return Helper3D.EulerAnglesFromQuaternion(Helper3D.QuaternionFromRotation3D(rotation));
        return new Vector3D();
      }
      set
      {
        this.SetValue(this.TransformRotationAngles, (object) value);
      }
    }

    public TranslateTransform3D CanonicalTranslateTransform3D
    {
      get
      {
        return (TranslateTransform3D) this.GetComputedValue(this.Transform3DTranslateTransform);
      }
      set
      {
        this.SetValue(this.Transform3DTranslateTransform, (object) value);
      }
    }

    public double CanonicalTranslationX
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DTranslationOffsetX);
      }
      set
      {
        this.SetValue(this.Transform3DTranslationOffsetX, (object) value);
      }
    }

    public double CanonicalTranslationY
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DTranslationOffsetY);
      }
      set
      {
        this.SetValue(this.Transform3DTranslationOffsetY, (object) value);
      }
    }

    public double CanonicalTranslationZ
    {
      get
      {
        return (double) this.GetComputedValue(this.Transform3DTranslationOffsetZ);
      }
      set
      {
        this.SetValue(this.Transform3DTranslationOffsetZ, (object) value);
      }
    }

    private PropertyReference TransformPropertyReference
    {
      get
      {
        if (this.transformPropertyReference == null)
          this.transformPropertyReference = new PropertyReference((ReferenceStep) this.ProjectContext.ResolveProperty(this.TransformPropertyId));
        return this.transformPropertyReference;
      }
    }

    private PropertyReference Transform3DRotateTransform
    {
      get
      {
        return this.TransformPropertyReference.Append(this.Platform.Metadata.CommonProperties.RotateTransform3DReference);
      }
    }

    private PropertyReference TransformRotationAngles
    {
      get
      {
        return this.Transform3DRotateTransform.Append((ReferenceStep) DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.EulerAnglesProperty, (IPlatformMetadata) this.Platform.Metadata));
      }
    }

    private PropertyReference Transform3DScaleTransform
    {
      get
      {
        return this.TransformPropertyReference.Append(this.Platform.Metadata.CommonProperties.ScaleTransform3DReference);
      }
    }

    private PropertyReference Transform3DScaleX
    {
      get
      {
        return this.Transform3DScaleTransform.Append(ScaleTransform3DProperties.ScaleXProperty);
      }
    }

    private PropertyReference Transform3DScaleY
    {
      get
      {
        return this.Transform3DScaleTransform.Append(ScaleTransform3DProperties.ScaleYProperty);
      }
    }

    private PropertyReference Transform3DScaleZ
    {
      get
      {
        return this.Transform3DScaleTransform.Append(ScaleTransform3DProperties.ScaleZProperty);
      }
    }

    private PropertyReference Transform3DTranslateTransform
    {
      get
      {
        return this.TransformPropertyReference.Append(this.Platform.Metadata.CommonProperties.TranslateTransform3DReference);
      }
    }

    private PropertyReference Transform3DTranslationOffsetX
    {
      get
      {
        return this.Transform3DTranslateTransform.Append(TranslateTransform3DProperties.OffsetXProperty);
      }
    }

    private PropertyReference Transform3DTranslationOffsetY
    {
      get
      {
        return this.Transform3DTranslateTransform.Append(TranslateTransform3DProperties.OffsetYProperty);
      }
    }

    private PropertyReference Transform3DTranslationOffsetZ
    {
      get
      {
        return this.Transform3DTranslateTransform.Append(TranslateTransform3DProperties.OffsetZProperty);
      }
    }

    public override IViewObject ViewTargetElement
    {
      get
      {
        Viewport3DElement viewport = this.Viewport;
        if (viewport != null)
          return viewport.ViewTargetElement;
        return base.ViewTargetElement;
      }
    }

    protected override void ModifyValue(PropertyReference propertyReference, object valueToSet, SceneNode.Modification modification, int index)
    {
      bool flag = DesignTimeProperties.EulerAnglesProperty.Equals((object) propertyReference.LastStep);
      if (!flag || !this.ViewModel.AnimationEditor.IsKeyFraming || this.ViewModel.IsForcingBaseValue)
        base.ModifyValue(propertyReference, valueToSet, modification, index);
      if (!flag)
        return;
      propertyReference = propertyReference.Subreference(0, propertyReference.Count - 2);
      propertyReference = propertyReference.Append(Base3DElement.RotateTransform3DRotationProperty);
      if (valueToSet is Vector3D)
      {
        Quaternion quaternion = Helper3D.QuaternionFromEulerAngles((Vector3D) valueToSet);
        valueToSet = (object) new AxisAngleRotation3D(RoundingHelper.RoundDirection(quaternion.Axis), RoundingHelper.RoundAngle(quaternion.Angle));
      }
      base.ModifyValue(propertyReference, valueToSet, modification, index);
    }

    protected override bool ShouldUseTrigger(BaseTriggerNode trigger, IPropertyId propertyKey)
    {
      return false;
    }

    public Matrix3D GetComputedTransformFromRoot3DElementToElement()
    {
      Base3DElement base3Delement = this;
      Matrix3D identity = Matrix3D.Identity;
      for (; base3Delement != null; base3Delement = base3Delement.ParentElement as Base3DElement)
      {
        Matrix3D matrix3D = base3Delement.Transform.Value;
        identity *= matrix3D;
      }
      return identity;
    }

    public Matrix3D GetComputedTransformFromViewport3DToElement()
    {
      Matrix3D delementToElement = this.GetComputedTransformFromRoot3DElementToElement();
      Viewport3DElement viewport = this.Viewport;
      if (viewport != null && viewport.Camera != null)
      {
        Matrix3D matrix3D = viewport.Camera.Transform.Value;
        if (matrix3D.HasInverse)
        {
          matrix3D.Invert();
          delementToElement *= matrix3D;
        }
      }
      return delementToElement;
    }

    public Matrix3D GetComputedTransformFromViewport3DToElement(Point3D centerOffset)
    {
      Matrix3D viewport3DtoElement = this.GetComputedTransformFromViewport3DToElement();
      viewport3DtoElement.TranslatePrepend((Vector3D) centerOffset);
      return viewport3DtoElement;
    }

    protected static Rect3D TransformAxisAligned(Matrix3D matrix, Rect3D boundingBox)
    {
      if (boundingBox.IsEmpty)
        return boundingBox;
      double x1 = boundingBox.X;
      double y1 = boundingBox.Y;
      double z1 = boundingBox.Z;
      double x2 = x1 + boundingBox.SizeX;
      double y2 = y1 + boundingBox.SizeY;
      double z2 = z1 + boundingBox.SizeZ;
      Point3D[] points = new Point3D[8]
      {
        new Point3D(x1, y1, z1),
        new Point3D(x1, y1, z2),
        new Point3D(x1, y2, z1),
        new Point3D(x1, y2, z2),
        new Point3D(x2, y1, z1),
        new Point3D(x2, y1, z2),
        new Point3D(x2, y2, z1),
        new Point3D(x2, y2, z2)
      };
      matrix.Transform(points);
      double num1 = points[0].X;
      double num2 = points[0].Y;
      double num3 = points[0].Z;
      double val1_1 = points[0].X;
      double val1_2 = points[0].Y;
      double val1_3 = points[0].Z;
      for (int index = 1; index < points.Length; ++index)
      {
        num1 = Math.Min(num1, points[index].X);
        num2 = Math.Min(num2, points[index].Y);
        num3 = Math.Min(num3, points[index].Z);
        val1_1 = Math.Max(val1_1, points[index].X);
        val1_2 = Math.Max(val1_2, points[index].Y);
        val1_3 = Math.Max(val1_3, points[index].Z);
      }
      return new Rect3D(num1, num2, num3, val1_1 - num1, val1_2 - num2, val1_3 - num3);
    }
  }
}
