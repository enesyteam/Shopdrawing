// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Text;
using System.Xml;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal abstract class XmlNode : Member
  {
    internal XmlDocument doc;
    private Node firstChild;
    private Node lastChild;
    public XmlNode Parent;
    internal XmlNamespaceScope scope;

    public bool HasChildNodes
    {
      get
      {
        return this.FirstChild != null;
      }
    }

    internal override Node FirstChild
    {
      get
      {
        return this.firstChild;
      }
    }

    public string NamespaceURI
    {
      get
      {
        if (this.Name != null && this.Name.NamespaceURI != null)
          return this.Name.NamespaceURI.Name;
        return string.Empty;
      }
    }

    public string LocalName
    {
      get
      {
        if (this.Name != null)
          return this.Name.Name;
        return string.Empty;
      }
    }

    public string Prefix
    {
      get
      {
        if (this.Name != null && this.Name.Prefix != null)
          return this.Name.Prefix.Name;
        return string.Empty;
      }
    }

    public bool HasValue
    {
      get
      {
        for (Node node = this.firstChild; node != null; node = node.NextNode)
        {
          if (node is Literal)
            return true;
        }
        return false;
      }
    }

    internal virtual Literal Value
    {
      get
      {
        if (this.firstChild == null)
        {
          Literal literal = new Literal();
          literal.SourceContext = this.SourceContext;
          literal.SourceContext.StartCol = literal.SourceContext.EndCol;
          literal.Value = string.Empty;
          return literal;
        }
        if (this.firstChild != null && this.firstChild.NextNode == null && this.firstChild is Literal)
          return this.firstChild as Literal;
        Literal literal1 = new Literal();
        StringBuilder stringBuilder = new StringBuilder();
        for (Node node = this.firstChild; node != null; node = node.NextNode)
        {
          if (node is Literal)
          {
            Literal literal2 = (Literal) node;
            if (node == this.firstChild)
              return literal2;
            stringBuilder.Append(literal2.Value.ToString());
            if (literal1.SourceContext.Document == null)
              literal1.SourceContext = literal2.SourceContext;
            else
              literal1.SourceContext.EndCol = literal2.SourceContext.EndCol;
          }
          else if (node is XmlEntityReference)
          {
            XmlEntityReference xmlEntityReference = (XmlEntityReference) node;
            stringBuilder.Append(xmlEntityReference.Value.ToString());
            literal1.SourceContext.EndCol = xmlEntityReference.SourceContext.EndCol;
          }
        }
        literal1.Value = stringBuilder.ToString();
        return literal1;
      }
    }

    public virtual string LiteralValue
    {
      get
      {
        if (!this.HasChildNodes)
          return "";
        if (this.firstChild != null && this.firstChild.NextNode == null)
        {
          Literal literal = this.firstChild as Literal;
          if (literal != null)
          {
            if (literal.Value == null)
              return "";
            string str = literal.Value;
            if (str != null)
              return str;
          }
        }
        StringBuilder sb = new StringBuilder();
        this.GetLiteralValue(sb);
        return sb.ToString();
      }
    }

    internal XmlNode(NodeType nt)
      : base(nt)
    {
    }

    internal XmlNode(NodeType nt, XmlDocument doc)
      : base(nt)
    {
      this.doc = doc;
    }

    internal static bool QualifiedNameMatches(Identifier a, Identifier b)
    {
      if (a == b)
        return true;
      if (a != null && b != null && (a.Name == b.Name && XmlNode.QualifiedNameMatches(a.Prefix, b.Prefix)))
        return XmlNode.QualifiedNameMatches(a.NamespaceURI, b.NamespaceURI);
      return false;
    }

    public void AddChild(Node e)
    {
      XmlNode xmlNode = e as XmlNode;
      if (xmlNode != null)
        xmlNode.Parent = this;
      if (this.firstChild == null)
      {
        this.firstChild = e;
        this.lastChild = e;
      }
      else
      {
        this.lastChild.NextNode = e;
        this.lastChild = e;
      }
    }

    public bool RemoveChild(Node toRemove)
    {
      if (this.firstChild == null)
        return false;
      Node node1 = (Node) null;
      if (this.firstChild == toRemove)
      {
        node1 = this.firstChild;
        this.firstChild = node1.NextNode;
        if (this.firstChild == null)
          this.lastChild = (Node) null;
      }
      else
      {
        Node node2 = this.firstChild;
        Node nextNode;
        for (nextNode = this.firstChild.NextNode; nextNode != null && nextNode != toRemove; nextNode = nextNode.NextNode)
          node2 = nextNode;
        if (nextNode != null)
        {
          node1 = nextNode;
          node2.NextNode = node1.NextNode;
          if (node1 == this.lastChild)
            this.lastChild = node2;
        }
      }
      if (node1 == null)
        return false;
      node1.NextNode = (Node) null;
      XmlNode xmlNode = node1 as XmlNode;
      if (xmlNode != null)
        xmlNode.Parent = (XmlNode) null;
      return true;
    }

    internal Identifier QualifyName(string name)
    {
      Identifier identifier = this.ParseQualifiedIdentifier(name);
      if (identifier.Prefix == null && this.Name != null && this.Name.Prefix != null)
        identifier.Prefix = this.Name.Prefix;
      if (identifier.NamespaceURI == null && this.Name != null && this.Name.NamespaceURI != null)
        identifier.NamespaceURI = this.Name.NamespaceURI;
      return identifier;
    }

    internal Identifier ParseQualifiedIdentifier(string name)
    {
      int length = name.IndexOf(':');
      Identifier identifier = Identifier.For(name);
      string str = "";
      if (length > 0)
      {
        identifier = Identifier.For(name.Substring(length + 1));
        str = name.Substring(0, length);
        identifier.Prefix = Identifier.For(str);
      }
      string name1 = this.LookupNamespace(str);
      if (name1 != null)
        identifier.NamespaceURI = Identifier.For(name1);
      return identifier;
    }

    internal Identifier GetQualifiedIdentifier(string localName, string nsuri)
    {
      Identifier identifier = Identifier.For(localName);
      if (nsuri != null)
      {
        identifier.NamespaceURI = Identifier.For(nsuri);
        string name = this.LookupPrefix(nsuri);
        identifier.Prefix = name == null ? Identifier.For("") : Identifier.For(name);
      }
      return identifier;
    }

    public virtual string LookupNamespace(string prefix)
    {
      if (this.Parent != null)
        return this.Parent.LookupNamespace(prefix);
      return (string) null;
    }

    public virtual string LookupPrefix(string nsuri)
    {
      if (this.Parent != null)
        return this.Parent.LookupPrefix(nsuri);
      return (string) null;
    }

    internal Identifier QualifiedIdentifier(XmlQualifiedName name)
    {
      Identifier identifier = Identifier.For(name.Name);
      if (name.Namespace != null)
      {
        identifier.NamespaceURI = Identifier.For(name.Namespace);
        string name1 = this.LookupPrefix(name.Namespace);
        if (name1 != null && name1 != "")
          identifier.Prefix = Identifier.For(name1);
      }
      return identifier;
    }

    internal bool NameMatches(XmlQualifiedName name, Identifier id)
    {
      if (name.Name == id.Name)
        return this.NamespaceMatches(name, id);
      return false;
    }

    internal bool NamespaceMatches(XmlQualifiedName name, Identifier id)
    {
      return (name.Namespace == null || name.Namespace == "") && (id.NamespaceURI == null || id.NamespaceURI.Name == null || id.NamespaceURI.Name == "") || id.NamespaceURI != null && name.Namespace == id.NamespaceURI.Name;
    }

    internal Identifier GetQualifiedIdentifier(XmlNamespaceScope scope, XmlQualifiedName qn, XmlNode parent)
    {
      string name1 = qn.Name;
      string @namespace = qn.Namespace;
      string def = "";
      while (parent != null && parent.Name == null)
        parent = parent.Parent;
      if (parent != null && parent.Name != null)
      {
        Identifier identifier = parent.Name;
        if (identifier.Prefix != null && identifier.Prefix.Name != null)
          def = identifier.Prefix.Name;
      }
      string name2 = scope.LookupPrefix(@namespace, def);
      Identifier identifier1 = Identifier.For(name1);
      if (!string.IsNullOrEmpty(name2))
        identifier1.Prefix = Identifier.For(name2);
      if (!string.IsNullOrEmpty(@namespace))
        identifier1.NamespaceURI = Identifier.For(@namespace);
      return identifier1;
    }

    public virtual void GetLiteralValue(StringBuilder sb)
    {
      for (Node node = this.firstChild; node != null; node = node.NextNode)
      {
        if (node is Literal)
          sb.Append(((Literal) node).Value.ToString());
        else if (node is XmlEntityReference)
          ((XmlNode) node).GetLiteralValue(sb);
      }
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.Prefix))
        return this.Prefix + ":" + this.LocalName;
      return this.LocalName;
    }
  }
}
