// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerCoordinateSpaces
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public static class AdornerCoordinateSpaces
  {
    private static AdornerCoordinateSpace _transform;
    [ThreadStatic]
    private static Transform _ltr;
    [ThreadStatic]
    private static Transform _rtl;

    private static Transform LTR
    {
      get
      {
        if (AdornerCoordinateSpaces._ltr == null)
          AdornerCoordinateSpaces._ltr = (Transform) new ScaleTransform(1.0, 1.0);
        return AdornerCoordinateSpaces._ltr;
      }
    }

    public static AdornerCoordinateSpace Default
    {
      get
      {
        if (AdornerCoordinateSpaces._transform == null)
          AdornerCoordinateSpaces._transform = (AdornerCoordinateSpace) new AdornerCoordinateSpaces.TransformAwareCoordinateSpace();
        return AdornerCoordinateSpaces._transform;
      }
    }

    private static Transform RTL
    {
      get
      {
        if (AdornerCoordinateSpaces._rtl == null)
          AdornerCoordinateSpaces._rtl = (Transform) new ScaleTransform(-1.0, 1.0);
        return AdornerCoordinateSpaces._rtl;
      }
    }

    private class RenderCoordinateSpace : AdornerCoordinateSpace
    {
      internal override Rect GetBoundingBox(ViewItem element)
      {
        return ElementUtilities.GetRenderSizeBounds(element);
      }

      protected virtual ViewItem GetLayoutView(ViewItem element)
      {
        return element;
      }

      internal override FlowDirection GetFlowDirection(ViewItem element)
      {
        if (element == (ViewItem) null)
          throw new ArgumentNullException("element");
        ViewItem layoutView = this.GetLayoutView(element);
        if (layoutView != (ViewItem) null)
          return layoutView.FlowDirection;
        return FlowDirection.LeftToRight;
      }

      internal override Transform GetLayoutTransform(ViewItem element)
      {
        if (element == (ViewItem) null)
          throw new ArgumentNullException("element");
        ViewItem layoutView = this.GetLayoutView(element);
        Transform transform = AdornerCoordinateSpaces.LTR;
        if (layoutView != (ViewItem) null && layoutView.FlowDirection == FlowDirection.RightToLeft)
          transform = AdornerCoordinateSpaces.RTL;
        return transform;
      }

      internal override Transform GetAncestorTransform(ViewItem element, UIElement ancestor)
      {
        if (element == (ViewItem) null)
          throw new ArgumentNullException("element");
        if (ancestor == null)
          throw new ArgumentNullException("ancestor");
        return element.TransformToVisual((Visual) ancestor) as Transform ?? Transform.Identity;
      }

      internal override Vector GetOrigin(ViewItem element)
      {
        if (element == (ViewItem) null)
          throw new ArgumentNullException("element");
        return new Vector();
      }

      public override string ToString()
      {
        return "Render";
      }
    }

    private class TransformAwareCoordinateSpace : AdornerCoordinateSpaces.RenderCoordinateSpace
    {
      internal override Rect GetBoundingBox(ViewItem element)
      {
        return ElementUtilities.GetSelectionFrameBounds(element);
      }

      public override string ToString()
      {
        return "TransformAware";
      }
    }
  }
}
