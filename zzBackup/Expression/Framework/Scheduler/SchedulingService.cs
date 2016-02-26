// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Scheduler.SchedulingService
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace Microsoft.Expression.Framework.Scheduler
{
  [Export(typeof (ISchedulingService))]
  public class SchedulingService : ISchedulingService
  {
    private Dictionary<string, ScheduleTask> taskDictionary = new Dictionary<string, ScheduleTask>();
    private List<ScheduleTask> taskList = new List<ScheduleTask>();
    private SchedulingService.DispatcherToken[] dispatcherTokens = new SchedulingService.DispatcherToken[11];
    private ScheduleTask updatingTask;

    public ScheduleTask RegisterTask(string name, DispatcherPriority priority, ScheduleTask.NoArgumentsReturningVoid updateHandler, string[] dependencyNames)
    {
      ScheduleTask scheduleTask1 = new ScheduleTask(this, name, priority, updateHandler);
      if (this.taskDictionary.ContainsKey(scheduleTask1.Name))
        throw new ArgumentException(ExceptionStringTable.CantRegisterTwoTasksWithSameName);
      if (dependencyNames != null)
      {
        foreach (string key in dependencyNames)
        {
          if (!this.taskDictionary.ContainsKey(key))
            throw new ArgumentException(ExceptionStringTable.CantRegisterADependencyOnANonexistentTask);
          if (this.taskDictionary[key].Priority < scheduleTask1.Priority)
            throw new ArgumentException(ExceptionStringTable.CantRegisterADependencyOnATaskAtALowerPriorityThanYourself);
        }
        foreach (string index in dependencyNames)
        {
          ScheduleTask scheduleTask2 = this.taskDictionary[index];
          scheduleTask1.Dependencies.Add(scheduleTask2);
          scheduleTask2.ReverseDependencies.Add(scheduleTask1);
        }
      }
      this.taskDictionary.Add(scheduleTask1.Name, scheduleTask1);
      this.taskList.Add(scheduleTask1);
      return scheduleTask1;
    }

    public void UnregisterTask(ScheduleTask task)
    {
      if (task.ReverseDependencies.Count > 0)
        throw new ArgumentException(ExceptionStringTable.CantRemoveATaskThatOtherTasksDependOn);
      foreach (ScheduleTask scheduleTask in task.Dependencies)
        scheduleTask.ReverseDependencies.Remove(task);
      task.Dependencies.Clear();
      this.taskDictionary.Remove(task.Name);
      this.taskList.Remove(task);
    }

    internal void SetUpdatingTaskInternal(ScheduleTask task)
    {
      this.updatingTask = task;
    }

    internal void ScheduleInternal(ScheduleTask task)
    {
      ScheduleTask scheduleTask = this.updatingTask;
      if (this.dispatcherTokens[(int) task.Priority] != null)
        return;
      SchedulingService.DispatcherToken dispatcherToken = new SchedulingService.DispatcherToken(this, task.Priority);
      this.dispatcherTokens[(int) task.Priority] = dispatcherToken;
    }

    private void DispatchTasksAtPriority(DispatcherPriority priority)
    {
      this.dispatcherTokens[(int) priority] = (SchedulingService.DispatcherToken) null;
      foreach (ScheduleTask scheduleTask in this.taskList)
      {
        if (scheduleTask.Priority == priority && scheduleTask.IsScheduled)
        {
          scheduleTask.Update();
          if (priority <= DispatcherPriority.Background)
            break;
        }
      }
    }

    private class DispatcherToken
    {
      private SchedulingService manager;
      private DispatcherPriority priority;

      public DispatcherToken(SchedulingService manager, DispatcherPriority priority)
      {
        this.manager = manager;
        this.priority = priority;
        Dispatcher.CurrentDispatcher.BeginInvoke(priority, (Delegate) new DispatcherOperationCallback(this.Dispatch), (object) null);
      }

      public object Dispatch(object arg)
      {
        this.manager.DispatchTasksAtPriority(this.priority);
        return (object) null;
      }
    }
  }
}
