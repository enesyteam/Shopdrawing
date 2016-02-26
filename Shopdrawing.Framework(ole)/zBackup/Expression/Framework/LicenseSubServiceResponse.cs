// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.LicenseSubServiceResponse
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework
{
  internal class LicenseSubServiceResponse : ILicenseSubServiceResponse
  {
    public int Version
    {
      get
      {
        return 1;
      }
    }

    public bool Success
    {
      get
      {
        if ((int) this.ErrorCode == 0)
          return this.Exception == null;
        return false;
      }
    }

    public uint ErrorCode { get; set; }

    public Exception Exception { get; set; }
  }
}
