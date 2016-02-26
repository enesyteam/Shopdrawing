// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.MoveAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class MoveAdorner : BoundingBoxAdorner, IClickable
  {
    private static Pen transparentPen = new Pen((Brush) Brushes.Transparent, 5.0);
    public const double MoveAdornerWidth = 5.0;

    private Rect TargetBounds
    {
      get
      {
        ISubElementAdornerSet elementAdornerSet = this.AdornerSet as ISubElementAdornerSet;
        if (elementAdornerSet != null)
          return elementAdornerSet.TargetRect;
        return this.ElementBounds;
      }
    }

    static MoveAdorner()
    {
      MoveAdorner.transparentPen.Freeze();
    }

    public MoveAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Rect targetBounds = this.TargetBounds;
      return this.TransformPoint(targetBounds.TopLeft + new Vector(targetBounds.Width * 0.25, 0.0));
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (!this.ShouldDraw)
        return;
      Rect targetBounds = this.TargetBounds;
      Matrix? additionalTransform = new Matrix?();
      ISubElementAdornerSet elementAdornerSet = this.AdornerSet as ISubElementAdornerSet;
      if (elementAdornerSet != null)
        additionalTransform = new Matrix?(elementAdornerSet.TargetMatrix);
      System.Windows.Media.Geometry rectangleGeometry = Adorner.GetTransformedRectangleGeometry(this.DesignerContext.ActiveView, this.Element, additionalTransform, targetBounds, MoveAdorner.transparentPen.Thickness, true);
      drawingContext.DrawGeometry((Brush) null, MoveAdorner.transparentPen, rectangleGeometry);
      base.Draw(drawingContext, matrix);
    }
  }
}
