// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlnsPrefixAndName
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XmlnsPrefixAndName
  {
    private XmlnsPrefix prefix;
    private string name;

    public XmlnsPrefix Prefix
    {
      get
      {
        return this.prefix;
      }
    }

    public string TypeQualifiedName
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
        if (this.prefix == null || this.prefix == XmlnsPrefix.EmptyPrefix)
          return this.name;
        return this.prefix.Value + ":" + this.name;
      }
    }

    public XmlnsPrefixAndName(string name)
      : this((XmlnsPrefix) null, name)
    {
    }

    public XmlnsPrefixAndName(XmlnsPrefix prefix, string name)
    {
      this.prefix = prefix;
      this.name = name;
    }

    public XmlnsPrefixAndName(XmlnsPrefix prefix, string typeQualifier, string name)
      : this(prefix, typeQualifier + "." + name)
    {
    }

    public override string ToString()
    {
      return this.FullName;
    }
  }
}
