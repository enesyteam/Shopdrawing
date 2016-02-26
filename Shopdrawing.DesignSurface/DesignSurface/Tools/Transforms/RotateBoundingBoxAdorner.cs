// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.Transforms.RotateBoundingBoxAdorner
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.UserInterface;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Tools.Transforms
{
  internal class RotateBoundingBoxAdorner : EventHandlingAdorner
  {
    private static double Thickness = 1.0;
    private double initialZoom;
    private double rotationAmount;
    private Vector initialOffset;
    private Point initialRenderTransformOrigin;
    private Rect initialBounds;
    private CanonicalDecomposition initialSharedTransform;
    private bool shouldDraw;
    private Point startArtboardOrigin;

    public RotateBoundingBoxAdorner(AdornerSet adornerSet)
      : base(adornerSet)
    {
    }

    protected override void HandleAdornerLayerEvent(AdornerPropertyChangedEventArgs eventArgs)
    {
      if (eventArgs.PropertyName == "RotateBoundingBoxAdornerIsVisible")
      {
        this.shouldDraw = (bool) eventArgs.Value;
        this.InvalidateRender();
        if (!this.shouldDraw)
          return;
        this.StartDrag();
      }
      else
      {
        if (!(eventArgs.PropertyName == "RotateBoundingBoxAdornerRotationAngle"))
          return;
        this.rotationAmount = (double) eventArgs.Value;
      }
    }

    private void StartDrag()
    {
      Matrix matrixToAdornerLayer = this.ElementSet.GetTransformMatrixToAdornerLayer();
      this.initialZoom = this.DesignerContext.ActiveView.Artboard.Zoom;
      this.initialBounds = this.ElementSet.ElementBounds;
      this.initialSharedTransform = this.ElementSet.CalculateSharedTransform();
      this.initialRenderTransformOrigin = this.ElementSet.RenderTransformOrigin;
      this.initialOffset = new Vector(matrixToAdornerLayer.OffsetX, matrixToAdornerLayer.OffsetY);
      this.startArtboardOrigin = this.DesignerContext.ActiveView.Artboard.ArtboardBounds.TopLeft;
    }

    public override void Draw(DrawingContext drawingContext, Matrix matrix)
    {
      if (!this.shouldDraw)
        return;
      Rect rect = this.initialBounds;
      System.Windows.Media.Geometry rectangleGeometry;
      if (this.ElementSet.HasHomogeneousRotation)
      {
        rectangleGeometry = Adorner.GetTransformedRectangleGeometry(rect, matrix, 1.0);
      }
      else
      {
        matrix.OffsetX = 0.0;
        matrix.OffsetY = 0.0;
        Matrix matrix1 = new Matrix();
        Point point = new Point((this.initialBounds.Left + this.initialBounds.Right) * this.initialRenderTransformOrigin.X, (this.initialBounds.Top + this.initialBounds.Bottom) * this.initialRenderTransformOrigin.Y);
        double angle = this.rotationAmount;
        if (this.initialSharedTransform.ScaleY < 0.0)
          angle = -angle;
        if (this.initialSharedTransform.ScaleX < 0.0)
          angle = -angle;
        matrix1.RotateAt(angle, point.X, point.Y);
        double zoom = this.DesignerContext.ActiveView.Artboard.Zoom;
        double num = zoom / this.initialZoom;
        Matrix matrix2 = new Matrix();
        matrix2.Translate(num * this.initialOffset.X, num * this.initialOffset.Y);
        Point topLeft = this.DesignerContext.ActiveView.Artboard.ArtboardBounds.TopLeft;
        Matrix matrix3 = new Matrix(1.0, 0.0, 0.0, 1.0, (this.startArtboardOrigin.X - topLeft.X) * zoom, (this.startArtboardOrigin.Y - topLeft.Y) * zoom);
        rectangleGeometry = Adorner.GetTransformedRectangleGeometry(rect, matrix1 * matrix * matrix2 * matrix3, RotateBoundingBoxAdorner.Thickness);
      }
      drawingContext.DrawGeometry((Brush) null, new Pen(this.ActiveBrush, RotateBoundingBoxAdorner.Thickness), rectangleGeometry);
    }
  }
}
