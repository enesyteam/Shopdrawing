// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimelinePane
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class TimelinePane : Grid, INotifyPropertyChanged, IComponentConnector
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof (Orientation), typeof (TimelinePane), new PropertyMetadata((object) Orientation.Vertical, new PropertyChangedCallback(TimelinePane.HandleOrientationChanged)));
    public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.RegisterAttached("IsAnimating", typeof (bool), typeof (TimelinePane), new PropertyMetadata((object) false, new PropertyChangedCallback(TimelinePane.HandleIsAnimatingChanged)));
    public static readonly DependencyProperty RowOneHeightProperty = DependencyProperty.Register("RowOneHeight", typeof (double), typeof (TimelinePane), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty RenameStoryboardCommandProperty = DependencyProperty.Register("RenameStoryboardCommand", typeof (ICommand), typeof (TimelinePane), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DesiredStructureViewWidthProperty = DependencyProperty.RegisterAttached("DesiredStructureViewWidth", typeof (double), typeof (TimelinePane), (PropertyMetadata) new FrameworkPropertyMetadata((object) 243.0));
    private double minimumTimelineWidth = 120.0;
    private VisibilityToMarginConverter visibilityToMarginConverter = new VisibilityToMarginConverter();
    private ObservableCollection<TimelineHeader> storyboards = new ObservableCollection<TimelineHeader>();
    private List<TimelineHeader> invalidatedHeaders = new List<TimelineHeader>();
    private TimelineHeader defaultTimelineHeader = new TimelineHeader();
    private VerticalScrollManager verticalScrollManager;
    private DesignerContext designerContext;
    private TimelineItem scopedTimelineItem;
    private Thickness originalStructureViewMargin;
    private Thickness originalTimelineViewMargin;
    private SceneNodeSubscription<object, StoryboardTimelineHeader> storyboardsSubscription;
    private SceneNodeSubscription<object, object> focusSubscription;
    private bool needToRebuildTimelineHeaders;
    private bool needToSelectTimelineHeader;
    private bool isRebuildingTimelineHeaders;
    private bool isShowingTimeline;
    private ICollectionView storyboardsView;
    private IDocumentRoot documentRoot;
    private ICommand closeStoryboardCommand;
    private ICommand reverseStoryboardCommand;
    private ICommand duplicateStoryboardCommand;
    private ICommand createNewTimelineCommand;
    private ICommand deleteTimelineCommand;
    private TransitionTimelineHeader transitionTimelineHeader;
    private StoryboardPickerPopup storyboardPopup;
    internal TimelinePane Root;
    internal Grid SplitterGrid;
    internal ColumnDefinition StructureColumn;
    internal ColumnDefinition TimelineColumn;
    internal Grid rowOne;
    internal Grid NormalTimelineUI;
    internal ToggleButton RecordButton;
    internal ActiveStoryboardControl StoryboardNameHost;
    internal Button StoryboardPickerButton;
    internal Canvas StoryboardPopupHost;
    internal Button CloseStoryboardButton;
    internal MenuButton StoryboardSplitButton;
    internal Grid StateTimelineUI;
    internal ToggleButton ShowTimelineButton;
    internal StructureView structureView;
    internal GridSplitter SplitterSlider;
    internal TimelineView timelineView;
    internal Grid VerticalScrollContainer;
    internal ScrollBar VerticalScroll;
    internal Border VerticalScrollCorner;
    private bool _contentLoaded;

    public double RowOneHeight
    {
      get
      {
        return (double) this.GetValue(TimelinePane.RowOneHeightProperty);
      }
      set
      {
        this.SetValue(TimelinePane.RowOneHeightProperty, (object) value);
      }
    }

    public ICommand RenameStoryboardCommand
    {
      get
      {
        return (ICommand) this.GetValue(TimelinePane.RenameStoryboardCommandProperty);
      }
      set
      {
        this.SetValue(TimelinePane.RenameStoryboardCommandProperty, (object) value);
      }
    }

    public ICommand ReverseStoryboardCommand
    {
      get
      {
        if (this.reverseStoryboardCommand == null)
          this.reverseStoryboardCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.ReverseStoryboard));
        return this.reverseStoryboardCommand;
      }
    }

    public bool CanReverseStoryboard
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.storyboardsView != null && this.storyboardsView.CurrentItem != null)
          return this.ActiveSceneViewModel.TransitionEditTarget == null;
        return false;
      }
    }

    public ICommand DuplicateStoryboardCommand
    {
      get
      {
        if (this.duplicateStoryboardCommand == null)
          this.duplicateStoryboardCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DuplicateStoryboard));
        return this.duplicateStoryboardCommand;
      }
    }

    public bool CanDuplicateStoryboard
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.storyboardsView != null && this.storyboardsView.CurrentItem != null)
          return this.ActiveSceneViewModel.TransitionEditTarget == null;
        return false;
      }
    }

    public ICommand CreateNewTimelineCommand
    {
      get
      {
        if (this.createNewTimelineCommand == null)
          this.createNewTimelineCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CreateNewTimeline));
        return this.createNewTimelineCommand;
      }
    }

    public bool CanCreateNewStoryboard
    {
      get
      {
        if (this.HasUnlockedActiveStoryboardContainer && this.HasActiveStoryboardContainer && this.ActiveSceneViewModel.TransitionEditTarget == null)
          return this.ActiveAnimationEditor.ActiveStoryboardContainer.AreResourcesSupported;
        return false;
      }
    }

    public ICommand DeleteTimelineCommand
    {
      get
      {
        if (this.deleteTimelineCommand == null)
          this.deleteTimelineCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.DeleteTimeline));
        return this.deleteTimelineCommand;
      }
    }

    public bool CanShowTimeline
    {
      get
      {
        return this.IsEditingStateStoryboard;
      }
    }

    public TimelineView TimelineView
    {
      get
      {
        return this.timelineView;
      }
    }

    public StructureView StructureView
    {
      get
      {
        return this.structureView;
      }
    }

    public SceneViewModel ActiveSceneViewModel
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.IsEditable && activeSceneViewModel.DefaultView.IsDesignSurfaceEnabled)
          return activeSceneViewModel;
        return (SceneViewModel) null;
      }
    }

    public SceneViewModel ScopeSafeActiveSceneViewModel
    {
      get
      {
        SceneViewModel activeSceneViewModel = this.designerContext.ActiveSceneViewModel;
        if (activeSceneViewModel != null && activeSceneViewModel.XamlDocument != null && (activeSceneViewModel.XamlDocument.IsEditable && activeSceneViewModel.DefaultView.IsDesignSurfaceEnabled))
          return activeSceneViewModel;
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

    public TimelineItemManager TimelineItemManager
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.TimelineItemManager;
        return (TimelineItemManager) null;
      }
    }

    public ReadOnlyObservableCollection<TimelineItem> ItemList
    {
      get
      {
        if (this.TimelineItemManager != null)
          return this.TimelineItemManager.ItemList;
        return (ReadOnlyObservableCollection<TimelineItem>) null;
      }
    }

    public bool HasUnlockedActiveStoryboardContainer
    {
      get
      {
        if (this.HasActiveStoryboardContainer)
          return this.ActiveAnimationEditor.ActiveStoryboardContainer.CanEdit;
        return false;
      }
    }

    public ICollectionView StoryboardsView
    {
      get
      {
        return this.storyboardsView;
      }
    }

    public bool CanDeleteStoryboard
    {
      get
      {
        if (this.storyboardsView.CurrentItem != null && ((TimelineHeader) this.storyboardsView.CurrentItem).CanDelete && this.ActiveAnimationEditor != null)
          return this.ActiveSceneViewModel.TransitionEditTarget == null;
        return false;
      }
    }

    public VerticalScrollManager VerticalScrollManager
    {
      get
      {
        return this.verticalScrollManager;
      }
    }

    internal DesignerContext DesignerContext
    {
      get
      {
        return this.designerContext;
      }
    }

    public Orientation Orientation
    {
      get
      {
        Orientation orientation = Orientation.Vertical;
        ViewPresenter viewPresenter = this.Parent as ViewPresenter;
        if (viewPresenter != null && viewPresenter.ActualHeight / viewPresenter.ActualWidth < 0.5 && viewPresenter.ActualWidth > TimelinePane.GetDesiredStructureViewWidth((DependencyObject) this))
          orientation = Orientation.Horizontal;
        return orientation;
      }
    }

    public bool IsAnimating
    {
      get
      {
        return (bool) this.GetValue(TimelinePane.IsAnimatingProperty);
      }
      set
      {
        this.SetValue(TimelinePane.IsAnimatingProperty, (object) (bool) (value ? true : false));
      }
    }

    public bool IsEditingStateStoryboard
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.StateStoryboardEditTarget != null;
        return false;
      }
    }

    public bool IsEditingTransitionStoryboard
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.TransitionEditTarget != null;
        return false;
      }
    }

    public bool IsRecordingStateStoryboard
    {
      get
      {
        if (this.IsEditingStateStoryboard && this.ActiveAnimationEditor != null)
          return this.ActiveAnimationEditor.IsRecording;
        return false;
      }
      set
      {
        if (this.ActiveAnimationEditor == null)
          return;
        this.ActiveAnimationEditor.IsRecording = value;
      }
    }

    public bool IsShowingTimeline
    {
      get
      {
        return this.isShowingTimeline;
      }
      set
      {
        if (this.IsShowingTimeline == value)
          return;
        this.isShowingTimeline = value;
        this.OnPropertyChanged("IsShowingTimeline");
        this.UpdateIsAnimating();
      }
    }

    public string StateStoryboardDisplayName
    {
      get
      {
        if (this.ActiveSceneViewModel == null || this.ActiveSceneViewModel.StateEditTarget == null)
          return (string) null;
        if (this.ActiveSceneViewModel.StateEditTarget.StateGroup == null || !this.ActiveSceneViewModel.StateEditTarget.StateGroup.IsSketchFlowAnimation)
          return this.ActiveSceneViewModel.StateEditTarget.Name;
        string str = this.ActiveSceneViewModel.StateEditTarget.StateGroup.Name;
        if (str.StartsWith(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter, StringComparison.Ordinal))
          str = str.Substring(VisualStateManagerSceneNode.SketchFlowAnimationXamlDelimiter.Length);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.StateAnimationTimelineFormatString, new object[2]
        {
          (object) str,
          (object) this.ActiveSceneViewModel.StateEditTarget.Description
        });
      }
    }

    public ICommand CloseStoryboardCommand
    {
      get
      {
        if (this.closeStoryboardCommand == null)
          this.closeStoryboardCommand = (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.CloseStoryboard));
        return this.closeStoryboardCommand;
      }
    }

    public bool CanCloseStoryboard
    {
      get
      {
        if (this.ActiveAnimationEditor != null && this.ActiveAnimationEditor.ActiveStoryboardTimeline != null)
          return this.ActiveSceneViewModel.TransitionEditTarget == null;
        return false;
      }
    }

    public TimelineHeader CurrentTimelineHeader
    {
      get
      {
        if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.TransitionEditTarget != null)
          return (TimelineHeader) this.transitionTimelineHeader;
        return (TimelineHeader) (this.storyboardsView.CurrentItem as StoryboardTimelineHeader) ?? this.defaultTimelineHeader;
      }
    }

    public bool StoryboardsExist
    {
      get
      {
        return this.storyboards.Count > 0;
      }
    }

    public bool HasAnythingLocked { get; private set; }

    public bool HasAnythingHidden { get; private set; }

    public string FocusedElementName
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
        {
          SceneElement sceneElement = this.ActiveSceneViewModel.ActiveEditingContainer as SceneElement;
          if (sceneElement != null)
            return sceneElement.ContainerDisplayName;
        }
        return string.Empty;
      }
    }

    public bool HasEditableDocument
    {
      get
      {
        if (this.ActiveSceneViewModel != null)
          return this.ActiveSceneViewModel.IsEditable;
        return false;
      }
    }

    public bool HasActiveStoryboardContainer
    {
      get
      {
        if (this.ActiveAnimationEditor != null)
          return this.ActiveAnimationEditor.ActiveStoryboardContainer != null;
        return false;
      }
    }

    public bool CanReturnScope
    {
      get
      {
        if (this.ScopeSafeActiveSceneViewModel != null)
          return this.ScopeSafeActiveSceneViewModel.CanPopActiveEditingContainer;
        return false;
      }
    }

    public string ReturnScopeToolTip
    {
      get
      {
        if (!this.CanReturnScope)
          return string.Empty;
        SceneElement sceneElement = this.ScopeSafeActiveSceneViewModel.NextActiveNonHiddenParentContext.EditingContainer as SceneElement;
        if (sceneElement == null)
          return StringTable.ScopeToRootToolTip;
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.ReturnScopeToolTip, new object[1]
        {
          (object) sceneElement.ContainerDisplayName
        });
      }
    }

    public TimelineItem ScopedTimelineItem
    {
      get
      {
        return this.scopedTimelineItem;
      }
      set
      {
        if (this.scopedTimelineItem == value)
          return;
        this.scopedTimelineItem = value;
        this.OnPropertyChanged("ScopedTimelineItem");
        this.OnPropertyChanged("CanReturnScope");
        this.OnPropertyChanged("ReturnScopeToolTip");
      }
    }

    public bool IsStoryboardSelected
    {
      get
      {
        if (this.designerContext.SelectionManager != null && this.designerContext.SelectionManager.StoryboardSelectionSet != null)
          return this.designerContext.SelectionManager.StoryboardSelectionSet.Count != 0;
        return false;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal TimelinePane(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.InitializeComponent();
    }

    public static double GetDesiredStructureViewWidth(DependencyObject dependencyObject)
    {
      return (double) dependencyObject.GetValue(TimelinePane.DesiredStructureViewWidthProperty);
    }

    public static void SetDesiredStructureViewWidth(DependencyObject dependencyObject, double value)
    {
      dependencyObject.SetValue(TimelinePane.DesiredStructureViewWidthProperty, (object) value);
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      EventManager.RegisterClassHandler(typeof (Window), Keyboard.KeyDownEvent, (Delegate) new KeyEventHandler(this.EventManager_KeyDownEvent));
      this.Loaded += new RoutedEventHandler(this.HandleLoaded);
      this.storyboardsView = CollectionViewSource.GetDefaultView((object) this.storyboards);
      this.storyboardsView.CurrentChanged += new EventHandler(this.HandleStoryboardSelectionChanged);
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ToggleLockAllCommand, new ExecutedRoutedEventHandler(this.ToggleLockAllCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ToggleShowAllCommand, new ExecutedRoutedEventHandler(this.ToggleShowAllCommandBinding_Execute)));
      this.CommandBindings.Add(new CommandBinding((ICommand) TimelineCommands.ScopeToRootCommand, new ExecutedRoutedEventHandler(this.ScopeToRootCommandBinding_Execute)));
      this.SetBinding(TimelinePane.RenameStoryboardCommandProperty, (BindingBase) new Binding("EditCommand")
      {
        ElementName = "StoryboardNameHost",
        Mode = BindingMode.OneWay
      });
      this.transitionTimelineHeader = new TransitionTimelineHeader(this.designerContext);
      SelectionManager selectionManager = this.designerContext.SelectionManager;
      selectionManager.LateActiveSceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.SelectionManager_LateActiveSceneUpdatePhase);
      selectionManager.ActiveSceneSwitching += new EventHandler(this.SelectionManager_ActiveSceneSwitching);
      this.storyboardsSubscription = new SceneNodeSubscription<object, StoryboardTimelineHeader>();
      this.storyboardsSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(TimelinePane.ShouldExposeStoryboardToUser), SearchScope.NodeTreeSelf), (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(TimelinePane.IsNotStyleOrTemplate), SearchScope.NodeTreeSelf))
      });
      this.storyboardsSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, StoryboardTimelineHeader>.PathNodeInsertedHandler(this.StoryboardsSubscription_TimelineInserted));
      this.storyboardsSubscription.PathNodeRemoved += new SceneNodeSubscription<object, StoryboardTimelineHeader>.PathNodeRemovedListener(this.StoryboardsSubscription_TimelineRemoved);
      this.storyboardsSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, StoryboardTimelineHeader>.PathNodeContentChangedListener(this.StoryboardsSubscription_TimelineContentChanged);
      if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.ActiveStoryboardContainer != null)
        this.SetSubscriptionBasis(this.ActiveSceneViewModel.ActiveStoryboardContainer as SceneNode);
      this.focusSubscription = new SceneNodeSubscription<object, object>();
      this.focusSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.Self)
      });
      this.focusSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, object>.PathNodeContentChangedListener(this.FocusSubscription_ContentChanged);
      if (this.ActiveSceneViewModel != null)
        this.SetFocusSubscriptionBasis(this.ActiveSceneViewModel.ActiveStoryboardContainer as SceneNode);
      this.verticalScrollManager = new VerticalScrollManager();
      this.verticalScrollManager.ScrollBar = this.VerticalScroll;
      this.OnActiveViewChanged();
      if (this.ActiveAnimationEditor != null && this.ActiveAnimationEditor.ActiveStoryboardContainer != null)
        this.SelectTimelineHeader();
      this.SetBinding(TimelinePane.RowOneHeightProperty, (BindingBase) new Binding("ActualHeight")
      {
        Source = (object) this.rowOne
      });
      this.originalStructureViewMargin = this.structureView.Margin;
      this.originalTimelineViewMargin = this.timelineView.Margin;
    }

    public static bool ShouldExposeStoryboardToUser(SceneNode sceneNode)
    {
      StoryboardTimelineSceneNode timelineSceneNode = sceneNode as StoryboardTimelineSceneNode;
      if (timelineSceneNode == null)
        return false;
      ResourceDictionaryNode resources = timelineSceneNode.ViewModel.ActiveStoryboardContainer.Resources;
      if (resources != null && timelineSceneNode.IsInResourceDictionary && timelineSceneNode.Parent != null)
        return timelineSceneNode.Parent.DocumentNode.Parent == resources.DocumentNode;
      return false;
    }

    public static bool IsNotStyleOrTemplate(SceneNode sceneNode)
    {
      if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNode.Type))
        return !PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type);
      return false;
    }

    public static string GetTimelineName(IStoryboardContainer storyboardContainer, string proposedName, out bool createAsResource)
    {
      SceneNode nearestResourceScopeElement = storyboardContainer as SceneNode;
      if (nearestResourceScopeElement != null)
      {
        CreateResourceModel model = new CreateResourceModel(nearestResourceScopeElement.ViewModel, nearestResourceScopeElement.DesignerContext.ResourceManager, typeof (Storyboard), (Type) null, (string) null, storyboardContainer as SceneElement, nearestResourceScopeElement, CreateResourceModel.ContextFlags.CanOnlyDefineKey);
        model.KeyString = proposedName;
        bool? nullable = new CreateResourceDialog(nearestResourceScopeElement.DesignerContext, model).ShowDialog();
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) != 0)
        {
          createAsResource = model.CreateAsResource;
          return model.KeyString;
        }
      }
      createAsResource = false;
      return (string) null;
    }

    private void CreateNewTimeline()
    {
      if (!this.CanCreateNewStoryboard)
        return;
      bool createAsResource;
      string timelineName = TimelinePane.GetTimelineName(this.ActiveAnimationEditor.ActiveStoryboardContainer, this.ActiveAnimationEditor.GenerateNewTimelineName(this.ActiveAnimationEditor.ActiveStoryboardContainer), out createAsResource);
      if (timelineName == null || this.ActiveAnimationEditor == null)
        return;
      this.ActiveAnimationEditor.CreateNewTimeline(this.ActiveAnimationEditor.ActiveStoryboardContainer, timelineName, TriggerCreateBehavior.Default, createAsResource);
    }

    private void ToggleLockAllCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null)
        return;
      if (this.HasLockedChildren(this.ScopedTimelineItem))
        this.SetAllChildrenIsLocked(this.ScopedTimelineItem, false);
      else
        this.SetAllChildrenIsLocked(this.ScopedTimelineItem, true);
    }

    private void ToggleShowAllCommandBinding_Execute(object target, ExecutedRoutedEventArgs args)
    {
      if (this.ActiveSceneViewModel == null)
        return;
      if (this.HasHiddenChildren(this.ScopedTimelineItem))
        this.SetAllChildrenIsShown(this.ScopedTimelineItem, true);
      else
        this.SetAllChildrenIsShown(this.ScopedTimelineItem, false);
    }

    private void ScopeToRootCommandBinding_Execute(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.ScopeSafeActiveSceneViewModel == null || !this.CanReturnScope)
        return;
      this.ScopeSafeActiveSceneViewModel.PopActiveEditingContainer();
    }

    private void OnActiveViewChanged()
    {
      if (this.documentRoot != null)
      {
        this.documentRoot.TypesChanged -= new EventHandler(this.ViewRootValidityChanged);
        this.documentRoot.RootNodeChanged -= new EventHandler(this.ViewRootValidityChanged);
        this.documentRoot = (IDocumentRoot) null;
      }
      if (this.designerContext.ActiveDocument != null)
      {
        this.documentRoot = this.designerContext.ActiveDocument.DocumentRoot;
        if (this.documentRoot != null)
        {
          this.documentRoot.TypesChanged += new EventHandler(this.ViewRootValidityChanged);
          this.documentRoot.RootNodeChanged += new EventHandler(this.ViewRootValidityChanged);
        }
      }
      this.UpdateUIForViewChange();
    }

    private void UpdateUIForViewChange()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.TimelinePaneOnActiveDocumentChanged);
      SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
      AnimationEditor activeAnimationEditor = this.ActiveAnimationEditor;
      if (activeSceneViewModel != null)
      {
        PerformanceUtility.MarkInterimStep(PerformanceEvent.TimelinePaneOnActiveDocumentChanged, "About to find and set root");
        this.ScopedTimelineItem = this.ActiveSceneViewModel.TimelineItemManager.ScopedTimelineItem;
        PerformanceUtility.MarkInterimStep(PerformanceEvent.TimelinePaneOnActiveDocumentChanged, "Found ActiveStoryboardContainer");
        this.OnPropertyChanged("FocusedElementName");
        PerformanceUtility.MarkInterimStep(PerformanceEvent.TimelinePaneOnActiveDocumentChanged, "Updated FocusedElementName");
      }
      else
      {
        this.ScopedTimelineItem = (TimelineItem) null;
        this.OnPropertyChanged("FocusedElementName");
      }
      this.OnPropertyChanged("HasEditableDocument");
      this.OnPropertyChanged("ItemList");
      this.UpdateIsAnimating();
      this.UpdateIsEditingStateStoryboard();
      this.RefreshOnStoryboardChange();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.TimelinePaneOnActiveDocumentChanged);
    }

    private void UpdateIsEditingStateStoryboard()
    {
      this.OnPropertyChanged("IsEditingStateStoryboard");
      this.OnPropertyChanged("IsEditingTransitionStoryboard");
      this.OnPropertyChanged("CanShowTimeline");
      this.OnPropertyChanged("IsRecordingStateStoryboard");
      this.OnPropertyChanged("StateStoryboardDisplayName");
    }

    private void UpdateIsAnimating()
    {
      if (this.ActiveSceneViewModel != null && this.ActiveSceneViewModel.StateStoryboardEditTarget != null)
        this.IsAnimating = this.IsShowingTimeline;
      else
        this.IsAnimating = this.ActiveAnimationEditor != null && this.ActiveAnimationEditor.ActiveStoryboardTimeline != null;
    }

    private void ViewRootValidityChanged(object sender, EventArgs e)
    {
      this.UpdateUIForViewChange();
    }

    private void OnIsEditableChanged()
    {
      this.OnPropertyChanged("HasEditableDocument");
    }

    private void OnTimelineScopedTimelineItemChanged()
    {
      this.ScopedTimelineItem = this.TimelineItemManager.ScopedTimelineItem;
      this.OnPropertyChanged("FocusedElementName");
    }

    private void OnRecordModeChanged()
    {
      foreach (TimelineHeader timelineHeader in (Collection<TimelineHeader>) this.storyboards)
        timelineHeader.UpdateIsRecording();
      this.OnPropertyChanged("IsRecordingStateStoryboard");
    }

    private void EventManager_KeyDownEvent(object sender, KeyEventArgs e)
    {
      if (Keyboard.Modifiers != ModifierKeys.Control || e.Key != Key.R)
        return;
      StoryboardTimelineHeader storyboardTimelineHeader = this.storyboardsView.CurrentItem as StoryboardTimelineHeader;
      if (storyboardTimelineHeader != null && storyboardTimelineHeader.CanRecord)
      {
        storyboardTimelineHeader.IsRecording = !storyboardTimelineHeader.IsRecording;
      }
      else
      {
        if (!this.IsEditingStateStoryboard)
          return;
        this.ActiveSceneViewModel.AnimationEditor.IsRecording = !this.ActiveSceneViewModel.AnimationEditor.IsRecording;
      }
    }

    private void OnActiveTimelineChanged()
    {
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.OnActiveTimelineChanged);
      PerformanceUtility.MarkInterimStep(PerformanceEvent.OnActiveTimelineChanged, "Enabled play controls");
      IStoryboardContainer storyboardContainer = this.ActiveAnimationEditor.ActiveStoryboardContainer;
      PerformanceUtility.MarkInterimStep(PerformanceEvent.OnActiveTimelineChanged, "Got active storyboard element");
      this.OnPropertyChanged("FocusedElementName");
      PerformanceUtility.MarkInterimStep(PerformanceEvent.OnActiveTimelineChanged, "Updated FocusedElementName");
      this.needToSelectTimelineHeader = true;
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.OnActiveTimelineChanged);
      this.OnPropertyChanged("HasUnlockedActiveStoryboardContainer");
      this.OnPropertyChanged("HasActiveStoryboardContainer");
      this.OnPropertyChanged("CanCreateNewStoryboard");
      this.UpdateIsAnimating();
      this.UpdateIsEditingStateStoryboard();
      this.OnRecordModeChanged();
    }

    private void MorphForTimelineChange()
    {
      Binding binding = new Binding("ActualWidth");
      binding.Mode = BindingMode.OneWay;
      binding.Source = (object) this;
      if (this.IsAnimating || this.Orientation == Orientation.Horizontal)
        binding.Converter = (IValueConverter) new AdditionConverter()
        {
          Offset = -this.StructureColumn.MinWidth
        };
      this.StructureColumn.SetBinding(ColumnDefinition.MaxWidthProperty, (BindingBase) binding);
      if (this.IsAnimating)
      {
        this.TimelineView.Visibility = Visibility.Visible;
        this.SplitterSlider.Visibility = Visibility.Visible;
        this.VerticalScrollCorner.Visibility = Visibility.Visible;
        this.TimelineColumn.MaxWidth = double.PositiveInfinity;
        Thickness margin = this.StructureView.Margin;
        margin.Right = 0.0;
        this.StructureView.Margin = margin;
        this.visibilityToMarginConverter.Left = this.originalTimelineViewMargin.Left;
        this.visibilityToMarginConverter.Top = this.originalTimelineViewMargin.Top;
        this.visibilityToMarginConverter.Right = this.VerticalScrollContainer.ActualWidth - 1.0;
        this.visibilityToMarginConverter.Bottom = this.originalTimelineViewMargin.Bottom;
        this.visibilityToMarginConverter.TargetSubProperty = MarginSubProperty.Right;
        this.TimelineView.SetBinding(FrameworkElement.MarginProperty, (BindingBase) new Binding("Visibility")
        {
          Source = (object) this.VerticalScrollContainer,
          Mode = BindingMode.OneWay,
          Converter = (IValueConverter) this.visibilityToMarginConverter
        });
        if (this.Orientation == Orientation.Vertical)
        {
          BindingOperations.SetBinding((DependencyObject) this.StructureColumn, ColumnDefinition.WidthProperty, (BindingBase) new Binding("ActualWidth")
          {
            Mode = BindingMode.OneWay,
            Source = (object) this,
            Converter = (IValueConverter) new IntervalClampingOffsetConverter()
            {
              Offset = (-this.VerticalScrollContainer.ActualWidth - this.minimumTimelineWidth),
              MinValue = 30.0,
              MaxValue = TimelinePane.GetDesiredStructureViewWidth((DependencyObject) this)
            }
          });
        }
        else
        {
          double pixels = this.StructureColumn.ActualWidth;
          if (pixels >= this.ActualWidth - this.StructureColumn.MinWidth)
            pixels = TimelinePane.GetDesiredStructureViewWidth((DependencyObject) this);
          this.StructureColumn.Width = new GridLength(pixels);
        }
      }
      else
      {
        this.TimelineView.Visibility = Visibility.Collapsed;
        if (this.Orientation == Orientation.Vertical)
        {
          this.SplitterSlider.Visibility = Visibility.Collapsed;
          this.VerticalScrollCorner.Visibility = Visibility.Visible;
          this.TimelineColumn.MaxWidth = 0.0;
          this.visibilityToMarginConverter.Left = this.originalStructureViewMargin.Left;
          this.visibilityToMarginConverter.Top = this.originalStructureViewMargin.Top;
          this.visibilityToMarginConverter.Right = this.VerticalScrollContainer.ActualWidth - 1.0;
          this.visibilityToMarginConverter.Bottom = this.originalStructureViewMargin.Bottom;
          this.visibilityToMarginConverter.TargetSubProperty = MarginSubProperty.Right;
          this.StructureView.SetBinding(FrameworkElement.MarginProperty, (BindingBase) new Binding("Visibility")
          {
            Source = (object) this.VerticalScrollContainer,
            Mode = BindingMode.OneWay,
            Converter = (IValueConverter) this.visibilityToMarginConverter
          });
          BindingOperations.ClearBinding((DependencyObject) this.StructureColumn, ColumnDefinition.WidthProperty);
          this.StructureColumn.Width = new GridLength(1.0, GridUnitType.Star);
        }
        else
        {
          this.VerticalScrollCorner.Visibility = Visibility.Hidden;
          double pixels = this.StructureColumn.ActualWidth;
          if (pixels >= this.ActualWidth - this.StructureColumn.MinWidth)
            pixels = TimelinePane.GetDesiredStructureViewWidth((DependencyObject) this);
          if (BindingOperations.GetBindingBase((DependencyObject) this.StructureColumn, ColumnDefinition.WidthProperty) != null)
            BindingOperations.ClearBinding((DependencyObject) this.StructureColumn, ColumnDefinition.WidthProperty);
          if (pixels != 0.0)
            this.StructureColumn.Width = new GridLength(pixels);
          Thickness margin = this.StructureView.Margin;
          margin.Right = 0.0;
          this.StructureView.Margin = margin;
          this.SplitterSlider.Visibility = Visibility.Visible;
          this.TimelineColumn.MaxWidth = double.PositiveInfinity;
        }
      }
    }

    private void HandleLoaded(object sender, EventArgs e)
    {
      this.MorphForTimelineChange();
    }

    private static void HandleOrientationChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      ((TimelinePane) target).MorphForTimelineChange();
    }

    private static void HandleIsAnimatingChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      ((TimelinePane) target).MorphForTimelineChange();
    }

    private void HandleStoryboardSelectionChanged(object sender, EventArgs e)
    {
      if (this.ActiveSceneViewModel == null || this.isRebuildingTimelineHeaders)
      {
        this.RefreshOnStoryboardChange();
      }
      else
      {
        TimelineHeader timelineHeader = (TimelineHeader) this.storyboardsView.CurrentItem;
        if (timelineHeader != null)
        {
          StoryboardTimelineSceneNode timeline = timelineHeader.Timeline;
          if (timeline != null)
          {
            if (timeline != this.ActiveAnimationEditor.ActiveStoryboardTimeline)
              this.ActiveAnimationEditor.SetActiveStoryboardTimeline(this.ActiveAnimationEditor.ActiveStoryboardContainer, timeline);
          }
          else
          {
            SceneElementSelectionSet elementSelectionSet = this.ActiveSceneViewModel.ElementSelectionSet;
            this.ActiveAnimationEditor.SetActiveStoryboardTimeline(this.TimelineItemManager.ViewModel.ActiveStoryboardContainer, (StoryboardTimelineSceneNode) null);
          }
        }
        if (this.storyboardPopup != null)
          this.storyboardPopup.IsOpen = false;
        this.RefreshOnStoryboardChange();
      }
    }

    private void RefreshOnStoryboardChange()
    {
      this.OnPropertyChanged("CanDeleteStoryboard");
      this.OnPropertyChanged("CanCloseStoryboard");
      this.OnPropertyChanged("CanReverseStoryboard");
      this.OnPropertyChanged("CanDuplicateStoryboard");
      this.OnPropertyChanged("CurrentTimelineHeader");
      this.OnPropertyChanged("IsStoryboardSelected");
      this.UpdateCurrentTimelineHeader();
    }

    private void UpdateCurrentTimelineHeader()
    {
      if (this.CurrentTimelineHeader == null)
        return;
      this.CurrentTimelineHeader.Update();
    }

    private void ReverseStoryboard()
    {
      if (!this.CanReverseStoryboard)
        return;
      this.CloseStoryboardPickerPopup();
      StoryboardTimelineSceneNode timeline = this.CurrentTimelineHeader.Timeline;
      if (timeline == null)
        return;
      double animationTime = this.ActiveAnimationEditor.AnimationTime;
      this.ActiveAnimationEditor.SeekTo(0.0);
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitReverseStoryboard))
      {
        double activeDuration = timeline.ActiveDuration;
        this.ActiveSceneViewModel.KeyFrameSelectionSet.Clear();
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) timeline.Children)
        {
          KeyFrameAnimationSceneNode animationSceneNode = timelineSceneNode as KeyFrameAnimationSceneNode;
          if (animationSceneNode != null)
            animationSceneNode.ReverseAllKeyFrames(new double?(activeDuration));
        }
        editTransaction.Commit();
      }
      this.ActiveAnimationEditor.SeekTo(animationTime);
    }

    private void DuplicateStoryboard()
    {
      if (!this.CanDuplicateStoryboard)
        return;
      StoryboardTimelineSceneNode newStoryboard = (StoryboardTimelineSceneNode) null;
      this.CloseStoryboardPickerPopup();
      StoryboardTimelineSceneNode timeline = this.CurrentTimelineHeader.Timeline;
      if (timeline != null)
      {
        using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(StringTable.UndoUnitDuplicateStoryboard))
        {
          this.ActiveSceneViewModel.StoryboardSelectionSet.Clear();
          this.ActiveSceneViewModel.KeyFrameSelectionSet.Clear();
          newStoryboard = this.ActiveSceneViewModel.GetSceneNode(timeline.DocumentNode.Clone(timeline.DocumentContext)) as StoryboardTimelineSceneNode;
          if (newStoryboard != null)
          {
            ((SceneNode) newStoryboard).Name = (string) null;
            string suggestion = timeline.Name;
            if (!string.IsNullOrEmpty(suggestion))
            {
              int val1 = suggestion.Length - 1;
              while (val1 >= 0 && char.IsDigit(suggestion[val1]))
                --val1;
              int num1 = 512 - StringTable.StoryboardCopySuffix.Length - int.MaxValue.ToString((IFormatProvider) CultureInfo.InvariantCulture).Length;
              int num2 = Math.Min(val1, num1 - 1);
              if (num2 > 0)
                suggestion = suggestion.Substring(0, num2 + 1);
              if (!suggestion.EndsWith(StringTable.StoryboardCopySuffix, StringComparison.Ordinal))
                suggestion += StringTable.StoryboardCopySuffix;
            }
            else
              suggestion = StringTable.StoryboardCopySuffix;
            this.ActiveAnimationEditor.AddResourceStoryboard(this.ActiveAnimationEditor.GenerateNewTimelineName(this.ActiveAnimationEditor.ActiveStoryboardContainer, suggestion), newStoryboard);
          }
          editTransaction.Commit();
        }
      }
      this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (o =>
      {
        this.SelectTimelineHeader(newStoryboard);
        this.storyboardsView.Refresh();
        return (object) null;
      }), (object) null);
    }

    public void CloseStoryboard()
    {
      if (this.TimelineItemManager == null)
        return;
      this.ActiveAnimationEditor.SetActiveStoryboardTimeline(this.TimelineItemManager.ViewModel.ActiveStoryboardContainer, (StoryboardTimelineSceneNode) null);
      this.storyboardsView.MoveCurrentTo((object) null);
    }

    internal void CloseStoryboardPickerPopup()
    {
      if (this.storyboardPopup == null)
        return;
      this.storyboardPopup.IsOpen = false;
    }

    private void DeleteTimeline()
    {
      if (!this.CanDeleteStoryboard)
        return;
      TimelineHeader timelineHeader = (TimelineHeader) this.storyboardsView.CurrentItem;
      if (timelineHeader.Timeline == null)
        return;
      this.ActiveAnimationEditor.DeleteTimeline(timelineHeader.Timeline);
    }

    private void SelectTimelineHeader(StoryboardTimelineSceneNode storyboard)
    {
      this.isRebuildingTimelineHeaders = true;
      if (storyboard != null)
      {
        foreach (TimelineHeader timelineHeader in (Collection<TimelineHeader>) this.storyboards)
        {
          if (timelineHeader.Timeline == storyboard)
          {
            this.storyboardsView.MoveCurrentTo((object) timelineHeader);
            break;
          }
        }
      }
      else
        this.storyboardsView.MoveCurrentTo((object) null);
      this.RefreshOnStoryboardChange();
      this.isRebuildingTimelineHeaders = false;
    }

    private void SelectTimelineHeader()
    {
      if (this.ActiveAnimationEditor.ActiveStoryboardTimeline != null && !this.IsEditingStateStoryboard)
        this.SelectTimelineHeader(this.ActiveAnimationEditor.ActiveStoryboardTimeline);
      else
        this.SelectTimelineHeader((StoryboardTimelineSceneNode) null);
    }

    private void OnPropertyReferenceChanged(PropertyReferenceSceneChange e)
    {
      if (e.PropertyChanged == null || e.Target == null || e.PropertyChanged.Count <= 0)
        return;
      IPropertyId propertyId = (IPropertyId) e.PropertyChanged[e.PropertyChanged.Count - 1];
      if (DesignTimeProperties.IsLockedProperty.Equals((object) propertyId))
      {
        if (this.ActiveAnimationEditor.ActiveStoryboardContainer != null && e.Target == (SceneNode) this.ActiveAnimationEditor.ActiveStoryboardContainer)
        {
          this.needToRebuildTimelineHeaders = true;
          this.needToSelectTimelineHeader = true;
        }
        this.OnPropertyChanged("HasUnlockedActiveStoryboardContainer");
        this.OnPropertyChanged("CanCreateNewStoryboard");
      }
      if (this.ActiveSceneViewModel == null)
        return;
      SceneNode sceneNode = (SceneNode) this.ActiveSceneViewModel.StateEditTarget;
      if (sceneNode == null || !propertyId.Equals((object) sceneNode.NameProperty))
        return;
      this.OnPropertyChanged("StateStoryboardDisplayName");
    }

    private void SelectionManager_LateActiveSceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      this.needToRebuildTimelineHeaders = false;
      this.needToSelectTimelineHeader = false;
      if (args.IsRadicalChange || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
      {
        this.OnActiveViewChanged();
        SceneViewModel currentViewModel = this.storyboardsSubscription.CurrentViewModel;
        SceneViewModel activeSceneViewModel = this.ActiveSceneViewModel;
        this.storyboardsSubscription.CurrentViewModel = activeSceneViewModel;
        if (currentViewModel == null && activeSceneViewModel != null)
        {
          this.SetSubscriptionBasis(activeSceneViewModel.ActiveStoryboardContainer as SceneNode);
          this.SetFocusSubscriptionBasis(activeSceneViewModel.ActiveStoryboardContainer as SceneNode);
        }
        this.needToRebuildTimelineHeaders = true;
      }
      if (this.ActiveSceneViewModel == null)
      {
        this.focusSubscription.CurrentViewModel = (SceneViewModel) null;
        this.storyboardsSubscription.CurrentViewModel = (SceneViewModel) null;
      }
      else
      {
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable))
          this.OnIsEditableChanged();
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer))
        {
          this.SetSubscriptionBasis(this.ActiveSceneViewModel.ActiveStoryboardContainer as SceneNode);
          this.SetFocusSubscriptionBasis(this.ActiveSceneViewModel.ActiveStoryboardContainer as SceneNode);
          this.OnPropertyChanged("CanCreateNewStoryboard");
        }
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTimeline))
          this.OnActiveTimelineChanged();
        if (args.IsDirtyPipelineCalcState(SceneViewModel.PipelineCalcBits.TimelineScopedTimelineItem))
          this.OnTimelineScopedTimelineItemChanged();
        if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.RecordMode))
          this.OnRecordModeChanged();
        if (args.DocumentChanges.Count > 0)
        {
          this.focusSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
          bool flag1 = false;
          bool flag2 = false;
          foreach (PropertySceneChange propertySceneChange1 in SceneChange.ChangesOfType<PropertySceneChange>(args.DocumentChanges, (SceneNode) null))
          {
            PropertyReferenceSceneChange e = propertySceneChange1 as PropertyReferenceSceneChange;
            if (e != null)
            {
              this.OnPropertyReferenceChanged(e);
            }
            else
            {
              DesignTimePropertySceneChange propertySceneChange2 = propertySceneChange1 as DesignTimePropertySceneChange;
              if (propertySceneChange2 != null)
              {
                IPropertyId designTimePropertyKey = propertySceneChange2.DesignTimePropertyKey;
                flag1 |= DesignTimeProperties.IsLockedProperty.Equals((object) designTimePropertyKey);
                flag2 |= DesignTimeProperties.IsHiddenProperty.Equals((object) designTimePropertyKey);
                if (propertySceneChange1.PropertyKey.Equals((object) propertySceneChange1.Target.NameProperty))
                  this.OnPropertyChanged("StateStoryboardDisplayName");
              }
            }
          }
          if (flag1)
          {
            this.HasAnythingLocked = this.TimelineItemManager.ComputeHasAnythingLocked();
            this.OnPropertyChanged("HasAnythingLocked");
          }
          if (flag2)
          {
            this.HasAnythingHidden = this.TimelineItemManager.ComputeHasAnythingHidden();
            this.OnPropertyChanged("HasAnythingHidden");
          }
        }
        this.storyboardsSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
        if (this.invalidatedHeaders.Count > 0)
        {
          for (int index = 0; index < this.invalidatedHeaders.Count; ++index)
            this.invalidatedHeaders[index].Update();
          this.invalidatedHeaders.Clear();
        }
        if (this.needToRebuildTimelineHeaders)
          this.needToSelectTimelineHeader = true;
        if (this.needToSelectTimelineHeader)
          this.SelectTimelineHeader();
        if (!args.IsDirtyViewState(SceneViewModel.ViewStateBits.StoryboardSelection))
          return;
        this.OnPropertyChanged("IsStoryboardSelected");
        this.UpdateCurrentTimelineHeader();
      }
    }

    private void SelectionManager_ActiveSceneSwitching(object sender, EventArgs e)
    {
      this.ScopedTimelineItem = (TimelineItem) null;
      this.UpdateUIForViewChange();
    }

    private StoryboardTimelineHeader StoryboardsSubscription_TimelineInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      StoryboardTimelineHeader storyboardTimelineHeader = new StoryboardTimelineHeader((StoryboardTimelineSceneNode) newPathNode);
      this.storyboards.Add((TimelineHeader) storyboardTimelineHeader);
      this.OnPropertyChanged("StoryboardsExist");
      return storyboardTimelineHeader;
    }

    private void StoryboardsSubscription_TimelineRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, StoryboardTimelineHeader oldContent)
    {
      if (oldContent == this.storyboardsView.CurrentItem)
        this.storyboardsView.MoveCurrentTo((object) null);
      this.storyboards.Remove((TimelineHeader) oldContent);
      this.OnPropertyChanged("StoryboardsExist");
    }

    private void StoryboardsSubscription_TimelineContentChanged(object sender, SceneNode timelineNode, StoryboardTimelineHeader oldContent, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (this.invalidatedHeaders.Contains((TimelineHeader) oldContent))
        return;
      this.invalidatedHeaders.Add((TimelineHeader) oldContent);
    }

    private void FocusSubscription_ContentChanged(object sender, SceneNode timelineNode, object oldContent, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      if (damageMarker.Property == null || !BaseFrameworkElement.FENameProperty.Equals((object) damageMarker.Property) && !DictionaryEntryNode.KeyProperty.Equals((object) damageMarker.Property) && (!DesignTimeProperties.XNameProperty.Equals((object) damageMarker.Property) && !ControlTemplateElement.TargetTypeProperty.Equals((object) damageMarker.Property)) && !StyleNode.TargetTypeProperty.Equals((object) damageMarker.Property))
        return;
      this.OnPropertyChanged("FocusedElementName");
    }

    public bool GetRepeatCount(ref double repeatCount)
    {
      RepeatDialog repeatDialog = new RepeatDialog();
      repeatDialog.RepeatCount = repeatCount;
      bool? nullable = repeatDialog.ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? true : false)) == 0)
        return false;
      repeatCount = repeatDialog.RepeatCount;
      return true;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private bool HasLockedChildren(TimelineItem timelineItem)
    {
      foreach (TimelineItem timelineItem1 in (Collection<TimelineItem>) timelineItem.Children)
      {
        ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
        if (elementTimelineItem != null && elementTimelineItem.IsLocked || this.HasLockedChildren(timelineItem1))
          return true;
      }
      return false;
    }

    private bool HasHiddenChildren(TimelineItem timelineItem)
    {
      foreach (TimelineItem timelineItem1 in (Collection<TimelineItem>) timelineItem.Children)
      {
        ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
        if (elementTimelineItem != null && !elementTimelineItem.IsShown || this.HasHiddenChildren(timelineItem1))
          return true;
      }
      return false;
    }

    private void SetAllChildrenIsLocked(TimelineItem timelineItem, bool isLocked)
    {
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(isLocked ? StringTable.UndoUnitLockAll : StringTable.UndoUnitUnlockAll))
      {
        foreach (TimelineItem timelineItem1 in (Collection<TimelineItem>) timelineItem.Children)
        {
          ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
          if (elementTimelineItem != null)
            elementTimelineItem.IsLocked = isLocked;
          this.SetAllChildrenIsLocked(timelineItem1, isLocked);
        }
        editTransaction.Commit();
      }
    }

    private void SetAllChildrenIsShown(TimelineItem timelineItem, bool isShown)
    {
      using (SceneEditTransaction editTransaction = this.ActiveSceneViewModel.CreateEditTransaction(isShown ? StringTable.UndoUnitShowAll : StringTable.UndoUnitHideAll))
      {
        foreach (TimelineItem timelineItem1 in (Collection<TimelineItem>) timelineItem.Children)
        {
          ElementTimelineItem elementTimelineItem = timelineItem1 as ElementTimelineItem;
          if (elementTimelineItem != null)
            elementTimelineItem.IsShown = isShown;
          this.SetAllChildrenIsShown(timelineItem1, isShown);
        }
        editTransaction.Commit();
      }
    }

    private void SetSubscriptionBasis(SceneNode sceneNode)
    {
      List<SceneNode> list = new List<SceneNode>();
      SceneViewModel viewModel = (SceneViewModel) null;
      if (sceneNode != null)
      {
        list.Add(sceneNode);
        viewModel = sceneNode.ViewModel;
      }
      this.storyboardsSubscription.SetBasisNodes(viewModel, (IEnumerable<SceneNode>) list);
    }

    private void SetFocusSubscriptionBasis(SceneNode focusElement)
    {
      List<SceneNode> list = new List<SceneNode>();
      SceneViewModel viewModel = (SceneViewModel) null;
      SceneNode sceneNode = (SceneNode) null;
      SceneElement sceneElement = focusElement as SceneElement;
      if (sceneElement != null)
        sceneNode = sceneElement.FindNameBaseNode;
      if (sceneNode != null)
      {
        list.Add(sceneNode);
        viewModel = sceneNode.ViewModel;
      }
      this.focusSubscription.SetBasisNodes(viewModel, (IEnumerable<SceneNode>) list);
    }

    private void StoryboardPickerButtonClick(object target, RoutedEventArgs args)
    {
      this.OpenStoryboardPicker(args.Source as FrameworkElement);
      this.StoryboardPickerButton.IsHitTestVisible = false;
    }

    private void SelectOpenStoryboard()
    {
      if (this.IsStoryboardSelected)
        return;
      bool flag = false;
      if (this.IsEditingStateStoryboard)
      {
        this.designerContext.SelectionManager.StoryboardSelectionSet.SetSelection(this.ActiveSceneViewModel.StateStoryboardEditTarget);
        flag = true;
      }
      else if (this.IsEditingTransitionStoryboard)
      {
        this.designerContext.SelectionManager.StoryboardSelectionSet.SetSelection(this.ActiveSceneViewModel.TransitionStoryboardEditTarget);
        flag = true;
      }
      else if (this.CurrentTimelineHeader != this.defaultTimelineHeader)
      {
        this.designerContext.SelectionManager.StoryboardSelectionSet.SetSelection(this.CurrentTimelineHeader.Timeline);
        flag = true;
      }
      if (!flag)
        return;
      this.OnPropertyChanged("IsStoryboardSelected");
      this.UpdateCurrentTimelineHeader();
    }

    private void StoryboardName_MouseDown(object target, RoutedEventArgs args)
    {
      if (!this.IsStoryboardSelected)
        this.SelectOpenStoryboard();
      else
        Keyboard.Focus((IInputElement) null);
    }

    public void OpenStoryboardPicker(FrameworkElement target)
    {
      if (this.storyboardPopup == null)
      {
        Canvas canvas = target.FindName("StoryboardPopupHost") as Canvas;
        canvas.Children.Clear();
        this.storyboardPopup = new StoryboardPickerPopup(new StoryboardPicker(this), target, this.DesignerContext.Configuration);
        canvas.Children.Add((UIElement) this.storyboardPopup);
        this.storyboardPopup.Closed += new EventHandler(this.storyboardPopup_Closed);
      }
      this.StoryboardsView.Refresh();
      this.storyboardPopup.IsOpen = true;
    }

    private void storyboardPopup_Closed(object sender, EventArgs e)
    {
      this.StoryboardPickerButton.IsHitTestVisible = true;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/resources/timeline/timeline.xaml", UriKind.Relative));
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
          this.Root = (TimelinePane) target;
          break;
        case 2:
          this.SplitterGrid = (Grid) target;
          break;
        case 3:
          this.StructureColumn = (ColumnDefinition) target;
          break;
        case 4:
          this.TimelineColumn = (ColumnDefinition) target;
          break;
        case 5:
          this.rowOne = (Grid) target;
          break;
        case 6:
          this.NormalTimelineUI = (Grid) target;
          break;
        case 7:
          this.RecordButton = (ToggleButton) target;
          break;
        case 8:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.StoryboardName_MouseDown);
          break;
        case 9:
          this.StoryboardNameHost = (ActiveStoryboardControl) target;
          break;
        case 10:
          this.StoryboardPickerButton = (Button) target;
          this.StoryboardPickerButton.Click += new RoutedEventHandler(this.StoryboardPickerButtonClick);
          break;
        case 11:
          this.StoryboardPopupHost = (Canvas) target;
          break;
        case 12:
          this.CloseStoryboardButton = (Button) target;
          break;
        case 13:
          this.StoryboardSplitButton = (MenuButton) target;
          break;
        case 14:
          this.StateTimelineUI = (Grid) target;
          break;
        case 15:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.StoryboardName_MouseDown);
          break;
        case 16:
          this.ShowTimelineButton = (ToggleButton) target;
          break;
        case 17:
          this.structureView = (StructureView) target;
          break;
        case 18:
          this.SplitterSlider = (GridSplitter) target;
          break;
        case 19:
          this.timelineView = (TimelineView) target;
          break;
        case 20:
          this.VerticalScrollContainer = (Grid) target;
          break;
        case 21:
          this.VerticalScroll = (ScrollBar) target;
          break;
        case 22:
          this.VerticalScrollCorner = (Border) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
