using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace ListBoxSelectionColorChange
{
    [ValueConversion(typeof(byte[]), typeof(BitmapImage))]
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {
            

            byte[] bufferedImaged = null;

            if (value is byte[] && value != null)
                bufferedImaged = value as byte[];  
            else // Set Default Image
                bufferedImaged = ImageHelper.BufferFromUri(string.Format("{0}/{1}/{2}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "/Images/", "defaultFace.jpg"));


            MemoryStream stream = new MemoryStream(bufferedImaged);
            BitmapImage image = new BitmapImage();
            image.DecodePixelHeight = 80;
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;

        }

        public object ConvertBack(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }



    }
}
