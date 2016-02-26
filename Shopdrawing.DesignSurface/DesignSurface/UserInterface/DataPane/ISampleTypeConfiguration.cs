// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.ISampleTypeConfiguration
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public interface ISampleTypeConfiguration
  {
    object Value { get; }

    string Format { get; }

    string FormatParameters { get; }

    SampleBasicType SampleType { get; }

    void SetConfigurationValue(ConfigurationPlaceholder placeholder, object value);

    object GetConfigurationValue(ConfigurationPlaceholder placeholder);
  }
}
