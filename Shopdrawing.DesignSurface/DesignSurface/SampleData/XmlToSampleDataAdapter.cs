// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.XmlToSampleDataAdapter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public class XmlToSampleDataAdapter : IXmlToClrAdapter
  {
    private SampleDataSet dataSet;
    private SampleDataValueBuilder valueBuilder;
    private List<XmlToSampleDataAdapter.BasicTypeInfo> basicTypes;
    private IType stringType;
    private string expectedRootTypeName;

    public string ExpectedRootTypeName
    {
      get
      {
        return this.expectedRootTypeName;
      }
    }

    public string CollectionSuffix
    {
      get
      {
        return "Collection";
      }
    }

    public IType StringType
    {
      get
      {
        return this.stringType;
      }
    }

    public XmlToSampleDataAdapter(SampleDataSet dataSet, string xmlFileName, string expectedRootTypeName)
    {
      this.dataSet = dataSet;
      this.expectedRootTypeName = expectedRootTypeName;
      this.stringType = this.dataSet.ProjectContext.ResolveType(PlatformTypes.String);
      this.basicTypes = new List<XmlToSampleDataAdapter.BasicTypeInfo>()
      {
        this.CreateBasicTypeInfo(SampleBasicType.Number, (TypeConverter) null),
        this.CreateBasicTypeInfo(SampleBasicType.Boolean, (TypeConverter) null)
      };
      if (!this.dataSet.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf))
        this.basicTypes.RemoveAll((Predicate<XmlToSampleDataAdapter.BasicTypeInfo>) (typeInfo => typeInfo.SampleType == SampleBasicType.Date));
      Uri uri = new Uri(xmlFileName, UriKind.Absolute);
      if (uri.IsFile || uri.IsUnc)
      {
        string directoryName = Path.GetDirectoryName(xmlFileName);
        ICollection<string> imageFileExtensions = this.dataSet.Platform.Metadata.ImageFileExtensions;
        this.basicTypes.Add(this.CreateBasicTypeInfo(SampleBasicType.Image, (TypeConverter) new AssetFileTypeConverter(directoryName, (IEnumerable<string>) imageFileExtensions)));
      }
      this.basicTypes.Add(this.CreateBasicTypeInfo(SampleBasicType.String, (TypeConverter) null));
    }

    public bool ShouldProcessAttribute(string localName, string prefix, string xmlNamespace)
    {
      return string.IsNullOrEmpty(prefix) && !(localName == "xmlns");
    }

    public bool ShouldProcessElement(string localName, string prefix, string xmlNamespace)
    {
      return string.IsNullOrEmpty(prefix);
    }

    public string GetClrName(string localName, string prefix, string xmlNamespace)
    {
      string name = localName;
      if (!this.dataSet.IsSafeIdentifier(name))
        name += "1";
      return this.dataSet.GetSafeName(name);
    }

    public IType InferValueType(string value)
    {
      return this.InferValueTypeInternal(new XmlToSampleDataAdapter.ValueInfo(value));
    }

    private string CoerceValue(string value, IType valueType)
    {
      if (valueType == this.stringType || value == null)
        return value;
      XmlToSampleDataAdapter.ValueInfo valueInfo = new XmlToSampleDataAdapter.ValueInfo(value);
      this.InferValueTypeInternal(valueInfo);
      return valueInfo.CoercedValue;
    }

    private IType InferValueTypeInternal(XmlToSampleDataAdapter.ValueInfo valueInfo)
    {
      foreach (XmlToSampleDataAdapter.BasicTypeInfo typeInfo in this.basicTypes)
      {
        if (this.IsTypeOf(valueInfo, typeInfo))
          return typeInfo.Type;
      }
      return this.stringType;
    }

    private bool IsTypeOf(XmlToSampleDataAdapter.ValueInfo valueInfo, XmlToSampleDataAdapter.BasicTypeInfo typeInfo)
    {
      try
      {
        object obj = typeInfo.Converter.ConvertFromString(valueInfo.Value);
        if (obj != null)
        {
          valueInfo.CoercedValue = typeInfo.Converter.ConvertToInvariantString(obj);
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public void Initialize(string rootClrName)
    {
      this.dataSet.RootType.Rename(rootClrName);
      this.valueBuilder = this.dataSet.CreateValueBuilder();
    }

    public void Finalize(bool success)
    {
      if (this.valueBuilder != null)
        this.valueBuilder.Finalize(success);
      this.valueBuilder = (SampleDataValueBuilder) null;
    }

    public IType CreateCompositeType(string name)
    {
      if (this.dataSet.RootType.Name == name)
        return (IType) this.dataSet.RootType;
      return (IType) this.dataSet.CreateCompositeType(name);
    }

    public IType CreateCollectionType(string name, IType itemType)
    {
      SampleType itemType1 = this.FromType(itemType);
      return (IType) this.dataSet.CreateCollectionType(name, itemType1);
    }

    public IProperty AddProperty(IType compositeType, string propertyName, IType propertyType)
    {
      SampleCompositeType sampleCompositeType = (SampleCompositeType) compositeType;
      SampleType sampleType = this.FromType(propertyType);
      return (IProperty) sampleCompositeType.AddProperty(propertyName, sampleType);
    }

    public DocumentNode CreateLeafNode(IType type, string value)
    {
      DocumentNode documentNode = (DocumentNode) null;
      if (type is SampleNonBasicType)
        documentNode = (DocumentNode) this.CreateCompositeNode(type);
      else if (value != null)
      {
        string str = !string.IsNullOrEmpty(value) ? this.CoerceValue(value, type) : value;
        documentNode = (DocumentNode) this.valueBuilder.CreateBasicNode((SampleBasicType) this.FromType(type), str);
      }
      return documentNode;
    }

    public DocumentCompositeNode CreateCompositeNode(IType type)
    {
      return this.valueBuilder.CreateCompositeNode((SampleNonBasicType) type);
    }

    private SampleType FromType(IType description)
    {
      return description as SampleType ?? (SampleType) this.basicTypes.Find((Predicate<XmlToSampleDataAdapter.BasicTypeInfo>) (b => b.Type == description)).SampleType;
    }

    private XmlToSampleDataAdapter.BasicTypeInfo CreateBasicTypeInfo(SampleBasicType basicType, TypeConverter converter)
    {
      IType description = this.dataSet.ResolveSampleType((SampleType) basicType);
      if (converter == null)
        converter = this.dataSet.Platform.Metadata.GetTypeConverter((MemberInfo) description.RuntimeType);
      return new XmlToSampleDataAdapter.BasicTypeInfo(basicType, description, converter);
    }

    public override string ToString()
    {
      return this.dataSet.ToString();
    }

    private class BasicTypeInfo
    {
      public SampleBasicType SampleType { get; private set; }

      public IType Type { get; private set; }

      public TypeConverter Converter { get; private set; }

      public BasicTypeInfo(SampleBasicType sampleType, IType description, TypeConverter converter)
      {
        this.SampleType = sampleType;
        this.Type = description;
        this.Converter = converter;
      }

      public override string ToString()
      {
        return this.SampleType.ToString();
      }
    }

    private class ValueInfo
    {
      public string Value { get; set; }

      public string CoercedValue { get; set; }

      public ValueInfo(string value)
      {
        this.Value = value;
        this.CoercedValue = value;
      }

      public override string ToString()
      {
        if (this.Value == this.CoercedValue)
          return this.Value;
        return this.Value + " (" + this.CoercedValue + ")";
      }
    }
  }
}
