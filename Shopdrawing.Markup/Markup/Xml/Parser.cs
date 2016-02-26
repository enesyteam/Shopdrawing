// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Parser
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class Parser
  {
    public static Identifier doctype = Identifier.For("DOCTYPE");
    public static Identifier element = Identifier.For("ELEMENT");
    public static Identifier attlist = Identifier.For("ATTLIST");
    public static Identifier entity = Identifier.For("ENTITY");
    public static Identifier notation = Identifier.For("NOTATION");
    internal Document doc;
    internal ErrorNodeList errors;
    internal Scanner scanner;
    internal Token currentToken;
    internal ErrorHandler errorHandler;
    internal XmlDocument root;
    internal Identifier endTag;
    internal SourceContext endTagContext;

    public Scanner Scanner
    {
      get
      {
        return this.scanner;
      }
    }

    public Parser(Document doc, ErrorNodeList errors)
    {
      this.doc = doc;
      this.errors = errors;
      this.errorHandler = new ErrorHandler(errors);
      this.scanner = this.CreateScanner(doc, errors);
    }

    internal virtual Scanner CreateScanner(Document doc, ErrorNodeList errors)
    {
      return new Scanner(doc, errors);
    }

    internal virtual Token GetNextToken()
    {
      this.currentToken = this.scanner.GetNextToken();
      while (this.currentToken == Token.IllegalCharacter)
        this.currentToken = this.scanner.GetNextToken();
      return this.currentToken;
    }

    public virtual XmlDocument ParseDocument()
    {
      int num1 = (int) this.GetNextToken();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Location = this.doc.Name;
      this.root = xmlDocument;
      xmlDocument.SourceContext = new SourceContext(this.doc, 0, this.doc.Text.Length);
      while (this.currentToken != Token.EndOfFile)
      {
        switch (this.currentToken)
        {
          case Token.StartOfTag:
            this.ParseElement(xmlDocument, (XmlElement) null);
            continue;
          case Token.StartLiteralComment:
            this.ParseComment((XmlNode) xmlDocument);
            continue;
          case Token.LiteralContentString:
            Literal stringLiteral = this.scanner.GetStringLiteral();
            if (!(stringLiteral.Value.ToString() == ""))
            {
              if (stringLiteral is WhitespaceLiteral)
                xmlDocument.AddChild((Node) stringLiteral);
              else
                this.errorHandler.HandleError((Node) stringLiteral, SR.UnexpectedToken, this.scanner.GetString());
            }
            int num2 = (int) this.GetNextToken();
            continue;
          case Token.MarkupDeclaration:
            this.ParseXmlMarkupDeclaration(xmlDocument);
            continue;
          case Token.StartCharacterData:
            this.ParseCData((XmlNode) xmlDocument);
            continue;
          case Token.StartProcessingInstruction:
            this.ParseProcessingInstruction((XmlNode) xmlDocument);
            continue;
          default:
            this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.UnexpectedToken, this.scanner.GetString());
            int num3 = (int) this.GetNextToken();
            continue;
        }
      }
      for (Node node = xmlDocument.FirstChild; node != null; node = node.NextNode)
      {
        XmlDeclaration xmlDeclaration = node as XmlDeclaration;
        if (xmlDeclaration != null)
          xmlDocument.XmlDecl = xmlDeclaration;
      }
      return xmlDocument;
    }

    public XmlElement ParsePartialElement(XmlElement parent)
    {
      this.root = new XmlDocument()
      {
        Location = this.doc.Name
      };
      int num = (int) this.GetNextToken();
      XmlElement e = new XmlElement(this.root);
      this.ParseStartTag(this.root, parent, e);
      return e;
    }

    internal Identifier ParseQualifiedName()
    {
      Identifier identifier1 = this.scanner.GetIdentifier();
      int num1 = (int) this.GetNextToken();
      if (this.currentToken == Token.Colon)
      {
        int num2 = (int) this.GetNextToken();
        if (this.currentToken == Token.Identifier)
        {
          Identifier identifier2 = (Identifier) new ComplexIdentifier(this.scanner.GetIdentifier());
          identifier2.Prefix = identifier1;
          identifier2.SourceContext.StartCol = identifier1.SourceContext.StartCol;
          identifier1 = identifier2;
          int num3 = (int) this.GetNextToken();
        }
        else
        {
          Identifier identifier2 = (Identifier) new ComplexIdentifier(Identifier.For(""));
          identifier2.SourceContext = identifier1.SourceContext;
          ++identifier2.SourceContext.EndCol;
          identifier2.Prefix = identifier1;
          identifier1 = identifier2;
        }
      }
      return identifier1;
    }

    private bool ParseStartTag(XmlDocument owner, XmlElement parent, XmlElement e)
    {
      e.SourceContext = this.scanner.CurrentSourceContext;
      e.startTagContext = this.scanner.CurrentSourceContext;
      int num1 = (int) this.GetNextToken();
      if (parent == null)
        owner.AddChild((Node) e);
      else
        parent.AddChild((Node) e);
      if (this.currentToken == Token.EndOfFile)
        return true;
      if (this.currentToken != Token.Identifier)
      {
        string name = "";
        if (this.currentToken == Token.StringLiteral || this.currentToken == Token.Whitespace || this.currentToken == Token.LiteralContentString)
          name = this.scanner.GetString().Replace("\n", "").Replace("\r", "");
        e.Name = Identifier.For(name);
        e.Name.SourceContext = e.startTagContext;
      }
      else
      {
        e.Name = this.ParseQualifiedName();
        this.ParseAttributes(e);
        e.startTagContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
        if (this.currentToken == Token.EndOfSimpleTag)
        {
          e.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
          int num2 = (int) this.GetNextToken();
          return true;
        }
        if (this.currentToken == Token.EndOfTag)
        {
          if (this.scanner.GetString() == "?>")
            this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.ExpectingToken, ">");
          int num2 = (int) this.GetNextToken();
          return false;
        }
      }
      if (this.currentToken == Token.EndOfFile)
        this.errorHandler.HandleError((Node) e.Name, SR.TagNotClosed, e.Name != null ? e.Name.ToString() : "");
      e.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      return true;
    }

    internal XmlElement ParseElement(XmlDocument owner, XmlElement parent)
    {
      XmlElement xmlElement = new XmlElement(this.root);
      if (this.ParseStartTag(owner, parent, xmlElement))
        return xmlElement;
      while (this.currentToken != Token.EndOfFile)
      {
        switch (this.currentToken)
        {
          case Token.StartCharacterData:
            this.ParseCData((XmlNode) xmlElement);
            continue;
          case Token.StartProcessingInstruction:
            this.ParseProcessingInstruction((XmlNode) xmlElement);
            continue;
          case Token.StartOfClosingTag:
            this.endTagContext = this.scanner.CurrentSourceContext;
            int num1 = (int) this.GetNextToken();
            if (this.currentToken != Token.Identifier)
            {
              xmlElement.endTagContext = this.endTagContext;
              if (this.currentToken == Token.Whitespace)
                this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.IllegalWhitespace);
              else
                this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.UnexpectedToken, this.scanner.GetString());
              xmlElement.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
              goto label_28;
            }
            else
            {
              this.endTag = this.ParseQualifiedName();
              break;
            }
          case Token.StartOfTag:
            this.ParseElement(owner, xmlElement);
            if (this.endTag == null)
              continue;
            break;
          case Token.StartLiteralComment:
            this.ParseComment((XmlNode) xmlElement);
            continue;
          case Token.LiteralContentString:
            Literal stringLiteral1 = this.scanner.GetStringLiteral();
            xmlElement.AddChild((Node) stringLiteral1);
            int num2 = (int) this.GetNextToken();
            continue;
          case Token.CharacterEntity:
            XmlEntityReference xmlEntityReference1 = new XmlEntityReference(owner, this.scanner.GetCharEntity());
            xmlEntityReference1.SourceContext = this.scanner.CurrentSourceContext;
            xmlElement.AddChild((Node) xmlEntityReference1);
            int num3 = (int) this.GetNextToken();
            continue;
          case Token.GeneralEntityReference:
            XmlEntityReference xmlEntityReference2 = new XmlEntityReference(owner, this.scanner.GetEntityName());
            xmlEntityReference2.SourceContext = this.scanner.CurrentSourceContext;
            xmlElement.AddChild((Node) xmlEntityReference2);
            int num4 = (int) this.GetNextToken();
            continue;
          default:
            int num5 = (int) this.GetNextToken();
            continue;
        }
        XmlNode xmlNode = (XmlNode) xmlElement;
        while (xmlNode != null && !XmlNode.QualifiedNameMatches(xmlNode.Name, this.endTag))
          xmlNode = xmlNode.Parent;
        if (xmlNode == null)
          this.errorHandler.HandleError((Node) this.endTag, SR.ClosingTagMismatch, xmlElement.Name.ToString());
        else if (xmlNode != xmlElement)
        {
          this.errorHandler.HandleError((Node) this.endTag, SR.ClosingTagMismatch, xmlElement.Name.ToString());
          return xmlElement;
        }
        xmlElement.endTagContext = this.endTagContext;
        xmlElement.endTag = this.endTag;
        this.endTag = (Identifier) null;
        Literal stringLiteral2 = this.scanner.GetStringLiteral();
        this.SkipWhitespace();
        if (this.currentToken != Token.EndOfEndTag)
        {
          this.errorHandler.HandleError((Node) stringLiteral2, SR.ExpectingToken, ">");
          xmlElement.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
          xmlElement.endTagContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
          break;
        }
        xmlElement.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
        xmlElement.endTagContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
        int num6 = (int) this.GetNextToken();
        return xmlElement;
      }
