// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.XmlUtilities
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Markup.Xml;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal static class XmlUtilities
  {
    public static XmlProcessingAttributeType GetProcessingAttributeType(XmlAttribute attribute)
    {
      if (!(attribute.Prefix == "xml"))
        return XmlProcessingAttributeType.NotProcessingAttribute;
      switch (attribute.LocalName)
      {
        case "lang":
          return XmlProcessingAttributeType.Lang;
        case "space":
          return XmlProcessingAttributeType.Space;
        default:
          return XmlProcessingAttributeType.Unrecognized;
      }
    }

    public static bool IsXmlnsDeclaration(XmlAttribute attribute)
    {
      string prefix = attribute.Prefix;
      if (prefix == "xmlns")
        return true;
      if (string.IsNullOrEmpty(prefix))
        return attribute.LocalName == "xmlns";
      return false;
    }

    public static ICollection<string> GetPrefixes(string listOfPrefixes)
    {
      List<string> list = new List<string>();
      string str1 = listOfPrefixes;
      char[] chArray = new char[4]
      {
        ' ',
        '\t',
        '\r',
        '\n'
      };
      foreach (string str2 in str1.Split(chArray))
      {
        if (str2.Length > 0)
          list.Add(str2);
      }
      return (ICollection<string>) list;
    }

    public static string SubstituteOrRemoveCarriageReturnCharacters(string text)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      for (int length = 0; length < text.Length; ++length)
      {
        char ch = text[length];
        if ((int) ch == 13)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(text, 0, length, text.Length);
          if (length == text.Length - 1 || (int) text[length + 1] != 10)
            stringBuilder.Append('\n');
        }
        else if (stringBuilder != null)
          stringBuilder.Append(ch);
      }
      if (stringBuilder != null)
        return stringBuilder.ToString();
      return text;
    }

    public static string GetAttributeValue(XmlAttribute attribute)
    {
      if (attribute.FirstChild == null)
        return string.Empty;
      if (attribute.FirstChild != null && attribute.FirstChild.NextNode == null)
      {
        Literal literal = attribute.FirstChild as Literal;
        if (literal != null)
          return XmlUtilities.GetLiteralValue(literal);
      }
      StringBuilder sb = new StringBuilder();
      for (Microsoft.Expression.DesignModel.Markup.Xml.Node node = attribute.FirstChild; node != null; node = node.NextNode)
      {
        Literal literal;
        if ((literal = node as Literal) != null)
        {
          sb.Append(XmlUtilities.GetLiteralValue(literal));
        }
        else
        {
          XmlEntityReference xmlEntityReference;
          if ((xmlEntityReference = node as XmlEntityReference) != null)
            xmlEntityReference.GetLiteralValue(sb);
        }
      }
      return XmlUtilities.SubstituteOrRemoveCarriageReturnCharacters(sb.ToString());
    }

    private static string GetLiteralValue(Literal literal)
    {
      string text = literal.Value;
      if (text == null)
        return string.Empty;
      string str = XmlUtilities.SubstituteOrRemoveCarriageReturnCharacters(text);
      char[] chArray = (char[]) null;
      for (int index = 0; index < str.Length; ++index)
      {
        switch (str[index])
        {
          case '\t':
          case '\n':
          case '\r':
            if (chArray == null)
              chArray = str.ToCharArray();
            chArray[index] = ' ';
            break;
        }
      }
      if (chArray != null)
        return new string(chArray);
      return str;
    }
  }
}
