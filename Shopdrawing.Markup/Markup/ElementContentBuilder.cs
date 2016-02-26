// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.ElementContentBuilder
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class ElementContentBuilder
  {
    private StringBuilder builder = new StringBuilder();
    private WhitespaceTrimming leadingWhitespace = WhitespaceTrimming.Remove;
    private int siblingCount;
    private XmlSpace xmlSpace;
    private WhitespaceSignificant whitespaceSignificant;

    public XmlSpace WhitespacePreserve
    {
      get
      {
        return this.xmlSpace;
      }
    }

    public int SiblingCount
    {
      get
      {
        return this.siblingCount;
      }
    }

    public ElementContentBuilder(XmlSpace xmlSpace, WhitespaceSignificant whitespaceSignificant)
    {
      this.xmlSpace = xmlSpace;
      this.whitespaceSignificant = whitespaceSignificant;
    }

    public void Append(string text)
    {
      this.builder.Append(text);
    }

    public string GetContentBeforeSibling(WhitespaceTrimming whitespaceSurroundingSibling)
    {
      string builderContent = ElementContentBuilder.GetBuilderContent(this.builder, this.xmlSpace, this.whitespaceSignificant, this.leadingWhitespace, whitespaceSurroundingSibling);
      this.Clear();
      if (builderContent != null)
        ++this.siblingCount;
      ++this.siblingCount;
      this.leadingWhitespace = whitespaceSurroundingSibling;
      return builderContent;
    }

    public string GetRemainingContent()
    {
      string builderContent = ElementContentBuilder.GetBuilderContent(this.builder, this.xmlSpace, this.whitespaceSignificant, this.leadingWhitespace, WhitespaceTrimming.Remove);
      this.Clear();
      if (builderContent != null)
        ++this.siblingCount;
      return builderContent;
    }

    private void Clear()
    {
      if (this.builder.Length <= 0)
        return;
      this.builder = new StringBuilder();
    }

    private static string GetBuilderContent(StringBuilder builder, XmlSpace xmlSpace, WhitespaceSignificant whitespaceSignificant, WhitespaceTrimming leadingWhitespace, WhitespaceTrimming trailingWhitespace)
    {
      int length1 = builder.Length;
      if (length1 > 0)
      {
        if (xmlSpace == XmlSpace.Preserve)
        {
          if (whitespaceSignificant == WhitespaceSignificant.Significant || ElementContentBuilder.ContainsNonWhitespace(builder))
            return builder.ToString();
        }
        else
        {
          char[] chArray = new char[length1];
          int length2 = 0;
          int num = -1;
          for (int index = 0; index < length1; ++index)
          {
            char c = builder[index];
            if (!Scanner.IsXmlWhitespace(c))
            {
              if (index > num + 1 && (length2 > 0 || leadingWhitespace == WhitespaceTrimming.Include))
                chArray[length2++] = ' ';
              chArray[length2++] = c;
              num = index;
            }
          }
          if (length1 > num + 1 && trailingWhitespace == WhitespaceTrimming.Include && (length2 > 0 || whitespaceSignificant == WhitespaceSignificant.Significant && leadingWhitespace == WhitespaceTrimming.Include))
            chArray[length2++] = ' ';
          if (length2 > 0)
            return new string(chArray, 0, length2);
        }
      }
      return (string) null;
    }

    private static bool ContainsNonWhitespace(StringBuilder builder)
    {
      int length = builder.Length;
      for (int index = 0; index < length; ++index)
      {
        if (!Scanner.IsXmlWhitespace(builder[index]))
          return true;
      }
      return false;
    }
  }
}
