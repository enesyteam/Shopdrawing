// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataPropertyConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataPropertyConfiguration
  {
    private SampleBasicType activeType;
    private Dictionary<SampleBasicType, ISampleTypeConfiguration> valueGenerators;

    public SampleBasicType SampleType
    {
      get
      {
        return this.activeType;
      }
      set
      {
        this.activeType = value;
      }
    }

    public string Format
    {
      get
      {
        return this.valueGenerators[this.activeType].Format;
      }
    }

    public string FormatParameters
    {
      get
      {
        return this.valueGenerators[this.activeType].FormatParameters;
      }
    }

    public SampleBooleanConfiguration BooleanValueGenerator
    {
      get
      {
        return (SampleBooleanConfiguration) this.valueGenerators[SampleBasicType.Boolean];
      }
    }

    public SampleImageConfiguration ImageValueGenerator
    {
      get
      {
        return (SampleImageConfiguration) this.valueGenerators[SampleBasicType.Image];
      }
    }

    public SampleNumberConfiguration NumberValueGenerator
    {
      get
      {
        return (SampleNumberConfiguration) this.valueGenerators[SampleBasicType.Number];
      }
    }

    public SampleStringConfiguration StringValueGenerator
    {
      get
      {
        return (SampleStringConfiguration) this.valueGenerators[SampleBasicType.String];
      }
    }

    public SampleDataPropertyConfiguration(SampleProperty sampleProperty)
    {
      this.activeType = (SampleBasicType) sampleProperty.PropertySampleType;
      this.valueGenerators = new Dictionary<SampleBasicType, ISampleTypeConfiguration>();
      foreach (SampleBasicType sampleBasicType in SampleBasicType.SampleBasicTypes)
      {
        string format = sampleBasicType == sampleProperty.PropertySampleType ? sampleProperty.Format : (string) null;
        string formatParameters = sampleBasicType == sampleProperty.PropertySampleType ? sampleProperty.FormatParameters : (string) null;
        ISampleTypeConfiguration configuration = SampleDataPropertyConfiguration.CreateConfiguration(sampleProperty.DeclaringDataSet, sampleBasicType, format, formatParameters);
        this.valueGenerators.Add(sampleBasicType, configuration);
      }
    }

    public static ISampleTypeConfiguration CreateConfiguration(SampleDataSet sampleData, SampleBasicType type, string format, string formatParameters)
    {
      if (type == SampleBasicType.Boolean)
        return (ISampleTypeConfiguration) new SampleBooleanConfiguration();
      if (type == SampleBasicType.Image)
        return (ISampleTypeConfiguration) new SampleImageConfiguration(sampleData, formatParameters);
      if (type == SampleBasicType.Number)
        return (ISampleTypeConfiguration) new SampleNumberConfiguration(formatParameters);
      if (type == SampleBasicType.String)
        return (ISampleTypeConfiguration) new SampleStringConfiguration(format, formatParameters);
      return (ISampleTypeConfiguration) null;
    }

    public object GetSelectedValue(ConfigurationPlaceholder placeholder)
    {
      if (placeholder == ConfigurationPlaceholder.Type)
      {
        if (this.activeType == SampleBasicType.Boolean)
          return (object) SampleDataConfigurationOption.TypeBoolean;
        if (this.activeType == SampleBasicType.Image)
          return (object) SampleDataConfigurationOption.TypeImage;
        if (this.activeType == SampleBasicType.Number)
          return (object) SampleDataConfigurationOption.TypeNumber;
        if (this.activeType == SampleBasicType.String)
          return (object) SampleDataConfigurationOption.TypeString;
      }
      else
      {
        if (placeholder == ConfigurationPlaceholder.NumberLength)
          return this.NumberValueGenerator.GetConfigurationValue(placeholder);
        if (placeholder == ConfigurationPlaceholder.ImageFolderBrowser)
          return this.ImageValueGenerator.GetConfigurationValue(placeholder);
        if (placeholder == ConfigurationPlaceholder.RandomLatinWordCount || placeholder == ConfigurationPlaceholder.RandomLatinWordLength || placeholder == ConfigurationPlaceholder.StringFormat)
          return this.StringValueGenerator.GetConfigurationValue(placeholder);
      }
      return (object) null;
    }

    public object GetConfigurationValue(ConfigurationPlaceholder placeholder)
    {
      if (placeholder == ConfigurationPlaceholder.Type)
        return (object) this.activeType;
      if (placeholder == ConfigurationPlaceholder.NumberLength)
        return this.NumberValueGenerator.GetConfigurationValue(placeholder);
      if (placeholder == ConfigurationPlaceholder.ImageFolderBrowser)
        return this.ImageValueGenerator.GetConfigurationValue(placeholder);
      if (placeholder == ConfigurationPlaceholder.RandomLatinWordCount || placeholder == ConfigurationPlaceholder.RandomLatinWordLength || placeholder == ConfigurationPlaceholder.StringFormat)
        return this.StringValueGenerator.GetConfigurationValue(placeholder);
      return (object) null;
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
      if (placeholder == ConfigurationPlaceholder.Type)
        this.activeType = (SampleBasicType) ((SampleDataConfigurationOption) value).Value;
      else if (placeholder == ConfigurationPlaceholder.NumberLength)
        this.NumberValueGenerator.SetConfigurationValue(placeholder, value);
      else if (placeholder == ConfigurationPlaceholder.ImageFolderBrowser)
      {
        this.ImageValueGenerator.SetConfigurationValue(placeholder, value);
      }
      else
      {
        if (placeholder != ConfigurationPlaceholder.RandomLatinWordCount && placeholder != ConfigurationPlaceholder.RandomLatinWordLength && placeholder != ConfigurationPlaceholder.StringFormat)
          return;
        this.StringValueGenerator.SetConfigurationValue(placeholder, value);
      }
    }
  }
}
