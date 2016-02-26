// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushScaleAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class BrushScaleAdorner : BrushAnchorPointAdorner, IClickable
  {
    public BrushScaleAdorner(BrushTransformAdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      return this.GetOffsetAnchorPoint(matrix, 0.5 * SizeAdorner.Size);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.ShouldDraw)
        return;
      Point anchorPoint = this.GetAnchorPoint(matrix);
      matrix = this.GetCompleteBrushTransformMatrix(true) * matrix;
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      SizeAdorner.DrawSizeAdorner(ctx, anchorPoint, this.EdgeFlags, matrix, brush, this.ThinPen, this.BrushBounds, AdornerRenderLocation.Outside);
    }
  }
}
