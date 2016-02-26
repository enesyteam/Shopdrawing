// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.AdornedToolBehavior
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal abstract class AdornedToolBehavior : ToolBehavior
  {
    private IAdorner activeAdorner;

    protected IAdorner ActiveAdorner
    {
      get
      {
        return this.activeAdorner;
      }
    }

    protected SceneElement EditingElement
    {
      get
      {
        return this.EditingElementSet.PrimaryElement;
      }
    }

    protected AdornerElementSet EditingElementSet
    {
      get
      {
        return this.activeAdorner.ElementSet;
      }
    }

    protected AdornedToolBehavior(ToolBehaviorContext toolContext)
      : base(toolContext)
    {
    }

    public void SetActiveAdorner(IAdorner activeAdorner)
    {
      this.activeAdorner = activeAdorner;
      this.activeAdorner.IsActive = true;
    }

    protected override bool OnClickEnd(Point pointerPosition, int clickCount)
    {
      if (this.activeAdorner != null)
        this.activeAdorner.IsActive = false;
      this.PopSelf();
      return true;
    }

    protected override bool OnDragEnd(Point dragStartPosition, Point dragEndPosition)
    {
      if (this.activeAdorner != null)
        this.activeAdorner.IsActive = false;
      this.PopSelf();
      return true;
    }

    protected static void UpdateElementTransform(SceneElement element, CanonicalTransform newTransform, AdornedToolBehavior.TransformPropertyFlags properties)
    {
      AdornedToolBehavior.UpdateElementTransform(element, newTransform.Decomposition, properties);
    }

    protected static void UpdateElementTransform(SceneElement element, CanonicalDecomposition newTransform, AdornedToolBehavior.TransformPropertyFlags properties)
    {
      Transform transform = element.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty) as Transform;
      CanonicalDecomposition canonicalDecomposition = new CanonicalDecomposition();
      if (transform != null)
        canonicalDecomposition = new CanonicalTransform(transform).Decomposition;
      double num1 = RoundingHelper.RoundScale(newTransform.ScaleX);
      double num2 = RoundingHelper.RoundScale(newTransform.ScaleY);
      double num3 = RoundingHelper.RoundLength(newTransform.TranslationX);
      double num4 = RoundingHelper.RoundLength(newTransform.TranslationY);
      double num5 = RoundingHelper.RoundLength(newTransform.SkewX);
      double num6 = RoundingHelper.RoundLength(newTransform.SkewY);
      double num7 = RoundingHelper.RoundAngle(newTransform.RotationAngle);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.ScaleX) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.ScaleX, num1))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformScaleX, (object) num1);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.ScaleY) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.ScaleY, num2))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformScaleY, (object) num2);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.TranslationX) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.TranslationX, num3))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformTranslationX, (object) num3);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.TranslationY) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.TranslationY, num4))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformTranslationY, (object) num4);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.SkewX) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.SkewX, num5))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformSkewX, (object) num5);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.SkewY) != AdornedToolBehavior.TransformPropertyFlags.None && !Tolerances.AreClose(canonicalDecomposition.SkewY, num6))
        element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformSkewY, (object) num6);
      if ((properties & AdornedToolBehavior.TransformPropertyFlags.RotatationAngle) == AdornedToolBehavior.TransformPropertyFlags.None || Tolerances.AreClose(canonicalDecomposition.RotationAngle, num7))
        return;
      element.SetValue(element.Platform.Metadata.CommonProperties.RenderTransformRotationAngle, (object) num7);
    }

    [Flags]
    protected enum TransformPropertyFlags
    {
      None = 0,
      ScaleX = 1,
      ScaleY = 2,
      Scale = ScaleY | ScaleX,
      RotatationAngle = 4,
      SkewX = 8,
      SkewY = 16,
      Skew = SkewY | SkewX,
      TranslationX = 32,
      TranslationY = 64,
      Translation = TranslationY | TranslationX,
      All = Translation | Skew | RotatationAngle | Scale,
    }
  }
}
