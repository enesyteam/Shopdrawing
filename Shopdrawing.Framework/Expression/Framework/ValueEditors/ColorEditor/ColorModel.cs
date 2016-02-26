// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.ColorModel
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Windows.Media;

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  public class ColorModel
  {
    private Color color;
    private RgbColor rgbColor;
    private HsbColor hsbColor;
    private HlsColor hlsColor;
    private CmykColor cmykColor;
    private bool isAlphaEnabled;

    public bool IsAlphaEnabled
    {
      get
      {
        return this.isAlphaEnabled;
      }
      set
      {
        if (this.isAlphaEnabled == value)
          return;
        if (!value)
          this.ScA = 1f;
        this.isAlphaEnabled = value;
      }
    }

    public Color Color
    {
      get
      {
        return this.color;
      }
      set
      {
        if (!(this.color != value))
          return;
        this.color = value;
        if (!this.isAlphaEnabled)
          this.color.ScA = 1f;
        this.UpdateFromColor();
      }
    }

    public string String
    {
      get
      {
        string str = this.color.ToString();
        if (!this.isAlphaEnabled && (int) str[0] == 35 && str.Length == 9)
          str = "#" + str.Substring(3, 6);
        return str;
      }
      set
      {
        bool flag = false;
        try
        {
          this.Color = (Color) ColorConverter.ConvertFromString(value);
          flag = true;
        }
        catch (FormatException ex)
        {
        }
        if (!flag)
        {
          try
          {
            this.Color = (Color) ColorConverter.ConvertFromString("#" + value);
            flag = true;
          }
          catch (FormatException ex)
          {
          }
        }
        if (flag)
          return;
        this.OnColorChanged();
      }
    }

    public byte A
    {
      get
      {
        return this.color.A;
      }
      set
      {
        if (!this.isAlphaEnabled)
          return;
        this.color.A = value;
        this.OnColorChanged();
      }
    }

    public byte R
    {
      get
      {
        return this.color.R;
      }
      set
      {
        this.color.R = value;
        this.UpdateFromColor();
      }
    }

    public byte G
    {
      get
      {
        return this.color.G;
      }
      set
      {
        this.color.G = value;
        this.UpdateFromColor();
      }
    }

    public byte B
    {
      get
      {
        return this.color.B;
      }
      set
      {
        this.color.B = value;
        this.UpdateFromColor();
      }
    }

    public float ScA
    {
      get
      {
        return this.color.ScA;
      }
      set
      {
        if (!this.isAlphaEnabled)
          return;
        this.color.ScA = value;
        this.OnColorChanged();
      }
    }

    public float ScR
    {
      get
      {
        return this.color.ScR;
      }
      set
      {
        this.color.ScR = value;
        this.UpdateFromColor();
      }
    }

    public float ScG
    {
      get
      {
        return this.color.ScG;
      }
      set
      {
        this.color.ScG = value;
        this.UpdateFromColor();
      }
    }

    public float ScB
    {
      get
      {
        return this.color.ScB;
      }
      set
      {
        this.color.ScB = value;
        this.UpdateFromColor();
      }
    }

    public float HsbH
    {
      get
      {
        return this.hsbColor.Hue;
      }
      set
      {
        this.hsbColor.Hue = value;
        this.UpdateFromHsb();
      }
    }

    public float HsbS
    {
      get
      {
        return this.hsbColor.Saturation;
      }
      set
      {
        this.hsbColor.Saturation = value;
        this.UpdateFromHsb();
      }
    }

    public float HsbB
    {
      get
      {
        return this.hsbColor.Brightness;
      }
      set
      {
        this.hsbColor.Brightness = value;
        this.UpdateFromHsb();
      }
    }

    public float HlsH
    {
      get
      {
        return this.hlsColor.Hue;
      }
      set
      {
        this.hlsColor.Hue = value;
        this.UpdateFromHls();
      }
    }

    public float HlsL
    {
      get
      {
        return this.hlsColor.Lightness;
      }
      set
      {
        this.hlsColor.Lightness = value;
        this.UpdateFromHls();
      }
    }

    public float HlsS
    {
      get
      {
        return this.hlsColor.Saturation;
      }
      set
      {
        this.hlsColor.Saturation = value;
        this.UpdateFromHls();
      }
    }

    public float Cyan
    {
      get
      {
        return this.cmykColor.Cyan;
      }
      set
      {
        this.cmykColor.Cyan = value;
        this.UpdateFromCmyk();
      }
    }

    public float Magenta
    {
      get
      {
        return this.cmykColor.Magenta;
      }
      set
      {
        this.cmykColor.Magenta = value;
        this.UpdateFromCmyk();
      }
    }

    public float Yellow
    {
      get
      {
        return this.cmykColor.Yellow;
      }
      set
      {
        this.cmykColor.Yellow = value;
        this.UpdateFromCmyk();
      }
    }

    public float Black
    {
      get
      {
        return this.cmykColor.Black;
      }
      set
      {
        this.cmykColor.Black = value;
        this.UpdateFromCmyk();
      }
    }

    public event EventHandler ColorChanged;

    public ColorModel(Color initialColor)
    {
      this.color = initialColor;
      this.isAlphaEnabled = true;
      this.UpdateFromColor();
    }

    public ColorModel(ColorModel source)
    {
      this.CopyFrom(source);
    }

    public void CopyFrom(ColorModel source)
    {
      this.color = source.color;
      this.rgbColor = source.rgbColor;
      this.hsbColor = source.hsbColor;
      this.hlsColor = source.hlsColor;
      this.cmykColor = source.cmykColor;
      this.isAlphaEnabled = source.isAlphaEnabled;
      this.OnColorChanged();
    }

    public bool IsEqualToColor(Color value)
    {
      if (this.GetIsSourceScRGB())
        return Color.AreClose(this.Color, value);
      if ((int) this.Color.A == (int) value.A && (int) this.Color.R == (int) value.R && (int) this.Color.G == (int) value.G)
        return (int) this.Color.B == (int) value.B;
      return false;
    }

    private bool GetIsSourceScRGB()
    {
      return this.color.ToString().StartsWith("sc#", StringComparison.Ordinal);
    }

    private void UpdateFromColor()
    {
      this.UpdateRgbFromColor();
      this.UpdateHsbFromRgb();
      this.UpdateHlsFromRgb();
      this.UpdateCmykFromRgb();
      this.OnColorChanged();
    }

    private void UpdateRgbFromColor()
    {
      this.rgbColor.Red = (float) this.color.R / (float) byte.MaxValue;
      this.rgbColor.Green = (float) this.color.G / (float) byte.MaxValue;
      this.rgbColor.Blue = (float) this.color.B / (float) byte.MaxValue;
    }

    private void UpdateColorFromRgb()
    {
      this.color.R = (byte) Math.Round((double) this.rgbColor.Red * (double) byte.MaxValue);
      this.color.G = (byte) Math.Round((double) this.rgbColor.Green * (double) byte.MaxValue);
      this.color.B = (byte) Math.Round((double) this.rgbColor.Blue * (double) byte.MaxValue);
    }

    private void UpdateFromHsb()
    {
      this.UpdateRgbFromHsb();
      this.UpdateColorFromRgb();
      this.UpdateHlsFromRgb();
      this.hlsColor.Hue = this.hsbColor.Hue;
      this.UpdateCmykFromRgb();
      this.OnColorChanged();
    }

    private void UpdateRgbFromHsb()
    {
      ColorUtility.HsbToRgb(this.hsbColor, ref this.rgbColor);
    }

    private void UpdateHsbFromRgb()
    {
      ColorUtility.RgbToHsb(this.rgbColor, ref this.hsbColor);
    }

    private void UpdateFromHls()
    {
      this.UpdateRgbFromHls();
      this.UpdateColorFromRgb();
      this.UpdateHsbFromRgb();
      this.hsbColor.Hue = this.hlsColor.Hue;
      this.UpdateCmykFromRgb();
      this.OnColorChanged();
    }

    private void UpdateRgbFromHls()
    {
      ColorUtility.HlsToRgb(this.hlsColor, ref this.rgbColor);
    }

    private void UpdateHlsFromRgb()
    {
      ColorUtility.RgbToHls(this.rgbColor, ref this.hlsColor);
    }

    private void UpdateFromCmyk()
    {
      this.UpdateRgbFromCmyk();
      this.UpdateColorFromRgb();
      this.UpdateHsbFromRgb();
      this.UpdateHlsFromRgb();
      this.OnColorChanged();
    }

    private void UpdateRgbFromCmyk()
    {
      ColorUtility.CmykToRgb(this.cmykColor, ref this.rgbColor);
    }

    private void UpdateCmykFromRgb()
    {
      ColorUtility.RgbToCmyk(this.rgbColor, ref this.cmykColor);
    }

    private void OnColorChanged()
    {
      if (this.ColorChanged == null)
        return;
      this.ColorChanged((object) this, new EventArgs());
    }
  }
}
