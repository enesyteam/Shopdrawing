// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.CodeAid.Xaml.CodeAidMemberInfoTypes
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Code.Classifiers;
using Microsoft.Expression.DesignModel.Code;

namespace Microsoft.Expression.Code.CodeAid.Xaml
{
  internal static class CodeAidMemberInfoTypes
  {
    private static string Xmlns = "xmlns";
    private const string CloseComment = "-->";
    private const string StartComment = "!--";

    internal static ICodeAidMemberInfo CreateCloseTagInfo(XamlNameDecomposition name)
    {
      string str = CodeAidMemberInfoTypes.ConstructCloseTagFullName(name);
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = str,
        DescriptionTitle = str,
        DescriptionSubtitle = string.Empty,
        DescriptionText = string.Empty
      };
    }

    internal static ICodeAidMemberInfo CreateXamlNamespaceAttributeInfo(string attributeName, string description)
    {
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = attributeName,
        DescriptionTitle = attributeName,
        DescriptionSubtitle = string.Empty,
        DescriptionText = description
      };
    }

    internal static ICodeAidMemberInfo CreatePrefixInfo(string prefix, string boundUri)
    {
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = prefix,
        DescriptionTitle = prefix,
        DescriptionSubtitle = boundUri,
        DescriptionText = string.Empty
      };
    }

    internal static ICodeAidMemberInfo CreateXmlnsInfo()
    {
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = CodeAidMemberInfoTypes.Xmlns,
        DescriptionTitle = CodeAidMemberInfoTypes.Xmlns,
        DescriptionSubtitle = string.Empty,
        DescriptionText = string.Empty
      };
    }

    internal static ICodeAidMemberInfo CreateStartCommentInfo()
    {
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = "!--",
        DescriptionTitle = "!--",
        DescriptionSubtitle = string.Empty,
        DescriptionText = string.Empty
      };
    }

    internal static ICodeAidMemberInfo CreateCloseCommentInfo()
    {
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = "-->",
        DescriptionTitle = "-->",
        DescriptionSubtitle = string.Empty,
        DescriptionText = string.Empty
      };
    }

    internal static ICodeAidMemberInfo CreateNullEnumInfo(string prefix)
    {
      string str = "{" + prefix + ":Null}";
      return (ICodeAidMemberInfo) new CodeAidMemberInfoTypes.CodeAidMemberInfoImpl()
      {
        Name = str,
        DescriptionTitle = str,
        DescriptionSubtitle = string.Empty,
        DescriptionText = string.Empty
      };
    }

    private static string ConstructCloseTagFullName(XamlNameDecomposition name)
    {
      string str = string.Empty;
      if (!string.IsNullOrEmpty(name.PrefixText))
        str = str + name.PrefixText + ":";
      if (!string.IsNullOrEmpty(name.TypeSpecifierText))
        str = str + name.TypeSpecifierText + ".";
      return "/" + (str + name.NameText);
    }

    private class CodeAidMemberInfoImpl : ICodeAidMemberInfo
    {
      public string Name { get; set; }

      public string DescriptionTitle { get; set; }

      public string DescriptionSubtitle { get; set; }

      public string DescriptionText { get; set; }
    }
  }
}
