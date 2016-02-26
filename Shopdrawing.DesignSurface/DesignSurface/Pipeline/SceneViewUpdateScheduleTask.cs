// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Pipeline.SceneViewUpdateScheduleTask
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using Microsoft.Expression.Framework.Scheduler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace Microsoft.Expression.DesignSurface.Pipeline
{
  public class SceneViewUpdateScheduleTask
  {
    private static DispatcherPriority earlyPriority = DispatcherPriority.Render;
    private static DispatcherPriority latePriority = DispatcherPriority.ContextIdle;
    private static Stopwatch stopwatch = new Stopwatch();
    private static readonly int scheduleCountThreshold = 20;
    private static readonly long scheduleTimeThreshold = 2000L;
    private ScheduleTask task;
    private Dictionary<SceneViewModel, SceneUpdatePhaseEventArgs> workItems;
    private DispatcherPriority priority;
    private static bool synchronous;
    private long scheduleTime;
    private int scheduleCount;

    public static bool Synchronous
    {
      internal get
      {
        return SceneViewUpdateScheduleTask.synchronous;
      }
      set
      {
        SceneViewUpdateScheduleTask.synchronous = value;
      }
    }

    public static DispatcherPriority EarlyPriority
    {
      get
      {
        return SceneViewUpdateScheduleTask.earlyPriority;
      }
      set
      {
        SceneViewUpdateScheduleTask.earlyPriority = value;
      }
    }

    public static DispatcherPriority LatePriority
    {
      get
      {
        return SceneViewUpdateScheduleTask.latePriority;
      }
      set
      {
        SceneViewUpdateScheduleTask.latePriority = value;
      }
    }

    public DispatcherPriority Priority
    {
      get
      {
        return this.priority;
      }
    }

    static SceneViewUpdateScheduleTask()
    {
      SceneViewUpdateScheduleTask.stopwatch.Reset();
      SceneViewUpdateScheduleTask.stopwatch.Start();
    }

    public SceneViewUpdateScheduleTask(ISchedulingService scheduleManager, bool early)
    {
      this.workItems = new Dictionary<SceneViewModel, SceneUpdatePhaseEventArgs>();
      if (early)
      {
        this.priority = SceneViewUpdateScheduleTask.earlyPriority;
        this.task = scheduleManager.RegisterTask("EarlySceneUpdatePhase", this.priority, new ScheduleTask.NoArgumentsReturningVoid(this.UpdateEarly), (string[]) null);
      }
      else
      {
        this.priority = SceneViewUpdateScheduleTask.latePriority;
        this.task = scheduleManager.RegisterTask("LateSceneUpdatePhase", this.priority, new ScheduleTask.NoArgumentsReturningVoid(this.UpdateLate), (string[]) null);
      }
    }

    public void Unregister(ISchedulingService scheduleManager)
    {
      scheduleManager.UnregisterTask(this.task);
    }

    public void ScheduleUpdate(SceneViewModel viewModel, bool viewSwitched, bool rootNodeChanged)
    {
      if (!this.task.IsScheduled)
        this.scheduleTime = SceneViewUpdateScheduleTask.stopwatch.ElapsedMilliseconds;
      if (!this.workItems.ContainsKey(viewModel))
        this.workItems[viewModel] = new SceneUpdatePhaseEventArgs(viewModel, viewSwitched, rootNodeChanged);
      else
        this.workItems[viewModel].Refresh(viewSwitched, rootNodeChanged);
      this.task.Schedule();
      ++this.scheduleCount;
      if (!SceneViewUpdateScheduleTask.synchronous && (this.scheduleCount <= SceneViewUpdateScheduleTask.scheduleCountThreshold || SceneViewUpdateScheduleTask.stopwatch.ElapsedMilliseconds - this.scheduleTime <= SceneViewUpdateScheduleTask.scheduleTimeThreshold))
        return;
      this.Validate();
    }

    public void Remove(SceneViewModel viewModel)
    {
      if (!this.workItems.ContainsKey(viewModel))
        return;
      this.workItems.Remove(viewModel);
    }

    private void UpdateEarly()
    {
      this.scheduleCount = 0;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewUpdateScheduleTaskUpdateEarly);
      SceneUpdatePhaseEventArgs args1 = (SceneUpdatePhaseEventArgs) null;
      SceneUpdatePhaseEventArgs[] updatePhaseEventArgsArray = Enumerable.ToArray<SceneUpdatePhaseEventArgs>((IEnumerable<SceneUpdatePhaseEventArgs>) this.workItems.Values);
      this.workItems.Clear();
      foreach (SceneUpdatePhaseEventArgs args2 in updatePhaseEventArgsArray)
      {
        if (args2.ViewModel.IsActiveSceneViewModel)
          args1 = args2;
        args2.ViewModel.FireEarlySceneUpdatePhase(args2);
      }
      if (args1 != null)
        args1.ViewModel.DesignerContext.SelectionManager.FireEarlyActiveSceneUpdatePhase(args1);
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewUpdateScheduleTaskUpdateEarly);
    }

    private void UpdateLate()
    {
      this.scheduleCount = 0;
      PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SceneViewUpdateScheduleTaskUpdateLate);
      SceneUpdatePhaseEventArgs args1 = (SceneUpdatePhaseEventArgs) null;
      SceneUpdatePhaseEventArgs[] updatePhaseEventArgsArray = Enumerable.ToArray<SceneUpdatePhaseEventArgs>((IEnumerable<SceneUpdatePhaseEventArgs>) this.workItems.Values);
      this.workItems.Clear();
      foreach (SceneUpdatePhaseEventArgs args2 in updatePhaseEventArgsArray)
      {
        if (args2.ViewModel.IsActiveSceneViewModel)
          args1 = args2;
        args2.ViewModel.FireLateSceneUpdatePhase(args2);
      }
      if (args1 != null)
        args1.ViewModel.DesignerContext.SelectionManager.FireLateActiveSceneUpdatePhase(args1);
      foreach (SceneUpdatePhaseEventArgs updatePhaseEventArgs in updatePhaseEventArgsArray)
        updatePhaseEventArgs.ViewModel.OnPipelineLatePhaseEnd();
      if (args1 != null)
        args1.ViewModel.DesignerContext.SelectionManager.FirePostSceneUpdatePhase();
      PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SceneViewUpdateScheduleTaskUpdateLate);
    }

    public void Validate()
    {
      this.task.ForceUpdate();
    }
  }
}
