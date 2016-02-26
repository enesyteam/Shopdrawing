// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleDataChange
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public abstract class SampleDataChange
  {
    public object Entity { get; private set; }

    public abstract SampleDataSet DeclaringDataSet { get; }

    protected SampleDataChange(object entity)
    {
      this.Entity = entity;
    }

    public virtual SampleDataChangeMergeResult MergeWith(SampleDataChange newerChange, out SampleDataChange mergedChange)
    {
      mergedChange = (SampleDataChange) null;
      return SampleDataChangeMergeResult.CouldNotMerge;
    }

    public abstract void Undo();
  }
}
