// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.ResourcePane.ResourceValueObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.ResourcePane
{
  internal class ResourceValueObjectSet : SceneNodeObjectSetBase
  {
    private ResourceEntryItem resource;
    private DesignerContext designerContext;
    private IInstanceBuilderContext context;

    public override bool ShouldWalkParentsForGetValue
    {
      get
      {
        return false;
      }
    }

    public override bool ShouldAllowAnimation
    {
      get
      {
        return false;
      }
    }

    public override SceneDocument Document
    {
      get
      {
        return this.resource.Container.Document;
      }
    }

    public override IDocumentContext DocumentContext
    {
      get
      {
        return this.resource.Container.DocumentContext;
      }
    }

    public override IProjectContext ProjectContext
    {
      get
      {
        return this.resource.Container.ProjectContext;
      }
    }

    private bool IsEditable
    {
      get
      {
        return this.resource.Container.IsEditable;
      }
    }

    public override bool IsViewRepresentationValid
    {
      get
      {
        if (this.IsViewOpen())
          return base.IsViewRepresentationValid;
        return true;
      }
    }

    public override SceneViewModel ViewModel
    {
      get
      {
        return this.resource.Container.ViewModel;
      }
    }

    public override bool IsValidForUpdate
    {
      get
      {
        return true;
      }
    }

    public override bool CanSetDynamicExpression
    {
      get
      {
        IType type = this.resource.Resource.ValueNode.Type;
        return PlatformTypes.DependencyObject.IsAssignableFrom((ITypeId) type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) type) || PlatformTypes.IKeyFrame.IsAssignableFrom((ITypeId) type);
      }
    }

    public override bool IsTextRange
    {
      get
      {
        return false;
      }
    }

    public override bool IsEmpty
    {
      get
      {
        return false;
      }
    }

    public override SceneNode[] Objects
    {
      get
      {
        SceneViewModel viewModel = this.ViewModel;
        return new SceneNode[1]
        {
          this.ViewModel.GetSceneNode((DocumentNode) this.resource.Resource.ResourceNode)
        };
      }
    }

    public override Type ObjectType
    {
      get
      {
        return this.resource.EffectiveType;
      }
    }

    public override IType ObjectTypeId
    {
      get
      {
        return this.resource.EffectiveTypeId;
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
      }
    }

    public override bool IsSelectionInvalid
    {
      get
      {
        return this.resource.Resource.ResourceNode.Parent == null;
      }
    }

    public override DocumentNode RepresentativeNode
    {
      get
      {
        return this.resource.Resource.ValueNode;
      }
    }

    public override bool IsTargetedByAnimation
    {
      get
      {
        return false;
      }
    }

    public ResourceValueObjectSet(ResourceEntryItem resource, DesignerContext designerContext, IPropertyInspector transactionContext)
      : base(designerContext, transactionContext)
    {
      this.resource = resource;
      this.designerContext = designerContext;
    }

    public bool IsViewOpen()
    {
      foreach (IView view in (IEnumerable<IView>) this.designerContext.ViewService.Views)
      {
        SceneView sceneView = view as SceneView;
        if (sceneView != null && this.resource.DocumentNode.DocumentRoot == sceneView.Document.DocumentRoot)
          return true;
      }
      return false;
    }

    public override SceneEditTransaction CreateEditTransaction(string description)
    {
      return this.resource.Container.Document.CreateEditTransaction(description);
    }

    public override SceneEditTransaction CreateEditTransaction(string description, bool hidden)
    {
      return this.resource.Container.Document.CreateEditTransaction(description, hidden);
    }

    public override DocumentNode GetLocalValueAsDocumentNode(SceneNodeProperty property, GetLocalValueFlags flags, out bool isMixed)
    {
      bool evaluateExpressions = (flags & GetLocalValueFlags.Resolve) != GetLocalValueFlags.None;
      if (!DictionaryEntryNode.ValueProperty.Equals((object) property.Reference[0]))
        return base.GetLocalValueAsDocumentNode(property, flags, out isMixed);
      PropertyReference reference = property.Reference;
      int stepsResolved;
      DocumentNodePath documentNodePath = DocumentNodeResolver.ResolveValue(new DocumentNodePath(this.resource.Container.DocumentNode, this.resource.DocumentNode), (IList<IProperty>) reference.GetPropertyKeys(), (SceneNode) null, reference.Count, evaluateExpressions, false, (BaseTriggerNode) null, (DocumentNodeResolver.ShouldUseTrigger) null, out stepsResolved);
      isMixed = false;
      if (stepsResolved == reference.Count)
        return documentNodePath.Node;
      return (DocumentNode) null;
    }

    protected override ObservableCollectionWorkaround<LocalResourceModel> RecalculateLocalResources(ObservableCollectionWorkaround<LocalResourceModel> currentResources)
    {
      List<ResourceContainer> activeContainers = new List<ResourceContainer>();
      ResourceContainer container = this.resource.Container;
      if (container is NodeResourceContainer)
      {
        foreach (ResourceContainer resourceContainer in this.designerContext.ResourceManager.ActiveResourceContainers)
        {
          if (resourceContainer.DocumentNode.DocumentRoot == container.DocumentNode.DocumentRoot && resourceContainer.DocumentNode.IsAncestorOf(container.DocumentNode))
            activeContainers.Add(resourceContainer);
        }
      }
      else
        activeContainers.Add(container);
      return this.ProvideLocalResources(activeContainers);
    }

    private void EnsureInstanceBuilderContext()
    {
      if (this.context != null)
        return;
      this.context = (IInstanceBuilderContext) new StandaloneInstanceBuilderContext(this.DocumentContext, this.designerContext);
    }

    public override SceneNodeProperty CreateSceneNodeProperty(PropertyReference propertyReference, AttributeCollection attributes)
    {
      if (propertyReference.Count != 1 || !DictionaryEntryNode.ValueProperty.Equals((object) propertyReference[0]))
        return base.CreateSceneNodeProperty(propertyReference, attributes);
      SceneNodeProperty sceneNodeProperty = (SceneNodeProperty) new ResourcePaneSceneNodeProperty((SceneNodeObjectSet) this, propertyReference, attributes, this.resource.Resource.TargetType, (ITypeResolver) this.ProjectContext);
      sceneNodeProperty.Recache();
      return sceneNodeProperty;
    }

    public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      this.designerContext.ResourceManager.RegisterPropertyChangedEventHandler(this.resource.Resource.Marker, this.resource.Resource.ResourceNode.Context, propertyReference, handler);
    }

    public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      this.designerContext.ResourceManager.UnregisterPropertyChangedEventHandler(this.resource.Resource.Marker, this.resource.Resource.ResourceNode.Context, propertyReference, handler);
    }

    public override object GetValue(PropertyReference propertyReference, PropertyReference.GetValueFlags flags)
    {
      if (!DictionaryEntryNode.ValueProperty.Equals((object) propertyReference[0]))
        return base.GetValue(propertyReference, flags);
      object target = (object) null;
      if (this.IsEditable)
      {
        IViewObject correspondingViewObject = this.ViewModel.DefaultView.GetCorrespondingViewObject(this.RepresentativeSceneNode.DocumentNodePath);
        object obj = correspondingViewObject != null ? correspondingViewObject.PlatformSpecificObject : (object) null;
        if (obj != null && obj is DictionaryEntry)
          target = ((DictionaryEntry) obj).Value;
      }
      if (target == null)
      {
        this.EnsureInstanceBuilderContext();
        using (this.context.DisablePostponedResourceEvaluation())
        {
          this.context.ViewNodeManager.RootNodePath = new DocumentNodePath(this.resource.Resource.ValueNode, this.resource.Resource.ValueNode);
          this.context.ViewNodeManager.Instantiate(this.context.ViewNodeManager.Root);
          target = this.context.ViewNodeManager.ValidRootInstance;
        }
      }
      if (target != null && propertyReference.Count > 1)
        target = propertyReference.PartialGetValue(target, 1, propertyReference.Count - 1);
      return target;
    }
  }
}
