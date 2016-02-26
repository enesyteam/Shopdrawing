// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Design.Interaction.AdornerPlacementValue
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;

namespace Microsoft.Windows.Design.Interaction
{
  public struct AdornerPlacementValue
  {
    private AdornerPlacementDimension _term;
    private double _contribution;

    public AdornerPlacementDimension Term
    {
      get
      {
        return this._term;
      }
      set
      {
        if (!EnumValidator.IsValid(value))
          throw new ArgumentOutOfRangeException("value");
        this._term = value;
      }
    }

    public double Contribution
    {
      get
      {
        return this._contribution;
      }
      set
      {
        this._contribution = value;
      }
    }

    public AdornerPlacementValue(AdornerPlacementDimension term, double contribution)
    {
      if (!EnumValidator.IsValid(term))
        throw new ArgumentOutOfRangeException("term");
      this._term = term;
      this._contribution = contribution;
    }

    public static bool operator ==(AdornerPlacementValue placementValue1, AdornerPlacementValue placementValue2)
    {
      if (placementValue1.Term == placementValue2.Term)
        return placementValue1.Contribution == placementValue2.Contribution;
      return false;
    }

    public static bool operator !=(AdornerPlacementValue placementValue1, AdornerPlacementValue placementValue2)
    {
      if (placementValue1.Term == placementValue2.Term)
        return placementValue1.Contribution != placementValue2.Contribution;
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj is AdornerPlacementValue)
        return this.Equals((AdornerPlacementValue) obj);
      return false;
    }

    public bool Equals(AdornerPlacementValue value)
    {
      if (this.Term == value.Term)
        return this.Contribution == value.Contribution;
      return false;
    }

    public override int GetHashCode()
    {
      return this.Term.GetHashCode() ^ this.Contribution.GetHashCode();
    }
  }
}
