// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.IXamlSerializerFilter
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.Markup
{
  public interface IXamlSerializerFilter
  {
    SerializedFormat ShouldSerializeNode(XamlSerializerContext serializerContext, DocumentNode node);

    SerializedFormat ShouldSerializeProperty(XamlSerializerContext serializerContext, DocumentCompositeNode parentNode, IPropertyId propertyKey, DocumentNode valueNode);

    SerializedFormat ShouldSerializeChild(XamlSerializerContext serializerContext, DocumentCompositeNode parentNode, DocumentNode childNode);

    bool IsDesignTimeProperty(IPropertyId property);

    IXmlNamespace GetReplacementNamespace(IXmlNamespace xmlNamespace);

    string GetClrNamespace(IType type);

    IAssembly GetClrAssembly(IType type);

    bool IsXmlSpacePreserveIgnored(IType typeId);

    string GetValueAsString(DocumentNode node);

    bool ShouldOverrideNamespaceForType(XamlSerializerContext serializerContext, ITypeId typeId);
  }
}
