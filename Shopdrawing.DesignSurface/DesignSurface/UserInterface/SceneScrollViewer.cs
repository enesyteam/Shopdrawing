// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SceneScrollViewer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal sealed class SceneScrollViewer : Border, INotifyPropertyChanged
  {
    private Rect previousArtboardBounds = Rect.Empty;
    private static readonly double[] ZoomPresetValueArray = new double[23]
    {
      1.0 / 32.0,
      1.0 / 24.0,
      1.0 / 16.0,
      1.0 / 12.0,
      0.125,
      1.0 / 6.0,
      0.25,
      1.0 / 3.0,
      0.5,
      2.0 / 3.0,
      1.0,
      1.5,
      2.0,
      3.0,
      4.0,
      6.0,
      8.0,
      12.0,
      16.0,
      24.0,
      32.0,
      48.0,
      64.0
    };
    private static readonly double[] ZoomComboValueArray = new double[10]
    {
      0.125,
      0.25,
      1.0 / 3.0,
      0.5,
      2.0 / 3.0,
      1.0,
      1.5,
      2.0,
      4.0,
      8.0
    };
    private static readonly double DefaultZoom = 1.0;
    private static readonly int SmallScrollPixels = 25;
    private static readonly float ScrollContentPadding = 10f;
    private static readonly float ScrollDocumentPadding = 60f;
    public static PresetCollection zoomPresetCollection = new PresetCollection(SceneScrollViewer.ZoomPresetValueArray, SceneScrollViewer.DefaultZoom);
    public static PresetCollection zoomComboCollection = new PresetCollection(SceneScrollViewer.ZoomComboValueArray, SceneScrollViewer.DefaultZoom);
    private SceneView sceneView;
    private ExtendedScrollBar horizontalScrollBar;
    private ExtendedScrollBar verticalScrollBar;
    private Artboard artboard;
    private ComboBoxItem zoomFitToScreenItem;
    private bool isUpdateScheduled;
    private bool isUpdatingScrollBars;
    private int accumulateMouseWheelDelta;
    private RecordTargetDescriptionBase previousRecordTargetDescription;
    private bool isInGridDesignModeLast;

    public Artboard Artboard
    {
      get
      {
        return this.artboard;
      }
    }

    public ICommand ZoomInCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ZoomInCommandBinding_Execute));
      }
    }

    public ICommand ZoomOutCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ZoomOutCommandBinding_Execute));
      }
    }

    public ICommand ZoomToFitCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ZoomToFitCommandBinding_Execute));
      }
    }

    public ICommand FinishEditingZoomCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.FinishEditingZoomCommand_Execute));
      }
    }

    public bool IsRecording
    {
      get
      {
        return this.sceneView.ViewModel.AnimationEditor.IsRecording;
      }
      set
      {
        this.sceneView.ViewModel.AnimationEditor.IsRecording = value;
      }
    }

    public bool CanRecord
    {
      get
      {
        return this.sceneView.ViewModel.AnimationEditor.CanRecord;
      }
    }

    public RecordTargetDescriptionBase RecordTargetDescription
    {
      get
      {
        if (this.previousRecordTargetDescription == null || this.IsRecording)
        {
          if (this.sceneView.ViewModel.StateStoryboardEditTarget != null)
          {
            if (this.sceneView.ViewModel.StateEditTarget.StateGroup.IsSketchFlowAnimation)
            {
              string str = this.sceneView.ViewModel.StateEditTarget.StateGroup.Name;
              if (str.StartsWith(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, StringComparison.Ordinal))
                str = str.Substring(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter.Length);
              Microsoft.Expression.DesignSurface.View.RecordTargetDescription targetDescription = new Microsoft.Expression.DesignSurface.View.RecordTargetDescription();
              targetDescription.TargetTypeName = this.sceneView.ViewModel.StateEditTarget.Description;
              targetDescription.TargetName = str;
              this.previousRecordTargetDescription = (RecordTargetDescriptionBase) targetDescription;
            }
            else
            {
              Microsoft.Expression.DesignSurface.View.RecordTargetDescription targetDescription = new Microsoft.Expression.DesignSurface.View.RecordTargetDescription();
              targetDescription.TargetTypeName = StringTable.State;
              targetDescription.TargetName = this.sceneView.ViewModel.StateEditTarget.Name;
              this.previousRecordTargetDescription = (RecordTargetDescriptionBase) targetDescription;
            }
          }
          else if (this.sceneView.ViewModel.TransitionStoryboardEditTarget != null)
          {
            TransitionRecordTargetDescription targetDescription = new TransitionRecordTargetDescription();
            targetDescription.TargetTypeName = StringTable.Transition;
            targetDescription.FromStateName = this.sceneView.ViewModel.TransitionEditTarget.FromStateName;
            targetDescription.ToStateName = this.sceneView.ViewModel.TransitionEditTarget.ToStateName;
            this.previousRecordTargetDescription = (RecordTargetDescriptionBase) targetDescription;
          }
          else if (this.sceneView.ViewModel.AnimationEditor.ActiveStoryboardTimeline != null)
          {
            Microsoft.Expression.DesignSurface.View.RecordTargetDescription targetDescription = new Microsoft.Expression.DesignSurface.View.RecordTargetDescription();
            targetDescription.TargetTypeName = StringTable.Timeline;
            targetDescription.TargetName = this.sceneView.ViewModel.AnimationEditor.ActiveStoryboardTimeline.Name;
            this.previousRecordTargetDescription = (RecordTargetDescriptionBase) targetDescription;
          }
          else if (this.sceneView.ViewModel.ActiveVisualTrigger != null)
          {
            Microsoft.Expression.DesignSurface.View.RecordTargetDescription targetDescription = new Microsoft.Expression.DesignSurface.View.RecordTargetDescription();
            targetDescription.TargetTypeName = StringTable.Trigger;
            targetDescription.TargetName = this.sceneView.ViewModel.ActiveVisualTrigger.PresentationName;
            this.previousRecordTargetDescription = (RecordTargetDescriptionBase) targetDescription;
          }
        }
        return this.previousRecordTargetDescription;
      }
    }

    public ICommand SwitchToDefaultStateCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.SwitchToDefaultState));
      }
    }

    public bool EffectButtonEnabled
    {
      get
      {
        return this.Artboard.Zoom <= this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.EffectsEnabledZoomThreshold;
      }
    }

    public bool EffectsEnabled
    {
      get
      {
        return this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.EffectsEnabled;
      }
      set
      {
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.EffectsEnabled = value;
        this.sceneView.ViewModel.DesignerContext.NotifyArtboardOptionsChanged();
        this.OnPropertyChanged("EffectsEnabled");
      }
    }

    public string EffectDisabledTooltip
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.EffectDisabledToolTip, new object[1]
        {
          (object) Math.Round(this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.EffectsEnabledZoomThreshold * 100.0, 2)
        });
      }
    }

    public bool SnapToGrid
    {
      get
      {
        return this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.SnapToGrid;
      }
      set
      {
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.SnapToGrid = value;
        this.sceneView.ViewModel.DesignerContext.NotifyArtboardOptionsChanged();
        this.OnPropertyChanged("SnapToGrid");
      }
    }

    public bool ShowGrid
    {
      get
      {
        return this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.ShowGrid;
      }
      set
      {
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.ShowGrid = value;
        this.sceneView.ViewModel.DesignerContext.NotifyArtboardOptionsChanged();
        this.OnPropertyChanged("ShowGrid");
      }
    }

    public bool SnapToSnapLines
    {
      get
      {
        return this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.SnapToSnapLines;
      }
      set
      {
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.SnapToSnapLines = value;
        this.sceneView.ViewModel.DesignerContext.NotifyArtboardOptionsChanged();
        this.OnPropertyChanged("SnapToSnapLines");
      }
    }

    public bool AnnotationsEnabled
    {
      get
      {
        AnnotationService annotationService = this.sceneView.DesignerContext.AnnotationService;
        if (annotationService == null)
          return false;
        return annotationService.AnnotationsEnabled;
      }
    }

    public bool ShowAnnotations
    {
      get
      {
        if (!this.AnnotationsEnabled)
          return false;
        AnnotationService annotationService = this.sceneView.DesignerContext.AnnotationService;
        if (annotationService == null)
          return false;
        return annotationService.ShowAnnotations;
      }
      set
      {
        AnnotationService annotationService = this.sceneView.DesignerContext.AnnotationService;
        if (annotationService == null)
          return;
        annotationService.ShowAnnotations = value;
      }
    }

    public Brush ArtboardBrush
    {
      get
      {
        ArtboardOptionsModel artboardOptionsModel = this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel;
        if (!artboardOptionsModel.IsUsingCustomBackgroundColor)
          return (Brush) Application.Current.Resources[(object) "DefaultArtboardBackgroundBrush"];
        SolidColorBrush solidColorBrush = new SolidColorBrush(artboardOptionsModel.CustomBackgroundColor);
        solidColorBrush.Freeze();
        return (Brush) solidColorBrush;
      }
    }

    public BreadcrumbBarModel BreadcrumbBarModel
    {
      get
      {
        return this.sceneView.ViewModel.DesignerContext.BreadcrumbBarModel;
      }
    }

    public Thickness VerticalScrollBarMargin
    {
      get
      {
        return this.verticalScrollBar.Margin;
      }
      set
      {
        this.verticalScrollBar.Margin = value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public SceneScrollViewer(SceneView sceneView, Artboard artboard)
    {
      this.sceneView = sceneView;
      this.sceneView.ViewModel.AnimationEditor.RecordModeChanged += new EventHandler(this.AnimationEditor_RecordModeChanged);
      this.artboard = artboard;
      this.CreateComponents();
      this.LayoutUpdated += new EventHandler(this.SceneScrollViewer_LayoutUpdated);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);
      switch (this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.ArtboardZoomGesture)
      {
        case ArtboardZoomGesture.MouseWheel:
          if (Keyboard.Modifiers == ModifierKeys.Control)
          {
            this.VerticallyScrollView(e);
            break;
          }
          if (Keyboard.Modifiers == ModifierKeys.Shift)
          {
            this.HorizontallyScrollView(e);
            break;
          }
          if (Keyboard.Modifiers != ModifierKeys.None)
            break;
          this.ZoomView(e);
          break;
        case ArtboardZoomGesture.ControlMouseWheel:
          if (Keyboard.Modifiers == ModifierKeys.Control)
          {
            this.ZoomView(e);
            break;
          }
          if (Keyboard.Modifiers == ModifierKeys.Shift)
          {
            this.HorizontallyScrollView(e);
            break;
          }
          if (Keyboard.Modifiers != ModifierKeys.None)
            break;
          this.VerticallyScrollView(e);
          break;
        case ArtboardZoomGesture.AltMouseWheel:
          if (Keyboard.Modifiers == ModifierKeys.Alt)
          {
            this.ZoomView(e);
            break;
          }
          if (Keyboard.Modifiers == ModifierKeys.Control)
          {
            this.HorizontallyScrollView(e);
            break;
          }
          if (Keyboard.Modifiers != ModifierKeys.None)
            break;
          this.VerticallyScrollView(e);
          break;
      }
    }

    private void CreateComponents()
    {
      this.Child = (UIElement) Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\SceneScrollViewer.xaml");
      this.zoomFitToScreenItem = ((FrameworkElement) this.Child).FindName("ZoomFitToScreenItem") as ComboBoxItem;
      this.horizontalScrollBar = (ExtendedScrollBar) ElementUtilities.FindElement((FrameworkElement) this, "HorizontalScrollBar");
      this.horizontalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HorizontalScrollBar_ValueChanged);
      this.verticalScrollBar = (ExtendedScrollBar) ElementUtilities.FindElement((FrameworkElement) this, "VerticalScrollBar");
      this.verticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VerticalScrollBar_ValueChanged);
      ((Decorator) ElementUtilities.FindElement((FrameworkElement) this, "ArtboardBorder")).Child = (UIElement) this.artboard;
      this.artboard.DataContext = (object) null;
      this.artboard.FocusVisualStyle = (Style) null;
      InputMethod.SetIsInputMethodEnabled((DependencyObject) this.artboard, false);
      this.artboard.ZoomPresetCollection = SceneScrollViewer.zoomPresetCollection;
      this.artboard.ZoomComboCollection = SceneScrollViewer.zoomComboCollection;
      this.artboard.ZoomChanged += new EventHandler(this.Artboard_ZoomChanged);
      this.artboard.CenterChanged += new EventHandler(this.Artboard_CenterChanged);
      this.artboard.SnapToGridRenderer.Attach(this.sceneView.ViewModel.DesignerContext);
      if (this.sceneView.ViewModel.DesignerContext != null)
      {
        AnnotationService annotationService = this.sceneView.DesignerContext.AnnotationService;
        if (annotationService != null)
        {
          annotationService.ShowAnnotationsChanged += new EventHandler(this.AnnotationService_ShowAnnotationsChanged);
          annotationService.AnnotationsEnabledChanged += new EventHandler(this.AnnotationService_AnnotationsEnabledChanged);
        }
      }
      if (this.sceneView.ViewModel.DesignerContext.SnappingEngine != null)
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsChanged += new EventHandler(this.OnArtboardOptionsChanged);
      if (this.sceneView.ViewModel.DesignerContext.WindowService != null)
        this.sceneView.ViewModel.DesignerContext.WindowService.ThemeChanged += new EventHandler(this.WindowManager_ThemeChanged);
      this.sceneView.ViewModel.LateSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdate);
      this.UpdateSmallChange();
    }

    internal void TearDown()
    {
      this.sceneView.ViewModel.AnimationEditor.RecordModeChanged -= new EventHandler(this.AnimationEditor_RecordModeChanged);
      this.sceneView.ViewModel.LateSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_LateSceneUpdate);
      this.artboard.ZoomChanged -= new EventHandler(this.Artboard_ZoomChanged);
      this.artboard.CenterChanged -= new EventHandler(this.Artboard_CenterChanged);
      this.artboard.SnapToGridRenderer.Detach();
      if (this.sceneView.ViewModel.DesignerContext != null)
      {
        AnnotationService annotationService = this.sceneView.DesignerContext.AnnotationService;
        if (annotationService != null)
        {
          annotationService.ShowAnnotationsChanged -= new EventHandler(this.AnnotationService_ShowAnnotationsChanged);
          annotationService.AnnotationsEnabledChanged -= new EventHandler(this.AnnotationService_AnnotationsEnabledChanged);
        }
      }
      if (this.sceneView.ViewModel.DesignerContext.SnappingEngine != null)
        this.sceneView.ViewModel.DesignerContext.ArtboardOptionsChanged -= new EventHandler(this.OnArtboardOptionsChanged);
      if (this.sceneView.ViewModel.DesignerContext.WindowService != null)
        this.sceneView.ViewModel.DesignerContext.WindowService.ThemeChanged -= new EventHandler(this.WindowManager_ThemeChanged);
      this.LayoutUpdated -= new EventHandler(this.SceneScrollViewer_LayoutUpdated);
      this.horizontalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.HorizontalScrollBar_ValueChanged);
      this.verticalScrollBar.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.VerticalScrollBar_ValueChanged);
      this.horizontalScrollBar = (ExtendedScrollBar) null;
      this.verticalScrollBar = (ExtendedScrollBar) null;
      this.artboard.TearDown();
      this.artboard = (Artboard) null;
      this.Child = (UIElement) null;
      this.sceneView = (SceneView) null;
    }

    private void AnnotationService_ShowAnnotationsChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("ShowAnnotations");
    }

    private void AnnotationService_AnnotationsEnabledChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("AnnotationsEnabled");
      this.OnPropertyChanged("ShowAnnotations");
    }

    private void OnArtboardOptionsChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("SnapToGrid");
      this.OnPropertyChanged("ShowGrid");
      this.OnPropertyChanged("SnapToSnapLines");
      this.OnPropertyChanged("ArtboardBrush");
      this.OnPropertyChanged("EffectsEnabled");
      this.OnPropertyChanged("EffectButtonEnabled");
      this.OnPropertyChanged("EffectDisabledTooltip");
      bool inGridDesignMode = this.sceneView.ViewModel.DesignerContext.ArtboardOptionsModel.IsInGridDesignMode;
      if (inGridDesignMode == this.isInGridDesignModeLast)
        return;
      this.isInGridDesignModeLast = inGridDesignMode;
      this.sceneView.ViewModel.RefreshSelection();
    }

    private void WindowManager_ThemeChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("ArtboardBrush");
    }

    private void Artboard_ZoomChanged(object sender, EventArgs args)
    {
      this.UpdateSmallChange();
      this.UpdateScrollBars();
      this.OnPropertyChanged("EffectsEnabled");
      this.OnPropertyChanged("EffectButtonEnabled");
    }

    private void Artboard_CenterChanged(object sender, EventArgs e)
    {
      this.UpdateScrollBars();
    }

    private void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
      if (this.isUpdatingScrollBars)
        return;
      double num = args.NewValue + this.horizontalScrollBar.ViewportSize / 2.0;
      if (this.Artboard.CenterX == num)
        return;
      this.Artboard.CenterX = num;
    }

    private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
      if (this.isUpdatingScrollBars)
        return;
      double num = args.NewValue + this.verticalScrollBar.ViewportSize / 2.0;
      if (this.Artboard.CenterY == num)
        return;
      this.Artboard.CenterY = num;
    }

    private void SceneScrollViewer_LayoutUpdated(object sender, EventArgs e)
    {
      this.UpdateScrollBars();
    }

    private void UpdateSmallChange()
    {
      double num = (double) SceneScrollViewer.SmallScrollPixels / this.Artboard.Zoom;
      this.horizontalScrollBar.SmallChange = num;
      this.verticalScrollBar.SmallChange = num;
    }

    private void UpdateScrollBars()
    {
      if (this.isUpdateScheduled)
        return;
      this.isUpdateScheduled = true;
      if (double.IsNaN(this.Artboard.CenterX) || double.IsNaN(this.Artboard.CenterY))
      {
        this.Artboard.CenterAll();
        this.isUpdateScheduled = false;
      }
      else
        UIThreadDispatcher.Instance.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.UpdateScrollBarsWorker));
    }

    private void UpdateScrollBarsWorker()
    {
      this.isUpdateScheduled = false;
      if (this.Artboard == null || this.horizontalScrollBar.IsScrolling || this.verticalScrollBar.IsScrolling)
        return;
      this.isUpdatingScrollBars = true;
      try
      {
        Rect artboardBounds = this.Artboard.ArtboardBounds;
        Rect rect1 = this.Artboard.ContentBounds;
        Rect documentBounds = this.Artboard.DocumentBounds;
        if (documentBounds.IsEmpty)
          return;
        if (rect1.IsEmpty)
          rect1 = documentBounds;
        double num1 = (double) SceneScrollViewer.ScrollContentPadding / this.Artboard.Zoom;
        double num2 = (double) SceneScrollViewer.ScrollDocumentPadding / this.Artboard.Zoom;
        this.horizontalScrollBar.ContentMinimum = Math.Min(rect1.Left - num1, Math.Min(-num2, documentBounds.Width - artboardBounds.Width + num2));
        this.horizontalScrollBar.ContentMaximum = Math.Max(rect1.Right + num1, Math.Max(artboardBounds.Width - num2, documentBounds.Width + num2)) - artboardBounds.Width;
        this.verticalScrollBar.ContentMinimum = Math.Min(rect1.Top - num1, Math.Min(-num2, documentBounds.Height - artboardBounds.Height + num2));
        this.verticalScrollBar.ContentMaximum = Math.Max(rect1.Bottom + num1, Math.Max(artboardBounds.Height - num2, documentBounds.Height + num2)) - artboardBounds.Height;
        Rect rect2 = new Rect(SceneScrollViewer.NormalizeValue(artboardBounds.Left, double.MinValue, double.MaxValue, double.MaxValue), SceneScrollViewer.NormalizeValue(artboardBounds.Top, double.MinValue, double.MaxValue, double.MaxValue), SceneScrollViewer.NormalizeValue(artboardBounds.Width, double.MaxValue, double.MaxValue, double.MaxValue), SceneScrollViewer.NormalizeValue(artboardBounds.Height, double.MaxValue, double.MaxValue, double.MaxValue));
        if (!(rect2 != this.previousArtboardBounds))
          return;
        this.horizontalScrollBar.ViewportSize = rect2.Width;
        this.horizontalScrollBar.Value = rect2.Left;
        this.verticalScrollBar.ViewportSize = rect2.Height;
        this.verticalScrollBar.Value = rect2.Top;
        this.previousArtboardBounds = rect2;
      }
      finally
      {
        this.isUpdatingScrollBars = false;
      }
    }

    private static double NormalizeValue(double value, double negativeInfinityValue, double positiveInfinityValue, double nanValue)
    {
      if (double.IsNegativeInfinity(value))
        return negativeInfinityValue;
      if (double.IsPositiveInfinity(value))
        return positiveInfinityValue;
      if (double.IsNaN(value))
        return nanValue;
      return value;
    }

    private void VerticallyScrollView(MouseWheelEventArgs e)
    {
      this.verticalScrollBar.Value -= (double) (3 * e.Delta) / 120.0 * this.verticalScrollBar.SmallChange;
      e.Handled = true;
    }

    private void HorizontallyScrollView(MouseWheelEventArgs e)
    {
      this.horizontalScrollBar.Value -= (double) (3 * e.Delta) / 120.0 * this.horizontalScrollBar.SmallChange;
      e.Handled = true;
    }

    private void ZoomView(MouseWheelEventArgs e)
    {
      this.accumulateMouseWheelDelta += e.Delta;
      int num1 = (int) Math.Round((double) Math.Abs(this.accumulateMouseWheelDelta) / 120.0);
      if (num1 != 0)
      {
        bool isZoomingIn = this.accumulateMouseWheelDelta > 0;
        int num2 = (isZoomingIn ? -1 : 1) * 120;
        Point position = e.GetPosition((IInputElement) this.artboard.ContentArea);
        for (int index = 0; index < num1; ++index)
        {
          this.artboard.ZoomAroundFixedPoint(isZoomingIn, position);
          this.accumulateMouseWheelDelta += num2;
        }
      }
      e.Handled = true;
    }

    private void ZoomInCommandBinding_Execute()
    {
      this.artboard.ZoomIn();
    }

    private void ZoomOutCommandBinding_Execute()
    {
      this.artboard.ZoomOut();
    }

    private void ZoomToFitCommandBinding_Execute()
    {
      this.sceneView.ZoomToFitScreen(true);
      this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) new DispatcherOperationCallback(this.ZoomToFitWorker), (object) null);
    }

    private void FinishEditingZoomCommand_Execute()
    {
      this.sceneView.ReturnFocus();
    }

    private object ZoomToFitWorker(object obj)
    {
      if (this.zoomFitToScreenItem != null)
        this.zoomFitToScreenItem.IsSelected = false;
      return (object) null;
    }

    private void AnimationEditor_RecordModeChanged(object sender, EventArgs e)
    {
      this.OnPropertyChanged("IsRecording");
      this.OnPropertyChanged("RecordTargetDescription");
      this.OnPropertyChanged("CanRecord");
    }

    private void ViewModel_LateSceneUpdate(object target, SceneUpdatePhaseEventArgs args)
    {
      bool flag = false;
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.RecordMode))
        this.OnPropertyChanged("IsRecording");
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTrigger | SceneViewModel.ViewStateBits.ActiveTimeline | SceneViewModel.ViewStateBits.ActiveStateContext))
      {
        this.OnPropertyChanged("CanRecord");
        flag = true;
      }
      else if (args.DocumentChanges != null && args.DocumentChanges.Count > 0)
      {
        foreach (DocumentNodeChange documentNodeChange in args.DocumentChanges.DistinctChanges)
        {
          if (documentNodeChange.IsPropertyChange && documentNodeChange.PropertyKey.Name == "Name" || documentNodeChange.ParentNode != null && (documentNodeChange.ParentNode.Type.Name == "Trigger" || documentNodeChange.ParentNode.Type.Name.Contains("Condition")))
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        return;
      this.previousRecordTargetDescription = (RecordTargetDescriptionBase) null;
      this.OnPropertyChanged("RecordTargetDescription");
    }

    private void SwitchToDefaultState()
    {
      this.sceneView.ViewModel.SetActiveTrigger((TriggerBaseNode) null);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
