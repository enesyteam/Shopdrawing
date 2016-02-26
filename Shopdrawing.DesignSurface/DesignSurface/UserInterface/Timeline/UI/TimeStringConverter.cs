// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI.TimeStringConverter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline.UI
{
  public class TimeStringConverter : IValueConverter
  {
    private bool snapToTimelineResolution = true;
    private bool useShortForm;

    public bool UseShortForm
    {
      get
      {
        return this.useShortForm;
      }
      set
      {
        this.useShortForm = value;
      }
    }

    public bool SnapToTimelineResolution
    {
      get
      {
        return this.snapToTimelineResolution;
      }
      set
      {
        this.snapToTimelineResolution = value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string s1 = (string) value;
      double num1 = double.NaN;
      culture = CultureInfo.CurrentCulture;
      string timeSeparator = culture.DateTimeFormat.TimeSeparator;
      int length = s1.IndexOf(timeSeparator, StringComparison.CurrentCulture);
      if (length < 0)
      {
        double result;
        if (double.TryParse(s1, NumberStyles.AllowDecimalPoint, (IFormatProvider) culture, out result) && !double.IsNaN(result) && !double.IsInfinity(result))
          num1 = TimelineView.SnapSeconds(result, this.snapToTimelineResolution && TimelineView.IsTimelineSnapping);
      }
      else if (s1.IndexOf(timeSeparator, length + timeSeparator.Length, StringComparison.CurrentCulture) == -1)
      {
        string s2 = s1.Substring(0, length);
        if (length + timeSeparator.Length < s1.Length)
        {
          string s3 = s1.Substring(length + timeSeparator.Length);
          int result1;
          double result2;
          if (s2 != null && s3 != null && (int.TryParse(s2, NumberStyles.None, (IFormatProvider) culture, out result1) && double.TryParse(s3, NumberStyles.AllowDecimalPoint, (IFormatProvider) culture, out result2)))
          {
            double num2 = result2 + 60.0 * (double) result1;
            if (!double.IsNaN(num2) && !double.IsInfinity(num2))
              num1 = TimelineView.SnapSeconds(num2, this.snapToTimelineResolution);
          }
        }
      }
      return (object) num1;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      culture = CultureInfo.CurrentCulture;
      double num1 = (double) value;
      int num2 = (int) num1 / 60;
      double num3 = num1 - (double) (60 * num2);
      string str;
      if (this.useShortForm)
      {
        if (num2 > 0)
        {
          str = (num2.ToString("0", (IFormatProvider) culture) + culture.DateTimeFormat.TimeSeparator + num3.ToString("00.000", (IFormatProvider) culture)).TrimEnd('0');
          if (str.EndsWith(culture.NumberFormat.NumberDecimalSeparator, StringComparison.CurrentCulture))
            str = str.Remove(str.Length - culture.NumberFormat.NumberDecimalSeparator.Length, culture.NumberFormat.NumberDecimalSeparator.Length);
        }
        else
          str = num3.ToString((IFormatProvider) culture);
      }
      else
        str = num2.ToString("0", (IFormatProvider) culture) + culture.DateTimeFormat.TimeSeparator + num3.ToString("00.000", (IFormatProvider) culture);
      return (object) str;
    }
  }
}
