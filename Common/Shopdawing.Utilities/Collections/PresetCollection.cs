// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.Collections.PresetCollection
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.Utility.Collections
{
  public sealed class PresetCollection : IEnumerable<double>, IEnumerable
  {
    private List<double> presetValues = new List<double>();
    private double defaultValue;

    public int Count
    {
      get
      {
        return this.presetValues.Count;
      }
    }

    public double this[int index]
    {
      get
      {
        return this.presetValues[index];
      }
    }

    public double Minimum
    {
      get
      {
        return this[0];
      }
    }

    public double Maximum
    {
      get
      {
        return this[this.Count - 1];
      }
    }

    public double Default
    {
      get
      {
        return this.defaultValue;
      }
    }

    public PresetCollection(IEnumerable<double> presetValues, double defaultValue)
    {
      this.presetValues.AddRange(presetValues);
      this.presetValues.Sort();
      this.defaultValue = defaultValue;
    }

    public IEnumerator<double> GetEnumerator()
    {
      return (IEnumerator<double>) this.presetValues.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.presetValues.GetEnumerator();
    }

    public double NextSmallerPreset(double value)
    {
      for (int index = this.Count - 1; index >= 0; --index)
      {
        double num = this[index];
        if (num < value)
          return num;
      }
      return this.Minimum;
    }

    public double NextLargerPreset(double value)
    {
      for (int index = 0; index < this.Count; ++index)
      {
        double num = this[index];
        if (num > value)
          return num;
      }
      return this.Maximum;
    }

    public bool Contains(double value)
    {
      foreach (double num in this.presetValues)
      {
        if (num == value)
          return true;
      }
      return false;
    }
  }
}
