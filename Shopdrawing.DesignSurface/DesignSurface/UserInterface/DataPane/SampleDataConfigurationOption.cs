// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataConfigurationOption
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataConfigurationOption
  {
    private static SampleDataConfigurationOption typeString;
    private static SampleDataConfigurationOption typeImage;
    private static SampleDataConfigurationOption typeNumber;
    private static SampleDataConfigurationOption typeBoolean;
    private static SampleDataConfigurationOption stringFormatRandomLatin;
    private static IList<SampleDataConfigurationOption> stringFormatCustom;

    internal static SampleDataConfigurationOption TypeString
    {
      get
      {
        if (SampleDataConfigurationOption.typeString == null)
          SampleDataConfigurationOption.typeString = new SampleDataConfigurationOption(StringTable.SampleDataConfigurationStringType, 1 != 0, new ConfigurationPlaceholder[1]
          {
            ConfigurationPlaceholder.StringFormat
          });
        return SampleDataConfigurationOption.typeString;
      }
    }

    internal static SampleDataConfigurationOption TypeImage
    {
      get
      {
        if (SampleDataConfigurationOption.typeImage == null)
          SampleDataConfigurationOption.typeImage = new SampleDataConfigurationOption(StringTable.SampleDataConfigurationImageType, 0 != 0, new ConfigurationPlaceholder[1]
          {
            ConfigurationPlaceholder.ImageFolderBrowser
          });
        return SampleDataConfigurationOption.typeImage;
      }
    }

    internal static SampleDataConfigurationOption TypeNumber
    {
      get
      {
        if (SampleDataConfigurationOption.typeNumber == null)
          SampleDataConfigurationOption.typeNumber = new SampleDataConfigurationOption(StringTable.SampleDataConfigurationNumberType, 0 != 0, new ConfigurationPlaceholder[1]
          {
            ConfigurationPlaceholder.NumberLength
          });
        return SampleDataConfigurationOption.typeNumber;
      }
    }

    internal static SampleDataConfigurationOption TypeBoolean
    {
      get
      {
        if (SampleDataConfigurationOption.typeBoolean == null)
          SampleDataConfigurationOption.typeBoolean = new SampleDataConfigurationOption(StringTable.SampleDataConfigurationBooleanType, false, new ConfigurationPlaceholder[0]);
        return SampleDataConfigurationOption.typeBoolean;
      }
    }

    internal static SampleDataConfigurationOption StringFormatRandomLatin
    {
      get
      {
        if (SampleDataConfigurationOption.stringFormatRandomLatin == null)
          SampleDataConfigurationOption.stringFormatRandomLatin = new SampleDataConfigurationOption(StringTable.SampleDataConfigurationLoremIpsumFormat, 1 != 0, new ConfigurationPlaceholder[2]
          {
            ConfigurationPlaceholder.RandomLatinWordCount,
            ConfigurationPlaceholder.RandomLatinWordLength
          });
        return SampleDataConfigurationOption.stringFormatRandomLatin;
      }
    }

    internal static IList<SampleDataConfigurationOption> StringFormatCustom
    {
      get
      {
        if (SampleDataConfigurationOption.stringFormatCustom == null)
        {
          SampleDataConfigurationOption.stringFormatCustom = (IList<SampleDataConfigurationOption>) new List<SampleDataConfigurationOption>();
          foreach (string str in SampleFormatValues.Instance.Columns)
            SampleDataConfigurationOption.stringFormatCustom.Add(new SampleDataConfigurationOption(str, false, new ConfigurationPlaceholder[0]));
        }
        return SampleDataConfigurationOption.stringFormatCustom;
      }
    }

    public string StringValue { get; private set; }

    public bool IsDefault { get; private set; }

    public ICollection<ConfigurationPlaceholder> ChildControls { get; private set; }

    public object Value
    {
      get
      {
        if (this == SampleDataConfigurationOption.TypeBoolean)
          return (object) SampleBasicType.Boolean;
        if (this == SampleDataConfigurationOption.TypeImage)
          return (object) SampleBasicType.Image;
        if (this == SampleDataConfigurationOption.TypeNumber)
          return (object) SampleBasicType.Number;
        if (this == SampleDataConfigurationOption.TypeString)
          return (object) SampleBasicType.String;
        return (object) this.StringValue;
      }
    }

    public SampleDataConfigurationOption(string value, bool isDefault, params ConfigurationPlaceholder[] resultantControls)
    {
      this.IsDefault = isDefault;
      this.StringValue = value;
      this.ChildControls = (ICollection<ConfigurationPlaceholder>) new List<ConfigurationPlaceholder>((IEnumerable<ConfigurationPlaceholder>) resultantControls);
    }

    public override string ToString()
    {
      return this.StringValue;
    }
  }
}
