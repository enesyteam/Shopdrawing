// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.EventTracingLogger
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class EventTracingLogger : PerformanceLogger
  {
    public static readonly TraceProvider EventProvider = new TraceProvider("ExpressionInteractiveInfo", new Guid("{4E7A6FD6-0C60-424c-BAC2-5447F3CE5585}"));
    public static readonly Guid InfoEventGuid = new Guid("{AC5542C1-473E-44cb-83D4-B08A21DCB5B9}");
    private const int Message = 1;

    internal EventTracingLogger()
    {
    }

    private bool IsEnabled(EventTracingLogger.Flags flag, EventTracingLogger.Level level)
    {
      if (EventTracingLogger.EventProvider != null && EventTracingLogger.EventProvider.IsEnabled && (uint) (flag & (EventTracingLogger.Flags) EventTracingLogger.EventProvider.Flags) > 0U)
        return (uint) level <= EventTracingLogger.EventProvider.Level;
      return false;
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 1U, (object) 1, (object) PerformanceEventParser.GetMessageForEvent(performanceEvent));
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 1U, (object) 1, (object) (PerformanceEventParser.GetMessageForEvent(performanceEvent) + ":" + additionalInformation));
    }

    public override void AddInterimStep(PerformanceEvent performanceEvent, string stepName)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 0U, (object) 1, (object) (PerformanceEventParser.GetMessageForEvent(performanceEvent) + " : " + stepName));
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 2U, (object) 1, (object) PerformanceEventParser.GetMessageForEvent(performanceEvent));
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 2U, (object) 1, (object) (PerformanceEventParser.GetMessageForEvent(performanceEvent) + ":" + additionalInformation));
    }

    public override void AddLogEvent(PerformanceEvent performanceEvent)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(performanceEvent), 0U, (object) 1, (object) PerformanceEventParser.GetMessageForEvent(performanceEvent));
    }

    public override void AddInfoEvent(string info)
    {
      if (!this.IsEnabled(EventTracingLogger.Flags.Performance, EventTracingLogger.Level.Normal))
        return;
      int num = (int) EventTracingLogger.EventProvider.TraceEvent(PerformanceEventParser.GetGuidForEvent(PerformanceEvent.InfoEvent), 0U, (object) 1, (object) info);
    }

    public override void Reset()
    {
    }

    private enum Flags
    {
      Debugging = 1,
      Performance = 2,
      Stress = 4,
      Security = 8,
      UIautomation = 16,
      All = 2147483647,
    }

    private enum Level
    {
      Error = 1,
      Warning = 2,
      Normal = 3,
      Verbose = 4,
    }
  }
}
