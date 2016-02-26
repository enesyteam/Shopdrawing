// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeStringValue
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeStringValue : IDocumentNodeValue
  {
    private string value;

    public string Value
    {
      get
      {
        return this.value;
      }
    }

    public DocumentNodeStringValue(string value)
    {
      this.value = value;
    }

    public DocumentNodeStringValue Clone(IDocumentContext documentContext)
    {
      return new DocumentNodeStringValue(this.value);
    }

    IDocumentNodeValue IDocumentNodeValue.Clone(IDocumentContext documentContext)
    {
      return (IDocumentNodeValue) this.Clone(documentContext);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      DocumentNodeStringValue documentNodeStringValue = obj as DocumentNodeStringValue;
      if (documentNodeStringValue != null)
        return documentNodeStringValue.value == this.value;
      return false;
    }

    public override int GetHashCode()
    {
      return this.value.GetHashCode();
    }

    public override string ToString()
    {
      return this.value;
    }
  }
}
