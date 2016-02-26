// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.TranslateBehavior3D
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
  internal class TranslateBehavior3D : AdornedToolBehavior3D
  {
    private Vector mouseMovementAxis;
    private Vector3D translationAxis;

    protected TranslateAdorner3D ActiveAdorner
    {
      get
      {
        return (TranslateAdorner3D) base.ActiveAdorner;
      }
    }

    protected override string UndoUnitNameString
    {
      get
      {
        return StringTable.UndoUnitTranslateObject;
      }
    }

    public TranslateBehavior3D(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    protected override void ButtonDownAction()
    {
      Vector3D translationAxis = this.ActiveAdorner.TranslationAxis;
      Vector3D vector3D = this.Selected3DElement.CanonicalRotateTransform3D.Value.Transform(translationAxis);
      this.mouseMovementAxis = AdornedToolBehavior3D.Vector3DInViewport3D(this.Selected3DElement, translationAxis);
      if (Math.Abs(this.mouseMovementAxis.X) <= AdornedToolBehavior3D.tolerance && Math.Abs(this.mouseMovementAxis.Y) <= AdornedToolBehavior3D.tolerance)
        this.mouseMovementAxis = new Vector(AdornedToolBehavior3D.sqrt2div2, AdornedToolBehavior3D.sqrt2div2);
      this.mouseMovementAxis.Normalize();
      this.translationAxis = vector3D;
    }

    protected override void UpdateModelFromMouse(Base3DElement selected3DElement, Vector mousePositionDelta)
    {
      double num = this.mouseMovementAxis * mousePositionDelta;
      Vector3D vector3D = new Vector3D(selected3DElement.CanonicalTranslationX, selected3DElement.CanonicalTranslationY, selected3DElement.CanonicalTranslationZ) + this.Scale * num * this.translationAxis;
      selected3DElement.CanonicalTranslationX = RoundingHelper.RoundLength(vector3D.X);
      selected3DElement.CanonicalTranslationY = RoundingHelper.RoundLength(vector3D.Y);
      selected3DElement.CanonicalTranslationZ = RoundingHelper.RoundLength(vector3D.Z);
    }
  }
}
