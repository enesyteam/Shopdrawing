// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XamlParserContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class XamlParserContext
  {
    private IDocumentContext documentContext;
    private ClrNamespaceUriParseCache namespaces;
    private ClassAttributes rootAttributes;
    private IList<XamlParseError> errors;

    public IPlatformMetadata PlatformMetadata
    {
      get
      {
        return this.documentContext.TypeResolver.PlatformMetadata;
      }
    }

    public IDocumentContext DocumentContext
    {
      get
      {
        return this.documentContext;
      }
    }

    public ITypeResolver TypeResolver
    {
      get
      {
        return this.documentContext.TypeResolver;
      }
    }

    internal ClrNamespaceUriParseCache DocumentNamespaces
    {
      get
      {
        return this.namespaces;
      }
    }

    public ClassAttributes RootClassAttributes
    {
      get
      {
        return this.rootAttributes;
      }
      set
      {
        this.rootAttributes = value;
      }
    }

    public IList<XamlParseError> Errors
    {
      get
      {
        return this.errors;
      }
    }

    public XamlParserContext(IDocumentContext documentContext, ClassAttributes rootAttributes)
    {
      if (documentContext == null)
        throw new ArgumentNullException("documentContext");
      this.documentContext = documentContext;
      this.namespaces = new ClrNamespaceUriParseCache(this.TypeResolver);
      this.rootAttributes = rootAttributes;
      this.errors = (IList<XamlParseError>) new List<XamlParseError>();
    }

    public XmlNamespace GetXmlNamespace(ITextLocation lineInformation, IXmlNamespaceResolver xmlNamespaceResolver, XmlnsPrefix prefix)
    {
      XmlNamespace xmlNamespace = xmlNamespaceResolver.GetXmlNamespace(prefix, XmlNamespace.GetNamespaceCanonicalization(this.TypeResolver));
      if (xmlNamespace != null)
        return xmlNamespace;
      if (prefix == XmlnsPrefix.EmptyPrefix)
        this.ReportError(XamlParseErrors.NoDefaultNamespace(lineInformation));
      else
        this.ReportError(XamlParseErrors.UnrecognizedXmlnsPrefix(lineInformation, prefix));
      return XmlNamespace.AvalonXmlNamespace;
    }

    public void ReportError(XamlParseError parseError)
    {
      int index1 = 0;
      for (int index2 = this.errors.Count - 1; index2 >= 0; --index2)
      {
        XamlParseError xamlParseError = this.errors[index2];
        if (xamlParseError.Equals((object) parseError))
          return;
        if (XamlParserContext.CompareTextLocation(xamlParseError.Line, xamlParseError.Column, parseError.Line, parseError.Column) <= 0)
        {
          index1 = index2 + 1;
          break;
        }
      }
      this.errors.Insert(index1, parseError);
    }

    private static int CompareTextLocation(int firstLine, int firstColumn, int secondLine, int secondColumn)
    {
      int num = firstLine - secondLine;
      if (num == 0)
        num = firstColumn - secondColumn;
      return num;
    }
  }
}
