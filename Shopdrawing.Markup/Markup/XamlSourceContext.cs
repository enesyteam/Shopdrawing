// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlSourceContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;
using System;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal abstract class XamlSourceContext : INodeSourceContext
  {
    private int ordering = -1;
    private XmlContainerReference parent;
    private ITextRange textRange;
    private ITextRange initialTextSpan;

    public XmlContainerReference Parent
    {
      get
      {
        return this.parent;
      }
      set
      {
        this.parent = value;
      }
    }

    public virtual IReadableSelectableTextBuffer TextBuffer
    {
      get
      {
        if (this.parent == null)
          return (IReadableSelectableTextBuffer) null;
        return this.parent.TextBuffer;
      }
      set
      {
        throw new InvalidOperationException("Shouldn't set textBuffer on most derived types");
      }
    }

    public int Ordering
    {
      get
      {
        return this.ordering;
      }
    }

    public ITextRange TextRange
    {
      get
      {
        return this.textRange;
      }
      protected set
      {
        this.textRange = value;
      }
    }

    public ITextRange InitialTextSpan
    {
      get
      {
        return this.initialTextSpan;
      }
      set
      {
        this.initialTextSpan = value;
      }
    }

    public ITextLocation TextLocation
    {
      get
      {
        return TextBufferHelper.GetHostBuffer(this.TextBuffer).GetLocation(this.TextRange.Offset);
      }
    }

    internal ITextLocation LineInformation
    {
      get
      {
        return TextBufferHelper.GetHostBuffer(this.TextBuffer).GetLocation(this.textRange.Offset);
      }
    }

    public virtual bool IsCloned
    {
      get
      {
        return false;
      }
    }

    protected XamlSourceContext(XmlContainerReference parent)
    {
      this.parent = parent;
    }

    protected XamlSourceContext(XmlContainerReference parent, SourceContext sourceContext)
      : this(parent, parent.TextBuffer, sourceContext, true)
    {
    }

    protected XamlSourceContext(XmlContainerReference parent, IReadableSelectableTextBuffer textBuffer, SourceContext sourceContext, bool beginTracking)
    {
      this.parent = parent;
      if (!beginTracking)
        return;
      this.textRange = XamlSourceContext.ToTextRange(textBuffer, sourceContext);
      this.ordering = sourceContext.StartCol;
    }

    public void ClearTextRange()
    {
      this.textRange = (ITextRange) null;
      this.ordering = -1;
    }

    public void SetTextRange(IReadableSelectableTextBuffer textBuffer, int offset, int length)
    {
      this.textRange = textBuffer.CreateRange(offset, length);
      this.ordering = offset;
    }

    public void RefreshOrdering()
    {
      if (this.textRange != null)
        this.ordering = this.textRange.Offset;
      else
        this.ordering = -1;
    }

    public abstract INodeSourceContext Clone(bool keepOldRanges);

    public virtual INodeSourceContext FreezeText(bool isClone)
    {
      return (INodeSourceContext) null;
    }

    internal virtual void CloneCopy(XamlSourceContext other, bool keepOldRanges)
    {
      other.InitialTextSpan = this.InitialTextSpan;
      other.ordering = this.ordering;
      if (!keepOldRanges)
        return;
      other.TextRange = this.TextRange;
    }

    internal static ITextRange ToTextRange(IReadableSelectableTextBuffer textBuffer, SourceContext sourceContext)
    {
      int offset = sourceContext.StartCol;
      int num = sourceContext.EndCol;
      int length = textBuffer.Length;
      if (num > length)
        num = length;
      return textBuffer.CreateRange(offset, num - offset);
    }
  }
}
