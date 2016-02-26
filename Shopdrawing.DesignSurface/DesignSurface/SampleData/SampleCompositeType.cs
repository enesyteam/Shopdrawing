// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleCompositeType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleCompositeType : SampleNonBasicType
  {
    private List<SampleProperty> sampleProperties = new List<SampleProperty>();

    public override bool IsCollection
    {
      get
      {
        return false;
      }
    }

    public IList<SampleProperty> SampleProperties
    {
      get
      {
        return (IList<SampleProperty>) this.sampleProperties;
      }
    }

    public override IType ItemType
    {
      get
      {
        return (IType) null;
      }
    }

    public override IType BaseType
    {
      get
      {
        return this.TypeResolver.ResolveType(PlatformTypes.Object);
      }
    }

    public override ReadOnlyCollection<IProperty> Properties
    {
      get
      {
        List<IProperty> list = new List<IProperty>(this.sampleProperties.Count);
        for (int index = 0; index < this.sampleProperties.Count; ++index)
          list.Add((IProperty) this.sampleProperties[index]);
        return new ReadOnlyCollection<IProperty>((IList<IProperty>) list);
      }
    }

    public SampleCompositeType(string name, SampleDataSet dataSet)
      : base(name, dataSet)
    {
    }

    public SampleProperty GetSampleProperty(string name)
    {
      foreach (SampleProperty sampleProperty in this.sampleProperties)
      {
        if (name == sampleProperty.Name)
          return sampleProperty;
      }
      return (SampleProperty) null;
    }

    public string GetUniquePropertyName(string name)
    {
      return this.GetUniquePropertyName(name, (string) null);
    }

    public string GetUniquePropertyName(string name, string nameToIgnore)
    {
      return this.DeclaringDataSet.GetUniqueName(name, Enumerable.Select<IProperty, string>((IEnumerable<IProperty>) this.Properties, (Func<IProperty, string>) (prop => prop.Name)), nameToIgnore);
    }

    public SampleProperty AddProperty(SampleNonBasicType sampleType)
    {
      return this.AddProperty(sampleType.Name, (SampleType) sampleType);
    }

    public SampleProperty AddProperty(string name, SampleType sampleType)
    {
      if (sampleType == this.DeclaringDataSet.RootType)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.RecursiveSampleDataRootTypeNotSupported, new object[1]
        {
          (object) this.DeclaringDataSet.RootType.Name
        }));
      if (this.GetSampleProperty(name) != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ExceptionStringTable.NameIsAlreadyInUse, new object[1]
        {
          (object) name
        }));
      SampleProperty sampleProperty = new SampleProperty(name, sampleType, this);
      this.sampleProperties.Add(sampleProperty);
      this.DeclaringDataSet.OnPropertyAdded(sampleProperty);
      return sampleProperty;
    }

    public void DeleteProperty(SampleProperty sampleProperty)
    {
      for (int index = 0; index < this.sampleProperties.Count; ++index)
      {
        if (sampleProperty == this.sampleProperties[index])
        {
          this.sampleProperties.RemoveAt(index);
          this.DeclaringDataSet.OnPropertyDeleted(sampleProperty);
          break;
        }
      }
    }

    public override IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
    {
      if ((memberTypes & MemberType.LocalProperty) != MemberType.LocalProperty)
        return (IMemberId) null;
      if ((access & MemberAccessTypes.Public) != MemberAccessTypes.Public)
        return (IMemberId) null;
      return (IMemberId) this.GetSampleProperty(memberName);
    }

    public override IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
    {
      if ((access & MemberAccessTypes.Public) != MemberAccessTypes.Public)
        return (IEnumerable<IProperty>) SampleNonBasicType.emptyProperties;
      return (IEnumerable<IProperty>) this.Properties;
    }
  }
}
