// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.Collections.CollectionExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Extensions.Collections
{
  public static class CollectionExtensions
  {
    public static void AddRange<TBase, TDerived>(this ICollection<TBase> collection, IEnumerable<TDerived> items) where TDerived : TBase
    {
      foreach (TDerived derived in items)
      {
        TBase @base = (TBase) derived;
        collection.Add(@base);
      }
    }

    public static void ReplaceAll<TBase, TDerived>(this ICollection<TBase> collection, IEnumerable<TDerived> items) where TDerived : TBase
    {
      collection.Clear();
      CollectionExtensions.AddRange<TBase, TDerived>(collection, items);
    }
  }
}
