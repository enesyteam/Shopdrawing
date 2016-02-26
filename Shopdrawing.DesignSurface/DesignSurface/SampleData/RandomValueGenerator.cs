// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.RandomValueGenerator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class RandomValueGenerator
  {
    private Dictionary<TypeFormatInfo, ISampleTypeConfiguration> valueBuilders = new Dictionary<TypeFormatInfo, ISampleTypeConfiguration>();
    private SampleDataSet dataSet;

    public RandomValueGenerator(SampleDataSet dataSet)
    {
      this.dataSet = dataSet;
    }

    public string GetRandomValue(SampleBasicType sampleType, string format, string formatParameters)
    {
      object obj = this.GetValueBuilder(sampleType, format, formatParameters).Value;
      if (obj == null)
        return (string) null;
      return obj as string ?? MetadataStore.GetTypeConverter(obj.GetType()).ConvertToInvariantString((ITypeDescriptorContext) null, obj);
    }

    private ISampleTypeConfiguration GetValueBuilder(SampleBasicType sampleType, string format, string formatParameters)
    {
      TypeFormatInfo key = new TypeFormatInfo(sampleType, format, formatParameters);
      ISampleTypeConfiguration configuration;
      if (!this.valueBuilders.TryGetValue(key, out configuration))
      {
        configuration = SampleDataPropertyConfiguration.CreateConfiguration(this.dataSet, sampleType, format, formatParameters);
        this.valueBuilders[key] = configuration;
      }
      return configuration;
    }
  }
}
