// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RadialGradientBrushTranslateAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal sealed class RadialGradientBrushTranslateAdorner : BrushAdorner, IClickable
  {
    public RadialGradientBrushTranslateAdorner(BrushTransformAdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      return this.TransformPoint(this.BrushCenter, true) * matrix;
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!this.ShouldDraw || !PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.RadialGradientBrush, (ITypeResolver) this.Element.ProjectContext))
        return;
      Pen pen = new Pen((Brush) Brushes.Transparent, 15.0);
      pen.Freeze();
      Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
      EllipseGeometry ellipseGeometry = new EllipseGeometry((Point) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.CenterProperty), (double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusXProperty), (double) this.GetBrushPropertyAsWpf(RadialGradientBrushNode.RadiusYProperty));
      ellipseGeometry.Transform = (Transform) new MatrixTransform(matrix1);
      ellipseGeometry.Freeze();
      context.DrawGeometry((Brush)null, pen, (System.Windows.Media.Geometry)ellipseGeometry);
      context.DrawGeometry((Brush)null, this.ThinPen, (System.Windows.Media.Geometry)ellipseGeometry);
    }
  }
}
