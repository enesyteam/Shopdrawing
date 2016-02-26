// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.DocumentProcessors.StateNameReferenceVerifier
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.DocumentProcessors
{
  internal sealed class StateNameReferenceVerifier : ReferenceVerifier
  {
    private StateNameBuilder builder;
    private DesignerContext designerContext;
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

    protected override bool CanReferentialPropertyBeUnset
    {
      get
      {
        return false;
      }
    }

    public StateNameReferenceVerifier(DesignerContext designerContext, ITypeId referentialObjectType, IPropertyId referentialProperty)
    {
      this.designerContext = designerContext;
      this.referentialObjectType = referentialObjectType;
      this.referentialProperty = referentialProperty;
    }

    public override bool Verify(DocumentNode node)
    {
      DocumentCompositeNode compositeNode = node as DocumentCompositeNode;
      if (this.builder == null)
      {
        this.builder = new StateNameBuilder(this.designerContext);
        this.builder.Begin();
      }
      if (compositeNode != null)
      {
        string referentialPropertyValue = this.GetReferentialPropertyValue(compositeNode);
        DocumentNode targetElement = GoToStateActionNode.FindTargetElement(node, true);
        List<string> results;
        if (targetElement != null && (DocumentNodeUtilities.IsBinding(targetElement) && !GoToStateActionNode.CanResolveTargetFromBinding(targetElement) || Enumerable.Contains<string>(this.builder.GetStateNamesForNode(targetElement), referentialPropertyValue) || PlatformNeutralAttributeHelper.TryGetAttributeValues<string>((IEnumerable) TypeUtilities.GetAttributes(targetElement.TargetType), ProjectNeutralTypes.TemplateVisualStateAttribute, "Name", out results) && results.Contains(referentialPropertyValue)))
          return true;
      }
      return false;
    }
  }
}
