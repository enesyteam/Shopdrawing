// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.SpotLightAdornerBehavior3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Geometry;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class SpotLightAdornerBehavior3D : AdornedToolBehavior3D
  {
    private Point3D centerOfCone;

    protected SpotLightAdorner3D ActiveAdorner
    {
      get
      {
        return (SpotLightAdorner3D) base.ActiveAdorner;
      }
    }

    protected override string UndoUnitNameString
    {
      get
      {
        return StringTable.UndoUnitRotateObject;
      }
    }

    public SpotLightAdornerBehavior3D(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void ButtonDownAction()
    {
      this.centerOfCone = this.CalculateTransformationForSpotLightAdorner().Transform(new Point3D(0.0, 0.0, 0.0));
    }

    protected override void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta)
    {
      Vector3D normal = this.CalculateTransformationForSpotLightAdorner().Transform(new Vector3D(0.0, 0.0, 1.0));
      normal.Normalize();
      Plane3D plane3D = new Plane3D(normal, this.centerOfCone);
      Viewport3DVisual adorningViewport3D = this.ActiveView.AdornerLayer.GetAdornerSet3DContainer(this.ActiveAdorner.Element.Viewport).ShadowAdorningViewport3D;
      Ray3D ray = CameraRayHelpers.RayFromViewportPoint(adorningViewport3D.Viewport.Size, adorningViewport3D.Camera, this.LastMousePosition + mousePositionDelta);
      double t;
      if (!plane3D.IntersectWithRay(ray, out t))
        return;
      double num1 = Math.Atan((ray.Evaluate(t) - this.centerOfCone).Length / 1.0) / Math.PI * 180.0;
      SpotLight spotLight = (SpotLight) this.Selected3DElement.ViewObject.PlatformSpecificObject;
      if (this.ActiveAdorner.TypeOfConeAngle == SpotLightAdornerBehavior3D.TypeOfConeAngle.InnerConeAngle)
      {
        double num2 = spotLight.OuterConeAngle - spotLight.InnerConeAngle;
        this.Selected3DElement.SetValue(SpotLightElement.InnerConeAngleProperty, (object) num1);
        this.Selected3DElement.SetValue(SpotLightElement.OuterConeAngleProperty, (object) (num1 + num2));
      }
      else
        this.Selected3DElement.SetValue(SpotLightElement.OuterConeAngleProperty, (object) num1);
      this.ActiveAdorner.PositionAndOrientGeometry();
    }

    private Matrix3D CalculateTransformationForSpotLightAdorner()
    {
      return this.ActiveAdorner.CalculateSpotLightTransformation().Value * this.ActiveAdorner.Element.GetComputedTransformFromViewport3DToElement();
    }

    public enum TypeOfConeAngle
    {
      InnerConeAngle,
      OuterConeAngle,
    }
  }
}
