// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.TransformAwareAdornerLayout
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using MS.Internal;
using MS.Internal.Transforms;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal class TransformAwareAdornerLayout : BaseAdornerLayout
  {
    internal static TransformAwareAdornerLayout Instance = new TransformAwareAdornerLayout();
    internal static readonly DependencyProperty DesignerElementScalingFactorWithZoom = DependencyProperty.RegisterAttached("ScaleToRoot", typeof (Vector), typeof (TransformAwareAdornerLayout), new PropertyMetadata((object) new Vector(1.0, 1.0), (PropertyChangedCallback) null, new CoerceValueCallback(TransformAwareAdornerLayout.MakeZoomPositive)));

    internal static object MakeZoomPositive(DependencyObject d, object baseValue)
    {
      if (baseValue != null)
        return (object) VectorUtilities.RemoveMirror((Vector) baseValue);
      return baseValue;
    }

    private void SetupTransform(UIElement adorner)
    {
      BaseAdornerLayout.LayoutCache cache = BaseAdornerLayout.GetCache((DependencyObject) adorner);
      CanonicalTransform canonicalTransform1 = new CanonicalTransform(cache.ElementToDesignerViewTransformMatrix);
      CanonicalTransform canonicalTransform2 = new CanonicalTransform(cache.DesignerViewToViewportMatrix);
      Vector vector = VectorUtilities.Scale(canonicalTransform1.Scale, canonicalTransform2.Scale);
      adorner.SetValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom, (object) vector);
      CanonicalTransform canonicalTransform3 = new CanonicalTransform(canonicalTransform1);
      canonicalTransform3.Scale = VectorUtilities.Unscale(new Vector((double) Math.Sign(vector.X), (double) Math.Sign(vector.Y)), canonicalTransform2.Scale);
      AdornerPanel adornerPanel = adorner as AdornerPanel;
      if (adornerPanel != null)
      {
        if (!adornerPanel.UseMirrorTransform)
          canonicalTransform3.Scale = VectorUtilities.Unscale(new Vector(1.0, 1.0), canonicalTransform2.Scale);
        else
          adornerPanel.IsMirroredTransform = vector.X < 0.0;
      }
      Transform renderTransform = AdornerProperties.GetRenderTransform((DependencyObject) adorner);
      Transform transform = (Transform) canonicalTransform3.ToTransform();
      if (renderTransform != null)
        transform = (Transform) new TransformGroup()
        {
          Children = {
            transform,
            renderTransform
          }
        };
      adorner.RenderTransform = transform;
    }

    public override void Arrange(UIElement adorner)
    {
      BaseAdornerLayout.LayoutCache cache = BaseAdornerLayout.GetCache((DependencyObject) adorner);
      Matrix m1_1 = cache.ElementToDesignerViewTransformMatrix;
      Matrix m1_2 = cache.DesignerViewToViewportMatrix;
      if (!MathUtilities.AreClose(m1_1, cache.ElementToDesignerViewTransformMatrix) || !MathUtilities.AreClose(m1_2, cache.DesignerViewToViewportMatrix))
        this.SetupTransform(adorner);
      Vector scale = (Vector) adorner.GetValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom);
      ViewItem view = AdornerProperties.GetView((DependencyObject) adorner);
      this.SetAdornerBounds(adorner, view, new Point(0.0, 0.0), scale);
      if (!(view != (ViewItem) null) || cache.PlatformObjectHashCode == 0 || cache.PlatformObjectHashCode == view.PlatformObject.GetHashCode())
        return;
      cache.View = view;
      cache.RenderSize = view.RenderSize;
      cache.PlatformObjectHashCode = view.PlatformObject.GetHashCode();
    }

    public override Size ArrangeChildren(FrameworkElement parent, UIElementCollection internalChildren, Size finalSize)
    {
      AdornerPanel adornerPanel = parent as AdornerPanel;
      if (adornerPanel == null)
        return finalSize;
      ModelItem model = adornerPanel.Model;
      ViewItem adornedElement = model == null ? (ViewItem) null : model.View;
      Vector vector = (Vector) parent.GetValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom);
      foreach (UIElement uiElement in internalChildren)
      {
        uiElement.SetValue(TransformAwareAdornerLayout.DesignerElementScalingFactorWithZoom, (object) vector);
        AdornerPlacementCollection currentPlacements = AdornerPanel.GetCurrentPlacements(uiElement);
        currentPlacements.ComputePlacement(AdornerCoordinateSpaces.Default, uiElement, adornedElement, new Vector(1.0, 1.0), finalSize);
        Rect finalRect = new Rect((Point) currentPlacements.TopLeft, (Size) currentPlacements.Size);
        finalRect.Width = BaseAdornerLayout.ValidateDouble(finalRect.Width, uiElement.RenderSize.Width);
        finalRect.Height = BaseAdornerLayout.ValidateDouble(finalRect.Height, uiElement.RenderSize.Height);
        uiElement.Arrange(finalRect);
        if (uiElement is FrameworkElement)
        {
          if (adornerPanel.IsMirroredTransform)
          {
            uiElement.SetValue(FrameworkElement.FlowDirectionProperty, (object) FlowDirection.RightToLeft);
            uiElement.RenderTransform = TransformAwareAdornerLayout.RTLAdornerTransformGroup.Create(uiElement, finalRect.Width);
          }
          else
          {
            uiElement.RenderTransform = TransformAwareAdornerLayout.RTLAdornerTransformGroup.Unwrap(uiElement);
            uiElement.SetValue(FrameworkElement.FlowDirectionProperty, (object) FlowDirection.LeftToRight);
          }
        }
      }
      return finalSize;
    }

    public override bool EvaluateLayout(DesignerView view, UIElement adorner)
    {
      BaseAdornerLayout.LayoutCache cache = BaseAdornerLayout.GetCache((DependencyObject) adorner);
      Matrix m1_1 = cache.ElementToDesignerViewTransformMatrix;
      Matrix m1_2 = cache.DesignerViewToViewportMatrix;
      bool flag = base.EvaluateLayout(view, adorner);
      if (!MathUtilities.AreClose(m1_1, cache.ElementToDesignerViewTransformMatrix) || !MathUtilities.AreClose(m1_2, cache.DesignerViewToViewportMatrix))
        this.SetupTransform(adorner);
      return flag;
    }

    private void SetAdornerBounds(UIElement childAdorner, ViewItem adornedElement, Point location, Vector scale)
    {
      AdornerCoordinateSpace @default = AdornerCoordinateSpaces.Default;
      Rect finalRect;
      if (adornedElement != (ViewItem) null)
      {
        finalRect = @default.GetBoundingBox(adornedElement);
      }
      else
      {
        Size desiredSize = childAdorner.DesiredSize;
        finalRect = new Rect();
        finalRect.Width = desiredSize.Width;
        finalRect.Height = desiredSize.Height;
      }
      finalRect.X = location.X;
      finalRect.Y = location.Y;
      finalRect.Width *= Math.Abs(scale.X);
      finalRect.Height *= Math.Abs(scale.Y);
      childAdorner.Arrange(finalRect);
    }

    private class RTLAdornerTransformGroup
    {
      private static string AutomationID = typeof (TransformAwareAdornerLayout.RTLAdornerTransformGroup).AssemblyQualifiedName;
      private static Matrix _rtlTransformStub = new Matrix(-1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

      public static Transform Create(UIElement element, double widthOfControl)
      {
        Transform transform = TransformAwareAdornerLayout.RTLAdornerTransformGroup.Unwrap(element);
        TransformGroup transformGroup = new TransformGroup();
        AutomationProperties.SetAutomationId((DependencyObject) transformGroup, TransformAwareAdornerLayout.RTLAdornerTransformGroup.AutomationID);
        if (transform != null)
          transformGroup.Children.Add(transform);
        Matrix m = TransformAwareAdornerLayout.RTLAdornerTransformGroup._rtlTransformStub;
        m.OffsetX = widthOfControl;
        transformGroup.Children.Add((Transform) new MatrixTransform(TransformUtil.SafeInvert(m)));
        return (Transform) transformGroup;
      }

      public static Transform Unwrap(UIElement element)
      {
        TransformGroup transformGroup = element.RenderTransform as TransformGroup;
        if (transformGroup != null && transformGroup.Children.Count > 0 && AutomationProperties.GetAutomationId((DependencyObject) transformGroup) == TransformAwareAdornerLayout.RTLAdornerTransformGroup.AutomationID)
          return transformGroup.Children[0];
        return element.RenderTransform;
      }
    }
  }
}
