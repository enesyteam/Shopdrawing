// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Timeline.DragDrop.DragDropContext
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.Timeline;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Timeline.DragDrop
{
  public sealed class DragDropContext
  {
    public IDataObject Data { get; private set; }

    public TimelineItem Target { get; private set; }

    public TimelineDropEffects AllowedEffects { get; private set; }

    public TimelineDragDescriptor Descriptor { get; set; }

    public DragDropContext(IDataObject data, TimelineItem target, TimelineDropEffects allowedEffects)
    {
      this.Data = data;
      this.Target = target;
      this.AllowedEffects = allowedEffects;
      this.Descriptor = new TimelineDragDescriptor(this);
    }
  }
}
