// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.Document
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal class Document
  {
    public int Offset;
    public string Name;
    public DocumentText Text;

    public Document()
    {
    }

    public Document(string name, DocumentText text)
    {
      this.Name = name;
      this.Text = text;
    }

    public string GetSourceText(int startOffset, int length)
    {
      if (this.Text == null)
        return (string) null;
      return this.Text.Substring(this.Offset + startOffset, length);
    }
  }
}
