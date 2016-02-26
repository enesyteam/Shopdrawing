// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.PointToPixelConverter
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.Code.UserInterface
{
  public sealed class PointToPixelConverter : IValueConverter
  {
    private static double pixelToPointRatio = 4.0 / 3.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) (double.Parse(value.ToString(), (IFormatProvider) CultureInfo.InvariantCulture) * PointToPixelConverter.pixelToPointRatio);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
