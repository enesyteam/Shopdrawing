// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlDocumentReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XmlDocumentReference : XmlContainerReference
  {
    private IReadableSelectableTextBuffer textBuffer;
    private readonly string encoding;

    internal string Encoding
    {
      get
      {
        return this.encoding;
      }
    }

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

    internal override XmlSpace XmlSpace
    {
      get
      {
        return XmlSpace.Default;
      }
    }

    internal XmlDocumentReference(IReadableSelectableTextBuffer textBuffer, XmlDocument xmlDocument, bool beginTracking)
      : this(textBuffer, xmlDocument.SourceContext, xmlDocument.Encoding, beginTracking)
    {
    }

    internal XmlDocumentReference(IReadableSelectableTextBuffer textBuffer, SourceContext sourceContext, string encoding, bool beginTracking)
      : base((XmlContainerReference) null, textBuffer, sourceContext, beginTracking)
    {
      this.textBuffer = textBuffer;
      this.encoding = encoding;
    }

    public override INodeSourceContext Clone(bool keepOldRanges)
    {
      XmlDocumentReference documentReference = new XmlDocumentReference(this.TextBuffer, new SourceContext(), this.encoding, false);
      this.CloneCopy((XamlSourceContext) documentReference, keepOldRanges);
      return (INodeSourceContext) documentReference;
    }
  }
}
