// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.VisualStudioAutomation.NativeMethods
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.VisualStudioAutomation
{
  internal class NativeMethods
  {
    public const uint OLE_E_PROMPTSAVECANCELLED = 2147745804U;

    [DllImport("ole32.dll")]
    public static extern int CoRegisterMessageFilter(NativeMethods.IOleMessageFilter newFilter, out NativeMethods.IOleMessageFilter oldFilter);

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000016-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IOleMessageFilter
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }
  }
}
