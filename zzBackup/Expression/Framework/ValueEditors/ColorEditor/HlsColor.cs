// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.HlsColor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  internal struct HlsColor
  {
    public float Hue;
    public float Lightness;
    public float Saturation;

    public HlsColor(float hue, float lightness, float saturation)
    {
      this.Hue = hue;
      this.Lightness = lightness;
      this.Saturation = saturation;
    }
  }
}
