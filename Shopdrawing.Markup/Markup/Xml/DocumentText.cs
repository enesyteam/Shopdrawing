// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.DocumentText
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class DocumentText
  {
    private string source;
    private int length;

    public int Length
    {
      get
      {
        return this.length;
      }
    }

    public char this[int index]
    {
      get
      {
        return this.source[index];
      }
    }

    public DocumentText(string source)
    {
      this.source = source;
      this.length = source.Length;
    }

    public string Substring(int start, int length)
    {
      if (start + length > this.Length)
        length = this.Length - start - 1;
      return this.source.Substring(start, length);
    }
  }
}
