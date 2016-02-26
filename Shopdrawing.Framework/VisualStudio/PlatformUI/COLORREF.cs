// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.COLORREF
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  internal struct COLORREF
  {
    public uint dwColor;

    public COLORREF(uint dwColor)
    {
      this.dwColor = dwColor;
    }

    public COLORREF(Color color)
    {
      this.dwColor = (uint) ((int) color.R + ((int) color.G << 8) + ((int) color.B << 16));
    }

    public Color GetMediaColor()
    {
      return Color.FromRgb((byte) ((uint) byte.MaxValue & this.dwColor), (byte) ((65280U & this.dwColor) >> 8), (byte) ((16711680U & this.dwColor) >> 16));
    }
  }
}
