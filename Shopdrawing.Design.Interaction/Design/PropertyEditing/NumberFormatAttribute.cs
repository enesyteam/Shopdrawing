// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.PropertyEditing.NumberFormatAttribute
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using MS.Internal.PropertyEditing;
using System;

namespace Microsoft.Windows.Design.PropertyEditing
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class NumberFormatAttribute : Attribute, IIndexableAttribute
  {
    private string formatString;
    private int? maxPrecision;
    private double? scale;
    private KeyAttributeMap<NumberFormatAttribute> map;

    public string FormatString
    {
      get
      {
        return this.formatString;
      }
    }

    public int? MaxPrecision
    {
      get
      {
        return this.maxPrecision;
      }
    }

    public double? Scale
    {
      get
      {
        return this.scale;
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

    public NumberFormatAttribute()
    {
      this.formatString = (string) null;
      this.maxPrecision = new int?();
      this.scale = new double?();
      this.map = (KeyAttributeMap<NumberFormatAttribute>) null;
    }

    public NumberFormatAttribute(string formatString, int? maxPrecision, double? scale)
    {
      this.formatString = formatString;
      this.maxPrecision = maxPrecision;
      this.scale = scale;
      this.map = (KeyAttributeMap<NumberFormatAttribute>) null;
    }

    public NumberFormatAttribute(KeyAttributePair<NumberFormatAttribute>[] mapArray)
    {
      this.formatString = (string) null;
      this.maxPrecision = new int?();
      this.scale = new double?();
      this.map = new KeyAttributeMap<NumberFormatAttribute>(mapArray);
    }

    public NumberFormatAttribute(string formatString, int? maxPrecision, double? scale, KeyAttributePair<NumberFormatAttribute>[] mapArray)
    {
      this.formatString = formatString;
      this.maxPrecision = maxPrecision;
      this.scale = scale;
      this.map = new KeyAttributeMap<NumberFormatAttribute>(mapArray);
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      NumberFormatAttribute numberFormatAttribute = obj as NumberFormatAttribute;
      if (numberFormatAttribute == null)
        return false;
      if (this.map != null && numberFormatAttribute.map != null)
        return this.map.Equals((object) numberFormatAttribute.map);
      if (numberFormatAttribute.formatString == this.formatString)
      {
        int? nullable1 = numberFormatAttribute.maxPrecision;
        int? nullable2 = this.maxPrecision;
        if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0)
        {
          double? nullable3 = numberFormatAttribute.scale;
          double? nullable4 = this.scale;
          if (nullable3.GetValueOrDefault() == nullable4.GetValueOrDefault())
            return nullable3.HasValue == nullable4.HasValue;
          return false;
        }
      }
      return false;
    }

    public override int GetHashCode()
    {
      if (this.map != null)
        return this.map.GetHashCode();
      return (this.formatString != null ? this.formatString.GetHashCode() : 0) ^ this.maxPrecision.GetHashCode() ^ this.scale.GetHashCode();
    }
  }
}
