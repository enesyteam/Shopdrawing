// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.CenterPointAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class CenterPointAdorner : EventHandlingAdorner, IClickable
  {
    private bool shouldDrawSingleElementAdorner = true;
    private const double radius = 2.5;

    public CenterPointAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (!(eventArgs.PropertyName == "SingleElementCenterPointAdornerIsVisible"))
        return;
      this.shouldDrawSingleElementAdorner = (bool) eventArgs.Value;
      this.InvalidateRender();
    }

    public Point GetClickablePoint(Matrix matrix)
    {
      Point point = new Point();
      return (!this.ElementSet.AdornsMultipleElements ? (this.Element as BaseFrameworkElement).RenderTransformOriginInElementCoordinates : this.ElementSet.RenderTransformOriginInElementCoordinates) * matrix;
    }

    public override void Draw(DrawingContext ctx, Matrix matrix)
    {
      Size ofTransformedRect = Adorner.GetSizeOfTransformedRect(this.ElementBounds, matrix);
      if (!this.ElementSet.AdornsMultipleElements && !this.shouldDrawSingleElementAdorner || (ofTransformedRect.Width < 10.0 || ofTransformedRect.Height < 10.0))
        return;
      Point clickablePoint = this.GetClickablePoint(matrix);
      Brush brush = this.IsActive ? this.ActiveBrush : this.InactiveBrush;
      ctx.DrawEllipse(brush, this.ThinPen, clickablePoint, 2.5, 2.5);
    }
  }
}
