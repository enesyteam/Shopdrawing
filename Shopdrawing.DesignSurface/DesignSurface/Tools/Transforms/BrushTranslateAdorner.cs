// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.BrushTranslateAdorner
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
  internal sealed class BrushTranslateAdorner : BrushAdorner, IClickable
  {
    public BrushTranslateAdorner(BrushTransformAdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public override Point GetClickablePoint(Matrix matrix)
    {
      return this.TransformPoint(this.BrushCenter, true) * matrix;
    }

    public override void Draw(DrawingContext context, Matrix matrix)
    {
      if (!this.ShouldDraw)
        return;
      bool flag = this.AdornerSet.Behavior.Tool is BrushTransformTool;
      Matrix matrix1 = this.GetCompleteBrushTransformMatrix(true) * matrix;
      if (flag)
      {
        Rect brushBounds = this.BrushBounds;
        if (brushBounds.Width <= 0.0 || brushBounds.Height <= 0.0)
          return;
        Pen thinPen = this.ThinPen;
        System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(brushBounds, matrix1, thinPen.Thickness);
        context.DrawGeometry((Brush) Brushes.Transparent, thinPen, rectangleGeometry);
      }
      else
      {
        Pen pen = new Pen((Brush) Brushes.Transparent, 15.0);
        pen.Freeze();
        if (!PlatformTypes.IsInstance(this.PlatformBrush, PlatformTypes.GradientBrush, (ITypeResolver) this.Element.ProjectContext))
          return;
        Point startPoint;
        Point endPoint;
        this.GetBrushOffsetEndpoints(out startPoint, out endPoint, 13.0, 11.0, matrix);
        context.DrawLine(pen, startPoint, endPoint);
        context.DrawLine(this.ThinPen, startPoint, endPoint);
      }
    }
  }
}
