// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.ImageCapturer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Extensions.DependencyObject;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using Microsoft.Expression.Framework.Extensions.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.View
{
  public abstract class ImageCapturer
  {
    protected static readonly Size MaxSize = new Size(2000.0, 1500.0);

    public double Scale { get; protected set; }

    protected Size DefaultSize { get; set; }

    protected ImageCapturer(Size defaultSize)
    {
      this.DefaultSize = defaultSize;
      this.Scale = 1.0;
    }

    public abstract BitmapSource Capture();

    public BitmapSource CaptureWithInk(StrokeCollection inkStrokes)
    {
      return ImageCapturer.OverlayInk(this.Capture(), this.Scale, inkStrokes);
    }

    public static BitmapSource OverlayInk(BitmapSource bitmapSource, double scale, StrokeCollection inkStrokes)
    {
      if (bitmapSource == null)
        return (BitmapSource) null;
      if (inkStrokes == null || inkStrokes.Count == 0)
        return bitmapSource;
      Rect rect1 = new Rect(0.0, 0.0, (double) bitmapSource.PixelWidth, (double) bitmapSource.PixelHeight);
      Vector offsetVector = (Vector) RectExtensions.GetCenter(rect1);
      Rect rect2 = RectExtensions.Union(Enumerable.Select<Stroke, Rect>((IEnumerable<Stroke>) inkStrokes, (Func<Stroke, Rect>) (stroke => stroke.GetBounds())));
      rect2.Scale(scale, scale);
      rect2.Offset(offsetVector);
      Rect rect3 = Rect.Union(rect1, rect2);
      Canvas canvas1 = new Canvas();
      canvas1.Width = rect3.Width;
      canvas1.Height = rect3.Height;
      canvas1.Children.Add((UIElement) new Image()
      {
        Source = (ImageSource) bitmapSource
      });
      UIElementCollection children = canvas1.Children;
      InkPresenter inkPresenter1 = new InkPresenter();
      inkPresenter1.Strokes = inkStrokes;
      inkPresenter1.Margin = new Thickness(offsetVector.X, offsetVector.Y, -offsetVector.X, -offsetVector.Y);
      inkPresenter1.LayoutTransform = (Transform) new ScaleTransform(scale, scale);
      InkPresenter inkPresenter2 = inkPresenter1;
      children.Add((UIElement) inkPresenter2);
      Canvas canvas2 = canvas1;
      Point newOrigin = new Point(-rect3.Left, -rect3.Top);
      EnumerableExtensions.ForEach<UIElement>(Enumerable.OfType<UIElement>((IEnumerable) canvas2.Children), (Action<UIElement>) (child => DependencyObjectExtensions.SetCanvasPos((DependencyObject) child, newOrigin)));
      return new FrameworkElementCapturer((FrameworkElement) canvas2, ImageCapturer.MaxSize).Capture();
    }

    public static BitmapSource CaptureElement(FrameworkElement frameworkElement, Size maxSize)
    {
      return FrameworkElementCapturer.Capture(frameworkElement, maxSize);
    }
  }
}
