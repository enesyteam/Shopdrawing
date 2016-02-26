// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeHelper
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public static class DocumentNodeHelper
  {
    public static void PreserveFormatting(DocumentNode node)
    {
      DocumentNodeHelper.PreserveFormatting(node.DocumentRoot, node);
    }

    internal static void PreserveFormatting(IDocumentRoot documentRoot, DocumentNode node)
    {
      if (node.SourceContext != null)
        documentRoot.SetSourceContext(node, node.SourceContext.FreezeText(false));
      if (node.ContainerSourceContext != null)
        documentRoot.SetContainerSourceContext(node, node.ContainerSourceContext.FreezeText(false));
      foreach (DocumentNode node1 in node.ChildNodes)
        DocumentNodeHelper.PreserveFormatting(documentRoot, node1);
    }

    public static bool IsMarkupExtension(DocumentNode node)
    {
      return node.Type.IsExpression;
    }

    public static DocumentCompositeNode NewStaticNode(IDocumentContext documentContext, IMember memberId)
    {
      IPlatformMetadata platformMetadata = documentContext.TypeResolver.PlatformMetadata;
      DocumentCompositeNode node = documentContext.CreateNode((ITypeId) documentContext.TypeResolver.ResolveType(platformMetadata.KnownTypes.StaticExtension));
      DocumentNode documentNode = (DocumentNode) documentContext.CreateNode(memberId.MemberTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue(memberId));
      node.Properties[platformMetadata.KnownProperties.StaticExtensionMember] = documentNode;
      return node;
    }

    public static DocumentNode GetResourceEntryKey(DocumentCompositeNode entryNode)
    {
      DocumentNode documentNode = entryNode.Properties[entryNode.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryKey];
      if (documentNode == null)
      {
        ITypeResolver typeResolver = entryNode.TypeResolver;
        if (entryNode.TypeResolver.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
        {
          DocumentCompositeNode documentCompositeNode = entryNode.Properties[entryNode.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryValue] as DocumentCompositeNode;
          if (documentCompositeNode != null && documentCompositeNode.NameProperty != null)
            documentNode = documentCompositeNode.Properties[(IPropertyId) documentCompositeNode.NameProperty];
        }
      }
      return documentNode;
    }

    public static bool IsStyleOrTemplate(IType type)
    {
      if (type == null)
        return false;
      if (!type.PlatformMetadata.KnownTypes.Style.IsAssignableFrom((ITypeId) type))
        return type.PlatformMetadata.KnownTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) type);
      return true;
    }

    public static IType GetStyleOrTemplateTargetType(DocumentNode node)
    {
      IType styleOrTemplateType;
      return DocumentNodeHelper.GetStyleOrTemplateTypeAndTargetType(node, out styleOrTemplateType);
    }

    public static IType GetStyleOrTemplateTypeAndTargetType(DocumentNode node, out IType styleOrTemplateType)
    {
      IPlatformMetadata platformMetadata = node.TypeResolver.PlatformMetadata;
      ITargetTypeMetadata targetTypeMetadata = node.Type.Metadata as ITargetTypeMetadata;
      if (platformMetadata.KnownTypes.Style.IsAssignableFrom((ITypeId) node.Type))
      {
        styleOrTemplateType = node.TypeResolver.ResolveType(platformMetadata.KnownTypes.Style);
        IType type1 = DocumentNodeHelper.GetValueAsMember(node as DocumentCompositeNode, targetTypeMetadata.TargetTypeProperty) as IType;
        if (type1 == null)
        {
          IType type2 = node.TypeResolver.ResolveType(platformMetadata.KnownTypes.IFrameworkInputElement);
          type1 = !platformMetadata.IsNullType((ITypeId) type2) ? type2 : (IType) null;
        }
        return type1;
      }
      if (platformMetadata.KnownTypes.DataTemplate.IsAssignableFrom((ITypeId) node.Type))
      {
        styleOrTemplateType = node.TypeResolver.ResolveType(platformMetadata.KnownTypes.DataTemplate);
        return node.TypeResolver.ResolveType(platformMetadata.KnownTypes.ContentPresenter);
      }
      if (platformMetadata.KnownTypes.ControlTemplate.IsAssignableFrom((ITypeId) node.Type))
      {
        styleOrTemplateType = node.TypeResolver.ResolveType(platformMetadata.KnownTypes.ControlTemplate);
        return DocumentNodeHelper.GetValueAsMember(node as DocumentCompositeNode, targetTypeMetadata.TargetTypeProperty) as IType ?? node.TypeResolver.ResolveType(platformMetadata.KnownTypes.Control);
      }
      if (platformMetadata.KnownTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) node.Type))
      {
        styleOrTemplateType = node.TypeResolver.ResolveType(platformMetadata.KnownTypes.FrameworkTemplate);
        return node.TypeResolver.ResolveType(platformMetadata.KnownTypes.FrameworkElement);
      }
      styleOrTemplateType = (IType) null;
      return (IType) null;
    }

    public static void StripExtraNamespaces(DocumentNode node)
    {
      XmlElementReference elementReference = node.SourceContext as XmlElementReference;
      if (elementReference == null)
        return;
      elementReference.RemoveMatchingAttributes((Func<XmlElementReference.Attribute, bool>) (attribute =>
      {
        if (attribute.Type != XmlElementReference.AttributeType.Compatibility)
          return attribute.Type == XmlElementReference.AttributeType.Xmlns;
        return true;
      }));
    }

    public static ITextRange GetNodeSpan(DocumentNode node)
    {
      if (node.DocumentRoot == null)
        return TextRange.Null;
      return XamlSerializerUtilities.GetNodeSpan(node.DocumentRoot.TextBuffer, node, false);
    }

    public static IMemberId GetValueAsMember(DocumentCompositeNode valueNode, IPropertyId property)
    {
      IMemberId memberId = (IMemberId) null;
      if (valueNode != null)
        memberId = (IMemberId) DocumentPrimitiveNode.GetValueAsMember(valueNode.Properties[property]);
      return memberId;
    }

    public static Uri GetUriValue(DocumentNode valueNode)
    {
      Uri uri = (Uri) null;
      DocumentPrimitiveNode documentPrimitiveNode = valueNode as DocumentPrimitiveNode;
      DocumentCompositeNode documentCompositeNode = valueNode as DocumentCompositeNode;
      if (documentPrimitiveNode != null)
        uri = documentPrimitiveNode.GetUriValue();
      else if (documentCompositeNode != null)
        uri = documentCompositeNode.GetUriValue(documentCompositeNode.PlatformMetadata.KnownProperties.BitmapImageUriSource);
      return uri;
    }
  }
}
