// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.IconWithOverlay
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Controls;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class IconWithOverlay : Icon
  {
    public static readonly DependencyProperty OverlayImageSourceProperty = DependencyProperty.Register("OverlayImageSource", typeof (ImageSource), typeof (IconWithOverlay), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ShowOverlayProperty = DependencyProperty.Register("ShowOverlay", typeof (bool), typeof (IconWithOverlay), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty OverlayRectProperty = DependencyProperty.Register("OverlayRect", typeof (Rect), typeof (IconWithOverlay), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Rect(0.0, 0.0, 0.0, 0.0), FrameworkPropertyMetadataOptions.AffectsRender));

    public ImageSource OverlayImageSource
    {
      get
      {
        return (ImageSource) this.GetValue(IconWithOverlay.OverlayImageSourceProperty);
      }
      set
      {
        this.SetValue(IconWithOverlay.OverlayImageSourceProperty, (object) value);
      }
    }

    public bool ShowOverlay
    {
      get
      {
        return (bool) this.GetValue(IconWithOverlay.ShowOverlayProperty);
      }
      set
      {
        this.SetValue(IconWithOverlay.ShowOverlayProperty, (object) (bool) (value ? true : false));
      }
    }

    public Rect OverlayRect
    {
      get
      {
        return (Rect) this.GetValue(IconWithOverlay.OverlayRectProperty);
      }
      set
      {
        this.SetValue(IconWithOverlay.OverlayRectProperty, (object) value);
      }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      ImageSource overlayImageSource = this.OverlayImageSource;
      if (overlayImageSource == null || !this.ShowOverlay)
        return;
      drawingContext.DrawImage(overlayImageSource, this.OverlayRect);
    }
  }
}
