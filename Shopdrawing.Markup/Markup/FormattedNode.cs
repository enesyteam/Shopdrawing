// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.FormattedNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal abstract class FormattedNode
  {
    public const int AtStart = -2147483648;
    public const int AtEnd = 2147483647;
    private ContainerNode parent;
    private readonly SourceContextReference sourceContextReference;
    private int ordering;
    private IndentingBehavior indentingBehavior;

    public ContainerNode Parent
    {
      get
      {
        return this.parent;
      }
      set
      {
        this.parent = value;
        this.indentingBehavior = FormattedNode.GetIndentingBehavior(this);
      }
    }

    public SourceContextReference SourceContextReference
    {
      get
      {
        return this.sourceContextReference;
      }
    }

    public abstract NodeType NodeType { get; }

    public int Ordering
    {
      get
      {
        return this.ordering;
      }
      set
      {
        this.ordering = value;
      }
    }

    public IndentingBehavior IndentingBehavior
    {
      get
      {
        return this.indentingBehavior;
      }
    }

    protected FormattedNode(SourceContextReference sourceContextReference, int ordering)
    {
      this.sourceContextReference = sourceContextReference;
      this.indentingBehavior = IndentingBehavior.LeaveUnchanged;
      this.ordering = ordering;
    }

    protected static IndentingBehavior GetIndentingBehavior(FormattedNode node)
    {
      for (; node != null && !(node is RootNode); node = (FormattedNode) node.Parent)
      {
        if (node.SourceContextReference == null || node.SourceContextReference.SourceContext == null || (node.SourceContextReference.SourceContext.TextRange == null || node.SourceContextReference.SourceContext.IsCloned) || node.SourceContextReference.DocumentNode != null && node.SourceContextReference.DocumentNode.ContainerSourceContext == null && (node.Parent == null || !(node.Parent is RootNode) && (node.Parent.SourceContextReference == null || node.Parent.SourceContextReference.SourceContext != node.SourceContextReference.SourceContext.Parent)))
          return IndentingBehavior.FromContainer;
      }
      return IndentingBehavior.LeaveUnchanged;
    }
  }
}
