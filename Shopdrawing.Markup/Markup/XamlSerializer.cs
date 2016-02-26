// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlSerializer
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class XamlSerializer
  {
    private IDocumentRoot document;
    private XamlSerializerContext serializerContext;
    private IXamlSerializerFilter filter;
    private bool shouldUpdateSourceContext;

    internal List<KeyValuePair<XmlAttribute, XmlAttribute>> RootAttributesModified { get; private set; }

    private IXmlNamespaceTypeResolver ProjectNamespaces
    {
      get
      {
        return this.serializerContext.TypeResolver.ProjectNamespaces;
      }
    }

    private PersistenceSettings PersistenceSettings
    {
      get
      {
        return this.document.PersistenceSettings;
      }
    }

    private IPlatformMetadata PlatformMetadata
    {
      get
      {
        return this.document.TypeResolver.PlatformMetadata;
      }
    }

    public static event EventHandler Serializing;

    public static event EventHandler Serialized;

    public XamlSerializer(IDocumentRoot document, IXamlSerializerFilter filter)
      : this(document, filter, false)
    {
      this.document = document;
      this.filter = filter;
    }

    public XamlSerializer(IDocumentRoot document, IXamlSerializerFilter filter, bool shouldUpdateSourceContext)
    {
      this.document = document;
      this.filter = filter;
      this.shouldUpdateSourceContext = shouldUpdateSourceContext;
    }

    public string Serialize()
    {
      if (this.document.RootNode == null)
        return string.Empty;
      return this.Serialize(this.document.RootNode);
    }

    public string Serialize(DocumentNode node)
    {
      StringBuilder sb = new StringBuilder();
      using (StringWriter stringWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
        this.Serialize(node, (TextWriter) stringWriter);
      return sb.ToString();
    }

    public void Serialize(DocumentNode node, TextWriter writer)
    {
      this.Serialize(node, (DocumentNode) null, new XamlSerializerContext(this.PersistenceSettings, this.document.DocumentContext, CultureInfo.InvariantCulture), writer);
    }

    public void Serialize(DocumentNode node, DocumentNode nodeInDocument, XamlSerializerContext context, TextWriter writer)
    {
      if (XamlSerializer.Serializing != null)
        XamlSerializer.Serializing((object) this, EventArgs.Empty);
      IDictionary<string, XmlAttribute> rootAttributeNames = this.GetRootAttributeNames(node.SourceContext as XmlElementReference);
      this.serializerContext = context;
      using (this.serializerContext.PushTargetType(node.Type))
      {
        using (this.serializerContext.PushNode(this.BuildElementNode(node, new int?(), true)))
        {
          if (nodeInDocument != null)
          {
            foreach (DocumentNode node1 in Enumerable.Reverse<DocumentNode>(nodeInDocument.AncestorNodes))
              this.WriteDirectiveAttributes(node1);
            this.WriteDirectiveAttributes(nodeInDocument);
          }
          this.WriteElementContentAndSetName(node);
        }
      }
      XamlDocument xamlDocument = this.document as XamlDocument;
      if (xamlDocument != null && node == xamlDocument.RootNode)
        this.WriteOtherChildNodes((XmlContainerReference) xamlDocument.XmlDocumentReference);
      new XamlFormatter(this.document.PersistenceSettings, writer, this.shouldUpdateSourceContext).WriteDocument((ContainerNode) this.serializerContext.DocumentRoot);
      this.RootAttributesModified = this.serializerContext.Node.Children.Count < 1 ? (List<KeyValuePair<XmlAttribute, XmlAttribute>>) null : this.GetModifiedAttributes(rootAttributeNames, this.serializerContext.Node.Children[0] as ElementNode);
      if (XamlSerializer.Serialized == null)
        return;
      XamlSerializer.Serialized((object) this, EventArgs.Empty);
    }

    internal StringBuilder SerializeRegion(DocumentNode node, out List<KeyValuePair<XmlAttribute, XmlAttribute>> rootAttributesModified)
    {
      this.serializerContext = new XamlSerializerContext(this.PersistenceSettings, this.document.DocumentContext, CultureInfo.InvariantCulture);
      List<DocumentNode> list = new List<DocumentNode>(node.AncestorNodes);
      if (list.Count <= 0)
        return this.SerializeRegion((IList<DocumentNode>) list, 0, node, CultureInfo.InvariantCulture, out rootAttributesModified);
      list.Reverse();
      using (this.serializerContext.PushTargetType(list[0].Type))
        return this.SerializeRegion((IList<DocumentNode>) list, 0, node, CultureInfo.InvariantCulture, out rootAttributesModified);
    }

    public static string SerializeValue(IDocumentRoot document, IXamlSerializerFilter filter, DocumentNode valueNode, CultureInfo cultureInfo)
    {
      return new XamlSerializer(document, filter).SerializeValue(valueNode, cultureInfo).Value;
    }

    private IDictionary<string, XmlAttribute> GetRootAttributeNames(XmlElementReference sourceContext)
    {
      Dictionary<string, XmlAttribute> dictionary = new Dictionary<string, XmlAttribute>();
      if (sourceContext != null)
      {
        foreach (XmlElementReference.Attribute attribute in Enumerable.Where<XmlElementReference.Attribute>(sourceContext.AttributesToPreserve, (Func<XmlElementReference.Attribute, bool>) (a =>
        {
          if (a.Type != XmlElementReference.AttributeType.Xmlns)
            return a.Type == XmlElementReference.AttributeType.SerializerAddedXmlns;
          return true;
        })))
          dictionary[attribute.XmlAttribute.Name.FullName] = attribute.XmlAttribute;
      }
      return (IDictionary<string, XmlAttribute>) dictionary;
    }

    private List<KeyValuePair<XmlAttribute, XmlAttribute>> GetModifiedAttributes(IDictionary<string, XmlAttribute> rootAttributeNames, ElementNode element)
    {
      if (element == null)
        return (List<KeyValuePair<XmlAttribute, XmlAttribute>>) null;
      List<KeyValuePair<XmlAttribute, XmlAttribute>> list = new List<KeyValuePair<XmlAttribute, XmlAttribute>>();
      foreach (FormattedNode formattedNode in element.Attributes)
      {
        string key = (string) null;
        XmlAttribute attribute = (XmlAttribute) null;
        AttributeNode attributeNode = formattedNode as AttributeNode;
        if (attributeNode != null)
        {
          key = attributeNode.Name.FullName;
          Identifier identifier = new Identifier(attributeNode.Name.TypeQualifiedName);
          if (attributeNode.Name.Prefix != null)
          {
            identifier = (Identifier) new ComplexIdentifier(identifier);
            identifier.Prefix = new Identifier(attributeNode.Name.Prefix.Value);
          }
          attribute = new XmlAttribute(identifier);
          attribute.AddChild((Microsoft.Expression.DesignModel.Markup.Xml.Node) new Literal(attributeNode.Value));
        }
        if (key != null && XmlUtilities.IsXmlnsDeclaration(attribute))
        {
          bool flag = false;
          XmlAttribute xmlAttribute;
          if (rootAttributeNames.TryGetValue(key, out xmlAttribute))
          {
            if (XmlUtilities.GetAttributeValue(xmlAttribute) != XmlUtilities.GetAttributeValue(attribute))
              flag = true;
            rootAttributeNames.Remove(key);
          }
          else
            flag = true;
          if (flag)
            list.Add(new KeyValuePair<XmlAttribute, XmlAttribute>(xmlAttribute, attribute));
        }
      }
      foreach (XmlAttribute xmlAttribute in (IEnumerable<XmlAttribute>) rootAttributeNames.Values)
      {
        if (XmlUtilities.IsXmlnsDeclaration(xmlAttribute))
          list.Add(new KeyValuePair<XmlAttribute, XmlAttribute>(xmlAttribute, (XmlAttribute) null));
      }
      if (list.Count == 0)
        list = (List<KeyValuePair<XmlAttribute, XmlAttribute>>) null;
      return list;
    }

    private StringBuilder SerializeRegion(IList<DocumentNode> ancestorNodes, int ancestorIndex, DocumentNode node, CultureInfo cultureInfo, out List<KeyValuePair<XmlAttribute, XmlAttribute>> rootAttributesModified)
    {
      rootAttributesModified = (List<KeyValuePair<XmlAttribute, XmlAttribute>>) null;
      if (ancestorIndex < ancestorNodes.Count)
      {
        DocumentNode documentNode1 = ancestorNodes[ancestorIndex];
        StringBuilder stringBuilder = (StringBuilder) null;
        if (ancestorIndex > 0)
        {
          DocumentNode documentNode2 = ancestorNodes[ancestorIndex - 1];
        }
        IDisposable disposable1 = (IDisposable) null;
        IDisposable disposable2 = (IDisposable) null;
        try
        {
          ElementNode elementNode = (ElementNode) null;
          XmlElementReference elementReference = documentNode1.ContainerSourceContext as XmlElementReference;
          if (elementReference != null)
          {
            elementNode = XamlSerializer.CreateElement((SourceContextReference) new ContainerSourceContextReference(documentNode1), XamlSerializer.GetOrdering((XamlSourceContext) elementReference));
            this.serializerContext.Node.Children.Add((FormattedNode) elementNode);
            disposable2 = this.serializerContext.PushNode(elementNode);
          }
          XmlElementReference sourceContext = documentNode1.SourceContext as XmlElementReference;
          if (sourceContext != null)
          {
            elementNode = XamlSerializer.CreateElement((SourceContextReference) new SelfSourceContextReference(documentNode1), XamlSerializer.GetOrdering((XamlSourceContext) sourceContext));
            this.serializerContext.Node.Children.Add((FormattedNode) elementNode);
            disposable1 = this.serializerContext.PushNode(elementNode);
          }
          this.WriteDirectiveAttributes(documentNode1);
          IDictionary<string, XmlAttribute> rootAttributeNames = (IDictionary<string, XmlAttribute>) null;
          if (ancestorIndex == 0)
            rootAttributeNames = this.GetRootAttributeNames(sourceContext);
          IType type = documentNode1.Type;
          List<KeyValuePair<XmlAttribute, XmlAttribute>> rootAttributesModified1;
          if (DocumentNodeHelper.IsStyleOrTemplate(type))
          {
            using (this.serializerContext.PushStyleOrTemplate((ITypeId) type))
            {
              using (this.serializerContext.PushTargetType(DocumentNodeHelper.GetStyleOrTemplateTargetType(documentNode1)))
                stringBuilder = this.SerializeRegion(ancestorNodes, ancestorIndex + 1, node, cultureInfo, out rootAttributesModified1);
            }
          }
          else
            stringBuilder = this.SerializeRegion(ancestorNodes, ancestorIndex + 1, node, cultureInfo, out rootAttributesModified1);
          if (ancestorIndex == 0)
            rootAttributesModified = this.GetModifiedAttributes(rootAttributeNames, elementNode);
        }
        finally
        {
          if (disposable1 != null)
            disposable1.Dispose();
          if (disposable2 != null)
            disposable2.Dispose();
        }
        return stringBuilder;
      }
      FormattedNode node1 = (FormattedNode) null;
      StringBuilder sb = new StringBuilder();
      if (node.Parent != null && node.IsProperty)
      {
        IProperty sitePropertyKey = node.SitePropertyKey;
        DocumentCompositeNode parent = node.Parent;
        switch (this.filter.ShouldSerializeProperty(this.serializerContext, parent, (IPropertyId) sitePropertyKey, node))
        {
          case SerializedFormat.SimpleString:
          case SerializedFormat.ComplexString:
            XmlAttributeReference container1 = node.ContainerSourceContext as XmlAttributeReference;
            this.WritePropertyAttribute(parent.Type, sitePropertyKey, container1, node);
            ElementNode elementNode1 = this.serializerContext.Node as ElementNode;
            if (elementNode1 != null)
            {
              using (IEnumerator<FormattedNode> enumerator = elementNode1.Attributes.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  AttributeNode attributeNode = enumerator.Current as AttributeNode;
                  if (attributeNode != null && attributeNode.SourceContextReference != null && attributeNode.SourceContextReference.DocumentNode == node)
                  {
                    node1 = (FormattedNode) attributeNode;
                    break;
                  }
                }
                break;
              }
            }
            else
              break;
          case SerializedFormat.Element:
            XmlElementReference container2 = node.ContainerSourceContext as XmlElementReference;
            int ordering = XamlSerializer.GetOrdering((XamlSourceContext) container2);
            this.WritePropertyElement(parent, sitePropertyKey, container2, ordering, node);
            ElementNode elementNode2 = (ElementNode) this.serializerContext.Node;
            if (elementNode2.Children.Count == 0)
              return (StringBuilder) null;
            using (StringWriter stringWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
            {
              XamlFormatter xamlFormatter = new XamlFormatter(this.document.PersistenceSettings, (TextWriter) stringWriter, this.shouldUpdateSourceContext);
              foreach (FormattedNode node2 in elementNode2.Children)
                xamlFormatter.WriteNode(node2);
            }
            return sb;
        }
      }
      else
        node1 = !this.PlatformMetadata.KnownTypes.DictionaryEntry.IsAssignableFrom((ITypeId) node.Type) ? this.WriteElement(node, new int?(), false) : (FormattedNode) this.WriteDictionaryEntry((DocumentCompositeNode) node);
      if (node1 == null)
        return (StringBuilder) null;
      using (StringWriter stringWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
        new XamlFormatter(this.document.PersistenceSettings, (TextWriter) stringWriter, this.shouldUpdateSourceContext).WriteNode(node1);
      return sb;
    }

    private XamlSerializer.EscapedString SerializeValue(DocumentNode valueNode, CultureInfo cultureInfo)
    {
      this.serializerContext = new XamlSerializerContext(this.PersistenceSettings, this.document.DocumentContext, cultureInfo);
      List<DocumentNode> list = new List<DocumentNode>(valueNode.AncestorNodes);
      list.Reverse();
      using (this.serializerContext.PushTargetType(list[0].Type))
        return this.SerializeValue((IList<DocumentNode>) list, 0, valueNode, cultureInfo);
    }

    private XamlSerializer.EscapedString SerializeValue(IList<DocumentNode> ancestorNodes, int ancestorIndex, DocumentNode valueNode, CultureInfo cultureInfo)
    {
      if (ancestorIndex >= ancestorNodes.Count)
        return this.GetValueAsString(valueNode, XamlSerializer.StringFlags.EscapeLiterals);
      DocumentNode documentNode = ancestorNodes[ancestorIndex];
      int ordering = XamlSerializer.GetOrdering((XamlSourceContext) (documentNode.SourceContext as XmlElementReference));
      ElementNode element = XamlSerializer.CreateElement((SourceContextReference) new SelfSourceContextReference(documentNode), ordering);
      this.serializerContext.Node.Children.Add((FormattedNode) element);
      using (this.serializerContext.PushNode(element))
      {
        this.WriteDirectiveAttributes(documentNode);
        IType type = documentNode.Type;
        XamlSerializer.EscapedString escapedString;
        if (DocumentNodeHelper.IsStyleOrTemplate(type))
        {
          using (this.serializerContext.PushStyleOrTemplate((ITypeId) type))
          {
            using (this.serializerContext.PushTargetType(DocumentNodeHelper.GetStyleOrTemplateTargetType(documentNode)))
              escapedString = this.SerializeValue(ancestorNodes, ancestorIndex + 1, valueNode, cultureInfo);
          }
        }
        else
          escapedString = this.SerializeValue(ancestorNodes, ancestorIndex + 1, valueNode, cultureInfo);
        this.WriteOtherChildNodes(documentNode);
        return escapedString;
      }
    }

    private bool HasAdditionalChildNodes(DocumentNode node)
    {
      XmlElementReference elementReference = node.SourceContext as XmlElementReference;
      if (elementReference == null)
        return false;
      if (!Enumerable.Any<XmlElementReference.Attribute>(elementReference.AttributesToPreserve))
        return Enumerable.Any<XmlContainerReference.ChildNode>(elementReference.ChildNodesToPreserve);
      return true;
    }

    private void WriteOtherChildNodes(DocumentNode node)
    {
      XmlContainerReference sourceContext = node.SourceContext as XmlContainerReference;
      if (sourceContext == null)
        return;
      this.WriteOtherChildNodes(sourceContext);
    }

    private void WriteOtherChildNodes(XmlContainerReference sourceContext)
    {
      ContainerNode node = this.serializerContext.Node;
      int index = 0;
      foreach (XmlContainerReference.ChildNode childNode in sourceContext.ChildNodesToPreserve)
      {
        UnprocessedNode unprocessedNode = new UnprocessedNode(childNode.Text, (SourceContextReference) new UnprocessedNodeSourceContextReference(childNode.SourceContext), XamlSerializer.GetOrdering(childNode.SourceContext));
        while (index < node.Children.Count && unprocessedNode.Ordering >= node.Children[index].Ordering)
          ++index;
        node.Children.Insert(index, (FormattedNode) unprocessedNode);
      }
    }

    private void WriteDirectiveAttributes(DocumentNode node)
    {
      XmlElementReference sourceContext = node.SourceContext as XmlElementReference;
      if (sourceContext == null)
        return;
      this.WriteDirectiveAttributes(sourceContext);
    }

    private void WriteDirectiveAttributes(XmlElementReference sourceContext)
    {
      ElementNode elementNode = this.serializerContext.Node as ElementNode;
      if (elementNode == null)
        return;
      List<string> list = new List<string>();
      XmlAttribute ignorableAttribute = (XmlAttribute) null;
      for (int index = sourceContext.AttributesToPreserveCount - 1; index >= 0; --index)
      {
        XmlElementReference.Attribute attributeToPreserveAt = sourceContext.GetAttributeToPreserveAt(index);
        FormattedNode formattedNode = (FormattedNode) null;
        XmlAttribute xmlAttribute = attributeToPreserveAt.XmlAttribute;
        string literalValue = xmlAttribute.LiteralValue;
        switch (attributeToPreserveAt.Type)
        {
          case XmlElementReference.AttributeType.XmlLang:
            formattedNode = (FormattedNode) new AttributeNode((SourceContextReference) new PreservedAttributeSourceContextReference(attributeToPreserveAt), XamlSerializer.GetOrdering(attributeToPreserveAt.SourceContext), ElementNode.XmlLangName, literalValue);
            break;
          case XmlElementReference.AttributeType.XmlSpace:
            formattedNode = (FormattedNode) new AttributeNode((SourceContextReference) new PreservedAttributeSourceContextReference(attributeToPreserveAt), XamlSerializer.GetOrdering(attributeToPreserveAt.SourceContext), ElementNode.XmlSpaceName, literalValue);
            break;
          case XmlElementReference.AttributeType.Xmlns:
            if (literalValue == "http://schemas.microsoft.com/expression/blend/2006" || literalValue == "http://schemas.microsoft.com/expression/interactivedesigner/2006")
            {
              if (xmlAttribute.LocalName == "d")
              {
                formattedNode = (FormattedNode) new AttributeNode(new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix(xmlAttribute.Prefix), xmlAttribute.LocalName), "http://schemas.microsoft.com/expression/blend/2008");
                formattedNode.Ordering = XamlSerializer.GetOrdering(attributeToPreserveAt.SourceContext);
              }
              else
                list.Add(xmlAttribute.LocalName);
              sourceContext.RemoveAttributeToPreserveAt(index);
            }
            else
            {
              IXmlNamespace replacementNamespace = this.filter.GetReplacementNamespace((IXmlNamespace) XmlNamespace.ToNamespace(literalValue, XmlNamespaceCanonicalization.None));
              formattedNode = (FormattedNode) new XmlnsNode((SourceContextReference) new PreservedAttributeSourceContextReference(attributeToPreserveAt), int.MinValue, new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix(xmlAttribute.Prefix), xmlAttribute.LocalName), replacementNamespace.Value);
            }
            if (formattedNode != null)
            {
              XmlnsPrefix prefix = xmlAttribute.Prefix == "xmlns" ? XmlnsPrefix.ToPrefix(xmlAttribute.LocalName) : XmlnsPrefix.EmptyPrefix;
              XmlNamespace xmlNamespace = XmlNamespace.ToNamespace(literalValue, XmlNamespace.GetNamespaceCanonicalization(this.document.TypeResolver));
              elementNode.AddNamespace(prefix, (IXmlNamespace) xmlNamespace);
              break;
            }
            break;
          case XmlElementReference.AttributeType.Compatibility:
            XmlnsPrefixAndName name = new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix(xmlAttribute.Prefix), xmlAttribute.LocalName);
            if (name.TypeQualifiedName == "Ignorable")
              ignorableAttribute = xmlAttribute;
            formattedNode = (FormattedNode) new AttributeNode((SourceContextReference) new PreservedAttributeSourceContextReference(attributeToPreserveAt), XamlSerializer.GetOrdering(attributeToPreserveAt.SourceContext), name, literalValue);
            break;
          case XmlElementReference.AttributeType.Ignored:
            formattedNode = (FormattedNode) new AttributeNode((SourceContextReference) new PreservedAttributeSourceContextReference(attributeToPreserveAt), XamlSerializer.GetOrdering(attributeToPreserveAt.SourceContext), new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix(xmlAttribute.Prefix), xmlAttribute.LocalName), literalValue);
            break;
        }
        if (formattedNode != null)
          elementNode.Attributes.Insert(0, formattedNode);
      }
      if (ignorableAttribute == null || list.Count <= 0)
        return;
      ICollection<string> prefixes = XmlUtilities.GetPrefixes(ignorableAttribute.Value.Value);
      foreach (string str in list)
        prefixes.Remove(str);
      if (prefixes.Count == 0)
      {
        sourceContext.RemoveMatchingAttributes((Func<XmlElementReference.Attribute, bool>) (attribute => attribute.XmlAttribute == ignorableAttribute));
      }
      else
      {
        bool flag = true;
        StringBuilder stringBuilder = new StringBuilder(ignorableAttribute.Value.Value.Length);
        foreach (string str in (IEnumerable<string>) prefixes)
        {
          if (flag)
            flag = false;
          else
            stringBuilder.Append(' ');
          stringBuilder.Append(str);
        }
        ignorableAttribute.Value.Value = stringBuilder.ToString();
      }
    }

    private void WritePrecedingCommentIfAny(XmlElementReference sourceContext)
    {
      if (sourceContext == null)
        return;
      XmlContainerReference.ChildNode comment = sourceContext.Comment;
      if (comment == null)
        return;
      this.serializerContext.Node.Children.Add((FormattedNode) new UnprocessedNode(comment.Text, (SourceContextReference) new UnprocessedNodeSourceContextReference(comment.SourceContext), XamlSerializer.GetOrdering(comment.SourceContext)));
    }

    private ElementNode BuildElementNode(DocumentNode node, int? requiredOrdering, bool shouldWritePrecedingComment)
    {
      ContainerNode node1 = this.serializerContext.Node;
      XmlElementReference sourceContext = node.SourceContext as XmlElementReference;
      int ordering = XamlSerializer.GetOrdering((XamlSourceContext) sourceContext, requiredOrdering);
      if (shouldWritePrecedingComment)
        this.WritePrecedingCommentIfAny(sourceContext);
      ElementNode element = XamlSerializer.CreateElement((SourceContextReference) new SelfSourceContextReference(node), ordering);
      node1.Children.Add((FormattedNode) element);
      if (node.Type.Metadata.IsWhitespaceSignificant)
        element.IsWhitespaceSignificant = true;
      return element;
    }

    private FormattedNode WriteElement(DocumentNode node)
    {
      return this.WriteElement(node, new int?(), true);
    }

    private FormattedNode WriteElement(DocumentNode node, int? requiredOrdering, bool shouldWritePrecedingComment)
    {
      switch (this.filter.ShouldSerializeNode(this.serializerContext, node))
      {
        case SerializedFormat.Element:
          ElementNode node1 = this.BuildElementNode(node, requiredOrdering, shouldWritePrecedingComment);
          using (this.serializerContext.PushNode(node1))
            this.WriteElementContentAndSetName(node);
          return (FormattedNode) node1;
        default:
          return (FormattedNode) null;
      }
    }

    private void WriteElementContentAndSetName(DocumentNode node)
    {
      ElementNode elementNode = (ElementNode) this.serializerContext.Node;
      ITypeResolver typeResolver = node.TypeResolver;
      IKnownTypes knownTypes = node.TypeResolver.PlatformMetadata.KnownTypes;
      this.WriteDirectiveAttributes(node);
      if (DocumentPrimitiveNode.IsNull(node))
        elementNode.Name = this.GetMemberName(typeResolver.ResolveType(knownTypes.NullExtension), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
      else if (knownTypes.Type.IsAssignableFrom((ITypeId) node.Type))
      {
        elementNode.Name = this.GetMemberName(typeResolver.ResolveType(knownTypes.TypeExtension), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
        IType typeId = DocumentPrimitiveNode.GetValueAsMember(node) as IType;
        if (typeId != null)
        {
          string fullName = this.GetMemberName(typeId, XamlSerializer.TypeNameSerialization.NoChanges).FullName;
          this.WritePropertyAttribute(typeResolver.ResolveType(knownTypes.TypeExtension), typeResolver.ResolveProperty(this.PlatformMetadata.KnownProperties.TypeExtensionTypeName), (XmlAttributeReference) null, (DocumentNode) null, (XamlSerializer.EscapedString) fullName);
        }
      }
      else if (node.Type.IsArray)
      {
        elementNode.Name = this.GetMemberName(typeResolver.ResolveType(knownTypes.ArrayExtension), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
        IType itemType = node.Type.ItemType;
        if (itemType != null)
        {
          string extensionAsString = this.GetTypeExtensionAsString(itemType);
          this.WritePropertyAttribute(typeResolver.ResolveType(knownTypes.ArrayExtension), typeResolver.ResolveProperty(this.PlatformMetadata.KnownProperties.ArrayExtensionType), (XmlAttributeReference) null, (DocumentNode) null, (XamlSerializer.EscapedString) extensionAsString);
        }
        this.WriteElementBody(node);
      }
      else
      {
        if (knownTypes.XData.Equals((object) node.Type))
        {
          elementNode.Name = new XmlnsPrefixAndName(this.GetXmlnsPrefix((IXmlNamespace) XmlNamespace.XamlXmlNamespace), "XData");
          string content = (string) null;
          DocumentPrimitiveNode documentPrimitiveNode = node as DocumentPrimitiveNode;
          if (documentPrimitiveNode != null)
          {
            DocumentNodeStringValue documentNodeStringValue = documentPrimitiveNode.Value as DocumentNodeStringValue;
            if (documentNodeStringValue != null)
              content = documentNodeStringValue.Value;
          }
          if (content == null)
            content = string.Empty;
          elementNode.Children.Add((FormattedNode) new ContentNode((SourceContextReference) new SelfSourceContextReference(node), content, false));
          return;
        }
        elementNode.Name = this.GetMemberName(this.GetNodeType(node), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
        this.WriteElementBody(node);
      }
      this.WriteOtherChildNodes(node);
    }

    private void WriteElementBody(DocumentNode node)
    {
      DocumentCompositeNode node1 = node as DocumentCompositeNode;
      if (node1 != null)
      {
        IType type = node.Type;
        XamlSerializer.IPropertyValueCollection sortedProperties = XamlSerializer.GetSortedProperties(node1);
        if (DocumentNodeHelper.IsStyleOrTemplate(type))
        {
          IType templateTargetType = DocumentNodeHelper.GetStyleOrTemplateTargetType((DocumentNode) node1);
          this.WritePropertiesAndVisualTree(node1, templateTargetType, sortedProperties);
        }
        else
          this.WriteAttributesElementsAndChildren(node1, sortedProperties);
      }
      else
      {
        XamlSerializer.EscapedString valueAsString = this.GetValueAsString(node, XamlSerializer.StringFlags.None);
        if (!valueAsString.IsNonNull)
          return;
        if (XamlSerializer.IsXmlSpacePreserveRequired(this.filter, node.Type, valueAsString.Value))
          this.EnsureXmlSpaceIsSetCorrectly(XmlSpace.Preserve);
        this.WriteElementContent(node, valueAsString.Value);
      }
    }

    private void WriteElementContent(DocumentNode documentNode, string content)
    {
      this.serializerContext.Node.Children.Add((FormattedNode) new ContentNode((SourceContextReference) new SelfSourceContextReference(documentNode), content, true));
    }

    private void WriteElementChildren(DocumentCompositeNode node)
    {
      if (!node.SupportsChildren || node.Children.Count <= 0)
        return;
      if (this.PlatformMetadata.KnownTypes.IDictionary.IsAssignableFrom((ITypeId) node.Type) || this.PlatformMetadata.KnownTypes.ResourceDictionary.IsAssignableFrom((ITypeId) node.Type))
      {
        this.WriteDictionaryContents(node);
      }
      else
      {
        List<XamlSerializer.NodeFormatting> list = new List<XamlSerializer.NodeFormatting>(node.Children.Count);
        foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) node.Children)
        {
          SerializedFormat format = this.filter.ShouldSerializeChild(this.serializerContext, node, documentNode);
          switch (format)
          {
            case SerializedFormat.ComplexString:
            case SerializedFormat.Element:
              list.Add(new XamlSerializer.NodeFormatting(documentNode, format));
              continue;
            default:
              continue;
          }
        }
        int num1 = int.MaxValue;
        foreach (XamlSerializer.NodeFormatting nodeFormatting in list)
        {
          num1 = nodeFormatting.Ordering;
          if (num1 < int.MaxValue)
            break;
        }
        foreach (XamlSerializer.NodeFormatting nodeFormatting in list)
        {
          DocumentNode node1 = nodeFormatting.Node;
          int num2 = nodeFormatting.Ordering;
          if (num2 == int.MaxValue)
            num2 = num1;
          switch (nodeFormatting.Format)
          {
            case SerializedFormat.ComplexString:
              XamlSerializer.EscapedString valueAsString = this.GetValueAsString(node1, XamlSerializer.StringFlags.None);
              if (valueAsString.IsNonNull)
              {
                if (XamlSerializer.IsXmlSpacePreserveRequired(this.filter, node1.Type, valueAsString.Value))
                  this.EnsureXmlSpaceIsSetCorrectly(XmlSpace.Preserve);
                this.WriteElementContent(node1, valueAsString.Value);
                break;
              }
              break;
            case SerializedFormat.Element:
              this.WriteElement(node1, new int?(num2), true);
              break;
          }
          num1 = num2;
        }
      }
    }

    private void WritePropertyAttributes(DocumentCompositeNode node, XamlSerializer.IPropertyValueCollection propertyValues)
    {
      IType nodeType = this.GetNodeType((DocumentNode) node);
      foreach (XamlSerializer.PropertyValue propertyValue in (IEnumerable<XamlSerializer.PropertyValue>) propertyValues)
      {
        IProperty propertyKey = propertyValue.PropertyKey;
        XmlAttributeReference container = propertyValue.SourceContext as XmlAttributeReference;
        DocumentNode valueNode = propertyValue.ValueNode;
        switch (this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) propertyKey, valueNode))
        {
          case SerializedFormat.SimpleString:
          case SerializedFormat.ComplexString:
            this.WritePropertyAttribute(nodeType, propertyKey, container, valueNode);
            continue;
          default:
            continue;
        }
      }
    }

    private void WritePropertyElements(DocumentCompositeNode node, XamlSerializer.IPropertyValueCollection propertyValues)
    {
      this.GetNodeType((DocumentNode) node);
      IPropertyId propertyId1 = this.WriteResourcesPropertyElement(node);
      IPropertyId propertyId2 = (IPropertyId) XamlSerializer.ResolveProperty((IPropertyId) node.Type.Metadata.DefaultContentProperty, (DocumentNode) node);
      DocumentNode documentNode = (DocumentNode) null;
      foreach (XamlSerializer.PropertyValue propertyValue in (IEnumerable<XamlSerializer.PropertyValue>) propertyValues)
      {
        IProperty propertyKey = propertyValue.PropertyKey;
        if (!propertyKey.Equals((object) propertyId1))
        {
          XmlElementReference elementReference = propertyValue.SourceContext as XmlElementReference;
          DocumentNode valueNode = propertyValue.ValueNode;
          if (this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) propertyKey, valueNode) == SerializedFormat.Element)
          {
            if (propertyKey.Equals((object) propertyId2))
            {
              documentNode = valueNode;
            }
            else
            {
              int ordering = XamlSerializer.GetOrdering((XamlSourceContext) elementReference);
              this.WritePrecedingCommentIfAny(elementReference);
              this.WritePropertyElement(node, propertyKey, elementReference, ordering, valueNode);
            }
          }
        }
      }
      if (documentNode == null)
        return;
      this.WriteContentPropertyElement(node);
    }

    private void WriteContentPropertyElement(DocumentCompositeNode node)
    {
      IPropertyId propertyKey1 = (IPropertyId) node.Type.Metadata.DefaultContentProperty;
      if (propertyKey1 == null)
        return;
      IProperty propertyKey2 = XamlSerializer.ResolveProperty(propertyKey1, (DocumentNode) node);
      if (propertyKey2 == null)
        return;
      INodeSourceContext containerContext;
      DocumentNode documentNode = node.GetValue((IPropertyId) propertyKey2, out containerContext);
      if (documentNode == null || this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) propertyKey2, documentNode) == SerializedFormat.DoNotSerialize)
        return;
      XmlElementReference elementReference = containerContext as XmlElementReference;
      int ordering = XamlSerializer.GetOrdering((XamlSourceContext) elementReference);
      this.WritePrecedingCommentIfAny(elementReference);
      this.WritePropertyElement(node, propertyKey2, elementReference, ordering, documentNode);
    }

    private void WritePropertyAttribute(IType parentType, IProperty propertyKey, XmlAttributeReference container, DocumentNode node)
    {
      XamlSerializer.EscapedString valueAsString = this.GetValueAsString(node, XamlSerializer.StringFlags.EscapeLiterals);
      if (!valueAsString.IsNonNull)
        return;
      this.WritePropertyAttribute(parentType, propertyKey, container, node, valueAsString);
    }

    private void WritePropertyAttribute(IType parentType, IProperty propertyKey, XmlAttributeReference container, DocumentNode node, XamlSerializer.EscapedString value)
    {
      XmlnsPrefixAndName memberName = this.GetMemberName(parentType, propertyKey, XamlSerializer.IncludeQualifier.IfNeeded, XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
      ((ElementNode) this.serializerContext.Node).Attributes.Add((FormattedNode) XamlSerializer.CreateAttribute(node, container, memberName, value));
    }

    private bool ShouldWritePropertyElement(DocumentCompositeNode parentNode, IProperty propertyKey, DocumentNode node, out bool canSkipCreatingElement, out bool childrenOnlyNoContainer)
    {
      ITypeMetadata metadata = this.GetNodeType((DocumentNode) parentNode).Metadata;
      DocumentCompositeNode node1 = node as DocumentCompositeNode;
      IType propertyType = propertyKey.PropertyType;
      bool allowProtectedProperties = parentNode == this.document.RootNode && this.document.RootClassAttributes != null;
      bool flag;
      if (TypeHelper.IsPropertyWritable(this.serializerContext.TypeResolver, propertyKey, allowProtectedProperties))
      {
        flag = propertyType.ItemType != null;
        if (flag && node1 != null && node1.IsExplicitCollection)
          flag = false;
      }
      else
        flag = true;
      childrenOnlyNoContainer = flag && node1 != null && (!propertyType.IsArray && propertyType == node1.Type) && !XamlSerializer.HasAnyNonContentPropertiesSet(node1) && !this.HasAdditionalChildNodes((DocumentNode) node1);
      if (childrenOnlyNoContainer && node1.Properties.Count == 0 && (node1.SupportsChildren && node1.Children.Count == 0))
      {
        canSkipCreatingElement = true;
        return false;
      }
      canSkipCreatingElement = (childrenOnlyNoContainer || propertyType.ItemType == null) && propertyKey.Equals((object) metadata.DefaultContentProperty);
      return true;
    }

    private ElementNode CreatePropertyElementNode(DocumentNode documentNode, int ordering, IType parentType, IProperty propertyKey)
    {
      ElementNode element = XamlSerializer.CreateElement((SourceContextReference) new ContainerSourceContextReference(documentNode), ordering);
      element.Name = this.GetMemberName(parentType, propertyKey, XamlSerializer.IncludeQualifier.Always, XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix);
      return element;
    }

    private void WritePropertyElement(DocumentCompositeNode parentNode, IProperty propertyKey, XmlElementReference container, int ordering, DocumentNode node)
    {
      bool canSkipCreatingElement;
      bool childrenOnlyNoContainer;
      if (!this.ShouldWritePropertyElement(parentNode, propertyKey, node, out canSkipCreatingElement, out childrenOnlyNoContainer))
        return;
      IType nodeType = this.GetNodeType((DocumentNode) parentNode);
      ElementNode elementNode = (ElementNode) this.serializerContext.Node;
      if (canSkipCreatingElement)
      {
        this.WritePropertyElementContent(node, elementNode, elementNode, childrenOnlyNoContainer);
      }
      else
      {
        ContainerNode node1 = this.serializerContext.Node;
        ElementNode propertyElementNode = this.CreatePropertyElementNode(node, ordering, nodeType, propertyKey);
        node1.Children.Add((FormattedNode) propertyElementNode);
        using (this.serializerContext.PushNode(propertyElementNode))
        {
          if (container != null)
            this.WriteDirectiveAttributes(container);
          this.WritePropertyElementContent(node, propertyElementNode, elementNode, childrenOnlyNoContainer);
          if (container != null)
            this.WriteOtherChildNodes((XmlContainerReference) container);
        }
        if (propertyElementNode.Children.Count != 0)
          return;
        node1.Children.Remove((FormattedNode) propertyElementNode);
      }
    }

    public static bool HasAnyNonContentPropertiesSet(DocumentCompositeNode node)
    {
      switch (node.Properties.Count)
      {
        case 0:
          return false;
        case 1:
          IPropertyId index = (IPropertyId) node.Type.Metadata.DefaultContentProperty;
          if (index != null && node.Properties[index] != null)
            return false;
          break;
      }
      return true;
    }

    private IPropertyId WriteResourcesPropertyElement(DocumentCompositeNode node)
    {
      IPropertyId resourcesProperty = node.Type.Metadata.ResourcesProperty;
      if (resourcesProperty != null)
      {
        IProperty propertyKey = XamlSerializer.ResolveProperty(resourcesProperty, (DocumentNode) node);
        if (propertyKey != null && this.WritePropertyElementIfAny(node, propertyKey, int.MinValue) != null)
          return (IPropertyId) propertyKey;
      }
      return (IPropertyId) null;
    }

    private DocumentNode WritePropertyElementIfAny(DocumentCompositeNode node, IProperty propertyKey, int ordering)
    {
      INodeSourceContext containerContext;
      DocumentNode documentNode = node.GetValue((IPropertyId) propertyKey, out containerContext);
      if (documentNode != null && this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) propertyKey, documentNode) == SerializedFormat.Element)
      {
        XmlElementReference elementReference = containerContext as XmlElementReference;
        this.WritePrecedingCommentIfAny(elementReference);
        this.WritePropertyElement(node, propertyKey, elementReference, ordering, documentNode);
      }
      return documentNode;
    }

    private void WritePropertyElementContent(DocumentNode node, ElementNode element, ElementNode xmlSpaceNode, bool childrenOnlyNoContainer)
    {
      if (childrenOnlyNoContainer)
      {
        DocumentCompositeNode node1 = (DocumentCompositeNode) node;
        if (node1.Type.Metadata.IsWhitespaceSignificant)
          element.IsWhitespaceSignificant = true;
        this.WriteContentPropertyElement(node1);
        this.WriteElementChildren(node1);
      }
      else
      {
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(node);
        if (valueAsString != null)
        {
          if (XamlSerializer.IsXmlSpacePreserveRequired(this.filter, node.Type, valueAsString))
            this.EnsureXmlSpaceIsSetCorrectly(xmlSpaceNode, XmlSpace.Preserve);
          this.WriteElementContent(node, valueAsString);
        }
        else
          this.WriteElement(node);
      }
    }

    private string GetTypeName(IType type, XamlSerializer.TypeNameSerialization typeNameSerialization)
    {
      string str = type.Name;
      if (type.IsExpression && typeNameSerialization == XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix && (str.EndsWith("Extension", StringComparison.Ordinal) && str.Length > "Extension".Length))
        str = str.Substring(0, str.Length - "Extension".Length);
      return str;
    }

    private IType GetNodeType(DocumentNode node)
    {
      return node.Type;
    }

    private XmlnsPrefix GetXmlnsPrefix(IAssembly assembly, string clrNamespace)
    {
      XmlNamespace clrNamespaceUri = this.serializerContext.CreateClrNamespaceUri(assembly, clrNamespace);
      ElementNode node = (ElementNode) this.serializerContext.Node;
      XmlnsPrefix prefix1 = node.GetPrefix((IXmlNamespace) clrNamespaceUri);
      if (prefix1 != null)
        return prefix1;
      string str1;
      if (string.IsNullOrEmpty(clrNamespace))
      {
        str1 = this.PersistenceSettings.EmptyXmlnsPrefixSubstitute;
      }
      else
      {
        string str2 = (string) null;
        if (this.document.RootClassAttributes != null)
        {
          string qualifiedClassName = this.document.RootClassAttributes.QualifiedClassName;
          int length = qualifiedClassName != null ? qualifiedClassName.LastIndexOf('.') : -1;
          if (length >= 0)
            str2 = qualifiedClassName.Substring(0, length);
        }
        str1 = !clrNamespace.Equals(str2, StringComparison.InvariantCulture) ? clrNamespace.Replace('.', '_') : "local";
      }
      string prefixWorkaround = this.ProjectNamespaces.GetClrNamespacePrefixWorkaround(assembly, clrNamespace);
      if (prefixWorkaround != null)
        str1 = prefixWorkaround;
      XmlnsPrefix prefix2 = !this.IsUniqueXmlnsPrefix(node, str1) ? this.ComputeUniqueNamespacePrefix(node, str1) : XmlnsPrefix.ToPrefix(str1);
      this.AddXmlnsDeclaration(this.serializerContext.GetRootElement(), prefix2, (IXmlNamespace) clrNamespaceUri);
      return prefix2;
    }

    private XmlnsPrefix ComputeUniqueNamespacePrefix(ElementNode node, string candidatePrefix)
    {
      return XmlnsPrefix.ToPrefix(XamlSerializer.ComputeUniqueName(candidatePrefix, (Predicate<string>) (name => this.IsUniqueXmlnsPrefix(node, name))));
    }

    private bool IsUniqueXmlnsPrefix(ElementNode node, string prefix)
    {
      return node.GetNamespace(XmlnsPrefix.ToPrefix(prefix)) == null;
    }

    private static string ComputeUniqueName(string candidateName, Predicate<string> isUnique)
    {
      int num = 1;
      string str;
      while (true)
      {
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
        {
          (object) candidateName,
          (object) num
        });
        if (!isUnique(str))
          ++num;
        else
          break;
      }
      return str;
    }

    private void AddXmlnsDeclaration(ElementNode element, XmlnsPrefix prefix, IXmlNamespace xmlNamespace)
    {
      AttributeNode attributeNode = new AttributeNode((SourceContextReference) null, int.MinValue, prefix != XmlnsPrefix.EmptyPrefix ? new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix("xmlns"), prefix.Value) : new XmlnsPrefixAndName("xmlns"), xmlNamespace.Value);
      element.Attributes.Add((FormattedNode) attributeNode);
      element.AddNamespace(prefix, xmlNamespace);
      this.serializerContext.InvalidatePrefixCache();
    }

    private XmlnsPrefix GetXmlnsPrefix(IType typeId)
    {
      bool flag = this.filter.ShouldOverrideNamespaceForType(this.serializerContext, (ITypeId) typeId);
      XmlnsPrefix nearestXmlnsPrefix = this.GetNearestXmlnsPrefix((ITypeId) typeId);
      if (nearestXmlnsPrefix != null && !flag)
        return nearestXmlnsPrefix;
      IXmlNamespace xmlNamespace = typeId.XmlNamespace;
      if (xmlNamespace == null || flag)
      {
        IAssembly assembly = this.filter.GetClrAssembly(typeId) ?? (IAssembly) XamlSerializer.UnknownAssembly.Instance;
        string clrNamespace = this.filter.GetClrNamespace(typeId) ?? string.Empty;
        xmlNamespace = this.ProjectNamespaces.GetNamespace(assembly, clrNamespace);
        if (xmlNamespace == null || flag)
          return this.GetXmlnsPrefix(assembly, clrNamespace);
      }
      return this.GetXmlnsPrefix(this.filter.GetReplacementNamespace(xmlNamespace));
    }

    private XmlnsPrefix GetXmlnsPrefix(IXmlNamespace xmlNamespace)
    {
      return this.GetXmlnsPrefix((ElementNode) this.serializerContext.Node, xmlNamespace);
    }

    private XmlnsPrefix GetXmlnsPrefix(ElementNode node, IXmlNamespace xmlNamespace)
    {
      XmlnsPrefix prefix1 = node.GetPrefix(xmlNamespace);
      if (prefix1 != null)
        return prefix1;
      XmlnsPrefix prefix2 = (XmlnsPrefix) null;
      if (this.serializerContext.IsDefaultNamespace(xmlNamespace) && this.IsUniqueXmlnsPrefix(node, XmlnsPrefix.EmptyPrefix.Value))
      {
        prefix2 = XmlnsPrefix.EmptyPrefix;
      }
      else
      {
        string str = this.ProjectNamespaces.GetDefaultPrefix(xmlNamespace) ?? this.PersistenceSettings.GetDefaultXmlnsPrefix(xmlNamespace);
        if (this.IsUniqueXmlnsPrefix(node, str))
        {
          prefix2 = XmlnsPrefix.ToPrefix(str);
        }
        else
        {
          if (string.IsNullOrEmpty(str))
          {
            str = this.PersistenceSettings.EmptyXmlnsPrefixSubstitute;
            if (this.IsUniqueXmlnsPrefix(node, str))
              prefix2 = XmlnsPrefix.ToPrefix(str);
          }
          if (prefix2 == null)
            prefix2 = this.ComputeUniqueNamespacePrefix(node, str);
        }
      }
      ElementNode rootElement = this.serializerContext.GetRootElement();
      this.AddXmlnsDeclaration(rootElement, prefix2, xmlNamespace);
      if (xmlNamespace.Equals((object) XmlNamespace.DesignTimeXmlNamespace) || xmlNamespace.Equals((object) XmlNamespace.AnnotationsXmlNamespace))
        this.EnsurePrefixIsMarkedIgnorable(rootElement, prefix2.Value);
      return prefix2;
    }

    private XmlnsPrefix GetNearestXmlnsPrefix(ITypeId typeId)
    {
      if (this.PlatformMetadata.KnownTypes.Annotation.Equals((object) typeId) || this.PlatformMetadata.KnownTypes.AnnotationManager.Equals((object) typeId))
        return this.GetXmlnsPrefix((IXmlNamespace) XmlNamespace.AnnotationsXmlNamespace);
      if (this.PlatformMetadata.KnownTypes.DesignDataExtension.Equals((object) typeId) || this.PlatformMetadata.KnownTypes.DesignInstanceExtension.Equals((object) typeId))
        return this.GetXmlnsPrefix((IXmlNamespace) XmlNamespace.DesignTimeXmlNamespace);
      return this.serializerContext.GetNearestXmlnsPrefix(typeId);
    }

    private AttributeNode FindIgnorableAttribute(ElementNode element)
    {
      XmlnsPrefix xmlnsPrefix = this.GetXmlnsPrefix(element, (IXmlNamespace) XmlNamespace.CompatibilityXmlNamespace);
      AttributeNode attributeNode1 = (AttributeNode) null;
      for (int index = 0; index < element.Attributes.Count; ++index)
      {
        AttributeNode attributeNode2 = element.Attributes[index] as AttributeNode;
        if (attributeNode2 != null)
        {
          XmlnsPrefixAndName name = attributeNode2.Name;
          if (object.Equals((object) name.Prefix, (object) xmlnsPrefix) && name.TypeQualifiedName == "Ignorable")
          {
            attributeNode1 = attributeNode2;
            break;
          }
        }
      }
      return attributeNode1;
    }

    private void EnsurePrefixIsMarkedIgnorable(ElementNode element, string ignorablePrefix)
    {
      XmlnsPrefix xmlnsPrefix = this.GetXmlnsPrefix(element, (IXmlNamespace) XmlNamespace.CompatibilityXmlNamespace);
      AttributeNode ignorableAttribute = this.FindIgnorableAttribute(element);
      if (ignorableAttribute != null)
      {
        string listOfPrefixes = ignorableAttribute.Value;
        if (XmlUtilities.GetPrefixes(listOfPrefixes).Contains(ignorablePrefix))
          return;
        if (listOfPrefixes.Length != 0)
          listOfPrefixes += " ";
        string str = listOfPrefixes + ignorablePrefix;
        ignorableAttribute.Value = str;
      }
      else
      {
        AttributeNode attributeNode = new AttributeNode(new XmlnsPrefixAndName(xmlnsPrefix, "Ignorable"), ignorablePrefix);
        FormattedNode formattedNode = Enumerable.LastOrDefault<FormattedNode>((IEnumerable<FormattedNode>) element.Attributes);
        attributeNode.Ordering = formattedNode == null || formattedNode.Ordering == int.MaxValue ? int.MinValue : formattedNode.Ordering + 1;
        element.Attributes.Add((FormattedNode) attributeNode);
      }
    }

    private void EnsureXmlSpaceIsSetCorrectly(XmlSpace xmlSpace)
    {
      ElementNode element = this.serializerContext.Node as ElementNode;
      if (element == null)
        return;
      this.EnsureXmlSpaceIsSetCorrectly(element, xmlSpace);
    }

    private void EnsureXmlSpaceIsSetCorrectly(ElementNode element, XmlSpace xmlSpace)
    {
      if (xmlSpace == element.XmlSpace)
        return;
      AttributeNode xmlSpaceAttribute = element.GetXmlSpaceAttribute();
      string str = ElementNode.ToString(xmlSpace);
      if (xmlSpaceAttribute != null)
      {
        xmlSpaceAttribute.Value = str;
      }
      else
      {
        AttributeNode attributeNode = new AttributeNode(ElementNode.XmlSpaceName, str);
        element.Attributes.Add((FormattedNode) attributeNode);
      }
    }

    private XmlnsPrefixAndName GetMemberName(IType parentTypeId, IProperty propertyKey, XamlSerializer.IncludeQualifier includeQualifier, XamlSerializer.TypeNameSerialization typeNameSerialization)
    {
      if (propertyKey.DeclaringType.Metadata.NameProperty == propertyKey)
        propertyKey = this.PlatformMetadata.ResolveProperty(this.PlatformMetadata.KnownProperties.DesignTimeXName);
      if (XamlProcessingAttributes.IsProcessingAttribute(propertyKey))
        return new XmlnsPrefixAndName(this.GetXmlnsPrefix((IXmlNamespace) XmlNamespace.XamlXmlNamespace), propertyKey.Name);
      if (parentTypeId.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree.Equals((object) propertyKey))
        propertyKey = this.document.TypeResolver.ResolveProperty(this.document.TypeResolver.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree);
      if (this.filter.IsDesignTimeProperty((IPropertyId) propertyKey))
        return this.GetDesignTimePropertyName((IPropertyId) propertyKey);
      IType type;
      if (XamlSerializer.IsLocal(propertyKey, (ITypeId) parentTypeId))
      {
        if (includeQualifier == XamlSerializer.IncludeQualifier.IfNeeded)
          return new XmlnsPrefixAndName(propertyKey.Name);
        type = parentTypeId;
      }
      else
        type = propertyKey.DeclaringType;
      return new XmlnsPrefixAndName(this.GetXmlnsPrefix(type), this.GetTypeName(type, typeNameSerialization), propertyKey.Name);
    }

    private XmlnsPrefixAndName GetMemberName(IType typeId, string memberName, XamlSerializer.TypeNameSerialization typeNameSerialization)
    {
      return new XmlnsPrefixAndName(this.GetXmlnsPrefix(typeId), this.GetTypeName(typeId, typeNameSerialization), memberName);
    }

    private XmlnsPrefixAndName GetMemberName(IType typeId, XamlSerializer.TypeNameSerialization typeNameSerialization)
    {
      return new XmlnsPrefixAndName(this.GetXmlnsPrefix(typeId), this.GetTypeName(typeId, typeNameSerialization));
    }

    private XmlnsPrefixAndName GetDesignTimePropertyName(IPropertyId propertyKey)
    {
      XmlNamespace xmlNamespace = XmlNamespace.DesignTimeXmlNamespace;
      if (this.PlatformMetadata.KnownProperties.DesignTimeFreeze.Equals((object) propertyKey))
        xmlNamespace = XmlNamespace.PresentationOptionsXmlNamespace;
      return new XmlnsPrefixAndName(this.GetXmlnsPrefix((IXmlNamespace) xmlNamespace), propertyKey.Name);
    }

    private string NullMarkupExtension()
    {
      return "{" + (object) this.GetMemberName(this.document.TypeResolver.ResolveType(this.document.TypeResolver.PlatformMetadata.KnownTypes.NullExtension), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix) + "}";
    }

    private XamlSerializer.EscapedString GetValueAsString(DocumentNode node, XamlSerializer.StringFlags flags)
    {
      Type targetType = node.TargetType;
      IType type1 = node.Type;
      DocumentPrimitiveNode documentPrimitiveNode1 = node as DocumentPrimitiveNode;
      DocumentCompositeNode node1 = node as DocumentCompositeNode;
      if (documentPrimitiveNode1 != null)
      {
        IDocumentNodeValue documentNodeValue = documentPrimitiveNode1.Value;
        if (documentNodeValue == null)
          return (XamlSerializer.EscapedString) this.NullMarkupExtension();
        DocumentNodeStringValue documentNodeStringValue;
        if ((documentNodeStringValue = documentNodeValue as DocumentNodeStringValue) != null)
        {
          string str1 = documentNodeStringValue.Value;
          if (this.serializerContext.RemoveProjectRootFromUri && node.TypeResolver.PlatformMetadata.KnownTypes.Uri == type1 && str1 != null)
          {
            bool flag = false;
            try
            {
              flag = Path.IsPathRooted(str1);
            }
            catch (ArgumentException ex)
            {
            }
            if (flag)
            {
              string projectPath = this.serializerContext.TypeResolver.ProjectPath;
              if (str1.StartsWith(projectPath, StringComparison.OrdinalIgnoreCase))
                return (XamlSerializer.EscapedString) XamlSerializer.EscapeIfNecessary(str1.Substring(projectPath.Length).TrimStart(Path.DirectorySeparatorChar), flags);
            }
          }
          string valueAsString = this.filter.GetValueAsString(node);
          if (valueAsString != null)
            return new XamlSerializer.EscapedString(valueAsString, false);
          if (this.serializerContext.Culture != CultureInfo.InvariantCulture)
          {
            TypeConverter valueConverter = documentPrimitiveNode1.ValueConverter;
            string str2 = (string) null;
            try
            {
              str2 = XamlSerializer.ConvertToString(valueConverter, this.serializerContext.Culture, valueConverter.ConvertFromString(str1));
            }
            catch (Exception ex)
            {
            }
            if (str2 != null)
              return (XamlSerializer.EscapedString) XamlSerializer.EscapeIfNecessary(str2, flags);
          }
          return (XamlSerializer.EscapedString) XamlSerializer.EscapeIfNecessary(str1, flags);
        }
        DocumentNodeMemberValue documentNodeMemberValue;
        if ((documentNodeMemberValue = documentNodeValue as DocumentNodeMemberValue) != null)
        {
          IMember member = documentNodeMemberValue.Member;
          IType type2 = member as IType;
          if (type2 != null)
          {
            IPlatformMetadata platformMetadata = this.serializerContext.TypeResolver.PlatformMetadata;
            if (platformMetadata.IsSupported(this.serializerContext.TypeResolver, platformMetadata.KnownTypes.TypeExtension))
              return (XamlSerializer.EscapedString) this.GetTypeExtensionAsString(type2);
            return (XamlSerializer.EscapedString) this.GetMemberName(type2, XamlSerializer.TypeNameSerialization.NoChanges).FullName;
          }
          if (typeof (Delegate).IsAssignableFrom(targetType))
            return (XamlSerializer.EscapedString) member.Name;
          IProperty propertyKey = member as IProperty;
          XmlnsPrefixAndName xmlnsPrefixAndName;
          if (propertyKey != null && this.filter.IsDesignTimeProperty((IPropertyId) propertyKey))
          {
            xmlnsPrefixAndName = this.GetDesignTimePropertyName((IPropertyId) propertyKey);
          }
          else
          {
            bool flag = true;
            if (propertyKey != null && this.serializerContext.InStyleOrTemplate && (node.Parent != null && node.SitePropertyKey != null))
            {
              ITypeId type3 = (ITypeId) node.SitePropertyKey.PropertyType;
              if (node.TypeResolver.PlatformMetadata.KnownTypes.DependencyProperty.IsAssignableFrom(type3))
              {
                ITypeId fallbackPropertyType = this.GetFallbackPropertyType(node.Parent, (IPropertyId) node.SitePropertyKey);
                if (fallbackPropertyType != null && XamlSerializer.IsLocal(propertyKey, fallbackPropertyType))
                  flag = false;
              }
            }
            if (flag)
            {
              IType typeId = member.DeclaringType;
              IEvent @event = member as IEvent;
              if (!this.serializerContext.DocumentContext.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf) && @event != null)
              {
                if (propertyKey != null && this.PlatformMetadata.KnownTypes.FrameworkElement.Equals((object) propertyKey.DeclaringType) && propertyKey.Name == "Loaded")
                  typeId = this.serializerContext.TypeResolver.ResolveType(this.PlatformMetadata.KnownTypes.Canvas);
                else if (this.serializerContext.TargetType != null && @event.DeclaringType.NearestResolvedType.IsAssignableFrom((ITypeId) this.serializerContext.TargetType))
                  typeId = this.serializerContext.TargetType;
              }
              xmlnsPrefixAndName = this.GetMemberName(typeId, member.Name, XamlSerializer.TypeNameSerialization.NoChanges);
            }
            else
              xmlnsPrefixAndName = new XmlnsPrefixAndName(member.Name);
          }
          return (XamlSerializer.EscapedString) xmlnsPrefixAndName.FullName;
        }
      }
      else if (node1 != null)
      {
        if (this.PlatformMetadata.KnownTypes.PropertyPath.IsAssignableFrom((ITypeId) type1))
          return (XamlSerializer.EscapedString) XamlSerializer.GetPropertyPathAsString(node1, (XamlSerializer.MemberPropertyToNameCallback) (p => this.GetMemberName(p.DeclaringType, p.Name, XamlSerializer.TypeNameSerialization.NoChanges).FullName));
        if (this.PlatformMetadata.KnownTypes.SolidColorBrush.IsAssignableFrom((ITypeId) type1))
        {
          DocumentPrimitiveNode documentPrimitiveNode2 = node1.Properties[this.PlatformMetadata.KnownProperties.SolidColorBrushColor] as DocumentPrimitiveNode;
          if (documentPrimitiveNode2 != null)
            return this.GetValueAsString((DocumentNode) documentPrimitiveNode2, flags);
          return (XamlSerializer.EscapedString) "#00000000";
        }
        if (type1.IsExpression)
          return (XamlSerializer.EscapedString) this.GetMarkupExtensionAsAttributeString(node1);
        string valueAsString = this.filter.GetValueAsString(node);
        if (valueAsString != null)
          return (XamlSerializer.EscapedString) valueAsString;
      }
      return (XamlSerializer.EscapedString) ((string) null);
    }

    private static string EscapeIfNecessary(string value, XamlSerializer.StringFlags flags)
    {
      if ((flags & XamlSerializer.StringFlags.EscapeLiterals) != XamlSerializer.StringFlags.None && value.StartsWith("{", StringComparison.Ordinal))
        return "{}" + value;
      return value;
    }

    private static string ConvertToString(TypeConverter typeConverter, CultureInfo cultureInfo, object value)
    {
      try
      {
        if (typeConverter.CanConvertTo(typeof (string)))
          return typeConverter.ConvertToString((ITypeDescriptorContext) null, cultureInfo, value);
      }
      catch (Exception ex)
      {
      }
      return (string) null;
    }

    private void WriteDictionaryContents(DocumentCompositeNode node)
    {
      foreach (DocumentCompositeNode entryNode in (IEnumerable<DocumentNode>) node.Children)
        this.WriteDictionaryEntry(entryNode);
    }

    private ElementNode WriteDictionaryEntry(DocumentCompositeNode entryNode)
    {
      INodeSourceContext containerContext1;
      DocumentNode keyNode = entryNode.GetValue(entryNode.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryKey, out containerContext1);
      INodeSourceContext containerContext2;
      DocumentNode valueNode = entryNode.GetValue(entryNode.TypeResolver.PlatformMetadata.KnownProperties.DictionaryEntryValue, out containerContext2);
      return this.WriteDictionaryEntry(entryNode, containerContext1, keyNode, valueNode);
    }

    private ElementNode WriteDictionaryEntry(DocumentCompositeNode entryNode, INodeSourceContext keyContainerContext, DocumentNode keyNode, DocumentNode valueNode)
    {
      if (valueNode == null || this.filter.ShouldSerializeNode(this.serializerContext, valueNode) == SerializedFormat.DoNotSerialize)
        return (ElementNode) null;
      ContainerNode node = this.serializerContext.Node;
      XmlElementReference sourceContext1 = valueNode.SourceContext as XmlElementReference;
      int ordering = XamlSerializer.GetOrdering((XamlSourceContext) sourceContext1);
      this.WritePrecedingCommentIfAny(sourceContext1);
      ElementNode element = XamlSerializer.CreateElement((SourceContextReference) new DictionaryEntrySourceContextReference(entryNode, valueNode), ordering);
      node.Children.Add((FormattedNode) element);
      using (this.serializerContext.PushNode(element))
      {
        this.WriteElementContentAndSetName(valueNode);
        if (keyNode != null)
        {
          XamlSerializer.EscapedString valueAsString = this.GetValueAsString(keyNode, XamlSerializer.StringFlags.EscapeLiterals);
          if (valueAsString.IsNonNull)
          {
            XmlAttributeReference sourceContext2 = keyContainerContext as XmlAttributeReference;
            XmlnsPrefixAndName name = new XmlnsPrefixAndName(this.GetXmlnsPrefix((IXmlNamespace) XmlNamespace.XamlXmlNamespace), "Key");
            AttributeNode attribute = XamlSerializer.CreateAttribute(keyNode, sourceContext2, name, valueAsString);
            element.Attributes.Insert(0, (FormattedNode) attribute);
          }
        }
      }
      return element;
    }

    private void WritePropertiesAndVisualTree(DocumentCompositeNode node, IType targetType, XamlSerializer.IPropertyValueCollection propertyValues)
    {
      using (this.serializerContext.PushStyleOrTemplate((ITypeId) node.Type))
      {
        this.WritePropertyAttributes(node, propertyValues);
        using (this.serializerContext.PushTargetType(targetType))
        {
          IPropertyId propertyId = this.WriteResourcesPropertyElement(node);
          IProperty propertyKey1 = this.document.TypeResolver.ResolveProperty(node.TypeResolver.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree);
          bool flag = this.PlatformMetadata.KnownTypes.DataTemplate.IsAssignableFrom((ITypeId) node.Type) && !this.serializerContext.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf);
          if (!flag)
            this.WritePropertyElementIfAny(node, propertyKey1, int.MinValue);
          IProperty propertyKey2 = XamlSerializer.ResolveProperty(this.PlatformMetadata.KnownProperties.StyleSetters, (DocumentNode) node);
          if (propertyKey2 != null)
            this.WritePropertyElementIfAny(node, propertyKey2, int.MinValue);
          foreach (XamlSerializer.PropertyValue propertyValue in (IEnumerable<XamlSerializer.PropertyValue>) propertyValues)
          {
            IProperty propertyKey3 = propertyValue.PropertyKey;
            if (!this.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree.Equals((object) propertyKey3) && !this.PlatformMetadata.KnownProperties.StyleSetters.Equals((object) propertyKey3) && !propertyKey3.Equals((object) propertyId))
            {
              DocumentNode valueNode = propertyValue.ValueNode;
              if (this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) propertyKey3, valueNode) == SerializedFormat.Element)
              {
                XmlElementReference elementReference = propertyValue.SourceContext as XmlElementReference;
                int ordering = XamlSerializer.GetOrdering((XamlSourceContext) elementReference);
                this.WritePrecedingCommentIfAny(elementReference);
                this.WritePropertyElement(node, propertyKey3, elementReference, ordering, valueNode);
              }
            }
          }
          if (!flag)
            return;
          this.WritePropertyElementIfAny(node, propertyKey1, int.MaxValue);
        }
      }
    }

    private ITypeId GetFallbackPropertyType(DocumentCompositeNode parentNode, IPropertyId propertyKey)
    {
      if (this.PlatformMetadata.KnownProperties.TemplateBindingProperty.Equals((object) propertyKey))
        return (ITypeId) this.serializerContext.TargetType;
      IPropertyValueTypeMetadata valueTypeMetadata = parentNode.Type.Metadata as IPropertyValueTypeMetadata;
      if (valueTypeMetadata != null && valueTypeMetadata.PropertyProperty.Equals((object) propertyKey))
      {
        if (this.serializerContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsTypePropertySetterSyntax))
          return (ITypeId) this.serializerContext.TargetType;
        if (valueTypeMetadata.TargetNameProperty != null)
        {
          DocumentNode node1 = parentNode.Properties[valueTypeMetadata.TargetNameProperty];
          if (node1 == null)
            return (ITypeId) this.serializerContext.TargetType;
          XamlSerializer.EscapedString valueAsString = this.GetValueAsString(node1, XamlSerializer.StringFlags.None);
          if (valueAsString.IsNonNull)
          {
            DocumentNodeNameScope containingNameScope = parentNode.FindContainingNameScope();
            if (containingNameScope != null)
            {
              string name = valueAsString.Value;
              DocumentNode node2 = containingNameScope.FindNode(name);
              if (node2 != null)
                return (ITypeId) node2.Type;
            }
          }
        }
      }
      return (ITypeId) null;
    }

    private void WriteAttributesElementsAndChildren(DocumentCompositeNode node, XamlSerializer.IPropertyValueCollection propertyValues)
    {
      this.WritePropertyAttributes(node, propertyValues);
      this.WritePropertyElements(node, propertyValues);
      this.WriteElementChildren(node);
    }

    public static string GetPropertyPathAsString(DocumentCompositeNode node, XamlSerializer.MemberPropertyToNameCallback memberPropertyToName)
    {
      string valueAsString = DocumentPrimitiveNode.GetValueAsString(node.Properties[node.PlatformMetadata.KnownProperties.PropertyPathPath]);
      if (valueAsString == null)
        return (string) null;
      IProperty property = node.PlatformMetadata.ResolveProperty(node.PlatformMetadata.KnownProperties.PropertyPathPathParameters);
      DocumentCompositeNode documentCompositeNode = property == null ? (DocumentCompositeNode) null : node.Properties[(IPropertyId) property] as DocumentCompositeNode;
      if (documentCompositeNode == null || documentCompositeNode.Children.Count == 0)
        return valueAsString;
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      for (int index1 = 0; index1 < valueAsString.Length; ++index1)
      {
        if ((int) valueAsString[index1] == 40)
        {
          int index2 = index1 + 1;
          while (index2 < valueAsString.Length && (int) valueAsString[index2] != 41)
            ++index2;
          int result;
          if (int.TryParse(valueAsString.Substring(index1 + 1, index2 - index1 - 1), out result))
          {
            IMember valueAsMember = DocumentPrimitiveNode.GetValueAsMember(documentCompositeNode.Children[result]);
            stringBuilder.Append(valueAsString.Substring(startIndex, index1 - startIndex + 1));
            stringBuilder.Append(memberPropertyToName((IProperty) valueAsMember));
            stringBuilder.Append(')');
            index1 = index2;
            startIndex = index2 + 1;
          }
        }
      }
      if (startIndex < valueAsString.Length)
        stringBuilder.Append(valueAsString.Substring(startIndex));
      return stringBuilder.ToString();
    }

    private string GetMarkupExtensionAsAttributeString(DocumentCompositeNode node)
    {
      IType type = node.Type;
      StringBuilder stringBuilder = new StringBuilder("{");
      stringBuilder.Append(this.GetMemberName(type, XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix).FullName);
      int num = 0;
      IConstructorArgumentNodeCollection constructorArguments;
      IConstructor bestConstructor = node.GetBestConstructor(out constructorArguments);
      IProperty[] array = (IProperty[]) null;
      if (bestConstructor != null && constructorArguments.Count > 0)
      {
        array = new IProperty[constructorArguments.Count];
        IConstructorArgumentProperties argumentProperties = type.GetConstructorArgumentProperties();
        for (int index = 0; index < constructorArguments.Count; ++index)
        {
          IParameter parameter = bestConstructor.Parameters[index];
          IProperty propertyKey = argumentProperties[parameter.Name];
          array[index] = propertyKey;
          DocumentNode documentNode = constructorArguments[index];
          string str1;
          if (documentNode != null)
          {
            str1 = this.GetValueAsString(documentNode, XamlSerializer.StringFlags.None).Value;
          }
          else
          {
            CultureInfo culture = this.serializerContext.Culture;
            str1 = propertyKey == null ? this.GetDefaultValueAsString(parameter.ParameterType) : this.GetDefaultValueAsString(type, propertyKey);
          }
          stringBuilder.Append(index > 0 ? ", " : " ");
          if (str1 != null)
          {
            string str2 = this.EscapeMarkupExtensionValueIfNecessary(documentNode, str1);
            stringBuilder.Append(str2);
          }
        }
        num = bestConstructor.Parameters.Count;
      }
      else if (this.PlatformMetadata.KnownTypes.Binding.Equals((object) type))
      {
        IProperty property = node.TypeResolver.ResolveProperty(this.PlatformMetadata.KnownProperties.BindingPath);
        DocumentNode documentNode = node.Properties[(IPropertyId) property];
        if (documentNode != null)
        {
          array = new IProperty[1]
          {
            property
          };
          stringBuilder.Append(" " + this.EscapeMarkupExtensionValueIfNecessary(documentNode, this.GetValueAsString(documentNode, XamlSerializer.StringFlags.None).Value));
          num = 1;
        }
      }
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) node.Properties)
      {
        IProperty key = keyValuePair.Key;
        if (array == null || Array.IndexOf<IProperty>(array, key) < 0)
        {
          DocumentNode documentNode = keyValuePair.Value;
          if (this.filter.ShouldSerializeProperty(this.serializerContext, node, (IPropertyId) key, documentNode) != SerializedFormat.DoNotSerialize)
          {
            string str1 = this.GetValueAsString(documentNode, XamlSerializer.StringFlags.None).Value;
            if (str1 != null)
            {
              string str2 = this.EscapeMarkupExtensionValueIfNecessary(documentNode, str1);
              stringBuilder.Append(num > 0 ? ", " : " ");
              XmlnsPrefixAndName memberName = this.GetMemberName(type, key, XamlSerializer.IncludeQualifier.IfNeeded, XamlSerializer.TypeNameSerialization.NoChanges);
              stringBuilder.Append(memberName.FullName);
              stringBuilder.Append("=");
              stringBuilder.Append(str2);
              ++num;
            }
          }
        }
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    private string EscapeMarkupExtensionValueIfNecessary(DocumentNode valueNode, string value)
    {
      if (valueNode != null && (DocumentPrimitiveNode.IsNull(valueNode) || valueNode.TypeResolver.PlatformMetadata.KnownTypes.Type.IsAssignableFrom((ITypeId) valueNode.Type) || DocumentNodeHelper.IsMarkupExtension(valueNode)))
        return value;
      StringBuilder stringBuilder = (StringBuilder) null;
      for (int length = 0; length < value.Length; ++length)
      {
        char ch = value[length];
        switch (ch)
        {
          case '=':
          case '\\':
          case '{':
          case '}':
          case '"':
          case '\'':
          case ',':
            if (stringBuilder == null)
              stringBuilder = new StringBuilder(value.Substring(0, length));
            stringBuilder.Append('\\');
            break;
        }
        if (stringBuilder != null)
          stringBuilder.Append(ch);
      }
      if (stringBuilder != null)
        value = stringBuilder.ToString();
      if (value.Length != 0 && !Scanner.IsXmlWhitespace(value[0]) && !Scanner.IsXmlWhitespace(value[value.Length - 1]))
        return value;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{0}", new object[2]
      {
        (object) this.serializerContext.Settings.MarkupExtensionQuoteCharacter,
        (object) value
      });
    }

    private string GetDefaultValueAsString(IType targetTypeId, IProperty propertyKey)
    {
      Type runtimeType = targetTypeId.NearestResolvedType.RuntimeType;
      if (!propertyKey.HasDefaultValue(runtimeType))
        return this.GetDefaultValueAsString(propertyKey.PropertyType);
      object defaultValue = propertyKey.GetDefaultValue(runtimeType);
      if (defaultValue == null)
        return this.NullMarkupExtension();
      return XamlSerializer.ConvertToString(propertyKey.TypeConverter, this.serializerContext.Culture, defaultValue);
    }

    private string GetDefaultValueAsString(IType typeId)
    {
      if (typeId.IsAssignableFrom((ITypeId) typeId.PlatformMetadata.ResolveType(typeId.PlatformMetadata.KnownTypes.String)))
        return string.Empty;
      Type runtimeType = typeId.NearestResolvedType.RuntimeType;
      if (!runtimeType.IsValueType)
        return this.NullMarkupExtension();
      bool supportInternal = this.serializerContext.TypeResolver.InTargetAssembly(typeId);
      ConstructorInfo defaultConstructor = XamlSerializer.GetDefaultConstructor(runtimeType, supportInternal);
      if (defaultConstructor != (ConstructorInfo) null)
      {
        try
        {
          object obj = defaultConstructor.Invoke((object[]) null);
          return XamlSerializer.ConvertToString(typeId.PlatformMetadata.GetTypeConverter((MemberInfo) runtimeType), this.serializerContext.Culture, obj);
        }
        catch (Exception ex)
        {
        }
      }
      return (string) null;
    }

    private string GetTypeExtensionAsString(IType type)
    {
      StringBuilder stringBuilder = new StringBuilder("{");
      stringBuilder.Append(this.GetMemberName(type.PlatformMetadata.ResolveType(type.PlatformMetadata.KnownTypes.TypeExtension), XamlSerializer.TypeNameSerialization.RemoveExtensionSuffix).FullName);
      stringBuilder.Append(" ");
      XmlnsPrefixAndName memberName = this.GetMemberName(type, XamlSerializer.TypeNameSerialization.NoChanges);
      stringBuilder.Append(memberName.FullName);
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    private static int GetOrdering(XamlSourceContext xamlContext)
    {
      if (xamlContext != null && xamlContext.TextRange != null)
        return xamlContext.Ordering;
      return int.MaxValue;
    }

    private static int GetOrdering(XamlSourceContext xamlContext, int? requiredOrdering)
    {
      if (requiredOrdering.HasValue)
        return requiredOrdering.Value;
      return XamlSerializer.GetOrdering(xamlContext);
    }

    private static AttributeNode CreateAttribute(DocumentNode documentNode, XmlAttributeReference sourceContext, XmlnsPrefixAndName name, XamlSerializer.EscapedString value)
    {
      int ordering = XamlSerializer.GetOrdering((XamlSourceContext) sourceContext);
      if (ordering == int.MaxValue && documentNode != null && (documentNode.Parent.IsNameProperty((IPropertyId) documentNode.SitePropertyKey) || documentNode.PlatformMetadata.KnownProperties.DictionaryEntryKey.Equals((object) documentNode.SitePropertyKey)))
        ordering = int.MinValue;
      return XamlSerializer.CreateAttribute(documentNode, ordering, name, value);
    }

    private static AttributeNode CreateAttribute(DocumentNode documentNode, int ordering, XmlnsPrefixAndName name, XamlSerializer.EscapedString value)
    {
      return new AttributeNode(documentNode == null ? (SourceContextReference) null : (documentNode.SourceContext != null ? (SourceContextReference) new SelfSourceContextReference(documentNode) : (SourceContextReference) new ContainerSourceContextReference(documentNode)), ordering, name, value.Value, value.EscapeWhitespace);
    }

    private static ElementNode CreateElement(SourceContextReference sourceContextReference, int ordering)
    {
      return new ElementNode(sourceContextReference, ordering);
    }

    private static bool IsLocal(IProperty propertyKey, ITypeId targetTypeId)
    {
      if (propertyKey.IsAttachable && !(propertyKey is IEvent))
        return false;
      return propertyKey.DeclaringType.IsAssignableFrom(targetTypeId);
    }

    private static bool IsXmlSpacePreserveRequired(IXamlSerializerFilter filter, IType typeId, string content)
    {
      Type runtimeType = typeId.NearestResolvedType.RuntimeType;
      if (filter.IsXmlSpacePreserveIgnored(typeId))
        return false;
      bool flag1 = false;
      for (int index = 0; index < content.Length; ++index)
      {
        char c = content[index];
        bool flag2 = Scanner.IsXmlWhitespace(c);
        if (flag2 && (index == 0 || flag1 || (int) c != 32))
          return true;
        flag1 = flag2;
      }
      return flag1;
    }

    private static XamlSerializer.IPropertyValueCollection GetSortedProperties(DocumentCompositeNode node)
    {
      if (node.Properties.Count > 0)
        return (XamlSerializer.IPropertyValueCollection) new XamlSerializer.PropertyValueCollection(node);
      return (XamlSerializer.IPropertyValueCollection) XamlSerializer.EmptyPropertyValueCollection.Instance;
    }

    private static IProperty ResolveProperty(IPropertyId propertyKey, DocumentNode node)
    {
      if (propertyKey == null)
        return (IProperty) null;
      return node.TypeResolver.ResolveProperty(propertyKey);
    }

    private static ConstructorInfo GetDefaultConstructor(Type type, bool supportInternal)
    {
      if ((type.IsNested && (type.IsNestedPublic || type.IsNestedAssembly && supportInternal) || (type.IsPublic || type.IsNotPublic && supportInternal)) && (!type.IsInterface && !type.IsAbstract && (!type.ContainsGenericParameters && !type.IsValueType)))
      {
        ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
        if (constructor != (ConstructorInfo) null && (constructor.IsPublic || constructor.IsAssembly && supportInternal))
          return constructor;
      }
      return (ConstructorInfo) null;
    }

    private struct NodeFormatting
    {
      private readonly DocumentNode node;
      private readonly SerializedFormat format;

      public DocumentNode Node
      {
        get
        {
          return this.node;
        }
      }

      public SerializedFormat Format
      {
        get
        {
          return this.format;
        }
      }

      public int Ordering
      {
        get
        {
          return XamlSerializer.GetOrdering(this.node.SourceContext as XamlSourceContext);
        }
      }

      public NodeFormatting(DocumentNode node, SerializedFormat format)
      {
        this.node = node;
        this.format = format;
      }
    }

    private enum TypeNameSerialization
    {
      NoChanges,
      RemoveExtensionSuffix,
    }

    private enum IncludeQualifier
    {
      Always,
      IfNeeded,
    }

    [Flags]
    private enum StringFlags
    {
      None = 0,
      EscapeLiterals = 1,
    }

    private struct EscapedString
    {
      private readonly string value;
      private readonly bool escapeWhitespace;

      public bool IsNonNull
      {
        get
        {
          return this.value != null;
        }
      }

      public string Value
      {
        get
        {
          return this.value;
        }
      }

      public bool EscapeWhitespace
      {
        get
        {
          return this.escapeWhitespace;
        }
      }

      public EscapedString(string value, bool escapeWhitespace)
      {
        this.value = value;
        this.escapeWhitespace = escapeWhitespace;
      }

      public static implicit operator XamlSerializer.EscapedString(string value)
      {
        return new XamlSerializer.EscapedString(value, true);
      }
    }

    public delegate string MemberPropertyToNameCallback(IProperty property);

    private struct PropertyValue
    {
      private readonly XamlSourceContext sourceContext;
      private readonly IProperty propertyKey;
      private readonly DocumentNode valueNode;

      public XamlSourceContext SourceContext
      {
        get
        {
          return this.sourceContext;
        }
      }

      public IProperty PropertyKey
      {
        get
        {
          return this.propertyKey;
        }
      }

      public DocumentNode ValueNode
      {
        get
        {
          return this.valueNode;
        }
      }

      public PropertyValue(XamlSourceContext sourceContext, IProperty propertyKey, DocumentNode valueNode)
      {
        this.sourceContext = sourceContext;
        this.propertyKey = propertyKey;
        this.valueNode = valueNode;
      }
    }

    private interface IPropertyValueCollection : IEnumerable<XamlSerializer.PropertyValue>, IEnumerable
    {
      int Count { get; }
    }

    private sealed class EmptyPropertyValueCollection : XamlSerializer.IPropertyValueCollection, IEnumerable<XamlSerializer.PropertyValue>, IEnumerable
    {
      public static XamlSerializer.EmptyPropertyValueCollection Instance = new XamlSerializer.EmptyPropertyValueCollection();
      private static ReadOnlyCollection<XamlSerializer.PropertyValue> emptyPropertyValueCollection = new ReadOnlyCollection<XamlSerializer.PropertyValue>((IList<XamlSerializer.PropertyValue>) new List<XamlSerializer.PropertyValue>());

      public int Count
      {
        get
        {
          return 0;
        }
      }

      private EmptyPropertyValueCollection()
      {
      }

      public IEnumerator<XamlSerializer.PropertyValue> GetEnumerator()
      {
        return XamlSerializer.EmptyPropertyValueCollection.emptyPropertyValueCollection.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }
    }

    private sealed class PropertyValueCollection : XamlSerializer.IPropertyValueCollection, IEnumerable<XamlSerializer.PropertyValue>, IEnumerable
    {
      private readonly DocumentCompositeNode node;
      private readonly List<XamlSerializer.PropertyValue> propertyValues;

      public int Count
      {
        get
        {
          return this.propertyValues.Count;
        }
      }

      public PropertyValueCollection(DocumentCompositeNode node)
      {
        this.node = node;
        this.propertyValues = new List<XamlSerializer.PropertyValue>(this.node.Properties.Count);
        foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) this.node.Properties)
        {
          IProperty key = keyValuePair.Key;
          this.propertyValues.Add(new XamlSerializer.PropertyValue(this.node.GetContainerContext((IPropertyId) key) as XamlSourceContext, key, keyValuePair.Value));
        }
        this.propertyValues.Sort(new Comparison<XamlSerializer.PropertyValue>(XamlSerializer.PropertyValueCollection.Compare));
      }

      public IEnumerator<XamlSerializer.PropertyValue> GetEnumerator()
      {
        return (IEnumerator<XamlSerializer.PropertyValue>) this.propertyValues.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      private static int Compare(XamlSerializer.PropertyValue valueA, XamlSerializer.PropertyValue valueB)
      {
        IProperty propertyKey1 = valueA.PropertyKey;
        IProperty propertyKey2 = valueB.PropertyKey;
        if (propertyKey1 == propertyKey2)
          return 0;
        XamlSourceContext sourceContext1 = valueA.SourceContext;
        XamlSourceContext sourceContext2 = valueB.SourceContext;
        if (sourceContext1 != null)
        {
          if (sourceContext2 != null)
          {
            int ordering1 = XamlSerializer.GetOrdering(sourceContext1);
            int ordering2 = XamlSerializer.GetOrdering(sourceContext2);
            if (ordering1 != ordering2)
              return ordering1 - ordering2;
          }
          return -1;
        }
        if (sourceContext2 != null)
          return 1;
        return propertyKey1.SortValue - propertyKey2.SortValue;
      }
    }

    private class UnknownAssembly : IAssembly, IAssemblyId
    {
      public static XamlSerializer.UnknownAssembly Instance = new XamlSerializer.UnknownAssembly();

      public bool IsLoaded
      {
        get
        {
          return false;
        }
      }

      public bool IsDynamic
      {
        get
        {
          return false;
        }
      }

      public bool IsResolvedImplicitAssembly
      {
        get
        {
          return false;
        }
      }

      public string Name
      {
        get
        {
          return "Unknown";
        }
      }

      public Version Version
      {
        get
        {
          return (Version) null;
        }
      }

      public string FullName
      {
        get
        {
          return string.Empty;
        }
      }

      public string Location
      {
        get
        {
          return string.Empty;
        }
      }

      public bool GlobalAssemblyCache
      {
        get
        {
          return false;
        }
      }

      public string ManifestModule
      {
        get
        {
          return string.Empty;
        }
      }

      public byte[] GetPublicKeyToken()
      {
        return (byte[]) null;
      }

      public Stream GetManifestResourceStream(string name)
      {
        return (Stream) null;
      }

      public bool CompareTo(IAssembly assembly)
      {
        return false;
      }

      public bool CompareTo(Assembly assembly)
      {
        return false;
      }

      public override bool Equals(object obj)
      {
        if (this == obj)
          return true;
        IAssembly assembly = obj as IAssembly;
        if (assembly != null)
          return assembly.Name == this.Name;
        return false;
      }

      public override int GetHashCode()
      {
        return this.Name.GetHashCode();
      }

      public override string ToString()
      {
        return this.Name;
      }
    }
  }
}
