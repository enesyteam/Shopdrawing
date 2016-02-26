// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlAttributeReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal class XmlAttributeReference : XamlSourceContext
  {
    public XmlAttributeReference(XmlElementReference parent)
      : base((XmlContainerReference) parent)
    {
    }

    public XmlAttributeReference(XmlElementReference parent, SourceContext context)
      : base((XmlContainerReference) parent, context)
    {
    }

    public override INodeSourceContext Clone(bool keepOldRanges)
    {
      XmlAttributeReference attributeReference = new XmlAttributeReference((XmlElementReference) this.Parent);
      this.CloneCopy((XamlSourceContext) attributeReference, keepOldRanges);
      return (INodeSourceContext) attributeReference;
    }

    public override INodeSourceContext FreezeText(bool isClone)
    {
      if (this.TextBuffer.Length != 0)
        return (INodeSourceContext) new RemovedXmlAttributeReference(this, isClone);
      return (INodeSourceContext) null;
    }
  }
}
