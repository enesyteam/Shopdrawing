// Decompiled with JetBrains decompiler
// Type: MS.Internal.EqualityArray
// Assembly: Microsoft.Windows.Design.Extensibility, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4673B7C2-4EF5-4715-85F2-D8E573468337
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Extensibility\Microsoft.Windows.Design.Extensibility.dll

namespace MS.Internal
{
  internal class EqualityArray
  {
    private object[] _values;

    internal EqualityArray(params object[] values)
    {
      this._values = values;
    }

    public override bool Equals(object other)
    {
      EqualityArray equalityArray = other as EqualityArray;
      if (equalityArray == null || equalityArray._values.Length != this._values.Length)
        return false;
      for (int index = 0; index < this._values.Length; ++index)
      {
        if (this._values[index] != equalityArray._values[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return this._values[0].GetHashCode();
    }
  }
}
