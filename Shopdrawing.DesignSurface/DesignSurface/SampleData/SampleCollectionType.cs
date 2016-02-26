// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleCollectionType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleCollectionType : SampleNonBasicType
  {
    private SampleType itemSampleType;
    private IType baseType;

    public override bool IsCollection
    {
      get
      {
        return true;
      }
    }

    public SampleType ItemSampleType
    {
      get
      {
        return this.itemSampleType;
      }
      internal set
      {
        this.itemSampleType = value;
      }
    }

    public override IType ItemType
    {
      get
      {
        return this.DeclaringDataSet.ResolveSampleType(this.itemSampleType);
      }
    }

    public override IType BaseType
    {
      get
      {
        return this.baseType;
      }
    }

    public override ReadOnlyCollection<IProperty> Properties
    {
      get
      {
        List<IProperty> list = new List<IProperty>();
        foreach (IProperty property in this.baseType.GetProperties(MemberAccessTypes.All))
          list.Add(property);
        return new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
      }
    }

    public SampleCollectionType(string name, SampleType itemType, SampleDataSet declaringDataSet)
      : base(name, declaringDataSet)
    {
      this.itemSampleType = itemType;
      IType type = this.TypeResolver.ResolveType(PlatformTypes.Object);
      this.baseType = this.TypeResolver.GetType(this.TypeResolver.ResolveType(PlatformTypes.ObservableCollection).RuntimeType.MakeGenericType(type.RuntimeType));
      this.DeclaringDataSet.ThrowIfRecursiveRootType(this.ItemSampleType);
    }

    public void ChangeItemType(SampleType itemType)
    {
      if (this.itemSampleType == itemType)
        return;
      this.DeclaringDataSet.ThrowIfRecursiveRootType(itemType);
      for (SampleCollectionType sampleCollectionType = itemType as SampleCollectionType; sampleCollectionType != null; sampleCollectionType = sampleCollectionType.ItemSampleType as SampleCollectionType)
      {
        if (sampleCollectionType == this)
          throw new InvalidOperationException(ExceptionStringTable.SampleDataCollectionItemTypeCannotBeSelf);
      }
      SampleType oldItemType = this.itemSampleType;
      this.itemSampleType = itemType;
      this.DeclaringDataSet.OnCollectionItemTypeChanged(this, oldItemType);
    }

    public override IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
    {
      return this.baseType.GetMember(memberTypes, memberName, access);
    }

    public override IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
    {
      return this.baseType.GetProperties(access);
    }
  }
}
