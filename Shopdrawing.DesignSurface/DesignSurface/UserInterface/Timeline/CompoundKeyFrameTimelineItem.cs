// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.CompoundKeyFrameTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public abstract class CompoundKeyFrameTimelineItem : TimelineItem, IKeyFramedTimelineItem, ITimedTimelineItem
  {
    private double originalBegin = double.NegativeInfinity;
    private double originalEnd = double.NegativeInfinity;
    private ObservableCollection<KeyFrameItem> keyFrameItems;
    private double begin;
    private double duration;
    private bool keyFramesDirty;
    private bool hasAnimation;
    private bool shouldShowTimeBar;

    public abstract bool ShouldBubbleKeyFrames { get; }

    public ICommand BeginScheduledPropertiesEditCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.BeginScheduledPropertiesEdit));
      }
    }

    public ICommand EndScheduledPropertiesEditCommand
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.EndScheduledPropertiesEdit));
      }
    }

    public double Begin
    {
      get
      {
        return this.begin;
      }
      set
      {
        this.begin = value;
        this.OnPropertyChanged("Begin");
        this.OnPropertyChanged("AbsoluteClipBegin");
        this.OnPropertyChanged("AbsoluteClipEnd");
        this.OnPropertyChanged("AbsoluteBeginTime");
        this.OnPropertyChanged("AbsoluteEndTime");
      }
    }

    public double Duration
    {
      get
      {
        return this.duration;
      }
      set
      {
        this.duration = value;
        this.OnPropertyChanged("Duration");
        this.OnPropertyChanged("AbsoluteClipEnd");
        this.OnPropertyChanged("AbsoluteEndTime");
      }
    }

    public double AbsoluteClipBegin
    {
      get
      {
        return this.begin;
      }
      set
      {
        this.Duration = this.AbsoluteClipEnd - value;
        this.Begin = value;
        this.OnPropertyChanged("AbsoluteClipBegin");
      }
    }

    public double AbsoluteClipEnd
    {
      get
      {
        return this.begin + this.duration;
      }
      set
      {
        this.Duration = value - this.begin;
        this.OnPropertyChanged("AbsoluteClipEnd");
      }
    }

    public ObservableCollection<KeyFrameItem> KeyFrameItems
    {
      get
      {
        return this.keyFrameItems;
      }
    }

    public double AbsoluteBeginTime
    {
      get
      {
        return this.begin;
      }
    }

    public double AbsoluteEndTime
    {
      get
      {
        return this.begin + this.duration;
      }
    }

    public virtual bool ShouldBubbleTimes
    {
      get
      {
        return this.IsActive;
      }
    }

    public virtual bool ShouldShowTimeBar
    {
      get
      {
        return this.shouldShowTimeBar;
      }
    }

    public bool DescendantOnlyHasAnimation { get; set; }

    public virtual bool HasAnimationVisual
    {
      get
      {
        return false;
      }
    }

    protected bool HasAnimation
    {
      get
      {
        if (this.keyFrameItems.Count <= 0)
          return this.hasAnimation;
        return true;
      }
      set
      {
        this.hasAnimation = value;
      }
    }

    public override bool IsExpanded
    {
      get
      {
        return base.IsExpanded;
      }
      set
      {
        base.IsExpanded = value;
        this.OnPropertyChanged("HasAnimationVisual");
      }
    }

    protected virtual bool IsCompoundKeyFrameRoot
    {
      get
      {
        return false;
      }
    }

    protected CompoundKeyFrameTimelineItem(TimelineItemManager timelineItemManager)
      : base(timelineItemManager)
    {
      this.keyFrameItems = new ObservableCollection<KeyFrameItem>();
    }

    public virtual void OnKeyFrameSelectionChanged()
    {
    }

    public override void OnActiveTimelineContextChanged()
    {
      base.OnActiveTimelineContextChanged();
      this.DirtyKeyFrames();
    }

    protected override void ChildPropertyChanged(string propertyName)
    {
      base.ChildPropertyChanged(propertyName);
      if (this.TimelineItemManager.IsInValidationPass)
        return;
      if (propertyName == "KeyFrameItems" || propertyName == "Begin" || propertyName == "Duration")
      {
        this.DirtyKeyFrames();
      }
      else
      {
        if (!(propertyName == "IsActive"))
          return;
        this.DirtyHasAnimation();
        this.DirtyKeyFrames();
      }
    }

    protected override void ChildAdded(TimelineItem child)
    {
      if (child is IKeyFramedTimelineItem)
        this.DirtyKeyFrames();
      else if (child is AnimationTimelineItem)
        this.DirtyHasAnimation();
      base.ChildAdded(child);
    }

    protected override void ChildRemoved(TimelineItem child)
    {
      if (child is IKeyFramedTimelineItem)
        this.DirtyKeyFrames();
      else if (child is AnimationTimelineItem)
        this.DirtyHasAnimation();
      base.ChildRemoved(child);
    }

    protected virtual void KeyFramesRebuilt()
    {
    }

    protected override void ValidateCore()
    {
      base.ValidateCore();
      if (this.keyFramesDirty)
      {
        this.RebuildKeyFrames();
        this.keyFramesDirty = false;
        this.OnPropertyChanged("HasAnimationVisual");
      }
      this.HasAnimation = false;
      this.DescendantOnlyHasAnimation = false;
      foreach (TimelineItem timelineItem in (Collection<TimelineItem>) this.Children)
      {
        AnimationTimelineItem animationTimelineItem = timelineItem as AnimationTimelineItem;
        if (animationTimelineItem != null && animationTimelineItem.IsInActiveTimeline)
        {
          this.HasAnimation = true;
          break;
        }
        CompoundKeyFrameTimelineItem frameTimelineItem = timelineItem as CompoundKeyFrameTimelineItem;
        if (frameTimelineItem != null)
        {
          if (!frameTimelineItem.IsCompoundKeyFrameRoot && frameTimelineItem.HasAnimation)
          {
            this.HasAnimation = true;
            break;
          }
          if (frameTimelineItem.HasAnimation || frameTimelineItem.DescendantOnlyHasAnimation)
            this.DescendantOnlyHasAnimation = true;
        }
      }
      this.OnPropertyChanged("HasAnimationVisual");
    }

    private void RebuildKeyFrames()
    {
      if (this.TimelineItemManager.IsInitializing)
        return;
      bool flag1 = false;
      if (this.keyFrameItems.Count > 0)
      {
        foreach (CompoundKeyFrameItem compoundKeyFrameItem in (Collection<KeyFrameItem>) this.keyFrameItems)
        {
          compoundKeyFrameItem.RemoveFromModel();
          compoundKeyFrameItem.ChildKeyFrameTimeChanged -= new EventHandler(this.CompoundKeyFrameItem_ChildKeyFrameTimeChanged);
        }
        this.keyFrameItems.Clear();
        flag1 = true;
      }
      double num1 = this.begin;
      double num2 = this.duration;
      double num3 = double.PositiveInfinity;
      double val1 = 0.0;
      bool flag2 = false;
      if (this.Children.Count > 0)
      {
        Dictionary<double, CompoundKeyFrameItem> dictionary = (Dictionary<double, CompoundKeyFrameItem>) null;
        for (int index = 0; index < this.Children.Count; ++index)
        {
          TimelineItem timelineItem = this.Children[index];
          if (timelineItem != null)
          {
            IKeyFramedTimelineItem framedTimelineItem = timelineItem as IKeyFramedTimelineItem;
            ITimedTimelineItem timedTimelineItem = timelineItem as ITimedTimelineItem;
            if (timedTimelineItem != null && timedTimelineItem.ShouldBubbleTimes)
            {
              num3 = Math.Min(num3, timedTimelineItem.AbsoluteBeginTime);
              val1 = Math.Max(val1, timedTimelineItem.AbsoluteEndTime);
              flag2 |= timedTimelineItem.ShouldShowTimeBar;
            }
            if (framedTimelineItem != null && framedTimelineItem.ShouldBubbleKeyFrames)
            {
              if (dictionary == null)
                dictionary = new Dictionary<double, CompoundKeyFrameItem>();
              foreach (KeyFrameItem keyFrameItem in (Collection<KeyFrameItem>) framedTimelineItem.KeyFrameItems)
              {
                CompoundKeyFrameItem compoundKeyFrameItem;
                if (dictionary.TryGetValue(keyFrameItem.Time, out compoundKeyFrameItem))
                {
                  compoundKeyFrameItem.AddKeyFrameItem(keyFrameItem);
                }
                else
                {
                  compoundKeyFrameItem = new CompoundKeyFrameItem(keyFrameItem.Time, (TimelineItem) this);
                  compoundKeyFrameItem.AddKeyFrameItem(keyFrameItem);
                  compoundKeyFrameItem.ChildKeyFrameTimeChanged += new EventHandler(this.CompoundKeyFrameItem_ChildKeyFrameTimeChanged);
                  dictionary[keyFrameItem.Time] = compoundKeyFrameItem;
                  num3 = Math.Min(num3, keyFrameItem.Time);
                  val1 = Math.Max(val1, keyFrameItem.Time);
                }
              }
            }
          }
        }
        if (dictionary != null)
        {
          List<CompoundKeyFrameItem> list = new List<CompoundKeyFrameItem>(dictionary.Count);
          list.AddRange((IEnumerable<CompoundKeyFrameItem>) dictionary.Values);
          list.Sort();
          foreach (KeyFrameItem keyFrameItem in list)
          {
            this.keyFrameItems.Add(keyFrameItem);
            flag1 = true;
          }
        }
        if (this.shouldShowTimeBar != flag2)
        {
          this.shouldShowTimeBar = flag2;
          this.OnPropertyChanged("ShouldShowTimeBar");
        }
      }
      this.Begin = !double.IsPositiveInfinity(num3) ? num3 : 0.0;
      this.Duration = val1 - this.begin;
      if (flag1)
        this.OnPropertyChanged("KeyFrameItems");
      if (this.begin != num1)
        this.OnPropertyChanged("Begin");
      if (this.duration != num2)
        this.OnPropertyChanged("Duration");
      this.KeyFramesRebuilt();
    }

    private void CompoundKeyFrameItem_ChildKeyFrameTimeChanged(object sender, EventArgs e)
    {
      if (this.TimelinePane.TimelineView.EditingKeyFrameItem == (KeyFrameItem) null)
      {
        this.DirtyKeyFrames();
      }
      else
      {
        foreach (CompoundKeyFrameItem compoundKeyFrameItem in (Collection<KeyFrameItem>) this.keyFrameItems)
        {
          if (compoundKeyFrameItem.IsSelected)
            compoundKeyFrameItem.UpdateTimeFromChildren();
        }
      }
    }

    private void DirtyKeyFrames()
    {
      for (CompoundKeyFrameTimelineItem frameTimelineItem = this; frameTimelineItem != null && !frameTimelineItem.keyFramesDirty; frameTimelineItem = frameTimelineItem.Parent as CompoundKeyFrameTimelineItem)
      {
        frameTimelineItem.keyFramesDirty = true;
        if (frameTimelineItem.IsCompoundKeyFrameRoot)
          break;
      }
      this.Invalidate();
    }

    private void DirtyHasAnimation()
    {
      this.Invalidate();
    }

    private void BeginScheduledPropertiesEdit()
    {
      this.originalBegin = this.begin;
      this.originalEnd = this.AbsoluteClipEnd;
      this.TimelinePane.TimelineView.BeginTimedItemEdit((ITimedTimelineItem) this);
    }

    private void EndScheduledPropertiesEdit()
    {
      try
      {
        using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(StringTable.EditScheduledPropertiesUndo))
        {
          this.RecursiveUpdateScheduledProperties((TimelineItem) this);
          editTransaction.Commit();
        }
      }
      finally
      {
        this.originalBegin = double.NegativeInfinity;
        this.originalEnd = double.NegativeInfinity;
        this.TimelinePane.TimelineView.EndTimedItemEdit((ITimedTimelineItem) this);
      }
    }

    private void RecursiveUpdateScheduledProperties(TimelineItem item)
    {
      ITimedTimelineItem timedTimelineItem = item as ITimedTimelineItem;
      if (item != this && timedTimelineItem != null && !timedTimelineItem.ShouldBubbleTimes)
        return;
      if (item.Children != null && item.Children.Count > 0)
      {
        for (int index = 0; index < item.Children.Count; ++index)
        {
          TimelineItem timelineItem = item.Children[index];
          if (timelineItem != null)
            this.RecursiveUpdateScheduledProperties(timelineItem);
        }
      }
      item.CommitScheduledPropertyChanges(this.originalBegin, this.originalEnd, this.begin, this.AbsoluteClipEnd);
    }
  }
}
