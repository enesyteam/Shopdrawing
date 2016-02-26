// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DocumentEncodingHelper
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using System.Text;

namespace Microsoft.Expression.DesignModel.Markup
{
  public static class DocumentEncodingHelper
  {
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public static Encoding GetTargetEncoding(Encoding sourceEncoding)
    {
      if (sourceEncoding == null)
        return DocumentEncodingHelper.DefaultEncoding;
      string webName = sourceEncoding.WebName;
      if (webName == Encoding.Unicode.WebName || webName == Encoding.BigEndianUnicode.WebName)
        return Encoding.Unicode;
      return DocumentEncodingHelper.DefaultEncoding;
    }
  }
}
