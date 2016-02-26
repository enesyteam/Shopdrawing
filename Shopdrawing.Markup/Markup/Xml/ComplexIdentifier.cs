// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.ComplexIdentifier
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal sealed class ComplexIdentifier : Identifier
  {
    public static Identifier Empty = new Identifier("");
    private Identifier prefix;
    private Identifier namespaceURI;

    public override Identifier Prefix
    {
      get
      {
        return this.prefix;
      }
      set
      {
        this.prefix = value;
      }
    }

    public override Identifier NamespaceURI
    {
      get
      {
        return this.namespaceURI;
      }
      set
      {
        this.namespaceURI = value;
      }
    }

    public ComplexIdentifier(DocumentText text, int offset, int length)
    {
      this.name = text.Substring(offset, length);
    }

    public ComplexIdentifier(string name)
    {
      if (name == null)
        name = "";
      this.name = name;
    }

    public ComplexIdentifier(Identifier identifier)
    {
      this.name = identifier.Name;
      this.SourceContext = identifier.SourceContext;
    }

    public static ComplexIdentifier ComplexIdentifierFor(DocumentText text, int offset, int length)
    {
      return new ComplexIdentifier(text, offset, length);
    }

    public static ComplexIdentifier ComplexIdentifierFor(string name)
    {
      return new ComplexIdentifier(name);
    }
  }
}
