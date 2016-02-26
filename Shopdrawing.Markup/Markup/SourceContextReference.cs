// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.SourceContextReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal abstract class SourceContextReference
  {
    private DocumentNode documentNode;

    public DocumentNode DocumentNode
    {
      get
      {
        return this.documentNode;
      }
    }

    public abstract XamlSourceContext SourceContext { get; set; }

    protected SourceContextReference(DocumentNode documentNode)
    {
      this.documentNode = documentNode;
    }

    public XmlElementReference BuildXmlElementReference()
    {
      XmlElementReference elementReference1 = this.SourceContext as XmlElementReference;
      DocumentNode documentNode = (DocumentNode) this.documentNode.Parent;
      XmlElementReference elementReference2;
      for (elementReference2 = (XmlElementReference) null; documentNode != null && elementReference2 == null; documentNode = (DocumentNode) documentNode.Parent)
        elementReference2 = documentNode.SourceContext as XmlElementReference;
      XmlElementReference elementReference3;
      if (elementReference1 == null)
      {
        if (elementReference2 == null)
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlElement xmlElement = new XmlElement(xmlDocument);
          xmlElement.Parent = (XmlNode) xmlDocument;
          elementReference3 = new XmlElementReference((XmlContainerReference) new XmlDocumentReference((IReadableSelectableTextBuffer) null, xmlDocument, false), xmlElement, false);
        }
        else
          elementReference3 = new XmlElementReference((XmlContainerReference) elementReference2, (XmlElement) null, false);
      }
      else
      {
        elementReference3 = (XmlElementReference) elementReference1.Clone(false);
        if (elementReference2 != null)
          elementReference3.Parent = (XmlContainerReference) elementReference2;
      }
      return elementReference3;
    }

    public virtual XmlAttributeReference BuildXmlAttributeReference()
    {
      XmlAttributeReference attributeReference1 = this.SourceContext as XmlAttributeReference;
      XmlElementReference parent = (XmlElementReference) null;
      if (this.documentNode != null)
      {
        for (DocumentNode documentNode = (DocumentNode) this.documentNode.Parent; documentNode != null && parent == null; documentNode = (DocumentNode) documentNode.Parent)
          parent = documentNode.SourceContext as XmlElementReference;
      }
      XmlAttributeReference attributeReference2;
      if (attributeReference1 == null)
      {
        attributeReference2 = new XmlAttributeReference(parent);
      }
      else
      {
        attributeReference2 = (XmlAttributeReference) attributeReference1.Clone(false);
        attributeReference2.Parent = (XmlContainerReference) parent;
      }
      return attributeReference2;
    }

    public string GetIndent(string childIndent)
    {
      if (this.documentNode == null)
        return string.Empty;
      DocumentCompositeNode parent = this.documentNode.Parent;
      if (parent == null)
        return string.Empty;
      XamlSourceContext xamlSourceContext = (XamlSourceContext) null;
      if (this.documentNode.IsChild)
      {
        for (int index = this.documentNode.SiteChildIndex - 1; index >= 0; --index)
        {
          xamlSourceContext = parent.Children[index].SourceContext as XamlSourceContext;
          if (xamlSourceContext != null && xamlSourceContext.TextRange != null)
            break;
        }
      }
      bool flag = false;
      while (parent != null && (xamlSourceContext == null || xamlSourceContext.TextRange == null))
      {
        xamlSourceContext = parent.SourceContext as XamlSourceContext ?? parent.ContainerSourceContext as XamlSourceContext;
        parent = parent.Parent;
        flag = true;
      }
      if (xamlSourceContext == null)
        return string.Empty;
      IReadableSelectableTextBuffer hostBuffer = TextBufferHelper.GetHostBuffer(xamlSourceContext.TextBuffer);
      ITextRange textRange = TextBufferHelper.ExpandSpanLeftToFillWhitespace((IReadableTextBuffer) hostBuffer, (ITextRange) new TextRange(xamlSourceContext.TextRange.Offset, xamlSourceContext.TextRange.Offset));
      string str = hostBuffer.GetText(textRange.Offset, textRange.Length);
      int num = str.LastIndexOf('\n');
      if (num != -1)
        str = str.Substring(num + 1);
      if (flag)
        str += childIndent;
      return str;
    }

    public void OffsetSourceContext(int start)
    {
      XamlSourceContext xamlSourceContext = this.SourceContext;
      if (xamlSourceContext == null)
      {
        if (this.documentNode == null)
          return;
        xamlSourceContext = this.documentNode.ContainerSourceContext as XamlSourceContext;
      }
      if (xamlSourceContext == null)
        return;
      if (this.documentNode != null)
        this.OffsetSourceContext(this.documentNode, start - xamlSourceContext.TextRange.Offset);
      else
        this.OffsetSourceContext((INodeSourceContext) xamlSourceContext, start - xamlSourceContext.TextRange.Offset);
    }

    protected void SetSourceContext(DocumentNode node, INodeSourceContext sourceContext)
    {
      if (node.DocumentRoot != null)
        node.DocumentRoot.SetSourceContext(node, sourceContext);
      else
        node.SourceContext = sourceContext;
    }

    private void OffsetSourceContext(DocumentNode node, int offset)
    {
      this.OffsetSourceContext(node.SourceContext, offset);
      this.OffsetSourceContext(node.ContainerSourceContext, offset);
      foreach (DocumentNode node1 in node.ChildNodes)
        this.OffsetSourceContext(node1, offset);
    }

    private void OffsetSourceContext(INodeSourceContext context, int offset)
    {
      XamlSourceContext xamlSourceContext = context as XamlSourceContext;
      if (xamlSourceContext == null || xamlSourceContext.TextRange == null)
        return;
      int start = xamlSourceContext.TextRange.Offset + offset;
      xamlSourceContext.InitialTextSpan = (ITextRange) new TextRange(start, start + xamlSourceContext.TextRange.Length);
      XmlElementReference elementReference = xamlSourceContext as XmlElementReference;
      if (elementReference == null || elementReference.StartTagRange == null)
        return;
      elementReference.InitialStartTagSpan = (ITextRange) new TextRange(start, elementReference.StartTagRange.Length);
    }
  }
}
