// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Annotations.ExceptionChecks
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.Annotations
{
  internal static class ExceptionChecks
  {
    [DebuggerHidden]
    public static void CheckNullArgument<T>(T arg, string name) where T : class
    {
      if ((object) arg == null)
        throw new ArgumentNullException(name);
    }

    [DebuggerHidden]
    public static void CheckNullInvalidOperation<T>(T arg, string message) where T : class
    {
      if ((object) arg == null)
        throw new InvalidOperationException(message);
    }

    [DebuggerHidden]
    public static void CheckEmptyListArgument<T>(IEnumerable<T> list, string name)
    {
      if (!Enumerable.Any<T>(list))
        throw new ArgumentOutOfRangeException(name);
    }
  }
}
