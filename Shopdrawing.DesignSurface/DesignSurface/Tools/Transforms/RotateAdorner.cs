// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Tools;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RotateAdorner : AnchorPointAdorner, IClickable
  {
    public RotateAdorner(AdornerSet adornerSet, EdgeFlags edgeFlags)
      : base(adornerSet, edgeFlags)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      return this.GetOffsetAnchorPoint(matrix, 9.0);
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      Point anchorPoint = this.GetAnchorPoint(matrix);
      RotateAdornerHelper.DrawAdorner(ctx, anchorPoint, this.EdgeFlags, matrix, (Brush) Brushes.Transparent, this.ElementBounds);
    }
  }
}
