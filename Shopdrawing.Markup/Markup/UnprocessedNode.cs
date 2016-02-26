// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.UnprocessedNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class UnprocessedNode : FormattedNode
  {
    private string text;

    public string Text
    {
      get
      {
        return this.text;
      }
    }

    public override NodeType NodeType
    {
      get
      {
        return NodeType.XmlComment;
      }
    }

    public UnprocessedNode(string text, SourceContextReference sourceContext, int ordering)
      : base(sourceContext, ordering)
    {
      this.text = text;
    }
  }
}
