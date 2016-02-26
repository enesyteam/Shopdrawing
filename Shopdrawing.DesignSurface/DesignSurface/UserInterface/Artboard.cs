// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Artboard
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Geometry.Core;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public class Artboard : Control
  {
    private bool centerDocument = true;
    private Rect? contentBounds = new Rect?();
    private List<Artboard.ExtraLayer> extraLayers = new List<Artboard.ExtraLayer>();
    private static readonly double DefaultCenterX = 0.0;
    private static readonly double DefaultCenterY = 0.0;
    private static readonly double DefaultZoom = 1.0;
    private static readonly double RectangleTolerance = 0.01;
    private static readonly double AdornerMargin = 90.0;
    private static readonly Type ThisType = typeof (Artboard);
    public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register("CenterX", typeof (double), Artboard.ThisType);
    public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register("CenterY", typeof (double), Artboard.ThisType);
    public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof (double), Artboard.ThisType);
    private PresetCollection zoomPresetCollection;
    private PresetCollection zoomComboCollection;
    private Transform transform;
    private bool isPositionLockSet;
    private Size positionLockSize;
    private Point positionLockOrigin;
    private Canvas canvas;
    private Canvas resourcesHost;
    private ArtboardBorder contentBorder;
    private AdornerService adornerService;
    private AdornerLayer adornerLayer;
    private FeedbackLayer feedbackLayer;
    private AnnotationLayer annotationLayer;
    private OverlayLayer overlayLayer;
    private Canvas liveControlLayer;
    private SnapToGridRenderer snapToGridRenderer;
    private VisualCollection childrenVisuals;
    private DesignerView designerView;
    private Canvas extraLayersCanvas;
    private bool showExtensibleAdorners;

    protected override int VisualChildrenCount
    {
      get
      {
        return this.childrenVisuals.Count;
      }
    }

    public double CenterX
    {
      get
      {
        return (double) this.GetValue(Artboard.CenterXProperty);
      }
      set
      {
        this.SetValue(Artboard.CenterXProperty, (object) value);
      }
    }

    public double CenterY
    {
      get
      {
        return (double) this.GetValue(Artboard.CenterYProperty);
      }
      set
      {
        this.SetValue(Artboard.CenterYProperty, (object) value);
      }
    }

    public double Zoom
    {
      get
      {
        return (double) this.GetValue(Artboard.ZoomProperty);
      }
      set
      {
        this.SetValue(Artboard.ZoomProperty, (object) value);
      }
    }

    public virtual Vector ViewRootToArtboardScale
    {
      get
      {
        return new Vector(1.0, 1.0);
      }
    }

    public bool CanZoomIn
    {
      get
      {
        return this.Zoom < this.zoomPresetCollection.Maximum;
      }
    }

    public bool CanZoomOut
    {
      get
      {
        return this.Zoom > this.zoomPresetCollection.Minimum;
      }
    }

    public PresetCollection ZoomPresetCollection
    {
      get
      {
        return this.zoomPresetCollection;
      }
      set
      {
        this.zoomPresetCollection = value;
        this.CoerceValue(Artboard.ZoomProperty);
      }
    }

    public PresetCollection ZoomComboCollection
    {
      get
      {
        return this.zoomComboCollection;
      }
      set
      {
        this.zoomComboCollection = value;
      }
    }

    public Rect ArtboardBounds
    {
      get
      {
        Size size = new Size(this.RenderSize.Width / this.Zoom, this.RenderSize.Height / this.Zoom);
        return new Rect(this.CenterX - size.Width / 2.0, this.CenterY - size.Height / 2.0, size.Width, size.Height);
      }
    }

    public virtual Rect DocumentBounds
    {
      get
      {
        Rect rect = Rect.Empty;
        if (this.EditableContent != null)
          rect = new Rect(new Point(0.0, 0.0), this.EditableContent.RenderSize);
        return rect;
      }
    }

    public bool ValidContentBounds
    {
      get
      {
        return this.contentBounds.HasValue;
      }
    }

    public Rect ContentBounds
    {
      get
      {
        if (!this.ValidContentBounds)
          return Rect.Empty;
        return this.contentBounds.Value;
      }
      protected set
      {
        this.contentBounds = new Rect?(value);
      }
    }

    public FrameworkElement ContentArea
    {
      get
      {
        return (FrameworkElement) this.contentBorder;
      }
    }

    public FrameworkElement EditableContent
    {
      get
      {
        return (FrameworkElement) this.contentBorder.Child;
      }
      set
      {
        if (this.contentBorder.Child == value)
          return;
        this.contentBorder.Child = (UIElement) null;
        this.overlayLayer.Children.Clear();
        if (value != null)
        {
          this.contentBorder.Child = (UIElement) value;
          this.InvalidateMeasure();
        }
        this.contentBounds = new Rect?();
      }
    }

    public virtual IViewObject EditableContentObject
    {
      get
      {
        return this.ViewObjectFactory.Instantiate((object) this.EditableContent);
      }
    }

    public FrameworkElement ResourcesHost
    {
      get
      {
        return (FrameworkElement) this.resourcesHost;
      }
    }

    public OverlayLayer OverlayLayer
    {
      get
      {
        return this.overlayLayer;
      }
    }

    public FeedbackLayer FeedbackLayer
    {
      get
      {
        return this.feedbackLayer;
      }
    }

    public AnnotationLayer AnnotationLayer
    {
      get
      {
        return this.annotationLayer;
      }
    }

    public AdornerLayer AdornerLayer
    {
      get
      {
        return this.adornerLayer;
      }
    }

    public AdornerService AdornerService
    {
      get
      {
        return this.adornerService;
      }
    }

    public Canvas LiveControlLayer
    {
      get
      {
        return this.liveControlLayer;
      }
    }

    public virtual bool IsLiveControlLayerActive
    {
      get
      {
        return false;
      }
    }

    public SnapToGridRenderer SnapToGridRenderer
    {
      get
      {
        return this.snapToGridRenderer;
      }
    }

    public DesignerView DesignerView
    {
      get
      {
        return this.designerView;
      }
    }

    protected Transform Transform
    {
      get
      {
        return this.transform;
      }
    }

    protected IViewObjectFactory ViewObjectFactory { get; private set; }

    internal bool ShowExtensibleAdorners
    {
      get
      {
        return this.showExtensibleAdorners;
      }
      set
      {
        if (this.showExtensibleAdorners == value)
          return;
        this.showExtensibleAdorners = value;
        this.UpdateExtensibleAdornersVisibility();
      }
    }

    private UIElement VisualTreeRoot
    {
      get
      {
        DependencyObject reference = (DependencyObject) this;
        UIElement uiElement1 = (UIElement) this;
        do
        {
          reference = VisualTreeHelper.GetParent(reference);
          UIElement uiElement2 = reference as UIElement;
          if (uiElement2 != null)
            uiElement1 = uiElement2;
        }
        while (reference != null);
        return uiElement1;
      }
    }

    public event EventHandler CenterChanged;

    public event EventHandler ZoomChanged;

    static Artboard()
    {
      UIElement.ClipToBoundsProperty.OverrideMetadata(Artboard.ThisType, (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
      FrameworkPropertyMetadata propertyMetadata1 = new FrameworkPropertyMetadata((object) Artboard.DefaultCenterX);
      propertyMetadata1.AffectsArrange = true;
      propertyMetadata1.PropertyChangedCallback = new PropertyChangedCallback(Artboard.CenterPropertyChanged);
      Artboard.CenterXProperty.OverrideMetadata(Artboard.ThisType, (PropertyMetadata) propertyMetadata1);
      FrameworkPropertyMetadata propertyMetadata2 = new FrameworkPropertyMetadata((object) Artboard.DefaultCenterY);
      propertyMetadata2.AffectsArrange = true;
      propertyMetadata2.PropertyChangedCallback = new PropertyChangedCallback(Artboard.CenterPropertyChanged);
      Artboard.CenterYProperty.OverrideMetadata(Artboard.ThisType, (PropertyMetadata) propertyMetadata2);
      FrameworkPropertyMetadata propertyMetadata3 = new FrameworkPropertyMetadata((object) Artboard.DefaultZoom);
      propertyMetadata3.AffectsArrange = true;
      propertyMetadata3.PropertyChangedCallback = new PropertyChangedCallback(Artboard.ZoomPropertyChanged);
      propertyMetadata3.CoerceValueCallback = new CoerceValueCallback(Artboard.ZoomPropertyCoerceValue);
      Artboard.ZoomProperty.OverrideMetadata(Artboard.ThisType, (PropertyMetadata) propertyMetadata3);
    }

    public Artboard(IViewObjectFactory viewObjectFactory, ViewExceptionCallback viewExceptionCallback)
    {
      this.ViewObjectFactory = viewObjectFactory;
      this.contentBorder = new ArtboardBorder(viewExceptionCallback);
      this.contentBorder.BorderBrush = (Brush) Brushes.Black;
      this.contentBorder.MinWidth = 20.0;
      this.contentBorder.MinHeight = 20.0;
      this.contentBorder.IsHitTestVisible = false;
      TextOptions.SetTextFormattingMode((DependencyObject) this.contentBorder, TextFormattingMode.Ideal);
      this.snapToGridRenderer = new SnapToGridRenderer(this);
      this.snapToGridRenderer.IsHitTestVisible = false;
      this.overlayLayer = new OverlayLayer();
      this.overlayLayer.IsHitTestVisible = false;
      this.adornerLayer = new AdornerLayer();
      this.adornerLayer.IsHitTestVisible = false;
      this.adornerLayer.IsVisibleChanged += (DependencyPropertyChangedEventHandler) ((s, e) => this.UpdateExtensibleAdornersVisibility());
      this.feedbackLayer = new FeedbackLayer();
      this.feedbackLayer.IsHitTestVisible = false;
      this.annotationLayer = new AnnotationLayer();
      this.annotationLayer.IsHitTestVisible = true;
      this.extraLayersCanvas = new Canvas();
      this.extraLayersCanvas.IsHitTestVisible = false;
      this.liveControlLayer = new Canvas();
      this.liveControlLayer.Resources.Add((object) typeof (TextBox), (object) new Style(typeof (TextBox), new TextBox().Style));
      this.liveControlLayer.Resources.Add((object) typeof (RichTextBox), (object) new Style(typeof (RichTextBox), new RichTextBox().Style));
      this.resourcesHost = new Canvas();
      this.resourcesHost.IsHitTestVisible = false;
      this.resourcesHost.Children.Add((UIElement) this.contentBorder);
      this.resourcesHost.Children.Add((UIElement) this.overlayLayer);
      this.designerView = (DesignerView) new Artboard.BlendDesignerView();
      this.designerView.Child = (UIElement) this.resourcesHost;
      this.canvas = new Canvas();
      this.canvas.Children.Add((UIElement) this.designerView);
      this.canvas.Children.Add((UIElement) this.liveControlLayer);
      this.canvas.Children.Add((UIElement) this.extraLayersCanvas);
      this.canvas.Children.Add((UIElement) this.snapToGridRenderer);
      this.canvas.Children.Add((UIElement) this.adornerLayer);
      this.canvas.Children.Add((UIElement) this.feedbackLayer);
      this.canvas.Children.Add((UIElement) this.annotationLayer);
      this.childrenVisuals = new VisualCollection((Visual) this);
      this.childrenVisuals.Add((Visual) this.canvas);
      this.adornerService = new AdornerService(this);
      this.SetValue(TabControl.TabStripPlacementProperty, (object) Dock.Top);
      this.InheritanceBehavior = InheritanceBehavior.SkipToThemeNow;
      this.AllowDrop = true;
      this.contentBorder.AllowDrop = false;
      AccessKeyManager.AddAccessKeyPressedHandler((DependencyObject) this.contentBorder, new AccessKeyPressedEventHandler(this.Content_AccessKeyPressed));
      AccessKeyManager.AddAccessKeyPressedHandler((DependencyObject) this.overlayLayer, new AccessKeyPressedEventHandler(this.Content_AccessKeyPressed));
      this.contentBorder.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Content_PreviewGotKeyboardFocus);
      this.overlayLayer.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Content_PreviewGotKeyboardFocus);
      this.SetTextDefaults();
    }

    internal void TearDown()
    {
      this.contentBorder.Resources = (ResourceDictionary) null;
      this.contentBorder.Child = (UIElement) null;
      this.contentBorder = (ArtboardBorder) null;
      this.resourcesHost = (Canvas) null;
      this.childrenVisuals.Clear();
      if (this.designerView == null)
        return;
      this.designerView.Adorners.Clear();
      this.designerView.Context = (EditingContext) null;
      this.designerView = (DesignerView) null;
    }

    protected override Visual GetVisualChild(int index)
    {
      return this.childrenVisuals[index];
    }

    public void ClearContentBounds()
    {
      this.contentBounds = new Rect?();
    }

    public virtual void AddLiveControl(IViewControl control)
    {
      this.liveControlLayer.Children.Add((UIElement) control.PlatformSpecificObject);
    }

    public virtual void RemoveLiveControl(IViewControl control)
    {
      this.liveControlLayer.Children.Remove((UIElement) control.PlatformSpecificObject);
    }

    public virtual void FocusLiveControlLayer()
    {
    }

    public void CenterAll()
    {
      this.EnsureValidContentBounds();
      Rect documentBounds = this.DocumentBounds;
      documentBounds.Union(this.ContentBounds);
      this.CenterRectangle(documentBounds);
    }

    public void CenterElementList(IEnumerable<SceneElement> elementList)
    {
      if (this.EditableContentObject == null)
        return;
      this.CenterRectangle(this.GetElementListBounds(elementList));
    }

    public void CenterRectangle(Rect rectangle)
    {
      if (!this.IsRectValid(rectangle))
        return;
      this.CenterX = (rectangle.Left + rectangle.Right) / 2.0;
      this.CenterY = (rectangle.Top + rectangle.Bottom) / 2.0;
    }

    public void ZoomAroundFixedPoint(bool isZoomingIn, Point fixedPoint)
    {
      double zoom1 = this.Zoom;
      Point point1 = new Point(this.CenterX, this.CenterY);
      if (isZoomingIn)
        this.ZoomIn();
      else
        this.ZoomOut();
      double zoom2 = this.Zoom;
      Point point2 = fixedPoint + zoom1 / zoom2 * (point1 - fixedPoint);
      this.CenterX = point2.X;
      this.CenterY = point2.Y;
    }

    public void ZoomIn()
    {
      if (this.zoomPresetCollection == null)
        return;
      this.Zoom = this.zoomPresetCollection.NextLargerPreset(this.Zoom);
    }

    public void ZoomOut()
    {
      if (this.zoomPresetCollection == null)
        return;
      this.Zoom = this.zoomPresetCollection.NextSmallerPreset(this.Zoom);
    }

    public void ZoomToFitAll()
    {
      this.EnsureValidContentBounds();
      Rect documentBounds = this.DocumentBounds;
      documentBounds.Union(this.ContentBounds);
      if (this.AnnotationLayer.IsVisible)
      {
        foreach (UIElement uiElement in this.annotationLayer.Children)
          documentBounds.Union((Point) VisualTreeHelper.GetOffset((Visual) uiElement));
      }
      foreach (Artboard.ExtraLayer extraLayer in this.extraLayers)
      {
        if (ArtboardExtraLayerOptionsExtensions.IsSet(extraLayer.Options, ArtboardExtraLayerOptions.IncludeBoundsInZoomToFitAll) && extraLayer.Layer.IsVisible)
        {
          Rect descendantBounds = VisualTreeHelper.GetDescendantBounds((Visual) extraLayer.Layer);
          if (!descendantBounds.IsEmpty && ArtboardExtraLayerOptionsExtensions.IsSet(extraLayer.Options, ArtboardExtraLayerOptions.OriginIsDocumentCenter))
            descendantBounds.Offset(this.DocumentBounds.Width / 2.0, this.DocumentBounds.Height / 2.0);
          documentBounds.Union(descendantBounds);
        }
      }
      this.ZoomToFitRectangleAndAdorners(documentBounds);
    }

    public void ZoomToFitElementList(IEnumerable<SceneElement> elementList)
    {
      if (this.EditableContentObject == null)
        return;
      this.ZoomToFitRectangleAndAdorners(this.GetElementListBounds(elementList));
    }

    public void ZoomToFitRectangle(Rect rectangle)
    {
      Size renderSize = this.RenderSize;
      this.ZoomToFitRectangleCore(rectangle, renderSize);
    }

    public void ZoomToFitRectangleAndAdorners(Rect rectangle)
    {
      Size artboardSize = new Size(Math.Max(this.RenderSize.Width - Artboard.AdornerMargin, 0.0), Math.Max(this.RenderSize.Height - Artboard.AdornerMargin, 0.0));
      this.ZoomToFitRectangleCore(rectangle, artboardSize);
    }

    private void ZoomToFitRectangleCore(Rect rectangle, Size artboardSize)
    {
      Vector rootToArtboardScale = this.ViewRootToArtboardScale;
      artboardSize.Width /= rootToArtboardScale.X;
      artboardSize.Height /= rootToArtboardScale.Y;
      if (!this.IsRectValid(rectangle))
        return;
      if (rectangle.Width < Artboard.RectangleTolerance)
        rectangle.Width = Artboard.RectangleTolerance;
      if (rectangle.Height < Artboard.RectangleTolerance)
        rectangle.Height = Artboard.RectangleTolerance;
      if (artboardSize.Width == 0.0 || artboardSize.Height == 0.0)
        return;
      double num = artboardSize.Width / artboardSize.Height;
      this.Zoom = rectangle.Width / rectangle.Height <= num ? artboardSize.Height / rectangle.Height : artboardSize.Width / rectangle.Width;
      this.CenterX = (rectangle.Left + rectangle.Right) / 2.0;
      this.CenterY = (rectangle.Top + rectangle.Bottom) / 2.0;
    }

    public void ResetZoom()
    {
      this.Zoom = Artboard.DefaultZoom;
    }

    public Transform CalculateTransformFromContentToArtboard()
    {
      return this.ContentArea.TransformToAncestor((Visual) this.canvas) as Transform;
    }

    public Transform CalculateTransformFromArtboardToContent()
    {
      return this.CalculateTransformFromContentToArtboard().Inverse as Transform;
    }

    public Point TransformFromContentToArtboard(Point point)
    {
      if (this.transform != null)
        return this.transform.Transform(point);
      return point;
    }

    public void SafelyRemoveEditableContent()
    {
      this.contentBorder.SafelyRemoveChild();
      this.overlayLayer.SafelyRemoveChildren();
      this.contentBounds = new Rect?();
    }

    public virtual bool IsInArtboard(IViewVisual visual)
    {
      Visual visual1 = visual.PlatformSpecificObject as Visual;
      if (visual1 != null)
        return this.IsAncestorOf((DependencyObject) visual1);
      return false;
    }

    public void AddExtraLayer(UIElement layer, ArtboardExtraLayerOptions options)
    {
      this.extraLayers.Add(new Artboard.ExtraLayer()
      {
        Layer = layer,
        Options = options
      });
      this.extraLayersCanvas.Children.Add(layer);
      this.UpdateChildren();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      return new Size(0.0, 0.0);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Size size = base.ArrangeOverride(finalSize);
      this.EnsureValidContentBounds();
      if (this.centerDocument && this.EditableContentObject != null)
      {
        IViewVisual viewVisual = this.EditableContentObject as IViewVisual;
        if (viewVisual != null)
        {
          Size desiredSize = viewVisual.DesiredSize;
          this.CenterX = desiredSize.Width / 2.0;
          this.CenterY = desiredSize.Height / 2.0;
          this.centerDocument = false;
        }
      }
      return size;
    }

    protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
    {
      if (new Rect(this.RenderSize).Contains(hitTestParams.HitPoint))
        return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParams.HitPoint);
      return (HitTestResult) null;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo info)
    {
      Point point = this.canvas.TranslatePoint(new Point(), (UIElement) Application.Current.MainWindow);
      if (!this.isPositionLockSet)
      {
        this.isPositionLockSet = true;
        this.positionLockSize = this.canvas.RenderSize;
        this.positionLockOrigin = point;
      }
      else if (this.canvas.RenderSize != this.positionLockSize || point != this.positionLockOrigin)
      {
        this.CenterX = this.CenterX - (this.positionLockSize.Width / 2.0 - this.canvas.RenderSize.Width / 2.0 + (this.positionLockOrigin.X - point.X)) / this.Zoom;
        this.CenterY = this.CenterY - (this.positionLockSize.Height / 2.0 - this.canvas.RenderSize.Height / 2.0 + (this.positionLockOrigin.Y - point.Y)) / this.Zoom;
        this.positionLockSize = this.canvas.RenderSize;
        this.positionLockOrigin = point;
      }
      this.UpdateChildren();
      base.OnRenderSizeChanged(info);
    }

    private void UpdateExtensibleAdornersVisibility()
    {
      this.DesignerView.AdornersVisible = this.ShowExtensibleAdorners && this.AdornerLayer.Visibility == Visibility.Visible;
    }

    private bool IsRectValid(Rect rect)
    {
      if (!double.IsNaN(rect.X) && !double.IsNaN(rect.Y) && rect.Width >= 0.0)
        return rect.Height >= 0.0;
      return false;
    }

    protected virtual void EnsureValidContentBounds()
    {
      if (this.ValidContentBounds)
        return;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetArtboardContentBounds);
      if (this.EditableContent != null)
      {
        this.contentBounds = new Rect?(VisualTreeHelper.GetDescendantBounds((Visual) this.EditableContent));
        if (double.IsInfinity(this.contentBounds.Value.Left) || double.IsInfinity(this.contentBounds.Value.Top) || (double.IsInfinity(this.contentBounds.Value.Width) || double.IsInfinity(this.contentBounds.Value.Height)))
          this.contentBounds = new Rect?(new Rect(this.EditableContent.RenderSize));
      }
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetArtboardContentBounds);
    }

    private void SetTextDefaults()
    {
      this.SetDefaultValue(TextElement.ForegroundProperty);
      this.SetDefaultValue(TextElement.FontFamilyProperty);
      this.SetDefaultValue(TextElement.FontSizeProperty);
    }

    private void SetDefaultValue(DependencyProperty property)
    {
      this.SetValue(property, property.DefaultMetadata.DefaultValue);
    }

    private static void CenterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      Artboard artboard = (Artboard) dependencyObject;
      artboard.UpdateChildren();
      if (artboard.CenterChanged == null)
        return;
      artboard.CenterChanged((object) artboard, EventArgs.Empty);
    }

    private static void ZoomPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.AdjustZoom);
      Artboard artboard = (Artboard) dependencyObject;
      artboard.UpdateChildren();
      if (artboard.ZoomChanged == null)
        return;
      artboard.ZoomChanged((object) artboard, EventArgs.Empty);
    }

    private static object ZoomPropertyCoerceValue(DependencyObject dependencyObject, object baseValue)
    {
      Artboard artboard = (Artboard) dependencyObject;
      double val1 = (double) baseValue;
      if (artboard.zoomPresetCollection != null)
        val1 = Math.Max(artboard.zoomPresetCollection.Minimum, Math.Min(val1, artboard.zoomPresetCollection.Maximum));
      return (object) val1;
    }

    public void ResetPositionLock()
    {
      this.isPositionLockSet = false;
    }

    public virtual void UpdateImageHostLocation()
    {
    }

    protected virtual void UpdateChildren()
    {
      if (this.designerView == null)
        return;
      double zoom = this.Zoom;
      Vector rootToArtboardScale = this.ViewRootToArtboardScale;
      rootToArtboardScale.X *= zoom;
      rootToArtboardScale.Y *= zoom;
      this.designerView.ZoomLevel = this.Zoom;
      double centerX = this.CenterX;
      double centerY = this.CenterY;
      TransformGroup transformGroup1 = new TransformGroup();
      transformGroup1.Children.Add((Transform) new ScaleTransform(rootToArtboardScale.X, rootToArtboardScale.Y, centerX, centerY));
      transformGroup1.Children.Add((Transform) new TranslateTransform(this.canvas.RenderSize.Width / 2.0 - centerX, this.canvas.RenderSize.Height / 2.0 - centerY));
      Point point = this.TranslatePoint(new Point(1.0 / rootToArtboardScale.X, 1.0 / rootToArtboardScale.Y) * transformGroup1.Value, this.VisualTreeRoot);
      point.X = Math.Round(point.X) - point.X;
      point.Y = Math.Round(point.Y) - point.Y;
      transformGroup1.Children.Add((Transform) new TranslateTransform(point.X, point.Y));
      transformGroup1.Freeze();
      this.designerView.RenderTransform = (Transform) transformGroup1;
      this.feedbackLayer.RenderTransform = (Transform) transformGroup1;
      this.liveControlLayer.RenderTransform = (Transform) transformGroup1;
      this.annotationLayer.RenderTransform = (Transform) transformGroup1;
      foreach (Artboard.ExtraLayer extraLayer in this.extraLayers)
      {
        TransformGroup transformGroup2 = transformGroup1;
        if (ArtboardExtraLayerOptionsExtensions.IsSet(extraLayer.Options, ArtboardExtraLayerOptions.OriginIsDocumentCenter))
        {
          transformGroup2 = new TransformGroup();
          transformGroup2.Children.Add((Transform) new TranslateTransform(this.DocumentBounds.Width / 2.0, this.DocumentBounds.Height / 2.0));
          transformGroup2.Children.Add((Transform) transformGroup1);
          transformGroup2.Freeze();
        }
        extraLayer.Layer.RenderTransform = (Transform) transformGroup2;
      }
      this.annotationLayer.ChildRenderTransform = (Transform) new ScaleTransform(1.0 / zoom, 1.0 / zoom, 0.0, 0.0);
      this.contentBorder.BorderThickness = new Thickness(1.0 / rootToArtboardScale.X);
      this.transform = (Transform) transformGroup1;
      this.snapToGridRenderer.InvalidateVisual();
    }

    public virtual Rect GetElementBounds(SceneElement sceneElement)
    {
      if (sceneElement == null || sceneElement.ViewTargetElement == null)
        return Rect.Empty;
      Rect actualBounds = sceneElement.ViewModel.DefaultView.GetActualBounds(sceneElement.ViewTargetElement);
      Matrix computedTransformToRoot = sceneElement.ViewModel.DefaultView.GetComputedTransformToRoot(sceneElement);
      actualBounds.Transform(computedTransformToRoot);
      return actualBounds;
    }

    internal Rect GetElementListBounds(IEnumerable<SceneElement> elementList)
    {
      Rect empty = Rect.Empty;
      if (elementList != null)
      {
        foreach (SceneElement sceneElement in elementList)
        {
          Rect elementBounds = this.GetElementBounds(sceneElement);
          empty.Union(elementBounds);
        }
      }
      return empty;
    }

    private void Content_AccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
    {
      e.Scope = sender;
      e.Handled = true;
    }

    private void Content_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      e.Handled = true;
    }

    public Rect MakeVisible(Visual visual)
    {
      return this.MakeVisible(visual, VisualTreeHelper.GetContentBounds(visual));
    }

    public Rect MakeVisible(Visual visual, Rect rect)
    {
      if (!visual.IsDescendantOf((DependencyObject) this))
        return rect;
      Matrix matrixFromTransform = VectorUtilities.GetMatrixFromTransform(visual.TransformToVisual((Visual) this));
      Rect rect1 = rect;
      rect1.Transform(matrixFromTransform);
      double num1 = rect1.Right - this.RenderSize.Width;
      if (rect1.Left < 0.0 && num1 <= 0.0)
      {
        this.CenterX += rect1.Left / this.Zoom;
        rect1.X = 0.0;
      }
      else if (rect1.Left >= 0.0 && num1 > 0.0)
      {
        this.CenterX += num1 / this.Zoom;
        rect1.X -= num1;
      }
      double num2 = rect1.Bottom - this.RenderSize.Height;
      if (rect1.Top < 0.0 && num2 <= 0.0)
      {
        this.CenterY += rect1.Top / this.Zoom;
        rect1.Y = 0.0;
      }
      else if (rect1.Top >= 0.0 && num2 > 0.0)
      {
        this.CenterY += num2 / this.Zoom;
        rect1.Y -= num2;
      }
      return rect1;
    }

    private class BlendDesignerView : DesignerView
    {
      public BlendDesignerView()
      {
        this.Focusable = false;
        this.AddHandler(Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnMouseDown), true);
      }

      protected override void OnPreviewDragOver(DragEventArgs e)
      {
        e.Handled = true;
        base.OnPreviewDragOver(e);
      }

      private void OnMouseDown(object sender, MouseButtonEventArgs e)
      {
        if (!(e.GetPosition((IInputElement) this) == new Point(0.0, 0.0)))
          return;
        e.Handled = false;
      }

      protected override AutomationPeer OnCreateAutomationPeer()
      {
        return (AutomationPeer) null;
      }

      protected override void SetZoomTransform()
      {
      }
    }

    private class ExtraLayer
    {
      public UIElement Layer { get; set; }

      public ArtboardExtraLayerOptions Options { get; set; }
    }
  }
}
