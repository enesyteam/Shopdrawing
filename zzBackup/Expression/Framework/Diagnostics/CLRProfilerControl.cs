// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.CLRProfilerControl
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class CLRProfilerControl
  {
    private static bool isProcessIsUnderProfiler;
    private static string killProcessMarker;

    public static bool AllocationLoggingActive
    {
      get
      {
        if (CLRProfilerControl.isProcessIsUnderProfiler)
          return CLRProfilerControl.GetAllocationLoggingActive();
        return false;
      }
      set
      {
        if (!CLRProfilerControl.isProcessIsUnderProfiler)
          return;
        CLRProfilerControl.SetAllocationLoggingActive(value);
      }
    }

    public static bool CallLoggingActive
    {
      get
      {
        if (CLRProfilerControl.isProcessIsUnderProfiler)
          return CLRProfilerControl.GetCallLoggingActive();
        return false;
      }
      set
      {
        if (!CLRProfilerControl.isProcessIsUnderProfiler)
          return;
        CLRProfilerControl.SetCallLoggingActive(value);
      }
    }

    public static bool ProcessIsUnderProfiler
    {
      get
      {
        return CLRProfilerControl.isProcessIsUnderProfiler;
      }
    }

    static CLRProfilerControl()
    {
      try
      {
        bool allocationLoggingActive = CLRProfilerControl.GetAllocationLoggingActive();
        CLRProfilerControl.SetAllocationLoggingActive(!allocationLoggingActive);
        CLRProfilerControl.isProcessIsUnderProfiler = allocationLoggingActive != CLRProfilerControl.GetAllocationLoggingActive();
        CLRProfilerControl.SetAllocationLoggingActive(allocationLoggingActive);
        CLRProfilerControl.killProcessMarker = Environment.GetEnvironmentVariable("OMV_KILLPROCESS_MARKER");
      }
      catch (DllNotFoundException ex)
      {
        throw new Exception("profilerObj.dll was not found");
      }
    }

    [DllImport("ProfilerOBJ.dll", CharSet = CharSet.Unicode)]
    private static extern void LogComment(string comment);

    [DllImport("ProfilerOBJ.dll")]
    private static extern bool GetAllocationLoggingActive();

    [DllImport("ProfilerOBJ.dll")]
    private static extern void SetAllocationLoggingActive(bool active);

    [DllImport("ProfilerOBJ.dll")]
    private static extern bool GetCallLoggingActive();

    [DllImport("ProfilerOBJ.dll")]
    private static extern void SetCallLoggingActive(bool active);

    [DllImport("ProfilerOBJ.dll")]
    private static extern bool DumpHeap(uint timeOut);

    public static void LogWriteLine(string comment)
    {
      if (!CLRProfilerControl.isProcessIsUnderProfiler)
        return;
      CLRProfilerControl.LogComment(comment);
      if (!(comment == CLRProfilerControl.killProcessMarker))
        return;
      Process.GetCurrentProcess().Kill();
    }

    public static void LogWriteLine(string format, params object[] args)
    {
      if (!CLRProfilerControl.isProcessIsUnderProfiler)
        return;
      CLRProfilerControl.LogComment(string.Format(format, args));
    }

    public static void DumpHeap()
    {
      if (CLRProfilerControl.isProcessIsUnderProfiler && !CLRProfilerControl.DumpHeap(60000U))
        throw new Exception("Failure to\tdump heap");
    }
  }
}
