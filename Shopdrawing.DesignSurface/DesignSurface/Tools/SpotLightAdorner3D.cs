// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.SpotLightAdorner3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools.Transforms;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class SpotLightAdorner3D : Adorner3D
  {
    public const double SpotLightConeAdornerDistance = 1.0;
    private const double torusRadius = 1.0;
    private const double bodyRadius = 0.015;
    private const int segmentCount = 24;
    private const int skinPanelCount = 15;
    private Model3DGroup spotPropertyGeometry;
    private SpotLightAdornerBehavior3D.TypeOfConeAngle typeOfConeAngle;

    public SpotLightAdornerBehavior3D.TypeOfConeAngle TypeOfConeAngle
    {
      get
      {
        return this.typeOfConeAngle;
      }
    }

    public SpotLightAdorner3D(AdornerSet3D adornerSet, SpotLightAdornerBehavior3D.TypeOfConeAngle typeOfConeAngle)
      : base(adornerSet)
    {
      this.typeOfConeAngle = typeOfConeAngle;
      this.spotPropertyGeometry = Torus.CreateTorus(1.0, 0.015, 24, 15, this.typeOfConeAngle != SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle ? Adorner3D.RedLook : Adorner3D.GreenLook);
      this.AdornerModel = new Model3DGroup();
      this.AdornerModel.Children.Add((Model3D) this.spotPropertyGeometry);
      this.SetName((DependencyObject) this.AdornerModel, "SpotLightAdorner3D");
    }

    public override void PositionAndOrientGeometry()
    {
      this.spotPropertyGeometry.Transform = this.CalculateSpotLightTransformation();
    }

    public Transform3D CalculateSpotLightTransformation()
    {
      SpotLight spotLight = (SpotLight) this.Element.ViewObject.PlatformSpecificObject;
      Transform3DGroup transform3Dgroup = new Transform3DGroup();
      double num = SpotLightAdorner3D.CalculateConeRadius(spotLight, this.typeOfConeAngle);
      transform3Dgroup.Children.Add((Transform3D) new ScaleTransform3D(num, num, 1.0));
      transform3Dgroup.Children.Add((Transform3D) new TranslateTransform3D(new Vector3D(0.0, 0.0, 1.0)));
      transform3Dgroup.Children.Add((Transform3D) new RotateTransform3D(Vector3DEditor.GetRotation3DFromDirection(spotLight.Direction)));
      transform3Dgroup.Children.Add((Transform3D) new TranslateTransform3D((Vector3D) spotLight.Position));
      return (Transform3D) transform3Dgroup;
    }

    public static double CalculateConeRadius(SpotLight spotLight, SpotLightAdornerBehavior3D.TypeOfConeAngle typeOfConeAngle)
    {
      return Math.Tan((typeOfConeAngle != SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle ? spotLight.OuterConeAngle : spotLight.InnerConeAngle) / 180.0 * Math.PI) * 1.0;
    }
  }
}