label_28:
      if (this.currentToken == Token.EndOfFile)
        this.errorHandler.HandleError((Node) xmlElement.Name, SR.TagNotClosed, xmlElement.Name != null ? xmlElement.Name.ToString() : "");
      xmlElement.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      return xmlElement;
    }

    internal void ParseCData(XmlNode parent)
    {
      XmlCDATA xmlCdata = new XmlCDATA();
      parent.AddChild((Node) xmlCdata);
      xmlCdata.SourceContext = this.scanner.CurrentSourceContext;
      int num1 = (int) this.GetNextToken();
      SourceContext currentSourceContext = this.scanner.CurrentSourceContext;
      while (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag)
      {
        if (parent != null && this.currentToken == Token.CharacterData)
          xmlCdata.AddChild((Node) this.scanner.GetStringLiteral());
        int num2 = (int) this.GetNextToken();
      }
      xmlCdata.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      if (this.currentToken != Token.EndOfTag)
        return;
      int num3 = (int) this.GetNextToken();
    }

    internal void ParseComment(XmlNode parent)
    {
      XmlComment xmlComment = new XmlComment();
      parent.AddChild((Node) xmlComment);
      xmlComment.SourceContext = this.scanner.CurrentSourceContext;
      int num1 = (int) this.GetNextToken();
      SourceContext currentSourceContext = this.scanner.CurrentSourceContext;
      while (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag)
      {
        if (parent != null && this.currentToken == Token.LiteralComment)
          xmlComment.AddChild((Node) this.scanner.GetStringLiteral());
        int num2 = (int) this.GetNextToken();
      }
      xmlComment.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      if (this.currentToken != Token.EndOfTag)
        return;
      int num3 = (int) this.GetNextToken();
    }

    internal void ParseProcessingInstruction(XmlNode parent)
    {
      SourceContext currentSourceContext = this.scanner.CurrentSourceContext;
      int num1 = (int) this.GetNextToken();
      Identifier name = (Identifier) null;
      if (this.currentToken == Token.Identifier)
        name = this.scanner.GetIdentifier();
      else
        this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.ExpectingToken, "NMTOKEN");
      if (name != null && name.Name == StandardXmlIdentifiers.xml.Name)
      {
        this.ParseXmlDeclaration(name, parent, currentSourceContext);
      }
      else
      {
        XmlProcessingInstruction processingInstruction = new XmlProcessingInstruction();
        processingInstruction.Name = name;
        parent.AddChild((Node) processingInstruction);
        processingInstruction.SourceContext = currentSourceContext;
        while (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag)
        {
          if (parent != null && this.currentToken == Token.ProcessingInstructions)
            processingInstruction.AddChild((Node) this.scanner.GetStringLiteral());
          int num2 = (int) this.GetNextToken();
        }
        processingInstruction.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
        if (this.currentToken != Token.EndOfTag)
          return;
        int num3 = (int) this.GetNextToken();
      }
    }

    internal void ParseXmlDeclaration(Identifier name, XmlNode parent, SourceContext openBrace)
    {
      XmlDeclaration xmlDeclaration = new XmlDeclaration(this.root);
      xmlDeclaration.Name = name;
      parent.AddChild((Node) xmlDeclaration);
      xmlDeclaration.SourceContext = openBrace;
      int num1 = (int) this.GetNextToken();
      this.ParseAttributes((XmlElement) xmlDeclaration);
      xmlDeclaration.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      xmlDeclaration.startTagContext = xmlDeclaration.SourceContext;
      if (this.currentToken == Token.EndOfTag)
      {
        if (this.scanner.GetString() != "?>")
          this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), SR.ExpectingToken, "?>");
        int num2 = (int) this.GetNextToken();
      }
      if (!xmlDeclaration.HasAttributes)
        return;
      for (Node node = xmlDeclaration.FirstAttribute; node != null; node = node.NextNode)
      {
        XmlAttribute xmlAttribute = (XmlAttribute) node;
        if (xmlAttribute.Name.Name == StandardXmlIdentifiers.encoding.Name)
          xmlDeclaration.Encoding = xmlAttribute.Value.Value;
      }
    }

    internal void ParseAttributes(XmlElement e)
    {
      bool flag1 = true;
      while (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag && this.currentToken != Token.EndOfSimpleTag)
      {
        string code = (string) null;
        Identifier identifier1 = (Identifier) null;
        if (this.currentToken == Token.Whitespace)
          this.SkipWhitespace();
        else if (flag1)
        {
          code = SR.MissingWhitespace;
          goto label_22;
        }
        if (this.currentToken == Token.EndOfTag || this.currentToken == Token.EndOfSimpleTag || this.currentToken == Token.EndOfFile)
          break;
        if (this.currentToken == Token.Identifier)
        {
          Identifier identifier2 = this.ParseQualifiedName();
          if (e.Name == null)
            e.Name = identifier2;
          XmlAttribute a = new XmlAttribute(e);
          a.doc = this.root;
          a.SourceContext = identifier2.SourceContext;
          a.Name = identifier2;
          e.AddAttribute(a);
          a.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
          this.SkipWhitespace();
          while (this.currentToken == Token.Assign)
          {
            a.hasAssign = true;
            a.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
            int num = (int) this.GetNextToken();
            this.SkipWhitespace();
            if (a.hasAssign && this.currentToken == Token.Identifier)
            {
              Identifier identifier3 = this.ParseQualifiedName();
              SourceContext currentSourceContext = this.scanner.CurrentSourceContext;
              this.SkipWhitespace();
              if (this.currentToken == Token.Assign)
              {
                this.errorHandler.HandleError((Node) a.Name, SR.MissingAttributeValue);
                a = new XmlAttribute(e);
                a.doc = this.root;
                a.SourceContext = currentSourceContext;
                a.Name = identifier3;
                e.AddAttribute(a);
                continue;
              }
              this.errorHandler.HandleError((Node) identifier3, SR.MissingQuotesOnAttributeValue, identifier3.Name);
              a.AddChild((Node) new Literal(identifier3.Name, identifier3.SourceContext));
            }
            SourceContext currentSourceContext1 = this.scanner.CurrentSourceContext;
            if (this.currentToken == Token.StartStringLiteral)
            {
              a.quoteChar = this.scanner.LiteralQuoteChar;
              a.SourceContext.EndCol = this.ParseStringLiteral((XmlNode) a).EndCol;
            }
            flag1 = true;
            if (!a.HasChildNodes && (int) a.quoteChar == 0)
            {
              a.SourceContext.EndCol = this.scanner.CurrentSourceContext.StartCol;
              identifier1 = a.Name;
              code = SR.MissingAttributeValue;
              goto label_22;
            }
            else
              goto label_22;
          }
          identifier1 = a.Name;
          code = SR.MissingEquals;
        }
        else
          code = SR.MissingAttributeName;
label_22:
        if (code != null)
        {
          if (identifier1 != null)
            this.errorHandler.HandleError((Node) identifier1, code, identifier1.Name);
          else
            this.errorHandler.HandleError((Node) this.scanner.GetStringLiteral(), code, this.scanner.GetString());
          while (this.currentToken != Token.Identifier && this.currentToken != Token.StartOfTag && (this.currentToken != Token.StartOfClosingTag && this.currentToken != Token.StartLiteralComment) && (this.currentToken != Token.StartCharacterData && this.currentToken != Token.StartProcessingInstruction && (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag)) && this.currentToken != Token.EndOfSimpleTag)
          {
            bool flag2 = this.currentToken != Token.Whitespace;
            int num = (int) this.GetNextToken();
          }
          if (this.currentToken != Token.Identifier)
            break;
          flag1 = false;
        }
      }
    }

    internal SourceContext ParseStringLiteral(XmlNode container)
    {
      SourceContext currentSourceContext = this.scanner.CurrentSourceContext;
      int num1 = (int) this.GetNextToken();
      if (this.currentToken == Token.EndOfFile)
      {
        Literal literal = new Literal("", currentSourceContext);
        ++literal.SourceContext.StartCol;
        literal.SourceContext.EndCol = literal.SourceContext.StartCol;
        container.AddChild((Node) literal);
      }
      currentSourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      while (this.currentToken == Token.StringLiteral || this.currentToken == Token.CharacterEntity || this.currentToken == Token.GeneralEntityReference)
      {
        if (this.currentToken == Token.StringLiteral)
        {
          Literal stringLiteral = this.scanner.GetStringLiteral();
          string str = stringLiteral.Value;
          if (str != null && str.Length > 0)
          {
            int num2 = (int) str[0];
          }
          --stringLiteral.SourceContext.EndCol;
          container.AddChild((Node) stringLiteral);
        }
        else if (this.currentToken == Token.CharacterEntity)
          container.AddChild((Node) new XmlEntityReference(container.doc, this.scanner.GetCharEntity()));
        else if (this.currentToken == Token.GeneralEntityReference)
          container.AddChild((Node) new XmlEntityReference(container.doc, this.scanner.GetEntityName()));
        currentSourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
        int num3 = (int) this.GetNextToken();
      }
      return currentSourceContext;
    }

    internal void ParseXmlMarkupDeclaration(XmlDocument doc)
    {
      int num = (int) this.GetNextToken();
      if (this.currentToken == Token.Identifier)
      {
        Identifier identifier = this.scanner.GetIdentifier();
        this.errorHandler.HandleError((Node) identifier, SR.InvalidMarkupDeclaration, identifier.ToString());
      }
      else
        this.AddErrorRecoveryNode((XmlNode) doc, (Node) this.scanner.GetStringLiteral(), SR.UnexpectedToken);
    }

    protected void SkipWhitespace()
    {
      if (this.currentToken != Token.Whitespace)
        return;
      Scanner scanner = this.scanner;
      while (this.GetNextToken() == Token.Whitespace && scanner != this.scanner)
        scanner = this.scanner;
    }

    internal void SkipToTagEnd()
    {
      while (this.currentToken != Token.EndOfFile && this.currentToken != Token.EndOfTag && (this.currentToken != Token.EndOfEndTag && this.currentToken != Token.EndOfSimpleTag))
      {
        int num1 = (int) this.GetNextToken();
      }
      if (this.currentToken != Token.EndOfTag && this.currentToken != Token.EndOfSimpleTag)
        return;
      int num2 = (int) this.GetNextToken();
    }

    internal void AddErrorRecoveryNode(XmlNode parent, Node ctx, string error)
    {
      this.errorHandler.HandleError(ctx, error, this.scanner.GetString());
      XmlError xmlError = new XmlError();
      xmlError.SourceContext = this.scanner.CurrentSourceContext;
      this.SkipToTagEnd();
      xmlError.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
      parent.AddChild((Node) xmlError);
      parent.SourceContext.EndCol = this.scanner.CurrentSourceContext.EndCol;
    }
  }
}
