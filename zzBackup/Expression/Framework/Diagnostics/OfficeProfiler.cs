// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.OfficeProfiler
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class OfficeProfiler
  {
    [DllImport("OFFPROF.DLL")]
    public static extern bool StartProf();

    [DllImport("OFFPROF.DLL")]
    public static extern bool StopProf();

    [DllImport("OFFPROF.DLL")]
    public static extern bool SuspendProf();

    [DllImport("OFFPROF.DLL")]
    public static extern bool ResumeProf();

    [DllImport("OFFPROF.DLL")]
    public static extern bool StartAll();

    [DllImport("OFFPROF.DLL")]
    public static extern bool StopNow();

    [DllImport("OFFPROF.DLL")]
    public static extern bool SuspendAll();

    [DllImport("OFFPROF.DLL")]
    public static extern bool ResumeAll();

    [DllImport("OFFPROF.DLL")]
    public static extern bool ResetAll();

    [DllImport("OFFPROF.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, BestFitMapping = false)]
    public static extern bool RestartAll([MarshalAs(UnmanagedType.LPStr)] string szReportName);

    [CLSCompliant(false)]
    [DllImport("OFFPROF.DLL", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern uint BeginPseudoFunction(string szName);

    [CLSCompliant(false)]
    [DllImport("OFFPROF.DLL", CallingConvention = CallingConvention.StdCall)]
    public static extern uint EndPseudoFunction();
  }
}
