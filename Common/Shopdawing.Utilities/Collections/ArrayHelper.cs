// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.ArrayHelper
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

namespace Microsoft.Expression.Utility.Collections
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
