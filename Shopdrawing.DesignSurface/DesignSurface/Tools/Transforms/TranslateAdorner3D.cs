// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.TranslateAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class TranslateAdorner3D : Adorner3D
  {
    private Vector3D translationAxis = new Vector3D();
    private static Model3DGroup xAxisAndArrowHead = TranslateAdorner3D.CreateAxisAndArrowHead(new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(0.0, 0.0, 1.0), -90.0)), new TranslateTransform3D(0.5, 0.0, 0.0), Adorner3D.RedLook);
    private static Model3DGroup yAxisAndArrowHead = TranslateAdorner3D.CreateAxisAndArrowHead((RotateTransform3D) null, new TranslateTransform3D(0.0, 0.5, 0.0), Adorner3D.GreenLook);
    private static Model3DGroup zAxisAndArrowHead = TranslateAdorner3D.CreateAxisAndArrowHead(new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(new Vector3D(1.0, 0.0, 0.0), 90.0)), new TranslateTransform3D(0.0, 0.0, 0.5), Adorner3D.BlueLook);
    private const double axisLength = 0.5;
    private const double arrowheadHeight = 0.1;
    private const double arrowheadRadius = 0.04;
    private const double cylinderRadius = 0.02;

    public Vector3D TranslationAxis
    {
      get
      {
        return this.translationAxis;
      }
    }

    public TranslateAdorner3D(AdornerSet3D adornerSet, Adorner3D.TransformVia direction)
      : base(adornerSet, direction)
    {
      switch (direction)
      {
        case Adorner3D.TransformVia.XAxis:
          this.translationAxis = new Vector3D(1.0, 0.0, 0.0);
          this.AdornerModel = TranslateAdorner3D.xAxisAndArrowHead;
          break;
        case Adorner3D.TransformVia.YAxis:
          this.translationAxis = new Vector3D(0.0, 1.0, 0.0);
          this.AdornerModel = TranslateAdorner3D.yAxisAndArrowHead;
          break;
        case Adorner3D.TransformVia.ZAxis:
          this.translationAxis = new Vector3D(0.0, 0.0, 1.0);
          this.AdornerModel = TranslateAdorner3D.zAxisAndArrowHead;
          break;
      }
    }

    private static Model3DGroup CreateAxisAndArrowHead(RotateTransform3D rotateTransform, TranslateTransform3D translateTransform, Material material)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      Model3DCollection model3Dcollection = new Model3DCollection();
      model3Dgroup.Children = model3Dcollection;
      Model3DGroup cylinder = Cylinder.CreateCylinder(0.02, 0.5, 12, material, material, (Material) null);
      if (rotateTransform != null)
        cylinder.Transform = (Transform3D) rotateTransform;
      model3Dgroup.Children.Add((Model3D) cylinder);
      Model3DGroup cone = Cone.CreateCone(0.04, 0.1, 12, material, material);
      if (rotateTransform != null)
        cone.Transform = (Transform3D) new Transform3DGroup()
        {
          Children = {
            (Transform3D) rotateTransform,
            (Transform3D) translateTransform
          }
        };
      else
        cone.Transform = (Transform3D) translateTransform;
      model3Dgroup.Children.Add((Model3D) cone);
      model3Dgroup.Freeze();
      return model3Dgroup;
    }
  }
}
