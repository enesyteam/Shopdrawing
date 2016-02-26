// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SampleData.IXmlToClrAdapter
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.SampleData
{
  public interface IXmlToClrAdapter
  {
    string ExpectedRootTypeName { get; }

    IType StringType { get; }

    string CollectionSuffix { get; }

    bool ShouldProcessAttribute(string localName, string prefix, string xmlNamespace);

    bool ShouldProcessElement(string localName, string prefix, string xmlNamespace);

    string GetClrName(string localName, string prefix, string xmlNamespace);

    void Initialize(string rootClrName);

    void Finalize(bool success);

    IType InferValueType(string value);

    IType CreateCompositeType(string name);

    IProperty AddProperty(IType compositeType, string propertyName, IType propertyType);

    IType CreateCollectionType(string name, IType itemType);

    DocumentNode CreateLeafNode(IType type, string value);

    DocumentCompositeNode CreateCompositeNode(IType type);
  }
}
