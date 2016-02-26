// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.TriggerSourceInformation
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  public abstract class TriggerSourceInformation : IComparable
  {
    public abstract string DisplayName { get; }

    public abstract string DetailedDescription { get; }

    public abstract string GroupBy { get; }

    public static bool operator ==(TriggerSourceInformation leftHandSide, TriggerSourceInformation rightHandSide)
    {
      return object.Equals((object) leftHandSide, (object) rightHandSide);
    }

    public static bool operator !=(TriggerSourceInformation leftHandSide, TriggerSourceInformation rightHandSide)
    {
      return !object.Equals((object) leftHandSide, (object) rightHandSide);
    }

    public static bool operator >(TriggerSourceInformation leftHandSide, TriggerSourceInformation rightHandSide)
    {
      if (leftHandSide != (TriggerSourceInformation) null)
        return leftHandSide.CompareTo((object) rightHandSide) > 0;
      return false;
    }

    public static bool operator <(TriggerSourceInformation leftHandSide, TriggerSourceInformation rightHandSide)
    {
      if (leftHandSide != (TriggerSourceInformation) null)
        return leftHandSide.CompareTo((object) rightHandSide) < 0;
      return false;
    }

    public abstract int CompareTo(object obj);

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }
  }
}
