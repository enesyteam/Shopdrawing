// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleTypeRenamed
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleTypeRenamed : SampleTypeChange
  {
    public string OldName { get; private set; }

    public string NewName { get; private set; }

    public SampleTypeRenamed(SampleNonBasicType sampleType, string oldName)
      : base(sampleType)
    {
      this.OldName = oldName;
      this.NewName = sampleType.Name;
    }

    public override SampleDataChangeMergeResult MergeWith(SampleDataChange newerChange, out SampleDataChange mergedChange)
    {
      if (this.Entity != newerChange.Entity)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.CouldNotMerge;
      }
      if (newerChange is SampleTypeDeleted)
      {
        mergedChange = (SampleDataChange) new SampleTypeDeleted(this.SampleType);
        return SampleDataChangeMergeResult.MergedIntoOneUnit;
      }
      SampleTypeRenamed sampleTypeRenamed = newerChange as SampleTypeRenamed;
      if (sampleTypeRenamed == null)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.CouldNotMerge;
      }
      if (this.OldName == sampleTypeRenamed.NewName)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.MergedIntoNothing;
      }
      string newName = sampleTypeRenamed.NewName;
      mergedChange = (SampleDataChange) new SampleTypeRenamed(sampleTypeRenamed.SampleType, this.OldName)
      {
        NewName = newName
      };
      return SampleDataChangeMergeResult.MergedIntoOneUnit;
    }

    public override void Undo()
    {
      this.SampleType.Rename(this.OldName);
    }

    public override string ToString()
    {
      return "Renamed " + this.SampleType.ToString() + ": " + this.OldName + " -> " + this.NewName;
    }
  }
}
