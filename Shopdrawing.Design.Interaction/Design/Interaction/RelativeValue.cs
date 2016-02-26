// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.RelativeValue
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal;

namespace Microsoft.Windows.Design.Interaction
{
  public struct RelativeValue
  {
    private RelativePosition _reference;
    private double _value;

    public RelativePosition Position
    {
      get
      {
        return this._reference;
      }
      set
      {
        this._reference = value;
      }
    }

    public double Value
    {
      get
      {
        return this._value;
      }
      set
      {
        this._value = value;
      }
    }

    public RelativeValue(RelativePosition reference, double value)
    {
      this._reference = reference;
      this._value = value;
    }

    public static bool operator ==(RelativeValue first, RelativeValue second)
    {
      if (first._reference == second._reference)
        return MathUtilities.AreClose(first._value, second._value);
      return false;
    }

    public static bool operator !=(RelativeValue first, RelativeValue second)
    {
      if (!(first._reference != second._reference))
        return !MathUtilities.AreClose(first._value, second._value);
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj is RelativeValue)
        return this == (RelativeValue) obj;
      return false;
    }

    public override int GetHashCode()
    {
      return this._reference.GetHashCode() ^ this._value.GetHashCode();
    }
  }
}
