// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Scanner
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal sealed class Scanner
  {
    private static readonly Keyword[] Keywords = Keyword.InitKeywords();
    private StringBuilder identifier = new StringBuilder(128);
    internal const int SurHighStart = 55296;
    internal const int SurHighEnd = 56319;
    internal const int SurLowStart = 56320;
    internal const int SurLowEnd = 57343;
    private Document document;
    private DocumentText sourceText;
    private int startPos;
    internal int endPos;
    private int maxPos;
    internal bool isXslKeyword;
    private int lastReportedErrorPos;
    internal bool isWhitespace;
    internal SourceContext beginBlock;
    internal char LiteralQuoteChar;
    private Token lastToken;
    private string xslPrefix;
    private char xslStartChar;
    private ErrorNodeList errors;
    private string unescapedString;
    private bool stillInsideMultiLineToken;
    internal Scanner.State mode;
    internal Scanner.State state;
    private char charEntity;
    private Identifier entityName;

    public string XslPrefix
    {
      get
      {
        return this.xslPrefix;
      }
      set
      {
        this.xslPrefix = value;
        if (this.xslPrefix == null)
          return;
        this.xslStartChar = this.xslPrefix.Length > 0 ? this.xslPrefix[0] : char.MinValue;
      }
    }

    internal SourceContext CurrentSourceContext
    {
      get
      {
        return new SourceContext(this.document, this.startPos, this.endPos);
      }
    }

    internal Scanner()
    {
    }

    internal Scanner(Document document, ErrorNodeList errors)
    {
      this.document = document;
      this.sourceText = document.Text;
      this.maxPos = document.Text.Length;
      this.errors = errors;
    }

    public string Substring(int start, int length)
    {
      return this.sourceText.Substring(start, length);
    }

    internal Token GetNextToken()
    {
      return this.GetNextToken(false);
    }

    private Token GetNextToken(bool stopAtEndOfLine)
    {
      this.isWhitespace = false;
      switch (this.state)
      {
        case Scanner.State.StringLiteral1:
        case Scanner.State.StringLiteral2:
          if (this.endPos >= this.maxPos)
          {
            if (!stopAtEndOfLine)
              this.HandleError(this.beginBlock, SR.UnclosedString, new string[0]);
            return Token.EndOfFile;
          }
          Token token1;
          if ((int) this.GetChar(this.endPos) == 38)
          {
            this.isWhitespace = false;
            token1 = this.ScanEntityReference();
          }
          else
          {
            token1 = this.ScanXmlString(this.state == Scanner.State.StringLiteral1 ? '"' : '\'', true);
            if (this.stillInsideMultiLineToken)
              this.stillInsideMultiLineToken = false;
            else
              this.state = Scanner.State.Attributes;
          }
          return token1;
        case Scanner.State.Text:
          if (this.endPos >= this.maxPos)
            return Token.EndOfFile;
          Token token2;
          if ((int) this.GetChar(this.endPos) == 38)
          {
            token2 = this.ScanEntityReference();
          }
          else
          {
            token2 = this.ScanXmlText(stopAtEndOfLine);
            if (this.stillInsideMultiLineToken)
              this.stillInsideMultiLineToken = false;
            else
              this.state = Scanner.State.Xml;
          }
          return token2;
        case Scanner.State.LiteralComment:
          if (this.endPos >= this.maxPos)
          {
            if (!stopAtEndOfLine)
              this.HandleError(this.beginBlock, SR.UnclosedComment, new string[0]);
            return Token.EndOfFile;
          }
          Token token3 = this.ScanXmlComment(stopAtEndOfLine);
          if (this.stillInsideMultiLineToken)
            this.stillInsideMultiLineToken = false;
          else
            this.state = Scanner.State.Xml;
          return token3;
        case Scanner.State.Xml:
          if (this.endPos >= this.maxPos)
            return Token.EndOfFile;
          this.startPos = this.endPos;
          if (this.mode == Scanner.State.InternalSubset)
          {
            this.SkipWhitespace();
            if (this.startPos < this.endPos)
            {
              this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
              this.isWhitespace = true;
              return Token.LiteralContentString;
            }
            if ((int) this.GetChar(this.endPos) == 93 && this.mode == Scanner.State.InternalSubset)
            {
              ++this.endPos;
              this.mode = Scanner.State.Xml;
              this.state = Scanner.State.Tag;
              return Token.RightBracket;
            }
          }
          if ((int) this.GetChar(this.endPos) == 38)
            return this.ScanEntityReference();
          Token token4 = this.ScanXmlText(stopAtEndOfLine);
          if (this.startPos < this.endPos)
          {
            this.state = (int) this.GetChar(this.endPos) != 60 ? Scanner.State.Text : Scanner.State.Tag;
            return token4;
          }
          this.isWhitespace = false;
          break;
        case Scanner.State.CData:
          if (this.endPos >= this.maxPos)
          {
            if (!stopAtEndOfLine)
              this.HandleError(this.beginBlock, SR.UnclosedCData, new string[0]);
            return Token.EndOfFile;
          }
          Token token5 = this.ScanXmlCharacterData(stopAtEndOfLine);
          if (this.stillInsideMultiLineToken)
            this.stillInsideMultiLineToken = false;
          else
            this.state = Scanner.State.Xml;
          return token5;
        case Scanner.State.PI:
          if (this.endPos >= this.maxPos)
          {
            if (!stopAtEndOfLine)
              this.HandleError(this.beginBlock, SR.UnclosedPI, new string[0]);
            return Token.EndOfFile;
          }
          Token token6 = this.ScanXmlProcessingInstructionsTag(stopAtEndOfLine);
          if (this.stillInsideMultiLineToken)
          {
            this.stillInsideMultiLineToken = false;
            if (token6 == Token.Identifier && this.unescapedString == "xml")
              this.state = Scanner.State.Tag;
          }
          else
            this.state = Scanner.State.Xml;
          return token6;
      }
      this.startPos = this.endPos;
      char char1 = this.GetChar(this.endPos++);
      switch (char1)
      {
        case ':':
          this.unescapedString = ":";
          return Token.Colon;
        case '<':
          char char2 = this.GetChar(this.endPos);
          switch (char2)
          {
            case '/':
              this.GetChar(++this.endPos);
              this.state = Scanner.State.EndTag;
              this.unescapedString = "</";
              return Token.StartOfClosingTag;
            case '?':
              ++this.endPos;
              this.beginBlock = this.CurrentSourceContext;
              this.state = Scanner.State.PI;
              this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
              return Token.StartProcessingInstruction;
            case '!':
              char char3 = this.GetChar(++this.endPos);
              switch (char3)
              {
                case '-':
                  if ((int) this.GetChar(++this.endPos) == 45)
                  {
                    ++this.endPos;
                    this.beginBlock = this.CurrentSourceContext;
                    this.state = Scanner.State.LiteralComment;
                    this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
                    return Token.StartLiteralComment;
                  }
                  --this.endPos;
                  --this.endPos;
                  break;
                case '[':
                  if ((int) this.GetChar(this.endPos + 1) == 67 && (int) this.GetChar(this.endPos + 2) == 68 && ((int) this.GetChar(this.endPos + 3) == 65 && (int) this.GetChar(this.endPos + 4) == 84) && ((int) this.GetChar(this.endPos + 5) == 65 && (int) this.GetChar(this.endPos + 6) == 91))
                  {
                    this.endPos += 7;
                    this.beginBlock = this.CurrentSourceContext;
                    this.state = Scanner.State.CData;
                    this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
                    return Token.StartCharacterData;
                  }
                  this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
                  this.state = Scanner.State.Xml;
                  this.HandleError(SR.ExpectingToken, "CDATA[");
                  return Token.IllegalCharacter;
                default:
                  if (XmlCharType.IsStartNameChar(char3))
                  {
                    this.state = Scanner.State.Tag;
                    this.unescapedString = "<!";
                    if (this.mode != Scanner.State.InternalSubset)
                      this.mode = Scanner.State.DocType;
                    return Token.MarkupDeclaration;
                  }
                  this.unescapedString = char3.ToString();
                  this.HandleError(SR.ExpectingToken, "DOCTYPE");
                  this.state = Scanner.State.Xml;
                  return Token.IllegalCharacter;
              }
              break;
            default:
              if (!XmlCharType.IsStartNameChar(char2))
              {
                this.HandleError(SR.ExpectingTagName);
                this.state = Scanner.State.Xml;
                break;
              }
              this.unescapedString = "<";
              this.state = Scanner.State.Tag;
              break;
          }
          return Token.StartOfTag;
        case '=':
          this.state = Scanner.State.Attributes;
          this.unescapedString = "=";
          return Token.Assign;
        case '>':
          this.unescapedString = ">";
          Token token7 = this.state == Scanner.State.EndTag ? Token.EndOfEndTag : Token.EndOfTag;
          this.state = Scanner.State.Xml;
          if (this.mode == Scanner.State.DocType)
            this.mode = Scanner.State.Xml;
          return token7;
        case '?':
          if ((int) this.GetChar(this.endPos) == 62)
          {
            ++this.endPos;
            this.unescapedString = "?>";
            this.state = Scanner.State.Xml;
            return Token.EndOfTag;
          }
          break;
        case '[':
          this.unescapedString = "[";
          this.mode = Scanner.State.InternalSubset;
          return Token.LeftBracket;
        case ']':
          this.unescapedString = "]";
          if ((int) this.GetChar(this.endPos) == 62)
          {
            this.unescapedString = "]";
            this.state = Scanner.State.Tag;
            this.mode = Scanner.State.Xml;
          }
          return Token.RightBracket;
        case '\'':
        case '"':
          this.LiteralQuoteChar = char1;
          this.unescapedString = char1.ToString();
          this.state = (int) char1 == 34 ? Scanner.State.StringLiteral1 : Scanner.State.StringLiteral2;
          this.beginBlock = this.CurrentSourceContext;
          return Token.StartStringLiteral;
        case '/':
          if ((int) this.GetChar(this.endPos) != 62)
            return Token.StringLiteral;
          this.unescapedString = "/>";
          ++this.endPos;
          this.state = Scanner.State.Xml;
          return Token.EndOfSimpleTag;
        case char.MinValue:
          this.startPos = this.endPos;
          return Token.EndOfFile;
        case '\t':
        case '\n':
        case '\r':
        case ' ':
          this.SkipWhitespace();
          if (this.state != Scanner.State.EndTag)
            this.state = Scanner.State.Attributes;
          return Token.Whitespace;
      }
      if (this.mode == Scanner.State.InternalSubset)
      {
        switch (char1)
        {
          case '#':
            this.unescapedString = "#";
            return Token.Pound;
          case '%':
            this.unescapedString = "%";
            return Token.Percent;
          case '(':
            this.unescapedString = "(";
            return Token.LeftParen;
          case ')':
            this.unescapedString = ")";
            return Token.RightParen;
          case '*':
            this.unescapedString = "*";
            return Token.Star;
          case '+':
            this.unescapedString = "+";
            return Token.Plus;
          case ',':
            this.unescapedString = ",";
            return Token.Comma;
          case '?':
            this.unescapedString = "?";
            return Token.QuestionMark;
          case '|':
            this.unescapedString = "|";
            return Token.Or;
        }
      }
      if (XmlCharType.IsNameChar(char1))
      {
        --this.endPos;
        if (this.mode == Scanner.State.InternalSubset || this.mode == Scanner.State.DocType)
        {
          Token token8 = this.ScanKeyword();
          if (token8 != Token.Identifier)
            return token8;
        }
        this.ScanName();
        if (this.lastToken != Token.Colon)
          this.isXslKeyword = this.IsXslKeyword();
        return Token.Identifier;
      }
      this.unescapedString = char1.ToString();
      this.HandleError(SR.IllegalNameCharacter, new string[2]
      {
        char1.ToString(),
        Convert.ToInt32(char1).ToString()
      });
      return Token.IllegalCharacter;
    }

    private bool IsXslKeyword()
    {
      if (this.xslPrefix != null)
      {
        char @char = this.GetChar(this.endPos);
        if ((int) @char == 58 && (int) this.xslStartChar != 0)
          return this.unescapedString == this.xslPrefix;
        if ((int) @char != 58 && (int) this.xslStartChar == 0)
          return true;
      }
      return false;
    }

    internal bool IsKeyword(Token t)
    {
      switch (t)
      {
        case Token.PUBLIC:
        case Token.SYSTEM:
        case Token.ANY:
        case Token.EMPTY:
        case Token.CDATA:
        case Token.NOTATION:
        case Token.NDATA:
        case Token.ID:
        case Token.IDREF:
        case Token.IDREFS:
        case Token.ENTITY:
        case Token.ENTITIES:
        case Token.NMTOKEN:
        case Token.NMTOKENS:
        case Token.REQUIRED:
        case Token.IMPLIED:
        case Token.PCDATA:
        case Token.FIXED:
          return true;
        default:
          return false;
      }
    }

    private void SkipWhitespace()
    {
      int start = this.startPos;
      char @char = this.GetChar(this.endPos);
      while ((int) @char == 32 || (int) @char == 9 || ((int) @char == 13 || (int) @char == 10))
        @char = this.GetChar(++this.endPos);
      this.unescapedString = this.Substring(start, this.endPos - start);
    }

    private char GetChar(int index)
    {
      if (index < this.maxPos && index >= 0)
        return this.sourceText[index];
      return char.MinValue;
    }

    internal Identifier GetIdentifier()
    {
      if (this.identifier.Length > 0)
      {
        Identifier identifier = new Identifier(this.identifier.ToString());
        identifier.SourceContext = this.CurrentSourceContext;
        return identifier;
      }
      int offset = this.startPos;
      int length = this.endPos - offset;
      Identifier identifier1 = new Identifier(this.sourceText, offset, length);
      identifier1.SourceContext = this.CurrentSourceContext;
      return identifier1;
    }

    internal string GetIdentifierString()
    {
      if (this.identifier.Length > 0)
        return this.identifier.ToString();
      int num = this.startPos;
      if ((int) this.GetChar(num) == 64)
        ++num;
      return this.Substring(num, this.endPos - this.startPos);
    }

    internal string GetString()
    {
      return this.unescapedString;
    }

    internal Literal GetStringLiteral()
    {
      if (this.isWhitespace)
        return (Literal) new WhitespaceLiteral(this.unescapedString, this.CurrentSourceContext);
      return new Literal(this.unescapedString, this.CurrentSourceContext);
    }

    internal string GetTokenSource()
    {
      return this.Substring(this.startPos, this.endPos - this.startPos);
    }

    private void ScanName()
    {
      char @char = this.GetChar(this.endPos);
      if (!XmlCharType.IsStartNameChar(@char))
      {
        if ((int) @char != 0)
          ++this.endPos;
        this.HandleError(SR.IllegalStartNameCharacter, new string[2]
        {
          @char.ToString(),
          Convert.ToInt32(@char).ToString()
        });
      }
      while (true)
      {
        @char = this.GetChar(this.endPos);
        if ((int) @char != 58 && XmlCharType.IsNameChar(@char))
          ++this.endPos;
        else
          break;
      }
      this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
    }

    private void ScanNMToken()
    {
      while (XmlCharType.IsNameChar(this.GetChar(this.endPos)))
        ++this.endPos;
    }

    private Token ScanKeyword()
    {
      int startPos = this.endPos;
      char @char = this.GetChar(this.endPos);
      if ((int) @char > 90 || (int) @char < 65)
        return Token.Identifier;
      while (XmlCharType.IsNameChar(this.GetChar(this.endPos)))
        ++this.endPos;
      Keyword keyword = Scanner.Keywords[(int) @char - 65];
      Token token = Token.Identifier;
      if (keyword != null)
        token = keyword.GetKeyword(this.sourceText, startPos, this.endPos);
      if (token == Token.Identifier)
        this.endPos = startPos;
      else
        this.unescapedString = this.GetTokenSource();
      return token;
    }

    internal bool ScanNamespaceSeparator()
    {
      if (this.endPos >= this.maxPos - 2 || (int) this.GetChar(this.endPos) != 58 || !XmlCharType.IsStartNameChar(this.GetChar(this.endPos + 1)))
        return false;
      this.startPos = this.endPos;
      ++this.endPos;
      return true;
    }

    private Token HandleIllegalSurrogatePair(char high, char low, Token inProgress)
    {
      this.stillInsideMultiLineToken = true;
      this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
      if (this.endPos > this.startPos)
        return inProgress;
      if (this.endPos < this.maxPos)
        ++this.endPos;
      this.HandleError(SR.IllegalSurrogatePair, new string[2]
      {
        Convert.ToInt32(high).ToString("x"),
        Convert.ToInt32(low).ToString("x")
      });
      return Token.IllegalCharacter;
    }

    private Token ScanXmlString(char closingQuote, bool stopAtEndOfLine)
    {
      this.startPos = this.endPos;
      this.unescapedString = (string) null;
      this.isWhitespace = false;
      char char1 = this.GetChar(this.endPos);
      if ((int) char1 >= 55296 && (int) char1 <= 56319)
      {
        char char2 = this.GetChar(this.endPos + 1);
        if ((int) char2 < 56320 || (int) char2 > 57343)
        {
          this.stillInsideMultiLineToken = true;
          if (this.endPos < this.maxPos)
            ++this.endPos;
          this.HandleError(SR.IllegalSurrogatePair, new string[2]
          {
            Convert.ToInt32(char1).ToString("x"),
            Convert.ToInt32(char2).ToString("x")
          });
          return Token.IllegalCharacter;
        }
        this.endPos += 2;
      }
      else
      {
        if (!XmlCharType.IsCharData(char1))
        {
          this.stillInsideMultiLineToken = true;
          ++this.endPos;
          this.unescapedString = char1.ToString();
          this.HandleError(SR.IllegalCharacter, new string[2]
          {
            char1.ToString(),
            Convert.ToInt32(char1).ToString()
          });
          return Token.IllegalCharacter;
        }
        if ((int) char1 == 60 && this.mode != Scanner.State.InternalSubset)
        {
          ++this.endPos;
          this.unescapedString = char1.ToString();
          this.HandleError(SR.IllegalAttributeCharacter, new string[2]
          {
            char1.ToString(),
            Convert.ToInt32(char1).ToString()
          });
          --this.endPos;
          return Token.IllegalCharacter;
        }
      }
      char char3;
      do
      {
        char3 = this.GetChar(this.endPos++);
        if ((int) char3 >= 55296 && (int) char3 <= 56319)
        {
          char char2 = this.GetChar(this.endPos++);
          if ((int) char2 < 56320 || (int) char2 > 57343)
          {
            this.stillInsideMultiLineToken = true;
            break;
          }
        }
        else
        {
          if (!XmlCharType.IsCharData(char3))
          {
            this.stillInsideMultiLineToken = true;
            break;
          }
          if ((int) char3 == 60 && this.mode != Scanner.State.InternalSubset)
          {
            this.stillInsideMultiLineToken = true;
            break;
          }
          if ((int) char3 == 38)
          {
            this.stillInsideMultiLineToken = true;
            break;
          }
          if ((int) char3 == 0 && this.endPos >= this.maxPos)
          {
            if (stopAtEndOfLine)
              this.stillInsideMultiLineToken = true;
            --this.endPos;
            break;
          }
        }
      }
      while ((int) char3 != (int) closingQuote);
      this.unescapedString = this.endPos > this.startPos + 1 ? this.Substring(this.startPos, this.endPos - this.startPos - 1) : "";
      if ((int) char3 != (int) closingQuote)
        --this.endPos;
      this.isWhitespace = false;
      return Token.StringLiteral;
    }

    private Token HandleIllegalCharacter(char c, Token inProgress)
    {
      this.stillInsideMultiLineToken = true;
      this.unescapedString = this.Substring(this.startPos, this.endPos - this.startPos);
      if (this.endPos > this.startPos)
        return inProgress;
      if (this.endPos < this.maxPos)
        ++this.endPos;
      this.HandleError(SR.IllegalCharacter, new string[2]
      {
        c.ToString(),
        Convert.ToInt32(c).ToString()
      });
      return Token.IllegalCharacter;
    }

    private Token ScanXmlCharacterData(bool stopAtEndOfLine)
    {
      int start = this.startPos = this.endPos;
      char char1;
      char char2;
      while (true)
      {
        char1 = this.GetChar(this.endPos);
        if ((int) char1 >= 55296 && (int) char1 <= 56319)
        {
          char2 = this.GetChar(this.endPos + 1);
          if ((int) char2 >= 56320 && (int) char2 <= 57343)
            this.endPos += 2;
          else
            break;
        }
        else if (XmlCharType.IsCharData(char1))
        {
          while ((int) char1 == 93)
          {
            char1 = this.GetChar(++this.endPos);
            if ((int) char1 == 93)
            {
              while ((int) char1 == 93)
              {
                char1 = this.GetChar(++this.endPos);
                if ((int) char1 == 62)
                {
                  if (this.endPos > start + 2)
                  {
                    this.endPos -= 2;
                    this.unescapedString = this.Substring(start, this.endPos - start);
                    this.stillInsideMultiLineToken = true;
                    return Token.CharacterData;
                  }
                  ++this.endPos;
                  this.state = Scanner.State.Xml;
                  return Token.EndOfTag;
                }
                if ((int) char1 == 0 && this.endPos >= this.maxPos)
                {
                  if (stopAtEndOfLine)
                    this.stillInsideMultiLineToken = true;
                  return Token.CharacterData;
                }
              }
            }
            else if ((int) char1 == 0 && this.endPos >= this.maxPos)
            {
              if (stopAtEndOfLine)
                this.stillInsideMultiLineToken = true;
              return Token.CharacterData;
            }
          }
          if ((int) char1 != 0 || this.endPos < this.maxPos)
            ++this.endPos;
          else
            goto label_23;
        }
        else
          goto label_6;
      }
      return this.HandleIllegalSurrogatePair(char1, char2, Token.CharacterData);
label_6:
      return this.HandleIllegalCharacter(char1, Token.CharacterData);
label_23:
      if (stopAtEndOfLine)
        this.stillInsideMultiLineToken = true;
      return Token.CharacterData;
    }

    private Token ScanXmlComment(bool stopAtEndOfLine)
    {
      int start = this.endPos;
      this.startPos = this.endPos;
      char char1;
      char char2;
      while (true)
      {
        char1 = this.GetChar(this.endPos);
        if ((int) char1 >= 55296 && (int) char1 <= 56319)
        {
          char2 = this.GetChar(this.endPos + 1);
          if ((int) char2 >= 56320 && (int) char2 <= 57343)
            this.endPos += 2;
          else
            break;
        }
        else if (XmlCharType.IsCharData(char1))
        {
          while ((int) char1 == 45)
          {
            char1 = this.GetChar(++this.endPos);
            if ((int) char1 == 45)
            {
              char1 = this.GetChar(++this.endPos);
              if ((int) char1 == 62)
              {
                if (this.endPos > start + 3)
                {
                  this.endPos -= 2;
                  this.unescapedString = this.Substring(start, this.endPos - start);
                  this.stillInsideMultiLineToken = true;
                  return Token.LiteralComment;
                }
                ++this.endPos;
                this.unescapedString = "-->";
                this.state = Scanner.State.Xml;
                return Token.EndOfTag;
              }
              if ((int) char1 != 0)
              {
                if (this.endPos > start + 2)
                {
                  this.endPos -= 2;
                  this.unescapedString = this.Substring(start, this.endPos - start);
                  this.stillInsideMultiLineToken = true;
                  return Token.LiteralComment;
                }
                this.unescapedString = this.Substring(start, this.endPos - start);
                this.stillInsideMultiLineToken = true;
                this.HandleError(SR.IllegalComment, this.unescapedString);
                return Token.IllegalCharacter;
              }
              if ((int) char1 == 0 && this.endPos >= this.maxPos)
                return Token.LiteralComment;
            }
            else if ((int) char1 == 0 && this.endPos >= this.maxPos)
            {
              if (stopAtEndOfLine)
                this.stillInsideMultiLineToken = true;
              return Token.LiteralComment;
            }
          }
          if ((int) char1 != 0 || this.endPos < this.maxPos)
            ++this.endPos;
          else
            goto label_24;
        }
        else
          goto label_6;
      }
      return this.HandleIllegalSurrogatePair(char1, char2, Token.LiteralComment);
label_6:
      return this.HandleIllegalCharacter(char1, Token.LiteralComment);
label_24:
      if (stopAtEndOfLine)
        this.stillInsideMultiLineToken = true;
      return Token.LiteralComment;
    }

    public Identifier GetEntityName()
    {
      return this.entityName;
    }

    public char GetCharEntity()
    {
      return this.charEntity;
    }

    private Token ScanEntityReference()
    {
      this.startPos = this.endPos;
      if ((int) this.GetChar(++this.endPos) == 35)
      {
        this.charEntity = this.ExpandCharEntity();
        return Token.CharacterEntity;
      }
      int start = this.endPos;
      this.ScanNMToken();
      if ((int) this.GetChar(this.endPos) == 59)
      {
        string name = this.Substring(start, this.endPos - start);
        char ch;
        switch (name)
        {
          case "amp":
            ch = '&';
            break;
          case "lt":
            ch = '<';
            break;
          case "gt":
            ch = '>';
            break;
          case "quot":
            ch = '"';
            break;
          case "apos":
            ch = '\'';
            break;
          default:
            this.entityName = Identifier.For(name);
            this.entityName.SourceContext = this.CurrentSourceContext;
            ++this.endPos;
            this.unescapedString = this.Substring(start - 1, this.endPos - start + 1);
            return Token.GeneralEntityReference;
        }
        this.charEntity = ch;
        ++this.endPos;
        this.unescapedString = this.Substring(start - 1, this.endPos - start + 1);
        return Token.CharacterEntity;
      }
      this.unescapedString = this.Substring(start - 1, this.endPos - start + 1);
      this.HandleError(SR.ExpectingToken, ";");
      return Token.IllegalCharacter;
    }

    public char ExpandCharEntity()
    {
      int start = this.endPos;
      char @char = this.GetChar(++this.endPos);
      int num1 = 0;
      if ((int) @char == 120)
      {
        for (@char = this.GetChar(++this.endPos); (int) @char != 0 && (int) @char != 59; @char = this.GetChar(++this.endPos))
        {
          int num2;
          if ((int) @char >= 48 && (int) @char <= 57)
            num2 = (int) @char - 48;
          else if ((int) @char >= 97 && (int) @char <= 102)
            num2 = (int) @char - 97 + 10;
          else if ((int) @char >= 65 && (int) @char <= 70)
          {
            num2 = (int) @char - 65 + 10;
          }
          else
          {
            this.HandleError(SR.BadHexDigit, this.endPos - 1, new string[1]
            {
              this.Substring(this.endPos, 1)
            });
            break;
          }
          if (num1 > ((int) ushort.MaxValue - num2) / 16)
          {
            this.HandleError(SR.EntityOverflow, this.Substring(start, this.endPos - start));
            break;
          }
          num1 = num1 * 16 + num2;
        }
      }
      else
      {
        for (; (int) @char != 0 && (int) @char != 59; @char = this.GetChar(++this.endPos))
        {
          if ((int) @char >= 48 && (int) @char <= 57)
          {
            int num2 = (int) @char - 48;
            if (num1 > ((int) ushort.MaxValue - num2) / 16)
            {
              this.HandleError(SR.EntityOverflow, this.Substring(start, this.endPos - start));
              break;
            }
            num1 = num1 * 10 + num2;
          }
          else
          {
            this.HandleError(SR.BadDecimalDigit, this.Substring(this.endPos, 1));
            break;
          }
        }
      }
      if ((int) @char == 0)
        this.HandleError(SR.ExpectingToken, ";");
      else
        ++this.endPos;
      this.unescapedString = this.Substring(start - 1, this.endPos - start + 1);
      return Convert.ToChar(num1);
    }

    private Token ScanXmlProcessingInstructionsTag(bool stopAtEndOfLine)
    {
      int start = this.endPos;
      this.startPos = this.endPos;
      if ((int) this.GetChar(this.endPos - 1) == 63)
      {
        this.stillInsideMultiLineToken = true;
        this.ScanName();
        return Token.Identifier;
      }
      char char1;
      char char2;
      while (true)
      {
        char1 = this.GetChar(this.endPos);
        if ((int) char1 >= 55296 && (int) char1 <= 56319)
        {
          char2 = this.GetChar(this.endPos + 1);
          if ((int) char2 >= 56320 && (int) char2 <= 57343)
            this.endPos += 2;
          else
            break;
        }
        else if (XmlCharType.IsCharData(char1))
        {
          while ((int) char1 == 63)
          {
            char1 = this.GetChar(++this.endPos);
            if ((int) char1 == 62)
            {
              if (this.endPos > start + 2)
              {
                --this.endPos;
                this.unescapedString = this.Substring(start, this.endPos - start);
                this.stillInsideMultiLineToken = true;
                return Token.ProcessingInstructions;
              }
              ++this.endPos;
              this.state = Scanner.State.Xml;
              return Token.EndOfTag;
            }
            if ((int) char1 == 0 && this.endPos >= this.maxPos)
            {
              if (stopAtEndOfLine)
                this.stillInsideMultiLineToken = true;
              return Token.ProcessingInstructions;
            }
          }
          if ((int) char1 != 0 || this.endPos < this.maxPos)
            ++this.endPos;
          else
            goto label_18;
        }
        else
          goto label_7;
      }
      return this.HandleIllegalSurrogatePair(char1, char2, Token.ProcessingInstructions);
label_7:
      return this.HandleIllegalCharacter(char1, Token.ProcessingInstructions);
label_18:
      if (stopAtEndOfLine)
        this.stillInsideMultiLineToken = true;
      return Token.ProcessingInstructions;
    }

    private Token ScanXmlText(bool stopAtEndOfLine)
    {
      int num = this.endPos;
      this.startPos = this.endPos;
      this.unescapedString = (string) null;
      this.isWhitespace = true;
      char char1;
      while (true)
      {
        do
        {
          char1 = this.GetChar(this.endPos++);
          if ((int) char1 == 38)
          {
            this.stillInsideMultiLineToken = true;
            goto label_15;
          }
          else if ((int) char1 == 0 && this.endPos >= this.maxPos)
          {
            if (stopAtEndOfLine)
            {
              this.stillInsideMultiLineToken = true;
              goto label_15;
            }
            else
              goto label_15;
          }
          else if ((int) char1 >= 55296 && (int) char1 <= 56319)
          {
            char char2 = this.GetChar(this.endPos);
            if ((int) char2 < 56320 || (int) char2 > 57343)
            {
              --this.endPos;
              return this.HandleIllegalSurrogatePair(char1, char2, Token.LiteralContentString);
            }
            ++this.endPos;
          }
          else
          {
            if (!XmlCharType.IsCharData(char1))
            {
              --this.endPos;
              return this.HandleIllegalCharacter(char1, Token.LiteralContentString);
            }
            if ((int) char1 == 60)
              goto label_15;
          }
        }
        while (!this.isWhitespace || Scanner.IsXmlWhitespace(char1));
        this.isWhitespace = false;
      }
label_15:
      int length = this.endPos - num - 1;
      this.unescapedString = length > 0 ? this.Substring(this.startPos, length) : "";
      if ((int) char1 == 60 || (int) char1 == 38)
        --this.endPos;
      return Token.LiteralContentString;
    }

    public static bool IsXmlWhitespace(char c)
    {
      return (int) c == 32 || (int) c == 9 || ((int) c == 13 || (int) c == 10);
    }

    internal static bool IsDigit(char c)
    {
      if (48 <= (int) c)
        return (int) c <= 57;
      return false;
    }

    internal static bool IsHexDigit(char c)
    {
      if (Scanner.IsDigit(c) || 65 <= (int) c && (int) c <= 70)
        return true;
      if (97 <= (int) c)
        return (int) c <= 102;
      return false;
    }

    internal static bool IsAsciiLetter(char c)
    {
      if (65 <= (int) c && (int) c <= 90)
        return true;
      if (97 <= (int) c)
        return (int) c <= 122;
      return false;
    }

    internal static bool IsUnicodeLetter(char c)
    {
      if ((int) c >= 128)
        return char.IsLetter(c);
      return false;
    }

    private void HandleError(string key, params string[] messageParameters)
    {
      this.HandleError(key, this.startPos, messageParameters);
    }

    private void HandleError(string code, int pos, params string[] messageParameters)
    {
      if (this.errors == null || this.endPos <= this.lastReportedErrorPos)
        return;
      this.lastReportedErrorPos = this.endPos;
      this.HandleError(new SourceContext(this.document, pos, this.endPos), code, messageParameters);
    }

    private void HandleError(SourceContext context, string code, params string[] messageParameters)
    {
      if (this.errors == null)
        return;
      this.errors.Add(new ErrorNode(code, messageParameters)
      {
        SourceContext = context
      });
    }

    internal enum State
    {
      Content = 0,
      Xml = 0,
      InternalSubset = 1,
      DocType = 2,
      ModeMask = 3,
      Tag = 4,
      Attributes = 8,
      CData = 16,
      PI = 32,
      Text = 64,
      LiteralComment = 128,
      StringLiteral1 = 256,
      StringLiteral2 = 512,
      EndTag = 1024,
    }
  }
}
