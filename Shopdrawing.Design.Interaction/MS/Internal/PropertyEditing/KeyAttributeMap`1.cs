// Decompiled with JetBrains decompiler
// Type: MS.Internal.PropertyEditing.KeyAttributeMap`1
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;

namespace MS.Internal.PropertyEditing
{
  internal class KeyAttributeMap<T> where T : Attribute
  {
    private KeyAttributePair<T>[] mapArray;

    public T this[string key]
    {
      get
      {
        if (this.mapArray != null)
        {
          for (int index = 0; index < this.mapArray.Length; ++index)
          {
            if (string.Equals(this.mapArray[index].Key, key, StringComparison.Ordinal))
              return this.mapArray[index].Value;
          }
        }
        return default (T);
      }
    }

    internal KeyAttributeMap(KeyAttributePair<T>[] mapArray)
    {
      this.mapArray = mapArray;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      KeyAttributeMap<T> keyAttributeMap = obj as KeyAttributeMap<T>;
      if (keyAttributeMap == null)
        return false;
      if (this.mapArray == keyAttributeMap.mapArray)
        return true;
      if (this.mapArray == null || keyAttributeMap.mapArray == null || this.mapArray.Length != keyAttributeMap.mapArray.Length)
        return false;
      for (int index = 0; index < this.mapArray.Length; ++index)
      {
        if (!this.mapArray[index].Equals((object) keyAttributeMap.mapArray[index]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int num = 0;
      for (int index = 0; index < this.mapArray.Length; ++index)
        num ^= this.mapArray[index].GetHashCode();
      return num;
    }
  }
}
