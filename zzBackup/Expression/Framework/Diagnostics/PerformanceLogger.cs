// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.PerformanceLogger
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal abstract class PerformanceLogger
  {
    public abstract void AddStartEvent(PerformanceEvent performanceEvent);

    public abstract void AddStartEvent(PerformanceEvent performanceEvent, string additionalInformation);

    public abstract void AddInterimStep(PerformanceEvent performanceEvent, string stepName);

    public abstract void AddEndEvent(PerformanceEvent performanceEvent);

    public abstract void AddEndEvent(PerformanceEvent performanceEvent, string additionalInformation);

    public abstract void AddLogEvent(PerformanceEvent performanceEvent);

    public abstract void AddInfoEvent(string info);

    public abstract void Reset();
  }
}
