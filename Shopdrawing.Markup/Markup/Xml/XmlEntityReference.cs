// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.XmlEntityReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Text;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class XmlEntityReference : XmlNode
  {
    public char value;

    internal override Literal Value
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        this.GetLiteralValue(sb);
        string str = sb.ToString();
        if (str == null)
          return (Literal) null;
        Literal literal = new Literal();
        literal.SourceContext = this.SourceContext;
        literal.Value = str;
        return literal;
      }
    }

    public XmlEntityReference(XmlDocument doc, Identifier name)
      : base(NodeType.XmlEntityReference)
    {
      this.Name = name;
      this.doc = doc;
    }

    public XmlEntityReference(XmlDocument doc, char ch)
      : base(NodeType.XmlEntityReference)
    {
      this.value = ch;
      this.doc = doc;
    }

    public override void GetLiteralValue(StringBuilder sb)
    {
      if ((int) this.value == 0)
        return;
      sb.Append(this.value);
    }
  }
}
