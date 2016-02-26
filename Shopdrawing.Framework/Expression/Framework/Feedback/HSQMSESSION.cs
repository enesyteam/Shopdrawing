// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Feedback.HSQMSESSION
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework.Feedback
{
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  internal sealed class HSQMSESSION : SafeHandle
  {
    [MarshalAs(UnmanagedType.LPWStr)]
    private string _filePattern;

    public string FilePattern
    {
      get
      {
        return this._filePattern;
      }
      set
      {
        this._filePattern = value;
      }
    }

    public static uint MaxFilesToQueue
    {
      get
      {
        return 48U;
      }
    }

    public override bool IsInvalid
    {
      get
      {
        if (!this.IsClosed)
          return this.handle == IntPtr.Zero;
        return true;
      }
    }

    public HSQMSESSION()
      : base(IntPtr.Zero, true)
    {
    }

    protected override bool ReleaseHandle()
    {
      if (this.handle == IntPtr.Zero)
        return true;
      UnsafeNativeMethods.EndSession(this.handle, this.FilePattern, HSQMSESSION.MaxFilesToQueue, 8U);
      Marshal.GetLastWin32Error();
      this.handle = IntPtr.Zero;
      return true;
    }
  }
}
