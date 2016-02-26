// Decompiled with JetBrains decompiler
// Type: MS.Internal.Interaction.AdornerLayer
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using MS.Internal.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MS.Internal.Interaction
{
  internal class AdornerLayer : Panel
  {
    private static Type TransformType = typeof (GeneralTransform);
    private AdornerLayer.AdornerCollection _adornerCollectionWrapper;
    private ViewService _viewService;
    private EditingContext _context;
    private DesignerView _currentDesignerView;
    private List<UIElement> _evaluateLayoutList;
    private bool _needEvaluateLayout;
    private bool _hasRendered;
    private Visibility _savedVisibility;
    private AdornerLayer.ProcessZoomCallback _processZoomCallback;

    internal ICollection<UIElement> Adorners
    {
      get
      {
        if (this._adornerCollectionWrapper == null)
          this._adornerCollectionWrapper = new AdornerLayer.AdornerCollection(this);
        return (ICollection<UIElement>) this._adornerCollectionWrapper;
      }
    }

    private DesignerView CurrentDesignerView
    {
      get
      {
        return this._currentDesignerView;
      }
      set
      {
        if (this._currentDesignerView == value)
          return;
        if (this._currentDesignerView != null)
          this._currentDesignerView.ZoomLevelChanged -= new EventHandler(this.OnZoomLevelChanged);
        this._currentDesignerView = value;
        if (this._currentDesignerView == null)
          return;
        this._currentDesignerView.ZoomLevelChanged += new EventHandler(this.OnZoomLevelChanged);
      }
    }

    internal AdornerLayer()
    {
      this.InheritanceBehavior = InheritanceBehavior.SkipToAppNext;
      DesignerProperties.SetIsInDesignMode((DependencyObject) this, false);
    }

    private void Add(UIElement adorner)
    {
      if (LogicalTreeHelper.GetParent((DependencyObject) adorner) != null)
          throw new ArgumentException(MS.Internal.Properties.Resources.Error_AdornerHasParent);
      this.InsertAdornerIntoVisualTree(adorner);
      if (VisualTreeHelper.GetParent((DependencyObject) this) == null)
        return;
      if (this._hasRendered)
      {
        if (this._evaluateLayoutList == null)
          this._evaluateLayoutList = new List<UIElement>();
        this._evaluateLayoutList.Add(adorner);
      }
      else
        this._needEvaluateLayout = true;
    }

    private void EvaluateLayout()
    {
      if (this.Visibility != Visibility.Collapsed && this._hasRendered)
      {
        if (this.VisualChildrenCount <= 0 || this.CurrentDesignerView == null)
          return;
        List<UIElement> list = (List<UIElement>) null;
        foreach (UIElement adorner in this.InternalChildren)
        {
          if (!AdornerProperties.GetLayout((DependencyObject) adorner).EvaluateLayout(this.CurrentDesignerView, adorner))
          {
            if (list == null)
              list = new List<UIElement>();
            list.Add(adorner);
          }
        }
        if (list == null)
          return;
        foreach (UIElement element in list)
          this.InternalChildren.Remove(element);
      }
      else
        this._needEvaluateLayout = true;
    }

    private void InsertAdornerIntoVisualTree(UIElement adorner)
    {
      int index = 0;
      AdornerOrder order1 = AdornerProperties.GetOrder((DependencyObject) adorner);
      foreach (DependencyObject adorner1 in this.InternalChildren)
      {
        AdornerOrder order2 = AdornerProperties.GetOrder(adorner1);
        if (order1.CompareTo((OrderToken) order2) <= 0)
          ++index;
        else
          break;
      }
      this.InternalChildren.Insert(index, adorner);
    }

    internal void OnOrderChanged(UIElement adorner)
    {
      this.InternalChildren.Remove(adorner);
      this.InsertAdornerIntoVisualTree(adorner);
      adorner.InvalidateVisual();
    }

    internal void OnLayoutChanged(UIElement adorner)
    {
      if (VisualTreeHelper.GetParent((DependencyObject) this) == null)
        return;
      AdornerProperties.GetLayout((DependencyObject) adorner).EvaluateLayout(this.CurrentDesignerView, adorner);
      this.InvalidateArrange();
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      this.CurrentDesignerView = VisualTreeHelper.GetParent((DependencyObject) this) as DesignerView;
      base.OnVisualParentChanged(oldParent);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      Performance.StartTiming(PerformanceMarks.AdornerArrange);
      if (VisualTreeHelper.GetParent((DependencyObject) this) != null)
      {
        if (this._needEvaluateLayout)
        {
          this._needEvaluateLayout = false;
          this._evaluateLayoutList = (List<UIElement>) null;
          this.EvaluateLayout();
        }
        else if (this._evaluateLayoutList != null)
        {
          List<UIElement> list = this._evaluateLayoutList;
          this._evaluateLayoutList = (List<UIElement>) null;
          DesignerView currentDesignerView = this.CurrentDesignerView;
          foreach (UIElement adorner in list)
            AdornerProperties.GetLayout((DependencyObject) adorner).EvaluateLayout(currentDesignerView, adorner);
        }
        foreach (UIElement adorner in this.InternalChildren)
          AdornerProperties.GetLayout((DependencyObject) adorner).Arrange(adorner);
      }
      Performance.StopTiming(PerformanceMarks.AdornerArrange);
      return arrangeSize;
    }

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);
      this._hasRendered = true;
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property == UIElement.VisibilityProperty && this._processZoomCallback != null)
        this._savedVisibility = (Visibility) e.NewValue;
      base.OnPropertyChanged(e);
    }

    private void Clear()
    {
      this.InternalChildren.Clear();
      this._evaluateLayoutList = (List<UIElement>) null;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      Performance.StartTiming(PerformanceMarks.AdornerMeasure);
      if (VisualTreeHelper.GetParent((DependencyObject) this) != null)
      {
        foreach (UIElement adorner in this.InternalChildren)
          AdornerProperties.GetLayout((DependencyObject) adorner).Measure(adorner, constraint);
      }
      Performance.StopTiming(PerformanceMarks.AdornerMeasure);
      return new Size();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);
      this.EvaluateLayout();
    }

    private void OnLayoutUpdated(object sender, EventArgs args)
    {
      this.EvaluateLayout();
    }

    private void OnViewServiceAvailable(ViewService vs)
    {
      this._context.Services.Unsubscribe<ViewService>(new SubscribeServiceCallback<ViewService>(this.OnViewServiceAvailable));
      this._viewService = vs;
      this._viewService.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    }

    private void OnZoomLevelChanged(object sender, EventArgs e)
    {
      if (this._processZoomCallback != null)
        return;
      this._savedVisibility = this.Visibility;
      if (this._hasRendered)
        this.Visibility = Visibility.Collapsed;
      this._processZoomCallback = new AdornerLayer.ProcessZoomCallback(this.ProcessZoom);
      this.Dispatcher.BeginInvoke((Delegate) this._processZoomCallback, DispatcherPriority.Background, new object[0]);
    }

    private void ProcessZoom()
    {
      this.Visibility = this._savedVisibility;
      this.EvaluateLayout();
      this.InvalidateArrange();
      this._processZoomCallback = (AdornerLayer.ProcessZoomCallback) null;
    }

    private bool Remove(UIElement adorner)
    {
      DependencyObject parent = LogicalTreeHelper.GetParent((DependencyObject) adorner);
      if (parent == null)
        return false;
      if (parent != this)
          throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, MS.Internal.Properties.Resources.Error_AdornerNotParentedToThisAdornerLayer, new object[0]));
      this.InternalChildren.Remove(adorner);
      if (this._evaluateLayoutList != null)
        this._evaluateLayoutList.Remove(adorner);
      return true;
    }

    internal void SetContext(EditingContext context)
    {
      if (this._context != null)
      {
        if (this._viewService == null)
        {
          this._context.Services.Unsubscribe<ViewService>(new SubscribeServiceCallback<ViewService>(this.OnViewServiceAvailable));
        }
        else
        {
          this._viewService.LayoutUpdated -= new EventHandler(this.OnLayoutUpdated);
          this._viewService = (ViewService) null;
        }
      }
      this._context = context;
      if (this._context == null)
        return;
      this._viewService = this._context.Services.GetService<ViewService>();
      if (this._viewService == null)
        this._context.Services.Subscribe<ViewService>(new SubscribeServiceCallback<ViewService>(this.OnViewServiceAvailable));
      else
        this._viewService.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    }

    private delegate void ProcessZoomCallback();

    private class AdornerCollection : ICollection<UIElement>, IEnumerable<UIElement>, IEnumerable
    {
      private AdornerLayer _adornerLayer;

      public int Count
      {
        get
        {
          int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) this._adornerLayer);
          int num = 0;
          for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
          {
            if (VisualTreeHelper.GetChild((DependencyObject) this._adornerLayer, childIndex) is UIElement)
              ++num;
          }
          return num;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      internal AdornerCollection(AdornerLayer adornerLayer)
      {
        this._adornerLayer = adornerLayer;
      }

      public void Add(UIElement item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        this._adornerLayer.Add(item);
      }

      public void Clear()
      {
        this._adornerLayer.Clear();
      }

      public bool Contains(UIElement item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) this._adornerLayer);
        for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
        {
          if (VisualTreeHelper.GetChild((DependencyObject) this._adornerLayer, childIndex) == item)
            return true;
        }
        return false;
      }

      public void CopyTo(UIElement[] array, int arrayIndex)
      {
        if (array == null)
          throw new ArgumentNullException("array");
        int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) this._adornerLayer);
        for (int childIndex = arrayIndex; childIndex < childrenCount; ++childIndex)
        {
          UIElement uiElement = VisualTreeHelper.GetChild((DependencyObject) this._adornerLayer, childIndex) as UIElement;
          if (uiElement != null)
            array.SetValue((object) uiElement, arrayIndex++);
        }
      }

      public bool Remove(UIElement item)
      {
        if (item == null)
          throw new ArgumentNullException("item");
        return this._adornerLayer.Remove(item);
      }

      public IEnumerator<UIElement> GetEnumerator()
      {
        int childCount = VisualTreeHelper.GetChildrenCount((DependencyObject) this._adornerLayer);
        for (int idx = 0; idx < childCount; ++idx)
        {
          UIElement a = VisualTreeHelper.GetChild((DependencyObject) this._adornerLayer, idx) as UIElement;
          if (a != null)
            yield return a;
        }
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }
  }
}
