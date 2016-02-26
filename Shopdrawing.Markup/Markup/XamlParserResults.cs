// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlParserResults
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  public sealed class XamlParserResults
  {
    private readonly ITypeId expectedRootType;
    private readonly DocumentNode rootNode;
    private readonly IList<XamlParseError> errors;
    private readonly XmlDocumentReference xmlDocumentReference;
    private readonly IReadableSelectableTextBuffer textBuffer;

    public ITypeId ExpectedRootType
    {
      get
      {
        return this.expectedRootType;
      }
    }

    public DocumentNode RootNode
    {
      get
      {
        return this.rootNode;
      }
    }

    public IList<XamlParseError> Errors
    {
      get
      {
        return this.errors;
      }
    }

    internal XmlDocumentReference XmlDocumentReference
    {
      get
      {
        return this.xmlDocumentReference;
      }
    }

    public IReadableSelectableTextBuffer TextBuffer
    {
      get
      {
        return this.textBuffer;
      }
    }

    public XamlParserResults(ITypeId expectedRootType, DocumentNode rootNode, IList<XamlParseError> errors, INodeSourceContext xmlDocumentReference, IReadableSelectableTextBuffer textBuffer)
    {
      this.expectedRootType = expectedRootType;
      this.rootNode = rootNode;
      this.errors = errors;
      this.xmlDocumentReference = (XmlDocumentReference) xmlDocumentReference;
      this.textBuffer = textBuffer;
    }

    public static XamlParserResults Parse(IDocumentContext context, ITypeId expectedRootType, IReadableSelectableTextBuffer textBuffer)
    {
      return new XamlParser(context, textBuffer, expectedRootType).Parse();
    }
  }
}
