// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.TimelineDropEffectsHelper
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public static class TimelineDropEffectsHelper
  {
    public static DragDropEffects DragDropEffect(TimelineDropEffects effects)
    {
      DragDropEffects dragDropEffects = DragDropEffects.None;
      if ((TimelineDropEffects.Copy & effects) != TimelineDropEffects.None)
        dragDropEffects |= DragDropEffects.Copy;
      if ((TimelineDropEffects.Move & effects) != TimelineDropEffects.None)
        dragDropEffects |= DragDropEffects.Move;
      return dragDropEffects;
    }

    public static TimelineDropEffects FromDragDropEffects(DragDropEffects effects)
    {
      TimelineDropEffects timelineDropEffects = TimelineDropEffects.None;
      if ((effects & DragDropEffects.Copy) != DragDropEffects.None)
        timelineDropEffects |= TimelineDropEffects.Copy;
      else if ((effects & DragDropEffects.Move) != DragDropEffects.None)
        timelineDropEffects |= TimelineDropEffects.Move;
      return timelineDropEffects;
    }
  }
}
