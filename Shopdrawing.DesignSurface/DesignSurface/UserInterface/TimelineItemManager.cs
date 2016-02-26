// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.TimelineItemManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.SceneNodeSearch;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class TimelineItemManager : ITreeViewItemProvider<TimelineItem>, INotifyPropertyChanged, IDisposable
  {
    private static string[] canonicalTransformNames = new string[4]
    {
      "Scale",
      "Skew",
      "Rotation",
      "Translation"
    };
    private static string[] canonicalTransform3DNames = new string[5]
    {
      "Center1",
      "Scale",
      "Rotation",
      "Center2",
      "Translation"
    };
    private static ITypeId[] canonicalTransformTypes = new ITypeId[4]
    {
      PlatformTypes.ScaleTransform,
      PlatformTypes.SkewTransform,
      PlatformTypes.RotateTransform,
      PlatformTypes.TranslateTransform
    };
    private static ITypeId[] canonicalTransform3DTypes = new ITypeId[5]
    {
      PlatformTypes.TranslateTransform3D,
      PlatformTypes.ScaleTransform3D,
      PlatformTypes.RotateTransform3D,
      PlatformTypes.TranslateTransform3D,
      PlatformTypes.TranslateTransform3D
    };
    private bool? sortByZOrder = new bool?();
    private bool transitionStatusDirty = true;
    private DocumentNodeMarkerSortedList lastDependencyObjectSelectionSet = new DocumentNodeMarkerSortedList();
    private DocumentNodeMarkerSortedList lastSceneElementSelectionSet = new DocumentNodeMarkerSortedList();
    private DocumentNodeMarkerSortedList lastKeyFrameSelectionSet = new DocumentNodeMarkerSortedList();
    private DocumentNodeMarkerSortedList lastAnimationSelectionSet = new DocumentNodeMarkerSortedList();
    private DocumentNodeMarkerSortedList lastBehaviorSelectionSet = new DocumentNodeMarkerSortedList();
    private DocumentNodeMarkerSortedList lastChildPropertySelectionSet = new DocumentNodeMarkerSortedList();
    private SceneViewModel viewModel;
    private DesignerContext designerContext;
    private TimelineItem activeRootTimelineItem;
    private double lastAnimationTime;
    private bool isLastAnimationTimeDirty;
    private TimelineItem scopedTimelineItem;
    private TimelinePane timelinePane;
    private VirtualizingTreeItemFlattener<TimelineItem> flattener;
    private DocumentNodeMarkerSortedListOf<TimelineItem> timelineItems;
    private PropertySceneInsertionPointMarker lastSceneInsertionPointMarker;
    private SceneNodeSubscription<object, ScheduledTimelineItem> timelineSubscription;
    private double viewportWidth;
    private double viewportOffset;
    private bool isInitializing;
    private bool isInValidationPass;
    private ISceneInsertionPoint hoverOverrideInsertionPoint;
    private TimelineItem hoverOverrideSelectionItem;

    TimelineItem ITreeViewItemProvider<TimelineItem>.RootItem
    {
      get
      {
        return this.ScopedTimelineItem;
      }
    }

    public bool IsInitializing
    {
      get
      {
        return this.isInitializing;
      }
    }

    public ReadOnlyObservableCollection<TimelineItem> ItemList
    {
      get
      {
        return this.flattener.ItemList;
      }
    }

    public SceneDocument Document
    {
      get
      {
        return this.viewModel.Document;
      }
    }

    public SceneViewModel ViewModel
    {
      get
      {
        return this.viewModel;
      }
    }

    public AnimationEditor AnimationEditor
    {
      get
      {
        return this.ViewModel.AnimationEditor;
      }
    }

    public TimelinePane TimelinePane
    {
      get
      {
        if (this.timelinePane == null)
          this.timelinePane = (TimelinePane) this.designerContext.WindowService.PaletteRegistry["Designer_TimelinePane"].Content;
        return this.timelinePane;
      }
    }

    public double LastAnimationTime
    {
      get
      {
        if (this.isLastAnimationTimeDirty)
        {
          this.lastAnimationTime = 0.0;
          if (this.ScopedTimelineItem != null)
            this.lastAnimationTime = this.RecalculateLastAnimationTime(this.ScopedTimelineItem);
          this.isLastAnimationTimeDirty = false;
        }
        return this.lastAnimationTime;
      }
    }

    public bool SortByZOrder
    {
      get
      {
        if (this.designerContext.Configuration != null && !this.sortByZOrder.HasValue)
          this.sortByZOrder = new bool?((bool) this.designerContext.Configuration.GetProperty("SortByZOrder", (object) false));
        bool? nullable = this.sortByZOrder;
        if (nullable.GetValueOrDefault())
          return nullable.HasValue;
        return false;
      }
      set
      {
        PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.TimelineTreeSortOrderChange);
        this.sortByZOrder = new bool?(value);
        if (this.designerContext.Configuration != null)
          this.designerContext.Configuration.SetProperty("SortByZOrder", (object) (bool) (value ? true : false));
        this.ResortItems();
        this.OnSortByZOrderChanged();
      }
    }

    public double ViewportWidth
    {
      get
      {
        return this.viewportWidth;
      }
    }

    public double ViewportOffset
    {
      get
      {
        return this.viewportOffset;
      }
    }

    public TimelineItem ScopedTimelineItem
    {
      get
      {
        return this.scopedTimelineItem;
      }
      private set
      {
        if (this.scopedTimelineItem == value)
          return;
        TimelineItem timelineItem = this.scopedTimelineItem;
        this.scopedTimelineItem = value;
        if (timelineItem != null)
        {
          ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
          if (elementTimelineItem != null)
            elementTimelineItem.RefreshCanLockAndHide();
        }
        if (this.scopedTimelineItem != null)
        {
          ElementTimelineItem elementTimelineItem = this.scopedTimelineItem as ElementTimelineItem;
          if (elementTimelineItem != null)
          {
            elementTimelineItem.RefreshCanLockAndHide();
            elementTimelineItem.OnActiveTimelineContextChanged();
          }
          this.ResortItems();
          this.UpdateIsAnimatable(this.scopedTimelineItem);
          this.scopedTimelineItem.IsExpanded = true;
          if (this.scopedTimelineItem.Children.Count == 1)
            this.scopedTimelineItem.Children[0].IsExpanded = true;
        }
        this.OnScopedTimelineItemChanged();
      }
    }

    public ISceneInsertionPoint HoverOverrideInsertionPoint
    {
      get
      {
        return this.hoverOverrideInsertionPoint;
      }
      set
      {
        if (this.hoverOverrideInsertionPoint == value)
          return;
        this.hoverOverrideInsertionPoint = value;
        this.OnSceneInsertionPointChanged();
      }
    }

    public PropertySceneInsertionPoint ActiveInsertionPoint
    {
      get
      {
        return (this.HoverOverrideInsertionPoint ?? this.viewModel.ActiveSceneInsertionPoint) as PropertySceneInsertionPoint;
      }
    }

    public TimelineItem HoverOverrideSelectionItem
    {
      get
      {
        return this.hoverOverrideSelectionItem;
      }
    }

    public bool IsInValidationPass
    {
      get
      {
        return this.isInValidationPass;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TimelineItemManager(SceneViewModel viewModel)
    {
      this.viewModel = viewModel;
      this.designerContext = viewModel.DesignerContext;
      this.flattener = new VirtualizingTreeItemFlattener<TimelineItem>((ITreeViewItemProvider<TimelineItem>) this);
      this.Initialize();
    }

    private void Initialize()
    {
      this.viewModel.EarlySceneUpdatePhase += new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
      this.timelineItems = new DocumentNodeMarkerSortedListOf<TimelineItem>();
      SceneNode rootNode = this.ViewModel.RootNode;
      this.timelineSubscription = new SceneNodeSubscription<object, ScheduledTimelineItem>();
      this.timelineSubscription.Path = new SearchPath(new SearchStep[1]
      {
        new SearchStep(SearchAxis.DocumentDescendant, (ISearchPredicate) new BooleanOrPredicate((ISearchPredicate) new SceneNodeTypePredicate(typeof (AnimationSceneNode)), (ISearchPredicate) new SceneNodeTypePredicate(typeof (MediaTimelineSceneNode))), (ISearchPredicate) new DelegatePredicate(new Predicate<SceneNode>(TimelineItemManager.IsNotStyleOrTemplate), SearchScope.NodeTreeSelf))
      });
      this.timelineSubscription.SetPathNodeInsertedHandler(new SceneNodeSubscription<object, ScheduledTimelineItem>.PathNodeInsertedHandler(this.TimelineSubscription_TimelineInserted));
      this.timelineSubscription.PathNodeRemoved += new SceneNodeSubscription<object, ScheduledTimelineItem>.PathNodeRemovedListener(this.TimelineSubscription_TimelineRemoved);
      this.timelineSubscription.PathNodeContentChanged += new SceneNodeSubscription<object, ScheduledTimelineItem>.PathNodeContentChangedListener(this.TimelineSubscription_TimelineContentChanged);
      SceneElement sceneElement = this.ViewModel.ActiveEditingContainer as SceneElement;
      if (this.ViewModel.RootNode != null && sceneElement != null)
      {
        TimelineItem timelineItem = (TimelineItem) null;
        this.isInitializing = true;
        try
        {
          timelineItem = this.BuildModel((SceneNode) sceneElement);
        }
        finally
        {
          this.isInitializing = false;
        }
        this.ScopedTimelineItem = timelineItem;
        this.activeRootTimelineItem = this.FindStoryboardContainerItem(this.AnimationEditor.ActiveStoryboardContainer);
        this.timelineSubscription.InsertBasisNode((SceneNode) sceneElement);
      }
      this.OnLastAnimationTimeChanged();
    }

    private static bool IsNotStyleOrTemplate(SceneNode sceneNode)
    {
      if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) sceneNode.Type))
        return !PlatformTypes.Style.IsAssignableFrom((ITypeId) sceneNode.Type);
      return false;
    }

    public void UpdateViewport(double viewportWidth, double viewportOffset)
    {
      if (this.viewportWidth == viewportWidth && this.ViewportOffset == viewportOffset)
        return;
      this.viewportWidth = viewportWidth;
      this.viewportOffset = viewportOffset;
      this.NotifyViewportChangedRecursive(this.scopedTimelineItem);
    }

    private void NotifyViewportChangedRecursive(TimelineItem item)
    {
      if (item == null)
        return;
      item.OnViewportChanged();
      for (int index = 0; index < item.Children.Count; ++index)
        this.NotifyViewportChangedRecursive(item.Children[index]);
    }

    public void SetHoverOverrideSelectionItem(SceneNode node)
    {
      TimelineItem timelineItem = (TimelineItem) (this.FindTimelineItem(node) as ElementTimelineItem);
      if (timelineItem == this.hoverOverrideSelectionItem)
        return;
      this.hoverOverrideSelectionItem = timelineItem;
      this.OnPropertyChanged("HoverOverrideSelectionItem");
    }

    private void UpdateTransitionStatus()
    {
      if (this.transitionStatusDirty)
      {
        Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData> transitionTable = new Dictionary<TimelineSceneNode.PropertyNodePair, TransitionAnimationData>();
        this.viewModel.TransitionEditTarget.UpdateTransitionStoryboard(false, transitionTable);
        foreach (TransitionAnimationData transitionAnimationData in transitionTable.Values)
        {
          if (transitionAnimationData.TransitionAnimation != null)
          {
            AnimationTimelineItem animationTimelineItem = this.FindTimelineItem((SceneNode) transitionAnimationData.TransitionAnimation) as AnimationTimelineItem;
            if (animationTimelineItem != null)
              animationTimelineItem.IsConforming = transitionAnimationData.IsConforming;
          }
        }
      }
      this.transitionStatusDirty = false;
    }

    public void UpdateEffectTimelineItemLockState()
    {
      foreach (TimelineItem timelineItem in this.timelineItems.Values)
      {
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        if (elementTimelineItem != null)
        {
          EffectTimelineItem effectTimelineItem = this.FindChildPropertyTimelineItem((SceneNode) elementTimelineItem.Element, Base2DElement.EffectProperty) as EffectTimelineItem;
          if (effectTimelineItem != null)
            effectTimelineItem.RefreshIsAncestorHidden();
        }
      }
    }

    public void UpdateItems()
    {
      if (this.scopedTimelineItem == null)
        return;
      this.isInValidationPass = true;
      this.scopedTimelineItem.Validate();
      this.isInValidationPass = false;
    }

    public void ExtendChildPropertySelection(SceneNode targetNode, IPropertyId targetProperty)
    {
      SceneNode primarySelection = this.ViewModel.ChildPropertySelectionSet.PrimarySelection;
      TimelineItem timelineItem1 = (TimelineItem) null;
      if (primarySelection != null)
        timelineItem1 = (TimelineItem) this.FindChildPropertyTimelineItem(primarySelection.Parent, targetProperty);
      TimelineItem timelineItem2 = (TimelineItem) this.FindChildPropertyTimelineItem(targetNode.Parent, targetProperty);
      List<SceneNode> list = new List<SceneNode>();
      if (timelineItem1 != null && primarySelection != targetNode)
      {
        list.Add(primarySelection);
        bool flag = false;
        foreach (TimelineItem timelineItem3 in (ReadOnlyCollection<TimelineItem>) this.ItemList)
        {
          ChildPropertyTimelineItem propertyTimelineItem = timelineItem3 as ChildPropertyTimelineItem;
          if (propertyTimelineItem != null)
          {
            if (flag)
            {
              if (timelineItem3.IsSelectable && propertyTimelineItem.Depth == timelineItem1.Depth && targetProperty.Equals((object) propertyTimelineItem.TargetProperty))
                list.Add(propertyTimelineItem.TargetPropertyValue);
              if (timelineItem3 != timelineItem1)
              {
                if (timelineItem3 == timelineItem2)
                  break;
              }
              else
                break;
            }
            else if (timelineItem3 == timelineItem1 || timelineItem3 == timelineItem2)
            {
              flag = true;
              if (timelineItem3.IsSelectable && targetProperty.Equals((object) propertyTimelineItem.TargetProperty))
                list.Add(propertyTimelineItem.TargetPropertyValue);
            }
          }
        }
      }
      else if (!list.Contains(targetNode))
        list.Add(targetNode);
      this.ViewModel.ChildPropertySelectionSet.SetSelection((ICollection<SceneNode>) list, primarySelection);
    }

    public void ExtendElementSelection(SceneElement targetElement)
    {
      SceneElement primarySelection = this.ViewModel.ElementSelectionSet.PrimarySelection;
      TimelineItem timelineItem1 = this.FindTimelineItem((SceneNode) primarySelection);
      TimelineItem timelineItem2 = this.FindTimelineItem((SceneNode) targetElement);
      List<SceneElement> list = new List<SceneElement>();
      if (timelineItem1 != null && primarySelection != targetElement)
      {
        if (primarySelection.Parent != targetElement.Parent)
          return;
        list.Add(primarySelection);
        bool flag = false;
        foreach (TimelineItem timelineItem3 in (ReadOnlyCollection<TimelineItem>) this.ItemList)
        {
          ElementTimelineItem elementTimelineItem = timelineItem3 as ElementTimelineItem;
          if (elementTimelineItem != null)
          {
            if (flag)
            {
              if (elementTimelineItem.IsSelectable && elementTimelineItem.Depth == timelineItem1.Depth && !list.Contains(elementTimelineItem.Element))
                list.Add(elementTimelineItem.Element);
              if (elementTimelineItem != timelineItem1)
              {
                if (elementTimelineItem == timelineItem2)
                  break;
              }
              else
                break;
            }
            else if (elementTimelineItem == timelineItem1 || elementTimelineItem == timelineItem2)
            {
              flag = true;
              if (elementTimelineItem.IsSelectable && !list.Contains(elementTimelineItem.Element))
                list.Add(elementTimelineItem.Element);
            }
          }
        }
      }
      else if (!list.Contains(targetElement))
        list.Add(targetElement);
      this.ViewModel.ElementSelectionSet.SetSelection((ICollection<SceneElement>) list, primarySelection);
    }

    internal void SelectItemAndTimeRange(TimelineItem startItem, TimelineItem endItem, double startTime, double endTime, bool extendSelection)
    {
      if (startTime < 0.0)
        startTime = 0.0;
      if (endTime < 0.0)
        endTime = 0.0;
      if (startItem == endItem && startTime == endTime)
        this.ViewModel.KeyFrameSelectionSet.Clear();
      if (startTime > endTime)
      {
        double num = endTime;
        endTime = startTime;
        startTime = num;
      }
      List<KeyFrameSceneNode> list = extendSelection ? new List<KeyFrameSceneNode>((IEnumerable<KeyFrameSceneNode>) this.ViewModel.KeyFrameSelectionSet.Selection) : new List<KeyFrameSceneNode>();
      bool flag1 = false;
      bool flag2 = false;
      foreach (TimelineItem timelineItem in (ReadOnlyCollection<TimelineItem>) this.ItemList)
      {
        if (flag1)
        {
          if (timelineItem == startItem || timelineItem == endItem)
            flag2 = true;
        }
        else if (timelineItem == startItem || timelineItem == endItem)
        {
          flag1 = true;
          flag2 = startItem == endItem;
        }
        if (flag1 && timelineItem.IsSelectable)
        {
          IKeyFramedTimelineItem framedTimelineItem = timelineItem as IKeyFramedTimelineItem;
          if (framedTimelineItem != null)
          {
            foreach (KeyFrameItem keyFrameItem in (Collection<KeyFrameItem>) framedTimelineItem.KeyFrameItems)
            {
              if (keyFrameItem.Time >= startTime && keyFrameItem.Time <= endTime && framedTimelineItem.IsActive)
              {
                foreach (KeyFrameSceneNode keyFrameSceneNode in (IEnumerable<KeyFrameSceneNode>) keyFrameItem.KeyFrameSceneNodesToSelect)
                {
                  if (!list.Contains(keyFrameSceneNode))
                    list.Add(keyFrameSceneNode);
                }
              }
            }
          }
        }
        if (flag2)
          break;
      }
      this.ViewModel.KeyFrameSelectionSet.SetSelection((ICollection<KeyFrameSceneNode>) list, (KeyFrameSceneNode) null);
    }

    internal void NavigateUp()
    {
      int selectedItemIndex = this.GetSelectedItemIndex();
      while (selectedItemIndex > 0)
      {
        --selectedItemIndex;
        ElementTimelineItem elementTimelineItem = this.ItemList[selectedItemIndex] as ElementTimelineItem;
        if (elementTimelineItem != null && elementTimelineItem.IsSelectable)
        {
          this.viewModel.ElementSelectionSet.SetSelection(elementTimelineItem.Element);
          break;
        }
      }
    }

    internal void NavigateDown()
    {
      int selectedItemIndex = this.GetSelectedItemIndex();
      while (selectedItemIndex < this.ItemList.Count - 1)
      {
        ++selectedItemIndex;
        ElementTimelineItem elementTimelineItem = this.ItemList[selectedItemIndex] as ElementTimelineItem;
        if (elementTimelineItem != null && elementTimelineItem.IsSelectable)
        {
          this.viewModel.ElementSelectionSet.SetSelection(elementTimelineItem.Element);
          break;
        }
      }
    }

    private int GetSelectedItemIndex()
    {
      int num = -1;
      SceneElementSelectionSet elementSelectionSet = this.ViewModel.ElementSelectionSet;
      if (elementSelectionSet != null)
      {
        SceneElement primarySelection = elementSelectionSet.PrimarySelection;
        if (primarySelection != null)
        {
          TimelineItem timelineItem = this.FindTimelineItem((SceneNode) primarySelection);
          num = this.ItemList.IndexOf(timelineItem);
          if (num < 0)
          {
            this.EnsureExpandedAncestors(timelineItem);
            this.flattener.RebuildList();
            num = this.ItemList.IndexOf(timelineItem);
          }
        }
      }
      return num;
    }

    internal void RegisterTimelineItem(TimelineItem timelineItem)
    {
      ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
      StyleTimelineItem styleTimelineItem = timelineItem as StyleTimelineItem;
      ScheduledTimelineItem scheduledTimelineItem = timelineItem as ScheduledTimelineItem;
      DependencyObjectTimelineItem objectTimelineItem = timelineItem as DependencyObjectTimelineItem;
      if (elementTimelineItem != null)
        this.RegisterTimelineItem((SceneNode) elementTimelineItem.Element, timelineItem);
      else if (styleTimelineItem != null)
      {
        this.RegisterTimelineItem((SceneNode) styleTimelineItem.StyleNode, timelineItem);
      }
      else
      {
        if (scheduledTimelineItem != null || objectTimelineItem == null)
          return;
        this.RegisterTimelineItem(objectTimelineItem.SceneNode, timelineItem);
      }
    }

    internal void RegisterTimelineItem(SceneNode node, TimelineItem timelineItem)
    {
      if (node.DocumentNode.Marker == null)
        return;
      this.timelineItems.Add(node.DocumentNode.Marker, timelineItem);
    }

    internal TimelineItem FindTimelineItem(SceneNode sceneNode)
    {
      if (sceneNode != null && sceneNode.DocumentNode.Marker != null)
        return this.timelineItems.Find(sceneNode.DocumentNode.Marker);
      return (TimelineItem) null;
    }

    internal void RemoveTimelineItem(SceneNode sceneNode)
    {
      if (sceneNode == null || sceneNode.DocumentNode.Marker == null)
        return;
      this.timelineItems.Remove(sceneNode.DocumentNode.Marker);
    }

    internal ChildPropertyTimelineItem FindChildPropertyTimelineItem(SceneNode node, IPropertyId targetProperty)
    {
      ElementTimelineItem elementTimelineItem = this.FindTimelineItem(node) as ElementTimelineItem;
      if (elementTimelineItem != null)
      {
        for (int index = 0; index < elementTimelineItem.Children.Count; ++index)
        {
          ChildPropertyTimelineItem propertyTimelineItem = elementTimelineItem.Children[index] as ChildPropertyTimelineItem;
          if (propertyTimelineItem != null && propertyTimelineItem.TargetProperty.Equals((object) targetProperty))
            return propertyTimelineItem;
        }
      }
      return (ChildPropertyTimelineItem) null;
    }

    internal SimpleKeyFrameItem FindSimpleKeyFrameItem(KeyFrameSceneNode keyFrameSceneNode, IStoryboardContainer storyboardContainer, StoryboardTimelineSceneNode storyboardTimeline)
    {
      TimelineItem parentTimelineItem = !TimelineItemManager.UseStoryboardContainerAsAnimationParent(storyboardContainer, keyFrameSceneNode.TargetElement) ? this.FindTimelineItem(keyFrameSceneNode.TargetElement) : this.FindTimelineItem((SceneNode) storyboardContainer);
      if (parentTimelineItem != null)
      {
        TimelineItem timelineItem = (TimelineItem) ScheduledTimelineItem.FindItem(parentTimelineItem, storyboardTimeline, keyFrameSceneNode.TargetProperty);
        if (timelineItem != null)
          return KeyFrameItem.FindItem(keyFrameSceneNode.Time, timelineItem) as SimpleKeyFrameItem;
      }
      return (SimpleKeyFrameItem) null;
    }

    private TimelineItem FindStoryboardContainerItem(IStoryboardContainer storyboardContainer)
    {
      if (storyboardContainer == null)
        return (TimelineItem) null;
      BaseFrameworkElement frameworkElement = storyboardContainer as BaseFrameworkElement;
      StyleNode styleNode = storyboardContainer as StyleNode;
      FrameworkTemplateElement frameworkTemplateElement = storyboardContainer as FrameworkTemplateElement;
      if (frameworkElement != null)
        return this.FindTimelineItem((SceneNode) frameworkElement);
      if (frameworkTemplateElement != null)
        return this.FindTimelineItem((SceneNode) frameworkTemplateElement);
      return this.FindTimelineItem((SceneNode) styleNode);
    }

    internal bool ComputeHasAnythingHidden()
    {
      foreach (TimelineItem timelineItem in this.timelineItems.Values)
      {
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        if (elementTimelineItem != null && !elementTimelineItem.IsShown)
          return true;
      }
      return false;
    }

    internal bool ComputeHasAnythingLocked()
    {
      foreach (TimelineItem timelineItem in this.timelineItems.Values)
      {
        ElementTimelineItem elementTimelineItem = timelineItem as ElementTimelineItem;
        if (elementTimelineItem != null && elementTimelineItem.IsLocked)
          return true;
      }
      return false;
    }

    internal void EnsureExpandedAncestors(TimelineItem item)
    {
      if (item == null)
        return;
      for (TimelineItem parent = item.Parent; parent != null; parent = parent.Parent)
        parent.IsExpanded = true;
    }

    private void ResortItems()
    {
      if (this.ScopedTimelineItem == null)
        return;
      this.ResortItemsRecursive(this.ScopedTimelineItem, (IComparer<TimelineItem>) new TimelineItemComparer());
    }

    private void ResortItemsRecursive(TimelineItem timelineItem, IComparer<TimelineItem> comparer)
    {
      timelineItem.Children.Sort(comparer);
      for (int index = 0; index < timelineItem.Children.Count; ++index)
        this.ResortItemsRecursive(timelineItem.Children[index], comparer);
    }

    public void Dispose()
    {
      if (this.timelineItems.Count > 0)
      {
        TimelineItem timelineItem = this.timelineItems.ValueAt(0);
        if (timelineItem != null)
          timelineItem.RemoveFromTree();
      }
      this.viewModel.EarlySceneUpdatePhase -= new SceneUpdatePhaseEventHandler(this.ViewModel_EarlySceneUpdatePhase);
      this.timelineSubscription.PathNodeRemoved -= new SceneNodeSubscription<object, ScheduledTimelineItem>.PathNodeRemovedListener(this.TimelineSubscription_TimelineRemoved);
      this.timelineSubscription.PathNodeContentChanged -= new SceneNodeSubscription<object, ScheduledTimelineItem>.PathNodeContentChangedListener(this.TimelineSubscription_TimelineContentChanged);
      this.timelineSubscription.CurrentViewModel = (SceneViewModel) null;
      this.timelineSubscription = (SceneNodeSubscription<object, ScheduledTimelineItem>) null;
    }

    private void UpdateIsAnimatable(TimelineItem timelineItem)
    {
      bool flag = false;
      if (timelineItem.Parent != null)
        flag = timelineItem.Parent.IsAnimatable;
      if (timelineItem == this.activeRootTimelineItem && this.AnimationEditor.ActiveStoryboardTimeline != null)
        flag = true;
      timelineItem.IsAnimatable = flag;
      for (int index = 0; index < timelineItem.Children.Count; ++index)
        this.UpdateIsAnimatable(timelineItem.Children[index]);
      if (this.AnimationEditor.ActiveStoryboardTimeline == null || !(this.AnimationEditor.ActiveStoryboardContainer is StyleNode) || this.FindTimelineItem((SceneNode) this.AnimationEditor.ActiveStoryboardContainer.TargetElement) != timelineItem)
        return;
      timelineItem.IsAnimatable = true;
    }

    private TimelineItem BuildModel(SceneNode rootNode)
    {
      TimelineItem timelineItem = (TimelineItem) null;
      if (rootNode != null)
      {
        FrameworkTemplateElement templateElement = rootNode as FrameworkTemplateElement;
        StyleNode styleNode = rootNode as StyleNode;
        BehaviorBaseNode behaviorNode = rootNode as BehaviorBaseNode;
        SceneElement rootElement = rootNode as SceneElement;
        if (templateElement != null)
          timelineItem = (TimelineItem) this.BuildTemplateModel(templateElement);
        else if (styleNode != null)
          timelineItem = (TimelineItem) this.BuildStyleModel(styleNode);
        else if (behaviorNode != null)
          timelineItem = (TimelineItem) new BehaviorTimelineItem(this, behaviorNode);
        else if (rootElement != null)
        {
          timelineItem = this.BuildElementModel(rootElement);
          this.BuildAnimationModelForElement((SceneNode) rootElement);
        }
        else if (PlatformTypes.DependencyObject.IsAssignableFrom((ITypeId) rootNode.Type))
          timelineItem = this.BuildDependencyObjectModel(rootNode);
      }
      if (timelineItem != null)
        timelineItem.Initialize();
      return timelineItem;
    }

    private TimelineItem BuildDependencyObjectModel(SceneNode rootNode)
    {
      DependencyObjectTimelineItem objectTimelineItem = new DependencyObjectTimelineItem(this, rootNode);
      this.BuildModelChildren(rootNode);
      return (TimelineItem) objectTimelineItem;
    }

    private TimelineItem BuildElementModel(SceneElement rootElement)
    {
      CameraElement element1;
      LightElement element2;
      Model3DGroupElement element3;
      GeometryModel3DElement element4;
      ModelVisual3DElement element5;
      TimelineItem timelineItem = (element1 = rootElement as CameraElement) == null ? ((element2 = rootElement as LightElement) == null ? ((element3 = rootElement as Model3DGroupElement) == null ? ((element4 = rootElement as GeometryModel3DElement) == null ? ((element5 = rootElement as ModelVisual3DElement) == null ? (TimelineItem) new ElementTimelineItem(this, rootElement) : (TimelineItem) new ModelVisual3DTimelineItem(this, element5)) : (TimelineItem) new Primitive3DTimelineItem(this, element4)) : (TimelineItem) new Model3DGroupTimelineItem(this, element3)) : (TimelineItem) new LightTimelineItem(this, element2)) : (TimelineItem) new CameraTimelineItem(this, element1);
      this.BuildModelChildren((SceneNode) rootElement);
      return timelineItem;
    }

    private void BuildModelChildren(SceneNode parentNode)
    {
      SceneElement parentElement = parentNode as SceneElement;
      if (parentElement != null)
      {
        ElementTimelineItem parentElementItem = this.FindTimelineItem((SceneNode) parentElement) as ElementTimelineItem;
        if (parentElementItem != null)
          this.BuildChildrenPropertyRow(parentElementItem, parentElement);
      }
      IEnumerator<DocumentNode> enumerator = parentNode.DocumentNode.DescendantNodes.GetEnumerator();
      while (enumerator.MoveNext())
      {
        DocumentNode current = enumerator.Current;
        if (TimelineItemManager.CanAddRemoveInContext(current))
        {
          SceneNode sceneNode = this.viewModel.GetSceneNode(current);
          SceneNode parentNode1 = (SceneNode) null;
          if (sceneNode != null)
            parentNode1 = sceneNode.Parent;
          IDescendantEnumerator descendantEnumerator = enumerator as IDescendantEnumerator;
          if (descendantEnumerator != null)
            descendantEnumerator.SkipPastDescendants(current);
          if (parentNode1 != null)
            this.OnNodeAdded(parentNode1, sceneNode);
        }
        else if (TimelineItemManager.ShouldSkipBuildingChildren((ITypeId) current.Type))
        {
          IDescendantEnumerator descendantEnumerator = enumerator as IDescendantEnumerator;
          if (descendantEnumerator != null)
            descendantEnumerator.SkipPastDescendants(current);
        }
      }
    }

    internal static void AddChildPropertyHelper(ref IList<IPropertyId> propertyList, IPropertyId property)
    {
      if (propertyList == null)
        propertyList = (IList<IPropertyId>) new List<IPropertyId>();
      propertyList.Add(property);
    }

    private IList<IPropertyId> RetreiveValidChildrenProperties(SceneElement parentSceneNode)
    {
      IList<IPropertyId> propertyList = (IList<IPropertyId>) null;
      if (parentSceneNode.ProjectContext.ResolveProperty(Base2DElement.EffectProperty) != null && parentSceneNode.IsSet(Base2DElement.EffectProperty) == PropertyState.Set && (!parentSceneNode.IsValueExpression(Base2DElement.EffectProperty) && parentSceneNode.GetLocalValue(Base2DElement.EffectProperty) != null))
        TimelineItemManager.AddChildPropertyHelper(ref propertyList, Base2DElement.EffectProperty);
      foreach (IPropertyId propertyId in parentSceneNode.Metadata.ContentProperties)
      {
        IProperty property = parentSceneNode.ProjectContext.ResolveProperty(propertyId);
        if (property != parentSceneNode.Metadata.DefaultContentProperty)
        {
          bool flag = !parentSceneNode.IsValueExpression((IPropertyId) property);
          if (flag && property.Equals((object) ModelVisual3DElement.ContentProperty))
          {
            ModelVisual3DElement modelVisual3Delement = parentSceneNode as ModelVisual3DElement;
            if (modelVisual3Delement.Content == null && modelVisual3Delement.Children.Count != 0)
              flag = false;
          }
          if (flag)
            TimelineItemManager.AddChildPropertyHelper(ref propertyList, propertyId);
        }
      }
      return propertyList;
    }

    private bool BuildChildrenPropertyRow(ElementTimelineItem parentElementItem, SceneElement parentElement)
    {
      if (parentElement == null || parentElementItem == null)
        return false;
      IList<IPropertyId> list = this.RetreiveValidChildrenProperties(parentElement);
      if (list == null)
        return false;
      int num = 0;
      foreach (IPropertyId propertyId in (IEnumerable<IPropertyId>) list)
      {
        IProperty key = parentElement.ProjectContext.ResolveProperty(propertyId);
        if (this.FindChildPropertyTimelineItem((SceneNode) parentElementItem.Element, (IPropertyId) key) == null)
        {
          ChildPropertyTimelineItem propertyTimelineItem = ChildPropertyTimelineItemFactory.CreateChildPropertyTimelineItem(this, key, parentElementItem);
          parentElementItem.AddChild((TimelineItem) propertyTimelineItem);
          propertyTimelineItem.StackOrder = num;
        }
        ++num;
      }
      return true;
    }

    private StyleTimelineItem BuildStyleModel(StyleNode styleNode)
    {
      StyleTimelineItem styleTimelineItem = new StyleTimelineItem(this, styleNode);
      this.BuildModelChildren((SceneNode) styleNode);
      this.BuildAnimationModelForStoryboard((IStoryboardContainer) styleNode);
      return styleTimelineItem;
    }

    private TemplateTimelineItem BuildTemplateModel(FrameworkTemplateElement templateElement)
    {
      TemplateTimelineItem templateTimelineItem = new TemplateTimelineItem(this, templateElement);
      this.BuildModelChildren((SceneNode) templateElement);
      FrameworkTemplateElement frameworkTemplateElement = templateElement;
      if (frameworkTemplateElement != null)
        this.BuildAnimationModelForStoryboard((IStoryboardContainer) frameworkTemplateElement);
      return templateTimelineItem;
    }

    private void BuildAnimationModelForStoryboard(IStoryboardContainer storyboardContainer)
    {
      foreach (StoryboardTimelineSceneNode parentTimeline in this.viewModel.AnimationEditor.EnumerateStoryboardsForContainer(storyboardContainer))
      {
        foreach (TimelineSceneNode animation in (IEnumerable<TimelineSceneNode>) parentTimeline.Children)
          this.BuildAnimationModelForAnimation(storyboardContainer, parentTimeline, animation);
      }
    }

    private void BuildAnimationModelForElement(SceneNode targetElement)
    {
      IStoryboardContainer storyboardContainer = targetElement.StoryboardContainer;
      if (storyboardContainer == targetElement)
      {
        this.BuildAnimationModelForStoryboard(storyboardContainer);
      }
      else
      {
        if (storyboardContainer == null)
          return;
        foreach (StoryboardTimelineSceneNode parentTimeline in this.viewModel.AnimationEditor.EnumerateStoryboardsForContainer(storyboardContainer))
        {
          foreach (TimelineSceneNode animation in (IEnumerable<TimelineSceneNode>) parentTimeline.Children)
          {
            AnimationSceneNode animationSceneNode = animation as AnimationSceneNode;
            MediaTimelineSceneNode timelineSceneNode = animation as MediaTimelineSceneNode;
            if (animationSceneNode != null && animationSceneNode.TargetElement == targetElement || timelineSceneNode != null && timelineSceneNode.TargetElement == targetElement)
              this.BuildAnimationModelForAnimation(storyboardContainer, parentTimeline, animation);
          }
        }
      }
    }

    private void BuildAnimationModelForAnimation(IStoryboardContainer storyboardContainer, StoryboardTimelineSceneNode parentTimeline, TimelineSceneNode animation)
    {
      if (this.FindTimelineItem((SceneNode) animation) != null)
        return;
      AnimationSceneNode animationSceneNode1 = animation as AnimationSceneNode;
      if (animationSceneNode1 != null)
      {
        TimelineSceneNode.PropertyNodePair elementAndProperty1 = animationSceneNode1.TargetElementAndProperty;
        SceneNode sceneNode = elementAndProperty1.SceneNode;
        PropertyReference propertyReference = elementAndProperty1.PropertyReference;
        if (propertyReference == null || parentTimeline.GetAnimation(sceneNode, propertyReference) != animationSceneNode1)
          return;
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) parentTimeline.Children)
        {
          AnimationSceneNode animationSceneNode2 = timelineSceneNode as AnimationSceneNode;
          if (animationSceneNode2 != null && animationSceneNode2 != animationSceneNode1)
          {
            TimelineSceneNode.PropertyNodePair elementAndProperty2 = animationSceneNode2.TargetElementAndProperty;
            if (elementAndProperty2.SceneNode == sceneNode && propertyReference.Equals((object) elementAndProperty2.PropertyReference))
            {
              TimelineItem timelineItem = this.FindTimelineItem((SceneNode) animationSceneNode2);
              if (timelineItem != null && timelineItem.Parent != null)
                timelineItem.Parent.RemoveChild(timelineItem);
            }
          }
        }
      }
      PathAnimationSceneNode pathAnimation = animation as PathAnimationSceneNode;
      if (pathAnimation != null)
      {
        foreach (TimelineSceneNode timelineSceneNode in (IEnumerable<TimelineSceneNode>) parentTimeline.Children)
        {
          PathAnimationSceneNode animationSceneNode2 = timelineSceneNode as PathAnimationSceneNode;
          if (animationSceneNode2 != null && animationSceneNode2 != pathAnimation && animationSceneNode2.TargetElement == pathAnimation.TargetElement)
          {
            PathTimelineItem pathTimelineItem = (PathTimelineItem) this.FindTimelineItem((SceneNode) animationSceneNode2);
            if (pathTimelineItem != null)
            {
              pathTimelineItem.AddPathAnimationSceneNode(pathAnimation);
              return;
            }
          }
        }
      }
      MediaTimelineSceneNode mediaTimelineSceneNode = animation as MediaTimelineSceneNode;
      TimelineItem timelineItem1 = (TimelineItem) null;
      if (animationSceneNode1 != null)
      {
        TimelineSceneNode.PropertyNodePair elementAndProperty = animationSceneNode1.TargetElementAndProperty;
        PropertyReference propertyReference1 = elementAndProperty.PropertyReference;
        SceneElement targetElement = elementAndProperty.SceneNode as SceneElement;
        if (targetElement != null)
        {
          ReferenceStep referenceStep1 = (ReferenceStep) targetElement.ProjectContext.ResolveProperty(BehaviorTriggerBaseNode.BehaviorActionsProperty);
          ReferenceStep referenceStep2 = (ReferenceStep) targetElement.ProjectContext.ResolveProperty(Base2DElement.EffectProperty);
          TimelineItem timelineItem2;
          if (propertyReference1.ReferenceSteps.Contains(referenceStep2))
          {
            targetElement.GetLocalValueAsSceneNode(propertyReference1.Subreference(0, propertyReference1.ReferenceSteps.IndexOf(referenceStep2)));
            timelineItem2 = (TimelineItem) this.FindChildPropertyTimelineItem((SceneNode) targetElement, Base2DElement.EffectProperty);
          }
          else
            timelineItem2 = !propertyReference1.ReferenceSteps.Contains(referenceStep1) ? this.FindTimelineItem((SceneNode) targetElement) : this.FindTimelineItem(targetElement.GetLocalValueAsSceneNode(propertyReference1.Subreference(0, propertyReference1.ReferenceSteps.IndexOf(referenceStep1) + 1)));
          if (timelineItem2 != null)
          {
            TimelineItem targetTimelineItem = timelineItem2;
            KeyFrameAnimationSceneNode propertyAnimation;
            PathAnimationSceneNode pathAnimationSceneNode;
            TimelineItem child1 = (propertyAnimation = animationSceneNode1 as KeyFrameAnimationSceneNode) == null ? ((pathAnimationSceneNode = animationSceneNode1 as PathAnimationSceneNode) == null ? (TimelineItem) new AnimationTimelineItem(this, parentTimeline, animationSceneNode1) : (TimelineItem) new PathTimelineItem(this, parentTimeline, pathAnimationSceneNode)) : (TimelineItem) new PropertyTimelineItem(this, targetTimelineItem, targetElement, parentTimeline, propertyAnimation);
            ReadOnlyCollection<ReferenceStep> referenceSteps = elementAndProperty.PropertyReference.ReferenceSteps;
            List<ReferenceStep> steps = new List<ReferenceStep>();
            for (int index = 0; index < referenceSteps.Count - 1; ++index)
            {
              steps.Add(referenceSteps[index]);
              PropertyReference propertyReference2 = new PropertyReference(steps);
              TimelineItem child2 = (TimelineItem) null;
              foreach (TimelineItem timelineItem3 in (Collection<TimelineItem>) timelineItem2.Children)
              {
                if (timelineItem3 is CompoundPropertyTimelineItem && ((CompoundPropertyTimelineItem) timelineItem3).TargetProperty.Path == propertyReference2.Path)
                {
                  child2 = timelineItem3;
                  break;
                }
              }
              if (child2 == null)
              {
                string displayNameOverride = (string) null;
                ReferenceStep referenceStep3 = referenceSteps[index];
                ReferenceStep referenceStep4 = referenceSteps[index + 1];
                if (index > 0 && PlatformTypes.TransformGroup.IsAssignableFrom((ITypeId) targetElement.Platform.Metadata.GetType(referenceSteps[index - 1].TargetType)))
                {
                  IndexedClrPropertyReferenceStep propertyReferenceStep = referenceStep3 as IndexedClrPropertyReferenceStep;
                  if (propertyReferenceStep != null && propertyReferenceStep.Index >= 0 && (propertyReferenceStep.Index < TimelineItemManager.canonicalTransformNames.Length && TimelineItemManager.canonicalTransformTypes[propertyReferenceStep.Index].IsAssignableFrom(referenceStep4.DeclaringTypeId)))
                    displayNameOverride = TimelineItemManager.canonicalTransformNames[propertyReferenceStep.Index];
                }
                else if (index > 0 && PlatformTypes.Transform3DGroup.IsAssignableFrom((ITypeId) targetElement.Platform.Metadata.GetType(referenceSteps[index - 1].TargetType)))
                {
                  IndexedClrPropertyReferenceStep propertyReferenceStep = referenceStep3 as IndexedClrPropertyReferenceStep;
                  if (propertyReferenceStep != null && propertyReferenceStep.Index >= 0 && (propertyReferenceStep.Index < TimelineItemManager.canonicalTransform3DNames.Length && TimelineItemManager.canonicalTransform3DTypes[propertyReferenceStep.Index].IsAssignableFrom(referenceStep4.DeclaringTypeId)))
                    displayNameOverride = TimelineItemManager.canonicalTransform3DNames[propertyReferenceStep.Index];
                }
                if (!PlatformTypes.TransformCollection.IsAssignableFrom((ITypeId) referenceStep3.PropertyType) && !PlatformTypes.Transform3DCollection.IsAssignableFrom((ITypeId) referenceStep3.PropertyType) && (!referenceStep3.PropertyType.Equals((object) ProjectNeutralTypes.BehaviorTriggerBase) && !referenceStep3.PropertyType.Equals((object) ProjectNeutralTypes.BehaviorTriggerAction)) && (!referenceStep3.PropertyType.Equals((object) ProjectNeutralTypes.BehaviorTriggerActionCollection) && !referenceStep3.PropertyType.Equals((object) ProjectNeutralTypes.BehaviorTriggerCollection)))
                {
                  child2 = (TimelineItem) new CompoundPropertyTimelineItem(this, new PropertyReference(steps), targetTimelineItem, displayNameOverride);
                  timelineItem2.AddChild(child2);
                }
              }
              if (child2 != null)
                timelineItem2 = child2;
              if (timelineItem1 == null)
                timelineItem1 = child2;
            }
            if (timelineItem1 == null)
              timelineItem1 = child1;
            timelineItem2.AddChild(child1);
          }
        }
      }
      else if (mediaTimelineSceneNode != null)
      {
        SceneElement mediaElement = storyboardContainer.ResolveTargetName(mediaTimelineSceneNode.TargetName) as SceneElement;
        if (mediaElement != null)
        {
          TimelineItem timelineItem2 = this.FindTimelineItem((SceneNode) mediaElement);
          if (timelineItem2 != null)
          {
            MediaTimelineItem mediaTimelineItem = new MediaTimelineItem(this, mediaElement, parentTimeline, mediaTimelineSceneNode);
            timelineItem2.AddChild((TimelineItem) mediaTimelineItem);
            timelineItem1 = (TimelineItem) mediaTimelineItem;
          }
        }
      }
      if (timelineItem1 == null)
        return;
      this.UpdateIsAnimatable(timelineItem1);
    }

    private void RebuildAnimationModelForElement(SceneNode targetElement)
    {
      TimelineItem timelineItem1 = this.FindTimelineItem(targetElement);
      if (timelineItem1 == null)
        return;
      List<TimelineItem> list = new List<TimelineItem>();
      for (int index = 0; index < timelineItem1.Children.Count; ++index)
      {
        TimelineItem timelineItem2 = timelineItem1.Children[index];
        if (timelineItem2 is PropertyTimelineItem || timelineItem2 is CompoundPropertyTimelineItem)
          list.Add(timelineItem2);
      }
      foreach (TimelineItem child in list)
        timelineItem1.RemoveSubtree(child);
      this.BuildAnimationModelForElement(targetElement);
    }

    private void OnActiveTriggerChanged()
    {
      if (this.ScopedTimelineItem == null)
        return;
      if (this.ScopedTimelineItem is ElementTimelineItem)
        ((ElementTimelineItem) this.ScopedTimelineItem).RefreshIsLocked();
      else if (this.ScopedTimelineItem is StyleTimelineItem)
        ((StyleTimelineItem) this.ScopedTimelineItem).RefreshIsLocked();
      this.ScopedTimelineItem.OnActiveTimelineContextChanged();
    }

    private void RemoveTimelineItem(ScheduledTimelineItem scheduledTimelineItem, TimelineSceneNode removingAnimation)
    {
      PathTimelineItem pathTimelineItem = scheduledTimelineItem as PathTimelineItem;
      if (pathTimelineItem != null)
      {
        PathAnimationSceneNode pathAnimation = (PathAnimationSceneNode) removingAnimation;
        if (pathTimelineItem.PathAnimationSceneNodes.Count == 1 && pathTimelineItem.PathAnimationSceneNodes[0] == pathAnimation)
          pathTimelineItem.Parent.RemoveChild((TimelineItem) pathTimelineItem);
        else
          pathTimelineItem.RemovePathAnimationSceneNode(pathAnimation);
      }
      else
      {
        if (scheduledTimelineItem.Parent == null)
          return;
        scheduledTimelineItem.Parent.RemoveChild((TimelineItem) scheduledTimelineItem);
      }
    }

    private void OnActiveTimelineChanged()
    {
      this.activeRootTimelineItem = this.FindStoryboardContainerItem(this.AnimationEditor.ActiveStoryboardContainer);
      if (this.ScopedTimelineItem != null)
      {
        ElementTimelineItem elementTimelineItem;
        if ((elementTimelineItem = this.ScopedTimelineItem as ElementTimelineItem) != null)
        {
          elementTimelineItem.RefreshIsLocked();
        }
        else
        {
          StyleTimelineItem styleTimelineItem;
          if ((styleTimelineItem = this.ScopedTimelineItem as StyleTimelineItem) != null)
            styleTimelineItem.RefreshIsLocked();
        }
        this.ScopedTimelineItem.OnActiveTimelineContextChanged();
        this.UpdateIsAnimatable(this.ScopedTimelineItem);
      }
      this.OnLastAnimationTimeChanged();
    }

    private void RemoveItemFromTimelineItemTree(TimelineItem childItem)
    {
      childItem.RemoveFromTree();
      this.OnLastAnimationTimeChanged();
    }

    private void OnNodeAdded(SceneNode parentNode, SceneNode childNode)
    {
      IList<IPropertyId> list = (IList<IPropertyId>) null;
      SceneElement parentSceneNode = parentNode as SceneElement;
      if (parentSceneNode != null)
        list = this.RetreiveValidChildrenProperties(parentSceneNode);
      IProperty propertyForChild = parentNode.GetPropertyForChild(childNode);
      if (propertyForChild != parentNode.DefaultContentProperty && !parentNode.ContentProperties.Contains((IPropertyId) propertyForChild) && (list == null || !list.Contains((IPropertyId) propertyForChild)) && !propertyForChild.Equals((object) BehaviorHelper.BehaviorsProperty))
        return;
      TextElementSceneElement elementSceneElement = parentNode as TextElementSceneElement;
      if (elementSceneElement != null)
      {
        parentNode = (SceneNode) elementSceneElement.HostElement;
        if (parentNode == null)
          return;
      }
      TimelineItem timelineItem1 = this.FindTimelineItem(parentNode);
      if (timelineItem1 == null)
      {
        SceneNode child = childNode;
        for (; timelineItem1 == null && parentNode != null; timelineItem1 = this.FindTimelineItem(parentNode))
        {
          if (parentNode is StyleNode)
          {
            timelineItem1 = (TimelineItem) null;
            break;
          }
          if (!parentNode.ContentProperties.Contains((IPropertyId) parentNode.GetPropertyForChild(child)))
          {
            timelineItem1 = (TimelineItem) null;
            break;
          }
          if (parentNode is FrameworkTemplateElement)
          {
            timelineItem1 = this.FindTimelineItem(parentNode);
            break;
          }
          child = parentNode;
          parentNode = parentNode.Parent;
        }
      }
      else if (elementSceneElement == null && !propertyForChild.Equals((object) BehaviorHelper.BehaviorsProperty) && propertyForChild != parentNode.DefaultContentProperty)
      {
        TimelineItem timelineItem2 = (TimelineItem) this.FindChildPropertyTimelineItem(parentNode, (IPropertyId) propertyForChild);
        if (timelineItem2 != null)
        {
          timelineItem1 = timelineItem2;
        }
        else
        {
          ElementTimelineItem parentElementItem = timelineItem1 as ElementTimelineItem;
          this.BuildChildrenPropertyRow(parentElementItem, parentElementItem.Element);
          timelineItem1 = (TimelineItem) this.FindChildPropertyTimelineItem(parentNode, (IPropertyId) propertyForChild);
        }
      }
      if (timelineItem1 == null)
        return;
      TimelineItem timelineItem3 = this.FindTimelineItem(childNode);
      if (timelineItem3 == null)
      {
        if (childNode is TextElementSceneElement || childNode is FlowDocumentElement)
        {
          this.BuildModelChildren(childNode);
          this.UpdateIsAnimatable(timelineItem1);
          this.OnLastAnimationTimeChanged();
          return;
        }
        timelineItem3 = this.BuildModel(childNode);
      }
      else
      {
        this.BuildModelChildren(childNode);
        this.BuildAnimationModelForElement(parentNode);
      }
      if (timelineItem3 != null && timelineItem1 != timelineItem3.Parent)
      {
        ChildPropertyTimelineItem propertyTimelineItem = timelineItem1 as ChildPropertyTimelineItem;
        if (propertyTimelineItem == null || propertyTimelineItem.IsAlternateContentProperty || !(timelineItem3 is DependencyObjectTimelineItem))
        {
          timelineItem1.AddChild(timelineItem3);
          this.UpdateIsAnimatable(timelineItem3);
        }
      }
      this.OnLastAnimationTimeChanged();
    }

    private static bool IsValidTypeForAddedRemoved(ITypeId type)
    {
      if (!PlatformTypes.UIElement.IsAssignableFrom(type) && !PlatformTypes.Visual3D.IsAssignableFrom(type) && (!PlatformTypes.Camera.IsAssignableFrom(type) && !PlatformTypes.Model3D.IsAssignableFrom(type)) && (!ProjectNeutralTypes.Behavior.IsAssignableFrom(type) && !ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom(type)))
        return PlatformTypes.Effect.IsAssignableFrom(type);
      return true;
    }

    private static bool ShouldSkipBuildingChildren(ITypeId type)
    {
      if (!PlatformTypes.FrameworkTemplate.IsAssignableFrom(type) && !PlatformTypes.Style.IsAssignableFrom(type))
        return ProjectNeutralTypes.Behavior.IsAssignableFrom(type);
      return true;
    }

    private static bool IsDependencyObjectInNonDefaultContentProperty(DocumentNode node)
    {
      if (PlatformTypes.DependencyObject.IsAssignableFrom((ITypeId) node.Type))
      {
        DocumentNode documentNode1 = (DocumentNode) node.Parent;
        DocumentNode documentNode2 = node;
        SceneElement sceneElement = (SceneElement) null;
        while (sceneElement == null && documentNode1 != null)
        {
          sceneElement = documentNode1.SceneNode as SceneElement;
          if (sceneElement == null && documentNode1.Parent != null)
          {
            documentNode1 = (DocumentNode) documentNode1.Parent;
            documentNode2 = (DocumentNode) documentNode2.Parent;
          }
          else
            break;
        }
        if (sceneElement != null)
        {
          IPropertyId propertyId = (IPropertyId) documentNode2.SitePropertyKey;
          foreach (object obj in sceneElement.Metadata.ContentProperties)
          {
            if (obj.Equals((object) propertyId) && sceneElement.Metadata.DefaultContentProperty != propertyId)
              return true;
          }
        }
      }
      return false;
    }

    private static bool CanAddRemoveInContext(DocumentNode node)
    {
      if (node is DocumentPrimitiveNode)
        return false;
      if (TimelineItemManager.IsDependencyObjectInNonDefaultContentProperty(node))
        return true;
      if (!TimelineItemManager.IsValidTypeForAddedRemoved((ITypeId) node.Type))
        return false;
      if (ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) node.Type))
      {
        for (DocumentNode documentNode = node; documentNode != null; documentNode = (DocumentNode) documentNode.Parent)
        {
          if (ProjectNeutralTypes.Behavior.IsAssignableFrom((ITypeId) documentNode.Type))
            return false;
        }
      }
      return true;
    }

    private void HandleElementAdds(DocumentNodeChangeList changes)
    {
      foreach (DocumentNodeMarker documentNodeMarker in changes.Markers)
      {
        if (!documentNodeMarker.IsDeleted)
        {
          DocumentNode node = documentNodeMarker.Node;
          DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
          SceneElement sceneElement = this.viewModel.GetSceneNode(node) as SceneElement;
          if (sceneElement == null || documentCompositeNode == null || !sceneElement.IsPlaceholder)
          {
            if (TimelineItemManager.CanAddRemoveInContext(node))
            {
              SceneNode sceneNode = this.viewModel.GetSceneNode(node);
              if (sceneNode != null)
              {
                SceneNode parent = sceneNode.Parent;
                if (parent != null)
                  this.OnNodeAdded(parent, sceneNode);
              }
            }
            else
            {
              IEnumerator<DocumentNode> enumerator = node.DescendantNodes.GetEnumerator();
              while (enumerator.MoveNext())
              {
                DocumentNode current = enumerator.Current;
                if (TimelineItemManager.CanAddRemoveInContext(current))
                {
                  SceneNode sceneNode = this.viewModel.GetSceneNode(current);
                  if (sceneNode != null)
                  {
                    SceneNode parent = sceneNode.Parent;
                    if (parent != null)
                    {
                      this.OnNodeAdded(parent, sceneNode);
                      IDescendantEnumerator descendantEnumerator = enumerator as IDescendantEnumerator;
                      if (descendantEnumerator != null)
                        descendantEnumerator.SkipPastDescendants(current);
                    }
                  }
                }
                else if (TimelineItemManager.ShouldSkipBuildingChildren((ITypeId) current.Type))
                {
                  IDescendantEnumerator descendantEnumerator = enumerator as IDescendantEnumerator;
                  if (descendantEnumerator != null)
                    descendantEnumerator.SkipPastDescendants(current);
                }
              }
            }
          }
        }
      }
    }

    private void HandleElementRemoves(DocumentNodeChangeList changes)
    {
      if (changes.Count == 0)
        return;
      int num = 0;
      List<TimelineItem> list = new List<TimelineItem>();
      for (int index = 0; index < this.timelineItems.Count; ++index)
      {
        if (!this.timelineItems.MarkerAt(index).IsDeleted)
        {
          if (num < index)
            this.timelineItems.Copy(num, (DocumentNodeMarkerSortedListBase) this.timelineItems, index);
          ++num;
        }
        else
        {
          TimelineItem timelineItem = this.timelineItems.ValueAt(index);
          list.Add(timelineItem);
        }
      }
      this.timelineItems.RemoveRange(num, this.timelineItems.Count - num);
      foreach (TimelineItem childItem in list)
        this.RemoveItemFromTimelineItemTree(childItem);
    }

    private void OnPropertyReferenceSceneChange(PropertyReferenceSceneChange e)
    {
      if (e.PropertyChanged == null || e.Target == null)
        return;
      ReferenceStep referenceStep = e.PropertyChanged[0];
      SceneNode target = e.Target;
      if (target == null)
        return;
      IPropertyId nameProperty = target.NameProperty;
      SceneElement parentElement = target as SceneElement;
      ElementTimelineItem parentElementItem = (ElementTimelineItem) null;
      if (nameProperty != null && nameProperty.Equals((object) referenceStep) || parentElement != null && parentElement.IsTextContentChange(e.PropertyChanged))
      {
        TimelineItem timelineItem = this.FindTimelineItem(e.Target);
        if (timelineItem != null)
          timelineItem.RefreshName();
      }
      if (parentElementItem == null)
        parentElementItem = this.FindTimelineItem(target) as ElementTimelineItem;
      if (parentElement == null)
        return;
      ChildPropertyTimelineItem propertyTimelineItem = this.FindChildPropertyTimelineItem(target, (IPropertyId) referenceStep);
      PropertyState propertyState = target.IsSet((IPropertyId) referenceStep);
      if (propertyTimelineItem != null && (propertyState == PropertyState.Unset || propertyState == PropertyState.Set && parentElement.IsValueExpression((IPropertyId) referenceStep)))
      {
        if (parentElement.ContentProperties.Contains((IPropertyId) referenceStep))
          return;
        propertyTimelineItem.RemoveFromTree();
      }
      else
      {
        if (propertyState != PropertyState.Set)
          return;
        if (propertyTimelineItem != null && propertyTimelineItem is EffectTimelineItem && parentElement.GetLocalValue((IPropertyId) referenceStep) == null)
          propertyTimelineItem.RemoveFromTree();
        else if (propertyTimelineItem == null)
          this.BuildChildrenPropertyRow(parentElementItem, parentElement);
        else
          propertyTimelineItem.ResetAllProperties();
      }
    }

    private void OnDesignTimePropertySceneChange(DesignTimePropertySceneChange e)
    {
      IPropertyId designTimePropertyKey = e.DesignTimePropertyKey;
      if (DesignTimeProperties.IsLockedProperty.Equals((object) designTimePropertyKey))
      {
        SceneElement sceneElement = e.Target as SceneElement;
        if (sceneElement != null)
        {
          ElementTimelineItem elementTimelineItem = this.FindTimelineItem((SceneNode) sceneElement) as ElementTimelineItem;
          if (elementTimelineItem == null)
            return;
          elementTimelineItem.RefreshIsLocked();
        }
        else
        {
          StyleNode styleNode = e.Target as StyleNode;
          if (styleNode == null)
            return;
          StyleTimelineItem styleTimelineItem = this.FindTimelineItem((SceneNode) styleNode) as StyleTimelineItem;
          if (styleTimelineItem == null)
            return;
          styleTimelineItem.RefreshIsLocked();
        }
      }
      else if (DesignTimeProperties.IsHiddenProperty.Equals((object) designTimePropertyKey))
      {
        SceneElement sceneElement = e.Target as SceneElement;
        if (sceneElement == null)
          return;
        ElementTimelineItem elementTimelineItem = this.FindTimelineItem((SceneNode) sceneElement) as ElementTimelineItem;
        if (elementTimelineItem == null)
          return;
        elementTimelineItem.RefreshIsShown();
      }
      else
      {
        if (!DesignTimeProperties.XNameProperty.Equals((object) designTimePropertyKey))
          return;
        SceneNode target = e.Target;
        if (target == null)
          return;
        TimelineItem timelineItem = this.FindTimelineItem(target);
        if (timelineItem == null)
          return;
        timelineItem.RefreshName();
      }
    }

    private void OnActiveEditingContainerChanged()
    {
      SceneNode editingContainer = this.ViewModel.ActiveEditingContainer;
      List<SceneNode> list = new List<SceneNode>();
      if (editingContainer != null)
        list.Add(editingContainer);
      TimelineItem timelineItem = this.FindStoryboardContainerItem(this.ViewModel.ActiveStoryboardContainer);
      if (timelineItem == null && this.ViewModel.ActiveEditingContainer is SceneElement)
        timelineItem = this.BuildModel(this.ViewModel.ActiveEditingContainer);
      this.timelineSubscription.SetBasisNodes(this.ViewModel, (IEnumerable<SceneNode>) list);
      this.ScopedTimelineItem = timelineItem;
      if (this.ScopedTimelineItem == null)
        return;
      this.ScopedTimelineItem.OnActiveTimelineContextChanged();
    }

    private void OnTriggersChanged()
    {
      if (this.ScopedTimelineItem == null)
        return;
      if (this.ScopedTimelineItem is ElementTimelineItem)
        ((ElementTimelineItem) this.ScopedTimelineItem).RefreshIsLocked();
      else if (this.ScopedTimelineItem is StyleTimelineItem)
        ((StyleTimelineItem) this.ScopedTimelineItem).RefreshIsLocked();
      this.ScopedTimelineItem.OnActiveTimelineContextChanged();
    }

    private void OnSceneInsertionPointChanged()
    {
      if (this.hoverOverrideInsertionPoint != null && !this.hoverOverrideInsertionPoint.SceneNode.IsInDocument)
        this.hoverOverrideInsertionPoint = (ISceneInsertionPoint) null;
      if (this.lastSceneInsertionPointMarker != null && !this.lastSceneInsertionPointMarker.IsDeleted)
      {
        PropertySceneInsertionPoint insertionPoint = this.lastSceneInsertionPointMarker.GetInsertionPoint(this.viewModel);
        if (insertionPoint != null)
        {
          IPropertyId targetProperty = (IPropertyId) insertionPoint.Property;
          SceneNode sceneNode = insertionPoint.SceneNode;
          ElementTimelineItem elementTimelineItem = this.FindTimelineItem(sceneNode) as ElementTimelineItem;
          if (elementTimelineItem != null)
            elementTimelineItem.RefreshInsertionPointProperties();
          ChildPropertyTimelineItem propertyTimelineItem = this.FindChildPropertyTimelineItem(sceneNode, targetProperty);
          if (propertyTimelineItem != null)
            propertyTimelineItem.RefreshInsertionPointProperties();
        }
      }
      this.lastSceneInsertionPointMarker = (PropertySceneInsertionPointMarker) null;
      PropertySceneInsertionPoint activeInsertionPoint = this.ActiveInsertionPoint;
      if (activeInsertionPoint == null)
        return;
      IPropertyId targetProperty1 = (IPropertyId) activeInsertionPoint.Property;
      SceneNode sceneNode1 = activeInsertionPoint.SceneNode;
      if (!sceneNode1.IsInDocument)
        return;
      this.lastSceneInsertionPointMarker = new PropertySceneInsertionPointMarker(activeInsertionPoint);
      if (targetProperty1.Equals((object) sceneNode1.DefaultContentProperty))
      {
        ElementTimelineItem elementTimelineItem = this.FindTimelineItem(sceneNode1) as ElementTimelineItem;
        if (elementTimelineItem == null)
          return;
        elementTimelineItem.RefreshInsertionPointProperties();
      }
      else
      {
        ChildPropertyTimelineItem propertyTimelineItem = this.FindChildPropertyTimelineItem(sceneNode1, targetProperty1);
        if (propertyTimelineItem == null)
          return;
        propertyTimelineItem.RefreshInsertionPointProperties();
      }
    }

    private void ViewModel_EarlySceneUpdatePhase(object sender, SceneUpdatePhaseEventArgs args)
    {
      if (args.DocumentChanges.Count > 0)
      {
        foreach (SceneChange sceneChange in SceneChange.Changes(args.DocumentChanges, this.viewModel.RootNode, (Type) null))
        {
          if (sceneChange.GetType() == typeof (PropertyReferenceSceneChange))
            this.OnPropertyReferenceSceneChange(sceneChange as PropertyReferenceSceneChange);
          else if (sceneChange.GetType() == typeof (DesignTimePropertySceneChange))
            this.OnDesignTimePropertySceneChange(sceneChange as DesignTimePropertySceneChange);
        }
        this.HandleElementRemoves(args.DocumentChanges);
        this.HandleElementAdds(args.DocumentChanges);
        this.timelineSubscription.Update(args.ViewModel, args.DocumentChanges, args.DocumentChangeStamp);
      }
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveEditingContainer) || args.IsDirtyViewState(SceneViewModel.ViewStateBits.IsEditable) && this.ScopedTimelineItem == null)
        this.OnActiveEditingContainerChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTimeline))
        this.OnActiveTimelineChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.TriggersChanged))
        this.OnTriggersChanged();
      this.UpdateItems();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ElementSelection))
        this.OnElementSelectionSetChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.DependencyObjectSelection))
        this.OnDependencyObjectSelectionSetChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.KeyFrameSelection))
        this.OnKeyFrameSelectionSetChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.AnimationSelection))
        this.OnAnimationSelectionSetChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.BehaviorSelection))
        this.OnBehaviorSelectionSetChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ChildPropertySelection))
        this.OnChildPropertySelectionSetChanged();
      this.UpdateItems();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveSceneInsertionPoint | SceneViewModel.ViewStateBits.LockedInsertionPoint))
        this.OnSceneInsertionPointChanged();
      if (args.IsDirtyViewState(SceneViewModel.ViewStateBits.ActiveTrigger))
        this.OnActiveTriggerChanged();
      if (this.viewModel.TransitionEditTarget == null)
        return;
      this.UpdateTransitionStatus();
    }

    private ScheduledTimelineItem TimelineSubscription_TimelineInserted(object sender, SceneNode basisNode, object basisContent, SceneNode newPathNode)
    {
      this.transitionStatusDirty = true;
      TimelineSceneNode timelineSceneNode = (TimelineSceneNode) newPathNode;
      timelineSceneNode.Invalidate();
      if ((ScheduledTimelineItem) this.FindTimelineItem((SceneNode) timelineSceneNode) == null)
      {
        StoryboardTimelineSceneNode parentTimeline = newPathNode.Parent as StoryboardTimelineSceneNode;
        if (parentTimeline != null && newPathNode.StoryboardContainer != null && !AnimationProxyManager.IsOptimizedAnimation(timelineSceneNode))
        {
          this.BuildAnimationModelForAnimation(newPathNode.StoryboardContainer, parentTimeline, timelineSceneNode);
          this.OnLastAnimationTimeChanged();
          this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationGeneralChange);
          ScheduledTimelineItem scheduledTimelineItem = (ScheduledTimelineItem) this.FindTimelineItem(newPathNode);
        }
      }
      return (ScheduledTimelineItem) null;
    }

    private void TimelineSubscription_TimelineRemoved(object sender, SceneNode basisNode, object basisContent, SceneNode oldPathNode, ScheduledTimelineItem oldTimelineItem)
    {
      oldTimelineItem = (ScheduledTimelineItem) this.FindTimelineItem(oldPathNode);
      this.transitionStatusDirty = true;
      if (oldTimelineItem != null && oldTimelineItem.Parent != null)
        this.RemoveTimelineItem(oldTimelineItem, (TimelineSceneNode) oldPathNode);
      this.OnLastAnimationTimeChanged();
      this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationGeneralChange);
    }

    private void TimelineSubscription_TimelineContentChanged(object sender, SceneNode timelineNode, ScheduledTimelineItem timelineItem, DocumentNodeMarker damageMarker, DocumentNodeChange damage)
    {
      this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationGeneralChange);
      ((TimelineSceneNode) timelineNode).Invalidate();
      PropertyReference propertyReference = new PropertyReference(DocumentNodeMarkerUtilities.PropertyReferencePath(damageMarker, timelineNode.DocumentNode.Marker));
      ReferenceStepQuery referenceStepQuery = new ReferenceStepQuery(new ReferenceStepQuery.Step[2]
      {
        new ReferenceStepQuery.Step("KeyFrames"),
        new ReferenceStepQuery.Step(ReferenceStepQuery.WildcardType.Indexer)
      });
      timelineItem = (ScheduledTimelineItem) this.FindTimelineItem(timelineNode);
      if (timelineItem == null)
      {
        if (DesignTimeProperties.IsOptimizedProperty.Equals((object) propertyReference[0]))
        {
          this.RebuildAnimationModelForTargetOf(timelineNode);
          timelineItem = (ScheduledTimelineItem) this.FindTimelineItem(timelineNode);
        }
        if (timelineItem == null)
          return;
      }
      ReferenceStepQuery.MatchType matchType = referenceStepQuery.Test(propertyReference);
      switch (matchType)
      {
        case ReferenceStepQuery.MatchType.Exact:
        case ReferenceStepQuery.MatchType.WildCardContains:
          KeyFramedTimelineItem framedTimelineItem = timelineItem as KeyFramedTimelineItem;
          if (matchType == ReferenceStepQuery.MatchType.Exact && !damageMarker.IsProperty)
          {
            if (damage.Action == DocumentNodeChangeAction.Add)
            {
              if (damage.NewChildNode.IsInDocument)
              {
                KeyFrameSceneNode keyFrameSceneNode = (KeyFrameSceneNode) this.viewModel.GetSceneNode(damage.NewChildNode);
                if (KeyFrameItem.FindItem(keyFrameSceneNode.Time, (TimelineItem) framedTimelineItem) != (KeyFrameItem) null)
                  framedTimelineItem.RemoveKeyFrameItem(keyFrameSceneNode.Time);
                framedTimelineItem.AddKeyFrameItem(keyFrameSceneNode.Time);
                this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationAddKeyFrame);
                break;
              }
              break;
            }
            if (damage.Action == DocumentNodeChangeAction.Remove)
            {
              KeyFrameSceneNode keyFrameSceneNode = (KeyFrameSceneNode) this.viewModel.GetSceneNode(damage.OldChildNode);
              SimpleKeyFrameItem simpleKeyFrameItem = KeyFrameItem.FindItem(keyFrameSceneNode.Time, (TimelineItem) framedTimelineItem) as SimpleKeyFrameItem;
              if ((KeyFrameItem) simpleKeyFrameItem == (KeyFrameItem) null || simpleKeyFrameItem.KeyFrameSceneNode == null)
              {
                framedTimelineItem.RemoveKeyFrameItem(keyFrameSceneNode.Time);
                break;
              }
              break;
            }
            KeyFrameSceneNode keyFrameSceneNode1 = (KeyFrameSceneNode) this.viewModel.GetSceneNode(damage.OldChildNode);
            KeyFrameSceneNode keyFrameSceneNode2 = (KeyFrameSceneNode) this.viewModel.GetSceneNode(damage.NewChildNode);
            framedTimelineItem.RemoveKeyFrameItem(keyFrameSceneNode1.Time);
            framedTimelineItem.AddKeyFrameItem(keyFrameSceneNode2.Time);
            break;
          }
          if (matchType == ReferenceStepQuery.MatchType.WildCardContains || matchType == ReferenceStepQuery.MatchType.Exact && damageMarker.IsProperty)
          {
            object oldChildNodeValue = damage.OldChildNodeValue;
            object newChildNodeValue = damage.NewChildNodeValue;
            string str = string.Empty;
            if (propertyReference.Count == 2)
              str = damageMarker.Property.Name;
            else if (propertyReference.Count == 3)
              str = propertyReference[2].Name;
            if (str.Contains("KeyEase"))
              this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationChangeKeyEase);
            if (str.Contains("Time"))
            {
              DocumentPrimitiveNode documentPrimitiveNode1 = damage.OldChildNode as DocumentPrimitiveNode;
              DocumentPrimitiveNode documentPrimitiveNode2 = damage.NewChildNode as DocumentPrimitiveNode;
              KeyTime keyTime1 = documentPrimitiveNode1 != null ? documentPrimitiveNode1.GetValue<KeyTime>() : KeyTime.FromTimeSpan(TimeSpan.Zero);
              KeyTime keyTime2 = documentPrimitiveNode2 != null ? documentPrimitiveNode2.GetValue<KeyTime>() : KeyTime.FromTimeSpan(TimeSpan.Zero);
              double num1 = keyTime1.Type == KeyTimeType.TimeSpan ? keyTime1.TimeSpan.TotalSeconds : 0.0;
              double num2 = keyTime2.Type == KeyTimeType.TimeSpan ? keyTime2.TimeSpan.TotalSeconds : 0.0;
              if (documentPrimitiveNode1 != null && documentPrimitiveNode2 != null && (keyTime1.Type == KeyTimeType.TimeSpan && keyTime2.Type == KeyTimeType.TimeSpan))
              {
                KeyFrameAnimationSceneNode animationSceneNode = timelineNode as KeyFrameAnimationSceneNode;
                if (animationSceneNode != null && animationSceneNode.GetKeyFrameAtTime(num1) == null)
                  framedTimelineItem.MoveKeyFrameItem(num1, num2, this.TimelinePane.TimelineView.IsApplyingKeyFrameEdits);
                else
                  framedTimelineItem.AddKeyFrameItem(num2);
              }
              else if ((documentPrimitiveNode2 == null || keyTime2.Type != KeyTimeType.TimeSpan) && keyTime1.Type == KeyTimeType.TimeSpan)
                framedTimelineItem.RemoveKeyFrameItem(num1);
              else if ((documentPrimitiveNode1 == null || keyTime1.Type != KeyTimeType.TimeSpan) && keyTime2.Type == KeyTimeType.TimeSpan)
                framedTimelineItem.AddKeyFrameItem(num2);
              this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.AnimationMoveKeyFrame);
              break;
            }
            break;
          }
          break;
        default:
          string name = propertyReference[0].Name;
          if (name == "TargetProperty" || name == "KeyFrames" || name == "IsOptimized")
          {
            this.RebuildAnimationModelForTargetOf(timelineNode);
            break;
          }
          if (name == "From" || name == "To")
          {
            this.transitionStatusDirty = true;
            break;
          }
          timelineItem.UpdateScheduledProperties();
          break;
      }
      this.OnLastAnimationTimeChanged();
    }

    private void RebuildAnimationModelForTargetOf(SceneNode timelineNode)
    {
      if (this.ViewModel == null || this.ViewModel.DefaultView == null || this.ViewModel.DefaultView.MessageContent != null)
        return;
      AnimationSceneNode animationSceneNode = timelineNode as AnimationSceneNode;
      if (animationSceneNode == null)
        return;
      SceneNode targetElement = animationSceneNode.TargetElement;
      if (targetElement == null)
        return;
      this.RebuildAnimationModelForElement(targetElement);
    }

    private void OnDependencyObjectSelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneNode>((IEnumerable<SceneNode>) this.ViewModel.DependencyObjectSelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastDependencyObjectSelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          TimelineItem timelineItem = this.FindTimelineItem(SceneNode.FromMarker<SceneNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel));
          if (timelineItem != null)
          {
            this.EnsureExpandedAncestors(timelineItem);
            timelineItem.IsSelected = true;
          }
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          SceneNode sceneNode = SceneNode.FromMarker<SceneNode>(this.lastDependencyObjectSelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (sceneNode != null)
          {
            TimelineItem timelineItem = this.FindTimelineItem(sceneNode);
            if (timelineItem != null)
              timelineItem.IsSelected = false;
          }
        }
      }
      this.lastDependencyObjectSelectionSet = markerList;
    }

    private void OnElementSelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneElement>((IEnumerable<SceneElement>) this.ViewModel.ElementSelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastSceneElementSelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          TimelineItem timelineItem = this.FindTimelineItem((SceneNode) SceneNode.FromMarker<SceneElement>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel));
          if (timelineItem != null)
          {
            this.EnsureExpandedAncestors(timelineItem);
            timelineItem.IsSelected = true;
          }
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          SceneElement sceneElement = SceneNode.FromMarker<SceneElement>(this.lastSceneElementSelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (sceneElement != null)
          {
            TimelineItem timelineItem = this.FindTimelineItem((SceneNode) sceneElement);
            if (timelineItem != null)
              timelineItem.IsSelected = false;
          }
        }
      }
      this.lastSceneElementSelectionSet = markerList;
    }

    private void OnKeyFrameSelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<KeyFrameSceneNode>((IEnumerable<KeyFrameSceneNode>) this.ViewModel.KeyFrameSelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastKeyFrameSelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          KeyFrameSceneNode keyFrameSceneNode = SceneNode.FromMarker<KeyFrameSceneNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel);
          StoryboardTimelineSceneNode controllingStoryboard = keyFrameSceneNode.ControllingStoryboard;
          IStoryboardContainer storyboardContainer = keyFrameSceneNode.StoryboardContainer;
          SimpleKeyFrameItem simpleKeyFrameItem = (SimpleKeyFrameItem) null;
          if (controllingStoryboard != null && storyboardContainer != null)
            simpleKeyFrameItem = this.FindSimpleKeyFrameItem(keyFrameSceneNode, storyboardContainer, controllingStoryboard);
          if ((KeyFrameItem) simpleKeyFrameItem != (KeyFrameItem) null)
            simpleKeyFrameItem.IsSelected = true;
          list.Add(keyFrameSceneNode);
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          KeyFrameSceneNode keyFrameSceneNode = SceneNode.FromMarker<KeyFrameSceneNode>(this.lastKeyFrameSelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (keyFrameSceneNode != null)
          {
            StoryboardTimelineSceneNode controllingStoryboard = keyFrameSceneNode.ControllingStoryboard;
            IStoryboardContainer storyboardContainer = keyFrameSceneNode.StoryboardContainer;
            SimpleKeyFrameItem simpleKeyFrameItem = (SimpleKeyFrameItem) null;
            if (controllingStoryboard != null && storyboardContainer != null)
              simpleKeyFrameItem = this.FindSimpleKeyFrameItem(keyFrameSceneNode, storyboardContainer, controllingStoryboard);
            if ((KeyFrameItem) simpleKeyFrameItem != (KeyFrameItem) null)
            {
              simpleKeyFrameItem.IsSelected = false;
              list.Add(keyFrameSceneNode);
            }
          }
        }
      }
      this.lastKeyFrameSelectionSet = markerList;
    }

    private void UpdateCompoundPropertyTimelineItemSelection(CompoundPropertyTimelineItem compoundPropertyTimelineItem, bool shouldBeSelected)
    {
    }

    private void OnAnimationSelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<AnimationSceneNode>((IEnumerable<AnimationSceneNode>) this.ViewModel.AnimationSelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastAnimationSelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          ScheduledTimelineItem scheduledTimelineItem = (ScheduledTimelineItem) this.FindTimelineItem((SceneNode) SceneNode.FromMarker<AnimationSceneNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel));
          if (scheduledTimelineItem != null)
          {
            scheduledTimelineItem.IsSelected = true;
            CompoundPropertyTimelineItem compoundPropertyTimelineItem = scheduledTimelineItem.Parent as CompoundPropertyTimelineItem;
            if (compoundPropertyTimelineItem != null)
              this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, true);
          }
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          AnimationSceneNode animationSceneNode = SceneNode.FromMarker<AnimationSceneNode>(this.lastAnimationSelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (animationSceneNode != null)
          {
            ScheduledTimelineItem scheduledTimelineItem = (ScheduledTimelineItem) this.FindTimelineItem((SceneNode) animationSceneNode);
            if (scheduledTimelineItem != null)
            {
              scheduledTimelineItem.IsSelected = false;
              CompoundPropertyTimelineItem compoundPropertyTimelineItem = scheduledTimelineItem.Parent as CompoundPropertyTimelineItem;
              if (compoundPropertyTimelineItem != null)
                this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, false);
            }
          }
        }
      }
      this.lastAnimationSelectionSet = markerList;
    }

    private void OnChildPropertySelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<SceneNode>((IEnumerable<SceneNode>) this.ViewModel.ChildPropertySelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastChildPropertySelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          SceneNode child = SceneNode.FromMarker<SceneNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel);
          ChildPropertyTimelineItem propertyTimelineItem = this.FindChildPropertyTimelineItem(child.Parent, (IPropertyId) child.Parent.GetPropertyForChild(child));
          if (propertyTimelineItem != null)
          {
            propertyTimelineItem.IsSelected = true;
            if (propertyTimelineItem.ExpandParentOnInsertion)
              propertyTimelineItem.Parent.IsExpanded = true;
            propertyTimelineItem.IsExpanded = true;
            CompoundPropertyTimelineItem compoundPropertyTimelineItem = propertyTimelineItem.Parent as CompoundPropertyTimelineItem;
            if (compoundPropertyTimelineItem != null)
              this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, true);
          }
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          SceneNode child = SceneNode.FromMarker<SceneNode>(this.lastChildPropertySelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (child != null)
          {
            ChildPropertyTimelineItem propertyTimelineItem = this.FindChildPropertyTimelineItem(child.Parent, (IPropertyId) child.Parent.GetPropertyForChild(child));
            if (propertyTimelineItem != null)
            {
              propertyTimelineItem.IsSelected = false;
              CompoundPropertyTimelineItem compoundPropertyTimelineItem = propertyTimelineItem.Parent as CompoundPropertyTimelineItem;
              if (compoundPropertyTimelineItem != null)
                this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, false);
            }
          }
        }
      }
      this.lastChildPropertySelectionSet = markerList;
    }

    private void OnBehaviorSelectionSetChanged()
    {
      if (this.ViewModel == null)
        return;
      DocumentNodeMarkerSortedList markerList = SceneNode.GetMarkerList<BehaviorBaseNode>((IEnumerable<BehaviorBaseNode>) this.ViewModel.BehaviorSelectionSet.Selection, true);
      foreach (DocumentNodeMarkerSortedListBase.IntersectionResult intersectionResult in this.lastBehaviorSelectionSet.UnionIdentity((DocumentNodeMarkerSortedListBase) markerList))
      {
        if (intersectionResult.LeftHandSideIndex == -1)
        {
          BehaviorTimelineItem behaviorTimelineItem = (BehaviorTimelineItem) this.FindTimelineItem((SceneNode) SceneNode.FromMarker<BehaviorBaseNode>(markerList.MarkerAt(intersectionResult.RightHandSideIndex), this.ViewModel));
          if (behaviorTimelineItem != null)
          {
            behaviorTimelineItem.IsSelected = true;
            CompoundPropertyTimelineItem compoundPropertyTimelineItem = behaviorTimelineItem.Parent as CompoundPropertyTimelineItem;
            if (compoundPropertyTimelineItem != null)
              this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, true);
            this.EnsureExpandedAncestors((TimelineItem) behaviorTimelineItem);
          }
        }
        else if (intersectionResult.RightHandSideIndex == -1)
        {
          BehaviorBaseNode behaviorBaseNode = SceneNode.FromMarker<BehaviorBaseNode>(this.lastBehaviorSelectionSet.MarkerAt(intersectionResult.LeftHandSideIndex), this.ViewModel);
          if (behaviorBaseNode != null)
          {
            BehaviorTimelineItem behaviorTimelineItem = (BehaviorTimelineItem) this.FindTimelineItem((SceneNode) behaviorBaseNode);
            if (behaviorTimelineItem != null)
            {
              behaviorTimelineItem.IsSelected = false;
              CompoundPropertyTimelineItem compoundPropertyTimelineItem = behaviorTimelineItem.Parent as CompoundPropertyTimelineItem;
              if (compoundPropertyTimelineItem != null)
                this.UpdateCompoundPropertyTimelineItemSelection(compoundPropertyTimelineItem, false);
            }
          }
        }
      }
      this.lastBehaviorSelectionSet = markerList;
    }

    private double RecalculateLastAnimationTime(TimelineItem timelineItem)
    {
      ScheduledTimelineItem scheduledTimelineItem = timelineItem as ScheduledTimelineItem;
      double val1 = 0.0;
      if (scheduledTimelineItem != null && scheduledTimelineItem.IsInActiveTimeline)
      {
        val1 = Math.Max(Math.Max(val1, scheduledTimelineItem.NaturalEnd), scheduledTimelineItem.AbsoluteClipEnd);
        if (!double.IsInfinity(scheduledTimelineItem.RepeatCount))
          val1 = Math.Max(val1, scheduledTimelineItem.RepeatDuration + scheduledTimelineItem.Begin);
      }
      for (int index = 0; index < timelineItem.Children.Count; ++index)
        val1 = Math.Max(val1, this.RecalculateLastAnimationTime(timelineItem.Children[index]));
      return val1;
    }

    private void OnLastAnimationTimeChanged()
    {
      this.isLastAnimationTimeDirty = true;
      this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.TimelineLastAnimationTime);
    }

    private void OnSortByZOrderChanged()
    {
      this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.TimelineSortZOrder);
    }

    private void OnScopedTimelineItemChanged()
    {
      this.viewModel.SetDirtyPipelineCalcBits(SceneViewModel.PipelineCalcBits.TimelineScopedTimelineItem);
      this.flattener.RebuildList();
    }

    internal static bool UseStoryboardContainerAsAnimationParent(IStoryboardContainer storyboardContainer, SceneNode targetElement)
    {
      if (storyboardContainer is StyleNode || storyboardContainer is FrameworkTemplateElement)
        return storyboardContainer.TargetElement == targetElement;
      return false;
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
