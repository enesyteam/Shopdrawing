// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.StyleTemplateResourceListCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class StyleTemplateResourceListCommand : ResourceListCommand
  {
    protected override IEnumerable<DocumentCompositeNode> ResourcesInternal
    {
      get
      {
        ResourceManager resourceManager = this.DesignerContext.ResourceManager;
        foreach (DocumentCompositeNode documentCompositeNode in resourceManager.GetResourcesInSelectionScope(PlatformTypes.Style, ResourceResolutionFlags.IncludeApplicationResources | ResourceResolutionFlags.UniqueKeysOnly))
          yield return documentCompositeNode;
        foreach (DocumentCompositeNode documentCompositeNode in resourceManager.GetResourcesInSelectionScope(PlatformTypes.ControlTemplate, ResourceResolutionFlags.IncludeApplicationResources | ResourceResolutionFlags.UniqueKeysOnly))
          yield return documentCompositeNode;
      }
    }

    public StyleTemplateResourceListCommand(SceneViewModel viewModel)
      : base(viewModel, ControlElement.TemplateProperty)
    {
    }

    public ReferenceStep GetTargetProperty(ITypeId resourceTypeId)
    {
      IPlatformMetadata platformMetadata = (IPlatformMetadata) this.ViewModel.ProjectContext.Platform.Metadata;
      if (PlatformTypes.Style.IsAssignableFrom(resourceTypeId))
        return platformMetadata.ResolveProperty(BaseFrameworkElement.StyleProperty) as ReferenceStep;
      if (PlatformTypes.Page.IsAssignableFrom((ITypeId) this.TargetElement.Type))
        return platformMetadata.ResolveProperty(PageElement.TemplateProperty) as ReferenceStep;
      return this.ViewModel.ProjectContext.ResolveProperty(ControlElement.TemplateProperty) as ReferenceStep;
    }

    protected override bool IsResourceCompatible(LocalResourceModel resourceModel)
    {
      DocumentNode resourceNode = resourceModel.ResourceNode;
      ReferenceStep targetProperty = this.GetTargetProperty((ITypeId) resourceNode.Type);
      if (!(this.TargetElement is StyleNode) || !BaseFrameworkElement.StyleProperty.Equals((object) targetProperty))
        return this.DoesStyleOrTemplateApply((IPropertyId) targetProperty, resourceNode);
      return false;
    }

    protected override void SetToResource(LocalResourceModel resourceModel)
    {
      this.SetToResourceInternal(new PropertyReference(this.GetTargetProperty((ITypeId) resourceModel.ResourceNode.Type)), resourceModel);
    }

    protected override bool IsResourceInUse(LocalResourceModel resourceModel)
    {
      DocumentNodePath valueAsDocumentNode = this.TargetElement.GetLocalValueAsDocumentNode(new PropertyReference(this.GetTargetProperty((ITypeId) resourceModel.ResourceNode.Type)));
      if (valueAsDocumentNode != null)
        return valueAsDocumentNode.Node == resourceModel.ResourceNode;
      return false;
    }
  }
}
