// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.SampleBasicType
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class SampleBasicType : SampleType
  {
    public static readonly string XsdImageTypeName = "Basic-Image";
    public static readonly SampleBasicType String = SampleBasicType.RegisterSampleType("String", PlatformTypes.String);
    public static readonly SampleBasicType Boolean = SampleBasicType.RegisterSampleType("Boolean", PlatformTypes.Boolean);
    public static readonly SampleBasicType Date = SampleBasicType.RegisterSampleType("Date", PlatformTypes.DateTime);
    public static readonly SampleBasicType Number = SampleBasicType.RegisterSampleType("Number", PlatformTypes.Double);
    public static readonly SampleBasicType Image = SampleBasicType.RegisterSampleType("Image", PlatformTypes.ImageSource);
    private ITypeId typeId;
    private string name;
    private static List<SampleBasicType> sampleBasicTypesInternal;

    public static ReadOnlyCollection<SampleBasicType> SampleBasicTypes
    {
      get
      {
        return new ReadOnlyCollection<SampleBasicType>((IList<SampleBasicType>) SampleBasicType.sampleBasicTypesInternal);
      }
    }

    public override bool IsBasicType
    {
      get
      {
        return true;
      }
    }

    public override bool IsCollection
    {
      get
      {
        return false;
      }
    }

    public override ITypeId TypeId
    {
      get
      {
        return this.typeId;
      }
    }

    public override string Name
    {
      get
      {
        return this.name;
      }
    }

    private SampleBasicType(string name, ITypeId typeId)
    {
      this.name = name;
      this.typeId = typeId;
    }

    private static SampleBasicType RegisterSampleType(string typeName, ITypeId platformType)
    {
      SampleBasicType sampleBasicType = new SampleBasicType(typeName, platformType);
      if (platformType != PlatformTypes.DateTime)
      {
        if (SampleBasicType.sampleBasicTypesInternal == null)
          SampleBasicType.sampleBasicTypesInternal = new List<SampleBasicType>();
        SampleBasicType.sampleBasicTypesInternal.Add(sampleBasicType);
      }
      return sampleBasicType;
    }

    public static SampleBasicType FromTypeId(ITypeId typeId)
    {
      if (typeId == SampleBasicType.String.TypeId)
        return SampleBasicType.String;
      if (typeId == SampleBasicType.Boolean.TypeId)
        return SampleBasicType.Boolean;
      if (typeId == SampleBasicType.Date.TypeId)
        return SampleBasicType.Date;
      if (typeId == SampleBasicType.Number.TypeId)
        return SampleBasicType.Number;
      if (typeId == SampleBasicType.Image.TypeId)
        return SampleBasicType.Image;
      return (SampleBasicType) null;
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
