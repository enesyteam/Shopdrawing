// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.ScreenNameReferenceVerifier
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class ScreenNameReferenceVerifier : ReferenceVerifier
  {
    private DesignerContext designerContext;
    private ITypeId referentialObjectType;
    private IPropertyId referentialProperty;
    private bool allowNullOrEmpty;

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

    protected override bool CanReferentialPropertyBeUnset
    {
      get
      {
        return false;
      }
    }

    public ScreenNameReferenceVerifier(DesignerContext designerContext, ITypeId referentialObjectType, IPropertyId referentialProperty, bool allowNullOrEmpty)
    {
      this.designerContext = designerContext;
      this.referentialObjectType = referentialObjectType;
      this.referentialProperty = referentialProperty;
      this.allowNullOrEmpty = allowNullOrEmpty;
    }

    public override bool Verify(DocumentNode node)
    {
      DocumentCompositeNode compositeNode = node as DocumentCompositeNode;
      if (compositeNode != null)
      {
        if (DocumentNodeUtilities.IsBinding(node))
          return true;
        string referentialPropertyValue = this.GetReferentialPropertyValue(compositeNode);
        if (string.IsNullOrEmpty(referentialPropertyValue))
          return this.allowNullOrEmpty;
        IPrototypingService prototypingService = this.designerContext.PrototypingService;
        if (prototypingService != null && prototypingService.ScreenExists(referentialPropertyValue))
          return true;
      }
      return false;
    }
  }
}
