// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.ScheduledTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public abstract class ScheduledTimelineItem : TimelineItem, ITimedTimelineItem
  {
    private List<TimelineSceneNode> scheduledAnimations;
    private StoryboardTimelineSceneNode parentTimeline;
    private bool editingProperties;
    private double newBeginTime;
    private double newClipBeginTime;
    private double newClipEndTime;

    public virtual ICommand EditRepeatCountCommand
    {
      get
      {
        return (ICommand) null;
      }
    }

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

    public StoryboardTimelineSceneNode ParentTimeline
    {
      get
      {
        return this.parentTimeline;
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return this.IsInActiveTimeline;
      }
    }

    public virtual bool IsInActiveTimeline
    {
      get
      {
        if (this.TimelineItemManager.AnimationEditor != null)
          return this.parentTimeline == this.TimelineItemManager.AnimationEditor.ActiveStoryboardTimeline;
        return false;
      }
    }

    public virtual double Begin
    {
      get
      {
        if (this.editingProperties)
          return this.newBeginTime;
        return this.PrimaryScheduledAnimation.Begin;
      }
      set
      {
        this.newBeginTime = value;
        this.newClipBeginTime = value;
        this.newClipEndTime = value + this.PrimaryScheduledAnimation.CalculatedDuration;
        this.OnPropertyChanged("Begin");
        this.OnPropertyChanged("AbsoluteClipBegin");
        this.OnPropertyChanged("AbsoluteClipEnd");
      }
    }

    public virtual double NaturalBegin
    {
      get
      {
        return this.Begin - this.ClipBegin;
      }
    }

    public virtual double NaturalEnd
    {
      get
      {
        return this.Begin + this.NaturalDuration;
      }
    }

    public virtual double NaturalDuration
    {
      get
      {
        return this.PrimaryScheduledAnimation.NaturalDuration;
      }
    }

    public virtual double ActiveDuration
    {
      get
      {
        if (this.editingProperties)
          return this.newClipEndTime - this.newClipBeginTime;
        return this.PrimaryScheduledAnimation.ActiveDuration;
      }
    }

    public virtual double ClipBegin
    {
      get
      {
        return this.PrimaryScheduledAnimation.ClipBegin;
      }
    }

    public virtual double AbsoluteClipBegin
    {
      get
      {
        return this.Begin;
      }
      set
      {
        this.newBeginTime = value;
        this.newClipBeginTime = value;
        this.OnPropertyChanged("Begin");
        this.OnPropertyChanged("AbsoluteClipBegin");
        this.OnPropertyChanged("ActiveDuration");
      }
    }

    public virtual double AbsoluteClipEnd
    {
      get
      {
        if (this.editingProperties)
          return this.newClipEndTime;
        return this.NaturalBegin + this.ClipEnd;
      }
      set
      {
        this.newClipEndTime = value;
        this.OnPropertyChanged("AbsoluteClipEnd");
        this.OnPropertyChanged("ActiveDuration");
      }
    }

    public virtual double ClipEnd
    {
      get
      {
        return this.PrimaryScheduledAnimation.ClipEnd;
      }
    }

    public virtual double RepeatCount
    {
      get
      {
        return this.PrimaryScheduledAnimation.RepeatCount;
      }
    }

    public virtual double RepeatDuration
    {
      get
      {
        if (this.ActiveDuration == 0.0)
          return 0.0;
        return this.RepeatCount * this.ActiveDuration;
      }
    }

    protected ReadOnlyCollection<TimelineSceneNode> ScheduledAnimations
    {
      get
      {
        return this.scheduledAnimations.AsReadOnly();
      }
    }

    protected TimelineSceneNode PrimaryScheduledAnimation
    {
      get
      {
        return this.scheduledAnimations[0];
      }
    }

    public virtual double AbsoluteBeginTime
    {
      get
      {
        return this.AbsoluteClipBegin;
      }
    }

    public virtual double AbsoluteEndTime
    {
      get
      {
        return this.AbsoluteClipEnd;
      }
    }

    public virtual bool ShouldBubbleTimes
    {
      get
      {
        return this.IsActive;
      }
    }

    public bool ShouldShowTimeBar
    {
      get
      {
        return true;
      }
    }

    protected ScheduledTimelineItem(TimelineItemManager timelineItemManager, StoryboardTimelineSceneNode parentTimeline, TimelineSceneNode scheduledAnimation)
      : base(timelineItemManager)
    {
      this.scheduledAnimations = new List<TimelineSceneNode>();
      this.AddScheduledAnimation(scheduledAnimation);
      this.parentTimeline = parentTimeline;
    }

    protected void AddScheduledAnimation(TimelineSceneNode scheduledAnimation)
    {
      this.scheduledAnimations.Add(scheduledAnimation);
      this.TimelineItemManager.RegisterTimelineItem((SceneNode) scheduledAnimation, (TimelineItem) this);
    }

    protected void InsertScheduledAnimation(int index, TimelineSceneNode scheduledAnimation)
    {
      this.scheduledAnimations.Insert(index, scheduledAnimation);
      this.TimelineItemManager.RegisterTimelineItem((SceneNode) scheduledAnimation, (TimelineItem) this);
    }

    protected void RemoveScheduledAnimation(TimelineSceneNode scheduledAnimation)
    {
      this.scheduledAnimations.Remove(scheduledAnimation);
      this.TimelineItemManager.RemoveTimelineItem((SceneNode) scheduledAnimation);
    }

    public void UpdateScheduledProperties()
    {
      this.SendTimingChangeNotifications();
    }

    public override void OnZoomChanged()
    {
      base.OnZoomChanged();
      this.SendTimingChangeNotifications();
    }

    public static ScheduledTimelineItem FindItem(TimelineItem parentTimelineItem, StoryboardTimelineSceneNode parentTimeline, PropertyReference targetProperty)
    {
      if (targetProperty == null)
        return (ScheduledTimelineItem) null;
      foreach (TimelineItem timelineItem in (Collection<TimelineItem>) parentTimelineItem.Children)
      {
        AnimationTimelineItem animationTimelineItem = timelineItem as AnimationTimelineItem;
        if (animationTimelineItem != null && animationTimelineItem.parentTimeline == parentTimeline && animationTimelineItem.TargetProperty.Path == targetProperty.Path)
          return (ScheduledTimelineItem) animationTimelineItem;
        CompoundPropertyTimelineItem propertyTimelineItem = timelineItem as CompoundPropertyTimelineItem;
        if (propertyTimelineItem != null)
        {
          ScheduledTimelineItem scheduledTimelineItem = ScheduledTimelineItem.FindItem((TimelineItem) propertyTimelineItem, parentTimeline, targetProperty);
          if (scheduledTimelineItem != null)
            return scheduledTimelineItem;
        }
        BehaviorTimelineItem behaviorTimelineItem = timelineItem as BehaviorTimelineItem;
        if (behaviorTimelineItem != null)
        {
          ScheduledTimelineItem scheduledTimelineItem = ScheduledTimelineItem.FindItem((TimelineItem) behaviorTimelineItem, parentTimeline, targetProperty);
          if (scheduledTimelineItem != null)
            return scheduledTimelineItem;
        }
        EffectTimelineItem effectTimelineItem = timelineItem as EffectTimelineItem;
        if (effectTimelineItem != null)
        {
          ScheduledTimelineItem scheduledTimelineItem = ScheduledTimelineItem.FindItem((TimelineItem) effectTimelineItem, parentTimeline, targetProperty);
          if (scheduledTimelineItem != null)
            return scheduledTimelineItem;
        }
      }
      return (ScheduledTimelineItem) null;
    }

    protected void SendTimingChangeNotifications()
    {
      this.OnPropertyChanged("NaturalBegin");
      this.OnPropertyChanged("NaturalEnd");
      this.OnPropertyChanged("NaturalDuration");
      this.OnPropertyChanged("Begin");
      this.OnPropertyChanged("ActiveDuration");
      this.OnPropertyChanged("ClipBegin");
      this.OnPropertyChanged("ClipEnd");
      this.OnPropertyChanged("AbsoluteClipBegin");
      this.OnPropertyChanged("AbsoluteClipEnd");
      this.OnPropertyChanged("RepeatCount");
      this.OnPropertyChanged("RepeatDuration");
    }

    protected override void OnRemoved()
    {
      foreach (SceneNode sceneNode in this.ScheduledAnimations)
        this.TimelineItemManager.RemoveTimelineItem(sceneNode);
      base.OnRemoved();
    }

    public override void CommitScheduledPropertyChanges(double originalRegionBegin, double originalRegionEnd, double finalRegionBegin, double finalRegionEnd)
    {
      base.CommitScheduledPropertyChanges(originalRegionBegin, originalRegionEnd, finalRegionBegin, finalRegionEnd);
      for (int index = 0; index < this.scheduledAnimations.Count; ++index)
        this.scheduledAnimations[index].SetTimeRegion(originalRegionBegin, originalRegionEnd, finalRegionBegin, finalRegionEnd);
    }

    protected void EditRepeatCount()
    {
      double repeatCount = this.RepeatCount;
      if (!this.TimelinePane.GetRepeatCount(ref repeatCount))
        return;
      using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(StringTable.UndoUnitRepeatCount))
      {
        foreach (TimelineSceneNode timeline in this.scheduledAnimations)
          this.TimelineItemManager.AnimationEditor.SetRepeatCount(timeline, repeatCount);
        editTransaction.Commit();
      }
    }

    private void BeginScheduledPropertiesEdit()
    {
      this.newBeginTime = this.Begin;
      this.newClipBeginTime = this.ClipBegin;
      this.newClipEndTime = this.ClipEnd;
      this.editingProperties = true;
      this.TimelinePane.TimelineView.BeginTimedItemEdit((ITimedTimelineItem) this);
    }

    private void EndScheduledPropertiesEdit()
    {
      using (SceneEditTransaction editTransaction = this.TimelineItemManager.ViewModel.CreateEditTransaction(StringTable.EditScheduledPropertiesUndo))
      {
        foreach (TimelineSceneNode timeline in this.scheduledAnimations)
          this.TimelineItemManager.AnimationEditor.MoveScheduledProperties(timeline, this.newBeginTime, this.newClipBeginTime, this.newClipEndTime);
        editTransaction.Commit();
        this.editingProperties = false;
      }
      this.TimelinePane.TimelineView.EndTimedItemEdit((ITimedTimelineItem) this);
    }
  }
}
