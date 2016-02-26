// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ReflectTransform
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  internal class ReflectTransform : IApplyRelativeTransform
  {
    private BasisComponent basisComponent;

    private Vector ComponentVector
    {
      get
      {
        return new Vector(this.basisComponent == BasisComponent.X ? 1.0 : 0.0, this.basisComponent == BasisComponent.Y ? 1.0 : 0.0);
      }
    }

    private Vector3D ComponentVector3D
    {
      get
      {
        return new Vector3D(this.basisComponent == BasisComponent.X ? 1.0 : 0.0, this.basisComponent == BasisComponent.Y ? 1.0 : 0.0, this.basisComponent == BasisComponent.Z ? 1.0 : 0.0);
      }
    }

    public ReflectTransform(BasisComponent basisComponent)
    {
      this.basisComponent = basisComponent;
    }

    public object ApplyRelativeTransform(object transform)
    {
      if (transform != null)
      {
        Transform transform1 = transform as Transform;
        if (transform1 != null)
        {
          CanonicalTransform canonicalTransform = new CanonicalTransform(transform1);
          Point center = canonicalTransform.Center;
          canonicalTransform.ApplyScale(new Vector(1.0, 1.0) - this.ComponentVector * 2.0, canonicalTransform.Center, center);
          canonicalTransform.RotationAngle = -canonicalTransform.RotationAngle;
          Vector skew = canonicalTransform.Skew;
          canonicalTransform.Skew = new Vector(-skew.X, -skew.Y);
          return (object) canonicalTransform;
        }
        Transform3D transform2 = transform as Transform3D;
        if (transform2 != null)
        {
          CanonicalTransform3D canonicalTransform3D = new CanonicalTransform3D(transform2);
          Point3D center = canonicalTransform3D.Center;
          Vector3D scale = new Vector3D(1.0, 1.0, 1.0) - this.ComponentVector3D * 2.0;
          canonicalTransform3D.ApplyScale(scale, center);
          canonicalTransform3D.RotationAngles = -canonicalTransform3D.RotationAngles;
          return (object) canonicalTransform3D;
        }
      }
      return (object) Transform.Identity;
    }
  }
}
