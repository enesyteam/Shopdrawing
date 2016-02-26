// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.ImageButton
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shopdrawing.App
{
  public class ImageButton : Button
  {
    public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register("HoverImage", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ImageButtonSizeProperty = DependencyProperty.Register("ImageButtonSize", typeof (string), typeof (ImageButton), new PropertyMetadata((PropertyChangedCallback) null));

    public ImageSource DefaultImage
    {
      get
      {
        return (ImageSource) this.GetValue(ImageButton.DefaultImageProperty);
      }
      set
      {
        this.SetValue(ImageButton.DefaultImageProperty, (object) value);
      }
    }

    public ImageSource HoverImage
    {
      get
      {
        return (ImageSource) this.GetValue(ImageButton.HoverImageProperty);
      }
      set
      {
        this.SetValue(ImageButton.HoverImageProperty, (object) value);
      }
    }

    [Localizability(LocalizationCategory.NeverLocalize)]
    public string ImageButtonSize
    {
      get
      {
        return (string) this.GetValue(ImageButton.ImageButtonSizeProperty);
      }
      set
      {
        this.SetValue(ImageButton.ImageButtonSizeProperty, (object) value);
      }
    }
  }
}
