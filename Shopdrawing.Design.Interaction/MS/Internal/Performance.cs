// Decompiled with JetBrains decompiler
// Type: MS.Internal.Performance
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Internal.Performance;
using System;

namespace MS.Internal
{
  internal static class Performance
  {
    private const string PerformanceTrackerInstanceKey = "Cider_PerformanceTrackerInstanceKey";
    private const string StartTimingFunctionKey = "Cider_StartTimingFunctionKey";
    private const string StopTimingFunctionKey = "Cider_StopTimingFunctionKey";
    private const string MarkTimeFunctionKey = "Cider_MarkTimeFunctionKey";
    private static bool _initialized;
    private static Func<ulong, string, bool> _startFunc;
    private static Func<ulong, string, bool> _endFunc;
    private static Func<ulong, string, bool> _markFunc;

    private static void Initialize()
    {
      if (Performance._initialized)
        return;
      AppDomain currentDomain = AppDomain.CurrentDomain;
      if (Performance._startFunc == null)
        Performance._startFunc = (Func<ulong, string, bool>) currentDomain.GetData("Cider_StartTimingFunctionKey");
      if (Performance._endFunc == null)
        Performance._endFunc = (Func<ulong, string, bool>) currentDomain.GetData("Cider_StopTimingFunctionKey");
      if (Performance._markFunc == null)
        Performance._markFunc = (Func<ulong, string, bool>) currentDomain.GetData("Cider_MarkTimeFunctionKey");
      Performance._initialized = true;
    }

    public static void Initialize(IPerformanceTracker tracker)
    {
      AppDomain.CurrentDomain.SetData("Cider_PerformanceTrackerInstanceKey", (object) tracker);
      Performance._startFunc = new Func<ulong, string, bool>(tracker.StartTiming);
      AppDomain.CurrentDomain.SetData("Cider_StartTimingFunctionKey", (object) Performance._startFunc);
      Performance._endFunc = new Func<ulong, string, bool>(tracker.StopTiming);
      AppDomain.CurrentDomain.SetData("Cider_StopTimingFunctionKey", (object) Performance._endFunc);
      Performance._markFunc = new Func<ulong, string, bool>(tracker.MarkTime);
      AppDomain.CurrentDomain.SetData("Cider_MarkTimeFunctionKey", (object) Performance._markFunc);
      Performance._initialized = true;
    }

    public static void StartTiming(PerformanceMark mark)
    {
      Performance.Initialize();
      CodeMarkers.Instance.CodeMarker(mark.BeginEvent);
      if (Performance._startFunc == null)
        return;
      int num = Performance._startFunc((ulong) mark.BeginEvent, mark.Description) ? 1 : 0;
    }

    public static void StopTiming(PerformanceMark mark)
    {
      if (Performance._endFunc != null)
      {
        int num = Performance._endFunc((ulong) mark.EndEvent, mark.Description) ? 1 : 0;
      }
      CodeMarkers.Instance.CodeMarker(mark.EndEvent);
    }

    public static void MarkTime(PerformanceMark mark)
    {
      if (Performance._markFunc != null)
      {
        int num = Performance._markFunc((ulong) mark.BeginEvent, mark.Description) ? 1 : 0;
      }
      CodeMarkers.Instance.CodeMarker(mark.BeginEvent);
    }
  }
}
