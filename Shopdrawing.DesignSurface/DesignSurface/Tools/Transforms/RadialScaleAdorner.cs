// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialScaleAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RadialScaleAdorner : BrushScaleAdorner
  {
    public RadialScaleAdorner(BrushTransformAdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext) || !(this.AdornerSet.Behavior.Tool is GradientBrushTool))
        return;
      base.Draw(ctx, matrix);
    }
  }
}
