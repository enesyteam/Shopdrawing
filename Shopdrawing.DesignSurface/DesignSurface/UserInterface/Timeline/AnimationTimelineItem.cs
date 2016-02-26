// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.AnimationTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class AnimationTimelineItem : ScheduledTimelineItem
  {
    private bool isConforming = true;

    public AnimationSceneNode AnimationSceneNode
    {
      get
      {
        return (AnimationSceneNode) this.PrimaryScheduledAnimation;
      }
    }

    public TimelineItem TargetElement
    {
      get
      {
        if (this.AnimationSceneNode != null)
          return this.TimelineItemManager.FindTimelineItem(this.AnimationSceneNode.TargetElement);
        return (TimelineItem) null;
      }
    }

    public override string DisplayName
    {
      get
      {
        ReadOnlyCollection<ReferenceStep> referenceSteps = this.TargetProperty.ReferenceSteps;
        return referenceSteps[referenceSteps.Count - 1].Name;
      }
      set
      {
      }
    }

    public PropertyReference TargetProperty
    {
      get
      {
        return this.AnimationSceneNode.TargetProperty;
      }
    }

    public override DrawingBrush IconBrush
    {
      get
      {
        if (this.AnimationSceneNode != null)
          return IconMapper.GetDrawingBrushForType((ITypeId) this.AnimationSceneNode.Type, false, 12, 12);
        return IconMapper.GetDrawingBrushForType(PlatformTypes.Object, false, 12, 12);
      }
    }

    public override string FullName
    {
      get
      {
        return this.AnimationSceneNode.TargetName + "." + this.TargetProperty.ShortPath;
      }
    }

    public bool IsConforming
    {
      get
      {
        return this.isConforming;
      }
      set
      {
        this.isConforming = value;
        this.OnPropertyChanged("IsConforming");
      }
    }

    public AnimationTimelineItem(TimelineItemManager timelineItemManager, StoryboardTimelineSceneNode parentTimeline, AnimationSceneNode animationSceneNode)
      : base(timelineItemManager, parentTimeline, (TimelineSceneNode) animationSceneNode)
    {
    }

    public override int CompareTo(TimelineItem timelineItem)
    {
      if (timelineItem is AnimationTimelineItem || timelineItem is CompoundPropertyTimelineItem)
        return string.Compare(this.DisplayName, timelineItem.DisplayName, StringComparison.CurrentCulture);
      return 0;
    }

    public override void Select()
    {
      this.TimelineItemManager.ViewModel.AnimationSelectionSet.SetSelection(this.AnimationSceneNode);
    }

    public override void ToggleSelect()
    {
      this.TimelineItemManager.ViewModel.AnimationSelectionSet.ToggleSelection(this.AnimationSceneNode);
    }

    public override void ExtendSelect()
    {
      this.TimelineItemManager.ViewModel.AnimationSelectionSet.ExtendSelection(this.AnimationSceneNode);
    }
  }
}
