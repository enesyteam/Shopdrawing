// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.SampleDataProperty
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.SampleData;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class SampleDataProperty : NotifyingObject
  {
    public SampleDataEditorModel Model { get; private set; }

    public SampleProperty SampleProperty { get; private set; }

    public SampleDataPropertyConfiguration SampleTypeConfiguration { get; private set; }

    public SampleDataSet SampleDataSet
    {
      get
      {
        return this.SampleProperty.DeclaringDataSet;
      }
    }

    public SampleBasicType SampleType
    {
      get
      {
        return (SampleBasicType) this.SampleProperty.PropertySampleType;
      }
    }

    public string Name
    {
      get
      {
        return this.SampleProperty.Name;
      }
    }

    public SampleDataProperty(SampleProperty sampleProperty, SampleDataEditorModel model)
    {
      this.SampleProperty = sampleProperty;
      this.Model = model;
      this.SampleTypeConfiguration = new SampleDataPropertyConfiguration(sampleProperty);
    }

    public void UpdateSampleProperty()
    {
      SampleDataPropertyConfiguration typeConfiguration = this.SampleTypeConfiguration;
      SampleProperty sampleProperty = this.SampleProperty;
      if (typeConfiguration.SampleType == sampleProperty.PropertySampleType && typeConfiguration.Format == sampleProperty.Format && typeConfiguration.FormatParameters == sampleProperty.FormatParameters)
        return;
      this.Model.SetModified();
      this.SampleProperty.ChangeTypeAndFormat((Microsoft.Expression.DesignSurface.SampleData.SampleType) typeConfiguration.SampleType, typeConfiguration.Format, typeConfiguration.FormatParameters);
      if (sampleProperty.PropertySampleType == SampleBasicType.Image)
        this.SampleDataSet.EnsureSampleImages();
      this.Model.UpdateEditingCollection();
    }

    public override string ToString()
    {
      return this.SampleProperty.ToString();
    }
  }
}
