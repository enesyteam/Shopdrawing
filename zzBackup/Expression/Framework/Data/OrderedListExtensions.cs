// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.OrderedListExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Data
{
  public static class OrderedListExtensions
  {
    public static int GenericBinarySearch<ElementT, CompareT>(this IList<ElementT> list, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      int num1 = 0;
      int num2 = list.Count - 1;
      while (num1 <= num2)
      {
        int index = (num1 + num2) / 2;
        ElementT elementT = list[index];
        int num3 = comparison(value, elementT);
        if (num3 == 0)
          return index;
        if (num3 < 0)
          num2 = index - 1;
        else
          num1 = index + 1;
      }
      return ~num1;
    }

    public static int HiBound<ElementT, CompareT>(this IList<ElementT> list, int v, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      if (v >= 0 && v < list.Count)
      {
        while (v + 1 < list.Count && comparison(value, list[v + 1]) == 0)
          ++v;
      }
      return v;
    }

    public static int LoBound<ElementT, CompareT>(this IList<ElementT> list, int v, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      if (v > 0 && v < list.Count)
      {
        ElementT elementT = list[v];
        while (v - 1 >= 0 && comparison(value, list[v - 1]) == 0)
          --v;
      }
      return v;
    }

    public static bool GetHiLoBounds<ElementT, CompareT>(this IList<ElementT> list, CompareT item, Func<CompareT, ElementT, int> comparison, out int highIndex, out int lowIndex)
    {
      int v = OrderedListExtensions.GenericBinarySearch<ElementT, CompareT>(list, item, comparison);
      if (v < 0)
      {
        lowIndex = v;
        highIndex = v;
        return false;
      }
      lowIndex = OrderedListExtensions.LoBound<ElementT, CompareT>(list, v, item, comparison);
      highIndex = OrderedListExtensions.HiBound<ElementT, CompareT>(list, v, item, comparison);
      return true;
    }

    public static int GenericBinarySearch<ElementT, CompareT>(this ElementT[] list, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      int num1 = 0;
      int num2 = list.Length - 1;
      while (num1 <= num2)
      {
        int index = (num1 + num2) / 2;
        ElementT elementT = list[index];
        int num3 = comparison(value, elementT);
        if (num3 == 0)
          return index;
        if (num3 < 0)
          num2 = index - 1;
        else
          num1 = index + 1;
      }
      return ~num1;
    }

    public static int HiBound<ElementT, CompareT>(this ElementT[] list, int v, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      if (v >= 0 && v < list.Length)
      {
        while (v + 1 < list.Length && comparison(value, list[v + 1]) == 0)
          ++v;
      }
      return v;
    }

    public static int LoBound<ElementT, CompareT>(this ElementT[] list, int v, CompareT value, Func<CompareT, ElementT, int> comparison)
    {
      if (v > 0 && v < list.Length)
      {
        ElementT elementT = list[v];
        while (v - 1 >= 0 && comparison(value, list[v - 1]) == 0)
          --v;
      }
      return v;
    }

    public static bool GetHiLoBounds<ElementT, CompareT>(this ElementT[] list, CompareT item, Func<CompareT, ElementT, int> comparison, out int highIndex, out int lowIndex)
    {
      int v = OrderedListExtensions.GenericBinarySearch<ElementT, CompareT>(list, item, comparison);
      if (v < 0)
      {
        lowIndex = v;
        highIndex = v;
        return false;
      }
      lowIndex = OrderedListExtensions.LoBound<ElementT, CompareT>(list, v, item, comparison);
      highIndex = OrderedListExtensions.HiBound<ElementT, CompareT>(list, v, item, comparison);
      return true;
    }
  }
}
