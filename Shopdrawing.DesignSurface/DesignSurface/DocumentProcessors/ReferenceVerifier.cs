// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ReferenceVerifier
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  public abstract class ReferenceVerifier
  {
    public Predicate<DocumentNode> ShouldVerify
    {
      get
      {
        return new Predicate<DocumentNode>(this.ShouldVerifyImpl);
      }
    }

    protected abstract IPropertyId ReferentialProperty { get; }

    protected abstract ITypeId ReferentialObjectType { get; }

    protected abstract bool CanReferentialPropertyBeUnset { get; }

    public abstract bool Verify(DocumentNode node);

    public virtual InvalidReferenceModel CreateInvalidReferenceModel(DocumentNode node)
    {
      return new InvalidReferenceModel((DocumentCompositeNode) node, this.ReferentialProperty);
    }

    protected string GetReferentialPropertyValue(DocumentCompositeNode compositeNode)
    {
      string str = (string) null;
      DocumentNode valueAsDocumentNode = this.GetReferentialPropertyValueAsDocumentNode(compositeNode);
      if (valueAsDocumentNode != null)
        str = DocumentPrimitiveNode.GetValueAsString(valueAsDocumentNode);
      return str;
    }

    protected DocumentNode GetReferentialPropertyValueAsDocumentNode(DocumentCompositeNode compositeNode)
    {
      DocumentNode documentNode = compositeNode.Properties[this.ReferentialProperty];
      if (documentNode != null && DocumentNodeUtilities.IsResource(documentNode))
        documentNode = ExpressionEvaluator.EvaluateExpression(documentNode);
      return documentNode;
    }

    protected virtual bool IsValueValidForVerification(DocumentNode valueNode)
    {
      if (!DocumentNodeUtilities.IsBinding(valueNode) && !DocumentNodeUtilities.IsTemplateBinding(valueNode))
        return !DocumentNodeUtilities.IsStaticExtension(valueNode);
      return false;
    }

    protected virtual bool IsNodeValidForVerification(DocumentNode node)
    {
      return this.ReferentialObjectType.IsAssignableFrom((ITypeId) node.Type);
    }

    private bool ShouldVerifyImpl(DocumentNode node)
    {
      DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
      if (documentCompositeNode == null || !this.IsNodeValidForVerification((DocumentNode) documentCompositeNode))
        return false;
      IProperty property = node.TypeResolver.ResolveProperty(this.ReferentialProperty);
      if (property == null)
        return false;
      DocumentNode valueNode = documentCompositeNode.Properties[(IPropertyId) property];
      if (valueNode != null)
        return this.IsValueValidForVerification(valueNode);
      return !this.CanReferentialPropertyBeUnset;
    }
  }
}
