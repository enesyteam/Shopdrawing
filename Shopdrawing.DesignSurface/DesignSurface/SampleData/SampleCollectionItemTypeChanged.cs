// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleCollectionItemTypeChanged
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleCollectionItemTypeChanged : SampleTypeChange
  {
    public SampleType OldItemType { get; private set; }

    public SampleType NewItemType { get; private set; }

    public SampleCollectionType SampleCollectionType
    {
      get
      {
        return (SampleCollectionType) this.Entity;
      }
    }

    public SampleCollectionItemTypeChanged(SampleCollectionType sampleType, SampleType oldType)
      : base((SampleNonBasicType) sampleType)
    {
      this.OldItemType = oldType;
      this.NewItemType = sampleType.ItemSampleType;
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
      SampleCollectionItemTypeChanged collectionItemTypeChanged = newerChange as SampleCollectionItemTypeChanged;
      if (collectionItemTypeChanged == null)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.CouldNotMerge;
      }
      if (this.OldItemType == collectionItemTypeChanged.NewItemType)
      {
        mergedChange = (SampleDataChange) null;
        return SampleDataChangeMergeResult.MergedIntoNothing;
      }
      SampleType newItemType = collectionItemTypeChanged.NewItemType;
      mergedChange = (SampleDataChange) new SampleCollectionItemTypeChanged(this.SampleCollectionType, this.OldItemType)
      {
        NewItemType = newItemType
      };
      return SampleDataChangeMergeResult.MergedIntoOneUnit;
    }

    public override void Undo()
    {
      this.SampleCollectionType.ChangeItemType(this.OldItemType);
    }

    public override string ToString()
    {
      return "Item type changed " + (object) this.SampleType.ToString() + ": " + (string) (object) this.OldItemType + " -> " + (string) (object) this.NewItemType;
    }
  }
}
