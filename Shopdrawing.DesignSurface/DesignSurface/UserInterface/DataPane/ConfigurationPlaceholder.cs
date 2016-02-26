// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ConfigurationPlaceholder
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class ConfigurationPlaceholder
  {
    private static ConfigurationPlaceholder randomLatinWordLength;
    private static ConfigurationPlaceholder randomLatinWordCount;
    private static ConfigurationPlaceholder stringFormat;
    private static ConfigurationPlaceholder type;
    private static ConfigurationPlaceholder numberLength;
    private static ConfigurationPlaceholder imageFolderBrowser;

    internal static ConfigurationPlaceholder RandomLatinWordLength
    {
      get
      {
        if (ConfigurationPlaceholder.randomLatinWordLength == null)
        {
          ConfigurationPlaceholder.randomLatinWordLength = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationLoremIpsumMaxWordLength, ConfigurationControlType.NumberSlider, "NumberEditor_LoremIpsumWordLength");
          ConfigurationPlaceholder.randomLatinWordLength.SliderRange = new ConfigurationSliderRange(3, 12, 8);
        }
        return ConfigurationPlaceholder.randomLatinWordLength;
      }
    }

    internal static ConfigurationPlaceholder RandomLatinWordCount
    {
      get
      {
        if (ConfigurationPlaceholder.randomLatinWordCount == null)
        {
          ConfigurationPlaceholder.randomLatinWordCount = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationLoremIpsumMaxWordCount, ConfigurationControlType.NumberSlider, "NumberEditor_LoremIpsumWordCount");
          ConfigurationPlaceholder.randomLatinWordCount.SliderRange = new ConfigurationSliderRange(1, 100, 4);
        }
        return ConfigurationPlaceholder.randomLatinWordCount;
      }
    }

    internal static ConfigurationPlaceholder StringFormat
    {
      get
      {
        if (ConfigurationPlaceholder.stringFormat == null)
        {
          ConfigurationPlaceholder.stringFormat = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationStringFormat, ConfigurationControlType.ComboBox, "ComboBox_StringFormat");
          ConfigurationPlaceholder.stringFormat.ComboBoxOptions.Add(SampleDataConfigurationOption.StringFormatRandomLatin);
          foreach (SampleDataConfigurationOption configurationOption in (IEnumerable<SampleDataConfigurationOption>) SampleDataConfigurationOption.StringFormatCustom)
            ConfigurationPlaceholder.stringFormat.ComboBoxOptions.Add(configurationOption);
        }
        return ConfigurationPlaceholder.stringFormat;
      }
    }

    internal static ConfigurationPlaceholder Type
    {
      get
      {
        if (ConfigurationPlaceholder.type == null)
        {
          ConfigurationPlaceholder.type = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationColumnType, ConfigurationControlType.ComboBox, "ComboBox_TypeSelector");
          ConfigurationPlaceholder.type.ComboBoxOptions.Add(SampleDataConfigurationOption.TypeString);
          ConfigurationPlaceholder.type.ComboBoxOptions.Add(SampleDataConfigurationOption.TypeNumber);
          ConfigurationPlaceholder.type.ComboBoxOptions.Add(SampleDataConfigurationOption.TypeBoolean);
          ConfigurationPlaceholder.type.ComboBoxOptions.Add(SampleDataConfigurationOption.TypeImage);
        }
        return ConfigurationPlaceholder.type;
      }
    }

    internal static ConfigurationPlaceholder NumberLength
    {
      get
      {
        if (ConfigurationPlaceholder.numberLength == null)
        {
          ConfigurationPlaceholder.numberLength = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationNumberLength, ConfigurationControlType.NumberSlider, "NumberEditor_NumberLength");
          ConfigurationPlaceholder.numberLength.SliderRange = new ConfigurationSliderRange(1, 10, 2);
        }
        return ConfigurationPlaceholder.numberLength;
      }
    }

    internal static ConfigurationPlaceholder ImageFolderBrowser
    {
      get
      {
        if (ConfigurationPlaceholder.imageFolderBrowser == null)
          ConfigurationPlaceholder.imageFolderBrowser = new ConfigurationPlaceholder(StringTable.SampleDataConfigurationImageBrowser, ConfigurationControlType.DirectoryBrowser, "FileBrowser_ImageFolder");
        return ConfigurationPlaceholder.imageFolderBrowser;
      }
    }

    public string Label { get; private set; }

    public string AutomationId { get; private set; }

    internal ConfigurationControlType ControlType { get; private set; }

    public IList<SampleDataConfigurationOption> ComboBoxOptions { get; private set; }

    public ConfigurationSliderRange SliderRange { get; private set; }

    public object DefaultValue
    {
      get
      {
        switch (this.ControlType)
        {
          case ConfigurationControlType.ComboBox:
            foreach (SampleDataConfigurationOption configurationOption in (IEnumerable<SampleDataConfigurationOption>) this.ComboBoxOptions)
            {
              if (configurationOption.IsDefault)
                return (object) configurationOption;
            }
            return (object) null;
          case ConfigurationControlType.NumberSlider:
            return (object) this.SliderRange.Default;
          case ConfigurationControlType.DirectoryBrowser:
            return (object) SampleDataFormatHelper.GetDefaultFormatParameters(SampleBasicType.Image);
          default:
            return (object) null;
        }
      }
    }

    private ConfigurationPlaceholder(string label, ConfigurationControlType controlType, string automationId)
    {
      this.ComboBoxOptions = (IList<SampleDataConfigurationOption>) new List<SampleDataConfigurationOption>();
      this.Label = label;
      this.ControlType = controlType;
      this.AutomationId = automationId;
    }
  }
}
