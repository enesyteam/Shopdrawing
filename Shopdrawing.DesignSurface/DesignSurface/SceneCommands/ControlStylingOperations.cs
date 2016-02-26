// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ControlStylingOperations
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  public static class ControlStylingOperations
  {
    public static DocumentNodePath ProvideStyleOrTemplateNodePath(SceneElement targetElement, PropertyReference targetPropertyReference)
    {
      DocumentNodePath documentNodePath = (DocumentNodePath) null;
      ReferenceStep targetProperty = targetPropertyReference[0];
      StyleNode styleNode = targetElement as StyleNode;
      if (targetPropertyReference.TargetType.IsAssignableFrom(targetElement.TargetType) || styleNode != null && targetPropertyReference.TargetType.IsAssignableFrom(styleNode.StyleTargetType))
      {
        IViewObjectFactory viewObjectFactory = targetElement.Platform.ViewObjectFactory;
        object computedValue = targetElement.GetComputedValue(targetPropertyReference);
        DocumentNodePath valuePath = (DocumentNodePath) null;
        if (computedValue != null)
        {
          IViewObject instance = viewObjectFactory.Instantiate(computedValue);
          valuePath = targetElement.ViewModel.DefaultView.GetCorrespondingNodePath(instance, true);
        }
        if (valuePath == null && !BaseFrameworkElement.StyleProperty.Equals((object) targetProperty))
        {
          BaseFrameworkElement frameworkElement = targetElement as BaseFrameworkElement;
          if (frameworkElement != null)
            frameworkElement.FindMissingImplicitStyle();
          if (computedValue != null)
          {
            IViewObject instance = viewObjectFactory.Instantiate(computedValue);
            DocumentNodePath correspondingNodePath = targetElement.ViewModel.DefaultView.GetCorrespondingNodePath(instance, true);
            if (correspondingNodePath != null)
              valuePath = ControlStylingOperations.FindTemplateWithinStyle(correspondingNodePath, targetElement, (IPropertyId) targetProperty, (IPropertyId) targetProperty);
          }
        }
        if (ControlStylingOperations.ShouldSetEditingContextToNodePath(valuePath, targetElement, targetProperty))
          documentNodePath = valuePath;
      }
      if (documentNodePath == null && styleNode != null && targetElement.IsSet(targetPropertyReference) == PropertyState.Set)
      {
        DocumentNodePath valueAsDocumentNode = targetElement.GetLocalValueAsDocumentNode(targetPropertyReference);
        if (ControlStylingOperations.ShouldSetEditingContextToNodePath(valueAsDocumentNode, targetElement, targetProperty))
          documentNodePath = valueAsDocumentNode;
      }
      return documentNodePath;
    }

    public static DocumentNodePath ResolveNodePathForTemplateWithinExistingStyle(SceneElement targetElement, PropertyReference targetPropertyReference)
    {
      DocumentNodePath documentNodePath = ControlStylingOperations.ProvideStyleOrTemplateNodePath(targetElement, targetPropertyReference);
      if (documentNodePath != null)
      {
        SceneView defaultView = targetElement.ViewModel.DefaultView;
        ICollection<IViewObject> instantiatedElements = defaultView.GetInstantiatedElements(documentNodePath);
        if (instantiatedElements.Count <= 0)
          return ControlStylingOperations.FindTemplateWithinStyle(documentNodePath, targetElement, ControlElement.TemplateProperty, (IPropertyId) targetPropertyReference[0]);
        foreach (IViewObject viewObject in (IEnumerable<IViewObject>) instantiatedElements)
        {
          if (PlatformTypes.Control.IsAssignableFrom((ITypeId) viewObject.GetIType((ITypeResolver) targetElement.ProjectContext)))
          {
            object platformObject = viewObject.GetValue(targetElement.ProjectContext.ResolveProperty(ControlElement.TemplateProperty));
            if (platformObject != null)
            {
              IViewObject instance = targetElement.ViewModel.ProjectContext.Platform.ViewObjectFactory.Instantiate(platformObject);
              DocumentNodePath correspondingNodePath = defaultView.GetCorrespondingNodePath(instance, true);
              if (correspondingNodePath != null && ControlStylingOperations.ShouldSetEditingContextToNodePath(correspondingNodePath, targetElement, targetPropertyReference[0]))
                return correspondingNodePath;
            }
          }
        }
      }
      return (DocumentNodePath) null;
    }

    private static DocumentNodePath FindTemplateWithinStyle(DocumentNodePath styleNodePath, SceneElement targetElement, IPropertyId targetSetterProperty, IPropertyId targetProperty)
    {
      DocumentCompositeNode documentCompositeNode1 = styleNodePath.Node as DocumentCompositeNode;
      if (documentCompositeNode1 != null && PlatformTypes.Style.IsAssignableFrom((ITypeId) documentCompositeNode1.Type))
      {
        DocumentCompositeNode documentCompositeNode2 = documentCompositeNode1.Properties[StyleNode.SettersProperty] as DocumentCompositeNode;
        if (documentCompositeNode2 != null && PlatformTypes.SetterBaseCollection.IsAssignableFrom((ITypeId) documentCompositeNode2.Type) && documentCompositeNode2.SupportsChildren)
        {
          foreach (DocumentNode documentNode in (IEnumerable<DocumentNode>) documentCompositeNode2.Children)
          {
            DocumentCompositeNode valueNode = documentNode as DocumentCompositeNode;
            if (valueNode != null && PlatformTypes.Setter.IsAssignableFrom((ITypeId) valueNode.Type))
            {
              IProperty property = DocumentNodeHelper.GetValueAsMember(valueNode, SetterSceneNode.PropertyProperty) as IProperty;
              if (targetSetterProperty.Equals((object) property))
              {
                DocumentNode expression = valueNode.Properties[SetterSceneNode.ValueProperty];
                if (expression != null)
                {
                  DocumentNodePath pathInContainer = styleNodePath.GetPathInContainer((DocumentNode) valueNode);
                  DocumentNode newContainer = new ExpressionEvaluator(targetElement.ViewModel.DocumentRootResolver).EvaluateExpression(pathInContainer, expression);
                  if (newContainer != null && newContainer.Parent != null)
                  {
                    DocumentNodePath pathInSubContainer = pathInContainer.GetPathInSubContainer(newContainer.SitePropertyKey, newContainer);
                    if (ControlStylingOperations.ShouldSetEditingContextToNodePath(pathInSubContainer, targetElement, (ReferenceStep) documentCompositeNode1.TypeResolver.ResolveProperty(targetProperty)))
                      return pathInSubContainer;
                  }
                }
              }
            }
          }
        }
      }
      return (DocumentNodePath) null;
    }

    public static bool CanEditInPlace(SceneElement targetElement, ReferenceStep targetProperty, DocumentNodePath template)
    {
      DocumentNodePath documentNodePath = targetElement.DocumentNodePath;
      SceneViewModel viewModel = targetElement.ViewModel.GetViewModel(template.Node.DocumentRoot, true);
      if (targetElement.IsViewObjectValid && (targetElement.IsInstantiatedElementVisible || PlatformTypes.Style.IsAssignableFrom((ITypeId) template.Node.Type) && (targetProperty.Equals((object) BaseFrameworkElement.StyleProperty) || targetProperty == StyleNode.BasedOnProperty)) && (viewModel == targetElement.ViewModel && documentNodePath.IsAncestorOf(template)))
      {
        if (viewModel.DefaultView.GetInstantiatedElements(template).Count > 0)
          return true;
        if (!viewModel.ProjectContext.IsCapabilitySet(PlatformCapability.IsWpf) && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) template.Node.Type))
        {
          SceneNode valueAsSceneNode = viewModel.GetSceneNode(template.Node).GetLocalValueAsSceneNode(viewModel.ProjectContext.PlatformMetadata.KnownProperties.FrameworkTemplateVisualTree);
          if (valueAsSceneNode != null)
          {
            IViewVisual viewVisual = targetElement.ViewTargetElement as IViewVisual;
            if (viewVisual == null && ProjectNeutralTypes.DataGridColumn.IsAssignableFrom((ITypeId) targetElement.Type))
            {
              DataGridElement dataGridElement = targetElement.Parent as DataGridElement;
              if (dataGridElement != null && targetElement.DocumentNode.IsChild && (targetElement.DocumentNode.Parent.IsProperty && DataGridElement.ColumnsProperty.Equals((object) targetElement.DocumentNode.Parent.SitePropertyKey)))
                viewVisual = dataGridElement.ViewTargetElement as IViewVisual;
            }
            if (viewVisual != null)
            {
              DocumentNodePath pathInContainer = template.GetPathInContainer(valueAsSceneNode.DocumentNode);
              foreach (IViewObject viewObject in (IEnumerable<IViewObject>) viewModel.DefaultView.GetInstantiatedElements(pathInContainer))
              {
                IViewVisual visual = viewObject as IViewVisual;
                if (visual != null && viewVisual.IsAncestorOf(visual))
                  return true;
              }
            }
          }
        }
      }
      return false;
    }

    public static bool DoesStyleTargetControl(IProjectContext projectContext, IType elementType, IPropertyId styleProperty)
    {
      IProperty property = projectContext.ResolveProperty(styleProperty);
      Type propertyTargetType = elementType.Metadata.GetStylePropertyTargetType((IPropertyId) property);
      Type runtimeType = projectContext.ResolveType(PlatformTypes.Control).RuntimeType;
      if (propertyTargetType != (Type) null)
        return runtimeType.IsAssignableFrom(propertyTargetType);
      return false;
    }

    public static bool ShouldSetEditingContextToNodePath(DocumentNodePath valuePath, SceneElement targetElement, ReferenceStep targetProperty)
    {
      if (valuePath != null)
      {
        DocumentNode node = valuePath.Node;
        if (node != null && (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) node.Type) || PlatformTypes.Style.IsAssignableFrom((ITypeId) node.Type) && node is DocumentCompositeNode && (!targetProperty.Equals((object) BaseFrameworkElement.StyleProperty) || !PlatformTypes.Style.IsAssignableFrom((ITypeId) targetElement.Type))) && !ControlStylingOperations.IsInsideDefaultStyleOrTemplate(valuePath))
        {
          DocumentNodePath containerOwnerPath = valuePath.GetContainerOwnerPath();
          if (!valuePath.IsValid())
            return false;
          if (containerOwnerPath != null)
            return !containerOwnerPath.Contains(valuePath.Node);
          return true;
        }
      }
      return false;
    }

    public static bool IsInsideDefaultStyleOrTemplate(DocumentNodePath styleOrTemplate)
    {
      DocumentNodePath documentNodePath = styleOrTemplate;
      while (documentNodePath != null)
      {
        if (StyleNode.IsDefaultValue(documentNodePath.Node))
          return true;
        documentNodePath = documentNodePath.GetContainerOwnerPath();
        if (documentNodePath != null)
          documentNodePath = documentNodePath.GetContainerNodePath();
      }
      return false;
    }

    public static bool DoesPropertyAffectRoot(IPropertyId propertyKey)
    {
      if (!BaseFrameworkElement.StyleProperty.Equals((object) propertyKey) && !ControlElement.TemplateProperty.Equals((object) propertyKey))
        return PageElement.TemplateProperty.Equals((object) propertyKey);
      return true;
    }

    public static SceneNode SetActiveEditingContainer(SceneElement targetElement, ReferenceStep targetProperty, DocumentNode node, DocumentNodePath knownPath, bool preferInPlaceEdit, SceneEditTransaction outerTransaction)
    {
      ControlStylingOperations.EditScope scope = new ControlStylingOperations.EditScope()
      {
        TargetElement = targetElement,
        TargetProperty = targetProperty,
        Node = node,
        NodePath = knownPath,
        EditInPlace = preferInPlaceEdit
      };
      SceneNode sceneNode = (SceneNode) null;
      ControlStylingOperations.EditScope templateScope;
      ControlStylingOperations.EditScope styleScope;
      if (ControlStylingOperations.ShouldNavigateToIntermediateStyle(scope, out templateScope, out styleScope))
      {
        templateScope.TargetElement = ControlStylingOperations.SetActiveEditingContainerInternal(styleScope) as SceneElement;
        if (templateScope.TargetElement != null)
        {
          templateScope.TargetElement.ViewModel.EditContextManager.ActiveEditContext.EnsureHidden();
          if (outerTransaction != null)
            outerTransaction.Update();
          if (ControlStylingOperations.UpdateNavigationInfo(templateScope))
            sceneNode = ControlStylingOperations.SetActiveEditingContainerInternal(templateScope);
        }
      }
      if (sceneNode == null)
        sceneNode = ControlStylingOperations.SetActiveEditingContainerInternal(scope);
      return sceneNode;
    }

    private static bool ShouldNavigateToIntermediateStyle(ControlStylingOperations.EditScope scope, out ControlStylingOperations.EditScope templateScope, out ControlStylingOperations.EditScope styleScope)
    {
      styleScope = (ControlStylingOperations.EditScope) null;
      templateScope = (ControlStylingOperations.EditScope) null;
      if (scope.EditInPlace || scope.Node == null || (scope.NodePath == null || PlatformTypes.Style.IsAssignableFrom((ITypeId) scope.TargetElement.Type)) || !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) scope.Node.Type))
        return false;
      DocumentCompositeNode valueNode = scope.NodePath.ContainerOwner as DocumentCompositeNode;
      if (valueNode == null || !PlatformTypes.Setter.IsAssignableFrom((ITypeId) valueNode.Type) || (valueNode.SitePropertyKey != null || valueNode.SiteChildIndex < 0))
        return false;
      DocumentCompositeNode parent1 = valueNode.Parent;
      if (parent1 == null || !PlatformTypes.SetterBaseCollection.IsAssignableFrom((ITypeId) parent1.Type) || (parent1.SitePropertyKey == null || !StyleNode.SettersProperty.Equals((object) parent1.SitePropertyKey)))
        return false;
      DocumentCompositeNode parent2 = parent1.Parent;
      if (parent2 == null || !PlatformTypes.Style.IsAssignableFrom((ITypeId) parent2.Type))
        return false;
      ReferenceStep referenceStep1 = DocumentNodeHelper.GetValueAsMember(valueNode, SetterSceneNode.PropertyProperty) as ReferenceStep;
      if (referenceStep1 == null || !PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) referenceStep1.PropertyType))
        return false;
      DocumentNodePath containerOwnerPath = scope.NodePath.GetContainerOwnerPath();
      if (containerOwnerPath == null || containerOwnerPath.ContainerOwner == null)
        return false;
      ReferenceStep referenceStep2 = (ReferenceStep) null;
      for (; containerOwnerPath != null; containerOwnerPath = containerOwnerPath.GetContainerOwnerPath())
      {
        if (PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) containerOwnerPath.ContainerOwner.Type))
        {
          referenceStep2 = containerOwnerPath.Node.TypeResolver.ResolveProperty(BaseFrameworkElement.StyleProperty) as ReferenceStep;
          break;
        }
        referenceStep2 = !PlatformTypes.Setter.IsAssignableFrom((ITypeId) containerOwnerPath.ContainerOwner.Type) ? containerOwnerPath.ContainerOwnerProperty as ReferenceStep : DocumentNodeHelper.GetValueAsMember((DocumentCompositeNode) containerOwnerPath.ContainerOwner, SetterSceneNode.PropertyProperty) as ReferenceStep;
        if (StyleNode.BasedOnProperty.Equals((object) referenceStep2))
          referenceStep2 = (ReferenceStep) null;
        else
          break;
      }
      if (referenceStep2 == null || !PlatformTypes.Style.IsAssignableFrom((ITypeId) referenceStep2.PropertyType))
        return false;
      styleScope = new ControlStylingOperations.EditScope()
      {
        TargetElement = scope.TargetElement,
        TargetProperty = referenceStep2
      };
      if (!ControlStylingOperations.UpdateNavigationInfo(styleScope))
        return false;
      templateScope = new ControlStylingOperations.EditScope()
      {
        TargetProperty = referenceStep1
      };
      return true;
    }

    private static bool UpdateNavigationInfo(ControlStylingOperations.EditScope scope)
    {
      if (scope.NodePath == null)
      {
        PropertyReference targetPropertyReference = new PropertyReference(scope.TargetProperty);
        scope.NodePath = ControlStylingOperations.ProvideStyleOrTemplateNodePath(scope.TargetElement, targetPropertyReference);
      }
      if (scope.NodePath != null)
      {
        scope.Node = scope.NodePath.Node;
        scope.EditInPlace = ControlStylingOperations.CanEditInPlace(scope.TargetElement, scope.TargetProperty, scope.NodePath);
      }
      return scope.NodePath != null;
    }

    private static SceneNode SetActiveEditingContainerInternal(ControlStylingOperations.EditScope scope)
    {
      SceneViewModel viewModel = scope.TargetElement.ViewModel.GetViewModel(scope.Node.DocumentRoot, true);
      if (viewModel == null)
        return (SceneNode) null;
      SceneElement selectionToSet = (SceneElement) viewModel.GetSceneNode(scope.Node);
      if (selectionToSet.IsLocked)
      {
        using (SceneEditTransaction editTransaction = viewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitEditStyleTemplate, new object[1]
        {
          (object) scope.TargetProperty.Name
        })))
        {
          selectionToSet.IsLocked = false;
          editTransaction.Commit();
        }
      }
      if (viewModel == scope.TargetElement.ViewModel && scope.EditInPlace)
      {
        if (scope.NodePath != null)
          viewModel.ActiveEditingContainerPath = scope.NodePath;
      }
      else
      {
        IPropertyId ancestorPropertyKey = (IPropertyId) scope.TargetProperty;
        if (scope.NodePath != null)
        {
          DocumentNodePath documentNodePath = scope.NodePath;
          ancestorPropertyKey = (IPropertyId) documentNodePath.ContainerOwnerProperty;
          while ((documentNodePath = documentNodePath.GetContainerOwnerPath()) != null && documentNodePath.Node != scope.TargetElement.DocumentNode)
            ancestorPropertyKey = (IPropertyId) documentNodePath.ContainerOwnerProperty;
        }
        BaseFrameworkElement frameworkElement = scope.TargetElement as BaseFrameworkElement;
        Size preferredSize = frameworkElement == null || !frameworkElement.IsViewObjectValid || !ControlStylingOperations.DoesPropertyAffectRoot((IPropertyId) scope.TargetProperty) ? Size.Empty : frameworkElement.GetComputedBounds((Base2DElement) frameworkElement).Size;
        viewModel.SetViewRoot(scope.TargetElement.ViewModel.DefaultView, scope.TargetElement, ancestorPropertyKey, scope.Node, preferredSize);
        viewModel.DefaultView.EnsureDesignSurfaceVisible();
      }
      if (viewModel.IsEditable)
        viewModel.ElementSelectionSet.SetSelection(selectionToSet);
      return viewModel.ActiveEditingContainer;
    }

    private class EditScope
    {
      public SceneElement TargetElement { get; set; }

      public ReferenceStep TargetProperty { get; set; }

      public DocumentNode Node { get; set; }

      public DocumentNodePath NodePath { get; set; }

      public bool EditInPlace { get; set; }
    }
  }
}
