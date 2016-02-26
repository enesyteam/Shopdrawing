// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerProperties
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using MS.Internal.Interaction;
using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public static class AdornerProperties
  {
    public static readonly DependencyProperty LayoutProperty = DependencyProperty.RegisterAttached("Layout", typeof (AdornerLayout), typeof (AdornerProperties), (PropertyMetadata) new FrameworkPropertyMetadata((object) TransformAwareAdornerLayout.Instance, FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(AdornerProperties.OnPropertyChanged)), new ValidateValueCallback(AdornerProperties.OnValidateNonNull));
    public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof (ModelItem), typeof (AdornerProperties), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(AdornerProperties.OnModelChanged)));
    public static readonly DependencyProperty OrderProperty = DependencyProperty.RegisterAttached("Order", typeof (AdornerOrder), typeof (AdornerProperties), new PropertyMetadata((object) AdornerOrder.Content, new PropertyChangedCallback(AdornerProperties.OnOrderChanged)), new ValidateValueCallback(AdornerProperties.OnValidateNonNull));
    public static readonly DependencyProperty RenderTransformProperty = DependencyProperty.RegisterAttached("RenderTransform", typeof (Transform), typeof (AdornerProperties), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsParentArrange, (PropertyChangedCallback) null));
    public static readonly DependencyProperty TaskProperty = DependencyProperty.RegisterAttached("Task", typeof (Task), typeof (AdornerProperties), new PropertyMetadata((object) null, new PropertyChangedCallback(AdornerProperties.OnPropertyChanged)));

    public static AdornerLayout GetLayout(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (AdornerLayout) adorner.GetValue(AdornerProperties.LayoutProperty);
    }

    public static void SetLayout(DependencyObject adorner, AdornerLayout value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerProperties.LayoutProperty, (object) value);
    }

    public static ModelItem GetModel(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (ModelItem) adorner.GetValue(AdornerProperties.ModelProperty);
    }

    public static void SetModel(DependencyObject adorner, ModelItem value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerProperties.ModelProperty, (object) value);
    }

    public static AdornerOrder GetOrder(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (AdornerOrder) adorner.GetValue(AdornerProperties.OrderProperty);
    }

    public static void SetOrder(DependencyObject adorner, AdornerOrder value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerProperties.OrderProperty, (object) value);
    }

    public static Task GetTask(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (Task) adorner.GetValue(AdornerProperties.TaskProperty);
    }

    public static void SetTask(DependencyObject adorner, Task value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerProperties.TaskProperty, (object) value);
    }

    public static ViewItem GetView(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      ModelItem model = AdornerProperties.GetModel(adorner);
      if (model != null)
        return model.View;
      return (ViewItem) null;
    }

    public static Transform GetRenderTransform(DependencyObject adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (Transform) adorner.GetValue(AdornerProperties.RenderTransformProperty);
    }

    public static void SetRenderTransform(DependencyObject adorner, Transform value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerProperties.RenderTransformProperty, (object) value);
    }

    private static void OnOrderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      UIElement adorner = sender as UIElement;
      if (adorner != null)
      {
        AdornerLayer adornerLayer = VisualTreeHelper.GetParent((DependencyObject) adorner) as AdornerLayer;
        if (adornerLayer != null)
          adornerLayer.OnOrderChanged(adorner);
      }
      AdornerProperties.GetLayout(sender).AdornerPropertyChanged(sender, args);
    }

    private static void OnModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      AdornerProperties.GetLayout(sender).AdornerPropertyChanged(sender, args);
    }

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      if (args.Property == AdornerProperties.LayoutProperty)
      {
        ((AdornerLayout) args.OldValue).AdornerPropertyChanged(sender, args);
        ((AdornerLayout) args.NewValue).AdornerPropertyChanged(sender, args);
        UIElement adorner = sender as UIElement;
        if (adorner == null)
          return;
        AdornerLayer adornerLayer = VisualTreeHelper.GetParent((DependencyObject) adorner) as AdornerLayer;
        if (adornerLayer == null)
          return;
        adornerLayer.OnLayoutChanged(adorner);
      }
      else
        AdornerProperties.GetLayout(sender).AdornerPropertyChanged(sender, args);
    }

    private static bool OnValidateNonNull(object value)
    {
      return value != null;
    }
  }
}
