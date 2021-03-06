﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignModel.Markup.DocumentRootReference
// Assembly: Microsoft.Expression.Markup, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C29AFBAF-B4D4-48F4-95E5-A72FADF351FB
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Markup.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignModel.Markup
{
  internal sealed class DocumentRootReference : IDocumentNodeReference
  {
    private readonly DocumentNodeNameScope nameScope;
    private IType targetType;

    public DocumentNodeNameScope NameScope
    {
      get
      {
        return this.nameScope;
      }
    }

    public IType TargetType
    {
      get
      {
        return this.targetType;
      }
    }

    public DocumentRootReference(IType targetType)
    {
      this.nameScope = new DocumentNodeNameScope();
      this.targetType = targetType;
    }
  }
}
