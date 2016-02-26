// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DocumentPropertyContentReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class DocumentPropertyContentReference : IDocumentNodeReference
  {
    private readonly IDocumentNodeReference parent;
    private readonly DocumentNodeNameScope nameScope;
    private readonly IProperty property;

    public DocumentNodeNameScope NameScope
    {
      get
      {
        if (this.nameScope != null)
          return this.nameScope;
        return this.parent.NameScope;
      }
    }

    public IType TargetType
    {
      get
      {
        return this.parent.TargetType;
      }
    }

    public IProperty Property
    {
      get
      {
        return this.property;
      }
    }

    public DocumentPropertyContentReference(IPlatformMetadata platformMetadata, IDocumentNodeReference parent, IProperty property)
    {
      this.parent = parent;
      this.property = property;
      if (!platformMetadata.GetIsTypeItsOwnNameScope((ITypeId) this.property.PropertyType))
        return;
      this.nameScope = new DocumentNodeNameScope();
    }
  }
}
