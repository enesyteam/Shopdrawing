// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.SizeRelativeToAdornerDesiredHeight
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using MS.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace MS.Internal.Interaction
{
  internal class SizeRelativeToAdornerDesiredHeight : IAdornerPlacement
  {
    private DependencyObject _relativeTo;
    private double _factor;
    private double _offset;

    internal SizeRelativeToAdornerDesiredHeight(double factor, double offset, DependencyObject relativeTo)
    {
      this._relativeTo = relativeTo;
      this._factor = factor;
      this._offset = offset;
    }

    public IEnumerable<AdornerPlacementValue> GetSizeTerms(AdornerCoordinateSpace space, UIElement adorner, ViewItem adornedElement, Vector zoom, Size adornedElementFinalSize)
    {
      double desiredHeight = 0.0;
      UIElement eAdorner = this._relativeTo as UIElement;
      desiredHeight = eAdorner == null ? adorner.DesiredSize.Height : eAdorner.DesiredSize.Height;
      Vector adornerSizeScale = ScaledSpace.GetAdornerSizeScale(adorner);
      double height = (this._factor * desiredHeight + this._offset) * adornerSizeScale.Y;
      yield return new AdornerPlacementValue(AdornerPlacementDimension.Height, height);
    }

    public IEnumerable<AdornerPlacementValue> GetPositionTerms(AdornerCoordinateSpace space, UIElement adorner, ViewItem adornedElement, Vector zoom, Size computedAdornerSize)
    {
      yield break;
    }

    public override string ToString()
    {
        return string.Format((IFormatProvider)CultureInfo.CurrentCulture, MS.Internal.Properties.Resources.AdornerPlacement_ToString, (object)this.GetType().Name, (object)this._factor, (object)this._offset);
    }
  }
}
