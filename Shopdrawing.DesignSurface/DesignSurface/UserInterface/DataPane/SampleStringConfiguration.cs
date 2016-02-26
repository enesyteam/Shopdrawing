// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleStringConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleStringConfiguration : ISampleTypeConfiguration
  {
    private Random random = new Random(DateTime.Now.Millisecond);
    private string format;
    private string formatParameters;
    private bool useLatin;
    private int latinWordCount;
    private int latinWordLength;

    public SampleBasicType SampleType
    {
      get
      {
        return SampleBasicType.String;
      }
    }

    public object Value
    {
      get
      {
        SampleFormatValues instance = SampleFormatValues.Instance;
        if (!this.useLatin)
          return (object) instance.GetNextStringGivenFormat(this.format);
        StringBuilder stringBuilder = new StringBuilder();
        int num = this.random.Next(Math.Max(1, this.latinWordCount / 2), this.latinWordCount + 1);
        for (int index = 0; index < num; ++index)
        {
          string str = instance.GetNextLoremIpsumWord(this.latinWordLength);
          if (index == 0)
            str = str[0].ToString((IFormatProvider) CultureInfo.CurrentCulture).ToUpper(CultureInfo.CurrentCulture) + str.Substring(1);
          else
            stringBuilder.Append(" ");
          stringBuilder.Append(str);
        }
        return (object) stringBuilder.ToString();
      }
    }

    public string Format
    {
      get
      {
        if (!this.useLatin)
          return this.format;
        return SampleDataConfigurationOption.StringFormatRandomLatin.StringValue;
      }
    }

    public string FormatParameters
    {
      get
      {
        if (!this.useLatin)
          return (string) null;
        return this.formatParameters;
      }
    }

    public SampleStringConfiguration(string format, string formatParameters)
    {
      this.useLatin = true;
      this.Initialize(format, formatParameters);
      SampleFormatValues.Instance.ResetNextValues();
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
      if (placeholder == ConfigurationPlaceholder.StringFormat)
      {
        string str1 = value == null ? string.Empty : value.ToString();
        if (str1 == SampleDataConfigurationOption.StringFormatRandomLatin.StringValue)
        {
          this.format = str1;
          this.useLatin = true;
        }
        else
        {
          if (string.IsNullOrEmpty(str1))
            return;
          foreach (string str2 in SampleFormatValues.Instance.Columns)
          {
            if (str2.ToUpperInvariant() == str1.ToUpperInvariant())
            {
              this.format = str2;
              this.useLatin = false;
              break;
            }
          }
        }
      }
      else if (placeholder == ConfigurationPlaceholder.RandomLatinWordCount)
      {
        int result;
        if (!int.TryParse(value.ToString(), out result))
          return;
        this.latinWordCount = result;
        this.formatParameters = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
        {
          (object) this.latinWordCount,
          (object) this.latinWordLength
        });
      }
      else
      {
        int result;
        if (placeholder != ConfigurationPlaceholder.RandomLatinWordLength || !int.TryParse(value.ToString(), out result))
          return;
        this.latinWordLength = result;
        this.formatParameters = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
        {
          (object) this.latinWordCount,
          (object) this.latinWordLength
        });
      }
    }

    public object GetConfigurationValue(ConfigurationPlaceholder placeholder)
    {
      if (placeholder == ConfigurationPlaceholder.StringFormat)
      {
        if (this.useLatin)
          return (object) SampleDataConfigurationOption.StringFormatRandomLatin;
        foreach (SampleDataConfigurationOption configurationOption in (IEnumerable<SampleDataConfigurationOption>) SampleDataConfigurationOption.StringFormatCustom)
        {
          if (this.format == configurationOption.StringValue)
            return (object) configurationOption;
        }
      }
      else
      {
        if (placeholder == ConfigurationPlaceholder.RandomLatinWordCount)
          return (object) this.latinWordCount;
        if (placeholder == ConfigurationPlaceholder.RandomLatinWordLength)
          return (object) this.latinWordLength;
      }
      return (object) null;
    }

    private void Initialize(string format, string formatParameters)
    {
      string str1 = SampleDataFormatHelper.NormalizeFormat(this.SampleType, format, false);
      if (str1 != this.Format)
        this.SetConfigurationValue(ConfigurationPlaceholder.StringFormat, (object) str1);
      string str2 = SampleDataFormatHelper.NormalizeFormatParameters(this.SampleType, formatParameters, false);
      if (str2 == this.FormatParameters || string.IsNullOrEmpty(str2))
        return;
      string[] strArray = str2.Split(',');
      if (strArray.Length != 2)
        return;
      string s1 = strArray[0];
      string s2 = strArray[1];
      int result1;
      if (int.TryParse(s1, out result1))
        this.SetConfigurationValue(ConfigurationPlaceholder.RandomLatinWordCount, (object) result1);
      int result2;
      if (int.TryParse(s2, out result2))
        this.SetConfigurationValue(ConfigurationPlaceholder.RandomLatinWordLength, (object) result2);
      this.formatParameters = str2;
    }
  }
}
