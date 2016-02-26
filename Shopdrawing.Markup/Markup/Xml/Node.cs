// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Node
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal abstract class Node
  {
    internal NodeType NodeType;
    public SourceContext SourceContext;
    private Node nextNode;

    internal Node NextNode
    {
      get
      {
        return this.nextNode;
      }
      set
      {
        this.nextNode = value;
      }
    }

    internal virtual Node FirstChild
    {
      get
      {
        return (Node) null;
      }
    }

    internal Node(NodeType nodeType)
    {
      this.NodeType = nodeType;
    }
  }
}
