// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.RemovedXmlElementReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal class RemovedXmlElementReference : XmlElementReference
  {
    private IReadableSelectableTextBuffer textBuffer;
    private bool isCloned;

    public override IReadableSelectableTextBuffer TextBuffer
    {
      get
      {
        return this.textBuffer;
      }
      set
      {
        this.textBuffer = value;
      }
    }

    public override bool IsCloned
    {
      get
      {
        return this.isCloned;
      }
    }

    public RemovedXmlElementReference(XmlElementReference originalReference, bool isClone)
      : base(originalReference.Parent, new SourceContext(), new SourceContext(), false)
    {
      originalReference.CloneCopy((XamlSourceContext) this, false);
      this.textBuffer = originalReference.TextBuffer.Freeze();
      if (this.Comment != null && this.Comment.SourceContext != null)
        this.Comment.SourceContext.FreezeText(isClone);
      if (originalReference.TextRange != null)
        this.TextRange = this.textBuffer.FreezeRange(originalReference.TextRange);
      if (originalReference.StartTagRange != null)
        this.StartTagRange = this.textBuffer.FreezeRange(originalReference.StartTagRange);
      this.isCloned = isClone;
    }

    public override INodeSourceContext FreezeText(bool isClone)
    {
      if (this.isCloned != isClone)
        return this.Clone(isClone);
      return (INodeSourceContext) this;
    }

    public override INodeSourceContext Clone(bool keepOldRanges)
    {
      if (keepOldRanges)
        return (INodeSourceContext) new RemovedXmlElementReference((XmlElementReference) this, true);
      return base.Clone(keepOldRanges);
    }
  }
}
