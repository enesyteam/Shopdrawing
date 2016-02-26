// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelineView
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.SceneCommands;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class TimelineView : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private bool seekOnTimelineChange = true;
    private bool isPaused = true;
    private bool areAdornersVisibleWhenPaused = true;
    public static readonly DependencyProperty TimelinePaneProperty = DependencyProperty.Register("TimelinePane", typeof (TimelinePane), typeof (TimelineView), new PropertyMetadata((object) null, new PropertyChangedCallback(TimelineView.HandleTimelinePaneChanged)));
    public static readonly DependencyProperty ElementAreaHeightProperty = DependencyProperty.Register("ElementAreaHeight", typeof (double), typeof (TimelineView), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty SeekControlProperty = DependencyProperty.RegisterAttached("SeekControl", typeof (SeekControlType), typeof (TimelineView), (PropertyMetadata) new FrameworkPropertyMetadata((object) SeekControlType.FullScrub, FrameworkPropertyMetadataOptions.Inherits));
    private static bool isSnapping = true;
    private static double playStopsPerSecond = 10.0;
    private static double dragTolerance = 2.0;
    private static double unitsPerSecond = 50.0;
    public static readonly double ZoomFactorMinimum = 0.000555555555555556;
    public static readonly double ZoomFactorMaximum = 16.0;
    private const int autoScrollTime = 50;
    private const double minimumUnitsPerSecond = 0.0277777777777778;
    private const double defaultUnitsPerSecond = 50.0;
    private const double maximumUnitsPerSecond = 800.0;
    public const double MaximumSupportedWidth = 390000.0;
    public const double LeftMarginUnits = 10.0;
    internal const double IndefiniteDuration = 700000.0;
    private DesignerContext designerContext;
    private DispatcherTimer autoScrollTimer;
    private bool isApplyingKeyFrameEdits;
    private KeyFrameItem editingKeyFrameItem;
    private ITimedTimelineItem editingTimedItem;
    private static bool isSuppressingSnapping;
    private ContextMenu keyFrameContextMenu;
    private HorizontalScrollManager horizontalScrollManager;
    private TimelinePane cachedTimelinePane;
    private IViewStoryboard currentPlayingViewStoryboard;
    private double currentTime;
    private Point mouseDownPoint;
    private bool capturing;
    private TimelineView.MarqueSelectionState marqueeSelectionState;
    private TimelineItem startMarqueeItem;
    private double startMarqueeTime;
    private double endMarqueeTime;
    internal TimelineView Root;
    internal Button ButtonFirst;
    internal Button ButtonBack;
    internal TimelinePlayPauseButton ButtonPlay;
    internal Button ButtonForward;
    internal Button ButtonLast;
    internal StringEditor CurrentTimeTextBox;
    internal Button ButtonNewKeyframe;
    internal Border TimelineCanvasBorder;
    internal Grid TimelineCanvas;
    internal ScrollViewer TimelineRowsViewer;
    internal ListBox TimelineRows;
    internal ScrollViewer KeyframeRowsViewer;
    internal ListBox KeyframeRows;
    internal FeedbackLayer FeedbackLayer;
    internal ScrollViewer BackgroundViewer;
    internal Grid TimelineBackground;
    internal TimeMarkerControl TimeMarkerControl;
    internal Grid PlaybackLayer;
    internal Rectangle PlaybackHeadScrollRectangle;
    internal Image PlaybackHead;
    internal Line PlaybackLine;
    internal Border TickBarOverlay;
    internal ExtendedScrollBar HorizontalScroll;
    private bool _contentLoaded;

    public TimelinePane TimelinePane
    {
      get
      {
        if (this.cachedTimelinePane != null)
          return this.cachedTimelinePane;
        return (TimelinePane) this.GetValue(TimelineView.TimelinePaneProperty);
      }
      set
      {
        this.SetValue(TimelineView.TimelinePaneProperty, (object) value);
      }
    }

    public HorizontalScrollManager HorizontalScrollManager
    {
      get
      {
        return this.horizontalScrollManager;
      }
    }

    public double ElementAreaHeight
    {
      get
      {
        return (double) this.GetValue(TimelineView.ElementAreaHeightProperty);
      }
      set
      {
        this.SetValue(TimelineView.ElementAreaHeightProperty, (object) value);
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        TimelinePane timelinePane = this.TimelinePane;
        if (timelinePane != null)
          return timelinePane.ActiveSceneViewModel;
        return (SceneViewModel) null;
      }
    }

    public AnimationEditor ActiveAnimationEditor
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        if (activeSceneViewModel != null)
          return activeSceneViewModel.AnimationEditor;
        return (AnimationEditor) null;
      }
    }

    public KeyFrameItem EditingKeyFrameItem
    {
      get
      {
        return this.editingKeyFrameItem;
      }
      set
      {
        this.editingKeyFrameItem = value;
      }
    }

    public bool CanCopyKeyframes
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return CopyCommand.CanCopySelection(this.ActiveSceneViewModel);
        return false;
      }
    }

    public bool CanCutKeyframes
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.CanDeleteSelection)
          return CopyCommand.CanCopySelection(this.ActiveSceneViewModel);
        return false;
      }
    }

    public bool CanDeleteKeyframes
    {
      get
      {
        return this.ActiveSceneViewModel != null && !this.ActiveSceneViewModel.KeyFrameSelectionSet.IsEmpty;
      }
    }

    public bool CanEaseSelectedKeyFrames
    {
      get
      {
        if (this.ActiveSceneViewModel == null)
          return false;
        foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection)
        {
          if (keyFrameSceneNode.KeyFrameAnimation.IsDiscreteOnly)
            return false;
        }
        return true;
      }
    }

    private static bool IsSuppressingSnapping
    {
      get
      {
        if (!Keyboard.IsKeyDown(Key.S))
          return TimelineView.isSuppressingSnapping;
        return true;
      }
    }

    public double EaseInSelectedKeyFrames
    {
      get
      {
        return this.CalculateCommonEase(EasePointType.EaseIn);
      }
      set
      {
        if (!this.CanEaseSelectedKeyFrames)
          return;
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.SetKeyEaseUndo))
        {
          if (!double.IsNaN(value))
            this.ActiveAnimationEditor.SetEaseOfSelection(new Point?(this.PercentageToEasePoint(EasePointType.EaseIn, value)), new Point?());
          editTransaction.Commit();
        }
      }
    }

    public double EaseOutSelectedKeyFrames
    {
      get
      {
        return this.CalculateCommonEase(EasePointType.EaseOut);
      }
      set
      {
        if (!this.CanEaseSelectedKeyFrames)
          return;
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.SetKeyEaseUndo))
        {
          if (!double.IsNaN(value))
            this.ActiveAnimationEditor.SetEaseOfSelection(new Point?(), new Point?(this.PercentageToEasePoint(EasePointType.EaseOut, value)));
          editTransaction.Commit();
        }
      }
    }

    public bool HoldInSelectedKeyFrames
    {
      get
      {
        if (this.ActiveSceneViewModel == null)
          return false;
        ICollection<KeyFrameSceneNode> collection = (ICollection<KeyFrameSceneNode>) this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection;
        bool flag = collection.Count > 0;
        foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) collection)
        {
          if (keyFrameSceneNode.InterpolationType != KeyFrameInterpolationType.Discrete)
          {
            flag = false;
            break;
          }
        }
        return flag;
      }
      set
      {
        if (!this.CanEaseSelectedKeyFrames)
          return;
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.SetKeyEaseUndo))
        {
          ICollection<KeyFrameSceneNode> collection = (ICollection<KeyFrameSceneNode>) this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection;
          List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
          this.ActiveSceneViewModel.KeyFrameSelectionSet.Clear();
          foreach (KeyFrameSceneNode keyFrame in (IEnumerable<KeyFrameSceneNode>) collection)
          {
            KeyFrameSceneNode keyFrameSceneNode = keyFrame;
            if (value)
            {
              if (keyFrame.InterpolationType != KeyFrameInterpolationType.Discrete)
                keyFrameSceneNode = keyFrame.KeyFrameAnimation.ReplaceKeyFrame(keyFrame, KeyFrameInterpolationType.Discrete, (KeySpline) null);
            }
            else if (keyFrame.InterpolationType == KeyFrameInterpolationType.Discrete)
            {
              KeyFrameInterpolationType interpolationType = !this.designerContext.ActiveSceneViewModel.ProjectContext.Platform.Metadata.IsSupported((ITypeResolver) this.designerContext.ActiveSceneViewModel.ProjectContext, PlatformTypes.IEasingFunction) ? KeyFrameInterpolationType.Spline : KeyFrameInterpolationType.Easing;
              keyFrameSceneNode = keyFrame.KeyFrameAnimation.ReplaceKeyFrame(keyFrame, interpolationType, (KeySpline) null);
            }
            if (keyFrameSceneNode != null)
              list.Add(keyFrameSceneNode);
          }
          this.ActiveSceneViewModel.KeyFrameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) list, (KeyFrameSceneNode) null);
          editTransaction.Commit();
        }
      }
    }

    public double UnitsPerSecond
    {
      get
      {
        return TimelineView.unitsPerSecond;
      }
      set
      {
        value = Math.Max(Math.Min(value, 800.0), 1.0 / 36.0);
        if (TimelineView.unitsPerSecond == value)
          return;
        TimelineView.unitsPerSecond = value;
        this.OnPropertyChanged("UnitsPerSecond");
        this.OnPropertyChanged("CurrentTime");
        this.OnPropertyChanged("ZoomFactor");
        if (!this.IsPaused)
          this.IsPaused = true;
        this.OnZoomChanged(this.TimelinePane.ScopedTimelineItem);
      }
    }

    public double ZoomFactor
    {
      get
      {
        return this.UnitsPerSecond / (28799.0 / 36.0) * (TimelineView.ZoomFactorMaximum - TimelineView.ZoomFactorMinimum);
      }
      set
      {
        this.UnitsPerSecond = value / (TimelineView.ZoomFactorMaximum - TimelineView.ZoomFactorMinimum) * (28799.0 / 36.0);
      }
    }

    public double LastSignificantTime
    {
      get
      {
        double num = this.CurrentTime;
        if (this.TimelinePane != null && this.TimelinePane.TimelineItemManager != null)
          num = Math.Max(num, this.TimelinePane.TimelineItemManager.LastAnimationTime);
        if (this.EditingKeyFrameItem != (KeyFrameItem) null)
          num = Math.Max(num, this.EditingKeyFrameItem.Time);
        if (this.editingTimedItem != null)
          num = Math.Max(num, this.editingTimedItem.AbsoluteEndTime);
        if (this.startMarqueeItem != null)
          num = Math.Max(Math.Max(this.startMarqueeTime, this.endMarqueeTime), num);
        return num;
      }
    }

    public bool CanPlay
    {
      get
      {
        if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveStoryboardTimeline == null)
          return false;
        return this.ActiveAnimationEditor.ActiveStoryboardTimeline.PlayDuration > 0.0;
      }
    }

    public bool CanKeyFrame
    {
      get
      {
        if (this.ActiveAnimationEditor != null)
          return this.ActiveAnimationEditor.CanKeyFrame;
        return false;
      }
    }

    public bool NeedKeyframeRowsViewer
    {
      get
      {
        if (this.CanKeyFrame)
          return true;
        if (this.ActiveAnimationEditor == null)
          return false;
        if (!(this.ActiveAnimationEditor.ActiveStoryboardContainer is StyleNode))
          return this.ActiveAnimationEditor.ActiveStoryboardContainer is FrameworkTemplateElement;
        return true;
      }
    }

    public bool CanAddExplicitKeyFrame
    {
      get
      {
        bool flag = false;
        if (this.ActiveSceneViewModel != null)
        {
          foreach (SceneNode sceneNode in this.ActiveSceneViewModel.ElementSelectionSet.Selection)
          {
            if (sceneNode.IsViewObjectValid)
            {
              flag = true;
              break;
            }
          }
          foreach (KeyFrameSceneNode keyFrameSceneNode in this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection)
          {
            SceneElement sceneElement = keyFrameSceneNode.TargetElement as SceneElement;
            if (sceneElement != null && sceneElement.IsViewObjectValid)
            {
              flag = true;
              break;
            }
          }
          foreach (SceneNode sceneNode in this.ActiveSceneViewModel.ChildPropertySelectionSet.Selection)
          {
            if (sceneNode is EffectNode && sceneNode.IsViewObjectValid)
            {
              flag = true;
              break;
            }
          }
        }
        if (this.CanKeyFrame && flag)
          return this.ActiveSceneViewModel.ElementSelectionSet.GetFilteredSelection<FrameworkTemplateElement>().Count == 0;
        return false;
      }
    }

    public bool IsEaseInOutAvailable
    {
      get
      {
        if (this.TimelinePane != null && this.TimelinePane.ActiveSceneViewModel != null)
          return this.TimelinePane.ActiveSceneViewModel.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsEaseInOut);
        return false;
      }
    }

    public bool IsSnapping
    {
      get
      {
        return TimelineView.isSnapping;
      }
      set
      {
        if (TimelineView.isSnapping == value)
          return;
        TimelineView.isSnapping = value;
        this.OnPropertyChanged("IsSnapping");
        if (!this.IsSnapping)
          return;
        this.CurrentTime = this.CurrentTime;
      }
    }

    public static bool IsTimelineSnapping
    {
      get
      {
        return TimelineView.isSnapping;
      }
    }

    public double CurrentTime
    {
      get
      {
        return this.currentTime;
      }
      set
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SetCurrentTime);
        this.SetCurrentTimeCore(value);
        if (this.ActiveSceneViewModel != null)
        {
          this.ActiveSceneViewModel.StoredSeekTime = this.currentTime;
          if (this.IsPaused)
            this.ActiveSceneViewModel.RefreshCurrentValues();
        }
        FrameworkElement frameworkElement = (FrameworkElement) this.PlaybackHeadScrollRectangle;
        if (frameworkElement != null)
          frameworkElement.BringIntoView();
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SetCurrentTime);
        ValueEditorUtils.UpdateBinding((FrameworkElement) this.CurrentTimeTextBox, StringEditor.ValueProperty, false);
      }
    }

    public bool CanSeek
    {
      get
      {
        if (this.IsPaused)
          return this.CanKeyFrame;
        return false;
      }
    }

    public bool IsPaused
    {
      get
      {
        return this.isPaused;
      }
      set
      {
        if (this.isPaused == value || this.ActiveSceneViewModel == null || this.ActiveAnimationEditor.ActiveStoryboardTimeline == null)
          return;
        this.isPaused = value;
        SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
        if (!this.isPaused)
        {
          if (this.CurrentTime >= TimelineView.SnapSeconds(this.ActiveAnimationEditor.ActiveStoryboardTimeline.PlayDuration))
            this.CurrentTime = 0.0;
          if (this.designerContext.ActiveView != null)
          {
            this.areAdornersVisibleWhenPaused = this.designerContext.ActiveView.AdornerLayer.IsVisible;
            this.designerContext.ActiveView.AdornerLayer.Hide();
          }
          if (this.ActiveAnimationEditor.IsActiveStoryboardMediaDirty)
            this.ActiveAnimationEditor.UpdateActiveTimeline(false);
          if (this.ActiveAnimationEditor != null && this.ActiveAnimationEditor.ActiveStoryboardTarget != null)
          {
            this.RegisterForPostPlayEvents();
            this.ActiveAnimationEditor.ActiveViewStoryboard.Play();
          }
        }
        else
        {
          InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
          double num1 = double.PositiveInfinity;
          if (this.ActiveAnimationEditor.ActiveStoryboardTarget != null)
            num1 = this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentTime;
          this.currentPlayingViewStoryboard = (IViewStoryboard) null;
          if (this.ActiveAnimationEditor.ActiveViewStoryboard != null)
          {
            this.ActiveAnimationEditor.ActiveViewStoryboard.Pause();
            this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentTimeChanged -= new EventHandler<EventArgs>(this.HandleStoryboardTimeChange);
            if (this.ActiveSceneViewModel.DesignerContext != null && this.ActiveSceneViewModel.ProjectContext.Platform.InstanceBuilderFactory != null && this.ActiveAnimationEditor.ActiveStoryboardTimeline != null)
            {
              double num2 = num1;
              double playDuration = this.ActiveAnimationEditor.ActiveStoryboardTimeline.PlayDuration;
              if (num2 > playDuration)
                num2 = playDuration;
              this.CurrentTime = num2;
            }
          }
          if (this.designerContext.ActiveView != null && this.areAdornersVisibleWhenPaused)
            this.designerContext.ActiveView.AdornerLayer.Show();
        }
        this.OnPropertyChanged("IsPaused");
        this.OnPropertyChanged("CanSeek");
      }
    }

    public double PlayStopsPerSecond
    {
      get
      {
        return TimelineView.playStopsPerSecond;
      }
    }

    public bool IsApplyingKeyFrameEdits
    {
      get
      {
        return this.isApplyingKeyFrameEdits;
      }
    }

    public ScrollViewer TimelineRowsScroller
    {
      get
      {
        this.TimelineRows.ApplyTemplate();
        return (ScrollViewer) this.TimelineRows.Template.FindName("Scroller", (FrameworkElement) this.TimelineRows);
      }
    }

    public ScrollViewer KeyframeRowsScroller
    {
      get
      {
        this.KeyframeRows.ApplyTemplate();
        return (ScrollViewer) this.KeyframeRows.Template.FindName("Scroller", (FrameworkElement) this.KeyframeRows);
      }
    }

    public ContextMenu KeyFrameContextMenu
    {
      get
      {
        if (this.keyFrameContextMenu == null)
        {
          this.keyFrameContextMenu = (ContextMenu) FileTable.GetElement("Resources\\Timeline\\KeyFrameContextMenu.xaml");
          this.keyFrameContextMenu.DataContext = (object) this;
          this.keyFrameContextMenu.Opened += new RoutedEventHandler(this.KeyFrameContextMenu_Opened);
        }
        return this.keyFrameContextMenu;
      }
    }

    public ICommand CutKeyFramesCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CutKeyframes));
      }
    }

    public ICommand CopyKeyFramesCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CopyKeyframes));
      }
    }

    public ICommand DeleteKeyFramesCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteKeyframes));
      }
    }

    public string DeleteKeyFramesCommandName
    {
      get
      {
        return StringTable.DeleteKeyFramesCommandName;
      }
    }

    public ObservableCollectionWorkaround<EaseInPercentModel> EaseInCollectionSelectedKeyFrames
    {
      get
      {
        ObservableCollectionWorkaround<EaseInPercentModel> collectionWorkaround = new ObservableCollectionWorkaround<EaseInPercentModel>();
        double percent = 0.0;
        while (percent <= 100.0)
        {
          collectionWorkaround.Add(new EaseInPercentModel(this, percent));
          percent += 25.0;
        }
        return collectionWorkaround;
      }
    }

    public ObservableCollectionWorkaround<EaseOutPercentModel> EaseOutCollectionSelectedKeyFrames
    {
      get
      {
        ObservableCollectionWorkaround<EaseOutPercentModel> collectionWorkaround = new ObservableCollectionWorkaround<EaseOutPercentModel>();
        double percent = 0.0;
        while (percent <= 100.0)
        {
          collectionWorkaround.Add(new EaseOutPercentModel(this, percent));
          percent += 25.0;
        }
        return collectionWorkaround;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TimelineView()
    {
      this.InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
      this.Loaded += new RoutedEventHandler(this.HandleLoaded);
      this.Unloaded += new RoutedEventHandler(this.HandleUnloaded);
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ApplyKeyFrameEditsCommand, new ExecutedRoutedEventHandler(this.ApplyKeyFrameEditsCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.EditSelectedKeyFramesCommand, new ExecutedRoutedEventHandler(this.EditSelectedKeyFramesCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.AddExplicitKeyFrameCommand, new ExecutedRoutedEventHandler(this.AddExplicitKeyFrameCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.SeekBackCommand, new ExecutedRoutedEventHandler(this.SeekBackCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.SeekForwardCommand, new ExecutedRoutedEventHandler(this.SeekForwardCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.SeekBeginCommand, new ExecutedRoutedEventHandler(this.SeekBeginCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.SeekEndCommand, new ExecutedRoutedEventHandler(this.SeekEndCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ShowSnapResolutionCommand, new ExecutedRoutedEventHandler(this.ShowSnapResolutionCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ZoomInCommand, new ExecutedRoutedEventHandler(this.ZoomInCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ZoomOutCommand, new ExecutedRoutedEventHandler(this.ZoomOutCommandBinding_Execute)));
      this.horizontalScrollManager = new HorizontalScrollManager(this);
      this.horizontalScrollManager.ScrollBar = (ScrollBar) this.HorizontalScroll;
      this.horizontalScrollManager.RegisterViewer(this.KeyframeRowsViewer);
      this.horizontalScrollManager.RegisterViewer(this.TimelineRowsViewer);
      this.horizontalScrollManager.RegisterViewer(this.BackgroundViewer);
      this.currentTime = 0.0;
      base.OnInitialized(e);
    }

    private void HandleLoaded(object sender, EventArgs e)
    {
      if (this.TimelinePane == null)
        return;
      this.designerContext = this.TimelinePane.DesignerContext;
      SelectionManager selectionManager = this.designerContext.SelectionManager;
      selectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      selectionManager.ActiveSceneSwitched += new EventHandler(this.SelectionManager_ActiveSceneSwitched);
      selectionManager.ActiveSceneSwitching += new EventHandler(this.SelectionManager_ActiveSceneSwitching);
      this.TimelinePane.VerticalScrollManager.RegisterViewer(this.TimelineRowsScroller);
      this.TimelinePane.VerticalScrollManager.RegisterViewer(this.KeyframeRowsScroller);
      this.KeyframeRowsScroller.ScrollChanged += new ScrollChangedEventHandler(this.KeyframeRowsScroller_ScrollChanged);
      this.UpdateCurrentTimeFromActiveScene();
      this.OnActiveTimelineChanged();
      this.TimelineCanvasBorder.PreviewMouseDown += new MouseButtonEventHandler(this.OnScrubAreaPointerButtonDown);
      this.TimelineCanvasBorder.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.OnScrubAreaPointerButtonUp);
    }

    private void HandleUnloaded(object sender, EventArgs e)
    {
      if (this.designerContext != null && this.designerContext.SelectionManager != null)
      {
        SelectionManager selectionManager = this.designerContext.SelectionManager;
        selectionManager.LateActiveSceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
        selectionManager.ActiveSceneSwitched -= new EventHandler(this.SelectionManager_ActiveSceneSwitched);
        selectionManager.ActiveSceneSwitching -= new EventHandler(this.SelectionManager_ActiveSceneSwitching);
      }
      this.KeyframeRowsScroller.ScrollChanged -= new ScrollChangedEventHandler(this.KeyframeRowsScroller_ScrollChanged);
    }

    public static SeekControlType GetSeekControl(DependencyObject dependencyObject)
    {
      return (SeekControlType) dependencyObject.GetValue(TimelineView.SeekControlProperty);
    }

    public static void SetSeekControl(DependencyObject dependencyObject, SeekControlType value)
    {
      dependencyObject.SetValue(TimelineView.SeekControlProperty, (object) value);
    }

    private static void SuppressSnapping()
    {
      TimelineView.isSuppressingSnapping = true;
    }

    private static void UnSupressSnapping()
    {
      TimelineView.isSuppressingSnapping = false;
    }

    private double EasePointToPercentage(EasePointType type, Point easePoint)
    {
      if (type != EasePointType.EaseIn)
        return easePoint.X * 200.0;
      return (1.0 - easePoint.X) * 200.0;
    }

    private Point PercentageToEasePoint(EasePointType type, double percentage)
    {
      if (type == EasePointType.EaseIn)
        return new Point(1.0 - percentage / 200.0, 1.0);
      return new Point(percentage / 200.0, 0.0);
    }

    private double CalculateCommonEase(EasePointType easePointType)
    {
      double num1 = double.NaN;
      bool flag1 = easePointType == EasePointType.EaseIn;
      if (this.ActiveSceneViewModel != null)
      {
        ICollection<KeyFrameSceneNode> collection = (ICollection<KeyFrameSceneNode>) this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection;
        bool flag2 = true;
        foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) collection)
        {
          if (flag1 && keyFrameSceneNode.InterpolationType == KeyFrameInterpolationType.Discrete)
            return double.NaN;
          Point easePoint = flag1 ? keyFrameSceneNode.EaseInControlPoint : keyFrameSceneNode.EaseOutControlPoint;
          if (!(flag1 ? easePoint.Y == 1.0 : easePoint.Y == 0.0))
            return double.NaN;
          double num2 = this.EasePointToPercentage(easePointType, easePoint);
          if (flag2)
          {
            num1 = num2;
            flag2 = false;
          }
          else if (num1 != num2)
            return double.NaN;
        }
      }
      return num1;
    }

    private void OnZoomChanged(TimelineItem timelineItem)
    {
      timelineItem.OnZoomChanged();
      foreach (TimelineItem timelineItem1 in (Collection<TimelineItem>) timelineItem.Children)
        this.OnZoomChanged(timelineItem1);
    }

    protected void SetCurrentTimeCore(double currentTime)
    {
      if (double.IsNaN(currentTime))
        return;
      if (currentTime < 0.0)
        currentTime = 0.0;
      this.currentTime = TimelineView.SnapSeconds(currentTime, false);
      if (this.ActiveAnimationEditor != null)
        this.ActiveAnimationEditor.SeekTo(this.currentTime);
      this.OnPropertyChanged("CurrentTime");
      this.OnPropertyChanged("LastSignificantTime");
    }

    private void RegisterForPostPlayEvents()
    {
      this.CleanupPostPlayEvents();
      InputManager.Current.PostNotifyInput += new NotifyInputEventHandler(this.Current_PostNotifyInput);
      this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentStateChanged += new EventHandler<EventArgs>(this.PlayHeadDone);
      this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentTimeChanged += new EventHandler<EventArgs>(this.HandleStoryboardTimeChange);
      this.currentPlayingViewStoryboard = this.ActiveAnimationEditor.ActiveViewStoryboard;
    }

    private void CleanupPostPlayEvents()
    {
      InputManager.Current.PostNotifyInput -= new NotifyInputEventHandler(this.Current_PostNotifyInput);
      if (this.currentPlayingViewStoryboard == null)
        return;
      this.currentPlayingViewStoryboard.CurrentStateChanged -= new EventHandler<EventArgs>(this.PlayHeadDone);
      this.currentPlayingViewStoryboard.CurrentTimeChanged -= new EventHandler<EventArgs>(this.HandleStoryboardTimeChange);
      this.currentPlayingViewStoryboard = (IViewStoryboard) null;
    }

    private void HandleStoryboardTimeChange(object sender, EventArgs e)
    {
      if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveViewStoryboard == null)
        return;
      this.currentTime = this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentTime;
      FrameworkElement frameworkElement = (FrameworkElement) this.PlaybackHeadScrollRectangle;
      if (frameworkElement != null)
        frameworkElement.BringIntoView();
      this.OnPropertyChanged("CurrentTime");
    }

    internal void BeginTimedItemEdit(ITimedTimelineItem timedItem)
    {
      INotifyPropertyChanged notifyPropertyChanged = timedItem as INotifyPropertyChanged;
      if (notifyPropertyChanged == null)
        return;
      this.editingTimedItem = timedItem;
      notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.editingTimedItem_PropertyChanged);
    }

    private void editingTimedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "AbsoluteClipBegin":
        case "AbsoluteClipEnd":
          this.OnPropertyChanged("LastSignificantTime");
          break;
      }
    }

    internal void EndTimedItemEdit(ITimedTimelineItem timedItem)
    {
      INotifyPropertyChanged notifyPropertyChanged = timedItem as INotifyPropertyChanged;
      if (notifyPropertyChanged == null)
        return;
      notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.editingTimedItem_PropertyChanged);
      this.editingTimedItem = (ITimedTimelineItem) null;
    }

    internal static double PositionFromSeconds(double seconds)
    {
      return 10.0 + seconds * TimelineView.unitsPerSecond;
    }

    internal static double SecondsFromPosition(double units)
    {
      return TimelineView.SnapSeconds((units - 10.0) / TimelineView.unitsPerSecond);
    }

    internal static double ValueFromSeconds(double seconds)
    {
      return seconds * TimelineView.unitsPerSecond;
    }

    internal static double SecondsFromValue(double value)
    {
      return TimelineView.SnapSeconds(value / TimelineView.unitsPerSecond);
    }

    internal static double SnapSeconds(double seconds)
    {
      return TimelineView.SnapSeconds(seconds, true);
    }

    internal static double SnapSeconds(double seconds, bool snapToTimelineResolution)
    {
      double num;
      if (TimelineView.isSnapping && !TimelineView.IsSuppressingSnapping)
        num = TimelineView.playStopsPerSecond;
      else if (snapToTimelineResolution)
      {
        num = TimelineView.unitsPerSecond;
      }
      else
      {
        double maxSeconds = TimelineSceneNode.MaxSeconds;
        if (seconds >= maxSeconds)
          return maxSeconds;
        return TimeSpan.FromSeconds(seconds).TotalSeconds;
      }
      return TimeSpan.FromSeconds(Math.Min(TimelineSceneNode.MaxSeconds, Math.Round(seconds * num) / num)).TotalSeconds;
    }

    private void PlayHeadDone(object o, EventArgs a)
    {
      if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveViewStoryboard == null || this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentState != ViewStoryboardState.Filling)
        return;
      if (!this.IsPaused)
      {
        this.OnPlayButton((object) null, (RoutedEventArgs) null);
      }
      else
      {
        this.ActiveAnimationEditor.ActiveViewStoryboard.CurrentStateChanged -= new EventHandler<EventArgs>(this.PlayHeadDone);
        this.currentPlayingViewStoryboard = (IViewStoryboard) null;
      }
    }

    private void OnPlayButton(object e, RoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      this.IsPaused = !this.IsPaused;
    }

    private void Current_PostNotifyInput(object sender, NotifyInputEventArgs e)
    {
      if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveViewStoryboard == null || this.currentPlayingViewStoryboard != null && this.ActiveAnimationEditor.ActiveViewStoryboard != this.currentPlayingViewStoryboard)
      {
        this.CleanupPostPlayEvents();
        this.isPaused = true;
      }
      else
      {
        if (e.StagingItem == null)
          return;
        MouseButtonEventArgs mouseButtonEventArgs = e.StagingItem.Input as MouseButtonEventArgs;
        KeyEventArgs keyEventArgs = e.StagingItem.Input as KeyEventArgs;
        KeyboardFocusChangedEventArgs changedEventArgs = e.StagingItem.Input as KeyboardFocusChangedEventArgs;
        if (mouseButtonEventArgs != null)
        {
          if (mouseButtonEventArgs.ButtonState != MouseButtonState.Pressed || this.ButtonPlay.IsMouseOver)
            return;
          this.IsPaused = true;
        }
        else if (keyEventArgs != null && keyEventArgs.IsDown)
        {
          this.IsPaused = true;
        }
        else
        {
          if (changedEventArgs == null || changedEventArgs.NewFocus == this.ButtonPlay)
            return;
          this.IsPaused = true;
        }
      }
    }

    private void KeyFrameContextMenu_Opened(object sender, RoutedEventArgs e)
    {
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ContextMenuRender, "Keyframe Context Menu");
      this.OnPropertyChanged("CanEaseSelectedKeyFrames");
      this.OnPropertyChanged("HoldInSelectedKeyFrames");
      this.OnPropertyChanged("EaseOutSelectedKeyFrames");
      this.OnPropertyChanged("EaseOutCollectionSelectedKeyFrames");
      this.OnPropertyChanged("EaseInSelectedKeyFrames");
      this.OnPropertyChanged("EaseInCollectionSelectedKeyFrames");
      this.OnPropertyChanged("CanCutKeyframes");
      this.OnPropertyChanged("CanCopyKeyframes");
      this.OnPropertyChanged("CanDeleteKeyframes");
    }

    private void CopyKeyframes()
    {
      if (!this.CanCopyKeyframes)
        return;
      new CopyCommand(this.ActiveSceneViewModel).Execute();
    }

    private void CutKeyframes()
    {
      if (!this.CanCutKeyframes)
        return;
      new CutCommand(this.ActiveSceneViewModel).Execute();
    }

    private void DeleteKeyframes()
    {
      if (!this.CanDeleteKeyframes)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.DeleteKeyFramesUndo))
      {
        foreach (KeyFrameSceneNode keyFrameSceneNode in this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection)
          this.ActiveAnimationEditor.DeleteKeyframe(keyFrameSceneNode.TargetElement, keyFrameSceneNode.TargetProperty, keyFrameSceneNode.Time);
        editTransaction.Commit();
      }
    }

    private void Backgrounds_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
      if (frameworkElement == null)
        return;
      TimelineItem timelineItem = frameworkElement.DataContext as TimelineItem;
      if (timelineItem == null)
        return;
      this.ShowTimingContextMenuForTimelineItem(timelineItem, (UIElement) frameworkElement);
    }

    internal static DispatcherTimer CreateAutoScrollTimer()
    {
      return new DispatcherTimer()
      {
        Interval = new TimeSpan(0, 0, 0, 0, 50)
      };
    }

    private void ActiveView_Updated(object sender, EventArgs e)
    {
      if (this.currentPlayingViewStoryboard == null || this.ActiveAnimationEditor != null && this.ActiveAnimationEditor.ActiveViewStoryboard == this.currentPlayingViewStoryboard)
        return;
      this.IsPaused = true;
    }

    private void OnScrubAreaPointerButtonDown(object dc, MouseButtonEventArgs args)
    {
      if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveStoryboardTimeline == null)
        return;
      FrameworkElement frameworkElement = args.OriginalSource as FrameworkElement;
      if (frameworkElement == null)
        return;
      if (!this.IsPaused)
        this.IsPaused = true;
      this.mouseDownPoint = args.MouseDevice.GetPosition((IInputElement) this.FeedbackLayer);
      SeekControlType seekControlType = TimelineView.GetSeekControl((DependencyObject) frameworkElement);
      bool flag = frameworkElement.Name == "TickBarOverlay";
      if ((args.ChangedButton == MouseButton.Left || args.ChangedButton == MouseButton.Right) && ((Keyboard.Modifiers & (ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift)) == ModifierKeys.None && seekControlType == SeekControlType.FullScrub) && !flag)
        this.ActiveSceneViewModel.KeyFrameSelectionSet.Clear();
      if (args.ChangedButton != MouseButton.Left)
        return;
      if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None)
        seekControlType = SeekControlType.FullScrub;
      if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None || !flag && seekControlType == SeekControlType.FullScrub)
      {
        this.marqueeSelectionState = TimelineView.MarqueSelectionState.Initiated;
        this.startMarqueeItem = this.HitTestTimelineItem().HitTimelineItem;
        seekControlType = SeekControlType.NoSeek;
      }
      if (this.marqueeSelectionState != TimelineView.MarqueSelectionState.Initiated && seekControlType != SeekControlType.FullScrub)
        return;
      if (!this.capturing)
      {
        this.capturing = true;
        args.MouseDevice.Capture((IInputElement) (dc as UIElement));
        double num = TimelineView.SecondsFromPosition(args.MouseDevice.GetPosition((IInputElement) this.FeedbackLayer).X);
        if (this.marqueeSelectionState != TimelineView.MarqueSelectionState.Initiated)
        {
          PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.PlayheadDrag);
          this.CurrentTime = num;
        }
        else
        {
          this.startMarqueeTime = num;
          this.endMarqueeTime = this.startMarqueeTime;
        }
        this.autoScrollTimer = TimelineView.CreateAutoScrollTimer();
        this.autoScrollTimer.Tick += new EventHandler(this.AutoScrollTimer_Tick);
        this.autoScrollTimer.Start();
      }
      args.Handled = true;
    }

    private void OnScrubAreaPointerButtonUp(object dc, MouseButtonEventArgs args)
    {
      if (args.ChangedButton != MouseButton.Left)
        return;
      bool flag = false;
      if (this.capturing)
      {
        this.autoScrollTimer.Stop();
        this.autoScrollTimer.Tick -= new EventHandler(this.AutoScrollTimer_Tick);
        this.autoScrollTimer = (DispatcherTimer) null;
        this.capturing = false;
        args.MouseDevice.Capture((IInputElement) null);
        if (this.marqueeSelectionState != TimelineView.MarqueSelectionState.Off)
        {
          try
          {
            if (this.ActiveSceneViewModel == null || this.ActiveAnimationEditor.ActiveStoryboardTimeline == null)
              return;
            if (this.marqueeSelectionState == TimelineView.MarqueSelectionState.On)
            {
              this.TimelinePane.TimelineItemManager.SelectItemAndTimeRange(this.startMarqueeItem, this.HitTestTimelineItem().HitTimelineItem, this.startMarqueeTime, this.endMarqueeTime, (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None);
            }
            else
            {
              if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.None)
              {
                KeyFrameItem hitKeyFrameItem = this.HitTestTimelineItem().HitKeyFrameItem;
                if (hitKeyFrameItem != (KeyFrameItem) null)
                {
                  if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
                    hitKeyFrameItem.ToggleSelectCommand.Execute((object) null);
                  else
                    hitKeyFrameItem.SelectCommand.Execute((object) null);
                }
              }
              if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                flag = true;
            }
          }
          finally
          {
            this.FeedbackLayer.Clear();
            this.startMarqueeItem = (TimelineItem) null;
            this.startMarqueeTime = 0.0;
            this.endMarqueeTime = 0.0;
            this.marqueeSelectionState = TimelineView.MarqueSelectionState.Off;
            this.OnPropertyChanged("LastSignificantTime");
          }
        }
      }
      else
      {
        if (this.ActiveAnimationEditor == null || this.ActiveAnimationEditor.ActiveStoryboardTimeline == null)
          return;
        if (!this.IsPaused)
          this.IsPaused = true;
        FrameworkElement frameworkElement = args.OriginalSource as FrameworkElement;
        if (frameworkElement == null)
          return;
        switch (TimelineView.GetSeekControl((DependencyObject) frameworkElement))
        {
          case SeekControlType.SingleSeek:
            KeyFrameItem keyFrameItem = frameworkElement.DataContext as KeyFrameItem;
            if (keyFrameItem != (KeyFrameItem) null)
            {
              PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.PlayheadDrag);
              TimelineView.SuppressSnapping();
              try
              {
                this.CurrentTime = keyFrameItem.Time;
                break;
              }
              finally
              {
                TimelineView.UnSupressSnapping();
              }
            }
            else
              break;
          case SeekControlType.SingleSeekOnClick:
            if (args.MouseDevice.GetPosition((IInputElement) this.FeedbackLayer) == this.mouseDownPoint)
            {
              flag = true;
              break;
            }
            break;
        }
      }
      if (!flag)
        return;
      Point position = args.MouseDevice.GetPosition((IInputElement) this.FeedbackLayer);
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.PlayheadDrag);
      this.CurrentTime = TimelineView.SecondsFromPosition(position.X);
    }

    private void AutoScrollTimer_Tick(object sender, EventArgs e)
    {
      MouseDevice primaryMouseDevice = InputManager.Current.PrimaryMouseDevice;
      Point position = primaryMouseDevice.GetPosition((IInputElement) this.FeedbackLayer);
      double num = TimelineView.SecondsFromPosition(position.X);
      if (this.marqueeSelectionState == TimelineView.MarqueSelectionState.Off)
      {
        if (this.CurrentTime == num)
          return;
        this.CurrentTime = num;
      }
      else
      {
        if (this.marqueeSelectionState != TimelineView.MarqueSelectionState.On && (Math.Abs(this.mouseDownPoint.X - position.X) >= TimelineView.dragTolerance || Math.Abs(this.mouseDownPoint.Y - position.Y) >= TimelineView.dragTolerance))
          this.marqueeSelectionState = TimelineView.MarqueSelectionState.On;
        if (this.marqueeSelectionState != TimelineView.MarqueSelectionState.On)
          return;
        this.endMarqueeTime = num;
        this.OnPropertyChanged("LastSignificantTime");
        this.FeedbackLayer.BringIntoView(new Rect(position, new Point(position.X + 1.0, position.Y + 1.0)));
        FrameworkElement treeItem = this.TimelinePane.StructureView.ElementTree.ItemContainerGenerator.ContainerFromItem((object) this.HitTestTimelineItem().HitTimelineItem) as FrameworkElement;
        if (treeItem != null)
          this.TimelinePane.StructureView.ScrollIfNeeded(treeItem);
        DrawingContext drawingContext = this.FeedbackLayer.RenderOpen();
        FeedbackHelper.DrawDashedRectangle(drawingContext, 1.0, this.mouseDownPoint, primaryMouseDevice.GetPosition((IInputElement) this.FeedbackLayer));
        drawingContext.Close();
      }
    }

    private void KeyframeRowsScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      this.FeedbackLayer.RenderTransform = (Transform) new TranslateTransform(0.0, -e.VerticalOffset * this.TimelinePane.RowOneHeight);
    }

    private TimelineView.TimelineItemHitTestResult HitTestTimelineItem()
    {
      Point point = InputManager.Current.PrimaryMouseDevice.GetPosition((IInputElement) this.TimelineCanvasBorder);
      point.X = Math.Max(0.0, point.X);
      point.Y = Math.Max(0.0, point.Y);
      point.X = Math.Min(this.TimelineCanvasBorder.ActualWidth, point.X);
      point.Y = Math.Min(this.TimelineCanvasBorder.ActualHeight, point.Y);
      if (point.Y < this.TimelinePane.RowOneHeight)
        point = new Point(point.X, this.TimelinePane.RowOneHeight);
      TimelineItem hitItem = (TimelineItem) null;
      KeyFrameItem hitKeyFrameItem = (KeyFrameItem) null;
      VisualTreeHelper.HitTest((Visual) this.TimelineCanvasBorder, (HitTestFilterCallback) (potentialHitTestTarget =>
      {
        FrameworkElement frameworkElement = potentialHitTestTarget as FrameworkElement;
        return frameworkElement != null && frameworkElement.IsVisible ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
      }), (HitTestResultCallback) (result =>
      {
        FrameworkElement frameworkElement = (FrameworkElement) result.VisualHit;
        if (frameworkElement != null)
        {
          KeyFrameItem keyFrameItem = frameworkElement.DataContext as KeyFrameItem;
          if (keyFrameItem != (KeyFrameItem) null)
            hitKeyFrameItem = keyFrameItem;
          hitItem = frameworkElement.DataContext as TimelineItem;
          if (hitItem != null)
            return HitTestResultBehavior.Stop;
        }
        return HitTestResultBehavior.Continue;
      }), (HitTestParameters) new PointHitTestParameters(point));
      return new TimelineView.TimelineItemHitTestResult(hitItem, hitKeyFrameItem);
    }

    private void ShowTimingContextMenuForTimelineItem(TimelineItem timelineItem, UIElement placementTarget)
    {
      ContextMenu contextMenu = (ContextMenu) null;
      ScheduledTimelineItem scheduledTimelineItem = timelineItem as ScheduledTimelineItem;
      if (scheduledTimelineItem != null)
      {
        ICommand repeatCountCommand = scheduledTimelineItem.EditRepeatCountCommand;
        if (repeatCountCommand != null)
        {
          contextMenu = new ContextMenu();
          MenuItem menuItem = new MenuItem();
          menuItem.Command = repeatCountCommand;
          menuItem.Header = (object) StringTable.EditRepeatCountCommandName;
          menuItem.Name = "EditRepeatCount";
          contextMenu.Items.Add((object) menuItem);
        }
      }
      if (contextMenu == null)
        return;
      contextMenu.PlacementTarget = placementTarget;
      contextMenu.IsOpen = true;
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      try
      {
        if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
          this.OnActiveViewChanged();
        if (this.ActiveSceneViewModel == null)
          return;
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.KeyFrameSelection))
          this.OnPropertyChanged("CanAddExplicitKeyFrame");
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        {
          this.OnPropertyChanged("CanAddExplicitKeyFrame");
          this.OnPropertyChanged("EaseInSelectedKeyFrames");
          this.OnPropertyChanged("EaseOutSelectedKeyFrames");
          this.OnPropertyChanged("HoldInSelectedKeyFrames");
          this.OnPropertyChanged("CanEaseSelectedKeyFrames");
        }
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTimeline))
          this.OnActiveTimelineChanged();
        if (args.IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits.TimelineLastAnimationTime))
          this.OnTimelineLastAnimationTimeChanged();
        if (!args.IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits.AnimationGeneralChange))
          return;
        this.OnAnimationPropertyChanged(args.IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits.AnimationChangeKeyEase));
      }
      finally
      {
        this.seekOnTimelineChange = true;
      }
    }

    private void OnActiveViewChanged()
    {
      if (this.ActiveSceneViewModel != null)
        this.ActiveSceneViewModel.DefaultView.Updated -= new EventHandler(this.ActiveView_Updated);
      SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
      AnimationEditor activeAnimationEditor = this.ActiveAnimationEditor;
      if (activeSceneViewModel != null)
        this.ActiveSceneViewModel.DefaultView.Updated += new EventHandler(this.ActiveView_Updated);
      else
        this.CurrentTime = 0.0;
      this.OnPropertyChanged("LastSignificantTime");
    }

    private void OnAnimationPropertyChanged(bool changeKeyEase)
    {
      this.OnPropertyChanged("CanPlay");
      if (changeKeyEase)
      {
        this.OnPropertyChanged("EaseInSelectedKeyFrames");
        this.OnPropertyChanged("EaseOutSelectedKeyFrames");
        this.OnPropertyChanged("HoldInSelectedKeyFrames");
        this.OnPropertyChanged("CanEaseSelectedKeyFrames");
      }
      this.OnPropertyChanged("LastSignificantTime");
    }

    private void OnStoryboardAddedOrRemoved()
    {
      this.OnPropertyChanged("LastSignificantTime");
    }

    private void OnActiveTimelineChanged()
    {
      if (this.seekOnTimelineChange)
        this.SetCurrentTimeCore(0.0);
      this.OnPropertyChanged("CanKeyFrame");
      this.OnPropertyChanged("CanSeek");
      this.OnPropertyChanged("CanAddExplicitKeyFrame");
      this.OnPropertyChanged("CanPlay");
      this.OnPropertyChanged("LastSignificantTime");
      this.OnPropertyChanged("NeedKeyframeRowsViewer");
    }

    private void OnTimelineLastAnimationTimeChanged()
    {
      this.OnPropertyChanged("LastSignificantTime");
      this.OnPropertyChanged("CanPlay");
      this.OnPropertyChanged("IsPaused");
      this.OnPropertyChanged("CanSeek");
    }

    private void SelectionManager_ActiveSceneSwitched(object sender, EventArgs args)
    {
      this.UpdateCurrentTimeFromActiveScene();
      this.OnPropertyChanged("IsEaseInOutAvailable");
    }

    private void SelectionManager_ActiveSceneSwitching(object sender, EventArgs args)
    {
      if (this.IsPaused)
        return;
      this.IsPaused = true;
    }

    private void UpdateCurrentTimeFromActiveScene()
    {
      if (this.ActiveSceneViewModel == null)
        return;
      this.CurrentTime = this.ActiveSceneViewModel.StoredSeekTime;
      this.seekOnTimelineChange = false;
    }

    private void ApplyKeyFrameEditsCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.EditKeyframe);
      this.EditingKeyFrameItem = (KeyFrameItem) null;
      List<SimpleKeyFrameItem> selectedKeyFrameItems = this.GetSelectedKeyFrameItems();
      if (selectedKeyFrameItems.Count == 0)
        return;
      SimpleKeyFrameItem simpleKeyFrameItem1 = selectedKeyFrameItems[0];
      selectedKeyFrameItems.Sort();
      if (simpleKeyFrameItem1.OldTime < simpleKeyFrameItem1.Time)
        selectedKeyFrameItems.Reverse();
      if (simpleKeyFrameItem1.OldTime == simpleKeyFrameItem1.Time)
      {
        foreach (SimpleKeyFrameItem simpleKeyFrameItem2 in selectedKeyFrameItems)
          simpleKeyFrameItem2.SetTimeFromModel(simpleKeyFrameItem2.Time);
        this.TimelinePane.TimelineItemManager.UpdateItems();
      }
      else
      {
        if (this.ActiveSceneViewModel == null)
          return;
        this.isApplyingKeyFrameEdits = true;
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.MoveKeyFramesUndo))
        {
          this.ActiveAnimationEditor.OffsetSelectedKeyframes(simpleKeyFrameItem1.Time - simpleKeyFrameItem1.OldTime);
          editTransaction.Commit();
        }
        this.isApplyingKeyFrameEdits = false;
      }
    }

    private void EditSelectedKeyFramesCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      double num1 = this.EditingKeyFrameItem.Time - this.EditingKeyFrameItem.OldTime;
      double val1 = 0.0;
      foreach (SimpleKeyFrameItem simpleKeyFrameItem in this.GetSelectedKeyFrameItems())
      {
        if (simpleKeyFrameItem.IsOldTimeSet)
          simpleKeyFrameItem.Time = simpleKeyFrameItem.OldTime + num1;
        else
          simpleKeyFrameItem.Time += num1;
        if (simpleKeyFrameItem.Time < 0.0)
          val1 = Math.Max(val1, -simpleKeyFrameItem.Time);
      }
      if (val1 != 0.0)
      {
        foreach (SimpleKeyFrameItem simpleKeyFrameItem in this.GetSelectedKeyFrameItems())
        {
          double num2 = simpleKeyFrameItem.Time + val1;
          simpleKeyFrameItem.Time = num2;
        }
      }
      this.OnPropertyChanged("LastSignificantTime");
    }

    private void AddExplicitKeyFrameCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitRecordAutoKeyFrame))
      {
        IList<SceneElement> list = (IList<SceneElement>) this.ActiveSceneViewModel.ElementSelectionSet.Selection;
        bool flag = false;
        if (list.Count == 0 && this.ActiveSceneViewModel.ChildPropertySelectionSet.Count != 0 && this.ActiveSceneViewModel.ChildPropertySelectionSet.PrimarySelection is EffectNode)
        {
          list = (IList<SceneElement>) new List<SceneElement>();
          foreach (SceneNode sceneNode in this.ActiveSceneViewModel.ChildPropertySelectionSet.Selection)
          {
            SceneElement sceneElement = sceneNode.Parent as SceneElement;
            if (sceneElement != null)
              list.Add(sceneElement);
          }
        }
        if (list.Count == 0)
        {
          list = (IList<SceneElement>) this.ActiveSceneViewModel.KeyFrameSelectionSet.DerivedTargetElements;
          flag = true;
          this.ActiveSceneViewModel.ClearSelections();
        }
        foreach (SceneElement element in (IEnumerable<SceneElement>) list)
        {
          if (element.IsViewObjectValid)
          {
            this.ActiveAnimationEditor.RecordAutoKeyframe(element, this.ActiveAnimationEditor.AnimationTime);
            if (flag)
            {
              foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) this.ActiveAnimationEditor.ActiveStoryboardTimeline.Children)
              {
                KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode as KeyFrameAnimationSceneNode;
                if (animationSceneNode != null && animationSceneNode.TargetElement == element)
                {
                  KeyFrameSceneNode keyFrameAtTime = animationSceneNode.GetKeyFrameAtTime(this.ActiveAnimationEditor.AnimationTime);
                  if (keyFrameAtTime != null)
                    this.ActiveSceneViewModel.KeyFrameSelectionSet.ExtendSelection(keyFrameAtTime);
                }
              }
            }
          }
        }
        editTransaction.Commit();
      }
    }

    private void SeekBackCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      if (this.IsSnapping)
        this.CurrentTime -= 1.0 / this.PlayStopsPerSecond;
      else
        this.CurrentTime -= 1.0 / TimelineView.unitsPerSecond;
    }

    private void SeekForwardCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      if (this.IsSnapping)
        this.CurrentTime += 1.0 / this.PlayStopsPerSecond;
      else
        this.CurrentTime += 1.0 / TimelineView.unitsPerSecond;
    }

    private void SeekBeginCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      this.CurrentTime = 0.0;
    }

    private void SeekEndCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.RootNode == null)
        return;
      StoryboardTimelineSceneNode storyboardTimeline = this.ActiveAnimationEditor.ActiveStoryboardTimeline;
      if (!double.IsInfinity(storyboardTimeline.PlayDuration))
        this.CurrentTime = storyboardTimeline.PlayDuration;
      else
        this.CurrentTime = storyboardTimeline.NaturalDuration;
    }

    private void ShowSnapResolutionCommandBinding_Execute(object sender, ExecutedRoutedEventArgs e)
    {
      double newFramesPerSecond = 0.0;
      if (!SnapDialog.GetNewSnapFramesPerSecond(this.PlayStopsPerSecond, out newFramesPerSecond))
        return;
      TimelineView.playStopsPerSecond = newFramesPerSecond;
      if (!this.IsSnapping)
        this.IsSnapping = true;
      else
        this.CurrentTime = this.CurrentTime;
    }

    private void ZoomInCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
    }

    private void ZoomOutCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
    }

    private List<SimpleKeyFrameItem> GetSelectedKeyFrameItems()
    {
      List<SimpleKeyFrameItem> list = new List<SimpleKeyFrameItem>();
      if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.KeyFrameSelectionSet != null)
      {
        foreach (KeyFrameSceneNode keyFrameSceneNode in this.ActiveSceneViewModel.KeyFrameSelectionSet.Selection)
        {
          SimpleKeyFrameItem simpleKeyFrameItem = this.TimelinePane.TimelineItemManager.FindSimpleKeyFrameItem(keyFrameSceneNode, this.ActiveAnimationEditor.ActiveStoryboardContainer, this.ActiveAnimationEditor.ActiveStoryboardTimeline);
          if ((KeyFrameItem) simpleKeyFrameItem != (KeyFrameItem) null)
            list.Add(simpleKeyFrameItem);
        }
      }
      return list;
    }

    private static void HandleTimelinePaneChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      TimelineView timelineView = (TimelineView) target;
      if (timelineView.cachedTimelinePane != null)
        return;
      timelineView.cachedTimelinePane = (TimelinePane) e.NewValue;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/timeline/timelineview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (TimelineView) target;
          break;
        case 2:
          this.ButtonFirst = (Button) target;
          break;
        case 3:
          this.ButtonBack = (Button) target;
          break;
        case 4:
          this.ButtonPlay = (TimelinePlayPauseButton) target;
          break;
        case 5:
          this.ButtonForward = (Button) target;
          break;
        case 6:
          this.ButtonLast = (Button) target;
          break;
        case 7:
          this.CurrentTimeTextBox = (StringEditor) target;
          break;
        case 8:
          this.ButtonNewKeyframe = (Button) target;
          break;
        case 9:
          this.TimelineCanvasBorder = (Border) target;
          break;
        case 10:
          this.TimelineCanvas = (Grid) target;
          break;
        case 11:
          this.TimelineRowsViewer = (ScrollViewer) target;
          break;
        case 12:
          this.TimelineRows = (ListBox) target;
          this.TimelineRows.MouseRightButtonUp += new MouseButtonEventHandler(this.Backgrounds_MouseRightButtonUp);
          break;
        case 13:
          this.KeyframeRowsViewer = (ScrollViewer) target;
          break;
        case 14:
          this.KeyframeRows = (ListBox) target;
          this.KeyframeRows.MouseRightButtonUp += new MouseButtonEventHandler(this.Backgrounds_MouseRightButtonUp);
          break;
        case 15:
          this.FeedbackLayer = (FeedbackLayer) target;
          break;
        case 16:
          this.BackgroundViewer = (ScrollViewer) target;
          break;
        case 17:
          this.TimelineBackground = (Grid) target;
          break;
        case 18:
          this.TimeMarkerControl = (TimeMarkerControl) target;
          break;
        case 19:
          this.PlaybackLayer = (Grid) target;
          break;
        case 20:
          this.PlaybackHeadScrollRectangle = (Rectangle) target;
          break;
        case 21:
          this.PlaybackHead = (Image) target;
          break;
        case 22:
          this.PlaybackLine = (Line) target;
          break;
        case 23:
          this.TickBarOverlay = (Border) target;
          break;
        case 24:
          this.HorizontalScroll = (ExtendedScrollBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private enum MarqueSelectionState
    {
      Off,
      Initiated,
      On,
    }

    private struct TimelineItemHitTestResult
    {
      private TimelineItem hitTimelineItem;
      private KeyFrameItem hitKeyFrameItem;

      public TimelineItem HitTimelineItem
      {
        get
        {
          return this.hitTimelineItem;
        }
      }

      public KeyFrameItem HitKeyFrameItem
      {
        get
        {
          return this.hitKeyFrameItem;
        }
      }

      public TimelineItemHitTestResult(TimelineItem hitTimelineItem, KeyFrameItem hitKeyFrameItem)
      {
        this.hitTimelineItem = hitTimelineItem;
        this.hitKeyFrameItem = hitKeyFrameItem;
      }
    }
  }
}
