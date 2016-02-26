// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.StringLogicalComparer
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework
{
  public class StringLogicalComparer : IComparer<string>
  {
    public static readonly StringLogicalComparer Instance = new StringLogicalComparer();

    private StringLogicalComparer()
    {
    }

    public int Compare(string x, string y)
    {
      return NativeMethods.StrCmpLogicalW(x, y);
    }
  }
}
