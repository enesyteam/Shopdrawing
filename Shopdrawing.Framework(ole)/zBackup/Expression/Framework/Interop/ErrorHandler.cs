// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Interop.ErrorHandler
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Expression.Framework.Interop
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
      return string.Format("HResult {0:D} [0x{0:X}]: {1}", (object) result.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ErrorHandler.FormatMessage(errorCode));
    }

    public static string LastErrorToString(int errorNumber)
    {
      return string.Format("Error {0}: {1}", (object) errorNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) ErrorHandler.FormatMessage(errorNumber));
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
