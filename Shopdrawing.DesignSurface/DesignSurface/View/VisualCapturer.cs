// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.VisualCapturer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.Framework.Extensions.Math;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.DesignSurface.View
{
  internal class VisualCapturer : ImageCapturer
  {
    private Visual visual;

    public VisualCapturer(SilverlightSceneView sceneView)
      : this(sceneView, ImageCapturer.MaxSize)
    {
    }

    public VisualCapturer(SilverlightSceneView sceneView, Size targetSize)
      : base(targetSize)
    {
      SilverlightArtboard silverlightArtboard = (SilverlightArtboard) sceneView.Artboard;
      Rect documentBounds = silverlightArtboard.DocumentBounds;
      ImageHost silverlightImageHost = silverlightArtboard.SilverlightImageHost;
      if (documentBounds.Width <= 0.0 || documentBounds.Height <= 0.0)
        return;
      using (new VisualCapturer.ImageHostActivator(silverlightImageHost))
      {
        silverlightImageHost.SetTransformMatrix(Matrix.Identity, new Vector(1.0, 1.0));
        silverlightImageHost.Measure(documentBounds.Size);
        silverlightImageHost.Arrange(documentBounds);
        silverlightImageHost.UpdateLayout();
        silverlightImageHost.Redraw(false);
        double proportionalScale = RectExtensions.GetProportionalScale(documentBounds, targetSize);
        documentBounds.Scale(proportionalScale, proportionalScale);
        Image image1 = new Image();
        image1.Source = silverlightImageHost.InternalSource;
        image1.LayoutTransform = (Transform) new ScaleTransform(proportionalScale, proportionalScale);
        Image image2 = image1;
        image2.Measure(documentBounds.Size);
        image2.Arrange(documentBounds);
        this.DefaultSize = new Size(documentBounds.Width, documentBounds.Height);
        this.visual = (Visual) image2;
      }
    }

    public override BitmapSource Capture()
    {
      return VisualCapturer.Capture(this.visual, this.DefaultSize);
    }

    internal static BitmapSource Capture(Visual visual, Size targetSize)
    {
      int pixelWidth = (int) Math.Ceiling(targetSize.Width);
      int pixelHeight = (int) Math.Ceiling(targetSize.Height);
      if (0 >= pixelWidth || pixelWidth > 4096 || (0 >= pixelHeight || pixelHeight > 4096))
        return (BitmapSource) null;
      RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, 96.0, 96.0, PixelFormats.Pbgra32);
      renderTargetBitmap.Render(visual);
      if (renderTargetBitmap.CanFreeze)
        renderTargetBitmap.Freeze();
      return (BitmapSource) renderTargetBitmap;
    }

    private class ImageHostActivator : IDisposable
    {
      private ImageHost imageHost;

      public ImageHostActivator(ImageHost imageHost)
      {
        this.imageHost = imageHost;
        this.imageHost.Activate();
      }

      public void Dispose()
      {
        this.imageHost.Deactivate();
        this.imageHost = (ImageHost) null;
        GC.SuppressFinalize((object) this);
      }
    }
  }
}
