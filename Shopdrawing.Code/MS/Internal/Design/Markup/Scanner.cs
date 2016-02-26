// Decompiled with JetBrains decompiler
// Type: MS.Internal.Design.Markup.Scanner
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using System;
using System.Globalization;
using System.Text;

namespace MS.Internal.Design.Markup
{
  internal class Scanner
  {
    private StringBuilder _stringBuilder = new StringBuilder(4096);
    private StringBuilder _identifierBuilder = new StringBuilder(128);
    internal int _startPos;
    internal int _endPos;
    internal int _maxPos;
    internal int _line;
    private IScannerSource _source;
    private string _text;
    private int _offset;
    private bool _wasStringLiteralTerminatedProperly;
    private bool _isWhitespace;
    private bool _wasWhitespaceSkipped;
    private bool _stillInsideMultiLineToken;
    private bool _ignoreTrailingText;
    private int _lastEndPosOnIdBuilder;
    private ScannerErrorHandler _errorHandler;
    private State _state;
    private int _scannerState;
    private bool _treatNumericAsStartingChar;

    public bool StringLiteralTerminatedProperly
    {
      get
      {
        return this._wasStringLiteralTerminatedProperly;
      }
    }

    public State State
    {
      get
      {
        return this._state;
      }
    }

    public bool IsNumericAsStartingChar
    {
      get
      {
        return this._treatNumericAsStartingChar;
      }
      set
      {
        this._treatNumericAsStartingChar = value;
      }
    }

    public bool IsWhitespaceOrEolBeforeStartingChar
    {
      get
      {
        int index = this._startPos - 1;
        if (index >= 0 && index < this._maxPos)
          return Scanner.IsWhitespaceOrEol(this._text[index]);
        return false;
      }
    }

    public bool WasWhitespaceSkipped
    {
      get
      {
        return this._wasWhitespaceSkipped;
      }
    }

    public bool Whitespace
    {
      get
      {
        return this._isWhitespace;
      }
    }

    public bool IgnoreTrailingText
    {
      get
      {
        return this._ignoreTrailingText;
      }
      set
      {
        this._ignoreTrailingText = value;
      }
    }

    public bool StillInsideMultiLineToken
    {
      get
      {
        return this._stillInsideMultiLineToken;
      }
    }

    public int StartPos
    {
      get
      {
        return this._offset + this._startPos;
      }
    }

    public int EndPos
    {
      get
      {
        return this._offset + this._endPos;
      }
    }

    public int Line
    {
      get
      {
        return this._line;
      }
    }

    public int MaxPos
    {
      get
      {
        return this._maxPos;
      }
    }

    public int ScannerState
    {
      get
      {
        return this._scannerState;
      }
      set
      {
        this._scannerState = value;
        this._state = (State) (value & 3);
      }
    }

    internal Scanner(IScannerSource source, ScannerErrorHandler errorHandler)
    {
      this._source = source;
      this._errorHandler = errorHandler;
    }

    private char GetChar(int position, int maxPos)
    {
      if (position < maxPos)
        return this._text[position];
      return char.MinValue;
    }

    internal Token GetNextToken()
    {
      return this.GetNextToken(false);
    }

    internal string GetTokenSource()
    {
      return this._text.Substring(this._startPos, this._endPos - this._startPos);
    }

    internal string GetStringLiteral()
    {
      return this._stringBuilder.ToString();
    }

    public Token ScanNextToken()
    {
      int num = this._scannerState >> 2;
      this._scannerState = 0;
      this._startPos = 0;
      this._stillInsideMultiLineToken = false;
      Token token;
      switch ((Token) (num & 31))
      {
        case Token.Comment:
          this._endPos = this.ScanXmlComment(this._endPos, true);
          token = Token.Comment;
          break;
        case Token.LiteralContentString:
          this._endPos = this.ScanXmlText(this._endPos, true);
          token = Token.LiteralContentString;
          break;
        case Token.ProcessingInstruction:
          this._endPos = this.ScanXmlProcessingInstructionsTag(this._endPos, true);
          token = Token.ProcessingInstruction;
          break;
        case Token.StringLiteral:
          char closingQuote = (char) (num >> 5);
          this._endPos = this.ScanXmlString(closingQuote, this._endPos, true);
          if (this._stillInsideMultiLineToken)
            this._scannerState |= (int) closingQuote << 7;
          token = Token.StringLiteral;
          break;
        default:
          token = this.GetNextToken(true);
          break;
      }
      if (this._stillInsideMultiLineToken)
        this._scannerState |= (int) ((State) ((int) token << 2) | this._state);
      else
        this._scannerState = (int) this._state;
      return token;
    }

