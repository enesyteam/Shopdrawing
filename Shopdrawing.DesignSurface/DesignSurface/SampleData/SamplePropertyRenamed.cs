// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SamplePropertyRenamed
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SamplePropertyRenamed : SamplePropertyChange
  {
    public string OldName { get; private set; }

    public string NewName { get; private set; }

    public SamplePropertyRenamed(SampleProperty sampleProperty, string oldName)
      : base(sampleProperty)
    {
      this.OldName = oldName;
      this.NewName = sampleProperty.Name;
    }

    public override SampleDataChangeMergeResult MergeWith(SampleDataChange newerChange, out SampleDataChange mergedChange)
    {
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
      SamplePropertyRenamed samplePropertyRenamed = newerChange as SamplePropertyRenamed;
      if (samplePropertyRenamed != null)
      {
        string newName = samplePropertyRenamed.NewName;
        if (this.OldName == newName)
        {
          mergedChange = (SampleDataChange) null;
          return SampleDataChangeMergeResult.MergedIntoNothing;
        }
        mergedChange = (SampleDataChange) new SamplePropertyRenamed(samplePropertyRenamed.SampleProperty, this.OldName)
        {
          NewName = newName
        };
        return SampleDataChangeMergeResult.MergedIntoOneUnit;
      }
      mergedChange = (SampleDataChange) null;
      return SampleDataChangeMergeResult.CouldNotMerge;
    }

    public override void Undo()
    {
      this.SampleProperty.Rename(this.OldName);
    }

    public override string ToString()
    {
      return "Renamed " + (object) this.SampleProperty + ": " + this.OldName + " -> " + this.NewName;
    }
  }
}
