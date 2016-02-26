// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.AttributeNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal class AttributeNode : FormattedNode
  {
    private XmlnsPrefixAndName name;
    private string value;
    private readonly bool escapeWhitespace;

    public XmlnsPrefixAndName Name
    {
      get
      {
        return this.name;
      }
    }

    public string Value
    {
      get
      {
        return this.value;
      }
      set
      {
        this.value = value;
      }
    }

    public bool EscapeWhitespace
    {
      get
      {
        return this.escapeWhitespace;
      }
    }

    public override NodeType NodeType
    {
      get
      {
        return NodeType.XmlAttribute;
      }
    }

    public AttributeNode(XmlnsPrefixAndName name, string value)
      : this((SourceContextReference) null, int.MaxValue, name, value)
    {
    }

    public AttributeNode(SourceContextReference sourceContextReference, int ordering, XmlnsPrefixAndName name, string value)
      : this(sourceContextReference, ordering, name, value, true)
    {
    }

    public AttributeNode(SourceContextReference sourceContextReference, int ordering, XmlnsPrefixAndName name, string value, bool escapeWhitespace)
      : base(sourceContextReference, ordering)
    {
      this.name = name;
      this.value = value;
      this.escapeWhitespace = escapeWhitespace;
    }

    public override string ToString()
    {
      return string.Concat(new object[4]
      {
        (object) this.name,
        (object) "=\"",
        (object) this.value,
        (object) "\""
      });
    }
  }
}
