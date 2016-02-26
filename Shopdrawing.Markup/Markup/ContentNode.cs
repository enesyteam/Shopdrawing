// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ContentNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class ContentNode : FormattedNode
  {
    private string content;
    private bool escapeSpecialCharacters;
    private NodeType nodeType;

    public string Content
    {
      get
      {
        return this.content;
      }
    }

    public bool EscapeSpecialCharacters
    {
      get
      {
        return this.escapeSpecialCharacters;
      }
    }

    public override NodeType NodeType
    {
      get
      {
        return this.nodeType;
      }
    }

    public ContentNode(SourceContextReference sourceContextReference, string content, bool escapeSpecialCharacters)
      : base(sourceContextReference, int.MaxValue)
    {
      this.content = content;
      this.escapeSpecialCharacters = escapeSpecialCharacters;
      this.nodeType = NodeType.Literal;
    }

    public override string ToString()
    {
      return "\"" + this.content + "\"";
    }
  }
}
