// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.KeyAttributePair`1
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  public struct KeyAttributePair<T> where T : Attribute
  {
    private string key;
    private T value;

    public string Key
    {
      get
      {
        return this.key;
      }
    }

    public T Value
    {
      get
      {
        return this.value;
      }
    }

    public KeyAttributePair(string key, T value)
    {
      this.key = key;
      this.value = value;
    }

    public static bool operator ==(KeyAttributePair<T> pair1, KeyAttributePair<T> pair2)
    {
      return pair1.Equals((object) pair2);
    }

    public static bool operator !=(KeyAttributePair<T> pair1, KeyAttributePair<T> pair2)
    {
      return !(pair1 == pair2);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is KeyAttributePair<T>))
        return false;
      KeyAttributePair<T> keyAttributePair = (KeyAttributePair<T>) obj;
      if (!string.Equals(this.Key, keyAttributePair.Key))
        return false;
      return object.Equals((object) this.Value, (object) keyAttributePair.Value);
    }

    public override int GetHashCode()
    {
      int num = 0;
      if (this.Key == null)
        num ^= this.Key.GetHashCode();
      if ((object) this.Value != null)
        num ^= this.Value.GetHashCode();
      return num;
    }
  }
}
