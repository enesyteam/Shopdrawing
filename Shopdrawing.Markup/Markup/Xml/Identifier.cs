// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Identifier
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class Identifier : Node
  {
    protected string name;

    public virtual Identifier Prefix
    {
      get
      {
        return (Identifier) null;
      }
      set
      {
        throw new InvalidOperationException("Cannot set Prefix on a SimpleIdentifier");
      }
    }

    public virtual Identifier NamespaceURI
    {
      get
      {
        return (Identifier) null;
      }
      set
      {
        throw new InvalidOperationException("Cannot set NamespaceURI on a SimpleIdentifier");
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string FullName
    {
      get
      {
        if (this.Prefix == null || string.IsNullOrEmpty(this.Prefix.Name))
          return this.Name;
        return this.Prefix.Name + ":" + this.Name;
      }
    }

    public Identifier()
      : base(NodeType.Identifier)
    {
    }

    public Identifier(DocumentText text, int offset, int length)
      : base(NodeType.Identifier)
    {
      this.name = text.Substring(offset, length);
    }

    public Identifier(string name)
      : base(NodeType.Identifier)
    {
      this.name = name;
    }

    public static Identifier For(DocumentText text, int offset, int length)
    {
      return new Identifier(text, offset, length);
    }

    public static Identifier For(string name)
    {
      return new Identifier(name);
    }

    public override string ToString()
    {
      return this.FullName;
    }
  }
}
