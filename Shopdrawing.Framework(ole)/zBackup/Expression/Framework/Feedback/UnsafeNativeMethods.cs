// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Feedback.UnsafeNativeMethods
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework.Feedback
{
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  internal static class UnsafeNativeMethods
  {
    [DllImport("sqmapi.dll", EntryPoint = "SqmIsWindowsOptedIn", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowsOptedIn();

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetSession", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern HSQMSESSION GetSession(string pwszSessionIdentifier, uint cbMaxSessionSize, uint dwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetSessionStartTime", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern System.Runtime.InteropServices.ComTypes.FILETIME GetSessionStartTime(HSQMSESSION hSession);

    [DllImport("sqmapi.dll", EntryPoint = "SqmStartSession", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool StartSession(HSQMSESSION hSession);

    [DllImport("sqmapi.dll", EntryPoint = "SqmEndSession", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EndSession(IntPtr hSession, string pwszPattern, uint dwMaxFilesToQueue, uint dwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmEndSession", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EndSession(HSQMSESSION hSession, string pwszPattern, uint dwMaxFilesToQueue, uint dwFlags);

    [DllImport("sqmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SqmFlushSession(HSQMSESSION hSession, string pwszFileName);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSet", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Set(HSQMSESSION hSession, uint dwId, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetBool", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetBool(HSQMSESSION hSession, uint dwId, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetBits", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetBits(HSQMSESSION hSession, uint dwId, uint dwOrBits);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetIfMax", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetIfMax(HSQMSESSION hSession, uint dwId, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetIfMin", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetIfMin(HSQMSESSION hSession, uint dwId, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmIncrement", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Increment(HSQMSESSION hSession, uint dwId, uint dwInc);

    [DllImport("sqmapi.dll", EntryPoint = "SqmAddToAverage", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddToAverage(HSQMSESSION hSession, uint dwId, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmAddToStream", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddToStream(HSQMSESSION hSession, uint dwId, int nArgs, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9);

    [DllImport("sqmapi.dll", EntryPoint = "SqmAddToStreamDWord", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddToStreamDWord(HSQMSESSION hSession, uint dwId, uint cTuple, uint dwVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmAddToStreamString", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddToStreamString(HSQMSESSION hSession, uint dwId, uint cTuple, string pwszVal);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetFlags", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetFlags(HSQMSESSION hSession, uint dwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmClearFlags", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClearFlags(HSQMSESSION hSession, uint dwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetFlags", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetFlags(HSQMSESSION hSession, out uint pdwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetEnabled", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetEnabled(HSQMSESSION hSession);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetEnabled", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetEnabled(HSQMSESSION hSession, [MarshalAs(UnmanagedType.Bool)] bool fEnabled);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetAppVersion", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetAppVersion(HSQMSESSION hSession, uint dwVersionHigh, uint dwVersionLow);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetAppId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetAppId(HSQMSESSION hSession, uint dwAppId);

    [DllImport("sqmapi.dll", EntryPoint = "SqmTimerStart", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TimerStart(HSQMSESSION hSession, uint dwId);

    [DllImport("sqmapi.dll", EntryPoint = "SqmTimerRecord", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TimerRecord(HSQMSESSION hSession, uint dwId);

    [DllImport("sqmapi.dll", EntryPoint = "SqmTimerAccumulate", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TimerAccumulate(HSQMSESSION hSession, uint dwId);

    [DllImport("sqmapi.dll", EntryPoint = "SqmTimerAddToAverage", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TimerAddToAverage(HSQMSESSION hSession, uint dwId);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetMachineId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetMachineId(HSQMSESSION hSession, ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetUserId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetUserId(HSQMSESSION hSession, ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetMachineId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMachineId(HSQMSESSION hSession, ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmGetUserId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetUserId(HSQMSESSION hSession, ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmReadSharedMachineId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadSharedMachineId(ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmReadSharedUserId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadSharedUserId(ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmWriteSharedMachineId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteSharedMachineId(ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmWriteSharedUserId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteSharedUserId(ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmCreateNewId", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateNewId(ref Guid pGuid);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetCurrentTimeAsUploadTime", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCurrentTimeAsUploadTime(string pszSqmFileName);

    [DllImport("sqmapi.dll", EntryPoint = "SqmStartUpload", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern uint StartUpload(string szPattern, string szUrl, string szSecureUrl, uint dwFlags, UnsafeNativeMethods.UploadCallBack pfnCallback);

    [DllImport("sqmapi.dll", EntryPoint = "SqmWaitForUploadComplete", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WaitForUploadComplete(uint dwTimeoutMilliseconds, uint dwFlags);

    [DllImport("sqmapi.dll", EntryPoint = "SqmSetString", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetString(HSQMSESSION hSession, uint dwId, string pwszVal);

    public enum UploadFlags
    {
      UPLOAD_SINGLE_FILE = 1,
      UPLOAD_ALL_FILES = 2,
      IGNORE_WINDOWS_OPTIN = 4,
    }

    [Flags]
    public enum SaveFlags
    {
      OVERWRITE_ANY_FILE = 1,
      OVERWRITE_OLDEST_FILE = 2,
      CONTINUE_SESSION = 4,
      RELEASE_SESSION = 8,
      USE_WINDOWS_UPLOADER = 16,
    }

    [Flags]
    public enum SessionFlags
    {
      SESSION_CREATE_NEW = 1,
    }

    public enum SuccessCodes
    {
      SQM_FILEPATH_IGNORED = 513,
    }

    public enum ErrorCodes
    {
      ERROR_INVALID_HANDLE = 6,
      ERROR_INVALID_PARAMETER = 87,
      SESSION_NOT_INITIALIZED = 268435713,
      SESSION_OUT_OF_CONTEXT = 268435714,
      SESSION_DISABLED = 268435715,
      DATA_TYPE_MISMATCH = 268435716,
      TIMER_NOT_STARTED = 268435717,
      SESSION_NOT_FOUND = 268435718,
      DATA_NOTFOUND = 268435719,
      FILE_NOTFOUND = 268435720,
      UPLOAD_TIMEOUT = 268435721,
      INIT_FAILED = 268435722,
      SESSION_FULL = 268435723,
      NETWORK_NOT_AVAILABLE = 268435724,
      STREAM_FULL = 268435725,
    }

    public enum UploadCodes : long
    {
      SQM_E_UPLOAD_FAILED = 2147483920L,
    }

    public enum HeaderFlags
    {
      FLAG_DEBUG = 1,
      FLAG_NEVER_THROTTLE = 2,
      SECURE_UPLOAD = 4,
      DO_NOT_UPLOAD = 8,
      FLAG_TEST = 16,
      FLAG_INTERNAL = 32,
      FLAG_PARTIAL_SESSION = 64,
    }

    public enum StreamType
    {
      STATIC,
      CYCLIC,
    }

    public delegate uint UploadCallBack(uint hr, [MarshalAs(UnmanagedType.LPWStr)] string filePath, uint dwHttpResponse);
  }
}
