// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Interop.ErrorHandler
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Utility.Interop
{
  public static class ErrorHandler
  {
    private const int FACILITY_WINDOWS = 8;

    private static int HRESULT_CODE(int hr)
    {
      return hr & (int) ushort.MaxValue;
    }

    private static int HRESULT_FACILITY(int hr)
    {
      return hr >> 16 & 8191;
    }

    private static int ConvertHResult(int result)
    {
      if (ErrorHandler.HRESULT_FACILITY(result) == 8)
        return ErrorHandler.HRESULT_CODE(result);
      return result;
    }

    public static string HResultToString(int result)
    {
      int errorCode = ErrorHandler.ConvertHResult(result);
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "HResult {0:D} [0x{0:X}]: {1}", new object[2]
      {
        (object) result.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        (object) ErrorHandler.FormatMessage(errorCode)
      });
    }

    public static string LastErrorToString(int errorNumber)
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Error {0}: {1}", new object[2]
      {
        (object) errorNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        (object) ErrorHandler.FormatMessage(errorNumber)
      });
    }

    private static string FormatMessage(int errorCode)
    {
      return new Win32Exception(errorCode).Message;
    }

    public static void ThrowWindowsError(int errorCode, string description)
    {
      throw new Win32Exception(errorCode, description + " " + ErrorHandler.LastErrorToString(errorCode));
    }

    public static void ThrowHResultError(int result, string description)
    {
      throw new Win32Exception(ErrorHandler.ConvertHResult(result), description + " " + ErrorHandler.HResultToString(result));
    }
  }
}
