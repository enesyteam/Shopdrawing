// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Feedback.RunTimeTracker
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.Expression.Framework.Feedback
{
  internal class RunTimeTracker : IDisposable
  {
    private const string TotalRunCountKey = "TotalRunCount";
    private const string TotalRunTimeKey = "TotalRunTime";
    private const string RunningCountKey = "RunningCount";
    private static RunTimeTracker instance;
    private ExpressionApplication application;
    private string registryPath;
    private Mutex runTimeMutex;
    private Process currentProcess;
    private bool isDeactivated;

    private RunTimeTracker(ExpressionApplication application)
    {
      this.application = application;
    }

    ~RunTimeTracker()
    {
      if (this.isDeactivated)
        return;
      int seconds = (DateTime.Now - this.currentProcess.StartTime).Seconds;
      try
      {
        if (!this.runTimeMutex.WaitOne(2000))
          return;
      }
      catch (AbandonedMutexException ex)
      {
      }
      RunTimeTracker.RunTimeReport runtimeReport = this.GetRuntimeReport();
      RegistryHelper.SetCurrentUserRegistryValue<uint>(this.registryPath, "TotalRunCount", runtimeReport.InstancesLaunched + 1U, RegistryValueKind.DWord);
      RegistryHelper.SetCurrentUserRegistryValue<long>(this.registryPath, "TotalRunTime", (long) runtimeReport.SecondsRun + (long) seconds, RegistryValueKind.DWord);
      RegistryHelper.SetCurrentUserRegistryValue<uint>(this.registryPath, "RunningCount", RegistryHelper.RetrieveCurrentUserRegistryValue<uint>(this.registryPath, "RunningCount") - 1U, RegistryValueKind.DWord);
    }

    public static RunTimeTracker GetInstance(ExpressionApplication application)
    {
      if (RunTimeTracker.instance == null)
      {
        RunTimeTracker.instance = new RunTimeTracker(application);
        RunTimeTracker.instance.CheckRunTimeState();
      }
      return RunTimeTracker.instance;
    }

    private void CheckRunTimeState()
    {
      RunTimeTracker.RunTimeReport runTimeReport = (RunTimeTracker.RunTimeReport) null;
      this.currentProcess = Process.GetCurrentProcess();
      string currentMainModuleFileName = this.currentProcess.MainModule.FileName;
      IFeedbackService service = this.application.Services.GetService<IFeedbackService>();
      if (service == null)
      {
        this.isDeactivated = true;
      }
      else
      {
        this.registryPath = service.CustomerFeedbackRegistryPath;
        this.runTimeMutex = new Mutex(false, this.registryPath.Replace('\\', '_'));
        try
        {
          if (!this.runTimeMutex.WaitOne(2000))
            return;
        }
        catch (AbandonedMutexException ex)
        {
        }
        int num1;
        try
        {
          num1 = Enumerable.Count<Process>(Enumerable.Where<Process>((IEnumerable<Process>) Process.GetProcessesByName(this.currentProcess.ProcessName), (Func<Process, bool>) (process =>
          {
            if (process.Id == this.currentProcess.Id)
              return true;
            if (process.MainModule.FileName.Equals(currentMainModuleFileName, StringComparison.OrdinalIgnoreCase))
              return process.StartInfo.UserName == this.currentProcess.StartInfo.UserName;
            return false;
          })));
        }
        catch (Exception ex)
        {
          this.isDeactivated = true;
          return;
        }
        if ((long) RegistryHelper.RetrieveCurrentUserRegistryValue<uint>(this.registryPath, "RunningCount") > (long) (num1 - 1))
        {
          runTimeReport = this.GetRuntimeReport();
          RegistryHelper.SetCurrentUserRegistryValue<int>(this.registryPath, "TotalRunCount", 0, RegistryValueKind.DWord);
          RegistryHelper.SetCurrentUserRegistryValue<int>(this.registryPath, "TotalRunTime", 0, RegistryValueKind.DWord);
        }
        RegistryHelper.SetCurrentUserRegistryValue<int>(this.registryPath, "RunningCount", num1, RegistryValueKind.DWord);
        this.runTimeMutex.ReleaseMutex();
        if (runTimeReport == null)
          return;
        int num2 = (int) runTimeReport.InstancesLaunched != 0 ? (int) (runTimeReport.SecondsRun / 60U) : 0;
        if (service == null)
          return;
        service.SetData(23, num2);
      }
    }

    private RunTimeTracker.RunTimeReport GetRuntimeReport()
    {
      return new RunTimeTracker.RunTimeReport(RegistryHelper.RetrieveCurrentUserRegistryValue<uint>(this.registryPath, "TotalRunCount"), RegistryHelper.RetrieveCurrentUserRegistryValue<uint>(this.registryPath, "TotalRunTime"));
    }

    public void Dispose()
    {
      GC.SuppressFinalize((object) this);
    }

    private class RunTimeReport
    {
      private uint instancesLaunched;
      private uint secondsRun;

      public uint InstancesLaunched
      {
        get
        {
          return this.instancesLaunched;
        }
      }

      public uint SecondsRun
      {
        get
        {
          return this.secondsRun;
        }
      }

      public RunTimeReport(uint instancesLaunched, uint secondsRun)
      {
        this.instancesLaunched = instancesLaunched;
        this.secondsRun = secondsRun;
      }
    }
  }
}
