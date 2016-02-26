// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.CmykColor
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  internal struct CmykColor
  {
    public float Cyan;
    public float Magenta;
    public float Yellow;
    public float Black;

    public CmykColor(float cyan, float magenta, float yellow, float black)
    {
      this.Cyan = cyan;
      this.Magenta = magenta;
      this.Yellow = yellow;
      this.Black = black;
    }
  }
}
