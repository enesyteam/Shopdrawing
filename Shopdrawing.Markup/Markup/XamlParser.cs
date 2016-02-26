// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlParser
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class XamlParser
  {
    private static ReadOnlyCollection<DocumentNode> emptyDocumentNodeCollection = new ReadOnlyCollection<DocumentNode>((IList<DocumentNode>) new List<DocumentNode>());
    private readonly IDocumentContext documentContext;
    private readonly IReadableSelectableTextBuffer textBuffer;
    private readonly ITypeId expectedRootType;
    private readonly XamlParserPolicy parserPolicy;

    public static event EventHandler Parsing;

    public static event EventHandler Parsed;

    public static event EventHandler XmlParsing;

    public static event EventHandler XmlParsed;

    public XamlParser(IDocumentContext documentContext, IReadableSelectableTextBuffer textBuffer, ITypeId expectedRootType)
    {
      this.documentContext = documentContext;
      this.textBuffer = textBuffer;
      this.expectedRootType = (ITypeId) documentContext.TypeResolver.ResolveType(expectedRootType);
      this.parserPolicy = new XamlParserPolicy();
    }

    public XamlParserResults Parse()
    {
      return this.Parse((ClassAttributes) null);
    }

    public XamlParserResults Parse(ClassAttributes rootAttributes)
    {
      return this.ParseInternal(new XamlParserContext(this.documentContext, rootAttributes));
    }

    private bool ProcessXmlErrors(XmlDocument xmlDocument, int offset, ErrorNodeList errorNodes, out XamlParserResults xamlParserResults)
    {
      if (errorNodes.Length > 0)
      {
        IList<XamlParseError> errors = (IList<XamlParseError>) new List<XamlParseError>();
        for (int index = 0; index < errorNodes.Length; ++index)
          errors.Add(XamlParseErrors.XmlError(errorNodes[index], this.textBuffer, offset));
        xamlParserResults = new XamlParserResults(this.expectedRootType, (DocumentNode) null, errors, (INodeSourceContext) new XmlDocumentReference(this.textBuffer, xmlDocument, true), this.textBuffer);
        return false;
      }
      xamlParserResults = (XamlParserResults) null;
      return true;
    }

    private bool ParseXml(ITextRange textRange, out XmlDocument xmlDocument, out XamlParserResults xamlParserResults)
    {
      if (XamlParser.XmlParsing != null)
        XamlParser.XmlParsing((object) this, EventArgs.Empty);
      ErrorNodeList errorNodeList = new ErrorNodeList();
      try
      {
        Compiler compiler = new Compiler();
        xmlDocument = compiler.Compile(this.textBuffer.GetText(textRange.Offset, textRange.Length), errorNodeList);
      }
      finally
      {
        if (XamlParser.XmlParsed != null)
          XamlParser.XmlParsed((object) this, EventArgs.Empty);
      }
      return this.ProcessXmlErrors(xmlDocument, textRange.Offset, errorNodeList, out xamlParserResults);
    }

    private XamlParserResults ParseInternal(XamlParserContext parserContext)
    {
      XmlDocument xmlDocument;
      XamlParserResults xamlParserResults;
      if (!this.ParseXml((ITextRange) new TextRange(0, this.textBuffer.Length), out xmlDocument, out xamlParserResults))
        return xamlParserResults;
      if (XamlParser.Parsing != null)
        XamlParser.Parsing((object) this, EventArgs.Empty);
      this.FixupXmlTree((Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlDocument, 0);
      XmlDocumentReference xmlDocumentReference = new XmlDocumentReference(this.textBuffer, xmlDocument, true);
      DocumentNode rootNode;
      try
      {
        rootNode = this.Parse(parserContext, xmlDocumentReference, xmlDocument);
      }
      finally
      {
        if (XamlParser.Parsed != null)
          XamlParser.Parsed((object) this, EventArgs.Empty);
      }
      return new XamlParserResults(this.expectedRootType, rootNode, parserContext.Errors, (INodeSourceContext) xmlDocumentReference, this.textBuffer);
    }

    public static DocumentNode ParseRegion(IReadableSelectableTextBuffer textBuffer, DocumentNode parentNode, DocumentNode oldNode, ITextRange span, out IList<XamlParseError> errors)
    {
      IDocumentRoot documentRoot = parentNode.DocumentRoot;
      IDocumentContext documentContext = documentRoot.DocumentContext;
      ICollection<DocumentNode> ancestorNodes;
      IXmlNamespaceResolver xmlNamespaceResolver;
      IDocumentNodeReference nodeReference;
      XamlParserContext context = XamlParser.CreateContext(documentRoot, parentNode, out ancestorNodes, out xmlNamespaceResolver, out nodeReference);
      span = TextBufferHelper.ExpandSpanToFillWhitespace((IReadableTextBuffer) textBuffer, span, true, true);
      XamlParser xamlParser = new XamlParser(documentContext, textBuffer, context.PlatformMetadata.KnownTypes.Object);
      XmlDocument xmlDocument;
      XamlParserResults xamlParserResults;
      if (!xamlParser.ParseXml(span, out xmlDocument, out xamlParserResults))
      {
        errors = xamlParserResults.Errors;
        return (DocumentNode) null;
      }
      xmlDocument.SourceContext.Document.Offset = -span.Offset;
      xamlParser.FixupXmlTree((Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlDocument, span.Offset);
      if (!xmlDocument.HasChildNodes)
      {
        errors = (IList<XamlParseError>) new List<XamlParseError>();
        return (DocumentNode) null;
      }
      XmlDocumentReference documentReference = new XmlDocumentReference(xamlParser.textBuffer, xmlDocument, true);
      XmlElement xmlElement1 = (XmlElement) null;
      Microsoft.Expression.DesignModel.Markup.Xml.Node node1 = xmlDocument.FirstChild;
      while (node1 != null && (xmlElement1 = node1 as XmlElement) == null)
        node1 = node1.NextNode;
      if (xmlElement1 == null)
      {
        errors = (IList<XamlParseError>) new List<XamlParseError>();
        return (DocumentNode) null;
      }
      xmlDocument.RemoveChild((Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlElement1);
      XmlElement xmlElement2 = new XmlElement(xmlDocument);
      xmlElement2.Parent = (XmlNode) xmlDocument;
      xmlElement2.AddChild((Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlElement1);
      XmlElementReference elementReference1 = new XmlElementReference((XmlContainerReference) documentReference, xmlElement2, false);
      bool flag = false;
      foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) ancestorNodes)
      {
        XmlElementReference elementReference2 = documentNode.SourceContext as XmlElementReference;
        if (elementReference2 != null)
        {
          foreach (XmlElementReference.Attribute attribute in elementReference2.AttributesToPreserve)
          {
            XmlAttribute xmlAttribute = attribute.XmlAttribute;
            if (XmlUtilities.IsXmlnsDeclaration(xmlAttribute))
            {
              if (!(xmlAttribute.Prefix == "xmlns"))
              {
                string str = string.Empty;
              }
              else
              {
                string localName = xmlAttribute.LocalName;
              }
              elementReference1.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.Xmlns, xmlAttribute, (XamlSourceContext) null));
            }
          }
          foreach (XmlNamespace xmlNamespace in elementReference2.IgnorableNamespaces)
            elementReference1.AddIgnorableNamespace(xmlNamespace);
          if (!flag && documentNode.NameScope != null)
          {
            flag = true;
            foreach (KeyValuePair<string, DocumentNode> keyValuePair in documentNode.NameScope.GetNames())
            {
              if (oldNode != keyValuePair.Value && !oldNode.Marker.Contains(keyValuePair.Value.Marker))
                nodeReference.NameScope.AddNode(keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
      }
      XmlElementReference xmlElementReference = xamlParser.NewXmlElementReference(context, (XmlContainerReference) elementReference1, xmlElement1);
      DocumentNode node2;
      int num = (int) xamlParser.ParseElement(context, nodeReference, xmlElementReference, xmlElement1, out node2);
      xamlParser.ReportUnnecessaryAttributes(context, nodeReference, xmlElementReference);
      if (node2 != null && typeof (DictionaryEntry).IsAssignableFrom(oldNode.TargetType))
      {
        node2 = xamlParser.ParseDictionaryEntry(context, nodeReference, xamlParser.NewXmlElementReference(context, (XmlContainerReference) elementReference1, xmlElement1), node2);
        if (node2 != null)
        {
          DocumentCompositeNode documentCompositeNode = parentNode as DocumentCompositeNode;
          if (documentCompositeNode != null)
          {
            Dictionary<DocumentNode, bool> dictionary = new Dictionary<DocumentNode, bool>(documentCompositeNode.Children.Count, (IEqualityComparer<DocumentNode>) DocumentNodeEqualityComparer.Instance);
            for (int index = 0; index <= documentCompositeNode.Children.Count; ++index)
            {
              DocumentNode documentNode;
              if (index == documentCompositeNode.Children.Count)
              {
                documentNode = node2;
              }
              else
              {
                documentNode = documentCompositeNode.Children[index];
                if (documentNode == oldNode)
                  continue;
              }
              DocumentCompositeNode entryNode = documentNode as DocumentCompositeNode;
              if (entryNode != null)
              {
                DocumentNode resourceEntryKey = DocumentNodeHelper.GetResourceEntryKey(entryNode);
                if (resourceEntryKey != null)
                {
                  if (dictionary.ContainsKey(resourceEntryKey))
                  {
                    XamlSourceContext xamlSourceContext = entryNode.SourceContext as XamlSourceContext;
                    context.ReportError(XamlParseErrors.RepeatedKey(xamlSourceContext.LineInformation));
                  }
                  else
                    dictionary.Add(resourceEntryKey, true);
                }
              }
            }
          }
        }
      }
      errors = context.Errors;
      return node2;
    }

    internal static XamlParserContext CreateContext(IDocumentRoot document, DocumentNode parentNode, out ICollection<DocumentNode> ancestorNodes, out IXmlNamespaceResolver xmlNamespaceResolver, out IDocumentNodeReference nodeReference)
    {
      List<DocumentNode> list = new List<DocumentNode>(2);
      list.Add(parentNode);
      list.AddRange(parentNode.AncestorNodes);
      ancestorNodes = (ICollection<DocumentNode>) list;
      xmlNamespaceResolver = (IXmlNamespaceResolver) DefaultXmlNamespaceResolver.Instance;
      nodeReference = (IDocumentNodeReference) new DocumentRootReference(document.RootNode.Type);
      for (int index = ancestorNodes.Count - 1; index >= 0; --index)
      {
        DocumentCompositeNode node = (DocumentCompositeNode) list[index];
        DocumentCompositeNodeReference compositeNodeReference = new DocumentCompositeNodeReference(nodeReference, node);
        nodeReference = (IDocumentNodeReference) compositeNodeReference;
        if (DocumentNodeHelper.IsStyleOrTemplate(node.Type))
        {
          IType templateTargetType = DocumentNodeHelper.GetStyleOrTemplateTargetType((DocumentNode) node);
          compositeNodeReference.SetTargetType(templateTargetType);
        }
        XmlElementReference elementReference = node.SourceContext as XmlElementReference;
        if (elementReference != null)
          xmlNamespaceResolver = (IXmlNamespaceResolver) elementReference;
      }
      return new XamlParserContext(document.DocumentContext, document.RootClassAttributes);
    }

    public static DocumentNode ParseValue(IDocumentRoot document, DocumentNode parentNode, IType valueType, string text, out IList<XamlParseError> errors)
    {
      ICollection<DocumentNode> ancestorNodes;
      IXmlNamespaceResolver xmlNamespaceResolver;
      IDocumentNodeReference nodeReference;
      XamlParserContext context = XamlParser.CreateContext(document, parentNode, out ancestorNodes, out xmlNamespaceResolver, out nodeReference);
      DocumentNode documentNode = MarkupExtensionParser.ParseMarkupExtension(context, (ITextLocation) null, nodeReference, xmlNamespaceResolver, valueType, text);
      errors = context.Errors;
      return documentNode;
    }

    private static string GetXmlnsPrefix(XmlAttribute attribute)
    {
      if (!(attribute.Prefix == "xmlns"))
        return string.Empty;
      return attribute.LocalName;
    }

    private void FixupXmlTree(Microsoft.Expression.DesignModel.Markup.Xml.Node node, int offset)
    {
      node.SourceContext.StartCol += offset;
      node.SourceContext.EndCol += offset;
      XmlElement element = node as XmlElement;
      if (element != null)
      {
        element.startTagContext.StartCol += offset;
        element.startTagContext.EndCol += offset;
        element.endTagContext.StartCol += offset;
        element.endTagContext.EndCol += offset;
        element.scope = new XmlNamespaceScope(element.Parent != null ? element.Parent.scope : (XmlNamespaceScope) null);
        foreach (XmlAttribute attribute in XmlAttributeEnumerable.All(element))
        {
          if (XmlUtilities.IsXmlnsDeclaration(attribute))
          {
            string xmlnsPrefix = XamlParser.GetXmlnsPrefix(attribute);
            element.scope.AddNamespace(xmlnsPrefix, attribute.LiteralValue);
          }
          this.FixupXmlTree((Microsoft.Expression.DesignModel.Markup.Xml.Node) attribute, offset);
        }
      }
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node1 = node.FirstChild; node1 != null; node1 = node1.NextNode)
        this.FixupXmlTree(node1, offset);
    }

    private DocumentNode Parse(XamlParserContext parserContext, XmlDocumentReference xmlDocumentReference, XmlDocument xmlDocument)
    {
      int num = 0;
      this.ParsePreservedChildNodes((XmlContainerReference) xmlDocumentReference, (XmlNode) xmlDocument);
      DocumentNode documentNode1 = (DocumentNode) null;
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node = xmlDocument.FirstChild; node != null; node = node.NextNode)
      {
        if (node.NodeType == NodeType.XmlElement)
        {
          DocumentNode documentNode2 = this.ParseRootElement(parserContext, this.NewXmlElementReference(parserContext, (XmlContainerReference) xmlDocumentReference, (XmlElement) node), (XmlElement) node);
          if (num == 0)
            documentNode1 = documentNode2;
          else if (num == 1)
            parserContext.ReportError(XamlParseErrors.MultipleRootElements(this.GetLineInformation(node.SourceContext)));
          ++num;
        }
      }
      if (num == 0)
      {
        SourceContext sourceContext = xmlDocument.SourceContext;
        parserContext.ReportError(XamlParseErrors.NoRootElement(this.textBuffer.GetLocation(Math.Max(0, this.textBuffer.Length - 1))));
      }
      return documentNode1;
    }

    private DocumentNode ParseRootElement(XamlParserContext parserContext, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      IType typeId = this.GetTypeId(parserContext, xmlElementReference, xmlElement);
      if (typeId == null)
        return (DocumentNode) null;
      if (typeId.IsResolvable && !this.expectedRootType.IsAssignableFrom((ITypeId) typeId))
        parserContext.ReportError(XamlParseErrors.UnexpectedRootType(xmlElementReference.LineInformation, this.expectedRootType));
      XmlAttribute attribute1 = (XmlAttribute) null;
      XmlAttribute attribute2 = (XmlAttribute) null;
      XmlAttribute attribute3 = (XmlAttribute) null;
      foreach (XmlAttribute xmlAttribute in XmlAttributeEnumerable.NotXmlnsNorDirective(xmlElement))
      {
        string prefix = xmlAttribute.Prefix;
        if (!string.IsNullOrEmpty(prefix) && parserContext.GetXmlNamespace(this.GetLineInformation(xmlAttribute.SourceContext), (IXmlNamespaceResolver) xmlElementReference, XmlnsPrefix.ToPrefix(prefix)) == XmlNamespace.XamlXmlNamespace)
        {
          switch (xmlAttribute.LocalName)
          {
            case "Class":
              attribute1 = xmlAttribute;
              continue;
            case "Subclass":
              attribute2 = xmlAttribute;
              continue;
            case "ClassModifier":
              attribute3 = xmlAttribute;
              continue;
            default:
              continue;
          }
        }
      }
      IAssembly projectAssembly = parserContext.TypeResolver.ProjectAssembly;
      string str = (string) null;
      if (attribute1 != null)
      {
        ITextLocation lineInformation = this.GetLineInformation(attribute1.SourceContext);
        str = XmlUtilities.GetAttributeValue(attribute1);
        if (!XamlParser.IsValidTypeName(str))
          parserContext.ReportError(XamlParseErrors.InvalidClassName(lineInformation, str));
      }
      string typeName = (string) null;
      if (attribute2 != null)
      {
        ITextLocation lineInformation = this.GetLineInformation(attribute2.SourceContext);
        typeName = XmlUtilities.GetAttributeValue(attribute2);
        if (!XamlParser.IsValidTypeName(typeName))
          parserContext.ReportError(XamlParseErrors.InvalidClassName(lineInformation, typeName));
        if (attribute1 == null)
          parserContext.ReportError(XamlParseErrors.AttributeRequiresExplicitClassAttribute(lineInformation, "Subclass"));
      }
      string generatedClassModifier = (string) null;
      if (attribute3 != null)
      {
        ITextLocation lineInformation = this.GetLineInformation(attribute3.SourceContext);
        generatedClassModifier = XmlUtilities.GetAttributeValue(attribute3);
        if (attribute1 == null)
          parserContext.ReportError(XamlParseErrors.AttributeRequiresExplicitClassAttribute(lineInformation, "ClassModifier"));
      }
      ClassAttributes classAttributes = (ClassAttributes) null;
      if (projectAssembly != null && str != null)
      {
        string codeBehindClassName = typeName ?? str;
        string rootNamespace = parserContext.TypeResolver.RootNamespace;
        string qualifiedClassName = string.IsNullOrEmpty(rootNamespace) || !parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsXClassRootNamespace) ? codeBehindClassName : rootNamespace + "." + codeBehindClassName;
        classAttributes = new ClassAttributes(codeBehindClassName, str, generatedClassModifier, qualifiedClassName);
      }
      if (classAttributes != null)
        parserContext.RootClassAttributes = classAttributes;
      IDocumentNodeReference documentNodeReference = (IDocumentNodeReference) new DocumentRootReference(typeId);
      DocumentNode documentNode = this.ParseElementContent(parserContext, documentNodeReference, typeId, xmlElementReference, xmlElement);
      if (documentNode != null)
      {
        XmlElementReference xmlElementReference1 = documentNode.SourceContext as XmlElementReference;
        if (xmlElementReference1 != null)
          this.ReportUnnecessaryAttributes(parserContext, documentNodeReference, xmlElementReference1);
        DocumentCompositeNode node = documentNode as DocumentCompositeNode;
        if (node != null)
        {
          DocumentCompositeNodeReference nodeReference = new DocumentCompositeNodeReference(documentNodeReference, node);
          if (str != null)
            this.SetStringProperty(parserContext, attribute1, nodeReference, xmlElementReference, parserContext.PlatformMetadata.KnownProperties.DesignTimeClass, str);
          if (typeName != null)
            this.SetStringProperty(parserContext, attribute2, nodeReference, xmlElementReference, parserContext.PlatformMetadata.KnownProperties.DesignTimeSubclass, typeName);
          if (generatedClassModifier != null)
            this.SetStringProperty(parserContext, attribute3, nodeReference, xmlElementReference, parserContext.PlatformMetadata.KnownProperties.DesignTimeClassModifier, generatedClassModifier);
        }
      }
      return documentNode;
    }

    private XamlParser.ElementType ParseElement(XamlParserContext parserContext, IDocumentNodeReference nodeReference, XmlElementReference xmlElementReference, XmlElement xmlElement, out DocumentNode node)
    {
      IType typeId = this.GetTypeId(parserContext, xmlElementReference, xmlElement);
      if (typeId == null)
      {
        node = (DocumentNode) null;
        return XamlParser.ElementType.Code;
      }
      node = this.ParseElementContent(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
      return node == null ? XamlParser.ElementType.Unknown : XamlParser.ElementType.Type;
    }

    private DocumentNode ParseElementContent(XamlParserContext parserContext, IDocumentNodeReference nodeReference, IType typeId, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentNode documentNode = (DocumentNode) null;
      if (typeId.Equals((object) parserContext.PlatformMetadata.KnownTypes.NullExtension))
      {
        documentNode = (DocumentNode) parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.Object, (IDocumentNodeValue) null);
      }
      else
      {
        if (typeId.Equals((object) parserContext.PlatformMetadata.KnownTypes.TypeExtension))
          return (DocumentNode) this.ParseTypeExtension(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
        if (typeId.Equals((object) parserContext.PlatformMetadata.KnownTypes.StaticExtension))
          return (DocumentNode) this.ParseStaticExtension(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
        if (typeId.Equals((object) parserContext.PlatformMetadata.KnownTypes.ArrayExtension))
          return (DocumentNode) this.ParseArrayExtension(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
        if (DocumentNodeHelper.IsStyleOrTemplate(typeId))
          documentNode = this.ParseStyleOrTemplate(parserContext, nodeReference, (ITypeId) typeId, xmlElementReference, xmlElement);
        else if (parserContext.PlatformMetadata.KnownTypes.XData.Equals((object) typeId))
        {
          documentNode = this.CreateInlineXmlNode(parserContext, xmlElementReference, xmlElement);
        }
        else
        {
          IProperty defaultContentProperty = typeId.Metadata.DefaultContentProperty;
          if ((defaultContentProperty == null || !XamlParser.SupportsContentType(parserContext.TypeResolver, defaultContentProperty.PropertyType, parserContext.TypeResolver.ResolveType(parserContext.PlatformMetadata.KnownTypes.String))) && !this.HasPropertiesOrChildren(parserContext, (IXmlNamespaceResolver) xmlElementReference, xmlElement))
          {
            IList<DocumentNode> list = this.ParseContent(parserContext, nodeReference, xmlElementReference, xmlElement, (IProperty) null, typeId.TypeConverter, (ITypeId) typeId, XamlParser.PropertyHandling.PropertiesNotAllowed);
            if (list.Count > 0)
              documentNode = list[0];
          }
          if (documentNode == null)
          {
            ITextLocation lineInformation = xmlElementReference.LineInformation;
            if (typeId.NearestResolvedType.RuntimeType.IsEnum)
            {
              parserContext.ReportError(XamlParseErrors.PrimitiveTypeWithNoValue(lineInformation, (ITypeId) typeId));
            }
            else
            {
              if (!xmlElementReference.IsRootElement || parserContext.RootClassAttributes == null)
              {
                if (typeId.IsAbstract)
                {
                  parserContext.ReportError(XamlParseErrors.AbstractTypeWithNoValue(lineInformation, (ITypeId) typeId));
                }
                else
                {
                  bool supportInternal = parserContext.TypeResolver.InTargetAssembly(typeId);
                  if (!typeId.HasDefaultConstructor(supportInternal))
                    parserContext.ReportError(XamlParseErrors.NoAccessibleConstructor(lineInformation, (ITypeId) typeId));
                }
              }
              documentNode = (DocumentNode) this.ParseCompositeElement(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
            }
          }
        }
      }
      if (documentNode != null)
        documentNode.SourceContext = (INodeSourceContext) xmlElementReference;
      return documentNode;
    }

    private DocumentCompositeNode ParseCompositeElement(XamlParserContext parserContext, IDocumentNodeReference nodeReference, IType typeId, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentCompositeNode node = parserContext.DocumentContext.CreateNode((ITypeId) typeId);
      if (xmlElementReference.IsRootElement && node.NameScope == null)
        node.NameScope = new DocumentNodeNameScope();
      if (typeId.ItemType != null)
        node.IsExplicitCollection = true;
      node.SourceContext = (INodeSourceContext) xmlElementReference;
      DocumentCompositeNodeReference nodeReference1 = new DocumentCompositeNodeReference(nodeReference, node);
      this.AddPropertiesAndChildren(parserContext, nodeReference1, xmlElementReference, xmlElement);
      return node;
    }

    private bool HasPropertiesOrChildren(XamlParserContext parserContext, IXmlNamespaceResolver namespaceResolver, XmlElement xmlElement)
    {
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node = xmlElement.FirstChild; node != null; node = node.NextNode)
      {
        if (node is XmlElement)
          return true;
      }
      foreach (XmlAttribute xmlAttribute in XmlAttributeEnumerable.NotXmlnsNorDirective(xmlElement))
      {
        ITextLocation lineInformation = this.GetLineInformation(xmlAttribute.SourceContext);
        XmlNamespace xmlNamespace = (XmlNamespace) null;
        if (!string.IsNullOrEmpty(xmlAttribute.Prefix))
        {
          XmlnsPrefix prefix = XmlnsPrefix.ToPrefix(xmlAttribute.Prefix);
          xmlNamespace = parserContext.GetXmlNamespace(lineInformation, namespaceResolver, prefix);
        }
        if (xmlNamespace != XmlNamespace.CompatibilityXmlNamespace)
        {
          if (xmlNamespace != XmlNamespace.XamlXmlNamespace)
            return true;
          switch (xmlAttribute.LocalName)
          {
            case "Uid":
            case "Shared":
            case "Key":
            case "Class":
            case "Subclass":
            case "ClassModifier":
              continue;
            default:
              return true;
          }
        }
      }
      return false;
    }

    private DocumentPrimitiveNode ParseTypeExtension(XamlParserContext parserContext, IDocumentNodeReference nodeReference, IType typeId, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentCompositeNode documentCompositeNode = this.ParseCompositeElement(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
      if (documentCompositeNode != null)
      {
        IType valueAsType = DocumentPrimitiveNode.GetValueAsType(documentCompositeNode.Properties[parserContext.PlatformMetadata.KnownProperties.TypeExtensionType]);
        if (valueAsType != null)
        {
          DocumentPrimitiveNode node = parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) valueAsType));
          node.SourceContext = (INodeSourceContext) xmlElementReference;
          return node;
        }
        ITextLocation lineInformation = xmlElementReference.LineInformation;
        IPropertyId extensionTypeName = parserContext.PlatformMetadata.KnownProperties.TypeExtensionTypeName;
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[extensionTypeName]);
        if (string.IsNullOrEmpty(valueAsString))
        {
          parserContext.ReportError(XamlParseErrors.MissingProperty(lineInformation, extensionTypeName));
        }
        else
        {
          IType typeId1 = XamlTypeHelper.GetTypeId(parserContext, lineInformation, (IXmlNamespaceResolver) xmlElementReference, valueAsString, false);
          if (typeId1 != null)
          {
            DocumentPrimitiveNode node = parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) typeId1));
            node.SourceContext = (INodeSourceContext) xmlElementReference;
            return node;
          }
        }
      }
      return (DocumentPrimitiveNode) null;
    }

    private DocumentCompositeNode ParseStaticExtension(XamlParserContext parserContext, IDocumentNodeReference nodeReference, IType typeId, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentCompositeNode documentCompositeNode = this.ParseCompositeElement(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
      if (documentCompositeNode != null)
      {
        ITextLocation lineInformation = xmlElementReference.LineInformation;
        IPropertyId staticExtensionMember = parserContext.PlatformMetadata.KnownProperties.StaticExtensionMember;
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(documentCompositeNode.Properties[staticExtensionMember]);
        if (string.IsNullOrEmpty(valueAsString))
        {
          parserContext.ReportError(XamlParseErrors.MissingProperty(lineInformation, staticExtensionMember));
        }
        else
        {
          IType typeId1;
          string memberName;
          if (MarkupExtensionParser.GetTypeAndMemberName(parserContext, lineInformation, (IXmlNamespaceResolver) xmlElementReference, valueAsString, (IType) null, out typeId1, out memberName))
          {
            MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, typeId1);
            IMember member = (IMember) typeId1.GetMember(MemberType.LocalProperty | MemberType.Field, memberName, allowableMemberAccess) ?? XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, typeId1, MemberType.LocalProperty, memberName);
            if (member != null)
            {
              DocumentNode documentNode = (DocumentNode) parserContext.DocumentContext.CreateNode(member.MemberTypeId, (IDocumentNodeValue) new DocumentNodeMemberValue(member));
              documentCompositeNode.Properties[staticExtensionMember] = documentNode;
              documentCompositeNode.SourceContext = (INodeSourceContext) xmlElementReference;
              return documentCompositeNode;
            }
            parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, memberName));
          }
        }
      }
      return (DocumentCompositeNode) null;
    }

    private DocumentCompositeNode ParseArrayExtension(XamlParserContext parserContext, IDocumentNodeReference nodeReference, IType typeId, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentCompositeNode documentCompositeNode1 = this.ParseCompositeElement(parserContext, nodeReference, typeId, xmlElementReference, xmlElement);
      if (documentCompositeNode1 == null)
        return (DocumentCompositeNode) null;
      ITextLocation lineInformation = xmlElementReference.LineInformation;
      IPropertyId arrayExtensionType = parserContext.PlatformMetadata.KnownProperties.ArrayExtensionType;
      Type type1 = (Type) null;
      IType valueAsType = DocumentPrimitiveNode.GetValueAsType(documentCompositeNode1.Properties[arrayExtensionType]);
      if (valueAsType != null)
        type1 = valueAsType.NearestResolvedType.RuntimeType;
      else
        parserContext.ReportError(XamlParseErrors.MissingProperty(lineInformation, arrayExtensionType));
      Type type2 = (Type) null;
      if (type1 != (Type) null)
        type2 = type1.MakeArrayType();
      if (type2 == (Type) null)
        type2 = typeof (object[]);
      ITypeId typeId1 = (ITypeId) parserContext.TypeResolver.GetType(type2);
      DocumentCompositeNode node1 = parserContext.DocumentContext.CreateNode(typeId1);
      node1.SourceContext = (INodeSourceContext) xmlElementReference;
      if (documentCompositeNode1.Properties.Count > 0)
      {
        List<KeyValuePair<IProperty, DocumentNode>> list1 = new List<KeyValuePair<IProperty, DocumentNode>>((IEnumerable<KeyValuePair<IProperty, DocumentNode>>) documentCompositeNode1.Properties);
        foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in list1)
          documentCompositeNode1.Properties[(IPropertyId) keyValuePair.Key] = (DocumentNode) null;
        foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in list1)
        {
          IProperty key = keyValuePair.Key;
          if (!parserContext.PlatformMetadata.KnownProperties.ArrayExtensionType.Equals((object) key))
          {
            if (parserContext.PlatformMetadata.KnownProperties.ArrayExtensionItems.Equals((object) key))
            {
              DocumentCompositeNode documentCompositeNode2 = keyValuePair.Value as DocumentCompositeNode;
              if (documentCompositeNode2 != null)
              {
                List<DocumentNode> list2 = new List<DocumentNode>((IEnumerable<DocumentNode>) documentCompositeNode2.Children);
                documentCompositeNode2.Children.Clear();
                foreach (DocumentNode documentNode in list2)
                  this.AddChild(parserContext, XamlParser.GetLineInformation(documentNode, lineInformation), new DocumentCompositeNodeReference(nodeReference, node1), documentNode);
              }
            }
            else
            {
              DocumentNode node2 = keyValuePair.Value;
              XamlParser.GetLineInformation(node2, lineInformation);
              if (!XamlParser.IsPropertyWritable(parserContext, key, XamlParser.AllowProtectedProperties(parserContext, xmlElementReference.IsRootElement)))
                parserContext.ReportError(XamlParseErrors.PropertyIsNotWritable(lineInformation, (IPropertyId) key));
              else if (XamlParser.CanAssignProperty(parserContext, lineInformation, typeId1, key))
                node1.Properties[(IPropertyId) key] = node2;
            }
          }
        }
      }
      return node1;
    }

    private void AddPropertiesAndChildren(XamlParserContext parserContext, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      IPropertyValueTypeMetadata metadata = nodeReference.Type.Metadata as IPropertyValueTypeMetadata;
      if (metadata != null)
      {
        IType propertyTargetType = nodeReference.TargetType;
        if (metadata.TargetNameProperty != null)
        {
          XmlAttribute attribute = this.GetAttribute(parserContext, nodeReference.Type, xmlElementReference, xmlElement, metadata.TargetNameProperty);
          DocumentNode documentNode = (DocumentNode) null;
          if (attribute != null)
          {
            string attributeValue = XmlUtilities.GetAttributeValue(attribute);
            DocumentNodeNameScope nameScope = nodeReference.Parent.NameScope;
            if (nameScope != null)
              documentNode = nameScope.FindNode(attributeValue);
            if (documentNode == null)
              parserContext.ReportError(XamlParseErrors.UnrecognizedName(this.GetLineInformation(attribute.SourceContext), attributeValue));
          }
          if (documentNode != null)
            propertyTargetType = documentNode.Type;
        }
        if (metadata.PropertyProperty == null)
          return;
        this.AddPropertyAndValue(parserContext, nodeReference, xmlElementReference, xmlElement, metadata, propertyTargetType);
      }
      else
        this.AddPropertiesAndChildren(parserContext, nodeReference, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (propertyKey => true));
    }

    private void AddPropertyAndValue(XamlParserContext parserContext, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, XmlElement xmlElement, IPropertyValueTypeMetadata metadata, IType propertyTargetType)
    {
      IType type = nodeReference.Type;
      bool addedPropertyProperty = false;
      nodeReference.SetTargetType(propertyTargetType);
      try
      {
        this.AddPropertiesAndChildren(parserContext, nodeReference, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (p =>
        {
          if (p == null || !p.Equals((object) metadata.PropertyProperty))
            return false;
          addedPropertyProperty = true;
          return true;
        }));
      }
      finally
      {
        nodeReference.ClearTargetType();
      }
      this.AddPropertiesAndChildren(parserContext, nodeReference, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (p => p == null || !p.Equals((object) metadata.PropertyProperty) && !p.Equals((object) metadata.ValueProperty)));
      IProperty propertyKey = (IProperty) null;
      bool flag1 = true;
      DocumentPrimitiveNode documentPrimitiveNode = nodeReference.Node.Properties[metadata.PropertyProperty] as DocumentPrimitiveNode;
      if (documentPrimitiveNode != null)
      {
        propertyKey = DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode) as IProperty;
        if (propertyKey != null && parserContext.PlatformMetadata.KnownTypes.Setter.IsAssignableFrom((ITypeId) type))
        {
          flag1 = XamlParser.IsPropertyWritable(parserContext, propertyKey, XamlParser.AllowProtectedProperties(parserContext, nodeReference.IsRoot));
          if (!flag1)
            parserContext.ReportError(XamlParseErrors.PropertyIsNotWritable(xmlElementReference.LineInformation, (IPropertyId) propertyKey));
        }
      }
      else if (metadata.IsPropertyRequired && !addedPropertyProperty)
        parserContext.ReportError(XamlParseErrors.MissingProperty(xmlElementReference.LineInformation, metadata.PropertyProperty));
      XmlNode childNode = this.GetChildNode(parserContext, type, xmlElementReference, xmlElement, (IPropertyId) metadata.ValueProperty);
      if (childNode != null)
      {
        ITextLocation lineInformation = this.GetLineInformation(childNode.SourceContext);
        DocumentNode valueNode = (DocumentNode) null;
        IType valueType = parserContext.TypeResolver.ResolveType(parserContext.PlatformMetadata.KnownTypes.Object);
        TypeConverter typeConverter = (TypeConverter) null;
        if (propertyKey != null)
        {
          valueType = propertyKey.PropertyType;
          typeConverter = propertyKey.TypeConverter;
        }
        if (typeConverter == null)
          typeConverter = valueType.TypeConverter;
        XamlSourceContext propertyContext = (XamlSourceContext) null;
        XmlAttribute attribute;
        if ((attribute = childNode as XmlAttribute) != null)
        {
          propertyContext = (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext);
          valueNode = this.GetAttributeValue(parserContext, lineInformation, nodeReference, xmlElementReference, typeConverter, (ITypeId) valueType, attribute);
        }
        else
        {
          XmlElement xmlElement1;
          if ((xmlElement1 = childNode as XmlElement) != null)
          {
            XmlElementReference xmlElementReference1 = this.NewXmlElementReference(parserContext, (XmlContainerReference) xmlElementReference, xmlElement1);
            propertyContext = (XamlSourceContext) xmlElementReference1;
            IList<DocumentNode> list = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, xmlElementReference1, xmlElement1, metadata.ValueProperty, typeConverter, (ITypeId) valueType, XamlParser.PropertyHandling.PropertiesNotAllowed);
            int count = list.Count;
            if (count > 0)
            {
              int index = 0;
              if (count > 1)
              {
                parserContext.ReportError(XamlParseErrors.SingleValuedPropertySetMultipleTimes(xmlElementReference1.LineInformation, (IPropertyId) metadata.ValueProperty));
                index = this.parserPolicy.ChooseFromMultiplePropertyValues((IPropertyId) metadata.ValueProperty, count);
              }
              valueNode = list[index];
            }
            else
              parserContext.ReportError(XamlParseErrors.PropertyElementWithNoValue(xmlElementReference1.LineInformation, (IPropertyId) metadata.ValueProperty));
            if (valueNode != null)
            {
              XmlElementReference xmlElementReference2 = valueNode.SourceContext as XmlElementReference;
              if (xmlElementReference2 != null)
                this.ReportUnnecessaryAttributes(parserContext, (IDocumentNodeReference) nodeReference, xmlElementReference2);
            }
          }
        }
        if (valueNode == null)
          return;
        bool flag2 = XamlParser.CanAssignTo(parserContext, lineInformation, valueType, valueNode);
        if (!flag1 || !flag2)
          return;
        XamlParser.SetProperty(parserContext, lineInformation, nodeReference, propertyContext, (IPropertyId) metadata.ValueProperty, valueNode);
      }
      else
        parserContext.ReportError(XamlParseErrors.MissingProperty(xmlElementReference.LineInformation, (IPropertyId) metadata.ValueProperty));
    }

    private void AddPropertiesAndChildren(XamlParserContext parserContext, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, XmlElement xmlElement, Predicate<IPropertyId> propertyFilter)
    {
      IType type1 = nodeReference.Type;
      bool flag1 = XamlParser.AllowProtectedProperties(parserContext, xmlElementReference.IsRootElement);
      foreach (XmlAttribute attribute in XmlAttributeEnumerable.NotXmlnsNorDirective(xmlElement))
      {
        ITextLocation lineInformation = this.GetLineInformation(attribute.SourceContext);
        XmlNamespace xmlNamespace = (XmlNamespace) null;
        if (!string.IsNullOrEmpty(attribute.Prefix))
        {
          XmlnsPrefix prefix = XmlnsPrefix.ToPrefix(attribute.Prefix);
          xmlNamespace = parserContext.GetXmlNamespace(lineInformation, (IXmlNamespaceResolver) xmlElementReference, prefix);
        }
        IProperty propertyKey = (IProperty) null;
        XamlParser.MemberResolution memberResolution = XamlParser.MemberResolution.Skipped;
        bool flag2 = false;
        if (xmlNamespace != XmlNamespace.CompatibilityXmlNamespace)
        {
          if (xmlNamespace == XmlNamespace.XamlXmlNamespace)
          {
            switch (attribute.LocalName)
            {
              case "Name":
                propertyKey = type1.Metadata.NameProperty;
                memberResolution = XamlParser.MemberResolution.Known;
                break;
              case "Key":
                break;
              case "Class":
              case "Subclass":
              case "ClassModifier":
                if (!xmlElementReference.IsRootElement)
                {
                  parserContext.ReportError(XamlParseErrors.AttributeValidAtRootOnly(xmlElementReference.LineInformation, attribute.LocalName));
                  break;
                }
                break;
              default:
                flag2 = true;
                break;
            }
          }
          else if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
          {
            switch (attribute.LocalName)
            {
              case "DesignWidth":
              case "DesignHeight":
                if (xmlElementReference.IsRootElement)
                {
                  flag2 = true;
                  break;
                }
                DocumentPropertyContentReference contentReference = nodeReference.Parent as DocumentPropertyContentReference;
                DocumentCompositeNodeReference compositeNodeReference = nodeReference.Parent as DocumentCompositeNodeReference;
                flag2 = compositeNodeReference == null ? contentReference != null && contentReference.Property.Equals((object) parserContext.TypeResolver.ResolveProperty(parserContext.TypeResolver.PlatformMetadata.KnownProperties.DesignTimeVisualTree)) : parserContext.PlatformMetadata.KnownTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) compositeNodeReference.Type);
                break;
              default:
                flag2 = true;
                break;
            }
          }
          else if (xmlNamespace == null && parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsBothNameAndXName) && (attribute.LocalName == "Name" && parserContext.PlatformMetadata.KnownTypes.DependencyObject.IsAssignableFrom((ITypeId) type1)))
          {
            propertyKey = type1.Metadata.NameProperty;
            memberResolution = XamlParser.MemberResolution.Known;
          }
          else
            flag2 = true;
        }
        if (flag2)
          memberResolution = this.GetPropertyKey(parserContext, xmlElementReference, xmlElement, attribute, type1, flag1, out propertyKey);
        switch (memberResolution)
        {
          case XamlParser.MemberResolution.Known:
            if (propertyFilter((IPropertyId) propertyKey))
            {
              this.SetProperty(parserContext, lineInformation, nodeReference, xmlElementReference, attribute, propertyKey);
              continue;
            }
            continue;
          case XamlParser.MemberResolution.Ignored:
            xmlElementReference.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.Ignored, attribute, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext)));
            continue;
          default:
            continue;
        }
      }
      if (xmlElement.FirstChild == null)
        return;
      XamlParser.XmlElementReferenceChildEnumerator referenceChildEnumerator = new XamlParser.XmlElementReferenceChildEnumerator(xmlElementReference, xmlElement);
      while (referenceChildEnumerator.MoveNext())
      {
        XmlElement xmlElement1 = referenceChildEnumerator.Current as XmlElement;
        if (xmlElement1 != null)
        {
          XmlElementReference elementReference = this.NewXmlElementReference(parserContext, (XmlContainerReference) xmlElementReference, xmlElement1);
          XmlNamespace xmlNamespace;
          string typeName;
          string memberName;
          if (this.SplitNamespaceAndName(parserContext, elementReference, xmlElement1, out xmlNamespace, out typeName, out memberName) == 3)
          {
            if (XamlTypeHelper.IsIgnorable(parserContext, (IXmlNamespaceResolver) elementReference, xmlNamespace))
              xmlElementReference.AddChildNode(new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.Ignored, (XmlContainerReference) xmlElementReference, (Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlElement1));
            else if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
            {
              this.SetDesignTimeIgnorableElement(parserContext, memberName, type1, nodeReference, elementReference, xmlElement, xmlElement1);
            }
            else
            {
              IProperty propertyKey = this.GetPropertyKey(parserContext, elementReference.LineInformation, (ITypeId) type1, xmlNamespace, typeName, memberName, flag1);
              if (propertyKey != null && propertyFilter((IPropertyId) propertyKey))
              {
                IType propertyType = propertyKey.PropertyType;
                IList<DocumentNode> children;
                if (XamlParser.IsPropertyWritable(parserContext, propertyKey, XamlParser.AllowProtectedProperties(parserContext, nodeReference.IsRoot)) && !this.HasPropertiesOrChildren(parserContext, (IXmlNamespaceResolver) elementReference, xmlElement1))
                {
                  children = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, elementReference, xmlElement1, propertyKey, this.GetTypeConverter(propertyKey), (ITypeId) propertyType, XamlParser.PropertyHandling.PropertiesNotAllowed);
                }
                else
                {
                  IType valueTextContentType = this.GetPropertyValueTextContentType(propertyType);
                  children = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, elementReference, xmlElement1, propertyKey, valueTextContentType.TypeConverter, (ITypeId) valueTextContentType, XamlParser.PropertyHandling.PropertiesNotAllowed);
                }
                referenceChildEnumerator.MovePrecedingCommentToChild(elementReference);
                this.SetProperty(parserContext, nodeReference, propertyKey, (XamlSourceContext) elementReference, elementReference, children);
              }
            }
          }
          else if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
            this.SetDesignTimeIgnorableElement(parserContext, typeName, type1, nodeReference, elementReference, xmlElement, xmlElement1);
        }
      }
      IProperty defaultContentProperty = type1.Metadata.DefaultContentProperty;
      if (!propertyFilter((IPropertyId) defaultContentProperty))
        return;
      IType type2 = defaultContentProperty != null ? this.GetPropertyValueTextContentType(defaultContentProperty.PropertyType) : parserContext.TypeResolver.ResolveType(parserContext.PlatformMetadata.KnownTypes.String);
      IList<DocumentNode> children1 = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, xmlElementReference, xmlElement, defaultContentProperty, type2.TypeConverter, (ITypeId) type2, XamlParser.PropertyHandling.PropertiesIgnored);
      if (!type1.IsResolvable)
        return;
      bool flag3 = false;
      for (int index = 0; index < children1.Count; ++index)
      {
        DocumentNode documentNode = children1[index];
        if (documentNode != null && parserContext.PlatformMetadata.KnownTypes.XData.Equals((object) documentNode.Type))
        {
          flag3 = true;
          break;
        }
      }
      if (flag3)
      {
        List<DocumentNode> list = new List<DocumentNode>();
        for (int index = 0; index < children1.Count; ++index)
        {
          DocumentNode documentNode = children1[index];
          if (documentNode != null && parserContext.PlatformMetadata.KnownTypes.XData.Equals((object) documentNode.Type))
          {
            ITextLocation lineInformation = XamlParser.GetLineInformation(documentNode, xmlElementReference.LineInformation);
            if (type1.Metadata.SupportsInlineXml)
            {
              XamlParser.VerifyPropertyIsUnset(parserContext, lineInformation, nodeReference.Node, parserContext.PlatformMetadata.KnownProperties.DesignTimeInlineXml);
              nodeReference.Node.Properties[parserContext.PlatformMetadata.KnownProperties.DesignTimeInlineXml] = documentNode;
            }
            else if (defaultContentProperty != null)
            {
              if (defaultContentProperty.PropertyType.Metadata.SupportsInlineXml)
                this.SetInlineXml(parserContext, lineInformation, nodeReference, defaultContentProperty, documentNode);
              else
                parserContext.ReportError(XamlParseErrors.TypeDoesNotSupportInlineXml(lineInformation, (ITypeId) type1));
            }
            else
              parserContext.ReportError(XamlParseErrors.TypeDoesNotSupportInlineXml(lineInformation, (ITypeId) type1));
          }
          else
            list.Add(documentNode);
        }
        children1 = (IList<DocumentNode>) list;
      }
      if (children1.Count <= 0)
        return;
      if (defaultContentProperty != null)
      {
        IType propertyType = defaultContentProperty.PropertyType;
        if (propertyType != null && propertyType.ItemType != null && propertyType.IsArray)
          parserContext.ReportError(XamlParseErrors.ArrayPropertyRequiresExplicitPropertyTag(xmlElementReference.LineInformation));
        else
          this.SetProperty(parserContext, nodeReference, defaultContentProperty, (XamlSourceContext) null, xmlElementReference, children1);
      }
      else
      {
        XamlParser.ContentHandling contentHandling = parserContext.PlatformMetadata.KnownTypes.IDictionary.IsAssignableFrom((ITypeId) type1) || parserContext.PlatformMetadata.KnownTypes.ResourceDictionary.IsAssignableFrom((ITypeId) type1) ? XamlParser.ContentHandling.DictionaryEntries : XamlParser.ContentHandling.NotDictionaryEntries;
        this.AddChildren(parserContext, nodeReference, xmlElementReference, contentHandling, children1);
      }
    }

    private void SetDesignTimeIgnorableElement(XamlParserContext parserContext, string typeName, IType targetType, DocumentCompositeNodeReference nodeReference, XmlElementReference childElementReference, XmlElement xmlElement, XmlElement childElement)
    {
      IProperty designTimeProperty = parserContext.PlatformMetadata.GetDesignTimeProperty(typeName, targetType);
      if (designTimeProperty == null)
        return;
      IType propertyType = designTimeProperty.PropertyType;
      IList<DocumentNode> children;
      if (XamlParser.IsPropertyWritable(parserContext, designTimeProperty, XamlParser.AllowProtectedProperties(parserContext, nodeReference.IsRoot)) && !this.HasPropertiesOrChildren(parserContext, (IXmlNamespaceResolver) childElementReference, xmlElement))
      {
        children = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, childElementReference, childElement, designTimeProperty, this.GetTypeConverter(designTimeProperty), (ITypeId) propertyType, XamlParser.PropertyHandling.PropertiesNotAllowed);
      }
      else
      {
        IType valueTextContentType = this.GetPropertyValueTextContentType(propertyType);
        children = this.ParseContent(parserContext, (IDocumentNodeReference) nodeReference, childElementReference, childElement, designTimeProperty, valueTextContentType.TypeConverter, (ITypeId) valueTextContentType, XamlParser.PropertyHandling.PropertiesNotAllowed);
      }
      this.SetProperty(parserContext, nodeReference, designTimeProperty, (XamlSourceContext) childElementReference, childElementReference, children);
    }

    private IType GetPropertyValueTextContentType(IType valueType)
    {
      if (valueType.ItemType == null)
        return valueType;
      return this.documentContext.TypeResolver.ResolveType(valueType.PlatformMetadata.KnownTypes.String);
    }

    private static bool CheckContainerType(IDocumentNodeReference nodeReference, ITypeId expectedType)
    {
      DocumentCompositeNodeReference compositeNodeReference = nodeReference as DocumentCompositeNodeReference;
      if (compositeNodeReference == null)
        return false;
      ITypeId type = (ITypeId) compositeNodeReference.Type;
      return !type.IsResolvable || expectedType.IsAssignableFrom(type);
    }

    private static bool AllowProtectedProperties(XamlParserContext parserContext, bool isRootElement)
    {
      if (isRootElement)
        return parserContext.RootClassAttributes != null;
      return false;
    }

    private IList<DocumentNode> ParseContent(XamlParserContext parserContext, IDocumentNodeReference nodeReference, XmlElementReference xmlElementReference, XmlElement xmlElement, IProperty childProperty, TypeConverter textTypeConverter, ITypeId textType, XamlParser.PropertyHandling propertyHandling)
    {
      if (childProperty != null)
        nodeReference = (IDocumentNodeReference) new DocumentPropertyContentReference(parserContext.TypeResolver.PlatformMetadata, nodeReference, childProperty);
      XamlParser.LazilyInstantiatedList<DocumentNode> instantiatedList = new XamlParser.LazilyInstantiatedList<DocumentNode>();
      WhitespaceSignificant whitespaceSignificant = WhitespaceSignificant.NotSignificant;
      if (childProperty != null && childProperty.PropertyType.Metadata.IsWhitespaceSignificant)
        whitespaceSignificant = WhitespaceSignificant.Significant;
      ElementContentBuilder elementContentBuilder = new ElementContentBuilder(xmlElementReference.XmlSpace, whitespaceSignificant);
      XamlParser.XmlElementReferenceChildEnumerator referenceChildEnumerator = new XamlParser.XmlElementReferenceChildEnumerator(xmlElementReference, xmlElement);
      while (referenceChildEnumerator.MoveNext())
      {
        Microsoft.Expression.DesignModel.Markup.Xml.Node current = referenceChildEnumerator.Current;
        XmlElement xmlElement1;
        if ((xmlElement1 = current as XmlElement) != null)
        {
          XmlElementReference elementReference = this.NewXmlElementReference(parserContext, (XmlContainerReference) xmlElementReference, xmlElement1);
          XmlNamespace xmlNamespace;
          string typeName;
          string memberName;
          switch (this.SplitNamespaceAndName(parserContext, elementReference, xmlElement1, out xmlNamespace, out typeName, out memberName))
          {
            case 3:
              if (propertyHandling == XamlParser.PropertyHandling.PropertiesNotAllowed)
              {
                parserContext.ReportError(XamlParseErrors.NestedPropertiesNotSupported(elementReference.LineInformation, xmlElement1.LocalName));
                continue;
              }
              continue;
            case 2:
              XamlParser.MemberResolution memberResolution = XamlParser.MemberResolution.Unknown;
              IType type = (IType) null;
              if (xmlNamespace == XmlNamespace.XamlXmlNamespace && xmlElement1.LocalName == "Code")
                memberResolution = XamlParser.MemberResolution.Skipped;
              else if (XamlTypeHelper.IsIgnorable(parserContext, (IXmlNamespaceResolver) elementReference, xmlNamespace))
                memberResolution = XamlParser.MemberResolution.Ignored;
              else if (xmlNamespace == XmlNamespace.DesignTimeXmlNamespace)
              {
                memberResolution = XamlParser.MemberResolution.Skipped;
              }
              else
              {
                type = XamlTypeHelper.GetTypeId(parserContext, elementReference.LineInformation, xmlNamespace, typeName);
                if (type != null)
                  memberResolution = XamlParser.MemberResolution.Known;
              }
              WhitespaceTrimming whitespaceSurroundingSibling = WhitespaceTrimming.Include;
              if (type != null && parserContext.PlatformMetadata.IsTrimSurroundingWhitespace(type))
                whitespaceSurroundingSibling = WhitespaceTrimming.Remove;
              string contentBeforeSibling = elementContentBuilder.GetContentBeforeSibling(whitespaceSurroundingSibling);
              if (contentBeforeSibling != null)
              {
                DocumentNode nodeFromTextValue = MarkupExtensionParser.CreateNodeFromTextValue(parserContext, xmlElementReference.LineInformation, nodeReference, (IXmlNamespaceResolver) xmlElementReference, textTypeConverter, textType, contentBeforeSibling);
                instantiatedList.Add(nodeFromTextValue);
              }
              switch (memberResolution)
              {
                case XamlParser.MemberResolution.Known:
                  DocumentNode node;
                  if (this.ParseElement(parserContext, nodeReference, elementReference, xmlElement1, out node) != XamlParser.ElementType.Code)
                  {
                    if (node != null)
                      referenceChildEnumerator.MovePrecedingCommentToChild(elementReference);
                    instantiatedList.Add(node);
                    continue;
                  }
                  continue;
                case XamlParser.MemberResolution.Ignored:
                  xmlElementReference.AddChildNode(new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.Ignored, (XmlContainerReference) xmlElementReference, (Microsoft.Expression.DesignModel.Markup.Xml.Node) xmlElement1));
                  continue;
                default:
                  continue;
              }
            default:
              continue;
          }
        }
        else
        {
          Literal literal;
          if ((literal = current as Literal) != null)
          {
            string sourceText = literal.SourceContext.SourceText;
            elementContentBuilder.Append(sourceText);
          }
          else
          {
            XmlEntityReference xmlEntityReference;
            if ((xmlEntityReference = current as XmlEntityReference) != null)
            {
              string text = xmlEntityReference.Value.Value;
              elementContentBuilder.Append(text);
            }
            else
            {
              XmlCDATA xmlCdata;
              if ((xmlCdata = current as XmlCDATA) != null)
              {
                if (!parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsCData))
                {
                  parserContext.ReportError(XamlParseErrors.CDataNotSupported(this.GetLineInformation(xmlCdata.SourceContext)));
                }
                else
                {
                  string literalValue = xmlCdata.LiteralValue;
                  elementContentBuilder.Append(literalValue);
                }
              }
            }
          }
        }
      }
      string remainingContent = elementContentBuilder.GetRemainingContent();
      if (remainingContent != null)
      {
        DocumentNode nodeFromTextValue = MarkupExtensionParser.CreateNodeFromTextValue(parserContext, xmlElementReference.LineInformation, nodeReference, (IXmlNamespaceResolver) xmlElementReference, textTypeConverter, textType, remainingContent);
        instantiatedList.Add(nodeFromTextValue);
      }
      if (instantiatedList.Count > 0)
        return instantiatedList.List;
      return (IList<DocumentNode>) XamlParser.emptyDocumentNodeCollection;
    }

    private void AddChildren(XamlParserContext parserContext, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, XamlParser.ContentHandling contentHandling, IList<DocumentNode> children)
    {
      Dictionary<DocumentNode, ITextLocation> dictionary = (Dictionary<DocumentNode, ITextLocation>) null;
      if (contentHandling == XamlParser.ContentHandling.DictionaryEntries)
        dictionary = new Dictionary<DocumentNode, ITextLocation>((IEqualityComparer<DocumentNode>) DocumentNodeEqualityComparer.Instance);
      for (int index = 0; index < children.Count; ++index)
      {
        DocumentNode documentNode = children[index];
        if (documentNode != null)
        {
          XmlElementReference xmlElementReference1 = documentNode.SourceContext as XmlElementReference;
          if (xmlElementReference1 != null)
            this.ReportUnnecessaryAttributes(parserContext, (IDocumentNodeReference) nodeReference, xmlElementReference1);
          ITextLocation lineInformation = XamlParser.GetLineInformation(documentNode, xmlElementReference.LineInformation);
          if (contentHandling == XamlParser.ContentHandling.DictionaryEntries)
          {
            if (xmlElementReference1 != null)
            {
              DocumentCompositeNode node = documentNode as DocumentCompositeNode;
              DocumentCompositeNodeReference compositeNodeReference = node != null ? new DocumentCompositeNodeReference((IDocumentNodeReference) nodeReference, node) : nodeReference;
              DocumentNode childNode = this.ParseDictionaryEntry(parserContext, (IDocumentNodeReference) compositeNodeReference, xmlElementReference1, documentNode);
              if (childNode != null)
              {
                DocumentCompositeNode entryNode = childNode as DocumentCompositeNode;
                if (entryNode != null)
                {
                  DocumentNode resourceEntryKey = DocumentNodeHelper.GetResourceEntryKey(entryNode);
                  if (resourceEntryKey != null)
                  {
                    if (dictionary.ContainsKey(resourceEntryKey))
                      parserContext.ReportError(XamlParseErrors.RepeatedKey(lineInformation));
                    else
                      dictionary.Add(resourceEntryKey, lineInformation);
                  }
                }
                this.AddChild(parserContext, lineInformation, nodeReference, childNode);
              }
            }
            else
              parserContext.ReportError(XamlParseErrors.MissingDictionaryKey(lineInformation));
          }
          else
            this.AddChild(parserContext, lineInformation, nodeReference, documentNode);
        }
      }
    }

    private DocumentNode CreateInlineXmlNode(XamlParserContext parserContext, XmlElementReference xmlElementReference, XmlElement xDataElement)
    {
      SourceContext startTagContext = xDataElement.StartTagContext;
      SourceContext endTagContext = xDataElement.EndTagContext;
      string str = string.Empty;
      if (startTagContext.Document != null)
      {
        int startCol = startTagContext.EndCol;
        int endCol = endTagContext.StartCol;
        if (startCol < endCol)
          str = new SourceContext(startTagContext.Document, startCol, endCol).SourceText;
      }
      DocumentNode documentNode = (DocumentNode) parserContext.DocumentContext.CreateNode(parserContext.PlatformMetadata.KnownTypes.XData, (IDocumentNodeValue) new DocumentNodeStringValue(str));
      documentNode.SourceContext = (INodeSourceContext) xmlElementReference;
      return documentNode;
    }

    private void SetStringProperty(XamlParserContext parserContext, XmlAttribute attribute, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, IPropertyId propertyKey, string value)
    {
      XamlSourceContext propertyContext = (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext);
      DocumentNode valueNode = (DocumentNode) parserContext.DocumentContext.CreateNode(value);
      XamlParser.SetProperty(parserContext, propertyContext.LineInformation, nodeReference, propertyContext, propertyKey, valueNode);
    }

    private static bool VerifyPropertyIsUnset(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNode nodeReference, IPropertyId propertyKey)
    {
      if (nodeReference.Properties[propertyKey] == null)
        return true;
      if (propertyKey.MemberType != MemberType.DesignTimeProperty)
        parserContext.ReportError(XamlParseErrors.PropertySetMultipleTimes(lineInformation, propertyKey));
      return false;
    }

    private static bool IsPropertyWritable(XamlParserContext parserContext, IProperty propertyKey, bool allowProtectedProperties)
    {
      if (TypeHelper.IsPropertyWritable(parserContext.TypeResolver, propertyKey, allowProtectedProperties) || propertyKey.PropertyType.Metadata.SupportsInlineXml)
        return true;
      if (parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.WorkaroundSL15899))
      {
        IPropertyId resourcesProperty = propertyKey.DeclaringType.Metadata.ResourcesProperty;
        if (resourcesProperty != null && resourcesProperty.Equals((object) propertyKey))
          return true;
      }
      return false;
    }

    internal static bool SetProperty(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, XamlSourceContext propertyContext, IPropertyId propertyKey, DocumentNode valueNode)
    {
      IProperty propertyKey1 = parserContext.TypeResolver.ResolveProperty(propertyKey);
      XamlParser.VerifyPropertyIsUnset(parserContext, lineInformation, nodeReference.Node, (IPropertyId) propertyKey1);
      if (propertyKey1.DeclaringType.Metadata.NameProperty == propertyKey1)
      {
        string valueAsString = DocumentPrimitiveNode.GetValueAsString(valueNode);
        if (valueAsString != null)
        {
          DocumentNodeNameScope nameScope = nodeReference.Parent.NameScope;
          if (nameScope != null)
          {
            if (nameScope.FindNode(valueAsString) != null)
            {
              parserContext.ReportError(XamlParseErrors.RepeatedName(lineInformation, valueAsString));
              return false;
            }
            nameScope.AddNode(valueAsString, (DocumentNode) nodeReference.Node);
          }
        }
      }
      bool flag1 = XamlParser.IsPropertyWritable(parserContext, propertyKey1, XamlParser.AllowProtectedProperties(parserContext, nodeReference.IsRoot));
      if (!flag1)
        parserContext.ReportError(XamlParseErrors.PropertyIsNotWritable(lineInformation, (IPropertyId) propertyKey1));
      bool flag2 = XamlParser.CanAssignProperty(parserContext, lineInformation, (ITypeId) nodeReference.Type, propertyKey1);
      bool flag3 = XamlParser.CanAssignTo(parserContext, lineInformation, propertyKey1.PropertyType, valueNode);
      if (!flag1 || !flag2 || !flag3)
        return false;
      nodeReference.Node.SetValue((IPropertyId) propertyKey1, (INodeSourceContext) propertyContext, valueNode);
      return true;
    }

    private static bool CanAssignProperty(XamlParserContext parserContext, ITextLocation lineInformation, ITypeId parentType, IProperty propertyKey)
    {
      if (!parentType.IsResolvable || !propertyKey.IsResolvable)
        return true;
      if (propertyKey.IsAttachable)
      {
        ITypeId typeId = (ITypeId) parserContext.TypeResolver.GetType(propertyKey.TargetType);
        if (typeId == null || !typeId.IsAssignableFrom(parentType))
        {
          parserContext.ReportError(XamlParseErrors.AttachedPropertyDoesNotApply(lineInformation, propertyKey));
          return false;
        }
      }
      else if (!propertyKey.DeclaringType.IsAssignableFrom(parentType))
      {
        parserContext.ReportError(XamlParseErrors.LocalPropertyDoesNotApply(lineInformation, propertyKey));
        return false;
      }
      return true;
    }

    internal static bool CanAssignTo(XamlParserContext parserContext, ITextLocation lineInformation, IType valueType, DocumentNode valueNode)
    {
      switch (DocumentNode.CanAssignTo(parserContext.TypeResolver, valueType, valueNode))
      {
        case CanAssignResult.CannotAssign:
          if (XamlParser.IsSDKType(valueNode.Type) && XamlParser.IsSDKType(valueType))
            parserContext.ReportError(XamlParseErrors.MismatchedVersionSDKType(lineInformation, valueNode.Type, valueNode.PlatformMetadata.TargetFramework));
          else
            parserContext.ReportError(XamlParseErrors.UnexpectedValueType(lineInformation, (ITypeId) valueType));
          return false;
        case CanAssignResult.NotNullable:
          parserContext.ReportError(XamlParseErrors.TypeIsNotNullable(lineInformation, (ITypeId) valueType));
          return false;
        case CanAssignResult.NotXmlSerializable:
          parserContext.ReportError(XamlParseErrors.TypeDoesNotSupportInlineXml(lineInformation, (ITypeId) valueType));
          return false;
        default:
          return true;
      }
    }

    private bool AddChild(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, DocumentNode childNode)
    {
      IType type = nodeReference.Type;
      IType itemType = type.ItemType;
      if (itemType == null || nodeReference.Node.Children == null)
      {
        parserContext.ReportError(XamlParseErrors.ParentTypeDoesNotSupportChildren(lineInformation, (ITypeId) type));
        return false;
      }
      switch (DocumentNode.CanAssignTo(this.documentContext.TypeResolver, itemType, childNode))
      {
        case CanAssignResult.CannotAssign:
          IType contentWrapperType = XamlParser.GetContentWrapperType(parserContext.TypeResolver, nodeReference.Type, childNode.Type);
          if (contentWrapperType != null)
          {
            DocumentCompositeNode node = parserContext.DocumentContext.CreateNode((ITypeId) contentWrapperType);
            IPropertyId index = (IPropertyId) contentWrapperType.Metadata.DefaultContentProperty;
            node.Properties[index] = childNode;
            nodeReference.Node.Children.Add((DocumentNode) node);
            return true;
          }
          if (DocumentPrimitiveNode.IsNull(childNode))
            parserContext.ReportError(XamlParseErrors.TypeIsNotNullable(lineInformation, (ITypeId) itemType));
          else if (XamlParser.IsSDKType(itemType) && XamlParser.IsSDKType(childNode.Type))
            parserContext.ReportError(XamlParseErrors.MismatchedVersionSDKType(lineInformation, childNode.Type, childNode.PlatformMetadata.TargetFramework));
          else
            parserContext.ReportError(XamlParseErrors.UnexpectedChildType(lineInformation, (ITypeId) itemType));
          return false;
        case CanAssignResult.NotNullable:
          parserContext.ReportError(XamlParseErrors.TypeIsNotNullable(lineInformation, (ITypeId) itemType));
          return false;
        default:
          nodeReference.Node.Children.Add(childNode);
          return true;
      }
    }

    private static bool IsSDKType(IType type)
    {
      for (; type != null; type = type.BaseType)
      {
        if (type.RuntimeAssembly.Name.Equals("System.Windows.Interactivity") && (type.Name.Equals("Behavior") || type.Name.Equals("TriggerBase") || type.Name.Equals("TriggerAction")))
          return true;
      }
      return false;
    }

    private XmlAttribute GetAttribute(XamlParserContext parserContext, IType targetType, XmlElementReference xmlElementReference, XmlElement xmlElement, IPropertyId propertyKey)
    {
      bool allowProtectedPropertiesOnTargetType = XamlParser.AllowProtectedProperties(parserContext, xmlElementReference.IsRootElement);
      foreach (XmlAttribute attribute in XmlAttributeEnumerable.NotXmlnsNorDirective(xmlElement))
      {
        IProperty propertyKey1;
        if (this.GetPropertyKey(parserContext, xmlElementReference, xmlElement, attribute, targetType, allowProtectedPropertiesOnTargetType, out propertyKey1) == XamlParser.MemberResolution.Known && propertyKey != null && propertyKey.Equals((object) propertyKey1))
          return attribute;
      }
      return (XmlAttribute) null;
    }

    private XmlElement GetChildElement(XamlParserContext parserContext, ITypeId targetType, XmlElementReference xmlElementReference, XmlElement xmlElement, IPropertyId propertyKey)
    {
      bool allowProtectedPropertiesOnParentType = XamlParser.AllowProtectedProperties(parserContext, xmlElementReference.IsRootElement);
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node = xmlElement.FirstChild; node != null; node = node.NextNode)
      {
        XmlElement xmlElement1 = node as XmlElement;
        if (xmlElement1 != null)
        {
          XmlElementReference xmlElementReference1 = this.NewXmlElementReference(parserContext, (XmlContainerReference) xmlElementReference, xmlElement1);
          XmlNamespace xmlNamespace;
          string typeName;
          string memberName;
          if (this.SplitNamespaceAndName(parserContext, xmlElementReference1, xmlElement1, out xmlNamespace, out typeName, out memberName) == 3 && !XamlTypeHelper.IsIgnorable(parserContext, (IXmlNamespaceResolver) xmlElementReference1, xmlNamespace))
          {
            IPropertyId propertyId = (IPropertyId) this.GetPropertyKey(parserContext, xmlElementReference1.LineInformation, targetType, xmlNamespace, typeName, memberName, allowProtectedPropertiesOnParentType);
            if (propertyId != null && propertyKey.Equals((object) propertyId))
              return xmlElement1;
          }
        }
      }
      return (XmlElement) null;
    }

    private XmlNode GetChildNode(XamlParserContext parserContext, IType targetType, XmlElementReference xmlElementReference, XmlElement xmlElement, IPropertyId propertyKey)
    {
      return (XmlNode) this.GetAttribute(parserContext, targetType, xmlElementReference, xmlElement, propertyKey) ?? (XmlNode) this.GetChildElement(parserContext, (ITypeId) targetType, xmlElementReference, xmlElement, propertyKey);
    }

    private void SetProperty(XamlParserContext parserContext, DocumentCompositeNodeReference nodeReference, IProperty propertyKey, XamlSourceContext propertyContext, XmlElementReference xmlElementReference, IList<DocumentNode> children)
    {
      IType propertyType = propertyKey.PropertyType;
      IType itemType = propertyType.ItemType;
      int count = children.Count;
      if (itemType != null)
      {
        bool flag = false;
        if (XamlParser.IsPropertyWritable(parserContext, propertyKey, XamlParser.AllowProtectedProperties(parserContext, nodeReference.IsRoot)))
        {
          if (count > 0)
          {
            DocumentNode valueNode = children[0];
            if (valueNode != null && DocumentNode.CanAssignTo(parserContext.TypeResolver, propertyType, valueNode) == CanAssignResult.CanAssign)
              flag = true;
          }
        }
        else if (propertyType.IsArray)
          parserContext.ReportError(XamlParseErrors.PropertyIsNotWritable(xmlElementReference.LineInformation, (IPropertyId) propertyKey));
        if (!flag)
        {
          XamlParser.VerifyPropertyIsUnset(parserContext, xmlElementReference.LineInformation, nodeReference.Node, (IPropertyId) propertyKey);
          DocumentCompositeNode node = parserContext.DocumentContext.CreateNode((ITypeId) propertyType);
          DocumentCompositeNodeReference nodeReference1 = new DocumentCompositeNodeReference((IDocumentNodeReference) nodeReference, node);
          if (XamlParser.CanAssignProperty(parserContext, xmlElementReference.LineInformation, (ITypeId) nodeReference.Type, propertyKey))
            nodeReference.Node.SetValue((IPropertyId) propertyKey, (INodeSourceContext) propertyContext, (DocumentNode) node);
          XamlParser.ContentHandling contentHandling = parserContext.PlatformMetadata.KnownTypes.IDictionary.IsAssignableFrom((ITypeId) propertyType) || parserContext.PlatformMetadata.KnownTypes.ResourceDictionary.IsAssignableFrom((ITypeId) propertyType) ? XamlParser.ContentHandling.DictionaryEntries : XamlParser.ContentHandling.NotDictionaryEntries;
          this.AddChildren(parserContext, nodeReference1, xmlElementReference, contentHandling, children);
          return;
        }
      }
      if (count > 0)
      {
        int index = 0;
        if (count > 1)
        {
          if (propertyKey.IsResolvable)
            parserContext.ReportError(XamlParseErrors.SingleValuedPropertySetMultipleTimes(xmlElementReference.LineInformation, (IPropertyId) propertyKey));
          index = this.parserPolicy.ChooseFromMultiplePropertyValues((IPropertyId) propertyKey, count);
        }
        DocumentNode documentNode = children[index];
        if (documentNode == null)
          return;
        XmlElementReference xmlElementReference1 = documentNode.SourceContext as XmlElementReference;
        if (xmlElementReference1 != null)
          this.ReportUnnecessaryAttributes(parserContext, (IDocumentNodeReference) nodeReference, xmlElementReference1);
        ITextLocation lineInformation = XamlParser.GetLineInformation(documentNode, xmlElementReference.LineInformation);
        if (parserContext.PlatformMetadata.KnownTypes.XData.Equals((object) documentNode.Type))
        {
          if (propertyType.Metadata.SupportsInlineXml)
            this.SetInlineXml(parserContext, lineInformation, nodeReference, propertyKey, documentNode);
          else
            parserContext.ReportError(XamlParseErrors.TypeDoesNotSupportInlineXml(lineInformation, (ITypeId) propertyType));
        }
        else
          XamlParser.SetProperty(parserContext, lineInformation, nodeReference, propertyContext, (IPropertyId) propertyKey, documentNode);
      }
      else
        parserContext.ReportError(XamlParseErrors.PropertyElementWithNoValue(xmlElementReference.LineInformation, (IPropertyId) propertyKey));
    }

    private void SetInlineXml(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, IProperty propertyKey, DocumentNode valueNode)
    {
      IType propertyType = propertyKey.PropertyType;
      DocumentCompositeNode node = parserContext.DocumentContext.CreateNode((ITypeId) propertyType);
      XamlParser.VerifyPropertyIsUnset(parserContext, lineInformation, nodeReference.Node, (IPropertyId) propertyKey);
      nodeReference.Node.Properties[(IPropertyId) propertyKey] = (DocumentNode) node;
      node.Properties[parserContext.PlatformMetadata.KnownProperties.DesignTimeInlineXml] = valueNode;
    }

    private DocumentNode GetAttributeValue(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, IProperty propertyKey, XmlAttribute attribute)
    {
      TypeConverter typeConverter = this.GetTypeConverter(propertyKey);
      return this.GetAttributeValue(parserContext, lineInformation, nodeReference, xmlElementReference, typeConverter, (ITypeId) propertyKey.PropertyType, attribute);
    }

    private DocumentNode GetAttributeValue(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, TypeConverter typeConverter, ITypeId valueType, XmlAttribute attribute)
    {
      string attributeValue = XmlUtilities.GetAttributeValue(attribute);
      return MarkupExtensionParser.GetPropertyValue(parserContext, lineInformation, (IDocumentNodeReference) nodeReference, (IXmlNamespaceResolver) xmlElementReference, typeConverter, parserContext.TypeResolver.ResolveType(valueType), attributeValue, false);
    }

    private DocumentNode ParseStyleOrTemplate(XamlParserContext parserContext, IDocumentNodeReference nodeReference, ITypeId type, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      DocumentCompositeNode node = parserContext.DocumentContext.CreateNode(type);
      node.SourceContext = (INodeSourceContext) xmlElementReference;
      DocumentCompositeNodeReference nodeReference1 = new DocumentCompositeNodeReference(nodeReference, node);
      this.AddPropertiesAndChildren(parserContext, nodeReference1, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (propertyKey =>
      {
        if (propertyKey != null)
          return this.IsTargetTypeProperty((DocumentNode) node, propertyKey);
        return false;
      }));
      nodeReference1.SetTargetType(DocumentNodeHelper.GetStyleOrTemplateTargetType((DocumentNode) node));
      bool isTemplate = parserContext.PlatformMetadata.KnownTypes.FrameworkTemplate.IsAssignableFrom(type);
      IPropertyId visualTreeProperty = (IPropertyId) this.documentContext.TypeResolver.ResolveProperty(parserContext.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree);
      if (isTemplate)
        this.AddPropertiesAndChildren(parserContext, nodeReference1, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (propertyKey =>
        {
          if (propertyKey != null)
            return propertyKey.Equals((object) visualTreeProperty);
          return false;
        }));
      this.AddPropertiesAndChildren(parserContext, nodeReference1, xmlElementReference, xmlElement, (Predicate<IPropertyId>) (propertyKey => propertyKey == null || (!isTemplate || !propertyKey.Equals((object) visualTreeProperty)) && !this.IsTargetTypeProperty((DocumentNode) node, propertyKey)));
      return (DocumentNode) node;
    }

    private bool IsTargetTypeProperty(DocumentNode node, IPropertyId propertyKey)
    {
      ITargetTypeMetadata targetTypeMetadata = node.Type.Metadata as ITargetTypeMetadata;
      if (targetTypeMetadata != null)
        return targetTypeMetadata.TargetTypeProperty.Equals((object) propertyKey);
      return false;
    }

    private DocumentNode ParseDictionaryEntry(XamlParserContext parserContext, IDocumentNodeReference nodeReference, XmlElementReference xmlElementReference, DocumentNode valueNode)
    {
      bool flag = false;
      DocumentCompositeNode node1 = parserContext.DocumentContext.CreateNode(typeof (DictionaryEntry));
      node1.SourceContext = (INodeSourceContext) xmlElementReference;
      DocumentCompositeNodeReference nodeReference1 = new DocumentCompositeNodeReference(nodeReference, node1);
      foreach (XmlElementReference.Attribute attribute in Enumerable.Where<XmlElementReference.Attribute>(xmlElementReference.AttributesToPreserve, (Func<XmlElementReference.Attribute, bool>) (attribute => XmlAttributeEnumerable.NotXmlnsNorDirective(attribute.XmlAttribute))))
      {
        XmlAttribute xmlAttribute = attribute.XmlAttribute;
        string prefix = xmlAttribute.Prefix;
        if (!string.IsNullOrEmpty(prefix) && ((IXmlNamespaceResolver) xmlElementReference).GetXmlNamespace(XmlnsPrefix.ToPrefix(prefix), XmlNamespace.GetNamespaceCanonicalization(parserContext.TypeResolver)) == XmlNamespace.XamlXmlNamespace && xmlAttribute.LocalName == "Key")
        {
          IProperty propertyKey = this.documentContext.TypeResolver.ResolveProperty(parserContext.PlatformMetadata.KnownProperties.DictionaryEntryKey);
          ITextLocation lineInformation = this.GetLineInformation(xmlAttribute.SourceContext);
          DocumentNode node2 = this.SetProperty(parserContext, lineInformation, nodeReference1, xmlElementReference, xmlAttribute, propertyKey);
          flag = parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsEmptyDictionaryKey) || !string.IsNullOrEmpty(DocumentPrimitiveNode.GetValueAsString(node2));
        }
      }
      if (!flag && parserContext.TypeResolver.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
      {
        DocumentCompositeNode documentCompositeNode = valueNode as DocumentCompositeNode;
        if (documentCompositeNode != null)
          flag = documentCompositeNode.Properties[(IPropertyId) documentCompositeNode.Type.Metadata.NameProperty] != null;
      }
      ITextLocation lineInformation1 = xmlElementReference.LineInformation;
      if (!flag && valueNode.Type.Metadata.ImplicitDictionaryKeyProperty == null)
        parserContext.ReportError(XamlParseErrors.MissingDictionaryKey(lineInformation1));
      IPropertyId propertyKey1 = (IPropertyId) this.documentContext.TypeResolver.ResolveProperty(parserContext.PlatformMetadata.KnownProperties.DictionaryEntryValue);
      XamlParser.SetProperty(parserContext, lineInformation1, nodeReference1, (XamlSourceContext) null, propertyKey1, valueNode);
      return (DocumentNode) node1;
    }

    private DocumentNode SetProperty(XamlParserContext parserContext, ITextLocation lineInformation, DocumentCompositeNodeReference nodeReference, XmlElementReference xmlElementReference, XmlAttribute attribute, IProperty propertyKey)
    {
      DocumentNode attributeValue = this.GetAttributeValue(parserContext, lineInformation, nodeReference, xmlElementReference, propertyKey, attribute);
      if (attributeValue != null)
      {
        XamlSourceContext propertyContext = (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext);
        XamlParser.SetProperty(parserContext, lineInformation, nodeReference, propertyContext, (IPropertyId) propertyKey, attributeValue);
      }
      return attributeValue;
    }

    private ITextLocation GetLineInformation(SourceContext sourceContext)
    {
      return this.textBuffer.GetLocation(sourceContext.StartCol);
    }

    private void ReportUnnecessaryAttributes(XamlParserContext parserContext, IDocumentNodeReference nodeReference, XmlElementReference xmlElementReference)
    {
      foreach (XmlElementReference.Attribute attribute in Enumerable.Where<XmlElementReference.Attribute>(xmlElementReference.AttributesToPreserve, (Func<XmlElementReference.Attribute, bool>) (attribute => XmlAttributeEnumerable.NotXmlnsNorDirective(attribute.XmlAttribute))))
      {
        XmlAttribute xmlAttribute = attribute.XmlAttribute;
        string prefix = xmlAttribute.Prefix;
        if (!string.IsNullOrEmpty(prefix) && ((IXmlNamespaceResolver) xmlElementReference).GetXmlNamespace(XmlnsPrefix.ToPrefix(prefix), XmlNamespace.GetNamespaceCanonicalization(parserContext.TypeResolver)) == XmlNamespace.XamlXmlNamespace)
        {
          switch (xmlAttribute.LocalName)
          {
            case "Key":
              if (!XamlParser.CheckContainerType(nodeReference, parserContext.PlatformMetadata.KnownTypes.IDictionary) && !XamlParser.CheckContainerType(nodeReference, parserContext.PlatformMetadata.KnownTypes.ResourceDictionary))
              {
                parserContext.ReportError(XamlParseErrors.AttributeValidInSpecificContainerOnly(xmlElementReference.LineInformation, parserContext.PlatformMetadata.KnownTypes.IDictionary, xmlAttribute.LocalName));
                continue;
              }
              continue;
            case "Shared":
              if (!XamlParser.CheckContainerType(nodeReference, parserContext.PlatformMetadata.KnownTypes.ResourceDictionary))
              {
                parserContext.ReportError(XamlParseErrors.AttributeValidInSpecificContainerOnly(xmlElementReference.LineInformation, parserContext.PlatformMetadata.KnownTypes.ResourceDictionary, xmlAttribute.LocalName));
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }

    private IType GetTypeId(XamlParserContext parserContext, XmlElementReference node, XmlElement xmlElement)
    {
      ITextLocation lineInformation = node.LineInformation;
      XmlnsPrefix prefix = XmlnsPrefix.ToPrefix(xmlElement.Prefix);
      XmlNamespace xmlNamespace = parserContext.GetXmlNamespace(lineInformation, (IXmlNamespaceResolver) node, prefix);
      if (xmlNamespace == null || XamlTypeHelper.IsIgnorable(parserContext, (IXmlNamespaceResolver) node, xmlNamespace))
        return (IType) null;
      string localName = xmlElement.LocalName;
      if (xmlNamespace == XmlNamespace.XamlXmlNamespace && localName == "Code")
        return (IType) null;
      return XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, localName);
    }

    private XamlParser.MemberResolution GetPropertyKey(XamlParserContext parserContext, XmlElementReference xmlElementReference, XmlElement xmlElement, XmlAttribute attribute, IType targetType, bool allowProtectedPropertiesOnTargetType, out IProperty propertyKey)
    {
      ITextLocation lineInformation = this.GetLineInformation(attribute.SourceContext);
      string localName = attribute.LocalName;
      bool flag = localName.IndexOf('.') >= 0;
      XmlnsPrefix prefix = XmlnsPrefix.ToPrefix(attribute.Prefix);
      XmlNamespace xmlNamespace1 = (XmlNamespace) null;
      if (prefix != XmlnsPrefix.EmptyPrefix || flag)
      {
        xmlNamespace1 = parserContext.GetXmlNamespace(lineInformation, (IXmlNamespaceResolver) xmlElementReference, prefix);
        if (xmlNamespace1 == null || XamlTypeHelper.IsIgnorable(parserContext, (IXmlNamespaceResolver) xmlElementReference, xmlNamespace1))
        {
          propertyKey = (IProperty) null;
          return XamlParser.MemberResolution.Ignored;
        }
      }
      MemberType memberTypes = MemberType.Property | MemberType.ClrEvent | MemberType.Field;
      MemberType defaultType = flag ? MemberType.AttachedProperty : MemberType.LocalProperty;
      XmlNamespace xmlNamespace2 = ((IXmlNamespaceResolver) xmlElementReference).GetXmlNamespace(XmlnsPrefix.ToPrefix(xmlElement.Prefix), XmlNamespace.GetNamespaceCanonicalization(parserContext.TypeResolver));
      propertyKey = XamlTypeHelper.GetPropertyKey(parserContext, lineInformation, xmlNamespace1, localName, xmlNamespace2, targetType, memberTypes, defaultType, allowProtectedPropertiesOnTargetType);
      return propertyKey == null ? XamlParser.MemberResolution.Unknown : XamlParser.MemberResolution.Known;
    }

    private int SplitNamespaceAndName(XamlParserContext parserContext, XmlElementReference xmlElementReference, XmlElement xmlElement, out XmlNamespace xmlNamespace, out string typeName, out string memberName)
    {
      XmlnsPrefix prefix = XmlnsPrefix.ToPrefix(xmlElement.Prefix);
      xmlNamespace = parserContext.GetXmlNamespace(xmlElementReference.LineInformation, (IXmlNamespaceResolver) xmlElementReference, prefix);
      typeName = (string) null;
      memberName = (string) null;
      if (xmlNamespace == null)
        return 0;
      string localName = xmlElement.LocalName;
      int length = localName.LastIndexOf('.');
      if (length >= 0)
      {
        ITextLocation lineInformation = xmlElementReference.LineInformation;
        if (length == 0 || length == localName.Length - 1)
        {
          parserContext.ReportError(XamlParseErrors.InvalidTypeQualifiedMemberName(lineInformation, localName));
          return 1;
        }
        typeName = localName.Substring(0, length);
        memberName = localName.Substring(length + 1);
        return 3;
      }
      typeName = localName;
      return 2;
    }

    private IProperty GetPropertyKey(XamlParserContext parserContext, ITextLocation lineInformation, ITypeId parentType, XmlNamespace xmlNamespace, string typeName, string propertyName, bool allowProtectedPropertiesOnParentType)
    {
      IProperty property = (IProperty) null;
      IType typeId = XamlTypeHelper.GetTypeId(parserContext, lineInformation, xmlNamespace, typeName);
      if (typeId != null)
      {
        MemberAccessTypes allowableMemberAccess = TypeHelper.GetAllowableMemberAccess(parserContext.TypeResolver, typeId);
        if (allowProtectedPropertiesOnParentType && typeId.IsAssignableFrom(parentType))
          allowableMemberAccess |= MemberAccessTypes.Protected;
        MemberType memberTypes = typeId.IsAssignableFrom(parentType) ? MemberType.Property | MemberType.Field : MemberType.AttachedProperty;
        property = typeId.GetMember(memberTypes, propertyName, allowableMemberAccess) as IProperty ?? (IProperty) XamlTypeHelper.AddMemberIfPossible(parserContext.PlatformMetadata, typeId, MemberType.LocalProperty, propertyName);
        if (property == null)
          parserContext.ReportError(XamlParseErrors.UnrecognizedOrInaccessibleMember(lineInformation, propertyName));
      }
      return property;
    }

    private XmlElementReference NewXmlElementReference(XamlParserContext parserContext, XmlContainerReference parent, XmlElement xmlElement)
    {
      XmlElementReference xmlElementReference = new XmlElementReference(parent, xmlElement, true);
      this.ParsePreservedAttributes(parserContext, xmlElementReference, xmlElement);
      this.ParsePreservedChildNodes((XmlContainerReference) xmlElementReference, (XmlNode) xmlElement);
      return xmlElementReference;
    }

    private void ParsePreservedAttributes(XamlParserContext parserContext, XmlElementReference xmlElementReference, XmlElement xmlElement)
    {
      List<XmlAttribute> list = (List<XmlAttribute>) null;
      XamlParser.LazilyInstantiatedList<string> instantiatedList = new XamlParser.LazilyInstantiatedList<string>();
      foreach (XmlAttribute attribute1 in XmlAttributeEnumerable.All(xmlElement))
      {
        XmlElementReference.Attribute attribute2 = (XmlElementReference.Attribute) null;
        switch (XmlUtilities.GetProcessingAttributeType(attribute1))
        {
          case XmlProcessingAttributeType.NotProcessingAttribute:
            if (XmlUtilities.IsXmlnsDeclaration(attribute1))
            {
              string xmlnsPrefix = XamlParser.GetXmlnsPrefix(attribute1);
              if (instantiatedList.Contains(xmlnsPrefix))
              {
                parserContext.ReportError(XamlParseErrors.DuplicateXmlnsPrefix((ITextLocation) null, XmlnsPrefix.ToPrefix(xmlnsPrefix)));
                break;
              }
              instantiatedList.Add(xmlnsPrefix);
              attribute2 = new XmlElementReference.Attribute(XmlElementReference.AttributeType.Xmlns, attribute1, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute1.SourceContext));
              XamlParser.ParseClrNamespaceUri(parserContext, attribute1, this.GetLineInformation(attribute1.SourceContext));
              break;
            }
            if (XmlnsPrefix.ToPrefix(attribute1.Prefix) != XmlnsPrefix.EmptyPrefix)
            {
              if (list == null)
                list = new List<XmlAttribute>(1);
              list.Add(attribute1);
              break;
            }
            break;
          case XmlProcessingAttributeType.Unrecognized:
            parserContext.ReportError(XamlParseErrors.UnrecognizedXmlAttribute(this.GetLineInformation(attribute1.SourceContext), attribute1.LocalName));
            break;
          case XmlProcessingAttributeType.Lang:
            attribute2 = new XmlElementReference.Attribute(XmlElementReference.AttributeType.XmlLang, attribute1, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute1.SourceContext));
            break;
          case XmlProcessingAttributeType.Space:
            attribute2 = new XmlElementReference.Attribute(XmlElementReference.AttributeType.XmlSpace, attribute1, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute1.SourceContext));
            string attributeValue = XmlUtilities.GetAttributeValue(attribute1);
            switch (attributeValue)
            {
              case "default":
                xmlElementReference.SetXmlSpace(XmlSpace.Default);
                break;
              case "preserve":
                xmlElementReference.SetXmlSpace(XmlSpace.Preserve);
                break;
              default:
                parserContext.ReportError(XamlParseErrors.InvalidXmlSpace(this.GetLineInformation(attribute1.SourceContext), attributeValue));
                break;
            }
            break;
        }
        if (attribute2 != null)
          xmlElementReference.AddAttributeToPreserve(attribute2);
      }
      if (list == null)
        return;
      foreach (XmlAttribute attribute in list)
      {
        XmlnsPrefix prefix1 = XmlnsPrefix.ToPrefix(attribute.Prefix);
        ITextLocation lineInformation = this.GetLineInformation(attribute.SourceContext);
        XmlNamespace xmlNamespace1 = parserContext.GetXmlNamespace(lineInformation, (IXmlNamespaceResolver) xmlElementReference, prefix1);
        if (xmlNamespace1 == XmlNamespace.CompatibilityXmlNamespace)
        {
          xmlElementReference.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.Compatibility, attribute, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext)));
          string localName = attribute.LocalName;
          if (localName == "Ignorable")
          {
            using (IEnumerator<string> enumerator = XmlUtilities.GetPrefixes(XmlUtilities.GetAttributeValue(attribute)).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                XmlnsPrefix prefix2 = XmlnsPrefix.ToPrefix(enumerator.Current);
                XmlNamespace xmlNamespace2 = ((IXmlNamespaceResolver) xmlElementReference).GetXmlNamespace(prefix2, XmlNamespace.GetNamespaceCanonicalization(parserContext.TypeResolver));
                if (xmlNamespace2 != null)
                  xmlElementReference.AddIgnorableNamespace(xmlNamespace2);
                else
                  parserContext.ReportError(XamlParseErrors.UnrecognizedXmlnsPrefix(lineInformation, prefix2));
              }
              break;
            }
          }
          else
            parserContext.ReportError(XamlParseErrors.UnrecognizedAttribute(lineInformation, xmlNamespace1, localName));
        }
        else if (xmlNamespace1 == XmlNamespace.XamlXmlNamespace)
          xmlElementReference.AddAttributeToPreserve(new XmlElementReference.Attribute(XmlElementReference.AttributeType.XamlNamespace, attribute, (XamlSourceContext) new XmlAttributeReference(xmlElementReference, attribute.SourceContext)));
      }
    }

    private void ParsePreservedChildNodes(XmlContainerReference xmlContainerReference, XmlNode xmlNode)
    {
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node = xmlNode.FirstChild; node != null; node = node.NextNode)
      {
        XmlContainerReference.ChildNode childNode = (XmlContainerReference.ChildNode) null;
        switch (node.NodeType)
        {
          case NodeType.XmlDeclaration:
            childNode = new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.Declaration, xmlContainerReference, node);
            break;
          case NodeType.XmlElement:
            XmlElement xmlElement = (XmlElement) node;
            if (xmlElement.LocalName == "Code" && xmlElement.LookupNamespace(xmlElement.Prefix) == "http://schemas.microsoft.com/winfx/2006/xaml")
            {
              childNode = new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.Code, xmlContainerReference, node);
              break;
            }
            break;
          case NodeType.XmlComment:
            childNode = new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.Comment, xmlContainerReference, node);
            break;
          case NodeType.XmlProcessingInstruction:
            childNode = new XmlContainerReference.ChildNode(XmlContainerReference.ChildType.ProcessingInstruction, xmlContainerReference, node);
            break;
        }
        if (childNode != null)
          xmlContainerReference.AddChildNode(childNode);
      }
    }

    public static bool TryParseClrNamespaceUri(string mapping, out string clrNamespace, out string assemblyName)
    {
      clrNamespace = (string) null;
      assemblyName = (string) null;
      if (!mapping.StartsWith("clr-namespace:", StringComparison.Ordinal))
        return false;
      int length = "clr-namespace:".Length;
      for (int index = length; index < mapping.Length; ++index)
      {
        if (Scanner.IsXmlWhitespace(mapping[index]))
          return false;
      }
      int num = mapping.IndexOf(';', length);
      if (num < 0)
      {
        clrNamespace = mapping.Substring(length);
      }
      else
      {
        if (string.Compare(mapping, num + 1, "assembly=", 0, "assembly=".Length, StringComparison.Ordinal) != 0)
          return false;
        clrNamespace = mapping.Substring(length, num - length);
        assemblyName = mapping.Substring(num + 1 + "assembly=".Length);
      }
      return true;
    }

    public static IList<IProperty> ParsePropertyPathParameters(DocumentNode siteNode, string valueString)
    {
      ICollection<DocumentNode> ancestorNodes;
      IXmlNamespaceResolver xmlNamespaceResolver;
      IDocumentNodeReference nodeReference;
      string propertyPath;
      IList<IProperty> pathParameters;
      MarkupExtensionParser.ParsePropertyPathParameters(XamlParser.CreateContext(siteNode.DocumentRoot, siteNode, out ancestorNodes, out xmlNamespaceResolver, out nodeReference), (ITextLocation) null, xmlNamespaceResolver, valueString, out propertyPath, out pathParameters);
      return pathParameters;
    }

    private static void ParseClrNamespaceUri(XamlParserContext parserContext, XmlAttribute attribute, ITextLocation lineInformation)
    {
      string attributeValue = XmlUtilities.GetAttributeValue(attribute);
      if (!attributeValue.StartsWith("clr-namespace:", StringComparison.Ordinal))
        return;
      AssemblyNamespace assemblyNamespace = (AssemblyNamespace) null;
      if (parserContext.DocumentNamespaces.GetNamespace((IXmlNamespace) XmlNamespace.ToNamespace(attributeValue, XmlNamespace.GetNamespaceCanonicalization(parserContext.TypeResolver)), out assemblyNamespace))
        return;
      parserContext.ReportError(XamlParseErrors.InvalidClrNamespaceUri(lineInformation, attributeValue));
    }

    private static bool SupportsContentType(ITypeResolver typeResolver, IType containerType, IType childType)
    {
      IType itemType = containerType.ItemType;
      if (itemType == null)
        return containerType.IsAssignableFrom((ITypeId) childType);
      return itemType.IsAssignableFrom((ITypeId) childType) || (ITypeId) XamlParser.GetContentWrapperType(typeResolver, containerType, childType) != null;
    }

    private static IType GetContentWrapperType(ITypeResolver typeResolver, IType parentType, IType childType)
    {
      return typeResolver.PlatformMetadata.GetContentWrapperType(typeResolver, parentType, childType);
    }

    private static bool IsValidTypeName(string typeName)
    {
      int num;
      for (int index = -1; index < typeName.Length; index = num)
      {
        num = typeName.IndexOf('.', index + 1);
        if (num < 0)
          num = typeName.Length;
        if (num - index == 1)
          return false;
      }
      return true;
    }

    private TypeConverter GetTypeConverter(IProperty propertyKey)
    {
      return propertyKey.TypeConverter ?? this.documentContext.TypeResolver.ResolveType((ITypeId) propertyKey.PropertyType).TypeConverter;
    }

    private static ITextLocation GetLineInformation(DocumentNode node, ITextLocation fallbackValue)
    {
      XamlSourceContext xamlSourceContext = node.SourceContext as XamlSourceContext;
      if (xamlSourceContext != null)
        return xamlSourceContext.LineInformation;
      return fallbackValue;
    }

    private enum ElementType
    {
      Unknown,
      Type,
      Code,
    }

    private enum ContentHandling
    {
      DictionaryEntries,
      NotDictionaryEntries,
    }

    private enum PropertyHandling
    {
      PropertiesIgnored,
      PropertiesNotAllowed,
    }

    private struct LazilyInstantiatedList<T>
    {
      private IList<T> list;

      public IList<T> List
      {
        get
        {
          return this.list;
        }
      }

      public int Count
      {
        get
        {
          if (this.list == null)
            return 0;
          return this.list.Count;
        }
      }

      public bool Contains(T item)
      {
        if (this.list == null)
          return false;
        return this.list.Contains(item);
      }

      public void Add(T item)
      {
        if (this.list == null)
          this.list = (IList<T>) new List<T>();
        this.list.Add(item);
      }
    }

    private enum MemberResolution
    {
      Unknown,
      Known,
      Ignored,
      Skipped,
    }

    private sealed class XmlElementReferenceChildEnumerator
    {
      private static XamlParser.XmlElementReferenceChildEnumerator.SentinelNode sentinelNode = new XamlParser.XmlElementReferenceChildEnumerator.SentinelNode();
      private Microsoft.Expression.DesignModel.Markup.Xml.Node currentChildNode = (Microsoft.Expression.DesignModel.Markup.Xml.Node) XamlParser.XmlElementReferenceChildEnumerator.sentinelNode;
      private XmlElementReference xmlElementReference;
      private XmlElement xmlElement;
      private XmlComment precedingComment;
      private int childToPreserveIndex;

      public Microsoft.Expression.DesignModel.Markup.Xml.Node Current
      {
        get
        {
          if (this.currentChildNode == XamlParser.XmlElementReferenceChildEnumerator.sentinelNode)
            return (Microsoft.Expression.DesignModel.Markup.Xml.Node) null;
          return this.currentChildNode;
        }
      }

      public XmlElementReferenceChildEnumerator(XmlElementReference xmlElementReference, XmlElement xmlElement)
      {
        this.xmlElementReference = xmlElementReference;
        this.xmlElement = xmlElement;
      }

      public bool MoveNext()
      {
        Microsoft.Expression.DesignModel.Markup.Xml.Node current = this.Current;
        if (current != null)
        {
          switch (current.NodeType)
          {
            case NodeType.WhitespaceLiteral:
              break;
            case NodeType.XmlComment:
              this.precedingComment = (XmlComment) current;
              break;
            default:
              this.precedingComment = (XmlComment) null;
              break;
          }
        }
        this.currentChildNode = this.currentChildNode != XamlParser.XmlElementReferenceChildEnumerator.sentinelNode ? this.currentChildNode.NextNode : this.xmlElement.FirstChild;
        return this.currentChildNode != null;
      }

      public bool MovePrecedingCommentToChild(XmlElementReference childReference)
      {
        if (this.precedingComment != null)
        {
          for (; this.childToPreserveIndex < this.xmlElementReference.ChildNodesToPreserveCount; ++this.childToPreserveIndex)
          {
            XmlContainerReference.ChildNode childNodeAt = this.xmlElementReference.GetChildNodeAt(this.childToPreserveIndex);
            if (childNodeAt.SourceContext.TextRange.Offset == this.precedingComment.SourceContext.StartCol)
            {
              childReference.Comment = childNodeAt;
              this.xmlElementReference.RemoveChildNodeAt(this.childToPreserveIndex);
              return true;
            }
          }
        }
        return false;
      }

      private class SentinelNode : Microsoft.Expression.DesignModel.Markup.Xml.Node
      {
        public SentinelNode()
          : base(NodeType.Error)
        {
        }
      }
    }
  }
}
