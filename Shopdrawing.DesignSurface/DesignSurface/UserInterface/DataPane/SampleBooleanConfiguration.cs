// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleBooleanConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleBooleanConfiguration : ISampleTypeConfiguration
  {
    private bool value;

    public SampleBasicType SampleType
    {
      get
      {
        return SampleBasicType.Boolean;
      }
    }

    public object Value
    {
      get
      {
        this.value = !this.value;
        return (object) (bool) (this.value ? true : false);
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
        return (string) null;
      }
    }

    public void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value)
    {
    }

    public object GetConfigurationValue(ConfigurationPlaceholder placeholder)
    {
      return (object) null;
    }
  }
}
