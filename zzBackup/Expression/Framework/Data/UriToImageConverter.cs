// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Data.UriToImageConverter
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Expression.Framework.Data
{
  [ValueConversion(typeof (string), typeof (ImageSource))]
  public class UriToImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return (object) null;
      Uri uri;
      try
      {
        uri = new Uri(value.ToString());
      }
      catch (UriFormatException ex)
      {
        return (object) null;
      }
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
      bitmapImage.UriSource = uri;
      try
      {
        bitmapImage.EndInit();
      }
      catch (NotSupportedException ex)
      {
        return (object) null;
      }
      catch (FileFormatException ex)
      {
        return (object) null;
      }
      catch (IOException ex)
      {
        return (object) null;
      }
      catch (SecurityException ex)
      {
        return (object) null;
      }
      catch (UnauthorizedAccessException ex)
      {
        return (object) null;
      }
      return (object) bitmapImage;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
