// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.BindingObjectReferenceVerifier
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal class BindingObjectReferenceVerifier : NameReferenceVerifier
  {
    private ITypeId referentialObjectType;
    private IPropertyId referentialProperty;

    protected override ITypeId ReferentialObjectType
    {
      get
      {
        return this.referentialObjectType;
      }
    }

    protected override IPropertyId ReferentialProperty
    {
      get
      {
        return this.referentialProperty;
      }
    }

    public BindingObjectReferenceVerifier(ITypeId referentialObjectType, IPropertyId referentialProperty)
    {
      this.referentialObjectType = referentialObjectType;
      this.referentialProperty = referentialProperty;
    }

    protected override string GetNameToValidate(DocumentCompositeNode compositeNode)
    {
      DocumentCompositeNode node = this.GetReferentialPropertyValueAsDocumentNode(compositeNode) as DocumentCompositeNode;
      if (node != null && BindingObjectReferenceVerifier.IsSupportedValue(node))
        return node.GetValueAsString(BindingSceneNode.ElementNameProperty);
      return (string) null;
    }

    protected override bool IsValueValidForVerification(DocumentNode valueNode)
    {
      DocumentCompositeNode node = valueNode as DocumentCompositeNode;
      if (node != null)
        return BindingObjectReferenceVerifier.IsSupportedValue(node);
      return false;
    }

    private static bool IsSupportedValue(DocumentCompositeNode node)
    {
      if (PlatformTypes.Binding.IsAssignableFrom((ITypeId) node.Type) && node.Properties[BindingSceneNode.PathProperty] == null)
        return node.Properties[BindingSceneNode.ElementNameProperty] != null;
      return false;
    }
  }
}
