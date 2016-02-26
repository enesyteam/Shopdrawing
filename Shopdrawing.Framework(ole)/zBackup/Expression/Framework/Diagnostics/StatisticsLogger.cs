// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.StatisticsLogger
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class StatisticsLogger : PerformanceLogger
  {
    private Dictionary<PerformanceEvent, PerformanceEventStatistics> statistics = new Dictionary<PerformanceEvent, PerformanceEventStatistics>();

    public IEnumerable<PerformanceEventStatistics> Statistics
    {
      get
      {
        return (IEnumerable<PerformanceEventStatistics>) this.statistics.Values;
      }
    }

    public StatisticsLogger()
    {
      this.InitializeDictionary();
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent)
    {
      this.statistics[performanceEvent].Start();
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      this.statistics[performanceEvent].Start();
    }

    public override void AddInterimStep(PerformanceEvent performanceEvent, string stepName)
    {
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent)
    {
      this.statistics[performanceEvent].Stop();
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      this.statistics[performanceEvent].Stop();
    }

    public override void AddLogEvent(PerformanceEvent performanceEvent)
    {
    }

    public override void AddInfoEvent(string info)
    {
    }

    private void InitializeDictionary()
    {
      foreach (PerformanceEvent performanceEvent in Enum.GetValues(typeof (PerformanceEvent)))
        this.statistics[performanceEvent] = new PerformanceEventStatistics(performanceEvent);
    }

    public override void Reset()
    {
      this.InitializeDictionary();
    }
  }
}
