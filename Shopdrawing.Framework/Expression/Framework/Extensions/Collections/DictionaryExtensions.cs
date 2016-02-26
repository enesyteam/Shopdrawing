// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Extensions.Collections.DictionaryExtensions
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Extensions.Collections
{
  public static class DictionaryExtensions
  {
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
      TValue obj;
      if (!dictionary.TryGetValue(key, out obj))
        return default (TValue);
      return obj;
    }
  }
}
