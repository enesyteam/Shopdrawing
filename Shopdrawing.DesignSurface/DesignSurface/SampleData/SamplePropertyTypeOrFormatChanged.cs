// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SamplePropertyTypeOrFormatChanged
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SamplePropertyTypeOrFormatChanged : SamplePropertyChange
  {
    public SampleType OldType { get; private set; }

    public string OldFormat { get; private set; }

    public string OldFormatParameters { get; private set; }

    public SampleType NewType { get; private set; }

    public string NewFormat { get; private set; }

    public string NewFormatParameters { get; private set; }

    public SamplePropertyTypeOrFormatChanged(SampleProperty sampleProperty, SampleType oldType, string oldFormat, string oldFormatParameters)
      : base(sampleProperty)
    {
      this.OldType = oldType;
      this.OldFormat = oldFormat;
      this.OldFormatParameters = oldFormatParameters;
      this.NewFormat = sampleProperty.Format;
      this.NewFormatParameters = sampleProperty.FormatParameters;
      this.NewType = sampleProperty.PropertySampleType;
    }

    public override SampleDataChangeMergeResult MergeWith(SampleDataChange newerChange, out SampleDataChange mergedChange)
    {
      SamplePropertyTypeOrFormatChanged typeOrFormatChanged = newerChange as SamplePropertyTypeOrFormatChanged;
      if (typeOrFormatChanged != null)
      {
        if (this.SampleProperty.DeclaringSampleType != typeOrFormatChanged.SampleProperty.DeclaringSampleType || this.SampleProperty.Name != typeOrFormatChanged.SampleProperty.Name)
        {
          mergedChange = (SampleDataChange) null;
          return SampleDataChangeMergeResult.CouldNotMerge;
        }
        SampleType newType = typeOrFormatChanged.NewType;
        if (this.OldType == typeOrFormatChanged.NewType && this.OldFormat == typeOrFormatChanged.NewFormat && this.OldFormatParameters == typeOrFormatChanged.NewFormatParameters)
        {
          mergedChange = (SampleDataChange) null;
          return SampleDataChangeMergeResult.MergedIntoNothing;
        }
        mergedChange = (SampleDataChange) new SamplePropertyTypeOrFormatChanged(typeOrFormatChanged.SampleProperty, this.OldType, this.OldFormat, this.OldFormatParameters)
        {
          NewType = typeOrFormatChanged.NewType,
          NewFormat = typeOrFormatChanged.NewFormat,
          NewFormatParameters = typeOrFormatChanged.NewFormatParameters
        };
        return SampleDataChangeMergeResult.MergedIntoOneUnit;
      }
      if (this.Entity != newerChange.Entity)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.CouldNotMerge;
      }
      SamplePropertyDeleted samplePropertyDeleted = newerChange as SamplePropertyDeleted;
      if (samplePropertyDeleted != null)
      {
        mergedChange = (SampleDataChange) samplePropertyDeleted;
        return SampleDataChangeMergeResult.MergedIntoOneUnit;
      }
      mergedChange = (SampleDataChange) null;
      return SampleDataChangeMergeResult.CouldNotMerge;
    }

    public override void Undo()
    {
      this.SampleProperty.ChangeTypeAndFormat(this.OldType, this.OldFormat, this.OldFormatParameters);
    }

    public override string ToString()
    {
      string str = "Type/Format changed " + this.SampleProperty.Name + ":";
      if (this.OldType != this.NewType)
        str = str + (object) " '" + (string) (object) this.OldType + "' -> '" + (string) (object) this.NewType + "'";
      if (this.OldFormat != this.NewFormat)
        str = str + " '" + this.OldFormat + "' -> '" + this.NewFormat + "'";
      if (this.OldFormatParameters != this.NewFormatParameters)
        str = str + " '" + this.OldFormatParameters + "' -> '" + this.NewFormatParameters + "'";
      return str;
    }
  }
}
