// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Keyword
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal sealed class Keyword
  {
    private Keyword next;
    private Token token;
    private string name;
    private int length;

    private Keyword(Token token, string name)
    {
      this.name = name;
      this.token = token;
      this.length = this.name.Length;
    }

    private Keyword(Token token, string name, Keyword next)
    {
      this.name = name;
      this.next = next;
      this.token = token;
      this.length = this.name.Length;
    }

    internal Token GetKeyword(string source, int startPos, int endPos)
    {
      int num = endPos - startPos;
      Keyword keyword = this;
label_9:
      while (keyword != null)
      {
        if (num == keyword.length)
        {
          string str = keyword.name;
          int index1 = 1;
          int index2 = startPos + 1;
          while (index1 < num)
          {
            if ((int) str[index1] != (int) source[index2])
            {
              keyword = keyword.next;
              goto label_9;
            }
            else
            {
              ++index1;
              ++index2;
            }
          }
          return keyword.token;
        }
        keyword = keyword.next;
      }
      return Token.Identifier;
    }

    internal Token GetKeyword(DocumentText source, int startPos, int endPos)
    {
      int num = endPos - startPos;
      Keyword keyword = this;
label_9:
      while (keyword != null)
      {
        if (num == keyword.length)
        {
          string str = keyword.name;
          int index1 = 1;
          int index2 = startPos + 1;
          while (index1 < num)
          {
            if ((int) str[index1] != (int) source[index2])
            {
              keyword = keyword.next;
              goto label_9;
            }
            else
            {
              ++index1;
              ++index2;
            }
          }
          return keyword.token;
        }
        keyword = keyword.next;
      }
      return Token.Identifier;
    }

    internal static Keyword[] InitKeywords()
    {
      Keyword[] keywordArray = new Keyword[26];
      Keyword keyword1 = new Keyword(Token.ANY, "ANY");
      keywordArray[0] = keyword1;
      Keyword keyword2 = new Keyword(Token.CDATA, "CDATA");
      keywordArray[2] = keyword2;
      Keyword keyword3 = new Keyword(Token.ENTITIES, "ENTITIES", new Keyword(Token.ENTITY, "ENTITY", new Keyword(Token.EMPTY, "EMPTY")));
      keywordArray[4] = keyword3;
      Keyword keyword4 = new Keyword(Token.FIXED, "FIXED");
      keywordArray[5] = keyword4;
      Keyword keyword5 = new Keyword(Token.IMPLIED, "IMPLIED", new Keyword(Token.IDREFS, "IDREFS", new Keyword(Token.IDREF, "IDREF", new Keyword(Token.ID, "ID"))));
      keywordArray[8] = keyword5;
      Keyword keyword6 = new Keyword(Token.NOTATION, "NOTATION", new Keyword(Token.NMTOKENS, "NMTOKENS", new Keyword(Token.NMTOKEN, "NMTOKEN", new Keyword(Token.NDATA, "NDATA"))));
      keywordArray[13] = keyword6;
      Keyword keyword7 = new Keyword(Token.PUBLIC, "PUBLIC", new Keyword(Token.PCDATA, "PCDATA"));
      keywordArray[15] = keyword7;
      Keyword keyword8 = new Keyword(Token.REQUIRED, "REQUIRED");
      keywordArray[17] = keyword8;
      Keyword keyword9 = new Keyword(Token.SYSTEM, "SYSTEM");
      keywordArray[18] = keyword9;
      return keywordArray;
    }
  }
}
