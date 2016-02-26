// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.NumberRangesAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.PropertyEditing;
using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class NumberRangesAttribute : Attribute, IIndexableAttribute
  {
    private double? hardMinimum;
    private double? minimum;
    private double? maximum;
    private double? hardMaximum;
    private bool? canBeAuto;
    private KeyAttributeMap<NumberRangesAttribute> map;

    public double? HardMinimum
    {
      get
      {
        return this.hardMinimum;
      }
    }

    public double? Minimum
    {
      get
      {
        return this.minimum;
      }
    }

    public double? Maximum
    {
      get
      {
        return this.maximum;
      }
    }

    public double? HardMaximum
    {
      get
      {
        return this.hardMaximum;
      }
    }

    public bool? CanBeAuto
    {
      get
      {
        return this.canBeAuto;
      }
    }

    public Attribute this[string key]
    {
      get
      {
        if (this.map != null)
          return (Attribute) this.map[key] ?? (Attribute) this;
        return (Attribute) this;
      }
    }

    public NumberRangesAttribute()
    {
      this.hardMinimum = new double?();
      this.minimum = new double?();
      this.maximum = new double?();
      this.hardMaximum = new double?();
      this.canBeAuto = new bool?();
      this.map = (KeyAttributeMap<NumberRangesAttribute>) null;
    }

    public NumberRangesAttribute(double? hardMinimum, double? minimum, double? maximum, double? hardMaximum, bool? canBeAuto)
    {
      this.hardMinimum = hardMinimum;
      this.minimum = minimum;
      this.maximum = maximum;
      this.hardMaximum = hardMaximum;
      this.canBeAuto = canBeAuto;
      this.map = (KeyAttributeMap<NumberRangesAttribute>) null;
    }

    public NumberRangesAttribute(KeyAttributePair<NumberRangesAttribute>[] mapArray)
    {
      this.hardMinimum = new double?();
      this.minimum = new double?();
      this.maximum = new double?();
      this.hardMaximum = new double?();
      this.canBeAuto = new bool?();
      this.map = new KeyAttributeMap<NumberRangesAttribute>(mapArray);
    }

    public NumberRangesAttribute(double? hardMinimum, double? minimum, double? maximum, double? hardMaximum, bool? canBeAuto, KeyAttributePair<NumberRangesAttribute>[] mapArray)
    {
      this.hardMinimum = hardMinimum;
      this.minimum = minimum;
      this.maximum = maximum;
      this.hardMaximum = hardMaximum;
      this.canBeAuto = canBeAuto;
      this.map = new KeyAttributeMap<NumberRangesAttribute>(mapArray);
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      NumberRangesAttribute numberRangesAttribute = obj as NumberRangesAttribute;
      if (numberRangesAttribute == null)
        return false;
      if (this.map != null && numberRangesAttribute.map != null)
        return this.map.Equals((object) numberRangesAttribute.map);
      double? nullable1 = numberRangesAttribute.hardMinimum;
      double? nullable2 = this.hardMinimum;
      if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0)
      {
        double? nullable3 = numberRangesAttribute.minimum;
        double? nullable4 = this.minimum;
        if ((nullable3.GetValueOrDefault() != nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue == nullable4.HasValue ? 1 : 0)) != 0)
        {
          double? nullable5 = numberRangesAttribute.maximum;
          double? nullable6 = this.maximum;
          if ((nullable5.GetValueOrDefault() != nullable6.GetValueOrDefault() ? 0 : (nullable5.HasValue == nullable6.HasValue ? 1 : 0)) != 0)
          {
            double? nullable7 = numberRangesAttribute.hardMaximum;
            double? nullable8 = this.hardMaximum;
            if ((nullable7.GetValueOrDefault() != nullable8.GetValueOrDefault() ? 0 : (nullable7.HasValue == nullable8.HasValue ? 1 : 0)) != 0)
            {
              bool? nullable9 = numberRangesAttribute.canBeAuto;
              bool? nullable10 = this.canBeAuto;
              if (nullable9.GetValueOrDefault() == nullable10.GetValueOrDefault())
                return nullable9.HasValue == nullable10.HasValue;
              return false;
            }
          }
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      if (this.map != null)
        return this.map.GetHashCode();
      return this.hardMinimum.GetHashCode() ^ this.minimum.GetHashCode() ^ this.maximum.GetHashCode() ^ this.hardMaximum.GetHashCode() ^ this.canBeAuto.GetHashCode();
    }
  }
}
