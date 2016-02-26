// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerPanel
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.Model;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Windows.Design.Interaction
{
  public class AdornerPanel : Panel
  {
    public static readonly DependencyProperty IsContentFocusableProperty = DependencyProperty.Register("IsContentFocusable", typeof (bool), typeof (AdornerPanel), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ModelProperty = AdornerProperties.ModelProperty.AddOwner(typeof (AdornerPanel));
    public static readonly DependencyProperty OrderProperty = AdornerProperties.OrderProperty.AddOwner(typeof (AdornerPanel));
    public static readonly DependencyProperty PlacementsProperty = DependencyProperty.RegisterAttached("Placements", typeof (AdornerPlacementCollection), typeof (AdornerPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new AdornerPlacementCollection(), FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static readonly DependencyProperty HorizontalStretchProperty = DependencyProperty.RegisterAttached("HorizontalStretch", typeof (AdornerStretch), typeof (AdornerPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AdornerStretch.None, FrameworkPropertyMetadataOptions.AffectsParentMeasure), (ValidateValueCallback) (target =>
    {
      AdornerStretch adornerStretch = (AdornerStretch) target;
      if (adornerStretch != AdornerStretch.None)
        return adornerStretch == AdornerStretch.Stretch;
      return true;
    }));
    public static readonly DependencyProperty VerticalStretchProperty = DependencyProperty.RegisterAttached("VerticalStretch", typeof (AdornerStretch), typeof (AdornerPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AdornerStretch.None, FrameworkPropertyMetadataOptions.AffectsParentMeasure), (ValidateValueCallback) (target =>
    {
      AdornerStretch adornerStretch = (AdornerStretch) target;
      if (adornerStretch != AdornerStretch.None)
        return adornerStretch == AdornerStretch.Stretch;
      return true;
    }));
    public static readonly DependencyProperty AdornerHorizontalAlignmentProperty = DependencyProperty.RegisterAttached("AdornerHorizontalAlignment", typeof (AdornerHorizontalAlignment), typeof (AdornerPanel), (PropertyMetadata) new UIPropertyMetadata((object) AdornerHorizontalAlignment.Left, new PropertyChangedCallback(AdornerPanel.OnAdornerAlignmentChanged)));
    public static readonly DependencyProperty AdornerVerticalAlignmentProperty = DependencyProperty.RegisterAttached("AdornerVerticalAlignment", typeof (AdornerVerticalAlignment), typeof (AdornerPanel), (PropertyMetadata) new UIPropertyMetadata((object) AdornerVerticalAlignment.Top, new PropertyChangedCallback(AdornerPanel.OnAdornerAlignmentChanged)));
    public static readonly DependencyProperty AdornerMarginProperty = DependencyProperty.RegisterAttached("AdornerMargin", typeof (Thickness), typeof (AdornerPanel), (PropertyMetadata) new UIPropertyMetadata((object) new Thickness(0.0), new PropertyChangedCallback(AdornerPanel.OnAdornerAlignmentChanged)));
    private static readonly DependencyProperty SyntaticSugarPlacementsProperty = DependencyProperty.RegisterAttached("SyntaticSugarPlacements", typeof (AdornerPanel.SyntaticSugarPlacementCollection), typeof (AdornerPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new AdornerPanel.SyntaticSugarPlacementCollection(), FrameworkPropertyMetadataOptions.AffectsParentArrange));
    private Rect _offsetRect = new Rect();
    private bool _isMirroredTransform;

    public bool IsContentFocusable
    {
      get
      {
        return (bool) this.GetValue(AdornerPanel.IsContentFocusableProperty);
      }
      set
      {
        this.SetValue(AdornerPanel.IsContentFocusableProperty, (object) (bool) (value ? true : false));
      }
    }

    protected internal virtual bool UseMirrorTransform
    {
      get
      {
        return true;
      }
    }

    internal bool IsMirroredTransform
    {
      get
      {
        if (this.UseMirrorTransform)
          return this._isMirroredTransform;
        return false;
      }
      set
      {
        this._isMirroredTransform = value;
      }
    }

    public ViewItem View
    {
      get
      {
        ModelItem model = this.Model;
        if (model != null)
          return model.View;
        return (ViewItem) null;
      }
    }

    public ModelItem Model
    {
      get
      {
        return AdornerProperties.GetModel((DependencyObject) this);
      }
      set
      {
        AdornerProperties.SetModel((DependencyObject) this, value);
      }
    }

    public AdornerOrder Order
    {
      get
      {
        return AdornerProperties.GetOrder((DependencyObject) this);
      }
      set
      {
        AdornerProperties.SetOrder((DependencyObject) this, value);
      }
    }

    protected internal Rect OffsetRect
    {
      get
      {
        return this._offsetRect;
      }
      set
      {
        this._offsetRect = value;
      }
    }

    internal static AdornerPlacementCollection GetCurrentPlacements(UIElement adorner)
    {
      AdornerPlacementCollection placements = AdornerPanel.GetPlacements(adorner);
      if (placements == null || placements.Count == 0)
        return (AdornerPlacementCollection) AdornerPanel.GetSyntaticSugarPlacements((DependencyObject) adorner);
      return placements;
    }

    public static AdornerPlacementCollection GetPlacements(UIElement adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (AdornerPlacementCollection) adorner.GetValue(AdornerPanel.PlacementsProperty);
    }

    public static void SetPlacements(UIElement adorner, AdornerPlacementCollection value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      adorner.SetValue(AdornerPanel.PlacementsProperty, (object) value);
    }

    public static AdornerStretch GetHorizontalStretch(UIElement adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (AdornerStretch) adorner.GetValue(AdornerPanel.HorizontalStretchProperty);
    }

    public static void SetHorizontalStretch(UIElement adorner, AdornerStretch value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      if (!EnumValidator.IsValid(value))
        throw new ArgumentOutOfRangeException("value");
      adorner.SetValue(AdornerPanel.HorizontalStretchProperty, (object) value);
    }

    public static AdornerStretch GetVerticalStretch(UIElement adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return (AdornerStretch) adorner.GetValue(AdornerPanel.VerticalStretchProperty);
    }

    public static void SetVerticalStretch(UIElement adorner, AdornerStretch value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      if (!EnumValidator.IsValid(value))
        throw new ArgumentOutOfRangeException("value");
      adorner.SetValue(AdornerPanel.VerticalStretchProperty, (object) value);
    }

    public static AdornerHorizontalAlignment GetAdornerHorizontalAlignment(DependencyObject obj)
    {
      return (AdornerHorizontalAlignment) obj.GetValue(AdornerPanel.AdornerHorizontalAlignmentProperty);
    }

    public static void SetAdornerHorizontalAlignment(DependencyObject obj, AdornerHorizontalAlignment value)
    {
      obj.SetValue(AdornerPanel.AdornerHorizontalAlignmentProperty, (object) value);
    }

    public static AdornerVerticalAlignment GetAdornerVerticalAlignment(DependencyObject obj)
    {
      return (AdornerVerticalAlignment) obj.GetValue(AdornerPanel.AdornerVerticalAlignmentProperty);
    }

    public static void SetAdornerVerticalAlignment(DependencyObject obj, AdornerVerticalAlignment value)
    {
      obj.SetValue(AdornerPanel.AdornerVerticalAlignmentProperty, (object) value);
    }

    public static Thickness GetAdornerMargin(DependencyObject obj)
    {
      return (Thickness) obj.GetValue(AdornerPanel.AdornerMarginProperty);
    }

    public static void SetAdornerMargin(DependencyObject obj, Thickness value)
    {
      obj.SetValue(AdornerPanel.AdornerMarginProperty, (object) value);
    }

    private static void OnAdornerAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      AdornerHorizontalAlignment horizontalAlignment = (AdornerHorizontalAlignment) d.GetValue(AdornerPanel.AdornerHorizontalAlignmentProperty);
      AdornerVerticalAlignment verticalAlignment = (AdornerVerticalAlignment) d.GetValue(AdornerPanel.AdornerVerticalAlignmentProperty);
      Thickness thickness = (Thickness) d.GetValue(AdornerPanel.AdornerMarginProperty);
      AdornerPanel.SetSyntaticSugarPlacements(d, new AdornerPanel.SyntaticSugarPlacementCollection()
      {
        VerticalAlign = verticalAlignment,
        HorizontalAlign = horizontalAlignment,
        Margin = thickness
      });
    }

    private static AdornerPanel.SyntaticSugarPlacementCollection GetSyntaticSugarPlacements(DependencyObject obj)
    {
      return (AdornerPanel.SyntaticSugarPlacementCollection) obj.GetValue(AdornerPanel.SyntaticSugarPlacementsProperty);
    }

    private static void SetSyntaticSugarPlacements(DependencyObject obj, AdornerPanel.SyntaticSugarPlacementCollection value)
    {
      obj.SetValue(AdornerPanel.SyntaticSugarPlacementsProperty, (object) value);
    }

    public static Task GetTask(UIElement adorner)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      return AdornerProperties.GetTask((DependencyObject) adorner);
    }

    public static void SetTask(UIElement adorner, Task value)
    {
      if (adorner == null)
        throw new ArgumentNullException("adorner");
      AdornerProperties.SetTask((DependencyObject) adorner, value);
    }

    public static AdornerPanel FromVisual(DependencyObject visual)
    {
      if (visual == null)
        throw new ArgumentNullException("visual");
      AdornerPanel adornerPanel = (AdornerPanel) null;
      for (DependencyObject parent = VisualTreeHelper.GetParent(visual); parent != null; parent = VisualTreeHelper.GetParent(parent))
      {
        adornerPanel = parent as AdornerPanel;
        if (adornerPanel != null)
          break;
      }
      return adornerPanel;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Size availableSize1 = new Size(double.PositiveInfinity, double.PositiveInfinity);
      int childrenCount = VisualTreeHelper.GetChildrenCount((DependencyObject) this);
      for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
      {
        UIElement uiElement = VisualTreeHelper.GetChild((DependencyObject) this, childIndex) as UIElement;
        if (uiElement != null)
          uiElement.Measure(availableSize1);
      }
      return new Size(0.0, 0.0);
    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
      if (this.ClipToBounds)
        return base.GetLayoutClip(layoutSlotSize);
      return (Geometry) null;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      return AdornerProperties.GetLayout((DependencyObject) this).ArrangeChildren((FrameworkElement) this, this.InternalChildren, finalSize);
    }

    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnPreviewGotKeyboardFocus(e);
      if (this.IsContentFocusable)
        return;
      e.Handled = true;
    }

    [Conditional("DEBUG")]
    internal static void Trace(string format, params object[] data)
    {
    }

    private class SyntaticSugarPlacementCollection : AdornerPlacementCollection
    {
      private AdornerHorizontalAlignment _hAlign;
      private AdornerVerticalAlignment _vAlign;
      private Thickness _adornerMargin;

      public AdornerHorizontalAlignment HorizontalAlign
      {
        get
        {
          return this._hAlign;
        }
        set
        {
          if (this._hAlign == value)
            return;
          this._hAlign = value;
          this.Clear();
        }
      }

      public AdornerVerticalAlignment VerticalAlign
      {
        get
        {
          return this._vAlign;
        }
        set
        {
          if (this._vAlign == value)
            return;
          this._vAlign = value;
          this.Clear();
        }
      }

      public Thickness Margin
      {
        get
        {
          return this._adornerMargin;
        }
        set
        {
          if (!(this._adornerMargin != value))
            return;
          this._adornerMargin = value;
          this.Clear();
        }
      }

      internal override void ComputePlacement(AdornerCoordinateSpace space, UIElement adorner, ViewItem adornedElement, Vector zoom, Size finalSize)
      {
        if (this.Items.Count == 0)
          this.Populate();
        base.ComputePlacement(space, adorner, adornedElement, zoom, finalSize);
      }

      private void Populate()
      {
        Thickness margin = this.Margin;
        switch (this.HorizontalAlign)
        {
          case AdornerHorizontalAlignment.Left:
            this.SizeRelativeToAdornerDesiredWidth(1.0, 0.0);
            this.PositionRelativeToContentWidth(0.0, margin.Left);
            break;
          case AdornerHorizontalAlignment.Center:
            this.SizeRelativeToAdornerDesiredWidth(1.0, 0.0);
            this.PositionRelativeToContentWidth(0.5, 0.0);
            this.PositionRelativeToAdornerWidth(-0.5, margin.Left);
            break;
          case AdornerHorizontalAlignment.Right:
            this.SizeRelativeToAdornerDesiredWidth(1.0, 0.0);
            this.PositionRelativeToContentWidth(1.0, -margin.Right);
            break;
          case AdornerHorizontalAlignment.Stretch:
            this.SizeRelativeToContentWidth(1.0, -(margin.Left + margin.Right));
            this.PositionRelativeToContentWidth(0.0, margin.Left);
            break;
          case AdornerHorizontalAlignment.OutsideLeft:
            this.SizeRelativeToAdornerDesiredWidth(1.0, 0.0);
            this.PositionRelativeToAdornerWidth(-1.0, -margin.Left);
            break;
          case AdornerHorizontalAlignment.OutsideRight:
            this.SizeRelativeToAdornerDesiredWidth(1.0, 0.0);
            this.PositionRelativeToContentWidth(1.0, 0.0);
            this.PositionRelativeToAdornerWidth(0.0, margin.Right);
            break;
        }
        switch (this.VerticalAlign)
        {
          case AdornerVerticalAlignment.Top:
            this.SizeRelativeToAdornerDesiredHeight(1.0, 0.0);
            this.PositionRelativeToContentHeight(0.0, margin.Top);
            break;
          case AdornerVerticalAlignment.Center:
            this.SizeRelativeToAdornerDesiredHeight(1.0, 0.0);
            this.PositionRelativeToContentHeight(0.5, 0.0);
            this.PositionRelativeToAdornerHeight(-0.5, margin.Top);
            break;
          case AdornerVerticalAlignment.Bottom:
            this.SizeRelativeToAdornerDesiredHeight(1.0, 0.0);
            this.PositionRelativeToContentHeight(1.0, -margin.Bottom);
            break;
          case AdornerVerticalAlignment.Stretch:
            this.SizeRelativeToContentHeight(1.0, -(margin.Top + margin.Bottom));
            this.PositionRelativeToContentHeight(0.0, margin.Top);
            break;
          case AdornerVerticalAlignment.OutsideTop:
            this.SizeRelativeToAdornerDesiredHeight(1.0, 0.0);
            this.PositionRelativeToAdornerHeight(-1.0, -margin.Top);
            break;
          case AdornerVerticalAlignment.OutsideBottom:
            this.SizeRelativeToAdornerDesiredHeight(1.0, 0.0);
            this.PositionRelativeToContentHeight(1.0, 0.0);
            this.PositionRelativeToAdornerHeight(0.0, margin.Bottom);
            break;
        }
      }
    }
  }
}
