// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.ModelVisual3DContentTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class ModelVisual3DContentTimelineItem : ChildPropertyTimelineItem
  {
    private bool isBeingRemoved;

    public ModelVisual3DContentTimelineItem(TimelineItemManager timelineItemManager, IProperty key, ElementTimelineItem elementTimelineItem, ChildPropertyTimelineItemType itemType)
      : base(timelineItemManager, key, elementTimelineItem, itemType, false)
    {
    }

    protected override void OnRemoving()
    {
      this.isBeingRemoved = true;
    }

    protected override void ChildRemoved(TimelineItem child)
    {
      base.ChildRemoved(child);
      if (this.isBeingRemoved || this.Parent.Children.Count == 1)
        return;
      this.RemoveFromTree();
    }
  }
}
