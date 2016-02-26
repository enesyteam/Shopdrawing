// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.AutoScroller
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.View
{
  public abstract class AutoScroller
  {
    private const double defaultScrollFrequency = 0.05;

    protected DispatcherTimer ScrollTimer { get; private set; }

    protected AutoScroller()
      : this(DispatcherPriority.Input, TimeSpan.FromSeconds(0.05))
    {
    }

    protected AutoScroller(DispatcherPriority priority, TimeSpan interval)
    {
      this.ScrollTimer = new DispatcherTimer(priority);
      this.ScrollTimer.Interval = interval;
      this.ScrollTimer.Tick += (EventHandler) ((sender, e) =>
      {
        if (this.DoScroll())
          return;
        this.ScrollTimer.Stop();
      });
    }

    public void StopScroll()
    {
      this.ScrollTimer.Stop();
    }

    protected abstract bool DoScroll();
  }
}
