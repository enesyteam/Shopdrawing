// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleNumberConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleNumberConfiguration : ISampleTypeConfiguration
  {
    private Random random = new Random(DateTime.Now.Millisecond);
    private int numberLength;

    public SampleBasicType SampleType
    {
      get
      {
        return SampleBasicType.Number;
      }
    }

    public object Value
    {
      get
      {
        int minValue = (int) Math.Pow(10.0, (double) (this.numberLength - 1));
        int maxValue = minValue * 10;
        return (object) this.random.Next(minValue, maxValue);
      }
    }

    public string Format
    {
      get
      {
        return (string) null;
      }
    }

    public string FormatParameters
    {
      get
      {
        return this.numberLength.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public SampleNumberConfiguration(string formatParameters)
    {
      if (int.TryParse(SampleDataFormatHelper.NormalizeFormatParameters(this.SampleType, formatParameters, false), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out this.numberLength))
        return;
      this.numberLength = Convert.ToInt32(ConfigurationPlaceholder.NumberLength.DefaultValue, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
      double result = double.NaN;
      if (value is double)
        result = (double) value;
      else
        double.TryParse(value as string, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
      if (double.IsNaN(result) || double.IsInfinity(result) || (result <= 0.0 || result > 10.0))
        return;
      this.numberLength = (int) result;
    }

    public object GetConfigurationValue(ConfigurationPlaceholder placeholder)
    {
      if (placeholder == ConfigurationPlaceholder.NumberLength)
        return (object) this.numberLength;
      return (object) null;
    }
  }
}
