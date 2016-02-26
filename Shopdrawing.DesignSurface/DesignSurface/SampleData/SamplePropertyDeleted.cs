// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SamplePropertyDeleted
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SamplePropertyDeleted : SamplePropertyChange
  {
    public SamplePropertyDeleted(SampleProperty sampleProperty)
      : base(sampleProperty)
    {
    }

    public override SampleDataChangeMergeResult MergeWith(SampleDataChange newerChange, out SampleDataChange mergedChange)
    {
      mergedChange = (SampleDataChange) null;
      return SampleDataChangeMergeResult.CouldNotMerge;
    }

    public override void Undo()
    {
      SampleProperty sampleProperty = this.SampleProperty;
      this.SampleProperty.DeclaringSampleType.AddProperty(sampleProperty.Name, sampleProperty.PropertySampleType);
    }

    public override string ToString()
    {
      return "Deleted " + this.SampleProperty.ToString();
    }
  }
}
