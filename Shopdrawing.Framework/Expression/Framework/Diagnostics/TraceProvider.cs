// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.TraceProvider
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Expression.Framework.Diagnostics
{
  internal class TraceProvider
  {
    private string _defaultString = "Foo";
    private const byte _noOfBytesPerArg = (byte) 3;
    private UnsafeNativeMethods.EtwTrace.EtwProc _etwProc;
    private ulong _registrationHandle;
    private ulong _traceHandle;
    private uint _level;
    private uint _flags;
    private bool _enabled;

    internal uint Flags
    {
      get
      {
        return this._flags;
      }
    }

    internal uint Level
    {
      get
      {
        return this._level;
      }
    }

    internal bool IsEnabled
    {
      get
      {
        return this._enabled;
      }
    }

    internal TraceProvider()
    {
    }

    [SecurityCritical]
    internal TraceProvider(string _applicationName, Guid controlGuid)
    {
      int num = (int) this.Register(controlGuid);
    }

    [SecuritySafeCritical]
    ~TraceProvider()
    {
      UnsafeNativeMethods.EtwTrace.UnregisterTraceGuids(this._registrationHandle);
      GC.KeepAlive((object) this._etwProc);
    }

    [SecuritySafeCritical]
    internal unsafe uint MyCallback(uint requestCode, IntPtr context, IntPtr bufferSize, byte* byteBuffer)
    {
      try
      {
        TraceProvider.BaseEvent* baseEventPtr = (TraceProvider.BaseEvent*) byteBuffer;
        switch (requestCode)
        {
          case 4U:
            this._traceHandle = baseEventPtr->HistoricalContext;
            this._flags = (uint) UnsafeNativeMethods.EtwTrace.GetTraceEnableFlags(baseEventPtr->HistoricalContext);
            this._level = (uint) UnsafeNativeMethods.EtwTrace.GetTraceEnableLevel(baseEventPtr->HistoricalContext);
            this._enabled = true;
            break;
          case 5U:
            this._enabled = false;
            this._traceHandle = 0UL;
            this._level = 0U;
            this._flags = 0U;
            break;
          default:
            this._enabled = false;
            this._traceHandle = 0UL;
            break;
        }
        return 0U;
      }
      catch (Exception ex)
      {
        return 0U;
      }
    }

    [SecurityCritical]
    private unsafe uint Register(Guid controlGuid)
    {
      UnsafeNativeMethods.EtwTrace.TraceGuidRegistration guidReg = new UnsafeNativeMethods.EtwTrace.TraceGuidRegistration();
      Guid guid = new Guid("{b4955bf0-3af1-4740-b475-99055d3fe9aa}");
      this._etwProc = new UnsafeNativeMethods.EtwTrace.EtwProc(this.MyCallback);
      guidReg.Guid = &guid;
      guidReg.RegHandle = (void*) null;
      return UnsafeNativeMethods.EtwTrace.RegisterTraceGuids(this._etwProc, (void*) null, ref controlGuid, 1U, ref guidReg, (string) null, (string) null, out this._registrationHandle);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0)
    {
      return this.TraceEvent(eventGuid, evtype, data0, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, (object) null, (object) null, (object) null, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, data3, (object) null, (object) null, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3, object data4)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, data3, data4, (object) null, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3, object data4, object data5)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, data3, data4, data5, (object) null, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3, object data4, object data5, object data6)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, data3, data4, data5, data6, (object) null, (object) null);
    }

    internal uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3, object data4, object data5, object data6, object data7)
    {
      return this.TraceEvent(eventGuid, evtype, data0, data1, data2, data3, data4, data5, data6, data7, (object) null);
    }

    [SecuritySafeCritical]
    internal unsafe uint TraceEvent(Guid eventGuid, uint evtype, object data0, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
    {
      char* chPtr1 = stackalloc char[128];
      uint offSet = 0U;
      char* ptr1 = chPtr1;
      int num1 = 0;
      uint num2 = 0U;
      int num3 = 0;
      string str1;
      string str2 = str1 = this._defaultString;
      string str3 = str1;
      string str4 = str1;
      string str5 = str1;
      string str6 = str1;
      string str7 = str1;
      string str8 = str1;
      string str9 = str1;
      string str10 = str1;
      TraceProvider.BaseEvent baseEvent;
      baseEvent.ClientContext = 0U;
      baseEvent.Flags = 1179648U;
      baseEvent.Guid = eventGuid;
      baseEvent.ProviderId = evtype;
      if (data0 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        if ((str10 = this.ProcessOneObject(data0, mofField, ptr1, ref offSet)) != null)
        {
          num1 |= 1;
          ++num3;
        }
      }
      if (data1 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str9 = this.ProcessOneObject(data1, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 2;
          ++num3;
        }
      }
      if (data2 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str8 = this.ProcessOneObject(data2, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 4;
          ++num3;
        }
      }
      if (data3 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str7 = this.ProcessOneObject(data3, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 8;
          ++num3;
        }
      }
      if (data4 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str6 = this.ProcessOneObject(data4, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 16;
          ++num3;
        }
      }
      if (data5 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str5 = this.ProcessOneObject(data5, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 32;
          ++num3;
        }
      }
      if (data6 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str4 = this.ProcessOneObject(data6, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 64;
          ++num3;
        }
      }
      if (data7 != null)
      {
        ++num2;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str3 = this.ProcessOneObject(data7, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 128;
          ++num3;
        }
      }
      if (data8 != null)
      {
        uint num4 = num2 + 1U;
        TraceProvider.MofField* mofField = &baseEvent.UserData + num3++;
        char* ptr2 = chPtr1 + offSet;
        if ((str2 = this.ProcessOneObject(data8, mofField, ptr2, ref offSet)) != null)
        {
          num1 |= 256;
          ++num3;
        }
      }
      uint num5;
      fixed (char* chPtr2 = str10)
        fixed (char* chPtr3 = str9)
          fixed (char* chPtr4 = str8)
            fixed (char* chPtr5 = str7)
              fixed (char* chPtr6 = str6)
                fixed (char* chPtr7 = str5)
                  fixed (char* chPtr8 = str4)
                    fixed (char* chPtr9 = str3)
                      fixed (char* chPtr10 = str2)
                      {
                        int index1 = 0;
                        if ((num1 & 1) != 0)
                        {
                          ++index1;
                          (&baseEvent.UserData)[index1].DataLength = (uint) (str10.Length * 2);
                          (&baseEvent.UserData)[index1].DataPointer = (void*) chPtr2;
                        }
                        int index2 = index1 + 1;
                        if ((num1 & 2) != 0)
                        {
                          ++index2;
                          (&baseEvent.UserData)[index2].DataLength = (uint) (str9.Length * 2);
                          (&baseEvent.UserData)[index2].DataPointer = (void*) chPtr3;
                        }
                        int index3 = index2 + 1;
                        if ((num1 & 4) != 0)
                        {
                          ++index3;
                          (&baseEvent.UserData)[index3].DataLength = (uint) (str8.Length * 2);
                          (&baseEvent.UserData)[index3].DataPointer = (void*) chPtr4;
                        }
                        int index4 = index3 + 1;
                        if ((num1 & 8) != 0)
                        {
                          ++index4;
                          (&baseEvent.UserData)[index4].DataLength = (uint) (str7.Length * 2);
                          (&baseEvent.UserData)[index4].DataPointer = (void*) chPtr5;
                        }
                        int index5 = index4 + 1;
                        if ((num1 & 16) != 0)
                        {
                          ++index5;
                          (&baseEvent.UserData)[index5].DataLength = (uint) (str6.Length * 2);
                          (&baseEvent.UserData)[index5].DataPointer = (void*) chPtr6;
                        }
                        int index6 = index5 + 1;
                        if ((num1 & 32) != 0)
                        {
                          ++index6;
                          (&baseEvent.UserData)[index6].DataLength = (uint) (str5.Length * 2);
                          (&baseEvent.UserData)[index6].DataPointer = (void*) chPtr7;
                        }
                        int index7 = index6 + 1;
                        if ((num1 & 64) != 0)
                        {
                          ++index7;
                          (&baseEvent.UserData)[index7].DataLength = (uint) (str4.Length * 2);
                          (&baseEvent.UserData)[index7].DataPointer = (void*) chPtr8;
                        }
                        int index8 = index7 + 1;
                        if ((num1 & 128) != 0)
                        {
                          ++index8;
                          (&baseEvent.UserData)[index8].DataLength = (uint) (str3.Length * 2);
                          (&baseEvent.UserData)[index8].DataPointer = (void*) chPtr9;
                        }
                        int num4 = index8 + 1;
                        if ((num1 & 256) != 0)
                        {
                          int index9 = num4 + 1;
                          (&baseEvent.UserData)[index9].DataLength = (uint) (str2.Length * 2);
                          (&baseEvent.UserData)[index9].DataPointer = (void*) chPtr10;
                        }
                        baseEvent.BufferSize = (uint) (48 + num3 * 16);
                        num5 = UnsafeNativeMethods.EtwTrace.TraceEvent(this._traceHandle, (char*) &baseEvent);
                      }
      return num5;
    }

    private unsafe string ProcessOneObject(object data, TraceProvider.MofField* mofField, char* ptr, ref uint offSet)
    {
      return this.EncodeObject(data, mofField, ptr, ref offSet, (byte*) null);
    }

    private unsafe string EncodeObject(object data, TraceProvider.MofField* mofField, char* ptr, ref uint offSet, byte* ptrArgInfo)
    {
      if (data == null)
      {
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
        {
          *ptrArgInfo = (byte) 0;
          *(short*) (ptrArgInfo + 1) = (short) 0;
        }
        mofField->DataLength = 0U;
        mofField->DataPointer = (void*) null;
        return (string) null;
      }
      string str1 = data as string;
      if (str1 != null)
      {
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
        {
          *ptrArgInfo = (byte) 2;
          *(short*) (ptrArgInfo + 1) = str1.Length < (int) ushort.MaxValue ? (short) (ushort) str1.Length : (short) -1;
        }
        else
        {
          mofField->DataLength = 2U;
          ushort* numPtr = (ushort*) ptr;
          *numPtr = str1.Length * 2 < (int) ushort.MaxValue ? (ushort) (str1.Length * 2) : ushort.MaxValue;
          mofField->DataPointer = (void*) numPtr;
          offSet += 2U;
        }
        return str1;
      }
      if (data is sbyte)
      {
        mofField->DataLength = 1U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 3;
        sbyte* numPtr = (sbyte*) ptr;
        *numPtr = (sbyte) data;
        mofField->DataPointer = (void*) numPtr;
        ++offSet;
      }
      else if (data is byte)
      {
        mofField->DataLength = 1U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 4;
        byte* numPtr = (byte*) ptr;
        *numPtr = (byte) data;
        mofField->DataPointer = (void*) numPtr;
        ++offSet;
      }
      else if (data is short)
      {
        mofField->DataLength = 2U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 5;
        short* numPtr = (short*) ptr;
        *numPtr = (short) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 2U;
      }
      else if (data is ushort)
      {
        mofField->DataLength = 2U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 6;
        ushort* numPtr = (ushort*) ptr;
        *numPtr = (ushort) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 2U;
      }
      else if (data is int)
      {
        mofField->DataLength = 4U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 7;
        int* numPtr = (int*) ptr;
        *numPtr = (int) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 4U;
      }
      else if (data is uint)
      {
        mofField->DataLength = 4U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 8;
        uint* numPtr = (uint*) ptr;
        *numPtr = (uint) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 4U;
      }
      else if (data is long)
      {
        mofField->DataLength = 8U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 9;
        long* numPtr = (long*) ptr;
        *numPtr = (long) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 8U;
      }
      else if (data is ulong)
      {
        mofField->DataLength = 8U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 10;
        ulong* numPtr = (ulong*) ptr;
        *numPtr = (ulong) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 8U;
      }
      else if (data is char)
      {
        mofField->DataLength = 2U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 11;
        char* chPtr = ptr;
        *chPtr = (char) data;
        mofField->DataPointer = (void*) chPtr;
        offSet += 2U;
      }
      else if (data is float)
      {
        mofField->DataLength = 4U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 12;
        float* numPtr = (float*) ptr;
        *numPtr = (float) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 4U;
      }
      else if (data is double)
      {
        mofField->DataLength = 8U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 13;
        double* numPtr = (double*) ptr;
        *numPtr = (double) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 8U;
      }
      else if (data is bool)
      {
        mofField->DataLength = 1U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 14;
        bool* flagPtr = (bool*) ptr;
        *flagPtr = (bool) data;
        mofField->DataPointer = (void*) flagPtr;
        ++offSet;
      }
      else if (data is Decimal)
      {
        mofField->DataLength = 16U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 15;
        Decimal* numPtr = (Decimal*) ptr;
        *numPtr = (Decimal) data;
        mofField->DataPointer = (void*) numPtr;
        offSet += 16U;
      }
      else if (data.GetType().IsEnum)
      {
        mofField->DataLength = 8U;
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
          *ptrArgInfo = (byte) 9;
        long* numPtr = (long*) ptr;
        *numPtr = Convert.ToInt64(data);
        mofField->DataPointer = (void*) numPtr;
        offSet += 8U;
      }
      else
      {
        string str2 = data.ToString();
        if ((IntPtr) ptrArgInfo != IntPtr.Zero)
        {
          *ptrArgInfo = (byte) 2;
          *(short*) (ptrArgInfo + 1) = str2.Length < (int) ushort.MaxValue ? (short) (ushort) str2.Length : (short) -1;
        }
        else
        {
          mofField->DataLength = 2U;
          ushort* numPtr = (ushort*) ptr;
          *numPtr = str2.Length * 2 < (int) ushort.MaxValue ? (ushort) (str2.Length * 2) : ushort.MaxValue;
          mofField->DataPointer = (void*) numPtr;
          offSet += 2U;
        }
        return str2;
      }
      if ((IntPtr) ptrArgInfo != IntPtr.Zero)
        *(short*) (ptrArgInfo + 1) = (short) (ushort) mofField->DataLength;
      return str1;
    }

    internal sealed class RequestCodes
    {
      internal const uint GetAllData = 0U;
      internal const uint GetSingleInstance = 1U;
      internal const uint SetSingleInstance = 2U;
      internal const uint SetSingleItem = 3U;
      internal const uint EnableEvents = 4U;
      internal const uint DisableEvents = 5U;
      internal const uint EnableCollection = 6U;
      internal const uint DisableCollection = 7U;
      internal const uint RegInfo = 8U;
      internal const uint ExecuteMethod = 9U;

      private RequestCodes()
      {
      }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct MofField
    {
      [FieldOffset(0)]
      internal unsafe void* DataPointer;
      [FieldOffset(8)]
      internal uint DataLength;
      [FieldOffset(12)]
      internal uint DataType;
    }

    [StructLayout(LayoutKind.Explicit, Size = 304)]
    internal struct BaseEvent
    {
      [FieldOffset(0)]
      internal uint BufferSize;
      [FieldOffset(4)]
      internal uint ProviderId;
      [FieldOffset(8)]
      internal ulong HistoricalContext;
      [FieldOffset(16)]
      internal long TimeStamp;
      [FieldOffset(24)]
      internal Guid Guid;
      [FieldOffset(40)]
      internal uint ClientContext;
      [FieldOffset(44)]
      internal uint Flags;
      [FieldOffset(48)]
      internal TraceProvider.MofField UserData;
    }
  }
}
