// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.Primitive3DTimelineItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  public class Primitive3DTimelineItem : Item3DTimelineItem
  {
    public Primitive3DTimelineItem(TimelineItemManager timelineItemManager, GeometryModel3DElement element)
      : base(timelineItemManager, (Base3DElement) element)
    {
    }
  }
}
