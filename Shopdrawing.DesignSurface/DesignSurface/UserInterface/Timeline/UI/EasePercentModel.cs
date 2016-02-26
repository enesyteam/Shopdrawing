// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.EasePercentModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public abstract class EasePercentModel
  {
    private TimelineView timelineView;
    private double percent;

    public abstract bool IsSelected { get; set; }

    protected TimelineView TimelineView
    {
      get
      {
        return this.timelineView;
      }
    }

    protected double Percent
    {
      get
      {
        return this.percent;
      }
    }

    internal EasePercentModel(TimelineView timelineView, double percent)
    {
      this.timelineView = timelineView;
      this.percent = percent;
    }

    public override string ToString()
    {
      return (this.percent / 100.0).ToString("0%", (IFormatProvider) CultureInfo.CurrentCulture);
    }
  }
}
