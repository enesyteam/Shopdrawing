// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.ScaleBehavior3D
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class ScaleBehavior3D : AdornedToolBehavior3D
  {
    private Vector mouseMovementAxis;
    private Vector3D scaleAxis;

    protected ScaleAdorner3D ActiveAdorner
    {
      get
      {
        return (ScaleAdorner3D) base.ActiveAdorner;
      }
    }

    protected override string UndoUnitNameString
    {
      get
      {
        return StringTable.UndoUnitScaleObject;
      }
    }

    public ScaleBehavior3D(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void ButtonDownAction()
    {
      this.scaleAxis = this.ActiveAdorner.ScaleAxis;
      this.mouseMovementAxis = AdornedToolBehavior3D.Vector3DInViewport3D(this.Selected3DElement, this.scaleAxis);
      if (Math.Abs(this.mouseMovementAxis.X) <= AdornedToolBehavior3D.tolerance && Math.Abs(this.mouseMovementAxis.Y) <= AdornedToolBehavior3D.tolerance)
        this.mouseMovementAxis = new Vector(AdornedToolBehavior3D.sqrt2div2, AdornedToolBehavior3D.sqrt2div2);
      this.mouseMovementAxis.Normalize();
    }

    protected override void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta)
    {
      double num = this.mouseMovementAxis * mousePositionDelta;
      ScaleTransform3D scaleTransform3D = selected3DElement.CanonicalScaleTransform3D;
      Vector3D vector3D = new Vector3D(scaleTransform3D.ScaleX, scaleTransform3D.ScaleY, scaleTransform3D.ScaleZ) + num * this.scaleAxis * 1.0 / 96.0;
      if (vector3D.X < 0.0)
        vector3D.X = 0.0;
      if (vector3D.Y < 0.0)
        vector3D.Y = 0.0;
      if (vector3D.Z < 0.0)
        vector3D.Z = 0.0;
      selected3DElement.CanonicalScaleX = RoundingHelper.RoundScale(vector3D.X);
      selected3DElement.CanonicalScaleY = RoundingHelper.RoundScale(vector3D.Y);
      selected3DElement.CanonicalScaleZ = RoundingHelper.RoundScale(vector3D.Z);
    }
  }
}
