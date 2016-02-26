// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Collections.ArrayHelper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.Collections
{
  public static class ArrayHelper
  {
    public static bool Compare<T>(T[] left, T[] right)
    {
      bool flag1 = left == null || left.Length == 0;
      bool flag2 = right == null || right.Length == 0;
      if (flag1 && flag2)
        return true;
      if (flag1 || flag2 || left.Length != right.Length)
        return false;
      for (int index = 0; index < left.Length; ++index)
      {
        if (!left[index].Equals((object) right[index]))
          return false;
      }
      return true;
    }
  }
}
