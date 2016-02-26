// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.FrameworkElementCapturer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Extensions.Math;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.View
{
  internal class FrameworkElementCapturer : ImageCapturer
  {
    private FrameworkElement element;

    public FrameworkElementCapturer(WPFSceneView sceneView)
      : this(sceneView, ImageCapturer.MaxSize)
    {
    }

    public FrameworkElementCapturer(WPFSceneView sceneView, Size targetSize)
      : base(targetSize)
    {
      Artboard artboard = sceneView.Artboard;
      FrameworkElement frameworkElement = (FrameworkElement) sceneView.DesignSurfaceRoot;
      SceneNode viewRoot = sceneView.ViewModel.ViewRoot;
      if (frameworkElement == null || viewRoot == null)
        return;
      this.element = frameworkElement;
      double? nullable1 = viewRoot.GetComputedValue(BaseFrameworkElement.WidthProperty) as double?;
      double? nullable2 = viewRoot.GetComputedValue(BaseFrameworkElement.HeightProperty) as double?;
      if (nullable1.HasValue && nullable2.HasValue && (!double.IsNaN(nullable1.Value) && !double.IsNaN(nullable2.Value)))
        return;
      Rect finalRect = new Rect(0.0, 0.0, 0.0, 0.0);
      artboard.Measure(finalRect.Size);
      artboard.Arrange(finalRect);
      artboard.UpdateLayout();
    }

    public FrameworkElementCapturer(FrameworkElement element, Size targetSize)
      : base(targetSize)
    {
      this.element = element;
    }

    internal static BitmapSource Capture(FrameworkElement element, Size maxSize)
    {
      double scale;
      Rect scaledRectangle = FrameworkElementCapturer.GetScaledRectangle(element, maxSize, out scale);
      element.LayoutTransform = (Transform) new ScaleTransform(scale, scale);
      element.Measure(scaledRectangle.Size);
      element.Arrange(scaledRectangle);
      return VisualCapturer.Capture((Visual) element, new Size(scaledRectangle.Width, scaledRectangle.Height));
    }

    private static Rect GetScaledRectangle(FrameworkElement element, Size targetSize, out double scale)
    {
      Rect rect = new Rect(0.0, 0.0, 0.0, 0.0);
      element.Measure(rect.Size);
      element.Arrange(rect);
      element.UpdateLayout();
      rect = new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight);
      scale = RectExtensions.GetProportionalScale(rect, targetSize);
      rect.Scale(scale, scale);
      return rect;
    }

    public override BitmapSource Capture()
    {
      if (this.element == null)
        return (BitmapSource) null;
      double scale;
      FrameworkElementCapturer.GetScaledRectangle(this.element, this.DefaultSize, out scale);
      this.Scale = scale;
      return FrameworkElementCapturer.Capture(this.element, this.DefaultSize);
    }
  }
}
