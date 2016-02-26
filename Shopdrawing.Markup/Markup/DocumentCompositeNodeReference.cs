// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DocumentCompositeNodeReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class DocumentCompositeNodeReference : IDocumentNodeReference
  {
    private readonly IDocumentNodeReference parent;
    private readonly DocumentCompositeNode node;
    private readonly DocumentNodeNameScope nameScope;
    private IType targetType;
    private bool isTargetTypeProvider;

    public bool IsRoot
    {
      get
      {
        if (this.parent == null)
          return true;
        return this.parent is DocumentRootReference;
      }
    }

    public IDocumentNodeReference Parent
    {
      get
      {
        return this.parent;
      }
    }

    public DocumentCompositeNode Node
    {
      get
      {
        return this.node;
      }
    }

    public IType Type
    {
      get
      {
        return this.node.Type;
      }
    }

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
        if (this.isTargetTypeProvider)
          return this.targetType;
        return this.parent.TargetType;
      }
    }

    public DocumentCompositeNodeReference(IDocumentNodeReference parent, DocumentCompositeNode node)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      if (node == null)
        throw new ArgumentNullException("node");
      this.parent = parent;
      this.node = node;
      if (!node.TypeResolver.PlatformMetadata.GetIsTypeItsOwnNameScope((ITypeId) this.node.Type))
        return;
      this.nameScope = new DocumentNodeNameScope();
    }

    public void SetTargetType(IType targetType)
    {
      this.isTargetTypeProvider = true;
      this.targetType = targetType;
    }

    public void ClearTargetType()
    {
      this.isTargetTypeProvider = false;
      this.targetType = (IType) null;
    }

    public override string ToString()
    {
      return this.node.ToString();
    }
  }
}
