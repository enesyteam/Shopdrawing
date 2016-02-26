// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.Item3DTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class Item3DTimelineItem : ElementTimelineItem
  {
    public override bool IsShown
    {
      get
      {
        return true;
      }
      set
      {
      }
    }

    public Item3DTimelineItem(TimelineItemManager timelineItemManager, Base3DElement element)
      : base(timelineItemManager, (SceneElement) element)
    {
    }
  }
}
