// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.Xml.SourceContext
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.Markup.Xml
{
  internal struct SourceContext
  {
    internal Document Document;
    public int EndCol;
    public int StartCol;

    public string SourceText
    {
      get
      {
        if (this.Document == null)
          return (string) null;
        return this.Document.GetSourceText(this.StartCol, this.EndCol - this.StartCol);
      }
    }

    internal SourceContext(Document document, int startCol, int endCol)
    {
      this.Document = document;
      this.StartCol = startCol;
      this.EndCol = endCol;
    }

    public override int GetHashCode()
    {
      return this.EndCol ^ this.StartCol ^ (this.Document != null ? this.Document.GetHashCode() : 0);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is SourceContext))
        return base.Equals(obj);
      SourceContext sourceContext = (SourceContext) obj;
      if (sourceContext.Document == this.Document && sourceContext.EndCol == this.EndCol && sourceContext.StartCol == this.StartCol)
        return sourceContext.SourceText == this.SourceText;
      return false;
    }
  }
}