    private Token GetNextToken(bool stopAtEndOfLine)
    {
        Token token;
        this._stringBuilder.Length = 0;
        int num = this._endPos;
        int num1 = num;
        this._startPos = num;
        int num2 = num1;
        int num3 = this._maxPos;
        if (this._state == State.XML)
        {
            if (num2 >= num3)
            {
                if (!this.ReadSource(num2))
                {
                    return Token.EndOfFile;
                }
                num2 = this._endPos;
                num3 = this._maxPos;
                if (num3 == 0)
                {
                    return Token.EndOfFile;
                }
            }
            if (this._text[num2] != '<')
            {
                num2 = this.ScanXmlText(num2, stopAtEndOfLine);
                if (this._startPos < num2)
                {
                    this._endPos = num2;
                    return Token.LiteralContentString;
                }
                if (this.GetChar(num2, num3) != '<')
                {
                    this._endPos = num2;
                    return Token.EndOfFile;
                }
                this._state = State.Tag;
            }
            else
            {
                this._state = State.Tag;
            }
        }
        this._isWhitespace = false;
        this._wasWhitespaceSkipped = false;
        while (true)
        {
            char chr = '\0';
            while (true)
            {
                if (num2 == num3)
                {
                    if (!this.ReadSource(num2))
                    {
                        return Token.EndOfFile;
                    }
                    num2 = this._endPos;
                    num3 = this._maxPos;
                }
                int num4 = num2;
                num2 = num4 + 1;
                chr = this._text[num4];
                if (chr == '\n')
                {
                    Scanner scanner = this;
                    scanner._line = scanner._line + 1;
                }
                if (!Scanner.IsWhitespace(chr))
                {
                    break;
                }
                this._wasWhitespaceSkipped = true;
            }
            this._startPos = num2 - 1;
            this._identifierBuilder.Length = 0;
            this._lastEndPosOnIdBuilder = num2 - 1;
            token = Token.EndOfFile;
            switch (chr)
            {
                case '\n':
                    {
                        Scanner scanner1 = this;
                        scanner1._line = scanner1._line + 1;
                        if (!stopAtEndOfLine)
                        {
                            continue;
                        }
                        token = Token.EndOfLine;
                        break;
                    }
                case '\v':
                case '\f':
                case '\u000E':
                case '\u000F':
                case '\u0010':
                case '\u0011':
                case '\u0012':
                case '\u0013':
                case '\u0014':
                case '\u0015':
                case '\u0016':
                case '\u0017':
                case '\u0018':
                case '\u0019':
                case '\u001A':
                case '\u001B':
                case '\u001C':
                case '\u001D':
                case '\u001E':
                case '\u001F':
                case ' ':
                case '!':
                case '#':
                case '$':
                case '%':
                case '&':
                case '-':
                case ';':
                case '@':
                case '\\':
                case '\u005E':
                case '\u0060':
                case '|':
                    {
                        bool flag = false;
                        if (chr == '&')
                        {
                            num2 = this.ScanXmlEscapedChar(num2, ref chr);
                            flag = true;
                        }
                        else if (chr <= '\u0080')
                        {
                            token = Token.IllegalCharacter;
                            this._endPos = num2;
                            return token;
                        }
                        if (!Scanner.IsIdentifierStartChar(chr))
                        {
                            token = Token.IllegalCharacter;
                        }
                        else
                        {
                            if (!flag)
                            {
                                goto case 'z';
                            }
                            this._identifierBuilder.Append(chr);
                            this._lastEndPosOnIdBuilder = num2;
                            goto case 'z';
                        }
                        break;
                    }
                case '\r':
                    {
                        if (this.GetChar(num2, num3) != '\n')
                        {
                            goto case '\n';
                        }
                        num2++;
                        goto case '\n';
                    }
                case '\"':
                case '\'':
                    {
                        this._wasStringLiteralTerminatedProperly = false;
                        Scanner scanner2 = this;
                        scanner2._scannerState = scanner2._scannerState | chr << '\a';
                        num2 = this.ScanXmlString(chr, num2, stopAtEndOfLine);
                        token = Token.StringLiteral;
                        break;
                    }
                case '(':
                    {
                        token = Token.LeftParenthesis;
                        break;
                    }
                case ')':
                    {
                        token = Token.RightParenthesis;
                        break;
                    }
                case '*':
                    {
                        token = Token.Star;
                        break;
                    }
                case '+':
                    {
                        token = Token.Plus;
                        break;
                    }
                case ',':
                    {
                        token = Token.Comma;
                        break;
                    }
                case '.':
                    {
                        token = Token.Dot;
                        break;
                    }
                case '/':
                    {
                        chr = this.GetChar(num2, num3);
                        if (chr != '>')
                        {
                            token = Token.IllegalCharacter;
                        }
                        else
                        {
                            num2++;
                            this._state = State.XML;
                            token = Token.EndOfSimpleTag;
                        }
                        break;
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        if (!this.IsNumericAsStartingChar)
                        {
                            goto case '|';
                        }
                        else
                        {
                            goto case 'z';
                        }
                    }
                case ':':
                    {
                        token = Token.Colon;
                        break;
                    }
                case '<':
                    {
                        token = Token.StartOfTag;
                        int num5 = num2;
                        int num6 = num2;
                        num2 = num6 + 1;
                        chr = this.GetChar(num6, num3);
                        char chr1 = chr;
                        if (chr1 == '!')
                        {
                            int num7 = num2;
                            num2 = num7 + 1;
                            chr = this.GetChar(num7, num3);
                            char chr2 = chr;
                            if (chr2 != '-')
                            {
                                if (chr2 == '[')
                                {
                                    int num8 = num2;
                                    num2 = num8 + 1;
                                    if (this.GetChar(num8, num3) == 'C')
                                    {
                                        int num9 = num2;
                                        num2 = num9 + 1;
                                        if (this.GetChar(num9, num3) == 'D')
                                        {
                                            int num10 = num2;
                                            num2 = num10 + 1;
                                            if (this.GetChar(num10, num3) == 'A')
                                            {
                                                int num11 = num2;
                                                num2 = num11 + 1;
                                                if (this.GetChar(num11, num3) == 'T')
                                                {
                                                    int num12 = num2;
                                                    num2 = num12 + 1;
                                                    if (this.GetChar(num12, num3) == 'A')
                                                    {
                                                        int num13 = num2;
                                                        num2 = num13 + 1;
                                                        if (this.GetChar(num13, num3) == '[')
                                                        {
                                                            num2 = this.ScanXmlCharacterData(num2, stopAtEndOfLine);
                                                            this._state = State.XML;
                                                            token = Token.CharacterData;
                                                            this._endPos = num2;
                                                            return token;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    num2 = num5;
                                }
                                else
                                {
                                    num2 = num5;
                                }
                            }
                            else if (this.GetChar(num2, num3) != '-')
                            {
                                num2 = num5;
                            }
                            else
                            {
                                num2 = this.ScanXmlComment(num2 + 1, stopAtEndOfLine);
                                this._state = State.XML;
                                token = Token.Comment;
                            }
                        }
                        else if (chr1 == '/')
                        {
                            token = Token.StartOfClosingTag;
                        }
                        else if (chr1 == '?')
                        {
                            num2 = this.ScanXmlProcessingInstructionsTag(num2, stopAtEndOfLine);
                            this._state = State.XML;
                            token = Token.ProcessingInstruction;
                        }
                        else
                        {
                            num2 = num5;
                        }
                        break;
                    }
                case '=':
                    {
                        token = Token.Assign;
                        break;
                    }
                case '>':
                    {
                        this._state = State.XML;
                        this._endPos = num2;
                        token = Token.EndOfTag;
                        break;
                    }
                case '?':
                    {
                        chr = this.GetChar(num2, num3);
                        if (chr != '>')
                        {
                            token = Token.IllegalCharacter;
                        }
                        else
                        {
                            num2++;
                            token = Token.EndOfProcessingInstruction;
                        }
                        break;
                    }
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '\u005F':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    {
                        while (true)
                        {
                            int num14 = num2;
                            num2 = num14 + 1;
                            char chr3 = this.GetChar(num14, num3);
                            chr = chr3;
                            switch (chr3)
                            {
                                case '.':
                                    {
                                        if (this._state == State.Tag)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            goto case '\u0060';
                                        }
                                    }
                                case '/':
                                case ':':
                                case ';':
                                case '<':
                                case '=':
                                case '>':
                                case '?':
                                case '@':
                                case '[':
                                case '\\':
                                case ']':
                                case '\u005E':
                                case '\u0060':
                                    {
                                        int num15 = num2 - 1;
                                        bool flag1 = false;
                                        if (chr == '&')
                                        {
                                            num2 = this.ScanXmlEscapedChar(num2, ref chr);
                                            flag1 = true;
                                        }
                                        else if (chr <= '\u0080')
                                        {
                                            num2--;
                                            goto Label6;
                                        }
                                        if (!Scanner.IsIdentifierPartChar(chr, this._state == State.Tag))
                                        {
                                            num2--;
                                        }
                                        else
                                        {
                                            if (!flag1)
                                            {
                                                continue;
                                            }
                                            this._identifierBuilder.Append(this.GetSubstring(this._lastEndPosOnIdBuilder, num15 - this._lastEndPosOnIdBuilder));
                                            this._identifierBuilder.Append(chr);
                                            this._lastEndPosOnIdBuilder = num2;
                                            continue;
                                        }
                                        break;
                                    }
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                case 'F':
                                case 'G':
                                case 'H':
                                case 'I':
                                case 'J':
                                case 'K':
                                case 'L':
                                case 'M':
                                case 'N':
                                case 'O':
                                case 'P':
                                case 'Q':
                                case 'R':
                                case 'S':
                                case 'T':
                                case 'U':
                                case 'V':
                                case 'W':
                                case 'X':
                                case 'Y':
                                case 'Z':
                                case '\u005F':
                                case 'a':
                                case 'b':
                                case 'c':
                                case 'd':
                                case 'e':
                                case 'f':
                                case 'g':
                                case 'h':
                                case 'i':
                                case 'j':
                                case 'k':
                                case 'l':
                                case 'm':
                                case 'n':
                                case 'o':
                                case 'p':
                                case 'q':
                                case 'r':
                                case 's':
                                case 't':
                                case 'u':
                                case 'v':
                                case 'w':
                                case 'x':
                                case 'y':
                                case 'z':
                                    {
                                        continue;
                                    }
                                default:
                                    {
                                        goto case '\u0060';
                                    }
                            }
                        }
                    Label6:
                        token = Token.Identifier;
                        break;
                    }
                case '[':
                    {
                        token = Token.LeftBracket;
                        break;
                    }
                case ']':
                    {
                        token = Token.RightBracket;
                        break;
                    }
                case '{':
                    {
                        token = Token.LeftBrace;
                        break;
                    }
                case '}':
                    {
                        token = Token.RightBrace;
                        break;
                    }
                default:
                    {
                        goto case '|';
                    }
            }
        }
        this._endPos = num2;
        return token;
    }

    private string GetSubstring(int start, int length)
    {
      return this._text.Substring(start, length);
    }

    private int ScanXmlString(char closingQuote, int current, bool stopAtEndOfLine)
    {
      int num = this._maxPos;
      while (true)
      {
        if (current == num)
        {
          bool flag = this.ReadSource(current);
          current = this._endPos;
          num = this._maxPos;
          if (!flag)
            break;
        }
        char ch = this._text[current++];
        if ((int) ch != (int) closingQuote)
        {
          if ((int) ch == 10)
            ++this._line;
          if (!stopAtEndOfLine || !Scanner.IsEndOfLine(ch))
          {
            if ((int) ch != 60)
            {
              if ((int) ch == 38)
              {
                current = this.ScanXmlEscapedChar(current, ref ch);
                this._stringBuilder.Append(ch);
              }
              else
                this._stringBuilder.Append(ch);
            }
            else
              goto label_11;
          }
          else
            goto label_9;
        }
        else
          goto label_5;
      }
      this.HandleError(current, Error.UnterminatedString, (object) closingQuote.ToString());
      goto label_15;
label_5:
      this._wasStringLiteralTerminatedProperly = true;
      goto label_15;
label_9:
      this._stillInsideMultiLineToken = true;
      goto label_15;
label_11:
      this.HandleError(current - 1, Error.UnterminatedString, (object) closingQuote.ToString());
      return current - 1;
label_15:
      return current;
    }

    private int ScanXmlCharacterData(int current, bool stopAtEndOfLine)
    {
      int maxPos = this._maxPos;
      string str = this._text;
      while (true)
      {
        if (current == maxPos)
        {
          bool flag = this.ReadSource(current);
          current = this._endPos;
          maxPos = this._maxPos;
          str = this._text;
          if (!flag)
            break;
        }
        char c = str[current++];
        if ((int) c == 93)
        {
          c = this.GetChar(current, maxPos);
          if ((int) c == 93)
          {
            c = this.GetChar(current + 1, maxPos);
            if ((int) c == 62)
              goto label_7;
          }
        }
        if ((int) c == 10)
          ++this._line;
        if (!stopAtEndOfLine || !Scanner.IsEndOfLine(c))
          this._stringBuilder.Append(c);
        else
          goto label_11;
      }
      this.HandleError(current, Error.UnterminatedCharacterData);
      goto label_13;
label_7:
      current += 2;
      goto label_13;
label_11:
      this._stillInsideMultiLineToken = true;
label_13:
      return current;
    }

    internal int ScanXmlComment(int current, bool stopAtEndOfLine)
    {
      int maxPos = this._maxPos;
      string str = this._text;
      char c;
      do
      {
        if (current >= maxPos)
        {
          bool flag = this.ReadSource(current);
          current = this._endPos;
          maxPos = this._maxPos;
          str = this._text;
          if (!flag)
          {
            if (stopAtEndOfLine)
            {
              this._stillInsideMultiLineToken = true;
              goto label_17;
            }
            else
            {
              this.HandleError(current, Error.UnterminatedComment);
              goto label_17;
            }
          }
        }
        c = str[current++];
        if ((int) c == 45)
        {
          c = this.GetChar(current, maxPos);
          if ((int) c == 45)
          {
            c = this.GetChar(current + 1, maxPos);
            if ((int) c == 62)
            {
              current += 2;
              goto label_17;
            }
            else if ((int) c == 33 && (int) this.GetChar(current + 2, maxPos) == 62)
            {
              current += 3;
              this.HandleError(current, Error.CommentedEndedWithDoubleHyphenBangGreaterThan);
              goto label_17;
            }
            else
            {
              ++current;
              this.HandleError(current, Error.CommentWithDoubleHyphen);
            }
          }
        }
        if ((int) c == 10)
          ++this._line;
      }
      while (!stopAtEndOfLine || !Scanner.IsEndOfLine(c));
      this._stillInsideMultiLineToken = true;
label_17:
      return current;
    }

    private int ScanXmlEscapedChar(int current, ref char ch)
    {
      int maxPos = this._maxPos;
      char @char = this.GetChar(current, maxPos);
      if ((int) @char == 35)
        return this.ScanNumericCharEntity(current, ref ch);
      int start = current;
      while ((int) @char != 0 && (int) @char != 59 && ((int) @char != 34 && (int) @char != 39) && ((int) @char != 62 && (int) @char != 60))
        @char = this.GetChar(++current, maxPos);
      char ch1;
      if ((int) @char == 59)
      {
        string substring = this.GetSubstring(start, current - start);
        switch (substring)
        {
          case "amp":
            ch1 = '&';
            break;
          case "lt":
            ch1 = '<';
            break;
          case "gt":
            ch1 = '>';
            break;
          case "quot":
            ch1 = '"';
            break;
          case "apos":
            ch1 = '\'';
            break;
          default:
            this.HandleError(current, Error.NoSuchNamedEntity, (object) substring);
            ch1 = char.MinValue;
            break;
        }
        ++current;
      }
      else
      {
        this.HandleError(current, Error.MissingSemicolonInEntity);
        ch1 = char.MinValue;
      }
      ch = ch1;
      return current;
    }

    private int ScanNumericCharEntity(int current, ref char ch)
    {
      int start = current;
      int maxPos = this._maxPos;
      char @char = this.GetChar(++current, maxPos);
      int val;
      current = (int) @char != 120 ? this.ScanDecimalCharValue(current, @char, start, out val) : this.ScanHexCharValue(current + 1, this.GetChar(current + 1, maxPos), start, out val);
      ch = val >= 0 ? Convert.ToChar(val) : char.MinValue;
      return current + 1;
    }

    private int ScanDecimalCharValue(int current, char ch, int start, out int val)
    {
      int num1 = 0;
      int maxPos = this._maxPos;
      for (; (int) ch != 0 && (int) ch != 59; ch = this.GetChar(++current, maxPos))
      {
        if ((int) ch >= 48 && (int) ch <= 57)
        {
          int num2 = (int) ch - 48;
          if (num1 > ((int) ushort.MaxValue - num2) / 10)
          {
            this.HandleError(current, Error.EntityOverflow, (object) this.GetSubstring(start, current - start));
            break;
          }
          num1 = num1 * 10 + num2;
        }
        else
        {
          this.HandleError(current, Error.BadDecimalDigit, (object) this.GetSubstring(current, 1));
          break;
        }
      }
      if ((int) ch != 59)
        this.HandleError(current, Error.ExpectedDifferentToken, (object) ";");
      val = num1;
      return current;
    }

    private int ScanHexCharValue(int current, char ch, int start, out int val)
    {
      int num1 = 0;
      int maxPos = this._maxPos;
      for (; (int) ch != 0 && (int) ch != 59 && ((int) ch != 34 && (int) ch != 39) && ((int) ch != 62 && (int) ch != 60); ch = this.GetChar(++current, maxPos))
      {
        int num2;
        if ((int) ch >= 48 && (int) ch <= 57)
          num2 = (int) ch - 48;
        else if ((int) ch >= 97 && (int) ch <= 102)
          num2 = (int) ch - 97 + 10;
        else if ((int) ch >= 65 && (int) ch <= 70)
        {
          num2 = (int) ch - 65 + 10;
        }
        else
        {
          this.HandleError(current, Error.BadHexDigit, (object) this.GetSubstring(current, 1));
          break;
        }
        if (num1 > ((int) ushort.MaxValue - num2) / 16)
        {
          this.HandleError(current, Error.EntityOverflow, (object) this.GetSubstring(start, current - start));
          break;
        }
        num1 = num1 * 16 + num2;
      }
      if ((int) ch != 59)
      {
        this.HandleError(current, Error.ExpectedDifferentToken, (object) ";");
        num1 = -1;
      }
      val = num1;
      return current;
    }

    internal int ScanXmlProcessingInstructionsTag(int current, bool stopAtEndOfLine)
    {
      int maxPos = this._maxPos;
      string str = this._text;
      char c;
      do
      {
        if (current >= maxPos)
        {
          bool flag = this.ReadSource(current);
          current = this._endPos;
          maxPos = this._maxPos;
          str = this._text;
          if (!flag)
          {
            this.HandleError(current, Error.UnterminatedProcessingInstruction);
            goto label_11;
          }
        }
        c = str[current++];
        if ((int) c == 63)
        {
          c = this.GetChar(current, maxPos);
          if ((int) c == 62)
          {
            ++current;
            goto label_11;
          }
        }
        if ((int) c == 10)
          ++this._line;
      }
      while (!stopAtEndOfLine || !Scanner.IsEndOfLine(c));
      this._stillInsideMultiLineToken = true;
label_11:
      return current;
    }

    private int ScanXmlText(int current, bool stopAtEndOfLine)
    {
      bool flag1 = true;
      int num = this._maxPos;
      string str = this._text;
      while (true)
      {
        if (current >= num)
        {
          bool flag2 = this.ReadSource(current);
          current = this._endPos;
          num = this._maxPos;
          str = this._text;
          if (!flag2)
            break;
        }
        char ch = str[current++];
        switch (ch)
        {
          case '<':
            goto label_6;
          case '\n':
            ++this._line;
            break;
        }
        if (!stopAtEndOfLine || !Scanner.IsEndOfLine(ch))
        {
          if ((int) ch == 38)
          {
            flag1 = false;
            current = this.ScanXmlEscapedChar(current, ref ch);
          }
          else
            flag1 = flag1 && Scanner.IsWhitespaceOrEol(ch);
          this._stringBuilder.Append(ch);
        }
        else
          goto label_9;
      }
      if (!this._ignoreTrailingText && !flag1)
      {
        this.HandleError(current, Error.UnterminatedXmlText);
        goto label_14;
      }
      else
        goto label_14;
label_6:
      --current;
      goto label_14;
label_9:
      this._stillInsideMultiLineToken = true;
label_14:
      this._isWhitespace = flag1;
      return current;
    }

    internal static bool IsWhitespace(char c)
    {
      switch (c)
      {
        case '\t':
        case '\v':
        case '\f':
        case ' ':
          return true;
        default:
          return false;
      }
    }

    internal static bool IsWhitespaceOrEol(char c)
    {
      switch (c)
      {
        case '\t':
        case '\n':
        case '\v':
        case '\f':
        case '\r':
        case ' ':
          return true;
        default:
          return false;
      }
    }

    internal static bool IsEndOfLine(char c)
    {
      if ((int) c != 13)
        return (int) c == 10;
      return true;
    }

    internal static bool IsIdentifierPartChar(char c, bool inTag)
    {
      return Scanner.IsIdentifierCharHelper(c, true) || 48 <= (int) c && (int) c <= 57 || ((int) c == 45 || (int) c == 46 && inTag);
    }

    internal static bool IsIdentifierStartChar(char c)
    {
      return Scanner.IsIdentifierCharHelper(c, false);
    }

    internal static bool IsIdentifierCharHelper(char c, bool partChar)
    {
      if (97 <= (int) c && (int) c <= 122 || 65 <= (int) c && (int) c <= 90 || (int) c == 95)
        return true;
      if ((int) c < 128)
        return false;
      switch (char.GetUnicodeCategory(c))
      {
        case UnicodeCategory.UppercaseLetter:
        case UnicodeCategory.LowercaseLetter:
        case UnicodeCategory.TitlecaseLetter:
        case UnicodeCategory.OtherLetter:
        case UnicodeCategory.LetterNumber:
          return true;
        case UnicodeCategory.ModifierLetter:
        case UnicodeCategory.NonSpacingMark:
        case UnicodeCategory.SpacingCombiningMark:
        case UnicodeCategory.EnclosingMark:
        case UnicodeCategory.DecimalDigitNumber:
          return partChar;
        default:
          return false;
      }
    }

    private void HandleError(int current, Error error, params object[] messageParameters)
    {
      if (this._errorHandler == null)
        return;
      this._errorHandler(this._offset + current, current - this._startPos, error, messageParameters);
    }

    private bool ReadSource(int current)
    {
      if (current == 0)
      {
        this._text = this._source.Text;
        if (string.IsNullOrEmpty(this._text))
          return false;
        this._maxPos = this._text.Length;
        if (this._maxPos < 1 || !Scanner.IsWhitespaceOrEol(this._text[this._maxPos - 1]))
          this._text = this._text + " ";
        this._offset = 0;
        return true;
      }
      this._endPos = current;
      return false;
    }

    internal void SetText(string source, int offset)
    {
      this._text = source;
      this._maxPos = source.Length;
      this._endPos = offset;
      if (this._maxPos >= 1 && Scanner.IsWhitespaceOrEol(this._text[this._maxPos - 1]))
        return;
      this._text = this._text + " ";
      ++this._maxPos;
    }
  }
}
