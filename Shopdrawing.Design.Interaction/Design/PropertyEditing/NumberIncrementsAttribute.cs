// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.NumberIncrementsAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.PropertyEditing;
using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class NumberIncrementsAttribute : Attribute, IIndexableAttribute
  {
    private double? smallChange;
    private double? defaultChange;
    private double? largeChange;
    private KeyAttributeMap<NumberIncrementsAttribute> map;

    public double? SmallChange
    {
      get
      {
        return this.smallChange;
      }
    }

    public double? DefaultChange
    {
      get
      {
        return this.defaultChange;
      }
    }

    public double? LargeChange
    {
      get
      {
        return this.largeChange;
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

    public NumberIncrementsAttribute()
    {
      this.smallChange = new double?();
      this.defaultChange = new double?();
      this.largeChange = new double?();
      this.map = (KeyAttributeMap<NumberIncrementsAttribute>) null;
    }

    public NumberIncrementsAttribute(double? smallChange, double? defaultChange, double? largeChange)
    {
      this.smallChange = smallChange;
      this.defaultChange = defaultChange;
      this.largeChange = largeChange;
      this.map = (KeyAttributeMap<NumberIncrementsAttribute>) null;
    }

    public NumberIncrementsAttribute(KeyAttributePair<NumberIncrementsAttribute>[] mapArray)
    {
      this.smallChange = new double?();
      this.defaultChange = new double?();
      this.largeChange = new double?();
      this.map = new KeyAttributeMap<NumberIncrementsAttribute>(mapArray);
    }

    public NumberIncrementsAttribute(double? smallChange, double? defaultChange, double? largeChange, KeyAttributePair<NumberIncrementsAttribute>[] mapArray)
    {
      this.smallChange = smallChange;
      this.defaultChange = defaultChange;
      this.largeChange = largeChange;
      this.map = new KeyAttributeMap<NumberIncrementsAttribute>(mapArray);
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      NumberIncrementsAttribute incrementsAttribute = obj as NumberIncrementsAttribute;
      if (incrementsAttribute == null)
        return false;
      if (this.map != null && incrementsAttribute.map != null)
        return this.map.Equals((object) incrementsAttribute.map);
      double? nullable1 = incrementsAttribute.smallChange;
      double? nullable2 = this.smallChange;
      if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0)
      {
        double? nullable3 = incrementsAttribute.defaultChange;
        double? nullable4 = this.defaultChange;
        if ((nullable3.GetValueOrDefault() != nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue == nullable4.HasValue ? 1 : 0)) != 0)
        {
          double? nullable5 = incrementsAttribute.largeChange;
          double? nullable6 = this.largeChange;
          if (nullable5.GetValueOrDefault() == nullable6.GetValueOrDefault())
            return nullable5.HasValue == nullable6.HasValue;
          return false;
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      if (this.map != null)
        return this.map.GetHashCode();
      return this.smallChange.GetHashCode() ^ this.defaultChange.GetHashCode() ^ this.largeChange.GetHashCode();
    }
  }
}
