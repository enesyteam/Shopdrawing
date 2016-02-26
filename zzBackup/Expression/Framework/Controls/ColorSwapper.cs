// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ColorSwapper
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework.Controls
{
  public static class ColorSwapper
  {
    public static ImageSource SwapColors(ImageSource imageSource, ColorCallback colorCallback)
    {
      if (colorCallback == null)
        throw new ArgumentNullException("colorCallback");
      ImageSource imageSource1 = imageSource;
      if (imageSource != null)
      {
        DrawingImage drawingImage;
        if ((drawingImage = imageSource as DrawingImage) != null)
        {
          ColorSwapper.SwapColorsWithoutCloning(((DrawingImage) (imageSource1 = (ImageSource) drawingImage.Clone())).Drawing, colorCallback);
          imageSource1.Freeze();
        }
        else
        {
          BitmapSource bitmapSource;
          if ((bitmapSource = imageSource as BitmapSource) != null)
            imageSource1 = (ImageSource) ColorSwapper.SwapColors(bitmapSource, colorCallback);
          else
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnexpectedImageSourceType, new object[1]
            {
              (object) imageSource.GetType().Name
            }));
        }
      }
      return imageSource1;
    }

    public static BitmapSource SwapColors(BitmapSource bitmapSource, ColorCallback colorCallback)
    {
      if (colorCallback == null)
        throw new ArgumentNullException("colorCallback");
      BitmapSource bitmapSource1 = bitmapSource;
      if (bitmapSource != null)
      {
        PixelFormat bgra32 = PixelFormats.Bgra32;
        BitmapPalette bitmapPalette = (BitmapPalette) null;
        double alphaThreshold = 0.0;
        FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap(bitmapSource, bgra32, bitmapPalette, alphaThreshold);
        int pixelWidth = formatConvertedBitmap.PixelWidth;
        int pixelHeight = formatConvertedBitmap.PixelHeight;
        int stride = 4 * pixelWidth;
        byte[] numArray = new byte[stride * pixelHeight];
        formatConvertedBitmap.CopyPixels((Array) numArray, stride, 0);
        int index = 0;
        while (index < numArray.Length)
        {
          Color color1 = Color.FromArgb(numArray[index + 3], numArray[index + 2], numArray[index + 1], numArray[index]);
          Color color2 = colorCallback(color1);
          if (color2 != color1)
          {
            numArray[index] = color2.B;
            numArray[index + 1] = color2.G;
            numArray[index + 2] = color2.R;
            numArray[index + 3] = color2.A;
          }
          index += 4;
        }
        bitmapSource1 = BitmapSource.Create(pixelWidth, pixelHeight, formatConvertedBitmap.DpiX, formatConvertedBitmap.DpiY, bgra32, bitmapPalette, (Array) numArray, stride);
        bitmapSource1.Freeze();
      }
      return bitmapSource1;
    }

    public static Drawing SwapColors(Drawing drawing, ColorCallback colorCallback)
    {
      if (colorCallback == null)
        throw new ArgumentNullException("colorCallback");
      Drawing drawing1 = drawing;
      if (drawing != null)
      {
        drawing1 = drawing.Clone();
        ColorSwapper.SwapColorsWithoutCloning(drawing1, colorCallback);
        drawing1.Freeze();
      }
      return drawing1;
    }

    public static Brush SwapColors(Brush brush, ColorCallback colorCallback)
    {
      if (colorCallback == null)
        throw new ArgumentNullException("colorCallback");
      Brush brush1 = brush;
      if (brush != null)
      {
        brush1 = brush.Clone();
        ColorSwapper.SwapColorsWithoutCloning(brush1, colorCallback);
        brush1.Freeze();
      }
      return brush1;
    }

    private static ImageSource SwapColorsWithoutCloningIfPossible(ImageSource imageSource, ColorCallback colorCallback)
    {
      ImageSource imageSource1 = imageSource;
      if (imageSource != null)
      {
        DrawingImage drawingImage;
        if ((drawingImage = imageSource as DrawingImage) != null)
        {
          ColorSwapper.SwapColorsWithoutCloning(drawingImage.Drawing, colorCallback);
        }
        else
        {
          BitmapSource bitmapSource;
          if ((bitmapSource = imageSource as BitmapSource) != null)
            imageSource1 = (ImageSource) ColorSwapper.SwapColors(bitmapSource, colorCallback);
          else
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnexpectedImageSourceType, new object[1]
            {
              (object) imageSource.GetType().Name
            }));
        }
      }
      return imageSource1;
    }

    private static void SwapColorsWithoutCloning(Drawing drawing, ColorCallback colorCallback)
    {
      if (drawing == null)
        return;
      DrawingGroup drawingGroup;
      if ((drawingGroup = drawing as DrawingGroup) != null)
      {
        for (int index = 0; index < drawingGroup.Children.Count; ++index)
          ColorSwapper.SwapColorsWithoutCloning(drawingGroup.Children[index], colorCallback);
      }
      else
      {
        GeometryDrawing geometryDrawing;
        if ((geometryDrawing = drawing as GeometryDrawing) != null)
        {
          ColorSwapper.SwapColorsWithoutCloning(geometryDrawing.Brush, colorCallback);
          if (geometryDrawing.Pen == null)
            return;
          ColorSwapper.SwapColorsWithoutCloning(geometryDrawing.Pen.Brush, colorCallback);
        }
        else
        {
          GlyphRunDrawing glyphRunDrawing;
          if ((glyphRunDrawing = drawing as GlyphRunDrawing) != null)
          {
            ColorSwapper.SwapColorsWithoutCloning(glyphRunDrawing.ForegroundBrush, colorCallback);
          }
          else
          {
            ImageDrawing imageDrawing;
            if ((imageDrawing = drawing as ImageDrawing) != null)
              imageDrawing.ImageSource = ColorSwapper.SwapColorsWithoutCloningIfPossible(imageDrawing.ImageSource, colorCallback);
            else if (!(drawing is VideoDrawing))
              throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnexpectedDrawingType, new object[1]
              {
                (object) drawing.GetType().Name
              }));
          }
        }
      }
    }

    private static void SwapColorsWithoutCloning(Brush brush, ColorCallback colorCallback)
    {
      if (brush == null)
        return;
      SolidColorBrush solidColorBrush;
      if ((solidColorBrush = brush as SolidColorBrush) != null)
      {
        solidColorBrush.Color = colorCallback(solidColorBrush.Color);
      }
      else
      {
        GradientBrush gradientBrush;
        if ((gradientBrush = brush as GradientBrush) != null)
        {
          foreach (GradientStop gradientStop in gradientBrush.GradientStops)
            gradientStop.Color = colorCallback(gradientStop.Color);
        }
        else
        {
          DrawingBrush drawingBrush;
          if ((drawingBrush = brush as DrawingBrush) != null)
          {
            ColorSwapper.SwapColorsWithoutCloning(drawingBrush.Drawing, colorCallback);
          }
          else
          {
            ImageBrush imageBrush;
            if ((imageBrush = brush as ImageBrush) != null)
              imageBrush.ImageSource = ColorSwapper.SwapColorsWithoutCloningIfPossible(imageBrush.ImageSource, colorCallback);
            else if (!(brush is VisualBrush))
              throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionStringTable.UnexpectedBrushType, new object[1]
              {
                (object) brush.GetType().Name
              }));
          }
        }
      }
    }
  }
}
