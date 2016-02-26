// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.ValueEditors.ColorEditor.YellowGradientConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.Expression.Framework.ValueEditors.ColorEditor
{
  public class YellowGradientConverter : BaseGradientConverter
  {
    protected override void UpdateModelForGradientMin(ColorModel model)
    {
      model.Yellow = 0.0f;
    }

    protected override void UpdateModelForGradientMax(ColorModel model)
    {
      model.Yellow = 1f;
    }
  }
}
