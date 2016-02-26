// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Scheduler.ScheduleTask
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework.Diagnostics;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Scheduler
{
  public class ScheduleTask
  {
    private List<ScheduleTask> dependencies = new List<ScheduleTask>();
    private List<ScheduleTask> reverseDependencies = new List<ScheduleTask>();
    private string name;
    private ScheduleTask.NoArgumentsReturningVoid updateHandler;
    private DispatcherPriority priority;
    private SchedulingService scheduleService;
    private bool isScheduled;

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public DispatcherPriority Priority
    {
      get
      {
        return this.priority;
      }
    }

    public bool IsScheduled
    {
      get
      {
        return this.isScheduled;
      }
    }

    internal List<ScheduleTask> Dependencies
    {
      get
      {
        return this.dependencies;
      }
    }

    internal List<ScheduleTask> ReverseDependencies
    {
      get
      {
        return this.reverseDependencies;
      }
    }

    internal ScheduleTask(SchedulingService scheduleService, string name, DispatcherPriority priority, ScheduleTask.NoArgumentsReturningVoid updateHandler)
    {
      this.scheduleService = scheduleService;
      this.name = name;
      this.priority = priority;
      this.updateHandler = updateHandler;
    }

    public void Schedule()
    {
      if (this.isScheduled)
        return;
      this.isScheduled = true;
      this.scheduleService.ScheduleInternal(this);
    }

    public void ForceUpdate()
    {
      foreach (ScheduleTask scheduleTask in this.Dependencies)
        scheduleTask.ForceUpdate();
      if (!this.isScheduled)
        return;
      this.Update();
    }

    internal void Update()
    {
      PerformanceUtility.LogInfoEvent("Start ScheduleTask.Update");
      this.isScheduled = false;
      this.scheduleService.SetUpdatingTaskInternal(this);
      this.updateHandler();
      this.scheduleService.SetUpdatingTaskInternal((ScheduleTask) null);
      PerformanceUtility.LogInfoEvent("End ScheduleTask.Update");
    }

    public delegate void NoArgumentsReturningVoid();
  }
}
