// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushRotateAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class BrushRotateAdorner : BrushAnchorPointAdorner, IClickable
  {
    public BrushRotateAdorner(BrushTransformAdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      return this.GetOffsetAnchorPoint(matrix, SizeAdorner.Size + 4.0);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      if (!this.ShouldDraw || PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.LinearGradientBrush, (ITypeResolver) this.Element.ProjectContext) && this.AdornerSet.Behavior.Tool is GradientBrushTool)
        return;
      Rect brushBounds = this.BrushBounds;
      if (brushBounds.Width > 0.0 && brushBounds.Height > 0.0)
      {
        Point anchorPoint = this.GetAnchorPoint(matrix);
        Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
        RotateAdornerHelper.DrawAdorner(ctx, anchorPoint, this.EdgeFlags, matrix1, (Brush) Brushes.Transparent);
      }
      else
      {
        if (brushBounds.Width <= 0.0 && brushBounds.Height <= 0.0 || this.EdgeFlags != EdgeFlags.TopLeft && this.EdgeFlags != EdgeFlags.BottomRight)
          return;
        Point anchorPoint = this.GetAnchorPoint(matrix);
        ctx.DrawEllipse((Brush) Brushes.Transparent, (Pen) null, anchorPoint, RotateAdornerHelper.Radius, RotateAdornerHelper.Radius);
      }
    }
  }
}
