// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Diagnostics.DebugHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Expression.Framework.Diagnostics
{
  public static class DebugHelper
  {
    private static Dictionary<string, int> assertHistory = new Dictionary<string, int>();

    [Conditional("DEBUG")]
    public static void AssertOnce(bool condition)
    {
      int num = condition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    public static void AssertOnce(bool condition, string message)
    {
      int num = condition ? 1 : 0;
    }

    [Conditional("DEBUG")]
    public static void FailOnce(string message)
    {
    }

    [Conditional("DEBUG")]
    private static void InternalFailOnce(string message)
    {
      StackFrame stackFrame = new StackFrame(2, true);
      string key = stackFrame.GetFileName() + stackFrame.GetFileLineNumber().ToString((IFormatProvider) CultureInfo.CurrentCulture);
      int num = 0;
      if (!DebugHelper.assertHistory.TryGetValue(key, out num))
      {
        DebugHelper.assertHistory[key] = 1;
      }
      else
      {
        Dictionary<string, int> dictionary;
        string index;
        (dictionary = DebugHelper.assertHistory)[index = key] = dictionary[index] + 1;
      }
    }
  }
}
