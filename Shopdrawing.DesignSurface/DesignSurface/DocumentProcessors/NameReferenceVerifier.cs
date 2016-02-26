// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.NameReferenceVerifier
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  public abstract class NameReferenceVerifier : ReferenceVerifier
  {
    protected override sealed bool CanReferentialPropertyBeUnset
    {
      get
      {
        return true;
      }
    }

    protected virtual string GetNameToValidate(DocumentCompositeNode compositeNode)
    {
      return this.GetReferentialPropertyValue(compositeNode);
    }

    public override sealed bool Verify(DocumentNode node)
    {
      string nameToValidate = this.GetNameToValidate((DocumentCompositeNode) node);
      return !string.IsNullOrEmpty(nameToValidate) && node.FindContainingNameScope().FindNode(nameToValidate) != null;
    }

    public override InvalidReferenceModel CreateInvalidReferenceModel(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode) node;
      return new InvalidReferenceModel(documentCompositeNode, this.ReferentialProperty, this.GetNameToValidate(documentCompositeNode));
    }
  }
}
