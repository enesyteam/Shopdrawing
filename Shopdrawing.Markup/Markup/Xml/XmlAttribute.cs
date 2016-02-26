// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlAttribute
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlAttribute : XmlNode
  {
    public bool hasAssign;
    public char quoteChar;

    public XmlAttribute()
      : base(NodeType.XmlAttribute)
    {
    }

    public XmlAttribute(XmlElement parent)
      : base(NodeType.XmlAttribute)
    {
      this.Parent = (XmlNode) parent;
    }

    internal XmlAttribute(Identifier name)
      : base(NodeType.XmlAttribute)
    {
      this.Name = name;
    }
  }
}
