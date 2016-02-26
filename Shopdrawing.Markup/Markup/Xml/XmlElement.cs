// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlElement
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlElement : XmlNode
  {
    internal Node firstAttribute;
    internal Node lastAttribute;
    internal Identifier endTag;
    internal SourceContext endTagContext;
    internal SourceContext startTagContext;

    internal Node FirstAttribute
    {
      get
      {
        return this.firstAttribute;
      }
    }

    public bool HasAttributes
    {
      get
      {
        return this.firstAttribute != null;
      }
    }

    internal SourceContext StartTagContext
    {
      get
      {
        return this.startTagContext;
      }
      set
      {
        this.startTagContext = value;
      }
    }

    internal SourceContext EndTagContext
    {
      get
      {
        return this.endTagContext;
      }
      set
      {
        this.endTagContext = value;
      }
    }

    internal XmlElement(XmlDocument doc)
      : base(NodeType.XmlElement)
    {
      this.doc = doc;
    }

    internal XmlElement(XmlDocument doc, Identifier name)
      : base(NodeType.XmlElement)
    {
      this.doc = doc;
      this.Name = name;
    }

    internal void AddAttribute(XmlAttribute a)
    {
      a.Parent = (XmlNode) this;
      if (this.firstAttribute == null)
      {
        this.firstAttribute = (Node) a;
        this.lastAttribute = (Node) a;
      }
      else
      {
        this.lastAttribute.NextNode = (Node) a;
        this.lastAttribute = (Node) a;
      }
    }

    public override string LookupNamespace(string prefix)
    {
      if (this.scope != null)
        return this.scope.LookupNamespace(prefix);
      return base.LookupNamespace(prefix);
    }

    public override string LookupPrefix(string nsuri)
    {
      if (this.scope != null)
        return this.scope.LookupPrefix(nsuri, "");
      return base.LookupPrefix(nsuri);
    }
  }
}
