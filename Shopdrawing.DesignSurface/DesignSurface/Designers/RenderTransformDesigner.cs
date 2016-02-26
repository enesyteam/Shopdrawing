// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Designers.RenderTransformDesigner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Designers
{
  internal class RenderTransformDesigner : LayoutDesigner
  {
    private static IPropertyId[] renderTransformLayoutProperties = new IPropertyId[1]
    {
      Base2DElement.RenderTransformProperty
    };

    public RenderTransformDesigner()
      : base((IEnumerable<IPropertyId>) RenderTransformDesigner.renderTransformLayoutProperties)
    {
    }

    public override Rect GetChildRect(BaseFrameworkElement child)
    {
      CanonicalTransform canonicalTransform = new CanonicalTransform(child.GetEffectiveRenderTransform(false));
      canonicalTransform.SkewX = 0.0;
      canonicalTransform.SkewY = 0.0;
      canonicalTransform.RotationAngle = 0.0;
      Rect computedTightBounds = child.GetComputedTightBounds();
      computedTightBounds.Transform(canonicalTransform.TransformGroup.Value);
      return computedTightBounds;
    }

    public override void SetChildRect(BaseFrameworkElement child, Rect rect, LayoutOverrides layoutOverrides, LayoutOverrides overridesToIgnore, LayoutOverrides nonExplicitOverrides, SetRectMode setRectMode)
    {
      if (child == null)
        throw new ArgumentNullException("child");
      CanonicalTransform canonicalTransform = new CanonicalTransform((Transform) child.GetComputedValueAsWpf(Base2DElement.RenderTransformProperty));
      Vector translation = canonicalTransform.Translation;
      canonicalTransform.TranslationX = 0.0;
      canonicalTransform.TranslationY = 0.0;
      canonicalTransform.SkewX = 0.0;
      canonicalTransform.SkewY = 0.0;
      canonicalTransform.RotationAngle = 0.0;
      Rect computedTightBounds = child.GetComputedTightBounds();
      double num1 = computedTightBounds.Width == 0.0 ? 0.0 : rect.Width / computedTightBounds.Width;
      double num2 = computedTightBounds.Height == 0.0 ? 0.0 : rect.Height / computedTightBounds.Height;
      double num3 = RoundingHelper.RoundScale(num1);
      double num4 = RoundingHelper.RoundScale(num2);
      if ((overridesToIgnore & LayoutOverrides.Width) != LayoutOverrides.None && !object.Equals((object) canonicalTransform.ScaleX, (object) num3))
        child.SetValueAsWpf(child.Platform.Metadata.CommonProperties.RenderTransformScaleX, (object) num3);
      if ((overridesToIgnore & LayoutOverrides.Height) != LayoutOverrides.None && !object.Equals((object) canonicalTransform.ScaleY, (object) num4))
        child.SetValueAsWpf(child.Platform.Metadata.CommonProperties.RenderTransformScaleY, (object) num4);
      canonicalTransform.ScaleX = num3;
      canonicalTransform.ScaleY = num4;
      canonicalTransform.Center = child.RenderTransformOriginInElementCoordinates;
      computedTightBounds.Transform(canonicalTransform.TransformGroup.Value);
      double num5 = rect.Left - computedTightBounds.Left + translation.X;
      double num6 = rect.Top - computedTightBounds.Top + translation.Y;
      double num7 = RoundingHelper.RoundLength(num5);
      double num8 = RoundingHelper.RoundLength(num6);
      if (!object.Equals((object) translation.X, (object) num7))
        child.SetValueAsWpf(child.Platform.Metadata.CommonProperties.RenderTransformTranslationX, (object) num7);
      if (object.Equals((object) translation.Y, (object) num8))
        return;
      child.SetValueAsWpf(child.Platform.Metadata.CommonProperties.RenderTransformTranslationY, (object) num8);
    }
  }
}
