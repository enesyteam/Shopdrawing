// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.Classifiers.XamlTokens
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.VisualStudio.Text.Classification;

namespace Microsoft.Expression.Code.Classifiers
{
  public static class XamlTokens
  {
    public static readonly IClassificationType Tag = TokenClassificationStore.GetTokenType("xmlTag");
    public static readonly IClassificationType QuotedString = TokenClassificationStore.GetTokenType("xmlQuotedString");
    public static readonly IClassificationType CommentDelimiter = TokenClassificationStore.GetTokenType("xmlCommentDelimiter");
    public static readonly IClassificationType ElementName = TokenClassificationStore.GetTokenType("xmlElementName");
    public static readonly IClassificationType Comment = TokenClassificationStore.GetTokenType("xmlComment");
    public static readonly IClassificationType Attribute = TokenClassificationStore.GetTokenType("xmlAttribute");
    public static readonly IClassificationType Text = TokenClassificationStore.GetTokenType("xmlText");
    public static readonly IClassificationType Quote = TokenClassificationStore.GetTokenType("xmlQuote");
    public static readonly IClassificationType MarkupExtension = TokenClassificationStore.GetTokenType("xamlMarkupExtension");
  }
}
