// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CanonicalTransform
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface
{
  public class CanonicalTransform : ICanonicalTransform, ICloneable
  {
    private TransformGroup transformGroup;
    private CanonicalDecomposition decomposition;

    internal CanonicalDecomposition Decomposition
    {
      get
      {
        return this.decomposition;
      }
    }

    public TransformGroup TransformGroup
    {
      get
      {
        return this.transformGroup;
      }
    }

    public Point Center
    {
      get
      {
        return this.decomposition.Center;
      }
      set
      {
        if (value.X != 0.0 || value.Y != 0.0 || this.decomposition.Center.X != 0.0 || this.decomposition.Center.Y != 0.0)
        {
          this.ScaleTransform.CenterX = value.X;
          this.ScaleTransform.CenterY = value.Y;
          this.SkewTransform.CenterX = value.X;
          this.SkewTransform.CenterY = value.Y;
          this.RotateTransform.CenterX = value.X;
          this.RotateTransform.CenterY = value.Y;
        }
        this.decomposition.Center = value;
      }
    }

    public double CenterX
    {
      get
      {
        return this.decomposition.Center.X;
      }
      set
      {
        this.Center = new Point(value, this.decomposition.Center.Y);
      }
    }

    public double CenterY
    {
      get
      {
        return this.decomposition.Center.Y;
      }
      set
      {
        this.Center = new Point(this.decomposition.Center.X, value);
      }
    }

    public Vector Scale
    {
      get
      {
        return this.decomposition.Scale;
      }
      set
      {
        this.decomposition.Scale = value;
        ScaleTransform scaleTransform = this.transformGroup.Children[CanonicalTransformOrder.ScaleIndex] as ScaleTransform;
        scaleTransform.ScaleX = this.decomposition.ScaleX;
        scaleTransform.ScaleY = this.decomposition.ScaleY;
      }
    }

    public ScaleTransform ScaleTransform
    {
      get
      {
        return this.transformGroup.Children[CanonicalTransformOrder.ScaleIndex] as ScaleTransform;
      }
      set
      {
        this.transformGroup.Children[CanonicalTransformOrder.ScaleIndex] = (Transform) value;
      }
    }

    public double ScaleX
    {
      get
      {
        return this.decomposition.ScaleX;
      }
      set
      {
        this.Scale = new Vector(value, this.decomposition.ScaleY);
      }
    }

    public double ScaleY
    {
      get
      {
        return this.decomposition.ScaleY;
      }
      set
      {
        this.Scale = new Vector(this.decomposition.ScaleX, value);
      }
    }

    public Vector Skew
    {
      get
      {
        return this.decomposition.Skew;
      }
      set
      {
        this.decomposition.Skew = value;
        SkewTransform skewTransform = this.transformGroup.Children[CanonicalTransformOrder.SkewIndex] as SkewTransform;
        skewTransform.AngleX = this.decomposition.SkewX;
        skewTransform.AngleY = this.decomposition.SkewY;
      }
    }

    public SkewTransform SkewTransform
    {
      get
      {
        return this.transformGroup.Children[CanonicalTransformOrder.SkewIndex] as SkewTransform;
      }
      set
      {
        this.transformGroup.Children[CanonicalTransformOrder.SkewIndex] = (Transform) value;
      }
    }

    public double SkewX
    {
      get
      {
        return this.decomposition.SkewX;
      }
      set
      {
        this.Skew = new Vector(value, this.decomposition.SkewY);
      }
    }

    public double SkewY
    {
      get
      {
        return this.decomposition.SkewY;
      }
      set
      {
        this.Skew = new Vector(this.decomposition.SkewX, value);
      }
    }

    public double RotationAngle
    {
      get
      {
        return this.decomposition.RotationAngle;
      }
      set
      {
        this.decomposition.RotationAngle = value;
        (this.transformGroup.Children[CanonicalTransformOrder.RotateIndex] as RotateTransform).Angle = this.decomposition.RotationAngle;
      }
    }

    public RotateTransform RotateTransform
    {
      get
      {
        return this.transformGroup.Children[CanonicalTransformOrder.RotateIndex] as RotateTransform;
      }
      set
      {
        this.transformGroup.Children[CanonicalTransformOrder.RotateIndex] = (Transform) value;
      }
    }

    public Vector Translation
    {
      get
      {
        return this.decomposition.Translation;
      }
      set
      {
        this.decomposition.Translation = value;
        TranslateTransform translateTransform = this.transformGroup.Children[CanonicalTransformOrder.TranslateIndex] as TranslateTransform;
        if (translateTransform.X != this.decomposition.TranslationX)
          translateTransform.X = this.decomposition.TranslationX;
        if (translateTransform.Y == this.decomposition.TranslationY)
          return;
        translateTransform.Y = this.decomposition.TranslationY;
      }
    }

    public TranslateTransform TranslateTransform
    {
      get
      {
        return this.transformGroup.Children[CanonicalTransformOrder.TranslateIndex] as TranslateTransform;
      }
      set
      {
        this.transformGroup.Children[CanonicalTransformOrder.TranslateIndex] = (Transform) value;
      }
    }

    public double TranslationX
    {
      get
      {
        return this.decomposition.TranslationX;
      }
      set
      {
        this.Translation = new Vector(value, this.decomposition.TranslationY);
      }
    }

    public double TranslationY
    {
      get
      {
        return this.decomposition.TranslationY;
      }
      set
      {
        this.Translation = new Vector(this.decomposition.TranslationX, value);
      }
    }

    public CanonicalTransform()
    {
      this.Initialize();
    }

    public CanonicalTransform(CanonicalTransform canonicalTransform)
    {
      if (canonicalTransform == (CanonicalTransform) null)
      {
        this.Initialize();
      }
      else
      {
        this.decomposition = (CanonicalDecomposition) canonicalTransform.decomposition.Clone();
        this.InitializeTransformGroup();
      }
    }

    public CanonicalTransform(Transform transform)
    {
      if (transform == null)
      {
        this.Initialize();
      }
      else
      {
        this.decomposition = new CanonicalDecomposition();
        this.ReadTransform(transform, false);
      }
    }

    public CanonicalTransform(Matrix transform)
    {
      if (transform.IsIdentity)
      {
        this.Initialize();
      }
      else
      {
        this.decomposition = new CanonicalDecomposition(transform);
        this.InitializeTransformGroup();
      }
    }

    private CanonicalTransform(Transform transform, bool useIfChangeable)
    {
      if (transform == null)
      {
        this.Initialize();
      }
      else
      {
        this.decomposition = new CanonicalDecomposition();
        this.ReadTransform(transform, useIfChangeable);
      }
    }

    public static bool operator ==(CanonicalTransform ct1, CanonicalTransform ct2)
    {
      bool flag1 = ct1 == null;
      bool flag2 = ct2 == null;
      if (flag1 && flag2)
        return true;
      if (!flag1 && !flag2)
        return ct1.decomposition == ct2.decomposition;
      return false;
    }

    public static bool operator !=(CanonicalTransform ct1, CanonicalTransform ct2)
    {
      return !(ct1 == ct2);
    }

    public static bool IsCanonical(Transform transform)
    {
      return CanonicalTransform.IsCanonical(transform, false);
    }

    public static bool IsCanonical(Transform transform, bool ignoreCenter)
    {
      bool flag = false;
      TransformGroup transformGroup = transform as TransformGroup;
      if (transformGroup != null && transformGroup.Children.Count == CanonicalTransformOrder.TransformCount)
      {
        ScaleTransform scaleTransform = transformGroup.Children[CanonicalTransformOrder.ScaleIndex] as ScaleTransform;
        SkewTransform skewTransform = transformGroup.Children[CanonicalTransformOrder.SkewIndex] as SkewTransform;
        RotateTransform rotateTransform = transformGroup.Children[CanonicalTransformOrder.RotateIndex] as RotateTransform;
        TranslateTransform translateTransform = transformGroup.Children[CanonicalTransformOrder.TranslateIndex] as TranslateTransform;
        if (scaleTransform != null && skewTransform != null && (rotateTransform != null && translateTransform != null) && (ignoreCenter || skewTransform.CenterX == rotateTransform.CenterX && skewTransform.CenterY == rotateTransform.CenterY && (scaleTransform.CenterX == rotateTransform.CenterX && scaleTransform.CenterY == rotateTransform.CenterY)))
          flag = true;
      }
      return flag;
    }

    public override string ToString()
    {
      return this.decomposition.ToString();
    }

    public override int GetHashCode()
    {
      return this.decomposition.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this == obj as CanonicalTransform;
    }

    public static bool Compare(CanonicalTransform ct1, CanonicalTransform ct2)
    {
      return ct1 == ct2;
    }

    public object GetPlatformTransform(IPlatformGeometryHelper geometryHelper)
    {
      return geometryHelper.ConvertCanonicalTransform((ICanonicalTransform) this);
    }

    public object Clone()
    {
      return (object) new CanonicalTransform(this);
    }

    public void UpdateForNewOrigin(Point oldOrigin, Point newOrigin)
    {
      Point point = new Point(0.0, 0.0);
      Vector vector1 = new Vector(oldOrigin.X, oldOrigin.Y);
      Vector vector2 = new Vector(newOrigin.X, newOrigin.Y);
      this.Translation += (point - vector1) * this.TransformGroup.Value + vector1 - (point - vector2) * this.TransformGroup.Value + vector2;
    }

    public void UpdateCenter(Point center)
    {
      if (!(this.Center != center))
        return;
      Point point1 = new Point(0.0, 0.0);
      Point point2 = this.TransformGroup.Value.Transform(point1);
      this.Center = center;
      Point point3 = this.TransformGroup.Value.Transform(point1);
      this.Translation += point2 - point3;
    }

    public void ApplyScale(Vector scale, Point origin, Point fixedPoint)
    {
      this.decomposition.ApplyScale(scale, origin, fixedPoint);
      this.UpdateTransformGroup();
    }

    public void ApplySkewScale(Vector basisX, Vector basisY, Point origin, Point fixedPoint, Vector newBasisX, Vector newBasisY)
    {
      this.decomposition.ApplySkewScale(basisX, basisY, origin, fixedPoint, newBasisX, newBasisY);
      this.UpdateTransformGroup();
    }

    public void ApplySkewScale(Vector appliedSkew, Vector appliedScale, Point origin, Point fixedPoint)
    {
      this.decomposition.ApplySkewScale(appliedSkew, appliedScale, origin, fixedPoint);
      this.UpdateTransformGroup();
    }

    public void ApplyRotation(double angle, Point fixedPoint)
    {
      this.decomposition.ApplyRotation(angle, fixedPoint);
      this.UpdateTransformGroup();
    }

    private void Initialize()
    {
      this.decomposition = new CanonicalDecomposition();
      this.InitializeTransformGroup();
    }

    private void UpdateTransformGroup()
    {
      this.Skew = this.Decomposition.Skew;
      this.Scale = this.Decomposition.Scale;
      this.RotationAngle = this.Decomposition.RotationAngle;
      this.Translation = this.Decomposition.Translation;
      this.Center = this.Decomposition.Center;
    }

    private void InitializeTransformGroup()
    {
      Transform[] transformArray = new Transform[CanonicalTransformOrder.TransformCount];
      ScaleTransform scaleTransform = new ScaleTransform();
      if (this.decomposition.ScaleX != 1.0)
        scaleTransform.ScaleX = this.decomposition.ScaleX;
      if (this.decomposition.ScaleY != 1.0)
        scaleTransform.ScaleY = this.decomposition.ScaleY;
      transformArray[CanonicalTransformOrder.ScaleIndex] = (Transform) scaleTransform;
      SkewTransform skewTransform = new SkewTransform();
      if (this.decomposition.SkewX != 0.0)
        skewTransform.AngleX = this.decomposition.SkewX;
      if (this.decomposition.SkewY != 0.0)
        skewTransform.AngleY = this.decomposition.SkewY;
      transformArray[CanonicalTransformOrder.SkewIndex] = (Transform) skewTransform;
      RotateTransform rotateTransform = new RotateTransform();
      if (this.decomposition.RotationAngle != 0.0)
        rotateTransform.Angle = this.decomposition.RotationAngle;
      transformArray[CanonicalTransformOrder.RotateIndex] = (Transform) rotateTransform;
      if (this.decomposition.Center.X != 0.0 || this.decomposition.Center.Y != 0.0)
      {
        Point center = this.decomposition.Center;
        scaleTransform.CenterX = center.X;
        scaleTransform.CenterY = center.Y;
        skewTransform.CenterX = center.X;
        skewTransform.CenterY = center.Y;
        rotateTransform.CenterX = center.X;
        rotateTransform.CenterY = center.Y;
      }
      TranslateTransform translateTransform = new TranslateTransform();
      if (this.decomposition.TranslationX != 0.0)
        translateTransform.X = this.decomposition.TranslationX;
      if (this.decomposition.TranslationY != 0.0)
        translateTransform.Y = this.decomposition.TranslationY;
      transformArray[CanonicalTransformOrder.TranslateIndex] = (Transform) translateTransform;
      this.transformGroup = new TransformGroup();
      this.transformGroup.Children = new TransformCollection((IEnumerable<Transform>) transformArray);
    }

    private void ReadTransform(Transform transform, bool useIfChangeable)
    {
      if (this.ReadCanonicalForm(transform, useIfChangeable))
        return;
      this.decomposition = new CanonicalDecomposition(transform.Value);
      this.InitializeTransformGroup();
    }

    private bool ReadCanonicalForm(Transform transform, bool useIfChangeable)
    {
      bool flag = false;
      if (CanonicalTransform.IsCanonical(transform))
      {
        TransformGroup transformGroup = (TransformGroup) transform;
        ScaleTransform scaleTransform = transformGroup.Children[CanonicalTransformOrder.ScaleIndex] as ScaleTransform;
        SkewTransform skewTransform = transformGroup.Children[CanonicalTransformOrder.SkewIndex] as SkewTransform;
        RotateTransform rotateTransform = transformGroup.Children[CanonicalTransformOrder.RotateIndex] as RotateTransform;
        TranslateTransform translateTransform = transformGroup.Children[CanonicalTransformOrder.TranslateIndex] as TranslateTransform;
        this.decomposition.Center = new Point(scaleTransform.CenterX, scaleTransform.CenterY);
        this.decomposition.Scale = new Vector(scaleTransform.ScaleX, scaleTransform.ScaleY);
        this.decomposition.Skew = new Vector(skewTransform.AngleX, skewTransform.AngleY);
        this.decomposition.RotationAngle = rotateTransform.Angle;
        this.decomposition.Translation = new Vector(translateTransform.X, translateTransform.Y);
        if (useIfChangeable && !transformGroup.IsFrozen)
          this.transformGroup = transformGroup;
        else
          this.InitializeTransformGroup();
        flag = true;
      }
      return flag;
    }
  }
}
