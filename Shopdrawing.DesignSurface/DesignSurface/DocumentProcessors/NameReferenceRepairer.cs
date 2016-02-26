// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.NameReferenceRepairer
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal abstract class NameReferenceRepairer : ReferenceRepairer
  {
    protected NameChangeModel NameChangeModel { get; private set; }

    protected abstract IPropertyId ReferentialProperty { get; }

    protected abstract ITypeId ReferentialType { get; }

    public override sealed Predicate<DocumentNode> AppliesTo
    {
      get
      {
        return new Predicate<DocumentNode>(this.AppliesToImpl);
      }
    }

    public NameReferenceRepairer(NameChangeModel nameChangeModel)
    {
      this.NameChangeModel = nameChangeModel;
    }

    public override sealed void Repair(DocumentNode node)
    {
      ((DocumentCompositeNode) node).SetValue<string>(this.ReferentialProperty, this.NameChangeModel.NewReferenceValue);
    }

    private bool AppliesToImpl(DocumentNode node)
    {
      DocumentNodeNameScope documentNodeNameScope = node.NameScope != null ? node.NameScope : node.FindContainingNameScope();
      if (this.ReferentialType.IsAssignableFrom((ITypeId) node.Type) && documentNodeNameScope.Equals((object) this.NameChangeModel.NameScope))
        return ((DocumentCompositeNode) node).GetValueAsString(this.ReferentialProperty) == this.NameChangeModel.OldReferenceValue;
      return false;
    }
  }
}
