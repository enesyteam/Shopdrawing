// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceEventStatistics
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class PerformanceEventStatistics
  {
    private Stopwatch timer = new Stopwatch();
    private TimeSpan totalElapsed = TimeSpan.Zero;
    private TimeSpan minTime = TimeSpan.MaxValue;
    private TimeSpan maxTime = TimeSpan.MinValue;
    private uint eventDepth;
    private int eventCount;
    private Thread thread;
    private PerformanceEvent performanceEvent;

    public int EventCount
    {
      get
      {
        return this.eventCount;
      }
    }

    public TimeSpan TotalTime
    {
      get
      {
        return this.totalElapsed;
      }
    }

    public TimeSpan MinTime
    {
      get
      {
        return this.minTime;
      }
    }

    public TimeSpan MaxTime
    {
      get
      {
        return this.maxTime;
      }
    }

    public TimeSpan AverageTime
    {
      get
      {
        return TimeSpan.FromMilliseconds(this.eventCount == 0 ? 0.0 : this.totalElapsed.TotalMilliseconds / (double) this.eventCount);
      }
    }

    public PerformanceEvent PeformanceEvent
    {
      get
      {
        return this.performanceEvent;
      }
    }

    public PerformanceEventStatistics(PerformanceEvent performanceEvent)
    {
      this.performanceEvent = performanceEvent;
    }

    public void Start()
    {
      ++this.eventCount;
      ++this.eventDepth;
      if ((int) this.eventDepth != 1)
        return;
      this.thread = Thread.CurrentThread;
      this.timer.Reset();
      this.timer.Start();
    }

    public void Stop()
    {
      int num = (int) this.eventDepth;
      --this.eventDepth;
      if ((int) this.eventDepth != 0)
        return;
      TimeSpan elapsed = this.timer.Elapsed;
      this.totalElapsed += elapsed;
      if (elapsed < this.minTime)
        this.minTime = elapsed;
      if (elapsed > this.maxTime)
        this.maxTime = elapsed;
      this.thread = (Thread) null;
    }
  }
}
