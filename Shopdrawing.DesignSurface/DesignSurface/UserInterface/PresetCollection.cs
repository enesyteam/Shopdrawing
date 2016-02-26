// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PresetCollection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Collections;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public sealed class PresetCollection : IEnumerable
  {
    private double[] presetValues;
    private double defaultValue;

    public int Count
    {
      get
      {
        return this.presetValues.Length;
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

    public PresetCollection(double[] presetValues, double defaultValue)
    {
      this.presetValues = (double[]) presetValues.Clone();
      Array.Sort<double>(this.presetValues);
      this.defaultValue = defaultValue;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.presetValues.GetEnumerator();
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
