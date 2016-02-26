// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.UnsafeNativeMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class UnsafeNativeMethods
  {
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal sealed class EtwTrace
    {
      private EtwTrace()
      {
      }

      [DllImport("advapi32", CharSet = CharSet.Unicode)]
      internal static extern int GetTraceEnableFlags(ulong traceHandle);

      [DllImport("advapi32", CharSet = CharSet.Unicode)]
      internal static extern byte GetTraceEnableLevel(ulong traceHandle);

      [DllImport("advapi32", EntryPoint = "RegisterTraceGuidsW", CharSet = CharSet.Unicode)]
      internal static extern unsafe uint RegisterTraceGuids([In] UnsafeNativeMethods.EtwTrace.EtwProc cbFunc, [In] void* context, [In] ref Guid controlGuid, [In] uint guidCount, ref UnsafeNativeMethods.EtwTrace.TraceGuidRegistration guidReg, [In] string mofImagePath, [In] string mofResourceName, out ulong regHandle);

      [DllImport("advapi32", CharSet = CharSet.Unicode)]
      internal static extern int UnregisterTraceGuids(ulong regHandle);

      [DllImport("advapi32", CharSet = CharSet.Unicode)]
      internal static extern unsafe uint TraceEvent(ulong traceHandle, char* header);

      internal unsafe delegate uint EtwProc(uint requestCode, IntPtr requestContext, IntPtr bufferSize, byte* buffer);

      internal struct CSTRACE_GUID_REGISTRATION
      {
        internal unsafe Guid* Guid;
        internal uint RegHandle;
      }

      internal struct TraceGuidRegistration
      {
        internal unsafe Guid* Guid;
        internal unsafe void* RegHandle;
      }
    }
  }
}
