// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SpotLightConeAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class SpotLightConeAdorner3D : Adorner3D
  {
    private Model3DGroup spotPropertyGeometry = new Model3DGroup();
    private const double lineThickness = 3.0;
    private const int numberOfLines = 4;
    private SpotLightAdornerBehavior3D.TypeOfConeAngle typeOfConeAngle;

    public SpotLightConeAdorner3D(AdornerSet3D adornerSet, SpotLightAdornerBehavior3D.TypeOfConeAngle typeOfConeAngle)
      : base(adornerSet)
    {
      this.typeOfConeAngle = typeOfConeAngle;
      for (int index = 0; index < 4; ++index)
        this.spotPropertyGeometry.Children.Add((Model3D) Cylinder.CreateCylinder(0.01, 1.0, 8, (Material) new DiffuseMaterial((Brush) new SolidColorBrush(this.typeOfConeAngle != SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle ? Color.FromRgb(byte.MaxValue, (byte) 0, (byte) 0) : Color.FromRgb((byte) 0, byte.MaxValue, (byte) 0))), (Material) null, (Material) null));
      this.AdornerModel = new Model3DGroup();
      this.AdornerModel.Children.Add((Model3D) this.spotPropertyGeometry);
      this.SetName((DependencyObject) this.AdornerModel, "SpotLightConeAdorner3D");
    }

    public override void PositionAndOrientGeometry()
    {
      SpotLight spotLight = (SpotLight) this.Element.ViewObject.PlatformSpecificObject;
      double angleInDegrees = this.typeOfConeAngle != SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle ? spotLight.OuterConeAngle : spotLight.InnerConeAngle;
      double num = SpotLightAdorner3D.CalculateConeRadius(spotLight, this.typeOfConeAngle);
      double y = Math.Sqrt(num * num + 1.0);
      for (int index = 0; index < 4; ++index)
      {
        Transform3DGroup transform3Dgroup = new Transform3DGroup();
        transform3Dgroup.Children.Add((Transform3D) new ScaleTransform3D(new Vector3D(1.0, y, 1.0)));
        Quaternion quaternion = Quaternion.Identity * new Quaternion(new Vector3D(0.0, 0.0, 1.0), (double) (90 * index)) * new Quaternion(new Vector3D(1.0, 0.0, 0.0), angleInDegrees) * new Quaternion(new Vector3D(1.0, 0.0, 0.0), 90.0);
        transform3Dgroup.Children.Add((Transform3D) new RotateTransform3D((Rotation3D) new QuaternionRotation3D(quaternion)));
        transform3Dgroup.Children.Add((Transform3D) new RotateTransform3D(Vector3DEditor.GetRotation3DFromDirection(spotLight.Direction)));
        transform3Dgroup.Children.Add((Transform3D) new TranslateTransform3D((Vector3D) spotLight.Position));
        this.spotPropertyGeometry.Children[index].Transform = (Transform3D) transform3Dgroup;
      }
    }
  }
}
