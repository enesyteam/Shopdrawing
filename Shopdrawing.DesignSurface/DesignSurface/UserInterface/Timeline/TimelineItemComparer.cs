// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.TimelineItemComparer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class TimelineItemComparer : Comparer<TimelineItem>
  {
    public override int Compare(TimelineItem xTimelineItem, TimelineItem yTimelineItem)
    {
      Type type1 = xTimelineItem.GetType();
      Type type2 = yTimelineItem.GetType();
      if (type1.IsAssignableFrom(type2))
        return xTimelineItem.CompareTo(yTimelineItem);
      if (type2.IsAssignableFrom(type1))
        return -1 * yTimelineItem.CompareTo(xTimelineItem);
      int priorityForTimelineItem1 = this.GetSortPriorityForTimelineItem(xTimelineItem);
      int priorityForTimelineItem2 = this.GetSortPriorityForTimelineItem(yTimelineItem);
      if (priorityForTimelineItem1 == priorityForTimelineItem2)
        return xTimelineItem.CompareTo(yTimelineItem);
      return priorityForTimelineItem1 <= priorityForTimelineItem2 ? -1 : 1;
    }

    private int GetSortPriorityForTimelineItem(TimelineItem timelineItem)
    {
      if (timelineItem is Primitive3DTimelineItem)
        return 9;
      if (timelineItem is Model3DGroupTimelineItem)
        return 8;
      if (timelineItem is LightTimelineItem)
        return 6;
      if (timelineItem is ChildPropertyTimelineItem || timelineItem is BehaviorTimelineItem)
        return 5;
      if (timelineItem is AnimationTimelineItem || timelineItem is CompoundPropertyTimelineItem)
        return 4;
      if (timelineItem is ModelVisual3DTimelineItem || timelineItem is Item3DTimelineItem)
        return 7;
      if (timelineItem is MediaTimelineItem)
        return 2;
      if (timelineItem is ElementTimelineItem)
        return 10;
      if (timelineItem is StyleTimelineItem)
        return 3;
      return timelineItem is DependencyObjectTimelineItem ? 10 : 0;
    }
  }
}
