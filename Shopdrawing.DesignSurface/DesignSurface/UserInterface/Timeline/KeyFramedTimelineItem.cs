// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.KeyFramedTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public abstract class KeyFramedTimelineItem : AnimationTimelineItem, IKeyFramedTimelineItem, ITimedTimelineItem
  {
    private ObservableCollection<KeyFrameItem> keyFrameItems;
    private SceneElement element;
    private bool hasSelectedKeyFrame;
    private bool hasSelectedKeyFrameDirty;

    public ObservableCollection<KeyFrameItem> KeyFrameItems
    {
      get
      {
        return this.keyFrameItems;
      }
    }

    public bool HasSelectedKeyFrame
    {
      get
      {
        return this.hasSelectedKeyFrame;
      }
      protected set
      {
        if (this.hasSelectedKeyFrame == value)
          return;
        this.hasSelectedKeyFrame = value;
        this.OnPropertyChanged("HasSelectedKeyFrame");
      }
    }

    public KeyFrameAnimationSceneNode KeyFrameAnimationSceneNode
    {
      get
      {
        return (KeyFrameAnimationSceneNode) this.PrimaryScheduledAnimation;
      }
    }

    public SceneElement Element
    {
      get
      {
        return this.element;
      }
    }

    public abstract bool ShouldBubbleKeyFrames { get; }

    public override double AbsoluteBeginTime
    {
      get
      {
        if (this.keyFrameItems != null && this.keyFrameItems.Count > 0)
          return this.keyFrameItems[0].Time;
        return base.AbsoluteBeginTime;
      }
    }

    public override double AbsoluteEndTime
    {
      get
      {
        return this.NaturalDuration;
      }
    }

    protected KeyFramedTimelineItem(TimelineItemManager timelineItemManager, SceneElement element, StoryboardTimelineSceneNode parentTimeline, KeyFrameAnimationSceneNode keyFramedAnimationSceneNode)
      : base(timelineItemManager, parentTimeline, (AnimationSceneNode) keyFramedAnimationSceneNode)
    {
      this.keyFrameItems = new ObservableCollection<KeyFrameItem>();
      this.element = element;
      foreach (KeyFrameSceneNode keyFrameSceneNode in keyFramedAnimationSceneNode.KeyFrames)
        this.AddKeyFrameItem(keyFrameSceneNode.Time);
    }

    public void OnKeyFrameSelectionChanged()
    {
      this.DirtyKeyFrameSelection();
    }

    private void DirtyKeyFrameSelection()
    {
      this.hasSelectedKeyFrameDirty = true;
      CompoundPropertyTimelineItem propertyTimelineItem = this.Parent as CompoundPropertyTimelineItem;
      if (propertyTimelineItem != null)
        propertyTimelineItem.DirtyHasSelectedKeyFrame();
      this.Invalidate();
    }

    public void RemoveKeyFrameItem(double time)
    {
      KeyFrameItem keyFrameItem1 = (KeyFrameItem) null;
      foreach (KeyFrameItem keyFrameItem2 in (Collection<KeyFrameItem>) this.KeyFrameItems)
      {
        if (keyFrameItem2.IsOldTimeSet)
        {
          if (keyFrameItem2.OldTime == time)
            keyFrameItem1 = keyFrameItem2;
        }
        else if (keyFrameItem2.Time == time)
          keyFrameItem1 = keyFrameItem2;
        if (keyFrameItem2.IsSelected)
        {
          int num = keyFrameItem2 != keyFrameItem1 ? true : false;
        }
      }
      if (keyFrameItem1 != (KeyFrameItem) null)
      {
        keyFrameItem1.RemoveFromModel();
        this.KeyFrameItems.Remove(keyFrameItem1);
        this.OnPropertyChanged("KeyFrameItems");
      }
      this.SendTimingChangeNotifications();
    }

    public void AddKeyFrameItem(double time)
    {
      if (!(KeyFrameItem.FindItem(time, (TimelineItem) this) == (KeyFrameItem) null))
        return;
      SimpleKeyFrameItem simpleKeyFrameItem = new SimpleKeyFrameItem(time, this);
      for (int index = 0; index <= this.KeyFrameItems.Count; ++index)
      {
        if (index == this.KeyFrameItems.Count || this.KeyFrameItems[index].Time > time)
        {
          this.KeyFrameItems.Insert(index, (KeyFrameItem) simpleKeyFrameItem);
          break;
        }
      }
      if (simpleKeyFrameItem.KeyFrameSceneNode != null && this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.IsSelected(simpleKeyFrameItem.KeyFrameSceneNode))
        simpleKeyFrameItem.IsSelected = true;
      this.OnPropertyChanged("KeyFrameItems");
      this.SendTimingChangeNotifications();
    }

    public void MoveKeyFrameItem(double oldTime, double newTime, bool useOldTime)
    {
      KeyFrameItem keyFrameItem1 = (KeyFrameItem) null;
      foreach (KeyFrameItem keyFrameItem2 in (Collection<KeyFrameItem>) this.KeyFrameItems)
      {
        if ((useOldTime && keyFrameItem2.OldTime == oldTime || !useOldTime && keyFrameItem2.Time == oldTime) && !keyFrameItem2.SetTimeFromModel(newTime))
          keyFrameItem1 = keyFrameItem2;
      }
      if (keyFrameItem1 != (KeyFrameItem) null)
        this.KeyFrameItems.Remove(keyFrameItem1);
      this.SendTimingChangeNotifications();
    }

    protected override void OnRemoved()
    {
      base.OnRemoved();
      foreach (KeyFrameItem keyFrameItem in (Collection<KeyFrameItem>) this.KeyFrameItems)
        keyFrameItem.RemoveFromModel();
      this.keyFrameItems.Clear();
    }

    protected override void ValidateCore()
    {
      base.ValidateCore();
      if (!this.hasSelectedKeyFrameDirty)
        return;
      bool flag = false;
      foreach (KeyFrameItem keyFrameItem in (Collection<KeyFrameItem>) this.keyFrameItems)
      {
        if (keyFrameItem.IsSelected)
        {
          flag = true;
          break;
        }
      }
      this.HasSelectedKeyFrame = flag;
      this.hasSelectedKeyFrameDirty = false;
    }
  }
}
