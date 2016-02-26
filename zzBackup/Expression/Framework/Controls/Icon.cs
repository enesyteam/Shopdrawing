// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.Icon
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework.Controls
{
  public class Icon : Image
  {
    public static readonly DependencyProperty SourceBrushProperty = DependencyProperty.Register("SourceBrush", typeof (DrawingBrush), typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RedChromaProperty = DependencyProperty.RegisterAttached("RedChroma", typeof (SolidColorBrush), typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty GreenChromaProperty = DependencyProperty.RegisterAttached("GreenChroma", typeof (SolidColorBrush), typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty BlueChromaProperty = DependencyProperty.RegisterAttached("BlueChroma", typeof (SolidColorBrush), typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty SelectedImageProperty = DependencyProperty.RegisterAttached("SelectedImage", typeof (ImageSource), typeof (Icon), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DeselectedImageProperty = DependencyProperty.RegisterAttached("DeselectedImage", typeof (ImageSource), typeof (Icon), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectedDrawingBrushProperty = DependencyProperty.RegisterAttached("SelectedDrawingBrush", typeof (DrawingBrush), typeof (Icon), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DeselectedDrawingBrushProperty = DependencyProperty.RegisterAttached("DeselectedDrawingBrush", typeof (DrawingBrush), typeof (Icon), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ShowSelectedIconOnMouseOverProperty = DependencyProperty.RegisterAttached("ShowSelectedIconOnMouseOver", typeof (bool), typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.Inherits));

    public DrawingBrush SourceBrush
    {
      get
      {
        return (DrawingBrush) this.GetValue(Icon.SourceBrushProperty);
      }
      set
      {
        this.SetValue(Icon.SourceBrushProperty, (object) value);
      }
    }

    public SolidColorBrush RedChroma
    {
      get
      {
        return (SolidColorBrush) this.GetValue(Icon.RedChromaProperty);
      }
      set
      {
        this.SetValue(Icon.RedChromaProperty, (object) value);
      }
    }

    public SolidColorBrush GreenChroma
    {
      get
      {
        return (SolidColorBrush) this.GetValue(Icon.GreenChromaProperty);
      }
      set
      {
        this.SetValue(Icon.GreenChromaProperty, (object) value);
      }
    }

    public SolidColorBrush BlueChroma
    {
      get
      {
        return (SolidColorBrush) this.GetValue(Icon.BlueChromaProperty);
      }
      set
      {
        this.SetValue(Icon.BlueChromaProperty, (object) value);
      }
    }

    private ImageSource RenderSource
    {
      get
      {
        if (this.Source == null)
          return (ImageSource) null;
        if (this.RedChroma == null && this.GreenChroma == null && this.BlueChroma == null)
          return this.Source;
        return ColorSwapper.SwapColors(this.Source, new ColorCallback(this.ConvertColor));
      }
    }

    private DrawingBrush RenderSourceBrush
    {
      get
      {
        if (this.SourceBrush == null)
          return (DrawingBrush) null;
        if (this.RedChroma == null && this.GreenChroma == null && this.BlueChroma == null)
          return this.SourceBrush;
        return (DrawingBrush) ColorSwapper.SwapColors((Brush) this.SourceBrush, new ColorCallback(this.ConvertColor));
      }
    }

    static Icon()
    {
      Image.StretchProperty.OverrideMetadata(typeof (Icon), (PropertyMetadata) new FrameworkPropertyMetadata((object) Stretch.None));
    }

    public static SolidColorBrush GetRedChroma(DependencyObject obj)
    {
      return (SolidColorBrush) obj.GetValue(Icon.RedChromaProperty);
    }

    public static void SetRedChroma(DependencyObject obj, SolidColorBrush value)
    {
      obj.SetValue(Icon.RedChromaProperty, (object) value);
    }

    public static SolidColorBrush GetGreenChroma(DependencyObject obj)
    {
      return (SolidColorBrush) obj.GetValue(Icon.GreenChromaProperty);
    }

    public static void SetGreenChroma(DependencyObject obj, SolidColorBrush value)
    {
      obj.SetValue(Icon.GreenChromaProperty, (object) value);
    }

    public static SolidColorBrush GetBlueChroma(DependencyObject obj)
    {
      return (SolidColorBrush) obj.GetValue(Icon.BlueChromaProperty);
    }

    public static void SetBlueChroma(DependencyObject obj, SolidColorBrush value)
    {
      obj.SetValue(Icon.BlueChromaProperty, (object) value);
    }

    public static ImageSource GetSelectedImage(DependencyObject obj)
    {
      return (ImageSource) obj.GetValue(Icon.SelectedImageProperty);
    }

    public static void SetSelectedImage(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(Icon.SelectedImageProperty, (object) value);
    }

    public static DrawingBrush GetSelectedDrawingBrush(DependencyObject obj)
    {
      return (DrawingBrush) obj.GetValue(Icon.SelectedDrawingBrushProperty);
    }

    public static void SetSelectedDrawingBrush(DependencyObject obj, DrawingBrush value)
    {
      obj.SetValue(Icon.SelectedDrawingBrushProperty, (object) value);
    }

    public static ImageSource GetDeselectedImage(DependencyObject obj)
    {
      return (ImageSource) obj.GetValue(Icon.DeselectedImageProperty);
    }

    public static void SetDeselectedImage(DependencyObject obj, ImageSource value)
    {
      obj.SetValue(Icon.DeselectedImageProperty, (object) value);
    }

    public static DrawingBrush GetDeselectedDrawingBrush(DependencyObject obj)
    {
      return (DrawingBrush) obj.GetValue(Icon.DeselectedDrawingBrushProperty);
    }

    public static void SetDeselectedDrawingBrush(DependencyObject obj, DrawingBrush value)
    {
      obj.SetValue(Icon.DeselectedDrawingBrushProperty, (object) value);
    }

    public static bool GetShowSelectedIconOnMouseOver(DependencyObject obj)
    {
      return (bool) obj.GetValue(Icon.ShowSelectedIconOnMouseOverProperty);
    }

    public static void SetShowSelectedIconOnMouseOver(DependencyObject obj, bool value)
    {
        obj.SetValue(Icon.ShowSelectedIconOnMouseOverProperty, value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.Source == null)
        return finalSize;
      return base.ArrangeOverride(finalSize);
    }

    public static Point GetPixelSnappingOffset(Visual visual)
    {
      PresentationSource presentationSource = PresentationSource.FromVisual(visual);
      if (presentationSource != null)
        return Icon.GetPixelSnappingOffset(visual, presentationSource.RootVisual);
      return new Point();
    }

    private static Point GetPixelSnappingOffset(Visual visual, Visual rootVisual)
    {
      Point point = new Point();
      if (rootVisual != null)
      {
        Transform transform = visual.TransformToAncestor(rootVisual) as Transform;
        if (transform != null && transform.Value.HasInverse)
          point = visual.PointFromScreen(visual.PointToScreen(point));
      }
      return point;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      BitmapSource bitmapSource = this.Source as BitmapSource;
      Rect rectangle = new Rect(0.0, 0.0, this.RenderSize.Width, this.RenderSize.Height);
      if (this.SourceBrush == null || bitmapSource != null && Icon.IsClose(bitmapSource.Width, this.RenderSize.Width) && Icon.IsClose(bitmapSource.Height, this.RenderSize.Height))
      {
        ImageSource renderSource = this.RenderSource;
        if (renderSource == null)
          return;
        drawingContext.DrawImage(renderSource, rectangle);
      }
      else
        drawingContext.DrawRectangle((Brush) this.RenderSourceBrush, (Pen) null, rectangle);
    }

    private Color ConvertColor(Color color)
    {
      if ((int) color.R == (int) color.G && (int) color.R == (int) color.B)
        return color;
      if ((int) color.G == (int) color.B && this.RedChroma != null)
        return this.ScaleColor(this.RedChroma.Color, color.R, color.G, color.A);
      if ((int) color.R == (int) color.B && this.GreenChroma != null)
        return this.ScaleColor(this.GreenChroma.Color, color.G, color.R, color.A);
      if ((int) color.R == (int) color.G && this.BlueChroma != null)
        return this.ScaleColor(this.BlueChroma.Color, color.B, color.R, color.A);
      return color;
    }

    private Color ScaleColor(Color color, byte primary, byte white, byte alpha)
    {
      return Color.FromArgb((byte) ((int) alpha * (int) color.A / (int) byte.MaxValue), (byte) ((double) color.R / (double) byte.MaxValue * (double) ((int) primary - (int) white) + (double) white), (byte) ((double) color.G / (double) byte.MaxValue * (double) ((int) primary - (int) white) + (double) white), (byte) ((double) color.B / (double) byte.MaxValue * (double) ((int) primary - (int) white) + (double) white));
    }

    private static bool IsClose(double num1, double num2)
    {
      return num1 > num2 * 0.9;
    }
  }
}
