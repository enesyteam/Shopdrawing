// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.BaseAdornerLayout
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using MS.Internal.Transforms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal.Interaction
{
  internal abstract class BaseAdornerLayout : AdornerLayout
  {
    private static readonly DependencyProperty CachedVisibilityProperty = DependencyProperty.RegisterAttached("CachedVisibility", typeof (Visibility), typeof (BaseAdornerLayout));
    private static readonly DependencyProperty OriginalVisibilityProperty = DependencyProperty.RegisterAttached("OriginalVisibility", typeof (Visibility), typeof (BaseAdornerLayout));
    private static readonly string RenderTransformProperty = "RenderTransform";
    protected static readonly DependencyProperty CacheProperty = DependencyProperty.RegisterAttached("Cache", typeof (BaseAdornerLayout.LayoutCache), typeof (BaseAdornerLayout));

    public override void AdornerPropertyChanged(DependencyObject adorner, DependencyPropertyChangedEventArgs args)
    {
      if (args.Property != AdornerProperties.ModelProperty || BaseAdornerLayout.GetCache(adorner).DesignerView == null)
        return;
      BaseAdornerLayout.EnsureActualValues(adorner);
    }

    public override bool EvaluateLayout(DesignerView view, UIElement adorner)
    {
      if (view == null)
        throw new ArgumentNullException("view");
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      BaseAdornerLayout.GetCache((DependencyObject) adorner).DesignerView = view;
      BaseAdornerLayout.EnsureActualValues((DependencyObject) adorner);
      ViewItem element = AdornerProperties.GetView((DependencyObject) adorner);
      if (BaseAdornerLayout.IsAdornableElement(view, element))
      {
        object obj = adorner.ReadLocalValue(BaseAdornerLayout.OriginalVisibilityProperty);
        if (obj != DependencyProperty.UnsetValue)
        {
          adorner.SetValue(UIElement.VisibilityProperty, obj);
          adorner.ClearValue(BaseAdornerLayout.OriginalVisibilityProperty);
        }
      }
      else
      {
        if (element != (ViewItem) null && adorner.ReadLocalValue(BaseAdornerLayout.OriginalVisibilityProperty) == DependencyProperty.UnsetValue)
        {
          adorner.SetValue(BaseAdornerLayout.OriginalVisibilityProperty, (object) adorner.Visibility);
          adorner.Visibility = Visibility.Collapsed;
        }
        element = (ViewItem) null;
      }
      BaseAdornerLayout.CheckAndInvalidateAdorner(view, element, adorner);
      return true;
    }

    private static void CheckAndInvalidateAdorner(DesignerView view, ViewItem element, UIElement adorner)
    {
      if (view.Context == null)
        return;
      Matrix m1 = TransformUtil.GetTransformToImmediateParent((DependencyObject) view).Value;
      Matrix identity;
      Size s1;
      if (element != (ViewItem) null)
      {
        identity = TransformUtil.GetSelectionFrameTransformToDesignerView(view.Context, element).Value;
        s1 = element.RenderSize;
      }
      else
      {
        identity = Matrix.Identity;
        s1 = Size.Empty;
      }
      BaseAdornerLayout.LayoutCache cache = BaseAdornerLayout.GetCache((DependencyObject) adorner);
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (element != (ViewItem) null && !MathUtilities.AreClose(s1, cache.RenderSize))
        flag1 = true;
      if (element != (ViewItem) null && !MathUtilities.AreClose(identity, cache.ElementToDesignerViewTransformMatrix))
      {
        flag1 = true;
        flag2 = true;
      }
      if (!MathUtilities.AreClose(m1, cache.DesignerViewToViewportMatrix))
      {
        flag1 = true;
        flag2 = true;
      }
      if (element != (ViewItem) null && !element.IsVisible)
      {
        ViewItem view1 = view.Context.Services.GetRequiredService<ModelService>().Root.View;
        for (ViewItem viewItem = element; viewItem != view1 && viewItem != (ViewItem) null; viewItem = viewItem.VisualParent)
        {
          if (viewItem.Visibility == Visibility.Collapsed)
          {
            flag3 = true;
            break;
          }
        }
      }
      if (flag3)
      {
        object obj = adorner.ReadLocalValue(UIElement.VisibilityProperty);
        if (adorner.ReadLocalValue(BaseAdornerLayout.CachedVisibilityProperty) == DependencyProperty.UnsetValue)
        {
          if (obj != DependencyProperty.UnsetValue)
            adorner.SetValue(BaseAdornerLayout.CachedVisibilityProperty, obj);
          else
            adorner.SetValue(BaseAdornerLayout.CachedVisibilityProperty, (object) Visibility.Visible);
          adorner.Visibility = Visibility.Collapsed;
        }
      }
      else
      {
        object obj = adorner.ReadLocalValue(BaseAdornerLayout.CachedVisibilityProperty);
        if (obj != DependencyProperty.UnsetValue)
          adorner.SetValue(UIElement.VisibilityProperty, obj);
        adorner.ClearValue(BaseAdornerLayout.CachedVisibilityProperty);
      }
      if (!flag1 && !flag2)
        return;
      if (element != (ViewItem) null)
      {
        cache.RenderSize = s1;
        cache.ElementToDesignerViewTransformMatrix = identity;
        cache.PlatformObjectHashCode = element.PlatformObject.GetHashCode();
      }
      cache.DesignerViewToViewportMatrix = m1;
      cache.DesignerView = view;
      UIElement uiElement = VisualTreeHelper.GetParent((DependencyObject) adorner) as UIElement;
      if (flag1)
      {
        adorner.InvalidateMeasure();
        if (uiElement != null)
          uiElement.InvalidateMeasure();
      }
      if (!flag2)
        return;
      adorner.InvalidateVisual();
      if (uiElement == null)
        return;
      uiElement.InvalidateVisual();
    }

    protected static BaseAdornerLayout.LayoutCache GetCache(DependencyObject element)
    {
      BaseAdornerLayout.LayoutCache layoutCache = (BaseAdornerLayout.LayoutCache) element.GetValue(BaseAdornerLayout.CacheProperty);
      if (layoutCache == null)
      {
        layoutCache = new BaseAdornerLayout.LayoutCache();
        element.SetValue(BaseAdornerLayout.CacheProperty, (object) layoutCache);
      }
      return layoutCache;
    }

    private static bool IsAdornableElement(DesignerView view, ViewItem element)
    {
      if (element != (ViewItem) null)
        return element.IsDescendantOf((Visual) view);
      return false;
    }

    public override void Measure(UIElement adorner, Size constraint)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
      adorner.Measure(availableSize);
    }

    public override bool IsAssociated(UIElement adorner, ModelItem item)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      if (item == null)
        throw new ArgumentNullException("item");
      return AdornerProperties.GetModel((DependencyObject) adorner) == item;
    }

    private static void EnsureActualValues(DependencyObject adorner)
    {
      ViewItem view = AdornerProperties.GetView(adorner);
      ModelItem model = AdornerProperties.GetModel(adorner);
      BaseAdornerLayout.LayoutCache cache = BaseAdornerLayout.GetCache(adorner);
      if (cache.Model == model && !(cache.View != view))
        return;
      if (cache.Model != null)
        cache.Model.PropertyChanged -= new PropertyChangedEventHandler(BaseAdornerLayout.OnModelItemPropertyChanged);
      cache.Model = model;
      cache.View = view;
      if (model == null)
        return;
      model.PropertyChanged += new PropertyChangedEventHandler(BaseAdornerLayout.OnModelItemPropertyChanged);
    }

    private static void OnModelItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == BaseAdornerLayout.RenderTransformProperty))
        return;
      ModelItem modelItem = sender as ModelItem;
      if (modelItem == null || !(modelItem.View != (ViewItem) null))
        return;
      DesignerView designerView = DesignerView.FromContext(modelItem.Context);
      if (designerView == null)
        return;
      designerView.InvalidateArrange();
    }

    internal static double ValidateDouble(double requested, double fallback)
    {
      if (double.IsNaN(requested) || double.IsInfinity(requested))
        return fallback;
      return requested;
    }

    protected class LayoutCache
    {
      internal Size RenderSize;
      internal Matrix ElementToDesignerViewTransformMatrix;
      internal Matrix DesignerViewToViewportMatrix;
      internal ModelItem Model;
      internal ViewItem View;
      internal DesignerView DesignerView;
      internal int PlatformObjectHashCode;

      internal Vector CalculateZoom()
      {
        return TransformUtil.GetScaleFromMatrix(this.DesignerViewToViewportMatrix);
      }
    }
  }
}
