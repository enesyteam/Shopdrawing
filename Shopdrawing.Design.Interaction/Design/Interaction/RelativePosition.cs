// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.RelativePosition
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Collections;

namespace Microsoft.Windows.Design.Interaction
{
  public sealed class RelativePosition : IEnumerable
  {
    private RelativePosition[] _values;
    private string _name;

    public RelativePosition(params RelativePosition[] values)
      : this((string) null, values)
    {
    }

    public RelativePosition(string name, params RelativePosition[] values)
    {
      if (values == null)
        throw new ArgumentNullException("values");
      if (values.Length > 0)
      {
        this._values = new RelativePosition[RelativePosition.CountValues(values)];
        RelativePosition.FillValues(this._values, values, 0);
      }
      else
        this._values = values;
      if (name == null || name.Length == 0)
        name = this._values.Length <= 0 ? string.Empty : string.Concat((object[]) this._values);
      this._name = name;
    }

    public static bool operator ==(RelativePosition point1, RelativePosition point2)
    {
      if (object.ReferenceEquals((object) point1, (object) point2))
        return true;
      if (object.ReferenceEquals((object) point1, (object) null))
        return false;
      return point1.Equals(point2);
    }

    public static bool operator !=(RelativePosition point1, RelativePosition point2)
    {
      if (object.ReferenceEquals((object) point1, (object) point2))
        return false;
      if (object.ReferenceEquals((object) point1, (object) null))
        return true;
      return !point1.Equals(point2);
    }

    public bool Contains(RelativePosition value)
    {
      if (value == (RelativePosition) null)
        throw new ArgumentNullException("value");
      if (object.ReferenceEquals((object) value, (object) this))
        return true;
      foreach (RelativePosition relativePosition in this._values)
      {
        if (object.ReferenceEquals((object) value, (object) relativePosition))
          return true;
      }
      return false;
    }

    private static int CountValues(RelativePosition[] values)
    {
      int num = 0;
      foreach (RelativePosition relativePosition in values)
      {
        if (relativePosition._values.Length > 0)
          num += RelativePosition.CountValues(relativePosition._values);
        else
          ++num;
      }
      return num;
    }

    public override bool Equals(object obj)
    {
      RelativePosition position = obj as RelativePosition;
      if (position != (RelativePosition) null)
        return this.Equals(position);
      return false;
    }

    public bool Equals(RelativePosition position)
    {
      if (object.ReferenceEquals((object) position, (object) null))
        return false;
      if (object.ReferenceEquals((object) position, (object) this))
        return true;
      if (this._values.Length == 0 || position._values.Length != this._values.Length)
        return false;
      int num = 0;
      for (int index1 = 0; index1 < this._values.Length; ++index1)
      {
        for (int index2 = 0; index2 < position._values.Length; ++index2)
        {
          if (object.ReferenceEquals((object) this._values[index1], (object) position._values[index2]))
          {
            ++num;
            break;
          }
        }
      }
      return num == this._values.Length;
    }

    private static int FillValues(RelativePosition[] array, RelativePosition[] values, int startingIndex)
    {
      foreach (RelativePosition relativePosition in values)
      {
        if (relativePosition._values.Length > 0)
          startingIndex = RelativePosition.FillValues(array, relativePosition._values, startingIndex);
        else
          array[startingIndex++] = relativePosition;
      }
      return startingIndex;
    }

    public override int GetHashCode()
    {
      if (this._values.Length <= 0)
        return base.GetHashCode();
      int hashCode = this._values[0].GetHashCode();
      for (int index = 1; index < this._values.Length; ++index)
        hashCode ^= this._values[index].GetHashCode();
      return hashCode;
    }

    public override string ToString()
    {
      if (this._name != null)
        return this._name;
      return base.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      if (this._values != null && this._values.Length > 0)
        return this._values.GetEnumerator();
      return new RelativePosition[1]
      {
        this
      }.GetEnumerator();
    }
  }
}
