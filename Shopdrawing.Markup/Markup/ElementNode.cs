// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ElementNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class ElementNode : ContainerNode
  {
    public static readonly XmlnsPrefixAndName XmlSpaceName = new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix("xml"), "space");
    public static readonly XmlnsPrefixAndName XmlLangName = new XmlnsPrefixAndName(XmlnsPrefix.ToPrefix("xml"), "lang");
    private XmlNamespaceMap namespaces;
    private XmlnsPrefixAndName name;
    private FormattedNodeCollection attributes;
    private bool whitespaceSignificant;

    public bool HasNamespaces
    {
      get
      {
        return this.namespaces != null;
      }
    }

    public XmlnsPrefixAndName Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    public FormattedNodeCollection Attributes
    {
      get
      {
        return this.attributes;
      }
    }

    public bool IsWhitespaceSignificant
    {
      get
      {
        return this.whitespaceSignificant;
      }
      set
      {
        this.whitespaceSignificant = value;
      }
    }

    public XmlSpace XmlSpace
    {
      get
      {
        AttributeNode xmlSpaceAttribute = this.GetXmlSpaceAttribute();
        if (xmlSpaceAttribute != null)
          return ElementNode.ToXmlSpace(xmlSpaceAttribute.Value);
        ElementNode elementNode = this.Parent as ElementNode;
        if (elementNode != null)
          return elementNode.XmlSpace;
        return XmlSpace.Default;
      }
    }

    public override NodeType NodeType
    {
      get
      {
        return NodeType.XmlElement;
      }
    }

    public ElementNode(SourceContextReference sourceContextReference, int ordering)
      : base(sourceContextReference, ordering)
    {
      this.attributes = new FormattedNodeCollection((ContainerNode) this);
    }

    public void AddNamespace(XmlnsPrefix prefix, IXmlNamespace xmlNamespace)
    {
      if (this.namespaces == null)
        this.namespaces = new XmlNamespaceMap();
      this.namespaces.AddNamespace(prefix, xmlNamespace);
    }

    public IXmlNamespace GetNamespace(XmlnsPrefix prefix)
    {
      if (this.HasNamespaces)
      {
        IXmlNamespace @namespace = this.namespaces.GetNamespace(prefix);
        if (@namespace != null)
          return @namespace;
      }
      ElementNode elementNode = this.Parent as ElementNode;
      if (elementNode != null)
        return elementNode.GetNamespace(prefix);
      return (IXmlNamespace) null;
    }

    public XmlnsPrefix GetPrefix(IXmlNamespace xmlNamespace)
    {
      if (this.HasNamespaces)
      {
        XmlnsPrefix prefix = this.namespaces.GetPrefix(xmlNamespace);
        if (prefix != null)
          return prefix;
      }
      ElementNode elementNode = this.Parent as ElementNode;
      if (elementNode != null)
        return elementNode.GetPrefix(xmlNamespace);
      return (XmlnsPrefix) null;
    }

    public IEnumerable<KeyValuePair<XmlnsPrefix, IXmlNamespace>> GetNamespacePrefixPairs()
    {
      for (ElementNode elementNode = this; elementNode != null; elementNode = elementNode.Parent as ElementNode)
      {
        if (elementNode.HasNamespaces)
        {
          foreach (KeyValuePair<XmlnsPrefix, IXmlNamespace> keyValuePair in elementNode.namespaces)
            yield return keyValuePair;
        }
      }
    }

    public AttributeNode GetXmlSpaceAttribute()
    {
      for (int index = 0; index < this.attributes.Count; ++index)
      {
        AttributeNode attributeNode = this.attributes[index] as AttributeNode;
        if (attributeNode != null)
        {
          XmlnsPrefixAndName name = attributeNode.Name;
          if (object.Equals((object) name.Prefix, (object) ElementNode.XmlSpaceName.Prefix) && name.TypeQualifiedName == ElementNode.XmlSpaceName.TypeQualifiedName)
            return attributeNode;
        }
      }
      return (AttributeNode) null;
    }

    public override string ToString()
    {
      if (this.name != null)
        return this.name.FullName;
      return base.ToString();
    }

    public static string ToString(XmlSpace xmlSpace)
    {
      return xmlSpace != XmlSpace.Default ? "preserve" : "default";
    }

    public static XmlSpace ToXmlSpace(string value)
    {
      return !(value == "default") ? XmlSpace.Preserve : XmlSpace.Default;
    }
  }
}
