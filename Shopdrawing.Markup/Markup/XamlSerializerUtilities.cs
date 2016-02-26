// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlSerializerUtilities
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class XamlSerializerUtilities
  {
    public static void UpdateSourceContextRangeFromInitialRange(IReadableSelectableTextBuffer textBuffer, DocumentNode node, int offset)
    {
      XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRangeWorker(textBuffer, node, offset);
      XamlSerializerUtilities.RefreshSiblingOrdering(node);
    }

    public static void ClearSourceContextInitialRanges(DocumentNode node)
    {
      XamlSerializerUtilities.ClearSourceContextInitialRange(node.SourceContext);
      XamlSerializerUtilities.ClearSourceContextInitialRange(node.ContainerSourceContext);
      foreach (DocumentNode node1 in node.ChildNodes)
        XamlSerializerUtilities.ClearSourceContextInitialRanges(node1);
    }

    public static void PrepareNodeForTextDeletion(IDocumentRoot documentRoot, DocumentNode node)
    {
      node.ClearOldSourceContainerContext();
      DocumentNodeHelper.PreserveFormatting(documentRoot, node);
    }

    internal static ITextRange GetNodeSpan(ITextBuffer textBuffer, DocumentNode node, bool includeContainer)
    {
      return XamlSerializerUtilities.GetNodeSpan(textBuffer, node, includeContainer, (DocumentNode) null);
    }

    internal static ITextRange GetNodeSpan(ITextBuffer textBuffer, DocumentNode node, bool includeContainer, DocumentNode skipSelf)
    {
      if (textBuffer == null)
        throw new ArgumentNullException("textBuffer");
      ITextRange range = TextRange.Null;
      if (node == skipSelf)
        return range;
      XamlSourceContext sourceContext1 = node.SourceContext as XamlSourceContext;
      if (sourceContext1 != null && !sourceContext1.IsCloned && sourceContext1.TextRange != null)
      {
        ITextRange other = XamlSerializerUtilities.CaculateTextRange(sourceContext1, textBuffer);
        range = TextRange.Union(range, other);
      }
      XamlSourceContext sourceContext2 = node.ContainerSourceContext as XamlSourceContext;
      if (sourceContext2 != null && !sourceContext2.IsCloned && sourceContext2.TextRange != null && (includeContainer || sourceContext2 is XmlAttributeReference))
      {
        ITextRange other = XamlSerializerUtilities.CaculateTextRange(sourceContext2, textBuffer);
        range = TextRange.Union(range, other);
      }
      if (!TextRange.IsNull(range))
        return range;
      foreach (DocumentNode node1 in node.ChildNodes)
        range = TextRange.Union(range, XamlSerializerUtilities.GetNodeSpan(textBuffer, node1, true, skipSelf));
      return range;
    }

    private static ITextRange CaculateTextRange(XamlSourceContext sourceContext, ITextBuffer textBuffer)
    {
      ITextRange textRange = sourceContext.TextRange;
      IMappingTextRange mappingTextRange = textRange as IMappingTextRange;
      if (mappingTextRange != null)
        textRange = mappingTextRange.GetRangeForBuffer(textBuffer);
      else if (TextBufferHelper.GetHostBuffer(sourceContext.TextBuffer) != TextBufferHelper.GetHostBuffer((IReadableSelectableTextBuffer) textBuffer))
        return TextRange.Null;
      int offset = textRange.Offset;
      return (ITextRange) new TextRange(offset, offset + textRange.Length);
    }

    public static DocumentNode GetClosestAncestorSupportedForIncrementalSerialize(DocumentNode node)
    {
      DocumentNode documentNode1 = node;
      foreach (DocumentNode documentNode2 in node.AncestorNodes)
      {
        if (documentNode2.Type.IsExpression)
          documentNode1 = documentNode2;
      }
      if (documentNode1.Parent != null && (documentNode1.Type.Metadata.IsWhitespaceSignificant || documentNode1.Parent.Type.Metadata.IsWhitespaceSignificant))
        documentNode1 = (DocumentNode) documentNode1.Parent;
      if (node.Parent != null && node.IsProperty && node.PlatformMetadata.KnownProperties.DesignTimeShouldSerialize.Equals((object) node.SitePropertyKey))
        documentNode1 = (DocumentNode) documentNode1.Parent;
      return documentNode1;
    }

    public static int GetEndOfXmlElementName(IReadableSelectableTextBuffer textBuffer, TextRange startTagSpan)
    {
      int offset = startTagSpan.Offset + 1;
      while (offset < startTagSpan.Offset + startTagSpan.Length)
      {
        int length = Math.Min(32, startTagSpan.Offset + startTagSpan.Length - offset);
        string text = textBuffer.GetText(offset, length);
        for (int index = 0; index < text.Length; ++index)
        {
          if (Scanner.IsXmlWhitespace(text[index]) || (int) text[index] == 47 || (int) text[index] == 62)
            return offset + index;
        }
        offset += length;
      }
      return offset;
    }

    public static int GetStartOfXmlCloseTag(IReadableSelectableTextBuffer textBuffer, int elementEnd)
    {
      if (elementEnd <= 0)
        return 0;
      int offset = elementEnd - 1;
      while (offset >= 0 && (int) textBuffer.GetText(offset, 1)[0] != 60)
        --offset;
      return offset;
    }

    public static void UpdateNodeSourceContext(IReadableSelectableTextBuffer textBuffer, DocumentNode node, int startTagLength, int elementLength)
    {
      DocumentNode node1 = (DocumentNode) node.Parent;
      if (node1 != null && typeof (DictionaryEntry).IsAssignableFrom(node1.TargetType) && node1.SourceContext is XmlElementReference)
        XamlSerializerUtilities.UpdateNodeSourceContextWorker(textBuffer, node1, startTagLength, elementLength);
      XamlSerializerUtilities.UpdateNodeSourceContextWorker(textBuffer, node, startTagLength, elementLength);
    }

    private static void UpdateNodeSourceContextWorker(IReadableSelectableTextBuffer textBuffer, DocumentNode node, int startTagLength, int elementLength)
    {
      XmlElementReference elementReference1 = (XmlElementReference) node.SourceContext;
      XmlElementReference elementReference2 = (XmlElementReference) elementReference1.Clone(false);
      if (startTagLength != elementLength)
        elementReference2.StartTagRange = textBuffer.CreateRange(elementReference1.TextRange.Offset, startTagLength);
      elementReference2.SetTextRange(textBuffer, elementReference1.TextRange.Offset, elementLength);
      if (node.DocumentRoot != null)
        node.DocumentRoot.SetSourceContext(node, (INodeSourceContext) elementReference2);
      else
        node.SourceContext = (INodeSourceContext) elementReference2;
    }

    private static void UpdateSourceContextRangeFromInitialRangeWorker(IReadableSelectableTextBuffer textBuffer, DocumentNode node, int offset)
    {
      node.ClearOldSourceContainerContext();
      XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRange(textBuffer, node.SourceContext, offset);
      XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRange(textBuffer, node.ContainerSourceContext, offset);
      foreach (DocumentNode node1 in node.ChildNodes)
        XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRangeWorker(textBuffer, node1, offset);
    }

    private static void UpdateSourceContextRangeFromInitialRange(IReadableSelectableTextBuffer textBuffer, INodeSourceContext context, int offset)
    {
      XamlSourceContext xamlSourceContext = context as XamlSourceContext;
      if (xamlSourceContext == null)
        return;
      if (!TextRange.IsNull(xamlSourceContext.InitialTextSpan))
      {
        ITextRange initialTextSpan = xamlSourceContext.InitialTextSpan;
        ITextRange textRange1 = (ITextRange) new TextRange(initialTextSpan.Offset + offset, initialTextSpan.Offset + initialTextSpan.Length + offset);
        xamlSourceContext.InitialTextSpan = textRange1;
        xamlSourceContext.SetTextRange(textBuffer, textRange1.Offset, textRange1.Length);
        XmlElementReference elementReference = xamlSourceContext as XmlElementReference;
        if (elementReference == null)
          return;
        foreach (XmlElementReference.Attribute attribute in elementReference.AttributesToPreserve)
          XamlSerializerUtilities.UpdateSourceContextRangeFromInitialRange(textBuffer, (INodeSourceContext) attribute.SourceContext, offset);
        if (TextRange.IsNull(elementReference.InitialStartTagSpan))
          return;
        ITextRange initialStartTagSpan = elementReference.InitialStartTagSpan;
        ITextRange textRange2 = (ITextRange) new TextRange(initialStartTagSpan.Offset + offset, initialStartTagSpan.Offset + initialStartTagSpan.Length + offset);
        elementReference.InitialStartTagSpan = textRange2;
        elementReference.StartTagRange = textBuffer.CreateRange(textRange2.Offset, textRange2.Length);
      }
      else
      {
        xamlSourceContext.ClearTextRange();
        XmlElementReference elementReference = xamlSourceContext as XmlElementReference;
        if (elementReference == null)
          return;
        elementReference.StartTagRange = (ITextRange) null;
      }
    }

    private static void RefreshSiblingOrdering(DocumentNode node)
    {
      IDocumentRoot documentRoot = node.DocumentRoot;
      if (documentRoot == null || node.Parent == null)
        return;
      foreach (KeyValuePair<IProperty, DocumentNode> keyValuePair in (IEnumerable<KeyValuePair<IProperty, DocumentNode>>) node.Parent.Properties)
      {
        if (node != keyValuePair.Value)
        {
          XamlSourceContext xamlSourceContext1 = keyValuePair.Value.ContainerSourceContext as XamlSourceContext;
          if (xamlSourceContext1 != null)
          {
            XamlSourceContext xamlSourceContext2 = (XamlSourceContext) xamlSourceContext1.Clone(true);
            xamlSourceContext2.RefreshOrdering();
            documentRoot.SetContainerSourceContext(keyValuePair.Value, (INodeSourceContext) xamlSourceContext2);
          }
        }
      }
    }

    private static void ClearSourceContextInitialRange(INodeSourceContext sourceContext)
    {
      XamlSourceContext xamlSourceContext = sourceContext as XamlSourceContext;
      if (xamlSourceContext == null)
        return;
      xamlSourceContext.InitialTextSpan = TextRange.Null;
      XmlElementReference elementReference = xamlSourceContext as XmlElementReference;
      if (elementReference == null)
        return;
      elementReference.InitialStartTagSpan = TextRange.Null;
    }
  }
}
