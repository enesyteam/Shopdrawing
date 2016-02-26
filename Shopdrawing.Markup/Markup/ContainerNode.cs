// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ContainerNode
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup
{
  internal abstract class ContainerNode : FormattedNode
  {
    private FormattedNodeCollection children;

    public FormattedNodeCollection Children
    {
      get
      {
        return this.children;
      }
    }

    protected ContainerNode(SourceContextReference sourceContextReference, int ordering)
      : base(sourceContextReference, ordering)
    {
      this.children = new FormattedNodeCollection(this);
    }
  }
}
