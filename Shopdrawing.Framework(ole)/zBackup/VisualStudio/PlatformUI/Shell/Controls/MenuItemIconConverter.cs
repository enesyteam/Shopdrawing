// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.MenuItemIconConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public class MenuItemIconConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ImageSource imageSource = value as ImageSource;
      Image image1 = (Image) null;
      if (imageSource != null)
      {
        Image image2 = new Image();
        image2.Source = imageSource;
        image2.Width = 16.0;
        image2.Height = 16.0;
        image1 = image2;
        RenderOptions.SetEdgeMode((DependencyObject) image1, EdgeMode.Aliased);
        RenderOptions.SetBitmapScalingMode((DependencyObject) image1, BitmapScalingMode.NearestNeighbor);
      }
      return (object) image1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
