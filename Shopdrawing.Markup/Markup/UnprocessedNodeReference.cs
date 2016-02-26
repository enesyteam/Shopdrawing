// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.UnprocessedNodeReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class UnprocessedNodeReference : XamlSourceContext
  {
    public UnprocessedNodeReference(XmlContainerReference parent)
      : base(parent)
    {
    }

    public UnprocessedNodeReference(XmlContainerReference parent, SourceContext context)
      : base(parent, context)
    {
    }

    public override INodeSourceContext Clone(bool keepOldRanges)
    {
      UnprocessedNodeReference unprocessedNodeReference = new UnprocessedNodeReference(this.Parent);
      this.CloneCopy((XamlSourceContext) unprocessedNodeReference, keepOldRanges);
      return (INodeSourceContext) unprocessedNodeReference;
    }
  }
}
