// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlDocument
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Collections;
using System.Xml.Schema;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlDocument : XmlNode
  {
    internal XmlDeclaration XmlDecl;
    public string Location;
    public XmlElement DocumentElement;

    public string Encoding
    {
      get
      {
        if (this.XmlDecl != null && this.XmlDecl.Encoding != null)
          return this.XmlDecl.Encoding;
        return (string) null;
      }
    }

    public string RootNamespaceURI
    {
      get
      {
        if (this.DocumentElement == null || this.DocumentElement.Name == null)
          return (string) null;
        Identifier identifier = this.DocumentElement.Name;
        if (identifier.NamespaceURI != null)
          return identifier.NamespaceURI.Name;
        return "";
      }
    }

    public XmlDocument()
      : base(NodeType.XmlDocument)
    {
    }

    protected XmlDocument(NodeType nt)
      : base(nt)
    {
    }

    internal static void AddBrackets(MemberList list)
    {
      int index = 0;
      for (int length = list.Length; index < length; ++index)
      {
        Member member = list[index];
        Identifier identifier1 = member.Name;
        if (identifier1.Prefix != null)
        {
          identifier1.Prefix = Identifier.For("<" + identifier1.Prefix.Name);
        }
        else
        {
          Identifier identifier2 = Identifier.For("<" + identifier1.Name);
          identifier2.NamespaceURI = identifier1.NamespaceURI;
          member.Name = identifier2;
        }
      }
    }

    public static bool IsSameUri(string a, string b)
    {
      try
      {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
          return a == b;
        Uri uri1 = new Uri(a);
        Uri uri2 = new Uri(b);
        return a == b;
      }
      catch
      {
        return false;
      }
    }

    internal SourceContext GetBestContext(Node found, Node scope)
    {
      Node node = found == null ? scope : found;
      if (node is XmlNode)
      {
        XmlNode xmlNode = (XmlNode) node;
        if (xmlNode.Name != null)
          return xmlNode.Name.SourceContext;
      }
      return node.SourceContext;
    }

    internal XmlSchemaObject FindSchemaAttribute(XmlSchema s, string localName)
    {
      if (s.Attributes == null)
        return (XmlSchemaObject) null;
      foreach (XmlSchemaObject xmlSchemaObject in (IEnumerable) s.Attributes.Values)
      {
        if (xmlSchemaObject is XmlSchemaAttribute)
        {
          XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute) xmlSchemaObject;
          if (xmlSchemaAttribute.Name == localName)
            return (XmlSchemaObject) xmlSchemaAttribute;
        }
      }
      return (XmlSchemaObject) null;
    }

    internal XmlSchemaObject FindSchemaAttributeGroup(XmlSchema s, string localName)
    {
      if (s.AttributeGroups == null)
        return (XmlSchemaObject) null;
      foreach (XmlSchemaAttributeGroup schemaAttributeGroup in (IEnumerable) s.AttributeGroups.Values)
      {
        if (schemaAttributeGroup.Name == localName)
          return (XmlSchemaObject) schemaAttributeGroup;
      }
      return (XmlSchemaObject) null;
    }

    internal XmlSchemaObject FindSchemaType(XmlSchema s, string localNname)
    {
      if (s.SchemaTypes == null)
        return (XmlSchemaObject) null;
      foreach (XmlSchemaObject xmlSchemaObject in (IEnumerable) s.SchemaTypes.Values)
      {
        if (xmlSchemaObject is XmlSchemaSimpleType)
        {
          XmlSchemaSimpleType schemaSimpleType = (XmlSchemaSimpleType) xmlSchemaObject;
          if (schemaSimpleType.Name == localNname)
            return (XmlSchemaObject) schemaSimpleType;
        }
        else if (xmlSchemaObject is XmlSchemaComplexType)
        {
          XmlSchemaComplexType schemaComplexType = (XmlSchemaComplexType) xmlSchemaObject;
          if (schemaComplexType.Name == localNname)
            return (XmlSchemaObject) schemaComplexType;
        }
      }
      return (XmlSchemaObject) null;
    }

    internal XmlSchemaObject FindSchemaElement(XmlSchema s, string localName)
    {
      if (s.Elements == null)
        return (XmlSchemaObject) null;
      foreach (XmlSchemaObject xmlSchemaObject in (IEnumerable) s.Elements.Values)
      {
        if (xmlSchemaObject is XmlSchemaElement)
        {
          XmlSchemaElement xmlSchemaElement = (XmlSchemaElement) xmlSchemaObject;
          if (xmlSchemaElement.Name == localName)
            return (XmlSchemaObject) xmlSchemaElement;
        }
      }
      return (XmlSchemaObject) null;
    }

    internal XmlSchemaObject FindSchemaGroup(XmlSchema s, string localName)
    {
      if (s.Groups == null)
        return (XmlSchemaObject) null;
      foreach (XmlSchemaObject xmlSchemaObject in (IEnumerable) s.Groups.Values)
      {
        if (xmlSchemaObject is XmlSchemaGroup)
        {
          XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup) xmlSchemaObject;
          if (xmlSchemaGroup.Name == localName)
            return (XmlSchemaObject) xmlSchemaGroup;
        }
      }
      return (XmlSchemaObject) null;
    }
  }
}
