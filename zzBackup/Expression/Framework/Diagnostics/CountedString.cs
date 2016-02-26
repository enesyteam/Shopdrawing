// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.CountedString
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public class CountedString
  {
    private int count;
    private string stringValue;

    public string StringValue
    {
      get
      {
        return this.stringValue;
      }
    }

    public int Count
    {
      get
      {
        return this.count;
      }
      set
      {
        this.count = value;
      }
    }

    public CountedString(string stringValue)
    {
      this.stringValue = stringValue;
      this.count = 1;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", new object[2]
      {
        (object) this.count,
        (object) this.stringValue
      });
    }
  }
}
