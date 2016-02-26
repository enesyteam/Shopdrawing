// Decompiled with JetBrains decompiler
// Type: MS.Internal.Transforms.Tolerances
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows;

namespace MS.Internal.Transforms
{
  internal static class Tolerances
  {
    private static readonly double ZeroThreshold = 2.22044604925031E-15;

    public static bool NearZero(double d)
    {
      return Math.Abs(d) < Tolerances.ZeroThreshold;
    }

    public static bool NearZero(Vector vector)
    {
      if (Tolerances.NearZero(vector.X))
        return Tolerances.NearZero(vector.Y);
      return false;
    }
  }
}
