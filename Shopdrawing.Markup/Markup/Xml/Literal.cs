// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Literal
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class Literal : Node
  {
    public string Value;

    public Literal()
      : base(NodeType.Literal)
    {
    }

    public Literal(string Value)
      : base(NodeType.Literal)
    {
      this.Value = Value;
    }

    public Literal(string Value, SourceContext sourceContext)
      : base(NodeType.Literal)
    {
      this.Value = Value;
      this.SourceContext = sourceContext;
    }

    public override string ToString()
    {
      if (this.Value == null)
        return "";
      return this.Value.ToString();
    }

    public Literal Substring(int start, int len)
    {
      if (this.Value == null)
        return (Literal) null;
      string str = this.Value.ToString();
      SourceContext sourceContext = this.SourceContext;
      sourceContext.StartCol += start;
      sourceContext.EndCol = sourceContext.StartCol + len;
      return new Literal(str.Substring(start, len), sourceContext);
    }
  }
}
