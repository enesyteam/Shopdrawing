// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlFormatter
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XamlFormatter
  {
    private PersistenceSettings settings;
    private XamlFormatter.Writer writer;
    private bool shouldUpdateSourceContext;

    public XamlFormatter(PersistenceSettings settings, TextWriter writer, bool shouldUpdateSourceContext)
    {
      this.settings = settings;
      this.writer = new XamlFormatter.Writer(writer);
      this.shouldUpdateSourceContext = shouldUpdateSourceContext;
    }

    public void WriteDocument(ContainerNode rootNode)
    {
      foreach (FormattedNode node in rootNode.Children)
        this.WriteNode(string.Empty, node);
      this.writer.Flush();
    }

    public void WriteNode(FormattedNode node)
    {
      this.WriteNode(node.SourceContextReference != null ? node.SourceContextReference.GetIndent(this.settings.IndentString) : string.Empty, node);
    }

    private void WriteNode(string indent, FormattedNode node)
    {
      ContentNode content;
      if ((content = node as ContentNode) != null)
      {
        this.WriteContent(content);
      }
      else
      {
        ElementNode element;
        if ((element = node as ElementNode) != null)
        {
          this.WriteElement(indent, element);
        }
        else
        {
          AttributeNode attribute;
          if ((attribute = node as AttributeNode) != null)
          {
            this.WriteAttribute(indent, attribute);
          }
          else
          {
            UnprocessedNode unprocessedNode;
            if ((unprocessedNode = node as UnprocessedNode) == null)
              return;
            this.WriteUnprocessedNode(indent, unprocessedNode);
          }
        }
      }
    }

    private void WriteUnprocessedNode(string indent, UnprocessedNode unprocessedNode)
    {
      char[] text = unprocessedNode.Text.ToCharArray();
      XamlFormatter.UnprocessedNodeFormatting nodeFormatting = this.GetNodeFormatting(unprocessedNode);
      if (nodeFormatting.LeadingWhitespace != null)
      {
        this.writer.SetWhitespace(nodeFormatting.LeadingWhitespace);
        if (unprocessedNode.IndentingBehavior == IndentingBehavior.FromContainer && this.writer.LineBreakCount > 0)
          this.writer.SetIndent(indent);
      }
      int length = this.writer.Length;
      this.writer.WriteRawText(text, text.Length);
      if (this.shouldUpdateSourceContext && unprocessedNode.SourceContextReference != null)
        unprocessedNode.SourceContextReference.OffsetSourceContext(length);
      if (nodeFormatting.TrailingWhitespace == null)
        return;
      this.writer.SetWhitespace(nodeFormatting.TrailingWhitespace);
    }

    private void WriteContent(ContentNode content)
    {
      string str;
      if (content.EscapeSpecialCharacters)
      {
        StringBuilder builder = new StringBuilder();
        this.WriteContent(builder, content.Content);
        str = builder.ToString();
      }
      else
        str = content.Content;
      char[] text = str.ToCharArray();
      this.writer.WriteRawText(text, text.Length);
    }

    private void WriteElement(string indent, ElementNode element)
    {
      ElementPersistenceSettings elementSettings = this.settings.GetElementSettings(typeof (object));
      XamlFormatter.XmlElementFormatting elementFormatting = this.GetElementFormatting(element);
      if (elementFormatting.LeadingWhitespace != null)
      {
        this.writer.SetWhitespace(elementFormatting.LeadingWhitespace);
        if (!this.writer.Started || this.writer.LineBreakCount > 0)
        {
          if (element.IndentingBehavior == IndentingBehavior.FromContainer)
            this.writer.SetIndent(indent);
          else
            indent = this.writer.Indent;
        }
      }
      else
      {
        this.writer.SetMinimumLineBreaks(elementSettings.LinesOutside);
        this.writer.SetIndent(indent);
      }
      XmlElementReference elementReference = (XmlElementReference) null;
      if (this.shouldUpdateSourceContext && element.SourceContextReference != null)
      {
        elementReference = element.SourceContextReference.BuildXmlElementReference();
        element.SourceContextReference.SourceContext = (XamlSourceContext) elementReference;
      }
      int start = this.writer.WriteBeginStartElement(element.Name.FullName);
      if (elementFormatting.AttributeLeadingWhitespace != null)
        this.writer.SetWhitespace(elementFormatting.AttributeLeadingWhitespace);
      string indent1 = indent + this.settings.IndentString;
      element.Attributes.EnsureOrdering();
      for (int index = 0; index < element.Attributes.Count; ++index)
        this.WriteNode(indent1, element.Attributes[index]);
      if (elementFormatting.AttributeTrailingWhitespace != null)
        this.writer.SetWhitespace(elementFormatting.AttributeTrailingWhitespace);
      int end;
      int length;
      if (element.Children.Count > 0)
      {
        bool flag = element.IsWhitespaceSignificant;
        for (int index = 0; index < element.Children.Count; ++index)
        {
          switch (element.Children[index].NodeType)
          {
            case NodeType.Literal:
            case NodeType.WhitespaceLiteral:
              flag = true;
              break;
          }
        }
        this.writer.WriteEndStartElement();
        end = this.writer.Length;
        if (elementFormatting.ContentLeadingWhitespace != null)
          this.writer.SetWhitespace(elementFormatting.ContentLeadingWhitespace);
        else
          this.writer.SetMinimumLineBreaks(elementSettings.LinesInside);
        for (int index = 0; index < element.Children.Count; ++index)
        {
          FormattedNode node = element.Children[index];
          if (flag)
            this.writer.SuppressWhitespace();
          this.WriteNode(indent1, node);
          if (flag)
            this.writer.SuppressWhitespace();
        }
        if (elementFormatting.ContentTrailingWhitespace != null)
        {
          this.writer.SetWhitespace(elementFormatting.ContentTrailingWhitespace);
          if (element.IndentingBehavior == IndentingBehavior.FromContainer && this.writer.LineBreakCount > 0)
            this.writer.SetIndent(indent);
        }
        else
        {
          this.writer.SetMinimumLineBreaks(elementSettings.LinesInside);
          this.writer.SetIndent(indent);
        }
        this.writer.WriteEndElement(true);
        length = this.writer.Length;
      }
      else
      {
        this.writer.WriteEndElement(false);
        end = -1;
        length = this.writer.Length;
      }
      if (this.shouldUpdateSourceContext && elementReference != null)
      {
        elementReference.InitialTextSpan = (ITextRange) new TextRange(start, length);
        if (end != -1)
          elementReference.InitialStartTagSpan = (ITextRange) new TextRange(start, end);
        element.SourceContextReference.SourceContext = (XamlSourceContext) elementReference;
      }
      if (elementFormatting.TrailingWhitespace != null)
        this.writer.SetWhitespace(elementFormatting.TrailingWhitespace);
      else
        this.writer.SetMinimumLineBreaks(elementSettings.LinesOutside);
    }

    private void WriteAttribute(string indent, AttributeNode attribute)
    {
      XamlFormatter.XmlAttributeFormatting attributeFormatting = this.GetAttributeFormatting(attribute);
      if (attributeFormatting.LeadingWhitespace != null)
      {
        this.writer.SetWhitespace(attributeFormatting.LeadingWhitespace);
        if (attribute.IndentingBehavior == IndentingBehavior.FromContainer && this.writer.LineBreakCount > 0)
          this.writer.SetIndent(indent);
      }
      else if (this.writer.LineBreakCount == 0 && string.IsNullOrEmpty(this.writer.Indent))
        this.writer.SetIndent(this.settings.AttributeIndentString);
      StringBuilder builder = new StringBuilder(attribute.Name.FullName);
      char attributeQuoteCharacter;
      if (attributeFormatting.ValuePrefix != null)
      {
        attributeQuoteCharacter = attributeFormatting.AttributeQuoteCharacter;
        builder.Append(attributeFormatting.ValuePrefix);
      }
      else
      {
        attributeQuoteCharacter = this.settings.AttributeQuoteCharacter;
        builder.Append('=');
        builder.Append(attributeQuoteCharacter);
      }
      this.WriteAttributeValue(attributeQuoteCharacter, builder, attribute.Value, attribute.EscapeWhitespace);
      if (attributeFormatting.ValueSuffix != null)
        builder.Append(attributeFormatting.ValueSuffix);
      else
        builder.Append(attributeQuoteCharacter);
      int start = this.writer.WriteAttribute(builder.ToString());
      int length = this.writer.Length;
      if (this.shouldUpdateSourceContext && attribute.SourceContextReference != null)
      {
        XmlAttributeReference attributeReference = attribute.SourceContextReference.BuildXmlAttributeReference();
        attributeReference.InitialTextSpan = (ITextRange) new TextRange(start, length);
        attribute.SourceContextReference.SourceContext = (XamlSourceContext) attributeReference;
      }
      if (attributeFormatting.TrailingWhitespace == null)
        return;
      this.writer.SetWhitespace(attributeFormatting.TrailingWhitespace);
    }

    private void WriteAttributeValue(char attributeQuoteCharacter, StringBuilder builder, string value, bool escapeWhitespace)
    {
      foreach (char c in value)
        this.WriteAttributeValueCharacter(attributeQuoteCharacter, builder, c, escapeWhitespace);
    }

    private void WriteContent(StringBuilder builder, string value)
    {
      foreach (char c in value)
        this.WriteContentCharacter(builder, c);
    }

    private void WriteAttributeValueCharacter(char attributeQuoteCharacter, StringBuilder builder, char c, bool escapeWhitespace)
    {
      if ((int) c == (int) attributeQuoteCharacter)
      {
        if ((int) c == 39)
          builder.Append("&apos;");
        else
          builder.Append("&quot;");
      }
      else if (escapeWhitespace)
      {
        switch (c)
        {
          case '\t':
          case '\n':
          case '\r':
            builder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&#x{0:x};", new object[1]
            {
              (object) (int) c
            }));
            break;
          default:
            this.WriteContentCharacter(builder, c);
            break;
        }
      }
      else
        this.WriteContentCharacter(builder, c);
    }

    private void WriteContentCharacter(StringBuilder builder, char c)
    {
      switch (c)
      {
        case '&':
          builder.Append("&amp;");
          break;
        case '<':
          builder.Append("&lt;");
          break;
        case '>':
          builder.Append("&gt;");
          break;
        default:
          builder.Append(c);
          break;
      }
    }

    private XamlFormatter.UnprocessedNodeFormatting GetNodeFormatting(UnprocessedNode unprocessedNode)
    {
      XamlFormatter.UnprocessedNodeFormatting unprocessedNodeFormatting = new XamlFormatter.UnprocessedNodeFormatting();
      XamlSourceContext sourceContext = unprocessedNode.SourceContextReference.SourceContext;
      if (sourceContext != null && sourceContext.TextRange != null)
      {
        unprocessedNodeFormatting.LeadingWhitespace = XamlFormatter.GetLeadingWhitespace(sourceContext);
        unprocessedNodeFormatting.TrailingWhitespace = XamlFormatter.GetTrailingWhitespace(sourceContext);
      }
      return unprocessedNodeFormatting;
    }

    private XamlFormatter.XmlElementFormatting GetElementFormatting(ElementNode elementNode)
    {
      XamlFormatter.XmlElementFormatting elementFormatting = new XamlFormatter.XmlElementFormatting();
      XmlElementReference elementReference = elementNode.SourceContextReference.SourceContext as XmlElementReference;
      if (elementReference != null && elementReference.TextRange != null)
      {
        IReadableTextBuffer textBuffer = (IReadableTextBuffer) TextBufferHelper.GetHostBuffer(elementReference.TextBuffer);
        elementFormatting.LeadingWhitespace = XamlFormatter.GetLeadingWhitespace((XamlSourceContext) elementReference);
        elementFormatting.TrailingWhitespace = XamlFormatter.GetTrailingWhitespace((XamlSourceContext) elementReference);
        elementFormatting.AttributeLeadingWhitespace = XamlFormatter.GetTrailingWhitespace(textBuffer, elementReference.TextRange.Offset);
        if (elementReference.StartTagRange != null)
        {
          elementFormatting.AttributeTrailingWhitespace = XamlFormatter.GetLeadingWhitespace(textBuffer, elementReference.StartTagRange.Offset + elementReference.StartTagRange.Length - 1);
          elementFormatting.ContentLeadingWhitespace = XamlFormatter.GetTrailingWhitespace(textBuffer, elementReference.StartTagRange.Offset + elementReference.StartTagRange.Length);
          int num = elementReference.TextRange.Offset + elementReference.TextRange.Length;
          elementFormatting.ContentTrailingWhitespace = XamlFormatter.GetLeadingWhitespace(textBuffer, num - (elementNode.Name.FullName.Length + 3));
        }
        else
        {
          elementFormatting.AttributeTrailingWhitespace = XamlFormatter.GetLeadingWhitespace(textBuffer, elementReference.TextRange.Offset + elementReference.TextRange.Length - 2);
          elementFormatting.ContentLeadingWhitespace = (string) null;
          elementFormatting.ContentTrailingWhitespace = (string) null;
        }
      }
      return elementFormatting;
    }

    private XamlFormatter.XmlAttributeFormatting GetAttributeFormatting(AttributeNode attributeNode)
    {
      XamlFormatter.XmlAttributeFormatting attributeFormatting = new XamlFormatter.XmlAttributeFormatting();
      if (attributeNode.SourceContextReference == null)
        return attributeFormatting;
      XmlAttributeReference attributeReference = attributeNode.SourceContextReference.SourceContext as XmlAttributeReference;
      if (attributeReference != null && attributeReference.TextRange != null)
      {
        string text = TextBufferHelper.GetHostBuffer(attributeReference.TextBuffer).GetText(attributeReference.TextRange.Offset, attributeReference.TextRange.Length);
        int num = text.IndexOf('=');
        if (num == -1)
          return attributeFormatting;
        int index1 = num - 1;
        while (index1 >= 0 && Scanner.IsXmlWhitespace(text[index1]))
          --index1;
        int index2 = num + 1;
        while (index2 < text.Length && Scanner.IsXmlWhitespace(text[index2]))
          ++index2;
        int startIndex = text.LastIndexOf(text[index2]);
        if (index2 >= 0 && startIndex > index2)
        {
          attributeFormatting.LeadingWhitespace = XamlFormatter.GetLeadingWhitespace((XamlSourceContext) attributeReference);
          attributeFormatting.TrailingWhitespace = XamlFormatter.GetTrailingWhitespace((XamlSourceContext) attributeReference);
          attributeFormatting.ValuePrefix = text.Substring(index1 + 1, index2 - index1);
          attributeFormatting.ValueSuffix = text.Substring(startIndex, 1);
          attributeFormatting.AttributeQuoteCharacter = text[index2];
        }
      }
      return attributeFormatting;
    }

    private static string GetLeadingWhitespace(XamlSourceContext sourceContext)
    {
      IReadableTextBuffer textBuffer = (IReadableTextBuffer) TextBufferHelper.GetHostBuffer(sourceContext.TextBuffer);
      if (textBuffer.Length == 0)
        return string.Empty;
      return XamlFormatter.GetLeadingWhitespace(textBuffer, sourceContext.TextRange.Offset);
    }

    private static string GetLeadingWhitespace(IReadableTextBuffer textBuffer, int offset)
    {
      if (textBuffer.Length == 0)
        return string.Empty;
      ITextRange textRange = TextBufferHelper.ExpandSpanToFillWhitespace(textBuffer, (ITextRange) new TextRange(offset, offset), true, false);
      return textBuffer.GetText(textRange.Offset, textRange.Length);
    }

    private static string GetTrailingWhitespace(XamlSourceContext sourceContext)
    {
      IReadableTextBuffer textBuffer = (IReadableTextBuffer) TextBufferHelper.GetHostBuffer(sourceContext.TextBuffer);
      if (textBuffer.Length == 0)
        return string.Empty;
      return XamlFormatter.GetTrailingWhitespace(textBuffer, sourceContext.TextRange.Offset + sourceContext.TextRange.Length);
    }

    private static string GetTrailingWhitespace(IReadableTextBuffer textBuffer, int offset)
    {
      if (textBuffer.Length == 0)
        return string.Empty;
      ITextRange textRange = TextBufferHelper.ExpandSpanToFillWhitespace(textBuffer, (ITextRange) new TextRange(offset, offset), false, true);
      return textBuffer.GetText(textRange.Offset, textRange.Length);
    }

    [Conditional("DEBUG")]
    private static void CheckContainsOnlyWhitespace(string text)
    {
    }

    [Conditional("DEBUG")]
    private static void CheckContainsOnlySpacesAndTabs(string text)
    {
    }

    [Conditional("DEBUG")]
    private static void CheckContainsOnly(string text, string characters, string message)
    {
      if (text == null)
        return;
      foreach (char ch in text)
      {
        if (characters.IndexOf(ch) < 0)
          break;
      }
    }

    private struct UnprocessedNodeFormatting
    {
      public string LeadingWhitespace;
      public string TrailingWhitespace;
    }

    private struct XmlElementFormatting
    {
      public string LeadingWhitespace;
      public string TrailingWhitespace;
      public string AttributeLeadingWhitespace;
      public string AttributeTrailingWhitespace;
      public string ContentLeadingWhitespace;
      public string ContentTrailingWhitespace;
    }

    private struct XmlAttributeFormatting
    {
      public string LeadingWhitespace;
      public string TrailingWhitespace;
      public string ValuePrefix;
      public string ValueSuffix;
      public char AttributeQuoteCharacter;
    }

    private sealed class Writer
    {
      private Stack<string> elementNames = new Stack<string>();
      private TextWriter writer;
      private bool started;
      private int lineBreakCount;
      private string explicitWhitespace;
      private string indent;
      private bool suppressWhitespace;
      private int length;

      public bool Started
      {
        get
        {
          return this.started;
        }
      }

      public int LineBreakCount
      {
        get
        {
          return this.lineBreakCount;
        }
      }

      public string Indent
      {
        get
        {
          return this.indent;
        }
      }

      public int Length
      {
        get
        {
          return this.length;
        }
      }

      public Writer(TextWriter writer)
      {
        this.writer = writer;
      }

      public void SetWhitespace(string whitespace)
      {
        string leadingText;
        string trailingText;
        int num = XamlFormatter.Writer.SplitAtLastLineBreak(whitespace, out leadingText, out trailingText);
        if (num >= this.lineBreakCount)
        {
          this.explicitWhitespace = leadingText;
          this.lineBreakCount = num;
          this.indent = trailingText;
        }
        else
        {
          if (num <= 0)
            return;
          this.indent = trailingText;
        }
      }

      public void SetMinimumLineBreaks(int count)
      {
        if (count <= this.lineBreakCount)
          return;
        this.explicitWhitespace = (string) null;
        this.lineBreakCount = count;
      }

      public void SetIndent(string indent)
      {
        this.indent = indent;
      }

      public void SuppressWhitespace()
      {
        this.suppressWhitespace = true;
      }

      public int WriteBeginStartElement(string name)
      {
        this.WriteAnyWhitespaceOrLineBreaks();
        int num = this.length;
        this.writer.Write('<');
        this.writer.Write(name);
        this.elementNames.Push(name);
        this.length += name.Length + 1;
        return num;
      }

      public void WriteEndStartElement()
      {
        this.WriteAnyWhitespaceOrLineBreaks();
        this.writer.Write('>');
        ++this.length;
      }

      public void WriteEndElement(bool separateEndTag)
      {
        string str = this.elementNames.Pop();
        this.WriteAnyWhitespaceOrLineBreaks();
        if (separateEndTag)
        {
          this.writer.Write("</");
          this.writer.Write(str);
          this.writer.Write('>');
          this.length += 3 + str.Length;
        }
        else
        {
          this.writer.Write("/>");
          this.length += 2;
        }
      }

      public int WriteAttribute(string text)
      {
        this.WriteAnyWhitespaceOrLineBreaks();
        int num = this.length;
        this.writer.Write(text);
        this.length += text.Length;
        return num;
      }

      public void WriteRawText(char[] text, int length)
      {
        this.WriteAnyWhitespaceOrLineBreaks();
        this.writer.Write(text, 0, length);
        this.length += length;
      }

      public void Flush()
      {
        this.WriteAnyWhitespaceOrLineBreaks();
      }

      private void WriteAnyWhitespaceOrLineBreaks()
      {
        if (!this.suppressWhitespace)
        {
          if (this.explicitWhitespace != null)
          {
            this.writer.Write(this.explicitWhitespace);
            this.length += this.explicitWhitespace.Length;
          }
          else if (this.lineBreakCount > 0)
          {
            if (!this.started)
              --this.lineBreakCount;
            for (int index = 0; index < this.lineBreakCount; ++index)
            {
              this.writer.WriteLine();
              this.length += this.writer.NewLine.Length;
            }
          }
          if (!string.IsNullOrEmpty(this.indent))
          {
            this.writer.Write(this.indent);
            this.length += this.indent.Length;
          }
        }
        this.started = true;
        this.explicitWhitespace = (string) null;
        this.lineBreakCount = 0;
        this.indent = (string) null;
        this.suppressWhitespace = false;
      }

      private static int SplitAtLastLineBreak(string text, out string leadingText, out string trailingText)
      {
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < text.Length; ++index)
        {
          switch (text[index])
          {
            case '\n':
              ++num1;
              num2 = index + 1;
              break;
            case '\r':
              num2 = index + 1;
              break;
          }
        }
        leadingText = text.Substring(0, num2);
        trailingText = text.Substring(num2);
        return num1;
      }
    }
  }
}
