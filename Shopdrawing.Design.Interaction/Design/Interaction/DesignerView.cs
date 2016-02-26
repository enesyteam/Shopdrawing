// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.DesignerView
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using MS.Internal;
using MS.Internal.Automation;
using MS.Internal.Interaction;
using MS.Internal.Properties;
using MS.Internal.Transforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Microsoft.Windows.Design.Interaction
{
  public class DesignerView : Decorator
  {
    private Point _lastPointInParentCoordinates = new Point();
    private static readonly DependencyPropertyKey DesignerViewPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DesignerView", typeof (DesignerView), typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
    public static readonly DependencyProperty DesignerViewProperty = DesignerView.DesignerViewPropertyKey.DependencyProperty;
    public static readonly DependencyProperty AdornersVisibleProperty = DependencyProperty.Register("AdornersVisible", typeof (bool), typeof (DesignerView), new PropertyMetadata((object) true, new PropertyChangedCallback(DesignerView.OnAdornersVisibleChanged)));
    public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel", typeof (double), typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(DesignerView.HandleZoomLevelChanged)), new ValidateValueCallback(DesignerView.IsZoomLevelValid));
    private static TraceSwitch _disableCatchAll = new TraceSwitch("DisableCatchAll", "Cider Switches");
    private int _doubleClickSizeX;
    private int _doubleClickSizeY;
    private int _hoverSizeX;
    private int _hoverSizeY;
    private long _hoverTimeout;
    private double _minimumHorizontalDragDistance;
    private double _minimumVerticalDragDistance;
    private EditingContext _context;
    private AdornerLayer _adornerLayer;
    private DesignerView.OpaqueElement _hitTestLayer;
    private ViewService _viewService;
    private ModelService _modelService;
    private bool _inCall;
    private bool _pendingClearCapture;
    private bool _clearingCapture;
    private bool _abortCapture;
    private EventArgs _secondaryHitTestArgs;
    private Point _clickPoint;
    private int _clickCount;
    private ModelItem _clickModel;
    private DependencyObject _clickAdorner;
    private bool _capturingMouse;
    private UIElement _captureOwner;
    private DependencyObject _captureAdorner;
    private ModelItem _captureModel;
    private Point _capturePointInRootElementCoords;
    private DependencyObject _hitRawAdornerVisual;
    private ViewItem _hitRawModelVisual;
    private DependencyObject _hitAdorner;
    private ModelItem _hitModel;
    private ViewHitTestFilterCallback _modelHitTestFilter;
    private DesignerView.MouseState[] _mouseState;
    private DragDropEffects _dragAllowedEffects;
    private Point _lastCurrentPoint;
    private IDataObject _dragData;
    private DispatcherTimer _hoverTimer;
    private Point _hoverPoint;
    private DesignerViewAutomationPeer _designerViewAutomationPeer;
    private Task _focusedTask;
    private static Style _defaultFocusVisualStyle;
    private bool _protectReEntrantCode;

    private new static Style DefaultFocusVisualStyle
    {
      get
      {
        if (DesignerView._defaultFocusVisualStyle == null)
        {
          DesignerView._defaultFocusVisualStyle = new Style();
          DesignerView._defaultFocusVisualStyle.Seal();
        }
        return DesignerView._defaultFocusVisualStyle;
      }
    }

    public ICollection<UIElement> Adorners
    {
      get
      {
        return this._adornerLayer.Adorners;
      }
    }

    public EditingContext Context
    {
      get
      {
        return this._context;
      }
      set
      {
        if (value == this._context)
          return;
        if (value != null && value.Items.Contains<CurrentDesignerView>() && value.Items.GetValue<CurrentDesignerView>().View != null)
            throw new InvalidOperationException(MS.Internal.Properties.Resources.Error_ContextHasView);
        if (this._context != null)
        {
          this._context.Items.SetValue((ContextItem) new CurrentDesignerView((DesignerView) null));
          this._context.Items.Unsubscribe<FocusedTask>(new SubscribeContextCallback<FocusedTask>(this.OnFocusedTaskChanged));
          this._context.Items.Unsubscribe<Tool>(new SubscribeContextCallback<Tool>(this.OnToolChanged));
          this._context.Disposing -= new EventHandler(this.OnContextDisposing);
        }
        this.ClearAllHitTestInfo();
        this._context = value;
        this._viewService = (ViewService) null;
        this._modelService = (ModelService) null;
        this._focusedTask = (Task) null;
        if (this._context != null)
        {
          this._context.Items.SetValue((ContextItem) new CurrentDesignerView(this));
          this._context.Items.Subscribe<FocusedTask>(new SubscribeContextCallback<FocusedTask>(this.OnFocusedTaskChanged));
          this._context.Items.Subscribe<Tool>(new SubscribeContextCallback<Tool>(this.OnToolChanged));
          this._context.Disposing += new EventHandler(this.OnContextDisposing);
        }
        if (this._designerViewAutomationPeer != null)
          this._designerViewAutomationPeer.Context = this._context;
        this._adornerLayer.SetContext(this._context);
      }
    }

    public bool IsContentHitTestVisible
    {
      get
      {
        return !this._hitTestLayer.IsHitTestVisible;
      }
      set
      {
        this._hitTestLayer.IsHitTestVisible = !value;
        if (value)
        {
          KeyboardNavigation.SetControlTabNavigation((DependencyObject) this, KeyboardNavigationMode.Continue);
          KeyboardNavigation.SetDirectionalNavigation((DependencyObject) this, KeyboardNavigationMode.Continue);
          KeyboardNavigation.SetTabNavigation((DependencyObject) this, KeyboardNavigationMode.Continue);
        }
        else
        {
          this.ClearValue(KeyboardNavigation.ControlTabNavigationProperty);
          this.ClearValue(KeyboardNavigation.DirectionalNavigationProperty);
          this.ClearValue(KeyboardNavigation.TabNavigationProperty);
        }
      }
    }

    public ViewItem RootView
    {
      get
      {
        if (this.ModelService != null && this.ModelService.Root != null)
          return this.ModelService.Root.View;
        return (ViewItem) null;
      }
    }

    public bool AdornersVisible
    {
      get
      {
        return (bool) this.GetValue(DesignerView.AdornersVisibleProperty);
      }
      set
      {
        this.SetValue(DesignerView.AdornersVisibleProperty, (object) (bool) (value ? true : false));
      }
    }

    private ViewService ViewService
    {
      get
      {
        if (this._viewService == null && this._context != null)
          this._viewService = this._context.Services.GetService<ViewService>();
        return this._viewService;
      }
    }

    private double MinHorizontalDragDistance
    {
      get
      {
        return this._minimumHorizontalDragDistance * DesignerUtilities.GetZoomRounding(this).X;
      }
    }

    private double MinVerticalDragDistance
    {
      get
      {
        return this._minimumVerticalDragDistance * DesignerUtilities.GetZoomRounding(this).Y;
      }
    }

    private ModelService ModelService
    {
      get
      {
        if (this._modelService == null && this._context != null)
          this._modelService = this._context.Services.GetService<ModelService>();
        return this._modelService;
      }
    }

    public override UIElement Child
    {
      get
      {
        return base.Child;
      }
      set
      {
        if (base.Child == value)
          return;
        if (value == null)
        {
          base.Child = (UIElement) null;
          this.RemoveVisualChild((Visual) this._hitTestLayer);
          this.RemoveVisualChild((Visual) this._adornerLayer);
          this.RemoveLogicalChild((object) this._adornerLayer);
        }
        else
        {
          base.Child = value;
          this.AddLogicalChild((object) this._adornerLayer);
          this.AddVisualChild((Visual) this._hitTestLayer);
          this.AddVisualChild((Visual) this._adornerLayer);
        }
      }
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return base.Child == null ? 0 : 3;
      }
    }

    public double ZoomLevel
    {
      get
      {
        return (double) this.GetValue(DesignerView.ZoomLevelProperty);
      }
      set
      {
        this.SetValue(DesignerView.ZoomLevelProperty, (object) value);
      }
    }

    public event EventHandler<CommandExceptionEventArgs> CommandException;

    public event EventHandler<MatchGestureEventArgs> MatchGesture;

    public event EventHandler ZoomLevelChanged;

    static DesignerView()
    {
      KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.None));
      KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.None));
      KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.None));
      UIElement.FocusableProperty.OverrideMetadata(typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
      FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof (DesignerView), (PropertyMetadata) new FrameworkPropertyMetadata((object) DesignerView.DefaultFocusVisualStyle));
    }

    public DesignerView()
    {
      this._mouseState = new DesignerView.MouseState[5];
      this._adornerLayer = new AdornerLayer();
      this._hitTestLayer = new DesignerView.OpaqueElement();
      KeyboardNavigation.SetIsTabStop((DependencyObject) this._hitTestLayer, false);
      KeyboardNavigation.SetIsTabStop((DependencyObject) this._adornerLayer, false);
      KeyboardNavigation.SetControlTabNavigation((DependencyObject) this._adornerLayer, KeyboardNavigationMode.Contained);
      KeyboardNavigation.SetDirectionalNavigation((DependencyObject) this._adornerLayer, KeyboardNavigationMode.Contained);
      KeyboardNavigation.SetTabNavigation((DependencyObject) this._adornerLayer, KeyboardNavigationMode.Contained);
      this._doubleClickSizeX = SafeNativeMethods.GetSystemMetrics(36);
      this._doubleClickSizeY = SafeNativeMethods.GetSystemMetrics(37);
      SafeNativeMethods.SystemParametersInfo(98, 0, out this._hoverSizeX, 0);
      SafeNativeMethods.SystemParametersInfo(100, 0, out this._hoverSizeY, 0);
      this._minimumHorizontalDragDistance = SystemParameters.MinimumHorizontalDragDistance;
      this._minimumVerticalDragDistance = SystemParameters.MinimumVerticalDragDistance;
      int num;
      SafeNativeMethods.SystemParametersInfo(102, 0, out num, 0);
      this._hoverTimeout = (long) (num * 10000);
      this._hitTestLayer.AllowDrop = true;
      this._adornerLayer.AllowDrop = true;
      DesignerView.AddHandler((UIElement) this, DragDrop.GiveFeedbackEvent, (Delegate) new GiveFeedbackEventHandler(this.OnDecoratorGiveFeedback));
      DesignerView.AddHandler((UIElement) this, Keyboard.KeyDownEvent, (Delegate) new KeyEventHandler(this.OnDecoratorKeyDown));
      DesignerView.AddHandler((UIElement) this, CommandManager.CanExecuteEvent, (Delegate) new CanExecuteRoutedEventHandler(this.OnCanExecuteCommand));
      DesignerView.AddHandler((UIElement) this, CommandManager.ExecutedEvent, (Delegate) new ExecutedRoutedEventHandler(this.OnExecutedCommand));
      DesignerView.AddHandler((UIElement) this._hitTestLayer, DragDrop.DragEnterEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragEnter));
      DesignerView.AddHandler((UIElement) this._hitTestLayer, DragDrop.DragLeaveEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragLeave));
      DesignerView.AddHandler((UIElement) this._hitTestLayer, DragDrop.DragOverEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragOver));
      DesignerView.AddHandler((UIElement) this._hitTestLayer, DragDrop.DropEvent, (Delegate) new DragEventHandler(this.OnDecoratorDrop));
      DesignerView.AddHandler((UIElement) this._adornerLayer, DragDrop.DragEnterEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragEnter));
      DesignerView.AddHandler((UIElement) this._adornerLayer, DragDrop.DragLeaveEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragLeave));
      DesignerView.AddHandler((UIElement) this._adornerLayer, DragDrop.DragOverEvent, (Delegate) new DragEventHandler(this.OnDecoratorDragOver));
      DesignerView.AddHandler((UIElement) this._adornerLayer, DragDrop.DropEvent, (Delegate) new DragEventHandler(this.OnDecoratorDrop));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseDownEvent, (Delegate) new MouseButtonEventHandler(this.OnMouseDown));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseMoveEvent, (Delegate) new MouseEventHandler(this.OnMouseMove));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.OnMouseUp));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseWheelEvent, (Delegate) new MouseWheelEventHandler(this.OnMouseWheel));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseEnterEvent, (Delegate) new MouseEventHandler(this.OnMouseEnter));
      DesignerView.AddHandler((UIElement) this, Mouse.MouseLeaveEvent, (Delegate) new MouseEventHandler(this.OnMouseLeave));
      DesignerView.AddHandler((UIElement) this, Mouse.QueryCursorEvent, (Delegate) new QueryCursorEventHandler(this.OnHitTestQueryCursor));
      DesignerView.AddHandler((UIElement) this, Mouse.LostMouseCaptureEvent, (Delegate) new MouseEventHandler(this.OnLostMouseCapture));
      this.SetValue(DesignerView.DesignerViewPropertyKey, (object) this);
    }

    public static DesignerView FromContext(EditingContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");
      return context.Items.GetValue<CurrentDesignerView>().View;
    }

    public static DesignerView GetDesignerView(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      return element.GetValue(DesignerView.DesignerViewProperty) as DesignerView;
    }

    internal static Transform GetScaleTransform(Visual visual)
    {
      DependencyObject reference = (DependencyObject) visual;
      DesignerView designerView;
      do
      {
        designerView = reference as DesignerView;
        reference = VisualTreeHelper.GetParent(reference);
      }
      while (designerView == null && reference != null);
      Transform transform = designerView == null ? (Transform) null : TransformUtil.GetTransformToImmediateParent((DependencyObject) designerView);
      if (transform == null)
        return Transform.Identity;
      Matrix m = transform.Value;
      m = new Matrix(m.M11, 0.0, 0.0, m.M22, 0.0, 0.0);
      return (Transform) new MatrixTransform(TransformUtil.SafeInvert(m));
    }

    private static void AddHandler(UIElement element, RoutedEvent id, Delegate handler)
    {
      element.AddHandler(id, handler, true);
    }

    private void OnFocusedTaskChanged(FocusedTask focusedTask)
    {
      this._focusedTask = focusedTask.Task;
      Mouse.UpdateCursor();
    }

    private void OnContextDisposing(object sender, EventArgs e)
    {
      if (this._inCall)
          throw new InvalidOperationException(MS.Internal.Properties.Resources.Error_DisposingDuringCall);
      this.Context = (EditingContext) null;
    }

    private void OnToolChanged(Tool currentTool)
    {
      this._clickCount = 0;
      for (int index = 0; index < this._mouseState.Length; ++index)
        this._mouseState[index] = new DesignerView.MouseState();
      Mouse.UpdateCursor();
    }

    private void AbortCapture()
    {
      if (this._captureOwner == null)
        return;
      this._abortCapture = true;
      this.ClearCapture();
    }

    private bool BeginCall()
    {
      bool flag = this._inCall;
      this._inCall = true;
      return flag;
    }

    private void ClearCapture()
    {
      if (this._inCall)
      {
        this._pendingClearCapture = true;
      }
      else
      {
        try
        {
          if (!this._abortCapture)
            return;
          this._abortCapture = false;
          if (this._focusedTask == null)
            return;
          this._focusedTask.Revert();
        }
        finally
        {
          try
          {
            this._clearingCapture = true;
            if (this._captureOwner == Mouse.Captured)
              this._captureOwner.ReleaseMouseCapture();
          }
          finally
          {
            this._clearingCapture = false;
            this._pendingClearCapture = false;
            this._captureOwner = (UIElement) null;
            this._captureModel = (ModelItem) null;
            this._captureAdorner = (DependencyObject) null;
            this._capturePointInRootElementCoords = new Point();
            for (int index = 0; index < this._mouseState.Length; ++index)
              this._mouseState[index] = new DesignerView.MouseState();
          }
        }
      }
    }

    private void ClearAdornerHitTestInfo()
    {
      this._hitRawAdornerVisual = (DependencyObject) null;
      this._hitAdorner = (DependencyObject) null;
    }

    private void ClearAllHitTestInfo()
    {
      this.ClearAdornerHitTestInfo();
      this.ClearModelHitTestInfo();
    }

    private void ClearModelHitTestInfo()
    {
      this._hitRawModelVisual = (ViewItem) null;
      this._hitModel = (ModelItem) null;
    }

    private void EndCall(bool nestedCall)
    {
      this._inCall = nestedCall;
      if (this._inCall || !this._pendingClearCapture)
        return;
      this.ClearCapture();
    }

    private HitTestFilterBehavior ModelHitTestFilter(ViewItem hit)
    {
      HitTestFilterBehavior testFilterBehavior = HitTestFilterBehavior.Continue;
      Task task = this._focusedTask;
      ViewService viewService;
      if (task.ModelFilter != null && (viewService = this.ViewService) != null)
      {
        ModelItem potentialHitTestTarget = (ModelItem) null;
        while (hit != (ViewItem) null && (potentialHitTestTarget = viewService.GetModel(hit)) == null)
          hit = hit.VisualParent;
        if (potentialHitTestTarget != null)
          testFilterBehavior = task.ModelFilter(potentialHitTestTarget);
      }
      return testFilterBehavior;
    }

    private void PerformHitTest(EventArgs eventArgs)
    {
      this._secondaryHitTestArgs = (EventArgs) null;
      Point mousePosition = this.GetMousePosition(eventArgs, (IInputElement) this._adornerLayer);
      if (DesignerUtilities.FindPopupRoot(Keyboard.FocusedElement as DependencyObject) != null)
        this.ClearAllHitTestInfo();
      else if (mousePosition.X < 0.0 || mousePosition.Y < 0.0)
      {
        this.ClearAllHitTestInfo();
      }
      else
      {
        Size renderSize = this._adornerLayer.RenderSize;
        if (mousePosition.X > renderSize.Width || mousePosition.Y > renderSize.Height)
        {
          this.ClearAllHitTestInfo();
        }
        else
        {
          this.PerformAdornerHitTest(mousePosition);
          this.PerformModelHitTest(eventArgs);
          if (this._hitModel != null)
            return;
          if (this._hitAdorner != null)
          {
            this.ClearModelHitTestInfo();
            this._hitModel = AdornerProperties.GetModel(this._hitAdorner);
            if (this._hitModel != null && this._focusedTask != null && this._focusedTask.ModelFilter != null)
            {
              switch (this._focusedTask.ModelFilter(this._hitModel))
              {
                case HitTestFilterBehavior.ContinueSkipSelf:
                case HitTestFilterBehavior.ContinueSkipSelfAndChildren:
                  this._hitModel = (ModelItem) null;
                  break;
              }
            }
          }
          if (this._hitModel != null)
            return;
          ModelService modelService = this.ModelService;
          if (modelService == null)
            return;
          this._hitModel = modelService.Root;
        }
      }
    }

    private void PerformAdornerHitTest(Point mouse)
    {
      HitTestFilterCallback filterCallback = (HitTestFilterCallback) null;
      if (this._focusedTask != null)
        filterCallback = this._focusedTask.AdornerFilter;
      PointHitTestResult pointHitTestResult = HitTestHelper.HitTest((Visual) this._adornerLayer, mouse, true, filterCallback) as PointHitTestResult;
      if (pointHitTestResult != null && pointHitTestResult.VisualHit != null)
      {
        if (pointHitTestResult.VisualHit == this._hitRawAdornerVisual)
          return;
        this.ClearAdornerHitTestInfo();
        this._hitRawAdornerVisual = (DependencyObject) pointHitTestResult.VisualHit;
        DependencyObject reference = (DependencyObject) pointHitTestResult.VisualHit;
        FrameworkElement frameworkElement;
        FrameworkContentElement frameworkContentElement;
        while (reference != null && ((frameworkElement = reference as FrameworkElement) == null ? ((frameworkContentElement = reference as FrameworkContentElement) == null ? (DependencyObject) null : frameworkContentElement.TemplatedParent) : frameworkElement.TemplatedParent) != null)
          reference = VisualTreeHelper.GetParent(reference);
        this._hitAdorner = reference;
      }
      else
        this.ClearAdornerHitTestInfo();
    }

    private void PerformModelHitTest(EventArgs eventArgs)
    {
      ViewHitTestFilterCallback filterCallback = (ViewHitTestFilterCallback) null;
      if (this._focusedTask != null)
      {
        if (this._modelHitTestFilter == null && this._focusedTask.ModelFilter != null)
          this._modelHitTestFilter = new ViewHitTestFilterCallback(this.ModelHitTestFilter);
        else if (this._focusedTask.ModelFilter == null)
          this._modelHitTestFilter = (ViewHitTestFilterCallback) null;
        filterCallback = this._modelHitTestFilter;
      }
      ViewItem rootView = this.RootView;
      ViewHitTestResult viewHitTestResult = (ViewHitTestResult) null;
      if (rootView != (ViewItem) null)
      {
        Point mousePosition = this.GetMousePosition(eventArgs, (IInputElement) this.Child);
        GeneralTransform generalTransform = rootView.TransformFromVisual((Visual) this.Child);
        if (generalTransform != null)
          viewHitTestResult = rootView.HitTest(filterCallback, (ViewHitTestResultCallback) null, (HitTestParameters) new PointHitTestParameters(generalTransform.Transform(mousePosition)));
      }
      if (viewHitTestResult != null && viewHitTestResult.ViewHit != (ViewItem) null)
      {
        if (!(viewHitTestResult.ViewHit != this._hitRawModelVisual))
          return;
        this.ClearModelHitTestInfo();
        this._hitRawModelVisual = viewHitTestResult.ViewHit;
        ViewService viewService = this.ViewService;
        if (viewService == null)
          return;
        ViewItem view = viewHitTestResult.ViewHit;
        while (view != (ViewItem) null && (this._hitModel = viewService.GetModel(view)) == null)
          view = view.VisualParent;
      }
      else
        this.ClearModelHitTestInfo();
    }

    private Point GetCapturePoint()
    {
      Point point = this._capturePointInRootElementCoords;
      if (this.ModelService != null && this.ModelService.Root != this._captureModel)
      {
        ViewItem rootView = this.RootView;
        if (rootView != (ViewItem) null)
          point = TransformUtil.GetTransformToAncestor(rootView, (Visual) this).Transform(this._capturePointInRootElementCoords);
      }
      return point;
    }

    private void SetCapturePoint(ViewItem rootElementOnSurface)
    {
      this._capturePointInRootElementCoords = Mouse.GetPosition((IInputElement) this);
      if (this.ModelService == null || this.ModelService.Root == this._captureModel || !(rootElementOnSurface != (ViewItem) null))
        return;
      GeneralTransform generalTransform = rootElementOnSurface.TransformFromVisual((Visual) this);
      if (generalTransform == null)
        return;
      this._capturePointInRootElementCoords = generalTransform.Transform(this._capturePointInRootElementCoords);
    }

    private void SetCapture(UIElement captureOwner)
    {
      this._captureModel = this._hitModel;
      this._captureAdorner = this._hitAdorner;
      this._captureOwner = captureOwner;
      if (this.ModelService != null)
        this.SetCapturePoint(this.RootView);
      try
      {
        this._capturingMouse = true;
        this._captureOwner.CaptureMouse();
      }
      finally
      {
        this._capturingMouse = false;
      }
      for (int index = 0; index < this._mouseState.Length; ++index)
        this._mouseState[index] = new DesignerView.MouseState();
    }

    private Point GetMousePosition(EventArgs inputArgs, IInputElement relativeTo)
    {
      ToolActionEventArgs toolActionEventArgs;
      if ((toolActionEventArgs = inputArgs as ToolActionEventArgs) != null)
      {
        if (ToolAction.DragIntent == toolActionEventArgs.ToolAction)
        {
          for (int index = 0; index < this._mouseState.Length; ++index)
          {
            if (this._mouseState[index].MouseButtonStage == DesignerView.MouseButtonStage.MouseDown)
              return this._mouseState[index].MouseDownPosition;
          }
        }
        inputArgs = toolActionEventArgs.SourceEvent;
      }
      DragEventArgs dragEventArgs;
      if ((dragEventArgs = inputArgs as DragEventArgs) != null)
        return dragEventArgs.GetPosition(relativeTo);
      MouseEventArgs mouseEventArgs;
      if ((mouseEventArgs = inputArgs as MouseEventArgs) != null)
        return mouseEventArgs.GetPosition(relativeTo);
      return Mouse.GetPosition(relativeTo);
    }

    private GestureData CreateGestureData(Task sourceTask, EventArgs inputArgs)
    {
      if (inputArgs is KeyEventArgs)
      {
        ModelItem modelItem = this.Context.Items.GetValue<Selection>().PrimarySelection;
        if (modelItem == null && this.ModelService != null)
          modelItem = this.ModelService.Root;
        if (modelItem != null)
          return new GestureData(this._context, modelItem, modelItem);
      }
      ModelItem targetModel = this._hitModel ?? this._captureModel;
      ToolActionEventArgs toolActionEventArgs;
      if ((toolActionEventArgs = inputArgs as ToolActionEventArgs) != null)
        inputArgs = toolActionEventArgs.SourceEvent;
      Point currentPosition = this.GetMousePosition(inputArgs, (IInputElement) this);
      GiveFeedbackEventArgs feedbackEventArgs;
      if ((feedbackEventArgs = inputArgs as GiveFeedbackEventArgs) != null)
        currentPosition = this._lastCurrentPoint;
      this._lastCurrentPoint = currentPosition;
      ModelItem sourceModel;
      DependencyObject sourceAdorner;
      DependencyObject targetAdorner;
      Point startPosition;
      if (this._captureOwner != null)
      {
        sourceModel = this._captureModel;
        sourceAdorner = this._captureAdorner;
        targetAdorner = this._hitAdorner;
        startPosition = this.GetCapturePoint();
      }
      else
      {
        sourceModel = this._hitModel;
        startPosition = currentPosition;
        sourceAdorner = this._hitAdorner;
        targetAdorner = this._hitAdorner;
      }
      DragEventArgs dragEventArgs;
      MouseWheelEventArgs mouseWheelEventArgs;
      GestureData gestureData = (dragEventArgs = inputArgs as DragEventArgs) == null ? (feedbackEventArgs == null ? ((mouseWheelEventArgs = inputArgs as MouseWheelEventArgs) == null ? (GestureData) new MouseGestureData(this._context, sourceModel, targetModel, (Visual) this, startPosition, currentPosition, sourceAdorner, targetAdorner) : (GestureData) new WheelGestureData(this._context, sourceModel, targetModel, mouseWheelEventArgs.Delta, sourceAdorner, targetAdorner)) : (GestureData) new DragGestureData(this._context, sourceModel, targetModel, (Visual) this, startPosition, currentPosition, this._dragAllowedEffects, this._dragData, sourceAdorner, targetAdorner)) : (GestureData) new DragGestureData(this._context, sourceModel, targetModel, (Visual) this, startPosition, currentPosition, this._dragAllowedEffects, dragEventArgs.Data, sourceAdorner, targetAdorner);
      gestureData.SourceTask = sourceTask;
      return gestureData;
    }

    private void ExecuteCommand(MatchGestureEventArgs command, DragEventArgs args, bool updateHitTest)
    {
      DragGestureData dragGestureData = (DragGestureData) command.Data;
      dragGestureData.Effects = args.Effects;
      this.ExecuteCommand(command, updateHitTest);
      args.Effects = dragGestureData.Effects;
      if (args.Effects == DragDropEffects.None)
        return;
      args.Handled = true;
    }

    private void ExecuteCommand(MatchGestureEventArgs command, bool updateHitTest)
    {
      bool nestedCall = this.BeginCall();
      try
      {
        if (this.CommandException != null)
        {
          if (DesignerView._disableCatchAll.Level == TraceLevel.Off)
          {
            try
            {
              command.Binding.Command.Execute((object) command.Data);
            }
            catch (Exception ex)
            {
              this.OnCommandException(new CommandExceptionEventArgs(command.Binding.Command, ex));
            }
          }
          else
            command.Binding.Command.Execute((object) command.Data);
        }
        else
          command.Binding.Command.Execute((object) command.Data);
      }
      finally
      {
        this.EndCall(nestedCall);
      }
      if (!updateHitTest)
        return;
      this._secondaryHitTestArgs = (EventArgs) command.InputEvent;
    }

    private MatchGestureEventArgs FindCommand(InputEventArgs eventArgs, DesignerView.FindCommandScope scope)
    {
      MatchGestureEventArgs e = (MatchGestureEventArgs) null;
      if (this._secondaryHitTestArgs != null)
        this.PerformHitTest(this._secondaryHitTestArgs);
      if (!this._inCall && !this._clearingCapture && this._context != null && (this._hitModel != null || this._captureModel != null || eventArgs is KeyboardEventArgs))
      {
        Task sourceTask;
        InputBinding inputBinding = this.FindInputBinding(eventArgs, scope, out sourceTask);
        e = new MatchGestureEventArgs(eventArgs, inputBinding, sourceTask, (MatchGestureEventArgs.GetGestureData) ((localTask, localArgs) => this.CreateGestureData(localTask, (EventArgs) localArgs)));
        this.OnMatchGesture(e);
        if (e.Binding == null || !e.Binding.Command.CanExecute((object) e.Data))
          e = (MatchGestureEventArgs) null;
      }
      return e;
    }

    private InputBinding FindInputBinding(InputEventArgs args, DesignerView.FindCommandScope scope, out Task sourceTask)
    {
      InputBinding inputBinding = (InputBinding) null;
      sourceTask = (Task) null;
      if (this._focusedTask != null)
      {
        sourceTask = this._focusedTask;
        inputBinding = DesignerView.FindInputBindingForTask(args, this._focusedTask);
      }
      else
      {
        bool flag = true;
        if ((scope & DesignerView.FindCommandScope.SourceAdorner) == DesignerView.FindCommandScope.SourceAdorner && this._captureAdorner != null)
        {
          inputBinding = DesignerView.FindInputBindingForAdorner(args, this._captureAdorner, out sourceTask);
          flag = false;
        }
        if (inputBinding == null && (scope & DesignerView.FindCommandScope.TargetAdorner) == DesignerView.FindCommandScope.TargetAdorner && this._hitAdorner != null)
        {
          inputBinding = DesignerView.FindInputBindingForAdorner(args, this._hitAdorner, out sourceTask);
          flag = false;
        }
        if (flag && inputBinding == null && (scope & DesignerView.FindCommandScope.Tool) == DesignerView.FindCommandScope.Tool)
        {
          foreach (Task task in this.Context.Items.GetValue<Tool>().Tasks)
          {
            inputBinding = DesignerView.FindInputBindingForTask(args, task);
            if (inputBinding != null)
            {
              sourceTask = task;
              break;
            }
          }
        }
      }
      return inputBinding;
    }

    private static InputBinding FindInputBindingForAdorner(InputEventArgs args, DependencyObject adorner, out Task sourceTask)
    {
      InputBinding inputBinding = (InputBinding) null;
      sourceTask = (Task) null;
      Task task = adorner.GetValue(AdornerProperties.TaskProperty) as Task;
      if (task != null)
      {
        inputBinding = DesignerView.FindInputBindingForTask(args, task);
        if (inputBinding != null)
          sourceTask = task;
      }
      return inputBinding;
    }

    private static InputBinding FindInputBindingForTask(InputEventArgs args, Task task)
    {
      foreach (InputBinding inputBinding in task.InputBindings)
      {
        if (inputBinding.Gesture.Matches(args.OriginalSource, args))
          return inputBinding;
      }
      return (InputBinding) null;
    }

    private static bool IsChildOf(DependencyObject parent, DependencyObject child)
    {
      Visual visual;
      if ((visual = parent as Visual) != null)
        return visual.IsAncestorOf(child);
      Visual3D visual3D;
      if ((visual3D = parent as Visual3D) != null)
        return visual3D.IsAncestorOf(child);
      return false;
    }

    private static void OnAdornersVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      DesignerView designerView = sender as DesignerView;
      if (designerView == null)
        return;
      Visibility visibility = object.Equals(args.NewValue, (object) true) ? Visibility.Visible : Visibility.Collapsed;
      designerView._adornerLayer.Visibility = visibility;
    }

    protected virtual void OnCommandException(CommandExceptionEventArgs e)
    {
      if (this.CommandException == null)
        return;
      this.CommandException((object) this, e);
    }

    protected virtual void OnMatchGesture(MatchGestureEventArgs e)
    {
      if (this.MatchGesture == null)
        return;
      this.MatchGesture((object) this, e);
    }

    private void OnCanExecuteCommand(object sender, CanExecuteRoutedEventArgs args)
    {
      if (this._context == null || args.OriginalSource == this._adornerLayer || args.Handled)
        return;
      Tool tool = this._context.Items.GetValue<Tool>();
      RoutedCommand routedCommand = args.Command as RoutedCommand;
      if (routedCommand == null)
        return;
      DependencyObject sourceAdorner;
      DependencyObject targetAdorner;
      if (this._captureOwner != null)
      {
        sourceAdorner = this._captureAdorner;
        targetAdorner = this._hitAdorner;
      }
      else
      {
        sourceAdorner = this._hitAdorner;
        targetAdorner = this._hitAdorner;
      }
      CommandBinding commandBinding = tool.GetCommandBinding(args.Command, sourceAdorner, targetAdorner);
      if (commandBinding == null)
        return;
      args.Handled = true;
      try
      {
        this._adornerLayer.CommandBindings.Add(commandBinding);
        args.CanExecute = routedCommand.CanExecute(args.Parameter, (IInputElement) this._adornerLayer);
      }
      finally
      {
        this._adornerLayer.CommandBindings.Remove(commandBinding);
      }
    }

    private void OnExecutedCommand(object sender, ExecutedRoutedEventArgs args)
    {
      if (this._context == null || args.Handled || (args.OriginalSource == this._adornerLayer || args.Handled))
        return;
      Tool tool = this._context.Items.GetValue<Tool>();
      RoutedCommand routedCommand = args.Command as RoutedCommand;
      if (routedCommand == null)
        return;
      DependencyObject sourceAdorner;
      DependencyObject targetAdorner;
      if (this._captureOwner != null)
      {
        sourceAdorner = this._captureAdorner;
        targetAdorner = this._hitAdorner;
      }
      else
      {
        sourceAdorner = this._hitAdorner;
        targetAdorner = this._hitAdorner;
      }
      CommandBinding commandBinding = tool.GetCommandBinding(args.Command, sourceAdorner, targetAdorner);
      if (commandBinding == null)
        return;
      args.Handled = true;
      try
      {
        this._adornerLayer.CommandBindings.Add(commandBinding);
        if (this.CommandException != null)
        {
          try
          {
            routedCommand.Execute(args.Parameter, (IInputElement) this._adornerLayer);
          }
          catch (Exception ex)
          {
            this.OnCommandException(new CommandExceptionEventArgs((ICommand) routedCommand, ex));
          }
        }
        else
          routedCommand.Execute(args.Parameter, (IInputElement) this._adornerLayer);
      }
      finally
      {
        this._adornerLayer.CommandBindings.Remove(commandBinding);
      }
    }

    protected override Visual GetVisualChild(int index)
    {
      if (base.Child != null)
      {
        switch (index)
        {
          case 0:
            return (Visual) this.Child;
          case 1:
            return (Visual) this._hitTestLayer;
          case 2:
            return (Visual) this._adornerLayer;
        }
      }
      return (Visual) base.Child;
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      Size size = base.ArrangeOverride(arrangeSize);
      if (VisualTreeHelper.GetParent((DependencyObject) this._adornerLayer) != null)
      {
        this._adornerLayer.Arrange(new Rect(arrangeSize));
        this._hitTestLayer.Arrange(new Rect(arrangeSize));
      }
      return size;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      Size size = base.MeasureOverride(constraint);
      if (VisualTreeHelper.GetParent((DependencyObject) this._adornerLayer) != null)
      {
        this._adornerLayer.Measure(constraint);
        this._hitTestLayer.Measure(constraint);
      }
      return size;
    }

    private void OnDecoratorDragEnter(object sender, DragEventArgs args)
    {
      this.PerformHitTest((EventArgs) args);
      if (args.Handled)
        return;
      this.PerformDragEnterLeave(args, (ModelItem) null, (DependencyObject) null);
      args.Handled = true;
    }

    private void OnDecoratorDragLeave(object sender, DragEventArgs args)
    {
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      this.ClearAllHitTestInfo();
      if (args.Handled)
        return;
      this.PerformDragEnterLeave(args, priorModelHit, priorAdornerHit);
    }

    private void OnDecoratorDragOver(object sender, DragEventArgs args)
    {
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      args.Effects = DragDropEffects.None;
      this.PerformHitTest((EventArgs) args);
      this._dragAllowedEffects = args.AllowedEffects;
      this._dragData = args.Data;
      if (args.Handled)
        return;
      this.PerformDragEnterLeave(args, priorModelHit, priorAdornerHit);
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragOver, (EventArgs) args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
      if (command != null)
        this.ExecuteCommand(command, args, true);
      args.Handled = true;
    }

    private void OnDecoratorDrop(object sender, DragEventArgs args)
    {
      try
      {
        this.PerformHitTest((EventArgs) args);
        if (args.Handled)
          return;
        MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragDrop, (EventArgs) args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
        if (command == null)
          return;
        this.ExecuteCommand(command, args, true);
      }
      finally
      {
        this._dragAllowedEffects = DragDropEffects.None;
        this._dragData = (IDataObject) null;
      }
    }

    private void OnDecoratorGiveFeedback(object sender, GiveFeedbackEventArgs args)
    {
      if (this._dragData == null || args.Handled)
        return;
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragFeedback, (EventArgs) args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
      if (command != null)
      {
        this.ExecuteCommand(command, false);
        args.UseDefaultCursors = false;
        args.Handled = true;
      }
      else
      {
        Cursor cursor = this._context.Items.GetValue<Tool>().Cursor;
        if (cursor == null)
          return;
        Mouse.SetCursor(cursor);
        args.UseDefaultCursors = false;
        args.Handled = true;
      }
    }

    private void PerformDragEnterLeave(DragEventArgs args, ModelItem priorModelHit, DependencyObject priorAdornerHit)
    {
      if (priorModelHit == this._hitModel && priorAdornerHit == this._hitAdorner)
        return;
      if (priorModelHit != null)
      {
        ModelItem modelItem = this._hitModel;
        DependencyObject dependencyObject = this._hitAdorner;
        this._hitModel = priorModelHit;
        this._hitAdorner = priorAdornerHit;
        try
        {
          MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragLeave, (EventArgs) args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
          if (command != null)
            this.ExecuteCommand(command, args, true);
        }
        finally
        {
          this._hitModel = modelItem;
          this._hitAdorner = dependencyObject;
          this._dragAllowedEffects = DragDropEffects.None;
          this._dragData = (IDataObject) null;
        }
      }
      this._dragAllowedEffects = args.AllowedEffects;
      this._dragData = args.Data;
      MatchGestureEventArgs command1 = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragEnter, (EventArgs) args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
      if (command1 == null)
        return;
      this.ExecuteCommand(command1, args, true);
    }

    private void OnMouseEnter(object sender, MouseEventArgs args)
    {
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      this.PerformHitTest((EventArgs) args);
      if (args.Handled)
        return;
      this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
    }

    private void OnMouseLeave(object sender, MouseEventArgs args)
    {
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      this.PerformHitTest((EventArgs) args);
      if (args.Handled)
        return;
      this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
    }

    private void OnMouseMove(object sender, MouseEventArgs args)
    {
      Point point = this.PointToScreen(args.GetPosition((IInputElement) this));
      if (point.Equals(this._lastPointInParentCoordinates) || this._capturingMouse)
        return;
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      this.PerformHitTest((EventArgs) args);
      this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
      this.PerformMouseMove(args);
      this.PerformMouseDragIntent(args);
      this.StartHoverCountdown();
      this._lastPointInParentCoordinates = point;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs args)
    {
      this.StopHoverCountdown();
      ModelItem priorModelHit = this._hitModel;
      DependencyObject priorAdornerHit = this._hitAdorner;
      this.PerformHitTest((EventArgs) args);
      if (priorModelHit != this._hitModel || priorAdornerHit != this._hitAdorner)
        this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
      if (args.Handled)
        return;
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Down, (InputEventArgs) args), DesignerView.FindCommandScope.All);
      if (command != null)
      {
        this.SetMouseDownState(args);
        this.ExecuteCommand(command, true);
        args.Handled = true;
      }
      else
      {
        if (this._hitModel == null)
          return;
        this.SetMouseDownState(args);
        args.Handled = true;
      }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs args)
    {
      this.PerformHitTest((EventArgs) args);
      int index = (int) args.ChangedButton;
      DesignerView.MouseState mouseState = this._mouseState[index];
      ModelItem priorModelHit = this._captureModel;
      DependencyObject priorAdornerHit = this._captureAdorner;
      try
      {
        if (!this._abortCapture)
        {
          bool handled = args.Handled;
          if (mouseState.MouseButtonStage != DesignerView.MouseButtonStage.None && !handled)
          {
            MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Up, (InputEventArgs) args), DesignerView.FindCommandScope.All);
            if (command != null)
            {
              this.ExecuteCommand(command, true);
              args.Handled = true;
            }
          }
          mouseState = this._mouseState[index];
          if (mouseState.MouseButtonStage == DesignerView.MouseButtonStage.DragIntentRaised)
          {
            ToolActionEventArgs toolActionEventArgs = new ToolActionEventArgs(ToolAction.DragComplete, (InputEventArgs) args);
            this.PerformHitTest((EventArgs) toolActionEventArgs);
            MatchGestureEventArgs command = this.FindCommand((InputEventArgs) toolActionEventArgs, DesignerView.FindCommandScope.All);
            if (command != null)
            {
              this.ExecuteCommand(command, true);
              args.Handled = true;
            }
          }
          mouseState = this._mouseState[index];
          if (mouseState.MouseButtonStage == DesignerView.MouseButtonStage.MouseDown)
          {
            if (mouseState.DispatcherClickCount > 0 && Math.Abs(mouseState.MouseDownPosition.X - this._clickPoint.X) < (double) this._doubleClickSizeX && Math.Abs(mouseState.MouseDownPosition.Y - this._clickPoint.Y) < (double) this._doubleClickSizeY)
              ++this._clickCount;
            else
              this._clickCount = 0;
            if (this._hitModel != null)
            {
              if (this._hitModel == this._clickModel)
              {
                if (this._hitAdorner == this._clickAdorner)
                {
                  if (!handled)
                  {
                    if (this._clickCount > 0)
                    {
                      ToolActionEventArgs toolActionEventArgs = new ToolActionEventArgs(ToolAction.Click, (InputEventArgs) args);
                      this.PerformHitTest((EventArgs) toolActionEventArgs);
                      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) toolActionEventArgs, DesignerView.FindCommandScope.SourceAdorner | DesignerView.FindCommandScope.Tool);
                      if (command != null)
                      {
                        this.ExecuteCommand(command, true);
                        args.Handled = true;
                      }
                    }
                    MatchGestureEventArgs command1 = this.FindCommand((InputEventArgs) args, DesignerView.FindCommandScope.All);
                    if (command1 != null)
                    {
                      this.ExecuteCommand(command1, true);
                      args.Handled = true;
                    }
                    if (this._clickCount == 2)
                    {
                      if (!handled)
                      {
                        ToolActionEventArgs toolActionEventArgs = new ToolActionEventArgs(ToolAction.DoubleClick, (InputEventArgs) args);
                        this.PerformHitTest((EventArgs) toolActionEventArgs);
                        MatchGestureEventArgs command2 = this.FindCommand((InputEventArgs) toolActionEventArgs, DesignerView.FindCommandScope.SourceAdorner | DesignerView.FindCommandScope.Tool);
                        if (command2 != null)
                        {
                          this.ExecuteCommand(command2, true);
                          args.Handled = true;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      finally
      {
        mouseState.MouseButtonStage = DesignerView.MouseButtonStage.None;
        this._mouseState[index] = mouseState;
        if (this._captureOwner != null && args.LeftButton == MouseButtonState.Released && (args.MiddleButton == MouseButtonState.Released && args.RightButton == MouseButtonState.Released) && (args.XButton1 == MouseButtonState.Released && args.XButton2 == MouseButtonState.Released))
          this.ClearCapture();
      }
      this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs args)
    {
      if (args.Handled)
        return;
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Wheel, (InputEventArgs) args), DesignerView.FindCommandScope.All);
      if (command == null)
        return;
      this.ExecuteCommand(command, true);
      args.Handled = true;
    }

    private void OnHoverTimeout(object sender, EventArgs args)
    {
      this._hoverTimer.Stop();
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Hover, args, (InputDevice) Mouse.PrimaryDevice, Environment.TickCount), DesignerView.FindCommandScope.All);
      if (command == null)
        return;
      this.ExecuteCommand(command, true);
    }

    private void PerformMouseEnterLeave(InputEventArgs args, ModelItem priorModelHit, DependencyObject priorAdornerHit, bool usePreviousModelForEnter)
    {
      if (priorModelHit == this._hitModel && priorAdornerHit == this._hitAdorner)
        return;
      this._clickCount = 0;
      this._clickPoint = new Point();
      ModelItem modelItem = this._hitModel;
      DependencyObject dependencyObject = this._hitAdorner;
      ViewItem viewItem = this._hitRawModelVisual;
      if (priorModelHit != null)
      {
        this._hitModel = priorModelHit;
        this._hitAdorner = priorAdornerHit;
        try
        {
          MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Leave, args), DesignerView.FindCommandScope.All);
          if (command != null)
          {
            this.ExecuteCommand(command, true);
            args.Handled = true;
          }
        }
        finally
        {
          this._hitModel = modelItem;
          this._hitAdorner = dependencyObject;
          this._hitRawModelVisual = viewItem;
        }
      }
      if (usePreviousModelForEnter)
      {
        this._hitModel = priorModelHit;
        this._hitAdorner = priorAdornerHit;
      }
      try
      {
        if (this._hitModel != null)
        {
          MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Enter, args), DesignerView.FindCommandScope.All);
          if (command == null)
            return;
          this.ExecuteCommand(command, true);
          args.Handled = true;
        }
        else
        {
          if (this._captureOwner == null)
            return;
          MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.DragOutside, args), DesignerView.FindCommandScope.All);
          if (command == null)
            return;
          this.ExecuteCommand(command, true);
          args.Handled = true;
        }
      }
      finally
      {
        if (usePreviousModelForEnter)
        {
          this._hitModel = modelItem;
          this._hitAdorner = dependencyObject;
          this._hitRawModelVisual = viewItem;
        }
      }
    }

    private void PerformMouseDragIntent(MouseEventArgs args)
    {
      if (this._captureOwner == null)
        return;
      for (int index = 0; index < this._mouseState.Length; ++index)
      {
        DesignerView.MouseState mouseState = this._mouseState[index];
        if (mouseState.MouseButtonStage == DesignerView.MouseButtonStage.MouseDown)
        {
          Point position = args.GetPosition((IInputElement) this);
          if (Math.Abs(position.X - mouseState.MouseDownPosition.X) >= this.MinHorizontalDragDistance || Math.Abs(position.Y - mouseState.MouseDownPosition.Y) >= this.MinVerticalDragDistance)
          {
            mouseState.MouseButtonStage = DesignerView.MouseButtonStage.DragIntentRaised;
            this._mouseState[index] = mouseState;
            if (this._hitModel != null)
            {
              ToolActionEventArgs toolActionEventArgs = new ToolActionEventArgs(ToolAction.DragIntent, (InputEventArgs) args);
              this.PerformHitTest((EventArgs) toolActionEventArgs);
              MatchGestureEventArgs command = this.FindCommand((InputEventArgs) toolActionEventArgs, DesignerView.FindCommandScope.SourceAdorner | DesignerView.FindCommandScope.Tool);
              if (command != null)
              {
                this.ExecuteCommand(command, true);
                args.Handled = true;
              }
            }
          }
        }
      }
    }

    private void PerformMouseMove(MouseEventArgs args)
    {
      MatchGestureEventArgs command = this.FindCommand((InputEventArgs) new ToolActionEventArgs(ToolAction.Move, (InputEventArgs) args), DesignerView.FindCommandScope.All);
      if (command == null)
        return;
      this.ExecuteCommand(command, true);
      args.Handled = true;
    }

    private void SetMouseDownState(MouseButtonEventArgs args)
    {
      if (this._captureOwner == null)
        this.SetCapture((UIElement) this);
      this.Focus();
      int index = (int) args.ChangedButton;
      DesignerView.MouseState mouseState = this._mouseState[index];
      mouseState.MouseDownPosition = args.GetPosition((IInputElement) this);
      mouseState.MouseButtonStage = DesignerView.MouseButtonStage.MouseDown;
      mouseState.DispatcherClickCount = args.ClickCount;
      this._mouseState[index] = mouseState;
      if (mouseState.DispatcherClickCount != 1)
        return;
      this._clickCount = 0;
      this._clickModel = this._hitModel;
      this._clickAdorner = this._hitAdorner;
      this._clickPoint = mouseState.MouseDownPosition;
    }

    private void StartHoverCountdown()
    {
      Point position = Mouse.GetPosition((IInputElement) this);
      if (this._hoverTimer == null)
      {
        this._hoverTimer = new DispatcherTimer(new TimeSpan(this._hoverTimeout), DispatcherPriority.ApplicationIdle, new EventHandler(this.OnHoverTimeout), Dispatcher.CurrentDispatcher);
        this._hoverTimer.Start();
        this._hoverPoint = position;
      }
      else
      {
        if (Math.Abs(position.X - this._hoverPoint.X) < (double) this._hoverSizeX && Math.Abs(position.Y - this._hoverPoint.Y) < (double) this._hoverSizeY)
          return;
        this._hoverPoint = position;
        this._hoverTimer.Stop();
        this._hoverTimer.Start();
      }
    }

    private void StopHoverCountdown()
    {
      if (this._hoverTimer == null)
        return;
      this._hoverTimer.Stop();
    }

    private void OnHitTestQueryCursor(object sender, QueryCursorEventArgs args)
    {
      if (args.Handled || this._context == null)
        return;
      Cursor cursor = this._context.Items.GetValue<Tool>().Cursor;
      if (cursor == null)
        return;
      args.Cursor = cursor;
      args.Handled = true;
    }

    private void OnDecoratorKeyDown(object sender, KeyEventArgs args)
    {
      if (args.Handled)
        return;
      if (args.Key == System.Windows.Input.Key.Escape)
      {
        this.AbortCapture();
        if (this._focusedTask != null)
          this._focusedTask.Revert();
        args.Handled = true;
      }
      else
      {
        ModelItem priorModelHit = this._hitModel;
        DependencyObject priorAdornerHit = this._hitAdorner;
        this.PerformHitTest((EventArgs) args);
        this.PerformMouseEnterLeave((InputEventArgs) args, priorModelHit, priorAdornerHit, false);
        MatchGestureEventArgs command = this.FindCommand((InputEventArgs) args, DesignerView.FindCommandScope.Tool);
        if (command == null)
          return;
        this.ExecuteCommand(command, true);
        args.Handled = true;
      }
    }

    private void OnLostMouseCapture(object sender, MouseEventArgs args)
    {
      if (this._clearingCapture)
        return;
      this.AbortCapture();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      if (!this._protectReEntrantCode)
      {
        if (this._designerViewAutomationPeer == null)
        {
          try
          {
            this._protectReEntrantCode = true;
            this._designerViewAutomationPeer = new DesignerViewAutomationPeer(this);
          }
          finally
          {
            this._protectReEntrantCode = false;
          }
        }
      }
      return (AutomationPeer) this._designerViewAutomationPeer;
    }

    private static bool IsZoomLevelValid(object value)
    {
      return (double) value > 0.0;
    }

    private static void HandleZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DesignerView designerView = d as DesignerView;
      if (designerView == null)
        return;
      designerView.SetZoomTransform();
      if (designerView.ZoomLevelChanged == null)
        return;
      designerView.ZoomLevelChanged((object) designerView, EventArgs.Empty);
    }

    public virtual Transform GetZoomTransform()
    {
      return TransformUtil.GetTransformToImmediateParent((DependencyObject) this);
    }

    protected virtual void SetZoomTransform()
    {
      double zoomLevel = this.ZoomLevel;
      this.LayoutTransform = (Transform) new ScaleTransform(zoomLevel, zoomLevel);
    }

    [System.Flags]
    private enum FindCommandScope
    {
      SourceAdorner = 1,
      TargetAdorner = 2,
      Tool = 4,
      Adorners = TargetAdorner | SourceAdorner,
      All = Adorners | Tool,
    }

    private struct MouseState
    {
      internal Point MouseDownPosition;
      internal DesignerView.MouseButtonStage MouseButtonStage;
      internal int DispatcherClickCount;
    }

    private enum MouseButtonStage
    {
      None,
      MouseDown,
      DragIntentRaised,
    }

    internal class OpaqueElement : UIElement
    {
      protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParams)
      {
        return new GeometryHitTestResult((Visual) this, IntersectionDetail.FullyContains);
      }

      protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
      {
        return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParams.HitPoint);
      }
    }
  }
}
