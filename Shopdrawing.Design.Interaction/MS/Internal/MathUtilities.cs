// Decompiled with JetBrains decompiler
// Type: MS.Internal.MathUtilities
// Assembly: Microsoft.Windows.Design.Interaction, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B798143-45D7-48F3-B5E1-F76B9E61E2E6
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Windows.Design.Interaction\Microsoft.Windows.Design.Interaction.dll

using System;
using System.Windows;
using System.Windows.Media;

namespace MS.Internal
{
  internal static class MathUtilities
  {
    private static double DBL_EPSILON = 2.22044604925031E-16;

    internal static bool AreClose(Size s1, Size s2)
    {
      if (MathUtilities.AreClose(s1.Width, s2.Width))
        return MathUtilities.AreClose(s1.Height, s2.Height);
      return false;
    }

    internal static bool AreClose(Vector s1, Vector s2)
    {
      if (MathUtilities.AreClose(s1.X, s2.X))
        return MathUtilities.AreClose(s1.Y, s2.Y);
      return false;
    }

    internal static bool AreClose(double value1, double value2)
    {
      if (value1 == value2)
        return true;
      double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * MathUtilities.DBL_EPSILON;
      double num2 = value1 - value2;
      if (-num1 < num2)
        return num1 > num2;
      return false;
    }

    internal static bool AreClose(Point value1, Point value2)
    {
      if (MathUtilities.AreClose(value1.X, value2.X))
        return MathUtilities.AreClose(value1.Y, value2.Y);
      return false;
    }

    internal static bool AreClose(Rect value1, Rect value2)
    {
      if (MathUtilities.AreClose(value1.TopLeft, value2.TopLeft))
        return MathUtilities.AreClose(value1.Size, value2.Size);
      return false;
    }

    internal static bool AreClose(Matrix m1, Matrix m2)
    {
      return MathUtilities.AreClose(m1.OffsetX, m2.OffsetX) && MathUtilities.AreClose(m1.OffsetY, m2.OffsetY) && (MathUtilities.AreClose(m1.M11, m2.M11) && MathUtilities.AreClose(m1.M12, m2.M12)) && (MathUtilities.AreClose(m1.M21, m2.M21) && MathUtilities.AreClose(m1.M22, m2.M22));
    }

    internal static double Round(double value, double rounding)
    {
      return Math.Round(rounding * Math.Round(value / rounding, MidpointRounding.AwayFromZero), DesignerUtilities.DesignerRoundingPrecision, MidpointRounding.AwayFromZero);
    }

    internal static double Round(double value)
    {
      return Math.Round(value, DesignerUtilities.DesignerRoundingPrecision, MidpointRounding.AwayFromZero);
    }
  }
}
