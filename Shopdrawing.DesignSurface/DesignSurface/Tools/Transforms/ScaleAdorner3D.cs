// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ScaleAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ScaleAdorner3D : Adorner3D
  {
    private Vector3D scaleAxis = new Vector3D();
    private static Model3DGroup xCube = ScaleAdorner3D.CreateCube(new Vector3D(1.0, 0.0, 0.0), Adorner3D.RedLook);
    private static Model3DGroup yCube = ScaleAdorner3D.CreateCube(new Vector3D(0.0, 1.0, 0.0), Adorner3D.GreenLook);
    private static Model3DGroup zCube = ScaleAdorner3D.CreateCube(new Vector3D(0.0, 0.0, 1.0), Adorner3D.BlueLook);
    private const double cubeCenter = 0.35;
    private const double cubeSize = 0.08;

    public double CenterPointRatio
    {
      get
      {
        return 0.35;
      }
    }

    public Vector3D ScaleAxis
    {
      get
      {
        return this.scaleAxis;
      }
    }

    public ScaleAdorner3D(AdornerSet3D adornerSet, Adorner3D.TransformVia direction)
      : base(adornerSet, direction)
    {
      switch (direction)
      {
        case Adorner3D.TransformVia.XAxis:
          this.scaleAxis = new Vector3D(1.0, 0.0, 0.0);
          this.AdornerModel = ScaleAdorner3D.xCube;
          break;
        case Adorner3D.TransformVia.YAxis:
          this.scaleAxis = new Vector3D(0.0, 1.0, 0.0);
          this.AdornerModel = ScaleAdorner3D.yCube;
          break;
        case Adorner3D.TransformVia.ZAxis:
          this.scaleAxis = new Vector3D(0.0, 0.0, 1.0);
          this.AdornerModel = ScaleAdorner3D.zCube;
          break;
      }
    }

    private static Model3DGroup CreateCube(Vector3D axis, Material material)
    {
      Model3DGroup model3Dgroup = new Model3DGroup();
      Model3DCollection model3Dcollection = new Model3DCollection();
      model3Dgroup.Children = model3Dcollection;
      double num = -0.04;
      Model3DGroup cube = Cube.CreateCube(new Rect3D(new Point3D(num, num, num), new Size3D(0.08, 0.08, 0.08)), material);
      cube.Transform = (Transform3D) new Transform3DGroup()
      {
        Children = {
          (Transform3D) new TranslateTransform3D(axis * 0.35)
        }
      };
      model3Dgroup.Children.Add((Model3D) cube);
      model3Dgroup.Freeze();
      return model3Dgroup;
    }
  }
}
