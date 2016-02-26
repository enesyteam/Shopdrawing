// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterLength
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  [TypeConverter(typeof (SplitterLengthConverter))]
  public struct SplitterLength : IEquatable<SplitterLength>
  {
    private double unitValue;
    private SplitterUnitType unitType;

    public SplitterUnitType SplitterUnitType
    {
      get
      {
        return this.unitType;
      }
    }

    public double Value
    {
      get
      {
        return this.unitValue;
      }
    }

    public bool IsFill
    {
      get
      {
        return this.SplitterUnitType == SplitterUnitType.Fill;
      }
    }

    public bool IsStretch
    {
      get
      {
        return this.SplitterUnitType == SplitterUnitType.Stretch;
      }
    }

    public bool IsFixed
    {
      get
      {
        return this.SplitterUnitType == SplitterUnitType.Fixed;
      }
    }

    public SplitterLength(double value)
    {
      this = new SplitterLength(value, SplitterUnitType.Stretch);
    }

    public SplitterLength(double value, SplitterUnitType unitType)
    {
      this.unitValue = value;
      this.unitType = unitType;
    }

    public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
    {
      if (obj1.SplitterUnitType == obj2.SplitterUnitType)
        return obj1.Value == obj2.Value;
      return false;
    }

    public static bool operator !=(SplitterLength obj1, SplitterLength obj2)
    {
      return !(obj1 == obj2);
    }

    public override bool Equals(object obj)
    {
      if (obj is SplitterLength)
        return this == (SplitterLength) obj;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) ((int) this.unitValue + this.unitType);
    }

    public bool Equals(SplitterLength other)
    {
      return this == other;
    }

    public override string ToString()
    {
      return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
    }
  }
}
