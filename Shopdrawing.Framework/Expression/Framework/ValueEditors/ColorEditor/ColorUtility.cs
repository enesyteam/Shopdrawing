// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.ColorUtility
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  internal class ColorUtility
  {
    public static void RgbToHsb(RgbColor rgbColor, ref HsbColor hsbColor)
    {
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
      float num1 = Math.Min(rgbColor.Red, Math.Min(rgbColor.Green, rgbColor.Blue));
      float num2 = Math.Max(rgbColor.Red, Math.Max(rgbColor.Green, rgbColor.Blue));
      hsbColor.Brightness = num2;
      if ((double) num1 == (double) num2)
      {
        if ((double) hsbColor.Brightness > 0.0)
          hsbColor.Saturation = 0.0f;
      }
      else
      {
        float num3 = num2 - num1;
        hsbColor.Saturation = num3 / num2;
        hsbColor.Hue = (double) rgbColor.Red != (double) num2 ? ((double) rgbColor.Green != (double) num2 ? (float) (4.0 + ((double) rgbColor.Red - (double) rgbColor.Green) / (double) num3) : (float) (2.0 + ((double) rgbColor.Blue - (double) rgbColor.Red) / (double) num3)) : (rgbColor.Green - rgbColor.Blue) / num3;
        if ((double) hsbColor.Hue < 0.0)
          hsbColor.Hue += 6f;
        hsbColor.Hue /= 6f;
      }
      hsbColor.Hue = Math.Min(1f, Math.Max(0.0f, hsbColor.Hue));
      hsbColor.Saturation = Math.Min(1f, Math.Max(0.0f, hsbColor.Saturation));
      hsbColor.Brightness = Math.Min(1f, Math.Max(0.0f, hsbColor.Brightness));
    }

    public static void HsbToRgb(HsbColor hsbColor, ref RgbColor rgbColor)
    {
      hsbColor.Hue = Math.Min(1f, Math.Max(0.0f, hsbColor.Hue));
      hsbColor.Saturation = Math.Min(1f, Math.Max(0.0f, hsbColor.Saturation));
      hsbColor.Brightness = Math.Min(1f, Math.Max(0.0f, hsbColor.Brightness));
      if ((double) hsbColor.Saturation == 0.0)
      {
        rgbColor.Red = rgbColor.Green = rgbColor.Blue = hsbColor.Brightness;
      }
      else
      {
        float num1 = (float) (((double) hsbColor.Hue - Math.Floor((double) hsbColor.Hue)) * 6.0);
        int num2 = (int) num1;
        float num3 = num1 - (float) num2;
        float num4 = hsbColor.Brightness * (1f - hsbColor.Saturation);
        float num5 = hsbColor.Brightness * (float) (1.0 - (double) hsbColor.Saturation * (double) num3);
        float num6 = hsbColor.Brightness * (float) (1.0 - (double) hsbColor.Saturation * (1.0 - (double) num3));
        switch (num2 % 6)
        {
          case 0:
            rgbColor.Red = hsbColor.Brightness;
            rgbColor.Green = num6;
            rgbColor.Blue = num4;
            break;
          case 1:
            rgbColor.Red = num5;
            rgbColor.Green = hsbColor.Brightness;
            rgbColor.Blue = num4;
            break;
          case 2:
            rgbColor.Red = num4;
            rgbColor.Green = hsbColor.Brightness;
            rgbColor.Blue = num6;
            break;
          case 3:
            rgbColor.Red = num4;
            rgbColor.Green = num5;
            rgbColor.Blue = hsbColor.Brightness;
            break;
          case 4:
            rgbColor.Red = num6;
            rgbColor.Green = num4;
            rgbColor.Blue = hsbColor.Brightness;
            break;
          case 5:
            rgbColor.Red = hsbColor.Brightness;
            rgbColor.Green = num4;
            rgbColor.Blue = num5;
            break;
          default:
            throw new InvalidOperationException(ExceptionStringTable.CannotReachHere);
        }
      }
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
    }

    public static void RgbToHls(RgbColor rgbColor, ref HlsColor hlsColor)
    {
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
      float num1 = Math.Min(rgbColor.Red, Math.Min(rgbColor.Green, rgbColor.Blue));
      float num2 = Math.Max(rgbColor.Red, Math.Max(rgbColor.Green, rgbColor.Blue));
      hlsColor.Lightness = (float) (((double) num1 + (double) num2) / 2.0);
      if ((double) num1 == (double) num2)
      {
        if ((double) hlsColor.Lightness > 0.0 && (double) hlsColor.Lightness < 1.0)
          hlsColor.Saturation = 0.0f;
      }
      else
      {
        float num3 = num2 - num1;
        hlsColor.Saturation = (double) hlsColor.Lightness <= 0.5 ? num3 / (num1 + num2) : num3 / (2f - num1 - num2);
        hlsColor.Hue = (double) rgbColor.Red != (double) num2 ? ((double) rgbColor.Green != (double) num2 ? (float) (4.0 + ((double) rgbColor.Red - (double) rgbColor.Green) / (double) num3) : (float) (2.0 + ((double) rgbColor.Blue - (double) rgbColor.Red) / (double) num3)) : (rgbColor.Green - rgbColor.Blue) / num3;
        if ((double) hlsColor.Hue < 0.0)
          hlsColor.Hue += 6f;
        hlsColor.Hue /= 6f;
      }
      hlsColor.Hue = Math.Min(1f, Math.Max(0.0f, hlsColor.Hue));
      hlsColor.Lightness = Math.Min(1f, Math.Max(0.0f, hlsColor.Lightness));
      hlsColor.Saturation = Math.Min(1f, Math.Max(0.0f, hlsColor.Saturation));
    }

    public static void HlsToRgb(HlsColor hlsColor, ref RgbColor rgbColor)
    {
      hlsColor.Hue = Math.Min(1f, Math.Max(0.0f, hlsColor.Hue));
      hlsColor.Lightness = Math.Min(1f, Math.Max(0.0f, hlsColor.Lightness));
      hlsColor.Saturation = Math.Min(1f, Math.Max(0.0f, hlsColor.Saturation));
      if ((double) hlsColor.Saturation == 0.0)
      {
        rgbColor.Red = rgbColor.Green = rgbColor.Blue = hlsColor.Lightness;
      }
      else
      {
        float hue = (float) (((double) hlsColor.Hue - Math.Floor((double) hlsColor.Hue)) * 6.0);
        float n2 = (double) hlsColor.Lightness > 0.5 ? (float) ((double) hlsColor.Lightness + (double) hlsColor.Saturation - (double) hlsColor.Lightness * (double) hlsColor.Saturation) : hlsColor.Lightness * (1f + hlsColor.Saturation);
        float n1 = 2f * hlsColor.Lightness - n2;
        rgbColor.Red = ColorUtility.HlsValue(n1, n2, hue + 2f);
        rgbColor.Green = ColorUtility.HlsValue(n1, n2, hue);
        rgbColor.Blue = ColorUtility.HlsValue(n1, n2, hue - 2f);
      }
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
    }

    public static void RgbToCmyk(RgbColor rgbColor, ref CmykColor cmykColor)
    {
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
      cmykColor.Cyan = 1f - rgbColor.Red;
      cmykColor.Magenta = 1f - rgbColor.Green;
      cmykColor.Yellow = 1f - rgbColor.Blue;
      cmykColor.Black = Math.Min(cmykColor.Cyan, Math.Min(cmykColor.Magenta, cmykColor.Yellow));
      if ((double) cmykColor.Black == 1.0)
      {
        cmykColor.Cyan = cmykColor.Magenta = cmykColor.Yellow = 0.0f;
      }
      else
      {
        cmykColor.Cyan = (float) (((double) cmykColor.Cyan - (double) cmykColor.Black) / (1.0 - (double) cmykColor.Black));
        cmykColor.Magenta = (float) (((double) cmykColor.Magenta - (double) cmykColor.Black) / (1.0 - (double) cmykColor.Black));
        cmykColor.Yellow = (float) (((double) cmykColor.Yellow - (double) cmykColor.Black) / (1.0 - (double) cmykColor.Black));
      }
      cmykColor.Cyan = Math.Min(1f, Math.Max(0.0f, cmykColor.Cyan));
      cmykColor.Magenta = Math.Min(1f, Math.Max(0.0f, cmykColor.Magenta));
      cmykColor.Yellow = Math.Min(1f, Math.Max(0.0f, cmykColor.Yellow));
      cmykColor.Black = Math.Min(1f, Math.Max(0.0f, cmykColor.Black));
    }

    public static void CmykToRgb(CmykColor cmykColor, ref RgbColor rgbColor)
    {
      cmykColor.Cyan = Math.Min(1f, Math.Max(0.0f, cmykColor.Cyan));
      cmykColor.Magenta = Math.Min(1f, Math.Max(0.0f, cmykColor.Magenta));
      cmykColor.Yellow = Math.Min(1f, Math.Max(0.0f, cmykColor.Yellow));
      cmykColor.Black = Math.Min(1f, Math.Max(0.0f, cmykColor.Black));
      rgbColor.Red = (float) (1.0 - (double) cmykColor.Cyan * (1.0 - (double) cmykColor.Black)) - cmykColor.Black;
      rgbColor.Green = (float) (1.0 - (double) cmykColor.Magenta * (1.0 - (double) cmykColor.Black)) - cmykColor.Black;
      rgbColor.Blue = (float) (1.0 - (double) cmykColor.Yellow * (1.0 - (double) cmykColor.Black)) - cmykColor.Black;
      rgbColor.Red = Math.Min(1f, Math.Max(0.0f, rgbColor.Red));
      rgbColor.Green = Math.Min(1f, Math.Max(0.0f, rgbColor.Green));
      rgbColor.Blue = Math.Min(1f, Math.Max(0.0f, rgbColor.Blue));
    }

    private static float HlsValue(float n1, float n2, float hue)
    {
      if ((double) hue < 0.0)
        hue += 6f;
      else if ((double) hue >= 6.0)
        hue -= 6f;
      if ((double) hue < 1.0)
        return n1 + (n2 - n1) * hue;
      if ((double) hue < 3.0)
        return n2;
      if ((double) hue < 4.0)
        return n1 + (float) (((double) n2 - (double) n1) * (4.0 - (double) hue));
      return n1;
    }
  }
}
