// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.CompoundPropertyTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class CompoundPropertyTimelineItem : CompoundKeyFrameTimelineItem
  {
    private PropertyReference targetProperty;
    private TimelineItem targetTimelineItem;
    private string displayNameOverride;
    private bool hasSelectedKeyFrame;
    private bool hasSelectedKeyFrameDirty;

    public override string DisplayName
    {
      get
      {
        if (this.displayNameOverride != null)
          return this.displayNameOverride;
        ReadOnlyCollection<ReferenceStep> referenceSteps = this.TargetProperty.ReferenceSteps;
        return referenceSteps[referenceSteps.Count - 1].Name;
      }
      set
      {
      }
    }

    public override DrawingBrush IconBrush
    {
      get
      {
        return IconMapper.GetDrawingBrushForType(PlatformTypes.Timeline, false, 12, 12);
      }
    }

    public override string FullName
    {
      get
      {
        return this.targetTimelineItem.FullName + "." + this.TargetProperty.ShortPath;
      }
    }

    public PropertyReference TargetProperty
    {
      get
      {
        return this.targetProperty;
      }
    }

    protected override bool IsActiveCore
    {
      get
      {
        return this.HasActiveChild;
      }
    }

    public override bool ShouldBubbleKeyFrames
    {
      get
      {
        return this.IsActive;
      }
    }

    public override bool ShouldBubbleTimes
    {
      get
      {
        return this.IsActive;
      }
    }

    public double ActiveDuration
    {
      get
      {
        return this.Duration;
      }
    }

    public override bool IsSelectable
    {
      get
      {
        if (this.Parent != null && this.Parent.IsSelectable)
          return this.TimelinePane.HasUnlockedActiveStoryboardContainer;
        return false;
      }
    }

    public bool HasSelectedKeyFrame
    {
      get
      {
        return this.hasSelectedKeyFrame;
      }
      private set
      {
        if (value == this.hasSelectedKeyFrame)
          return;
        this.hasSelectedKeyFrame = value;
        this.OnPropertyChanged("HasSelectedKeyFrame");
      }
    }

    public TimelineItem TargetTimelineItem
    {
      get
      {
        return this.targetTimelineItem;
      }
    }

    private ICollection<KeyFrameSceneNode> KeyFrameSceneNodesToSelect
    {
      get
      {
        List<KeyFrameSceneNode> list = new List<KeyFrameSceneNode>();
        foreach (KeyFrameItem keyFrameItem in (Collection<KeyFrameItem>) this.KeyFrameItems)
          list.AddRange((IEnumerable<KeyFrameSceneNode>) keyFrameItem.KeyFrameSceneNodesToSelect);
        return (ICollection<KeyFrameSceneNode>) list;
      }
    }

    public CompoundPropertyTimelineItem(TimelineItemManager timelineItemManager, PropertyReference targetProperty, TimelineItem targetTimelineItem)
      : this(timelineItemManager, targetProperty, targetTimelineItem, (string) null)
    {
    }

    public CompoundPropertyTimelineItem(TimelineItemManager timelineItemManager, PropertyReference targetProperty, TimelineItem targetTimelineItem, string displayNameOverride)
      : base(timelineItemManager)
    {
      this.targetProperty = targetProperty;
      this.displayNameOverride = displayNameOverride;
      this.targetTimelineItem = targetTimelineItem;
      this.targetTimelineItem.PropertyChanged += new PropertyChangedEventHandler(this.TargetTimelineItem_PropertyChanged);
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      if (timelineItem is CompoundPropertyTimelineItem || timelineItem is AnimationTimelineItem)
        return string.Compare(this.DisplayName, timelineItem.DisplayName, StringComparison.CurrentCulture);
      return 0;
    }

    public void DirtyHasSelectedKeyFrame()
    {
      this.hasSelectedKeyFrameDirty = true;
      CompoundPropertyTimelineItem propertyTimelineItem = this.Parent as CompoundPropertyTimelineItem;
      if (propertyTimelineItem != null)
        propertyTimelineItem.DirtyHasSelectedKeyFrame();
      this.Invalidate();
    }

    protected override void ChildPropertyChanged(string propertyName)
    {
      base.ChildPropertyChanged(propertyName);
      if (!(propertyName == "IsActive"))
        return;
      this.IsActive = this.IsActiveCore;
    }

    protected override void ChildRemoved(TimelineItem child)
    {
      base.ChildRemoved(child);
      this.IsActive = this.IsActiveCore;
    }

    protected override void ChildAdded(TimelineItem child)
    {
      base.ChildAdded(child);
      this.IsActive = this.IsActiveCore;
    }

    protected override void KeyFramesRebuilt()
    {
      base.KeyFramesRebuilt();
      this.OnPropertyChanged("ActiveDuration");
    }

    public override void Select()
    {
      if (this.IsSelectable)
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.SetSelection(this.KeyFrameSceneNodesToSelect, (KeyFrameSceneNode) null);
      else
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.Clear();
    }

    public override void ToggleSelect()
    {
      if (!this.IsSelectable)
        return;
      if (this.IsSelected)
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.RemoveSelection(this.KeyFrameSceneNodesToSelect);
      else
        this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.ExtendSelection(this.KeyFrameSceneNodesToSelect);
    }

    public override void ExtendSelect()
    {
      if (!this.IsSelectable)
        return;
      this.TimelineItemManager.ViewModel.KeyFrameSelectionSet.ExtendSelection(this.KeyFrameSceneNodesToSelect);
    }

    protected override void ValidateCore()
    {
      base.ValidateCore();
      if (!this.hasSelectedKeyFrameDirty)
        return;
      bool flag = false;
      for (int index = 0; index < this.Children.Count; ++index)
      {
        CompoundPropertyTimelineItem propertyTimelineItem = this.Children[index] as CompoundPropertyTimelineItem;
        KeyFramedTimelineItem framedTimelineItem = this.Children[index] as KeyFramedTimelineItem;
        if (propertyTimelineItem != null && propertyTimelineItem.HasSelectedKeyFrame || framedTimelineItem != null && framedTimelineItem.HasSelectedKeyFrame)
        {
          flag = true;
          break;
        }
      }
      this.HasSelectedKeyFrame = flag;
      this.hasSelectedKeyFrameDirty = false;
    }

    private void TargetTimelineItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "FullName"))
        return;
      this.OnPropertyChanged("FullName");
    }
  }
}
