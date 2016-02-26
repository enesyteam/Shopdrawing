// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimeToNonNegativeValueConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimeToNonNegativeValueConverter : IValueConverter
  {
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) TimelineView.SecondsFromValue((double) value);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) Math.Max(0.0, TimelineView.ValueFromSeconds((double) value));
    }
  }
}
