// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RotateAdorner3D : Adorner3D
  {
    private Vector3D rotationAxis = new Vector3D();
    private static Model3DGroup xTorus = RotateAdorner3D.CreateTorus(new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(0.0, 1.0, 0.0), -90.0)), Adorner3D.RedLook);
    private static Model3DGroup yTorus = RotateAdorner3D.CreateTorus(new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(1.0, 0.0, 0.0), 90.0)), Adorner3D.GreenLook);
    private static Model3DGroup zTorus = RotateAdorner3D.CreateTorus((RotateTransform3D) null, Adorner3D.BlueLook);

    public Vector3D RotationAxis
    {
      get
      {
        return this.rotationAxis;
      }
    }

    public RotateAdorner3D(AdornerSet3D adornerSet, Adorner3D.TransformVia direction)
      : base(adornerSet, direction)
    {
      switch (direction)
      {
        case Adorner3D.TransformVia.XAxis:
          this.rotationAxis = new Vector3D(1.0, 0.0, 0.0);
          this.AdornerModel = RotateAdorner3D.xTorus;
          break;
        case Adorner3D.TransformVia.YAxis:
          this.rotationAxis = new Vector3D(0.0, 1.0, 0.0);
          this.AdornerModel = RotateAdorner3D.yTorus;
          break;
        case Adorner3D.TransformVia.ZAxis:
          this.rotationAxis = new Vector3D(0.0, 0.0, 1.0);
          this.AdornerModel = RotateAdorner3D.zTorus;
          break;
      }
    }

    private static Model3DGroup CreateTorus(RotateTransform3D rotateTransform3D, Material material)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      Model3DCollection model3Dcollection = new Model3DCollection();
      model3Dgroup.Children = model3Dcollection;
      Model3DGroup quarterTorus = Torus.CreateQuarterTorus(0.35, 0.015, 24, 15, material);
      if (rotateTransform3D != null)
        quarterTorus.Transform = (Transform3D) new Transform3DGroup()
        {
          Children = {
            (Transform3D) rotateTransform3D
          }
        };
      model3Dgroup.Children.Add((Model3D) quarterTorus);
      model3Dgroup.Freeze();
      return model3Dgroup;
    }
  }
}
