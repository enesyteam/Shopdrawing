// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.ProfileLogger
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class ProfileLogger : PerformanceLogger
  {
    private PerformanceEvent targetEvent;
    private bool enabled;
    private bool profileOpened;

    public void EnableEvent(PerformanceEvent targetEvent)
    {
      this.targetEvent = targetEvent;
      this.enabled = true;
    }

    public void Disable()
    {
      this.enabled = false;
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent)
    {
      if (!this.enabled)
        return;
      if (this.targetEvent != performanceEvent)
        return;
      try
      {
        this.profileOpened = OfficeProfiler.StartAll();
      }
      catch (DllNotFoundException ex)
      {
      }
    }

    public override void AddStartEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      this.AddStartEvent(performanceEvent);
    }

    public override void AddInterimStep(PerformanceEvent performanceEvent, string stepName)
    {
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent)
    {
      if (!this.profileOpened || !this.enabled)
        return;
      if (this.targetEvent != performanceEvent)
        return;
      try
      {
        OfficeProfiler.StopNow();
      }
      catch (DllNotFoundException ex)
      {
      }
    }

    public override void AddEndEvent(PerformanceEvent performanceEvent, string additionalInformation)
    {
      this.AddEndEvent(performanceEvent);
    }

    public override void AddLogEvent(PerformanceEvent performanceEvent)
    {
    }

    public override void AddInfoEvent(string info)
    {
    }

    public override void Reset()
    {
      if (!this.enabled)
        return;
      try
      {
        OfficeProfiler.ResetAll();
      }
      catch (DllNotFoundException ex)
      {
      }
    }
  }
}
