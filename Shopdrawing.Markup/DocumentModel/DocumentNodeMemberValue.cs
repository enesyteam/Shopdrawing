// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.DocumentModel.DocumentNodeMemberValue
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.DocumentModel
{
  public sealed class DocumentNodeMemberValue : IDocumentNodeValue
  {
    private IMember member;

    public IMember Member
    {
      get
      {
        return this.member;
      }
    }

    public DocumentNodeMemberValue(IMember member)
    {
      this.member = member;
    }

    public IDocumentNodeValue Clone(IDocumentContext documentContext)
    {
      IMember member = this.member != null ? this.member.Clone(documentContext.TypeResolver) : (IMember) null;
      if (member == null)
        return (IDocumentNodeValue) null;
      return (IDocumentNodeValue) new DocumentNodeMemberValue(member);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      DocumentNodeMemberValue documentNodeMemberValue = obj as DocumentNodeMemberValue;
      if (documentNodeMemberValue == null)
        return false;
      if (documentNodeMemberValue.member == null)
        return documentNodeMemberValue.member == this.member;
      return documentNodeMemberValue.member.Equals((object) this.member);
    }

    public override int GetHashCode()
    {
      return this.member.GetHashCode();
    }

    public override string ToString()
    {
      return this.member.ToString();
    }
  }
}
