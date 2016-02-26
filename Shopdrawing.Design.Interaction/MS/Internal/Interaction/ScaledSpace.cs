// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.ScaledSpace
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using System.Windows;

namespace MS.Internal.Interaction
{
  internal static class ScaledSpace
  {
    internal static Vector GetTargetSizeScale(UIElement adorner, Vector scaleFactor)
    {
      object obj = adorner.ReadLocalValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom);
      if (obj != DependencyProperty.UnsetValue)
        return (Vector) obj;
      Vector vector = new Vector(1.0, 1.0);
      if (adorner != null)
      {
        switch (AdornerPanel.GetHorizontalStretch(adorner))
        {
          case AdornerStretch.None:
            vector.X = 1.0;
            break;
          case AdornerStretch.Stretch:
            vector.X = 1.0 / scaleFactor.X;
            break;
        }
        switch (AdornerPanel.GetVerticalStretch(adorner))
        {
          case AdornerStretch.None:
            vector.Y = 1.0;
            break;
          case AdornerStretch.Stretch:
            vector.Y = 1.0 / scaleFactor.Y;
            break;
        }
      }
      return vector;
    }

    internal static Vector GetTargetPositionScale(UIElement adorner)
    {
      object obj = adorner.ReadLocalValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom);
      if (obj != DependencyProperty.UnsetValue)
        return (Vector) obj;
      return new Vector(1.0, 1.0);
    }

    internal static Vector GetAdornerSizeScale(UIElement adorner)
    {
      return new Vector(1.0, 1.0);
    }

    internal static Vector GetAdornerPositionScale(UIElement adorner, Vector scaleFactor)
    {
      if (adorner.ReadLocalValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom) != DependencyProperty.UnsetValue)
        scaleFactor = new Vector(1.0, 1.0);
      Vector vector = new Vector();
      if (adorner != null)
      {
        switch (AdornerPanel.GetHorizontalStretch(adorner))
        {
          case AdornerStretch.None:
            vector.X = scaleFactor.X;
            break;
          case AdornerStretch.Stretch:
            vector.X = scaleFactor.X;
            break;
        }
        switch (AdornerPanel.GetVerticalStretch(adorner))
        {
          case AdornerStretch.None:
            vector.Y = scaleFactor.Y;
            break;
          case AdornerStretch.Stretch:
            vector.Y = scaleFactor.Y;
            break;
        }
      }
      return vector;
    }
  }
}
